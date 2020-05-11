using System;
using System.Linq;

namespace Xamarin.Bundler {
	public partial class Application
	{
		public const string ProductName = "Xamarin.Mac";
		public const string Error91LinkerSuggestion = "use the dynamic registrar or set the managed linker behaviour to Link Platform or Link Framework SDKs Only in your project's Mac Build Options > Linker Behavior";

		public bool Is32Build => false;
		public bool Is64Build => true;
		public bool IsDualBuild => false;
		public bool IsSimulatorBuild => false;
		public bool IsDeviceBuild => false;

		bool RequiresXcodeHeaders => Driver.Registrar == RegistrarMode.Static && LinkMode == LinkMode.None;

		// Use this to get the single Abi we currently support for Xamarin.Mac.
		// This makes it easy to find everywhere we need to update when Apple adds support for new Abis.
		public Abi Abi { get { return Abis.First (); } }

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
