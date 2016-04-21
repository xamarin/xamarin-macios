//
// Bindings to the MPNowPlayingInfoCenter
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, Xamarin Inc
//

#if !TVOS

using XamCore.Foundation;

namespace XamCore.MediaPlayer {

	public class MPNowPlayingInfo {
		public MPNowPlayingInfo ()
		{
		}
		
		public double? ElapsedPlaybackTime;
		public double? PlaybackRate;
		public int? PlaybackQueueIndex;
		public int? PlaybackQueueCount;
		public int? ChapterNumber;
		public int? ChapterCount;
		public int? AlbumTrackCount;
		public int? AlbumTrackNumber;
		public int? DiscCount; 
		public int? DiscNumber;
		public ulong? PersistentID;
		public double? PlaybackDuration;
		public double? DefaultPlaybackRate;

		public string AlbumTitle;
		public string Artist;
		public MPMediaItemArtwork Artwork;
		public string Composer;
		public string Genre;
		public string Title;

		internal NSDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			if (ElapsedPlaybackTime.HasValue)
				dict.Add (MPNowPlayingInfoCenter.PropertyElapsedPlaybackTime, new NSNumber (ElapsedPlaybackTime.Value));
			if (PlaybackRate.HasValue)
				dict.Add (MPNowPlayingInfoCenter.PropertyPlaybackRate, new NSNumber (PlaybackRate.Value));
			if (PlaybackQueueIndex.HasValue)
				dict.Add (MPNowPlayingInfoCenter.PropertyPlaybackQueueIndex, new NSNumber (PlaybackQueueIndex.Value));
			if (PlaybackQueueCount.HasValue)
				dict.Add (MPNowPlayingInfoCenter.PropertyPlaybackQueueCount, new NSNumber (PlaybackQueueCount.Value));
			if (ChapterNumber.HasValue)
				dict.Add (MPNowPlayingInfoCenter.PropertyChapterNumber, new NSNumber (ChapterNumber.Value));
			if (ChapterCount.HasValue)
				dict.Add (MPNowPlayingInfoCenter.PropertyChapterCount, new NSNumber (ChapterCount.Value));
			if (AlbumTrackCount.HasValue)
				dict.Add (MPMediaItem.AlbumTrackCountProperty, new NSNumber (AlbumTrackCount.Value));
			if (AlbumTrackNumber.HasValue)
				dict.Add (MPMediaItem.AlbumTrackNumberProperty, new NSNumber (AlbumTrackNumber.Value));
			if (DiscCount.HasValue)
				dict.Add (MPMediaItem.DiscCountProperty, new NSNumber (DiscCount.Value));
			if (DiscNumber.HasValue)
				dict.Add (MPMediaItem.DiscNumberProperty, new NSNumber (DiscNumber.Value));
			if (PersistentID.HasValue)
				dict.Add (MPMediaItem.PersistentIDProperty, new NSNumber (PersistentID.Value));
			if (PlaybackDuration.HasValue)
				dict.Add (MPMediaItem.PlaybackDurationProperty, new NSNumber (PlaybackDuration.Value));

			if (AlbumTitle != null)
				dict.Add (MPMediaItem.AlbumTitleProperty, new NSString (AlbumTitle));
			if (Artist != null)
				dict.Add (MPMediaItem.ArtistProperty, new NSString (Artist));
			if (Artwork != null)
				dict.Add (MPMediaItem.ArtworkProperty, Artwork);
			if (Composer != null)
				dict.Add (MPMediaItem.ComposerProperty, new NSString (Composer));
			if (Genre != null)
				dict.Add (MPMediaItem.GenreProperty, new NSString (Genre));
			if (Title != null)
				dict.Add (MPMediaItem.TitleProperty, new NSString (Title));

			return dict;
		}

		internal MPNowPlayingInfo (NSDictionary source)
		{
			if (source == null)
				return;
			
			NSObject result;

			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyElapsedPlaybackTime, out result))
				ElapsedPlaybackTime = (result as NSNumber).DoubleValue;
			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyPlaybackRate, out result))
				PlaybackRate = (result as NSNumber).DoubleValue;
			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyPlaybackQueueIndex, out result))
				PlaybackQueueIndex = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyPlaybackQueueCount, out result))
				PlaybackQueueCount = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyChapterNumber, out result))
				ChapterNumber = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyChapterCount, out result))
				ChapterCount = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPNowPlayingInfoCenter.PropertyDefaultPlaybackRate, out result))
				DefaultPlaybackRate = (double) (result as NSNumber).DoubleValue;
			if (source.TryGetValue (MPMediaItem.AlbumTrackCountProperty, out result))
				AlbumTrackCount = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPMediaItem.AlbumTrackNumberProperty, out result))
				AlbumTrackNumber = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPMediaItem.DiscCountProperty, out result))
				DiscCount = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPMediaItem.DiscNumberProperty, out result))
				DiscNumber = (int) (result as NSNumber).UInt32Value;
			if (source.TryGetValue (MPMediaItem.PersistentIDProperty, out result))
				PersistentID = (result as NSNumber).UInt64Value;
			if (source.TryGetValue (MPMediaItem.PlaybackDurationProperty, out result))
				PlaybackDuration = (result as NSNumber).DoubleValue;

			if (source.TryGetValue (MPMediaItem.AlbumTitleProperty, out result))
				AlbumTitle = (string) (result as NSString);
			if (source.TryGetValue (MPMediaItem.ArtistProperty, out result))
				Artist = (string) (result as NSString);
			if (source.TryGetValue (MPMediaItem.ArtworkProperty, out result))
				Artwork = result as MPMediaItemArtwork;
			if (source.TryGetValue (MPMediaItem.ComposerProperty, out result))
				Composer = (string) (result as NSString);
			if (source.TryGetValue (MPMediaItem.GenreProperty, out result))
				Genre = (string) (result as NSString);
			if (source.TryGetValue (MPMediaItem.TitleProperty, out result))
				Title = (string) (result as NSString);
		}
	}
	
	public partial class MPNowPlayingInfoCenter {

		public MPNowPlayingInfo NowPlaying {
			get {
				return new MPNowPlayingInfo (_NowPlayingInfo);
			}
			set {
				if (value == null)
					_NowPlayingInfo = null;
				else
					_NowPlayingInfo = value.ToDictionary ();
			}
		}
	}
}

#endif // !TVOS
