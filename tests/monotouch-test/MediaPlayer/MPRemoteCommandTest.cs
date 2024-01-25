//
// Unit tests for MPRemoteCommandTest
//
// Authors:
//	TJ Lambert <TJ.Lambert@microsoft.com>
//
//
// Copyright 2022 Microsoft. All rights reserved.
//

using System;

using Foundation;

using MediaPlayer;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MPRemoteCommandTest {

		[Test]
		public void RemoteTargetNull ()
		{
			var center = MPRemoteCommandCenter.Shared;
			Func<MPRemoteCommandEvent, MPRemoteCommandHandlerStatus> handler = m => MPRemoteCommandHandlerStatus.Success;
			center.PlayCommand.AddTarget (handler);

			// at this point, the PlayCommand contains the target.

			Assert.DoesNotThrow (delegate { center.PlayCommand.RemoveTarget (null, null); },
				"MPRemoteCommand.RemoveTarget should not throw an ArgumentNullException.");

			// at this point, the PlayCommand does not contain any targets as expected.
		}
	}
}
