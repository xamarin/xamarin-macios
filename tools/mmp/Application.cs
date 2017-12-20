using System;

namespace Xamarin.Bundler {
	public partial class Application
	{
		public const string ProductName = "Xamarin.Mac";
		public const string Error91LinkerSuggestion = "use the dynamic registrar or set the managed linker behaviour to Link Platform or Link Framework SDKs Only";

		public bool Is32Build => !Driver.Is64Bit; 
		public bool Is64Build => Driver.Is64Bit;

		bool RequiresXcodeHeaders => Driver.Registrar == RegistrarMode.Static && LinkMode == LinkMode.None;

		internal void Initialize ()
		{
			if (DeploymentTarget == null) 
				DeploymentTarget = new Version (10, 7);
			else if (DeploymentTarget < SdkVersions.GetMinVersion (Platform))
				throw new MonoMacException (73, true, "Xamarin.Mac {0} does not support a deployment target of {1} for {3} (the minimum is {2}). Please select a newer deployment target in your project's Info.plist.", Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetMinVersion (Platform), PlatformName);
		}
	}
}
