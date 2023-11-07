//
// AVPlayer.cs: Complementing methods
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012, 2014 Xamarin Inc
//
#if !MONOMAC && !WATCH

using System;
using Foundation;
using CoreMedia;
using CoreVideo;

#nullable enable

namespace AVFoundation {
	public partial class AVPlayerItemVideoOutput {
		public CVPixelBuffer? CopyPixelBuffer (CMTime itemTime, ref CMTime outItemTimeForDisplay)
		{
			var ptr = WeakCopyPixelBuffer (itemTime, ref outItemTimeForDisplay);
			if (ptr == IntPtr.Zero)
				return null;

			return new CVPixelBuffer (ptr, true);
		}
	}
}

#endif
