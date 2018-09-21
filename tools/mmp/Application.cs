using System;

namespace Xamarin.Bundler {
	public partial class Application
	{
		public const string ProductName = "Xamarin.Mac";
		public const string Error91LinkerSuggestion = "use the dynamic registrar or set the managed linker behaviour to Link Platform or Link Framework SDKs Only in your project's Mac Build Options > Linker Behavior";

		public bool Is32Build => !Driver.Is64Bit; 
		public bool Is64Build => Driver.Is64Bit;
		public bool IsDualBuild => false;

		bool RequiresXcodeHeaders => Driver.Registrar == RegistrarMode.Static && LinkMode == LinkMode.None;

		internal void Initialize ()
		{
			if (DeploymentTarget == null) 
				DeploymentTarget = new Version (10, 7);
		}

		void SelectRegistrar ()
		{
			Driver.SelectRegistrar ();
		}
	}
}
