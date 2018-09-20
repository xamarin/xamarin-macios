//
// INPlayMediaIntentResponse.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#if XAMCORE_2_0 && !MONOMAC
using System;
using Foundation;
using ObjCRuntime;
using MediaPlayer;

namespace Intents {
	public partial class INPlayMediaIntentResponse {

		// MPNowPlayingInfo is not a DictionaryContainer but a manual NSDictionary wrapper class.
		MPNowPlayingInfo NowPlayingInfo {
			get => new MPNowPlayingInfo (WeakNowPlayingInfo);
			set => WeakNowPlayingInfo = value?.ToDictionary ();
		}
	}
}
#endif