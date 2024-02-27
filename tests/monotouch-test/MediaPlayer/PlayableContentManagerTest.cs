//
// Unit tests for MPPlayableContentManager
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC && !NET

using System;
using Foundation;
using MediaPlayer;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayableContentManagerTest {

		class DataSource : MPPlayableContentDataSource {
			#region implemented abstract members of MPPlayableContentDataSource
			public override MPContentItem ContentItem (NSIndexPath indexPath)
			{
				throw new NotImplementedException ();
			}
			public override nint NumberOfChildItems (NSIndexPath indexPath)
			{
				throw new NotImplementedException ();
			}
			#endregion
		}

		class Delegate : MPPlayableContentDelegate {
		}

		[Test]
		public void Shared ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 1, throwIfOtherPlatform: false);

			MPPlayableContentManager shared = MPPlayableContentManager.Shared;
			Assert.Null (shared.DataSource, "DataSource");
			Assert.Null (shared.Delegate, "Delegate");

			try {
				using (var ds = new DataSource ())
				using (var dg = new Delegate ()) {
					shared.DataSource = ds;
					shared.Delegate = dg;
				}
			} finally {
				shared.DataSource = null;
				shared.Delegate = null;
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
