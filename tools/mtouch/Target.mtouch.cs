// Copyright 2013--2014 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using MonoTouch.Tuner;

using Mono.Cecil;
using Mono.Tuner;
using Mono.Linker;
using Xamarin.Linker;

using Xamarin.Utils;

using Registrar;

namespace Xamarin.Bundler {
	public class BundleFileInfo {
		public HashSet<string> Sources = new HashSet<string> ();
		public bool DylibToFramework;
	}

	public partial class Target {
		public string TargetDirectory;
		public string AppTargetDirectory;

		public MonoTouchManifestResolver ManifestResolver = new MonoTouchManifestResolver ();
		public AssemblyDefinition ProductAssembly;

		// directories used during the build process
		public string ArchDirectory;
		public string PreBuildDirectory;
		public string BuildDirectory;
		public string LinkDirectory;

		public Dictionary<string, BundleFileInfo> BundleFiles = new Dictionary<string, BundleFileInfo> ();

		Dictionary<Abi, CompileTask> pinvoke_tasks = new Dictionary<Abi, CompileTask> ();
		List<CompileTask> link_with_task_output = new List<CompileTask> ();
		List<AOTTask> aot_dependencies = new List<AOTTask> ();
		List<LinkTask> embeddinator_tasks = new List<LinkTask> ();
		Dictionary<Abi, CompilerFlags> linker_flags_by_abi = new Dictionary<Abi, CompilerFlags> ();
		Dictionary<Abi, NativeLinkTask> link_tasks = new Dictionary<Abi, NativeLinkTask> ();

		// If the assemblies were symlinked.
		public bool Symlinked;

		// This is a list of all the architectures we need to build, which may include any architectures
		// in any extensions (but not the main app).
		List<Abi> all_architectures;
		public List<Abi> AllArchitectures {
			get {
				if (all_architectures is null) {
					all_architectures = new List<Abi> ();
					var mask = Is32Build ? Abi.Arch32Mask : Abi.Arch64Mask;
					foreach (var abi in App.AllArchitectures) {
						var a = abi & mask;
						if (a != 0)
							all_architectures.Add (abi);
					}
				}
				return all_architectures;
			}
		}

		List<Abi> GetArchitectures (AssemblyBuildTarget build_target)
		{
			switch (build_target) {
			case AssemblyBuildTarget.StaticObject:
			case AssemblyBuildTarget.DynamicLibrary:
				return Abis;
			case AssemblyBuildTarget.Framework:
				return AllArchitectures;
			default:
				throw ErrorHelper.CreateError (100, Errors.MT0100, build_target);
			}
		}

		public void AddToBundle (string source, string bundle_path = null, bool dylib_to_framework_conversion = false)
		{
			BundleFileInfo info;

			if (bundle_path is null)
				bundle_path = Path.GetFileName (source);

			if (!BundleFiles.TryGetValue (bundle_path, out info))
				BundleFiles [bundle_path] = info = new BundleFileInfo () { DylibToFramework = dylib_to_framework_conversion };

			if (info.DylibToFramework != dylib_to_framework_conversion)
				throw ErrorHelper.CreateError (99, Errors.MX0099, "'invalid value for framework conversion'");

			info.Sources.Add (source);
		}

		void LinkWithBuildTarget (AssemblyBuildTarget build_target, string name, CompileTask link_task, IEnumerable<Assembly> assemblies)
		{
			switch (build_target) {
			case AssemblyBuildTarget.StaticObject:
				LinkWithTaskOutput (link_task);
				break;
			case AssemblyBuildTarget.DynamicLibrary:
				if (!(!App.HasFrameworksDirectory && assemblies.Any ((asm) => asm.IsCodeShared)))
					AddToBundle (link_task.OutputFile);
				LinkWithTaskOutput (link_task);
				break;
			case AssemblyBuildTarget.Framework:
				if (!(!App.HasFrameworksDirectory && assemblies.Any ((asm) => asm.IsCodeShared)))
					AddToBundle (link_task.OutputFile, $"Frameworks/{name}.framework/{name}", dylib_to_framework_conversion: true);
				LinkWithTaskOutput (link_task);
				break;
			default:
				throw ErrorHelper.CreateError (100, Errors.MT0100, build_target);
			}
		}

		public void LinkWithTaskOutput (CompileTask task)
		{
			if (task.SharedLibrary) {
				LinkWithDynamicLibrary (task.Abi, task.OutputFile);
			} else {
				LinkWithStaticLibrary (task.Abi, task.OutputFile);
			}
			link_with_task_output.Add (task);
		}

		public void LinkWithTaskOutput (IEnumerable<CompileTask> tasks)
		{
			foreach (var t in tasks)
				LinkWithTaskOutput (t);
		}

		public void LinkWithStaticLibrary (Abi abi, string path)
		{
			linker_flags_by_abi [abi & Abi.ArchMask].AddLinkWith (path);
		}

		public void LinkWithStaticLibrary (Abi abi, IEnumerable<string> paths)
		{
			linker_flags_by_abi [abi & Abi.ArchMask].AddLinkWith (paths);
		}

		public void LinkWithFramework (Abi abi, string path)
		{
			linker_flags_by_abi [abi & Abi.ArchMask].AddFramework (path);
		}

		public void LinkWithDynamicLibrary (Abi abi, string path)
		{
			linker_flags_by_abi [abi & Abi.ArchMask].AddLinkWith (path);
		}

		PInvokeWrapperGenerator pinvoke_state;
		PInvokeWrapperGenerator MarshalNativeExceptionsState {
			get {
				if (!App.RequiresPInvokeWrappers)
					return null;

				if (pinvoke_state is null) {
					pinvoke_state = new PInvokeWrapperGenerator () {
						App = App,
						SourcePath = Path.Combine (ArchDirectory, "pinvokes.m"),
						HeaderPath = Path.Combine (ArchDirectory, "pinvokes.h"),
						Registrar = (StaticRegistrar) StaticRegistrar,
					};
				}

				return pinvoke_state;
			}
		}

		Dictionary<Abi, string> executables;
		public IDictionary<Abi, string> Executables {
			get {
				if (executables is null) {
					executables = new Dictionary<Abi, string> ();
					if (App.IsSimulatorBuild && App.ArchSpecificExecutable) {
						// When using simlauncher, we copy the executable directly to the target directory.
						// When not using the simlauncher, but still building for the simulator, we write the executable to a arch-specific app directory (if building for both 32-bit and 64-bit), or just the app directory (if building for a single architecture)
						if (Abis.Count != 1)
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"expected exactly one abi for a simulator architecture, found: {string.Join (", ", Abis.Select ((v) => v.ToString ()))}");
						executables.Add (Abis [0], Path.Combine (TargetDirectory, App.ExecutableName));
					} else {
						foreach (var abi in Abis)
							executables.Add (abi, Path.Combine (Path.Combine (App.Cache.Location, abi.AsArchString (), App.ExecutableName)));
					}

				}
				return executables;
			}
		}

		public void Initialize (bool show_warnings)
		{
			// we want to load our own mscorlib[-runtime].dll, not something else we're being feeded
			// (e.g. bug #6612) since it might not match the libmono[-sgen].a library we'll link with,
			// so load the corlib we want first.

			var corlib_path = Path.Combine (Resolver.FrameworkDirectory, "mscorlib.dll");
			var corlib = ManifestResolver.Load (corlib_path);
			if (corlib is null)
				throw new ProductException (2006, true, Errors.MT2006, corlib_path);

			var roots = new List<AssemblyDefinition> ();
			foreach (var root_assembly in App.RootAssemblies) {
				var root = ManifestResolver.Load (root_assembly);
				if (root is null) {
					// We check elsewhere that the path exists, so I'm not sure how we can get into this.
					throw ErrorHelper.CreateError (2019, Errors.MT2019, root_assembly);
				}
				roots.Add (root);
			}

			foreach (var reference in App.References) {
				var ad = ManifestResolver.Load (reference);
				if (ad is null)
					throw new ProductException (2002, true, Errors.MT2002, reference);

				var root_assembly = roots.FirstOrDefault ((v) => v.MainModule.FileName == ad.MainModule.FileName);
				if (root_assembly is not null) {
					// If we asked the manifest resolver for assembly X and got back a root assembly, it means the requested assembly has the same identity as the root assembly, which is not allowed.
					throw ErrorHelper.CreateError (23, Errors.MT0023, root_assembly.MainModule.FileName, reference);
				}

				if (ad.MainModule.Runtime > TargetRuntime.Net_4_0)
					ErrorHelper.Show (new ProductException (11, false, Errors.MT0011, Path.GetFileName (reference), ad.MainModule.Runtime));

				// Figure out if we're referencing Xamarin.iOS or monotouch.dll
				var filename = ad.MainModule.FileName;
				if (Path.GetFileNameWithoutExtension (filename) == Driver.GetProductAssembly (App))
					ProductAssembly = ad;

				// repl / interpreter is a special case where some assemblies can switch location
				if (ManifestResolver.EnableRepl) {
					// in that case just tweak it before testing for mixed paths - since it's not a problem and should no warning should be in the logs
					if (filename.StartsWith (Path.Combine (Resolver.FrameworkDirectory, "repl"), StringComparison.Ordinal))
						filename = Path.Combine (Resolver.FrameworkDirectory, Path.GetFileName (filename));
				}

				if (ad != ProductAssembly && GetRealPath (filename) != GetRealPath (reference) && !filename.EndsWith (".resources.dll", StringComparison.Ordinal))
					ErrorHelper.Show (ErrorHelper.CreateWarning (109, Errors.MT0109, Path.GetFileName (reference), reference, filename));
			}

			ComputeListOfAssemblies ();

			if (App.LinkMode == LinkMode.None && App.I18n != I18nAssemblies.None)
				AddI18nAssemblies ();

			if (!App.Embeddinator) {
				if (!Assemblies.Any ((v) => v.AssemblyDefinition.Name.Name == Driver.GetProductAssembly (App)))
					throw ErrorHelper.CreateError (123, Errors.MT0123, App.RootAssemblies [0], Driver.GetProductAssembly (App));
			}

			foreach (var abi in Abis)
				linker_flags_by_abi [abi & Abi.ArchMask] = new CompilerFlags (this);

			// Verify that there are no entries in our list of intepreted assemblies that doesn't match
			// any of the assemblies we know about.
			if (App.InterpretedAssemblies.Count > 0) {
				var exceptions = new List<Exception> ();
				foreach (var entry in App.InterpretedAssemblies) {
					var assembly = entry;
					if (string.IsNullOrEmpty (assembly))
						continue;

					if (assembly [0] == '-')
						assembly = assembly.Substring (1);

					if (assembly == "all")
						continue;

					if (Assemblies.ContainsKey (assembly))
						continue;

					exceptions.Add (ErrorHelper.CreateWarning (142, Errors.MT0142, assembly));
				}
				ErrorHelper.ThrowIfErrors (exceptions);
			}
		}

		IEnumerable<AssemblyDefinition> GetAssemblies ()
		{
			if (App.LinkMode == LinkMode.None)
				return ManifestResolver.GetAssemblies ();

			List<AssemblyDefinition> assemblies = new List<AssemblyDefinition> ();
			if (LinkContext is null) {
				// use data from cache
				foreach (var assembly in Assemblies)
					assemblies.Add (assembly.AssemblyDefinition);
			} else {
				foreach (var assembly in LinkContext.GetAssemblies ()) {
					if (LinkContext.Annotations.GetAction (assembly) == AssemblyAction.Delete)
						continue;

					assemblies.Add (assembly);
				}
			}
			return assemblies;
		}

		//
		// Gets a flattened list of all the assemblies pulled by the root assembly
		//
		public void ComputeListOfAssemblies ()
		{
			var exceptions = new List<Exception> ();
			var assemblies = new HashSet<string> ();
			var cache_file = Path.Combine (this.ArchDirectory, "assembly-references.txt");

			if (File.Exists (cache_file)) {
				assemblies.UnionWith (File.ReadAllLines (cache_file));
				// Check if any of the referenced assemblies changed after we cached the complete set of references
				if (Application.IsUptodate (assemblies, new string [] { cache_file })) {
					// Load all the assemblies in the cached list of assemblies
					foreach (var assembly in assemblies) {
						var ad = ManifestResolver.Load (assembly);
						var asm = AddAssembly (ad);
						asm.ComputeSatellites ();
					}
					return;
				}

				// We must manually find all the references.
				assemblies.Clear ();
			}

			try {
				foreach (var root in App.RootAssemblies) {
					var assembly = ManifestResolver.Load (root);
					ComputeListOfAssemblies (assemblies, assembly, exceptions);
				}
			} catch (ProductException mte) {
				exceptions.Add (mte);
			} catch (Exception e) {
				exceptions.Add (new ProductException (9, true, e, Errors.MX0009, e.Message));
			}

			if (App.LinkMode == LinkMode.None)
				exceptions.AddRange (ManifestResolver.list);

			if (exceptions.Count > 0)
				throw new AggregateException (exceptions);

			// Cache all the assemblies we found.
			Directory.CreateDirectory (Path.GetDirectoryName (cache_file));
			File.WriteAllLines (cache_file, assemblies);
		}

		void ComputeListOfAssemblies (HashSet<string> assemblies, AssemblyDefinition assembly, List<Exception> exceptions)
		{
			if (assembly is null)
				return;

			var fqname = assembly.MainModule.FileName;
			if (assemblies.Contains (fqname))
				return;

			PrintAssemblyReferences (assembly);
			assemblies.Add (fqname);

			var asm = AddAssembly (assembly);
			asm.ComputeSatellites ();

			var main = assembly.MainModule;
			foreach (AssemblyNameReference reference in main.AssemblyReferences) {
				// Verify that none of the references references an incorrect platform assembly.
				switch (reference.Name) {
				case "Xamarin.iOS":
				case "Xamarin.TVOS":
				case "Xamarin.WatchOS":
				case "Xamarin.MacCatalyst":
					if (reference.Name != Driver.GetProductAssembly (App)) {
						if (App.Platform == ApplePlatform.MacCatalyst && reference.Name == "Xamarin.iOS") {
							// This is allowed, because it's a facade
							break;
						}
						exceptions.Add (ErrorHelper.CreateError (34, Errors.MT0034, reference.Name, Driver.TargetFramework.Identifier, assembly.FullName));
					}
					break;
				}

				var reference_assembly = ManifestResolver.Resolve (reference);
				if (reference_assembly is null) {
					ErrorHelper.Warning (136, Errors.MT0136, reference.FullName, main.FileName);
					continue;
				}
				ComputeListOfAssemblies (assemblies, reference_assembly, exceptions);
			}

			if (Profile.IsSdkAssembly (assembly) || Profile.IsProductAssembly (assembly))
				return; // We know there are no new assembly references from attributes in assemblies we ship

			// Custom Attribute metadata can include references to other assemblies, e.g. [X (typeof (Y)], 
			// but it is not reflected in AssemblyReferences :-( ref: #37611
			// so we must scan every custom attribute to look for System.Type
			GetCustomAttributeReferences (main, main, assemblies, exceptions);
			foreach (var ca in main.GetCustomAttributes ())
				GetCustomAttributeReferences (main, ca, assemblies, exceptions);
		}

		void GetCustomAttributeReferences (ModuleDefinition main, ICustomAttributeProvider cap, HashSet<string> assemblies, List<Exception> exceptions)
		{
			if (!cap.HasCustomAttributes)
				return;
			foreach (var ca in cap.CustomAttributes)
				GetCustomAttributeReferences (main, ca, assemblies, exceptions);
		}

		void GetCustomAttributeReferences (ModuleDefinition main, CustomAttribute ca, HashSet<string> assemblies, List<Exception> exceptions)
		{
			if (ca.HasConstructorArguments) {
				foreach (var arg in ca.ConstructorArguments)
					GetCustomAttributeArgumentReference (main, ca, arg, assemblies, exceptions);
			}
			if (ca.HasFields) {
				foreach (var arg in ca.Fields)
					GetCustomAttributeArgumentReference (main, ca, arg.Argument, assemblies, exceptions);
			}
			if (ca.HasProperties) {
				foreach (var arg in ca.Properties)
					GetCustomAttributeArgumentReference (main, ca, arg.Argument, assemblies, exceptions);
			}
		}

		void GetCustomAttributeArgumentReference (ModuleDefinition main, CustomAttribute ca, CustomAttributeArgument arg, HashSet<string> assemblies, List<Exception> exceptions)
		{
			if (!arg.Type.Is ("System", "Type"))
				return;
			var ar = (arg.Value as TypeReference)?.Scope as AssemblyNameReference;
			if (ar is null)
				return;
			var reference_assembly = ManifestResolver.Resolve (ar);
			if (reference_assembly is null) {
				ErrorHelper.Warning (137, Errors.MT0137, ar.FullName, main.Name, ca.AttributeType.FullName);
				return;
			}
			ComputeListOfAssemblies (assemblies, reference_assembly, exceptions);
		}

		bool IncludeI18nAssembly (Mono.Linker.I18nAssemblies assembly)
		{
			return (App.I18n & assembly) != 0;
		}

		public void AddI18nAssemblies ()
		{
			Assemblies.Add (LoadI18nAssembly ("I18N"));

			if (IncludeI18nAssembly (Mono.Linker.I18nAssemblies.CJK))
				Assemblies.Add (LoadI18nAssembly ("I18N.CJK"));

			if (IncludeI18nAssembly (Mono.Linker.I18nAssemblies.MidEast))
				Assemblies.Add (LoadI18nAssembly ("I18N.MidEast"));

			if (IncludeI18nAssembly (Mono.Linker.I18nAssemblies.Other))
				Assemblies.Add (LoadI18nAssembly ("I18N.Other"));

			if (IncludeI18nAssembly (Mono.Linker.I18nAssemblies.Rare))
				Assemblies.Add (LoadI18nAssembly ("I18N.Rare"));

			if (IncludeI18nAssembly (Mono.Linker.I18nAssemblies.West))
				Assemblies.Add (LoadI18nAssembly ("I18N.West"));
		}

		Assembly LoadI18nAssembly (string name)
		{
			var assembly = ManifestResolver.Resolve (AssemblyNameReference.Parse (name));
			return new Assembly (this, assembly);
		}

		public void LinkAssemblies (out List<AssemblyDefinition> assemblies, string output_dir, IEnumerable<Target> sharedCodeTargets)
		{
			var cache = (Dictionary<string, AssemblyDefinition>) Resolver.ResolverCache;
			var resolver = new AssemblyResolver (cache);

			resolver.AddSearchDirectory (Resolver.RootDirectory);
			resolver.AddSearchDirectory (Resolver.FrameworkDirectory);

			var main_assemblies = new List<AssemblyDefinition> ();
			foreach (var root in App.RootAssemblies)
				main_assemblies.Add (Resolver.Load (root));
			foreach (var appex in sharedCodeTargets) {
				foreach (var root in appex.App.RootAssemblies)
					main_assemblies.Add (Resolver.Load (root));
			}

			if (Driver.Verbosity > 0)
				Console.WriteLine ("Linking {0} into {1} using mode '{2}'", string.Join (", ", main_assemblies.Select ((v) => v.MainModule.FileName)), output_dir, App.LinkMode);

			LinkerOptions = new LinkerOptions {
				MainAssemblies = main_assemblies,
				OutputDirectory = output_dir,
				LinkMode = App.LinkMode,
				Resolver = resolver,
				SkippedAssemblies = App.LinkSkipped,
				I18nAssemblies = App.I18n,
				LinkSymbols = true,
				LinkAway = App.LinkAway,
				ExtraDefinitions = App.Definitions,
				Device = App.IsDeviceBuild,
				DebugBuild = App.EnableDebug,
				DumpDependencies = App.LinkerDumpDependencies,
				RuntimeOptions = App.RuntimeOptions,
				MarshalNativeExceptionsState = MarshalNativeExceptionsState,
				WarnOnTypeRef = App.WarnOnTypeRef,
				Target = this,
			};

			MonoTouch.Tuner.Linker.Process (LinkerOptions, out LinkContext, out assemblies);

			var state = MarshalNativeExceptionsState;
			if (state?.Started == true) {
				// The generator is 'started' by the linker, which means it may not
				// be started if the linker was not executed due to re-using cached results.
				state.End ();
			}

			ErrorHelper.Show (LinkContext.Exceptions);

			Driver.Watch ("Link Assemblies", 1);
		}

		bool linked;
		public void ManagedLink ()
		{
			if (linked)
				return;

			var cache_path = Path.Combine (ArchDirectory, "linked-assemblies.txt");

			// Get all the Target instances we're sharing code with. Make sure to only select targets with matching pointer size.
			var sharingTargets = App.SharedCodeApps.SelectMany ((v) => v.Targets).Where ((v) => v.Is32Build == Is32Build).ToList ();
			var allTargets = new List<Target> ();
			allTargets.Add (this); // We want ourselves first in this list.
			allTargets.AddRange (sharingTargets);

			// Include any assemblies from appex's we're sharing code with.
			foreach (var target in sharingTargets) {
				var targetAssemblies = target.Assemblies.ToList (); // We need to clone the list of assemblies, since we'll be modifying the original
				foreach (var asm in targetAssemblies) {
					Assembly main_asm;
					if (!Assemblies.TryGetValue (asm.Identity, out main_asm)) {
						// The appex has an assembly that's not present in the main app.
						// Re-load it into the main app.
						main_asm = new Assembly (this, asm.FullPath);
						main_asm.LoadAssembly (main_asm.FullPath);
						Assemblies.Add (main_asm);
						Driver.Log (1, "Added '{0}' from {1} to the set of assemblies to be linked.", main_asm.Identity, Path.GetFileNameWithoutExtension (target.App.AppDirectory));
					} else {
						asm.IsCodeShared = true;
					}
					// Use the same AOT information between both Assembly instances.
					target.Assemblies [main_asm.Identity].AotInfos = main_asm.AotInfos;
					main_asm.IsCodeShared = true;
				}
			}

			foreach (var a in Assemblies)
				a.CopyToDirectory (LinkDirectory, false, check_case: true);

			// Check if we can use a previous link result.
			var cached_output = new Dictionary<string, List<string>> ();
			if (!Driver.Force) {
				if (File.Exists (cache_path)) {
					using (var reader = new StreamReader (cache_path)) {
						string line = null;

						while ((line = reader.ReadLine ()) is not null) {
							var colon = line.IndexOf (':');
							if (colon == -1)
								continue;
							var key = line.Substring (0, colon);
							var value = line.Substring (colon + 1);
							switch (key) {
							case "RemoveDynamicRegistrar":
								switch (value) {
								case "true":
									App.Optimizations.RemoveDynamicRegistrar = true;
									break;
								case "false":
									App.Optimizations.RemoveDynamicRegistrar = false;
									break;
								default:
									App.Optimizations.RemoveDynamicRegistrar = null;
									break;
								}
								foreach (var t in sharingTargets)
									t.App.Optimizations.RemoveDynamicRegistrar = App.Optimizations.RemoveDynamicRegistrar;
								Driver.Log (1, $"Optimization dynamic registrar removal loaded from cached results: {(App.Optimizations.RemoveDynamicRegistrar.HasValue ? (App.Optimizations.RemoveUIThreadChecks.Value ? "enabled" : "disabled") : "not set")}");
								break;
							default:
								// key: app(ex)
								// value: assembly
								List<string> asms;
								if (!cached_output.TryGetValue (key, out asms))
									cached_output [key] = asms = new List<string> ();
								asms.Add (value);
								break;
							}
						}
					}

					var cache_valid = true;
					foreach (var target in allTargets) {
						List<string> cached_files;
						if (!cached_output.TryGetValue (target.App.AppDirectory, out cached_files)) {
							cache_valid = false;
							Driver.Log (2, $"The cached assemblies are not valid because there are no cached assemblies for {target.App.Name}.");
							break;
						}

						var outputs = new List<string> ();
						var inputs = new List<string> (cached_files);
						foreach (var input in inputs.ToArray ()) {
							var output = Path.Combine (PreBuildDirectory, Path.GetFileName (input));
							outputs.Add (output);
							if (File.Exists (input + ".mdb")) {
								// Debug files can change without the assemblies themselves changing
								// This should also invalidate the cached linker results, since the non-linked mdbs can't be copied.
								inputs.Add (input + ".mdb");
								outputs.Add (output + ".mdb");
							}
							var pdb = Path.ChangeExtension (input, "pdb");
							if (File.Exists (pdb)) {
								inputs.Add (pdb);
								outputs.Add (Path.ChangeExtension (output, "pdb"));
							}
							if (File.Exists (input + ".config")) {
								// If a config file changes, then the AOT-compiled output can be different,
								// so make sure to take config files into account as well.
								inputs.Add (input + ".config");
								outputs.Add (output + ".config");
							}
						}

						if (!cache_valid)
							break;

						if (!Application.IsUptodate (inputs, outputs)) {
							Driver.Log (2, $"The cached assemblies are not valid because some of the assemblies in {target.App.Name} are out-of-date.");
							cache_valid = false;
							break;
						}
					}

					cached_link = cache_valid;
				}
			}

			List<AssemblyDefinition> output_assemblies;
			if (cached_link) {
				Driver.Log (2, $"Reloading cached assemblies.");
				output_assemblies = new List<AssemblyDefinition> ();
				foreach (var file in cached_output.Values.SelectMany ((v) => v).Select ((v) => Path.GetFileName (v)).Distinct ())
					output_assemblies.Add (Resolver.Load (Path.Combine (PreBuildDirectory, file)));
				Driver.Watch ("Cached assemblies reloaded", 1);
				Driver.Log ("Cached assemblies reloaded.");
			} else {
				// Load the assemblies into memory.
				foreach (var a in Assemblies)
					a.LoadAssembly (a.FullPath);

				// Link!
				Driver.Watch ("Managed Link Preparation", 1);
				LinkAssemblies (out output_assemblies, PreBuildDirectory, sharingTargets);
			}

			// Verify that we don't get multiple identical assemblies from the linker.
			foreach (var group in output_assemblies.GroupBy ((v) => v.Name.Name)) {
				if (group.Count () != 1)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"The linker output contains more than one assemblies named '{group.Key}':\n\t{string.Join ("\n\t", group.Select ((v) => v.MainModule.FileName).ToArray ())}");
			}

			// Update (add/remove) list of assemblies in each app, since the linker may have both added and removed assemblies.
			// The logic for updating assemblies when doing code-sharing is not equivalent to when we're not code sharing
			// (in particular code sharing is not supported when there are xml linker definitions), so we need
			// to maintain two paths here.
			if (sharingTargets.Count == 0) {
				Assemblies.Update (this, output_assemblies);
			} else {
				// For added assemblies we have to determine exactly which apps need which assemblies.
				// Code sharing is only allowed if there are no linker xml definitions, nor any I18N values, which means that
				// we can limit ourselves to iterate over assembly references to create the updated list of assemblies.
				foreach (var t in allTargets) {
					// Find the root assembly
					// Here we assume that 'AssemblyReference.Name' == 'Assembly.Identity'.
					var rootAssemblies = new List<Assembly> ();
					foreach (var root in t.App.RootAssemblies)
						rootAssemblies.Add (t.Assemblies [Assembly.GetIdentity (root)]);
					var queue = new Queue<string> ();
					var collectedNames = new HashSet<string> ();

					// First collect the set of all assemblies in the app by walking the assembly references.
					foreach (var root in rootAssemblies)
						queue.Enqueue (root.Identity);
					do {
						var next = queue.Dequeue ();
						collectedNames.Add (next);

						var ad = output_assemblies.SingleOrDefault ((AssemblyDefinition v) => v.Name.Name == next);
						if (ad is null)
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"The assembly {next} was referenced by another assembly, but at the same time linked out by the linker");
						if (ad.MainModule.HasAssemblyReferences) {
							foreach (var ar in ad.MainModule.AssemblyReferences) {
								if (!collectedNames.Contains (ar.Name) && !queue.Contains (ar.Name))
									queue.Enqueue (ar.Name);
							}
						}
					} while (queue.Count > 0);

					// Now update the assembly collection
					var appexAssemblies = collectedNames.Select ((v) => output_assemblies.Single ((v2) => v2.Name.Name == v));
					t.Assemblies.Update (t, appexAssemblies);
					// And make sure every Target's assembly resolver knows about all the assemblies.
					foreach (var asm in t.Assemblies)
						t.Resolver.Add (asm.AssemblyDefinition);
				}

				// Find assemblies that are in more than 1 appex, but not in the container app.
				// These assemblies will be bundled once into the container .app instead of in each appex.
				var grouped = sharingTargets.SelectMany ((v) => v.Assemblies).
								GroupBy ((v) => Assembly.GetIdentity (v.AssemblyDefinition)).
								Where ((v) => !Assemblies.ContainsKey (v.Key)).
								Where ((v) => v.Count () > 1);
				foreach (var gr in grouped) {
					var asm = gr.First ();
					Assemblies.Add (asm);
					Resolver.Add (asm.AssemblyDefinition);
					gr.All ((v) => v.BundleInContainerApp = true);
				}


				// If any of the appex'es build to a grouped SDK framework, then we must ensure that all SDK assemblies
				// in that appex are also in the container app.
				foreach (var st in sharingTargets) {
					if (!st.App.ContainsGroupedSdkAssemblyBuildTargets)
						continue;
					foreach (var asm in st.Assemblies.Where ((v) => Profile.IsSdkAssembly (v.AssemblyDefinition) || Profile.IsProductAssembly (v.AssemblyDefinition))) {
						if (!Assemblies.ContainsKey (asm.Identity)) {
							Driver.Log (2, $"The SDK assembly {asm.Identity} will be included in the app because it's referenced by the extension {st.App.Name}");
							Assemblies.Add (asm);
						}
					}
				}
			}

			// Write the input files to the cache
			using (var writer = new StreamWriter (cache_path, false)) {
				var opt = App.Optimizations.RemoveDynamicRegistrar;
				writer.WriteLine ($"RemoveDynamicRegistrar:{(opt.HasValue ? (opt.Value ? "true" : "false") : string.Empty)}");
				foreach (var target in allTargets) {
					foreach (var asm in target.Assemblies) {
						writer.WriteLine ($"{target.App.AppDirectory}:{asm.FullPath}");
					}
				}
			}

			// Now the assemblies are in PreBuildDirectory, and they need to be in the BuildDirectory for the AOT compiler.
			foreach (var t in allTargets) {
				foreach (var a in t.Assemblies) {
					// All these assemblies are in the main app's PreBuildDirectory.
					a.FullPath = Path.Combine (PreBuildDirectory, a.FileName);

					// The linker can copy files (and not update timestamps), and then we run into this sequence:
					// * We run the linker, nothing changes, so the linker copies 
					//   all files to the PreBuild directory, with timestamps intact.
					// * This means that for instance SDK assemblies will have the original
					//   timestamp from their installed location, and the exe will have the 
					//   timestamp of when it was built.
					// * mtouch is executed again for some reason, and none of the input assemblies changed.
					//   We'll still re-execute the linker, because at least one of the input assemblies
					//   (the .exe) has a newer timestamp than some of the assemblies in the PreBuild directory.
					// So here we manually touch all the assemblies we have, to make sure their timestamps
					// change (this is us saying 'we know these files are up-to-date at this point in time').
					if (!cached_link) {
						Driver.Touch (a.FullPath);
						if (File.Exists (a.FullPath + ".mdb"))
							Driver.Touch (a.FullPath + ".mdb");
						var pdb = Path.ChangeExtension (a.FullPath, "pdb");
						if (File.Exists (pdb))
							Driver.Touch (pdb);
						var config = a.FullPath + ".config";
						if (File.Exists (config))
							Driver.Touch (config);
					}

					// Now copy to the build directory
					var target = Path.Combine (BuildDirectory, a.FileName);
					if (!a.CopyAssembly (a.FullPath, target))
						Driver.Log (3, "Target '{0}' is up-to-date.", target);
					a.FullPath = target;
				}
			}

			// Set the 'linked' flag for the targets sharing code, so that this method can be called
			// again, and it won't do anything for the appex's sharing code with the main app (but 
			// will still work for any appex's not sharing code).
			allTargets.ForEach ((v) => v.linked = true);
		}

		public void ProcessAssemblies ()
		{
			//
			// * Linking
			//   Copy assemblies to LinkDirectory
			//   Link and save to PreBuildDirectory
			//   If marshalling native exceptions:
			//     * Generate/calculate P/Invoke wrappers and save to PreBuildDirectory
			//   [AOT assemblies in BuildDirectory]
			//   Strip managed code save to TargetDirectory (or just copy the file if stripping is disabled).
			//
			// * No linking
			//   If marshalling native exceptions:
			//     Generate/calculate P/Invoke wrappers and save to PreBuildDirectory.
			//   If not marshalling native exceptions:
			//     Copy assemblies to PreBuildDirectory
			//     Copy unmodified assemblies to BuildDirectory
			//   [AOT assemblies in BuildDirectory]
			//   Strip managed code save to TargetDirectory (or just copy the file if stripping is disabled).
			//
			// Note that we end up copying assemblies around quite much,
			// this is because we we're comparing contents instead of 
			// filestamps, so we need the previous file around to be
			// able to do the actual comparison. For instance: in the
			// 'No linking' case above, we copy the assembly to PreBuild
			// before removing the resources and saving that result to Build.
			// The copy in PreBuild is required for the next build iteration,
			// to see if the original assembly has been modified or not (the
			// file in the Build directory might be different due to resource
			// removal even if the original assembly didn't change).
			//
			// This can probably be improved by storing digests/hashes instead
			// of the entire files, but this turned out a bit messy when
			// trying to make it work with the linker, so I decided to go for
			// simple file copying for now.
			//

			// 
			// Other notes:
			//
			// * We need all assemblies in the same directory when doing AOT-compilation.
			// * We cannot overwrite in-place, because it will mess up dependency tracking 
			//   and besides if we overwrite in place we might not be able to ignore
			//   insignificant changes (such as only a GUID change - the code is identical,
			//   but we still need the original assembly since the AOT-ed image also stores
			//   the GUID, and we fail at runtime if the GUIDs in the assembly and the AOT-ed
			//   image don't match - if we overwrite in-place we lose the original assembly and
			//   its GUID).
			// 

			LinkDirectory = Path.Combine (ArchDirectory, "1-Link");
			if (!Directory.Exists (LinkDirectory))
				Directory.CreateDirectory (LinkDirectory);

			PreBuildDirectory = Path.Combine (ArchDirectory, "2-PreBuild");
			if (!Directory.Exists (PreBuildDirectory))
				Directory.CreateDirectory (PreBuildDirectory);

			BuildDirectory = Path.Combine (ArchDirectory, "3-Build");
			if (!Directory.Exists (BuildDirectory))
				Directory.CreateDirectory (BuildDirectory);

			if (!Directory.Exists (TargetDirectory))
				Directory.CreateDirectory (TargetDirectory);

			ValidateAssembliesBeforeLink ();

			ManagedLink ();

			GatherFrameworks ();
		}

		public void CompilePInvokeWrappers ()
		{
			if (!App.RequiresPInvokeWrappers)
				return;

			if (!App.HasFrameworksDirectory && App.IsCodeShared)
				return;

			// Write P/Invokes
			var state = MarshalNativeExceptionsState;
			var ifile = state.SourcePath;
			var mode = App.LibPInvokesLinkMode;
			foreach (var abi in GetArchitectures (mode)) {
				var arch = abi.AsArchString ();
				string ofile;

				switch (mode) {
				case AssemblyBuildTarget.StaticObject:
					ofile = Path.Combine (App.Cache.Location, arch, "libpinvokes.a");
					break;
				case AssemblyBuildTarget.DynamicLibrary:
					ofile = Path.Combine (App.Cache.Location, arch, "libpinvokes.dylib");
					break;
				case AssemblyBuildTarget.Framework:
					ofile = Path.Combine (App.Cache.Location, arch, "Xamarin.PInvokes.framework", "Xamarin.PInvokes");

					var plist_path = Path.Combine (Path.GetDirectoryName (ofile), "Info.plist");
					var fw_name = Path.GetFileNameWithoutExtension (ofile);
					App.CreateFrameworkInfoPList (plist_path, fw_name, App.BundleId + ".frameworks." + fw_name, fw_name);
					break;
				default:
					throw ErrorHelper.CreateError (100, Errors.MT0100, mode);
				}

				var pinvoke_task = new PinvokesTask {
					Target = this,
					Abi = abi,
					InputFile = ifile,
					OutputFile = ofile,
					SharedLibrary = mode != AssemblyBuildTarget.StaticObject,
					Language = "objective-c++",
				};
				pinvoke_task.CompilerFlags.AddStandardCppLibrary ();
				if (pinvoke_task.SharedLibrary) {
					if (mode == AssemblyBuildTarget.Framework) {
						var name = Path.GetFileNameWithoutExtension (ifile);
						pinvoke_task.InstallName = $"@rpath/{name}.framework/{name}";
						AddToBundle (pinvoke_task.OutputFile, $"Frameworks/{name}.framework/{name}", dylib_to_framework_conversion: true);
					} else {
						pinvoke_task.InstallName = $"@rpath/{Path.GetFileName (ofile)}";
						AddToBundle (pinvoke_task.OutputFile);
					}
					pinvoke_task.CompilerFlags.AddFramework ("Foundation");
					pinvoke_task.CompilerFlags.LinkWithXamarin ();
				}
				pinvoke_tasks.Add (abi, pinvoke_task);

				LinkWithTaskOutput (pinvoke_task);
			}
		}

		void AOTCompile ()
		{
			if (App.IsSimulatorBuild)
				return;

			if (App.Platform == ApplePlatform.MacCatalyst)
				return;

			// Here we create the tasks to run the AOT compiler.
			foreach (var a in Assemblies) {
				if (!a.IsAOTCompiled)
					continue;

				foreach (var abi in GetArchitectures (a.BuildTarget)) {
					a.CreateAOTTask (abi);
				}
			}

			// Group the assemblies according to their target name, and link everything together accordingly.
			var grouped = Assemblies.GroupBy ((arg) => arg.BuildTargetName);
			foreach (var @group in grouped) {
				var name = @group.Key;
				var assemblies = @group.AsEnumerable ().ToArray ();
				if (assemblies.Length <= 0)
					continue;

				// We ensure elsewhere that all assemblies in a group have the same build target.
				var build_target = assemblies [0].BuildTarget;

				foreach (var abi in GetArchitectures (build_target)) {
					Driver.Log (2, "Building {0} from {1}", name, string.Join (", ", assemblies.Select ((arg1) => Path.GetFileName (arg1.FileName))));

					string install_name;
					string compiler_output;
					var compiler_flags = new CompilerFlags (this);
					var link_dependencies = new List<CompileTask> ();
					var infos = assemblies.Where ((asm) => asm.AotInfos.ContainsKey (abi)).Select ((asm) => asm.AotInfos [abi]).ToList ();
					var aottasks = infos.Select ((info) => info.Task);
					if (aottasks is null)
						continue;

					var existingLinkTask = infos.Where ((v) => v.LinkTask is not null).Select ((v) => v.LinkTask).ToList ();
					if (existingLinkTask.Count > 0) {
						if (existingLinkTask.Count != infos.Count)
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"Not all assemblies for {name} have link tasks");
						if (!existingLinkTask.All ((v) => v == existingLinkTask [0]))
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"Link tasks for {name} aren't all the same");

						LinkWithBuildTarget (build_target, name, existingLinkTask [0], assemblies);
						continue;
					}

					// We have to compile any source files to object files before we can link.
					var sources = infos.SelectMany ((info) => info.AsmFiles);
					if (sources.Count () > 0) {
						foreach (var src in sources) {
							// We might have to convert .s to bitcode assembly (.ll) first
							var assembly = src;
							BitCodeifyTask bitcode_task = null;
							if (App.EnableAsmOnlyBitCode) {
								bitcode_task = new BitCodeifyTask () {
									Input = assembly,
									OutputFile = Path.ChangeExtension (assembly, ".ll"),
									Platform = App.Platform,
									Abi = abi,
									DeploymentTarget = App.DeploymentTarget,
								};
								bitcode_task.AddDependency (aottasks);
								assembly = bitcode_task.OutputFile;
							}

							// Compile assembly code (either .s or .ll) to object file
							var compile_task = new CompileTask {
								Target = this,
								SharedLibrary = false,
								InputFile = assembly,
								OutputFile = Path.ChangeExtension (assembly, ".o"),
								Abi = abi,
								Language = bitcode_task is not null ? null : "assembler",
							};
							compile_task.AddDependency (bitcode_task);
							compile_task.AddDependency (aottasks);
							link_dependencies.Add (compile_task);
						}
					} else {
						aot_dependencies.AddRange (aottasks);
					}

					// Compile any .bc files to .o
					foreach (var info in infos) {
						foreach (var bc in info.BitcodeFiles) {
							var compile_task = new CompileTask {
								Target = this,
								SharedLibrary = false,
								InputFile = bc,
								OutputFile = bc + ".o",
								Abi = abi,
							};
							compile_task.CompilerFlags.AddOtherFlag (App.CustomLinkFlags);
							compile_task.AddDependency (info.Task);
							link_dependencies.Add (compile_task);
						}
					}

					var arch = abi.AsArchString ();
					switch (build_target) {
					case AssemblyBuildTarget.StaticObject:
						LinkWithTaskOutput (link_dependencies); // Any .s or .ll files from the AOT compiler (compiled to object files)
						foreach (var info in infos) {
							LinkWithStaticLibrary (abi, info.ObjectFiles);
						}
						continue; // no linking to do here.
					case AssemblyBuildTarget.DynamicLibrary:
						install_name = $"@rpath/lib{name}.dylib";
						compiler_output = Path.Combine (App.Cache.Location, arch, $"lib{name}.dylib");
						break;
					case AssemblyBuildTarget.Framework:
						install_name = $"@rpath/{name}.framework/{name}";
						compiler_output = Path.Combine (App.Cache.Location, arch, name);
						break;
					default:
						throw ErrorHelper.CreateError (100, Errors.MT0100, build_target);
					}

					CompileTask pinvoke_task;
					if (pinvoke_tasks.TryGetValue (abi, out pinvoke_task))
						link_dependencies.Add (pinvoke_task);

					foreach (var info in infos) {
						compiler_flags.AddLinkWith (info.ObjectFiles);
					}

					foreach (var task in link_dependencies)
						compiler_flags.AddLinkWith (task.OutputFile);

					foreach (var a in assemblies) {
						compiler_flags.AddFrameworks (a.Frameworks, a.WeakFrameworks);
						compiler_flags.AddLinkWith (a.LinkWith, a.ForceLoad);
						compiler_flags.AddOtherFlag (a.LinkerFlags.ToArray ());
						if (a.HasLinkWithAttributes) {
							var symbols = GetRequiredSymbols (a);
							switch (App.SymbolMode) {
							case SymbolMode.Ignore:
								break;
							case SymbolMode.Code:
								var tasks = GenerateReferencingSource (Path.Combine (App.Cache.Location, Path.GetFileNameWithoutExtension (a.FullPath) + "-unresolved-externals.m"), symbols);
								foreach (var task in tasks)
									compiler_flags.AddLinkWith (task.OutputFile);
								link_dependencies.AddRange (tasks);
								break;
							case SymbolMode.Linker:
								compiler_flags.ReferenceSymbols (symbols, abi);
								break;
							default:
								throw ErrorHelper.CreateError (99, Errors.MX0099, $"invalid symbol mode: {App.SymbolMode}");
							}
						}
					}
					if (App.Embeddinator)
						compiler_flags.AddOtherFlag (App.CustomLinkFlags);
					compiler_flags.LinkWithMono ();
					compiler_flags.LinkWithXamarin ();
					if (GetAllSymbols ().Contains ("UIApplicationMain"))
						compiler_flags.AddFramework ("UIKit");

					if (App.EnableLLVMOnlyBitCode) {
						// The AOT compiler doesn't optimize the bitcode so clang will do it
						compiler_flags.AddOtherFlag ("-fexceptions");
						var optimizations = assemblies.Select ((a) => App.GetLLVMOptimizations (a)).Where ((opt) => opt is not null).Distinct ().ToList ();
						if (optimizations.Count == 0) {
							compiler_flags.AddOtherFlag ("-O2");
						} else if (optimizations.Count == 1) {
							compiler_flags.AddOtherFlag (optimizations [0]);
						} else {
							throw ErrorHelper.CreateError (107, Errors.MT0107, string.Join (", ", assemblies.Select ((v) => v.Identity)), string.Join ("', '", optimizations));
						}
					}

					HandleMonoNative (App, compiler_flags);

					var link_task = new LinkTask () {
						Target = this,
						Abi = abi,
						OutputFile = compiler_output,
						InstallName = install_name,
						CompilerFlags = compiler_flags,
						Language = compiler_output.EndsWith (".s", StringComparison.Ordinal) ? "assembler" : null,
						SharedLibrary = build_target != AssemblyBuildTarget.StaticObject,
					};
					link_task.AddDependency (link_dependencies);
					link_task.AddDependency (aottasks);

					if (App.Embeddinator) {
						link_task.AddDependency (link_with_task_output);
						link_task.CompilerFlags.AddLinkWith (link_with_task_output.Select ((v) => v.OutputFile));
						embeddinator_tasks.Add (link_task);
					}

					LinkWithBuildTarget (build_target, name, link_task, assemblies);

					foreach (var info in infos)
						info.LinkTask = link_task;
				}
			}

			if (App.UseInterpreter)
				/* TODO: not sure? we might have to continue here, depending on
				 * the set of assemblies are AOT'd? */
				return;

			// Code in one assembly (either in a P/Invoke or a third-party library) can depend on a third-party library in another assembly.
			// This means that we must always build assemblies only when all their dependent assemblies have been built, so that 
			// we can link (natively) with the frameworks/dylibs for those dependent assemblies.
			// Fortunately we can cheat a bit, since this can (currently at least) only happen for assemblies that
			// have third-party libraries. This means that we only enforce this order for any assemblies that depend
			// on other assemblies that have third-party libraries.
			// Example:
			// * We can build System.dll and mscorlib.dll in parallel, even if System.dll depends on mscorlib.dll,
			//   because we know that mscorlib.dll does not have any third-party libraries.
			if (Assemblies.All ((arg) => arg.HasDependencyMap)) {
				var dict = Assemblies.ToDictionary ((arg) => Path.GetFileNameWithoutExtension (arg.FileName));
				foreach (var asm in Assemblies) {
					if (!asm.HasDependencyMap)
						continue;

					if (asm.BuildTarget == AssemblyBuildTarget.StaticObject)
						continue;

					if (Profile.IsSdkAssembly (asm.AssemblyDefinition) || Profile.IsProductAssembly (asm.AssemblyDefinition)) {
						//Console.WriteLine ("SDK assembly, so skipping assembly dependency checks: {0}", Path.GetFileNameWithoutExtension (asm.FileName));
						continue;
					}

					HashSet<Assembly> dependent_assemblies = new HashSet<Assembly> ();
					foreach (var dep in asm.DependencyMap) {
						Assembly dependentAssembly;
						if (!dict.TryGetValue (Path.GetFileNameWithoutExtension (dep), out dependentAssembly)) {
							//Console.WriteLine ("Could not find dependency '{0}' of '{1}'", dep, asm.Identity);
							continue;
						}
						if (asm == dependentAssembly)
							continue; // huh?

						// Nothing can depend on anything in our SDK, nor does our SDK depend on anything else in our SDK
						// So we can remove any SDK dependency
						if (Profile.IsSdkAssembly (dependentAssembly.AssemblyDefinition) || Profile.IsProductAssembly (dependentAssembly.AssemblyDefinition)) {
							//Console.WriteLine ("SDK assembly, so not a dependency of anything: {0}", Path.GetFileNameWithoutExtension (dependentAssembly.FileName));
							continue;
						}

						if (!dependentAssembly.HasLinkWithAttributes) {
							//Console.WriteLine ("Assembly {0} does not have LinkWith attributes, so there's nothing we can depend on.", dependentAssembly.Identity);
							continue;
						}

						if (dependentAssembly.BuildTargetName == asm.BuildTargetName) {
							//Console.WriteLine ("{0} is a dependency of {1}, but both are being built into the same target, so no dependency added.", Path.GetFileNameWithoutExtension (dep), Path.GetFileNameWithoutExtension (asm.FileName));
							continue;
						}

						//Console.WriteLine ("Added {0} as a dependency of {1}", Path.GetFileNameWithoutExtension (dep), Path.GetFileNameWithoutExtension (asm.FileName));
						dependent_assemblies.Add (dependentAssembly);
					}

					// Circular dependencies shouldn't happen, but still make sure, since it's technically possible
					// for users to do it.
					foreach (var abi in GetArchitectures (asm.BuildTarget)) {
						var target_task = asm.AotInfos [abi].LinkTask;
						var dependent_tasks = dependent_assemblies.Select ((v) => v.AotInfos [abi].LinkTask);

						var stack = new Stack<BuildTask> ();
						foreach (var dep in dependent_tasks) {
							stack.Clear ();
							stack.Push (target_task);
							if (target_task == dep || IsCircularTask (target_task, stack, dep)) {
								Driver.Log ("Found circular task.");
								Driver.Log ("Task {0} (with output {1}) depends on:", target_task.GetType ().Name, target_task.Outputs.First ());
								stack = new Stack<BuildTask> (stack.Reverse ());
								while (stack.Count > 0) {
									var node = stack.Pop ();
									Driver.Log ("   -> {0} (Output: {1})", node.GetType ().Name, node.Outputs.First ());
								}
							} else {
								target_task.AddDependency (dep);
								target_task.CompilerFlags.AddLinkWith (dep.OutputFile);
							}
						}
					}
				}
			}
		}

		bool IsCircularTask (BuildTask root, Stack<BuildTask> stack, BuildTask task)
		{
			stack.Push (task);

			foreach (var d in task?.Dependencies) {
				if (stack.Contains (d))
					return true;
				if (IsCircularTask (root, stack, d))
					return true;
			}
			stack.Pop ();

			return false;
		}

		public void Compile ()
		{
			// Compute the dependency map, and show warnings if there are any problems.
			List<Exception> exceptions = new List<Exception> ();
			foreach (var a in Assemblies)
				a.ComputeDependencyMap (exceptions);
			if (exceptions.Count > 0) {
				ErrorHelper.Show (exceptions);
				ErrorHelper.Warning (3006, Errors.MT3006);
			}

			List<string> registration_methods = new List<string> ();

			// The static registrar.
			RunRegistrarTask run_registrar_task = null;
			if (App.Registrar == RegistrarMode.Static) {
				var registrar_m = Path.Combine (ArchDirectory, "registrar.m");
				var registrar_h = Path.Combine (ArchDirectory, "registrar.h");

				run_registrar_task = new RunRegistrarTask {
					Target = this,
					RegistrarCodePath = registrar_m,
					RegistrarHeaderPath = registrar_h,
					RegistrationMethods = registration_methods,
				};

				foreach (var abi in GetArchitectures (AssemblyBuildTarget.StaticObject)) {
					var arch = abi.AsArchString ();
					var ofile = Path.Combine (App.Cache.Location, arch, Path.GetFileNameWithoutExtension (registrar_m) + ".o");

					var registrar_task = new CompileRegistrarTask {
						Target = this,
						Abi = abi,
						InputFile = registrar_m,
						OutputFile = ofile,
						RegistrarCodePath = registrar_m,
						RegistrarHeaderPath = registrar_h,
						SharedLibrary = false,
						Language = "objective-c++",
					};
					registrar_task.AddDependency (run_registrar_task);

					// This is because iOS has a forward declaration of NSPortMessage, but no actual declaration.
					// They still use NSPortMessage in other API though, so it can't just be removed from our bindings.
					registrar_task.CompilerFlags.AddOtherFlag ("-Wno-receiver-forward-class");

					// clang sometimes detects missing [super ...] calls, but clang doesn't know about
					// calling super through managed code, so ignore those warnings.
					registrar_task.CompilerFlags.AddOtherFlag ("-Wno-objc-missing-super-calls");

					if (Driver.XcodeVersion >= new Version (9, 0))
						registrar_task.CompilerFlags.AddOtherFlag ("-Wno-unguarded-availability-new");

					registrar_task.CompilerFlags.AddStandardCppLibrary ();

					LinkWithTaskOutput (registrar_task);
				}
			}

			if (App.Registrar == RegistrarMode.Dynamic && App.LinkMode == LinkMode.None) {
				string method;
				string library;
				string libraryName;
				switch (App.Platform) {
				case ApplePlatform.iOS:
					libraryName = "Xamarin.iOS";
					break;
				case ApplePlatform.WatchOS:
					libraryName = "Xamarin.WatchOS";
					break;
				case ApplePlatform.TVOS:
					libraryName = "Xamarin.TVOS";
					break;
				case ApplePlatform.MacCatalyst:
					libraryName = "Xamarin.MacCatalyst";
					break;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, App.Platform, App.ProductName);
				}
				method = StaticRegistrar.GetInitializationMethodName (libraryName);
				library = libraryName + ".registrar.a";

				var lib = Path.Combine (Driver.GetProductSdkLibDirectory (App), library);
				if (File.Exists (lib)) {
					registration_methods.Add (method);
					foreach (var abi in Abis)
						LinkWithStaticLibrary (abi, lib);
				}
			}

			// The main method.
			foreach (var abi in GetArchitectures (AssemblyBuildTarget.StaticObject)) {
				var arch = abi.AsArchString ();
				var main_m = Path.Combine (App.Cache.Location, arch, "main.m");

				var generate_main_task = new GenerateMainTask {
					Target = this,
					Abi = abi,
					MainM = main_m,
					RegistrationMethods = registration_methods,
				};
				if (run_registrar_task is not null)
					generate_main_task.AddDependency (run_registrar_task);
				var main_o = Path.Combine (App.Cache.Location, arch, "main.o");
				var main_task = new CompileMainTask {
					Target = this,
					Abi = abi,
					InputFile = main_m,
					OutputFile = main_o,
					SharedLibrary = false,
					Language = "objective-c++",
				};
				main_task.AddDependency (generate_main_task);
				main_task.CompilerFlags.AddDefine ("MONOTOUCH");
				main_task.CompilerFlags.AddStandardCppLibrary ();
				LinkWithTaskOutput (main_task);
			}

			// Compile the managed assemblies into object files, frameworks or shared libraries
			AOTCompile ();

			Driver.Watch ("Compile", 1);
		}

		public IEnumerable<NativeLinkTask> NativeLink (BuildTasks build_tasks)
		{
			if (App.Embeddinator && App.IsDeviceBuild) {
				build_tasks.AddRange (embeddinator_tasks);
				return Array.Empty<NativeLinkTask> ();
			}

			foreach (var abi in Abis) {
				var link_task = NativeLink (build_tasks, abi, Executables [abi]);
				link_tasks [abi] = link_task;
			}
			return link_tasks.Values;
		}

		public NativeLinkTask NativeLink (BuildTasks build_tasks, Abi abi, string output_file)
		{
			if (App.CustomLinkFlags?.Count > 0)
				App.DeadStrip = false;
			if (App.EnableLLVMOnlyBitCode)
				App.DeadStrip = false;

			var linker_flags = linker_flags_by_abi [abi & Abi.ArchMask];

			// Get global frameworks
			linker_flags.AddFrameworks (App.Frameworks, App.WeakFrameworks);
			linker_flags.AddFrameworks (Frameworks, WeakFrameworks);

			// Collect all LinkWith flags and frameworks from all assemblies.
			foreach (var a in Assemblies) {
				linker_flags.AddFrameworks (a.Frameworks, a.WeakFrameworks);
				if (a.BuildTarget == AssemblyBuildTarget.StaticObject)
					linker_flags.AddLinkWith (a.LinkWith, a.ForceLoad);
				linker_flags.AddOtherFlag (a.LinkerFlags.ToArray ());

				if (a.BuildTarget == AssemblyBuildTarget.StaticObject) {
					AotInfo info;
					if (!a.AotInfos.TryGetValue (abi, out info))
						continue;
					linker_flags.AddLinkWith (info.ObjectFiles);
				}
			}

			var bitcode = App.EnableBitCode;
			if (bitcode && Driver.XcodeVersion.Major < 14)
				linker_flags.AddOtherFlag (App.EnableMarkerOnlyBitCode ? "-fembed-bitcode-marker" : "-fembed-bitcode");

			if (!App.EnablePie.HasValue)
				App.EnablePie = true;

			if (App.Platform == ApplePlatform.iOS) {
				if (App.EnablePie.Value) {
					linker_flags.AddOtherFlag ("-Wl,-pie");
				} else {
					linker_flags.AddOtherFlag ("-Wl,-no_pie");
				}
			}

			CompileTask.GetArchFlags (linker_flags, abi);
			if (App.IsDeviceBuild) {
				linker_flags.AddOtherFlag ($"-m{Driver.GetTargetMinSdkName (App)}-version-min={App.DeploymentTarget}");
				linker_flags.AddOtherFlag ($"-isysroot", Driver.GetFrameworkDirectory (App));
			} else if (App.Platform == ApplePlatform.MacCatalyst) {
				CompileTask.GetCatalystCompilerFlags (linker_flags, abi, App);
			} else {
				CompileTask.GetSimulatorCompilerFlags (linker_flags, false, App);
			}
			linker_flags.LinkWithMono ();
			if (App.LibMonoLinkMode != AssemblyBuildTarget.StaticObject)
				AddToBundle (App.GetLibMono (App.LibMonoLinkMode));
			linker_flags.LinkWithXamarin ();
			if (App.LibXamarinLinkMode != AssemblyBuildTarget.StaticObject)
				AddToBundle (App.GetLibXamarin (App.LibXamarinLinkMode));

			linker_flags.AddOtherFlag ("-o", output_file);

			bool need_libcpp = false;
			if (App.EnableBitCode)
				need_libcpp = true;
			if (need_libcpp)
				linker_flags.AddOtherFlag ("-lc++");

			// allow the native linker to remove unused symbols (if the caller was removed by the managed linker)
			// Note that we include *all* (__Internal) p/invoked symbols here
			// We also include any fields from [Field] attributes.
			switch (App.SymbolMode) {
			case SymbolMode.Ignore:
				break;
			case SymbolMode.Code:
				LinkWithTaskOutput (GenerateReferencingSource (Path.Combine (App.Cache.Location, "reference.m"), GetRequiredSymbols ()));
				break;
			case SymbolMode.Linker:
				linker_flags.ReferenceSymbols (GetRequiredSymbols (), abi);
				break;
			default:
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"invalid symbol mode: {App.SymbolMode}");
			}

			if (App.Embeddinator) {
				linker_flags.AddOtherFlag ("-shared");
				linker_flags.AddOtherFlag ("-install_name", $"@rpath/{App.ExecutableName}.framework/{App.ExecutableName}");
			} else {
				string mainlib = null;
				if (App.IsWatchExtension) {
					linker_flags.AddOtherFlag ("-e", "_xamarin_watchextension_main");
					if (App.SdkVersion.Major >= 6 && App.DeploymentTarget.Major < 6) {
						// watchOS 6.0's WatchKit contains a WKExtensionMain function, and that's the entry point for Xcode-compiled watch extensions.
						// To make watch extensions work on earlier watchOS versions, there's a libWKExtensionMainLegacy.a library with a
						// a WKExtensionMain function that does what's needed (Xcode links with this library when deployment target < 6.0).
						linker_flags.AddOtherInitialFlag ("-lWKExtensionMainLegacy");
					}
				} else if (App.IsTVExtension) {
					mainlib = "libtvextension.a";
				} else if (App.IsExtension) {
					mainlib = "libextension.a";
				} else {
					mainlib = "libapp.a";
				}
				if (mainlib is not null) {
					var libmain = Path.Combine (Driver.GetProductSdkLibDirectory (App), mainlib);
					linker_flags.AddLinkWith (libmain, true);
				}
			}

			var libmonodir = Driver.GetMonoLibraryDirectory (App);
			if (App.EnableProfiling) {
				string libprofiler;
				switch (App.LibProfilerLinkMode) {
				case AssemblyBuildTarget.DynamicLibrary:
					libprofiler = Path.Combine (libmonodir, "libmono-profiler-log.dylib");
					linker_flags.AddLinkWith (libprofiler);
					AddToBundle (libprofiler);
					break;
				case AssemblyBuildTarget.StaticObject:
					libprofiler = Path.Combine (libmonodir, "libmono-profiler-log.a");
					linker_flags.AddLinkWith (libprofiler);
					break;
				case AssemblyBuildTarget.Framework: // We don't ship the profiler as a framework, so this should be impossible.
				default:
					throw ErrorHelper.CreateError (100, Errors.MT0100, App.LibProfilerLinkMode);
				}
			}

			if (App.UseInterpreter) {
				string libinterp = Path.Combine (libmonodir, "libmono-ee-interp.a");
				linker_flags.AddLinkWith (libinterp);
				string libicalltable = Path.Combine (libmonodir, "libmono-icall-table.a");
				linker_flags.AddLinkWith (libicalltable);
				string libilgen = Path.Combine (libmonodir, "libmono-ilgen.a");
				linker_flags.AddLinkWith (libilgen);
			}

			linker_flags.AddOtherFlag (App.CustomLinkFlags);

			if (App.DeadStrip)
				linker_flags.AddOtherFlag ("-dead_strip");

			if (App.IsExtension) {
				if (App.Platform == ApplePlatform.iOS && Driver.XcodeVersion.Major < 7) {
					linker_flags.AddOtherFlag ("-lpkstart");
					linker_flags.AddOtherFlag ("-F", Path.Combine (Driver.GetFrameworkDirectory (App), "System/Library/PrivateFrameworks"), "-framework", "PlugInKit");
				}
				linker_flags.AddOtherFlag ("-fapplication-extension");
			}

			HandleMonoNative (App, linker_flags);

			var link_task = new NativeLinkTask {
				Target = this,
				OutputFile = output_file,
				CompilerFlags = linker_flags,
			};
			link_task.AddDependency (link_with_task_output);
			link_task.AddDependency (aot_dependencies);
			build_tasks.Add (link_task);
			return link_task;
		}

		public class MonoNativeInfo {
			public bool RequireMonoNative { get; set; }

			public void Load (string filename)
			{
				using (var reader = new StreamReader (filename)) {
					string line;
					while ((line = reader.ReadLine ()) is not null) {
						if (line.Length == 0)
							continue;
						var eq = line.IndexOf ('=');
						var typestr = line.Substring (0, eq);
						var valstr = line.Substring (eq + 1);
						bool value = Convert.ToBoolean (valstr);
						switch (typestr) {
						case "RequireMonoNative":
							RequireMonoNative = value;
							break;
						default:
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"invalid type string while loading cached Mono.Native info: {typestr}");
						}
					}
				}
			}

			public void Save (string filename)
			{
				using (var writer = new StreamWriter (filename)) {
					writer.WriteLine ("RequireMonoNative={0}", RequireMonoNative);
				}
			}
		}

		MonoNativeInfo mono_native_info;

		public MonoNativeInfo MonoNative {
			get {
				if (mono_native_info is not null)
					return mono_native_info;

				mono_native_info = new MonoNativeInfo ();
				var cache_location = Path.Combine (App.Cache.Location, "mono-native-info.txt");
				if (cached_link) {
					mono_native_info.Load (cache_location);
				} else {
					mono_native_info.RequireMonoNative = LinkContext?.RequireMonoNative ?? true;
					mono_native_info.Save (cache_location);
				}

				return mono_native_info;
			}
		}

		void HandleMonoNative (Application app, CompilerFlags compiler_flags)
		{
			if (app.MonoNativeMode == MonoNativeMode.None)
				return;
			if (!MonoNative.RequireMonoNative)
				return;
			var libnative = app.GetLibNativeName ();
			var libdir = Driver.GetMonoLibraryDirectory (app);
			Driver.Log (3, "Adding mono-native library {0} for {1}.", libnative, app);
			switch (app.LibMonoNativeLinkMode) {
			case AssemblyBuildTarget.DynamicLibrary:
				libnative = Path.Combine (libdir, libnative + ".dylib");
				compiler_flags.AddLinkWith (libnative);
				break;
			case AssemblyBuildTarget.StaticObject:
				libnative = Path.Combine (libdir, libnative + ".a");
				compiler_flags.AddLinkWith (libnative);
				switch (app.Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.MacCatalyst:
					Driver.Log (3, "Adding GSS framework reference.");
					compiler_flags.AddFramework ("GSS");
					break;
				}
				break;
			default:
				throw ErrorHelper.CreateError (100, Errors.MT0100, app.LibMonoLinkMode);
			}
		}

		public void AdjustDylibs (string output)
		{
			var sb = new List<string> ();
			foreach (var dependency in Xamarin.MachO.GetNativeDependencies (output)) {
				if (!dependency.StartsWith ("/System/Library/PrivateFrameworks/", StringComparison.Ordinal))
					continue;
				var fixed_dep = dependency.Replace ("/PrivateFrameworks/", "/Frameworks/");
				sb.Add ("-change");
				sb.Add (dependency);
				sb.Add (fixed_dep);
			}
			if (sb.Count > 0) {
				sb.Add (output);
				Driver.RunInstallNameTool (App, sb);
				sb.Clear ();
			}
		}

		public bool CanWeSymlinkTheApplication ()
		{
			if (!Driver.CanWeSymlinkTheApplication (App))
				return false;

			foreach (var a in Assemblies)
				if (!a.CanSymLinkForApplication ())
					return false;

			return true;
		}

		public void Symlink ()
		{
			foreach (var a in Assemblies)
				a.Symlink ();

			if (Abis.Count != 1)
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"expected exactly one abi for a simulator architecture, found: {string.Join (", ", Abis.Select ((v) => v.ToString ()))}");

			var targetExecutable = Executables.Values.First ();

			try {
				var launcher = new StringBuilder ();
				launcher.Append (Path.Combine (Driver.GetFrameworkBinDirectory (App), "simlauncher"));
				if (Is32Build)
					launcher.Append ("32");
				else if (Is64Build)
					launcher.Append ("64");
				launcher.Append ("-sgen");
				if (Directory.Exists (targetExecutable))
					throw new ArgumentException ($"{targetExecutable} is a directory.");
				else
					File.Delete (targetExecutable);
				File.Copy (launcher.ToString (), targetExecutable);
				File.SetLastWriteTime (targetExecutable, DateTime.Now);
			} catch (ProductException) {
				throw;
			} catch (Exception ex) {
				throw new ProductException (1015, true, ex, Errors.MT1015, targetExecutable, ex.Message);
			}

			Symlinked = true;

			if (App.MonoNativeMode != MonoNativeMode.None) {
				var lib_native_target = Path.Combine (TargetDirectory, "libmono-native.dylib");

				var lib_native_name = App.GetLibNativeName () + ".dylib";
				var lib_native_path = Path.Combine (Driver.GetMonoLibraryDirectory (App), lib_native_name);
				Application.UpdateFile (lib_native_path, lib_native_target);
				Driver.Log (3, "Added mono-native library {0} for {1}.", lib_native_name, App.MonoNativeMode);
			}

			if (Driver.Verbosity > 0)
				Console.WriteLine ("Application ({0}) was built using fast-path for simulator.", string.Join (", ", Abis.ToArray ()));
		}
	}
}
