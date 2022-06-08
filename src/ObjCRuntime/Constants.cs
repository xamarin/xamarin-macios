#if MTOUCH || MMP || BUNDLER
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	public static partial class Constants {
		public const string libSystemLibrary = "/usr/lib/libSystem.B.dylib";
		public const string libcLibrary = libSystemLibrary;
		public const string ObjectiveCLibrary = "/usr/lib/libobjc.A.dylib";
		public const string SystemLibrary = libSystemLibrary;
		public const string libdispatchLibrary = "/usr/lib/system/libdispatch.dylib";
#if !NET
		public const string libcompression = "/usr/lib/libcompression.dylib";
#endif
	}
}
