using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace ARKit {
	public partial class ARSkeleton {

#if !NET
		[iOS (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
#endif
		[DllImport (Constants.ARKitLibrary)]
		static extern IntPtr /* NSString */ ARSkeletonJointNameForRecognizedPointKey (/* NSString */ IntPtr recognizedPointKey);
		
#if !NET
		[iOS (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
#endif
		public static NSString CreateJointName (NSString recognizedPointKey)
		{
			if (recognizedPointKey == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (recognizedPointKey));
			return (NSString) Runtime.GetNSObject (ARSkeletonJointNameForRecognizedPointKey (recognizedPointKey.Handle));
		}
	}
}
