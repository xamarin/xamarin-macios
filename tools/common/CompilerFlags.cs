using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Bundler;

namespace Xamarin.Utils
{
	public class CompilerFlags
	{
		public Application Application { get { return Target.App; } }
		public Target Target;

		public HashSet<string> Frameworks; // if a file, "-F /path/to/X --framework X" and added to Inputs, otherwise "--framework X".
		public HashSet<string> WeakFrameworks;
		public HashSet<string> LinkWithLibraries; // X, added to Inputs
		public HashSet<string> ForceLoadLibraries; // -force_load X, added to Inputs
		public HashSet<string> OtherFlags; // X
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
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			this.Target = target;
		}

		public HashSet<string> AllLibraries {
			get {
				var rv = new HashSet<string> ();
				if (LinkWithLibraries != null)
					rv.UnionWith (LinkWithLibraries);
				if (ForceLoadLibraries != null)
					rv.UnionWith (ForceLoadLibraries);
				return rv;
			}
		}

		public void ReferenceSymbols (IEnumerable<Symbol> symbols)
		{
			if (UnresolvedSymbols == null)
				UnresolvedSymbols = new HashSet<string> ();

			foreach (var symbol in symbols)
				UnresolvedSymbols.Add (symbol.Prefix + symbol.Name);
		}

		public void AddDefine (string define)
		{
			if (Defines == null)
				Defines = new HashSet<string> ();
			Defines.Add (define);
		}

		public void AddLinkWith (string library, bool force_load = false)
		{
			if (LinkWithLibraries == null)
				LinkWithLibraries = new HashSet<string> ();
			if (ForceLoadLibraries == null)
				ForceLoadLibraries = new HashSet<string> ();

			if (force_load) {
				ForceLoadLibraries.Add (library);
			} else {
				LinkWithLibraries.Add (library);
			}
		}

		public void AddLinkWith (IEnumerable<string> libraries, bool force_load = false)
		{
			if (libraries == null)
				return;

			foreach (var lib in libraries)
				AddLinkWith (lib, force_load);
		}

		public void AddSourceFile (string file)
		{
			if (SourceFiles == null)
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

		public void AddOtherFlag (string flag)
		{
			if (OtherFlags == null)
				OtherFlags = new HashSet<string> ();
			OtherFlags.Add (flag);
		}

		public void AddOtherFlags (IEnumerable<string> flags)
		{
			if (flags == null)
				return;

			if (OtherFlags == null)
				OtherFlags = new HashSet<string> ();
			OtherFlags.UnionWith (flags);
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
				throw ErrorHelper.CreateError (100, "Invalid assembly build target: '{0}'. Please file a bug report with a test case (https://github.com/xamarin/xamarin-macios/issues/new).", mode);
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
				throw ErrorHelper.CreateError (100, "Invalid assembly build target: '{0}'. Please file a bug report with a test case (https://github.com/xamarin/xamarin-macios/issues/new).", mode);
			}
			AddFramework ("Foundation");
			AddOtherFlag ("-lz");
			if (Application.Platform != ApplePlatform.WatchOS && Application.Platform != ApplePlatform.TVOS)
				Frameworks.Add ("CFNetwork"); // required by xamarin_start_wwan
		}

		public void AddFramework (string framework)
		{
			if (Frameworks == null)
				Frameworks = new HashSet<string> ();
			Frameworks.Add (framework);
		}

		public void AddFrameworks (IEnumerable<string> frameworks, IEnumerable<string> weak_frameworks)
		{
			if (frameworks != null) {
				if (Frameworks == null)
					Frameworks = new HashSet<string> ();
				Frameworks.UnionWith (frameworks);
			}

			if (weak_frameworks != null) {
				if (WeakFrameworks == null)
					WeakFrameworks = new HashSet<string> ();
				WeakFrameworks.UnionWith (weak_frameworks);
			}
		}

		public void Prepare ()
		{
			// Check for system frameworks that are only available in newer iOS versions,
			// (newer than the deployment target), in which case those need to be weakly linked.
			if (Frameworks != null) {
				if (WeakFrameworks == null)
					WeakFrameworks = new HashSet<string> ();
				
				foreach (var fwk in Frameworks) {
					if (!fwk.EndsWith (".framework", StringComparison.Ordinal)) {
						var add_to = WeakFrameworks;
						var framework = Driver.GetFrameworks (Application).Find (fwk);
						if (framework != null) {
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
			if (LinkWithLibraries != null)
				LinkWithLibraries.ExceptWith (ForceLoadLibraries);
		}

		void AddInput (string file)
		{
			if (Inputs == null)
				return;

			Inputs.Add (file);
		}

		public void WriteArguments (StringBuilder args)
		{
			Prepare ();

			ProcessFrameworksForArguments (args);

			if (LinkWithLibraries != null) {
				foreach (var lib in LinkWithLibraries) {
					args.Append (' ').Append (StringUtils.Quote (lib));
					AddInput (lib);
				}
			}

			if (ForceLoadLibraries != null) {
				foreach (var lib in ForceLoadLibraries) {
					args.Append (" -force_load ").Append (StringUtils.Quote (lib));
					AddInput (lib);
				}
			}

			if (OtherFlags != null) {
				foreach (var flag in OtherFlags)
					args.Append (' ').Append (flag);
			}

			if (Defines != null) {
				foreach (var define in Defines)
					args.Append (" -D").Append (define);
			}

			if (UnresolvedSymbols != null) {
				foreach (var symbol in UnresolvedSymbols)
					args.Append (" -u ").Append (StringUtils.Quote (symbol));
			}

			if (SourceFiles != null) {
				foreach (var src in SourceFiles) {
					args.Append (' ').Append (StringUtils.Quote (src));
					AddInput (src);
				}
			}
		}

		void ProcessFrameworksForArguments (StringBuilder args)
		{
			bool any_user_framework = false;

			if (Frameworks != null) {
				foreach (var fw in Frameworks)
					ProcessFrameworkForArguments (args, fw, false, ref any_user_framework);
			}

			if (WeakFrameworks != null) {
				foreach (var fw in WeakFrameworks)
					ProcessFrameworkForArguments (args, fw, true, ref any_user_framework);
			}

			if (any_user_framework) {
				args.Append (" -Xlinker -rpath -Xlinker @executable_path/Frameworks");
				if (Application.IsExtension)
					args.Append (" -Xlinker -rpath -Xlinker @executable_path/../../Frameworks");
			}

			if (Application.HasAnyDynamicLibraries) {
				args.Append (" -Xlinker -rpath -Xlinker @executable_path");
				if (Application.IsExtension)
					args.Append (" -Xlinker -rpath -Xlinker @executable_path/../..");
			}
		}

		void ProcessFrameworkForArguments (StringBuilder args, string fw, bool is_weak, ref bool any_user_framework)
		{
			var name = Path.GetFileNameWithoutExtension (fw);
			if (fw.EndsWith (".framework", StringComparison.Ordinal)) {
				// user framework, we need to pass -F to the linker so that the linker finds the user framework.
				any_user_framework = true;
				AddInput (Path.Combine (fw, name));
				args.Append (" -F ").Append (StringUtils.Quote (Path.GetDirectoryName (fw)));
			}
			args.Append (is_weak ? " -weak_framework " : " -framework ").Append (name);
		}

		public override string ToString ()
		{
			var args = new StringBuilder ();
			WriteArguments (args);
			return args.ToString ();
		}

		public void PopulateInputs ()
		{
			var args = new StringBuilder ();
			Inputs = new List<string> ();
			WriteArguments (args);
		}
	}
}
