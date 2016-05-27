using System;
using System.Runtime.InteropServices;

#if !XAMCORE_2_0
#if MONOMAC
using MonoMac.Foundation;
#else
using MonoTouch.Foundation;
#endif
#else
using Foundation;
#endif

namespace AudioUnit {
	[BaseType (typeof(NSObject))]
	interface AUParameterNode
	{
		[Export ("tokenByAddingParameterRecordingObserver:")]
		IntPtr TokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer);
	}

	public unsafe delegate void AUParameterRecordingObserver (int arg0, AURecordedParameterEvent arg1);

	// Commenting this out "fixes" the problem
	[BaseType (typeof(NSObject))]
	interface AudioUnit
	{
	}
}
