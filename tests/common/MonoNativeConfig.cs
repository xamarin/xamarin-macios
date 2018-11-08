using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Xamarin.Tests
{
	public enum MonoNativeLinkMode
	{
		None,
		Static,
		Dynamic,
		Framework
	}

	public enum MonoNativeFlavor
	{
		None,
		Compat,
		Unified
	}

	public static class MonoNativeConfig
	{
		public static MonoNativeLinkMode LinkMode {
			get {
#if MONO_NATIVE_STATIC
				return MonoNativeLinkMode.Static;
#elif MONO_NATIVE_DYNAMIC
				return MonoNativeLinkMode.Dynamic;
#else
				return MonoNativeLinkMode.None;
#endif
			}
		}

		public static MonoNativeFlavor Flavor {
			get {
#if MONO_NATIVE_COMPAT
				return MonoNativeFlavor.Compat;
#elif MONO_NATIVE_UNIFIED
				return MonoNativeFlavor.Unified;
#else
				return MonoNativeFlavor.None;
#endif
			}
		}

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
	}
}
