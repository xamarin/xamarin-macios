namespace Xamarin.Messaging.Build.Contracts {
	public class VerifyXcodeVersionResult {
		public bool IsCompatibleVersion { get; set; }

		public string XcodeVersion { get; set; }

		public string RecommendedXcodeVersion { get; set; }
	}
}
