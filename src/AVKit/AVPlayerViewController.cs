//
// AVPlayerViewController.cs: Complementing methods
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2014 Xamarin Inc
//
using System;
using XamCore.Foundation;
using XamCore.CoreMedia;
using XamCore.CoreVideo;
using XamCore.AVFoundation;

namespace XamCore.AVKit {
#if !MONOMAC
	partial class AVPlayerViewController {
		public AVLayerVideoGravity VideoGravity {
			get {
				return AVPlayerLayer.KeyToEnum (WeakVideoGravity);
			}
			set {
				WeakVideoGravity = AVPlayerLayer.EnumToKey (value);
			}
		}
	}
#endif
}
