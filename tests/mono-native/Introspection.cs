using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using ObjCRuntime;

namespace Xamarin.Tests
{
	[TestFixture]
	public class Introspection
	{
		public static string RootDirectory => Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);

		void AssertShouldExist (string name)
		{
			var pathName = Path.Combine (RootDirectory, name);
			Assert.That (File.Exists (pathName), $"Found {name}.");
		}

		void AssertShouldNotExist (string name)
		{
			var pathName = Path.Combine (RootDirectory, name);
			Assert.That (File.Exists (pathName), Is.False, $"Should not have {name}.");
		}

		int CountFiles (string pattern)
		{
			// Don't use Linq as it'd use too much memory.
			return Directory.GetFiles (RootDirectory, pattern, SearchOption.AllDirectories).Length;
		}

		void CheckDynamicLibrary ()
		{
			AssertShouldExist (MonoNativeConfig.DynamicLibraryName);
			AssertShouldNotExist (MonoNativeConfig.GetDynamicLibraryName (!MonoNativeConfig.UsingCompat));
			AssertShouldNotExist ("libmono-native.dylib");

			var count = CountFiles ("libmono-native*");
			Assert.That (count, Is.EqualTo (1), "exactly one mono-native library.");
		}

		void CheckStaticLibrary ()
		{
			AssertShouldNotExist ("libmono-native.dylib");
			AssertShouldNotExist ("libmono-native-compat.dylib");
			AssertShouldNotExist ("libmono-native-unified.dylib");

			var count = CountFiles ("libmono-native*");
			Assert.That (count, Is.EqualTo (0), "zero mono-native libraries.");
		}

		void CheckSymlinkedLibrary ()
		{
			AssertShouldExist ("libmono-native.dylib");
			AssertShouldNotExist ("libmono-native-compat.dylib");
			AssertShouldNotExist ("libmono-native-unified.dylib");

			var count = CountFiles ("libmono-native*");
			Assert.That (count, Is.EqualTo (1), "exactly one mono-native library.");
		}

		[Test]
		public void CheckLibrary ()
		{
			switch (MonoNativeConfig.LinkMode) {
			case MonoNativeLinkMode.Dynamic:
				CheckDynamicLibrary ();
				break;
			case MonoNativeLinkMode.Static:
				CheckStaticLibrary ();
				break;
			case MonoNativeLinkMode.Symlink:
				CheckSymlinkedLibrary ();
				break;
			default:
				Assert.Fail ($"Unknown link mode: {MonoNativeConfig.LinkMode}");
				break;
			}
		}

		[Test]
		public void CheckSymbols ()
		{
			string libname;
			switch (MonoNativeConfig.LinkMode) {
			case MonoNativeLinkMode.Dynamic:
				libname = MonoNativeConfig.DynamicLibraryName;
				break;
			case MonoNativeLinkMode.Static:
				libname = null;
				break;
			case MonoNativeLinkMode.Symlink:
				libname = "libmono-native.dylib";
				break;
			default:
				Assert.Fail ($"Unknown link mode: {MonoNativeConfig.LinkMode}");
				return;
			}

			mono_native_initialize ();

			var dylib = Dlfcn.dlopen (libname, 0);
			Assert.That (dylib, Is.Not.EqualTo (IntPtr.Zero), "dlopen()ed mono-native");

			try {
#if MONOTOUCH_TV || MONOTOUCH_WATCH // on tvOS/watchOS we emit a native reference for P/Invokes in all assemblies, so we'll strip away the 'mono_native_initialize' symbol when we're linking statically (since we don't need the symbol).
				var has_symbol = MonoNativeConfig.LinkMode != MonoNativeLinkMode.Static || Runtime.Arch == Arch.SIMULATOR;
#else
				var has_symbol = true;
#endif
				var symbol = Dlfcn.dlsym (dylib, "mono_native_initialize");

				if (has_symbol) {
					Assert.That (symbol, Is.Not.EqualTo (IntPtr.Zero), "dlsym() found mono_native_initialize()");
				} else {
					Assert.That (symbol, Is.EqualTo (IntPtr.Zero), "dlsym() did not find mono_native_initialize()");
				}
			} finally {
				Dlfcn.dlclose (dylib);
			}
		}

		[DllImport ("System.Native")]
		extern static void mono_native_initialize ();

		[Test]
		public void TestInvoke ()
		{
			mono_native_initialize ();
		}
	}
}
