//
// Bindings to the MPNowPlayingInfoCenter
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, Xamarin Inc
//

using System.Diagnostics.CodeAnalysis;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace MediaPlayer {

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public double? DefaultPlaybackRate;

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public MPNowPlayingInfoLanguageOptionGroup []? AvailableLanguageOptions { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public MPNowPlayingInfoLanguageOption []? CurrentLanguageOptions { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public string? CollectionIdentifier { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public string? ExternalContentIdentifier { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public string? ExternalUserProfileIdentifier { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public float? PlaybackProgress { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public MPNowPlayingInfoMediaType? MediaType { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool? IsLiveStream { get; set; }
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSUrl? AssetUrl { get; set; }
#if NET
		[SupportedOSPlatform ("ios11.1")]
		[SupportedOSPlatform ("tvos11.1")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (11, 1)]
		[TV (11, 1)]
#endif
		public NSDate? CurrentPlaybackDate { get; set; }

		public string? AlbumTitle;
		public string? Artist;
		public MPMediaItemArtwork? Artwork;
		public string? Composer;
		public string? Genre;
		public string? Title;

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

			if (AvailableLanguageOptions is not null && AvailableLanguageOptions.Length != 0)
				Add (dict, MPNowPlayingInfoCenter.PropertyAvailableLanguageOptions, NSArray.FromObjects (AvailableLanguageOptions));
			if (CurrentLanguageOptions is not null && CurrentLanguageOptions.Length != 0)
				Add (dict, MPNowPlayingInfoCenter.PropertyCurrentLanguageOptions, NSArray.FromObjects (CurrentLanguageOptions));
			if (CollectionIdentifier is not null)
				Add (dict, MPNowPlayingInfoCenter.PropertyCollectionIdentifier, new NSString (CollectionIdentifier));
			if (ExternalContentIdentifier is not null)
				Add (dict, MPNowPlayingInfoCenter.PropertyExternalContentIdentifier, new NSString (ExternalContentIdentifier));
			if (ExternalUserProfileIdentifier is not null)
				Add (dict, MPNowPlayingInfoCenter.PropertyExternalUserProfileIdentifier, new NSString (ExternalUserProfileIdentifier));
			if (PlaybackProgress.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyPlaybackProgress, new NSNumber (PlaybackProgress.Value));
			if (MediaType.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyMediaType, new NSNumber ((int) MediaType.Value));
			if (IsLiveStream.HasValue)
				Add (dict, MPNowPlayingInfoCenter.PropertyIsLiveStream, new NSNumber (IsLiveStream.Value));
			if (AssetUrl is not null)
				Add (dict, MPNowPlayingInfoCenter.PropertyAssetUrl, AssetUrl);
			if (CurrentPlaybackDate is not null)
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

			if (AlbumTitle is not null)
				dict.Add (MPMediaItem.AlbumTitleProperty, new NSString (AlbumTitle));
			if (Artist is not null)
				dict.Add (MPMediaItem.ArtistProperty, new NSString (Artist));
			if (Artwork is not null)
				dict.Add (MPMediaItem.ArtworkProperty, Artwork);
			if (Composer is not null)
				dict.Add (MPMediaItem.ComposerProperty, new NSString (Composer));
			if (Genre is not null)
				dict.Add (MPMediaItem.GenreProperty, new NSString (Genre));
			if (Title is not null)
				dict.Add (MPMediaItem.TitleProperty, new NSString (Title));

			return dict;
		}

		void Add (NSMutableDictionary dictionary, NSObject key, NSObject value)
		{
			if (key is not null)
				dictionary.Add (key, value);
		}

		bool TryGetValue (NSDictionary source, NSObject? key, [NotNullWhen (true)] out NSObject? result)
		{
			if (key is not null)
				return source.TryGetValue (key, out result);
			result = null;
			return false;
		}

		internal MPNowPlayingInfo (NSDictionary? source)
		{
			if (source is null)
				return;

			NSObject? result;

			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyElapsedPlaybackTime, out result))
				ElapsedPlaybackTime = (result as NSNumber)?.DoubleValue;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyPlaybackRate, out result))
				PlaybackRate = (result as NSNumber)?.DoubleValue;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyPlaybackQueueIndex, out result))
				PlaybackQueueIndex = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyPlaybackQueueCount, out result))
				PlaybackQueueCount = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyChapterNumber, out result))
				ChapterNumber = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyChapterCount, out result))
				ChapterCount = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyDefaultPlaybackRate, out result))
				DefaultPlaybackRate = (result as NSNumber)?.DoubleValue;

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
				PlaybackProgress = (result as NSNumber)?.FloatValue;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyMediaType, out result))
				MediaType = (MPNowPlayingInfoMediaType?) (result as NSNumber)?.UInt32Value;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyIsLiveStream, out result))
				IsLiveStream = (result as NSNumber)?.BoolValue;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyAssetUrl, out result))
				AssetUrl = result as NSUrl;
			if (TryGetValue (source, MPNowPlayingInfoCenter.PropertyCurrentPlaybackDate, out result))
				CurrentPlaybackDate = result as NSDate;

			if (TryGetValue (source, MPMediaItem.AlbumTrackCountProperty, out result))
				AlbumTrackCount = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPMediaItem.AlbumTrackNumberProperty, out result))
				AlbumTrackNumber = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPMediaItem.DiscCountProperty, out result))
				DiscCount = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPMediaItem.DiscNumberProperty, out result))
				DiscNumber = (result as NSNumber)?.Int32Value;
			if (TryGetValue (source, MPMediaItem.PersistentIDProperty, out result))
				PersistentID = (result as NSNumber)?.UInt64Value;
			if (TryGetValue (source, MPMediaItem.PlaybackDurationProperty, out result))
				PlaybackDuration = (result as NSNumber)?.DoubleValue;

			if (TryGetValue (source, MPMediaItem.AlbumTitleProperty, out result))
				AlbumTitle = (string) (result as NSString);
			if (TryGetValue (source, MPMediaItem.ArtistProperty, out result))
				Artist = (string) (result as NSString);
			if (TryGetValue (source, MPMediaItem.ArtworkProperty, out result))
				Artwork = result as MPMediaItemArtwork;
			if (TryGetValue (source, MPMediaItem.ComposerProperty, out result))
				Composer = (string) (result as NSString);
			if (TryGetValue (source, MPMediaItem.GenreProperty, out result))
				Genre = (string) (result as NSString);
			if (TryGetValue (source, MPMediaItem.TitleProperty, out result))
				Title = (string) (result as NSString);
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
