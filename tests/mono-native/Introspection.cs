using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using ObjCRuntime;

namespace Mono.Native.Tests
{
	[TestFixture]
	public class Introspection
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
#if !JENKINS
			Assert.Ignore ("Only run on Jenkins.");
#endif
		}

		void CheckDynamicLibrary ()
		{
			var filename = Path.Combine (NativePlatformConfig.RootDirectory, "libmono-native.dylib");
			Assert.That (File.Exists (filename), "Found libmono-native.dylib");

			var count = Directory.GetFiles (NativePlatformConfig.RootDirectory).Count (file => file.Contains ("mono-native"));
			Assert.That (count, Is.EqualTo (1), "exactly one mono-native library.");
		}

		void CheckStaticLibrary ()
		{
			var filename = Path.Combine (NativePlatformConfig.RootDirectory, "libmono-native.dylib");
			Assert.That (File.Exists (filename), Is.False, "Should not have libmono-native.dylib");

			var count = Directory.GetFiles (NativePlatformConfig.RootDirectory).Count (file => file.Contains ("mono-native"));
			Assert.That (count, Is.EqualTo (0), "zero mono-native libraries.");
		}

		[Test]
		public void CheckLibrary ()
		{
			switch (NativePlatformConfig.LinkMode) {
			case MonoNativeLinkMode.Dynamic:
				CheckDynamicLibrary ();
				break;
			case MonoNativeLinkMode.Static:
				CheckStaticLibrary ();
				break;
			default:
				Assert.Fail ($"Unknown link mode: {NativePlatformConfig.LinkMode}");
				break;
			}
		}

		[Test]
		public void CheckSymbols ()
		{
			string libname;
			switch (NativePlatformConfig.LinkMode) {
			case MonoNativeLinkMode.Dynamic:
				libname = "libmono-native.dylib";
				break;
			case MonoNativeLinkMode.Static:
				libname = null;
				break;
			default:
				Assert.Fail ($"Unknown link mode: {NativePlatformConfig.LinkMode}");
				return;
			}

			var dylib = Dlfcn.dlopen (libname, 0);
			Console.Error.WriteLine ($"DYLIB: {dylib}");
			Assert.That (dylib, Is.Not.EqualTo (IntPtr.Zero), "dlopen()ed mono-native");

			var symbol = Dlfcn.dlsym (dylib, "mono_native_initialize");
			Console.Error.WriteLine ($"SYMBOL: {symbol}");
			Assert.That (symbol, Is.Not.EqualTo (IntPtr.Zero), "dlsym() found mono_native_initialize()");
		}

		[Test]
		public void MartinTest ()
		{
			var linkMode = NativePlatformConfig.LinkMode;
			Console.Error.WriteLine ($"LINK MODE: {linkMode}");

			switch (linkMode) {
			case MonoNativeLinkMode.Dynamic:
				CheckDynamicLibrary ();
				break;
			}

			Dlfcn.dlopen (null, 0);

			CheckDynamicLibrary ();
		}
	}
}
