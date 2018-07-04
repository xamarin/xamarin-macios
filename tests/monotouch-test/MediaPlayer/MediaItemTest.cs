// Copyright 2014 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MediaItemTest {

		[Test]
		public void DefaultValues ()
		{
			if (Runtime.Arch != Arch.DEVICE)
				Assert.Inconclusive ("This test only works on device (the simulator does not have an iPod Music library).");

			TestRuntime.RequestMediaLibraryPermission (true);

			using (var q = new MPMediaQuery ()) {
				var items = q.Items;
				if (items == null)
					Assert.Inconclusive ("This test needs media library privacy permission to be executed.");
				if (items.Length == 0)
					Assert.Inconclusive ("This test needs music in the music library on the device.");

				var six_dot_oh = true;
				var nine_dot_two = TestRuntime.CheckSystemVersion (PlatformName.iOS, 9, 2);
				var ten_dot_oh = TestRuntime.CheckSystemVersion (PlatformName.iOS, 10, 0);
				var ten_dot_three = TestRuntime.CheckSystemVersion (PlatformName.iOS, 10, 3);

				foreach (var i in items) {
					object dummy;
					Assert.DoesNotThrow (() => dummy = i.AlbumArtist, "AlbumArtist");
					Assert.DoesNotThrow (() => dummy = i.AlbumArtistPersistentID, "AlbumArtistPersistentID");
					Assert.DoesNotThrow (() => dummy = i.AlbumPersistentID, "AlbumPersistentID");
					Assert.DoesNotThrow (() => dummy = i.AlbumTitle, "AlbumTitle");
					Assert.DoesNotThrow (() => dummy = i.AlbumTrackCount, "AlbumTrackCount");
					Assert.DoesNotThrow (() => dummy = i.AlbumTrackNumber, "AlbumTrackNumber");
					Assert.DoesNotThrow (() => dummy = i.Artist, "Artist");
					Assert.DoesNotThrow (() => dummy = i.ArtistPersistentID, "ArtistPersistentID");
					Assert.DoesNotThrow (() => dummy = i.Artwork, "Artwork");
					Assert.DoesNotThrow (() => dummy = i.AssetURL, "AssetURL");
					Assert.DoesNotThrow (() => dummy = i.BeatsPerMinute, "BeatsPerMinute");
					Assert.DoesNotThrow (() => dummy = i.BookmarkTime, "BookmarkTime");
					Assert.DoesNotThrow (() => dummy = i.Comments, "Comments");
					Assert.DoesNotThrow (() => dummy = i.Composer, "Composer");
					Assert.DoesNotThrow (() => dummy = i.ComposerPersistentID, "ComposerPersistentID");
					Assert.DoesNotThrow (() => dummy = i.DiscCount, "DiscCount");
					Assert.DoesNotThrow (() => dummy = i.DiscNumber, "DiscNumber");
					Assert.DoesNotThrow (() => dummy = i.Genre, "Genre");
					Assert.DoesNotThrow (() => dummy = i.GenrePersistentID, "GenrePersistentID");
					if (six_dot_oh)
						Assert.DoesNotThrow (() => dummy = i.IsCloudItem, "IsCloudItem");
					Assert.DoesNotThrow (() => dummy = i.IsCompilation, "IsCompilation");
					Assert.DoesNotThrow (() => dummy = i.LastPlayedDate, "LastPlayedDate");
					Assert.DoesNotThrow (() => dummy = i.Lyrics, "Lyrics");
					Assert.DoesNotThrow (() => dummy = i.MediaType, "MediaType");
					Assert.DoesNotThrow (() => dummy = i.PersistentID, "PersistentID");
					Assert.DoesNotThrow (() => dummy = i.PlaybackDuration, "PlaybackDuration");
					Assert.DoesNotThrow (() => dummy = i.PlayCount, "PlayCount");
					Assert.DoesNotThrow (() => dummy = i.PodcastPersistentID, "PodcastPersistentID");
					Assert.DoesNotThrow (() => dummy = i.PodcastTitle, "PodcastTitle");
					Assert.DoesNotThrow (() => dummy = i.Rating, "Rating");
					Assert.DoesNotThrow (() => dummy = i.ReleaseDate, "ReleaseDate");
					Assert.DoesNotThrow (() => dummy = i.SkipCount, "SkipCount");
					Assert.DoesNotThrow (() => dummy = i.Title, "Title");
					Assert.DoesNotThrow (() => dummy = i.UserGrouping, "UserGrouping");
					if (nine_dot_two)
						Assert.DoesNotThrow (() => dummy = i.HasProtectedAsset, "HasProtectedAsset");
					if (ten_dot_oh) {
						Assert.DoesNotThrow (() => dummy = i.IsExplicitItem, "IsExplicitItem");
						Assert.DoesNotThrow (() => dummy = i.DateAdded, "DateAdded");
					}
					if (ten_dot_three)
						Assert.DoesNotThrow (() => dummy = i.PlaybackStoreID, "PlaybackStoreID");
				}
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
