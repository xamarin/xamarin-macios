using System;

#if MTOUCH || MMP || BUNDLER || PRETRIM
using Xamarin.Bundler;
using Xamarin.Utils;
#endif

#if MTOUCH
using MonoTouch;
#endif

namespace Xamarin {
	sealed class ProductConstants {
		public string Version;
		public string Revision;
		public const string Hash = "@PRODUCT_HASH@";

		ProductConstants (string version, string revision)
		{
			Version = version;
			Revision = revision;
		}
#if BUNDLER
		public readonly static ProductConstants iOS = new ProductConstants ("@IOS_NUGET_VERSION@", "@IOS_NUGET_REVISION@");
		public readonly static ProductConstants tvOS = new ProductConstants ("@TVOS_NUGET_VERSION@", "@TVOS_NUGET_REVISION@");
		public readonly static ProductConstants macOS = new ProductConstants ("@MACOS_NUGET_VERSION@", "@MACOS_NUGET_REVISION@");
		public readonly static ProductConstants MacCatalyst = new ProductConstants ("@MACCATALYST_NUGET_VERSION@", "@MACCATALYST_NUGET_VERSION@");
#else
		public readonly static ProductConstants iOS = new ProductConstants ("@IOS_VERSION@", "@IOS_REVISION@");
		public readonly static ProductConstants tvOS = new ProductConstants ("@TVOS_VERSION@", "@TVOS_REVISION@");
		public readonly static ProductConstants macOS = new ProductConstants ("@MACOS_VERSION@", "@MACOS_REVISION@");
		public readonly static ProductConstants MacCatalyst = new ProductConstants ("@MACCATALYST_VERSION@", "@MACCATALYST_REVISION@");
#endif
	}
}
