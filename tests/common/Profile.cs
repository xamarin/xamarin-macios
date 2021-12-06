using System;
using Xamarin.Utils;

namespace Xamarin.Tests
{
	public enum Profile
	{
		None,
		iOS,
		tvOS,
		watchOS,
		macOSClassic,
		macOSMobile,
		macOSFull,
		macOSSystem,
	}

	public static class ProfileExtensions {
		public static ApplePlatform AsPlatform (this Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return ApplePlatform.iOS;
			case Profile.tvOS:
				return ApplePlatform.TVOS;
			case Profile.watchOS:
				return ApplePlatform.WatchOS;
			case Profile.macOSClassic:
			case Profile.macOSFull:
			case Profile.macOSMobile:
			case Profile.macOSSystem:
				return ApplePlatform.MacCatalyst;
			case Profile.None:
			default:
				throw new NotImplementedException (profile.ToString ());
			}
		}
	}
}
