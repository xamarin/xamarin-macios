#if !XAMCORE_3_0
	
using System;
using XamCore.Foundation;

namespace XamCore.MediaPlayer {
	public partial class MPPlayableContentDelegate {
		[Obsolete ("Use InitiatePlaybackOfContentItem instead")]
		public virtual void PlayableContentManager (MPPlayableContentManager contentManager, NSIndexPath indexPath, Action<NSError> completionHandler)
		{
			InitiatePlaybackOfContentItem (contentManager, indexPath, completionHandler);
		}

	}

	public static partial class MPPlayableContentDelegate_Extensions {
		[Obsolete ("Use InitiatePlaybackOfContentItem instead")]
		public static void PlayableContentManager (this IMPPlayableContentDelegate This, MPPlayableContentManager contentManager, NSIndexPath indexPath, Action<NSError> completionHandler)
		{
			This.InitiatePlaybackOfContentItem (contentManager, indexPath, completionHandler);
		}

	}
}

#endif
