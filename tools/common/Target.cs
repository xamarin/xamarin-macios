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
		public AssemblyCollection Assemblies = new AssemblyCollection ();

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
					if (App.FastDev && a.HasLinkWithAttributes && App.EnableBitCode) {
						ErrorHelper.Warning (110, "Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include third-party binding libraries and that compiles to bitcode.");
						App.FastDev = false;
					}
#endif
				} catch (Exception e) {
					exceptions.Add (e);
				}
			}
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
	}
}
