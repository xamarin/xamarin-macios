using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Bundler;

namespace Xamarin.Utils {
	public class CompilerFlags {
		public Application Application { get { return Target.App; } }
		public Target Target;

		public HashSet<string> Frameworks; // if a file, "-F /path/to/X --framework X" and added to Inputs, otherwise "--framework X".
		public HashSet<string> WeakFrameworks;
		public HashSet<string> LinkWithLibraries; // X, added to Inputs
		public HashSet<string> ForceLoadLibraries; // -force_load X, added to Inputs
		public HashSet<string []> OtherFlags; // X
		public List<string> InitialOtherFlags; // same as OtherFlags, only that they're the first argument(s) to clang (because order matters!). This is a list to preserve order (fifo).

		public HashSet<string> Defines; // -DX
		public HashSet<string> UnresolvedSymbols; // -u X
		public HashSet<string> SourceFiles; // X, added to Inputs

		// Here we store a list of all the file-system based inputs
		// to the compiler. This is used when determining if the
		// compiler needs to be called in the first place (dependency
		// tracking).
		public List<string> Inputs;

		public CompilerFlags (Target target)
		{
			if (target is null)
				throw new ArgumentNullException (nameof (target));
			this.Target = target;
		}

		public HashSet<string> AllLibraries {
			get {
				var rv = new HashSet<string> ();
				if (LinkWithLibraries is not null)
					rv.UnionWith (LinkWithLibraries);
				if (ForceLoadLibraries is not null)
					rv.UnionWith (ForceLoadLibraries);
				return rv;
			}
		}

		public void ReferenceSymbols (IEnumerable<Symbol> symbols, Abi abi)
		{
			if (UnresolvedSymbols is null)
				UnresolvedSymbols = new HashSet<string> ();

			foreach (var symbol in symbols) {
				if (symbol.ValidAbis.HasValue && (symbol.ValidAbis.Value & abi) == 0)
					continue;
				UnresolvedSymbols.Add (symbol.Prefix + symbol.Name);
			}
		}

		public void AddDefine (string define)
		{
			if (Defines is null)
				Defines = new HashSet<string> ();
			Defines.Add (define);
		}

		public void AddLinkWith (string library, bool force_load = false)
		{
			if (LinkWithLibraries is null)
				LinkWithLibraries = new HashSet<string> ();
			if (ForceLoadLibraries is null)
				ForceLoadLibraries = new HashSet<string> ();

			if (force_load) {
				ForceLoadLibraries.Add (library);
			} else {
				LinkWithLibraries.Add (library);
			}
		}

		public void AddLinkWith (IEnumerable<string> libraries, bool force_load = false)
		{
			if (libraries is null)
				return;

			foreach (var lib in libraries)
				AddLinkWith (lib, force_load);
		}

		public void AddSourceFile (string file)
		{
			if (SourceFiles is null)
				SourceFiles = new HashSet<string> ();
			SourceFiles.Add (file);
		}

		public void AddStandardCppLibrary ()
		{
			if (Driver.XcodeVersion.Major < 10)
				return;
			// Xcode 10 doesn't ship with libstdc++, so use libc++ instead.
			AddOtherFlag ("-stdlib=libc++");
		}

		public void AddOtherInitialFlag (string flag)
		{
			if (InitialOtherFlags is null)
				InitialOtherFlags = new List<string> ();
			InitialOtherFlags.Add (flag);
		}

		public void AddOtherFlag (IList<string> flags)
		{
			if (flags is null)
				return;
			AddOtherFlag ((string []) flags.ToArray ());
		}

		public void AddOtherFlag (params string [] flags)
		{
			if (flags is null || flags.Length == 0)
				return;
			if (OtherFlags is null)
				OtherFlags = new HashSet<string []> ();
			OtherFlags.Add (flags);
		}

		public void LinkWithMono ()
		{
			var mode = Target.App.LibMonoLinkMode;
			switch (mode) {
			case AssemblyBuildTarget.DynamicLibrary:
			case AssemblyBuildTarget.StaticObject:
				AddLinkWith (Application.GetLibMono (mode));
				break;
			case AssemblyBuildTarget.Framework:
				AddFramework (Application.GetLibMono (mode));
				break;
			default:
				throw ErrorHelper.CreateError (100, Errors.MT0100, mode);
			}
			AddOtherFlag ("-lz");
			AddOtherFlag ("-liconv");
		}

		public void LinkWithXamarin ()
		{
			var mode = Target.App.LibXamarinLinkMode;
			switch (mode) {
			case AssemblyBuildTarget.DynamicLibrary:
			case AssemblyBuildTarget.StaticObject:
				AddLinkWith (Application.GetLibXamarin (mode));
				break;
			case AssemblyBuildTarget.Framework:
				AddFramework (Application.GetLibXamarin (mode));
				break;
			default:
				throw ErrorHelper.CreateError (100, Errors.MT0100, mode);
			}
			AddFramework ("Foundation");
			AddOtherFlag ("-lz");
			if (Application.Platform != ApplePlatform.WatchOS && Application.Platform != ApplePlatform.TVOS)
				AddFramework ("CFNetwork"); // required by xamarin_start_wwan
		}

		public void AddFramework (string framework)
		{
			if (Frameworks is null)
				Frameworks = new HashSet<string> ();
			Frameworks.Add (framework);
		}

		public void AddFrameworks (IEnumerable<string> frameworks, IEnumerable<string> weak_frameworks)
		{
			if (frameworks is not null) {
				if (Frameworks is null)
					Frameworks = new HashSet<string> ();
				Frameworks.UnionWith (frameworks);
			}

			if (weak_frameworks is not null) {
				if (WeakFrameworks is null)
					WeakFrameworks = new HashSet<string> ();
				WeakFrameworks.UnionWith (weak_frameworks);
			}
		}

		public void Prepare ()
		{
			// Check for system frameworks that are only available in newer iOS versions,
			// (newer than the deployment target), in which case those need to be weakly linked.
			if (Frameworks is not null) {
				if (WeakFrameworks is null)
					WeakFrameworks = new HashSet<string> ();

				foreach (var fwk in Frameworks) {
					if (!fwk.EndsWith (".framework", StringComparison.Ordinal)) {
						var add_to = WeakFrameworks;
						var framework = Driver.GetFrameworks (Application).Find (fwk);
						if (framework is not null) {
							if (framework.Version > Application.SdkVersion)
								continue;
							add_to = Application.DeploymentTarget >= framework.Version ? Frameworks : WeakFrameworks;
						}
						add_to.Add (fwk);
					} else {
						// believe what we got about user frameworks.
					}
				}

				// Make sure frameworks aren't duplicated, favoring any weak frameworks.
				Frameworks.ExceptWith (WeakFrameworks);
			}

			// force_load libraries take precedence, so remove the libraries
			// we need to force load from the list of libraries we just load.
			if (LinkWithLibraries is not null)
				LinkWithLibraries.ExceptWith (ForceLoadLibraries);
		}

		void AddInput (string file)
		{
			if (Inputs is null)
				return;

			Inputs.Add (file);
		}

		public void WriteArguments (IList<string> args)
		{
			Prepare ();

			if (InitialOtherFlags is not null) {
				var idx = 0;
				foreach (var flag in InitialOtherFlags) {
					args.Insert (idx, flag);
					idx++;
				}
			}

			// check if needs to be removed: https://github.com/xamarin/xamarin-macios/issues/18693
			if (Driver.XcodeVersion.Major >= 15 && !Application.DisableAutomaticLinkerSelection) {
				args.Insert (0, "-Xlinker");
				args.Insert (1, "-ld_classic");
			}

			ProcessFrameworksForArguments (args);

			if (LinkWithLibraries is not null) {
				foreach (var lib in LinkWithLibraries) {
					args.Add (lib);
					AddInput (lib);
				}
			}

			if (ForceLoadLibraries is not null) {
				foreach (var lib in ForceLoadLibraries) {
					args.Add ("-force_load");
					args.Add (lib);
					AddInput (lib);
				}
			}

			if (OtherFlags is not null) {
				foreach (var flags in OtherFlags) {
					foreach (var flag in flags)
						args.Add (flag);
				}
			}

			if (Defines is not null) {
				foreach (var define in Defines) {
					args.Add ("-D");
					args.Add (define);
				}
			}

			if (UnresolvedSymbols is not null) {
				foreach (var symbol in UnresolvedSymbols) {
					args.Add ("-u");
					args.Add (symbol);
				}
			}

			if (SourceFiles is not null) {
				foreach (var src in SourceFiles) {
					args.Add (src);
					AddInput (src);
				}
			}
		}

		void ProcessFrameworksForArguments (IList<string> args)
		{
			bool any_user_framework = false;

			if (Frameworks is not null) {
				foreach (var fw in Frameworks)
					ProcessFrameworkForArguments (args, fw, false, ref any_user_framework);
			}

			if (WeakFrameworks is not null) {
				foreach (var fw in WeakFrameworks)
					ProcessFrameworkForArguments (args, fw, true, ref any_user_framework);
			}

			if (any_user_framework) {
				args.Add ("-Xlinker");
				args.Add ("-rpath");
				args.Add ("-Xlinker");
				switch (Application.Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					args.Add ("@executable_path/Frameworks");
					break;
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					args.Add ("@executable_path/../Frameworks");
					break;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Application.Platform, Application.ProductName);
				}
				if (Application.IsExtension) {
					args.Add ("-Xlinker");
					args.Add ("-rpath");
					args.Add ("-Xlinker");
					args.Add ("@executable_path/../../Frameworks");
				}
			}

			if (Application.HasAnyDynamicLibraries) {
				args.Add ("-Xlinker");
				args.Add ("-rpath");
				args.Add ("-Xlinker");
				args.Add ("@executable_path/");
				if (Application.IsExtension) {
					args.Add ("-Xlinker");
					args.Add ("-rpath");
					args.Add ("-Xlinker");
					args.Add ("@executable_path/../..");
				}
			}
		}

		void ProcessFrameworkForArguments (IList<string> args, string fw, bool is_weak, ref bool any_user_framework)
		{
			var name = Path.GetFileNameWithoutExtension (fw);
			if (fw.EndsWith (".framework", StringComparison.Ordinal)) {
				// user framework, we need to pass -F to the linker so that the linker finds the user framework.
				any_user_framework = true;
				AddInput (Path.Combine (fw, name));
				args.Add ("-F");
				args.Add (Path.GetDirectoryName (fw));
			}
			args.Add (is_weak ? "-weak_framework" : "-framework");
			args.Add (name);
		}

		public string [] ToArray ()
		{
			var args = new List<string> ();
			WriteArguments (args);
			return args.ToArray ();
		}

		public override string ToString ()
		{
			return string.Join (" ", ToArray ());
		}

		public void PopulateInputs ()
		{
			var args = new List<string> ();
			Inputs = new List<string> ();
			WriteArguments (args);
		}
	}
}
