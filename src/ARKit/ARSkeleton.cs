using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace ARKit {
	public partial class ARSkeleton {

		[iOS (14,0)]
		[DllImport (Constants.ARKitLibrary)]
		static extern IntPtr /* NSString */ ARSkeletonJointNameForRecognizedPointKey (/* NSString */ IntPtr recognizedPointKey);
		
		[iOS (14,0)]
		public static NSString? CreateJointName (NSString recognizedPointKey)
		{
			if (recognizedPointKey is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (recognizedPointKey));
			return Runtime.GetNSObject<NSString> (ARSkeletonJointNameForRecognizedPointKey (recognizedPointKey.Handle));
		}
	}
}
