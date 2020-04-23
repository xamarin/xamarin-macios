using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Bundler;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class CompileExecutableTaskBase : XcodeToolTask {

		#region Inputs

		[Required]
		public string AppBundlePath { get; set; }

		[Required]
		public string Architecture { get; set; }

		[Required]
		public string AssemblyName { get; set; }

		[Required]
		public bool IsAppExtension { get; set; }

		[Required]
		public bool IsDebug { get; set; }

		[Required]
		public ITaskItem [] NativeDynamicLibraries { get; set; }

		[Required]
		public ITaskItem [] ResolvedFileToPublish { get; set; }

		[Required]
		public string OutputPath { get; set; }

		[Required]
		public ITaskItem [] RuntimePackAsset { get; set; }

		#endregion

		protected override string XcodeToolName => "clang";

		protected override void GetArguments (IList<string> arguments)
		{
			// Get frameworks
			var frameworks = Frameworks.GetFrameworks (Platform, SdkIsSimulator);
			var sdk_version = Version.Parse (SdkVersion);
			var deployment_target_str = GetDeploymentTarget (AppBundlePath);
			var deployment_target = Version.Parse (deployment_target_str);
			foreach (var fw in frameworks) {
				bool available;
				if (SdkIsSimulator) {
					available = fw.Value.VersionAvailableInSimulator <= sdk_version;
				} else {
					available = fw.Value.Version <= sdk_version;
				}
				if (!available)
					continue;

				var weak = fw.Value.Version > deployment_target;
				arguments.Add (weak ? "-weak_framework" : "-framework");
				arguments.Add (fw.Value.Name);
			}

			// -u
			// FIXME: get this from the linker when the linker is working
			foreach (var symbol in Driver.UnresolvedSymbols) {
				switch (symbol) {
				case "_CloseZStream":
				case "_CreateZStream":
				case "_Flush":
				case "_ReadZStream":
				case "_WriteZStream":
					// FIXME: investigate why the linker can't find these (do we even need them?)
					continue;
				}
				arguments.Add ("-u");
				arguments.Add (symbol);
			}

			// defines
			if (IsDebug)
				arguments.Add ("-DDEBUG");
			arguments.Add ("-DSIMLAUNCHER");
			arguments.Add ("-DNET");
			arguments.Add ("-DMONOTOUCH");
			arguments.Add ($"-DXAMARIN_EXECUTABLE_NAME={AssemblyName}.dll");

			// warnings
			arguments.Add ("-Wl,-w");
			arguments.Add ("-Wall");
			arguments.Add ("-Werror");
			arguments.Add ("-Wconversion");
			arguments.Add ("-Wdeprecated");
			arguments.Add ("-Wuninitialized");

			// linker flags
			var library_paths = NativeDynamicLibraries.Select ((v) => Path.Combine (v.GetMetadata ("RootDir"), v.GetMetadata ("Directory"))).Distinct ().ToArray ();
			var libraries = NativeDynamicLibraries.Select ((v) => v.GetMetadata ("LibraryName")).ToArray ();
			foreach (var path in library_paths)
				arguments.Add ($"-L{path}");
			foreach (var lib in libraries)
				arguments.Add ($"-l{lib}");

			arguments.Add ("-isysroot");
			arguments.Add (SdkRoot);

			arguments.Add ("-arch");
			arguments.Add (Architecture.ToLowerInvariant ());

			// Other flags
			arguments.Add ("-gdwarf-2");
			arguments.Add ("-fobjc-abi-version=2");
			arguments.Add ("-fobjc-legacy-dispatch");
			arguments.Add ("-fms-extensions");
			arguments.Add ("-fstack-protector-strong");
			arguments.Add ("-ObjC++");
			arguments.Add ("-std=c++14");
			arguments.Add ("-fno-exceptions");
			arguments.Add ("-stdlib=libc++");
			arguments.Add (PlatformFrameworkHelper.GetMinimumVersionArgument (TargetFrameworkMoniker, SdkIsSimulator, deployment_target_str));
			arguments.Add ("-g");
			arguments.Add ("-Wl,-rpath");
			arguments.Add ("-Wl,@executable_path/");

			// _LibMono
			var lib_mono_name = "libmono.a";
			var lib_mono = RuntimePackAsset.FirstOrDefault ((v) => v.GetMetadata ("DestinationSubPath") == lib_mono_name);
			arguments.Add (lib_mono.ItemSpec);

			// _LibXamarin
			var lib_xamarin_name = IsDebug ? "libxamarin-debug.a" : "libxamarin.a";
			var lib_xamarin = RuntimePackAsset.FirstOrDefault ((v) => v.GetMetadata ("DestinationSubPath") == lib_xamarin_name);
			arguments.Add (lib_xamarin.ItemSpec);

			// _LibRegistrar
			string lib_registrar_name;
			switch (Platform) {
			case ApplePlatform.iOS:
				lib_registrar_name = "Xamarin.iOS.registrar.a";
				break;
			case ApplePlatform.TVOS:
				lib_registrar_name = "Xamarin.TVOS.registrar.a";
				break;
			case ApplePlatform.WatchOS:
				lib_registrar_name = "Xamarin.WatchOS.registrar.a";
				break;
			case ApplePlatform.MacOSX:
				lib_registrar_name = "Xamarin.Mac.registrar.mobile.a";
				break;
			default:
				// FIXME: Add error
				throw new NotImplementedException ($"Unknown platform: {Platform}");
			}
			var lib_registrar = RuntimePackAsset.FirstOrDefault ((v) => v.GetMetadata ("DestinationSubPath") == lib_registrar_name);
			arguments.Add (lib_registrar.ItemSpec);

			// _LibApp
			var lib_app_name = "libapp.a"; // FIXME: extensions
			var lib_app = RuntimePackAsset.FirstOrDefault ((v) => v.GetMetadata ("DestinationSubPath") == lib_app_name);
			arguments.Add (lib_app.ItemSpec);

			// includes
			var runtime_tools_dir = Path.Combine (lib_xamarin.GetMetadata ("RootDir"), lib_xamarin.GetMetadata ("Directory"));
			var runtime_include_dir = Path.Combine (runtime_tools_dir, "include");
			arguments.Add ($"-I{runtime_include_dir}");

			// _SimLauncher
			var simlauncher_mm = Path.Combine (runtime_tools_dir, "simlauncher.mm");
			arguments.Add (simlauncher_mm);

			arguments.Add ("-o");
			arguments.Add (OutputPath);
			Directory.CreateDirectory (Path.GetDirectoryName (OutputPath));
		}

		public override bool Execute ()
		{
			return base.Execute ();
		}
	}
}

