using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.MaciOS.Nnyeah.Tests.Integration {
	[TestFixture]
	public class IntegrationExamples {
		string IntegrationRoot => Path.Combine (Configuration.SourceRoot, "tools", "nnyeah", "integration");
		string Nnyeah => Path.Combine (Configuration.SourceRoot, "tools", "nnyeah", "nnyeah", "bin", "Debug", Configuration.DotNetTfm, "nnyeah.dll");
		string NnyeahNupkg => Path.Combine (Configuration.SourceRoot, "tools", "nnyeah", "nupkg");

		// TODO - This code, and passing xamarin-assembly/microsoft-assembly should be removed long term from nnyeah
		string GetLegacyPlatform (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX:
				return Configuration.GetBaseLibrary (TargetFramework.Xamarin_Mac_2_0_Mobile);
			case ApplePlatform.iOS:
				return Configuration.GetBaseLibrary (TargetFramework.Xamarin_iOS_1_0);
			default:
				throw new NotImplementedException ();
			}
		}

		string GetNetPlatform (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.MacOSX:
				return Configuration.GetBaseLibrary (TargetFramework.DotNet_macOS);
			case ApplePlatform.iOS:
				return Configuration.GetBaseLibrary (TargetFramework.DotNet_iOS);
			default:
				throw new NotImplementedException ();
			}
		}

		async Task AssertLegacyBuild (string project, ApplePlatform platform)
		{
			const string MSBuildPath = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild";

			var environment = Configuration.GetBuildEnvironment (platform);

			Execution execution = await Execution.RunAsync (MSBuildPath, new List<string> () { project }, environment, mergeOutput: true);
			var output = execution.StandardOutput?.ToString () ?? "";
			Assert.Zero (execution.ExitCode, $"Build Output: {output}");
		}

		void ExecuteNnyeah (string tmpDir, string inputPath, string convertedPath, ApplePlatform platform, bool useCannedLegacyPlatform = true)
		{
			var legacyPlatform = useCannedLegacyPlatform ? GetLegacyPlatform (platform) : null;
			AssemblyConverter.Convert (legacyPlatform, GetNetPlatform (platform), inputPath, convertedPath, true, true, false);
		}

		[Test]
		[TestCase ("API/macOSIntegration.csproj", "API/bin/Debug/macOSIntegration.dll", "Consumer/macOS/macOS.csproj", ApplePlatform.MacOSX, true)]
		[TestCase ("API/iOSIntegration.csproj", "API/bin/Debug/iOSIntegration.dll", "Consumer/ios/ios.csproj", ApplePlatform.iOS, true)]
		[TestCase ("API/macOSIntegration.csproj", "API/bin/Debug/macOSIntegration.dll", "Consumer/macOS/macOS.csproj", ApplePlatform.MacOSX, false)]
		[TestCase ("API/iOSIntegration.csproj", "API/bin/Debug/iOSIntegration.dll", "Consumer/ios/ios.csproj", ApplePlatform.iOS, false)]
		public async Task BuildAndRunSynthetic (string libraryProject, string libraryPath, string consumerProject, ApplePlatform platform,
			bool useCannedLegacyPlatform)
		{
			await AssertLegacyBuild (Path.Combine (IntegrationRoot, libraryProject), platform);

			string convertedDir = Path.Combine (IntegrationRoot, "API", "Converted");
			Directory.CreateDirectory (convertedDir);

			string inputPath = Path.Combine (IntegrationRoot, libraryPath);
			string convertedPath = Path.Combine (convertedDir, Path.GetFileName (libraryPath));

			var tmpDir = Cache.CreateTemporaryDirectory ("BuildAndRunSynthetic");

			ExecuteNnyeah (tmpDir, inputPath, convertedPath, platform, useCannedLegacyPlatform);

			DotNet.AssertBuild (Path.Combine (IntegrationRoot, consumerProject));
		}

		[Test]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.bundle_e_sqlite3/2.0.7/sqlitepclraw.bundle_e_sqlite3.2.0.7.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.batteries_v2.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.bundle_green/2.0.7/sqlitepclraw.bundle_green.2.0.7.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.batteries_v2.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/bouncycastle.netcore/1.8.10/bouncycastle.netcore.1.8.10.nupkg", "lib/xamarinios/BouncyCastle.Crypto.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.permissions/6.0.1/plugin.permissions.6.0.1.nupkg", "lib/xamarinios10/Plugin.Permissions.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.ffimageloading/2.4.11.982/xamarin.ffimageloading.2.4.11.982.nupkg", "lib/Xamarin.iOS10/FFImageLoading.Platform.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.ffimageloading/2.4.11.982/xamarin.ffimageloading.2.4.11.982.nupkg", "lib/Xamarin.Mac20/FFImageLoading.Platform.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/rg.plugins.popup/2.1.0/rg.plugins.popup.2.1.0.nupkg", "lib/xamarinios10/Rg.Plugins.Popup.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/rg.plugins.popup/2.1.0/rg.plugins.popup.2.1.0.nupkg", "lib/xamarinmac20/Rg.Plugins.Popup.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/acr.userdialogs/7.2.0.564/acr.userdialogs.7.2.0.564.nupkg", "lib/xamarinios10/Acr.UserDialogs.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/acr.userdialogs/7.2.0.564/acr.userdialogs.7.2.0.564.nupkg", "lib/xamarinmac20/Acr.UserDialogs.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.media/5.0.1/xam.plugin.media.5.0.1.nupkg", "lib/xamarinios10/Plugin.Media.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.ffimageloading.transformations/2.4.11.982/xamarin.ffimageloading.transformations.2.4.11.982.nupkg", "lib/Xamarin.iOS10/FFImageLoading.Transformations.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.ffimageloading.transformations/2.4.11.982/xamarin.ffimageloading.transformations.2.4.11.982.nupkg", "lib/Xamarin.Mac20/FFImageLoading.Transformations.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.connectivity/3.2.0/xam.plugin.connectivity.3.2.0.nupkg", "lib/Xamarin.iOS10/Plugin.Connectivity.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.connectivity/3.2.0/xam.plugin.connectivity.3.2.0.nupkg", "lib/Xamarin.Mac20/Plugin.Connectivity.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.settings/3.1.1/xam.plugins.settings.3.1.1.nupkg", "lib/Xamarin.iOS10/Plugin.Settings.Abstractions.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.settings/3.1.1/xam.plugins.settings.3.1.1.nupkg", "lib/Xamarin.Mac20/Plugin.Settings.Abstractions.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/pclcrypto/2.0.147/pclcrypto.2.0.147.nupkg", "lib/portable-net45%2Bwin%2Bwpa81%2Bwp80%2BMonoAndroid10%2Bxamarinios10%2BMonoTouch10/PCLCrypto.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross/8.0.2/mvvmcross.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross/8.0.2/mvvmcross.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/prism.plugin.popups/8.0.76/prism.plugin.popups.8.0.76.nupkg", "lib/xamarinios10/Prism.Plugin.Popups.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/fubarcoder.restsharp.portable.core/4.0.8/fubarcoder.restsharp.portable.core.4.0.8.nupkg", "lib/xamarinios10/FubarCoder.RestSharp.Portable.Core.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/fubarcoder.restsharp.portable.httpclient/4.0.8/fubarcoder.restsharp.portable.httpclient.4.0.8.nupkg", "lib/xamarinios10/FubarCoder.RestSharp.Portable.HttpClient.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/webp.touch/1.0.8/webp.touch.1.0.8.nupkg", "lib/Xamarin.iOS10/WebP.Touch.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/webp.touch/1.0.8/webp.touch.1.0.8.nupkg", "lib/Xamarin.Mac20/WebP.Mac.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/zxing.net.mobile/2.4.1/zxing.net.mobile.2.4.1.nupkg", "lib/Xamarin.iOS10/ZXingNetMobile.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.geolocator/4.5.0.6/xam.plugin.geolocator.4.5.0.6.nupkg", "lib/xamarinios10/Plugin.Geolocator.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.geolocator/4.5.0.6/xam.plugin.geolocator.4.5.0.6.nupkg", "lib/xamarinmac20/Plugin.Geolocator.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.provider.sqlite3.ios_unified/1.1.14/sqlitepclraw.provider.sqlite3.ios_unified.1.1.14.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.provider.sqlite3.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/modernhttpclient-updated/3.4.3/modernhttpclient-updated.3.4.3.nupkg", "lib/Xamarin.iOS10/ModernHttpClient.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.ffimageloading.svg/2.4.11.982/xamarin.ffimageloading.svg.2.4.11.982.nupkg", "lib/Xamarin.iOS10/FFImageLoading.Svg.Platform.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.ffimageloading.svg/2.4.11.982/xamarin.ffimageloading.svg.2.4.11.982.nupkg", "lib/Xamarin.Mac20/FFImageLoading.Svg.Platform.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/com.airbnb.ios.lottie/2.5.13/com.airbnb.ios.lottie.2.5.13.nupkg", "lib/xamarinios10/Lottie.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/com.airbnb.ios.lottie/2.5.13/com.airbnb.ios.lottie.2.5.13.nupkg", "lib/xamarinmac20/Lottie.iOS.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/ngraphics/0.5.0/ngraphics.0.5.0.nupkg", "lib/Xamarin.iOS10/NGraphics.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/ngraphics/0.5.0/ngraphics.0.5.0.nupkg", "lib/Xamarin.Mac20/NGraphics.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xlabs.core/2.0.5782/xlabs.core.2.0.5782.nupkg", "lib/Xamarin.iOS10/XLabs.Core.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xlabs.serialization/2.0.5782/xlabs.serialization.2.0.5782.nupkg", "lib/Xamarin.iOS10/XLabs.Serialization.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xlabs.platform/2.0.5782/xlabs.platform.2.0.5782.nupkg", "lib/Xamarin.iOS10/XLabs.Platform.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/portable.dataannotations/1.0.0/portable.dataannotations.1.0.0.nupkg", "lib/Xamarin.iOS10/Portable.DataAnnotations.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/portable.dataannotations/1.0.0/portable.dataannotations.1.0.0.nupkg", "lib/Xamarin.Mac20/Portable.DataAnnotations.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/modernhttpclient/2.4.2/modernhttpclient.2.4.2.nupkg", "lib/Xamarin.iOS10/ModernHttpClient.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.deviceinfo/4.1.1/xam.plugin.deviceinfo.4.1.1.nupkg", "lib/xamarinios10/Plugin.DeviceInfo.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.deviceinfo/4.1.1/xam.plugin.deviceinfo.4.1.1.nupkg", "lib/xamarinmac20/Plugin.DeviceInfo.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.texttospeech/4.0.0.7/xam.plugins.texttospeech.4.0.0.7.nupkg", "lib/xamarinios10/Plugin.TextToSpeech.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.texttospeech/4.0.0.7/xam.plugins.texttospeech.4.0.0.7.nupkg", "lib/xamarinmac20/Plugin.TextToSpeech.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.platform/5.7.0/mvvmcross.platform.5.7.0.nupkg", "lib/Xamarin.iOS10/MvvmCross.Platform.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.platform/5.7.0/mvvmcross.platform.5.7.0.nupkg", "lib/Xamarin.Mac20/MvvmCross.Platform.Mac.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight/6.0.5000.975/intersoft.crosslight.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.core/5.7.0/mvvmcross.core.5.7.0.nupkg", "lib/Xamarin.iOS10/MvvmCross.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.core/5.7.0/mvvmcross.core.5.7.0.nupkg", "lib/Xamarin.Mac20/MvvmCross.Mac.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.visibility/8.0.2/mvvmcross.plugin.visibility.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.Visibility.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.visibility/8.0.2/mvvmcross.plugin.visibility.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.Visibility.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.binding/5.7.0/mvvmcross.binding.5.7.0.nupkg", "lib/Xamarin.iOS10/MvvmCross.Binding.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.binding/5.7.0/mvvmcross.binding.5.7.0.nupkg", "lib/Xamarin.Mac20/MvvmCross.Binding.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlite.net-pcl/3.1.1/sqlite.net-pcl.3.1.1.nupkg", "lib/Xamarin.iOS10/SQLite.Net.Platform.XamarinIOS.Unified.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.fingerprint/2.1.5/plugin.fingerprint.2.1.5.nupkg", "lib/xamarinios10/Plugin.Fingerprint.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.fingerprint/2.1.5/plugin.fingerprint.2.1.5.nupkg", "lib/xamarinmac20/Plugin.Fingerprint.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.messaging/5.2.0/xam.plugins.messaging.5.2.0.nupkg", "lib/Xamarin.iOS10/Plugin.Messaging.Abstractions.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.provider.internal.ios_unified/1.1.14/sqlitepclraw.provider.internal.ios_unified.1.1.14.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.provider.internal.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/acr.support/2.1.0/acr.support.2.1.0.nupkg", "lib/Xamarin.iOS10/Acr.Support.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.plugin.filepicker/2.1.41/xamarin.plugin.filepicker.2.1.41.nupkg", "lib/xamarinios10/Plugin.FilePicker.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.plugin.filepicker/2.1.41/xamarin.plugin.filepicker.2.1.41.nupkg", "lib/xamarinmac20/Plugin.FilePicker.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.file/7.0.1/mvvmcross.plugin.file.7.0.1.nupkg", "lib/xamarinios10/MvvmCross.Plugin.File.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.file/7.0.1/mvvmcross.plugin.file.7.0.1.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.File.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/emgu.cv/4.5.5.4823/emgu.cv.4.5.5.4823.nupkg", "lib/xamarinios10/Emgu.CV.Platform.NetStandard.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/cardsview/2.8.1/cardsview.2.8.1.nupkg", "lib/Xamarin.iOS10/PanCardView.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/cardsview/2.8.1/cardsview.2.8.1.nupkg", "lib/Xamarin.Mac20/PanCardView.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/serilog.sinks.xamarin/0.2.0.64/serilog.sinks.xamarin.0.2.0.64.nupkg", "lib/Xamarin.iOS10/Serilog.Sinks.Xamarin.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/serilog.sinks.xamarin/0.2.0.64/serilog.sinks.xamarin.0.2.0.64.nupkg", "lib/Xamarin.Mac20/Serilog.Sinks.Xamarin.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/engineioclientdotnet/0.9.22/engineioclientdotnet.0.9.22.nupkg", "lib/xamarinios10/EngineIoClientDotNet.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.color/8.0.2/mvvmcross.plugin.color.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.Color.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.color/8.0.2/mvvmcross.plugin.color.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.Color.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/socketioclientdotnet/0.9.13/socketioclientdotnet.0.9.13.nupkg", "lib/xamarinios10/SocketIoClientDotNet.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/anyline.xamarin.sdk.ios/39.0.0/anyline.xamarin.sdk.ios.39.0.0.nupkg", "lib/Xamarin.iOS10/AnylineXamarinSDK.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/zebble/4.5.630/zebble.4.5.630.nupkg", "lib/xamarinios10/Zebble.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/protogame/6.0.4/protogame.6.0.4.nupkg", "lib/xamarinios/Protogame.dll.mdb", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/protogame/6.0.4/protogame.6.0.4.nupkg", "lib/xamarinmac/Protogame.dll.mdb", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.lib.e_sqlite3.ios/2.0.7/sqlitepclraw.lib.e_sqlite3.ios.2.0.7.nupkg", "lib/xamarinios10/SQLitePCLRaw.lib.e_sqlite3.ios.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepcl.raw/0.9.2/sqlitepcl.raw.0.9.2.nupkg", "lib/portable-net45%2Bnetcore45%2Bwp8%2BMonoAndroid10%2BMonoTouch10%2BXamarin.iOS10/SQLitePCL.raw.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.lib.e_sqlite3.ios_unified.static/1.1.14/sqlitepclraw.lib.e_sqlite3.ios_unified.static.1.1.14.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.lib.e_sqlite3.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepcl.raw_basic/0.8.5/sqlitepcl.raw_basic.0.8.5.nupkg", "lib/portable-net45%2Bnetcore45%2Bwp8%2BMonoAndroid10%2BMonoTouch10%2BXamarin.iOS10/SQLitePCL.raw.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepcl.raw_basic/0.8.5/sqlitepcl.raw_basic.0.8.5.nupkg", "build/Xamarin.Mac20/pinvoke_sqlite3/anycpu/SQLitePCL.raw.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/cirrious.fluentlayout/2.9.0/cirrious.fluentlayout.2.9.0.nupkg", "lib/Xamarin.iOS10/Cirrious.FluentLayouts.Touch.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/lextm.sharpsnmplib/12.4.0/lextm.sharpsnmplib.12.4.0.nupkg", "lib/xamarinios10/SharpSnmpLib.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.share/7.1.1/plugin.share.7.1.1.nupkg", "lib/Xamarin.iOS10/Plugin.Share.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/appdynamics.agent/2022.5.0/appdynamics.agent.2022.5.0.nupkg", "lib/xamarinios10/AppDynamics.Agent.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/microcharts/0.9.5.9/microcharts.0.9.5.9.nupkg", "lib/xamarinios1.0/Microcharts.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/microcharts/0.9.5.9/microcharts.0.9.5.9.nupkg", "lib/xamarinmac2.0/Microcharts.macOS.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/acr.deviceinfo/6.5.0/acr.deviceinfo.6.5.0.nupkg", "lib/xamarinios10/Plugin.DeviceInfo.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.bundle_sqlcipher/1.1.14/sqlitepclraw.bundle_sqlcipher.1.1.14.nupkg", "lib/portable-net45+netcore45+wpa81+MonoAndroid10+MonoTouch10+Xamarin.iOS10/SQLitePCLRaw.batteries_v2.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.bundle_sqlcipher/1.1.14/sqlitepclraw.bundle_sqlcipher.1.1.14.nupkg", "lib/Xamarin.Mac20/SQLitePCLRaw.batteries_v2.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.iconize/3.5.0.129/xam.plugin.iconize.3.5.0.129.nupkg", "lib/xamarinios10/Plugin.Iconize.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.simpleaudioplayer/1.6.0/xam.plugin.simpleaudioplayer.1.6.0.nupkg", "lib/Xamarin.iOS10/Plugin.SimpleAudioPlayer.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.simpleaudioplayer/1.6.0/xam.plugin.simpleaudioplayer.1.6.0.nupkg", "lib/Xamarin.Mac20/Plugin.SimpleAudioPlayer.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.externalmaps/4.0.1/xam.plugin.externalmaps.4.0.1.nupkg", "lib/Xamarin.iOS10/Plugin.ExternalMaps.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.externalmaps/4.0.1/xam.plugin.externalmaps.4.0.1.nupkg", "lib/Xamarin.Mac20/Plugin.ExternalMaps.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.bundle_e_sqlcipher/2.0.7/sqlitepclraw.bundle_e_sqlcipher.2.0.7.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.batteries_v2.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.ble/2.1.3/plugin.ble.2.1.3.nupkg", "lib/Xamarin.iOS10/Plugin.BLE.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.ble/2.1.3/plugin.ble.2.1.3.nupkg", "lib/xamarinmac20/Plugin.BLE.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.logging/6.0.5000.975/intersoft.crosslight.logging.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.Logging.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/com.onesignal/3.10.6/com.onesignal.3.10.6.nupkg", "lib/Xamarin.iOS10/Com.OneSignal.Abstractions.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sameeriotapps.plugin.securestorage/2.5.0/sameeriotapps.plugin.securestorage.2.5.0.nupkg", "lib/Xamarin.iOS10/Plugin.SecureStorage.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.webbrowser/8.0.2/mvvmcross.plugin.webbrowser.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.WebBrowser.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.webbrowser/8.0.2/mvvmcross.plugin.webbrowser.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.WebBrowser.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.data.sqlite/6.0.5000.975/intersoft.crosslight.data.sqlite.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.Data.SQLite.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.mediamanager/1.1.1/plugin.mediamanager.1.1.1.nupkg", "lib/xamarinios10/MediaManager.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.mediamanager/1.1.1/plugin.mediamanager.1.1.1.nupkg", "lib/xamarinmac20/MediaManager.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/acr.core/3.0.1/acr.core.3.0.1.nupkg", "lib/xamarinios10/Acr.Core.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/acr.core/3.0.1/acr.core.3.0.1.nupkg", "lib/xamarinmac20/Acr.Core.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.firebasepushnotification/3.4.25/plugin.firebasepushnotification.3.4.25.nupkg", "lib/xamarinios10/Plugin.FirebasePushNotification.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/branch-xamarin-linking-sdk/8.0.0/branch-xamarin-linking-sdk.8.0.0.nupkg", "lib/Xamarin.iOS10/Branch-Xamarin-SDK.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/dannycabrera.getiosmodel/1.17.0/dannycabrera.getiosmodel.1.17.0.nupkg", "lib/xamarinios10/GetiOSModel.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.notifier/3.0.1/xam.plugins.notifier.3.0.1.nupkg", "lib/Xamarin.iOS10/Plugin.LocalNotifications.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugins.notifier/3.0.1/xam.plugins.notifier.3.0.1.nupkg", "lib/Xamarin.Mac20/Plugin.LocalNotifications.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.multilingual/1.0.2/plugin.multilingual.1.0.2.nupkg", "lib/Xamarin.iOS10/Plugin.Multilingual.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.multilingual/1.0.2/plugin.multilingual.1.0.2.nupkg", "lib/Xamarin.Mac20/Plugin.Multilingual.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/csj2k/3.0.0/csj2k.3.0.0.nupkg", "lib/Xamarin.iOS10/CSJ2K.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.services.social/6.0.5000.975/intersoft.crosslight.services.social.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.Services.Social.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.services.pushnotification/6.0.5000.975/intersoft.crosslight.services.pushnotification.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.Services.PushNotification.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/monogame.framework.redpoint/3.5.0/monogame.framework.redpoint.3.5.0.nupkg", "lib/xamarinios/_AutomaticExternals/ThirdParty/Dependencies/Tests/nunit.core.interfaces.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/monogame.framework.redpoint/3.5.0/monogame.framework.redpoint.3.5.0.nupkg", "lib/xamarinmac/MonoGame.Framework.Content.Pipeline.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepcl/3.8.7.2/sqlitepcl.3.8.7.2.nupkg", "lib/portable-net45%2Bsl50%2Bwin%2Bwpa81%2Bwp80%2BMonoAndroid10%2Bxamarinios10%2BMonoTouch10/SQLitePCL.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepcl/3.8.7.2/sqlitepcl.3.8.7.2.nupkg", "lib/Xamarin.iOS10/SQLitePCL.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.inappbilling/4.0.2/plugin.inappbilling.4.0.2.nupkg", "lib/xamarinios10/Plugin.InAppBilling.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.picturechooser/8.0.2/mvvmcross.plugin.picturechooser.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.PictureChooser.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.picturechooser/8.0.2/mvvmcross.plugin.picturechooser.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.PictureChooser.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.badge/2.3.1/plugin.badge.2.3.1.nupkg", "lib/Xamarin.iOS10/Plugin.Badge.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.badge/2.3.1/plugin.badge.2.3.1.nupkg", "lib/xamarinmac20/Plugin.Badge.Abstractions.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.iconize.fontawesome/3.5.0.129/xam.plugin.iconize.fontawesome.3.5.0.129.nupkg", "lib/xamarinios10/Plugin.Iconize.FontAwesome.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.services.barcode/6.0.5000.975/intersoft.crosslight.services.barcode.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.Services.Barcode.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.deviceorientation/1.0.7/plugin.deviceorientation.1.0.7.nupkg", "lib/Xamarin.iOS10/Plugin.DeviceOrientation.Abstractions.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.services.reporting/6.0.5000.975/intersoft.crosslight.services.reporting.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.Services.Reporting.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.ui.datavisualization/6.0.5000.975/intersoft.crosslight.ui.datavisualization.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.UI.DataVisualization.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/ncontrol/0.9.1/ncontrol.0.9.1.nupkg", "lib/Xamarin.iOS10/NControl.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mr.gestures/2.2.1/mr.gestures.2.2.1.nupkg", "lib/Xamarin.iOS10/MR.Gestures.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mr.gestures/2.2.1/mr.gestures.2.2.1.nupkg", "lib/xamarinmac20/MR.Gestures.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/intersoft.crosslight.ui/6.0.5000.975/intersoft.crosslight.ui.6.0.5000.975.nupkg", "lib/Xamarin.iOS10/Intersoft.Crosslight.UI.Core.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/oxyplot.xamarin.ios/1.0.0/oxyplot.xamarin.ios.1.0.0.nupkg", "lib/Xamarin.iOS10/OxyPlot.Xamarin.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/slideoverkit/2.1.6.2/slideoverkit.2.1.6.2.nupkg", "lib/Xamarin.iOS10/SlideOverKit.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/shiny.core/2.5.1/shiny.core.2.5.1.nupkg", "lib/xamarinios10/Shiny.Core.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.resourceloader/8.0.2/mvvmcross.plugin.resourceloader.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.ResourceLoader.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.resourceloader/8.0.2/mvvmcross.plugin.resourceloader.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.ResourceLoader.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/thinkgeo.dependency.skiasharp/12.3.0/thinkgeo.dependency.skiasharp.12.3.0.nupkg", "lib/xamarinios1.0/SkiaSharp.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/thinkgeo.dependency.skiasharp/12.3.0/thinkgeo.dependency.skiasharp.12.3.0.nupkg", "lib/xamarinmac2.0/SkiaSharp.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xunit.runner.devices/2.5.25/xunit.runner.devices.2.5.25.nupkg", "lib/xamarinios10/xunit.runner.devices.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sharpnado.shadows/1.2.0/sharpnado.shadows.1.2.0.nupkg", "lib/Xamarin.iOS10/Sharpnado.Shadows.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/keystone/5.4.0/keystone.5.4.0.nupkg", "lib/Xamarin.iOS10/Keystone.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/keystone/5.4.0/keystone.5.4.0.nupkg", "lib/Xamarin.Mac20/Keystone.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sqlitepclraw.lib.sqlcipher.ios_unified.static/1.1.14/sqlitepclraw.lib.sqlcipher.ios_unified.static.1.1.14.nupkg", "lib/Xamarin.iOS10/SQLitePCLRaw.lib.sqlcipher.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xamarin.iqkeyboardmanager/1.4.1/xamarin.iqkeyboardmanager.1.4.1.nupkg", "lib/Xamarin.iOS10/IQKeyboardManager.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.downloadcache/5.7.0/mvvmcross.plugin.downloadcache.5.7.0.nupkg", "lib/Xamarin.iOS10/MvvmCross.Plugins.DownloadCache.iOS.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.downloadcache/5.7.0/mvvmcross.plugin.downloadcache.5.7.0.nupkg", "lib/Xamarin.Mac20/MvvmCross.Plugins.DownloadCache.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.storereview/3.3.1/plugin.storereview.3.3.1.nupkg", "lib/xamarinios10/Plugin.StoreReview.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.storereview/3.3.1/plugin.storereview.3.3.1.nupkg", "lib/xamarinmac20/Plugin.StoreReview.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/rda.socketsforpcl/2.0.2/rda.socketsforpcl.2.0.2.nupkg", "lib/Xamarin.iOS10/Sockets.Plugin.Abstractions.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/virgil.crypto/2.4.10/virgil.crypto.2.4.10.nupkg", "lib/xamarinios/Virgil.Crypto.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.location/8.0.2/mvvmcross.plugin.location.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.Location.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.location/8.0.2/mvvmcross.plugin.location.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.Location.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/sidebarnavigation/2.1.0/sidebarnavigation.2.1.0.nupkg", "lib/xamarinios10/SidebarNavigation.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/ndesk.options.redpoint/0.2.1/ndesk.options.redpoint.0.2.1.nupkg", "lib/xamarinios/NDesk.Options.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/ndesk.options.redpoint/0.2.1/ndesk.options.redpoint.0.2.1.nupkg", "lib/xamarinmac/NDesk.Options.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.phonecall/8.0.2/mvvmcross.plugin.phonecall.8.0.2.nupkg", "lib/xamarinios10/MvvmCross.Plugin.PhoneCall.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/mvvmcross.plugin.phonecall/8.0.2/mvvmcross.plugin.phonecall.8.0.2.nupkg", "lib/xamarinmac20/MvvmCross.Plugin.PhoneCall.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/cclarke.plugin.calendars/1.1.0/cclarke.plugin.calendars.1.1.0.nupkg", "lib/Xamarin.iOS10/Plugin.Calendars.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.iconize.material/3.5.0.129/xam.plugin.iconize.material.3.5.0.129.nupkg", "lib/xamarinios10/Plugin.Iconize.Material.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/urbanairship.netstandard/15.0.3/urbanairship.netstandard.15.0.3.nupkg", "lib/Xamarin.iOS10/AirshipBindings.NETStandard.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.latestversion/2.1.0/xam.plugin.latestversion.2.1.0.nupkg", "lib/xamarinios10/Plugin.LatestVersion.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/xam.plugin.latestversion/2.1.0/xam.plugin.latestversion.2.1.0.nupkg", "lib/xamarinmac20/Plugin.LatestVersion.dll", ApplePlatform.MacOSX)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/couchbase.lite/3.0.0/couchbase.lite.3.0.0.nupkg", "lib/xamarinios/Couchbase.Lite.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/uno.ui.maps/4.2.6/uno.ui.maps.4.2.6.nupkg", "lib/xamarinios10/Uno.UI.Maps.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.bluetoothle/6.3.0.19/plugin.bluetoothle.6.3.0.19.nupkg", "lib/xamarinios10/Plugin.BluetoothLE.dll", ApplePlatform.iOS)]
		[TestCase ("https://api.nuget.org/v3-flatcontainer/plugin.bluetoothle/6.3.0.19/plugin.bluetoothle.6.3.0.19.nupkg", "lib/xamarinmac20/Plugin.BluetoothLE.dll", ApplePlatform.MacOSX)]
		public async Task NugetExamples (string downloadUrl, string libraryPath, ApplePlatform platform)
		{
			var tmpDir = Cache.CreateTemporaryDirectory ("NugetExamples");

			// Remove the leading / from the download uri
			var nupkgPath = Path.Combine (tmpDir, downloadUrl.Split ('/').Last ());
			var nugetPath = Path.Combine (tmpDir, libraryPath);

			using (var client = new HttpClient ()) {
				var response = await client.GetAsync (downloadUrl);
				var fs = new FileStream (nupkgPath, FileMode.CreateNew);
				await response.Content.CopyToAsync (fs);
			}
			System.IO.Compression.ZipFile.ExtractToDirectory (nupkgPath, tmpDir);

			string convertedDir = Path.Combine (tmpDir, "Converted");
			Directory.CreateDirectory (convertedDir);

			string convertedPath = Path.Combine (convertedDir, Path.GetFileName (libraryPath));

			ExecuteNnyeah (tmpDir, nugetPath, convertedPath, platform);
		}
	}
}
