//
// Unit tests for AVVideoCompositionInstruction
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using AVFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class VideoCompositionInstructionTest {

		[Test]
		public void Defaults ()
		{
			using (var i = new AVVideoCompositionInstruction ()) {
				Assert.Null (i.BackgroundColor, "BackgroundColor");
				Assert.True (i.EnablePostProcessing, "EnablePostProcessing");
				Assert.Null (i.LayerInstructions, "LayerInstructions");
				Assert.True (i.TimeRange.Start.IsInvalid, "TimeRange.Start");
				Assert.True (i.TimeRange.Duration.IsInvalid, "TimeRange.Duration");
			}
		}

		[Test]
		public void Seven ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var i = new AVVideoCompositionInstruction ()) {
				Assert.False (i.ContainsTweening, "ContainsTweening");
				Assert.That (i.PassthroughTrackID, Is.EqualTo (0), "PassthroughTrackID");
				Assert.That (i.RequiredSourceTrackIDs.Length, Is.EqualTo (0), "RequiredSourceTrackIDs");
			}
		}
	}
}

#endif // !__WATCHOS__
