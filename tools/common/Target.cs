// Copyright 2013--2014 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using Mono.Cecil;
using Mono.Tuner;
using Mono.Linker;
using Xamarin.Linker;

using Xamarin.Utils;
using XamCore.Registrar;

#if MONOTOUCH
using MonoTouch;
using MonoTouch.Tuner;
using PlatformResolver = MonoTouch.Tuner.MonoTouchResolver;
using PlatformLinkContext = MonoTouch.Tuner.MonoTouchLinkContext;
#else
using MonoMac.Tuner;
using PlatformResolver = Xamarin.Bundler.MonoMacResolver;
using PlatformLinkContext = MonoMac.Tuner.MonoMacLinkContext;
#endif

namespace Xamarin.Bundler {
	public partial class Target {
		public Application App;
		public AssemblyCollection Assemblies = new AssemblyCollection (); // The root assembly is not in this list.

		public PlatformLinkContext LinkContext;
		public LinkerOptions LinkerOptions;
		public PlatformResolver Resolver = new PlatformResolver ();

		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();

		internal StaticRegistrar StaticRegistrar { get; set; }

#if MONOMAC
		public bool Is32Build { get { return !Driver.Is64Bit; } }
		public bool Is64Build { get { return Driver.Is64Bit; } }
#endif

		public Target (Application app)
		{
			this.App = app;
		}

		public void ExtractNativeLinkInfo (List<Exception> exceptions)
		{
			foreach (var a in Assemblies) {
				try {
					a.ExtractNativeLinkInfo ();

#if MTOUCH
					if (a.HasLinkWithAttributes && App.EnableBitCode && !App.OnlyStaticLibraries) {
						ErrorHelper.Warning (110, "Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include third-party binding libraries and that compiles to bitcode.");
						App.ClearAssemblyBuildTargets (); // the default is to compile to static libraries, so just revert to the default.
					}
#endif
				} catch (Exception e) {
					exceptions.Add (e);
				}
			}

#if MTOUCH
			if (!App.OnlyStaticLibraries && Assemblies.Count ((v) => v.HasLinkWithAttributes) > 1) {
				ErrorHelper.Warning (127, "Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include more than one third-party binding libraries.");
				App.ClearAssemblyBuildTargets (); // the default is to compile to static libraries, so just revert to the default.
			}
#endif
		}

		[DllImport (Constants.libSystemLibrary, SetLastError = true, EntryPoint = "strerror")]
		static extern IntPtr _strerror (int errno);

		internal static string strerror (int errno)
		{
			return Marshal.PtrToStringAuto (_strerror (errno));
		}

		[DllImport (Constants.libSystemLibrary, SetLastError = true)]
		static extern string realpath (string path, IntPtr zero);

		public static string GetRealPath (string path)
		{
			var rv = realpath (path, IntPtr.Zero);
			if (rv != null)
				return rv;

			var errno = Marshal.GetLastWin32Error ();
			ErrorHelper.Warning (54, "Unable to canonicalize the path '{0}': {1} ({2}).", path, strerror (errno), errno);
			return path;
		}

		public void ComputeLinkerFlags ()
		{
			foreach (var a in Assemblies)
				a.ComputeLinkerFlags ();
#if MTOUCH
			if (App.Platform != ApplePlatform.WatchOS && App.Platform != ApplePlatform.TVOS)
				Frameworks.Add ("CFNetwork"); // required by xamarin_start_wwan
#endif
		}

		public void GatherFrameworks ()
		{
			Assembly asm = null;
			AssemblyDefinition productAssembly = null;

			foreach (var assembly in Assemblies) {
				if (assembly.AssemblyDefinition.Name.Name == Driver.GetProductAssembly (App)) {
					asm = assembly;
					break;
				}
			}

			productAssembly = asm.AssemblyDefinition;

			// *** make sure any change in the above lists (or new list) are also reflected in 
			// *** Makefile so simlauncher-sgen does not miss any framework

			HashSet<string> processed = new HashSet<string> ();
#if !MONOMAC
			Version v80 = new Version (8, 0);
#endif

			foreach (ModuleDefinition md in productAssembly.Modules) {
				foreach (TypeDefinition td in md.Types) {
					// process only once each namespace (as we keep adding logic below)
					string nspace = td.Namespace;
					if (processed.Contains (nspace))
						continue;
					processed.Add (nspace);

					Framework framework;
					if (Driver.GetFrameworks (App).TryGetValue (nspace, out framework)) {
						// framework specific processing
						switch (framework.Name) {
#if !MONOMAC
						case "CoreAudioKit":
							// CoreAudioKit seems to be functional in the iOS 9 simulator.
							if (App.IsSimulatorBuild && App.SdkVersion.Major < 9)
								continue;
							break;
						case "Metal":
						case "MetalKit":
						case "MetalPerformanceShaders":
							// some frameworks do not exists on simulators and will result in linker errors if we include them
							if (App.IsSimulatorBuild)
								continue;
							break;
						case "PushKit":
							// in Xcode 6 beta 7 this became an (ld) error - it was a warning earlier :(
							// ld: embedded dylibs/frameworks are only supported on iOS 8.0 and later (@rpath/PushKit.framework/PushKit) for architecture armv7
							// this was fixed in Xcode 6.2 (6.1 was still buggy) see #29786
							if ((App.DeploymentTarget < v80) && (Driver.XcodeVersion < new Version (6, 2))) {
								ErrorHelper.Warning (49, "{0}.framework is supported only if deployment target is 8.0 or later. {0} features might not work correctly.", framework.Name);
								continue;
							}
							break;
#endif
						}

						if (App.SdkVersion >= framework.Version) {
							var add_to = App.DeploymentTarget >= framework.Version ? asm.Frameworks : asm.WeakFrameworks;
							add_to.Add (framework.Name);
							continue;
						}
					}
				}
			}

			// Make sure there are no duplicates between frameworks and weak frameworks.
			// Keep the weak ones.
			asm.Frameworks.ExceptWith (asm.WeakFrameworks);
		}
	}
}
