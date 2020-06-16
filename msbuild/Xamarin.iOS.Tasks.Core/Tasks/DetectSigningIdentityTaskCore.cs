using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectSigningIdentityTaskCore : DetectSigningIdentityTaskBase
	{
		static readonly string[] directDistributionPrefixes = new string[0];

		protected override string[] DevelopmentPrefixes { get { return IPhoneCertificate.DevelopmentPrefixes; } }
		protected override string[] DirectDistributionPrefixes { get { return directDistributionPrefixes; } }
		protected override string[] AppStoreDistributionPrefixes { get { return IPhoneCertificate.DistributionPrefixes; } }
		protected override string DeveloperRoot { get { return IPhoneSdks.GetSdk (TargetFrameworkMoniker).DeveloperRoot; } }
		protected override string ApplicationIdentifierKey { get { return "application-identifier"; } }
	}
}

