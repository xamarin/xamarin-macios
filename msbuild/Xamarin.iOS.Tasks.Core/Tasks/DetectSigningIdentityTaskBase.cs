using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectSigningIdentityTaskBase : Xamarin.MacDev.Tasks.DetectSigningIdentityTaskBase
	{
		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		static readonly string[] directDistributionPrefixes = new string[0];

		protected override string[] DevelopmentPrefixes { get { return IPhoneCertificate.DevelopmentPrefixes; } }
		protected override string[] DirectDistributionPrefixes { get { return directDistributionPrefixes; } }
		protected override string[] AppStoreDistributionPrefixes { get { return IPhoneCertificate.DistributionPrefixes; } }
		protected override string DeveloperRoot { get { return IPhoneSdks.GetSdk (TargetFrameworkIdentifier).DeveloperRoot; } }
		protected override PlatformFramework Framework {
			get { return PlatformFrameworkHelper.GetFramework (TargetFrameworkIdentifier); }
		}
		protected override string PlatformName {
			get {
				switch (Framework) {
				case PlatformFramework.iOS:
					return "iOS";
				case PlatformFramework.WatchOS:
					return "watchOS";
				case PlatformFramework.TVOS:
					return "tvOS";
				default:
					throw new System.InvalidOperationException (string.Format ("Invalid framework: {0}", Framework));
				}
			}
		}
		protected override string ApplicationIdentifierKey { get { return "application-identifier"; } }
	}
}
