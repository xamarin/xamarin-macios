using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Build;
using Xamarin.Bundler;
using Xamarin.Utils;
using Xamarin.Tuner;

#nullable enable

namespace Xamarin.Linker {
	public class LinkerConfiguration {
		string LinkerFile;

		public Abi Abi = Abi.None;
		public string AOTCompiler = string.Empty;
		public string AOTOutputDirectory = string.Empty;
		public string AssemblyPublishDir = string.Empty;
		public string DedupAssembly = string.Empty;
		public string CacheDirectory { get; private set; } = string.Empty;
		public Version? DeploymentTarget { get; private set; }
		// The user-provided value of the $(DynamicRegistrationSupported) MSBuild property (null if not set).
		// When set, DetectApiUsageStep doesn't need to compute this value in the assembly-preparer,
		// although it may still run to detect blockers for other optimizations.
		// This is also how the value DetectApiUsageStep computed during the preparation pass is
		// passed to the post-processing pass (which needs it to generate the native main file).
		public bool? DynamicRegistrationSupported { get; set; }
		public HashSet<string> FrameworkAssemblies { get; private set; } = new HashSet<string> ();
		public string IntermediateLinkDir { get; private set; } = string.Empty;
		public bool InvariantGlobalization { get; private set; }
		public bool HybridGlobalization { get; private set; }
		// The value of the $(HotReloadCompatibleBuild) MSBuild property. When enabled, steps that
		// re-serialize user assemblies (breaking Hot Reload) must leave reloadable assemblies untouched.
		public bool HotReloadCompatibleBuild { get; private set; }
		// The value of the $(_HotReloadModifiedAssemblySafetyNet) MSBuild property. This is an
		// undocumented escape hatch to opt out of the error we show when a reloadable assembly is
		// modified during a Hot Reload compatible build, in case the safety net itself has a bug.
		public bool HotReloadModifiedAssemblySafetyNet { get; private set; } = true;
		public InlineDlfcnMethodsMode InlineDlfcnMethods { get; set; }
		public bool InlineDlfcnMethodsEnabled => InlineDlfcnMethods != InlineDlfcnMethodsMode.Disabled;
		public InlineClassGetHandleMode InlineClassGetHandle { get; set; }
		// Per-assembly field symbols collected by InlineDlfcnMethodsStep, keyed by assembly name.
		public Dictionary<string, HashSet<string>> InlinedDlfcnFields { get; } = new Dictionary<string, HashSet<string>> ();
		// All [Field] symbol names collected by ProcessExportedFields, used in compatibility mode.
		public HashSet<string> FieldSymbols { get; } = new HashSet<string> ();
		public string IntermediateOutputPath { get; private set; } = string.Empty;
		public string ItemsDirectory { get; private set; } = string.Empty;
		// The files the assembly-preparer writes its MSBuild output properties to (one per pass).
		public string MSBuildOutputFile { get; private set; } = string.Empty;
		public string MSBuildPostProcessOutputFile { get; private set; } = string.Empty;
		public bool IsSimulatorBuild { get; private set; }
		public string PartialStaticRegistrarLibrary { get; set; } = string.Empty;
		public ApplePlatform Platform { get; private set; }
		public string PlatformAssembly { get; private set; } = string.Empty;
		public bool PublishTrimmed { get; private set; }
		public string RelativeAppBundlePath { get; private set; } = string.Empty;
		public string RuntimeConfigurationFilePath { get; private set; } = string.Empty;
		public Version? SdkVersion { get; private set; }
		public string SdkRootDirectory { get; private set; } = string.Empty;
		public string TypeMapFilePath { get; set; } = string.Empty;
		public string TrimMode { get; private set; } = string.Empty;
		public string UnmanagedCallersOnlyMapPath { get; private set; } = string.Empty;
		public int Verbosity => Application.Verbosity;
		public string XamarinNativeLibraryDirectory { get; private set; } = string.Empty;
		public Version? XcodeVersion { get; private set; }

		static ConditionalWeakTable<LinkContext, LinkerConfiguration> configurations = new ConditionalWeakTable<LinkContext, LinkerConfiguration> ();

		public Application Application { get; private set; }

		public IToolLog Logger { get; private set; }

		public IList<string> RegistrationMethods { get; set; } = new List<string> ();
		public List<string> NativeCodeToCompileAndLink { get; private set; } = new List<string> ();
		public CompilerFlags CompilerFlags;

		List<ProductException> exceptions = new List<ProductException> ();
		public List<ProductException> Exceptions {
			get {
				return exceptions;
			}
		}
		public DotNetResolver AssemblyResolver { get; private set; }
		public IMetadataResolver MetadataResolver { get; private set; }

		public LinkContext Context { get => DerivedLinkContext; }
		public DerivedLinkContext DerivedLinkContext { get => Application.LinkContext; }
		public Profile Profile { get; private set; }

		public List<AssemblyDefinition> Assemblies => Application.LinkContext.Assemblies;
		public required List<AssemblyPreparerInfo> AssemblyInfos;
		public List<(string Path, AssemblyDefinition Assembly, string? OriginatingAssembly)> AddedAssemblies = new ();
		// The set of assemblies that were modified (i.e. that AppBundleRewriter.SaveAssembly was called for).
		// Assemblies that aren't modified don't need to be re-serialized when saved.
		public HashSet<AssemblyDefinition> ModifiedAssemblies = new ();

		string? user_optimize_flags;

		Dictionary<string, List<MSBuildItem>> msbuild_items = new Dictionary<string, List<MSBuildItem>> ();

		// MSBuild output properties (property name -> value), written (alphabetically sorted) to the
		// MSBuild output file by FlushOutputForMSBuild.
		SortedDictionary<string, string> msbuild_properties = new SortedDictionary<string, string> (StringComparer.Ordinal);

		AppBundleRewriter? abr;
		internal AppBundleRewriter AppBundleRewriter {
			get {
				if (abr is null)
					abr = new AppBundleRewriter (this);
				return abr;
			}
		}

		public AssemblyDefinition EntryAssembly {
			get {
				var entryAssemblyName = Path.GetFileNameWithoutExtension (Application.AssemblyName);
				var entryAssembly = Assemblies.FirstOrDefault (a => a.Name.Name == entryAssemblyName);
				if (entryAssembly is null)
					throw new InvalidOperationException ($"The entry assembly '{entryAssemblyName}' was not found among the loaded assemblies.");

				return entryAssembly;
			}
		}

		// This dictionary contains information about the trampolines created for each assembly.
		public AssemblyTrampolineInfos AssemblyTrampolineInfos = new ();

		// The per-assembly companion TypeMap assemblies (_<Asm>.TypeMap.dll), keyed by the user
		// assembly they belong to. When HotReloadCompatibleBuild is enabled, ManagedRegistrarStep
		// creates these early (so it can emit the registrar trampolines into them instead of into
		// the user assembly) and TrimmableRegistrarStep reuses them.
		internal Dictionary<AssemblyDefinition, RegistrarCompanionAssembly> RegistrarCompanionAssemblies = new ();

		// ASSEMBLY_PREPARER TODO move pinvoke wrapper generation out of ListExportedFields step (and remove the #pragma warning)
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
		internal PInvokeWrapperGenerator? PInvokeWrapperGenerationState;
#pragma warning restore CS0649

		public static bool TryGetInstance (LinkContext context, [NotNullWhen (true)] out LinkerConfiguration? configuration)
		{
			return configurations.TryGetValue (context, out configuration);
		}
		public static LinkerConfiguration GetInstance (LinkContext context)
		{
			if (!TryGetInstance (context, out var instance)) {
				throw new InvalidOperationException ($"No LinkerConfiguration instance found for the given LinkContext.");
			}

			return instance;
		}

		public delegate void LoadValue (string key, string value);
		public delegate void SaveValue (string key, List<string> storage);

		delegate void LoadBool (string key, string value, out bool result);
		delegate void LoadNullableBool (string key, string value, out bool? result);

		public class Configurator : Dictionary<string, (LoadValue Load, SaveValue Save)> { }

		Configurator GetConfigurator (string linker_file)
		{
			var saveNonEmpty = new Action<string, string?, List<string>> ((key, value, storage) => {
				if (string.IsNullOrEmpty (value))
					return;
				storage.Add ($"{key}={value}");
			});
			var saveNullableBool = new Action<string, bool?, List<string>> ((key, value, storage) => {
				if (!value.HasValue)
					return;
				storage.Add ($"{key}={(value.Value ? "true" : "false")}");
			});
			var saveOptionalDefaultFalseBool = new Action<string, bool, List<string>> ((key, value, storage) => {
				if (!value)
					return;
				storage.Add ($"{key}=true");
			});
			var loadBool = new LoadBool ((string key, string value, out bool result) => {
				result = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase);
			});
			var loadNullableBool = new LoadNullableBool ((key, value, out result) => {
				if (!TryParseOptionalBoolean (value, out result))
					throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
			});

			var loadWarningLevel = new Action<string, string, ErrorHelper.WarningLevel> ((key, value, level) => {
				try {
					ErrorHelper.ParseWarningLevel (Application, level, value);
				} catch (Exception ex) {
					throw new InvalidOperationException ($"Invalid {key} '{value}' in {linker_file}", ex);
				}
			});
			var saveWarningLevel = new Action<string, List<string>, ErrorHelper.WarningLevel> ((key, storage, level) => {
				var warningLevels = ErrorHelper.GetWarningLevels (Application);
				if (warningLevels is null)
					return;
				foreach (var kvp in warningLevels.Where (v => v.Value == level).OrderBy (v => v.Key)) {
					if (kvp.Key == -1) {
						storage.Add (key);
					} else {
						storage.Add ($"{key}={kvp.Key}");
					}
				}
			});

			var dict = new Configurator () {
				{ "AreAnyAssembliesTrimmed", (
					new LoadValue ((key, value) => Application.AreAnyAssembliesTrimmed = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(Application.AreAnyAssembliesTrimmed ? "true" : "false")}"))
				)},
				{ "AssemblyName", (
					// This is the _AssemblyName MSBuild property for the main project (which is also the root/entry assembly)
					new LoadValue ((key, value) => Application.RootAssemblies.Add (value)),
					new SaveValue ((key, storage) => storage.AddRange (Application.RootAssemblies.Select (v => $"{key}={v}")))
				)},
				{ "AssemblyPublishDir", (
					// This is the AssemblyPublishDir MSBuild property for the main project
					new LoadValue ((key, value) => AssemblyPublishDir = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, AssemblyPublishDir, storage))
				)},
				{ "AOTArgument",  (
					new LoadValue ((key, value) =>
					{
						if (!string.IsNullOrEmpty (value))
							Application.AotArguments.Add (value);
					}),
					new SaveValue ((key, storage) =>
						storage.AddRange (Application.AotArguments.Where (v => !string.IsNullOrEmpty (v)).Select (v => $"{key}={v}")))
				)},
				{ "AOTCompiler", (
					new LoadValue ((key, value) => AOTCompiler = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, AOTCompiler, storage))
				)},
				{ "AOTOutputDirectory", (
					new LoadValue ((key, value) => AOTOutputDirectory = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, AOTOutputDirectory, storage))
				)},
				{ "AppBundleManifestPath", (
					new LoadValue ((key, value) => Application.InfoPListPath = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.InfoPListPath, storage))
				)},
				{ "CacheDirectory", (
					new LoadValue ((key, value) => CacheDirectory = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, CacheDirectory, storage))
				)},
				{ "CustomLinkFlags", (
					new LoadValue ((key, value) => Application.ParseCustomLinkFlags (value, "gcc_flags")),
					new SaveValue ((key, storage) => storage.AddRange (Application.CustomLinkFlags?.Select (v => $"{key}={v}") ?? []))
				)},
				{ "Debug", (
					new LoadValue ((key, value) => Application.EnableDebug = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(Application.EnableDebug ? "true" : "false")}"))
				)},
				{ "DedupAssembly", (
					new LoadValue ((key, value) => DedupAssembly = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, DedupAssembly, storage))
				)},
				{ "DeploymentTarget", (
					new LoadValue ((key, value) => {
						if (!Version.TryParse (value, out var deployment_target))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						DeploymentTarget = deployment_target;
					}),
					new SaveValue ((key, storage) => saveNonEmpty (key, DeploymentTarget?.ToString (), storage))
				)},
				{ "Dlsym", (
					new LoadValue ((key, value) => Application.ParseDlsymOptions (value)),
					new SaveValue ((key, storage) => {
						switch (Application.DlsymOptions) {
						case DlsymOptions.None:
							storage.Add ($"Dlsym=false");
							break;
						case DlsymOptions.All:
							storage.Add ($"Dlsym=true");
							break;
						case DlsymOptions.Custom:
							if (Application.DlsymAssemblies is not null)
								storage.Add ($"Dlsym={string.Join (",", Application.DlsymAssemblies.Select (v => (v.Item2 ? "+" : "-") + v.Item1 + ".dll"))}");
							break;
						case DlsymOptions.Default:
							// don't store default
							break;
						default:
							throw new InvalidOperationException ($"Unknown DlsymOptions value: {Application.DlsymOptions}");
						}
					})
				)},
				{ "DylibToConvertToFramework", (
					new LoadValue ((key, value) => Application.DylibsToConvertToFrameworks.Add (value)),
					new SaveValue ((key, storage) => storage.AddRange (Application.DylibsToConvertToFrameworks.OrderBy (v => v).Select (v => $"{key}={v}")))
				)},
				{ "DynamicRegistrationSupported", (
					// This is the user-overridable $(DynamicRegistrationSupported) MSBuild property. It maps to
					// the RemoveDynamicRegistrar optimization (inverted): if dynamic registration is supported,
					// then we're not removing the dynamic registrar. When set, DetectApiUsageStep doesn't
					// need to compute the value in the assembly-preparer (it's passed straight through to the trimmer
					// feature switch), and it won't recompute the value in the real linker either.
					new LoadValue ((key, value) => {
						if (string.IsNullOrEmpty (value))
							return; // Not set: DetectApiUsageStep will compute a default value.
						if (!TryParseOptionalBoolean (value, out var dynamicRegistrationSupported))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						if (dynamicRegistrationSupported.HasValue) {
							DynamicRegistrationSupported = dynamicRegistrationSupported.Value;
							Application.Optimizations.RemoveDynamicRegistrar = !dynamicRegistrationSupported.Value;
						}
					}),
					new SaveValue ((key, storage) => saveNullableBool (key, DynamicRegistrationSupported, storage))
				)},
				{ "EnableSGenConc", (
					new LoadValue ((key, value) => Application.EnableSGenConc = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(Application.EnableSGenConc ? "true" : "false")}"))
				)},
				{ "EnvironmentVariable", (
					// Format is either of:
					//     NAME=VALUE
					//     Overwrite=BOOL|NAME=VALUE
					new LoadValue ((key, value) => {
						var overwrite = true;
						var needle = "Overwrite=";
						if (value.StartsWith (needle, StringComparison.Ordinal)) {
							var pipe = value.IndexOf ('|', needle.Length);
							if (pipe > 0) {
								var overwriteString = value [needle.Length..pipe];
								if (!TryParseOptionalBoolean (overwriteString, out var parsedOverwrite))
									throw new InvalidOperationException ($"Unable to parse the 'Overwrite' value '{overwriteString}' for the environment variable entry '{value}' in {linker_file}");
								overwrite = parsedOverwrite.Value;
								value = value [(pipe + 1)..];
							}
						}
						var separators = new char [] { ':', '=' };
						var equals = value.IndexOfAny (separators);
						var name = value.Substring (0, equals);
						var val = value.Substring (equals + 1);
						Application.EnvironmentVariables.Add (name, new (val, overwrite));
					}),
					new SaveValue ((key, storage) => storage.AddRange (Application.EnvironmentVariables.Select (v => $"{key}=Overwrite={v.Value.Overwrite}|{v.Key}={v.Value.Value}").OrderBy (v => v)))
				)},
				{ "FrameworkAssembly", (
					new LoadValue ((key, value) => FrameworkAssemblies.Add (value)),
					new SaveValue ((key, storage) => storage.AddRange (FrameworkAssemblies.OrderBy (v => v).Select (v => $"{key}={v}")))
				)},
				{ "GenerateTrustedPlatformAssemblies", (
					new LoadValue ((key, value) => loadBool (key, value, out Application.GenerateTrustedPlatformAssemblies)),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, Application.GenerateTrustedPlatformAssemblies, storage))
				)},
				{ "HotReloadCompatibleBuild", (
					new LoadValue ((key, value) => HotReloadCompatibleBuild = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, HotReloadCompatibleBuild, storage))
				)},
				{ "HotReloadModifiedAssemblySafetyNet", (
					new LoadValue ((key, value) => HotReloadModifiedAssemblySafetyNet = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(HotReloadModifiedAssemblySafetyNet ? "true" : "false")}"))
				)},
				{ "InlineDlfcnMethods", (
					new LoadValue ((key, value) => {
						if (Enum.TryParse<InlineDlfcnMethodsMode> (value, true, out var inlineDlfcnMode))
							InlineDlfcnMethods = inlineDlfcnMode;
						else if (string.IsNullOrEmpty (value))
							InlineDlfcnMethods = InlineDlfcnMethodsMode.Disabled;
						else
							throw new InvalidOperationException ($"Unknown InlineDlfcnMethods value: {value}");
					}),
					new SaveValue ((key, storage) => storage.Add ($"{key}={InlineDlfcnMethods}"))
				)},
				{ "InlineClassGetHandle", (
					new LoadValue ((key, value) => {
						if (Enum.TryParse<InlineClassGetHandleMode> (value, true, out var inlineClassGetHandleMode))
							InlineClassGetHandle = inlineClassGetHandleMode;
						else if (string.IsNullOrEmpty (value))
							InlineClassGetHandle = InlineClassGetHandleMode.Disabled;
						else
							throw new InvalidOperationException ($"Unknown InlineClassGetHandle value: {value}");
					}),
					new SaveValue ((key, storage) => storage.Add ($"{key}={InlineClassGetHandle}"))
				)},
				{ "Interpreter", (
					new LoadValue ((key, value) => {
						if (!string.IsNullOrEmpty (value))
							Application.ParseInterpreter (value);
					}),
					new SaveValue ((key, storage) => {
						if (!Application.UseInterpreter)
							return;
						storage.Add ($"{key}={string.Join (",", Application.InterpretedAssemblies)}");
					})
				)},
				{ "IntermediateLinkDir", (
					new LoadValue ((key, value) => IntermediateLinkDir = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, IntermediateLinkDir, storage))
				)},
				{ "IntermediateOutputPath", (
					new LoadValue ((key, value) => IntermediateOutputPath = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, IntermediateOutputPath, storage))
				)},
				{ "IsAppExtension", (
					new LoadValue ((key, value) => Application.IsExtension = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(Application.IsExtension ? "true" : "false")}"))
				)},
				{ "IsMultiRidBuild", (
					new LoadValue ((key, value) => loadBool (key, value, out Application.IsMultiRidBuild)),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, Application.IsMultiRidBuild, storage))
				)},
				{ "ItemsDirectory", (
					new LoadValue ((key, value) => ItemsDirectory = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, ItemsDirectory, storage))
				)},
				{ "IsSimulatorBuild", (
					new LoadValue ((key, value) => IsSimulatorBuild = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(IsSimulatorBuild ? "true" : "false")}"))
				)},
				{ "LibMonoLinkMode", (
					new LoadValue ((key, value) => Application.LibMonoLinkMode = ParseLinkMode (value, key)),
					new SaveValue ((key, storage) => { if (Application.HasLibMonoLinkMode) storage.Add ($"{key}={Application.LibMonoLinkMode}"); })
				)},
				{ "LibXamarinLinkMode", (
					new LoadValue ((key, value) => Application.LibXamarinLinkMode = ParseLinkMode (value, key)),
					new SaveValue ((key, storage) => { if (Application.HasLibXamarinLinkMode) storage.Add ($"{key}={Application.LibXamarinLinkMode}"); })
				)},
				{ "MarshalManagedExceptionMode", (
					new LoadValue ((key, value) => {
						if (!string.IsNullOrEmpty (value)) {
							if (!Application.TryParseManagedExceptionMode (value, out var mode))
								throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
							Application.MarshalManagedExceptions = mode;
						}
					}),
					new SaveValue ((key, storage) => storage.Add ($"{key}={Application.MarshalManagedExceptions.ToString ().ToLowerInvariant ()}"))
				)},
				{ "MarshalObjectiveCExceptionMode", (
					new LoadValue ((key, value) => {
						if (!string.IsNullOrEmpty (value)) {
							if (!Application.TryParseObjectiveCExceptionMode (value, out var mode))
								throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
							Application.MarshalObjectiveCExceptions = mode;
						}
					}),
					new SaveValue ((key, storage) => storage.Add ($"{key}={Application.MarshalObjectiveCExceptions.ToString ().ToLowerInvariant ()}"))
				)},
				{ "MonoLibrary", (
					new LoadValue ((key, value) => Application.MonoLibraries.Add (value)),
					new SaveValue ((key, storage) => storage.AddRange (Application.MonoLibraries.OrderBy (v => v).Select (v => $"{key}={v}")))
				)},
				{ "MSBuildOutputFile", (
					// The file the assembly-preparer's preparation pass writes its MSBuild output properties to.
					new LoadValue ((key, value) => MSBuildOutputFile = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, MSBuildOutputFile, storage))
				)},
				{ "MSBuildPostProcessOutputFile", (
					// The file the assembly-preparer's post-processing pass writes its MSBuild output properties to.
					new LoadValue ((key, value) => MSBuildPostProcessOutputFile = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, MSBuildPostProcessOutputFile, storage))
				)},
				{ "MtouchFloat32", (
					new LoadValue ((key, value) => loadNullableBool (key, value, out Application.AotFloat32)),
					new SaveValue ((key, storage) => saveNullableBool (key, Application.AotFloat32, storage))
				)},
				{ "NoWarn", ( // we should support '$(NoWarn)' at some point: https://github.com/dotnet/macios/issues/25645
					new LoadValue ((key, value) => loadWarningLevel (key, value, ErrorHelper.WarningLevel.Disable)),
					new SaveValue ((key, storage) => saveWarningLevel (key, storage, ErrorHelper.WarningLevel.Disable))
				)},
				{ "Optimize", (
					new LoadValue ((key, value) => user_optimize_flags = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, user_optimize_flags, storage))
				)},
				{ "PartialStaticRegistrarLibrary", (
					new LoadValue ((key, value) => PartialStaticRegistrarLibrary = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, PartialStaticRegistrarLibrary, storage))
				)},
				{ "Platform", (
					new LoadValue ((key, value) => Platform = ApplePlatformExtensions.Parse (value)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={Platform.AsString ()}"))
				)},
				{ "PlatformAssembly", (
					new LoadValue ((key, value) => PlatformAssembly = Path.GetFileNameWithoutExtension (value)),
					new SaveValue ((key, storage) => saveNonEmpty (key, string.IsNullOrEmpty (PlatformAssembly) ? PlatformAssembly : PlatformAssembly + ".dll", storage))
				)},
				{ "PublishTrimmed", (
					new LoadValue ((key, value) => PublishTrimmed = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(PublishTrimmed ? "true" : "false")}"))
				 )},
				{ "PublishReadyToRun", (
					new LoadValue ((key, value) => {
						if (!string.IsNullOrEmpty (value)) {
							if (!TryParseOptionalBoolean (value, out var publishReadyToRun))
								throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
							Application.PublishReadyToRun = publishReadyToRun;
						}
					}),
					new SaveValue ((key, storage) => saveNullableBool (key, Application.PublishReadyToRun, storage))
				)},
				{ "PublishReadyToRunContainerFormat", (
					new LoadValue ((key, value) => Application.PublishReadyToRunContainerFormat = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.PublishReadyToRunContainerFormat, storage))
				)},
				{ "ReferenceNativeSymbol", (
					new LoadValue ((key, value) => {
						(string symbolType, string symbolMode, string symbol) = SplitString3 (value, ':');
						var mode = SymbolMode.Default;
						switch (symbolMode) {
						case "Ignore":
							mode = SymbolMode.Ignore;
							break;
						case "":
							break;
						default:
							throw new InvalidOperationException ($"Unknown symbol mode '{symbolMode}' for symbol '{symbol}'. Expected 'Ignore' or nothing at all.");
						}
						switch (symbolType) {
						case "Function":
							DerivedLinkContext.RequiredSymbols.AddFunction (symbol, mode);
							break;
						case "ObjectiveCClass":
							DerivedLinkContext.RequiredSymbols.AddObjectiveCClass (symbol, mode);
							break;
						case "Field":
							DerivedLinkContext.RequiredSymbols.AddField (symbol, mode);
							break;
						default:
							throw new InvalidOperationException ($"Unknown symbol type '{symbolType}' for symbol '{symbol}'. Expected 'Function', 'ObjectiveCClass', or 'Field'.");
						}
					}),
					new SaveValue ((key, storage) => {
						foreach (var symbol in DerivedLinkContext.RequiredSymbols) {
							var mode = symbol.Mode == SymbolMode.Ignore ? "Ignore" : "";
							switch (symbol.Type) {
							case SymbolType.Function:
							case SymbolType.ObjectiveCClass:
							case SymbolType.Field:
								storage.Add ($"{key}={symbol.Type}:{mode}:{symbol.Name}");
								break;
							default:
								throw new InvalidOperationException ($"Unknown symbol type '{symbol.Type}' for symbol '{symbol.Name}'. Expected 'Function', 'ObjectiveCClass', or 'Field'.");
							}
						}
					})
				)},
				{ "RelativeAppBundlePath", (
					new LoadValue ((key, value) => RelativeAppBundlePath = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, RelativeAppBundlePath, storage))
				)},
				{ "Registrar", (
					new LoadValue ((key, value) => Application.ParseRegistrar (value)),
					new SaveValue ((key, storage) => {
						if (Application.Registrar == RegistrarMode.Default)
							return;
						storage.Add ($"{key}={Application.Registrar}");
					})
				)},
				{ "RequireLinkWithAttributeForObjectiveCClassSearch", (
					new LoadValue ((key, value) => {
						if (!TryParseOptionalBoolean (value, out var require_link_with_attribute_for_objectivec_class_search, defaultValue: false))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						Application.RequireLinkWithAttributeForObjectiveCClassSearch = require_link_with_attribute_for_objectivec_class_search.Value;
					}),
					new SaveValue ((key, storage) => storage.Add ($"{key}={(Application.RequireLinkWithAttributeForObjectiveCClassSearch ? "true" : "false")}"))
				)},
				{ "RequirePInvokeWrappers", (
					new LoadValue ((key, value) => {
						if (!TryParseOptionalBoolean (value, out var require_pinvoke_wrappers, defaultValue: false))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						Application.RequiresPInvokeWrappers = require_pinvoke_wrappers.Value;
					}),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, Application.RequiresPInvokeWrappers, storage))
				)},
				{ "RuntimeConfigurationFile", (
					new LoadValue ((key, value) => Application.RuntimeConfigurationFile = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.RuntimeConfigurationFile, storage))
				)},
				{ "RuntimeConfigurationFilePath", (
					new LoadValue ((key, value) => RuntimeConfigurationFilePath = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, RuntimeConfigurationFilePath, storage))
				)},
				{ "SdkDevPath", (
					new LoadValue ((key, value) => Application.SdkRoot = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.SdkRoot, storage))
				)},
				{ "SdkRootDirectory", (
					new LoadValue ((key, value) => {
						SdkRootDirectory = value;
						Application.FrameworkCurrentDirectory = value;
					}),
					new SaveValue ((key, storage) => saveNonEmpty (key, SdkRootDirectory, storage))
				)},
				{ "SdkVersion", (
					new LoadValue ((key, value) => {
						if (!Version.TryParse (value, out var sdk_version))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						SdkVersion = sdk_version;
					}),
					new SaveValue ((key, storage) => saveNonEmpty (key, SdkVersion?.ToString (), storage))
				)},
				{ "SkipMarkingNSObjectsInUserAssemblies", (
					new LoadValue ((key, value) => {
						if (!TryParseOptionalBoolean (value, out var skip_marking_nsobjects_in_user_assemblies))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						Application.SkipMarkingNSObjectsInUserAssemblies = skip_marking_nsobjects_in_user_assemblies.Value;
					}),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, Application.SkipMarkingNSObjectsInUserAssemblies, storage))
				)},
				{ "TargetArchitectures", (
					new LoadValue ((key, value) => {
						if (!Enum.TryParse<Abi> (value, out var abi))
							throw new InvalidOperationException ($"Unknown target architectures: {value} in {linker_file}");
						Abi = abi | (Abi & Abi.LLVM); // Preserve the LLVM flag if it was set, since TargetArchitectures is orthogonal to LLVM
					}),
					new SaveValue ((key, storage) => saveNonEmpty (key, (Abi & ~Abi.LLVM).ToString (), storage))
				)},
				{ "TargetFramework", (
					new LoadValue ((key, value) => {
						if (!TargetFramework.TryParse (value, out var tf))
							throw new InvalidOperationException ($"Invalid TargetFramework '{value}' in {linker_file}");
						Application.TargetFramework = TargetFramework.Parse (value);
					}),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.TargetFramework.ToString (), storage))
				)},
				{ "TrimMode", (
					new LoadValue ((key, value) => TrimMode = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, TrimMode, storage))
				)},
				{ "TrimExportAttributes", (
					new LoadValue ((key, value) => {
						if (string.IsNullOrEmpty (value)) {
							Application.TrimExportAttributes = null;
						} else {
							loadNullableBool (key, value, out Application.TrimExportAttributes);
						}
					}),
					new SaveValue ((key, storage) => saveNullableBool (key, Application.TrimExportAttributes, storage))
				)},
				{ "TypeMapAssemblyName", (
					new LoadValue ((key, value) => Application.TypeMapAssemblyName = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.TypeMapAssemblyName, storage))
				)},
				{ "TypeMapFilePath", (
					new LoadValue ((key, value) => TypeMapFilePath = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, TypeMapFilePath, storage))
				)},
				{ "TypeMapOutputDirectory", (
					new LoadValue ((key, value) => Application.TypeMapOutputDirectory = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, Application.TypeMapOutputDirectory, storage))
				)},
				{ "UnmanagedCallersOnlyMapPath", (
					new LoadValue ((key, value) => UnmanagedCallersOnlyMapPath = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, UnmanagedCallersOnlyMapPath, storage))
				)},
				{ "UseLlvm", (
					new LoadValue ((key, value) => {
						var use_llvm = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase);
						if (use_llvm) {
							Abi |= Abi.LLVM;
						} else {
							Abi &= ~Abi.LLVM;
						}
					}),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, Abi.HasFlag (Abi.LLVM), storage))
				)},
				{ "Verbosity", (
					new LoadValue ((key, value) => {
						if (!int.TryParse (value, out var verbosity))
							throw new InvalidOperationException ($"Invalid Verbosity '{value}' in {linker_file}");
						Application.Verbosity = verbosity;
					}),
					new SaveValue ((key, storage) => {
						if (Application.Verbosity != 0)
							storage.Add ($"{key}={Application.Verbosity}");
					})
				)},
				{ "Warn", (
					new LoadValue ((key, value) => loadWarningLevel (key, value, ErrorHelper.WarningLevel.Warning)),
					new SaveValue ((key, storage) => saveWarningLevel (key, storage, ErrorHelper.WarningLevel.Warning))
				)},
				{ "WarnAsError", (
					new LoadValue ((key, value) => loadWarningLevel (key, value, ErrorHelper.WarningLevel.Error)),
					new SaveValue ((key, storage) => saveWarningLevel (key, storage, ErrorHelper.WarningLevel.Error))
				)},
				{ "XamarinRuntime", (
					new LoadValue ((key, value) => {
						if (!Enum.TryParse<XamarinRuntime> (value, out var rv))
							throw new InvalidOperationException ($"Invalid XamarinRuntime '{value}' in {linker_file}");
						Application.XamarinRuntime = rv;
					}),
					new SaveValue ((key, storage) => {
						storage.Add ($"{key}={Application.XamarinRuntime}");
					})
				)},
				{ "InvariantGlobalization", (
					new LoadValue ((key, value) => InvariantGlobalization = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, InvariantGlobalization, storage))
				)},
				{ "HybridGlobalization", (
					new LoadValue ((key, value) => HybridGlobalization = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase)),
					new SaveValue ((key, storage) => saveOptionalDefaultFalseBool (key, HybridGlobalization, storage))
				)},
				{ "XamarinNativeLibraryDirectory", (
					new LoadValue ((key, value) => XamarinNativeLibraryDirectory = value),
					new SaveValue ((key, storage) => saveNonEmpty (key, XamarinNativeLibraryDirectory, storage))
				)},
				{ "XcodeVersion", (
					new LoadValue ((key, value) => {
						if (!Version.TryParse (value, out var xcode_version))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						XcodeVersion = xcode_version;
					}),
					new SaveValue ((key, storage) => saveNonEmpty (key, XcodeVersion?.ToString (), storage))
				)},
			};

			return dict;
		}

		public LinkerConfiguration (IToolLog log, string linker_file, Configurator? customConfigurator = null)
			 : this (log, File.ReadAllLines (linker_file).ToList (), linker_file, customConfigurator)
		{
		}

		public LinkerConfiguration (IToolLog log, List<string> lines, string linker_file, Configurator? customConfigurator = null)
		{
			this.Logger = log;

			LinkerFile = linker_file;

			Profile = new BaseProfile (this);
			Application = new Application (this);

			AssemblyResolver = new DotNetResolver (Application);
			MetadataResolver = new MetadataResolver (AssemblyResolver);

			configurations.Add (this.Context, this);

			CompilerFlags = new CompilerFlags (Application);

			var configurator = GetConfigurator (linker_file);

			var significantLines = new List<string> (); // This is the input the cache uses to verify if the cache is still valid
			for (var i = 0; i < lines.Count; i++) {
				var line = lines [i].TrimStart ();
				if (line.Length == 0 || line [0] == '#')
					continue; // Allow comments

				var eq = line.IndexOf ('=');
				if (eq == -1)
					throw new InvalidOperationException ($"Invalid syntax for line {i + 1} ('{line}') in {linker_file}:{i + 1} : No equals sign.");

				significantLines.Add (line);

				var key = line [..eq];
				var value = line [(eq + 1)..];

				if (string.IsNullOrEmpty (value))
					continue;

				if (configurator.TryGetValue (key, out var actions)) {
					actions.Load (key, value);
				} else if (customConfigurator?.TryGetValue (key, out var customActions) == true) {
					customActions.Load (key, value);
				} else {
					throw new InvalidOperationException ($"Unknown configuration key '{key}' in {linker_file} at line {i + 1}.");
				}
			}

			// Optimizations.Parse can only be called after setting ErrorHelper.Platform
			if (!StringUtils.IsNullOrEmpty (user_optimize_flags)) {
				var messages = new List<ProductException> ();
				Application.Optimizations.Parse (Application.Platform, user_optimize_flags, messages);
				ErrorHelper.Show (Application, messages);
			}

			Application.CreateCache (significantLines.ToArray ());
			if (Application.Cache is not null && !string.IsNullOrEmpty (CacheDirectory))
				Application.Cache.SetLocation (Application, CacheDirectory);
			if (DeploymentTarget is not null)
				Application.DeploymentTarget = DeploymentTarget;
			if (SdkVersion is not null) {
				Application.SdkVersion = SdkVersion;
				Application.NativeSdkVersion = SdkVersion;
			}

			Application.Abi = Abi;
			Application.LinkContext = DerivedLinkContext;

			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				Application.BuildTarget = IsSimulatorBuild ? BuildTarget.Simulator : BuildTarget.Device;
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				break;

			default:
				throw new System.InvalidOperationException ($"Unknown platform: {Platform}");
			}

			if (Application.TargetFramework.Platform != Platform)
				throw ErrorHelper.CreateError (99, "Inconsistent platforms. TargetFramework={0}, Platform={1}", Application.TargetFramework.Platform, Platform);

			if (Application.XamarinRuntime != XamarinRuntime.MonoVM && Application.UseInterpreter) {
				Application.Log (4, "The interpreter is enabled, but the current runtime isn't MonoVM. The interpreter settings will be ignored.");
				Application.UnsetInterpreter ();
			}

			if (RuntimeInformation.IsOSPlatform (OSPlatform.OSX)) {
				Driver.ValidateXcode (Application, false, false);
			} else if (XcodeVersion is not null) {
				// Xcode only exists on macOS, so when running on any other OS (which happens when
				// building remotely from Windows) we can't look at the Xcode installation. Use the
				// Xcode version MSBuild fetched from the Mac instead.
				Application.XcodeVersion = XcodeVersion;
			}

			Application.InitializeCommon ();
			Application.Initialize ();
		}

		public void Save (List<string> storage, Configurator? customConfigurator = null)
		{
			var configurator = GetConfigurator (LinkerFile);
			foreach (var kvp in configurator.OrderBy (v => v.Key)) {
				kvp.Value.Save (kvp.Key, storage);
			}

			if (customConfigurator is not null) {
				foreach (var kvp in customConfigurator.OrderBy (v => v.Key)) {
					kvp.Value.Save (kvp.Key, storage);
				}
			}
		}

		// Splits a string in three based on the split character.
		//   "a:b" => "a", "b", ""
		//   "a:b:c" => "a", "b", "c"
		//   "a:b:c:d" => "a", "b", "c:d"
		(string, string, string) SplitString3 (string value, char split)
		{
			var idx = value.IndexOf (':');
			if (idx == -1)
				return (value, "", "");
			var a = value [..idx];
			var idx2 = value.IndexOf (':', idx + 1);
			if (idx2 == -1) {
				var bc = value [(idx + 1)..];
				return (a, bc, "");
			}
			var b = value [(idx + 1)..idx2];
			var c = value [(idx2 + 1)..];
			return (a, b, c);
		}


		bool TryParseOptionalBoolean (string input, [NotNullWhen (true)] out bool? value, bool defaultValue = true)
		{
			value = null;

			if (string.IsNullOrEmpty (input)) {
				value = defaultValue;
				return true;
			}

			if (string.Equals (input, "true", StringComparison.OrdinalIgnoreCase)) {
				value = true;
				return true;
			}

			if (string.Equals (input, "false", StringComparison.OrdinalIgnoreCase)) {
				value = false;
				return true;
			}

			return false;
		}

		AssemblyBuildTarget ParseLinkMode (string value, string variableName)
		{
			if (string.Equals (value, "dylib", StringComparison.OrdinalIgnoreCase)) {
				return AssemblyBuildTarget.DynamicLibrary;
			} else if (string.Equals (value, "static", StringComparison.OrdinalIgnoreCase)) {
				return AssemblyBuildTarget.StaticObject;
			}

			throw new InvalidOperationException ($"Invalid {variableName} '{value}' in {LinkerFile}");
		}

		public void Write ()
		{
			if (Verbosity > 0) {
				Application.Log ($"LinkerConfiguration:");
				Application.Log ($"    ABI: {Abi.AsArchString ()}");
				Application.Log ($"    AOTArguments: {string.Join (", ", Application.AotArguments)}");
				Application.Log ($"    AOTOutputDirectory: {AOTOutputDirectory}");
				Application.Log ($"    DedupAssembly: {DedupAssembly}");
				Application.Log ($"    AppBundleManifestPath: {Application.InfoPListPath}");
				Application.Log ($"    AreAnyAssembliesTrimmed: {Application.AreAnyAssembliesTrimmed}");
				Application.Log ($"    AssemblyName: {Application.AssemblyName}");
				Application.Log ($"    CacheDirectory: {CacheDirectory}");
				Application.Log ($"    Debug: {Application.EnableDebug}");
				Application.Log ($"    Dlsym: {Application.DlsymOptions} {(Application.DlsymAssemblies is not null ? string.Join (" ", Application.DlsymAssemblies.Select (v => (v.Item2 ? "+" : "-") + v.Item1)) : string.Empty)}");
				Application.Log ($"    DeploymentTarget: {DeploymentTarget}");
				Application.Log ($"    EnableSGenConc {Application.EnableSGenConc}");
				Application.Log ($"    InlineDlfcnMethods: {InlineDlfcnMethods}");
				Application.Log ($"    IntermediateLinkDir: {IntermediateLinkDir}");
				Application.Log ($"    IntermediateOutputPath: {IntermediateOutputPath}");
				Application.Log ($"    InterpretedAssemblies: {string.Join (", ", Application.InterpretedAssemblies)}");
				Application.Log ($"    ItemsDirectory: {ItemsDirectory}");
				Application.Log ($"    {FrameworkAssemblies.Count} framework assemblies:");
				foreach (var fw in FrameworkAssemblies.OrderBy (v => v))
					Application.Log ($"        {fw}");
				Application.Log ($"    IsSimulatorBuild: {IsSimulatorBuild}");
				Application.Log ($"    MarshalManagedExceptions: {Application.MarshalManagedExceptions} (IsDefault: {Application.IsDefaultMarshalManagedExceptionMode})");
				Application.Log ($"    MarshalObjectiveCExceptions: {Application.MarshalObjectiveCExceptions}");
				Application.Log ($"    {Application.MonoLibraries.Count} mono libraries:");
				foreach (var lib in Application.MonoLibraries.OrderBy (v => v))
					Application.Log ($"        {lib}");
				Application.Log ($"    Optimize: {user_optimize_flags} => {Application.Optimizations}");
				Application.Log ($"    PartialStaticRegistrarLibrary: {PartialStaticRegistrarLibrary}");
				Application.Log ($"    Platform: {Platform}");
				Application.Log ($"    PlatformAssembly: {PlatformAssembly}.dll");
				Application.Log ($"    RelativeAppBundlePath: {RelativeAppBundlePath}");
				Application.Log ($"    Registrar: {Application.Registrar} (Options: {Application.RegistrarOptions})");
				Application.Log ($"    RuntimeConfigurationFile: {Application.RuntimeConfigurationFile}");
				Application.Log ($"    RuntimeConfigurationFilePath: {RuntimeConfigurationFilePath}");
				Application.Log ($"    RequirePInvokeWrappers: {Application.RequiresPInvokeWrappers}");
				Application.Log ($"    SdkDevPath: {Application.SdkRoot}");
				Application.Log ($"    SdkRootDirectory: {SdkRootDirectory}");
				Application.Log ($"    SdkVersion: {SdkVersion}");
				Application.Log ($"    TypeMapAssemblyName: {Application.TypeMapAssemblyName}");
				Application.Log ($"    TypeMapFilePath: {TypeMapFilePath}");
				Application.Log ($"    TypeMapOutputDirectory: {Application.TypeMapOutputDirectory}");
				Application.Log ($"    UnmanagedCallersOnlyMapPath: {UnmanagedCallersOnlyMapPath}");
				Application.Log ($"    UseInterpreter: {Application.UseInterpreter}");
				Application.Log ($"    UseLlvm: {Application.IsLLVM}");
				Application.Log ($"    Verbosity: {Verbosity}");
				Application.Log ($"    XamarinNativeLibraryDirectory: {XamarinNativeLibraryDirectory}");
				Application.Log ($"    XamarinRuntime: {Application.XamarinRuntime}");
				Application.Log ($"    XcodeVersion: {XcodeVersion}");
			}
		}


		public void WriteOutputForMSBuild (string itemName, List<MSBuildItem> items)
		{
			if (!msbuild_items.TryGetValue (itemName, out var list)) {
				msbuild_items [itemName] = items;
			} else {
				list.AddRange (items);
			}
		}

		// Register an MSBuild output property. The collected properties are written to the MSBuild
		// output file (see FlushOutputForMSBuild) so that MSBuild can read them back.
		public void SetOutputForMSBuild (string propertyName, string value)
		{
			msbuild_properties [propertyName] = value;
		}

		public void FlushOutputForMSBuild ()
		{
			// ItemsDirectory isn't set when running in the assembly-preparer, so only write
			// the item files when we have a directory to write them to.
			if (!string.IsNullOrEmpty (ItemsDirectory)) {
				Directory.CreateDirectory (ItemsDirectory);
				foreach (var kvp in msbuild_items) {
					var itemName = kvp.Key;
					var items = kvp.Value;

					var xmlNs = XNamespace.Get ("http://schemas.microsoft.com/developer/msbuild/2003");
					var elements = items.Select (item =>
						new XElement (xmlNs + itemName,
							new XAttribute ("Include", item.Include),
								item.Metadata.Select (metadata => new XElement (xmlNs + metadata.Key, metadata.Value))));

					var document = new XDocument (
						new XElement (xmlNs + "Project",
							new XElement (xmlNs + "ItemGroup",
								elements)));

					document.Save (Path.Combine (ItemsDirectory, itemName + ".items"));
				}
			}

			// Write the collected MSBuild output properties (alphabetically sorted, one 'Name=Value' per line)
			// to the output file for the current pass, so that MSBuild can read them back. We always write the
			// file (even when there are no properties), so it's a consistent, persistent artifact of the pass
			// (it's added to FileWrites by the _PrepareAssemblies/_PostprocessAssemblies targets).
			var outputFile = Application.IsPostProcessingAssemblies ? MSBuildPostProcessOutputFile : MSBuildOutputFile;
			if (!string.IsNullOrEmpty (outputFile)) {
				var directory = Path.GetDirectoryName (outputFile);
				if (!string.IsNullOrEmpty (directory))
					Directory.CreateDirectory (directory);
				File.WriteAllLines (outputFile, msbuild_properties.Select (kvp => $"{kvp.Key}={kvp.Value}"));
			}
		}

		public static void Report (LinkContext Context, params Exception [] exceptions)
		{
			Report (Context, (IList<Exception>) exceptions);
		}

		public static void Report (LinkContext context, IList<Exception> exceptions)
		{
			// Unwrap aggregate exceptions, and collect all exceptions into a single list.
			var list = ErrorHelper.CollectExceptions (exceptions);
			var log = context.Configuration.Logger;
			foreach (var ex in list) {
				if (ex is ProductException pe) {
					if (pe.IsError (context.Configuration.Application)) {
						log.LogError (pe);
					} else {
						log.LogWarning (pe);
					}
				} else {
					log.LogException (ex);
				}
			}
		}

		public IEnumerable<AssemblyDefinition> GetNonDeletedAssemblies (BaseStep step)
		{
			foreach (var assembly in Assemblies) {
				if (step.Annotations.GetAction (assembly) == Mono.Linker.AssemblyAction.Delete)
					continue;
				yield return assembly;
			}
		}

		public void Log (string value)
		{
			Log (0, value);
		}

		public void Log (string format, params object? [] args)
		{
			Log (0, format, args);
		}

		public void Log (int min_verbosity, string value)
		{
			if (min_verbosity > Verbosity)
				return;

			if (Logger is not null) {
				Logger.Log (value);
				return;
			}

			Console.WriteLine (value);
		}

		public void Log (int min_verbosity, string format, params object? [] args)
		{
			if (min_verbosity > Verbosity)
				return;

			var value = string.Format (format, args);
			Log (min_verbosity, value);
		}
	}
}

public class MSBuildItem {
	public string Include;
	public Dictionary<string, string> Metadata = new Dictionary<string, string> ();

	public MSBuildItem (string include)
	{
		Include = include;
	}
	public MSBuildItem (string include, Dictionary<string, string> metadata)
	{
		Include = include;
		Metadata = metadata;
	}
}

public enum InlineDlfcnMethodsMode {
	Disabled,
	Strict,
	Compat,
	Compatibility = Compat,
}

public enum InlineClassGetHandleMode {
	Disabled,
	Strict,
	Compat,
	Compatibility = Compat,
}
