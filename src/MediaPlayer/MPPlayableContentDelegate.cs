#if !XAMCORE_3_0 && !MONOMAC && !NET

using System;
using System.Threading.Tasks;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace MediaPlayer {
	public partial class MPPlayableContentDelegate {
		[Obsolete ("Use 'InitiatePlaybackOfContentItem' instead.")]
		public virtual void PlayableContentManager (MPPlayableContentManager contentManager, NSIndexPath indexPath, Action<NSError> completionHandler)
		{
			InitiatePlaybackOfContentItem (contentManager, indexPath, completionHandler);
		}

	}

	public static partial class MPPlayableContentDelegate_Extensions {
		[Obsolete ("Use 'InitiatePlaybackOfContentItem' instead.")]
		public static void PlayableContentManager (this IMPPlayableContentDelegate This, MPPlayableContentManager contentManager, NSIndexPath indexPath, Action<NSError> completionHandler)
		{
			This.InitiatePlaybackOfContentItem (contentManager, indexPath, completionHandler);
		}

	}

	public partial class MPPlayableContentDataSource : NSObject {
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		[Obsolete ("Use 'MPPlayableContentDataSource_Extensions.GetContentItemAsync' instead.")]
		public unsafe virtual Task<MPContentItem> GetContentItemAsync (string identifier)
		{
			return MPPlayableContentDataSource_Extensions.GetContentItemAsync (this, identifier);
		}
	}
}

#endif
