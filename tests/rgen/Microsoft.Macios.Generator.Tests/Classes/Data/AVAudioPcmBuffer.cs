using System;
using AVFoundation;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

public enum MyEnum :  int {
	One = 1,
	Two = 2,
	Three = 3,
}

[BindingType (Name = "AVAudioPCMBuffer")]
public partial class AVAudioPcmBuffer {
	[Export<Constructor> ("initWithPCMFormat:frameCapacity:")]
	public virtual partial NativeHandle InitWithPCMFormatFrameCapacity (AVAudioFormat format, uint /* AVAudioFrameCount = uint32_t */ frameCapacity);

	[Export<Constructor> ("initWithPCMFormat:bufferListNoCopy:deallocator:")]
	public virtual partial NativeHandle InitWithPCMFormatBufferListNoCopy (AVAudioFormat format, AudioBuffers bufferList, Action<AudioBuffers>? deallocator);

	[Export<Property> ("frameCapacity")]
	public virtual partial uint FrameCapacity { get; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> ("frameLength")]
	public virtual partial uint FrameLength { get; set; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> ("stride")]
	public virtual partial nuint Stride { get; }

	[Export<Method> ("audioPlayerDidFinishPlaying:successfully:")]
	public virtual partial void FinishedPlaying (AVAudioPlayer player, bool flag);

	[Export<Method> ("audioPlayerDecodeErrorDidOccur:error:")]
	public virtual partial void DecoderError (AVAudioPlayer player, NSError? error);

	[Export<Property> ("test")]
	public virtual partial MyEnum TestEnum { get; }
}
