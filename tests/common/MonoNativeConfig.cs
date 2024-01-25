using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Xamarin.Tests {
	public enum MonoNativeLinkMode {
		None,
		Static,
		Dynamic,
		Framework,
		Symlink,
	}

	public enum MonoNativeFlavor {
		None,
		Unified,
	}

	public static class MonoNativeConfig {
		public static MonoNativeLinkMode LinkMode {
			get {
#if MONO_NATIVE_STATIC
				return MonoNativeLinkMode.Static;
#elif MONO_NATIVE_DYNAMIC
				return MonoNativeLinkMode.Dynamic;
#elif MONO_NATIVE_SYMLINK
				return MonoNativeLinkMode.Symlink;
#else
				return MonoNativeLinkMode.None;
#endif
			}
		}

		public static MonoNativeFlavor Flavor {
			get {
#if MONO_NATIVE_UNIFIED
				return MonoNativeFlavor.Unified;
#else
				return MonoNativeFlavor.None;
#endif
			}
		}

		public static bool UsingCompat {
			get {
#if MONO_NATIVE_UNIFIED
				return false;
#else
				Assert.Fail ("Missing `MONO_NATIVE_UNIFIED`");
				throw new NotImplementedException ();
#endif
			}
		}

		public static string GetDynamicLibraryName ()
		{
			return GetDynamicLibraryName (MonoNativeFlavor.Unified);
		}

		public static string GetDynamicLibraryName (MonoNativeFlavor flavor)
		{
			switch (flavor) {
			case MonoNativeFlavor.Unified:
				return "libmono-native-unified.dylib";
			default:
				Assert.Fail ($"Invalid MonoNativeFlavor: {flavor}");
				throw new NotImplementedException ();
			}
		}

		public static string GetPInvokeLibraryName (MonoNativeFlavor flavor, MonoNativeLinkMode link)
		{
			switch (link) {
			case MonoNativeLinkMode.Static:
				return null;
			case MonoNativeLinkMode.Dynamic:
				return GetDynamicLibraryName (flavor);
			case MonoNativeLinkMode.Symlink:
				return "libmono-native.dylib";
			default:
				Assert.Fail ($"Invalid link mode: {MonoNativeConfig.LinkMode}");
				throw new NotImplementedException ();
			}
		}

		public static string DynamicLibraryName => GetDynamicLibraryName ();
	}
}
