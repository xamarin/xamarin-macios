namespace Xamarin.Tests {
	[TestFixture]
	public class TrimmerWarningsTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-x64")]
		public void TrimmerWarningsManagedStaticRegistrar (ApplePlatform platform, string runtimeIdentifiers)
		{
			// FIXME: dotnet/runtime#100256
			ExpectedBuildMessage [] expectedWarnings;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedWarnings = Array.Empty<ExpectedBuildMessage> ();
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				expectedWarnings = Array.Empty<ExpectedBuildMessage> ();
				break;
			default:
				Assert.Fail ($"Unknown platform: {platform}");
				return;
			}

			TrimmerWarnings (platform, runtimeIdentifiers, "managed-static", expectedWarnings);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void TrimmerWarningsStaticRegistrar (ApplePlatform platform, string runtimeIdentifiers)
		{
			// FIXME: dotnet/runtime#100256
			ExpectedBuildMessage [] expectedWarnings;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedWarnings = Array.Empty<ExpectedBuildMessage> ();
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				expectedWarnings = Array.Empty<ExpectedBuildMessage> ();
				break;
			default:
				Assert.Fail ($"Unknown platform: {platform}");
				return;
			}

			TrimmerWarnings (platform, runtimeIdentifiers, "static", expectedWarnings);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64")]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.TVOS, "tvos-arm64")]
		public void TrimmerWarningsDynamicRegistrar (ApplePlatform platform, string runtimeIdentifiers)
		{
			// FIXME: dotnet/runtime#100256
			ExpectedBuildMessage [] expectedWarnings;
			switch (platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				expectedWarnings = Array.Empty<ExpectedBuildMessage> ();
				break;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				expectedWarnings = Array.Empty<ExpectedBuildMessage> ();
				break;
			default:
				Assert.Fail ($"Unknown platform: {platform}");
				return;
			}

			TrimmerWarnings (platform, runtimeIdentifiers, "dynamic", expectedWarnings);
		}

		void TrimmerWarnings (ApplePlatform platform, string runtimeIdentifiers, string registrar, params ExpectedBuildMessage [] expectedWarnings)
		{
			// https://github.com/xamarin/xamarin-macios/issues/10405
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			var properties = GetDefaultProperties (runtimeIdentifiers);

			// Don't be shy, we want the warnings
			properties ["SuppressTrimAnalysisWarnings"] = "false";
			// And we want all of them!
			properties ["TrimmerSingleWarn"] = "false";

			// Select the registrar we want.
			properties ["Registrar"] = registrar;

			// The trimmer must trim at least the sdk assemblies for trimmer warnings to make sense,
			// otherwise it'll report warnings for features it would be told to swift off (and trim away).
			// Setting link mode to "Full" also works here, but I figure we won't get any warnings in "Full"
			// mode we wouldn't get in "SdkOnly" mode, but we could get warnings in "SdkOnly" that we wouldn't
			// get in "Full", so just use "SdkOnly" in the test for greater potential coverage (although
			// at the writing of this test it doesn't matter, either linker mode produce the same results).
			var linkMode = "SdkOnly";
			properties ["MtouchLink"] = linkMode;
			properties ["LinkMode"] = linkMode;

			// Sidenote to the previous point: the SdkOnly link mode produces the same results as the Full
			// link mode as long as neither Touch.Unit nor NUnitLite are included in LinkSdk, beacuse neiter
			// are anywhere close to trim-safe (they're both trimmed away in Full mode, so replicate that
			// in SdkOnly mode):
			properties ["ExcludeTouchUnitReference"] = "true";
			properties ["ExcludeNUnitLiteReference"] = "true";

			// Enable all optimizations
			var extraArgs = string.Empty;
			if (registrar != "dynamic") {
				// If we're not using the dynamic registrar, we want it gone.
				extraArgs = "--optimize:remove-dynamic-registrar";
			}
			properties ["MtouchExtraArgs"] = extraArgs;
			properties ["MonoBundlingExtraArgs"] = extraArgs;

			var rv = DotNet.AssertBuild (project_path, properties);

			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath)
								.Where (evt => {
									if (platform == ApplePlatform.iOS && evt.Message?.Trim () == "Supported iPhone orientations have not been set")
										return false;
									return true;
								});
			warnings.AssertWarnings (expectedWarnings);
		}
	}
}
