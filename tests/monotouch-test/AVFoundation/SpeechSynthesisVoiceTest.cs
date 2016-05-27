//
// Unit tests for AVSpeechSynthesisVoice
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using AVFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class SpeechSynthesisVoiceTest {

		[Test]
		public void Default ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("Requires iOS 7.0+");

			// it's not clear that `init` should be called... it works (as it does not crash) but you can't set anything
			using (var ssv = new AVSpeechSynthesisVoice ()) {
				Assert.Null (ssv.Language, "Language");
			}
		}

		[Test]
		public void Static ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("Requires iOS 7.0+");

			Assert.NotNull (AVSpeechSynthesisVoice.CurrentLanguageCode, "CurrentLanguageCode");
			foreach (var ssv in AVSpeechSynthesisVoice.GetSpeechVoices ()) {
				Assert.NotNull (ssv.Language, ssv.Language);
			}
		}
	}
}

#endif // !__WATCHOS__
