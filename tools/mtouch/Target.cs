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

using XamCore.Registrar;

namespace Xamarin.Bundler
{
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

		// Note that each 'Target' can have multiple abis: armv7+armv7s for instance.
		public List<Abi> Abis;

		// This is a list of native libraries to into the final executable.
		// All the native libraries are included here (this means all the libraries
		// that were AOTed from managed code, main, registrar, extra static libraries, etc).
		List<string> link_with = new List<string> ();

		// If we didn't link because the existing (cached) assemblyes are up-to-date.
		bool cached_link;

		// If any assemblies were updated (only set to false if the linker is disabled and no assemblies were modified).
		bool any_assembly_updated = true;

		BuildTasks compile_tasks = new BuildTasks ();

		// If we didn't link the final executable because the existing binary is up-to-date.
		public bool cached_executable; 

		// If the assemblies were symlinked.
		public bool Symlinked;

		public bool Is32Build { get { return Application.IsArchEnabled (Abis, Abi.Arch32Mask); } } // If we're targetting a 32 bit arch for this target.
		public bool Is64Build { get { return Application.IsArchEnabled (Abis, Abi.Arch64Mask); } } // If we're targetting a 64 bit arch for this target.

		List<string> link_with_and_ship = new List<string> ();
		public IEnumerable<string> LibrariesToShip { get { return link_with_and_ship; } }

		PInvokeWrapperGenerator pinvoke_state;
		PInvokeWrapperGenerator MarshalNativeExceptionsState {
			get {
				if (!App.RequiresPInvokeWrappers)
					return null;

				if (pinvoke_state == null) {
					pinvoke_state = new PInvokeWrapperGenerator ()
					{
						App = App,
						SourcePath = Path.Combine (ArchDirectory, "pinvokes.m"),
						HeaderPath = Path.Combine (ArchDirectory, "pinvokes.h"),
						Registrar = (StaticRegistrar) StaticRegistrar,
					};
				}

				return pinvoke_state;
			}
		}

		public string Executable {
			get {
				return Path.Combine (TargetDirectory, App.ExecutableName);
			}
		}

		public void Initialize (bool show_warnings)
		{
			// we want to load our own mscorlib[-runtime].dll, not something else we're being feeded
			// (e.g. bug #6612) since it might not match the libmono[-sgen].a library we'll link with,
			// so load the corlib we want first.

			var corlib_path = Path.Combine (Resolver.FrameworkDirectory, "mscorlib.dll");
			var corlib = ManifestResolver.Load (corlib_path);
			if (corlib == null)
				throw new MonoTouchException (2006, true, "Can not load mscorlib.dll from: '{0}'. Please reinstall Xamarin.iOS.", corlib_path);

			foreach (var reference in App.References) {
				var ad = ManifestResolver.Load (reference);
				if (ad == null)
					throw new MonoTouchException (2002, true, "Can not resolve reference: {0}", reference);
				if (ad.MainModule.Runtime > TargetRuntime.Net_4_0)
					ErrorHelper.Show (new MonoTouchException (11, false, "{0} was built against a more recent runtime ({1}) than Xamarin.iOS supports.", Path.GetFileName (reference), ad.MainModule.Runtime));

				// Figure out if we're referencing Xamarin.iOS or monotouch.dll
				if (Path.GetFileNameWithoutExtension (ad.MainModule.FileName) == Driver.GetProductAssembly (App))
					ProductAssembly = ad;
			}

			ComputeListOfAssemblies ();

			if (App.LinkMode == LinkMode.None && App.I18n != I18nAssemblies.None)
				AddI18nAssemblies ();

			// an extension is a .dll and it would match itself
			if (App.IsExtension)
				return;

			var root_wo_ext = Path.GetFileNameWithoutExtension (App.RootAssembly);
			foreach (var assembly in Assemblies) {
				if (!assembly.FullPath.EndsWith (".exe", StringComparison.OrdinalIgnoreCase)) {
					if (root_wo_ext == Path.GetFileNameWithoutExtension (assembly.FullPath))
						throw new MonoTouchException (23, true, "Application name '{0}.exe' conflicts with another user assembly.", root_wo_ext);
				}
			}
		}

		// This is to load the symbols for all assemblies, so that we can give better error messages
		// (with file name / line number information).
		public void LoadSymbols ()
		{
			foreach (var a in Assemblies)
				a.LoadSymbols ();
		}

		IEnumerable<AssemblyDefinition> GetAssemblies ()
		{
			if (App.LinkMode == LinkMode.None)
				return ManifestResolver.GetAssemblies ();

			List<AssemblyDefinition> assemblies = new List<AssemblyDefinition> ();
			if (LinkContext == null) {
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

		public void ComputeLinkerFlags ()
		{
			foreach (var a in Assemblies)
				a.ComputeLinkerFlags ();

			if (App.Platform != ApplePlatform.WatchOS && App.Platform != ApplePlatform.TVOS)
				Frameworks.Add ("CFNetwork"); // required by xamarin_start_wwan
		}

		Dictionary<string, List<MemberReference>> entry_points;
		public IDictionary<string, List<MemberReference>> GetEntryPoints ()
		{
			if (entry_points == null)
				GetRequiredSymbols ();
			return entry_points;
		}

		public IEnumerable<string> GetRequiredSymbols ()
		{
			if (entry_points != null)  
				return entry_points.Keys;

			var cache_location = Path.Combine (App.Cache.Location, "entry-points.txt");
			if (cached_link || !any_assembly_updated) {
				entry_points = new Dictionary<string, List<MemberReference>> ();
				foreach (var ep in File.ReadAllLines (cache_location))
					entry_points.Add (ep, null);
			} else {
				List<MethodDefinition> marshal_exception_pinvokes;
				if (LinkContext == null) {
					// This happens when using the simlauncher and the msbuild tasks asked for a list
					// of symbols (--symbollist). In that case just produce an empty list, since the
					// binary shouldn't end up stripped anyway.
					entry_points = new Dictionary<string, List<MemberReference>> ();
					marshal_exception_pinvokes = new List<MethodDefinition> ();
				} else {
					entry_points = LinkContext.RequiredSymbols;
					marshal_exception_pinvokes = LinkContext.MarshalExceptionPInvokes;
				}
				
				// keep the debugging helper in debugging binaries only
				if (App.EnableDebug && !App.EnableBitCode)
					entry_points.Add ("mono_pmip", null);

				if (App.IsSimulatorBuild) {
					entry_points.Add ("xamarin_dyn_objc_msgSend", null);
					entry_points.Add ("xamarin_dyn_objc_msgSendSuper", null);
					entry_points.Add ("xamarin_dyn_objc_msgSend_stret", null);
					entry_points.Add ("xamarin_dyn_objc_msgSendSuper_stret", null);
				}

				File.WriteAllText (cache_location, string.Join ("\n", entry_points.Keys.ToArray ()));
			}
			return entry_points.Keys;
		}

		public IEnumerable<string> GetRequiredSymbols (Assembly assembly, bool includeObjectiveCClasses)
		{
			if (entry_points == null)
				GetRequiredSymbols ();

			foreach (var ep in entry_points) {
				if (ep.Value == null)
					continue;
				foreach (var mr in ep.Value) {
					if (mr.Module.Assembly == assembly.AssemblyDefinition)
						yield return ep.Key;
				}
			}

			if (includeObjectiveCClasses) {
				foreach (var kvp in LinkContext.ObjectiveCClasses) {
					if (kvp.Value.Module.Assembly == assembly.AssemblyDefinition)
						yield return $"OBJC_CLASS_$_{kvp.Key}";
				}
			}
		}

		public List<MemberReference> GetMembersForSymbol (string symbol)
		{
			List<MemberReference> rv = null;
			entry_points?.TryGetValue (symbol, out rv);
			return rv;
		}

		//
		// Gets a flattened list of all the assemblies pulled by the root assembly
		//
		public void ComputeListOfAssemblies ()
		{
			var exceptions = new List<Exception> ();
			var assemblies = new HashSet<string> ();

			try {
				var assembly = ManifestResolver.Load (App.RootAssembly);
				ComputeListOfAssemblies (assemblies, assembly, exceptions);
			} catch (MonoTouchException mte) {
				exceptions.Add (mte);
			} catch (Exception e) {
				exceptions.Add (new MonoTouchException (9, true, e, "Error while loading assemblies: {0}", e.Message));
			}

			if (App.LinkMode == LinkMode.None)
				exceptions.AddRange (ManifestResolver.list);

			if (exceptions.Count > 0)
				throw new AggregateException (exceptions);
		}

		void ComputeListOfAssemblies (HashSet<string> assemblies, AssemblyDefinition assembly, List<Exception> exceptions)
		{
			if (assembly == null)
				return;

			var fqname = assembly.MainModule.FileName;
			if (assemblies.Contains (fqname))
				return;

			assemblies.Add (fqname);

			var asm = new Assembly (this, assembly);
			asm.ComputeSatellites ();
			this.Assemblies.Add (asm);

			var main = assembly.MainModule;
			foreach (AssemblyNameReference reference in main.AssemblyReferences) {
				// Verify that none of the references references an incorrect platform assembly.
				switch (reference.Name) {
				case "monotouch":
				case "Xamarin.iOS":
				case "Xamarin.TVOS":
				case "Xamarin.WatchOS":
					if (reference.Name != Driver.GetProductAssembly (App))
						exceptions.Add (ErrorHelper.CreateError (34, "Cannot reference '{0}.dll' in a {1} project - it is implicitly referenced by '{2}'.", reference.Name, Driver.TargetFramework.Identifier, assembly.FullName));
					break;
				}

				var reference_assembly = ManifestResolver.Resolve (reference);
				ComputeListOfAssemblies (assemblies, reference_assembly, exceptions);
			}

			// Custom Attribute metadata can include references to other assemblies, e.g. [X (typeof (Y)], 
			// but it is not reflected in AssemblyReferences :-( ref: #37611
			// so we must scan every custom attribute to look for System.Type
			GetCustomAttributeReferences (assembly, assemblies, exceptions);
			GetCustomAttributeReferences (main, assemblies, exceptions);
			if (main.HasTypes) {
				foreach (var ca in main.GetCustomAttributes ())
					GetCustomAttributeReferences (ca, assemblies, exceptions);
			}
		}

		void GetCustomAttributeReferences (ICustomAttributeProvider cap, HashSet<string> assemblies, List<Exception> exceptions)
		{
			if (!cap.HasCustomAttributes)
				return;
			foreach (var ca in cap.CustomAttributes)
				GetCustomAttributeReferences (ca, assemblies, exceptions);
		}

		void GetCustomAttributeReferences (CustomAttribute ca, HashSet<string> assemblies, List<Exception> exceptions)
		{
			if (ca.HasConstructorArguments) {
				foreach (var arg in ca.ConstructorArguments)
					GetCustomAttributeArgumentReference (arg, assemblies, exceptions);
			}
			if (ca.HasFields) {
				foreach (var arg in ca.Fields)
					GetCustomAttributeArgumentReference (arg.Argument, assemblies, exceptions);
			}
			if (ca.HasProperties) {
				foreach (var arg in ca.Properties)
					GetCustomAttributeArgumentReference (arg.Argument, assemblies, exceptions);
			}
		}

		void GetCustomAttributeArgumentReference (CustomAttributeArgument arg, HashSet<string> assemblies, List<Exception> exceptions)
		{
			if (!arg.Type.Is ("System", "Type"))
				return;
			var ar = (arg.Value as TypeReference)?.Scope as AssemblyNameReference;
			if (ar == null)
				return;
			var reference_assembly = ManifestResolver.Resolve (ar);
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

		public void LinkAssemblies (string main, out List<AssemblyDefinition> assemblies, string output_dir, out MonoTouchLinkContext link_context)
		{
			if (Driver.Verbosity > 0)
				Console.WriteLine ("Linking {0} into {1} using mode '{2}'", main, output_dir, App.LinkMode);

			var cache = Resolver.ToResolverCache ();
			var resolver = cache != null
				? new AssemblyResolver (cache)
				: new AssemblyResolver ();

			resolver.AddSearchDirectory (Resolver.RootDirectory);
			resolver.AddSearchDirectory (Resolver.FrameworkDirectory);

			LinkerOptions = new LinkerOptions {
				MainAssembly = Resolver.Load (main),
				OutputDirectory = output_dir,
				LinkMode = App.LinkMode,
				Resolver = resolver,
				SkippedAssemblies = App.LinkSkipped,
				I18nAssemblies = App.I18n,
				LinkSymbols = true,
				LinkAway = App.LinkAway,
				ExtraDefinitions = App.Definitions,
				Device = App.IsDeviceBuild,
				// by default we keep the code to ensure we're executing on the UI thread (for UI code) for debug builds
				// but this can be overridden to either (a) remove it from debug builds or (b) keep it in release builds
				EnsureUIThread = App.ThreadCheck.HasValue ? App.ThreadCheck.Value : App.EnableDebug,
				DebugBuild = App.EnableDebug,
				Arch = Is64Build ? 8 : 4,
				IsDualBuild = App.IsDualBuild,
				DumpDependencies = App.LinkerDumpDependencies,
				RuntimeOptions = App.RuntimeOptions,
				MarshalNativeExceptionsState = MarshalNativeExceptionsState,
				Target = this,
			};

			MonoTouch.Tuner.Linker.Process (LinkerOptions, out link_context, out assemblies);

			Driver.Watch ("Link Assemblies", 1);
		}

		public void ManagedLink ()
		{
			var cache_path = Path.Combine (ArchDirectory, "linked-assemblies.txt");

			foreach (var a in Assemblies)
				a.CopyToDirectory (LinkDirectory, false, check_case: true);

			// Check if we can use a previous link result.
			if (!Driver.Force) {
				var input = new List<string> ();
				var output = new List<string> ();
				var cached_output = new List<string> ();

				if (File.Exists (cache_path)) {
					cached_output.AddRange (File.ReadAllLines (cache_path));

					var cached_loaded = new HashSet<string> ();
					// Only add the previously linked assemblies (and their satellites) as the input/output assemblies.
					// Do not add assemblies which the linker process removed.
					foreach (var a in Assemblies) {
						if (!cached_output.Contains (a.FullPath))
							continue;
						cached_loaded.Add (a.FullPath);
						input.Add (a.FullPath);
						output.Add (Path.Combine (PreBuildDirectory, a.FileName));
						if (File.Exists (a.FullPath + ".mdb")) {
							// Debug files can change without the assemblies themselves changing
							// This should also invalidate the cached linker results, since the non-linked mdbs can't be copied.
							input.Add (a.FullPath + ".mdb");
							output.Add (Path.Combine (PreBuildDirectory, a.FileName) + ".mdb");
						}
						
						if (a.Satellites != null) {
							foreach (var s in a.Satellites) {
								input.Add (s);
								output.Add (Path.Combine (PreBuildDirectory, Path.GetFileName (Path.GetDirectoryName (s)), Path.GetFileName (s)));
								// No need to copy satellite mdb files, satellites are resource-only assemblies.
							}
						}
					}

					// The linker might have added assemblies that weren't specified/reachable
					// from the command line arguments (such as I18N assemblies). Those are not
					// in the Assemblies list at this point (since we haven't run the linker yet)
					// so make sure we take those into account as well.
					var not_loaded = cached_output.Except (cached_loaded);
					foreach (var path in not_loaded) {
						input.Add (path);
						output.Add (Path.Combine (PreBuildDirectory, Path.GetFileName (path)));
					}

					// Include mtouch here too?
					// input.Add (Path.Combine (MTouch.MonoTouchDirectory, "usr", "bin", "mtouch"));

					if (Application.IsUptodate (input, output)) {
						cached_link = true;
						foreach (var a in Assemblies.ToList ()) {
							if (!cached_output.Contains (a.FullPath)) {
								Assemblies.Remove (a);
								continue;
							}
							// Load the cached assembly
							a.LoadAssembly (Path.Combine (PreBuildDirectory, a.FileName));
							Driver.Log (3, "Target '{0}' is up-to-date.", a.FullPath);
						}

						foreach (var path in not_loaded) {
							var a = new Assembly (this, path);
							a.LoadAssembly (Path.Combine (PreBuildDirectory, a.FileName));
							Assemblies.Add (a);
						}

						Driver.Watch ("Cached assemblies reloaded", 1);
						Driver.Log ("Cached assemblies reloaded.");

						return;
					}
				}
			}

			// Load the assemblies into memory.
			foreach (var a in Assemblies)
				a.LoadAssembly (a.FullPath);

			List<AssemblyDefinition> linked_assemblies_definitions;

			LinkAssemblies (App.RootAssembly, out linked_assemblies_definitions, PreBuildDirectory, out LinkContext);

			// Update (add/remove) the assemblies, since the linker may have both added and removed assemblies.
			Assemblies.Update (this, linked_assemblies_definitions);

			// Make the assemblies point to the right path.
			foreach (var a in Assemblies) {
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
				Driver.Touch (a.GetRelatedFiles ());
			}

			List<string> linked_assemblies = linked_assemblies_definitions.Select ((v) => v.MainModule.FileName).ToList ();
			File.WriteAllText (cache_path, string.Join ("\n", linked_assemblies));
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

			LinkDirectory = Path.Combine (ArchDirectory, "Link");
			if (!Directory.Exists (LinkDirectory))
				Directory.CreateDirectory (LinkDirectory);

			PreBuildDirectory = Path.Combine (ArchDirectory, "PreBuild");
			if (!Directory.Exists (PreBuildDirectory))
				Directory.CreateDirectory (PreBuildDirectory);
			
			BuildDirectory = Path.Combine (ArchDirectory, "Build");
			if (!Directory.Exists (BuildDirectory))
				Directory.CreateDirectory (BuildDirectory);

			if (!Directory.Exists (TargetDirectory))
				Directory.CreateDirectory (TargetDirectory);

			ManagedLink ();

			CompilePInvokeWrappers ();
			// Now the assemblies are in PreBuildDirectory.

			foreach (var a in Assemblies) {
				var target = Path.Combine (BuildDirectory, a.FileName);
				if (!a.CopyAssembly (a.FullPath, target))
					Driver.Log (3, "Target '{0}' is up-to-date.", target);
				a.FullPath = target;
			}

			Driver.GatherFrameworks (this, Frameworks, WeakFrameworks);

			// Make sure there are no duplicates between frameworks and weak frameworks.
			// Keep the weak ones.
			Frameworks.ExceptWith (WeakFrameworks);
		}

		public void CompilePInvokeWrappers ()
		{
			if (!App.RequiresPInvokeWrappers)
				return;

			// Write P/Invokes
			var state = MarshalNativeExceptionsState;
			if (state.Started) {
				// The generator is 'started' by the linker, which means it may not
				// be started if the linker was not executed due to re-using cached results.
				state.End ();
			}

			var ifile = state.SourcePath;
			foreach (var abi in Abis) {
				var arch = abi.AsArchString ();
				var ext = App.FastDev ? ".dylib" : ".o";
				var ofile = Path.Combine (App.Cache.Location, arch, "lib" + Path.GetFileNameWithoutExtension (ifile) + ext);

				if (!Application.IsUptodate (ifile, ofile)) {
					var task = new PinvokesTask
					{
						Target = this,
						Abi = abi,
						InputFile = ifile,
						OutputFile = ofile,
						SharedLibrary = App.FastDev,
						Language = "objective-c++",
					};
					if (App.FastDev) {
						task.InstallName = "lib" + Path.GetFileNameWithoutExtension (ifile) + ext;
						task.CompilerFlags.AddFramework ("Foundation");
						task.CompilerFlags.LinkWithXamarin ();
					}
					compile_tasks.Add (task);
				}
				LinkWith (ofile);
				LinkWithAndShip (ofile);
			}

			if (App.FastDev) {
				// In this case assemblies must link with the resulting dylib,
				// so we can't compile the pinvoke dylib in parallel with later
				// stuff.
				compile_tasks.ExecuteInParallel ();
			}
		}

		public void SelectStaticRegistrar ()
		{
			switch (App.Registrar) {
			case RegistrarMode.Static:
			case RegistrarMode.Dynamic:
			case RegistrarMode.Default:
				StaticRegistrar = new StaticRegistrar (this)
				{
					LinkContext = LinkContext,
				};
				break;
			}
		}

		public void Compile ()
		{
			// Compute the dependency map, and show warnings if there are any problems.
			List<Exception> exceptions = new List<Exception> ();
			foreach (var a in Assemblies)
				a.ComputeDependencyMap (exceptions);
			if (exceptions.Count > 0) {
				ErrorHelper.Show (exceptions);
				ErrorHelper.Warning (3006, "Could not compute a complete dependency map for the project. This will result in slower build times because Xamarin.iOS can't properly detect what needs to be rebuilt (and what does not need to be rebuilt). Please review previous warnings for more details.");
			}

			// Compile the managed assemblies into object files or shared libraries
			if (App.IsDeviceBuild) {
				foreach (var a in Assemblies)
					a.CreateCompilationTasks (compile_tasks, BuildDirectory, Abis);
			}

			// The static registrar.
			List<string> registration_methods = null;
			if (App.Registrar == RegistrarMode.Static) {
				RunRegistrarTask run_registrar = null;
				var registrar_m = Path.Combine (ArchDirectory, "registrar.m");
				var registrar_h = Path.Combine (ArchDirectory, "registrar.h");
				if (!Application.IsUptodate (Assemblies.Select (v => v.FullPath), new string[] { registrar_m, registrar_h })) {
					run_registrar = new RunRegistrarTask
					{
						Target = this,
						RegistrarM = registrar_m,
						RegistrarH = registrar_h,
					};
					registration_methods = new List<string> ();
					registration_methods.Add ("xamarin_create_classes");
					Driver.Watch ("Registrar", 1);
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date.", registrar_m);
				}

				var compile_registrar_tasks = new List<BuildTask> ();
				foreach (var abi in Abis) {
					var arch = abi.AsArchString ();
					var ofile = Path.Combine (App.Cache.Location, arch, Path.GetFileNameWithoutExtension (registrar_m) + ".o");

					if (!Application.IsUptodate (registrar_m, ofile)) {
						var registrar_task = new CompileRegistrarTask ()
						{
							Target = this,
							Abi = abi,
							InputFile = registrar_m,
							OutputFile = ofile,
							RegistrarM = registrar_m,
							RegistrarH = registrar_h,
							SharedLibrary = false,
							Language = "objective-c++",
						};
						// This is because iOS has a forward declaration of NSPortMessage, but no actual declaration.
						// They still use NSPortMessage in other API though, so it can't just be removed from our bindings.
						registrar_task.CompilerFlags.AddOtherFlag ("-Wno-receiver-forward-class");
						compile_registrar_tasks.Add (registrar_task);
					} else {
						Driver.Log (3, "Target '{0}' is up-to-date.", ofile);
					}

					LinkWith (ofile);
				}
				if (run_registrar != null) {
					run_registrar.NextTasks = compile_registrar_tasks;
					compile_tasks.Add (run_registrar);
				} else {
					compile_tasks.AddRange (compile_registrar_tasks);
				}
			}

			if (App.Registrar == RegistrarMode.Dynamic && App.IsSimulatorBuild && App.LinkMode == LinkMode.None) {
				if (registration_methods == null)
					registration_methods = new List<string> ();

				string method;
				string library;
				switch (App.Platform) {
				case ApplePlatform.iOS:
					method = "xamarin_create_classes_Xamarin_iOS";
					library = "Xamarin.iOS.registrar.a";
					break;
				case ApplePlatform.WatchOS:
					method = "xamarin_create_classes_Xamarin_WatchOS";
					library = "Xamarin.WatchOS.registrar.a";
					break;					
				case ApplePlatform.TVOS:
					method = "xamarin_create_classes_Xamarin_TVOS";
					library = "Xamarin.TVOS.registrar.a";
					break;
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", App.Platform);
				}

				registration_methods.Add (method);
				link_with.Add (Path.Combine (Driver.GetProductSdkDirectory (App), "usr", "lib", library));
			}

			// The main method.
			foreach (var abi in Abis) {
				var arch = abi.AsArchString ();

				GenerateMainTask generate_main_task = null;
				var main_m = Path.Combine (App.Cache.Location, arch, "main.m");
				var files = Assemblies.Select (v => v.FullPath);
				if (!Application.IsUptodate (files, new string [] { main_m })) {
					generate_main_task = new GenerateMainTask
					{
						Target = this,
						Abi = abi,
						MainM = main_m,
						RegistrationMethods = registration_methods,
					};
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date.", main_m);
				}

				var main_o = Path.Combine (App.Cache.Location, arch, "main.o");
				if (!Application.IsUptodate (main_m, main_o)) {
					var main_task = new CompileMainTask ()
					{
						Target = this,
						Abi = abi,
						AssemblyName = App.AssemblyName,
						InputFile = main_m,
						OutputFile = main_o,
						SharedLibrary = false,
						Language = "objective-c++",
					};
					main_task.CompilerFlags.AddDefine ("MONOTOUCH");
					if (generate_main_task == null) {
						compile_tasks.Add (main_task);
					} else {
						generate_main_task.NextTasks = new [] { main_task };
						compile_tasks.Add (generate_main_task);
					}
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date.", main_o);
				}

				LinkWith (main_o);
			}

			// Start compiling.
			compile_tasks.ExecuteInParallel ();

			if (App.FastDev) {
				foreach (var a in Assemblies) {
					if (a.Dylibs == null)
						continue;
					foreach (var dylib in a.Dylibs)
						LinkWith (dylib);
				}
			}

			Driver.Watch ("Compile", 1);
		}

		public void NativeLink ()
		{
			if (!string.IsNullOrEmpty (App.UserGccFlags))
				App.DeadStrip = false;
			if (App.EnableLLVMOnlyBitCode)
				App.DeadStrip = false;

			var linker_flags = new CompilerFlags () { Target = this };

			// Get global frameworks
			linker_flags.AddFrameworks (App.Frameworks, App.WeakFrameworks);
			linker_flags.AddFrameworks (Frameworks, WeakFrameworks);

			// Collect all LinkWith flags and frameworks from all assemblies.
			foreach (var a in Assemblies) {
				linker_flags.AddFrameworks (a.Frameworks, a.WeakFrameworks);
				if (!App.FastDev || App.IsSimulatorBuild)
					linker_flags.AddLinkWith (a.LinkWith, a.ForceLoad);
				linker_flags.AddOtherFlags (a.LinkerFlags);
			}

			var bitcode = App.EnableBitCode;
			if (bitcode)
				linker_flags.AddOtherFlag (App.EnableMarkerOnlyBitCode ? "-fembed-bitcode-marker" : "-fembed-bitcode");
			
			if (App.EnablePie.HasValue && App.EnablePie.Value && (App.DeploymentTarget < new Version (4, 2)))
				ErrorHelper.Error (28, "Cannot enable PIE (-pie) when targeting iOS 4.1 or earlier. Please disable PIE (-pie:false) or set the deployment target to at least iOS 4.2");

			if (!App.EnablePie.HasValue)
				App.EnablePie = true;

			if (App.Platform == ApplePlatform.iOS) {
				if (App.EnablePie.Value && (App.DeploymentTarget >= new Version (4, 2))) {
					linker_flags.AddOtherFlag ("-Wl,-pie");
				} else {
					linker_flags.AddOtherFlag ("-Wl,-no_pie");
				}
			}

			CompileTask.GetArchFlags (linker_flags, Abis);
			if (App.IsDeviceBuild) {
				linker_flags.AddOtherFlag ($"-m{Driver.GetTargetMinSdkName (App)}-version-min={App.DeploymentTarget}");
				linker_flags.AddOtherFlag ($"-isysroot {Driver.Quote (Driver.GetFrameworkDirectory (App))}");
			} else {
				CompileTask.GetSimulatorCompilerFlags (linker_flags, false, App);
			}
			linker_flags.LinkWithMono ();
			linker_flags.LinkWithXamarin ();

			linker_flags.AddLinkWith (link_with);
			linker_flags.AddOtherFlag ($"-o {Driver.Quote (Executable)}");

			linker_flags.AddOtherFlag ("-lz");
			linker_flags.AddOtherFlag ("-liconv");

			bool need_libcpp = false;
			if (App.EnableBitCode)
				need_libcpp = true;
#if ENABLE_BITCODE_ON_IOS
			need_libcpp = true;
#endif
			if (need_libcpp)
				linker_flags.AddOtherFlag ("-lc++");

			// allow the native linker to remove unused symbols (if the caller was removed by the managed linker)
			if (!bitcode) {
				// Note that we include *all* (__Internal) p/invoked symbols here
				// We also include any fields from [Field] attributes.
				linker_flags.ReferenceSymbols (GetRequiredSymbols ());
			}

			string mainlib;
			if (App.IsWatchExtension) {
				mainlib = "libwatchextension.a";
				linker_flags.AddOtherFlag (" -e _xamarin_watchextension_main");
			} else if (App.IsTVExtension) {
				mainlib = "libtvextension.a";
			} else if (App.IsExtension) {
				mainlib = "libextension.a";
			} else {
				mainlib = "libapp.a";
			}
			var libdir = Path.Combine (Driver.GetProductSdkDirectory (App), "usr", "lib");
			var libmain = Path.Combine (libdir, mainlib);
			linker_flags.AddLinkWith (libmain, true);

			if (App.EnableProfiling) {
				string libprofiler;
				if (App.FastDev) {
					libprofiler = Path.Combine (libdir, "libmono-profiler-log.dylib");
					linker_flags.AddLinkWith (libprofiler);
				} else {
					libprofiler = Path.Combine (libdir, "libmono-profiler-log.a");
					linker_flags.AddLinkWith (libprofiler);
					if (!App.EnableBitCode)
						linker_flags.ReferenceSymbol ("mono_profiler_startup_log");
				}
			}

			if (!string.IsNullOrEmpty (App.UserGccFlags))
				linker_flags.AddOtherFlag (App.UserGccFlags);

			if (App.DeadStrip)
				linker_flags.AddOtherFlag ("-dead_strip");

			if (App.IsExtension) {
				if (App.Platform == ApplePlatform.iOS && Driver.XcodeVersion.Major < 7) {
					linker_flags.AddOtherFlag ("-lpkstart");
					linker_flags.AddOtherFlag ($"-F {Driver.Quote (Path.Combine (Driver.GetFrameworkDirectory (App), "System/Library/PrivateFrameworks"))} -framework PlugInKit");
				}
				linker_flags.AddOtherFlag ("-fapplication-extension");
			}

			linker_flags.Inputs = new List<string> ();
			var flags = linker_flags.ToString (); // This will populate Inputs.

			if (!Application.IsUptodate (linker_flags.Inputs, new string [] { Executable } )) {
				var linker_task = new NativeLinkTask
				{
					Target = this,
					OutputFile = Executable,
					CompilerFlags = linker_flags,
				};
				linker_task.Link ();
			} else {
				cached_executable = true;
				Driver.Log (3, "Target '{0}' is up-to-date.", Executable);
			}
		}

		public void AdjustDylibs ()
		{
			var sb = new StringBuilder ();
			foreach (var dependency in Xamarin.MachO.GetNativeDependencies (Executable)) {
				if (!dependency.StartsWith ("/System/Library/PrivateFrameworks/", StringComparison.Ordinal))
					continue;
				var fixed_dep = dependency.Replace ("/PrivateFrameworks/", "/Frameworks/");
				sb.Append (" -change ").Append (dependency).Append (' ').Append (fixed_dep);
			}
			if (sb.Length > 0) {
				var quoted_name = Driver.Quote (Executable);
				sb.Append (' ').Append (quoted_name);
				Driver.XcodeRun ("install_name_tool", sb.ToString ());
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

			var targetExecutable = Executable;

			Application.TryDelete (targetExecutable);

			try {
				var launcher = new StringBuilder ();
				launcher.Append (Path.Combine (Driver.MonoTouchDirectory, "bin", "simlauncher"));
				if (Is32Build)
					launcher.Append ("32");
				else if (Is64Build)
					launcher.Append ("64");
				launcher.Append ("-sgen");
				File.Copy (launcher.ToString (), Executable);
				File.SetLastWriteTime (Executable, DateTime.Now);
			} catch (MonoTouchException) {
				throw;
			} catch (Exception ex) {
				throw new MonoTouchException (1015, true, ex, "Failed to create the executable '{0}': {1}", targetExecutable, ex.Message);
			}

			Symlinked = true;

			if (Driver.Verbosity > 0)
				Console.WriteLine ("Application ({0}) was built using fast-path for simulator.", string.Join (", ", Abis.ToArray ()));
		}

		// Thread-safe
		public void LinkWith (string native_library)
		{
			lock (link_with)
				link_with.Add (native_library);
		}

		public void LinkWithAndShip (string dylib)
		{
			link_with_and_ship.Add (dylib);
		}
	}
}
