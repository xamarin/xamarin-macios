using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using ObjCRuntime;

namespace Xamarin.Tests {
	[TestFixture]
	public class Introspection {
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

		[DllImport ("System.Native")]
		extern static void mono_native_initialize ();

		[Test]
		public void TestInvoke ()
		{
			mono_native_initialize ();
		}
	}
}
