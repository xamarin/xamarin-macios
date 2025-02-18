#if MTOUCH || MMP || BUNDLER
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	public static partial class Constants {
		/// <summary>Path to the System library to use with DllImport attributes.</summary>
		///         <remarks>
		///         </remarks>
		public const string libSystemLibrary = "/usr/lib/libSystem.dylib";
		/// <summary>Path to the libc library to use with DllImport attributes.</summary>
		///         <remarks>
		///         </remarks>
		public const string libcLibrary = "/usr/lib/libc.dylib";
		/// <summary>Path to the libobjc library to use with DllImport attributes.</summary>
		///         <remarks>
		///         </remarks>
		public const string ObjectiveCLibrary = "/usr/lib/libobjc.dylib";
		/// <summary>Path to the System library to use with DllImport attributes.</summary>
		///         <remarks>
		///         </remarks>
		public const string SystemLibrary = "/usr/lib/libSystem.dylib";
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public const string libdispatchLibrary = "/usr/lib/system/libdispatch.dylib";
#if !NET
		public const string libcompression = "/usr/lib/libcompression.dylib";
#endif
	}
}
