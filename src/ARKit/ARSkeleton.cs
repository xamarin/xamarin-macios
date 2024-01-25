using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace ARKit {
	public partial class ARSkeleton {

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (14, 0)]
#endif
		[DllImport (Constants.ARKitLibrary)]
		static extern IntPtr /* NSString */ ARSkeletonJointNameForRecognizedPointKey (/* NSString */ IntPtr recognizedPointKey);

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (14, 0)]
#endif
		public static NSString? CreateJointName (NSString recognizedPointKey)
		{
			if (recognizedPointKey is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (recognizedPointKey));
			return Runtime.GetNSObject<NSString> (ARSkeletonJointNameForRecognizedPointKey (recognizedPointKey.Handle));
		}
	}
}
