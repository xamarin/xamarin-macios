using System;
using Xamarin.Messaging.Client;
using System.Threading.Tasks;
using Xamarin.MacDev;
using Xamarin.Messaging.Build.Contracts;

namespace Xamarin.Messaging.Build {
	public class VerifyXcodeVersionMessageHandler : RequestHandler<VerifyXcodeVersionMessage, VerifyXcodeVersionResult> {
		protected override Task<VerifyXcodeVersionResult> ExecuteAsync (VerifyXcodeVersionMessage message)
		{
			var result = new VerifyXcodeVersionResult ();

			try {
				var xcodeVersion = AppleSdkSettings.XcodeVersion;
				var recommendedXcodeVersion = GetRecommendedXcodeVersion ();

				result.IsCompatibleVersion = xcodeVersion >= recommendedXcodeVersion;
				result.XcodeVersion = xcodeVersion.ToString ();
				result.RecommendedXcodeVersion = recommendedXcodeVersion.ToString ();
			} catch {
				result.IsCompatibleVersion = false;
			}

			return Task.FromResult (result);
		}

		Version GetRecommendedXcodeVersion ()
		{
			var monoTouchSdk = GetMonoTouchSdk ();

			return monoTouchSdk is null ? new Version (0, 0) : monoTouchSdk?.RecommendedXcodeVersion;
		}

		MonoTouchSdk GetMonoTouchSdk ()
		{
			return Sdks.XamIOS;
		}
	}
}
