#if !WATCH
using System;

using Foundation;
using ObjCRuntime;

namespace GameKit {
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="GKGameCenterViewController" />.</summary>
	public enum GKGameCenterViewControllerInitializationOption {
		/// <summary>The <c>id</c> parameter passed to the constructor is an achievement ID.</summary>
		AchievementId,
		/// <summary>The <c>id</c> parameter passed to the constructor is a leaderboard set ID.</summary>
		LeaderboardSetId,
	}

	public partial class GKGameCenterViewController {
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("tvos14.0")]
#else
		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0), MacCatalyst (14, 0)]
#endif
		public GKGameCenterViewController (string id)
			: this (id, GKGameCenterViewControllerInitializationOption.AchievementId)
		{
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0), NoWatch]
#endif
		public GKGameCenterViewController (string id, GKGameCenterViewControllerInitializationOption option)
		{
			switch (option) {
			case GKGameCenterViewControllerInitializationOption.AchievementId:
				InitializeHandle (_InitWithAchievementId (id));
				break;
			case GKGameCenterViewControllerInitializationOption.LeaderboardSetId:
				InitializeHandle (_InitWithLeaderboardSetId (id));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
#endif // !WATCH
