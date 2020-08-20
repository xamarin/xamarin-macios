using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace ARKit {
	public partial class ARSkeleton {

		[iOS (14,0)]
		[DllImport (Constants.ARKitLibrary)]
		static extern IntPtr /* NSString */ ARSkeletonJointNameForRecognizedPointKey (/* NSString */ IntPtr recognizedPointKey);
		[iOS (14,0)]
		public static NSString CreateJointName (NSString recognizedPointKey)
		{
			if (recognizedPointKey == null)
				throw new ArgumentNullException (nameof (recognizedPointKey));
			return (NSString) Runtime.GetNSObject (ARSkeletonJointNameForRecognizedPointKey (recognizedPointKey.Handle));
		}
	}
}
