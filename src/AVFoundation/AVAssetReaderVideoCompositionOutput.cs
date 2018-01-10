//
// AVAssetReaderVideoCompositionOutput.cs: Extra support methods
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, 2014 Novell, Inc.
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
using System;
using Foundation;
using ObjCRuntime;
using CoreVideo;

namespace AVFoundation {
	public partial class AVAssetReaderVideoCompositionOutput {
#if !XAMCORE_2_0
		[Advice ("Use overload with PixelBufferAttributes")]
		public AVAssetReaderVideoCompositionOutput (AVAssetTrack [] videoTracks, AVVideoSettings videoSettings)
		: this (videoTracks, videoSettings == null ? null : videoSettings.ToDictionary ())
		{
		}

		[Advice ("Use 'Create' method or constructor.")]
		public AVAssetReaderVideoCompositionOutput FromTracks (AVAssetTrack [] videoTracks, AVVideoSettings videoSettings)
		{
			return WeakFromTracks (videoTracks, videoSettings == null ? null : videoSettings.ToDictionary ());
		}

		[Advice ("Use UncompressedVideoSettings property")]		
		public AVVideoSettings VideoSettings {
			get {
				var dict = WeakVideoSettings;
				NSObject val;
				if (dict.TryGetValue (CVPixelBuffer.PixelFormatTypeKey, out val) && val is NSNumber){
					var number = val as NSNumber;
					return new AVVideoSettings ((CVPixelFormatType) number.Int32Value);
				}
				return new AVVideoSettings ();
			}
		}
#endif
	}
}