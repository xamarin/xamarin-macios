//
// Unit tests for AVSpeechSynthesisVoice
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !MONOMAC
using System;
using System.Drawing;
#if XAMCORE_2_0
using AVFoundation;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
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

		[SetUp]
		public void SetUp ()
		{
#if __WATCHOS__
			if (!TestRuntime.CheckWatchOSSystemVersion (3, 0))
				Assert.Inconclusive ("Requires watchOS 3.0+");
#else
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);
#endif
		}

		[Test]
		public void Default ()
		{
			// it's not clear that `init` should be called... it works (as it does not crash) but you can't set anything
			using (var ssv = new AVSpeechSynthesisVoice ()) {
				Assert.Null (ssv.Language, "Language");
			}
		}

		[Test]
		public void Static ()
		{
			Assert.NotNull (AVSpeechSynthesisVoice.CurrentLanguageCode, "CurrentLanguageCode");
			foreach (var ssv in AVSpeechSynthesisVoice.GetSpeechVoices ()) {
				Assert.NotNull (ssv.Language, ssv.Language);
			}
		}
	}
}
#endif