// 
// AVPlayerLayer.cs: AVPlayerLayer class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

#if !WATCH

using ObjCRuntime;
using CoreVideo;

#nullable enable

namespace AVFoundation {
	public partial class AVPlayerLayer {
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public CVPixelBufferAttributes? PixelBufferAttributes {
			get {
				if (WeakPixelBufferAttributes is not null) {
					var strongDict = new CVPixelBufferAttributes (WeakPixelBufferAttributes);
					return strongDict;
				}
				return null;
			}
			set {
				WeakPixelBufferAttributes = value?.Dictionary;
			}
		}
	}
}

#endif
