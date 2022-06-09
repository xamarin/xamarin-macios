#if MTOUCH || MMP || BUNDLER
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	public static partial class Constants {
		public const string libSystemLibrary = "/usr/lib/libSystem.B.dylib";
		public const string libcLibrary = libSystemLibrary;
#if __MACOS__ && NET
		// For macOS we have a redirect for the messaging functions that depends on the library being "/usr/lib/libobjc.dylib":
		// https://github.com/dotnet/runtime/blob/cca6bb6be279ee92325148b8525ece2ebdcb2070/src/coreclr/vm/interoplibinterface_objc.cpp#L120
		public const string ObjectiveCLibrary = "/usr/lib/libobjc.dylib";
#else
		public const string ObjectiveCLibrary = "/usr/lib/libobjc.A.dylib";
#endif
		public const string SystemLibrary = libSystemLibrary;
		public const string libdispatchLibrary = "/usr/lib/system/libdispatch.dylib";
#if !NET
		public const string libcompression = "/usr/lib/libcompression.dylib";
#endif
	}
}
