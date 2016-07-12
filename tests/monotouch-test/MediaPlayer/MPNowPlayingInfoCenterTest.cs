// Copyright 2016 Xamarin Inc. All rights reserved

#if !__TVOS__

using System;
using System.Drawing;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MPNowPlayingInfoCenterTest
	{
		MPNowPlayingInfo NowPlayingInfo;

		[SetUp]
		public void SetUp ()
		{
			var languageOption = new MPNowPlayingInfoLanguageOption (MPNowPlayingInfoLanguageOptionType.Audible, "en", null, "English", "en");
			var languageOptionGroup = new MPNowPlayingInfoLanguageOptionGroup (new MPNowPlayingInfoLanguageOption [] { languageOption }, languageOption, false);
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var img = UIImage.FromFile (file)) {
				NowPlayingInfo = new MPNowPlayingInfo {
					//MPNowPlayingInfoCenter
					ElapsedPlaybackTime = 1.0,
					PlaybackRate = 1.0,
					DefaultPlaybackRate = 1.0,
					PlaybackQueueIndex = 0,
					PlaybackQueueCount = 10,
					ChapterNumber = 1,
					ChapterCount = 10,
					IsLiveStream = false,
					AvailableLanguageOptions = new MPNowPlayingInfoLanguageOptionGroup [] { languageOptionGroup },
					CurrentLanguageOptions = new MPNowPlayingInfoLanguageOption [] { new MPNowPlayingInfoLanguageOption (MPNowPlayingInfoLanguageOptionType.Audible, "en", null, "English", "en") },
					CollectionIdentifier = "Collection",
					ExternalContentIdentifier = "ExternalContent",
					ExternalUserProfileIdentifier = "ExternalUserProfile",
					PlaybackProgress = 0.5f,
					MediaType = MPNowPlayingInfoMediaType.Audio,

					//MPMediaItem
					AlbumTitle = "AlbumTitle",
					AlbumTrackCount = 13,
					AlbumTrackNumber = 1,
					Artist = "Artist",
					Artwork = new MPMediaItemArtwork (img),
					Composer = "Composer",
					DiscCount = 1,
					DiscNumber = 1,
					Genre = "Genre",
					PersistentID = 1,
					PlaybackDuration = 100.0,
					Title = "Title",
				};
			}
		}

		[Test]
		public void NowPlaying ()
		{
			using (var dc = MPNowPlayingInfoCenter.DefaultCenter) {
				dc.NowPlaying = NowPlayingInfo; // internal NSDictionary ToDictionary ()
				var np = dc.NowPlaying; // internal MPNowPlayingInfo (NSDictionary source)

				Assert.IsInstanceOfType (typeof (double), np.ElapsedPlaybackTime, "#1");
				Assert.IsInstanceOfType (typeof (double), np.PlaybackRate, "#2");
				Assert.IsInstanceOfType (typeof (double), np.DefaultPlaybackRate, "#3");
				Assert.IsInstanceOfType (typeof (int), np.PlaybackQueueIndex, "#4");
				Assert.IsInstanceOfType (typeof (int), np.PlaybackQueueCount, "#5");
				Assert.IsInstanceOfType (typeof (int), np.ChapterNumber, "#6");
				Assert.IsInstanceOfType (typeof (int), np.ChapterCount, "#7");
				Assert.IsInstanceOfType (typeof (bool), np.IsLiveStream, "#8");
				Assert.IsInstanceOfType (typeof (MPNowPlayingInfoLanguageOptionGroup []), np.AvailableLanguageOptions, "#9");
				Assert.IsInstanceOfType (typeof (MPNowPlayingInfoLanguageOption []), np.CurrentLanguageOptions, "#10");
				Assert.IsInstanceOfType (typeof (string), (object)np.CollectionIdentifier, "#11");
				Assert.IsInstanceOfType (typeof (string), (object)np.ExternalContentIdentifier, "#12");
				Assert.IsInstanceOfType (typeof (string), (object)np.ExternalUserProfileIdentifier, "#13");
				Assert.IsInstanceOfType (typeof (float), np.PlaybackProgress, "#14");
				Assert.IsInstanceOfType (typeof (MPNowPlayingInfoMediaType), np.MediaType, "#15");

				Assert.IsInstanceOfType (typeof (string), (object)np.AlbumTitle, "#16");
				Assert.IsInstanceOfType (typeof (int), np.AlbumTrackCount, "#17");
				Assert.IsInstanceOfType (typeof (int), np.AlbumTrackNumber, "#18");
				Assert.IsInstanceOfType (typeof (string), (object)np.Artist, "#19");
				Assert.IsInstanceOfType (typeof (MPMediaItemArtwork), np.Artwork, "#20");
				Assert.IsInstanceOfType (typeof (string), (object)np.Composer, "#21");
				Assert.IsInstanceOfType (typeof (int), np.DiscCount, "#22");
				Assert.IsInstanceOfType (typeof (int), np.DiscNumber, "#23");
				Assert.IsInstanceOfType (typeof (string), (object)np.Genre, "#24");
				Assert.IsInstanceOfType (typeof (ulong), np.PersistentID, "#25");
				Assert.IsInstanceOfType (typeof (double), np.PlaybackDuration, "#26");
				Assert.IsInstanceOfType (typeof (string), (object)np.Title, "#27");
			}
		}
	}
}

#endif // !__TVOS__
