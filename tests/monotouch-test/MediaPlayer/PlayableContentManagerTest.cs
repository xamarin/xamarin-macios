//
// Unit tests for MPPlayableContentManager
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayableContentManagerTest	{

		class DataSource : MPPlayableContentDataSource {
#if XAMCORE_2_0
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
#endif
		}

		class Delegate : MPPlayableContentDelegate {
		}

		[Test]
		public void Shared ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (7, 1))
				Assert.Inconclusive ("Requires 7.1+");

			MPPlayableContentManager shared = MPPlayableContentManager.Shared;
			Assert.Null (shared.DataSource, "DataSource");
			Assert.Null (shared.Delegate, "Delegate");

			try {
				using (var ds = new DataSource ())
				using (var dg = new Delegate ()) {
					shared.DataSource = ds;
					shared.Delegate = dg;
				}
			}
			finally {
				shared.DataSource = null;
				shared.Delegate = null;
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
