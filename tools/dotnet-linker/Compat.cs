// Compat.cs: might not be ideal but it eases code sharing with existing code during the initial implementation.
using System;

using Xamarin.Linker;
using Xamarin.Utils;

namespace Xamarin.Bundler {
	public partial class Application {
		public LinkerConfiguration Configuration { get; private set; }

		public Application (LinkerConfiguration configuration, string[] arguments)
			: this (arguments)
		{
			this.Configuration = configuration;
		}

		// This method is needed for ErrorHelper.tools.cs to compile.
		public void LoadSymbols ()
		{
		}

		public string GetProductName ()
		{
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return "Xamarin.iOS";
			case ApplePlatform.MacOSX:
				return "Xamarin.Mac";
			default:
				throw ErrorHelper.CreateError (177, Errors.MX0177 /* "Unknown platform: {0}. This usually indicates a bug; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case." */, Platform);
			}
		}

		public Version SdkVersion {
			get { return Configuration.SdkVersion; }
		}

		public Version DeploymentTarget {
			get { return Configuration.DeploymentTarget; }
		}

		public ApplePlatform Platform {
			get { return Configuration.Platform; }
		}
	}
}
