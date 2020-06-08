using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks {
	public abstract class DetectSigningIdentityTaskCore : DetectSigningIdentityTaskBase {
		static readonly string [] appStoreDistributionPrefixes = { "3rd Party Mac Developer Application", "Apple Distribution" };
		static readonly string [] directDistributionPrefixes = { "Developer ID Application" };
		static readonly string [] developmentPrefixes = { "Mac Developer", "Apple Development" };

		protected override string [] DevelopmentPrefixes { get { return developmentPrefixes; } }
		protected override string [] DirectDistributionPrefixes { get { return directDistributionPrefixes; } }
		protected override string [] AppStoreDistributionPrefixes { get { return appStoreDistributionPrefixes; } }
		protected override string DeveloperRoot { get { return MacOSXSdks.Native.DeveloperRoot; } }
		protected override string ApplicationIdentifierKey { get { return "com.apple.application-identifier"; } }
	}
}

