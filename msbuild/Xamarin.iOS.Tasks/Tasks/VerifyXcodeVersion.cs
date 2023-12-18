using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;
using Xamarin.Messaging.Build.Contracts;
using Threading = System.Threading.Tasks;

namespace Xamarin.iOS.Tasks {
	public class VerifyXcodeVersion : XamarinTask {
		public override bool Execute () => ExecuteAsync ().Result;

		async Threading.Task<bool> ExecuteAsync ()
		{
			try {
				var connection = await BuildConnection.GetAsync (BuildEngine4).ConfigureAwait (continueOnCapturedContext: false);
				var buildClient = connection.GetClient (SessionId) as BuildClient;

				if (!connection.IsConnected || buildClient is null) {
					Log.LogWarning (MSBStrings.E0179, nameof (VerifyXcodeVersion));

					return true;
				}

				var xcodeVerificationResult = await buildClient
					.RunMessagingAsync<VerifyXcodeVersionMessage, VerifyXcodeVersionResult> (new VerifyXcodeVersionMessage ())
					.ConfigureAwait (continueOnCapturedContext: false);

				if (!xcodeVerificationResult.IsCompatibleVersion) {
					if (string.IsNullOrEmpty (xcodeVerificationResult.XcodeVersion) || string.IsNullOrEmpty (xcodeVerificationResult.RecommendedXcodeVersion)) {
						Log.LogWarning (MSBStrings.E0183);
					} else {
						Log.LogWarning (MSBStrings.E0184, xcodeVerificationResult.XcodeVersion, xcodeVerificationResult.RecommendedXcodeVersion);
					}
				}

				return true;
			} catch (Exception ex) {
				Log.LogWarning (MSBStrings.E0185, ex.Message);

				return true;
			}
		}
	}
}
