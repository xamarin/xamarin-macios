//
// ApiPInvokeTest.cs: enforce P/Invoke signatures
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin, Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xamarin.Tests;

using NUnit.Framework;

using ObjCRuntime;
using Foundation;

namespace Introspection {
	[Preserve (AllMembers = true)]
	public abstract class ApiPInvokeTest : ApiBaseTest {
		IEnumerable pinvokeQuery;

		public ApiPInvokeTest ()
		{
			ContinueOnFailure = true;
			LogProgress = false;

			pinvokeQuery = from type in Assembly.GetTypes ()
						   where !Skip (type)
						   from mi in type.GetMethods (
							   BindingFlags.NonPublic |
							   BindingFlags.Public |
							   BindingFlags.Static)
						   let attr = mi.GetCustomAttribute<DllImportAttribute> ()
						   where attr != null && !Skip (mi)
						   select mi;
		}

		protected virtual bool Skip (Type type)
		{
			return SkipDueToAttribute (type);
		}

		protected virtual bool Skip (MethodInfo methodInfo)
		{
			return SkipDueToAttribute (methodInfo);
		}

		[Test]
		public void Signatures ()
		{
			int totalPInvokes = 0;
			Errors = 0;

			foreach (MethodInfo mi in pinvokeQuery) {
				totalPInvokes++;
				if (!CheckSignature (mi)) {

					if (!ContinueOnFailure)
						break;
				}
			}

			AssertIfErrors (
				"{0} errors found in {1} P/Invoke signatures validated",
				Errors, totalPInvokes);
		}

		protected virtual bool CheckSignature (MethodInfo mi)
		{
			var success = true;

			if (!CheckReturnParameter (mi, mi.ReturnParameter))
				success = false;

			foreach (var pi in mi.GetParameters ()) {
				if (!CheckParameter (mi, pi))
					success = false;
			}

			return success;
		}

		protected virtual bool CheckReturnParameter (MethodInfo mi, ParameterInfo pi)
		{
			return CheckParameter (mi, pi);
		}

		protected virtual bool CheckParameter (MethodInfo mi, ParameterInfo pi)
		{
			bool result = true;
			// `ref` is fine but it can hide the droids we're looking for
			var pt = pi.ParameterType;
			if (pt.IsByRef)
				pt = pt.GetElementType ();
			// we don't want generics in p/invokes except for delegates like Func<> and Action<> which we know how to deal with
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=42699
			if (pt.IsGenericType && !pt.IsSubclassOf (typeof (Delegate))) {
				AddErrorLine ("[FAIL] {0}.{1} has a generic parameter in its signature: {2} {3}",
					mi.DeclaringType.FullName, mi.Name, pt, pi.Name);
				result = false;
			}
			result &= CheckForEnumParameter (mi, pi);
			return result;
		}

		protected virtual bool CheckForEnumParameter (MethodInfo mi, ParameterInfo pi)
		{
			if (pi.ParameterType.IsEnum && pi.ParameterType.GetCustomAttribute<NativeAttribute> () != null) {
				AddErrorLine ("[FAIL] {0}.{1} has a [Native] enum parameter in its signature: {2} {3}",
					mi.DeclaringType.FullName, mi.Name, pi.ParameterType, pi.Name);
				return false;
			}

			return true;
		}

		protected virtual bool Skip (string symbolName)
		{
			switch (symbolName) {
			// it's not needed for ARM64/ARM64_32 and Apple does not have stubs for them in libobjc.dylib
			// also the linker normally removes them (unreachable due to other optimizations)
			case "objc_msgSend_stret":
			case "objc_msgSendSuper_stret":
				return true;
			}
			return false;
		}

		protected virtual bool SkipLibrary (string libraryName)
		{
			return false;
		}

		[Test]
		public void SymbolExists ()
		{
			var failed_api = new HashSet<string> ();
			Errors = 0;
			int c = 0, n = 0;
			foreach (MethodInfo mi in pinvokeQuery) {

				if (LogProgress)
					Console.WriteLine ("{0}. {1}", c++, mi);

				var dllimport = mi.GetCustomAttribute<DllImportAttribute> ();

				string libname = dllimport.Value;
				switch (libname) {
				case "__Internal":
					continue;
				case "System.Native":
				case "System.Security.Cryptography.Native.Apple":
				case "System.Net.Security.Native":
					if (MonoNativeConfig.LinkMode == MonoNativeLinkMode.None)
						continue;
#if __IOS__
					libname = MonoNativeConfig.GetPInvokeLibraryName (MonoNativeFlavor.Compat, MonoNativeConfig.LinkMode);
#else
					libname = null;
#endif
					break;
				}

				if (SkipLibrary (libname))
					continue;

				string path = FindLibrary (libname, requiresFullPath: true);

				string name = dllimport.EntryPoint ?? mi.Name;
				if (Skip (name))
					continue;

				IntPtr lib = Dlfcn.dlopen (path, 0);
				if (Dlfcn.GetIndirect (lib, name) == IntPtr.Zero && !failed_api.Contains (name)) {
					ReportError ("Could not find the field '{0}' in {1}", name, path);
					failed_api.Add (name);
				}
				Dlfcn.dlclose (lib);
				n++;
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} functions validated: {2}", Errors, n, string.Join (", ", failed_api));
		}

		// we just want to confirm the symbol exists so `dlsym` can be disabled
		protected void Check (Assembly a)
		{
			Errors = 0;
			ErrorData.Clear ();
			int n = 0;
			foreach (var t in a.GetTypes ()) {
				foreach (var m in t.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
					if ((m.Attributes & MethodAttributes.PinvokeImpl) == 0)
						continue;

					var dllimport = m.GetCustomAttribute<DllImportAttribute> ();

					string name = dllimport.EntryPoint ?? m.Name;
					switch (name) {
					// known not to be present in ARM64
					case "objc_msgSend_stret":
					case "objc_msgSendSuper_stret":
						// the linker normally removes them (IntPtr.Size optimization)
						continue;
					}

					string path = dllimport.Value;
					switch (path) {
					case "__Internal":
						// load from executable
						path = null;
						break;
#if NET
					case "libSystem.Globalization.Native":
						// load from executable (like __Internal above since it's part of the static library)
						path = null;
						break;
					case "libSystem.Native":
						var staticallyLinked = false;
#if __MACCATALYST__
						// always statically linked
						staticallyLinked = true;
#elif __IOS__ || __TVOS__
						// statically linked on device
						staticallyLinked = Runtime.Arch == Arch.DEVICE;
#elif __MACOS__
						// never statically linked (by default)
#else
#error Unknown platform
#endif
						if (staticallyLinked) {
							path = null;
						} else {
							path += ".dylib";
						}
						break;
#endif
					case "libc":
						// we still have some rogue/not-fully-qualified DllImport
						path = "/usr/lib/libSystem.dylib";
						break;
					case "System.Native":
					case "System.Security.Cryptography.Native.Apple":
					case "System.Net.Security.Native":
						if (MonoNativeConfig.LinkMode == MonoNativeLinkMode.None)
							continue;
#if __IOS__
						path = MonoNativeConfig.GetPInvokeLibraryName (MonoNativeFlavor.Compat, MonoNativeConfig.LinkMode);
#else
						path = null;
#endif
						break;
					}

					var lib = Dlfcn.dlopen (path, 0);
					var h = Dlfcn.dlsym (lib, name);
					if (h == IntPtr.Zero) {
						ReportError ("Could not find the symbol '{0}' in {1} for the P/Invoke {2}.{3} in {4}", name, path, t.FullName, m.Name, a.GetName ().Name);
					} else if (path != null) {
						// Verify that the P/Invoke points to the right library.
						Dl_info info = default (Dl_info);
						var found = dladdr (h, out info);
						if (found != 0) {
							// Resolve symlinks in both cases
							var dllImportPath = ResolveLibrarySymlinks (path);
							var foundLibrary = ResolveLibrarySymlinks (Marshal.PtrToStringAuto (info.dli_fname));
							if (Skip (name, ref dllImportPath, ref foundLibrary)) {
								// Skipped
							} else if (foundLibrary != dllImportPath) {
								ReportError ($"Found the symbol '{name}' in the library '{foundLibrary}', but the P/Invoke {t.FullName}.{m.Name} in {a.GetName ().Name} claims it's in '{dllimport.Value}'.");
							}
						} else {
							Console.WriteLine ($"Unable to find the library for the symbol '{name}' claimed to be in {path} for the P/Invoke {t.FullName}.{m.Name} in {a.GetName ().Name} (rv: {found})");
						}
					}

					Dlfcn.dlclose (lib);
					n++;
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} symbol lookups{2}", Errors, n, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}

		protected string ResolveLibrarySymlinks (string path)
		{
			var resolved = ((NSString) path).ResolveSymlinksInPath ().ToString ();
			// ResolveSymlinksInPath will return the input if something goes wrong.
			// Something usually goes wrong with system libraries: they don't actually exist on disk :/
			// So add some custom logic to handle those cases.
			resolved = resolved.Replace ("/Versions/A/", "/");
			resolved = resolved.Replace ("/Versions/C/", "/");
			resolved = resolved.Replace (".A.dylib", ".dylib");
			return resolved;
		}

		protected virtual bool Skip (string symbol, ref string dllImportLibrary, ref string nativeLibrary)
		{
			// We only care about system libraries for this test.
			if (!nativeLibrary.StartsWith ("/System", StringComparison.Ordinal))
				return true;

			// Assume that if the symbol is in a private framework, then the DllImport is pointing
			// to the corresponding public/official location, and that we're just running into an
			// implementation detail.
			if (nativeLibrary.Contains ("/PrivateFrameworks/", StringComparison.Ordinal))
				return true;

			// System libraries in /usr/lib/system/ have public/official entry points in other
			// libraries, so skip those too.
			if (nativeLibrary.StartsWith ("/usr/lib/system/", StringComparison.Ordinal))
				return true;

			switch (nativeLibrary) {
			case "/usr/lib/libnetwork.dylib":
				return dllImportLibrary == "/System/Library/Frameworks/Network.framework/Network";
			case "/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/LaunchServices":
				switch (dllImportLibrary) {
				case "/System/Library/Frameworks/MobileCoreServices.framework/MobileCoreServices":
				case "/System/Library/Frameworks/CoreServices.framework/CoreServices":
					return true;

				}
				break;
			case "/System/Library/Frameworks/CoreServices.framework/Frameworks/FSEvents.framework/FSEvents":
				return dllImportLibrary == "/System/Library/Frameworks/CoreServices.framework/CoreServices";
#if __MACOS__
			case "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics":
				// Years ago, CoreGraphics was somewhere else on macOS
				return dllImportLibrary == "/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreGraphics.framework/CoreGraphics";
#endif
			case "/System/Library/Frameworks/OpenGL.framework/Libraries/libGL.dylib":
				return dllImportLibrary == "/System/Library/Frameworks/OpenGL.framework/OpenGL";
			case "/System/Library/Frameworks/CoreServices.framework/Frameworks/CarbonCore.framework/CarbonCore":
				return dllImportLibrary == "/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon";
			case "/System/Library/Frameworks/MetalPerformanceShaders.framework/Frameworks/MPSCore.framework/MPSCore":
				// Check the umbrella framework
				nativeLibrary = "/System/Library/Frameworks/MetalPerformanceShaders.framework/MetalPerformanceShaders";
				return false;
			}

#if __MACCATALYST__
			if (nativeLibrary.StartsWith ("/System/iOSSupport/", StringComparison.Ordinal))
				nativeLibrary = nativeLibrary.Substring ("/System/iOSSupport".Length);
#endif

			return false;
		}

		[DllImport (Constants.libcLibrary)]
		static extern int dladdr (IntPtr addr, out Dl_info info);

		struct Dl_info {
			internal IntPtr dli_fname; /* Pathname of shared object */
			internal IntPtr dli_fbase; /* Base address of shared object */
			internal IntPtr dli_sname; /* Name of nearest symbol */
			internal IntPtr dli_saddr; /* Address of nearest symbol */
		}

		protected abstract bool SkipAssembly (Assembly a);

		// Note: this looks very similar to the "SymbolExists" test above (and it is)
		// except that we never skip based on availability attributes or __Internal...
		// since this is a test to ensure things will work at native link time (e.g.
		// for devices) when dlsym is disabled

		[Test]
		public void Product ()
		{
			var a = typeof (NSObject).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

		// since we already have non-linked version of the most common assemblies available here
		// we can use them to check for missing symbols (from DllImport)
		// it's not complete (there's many more SDK assemblies) but we cannot add all of them into a single project anyway

		[Test]
		public void Corlib ()
		{
			var a = typeof (int).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

		[Test]
		public void System ()
		{
			var a = typeof (System.Net.WebClient).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

		[Test]
		public void SystemCore ()
		{
			var a = typeof (Enumerable).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

#if !NET
		[Test]
		public void SystemData ()
		{
			var a = typeof (System.Data.SqlClient.SqlCredential).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}
#endif
	}
}
