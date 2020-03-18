using System;

namespace Xamarin.Bundler {
	public partial class Application
	{
		public const string ProductName = "Xamarin.Mac";
		public const string Error91LinkerSuggestion = "use the dynamic registrar or set the managed linker behaviour to Link Platform or Link Framework SDKs Only in your project's Mac Build Options > Linker Behavior";

		public bool Is32Build => false;
		public bool Is64Build => true;
		public bool IsDualBuild => false;
		public bool IsSimulatorBuild => false;

		bool RequiresXcodeHeaders => Driver.Registrar == RegistrarMode.Static && LinkMode == LinkMode.None;

		internal void Initialize ()
		{
			if (DeploymentTarget == null) 
				DeploymentTarget = SdkVersions.MinOSXVersion;
		}

		void SelectRegistrar ()
		{
			Driver.SelectRegistrar ();
		}
	}
}
