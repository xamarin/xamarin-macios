using System;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectSdkLocationsTaskBase : DetectSdkLocationsCoreTaskBase
	{
		protected override IAppleSdkVersion GetDefaultSdkVersion ()
		{
			return IPhoneSdkVersion.UseDefault;
		}

		public override bool Execute ()
		{
			AppleSdkSettings.Init ();
			Sdks.Reload ();

			TargetArchitecture architectures;
			if (string.IsNullOrEmpty (TargetArchitectures) || !Enum.TryParse (TargetArchitectures, out architectures))
				architectures = TargetArchitecture.Default;

			SdkIsSimulator = (architectures & (TargetArchitecture.i386 | TargetArchitecture.x86_64)) != 0;

			return base.Execute ();
		}

		protected override string GetDefaultXamarinSdkRoot ()
		{
			return Sdks.XamIOS.SdkDir;
		}
	}
}
