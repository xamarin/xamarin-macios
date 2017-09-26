using System;

namespace Xamarin.Bundler {
	public partial class Application
	{
		public const string ProductName = "Xamarin.Mac";
		public const string Error91LinkerSuggestion = "Link Platform or Link Framework SDKs Only";

		public bool Is32Build => !Driver.Is64Bit; 
		public bool Is64Build => Driver.Is64Bit;

		internal void Initialize ()
		{
			if (DeploymentTarget == null) 
				DeploymentTarget = new Version (10, 7);
		}
	}
}
