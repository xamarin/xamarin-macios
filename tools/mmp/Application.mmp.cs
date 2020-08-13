using System;
using System.Linq;

namespace Xamarin.Bundler {
	public partial class Application
	{
		public string ProductName = "Xamarin.Mac";
		public string CustomBundleName = "MonoBundle";
		public AOTOptions AOTOptions;
		public bool? DisableLldbAttach = null;
		public bool? DisableOmitFramePointer = null;

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
