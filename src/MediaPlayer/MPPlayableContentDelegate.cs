#if !XAMCORE_3_0 && !MONOMAC

using System;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;

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

#if !XAMCORE_4_0
	public partial class MPPlayableContentDataSource : NSObject {
		[Unavailable (PlatformName.MacOSX, PlatformArchitecture.All)]
		[iOS (10, 0)]
		[Obsolete ("Use 'MPPlayableContentDataSource_Extensions.GetContentItemAsync' instead.")]
		public unsafe virtual Task<MPContentItem> GetContentItemAsync (string identifier)
		{
			return MPPlayableContentDataSource_Extensions.GetContentItemAsync (this, identifier);
		}
	}
#endif
}

#endif
