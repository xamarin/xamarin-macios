using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreML {

	public partial class MLModel {
#if NET
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Watch (10,0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		[DllImport (Constants.CoreMLLibrary)]
		static extern /* MLComputeDeviceProtocol[] */ IntPtr MLAllComputeDevices ();

#if NET
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Watch (10,0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		public static IMLComputeDeviceProtocol[] AllComputeDevices  {
			get {
				var ptr = MLAllComputeDevices ();
				if (ptr == IntPtr.Zero)
					return Array.Empty<IMLComputeDeviceProtocol> ();
				return NSArray.ArrayFromHandle<IMLComputeDeviceProtocol> (ptr);
			}
		} 
	}
}
