using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using Xamarin.MacDev;
using Xamarin.Messaging.Apple.Tasks.Properties;

namespace Xamarin.Messaging.Apple.Tasks {
	public class VerifyXcodeVersion : Task {
		[Required]
		public string SessionId { get; set; }

		public override bool Execute ()
		{
			try {
				var xcodeVersion = AppleSdkSettings.XcodeVersion;
				var recommendedXcodeVersion = GetRecommendedXcodeVersion ();

				if (xcodeVersion < recommendedXcodeVersion) {
					Log.LogWarning (Resources.VerifyXcodeVersion_Mismatch, xcodeVersion, recommendedXcodeVersion);
				}

				return true;
			} catch (Exception ex) {
				Log.LogWarning (Resources.VerifyXcodeVersion_Error, ex.Message);

				return true;
			}
		}

		Version GetRecommendedXcodeVersion()
		{
			var monoTouchSdk = GetMonoTouchSdk ();

			return monoTouchSdk == null ? new Version (0, 0) : monoTouchSdk?.RecommendedXcodeVersion;
		}

		MonoTouchSdk GetMonoTouchSdk()
		{
			var sdk = default(MonoTouchSdk);

			foreach (var sdkDir in MonoTouchSdk.DefaultLocations) {
				if (MonoTouchSdk.ValidateSdkLocation(sdkDir)) {
					sdk = new MonoTouchSdk (sdkDir);
					break;
				}
			}

			return sdk;
		}
	}
}