#if !WATCH

using System;

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.AudioToolbox;

namespace XamCore.AVFoundation {
	public partial class AVPlayerItemVideoOutput {

		enum InitMode {
			PixelAttributes,
			OutputSettings
		} 

		AVPlayerItemVideoOutput (NSDictionary data, AVPlayerItemVideoOutput.InitMode mode) : this (IntPtr.Zero)
		{
			switch (mode) {
			case InitMode.PixelAttributes:
				Handle = _FromPixelBufferAttributes (data);
				break;
			case InitMode.OutputSettings:
				Handle = _FromOutputSettings (data);
				break;
			default:
				throw new ArgumentException (nameof (mode));
			}
		}

		[DesignatedInitializer]
		[Advice ("Please use the constructor that uses one of the available StrongDictionaries. This constructor expects Pixelbugger attributes.")]
		protected AVPlayerItemVideoOutput (NSDictionary pixelBufferAttributes) : this (pixelBufferAttributes, InitMode.PixelAttributes) {}
	}
}

#endif
