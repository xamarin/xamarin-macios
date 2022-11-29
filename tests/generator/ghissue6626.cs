using System;
using Foundation;
using AudioToolbox;
using ObjCRuntime;

namespace GHIssue6626 {

	public delegate int AVAudioSourceNodeRenderBlock (bool isSilence, double timestamp, uint frameCount, ref AudioBuffers outputData);

	[BaseType (typeof (NSObject))]
	interface AVAudioSourceNode {
		[Export ("initWithRenderBlock:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAudioSourceNodeRenderBlock block);
	}
}
