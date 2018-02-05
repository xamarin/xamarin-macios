// 
// AVLayerVideoGravity.cs: Implements strongly typed access for AVLayerVideoGravity
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, 2014 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !WATCH

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace AVFoundation {
	// Convenience enum for native strings - AVAnimation.h
	public enum AVLayerVideoGravity
	{
		ResizeAspect ,
		ResizeAspectFill,
		Resize
	}

	partial class AVPlayerLayer
	{
		static internal AVLayerVideoGravity KeyToEnum (NSString key, bool clamp = true)
		{
			if (key == AVPlayerLayer.GravityResize)
				return AVLayerVideoGravity.Resize;
			if (key == AVPlayerLayer.GravityResizeAspectFill)
				return AVLayerVideoGravity.ResizeAspectFill;
			if (key == AVPlayerLayer.GravityResizeAspect)
				return AVLayerVideoGravity.ResizeAspect;
			if (clamp)
				return AVLayerVideoGravity.ResizeAspect;
			return (AVLayerVideoGravity) (-1);
		}

		static internal NSString EnumToKey (AVLayerVideoGravity vg)
		{
			switch (vg) {
			case AVLayerVideoGravity.ResizeAspect:
				return AVPlayerLayer.GravityResizeAspect;
			case AVLayerVideoGravity.ResizeAspectFill:
				return AVPlayerLayer.GravityResizeAspectFill;
			case AVLayerVideoGravity.Resize:
				return AVPlayerLayer.GravityResize;
			default:
				return null;
			}
		}
		
		
		// Should be VideoGravity only but previous binding was wrong
		public AVLayerVideoGravity 
#if XAMCORE_2_0
			VideoGravity
#else
			LayerVideoGravity
#endif
		{
			set {
				WeakVideoGravity = EnumToKey (value);
			}
			get {
				return KeyToEnum (WeakVideoGravity);
			}
		}
	}

#if !TVOS
	partial class AVCaptureVideoPreviewLayer
	{
		// Should be VideoGravity only but previous binding was wrong
		public AVLayerVideoGravity 
#if XAMCORE_2_0
			VideoGravity
#else
			LayerVideoGravity
#endif
		{
			set {
				WeakVideoGravity = AVPlayerLayer.EnumToKey (value);
			}
			get {
				return AVPlayerLayer.KeyToEnum (WeakVideoGravity);
			}
		}
	}
#endif

	partial class AVPlayer
	{
		public AVLayerVideoGravity? ExternalPlaybackVideoGravity {
			[iOS (6,0)]
			set {
				WeakExternalPlaybackVideoGravity = value.HasValue ? AVPlayerLayer.EnumToKey (value.Value) : null;
			}
			[iOS (6,0)]
			get {
				var r = AVPlayerLayer.KeyToEnum (WeakExternalPlaybackVideoGravity, false);
				if (((int) r) == -1)
					return null;
				return r;
			}
		}
	}
}

#endif
