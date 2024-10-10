#if !WATCH
using System;

using Foundation;
using ObjCRuntime;

namespace GameKit {
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="GKGameCenterViewController" />.</summary>
	public enum GKGameCenterViewControllerInitializationOption {
		/// <summary>The <c>id</c> parameter passed to the constructor is an achievement ID.</summary>
		Achievement,
		/// <summary>The <c>id</c> parameter passed to the constructor is a leaderboard set ID.</summary>
		LeaderboardSet,
	}

	public partial class GKGameCenterViewController {
		/// <summary>Create a new GKGameCenterViewController instance that presents an achievement.</summary>
		/// <param name="achievementId">The ID of the achievement to show.</param>
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos14.0")]
#else
		[TV (14, 0), Mac (11, 0), iOS (14, 0), MacCatalyst (14, 0)]
#endif
		public GKGameCenterViewController (string achievementId)
			: this (achievementId, GKGameCenterViewControllerInitializationOption.Achievement)
		{
		}

		/// <summary>Create a new GKGameCenterViewController instance that presents an achievement or a leaderboard set.</summary>
		/// <param name="id">The ID of the achievement or the leaderboard set to show.</param>
		/// <param name="option">Use this option to specify whether the GKGameCenterViewController shows an achievement or a leader board set.</param>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public GKGameCenterViewController (string id, GKGameCenterViewControllerInitializationOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case GKGameCenterViewControllerInitializationOption.Achievement:
				InitializeHandle (_InitWithAchievementId (id));
				break;
			case GKGameCenterViewControllerInitializationOption.LeaderboardSet:
				InitializeHandle (_InitWithLeaderboardSetId (id));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
#endif // !WATCH
