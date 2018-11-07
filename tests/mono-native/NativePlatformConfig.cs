using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Mono.Native.Tests
{
	public enum MonoNativeLinkMode
	{
		None,
		Static,
		Dynamic,
		Framework
	}

	[TestFixture]
	public class NativePlatformConfig
	{
		public static MonoNativeLinkMode LinkMode {
			get {
#if MONO_NATIVE_STATIC
				return MonoNativeLinkMode.Static;
#elif MONO_NATIVE_DYNAMIC
				return MonoNativeLinkMode.Dynamic;
#else
				Assert.Fail ("Missing `MONO_NATIVE_STATIC` or `MONO_NATIVE_DYNAMIC`");
				throw new NotImplementedException ();
#endif
			}
		}

		public static string RootDirectory => Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);

		public static bool UsingCompat {
			get {
#if MONO_NATIVE_COMPAT
				return true;
#elif MONO_NATIVE_UNIFIED
				return false;
#else
				Assert.Fail ("Missing `MONO_NATIVE_COMPAT` or `MONO_NATIVE_UNIFIED`");
				throw new NotImplementedException ();
#endif
			}
		}

		public static string GetDynamicLibraryName (bool usingCompat)
		{
			if (usingCompat)
				return "libmono-native-compat.dylib";
			else
				return "libmono-native-unified.dylib";
		}

		public static string DynamicLibraryName => GetDynamicLibraryName (UsingCompat);

		[Test]
		public void PlatformType ()
		{
			var type = MonoNativePlatform.GetPlatformType ();
			Assert.That ((int)type, Is.GreaterThan (0), "platform type");

			Console.Error.WriteLine ($"NATIVE PLATFORM TYPE: {type}");

			var usingCompat = (type & MonoNativePlatformType.MONO_NATIVE_PLATFORM_TYPE_COMPAT) != 0;
			Assert.AreEqual (UsingCompat, usingCompat, "using compatibility layer");
		}
	}
}
