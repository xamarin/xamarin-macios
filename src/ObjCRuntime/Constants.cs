#if MTOUCH || MMP || BUNDLER
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	public static partial class Constants {
		public const string libSystemLibrary = "/usr/lib/libSystem.dylib";
		public const string libcLibrary = "/usr/lib/libc.dylib";
		public const string ObjectiveCLibrary = "/usr/lib/libobjc.dylib";
		public const string SystemLibrary = "/usr/lib/libSystem.dylib";
		public const string libdispatchLibrary = "/usr/lib/system/libdispatch.dylib";
#if !NET
		public const string libcompression = "/usr/lib/libcompression.dylib";
#endif
	}
}
