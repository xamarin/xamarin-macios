//
// Unit tests for MPRemoteCommandCenter
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
#if !MONOMAC
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
#if !MONOMAC
using MonoTouch.UIKit;
#endif
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RemoteCommandCenterTest {

		[Test]
		public void Shared ()
		{
#if !MONOMAC
			if (!UIDevice.CurrentDevice.CheckSystemVersion (7, 1))
				Assert.Inconclusive ("Requires 7.1+");
#endif

			MPRemoteCommandCenter shared = MPRemoteCommandCenter.Shared;
			Assert.NotNull (shared.BookmarkCommand, "BookmarkCommand");
			Assert.NotNull (shared.ChangePlaybackRateCommand, "ChangePlaybackRateCommand");
			Assert.NotNull (shared.ChangeRepeatModeCommand, "ChangeRepeatModeCommand");
			Assert.NotNull (shared.ChangeShuffleModeCommand, "ChangeShuffleModeCommand");
			Assert.NotNull (shared.DislikeCommand, "DislikeCommand");
			Assert.NotNull (shared.LikeCommand, "LikeCommand");
			Assert.NotNull (shared.NextTrackCommand, "NextTrackCommand");
			Assert.NotNull (shared.PauseCommand, "PauseCommand");
			Assert.NotNull (shared.PlayCommand, "PlayCommand");
			Assert.NotNull (shared.PreviousTrackCommand, "PreviousTrackCommand");
			Assert.NotNull (shared.SeekBackwardCommand, "SeekBackwardCommand");
			Assert.NotNull (shared.SeekForwardCommand, "SeekForwardCommand");
			Assert.NotNull (shared.SkipBackwardCommand, "SkipBackwardCommand");
			Assert.NotNull (shared.SkipForwardCommand, "SkipForwardCommand");
			Assert.NotNull (shared.StopCommand, "StopCommand");
			Assert.NotNull (shared.TogglePlayPauseCommand, "TogglePlayPauseCommand");
		}
	}
}

#endif // !__WATCHOS__
