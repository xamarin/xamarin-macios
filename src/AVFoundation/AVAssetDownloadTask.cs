// 
// AVAssetDownloadTask.cs: AVAssetDownloadTask class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

using System;
using Foundation;

namespace AVFoundation {
#if !MONOMAC && !WATCH && !TVOS
	public partial class AVAssetDownloadTask : NSUrlSessionTask {

		// NSURLRequest and NSURLResponse objects are not available for AVAssetDownloadTask
		public override NSUrlRequest OriginalRequest { 
			get {
				throw new NotSupportedException ("OriginalRequest not available for AVAssetDownloadTask");
			}
		}

		public override NSUrlRequest CurrentRequest { 
			get {
				throw new NotSupportedException ("CurrentRequest not available for AVAssetDownloadTask");
			}
		}

		public override NSUrlResponse Response { 
			get {
				throw new NotSupportedException ("Response not available for AVAssetDownloadTask");
			}
		}
	}
#endif
}

