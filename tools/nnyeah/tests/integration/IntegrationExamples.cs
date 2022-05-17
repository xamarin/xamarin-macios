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
		string Nnyeah => Path.Combine (Configuration.SourceRoot, "tools", "nnyeah", "nnyeah", "bin", "Debug", "net6.0", "nnyeah.dll");

		// TODO - This code, and passing xamarin-assembly/microsoft-assembly should be removed long term from nnyeah
		string GetLegacyPlatform (ApplePlatform platform)
		{
			switch (platform)
			{
				case ApplePlatform.MacOSX:
					return Configuration.GetBaseLibrary (TargetFramework.Xamarin_Mac_2_0_Mobile);
				case ApplePlatform.iOS:
					return Configuration.GetBaseLibrary (TargetFramework.Xamarin_iOS_1_0);
				default:
					throw new NotImplementedException ();
			}
		}

		string GetNETPlatform (ApplePlatform platform)
		{
			switch (platform)
			{
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

			Execution execution = await Execution.RunAsync (MSBuildPath, new List<string>() { project }, environment, mergeOutput: true);
			Assert.Zero (execution.ExitCode, $"Build Output: {execution.StandardOutput}");
		}

		// [Test]
		// [TestCase("API/macOSIntegration.csproj", "API/bin/Debug/macOSIntegration.dll", "Consumer/macOS/macOS.csproj", ApplePlatform.MacOSX)]
		// [TestCase("API/iOSIntegration.csproj", "API/bin/Debug/iOSIntegration.dll", "Consumer/ios/ios.csproj", ApplePlatform.iOS)]
		public async Task BuildAndRunSynthetic (string libraryProject, string libraryPath, string consumerProject, ApplePlatform platform)
		{
			await AssertLegacyBuild (Path.Combine (IntegrationRoot, libraryProject), platform);

			string convertedDir = Path.Combine (IntegrationRoot, "API", "Converted");
			Directory.CreateDirectory (convertedDir);

			string inputPath = Path.Combine (IntegrationRoot, libraryPath);
			string convertedPath = Path.Combine (convertedDir, Path.GetFileName (libraryPath));

			var args = new List<string> { Nnyeah, $"--input={inputPath}", $"--output={convertedPath}", $"--xamarin-assembly={GetLegacyPlatform(platform)}", 
				$"--microsoft-assembly={GetNETPlatform(platform)}", "--force-overwrite"};
			Execution execution = await Execution.RunAsync (DotNet.Executable, args, null, mergeOutput: true);
			Assert.Zero (execution.ExitCode, $"Nnyeah Output: {execution.StandardOutput}");

			DotNet.AssertBuild (Path.Combine (IntegrationRoot, consumerProject));
		}

		// [Test]
		// [TestCase("https://api.nuget.org/v3-flatcontainer/xamarin.forms.googlemaps/3.3.0/xamarin.forms.googlemaps.3.3.0.nupkg", "lib/Xamarin.iOS10/Xamarin.Forms.GoogleMaps.iOS.dll", ApplePlatform.iOS)]
		public async Task NugetExamples (string downloadUrl, string libraryPath, ApplePlatform platform)
		{
			var dir = Cache.CreateTemporaryDirectory ("NugetExamples");

			// Remove the leading / from the download uri
			var nupkgPath = Path.Combine (dir, downloadUrl.Split ('/').Last());
			var nugetPath = Path.Combine (dir, libraryPath);

			using (var client = new HttpClient()) {
				var response = await client.GetAsync (downloadUrl);
				var fs = new FileStream (nupkgPath, FileMode.CreateNew);
				await response.Content.CopyToAsync(fs);
			}
			System.IO.Compression.ZipFile.ExtractToDirectory (nupkgPath, dir);

			string convertedDir = Path.Combine (dir, "Converted");
			Directory.CreateDirectory (convertedDir);

			string convertedPath = Path.Combine (convertedDir, Path.GetFileName (libraryPath));

			var args = new List<string> { Nnyeah, $"--input={nugetPath}", $"--output={convertedPath}", $"--xamarin-assembly={GetLegacyPlatform(platform)}", 
				$"--microsoft-assembly={GetNETPlatform(platform)}", "--force-overwrite"};
			Execution execution = await Execution.RunAsync (DotNet.Executable, args, null, mergeOutput: true);
			Assert.Zero (execution.ExitCode, $"Nnyeah Output: {execution.StandardOutput}");
		}
	}
}
