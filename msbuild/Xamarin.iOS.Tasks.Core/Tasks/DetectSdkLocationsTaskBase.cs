using System;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Utils;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectSdkLocationsTaskBase : DetectSdkLocationsCoreTaskBase
	{
#region Inputs

		public string TargetArchitectures {
			get; set;
		}

#endregion Inputs

		protected override IAppleSdk CurrentSdk {
			get {
				return IPhoneSdks.GetSdk (Platform);
			}
		}

		protected override IAppleSdkVersion GetDefaultSdkVersion ()
		{
			return IPhoneSdkVersion.UseDefault;
		}

		public override bool Execute ()
		{
			AppleSdkSettings.Init ();
			IPhoneSdks.Reload ();

			TargetArchitecture architectures;
			if (string.IsNullOrEmpty (TargetArchitectures) || !Enum.TryParse (TargetArchitectures, out architectures))
				architectures = TargetArchitecture.Default;

			SdkIsSimulator = (architectures & (TargetArchitecture.i386 | TargetArchitecture.x86_64)) != 0;

			return base.Execute ();
		}

		protected override string GetSdkPlatform ()
		{
			switch (Platform) {
			case ApplePlatform.iOS:
				return SdkIsSimulator ? "iPhoneSimulator" : "iPhoneOS";
			case ApplePlatform.TVOS:
				return SdkIsSimulator ? "AppleTVSimulator" : "AppleTVOS";
			case ApplePlatform.WatchOS:
				return SdkIsSimulator ? "WatchSimulator" : "WatchOS";
			default:
				throw new InvalidOperationException (string.Format ("Invalid framework: {0}", Platform));
			}
		}

		protected override string GetDefaultXamarinSdkRoot ()
		{
			return IPhoneSdks.MonoTouch.SdkDir;
		}
	}
}
