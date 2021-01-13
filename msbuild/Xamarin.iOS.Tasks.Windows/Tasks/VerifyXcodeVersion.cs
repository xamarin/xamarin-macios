using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.iOS.Tasks.Windows.Properties;
using Xamarin.Messaging.Build.Client;
using Xamarin.Messaging.Build.Contracts;
using Threading = System.Threading.Tasks;

namespace Xamarin.iOS.Tasks.Windows {
	public class VerifyXcodeVersion : Task {
		[Required]
		public string SessionId { get; set; }

		public override bool Execute () => ExecuteAsync ().Result;

		async Threading.Task<bool> ExecuteAsync()
		{
			try {
				var connection = await BuildConnection.GetAsync (SessionId, BuildEngine4).ConfigureAwait (continueOnCapturedContext: false);
				var buildClient = connection.Client as BuildClient;

				if (!connection.IsConnected || buildClient == null) {
					Log.LogWarning (Resources.Task_NoConnectionAvailable, nameof (VerifyXcodeVersion));

					return true;
				}

				var xcodeVerificationResult = await buildClient
					.RunMessagingAsync<VerifyXcodeVersionMessage, VerifyXcodeVersionResult> (new VerifyXcodeVersionMessage ())
					.ConfigureAwait (continueOnCapturedContext: false);

				if (!xcodeVerificationResult.IsCompatibleVersion) {
					if (string.IsNullOrEmpty(xcodeVerificationResult.XcodeVersion) || string.IsNullOrEmpty(xcodeVerificationResult.RecommendedXcodeVersion)) {
						Log.LogWarning (Resources.VerifyXcodeVersion_Error);
					} else {
						Log.LogWarning (Resources.VerifyXcodeVersion_Mismatch, xcodeVerificationResult.XcodeVersion, xcodeVerificationResult.RecommendedXcodeVersion);	
					}
				}

				return true;
			} catch (Exception ex) {
				Log.LogWarning (Resources.VerifyXcodeVersion_Error_WithDetails, ex.Message);

				return true;
			}
		}
	}
}