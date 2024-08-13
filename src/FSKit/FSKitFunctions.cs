#if NET

using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace FSKit {
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	public static class FSKitFunctions {
		[DllImport (Constants.FSKitLibrary)]
		static extern IntPtr fskit_std_log ();

		public static NSObject GetStdLog ()
		{
			return Runtime.GetNSObject (fskit_std_log ())!;
		}

		[DllImport (Constants.FSKitLibrary)]
		static extern IntPtr fs_errorForPOSIXError (int errorCode);

		public static NSError GetErrorForPosixError (int errorCode)
		{
			return Runtime.GetNSObject<NSError> (fs_errorForPOSIXError (errorCode))!;
		}

		[DllImport (Constants.FSKitLibrary)]
		static extern IntPtr fs_errorForMachError (int errorCode);

		public static NSError GetErrorForMachError (int errorCode)
		{
			return Runtime.GetNSObject<NSError> (fs_errorForMachError (errorCode))!;
		}

		[DllImport (Constants.FSKitLibrary)]
		static extern IntPtr fs_errorForCocoaError (int errorCode);

		public static NSError GetErrorForCocoaError (int errorCode)
		{
			return Runtime.GetNSObject<NSError> (fs_errorForCocoaError (errorCode))!;
		}
	}
}
#endif // NET
