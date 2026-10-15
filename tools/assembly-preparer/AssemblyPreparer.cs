// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.CompilerServices.SymbolWriter;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;
using MonoTouch.Tuner;
using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;
using Xamarin.Tuner;
using Xamarin.Utils;

namespace Xamarin.Build;

public class AssemblyPreparer : IDisposable {
	AggregateLog log = new AggregateLog ();
	readonly IToolLog toolLog;

	LinkerConfiguration configuration;

	public LinkerConfiguration Configuration => configuration;

	public string MakeReproPath { get; set; } = "";

	// The original assemblies from before preparation and trimming. Used during post-processing by
	// static registrars to read metadata removed during trimming.
	// This is a complete metadata universe separate from the post-trim assemblies.
	public List<string> PreTrimAssemblies { get; } = new List<string> ();

	public bool? TrimExportAttributes {
		get => configuration.Application.TrimExportAttributes;
		set => configuration.Application.TrimExportAttributes = value;
	}

	public RegistrarMode Registrar {
		get => configuration.Application.Registrar;
		set => configuration.Application.Registrar = value;
	}

	public string IntermediateOutputPath {
		get => configuration.IntermediateOutputPath;
	}

	public Optimizations Optimizations => configuration.Application.Optimizations;

	public List<AssemblyPreparerInfo> Assemblies { get; set; } = new List<AssemblyPreparerInfo> ();

	// The list of steps that were executed, along with how long each step took and whether it modified any assemblies.
	public List<StepExecution> StepExecutions { get; } = new List<StepExecution> ();

	// Set to true (via the AppBundleRewriter.AssemblySaved callback) whenever the currently executing step modifies an assembly.
	bool currentStepModifiedAssemblies;

	public IList<(string Path, AssemblyDefinition Assembly, string? OriginatingAssembly)> AddedAssemblies => configuration.AddedAssemblies;

	LinkerConfiguration.Configurator GetConfigurator (string? reproPath = null, Func<string, string, AssemblyPreparerInfo>? assemblyPreparerInfoFactory = null)
	{
		var dict = new LinkerConfiguration.Configurator () {
			{ "AssemblyPreparer", (
				new LinkerConfiguration.LoadValue ((key, value) => {
					var split = value.Split ('|');
					var input = split[0];
					var output = split[1];
					var isTrimmableString = split[2];
					var isTrimmable = string.IsNullOrEmpty (isTrimmableString) ? (bool?) null : string.Equals (isTrimmableString, "true", StringComparison.OrdinalIgnoreCase);
					var trimMode = split[3];
					var originalInput = split.Length > 4 ? split[4] : null;
					var apinfo = assemblyPreparerInfoFactory is not null ? assemblyPreparerInfoFactory (input, output) : new AssemblyPreparerInfo (input, output, originalInput, isTrimmable, trimMode);
					Assemblies.Add (apinfo);
				}),
				new LinkerConfiguration.SaveValue ((key, storage) => SaveAssemblies (key, storage, reproPath, Assemblies))
			)},
		};
		return dict;
	}

	void SaveAssemblies (string key, List<string> storage, string? reproPath, IList<AssemblyPreparerInfo> assemblies)
	{
		foreach (var assembly in assemblies) {
			var input = assembly.InputPath;
			var output = assembly.OutputPath;
			var originalInput = assembly.OriginalInputPath;
			if (!string.IsNullOrEmpty (reproPath)) {
				output = Path.Combine (reproPath, Path.GetFileName (output));
				File.Copy (input, output);
				if (!StringUtils.IsNullOrEmpty (originalInput) && CopyBindingResourcePackage (originalInput, output))
					originalInput = output;
			}
			storage.Add ($"{key}={input}|{output}|{(assembly.IsTrimmable.HasValue ? (assembly.IsTrimmable.Value ? "true" : "false") : "")}|{assembly.TrimMode}|{originalInput}");
		}
	}

	bool CopyBindingResourcePackage (string originalAssemblyPath, string destinationAssemblyPath)
	{
		var copied = false;
		var sourceDirectory = Path.ChangeExtension (originalAssemblyPath, ".resources");
		if (Directory.Exists (sourceDirectory)) {
			var destinationDirectory = Path.ChangeExtension (destinationAssemblyPath, ".resources");
			var destinationParentDirectory = Path.GetDirectoryName (destinationDirectory);
			if (destinationParentDirectory is null)
				throw new InvalidOperationException ($"Could not get the directory name for '{destinationDirectory}'.");
			FileCopier.UpdateDirectory (toolLog, sourceDirectory, destinationParentDirectory);
			copied = true;
		}

		var sourceZip = sourceDirectory + ".zip";
		if (File.Exists (sourceZip)) {
			File.Copy (sourceZip, Path.ChangeExtension (destinationAssemblyPath, ".resources.zip"), true);
			copied = true;
		}
		return copied;
	}

	public AssemblyPreparer (IToolLog log, AssemblyPreparerInfo [] assemblies, string linker_file)
	{
		toolLog = log;
		var lines = File.ReadAllLines (linker_file).ToList ();
		SaveAssemblies ("AssemblyPreparer", lines, null, assemblies);
		configuration = new LinkerConfiguration (log, lines, linker_file, GetConfigurator (null, assemblies.Length == 0 ? null : (input, output) => assemblies.Single (a => a.InputPath == input && a.OutputPath == output))) {
			AssemblyInfos = Assemblies,
		};
	}

	public void AddLog (IAssemblyPreparerLog log)
	{
		if (log is null)
			throw new ArgumentNullException (nameof (log));
		this.log.Add (log);
	}

	bool SaveToReproPath (List<ProductException> exceptions)
	{
		if (File.Exists (MakeReproPath) || Directory.Exists (MakeReproPath)) {
			exceptions.Add (ErrorHelper.CreateError (99, $"Repro location already exists: {MakeReproPath}"));
			return false;
		}
		Directory.CreateDirectory (MakeReproPath);
		var lines = new List<string> ();
		configuration.Save (lines, GetConfigurator (MakeReproPath));
		File.WriteAllLines (Path.Combine (MakeReproPath, "arguments.txt"), lines);
		log.Log ($"Created repro in {MakeReproPath}");

		return true;
	}

	public static AssemblyPreparer LoadFromReproPath (string reproPath)
	{
		var file = Path.Combine (reproPath, "arguments.txt");
		if (!File.Exists (file))
			throw new FileNotFoundException ($"Repro arguments file not found: {file}");
		return new AssemblyPreparer (ConsoleLog.Instance, [], file);
	}

	public bool Prepare (out List<ProductException> exceptions)
	{
		// All the same steps as the custom trimmer steps that are run before MarkStep in Xamarin.Shared.Sdk.targets (and in the same order).
		var steps = new List<ConfigurationAwareStep> {
			// CollectAssembliesStep
			new LoadAssembliesStep (),
		};
		steps.Add (new ComputeMethodOverridesStep ());
		steps.Add (new CoreTypeMapStep ());
		steps.Add (new CollectFieldsStep ()); // ProcessExportedFields

		// These steps only do anything for assemblies that are being trimmed (their IsActiveFor requires
		// AssemblyAction.Link), so don't even add them to the list when nothing's being trimmed.
		if (configuration.Application.AreAnyAssembliesTrimmed) {
			steps.Add (new PreserveProtocolsStep ());
			steps.Add (new PreserveSmartEnumConversionsStep ());
			steps.Add (new PreserveBlockCodeStep ());
			steps.Add (new OptimizeGeneratedCodeStep ());
			steps.Add (new ApplyPreserveAttributeStep ());
			steps.Add (new MarkForStaticRegistrarStep ());
			steps.Add (new MarkNSObjectsStep ());
		}

		steps.Add (new InlineDlfcnMethodsStep ());

		// Only add DetectApiUsageStep if it's needed:
		// * If the user explicitly set $(DynamicRegistrationSupported), we don't need to compute the value, but
		//   Export attribute removal still needs the step to detect NSXpcInterface reflection.
		// * If nothing is being trimmed, the dynamic registrar (which lives in the platform assembly, an SDK
		//   assembly that's only trimmed when trimming is enabled) can't be removed, so there's nothing to compute.
		if (configuration.Application.AreAnyAssembliesTrimmed && (!configuration.DynamicRegistrationSupported.HasValue || configuration.Application.TrimExportAttributes != false))
			steps.Add (new DetectApiUsageStep ());

		// PreMarkDispatcher: I don't think we need this one
		steps.Add (new ManagedRegistrarStep ());
		steps.Add (new ComputeExportAttributeRemovalStep ());
		steps.Add (new TrimmableRegistrarStep ());
		steps.Add (new ManagedRegistrarLookupTablesStep ());
		steps.Add (new InlineClassGetHandleStep ());
		steps.Add (new SaveAssembliesStep ());

		return RunSteps (steps, out exceptions);
	}

	// Load the original assemblies so the static registrars can find protocol methods removed
	// during trimming (and the trimmable static registrar can read selected removed attributes).
	// The assemblies are loaded into their own resolver (a separate, self-contained
	// metadata universe from the post-trim assemblies), and stored on the Application for the registrar to
	// consult. There's no fallback to the post-trim resolver: the original set is complete, and falling back
	// would mix the two universes and resolve trimmed-away references incorrectly.
	void LoadPreTrimAssemblies ()
	{
		if (PreTrimAssemblies.Count == 0)
			return;

		if (configuration.Application.Registrar != RegistrarMode.Static && configuration.Application.Registrar != RegistrarMode.TrimmableStatic && configuration.Application.Registrar != RegistrarMode.ManagedStatic)
			return;

		var resolver = new PreTrimAssemblyResolver (configuration.Logger, PreTrimAssemblies);
		configuration.Application.PreTrimAssemblyResolver = resolver;
	}

	public bool PostProcess (out List<ProductException> exceptions)
	{
		configuration.Application.IsPostProcessingAssemblies = true;

		LoadPreTrimAssemblies ();

		// For NativeAOT, postprocessing runs after the NativeAOT compiler (ILC) has already compiled the
		// assemblies, which means that modifying an assembly at this point is pointless (the modification
		// would be silently lost, and we'd show an MT0099 warning about it). So skip the step that removes
		// attributes in that case; the static registrar will find the attributes on the assemblies instead
		// (since they're not removed).
		var isPostILC = configuration.Application.XamarinRuntime == XamarinRuntime.NativeAOT;
		ConfigurationAwareStep [] removeAttributesStep = isPostILC ? [] : [new RemoveAttributesStep ()];

		ConfigurationAwareStep [] steps = [
			// All the same steps as the custom trimmer steps that are run after sweeping in Xamarin.Shared.Sdk.targets (and in the same order).
			new LoadAssembliesStep (), // LoadNonSkippedAssembliesStep

			// Populate Application.Assemblies with the loaded assemblies. This must happen before
			// ExtractBindingLibrariesStep (which iterates over Application.Assemblies to find the native
			// libraries and frameworks embedded in binding assemblies), just like in the ILLink flow,
			// where LoadNonSkippedAssembliesStep runs before ExtractBindingLibrariesStep.
			new PopulateApplicationAssembliesStep (),

			// post-sweep
			.. removeAttributesStep, // from PostSweepDispatcher.
			new CollectFieldsStep (), // Must run before ListExportedSymbols to populate ExportedFields annotation
			new ExtractBindingLibrariesStep (),
			// The ListExportedSymbols must run after ExtractBindingLibrariesStep, otherwise we won't properly list exported Objective-C classes from binding libraries
			new ListExportedSymbols (),
			new RemoveUserResourcesSubStep (), // from PreOutputDispatcher.

			// ManagedRegistrarStep/TrimmableRegistrarStep/ManagedRegistrarLookupTablesStep
			// must run before SaveAssembliesStep because they modify assemblies (adding
			// lookup tables and ldtoken instructions). SaveAssembliesStep writes the final
			// versions to disk with correct metadata tokens.
			new ManagedRegistrarStep (),
			new TrimmableRegistrarStep (),
			new ManagedRegistrarLookupTablesStep (),

			// Must run after the trimmer (so that we know which assemblies the trimmer removed),
			// and before SaveAssembliesStep (so that the modification is written to disk).
			new RemoveStaleTypeMapAssemblyTargetsStep (),

			new ClassHandleRewriterStep (),
			new SaveAssembliesStep (),

			// post-output

			new RegistrarStep (),

			new GenerateMainStep (),
			new GenerateReferencesStep (),
			new GatherFrameworksStep (),
			new ComputeNativeBuildFlagsStep (),
			new ComputeAOTArguments (),

			// Must be the last step.
			new DoneStep (),
		];

		var rv = RunSteps (steps, out exceptions);

		// If postprocessing runs after ILC has already compiled the assemblies, then no step should
		// modify an assembly (the change would be silently lost). Report a warning if we detect this.
		if (configuration.Application.XamarinRuntime == XamarinRuntime.NativeAOT && configuration.ModifiedAssemblies.Any ()) {
			foreach (var name in configuration.ModifiedAssemblies.Select (v => v.Name.Name).OrderBy (v => v))
				exceptions.Add (ErrorHelper.CreateWarning (99, $"The assembly '{name}' was modified during post-ILC postprocessing, but this is useless because the NativeAOT compiler (ILC) has already compiled it."));
		}

		return rv;
	}

	bool RunSteps (IList<ConfigurationAwareStep> steps, out List<ProductException> exceptions)
	{
		exceptions = configuration.Exceptions;

		if (Registrar == RegistrarMode.Default) {
			exceptions.Add (ErrorHelper.CreateError (99, "RegistrarMode must be explicitly set."));
			return false;
		}
		if (Registrar != RegistrarMode.TrimmableStatic) {
			if (TrimExportAttributes == true) {
				exceptions.Add (ErrorHelper.CreateError (99, "Export attributes can only be trimmed with the trimmable static registrar."));
				return false;
			}
			TrimExportAttributes = false;
		}

		if (!string.IsNullOrEmpty (MakeReproPath) && !SaveToReproPath (exceptions))
			return false;

		var linkContext = configuration.DerivedLinkContext;

		// We detect whether a step modified any assemblies by subscribing to the AppBundleRewriter's
		// AssemblySaved callback, which is called whenever an assembly is modified. All the steps that
		// modify assemblies go through the AppBundleRewriter to do so.
		Action<AssemblyDefinition>? assemblySavedHandler = null;
		try {
			foreach (var step in steps) {
				// Subscribe once the assemblies have been loaded: accessing the AppBundleRewriter before
				// that point would create it without finding the corlib and platform assemblies.
				if (assemblySavedHandler is null && configuration.Assemblies.Count > 0) {
					assemblySavedHandler = (asm) => {
						currentStepModifiedAssemblies = true;
						configuration.ModifiedAssemblies.Add (asm);
					};
					configuration.AppBundleRewriter.AssemblySaved += assemblySavedHandler;
				}

				currentStepModifiedAssemblies = false;
				var watch = Stopwatch.StartNew ();
				step.Process (linkContext);
				watch.Stop ();
				StepExecutions.Add (new StepExecution (step.GetType ().Name, watch.Elapsed, currentStepModifiedAssemblies));
			}
		} finally {
			if (assemblySavedHandler is not null)
				configuration.AppBundleRewriter.AssemblySaved -= assemblySavedHandler;
		}

		// The post-processing pass flushes its MSBuild output as its last step (DoneStep). The preparation
		// pass has no DoneStep, so flush here so that its MSBuild output properties are written.
		if (!configuration.Application.IsPostProcessingAssemblies)
			configuration.FlushOutputForMSBuild ();

		return exceptions.Count == 0;
	}

	public void Dispose ()
	{
		foreach (var assembly in Assemblies)
			assembly.Assembly?.Dispose ();
		configuration.Application.PreTrimAssemblyResolver?.Dispose ();
		configuration.Application.PreTrimAssemblyResolver = null;
		configuration.AssemblyResolver.ResolverCache.Clear ();
		configuration.DerivedLinkContext.Assemblies.Clear ();
	}
}

// The result of executing a single step: its name, how long it took, and whether it modified any assemblies.
public record struct StepExecution (string Name, TimeSpan Duration, bool ModifiedAssemblies);

public class AssemblyPreparerInfo {
	internal AssemblyDefinition? Assembly { get; set; }
	public bool IsCILAssembly { get; internal set; }

	public string InputPath { get; private set; }
	public string? OriginalInputPath { get; private set; }
	public bool? IsTrimmable { get; set; }
	public string TrimMode { get; set; }
	public string OutputPath { get; set; }

	public AssemblyPreparerInfo (string inputPath, string outputPath, string? originalInputPath, bool? isTrimmable, string trimMode)
	{
		InputPath = inputPath;
		OutputPath = outputPath;
		OriginalInputPath = originalInputPath;
		IsTrimmable = isTrimmable;
		TrimMode = trimMode;
	}

	public AssemblyPreparerInfo (string inputPath, string outputPath, bool? isTrimmable, string trimMode)
	{
		InputPath = inputPath;
		OutputPath = outputPath;
		IsTrimmable = isTrimmable;
		TrimMode = trimMode;
	}
}

// A resolver for the pre-trim (untrimmed) assemblies. It's given the complete set of assemblies that
// were fed into the trimmer (ILLink's input), so it forms a self-contained metadata universe: it loads
// assemblies lazily from that set on demand and never falls back to the post-trim resolver (which would
// mix the two universes and resolve trimmed-away references incorrectly).
class PreTrimAssemblyResolver : Xamarin.Bundler.CoreResolver {
	readonly IToolLog log;
	readonly Dictionary<string, string> paths = new Dictionary<string, string> ();

	public PreTrimAssemblyResolver (IToolLog log, IEnumerable<string> assemblyPaths)
	{
		this.log = log;
		foreach (var path in assemblyPaths)
			paths [Path.GetFileNameWithoutExtension (path)] = path;
	}

	public override AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
	{
		if (cache.TryGetValue (name.Name, out var assembly))
			return assembly;
		if (paths.TryGetValue (name.Name, out var path)) {
			var loaded = Load (log, path);
			if (loaded is not null)
				return loaded;
		}
		throw new AssemblyResolutionException (name);
	}
}
