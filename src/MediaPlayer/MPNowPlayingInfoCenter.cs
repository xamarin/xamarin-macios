//
// Bindings to the MPNowPlayingInfoCenter
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, Xamarin Inc
//

#if XAMCORE_2_0 || !MONOMAC

using Foundation;
using ObjCRuntime;

namespace MediaPlayer {

	[Mac (10,12,2, onlyOn64: true)]
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
		[iOS (8,0)]
		public double? DefaultPlaybackRate;

		[iOS (9,0)]
		public MPNowPlayingInfoLanguageOptionGroup[] AvailableLanguageOptions { get; set; }
		[iOS (9,0)]
		public MPNowPlayingInfoLanguageOption[] CurrentLanguageOptions { get; set; }
		[iOS (10,0)]
		public string CollectionIdentifier { get; set; }
		[iOS (10,0)]
		public string ExternalContentIdentifier { get; set; }
		[iOS (10,0)]
		public string ExternalUserProfileIdentifier { get; set; }
		[iOS (10,0)]
		public float? PlaybackProgress { get; set; }
		[iOS (10,0)]
		public MPNowPlayingInfoMediaType? MediaType { get; set; }
		[iOS (10,0)]
		public bool? IsLiveStream { get; set; }
		[iOS (10,3)]
		public NSUrl AssetUrl { get; set; }
		[iOS (11,1), TV (11,1), Mac (10,13,1, onlyOn64: true)]
		public NSDate CurrentPlaybackDate { get; set; }

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
			if (DefaultPlaybackRate.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyDefaultPlaybackRate, new NSNumber (DefaultPlaybackRate.Value));

			if (AvailableLanguageOptions != null && AvailableLanguageOptions.Length != 0)
				Add (dict, MPNowPlayingInfoCenter.PropertyAvailableLanguageOptions, NSArray.FromObjects (AvailableLanguageOptions));
			if (CurrentLanguageOptions != null && CurrentLanguageOptions.Length != 0)
				Add (dict, MPNowPlayingInfoCenter.PropertyCurrentLanguageOptions, NSArray.FromObjects (CurrentLanguageOptions));
			if (CollectionIdentifier != null)
				Add (dict, MPNowPlayingInfoCenter.PropertyCollectionIdentifier, new NSString (CollectionIdentifier));
			if (ExternalContentIdentifier != null)
				Add (dict, MPNowPlayingInfoCenter.PropertyExternalContentIdentifier, new NSString (ExternalContentIdentifier));
			if (ExternalUserProfileIdentifier != null)
				Add (dict, MPNowPlayingInfoCenter.PropertyExternalUserProfileIdentifier, new NSString (ExternalUserProfileIdentifier));
			if (PlaybackProgress.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyPlaybackProgress, new NSNumber (PlaybackProgress.Value));
			if (MediaType.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyMediaType, new NSNumber ((int)MediaType.Value));
			if (IsLiveStream.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyIsLiveStream, new NSNumber (IsLiveStream.Value));
			if (AssetUrl != null)
				Add (dict, MPNowPlayingInfoCenter.PropertyAssetUrl, AssetUrl);
			if (CurrentPlaybackDate != null)
				Add (dict, MPNowPlayingInfoCenter.PropertyCurrentPlaybackDate, CurrentPlaybackDate);

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

		void Add (NSMutableDictionary dictionary, NSObject key, NSObject value)
		{
			if (key != null)
				dictionary.Add (key, value);
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
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyDefaultPlaybackRate, out result))
				DefaultPlaybackRate = (double) (result as NSNumber).DoubleValue;

			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyAvailableLanguageOptions, out result))
				AvailableLanguageOptions = NSArray.ArrayFromHandle<MPNowPlayingInfoLanguageOptionGroup> (result.Handle);
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyCurrentLanguageOptions, out result))
				CurrentLanguageOptions = NSArray.ArrayFromHandle<MPNowPlayingInfoLanguageOption> (result.Handle);
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyCollectionIdentifier, out result))
				CollectionIdentifier = (string) (result as NSString);
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyExternalContentIdentifier, out result))
				ExternalContentIdentifier = (string) (result as NSString);
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyExternalUserProfileIdentifier, out result))
				ExternalUserProfileIdentifier = (string) (result as NSString);
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyPlaybackProgress, out result))
				PlaybackProgress = (float) (result as NSNumber).FloatValue;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyMediaType, out result))
				MediaType = (MPNowPlayingInfoMediaType) (result as NSNumber).UInt32Value;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyIsLiveStream, out result))
				IsLiveStream = (bool) (result as NSNumber).BoolValue;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyAssetUrl, out result))
				AssetUrl = result as NSUrl;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyCurrentPlaybackDate, out result))
				CurrentPlaybackDate = result as NSDate;

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

		bool TryGetValue (NSDictionary source, NSObject key, out NSObject result)
		{
			var b = false;
			result = null;
			if (key != null) {
				source.TryGetValue (key, out result);
				b = true;
			}
			return b;
		}
	}
	
	public partial class MPNowPlayingInfoCenter {

		public MPNowPlayingInfo NowPlaying {
			get {
				return new MPNowPlayingInfo (_NowPlayingInfo);
			}
			set {
				_NowPlayingInfo = value?.ToDictionary ();
			}
		}
	}
}

#endif
