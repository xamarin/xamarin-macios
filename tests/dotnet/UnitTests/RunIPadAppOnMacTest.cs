// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;

namespace Xamarin.Tests;

[TestFixture]
public class RunIPadAppOnMacTest {
	[SetUp]
	public void SetUp ()
	{
		if (!OperatingSystem.IsMacOS () || RuntimeInformation.OSArchitecture != Architecture.Arm64)
			Assert.Ignore ("Requires an Apple silicon Mac.");
		Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
	}

	string CreateProject (out string directory, string deviceFamilies = "<integer>2</integer>", string runtimeIdentifier = "ios-arm64")
	{
		directory = Cache.CreateTemporaryDirectory ();
		var project = Path.Combine (directory, "Test.csproj");
		File.WriteAllText (Path.Combine (directory, "Info.plist"), $"""
			<?xml version="1.0" encoding="UTF-8"?>
			<plist version="1.0">
			<dict>
				<key>CFBundleIdentifier</key><string>com.xamarin.ipadonmac</string>
				<key>CFBundleExecutable</key><string>Test App</string>
				<key>MinimumOSVersion</key><string>15.0</string>
				<key>UIDeviceFamily</key><array>{deviceFamilies}</array>
			</dict>
			</plist>
			""");
		File.WriteAllText (project, $$"""
			<Project>
				<Import Project="Sdk.props" Sdk="Microsoft.NET.Sdk" />
				<PropertyGroup>
					<TargetFramework>{{Configuration.DotNetTfm}}-ios</TargetFramework>
					<UseFloatingTargetPlatformVersion>true</UseFloatingTargetPlatformVersion>
					<SupportedOSPlatformVersion>15.0</SupportedOSPlatformVersion>
					<OutputType>Exe</OutputType>
					<RuntimeIdentifier>{{runtimeIdentifier}}</RuntimeIdentifier>
					<StandardOutputPath>$(MSBuildProjectDirectory)/stdout.txt</StandardOutputPath>
					<StandardErrorPath>$(MSBuildProjectDirectory)/stderr.txt</StandardErrorPath>
				</PropertyGroup>
				<ItemGroup>
					<RuntimeEnvironmentVariable Include="IPAD_TEST_VARIABLE" Value="value with spaces" />
				</ItemGroup>
				<Import Project="Sdk.targets" Sdk="Microsoft.NET.Sdk" />
				<Target Name="SetFixtureBundlePaths" AfterTargets="_GenerateBundleName">
					<PropertyGroup>
						<_AppBundleName>Test App</_AppBundleName>
						<_AppBundlePath>$(MSBuildProjectDirectory)/input/Test App.app/</_AppBundlePath>
						<_AppBundleManifestPath>$(_AppBundlePath)Info.plist</_AppBundleManifestPath>
					</PropertyGroup>
				</Target>
				<!-- Create an unsigned fixture bundle instead of compiling an app or requiring signing credentials. -->
				<Target Name="Build" DependsOnTargets="_ComputeTargetFrameworkMoniker;_GenerateBundleName">
					<Error Condition="'$(FailBuild)' == 'true'" Text="Intentional build failure." />
					<MakeDir Directories="$(_AppBundlePath)" />
					<Copy SourceFiles="Info.plist" DestinationFiles="$(_AppBundleManifestPath)" />
					<WriteLinesToFile File="$(_AppBundlePath)Test App" Lines="fixture executable" Overwrite="true" />
					<WriteLinesToFile File="build-ran.txt" Lines="Build" />
				</Target>
				<Target Name="RecordWrapper" AfterTargets="_PrepareIPadAppOnMac;_PrepareRunIPadAppOnMac">
					<WriteLinesToFile File="wrapper-path.txt" Lines="$(_IPadAppOnMacBundle)" Overwrite="true" />
					<WriteLinesToFile File="wrapper-files.txt" Lines="@(FileWrites->'%(FullPath)')" Overwrite="true" />
					<WriteLinesToFile File="run-command.txt" Lines="$(RunCommand)" Overwrite="true" />
					<WriteLinesToFile File="run-arguments.txt" Lines="$(RunArguments)" Overwrite="true" />
					<WriteLinesToFile File="run-directory.txt" Lines="$(RunWorkingDirectory)" Overwrite="true" />
				</Target>
				<Target Name="KeepOnlyMacDestination">
					<ItemGroup>
						<Devices Remove="@(Devices)" Condition="'%(Identity)' != '$(_IPadAppOnMacDeviceName)'" />
					</ItemGroup>
				</Target>
				<Target Name="RecordDevices" AfterTargets="ComputeAvailableDevices;KeepOnlyMacDestination">
					<WriteLinesToFile File="devices.txt" Lines="@(Devices->'%(Identity)|%(Description)|%(Type)|%(UDID)|%(RuntimeIdentifier)')" Overwrite="true" />
				</Target>
			</Project>
			""");
		return project;
	}

	[TestCase ("<integer>2</integer>")]
	[TestCase ("<integer>1</integer><integer>2</integer>")]
	public void BuildsAndWrapsApp (string deviceFamilies)
	{
		var project = CreateProject (out var directory, deviceFamilies);
		DotNet.AssertBuild (project, target: "_PrepareIPadAppOnMac");
		Assert.That (Path.Combine (directory, "build-ran.txt"), Does.Exist, "Build must run before wrapping");

		var wrapper = Path.Combine (directory, File.ReadAllText (Path.Combine (directory, "wrapper-path.txt")).Trim ());
		var wrappedApp = Path.Combine (wrapper, "Wrapper", "Test App.app");
		Assert.That (new DirectoryInfo (Path.Combine (wrapper, "WrappedBundle")).LinkTarget, Is.EqualTo ("Wrapper/Test App.app"), "relative bundle symlink");
		foreach (var name in new [] { "Info.plist", "Test App" }) {
			var original = Path.Combine (directory, "input", "Test App.app", name);
			var copy = Path.Combine (wrappedApp, name);
			Assert.That (File.ReadAllBytes (copy), Is.EqualTo (File.ReadAllBytes (original)), name);
			Assert.That (File.ReadAllLines (Path.Combine (directory, "wrapper-files.txt")), Does.Contain (copy), $"{name} FileWrites");
		}
		Assert.That (File.ReadAllLines (Path.Combine (directory, "wrapper-files.txt")), Does.Contain (Path.Combine (wrapper, "WrappedBundle")), "symlink FileWrites");

		DotNet.AssertBuild (project, target: "_PrepareIPadAppOnMac");
		var secondWrapper = Path.Combine (directory, File.ReadAllText (Path.Combine (directory, "wrapper-path.txt")).Trim ());
		Assert.That (secondWrapper, Is.Not.EqualTo (wrapper), "each invocation must create a fresh wrapper");
		Assert.That (Directory.Exists (wrapper), Is.True, "previous wrapper must not be modified");

		DotNet.AssertBuild (project, target: "Clean");
		Assert.That (Directory.Exists (wrapper), Is.False, "first wrapper cleaned");
		Assert.That (Directory.Exists (secondWrapper), Is.False, "second wrapper cleaned");
	}

	[TestCase ("ios-arm64")]
	[TestCase ("")]
	public void ComputesRunArgumentsWithoutBuildingOrDeploying (string runtimeIdentifier)
	{
		var project = CreateProject (out var directory, runtimeIdentifier: runtimeIdentifier);
		DotNet.AssertBuild (project);
		var properties = new Dictionary<string, string> {
			{ "Device", "My Mac (Designed for iPad)" },
			{ "FailBuild", "true" },
			{ "MlaunchPath", "/must-not-launch-a-device" },
			{ "StandardInputPath", Path.Combine (directory, "input.txt") },
		};
		DotNet.AssertBuild (project, properties, target: "DeployToDevice;ComputeRunArguments");

		Assert.That (File.ReadAllLines (Path.Combine (directory, "build-ran.txt")), Has.Length.EqualTo (1), "must reuse the existing build");
		Assert.That (File.ReadAllText (Path.Combine (directory, "run-command.txt")).Trim (), Is.EqualTo ("/bin/sh"), "launcher");
		Assert.That (File.ReadAllText (Path.Combine (directory, "run-directory.txt")).Trim (), Is.EqualTo (directory), "working directory");
		var arguments = File.ReadAllText (Path.Combine (directory, "run-arguments.txt")).Trim ();
		Assert.That (arguments, Does.EndWith ("launch.sh\""), "launcher arguments");
		var launcher = Path.Combine (directory, arguments.Trim ('"'));
		Assert.That (File.ReadAllLines (Path.Combine (directory, "wrapper-files.txt")), Does.Contain (launcher), "launcher FileWrites");
		var script = File.ReadAllText (launcher);
		Assert.That (script, Does.Contain ("\"--app-arg=$arg\""), "forward managed application arguments");
		Assert.That (script, Does.Contain ("exec /usr/bin/open -n -W"), "wait for exit by default");
		Assert.That (script, Does.Contain ($"--stdout \"{directory}/stdout.txt\""), "stdout override");
		Assert.That (script, Does.Contain ($"--stderr \"{directory}/stderr.txt\""), "stderr override");
		Assert.That (script, Does.Contain ($"--stdin \"{directory}/input.txt\""), "stdin override");
		Assert.That (script, Does.Contain ("--env \"IPAD_TEST_VARIABLE=value with spaces\""), "runtime environment");
	}

	[Test]
	public void MlaunchDoesNotSelectMacAsPhysicalDevice ()
	{
		var project = CreateProject (out var directory);
		var properties = new Dictionary<string, string> {
			{ "MlaunchPath", "/must-not-launch-a-device" },
		};
		var result = DotNet.Execute ("build", project, properties, assert_success: false, target: "Build;ComputeAvailableDevices;KeepOnlyMacDestination;_ComputeMlaunchRunArguments");
		Assert.That (File.ReadAllLines (Path.Combine (directory, "devices.txt")), Has.Length.EqualTo (1), "only the Mac destination remains");
		Assert.That (result.ExitCode, Is.Not.EqualTo (0), "must not compute physical-device arguments for the Mac");
		Assert.That (result.StandardOutput + result.StandardError, Does.Contain ("No applicable and available devices found."), "no physical device");
	}

	[TestCase ("OutputType", "Library")]
	[TestCase ("IsAppExtension", "true")]
	[TestCase ("IsWatchApp", "true")]
	public void DoesNotDiscoverMacForUnsupportedProjects (string property, string value)
	{
		var project = CreateProject (out var directory);
		var properties = new Dictionary<string, string> {
			{ property, value },
		};
		DotNet.AssertBuild (project, properties, target: "ComputeAvailableDevices");
		Assert.That (File.ReadAllText (Path.Combine (directory, "devices.txt")), Does.Not.Contain ("My Mac (Designed for iPad)"), "unsupported project");
	}

	[TestCase ("<integer>2</integer>", "ios-arm64", true)]
	[TestCase ("<integer>1</integer><integer>2</integer>", "ios-arm64", true)]
	[TestCase ("<integer>1</integer>", "ios-arm64", false)]
	[TestCase ("<integer>2</integer>", "iossimulator-arm64", false)]
	[TestCase ("<integer>2</integer>", "", true)]
	public void DiscoversMacDestination (string deviceFamilies, string runtimeIdentifier, bool expected)
	{
		var project = CreateProject (out var directory, deviceFamilies, runtimeIdentifier);
		DotNet.AssertBuild (project, target: "ComputeAvailableDevices");
		var devices = File.ReadAllLines (Path.Combine (directory, "devices.txt"));
		const string mac = "My Mac (Designed for iPad)|My Mac (Designed for iPad)|Mac|My Mac (Designed for iPad)|ios-arm64";
		Assert.That (devices.Contains (mac), Is.EqualTo (expected), "Mac destination");
		Assert.That (File.Exists (Path.Combine (directory, "build-ran.txt")), Is.False, "discovery must not build the app");
	}

	[TestCase ("RuntimeIdentifier", "iossimulator-arm64", false)]
	[TestCase ("OutputType", "Library", false)]
	[TestCase ("IsAppExtension", "true", false)]
	[TestCase ("IsWatchApp", "true", false)]
	[TestCase ("FailBuild", "true", false)]
	[TestCase ("", "", true)]
	public void RejectsInvalidApp (string property, string value, bool iPhoneOnly)
	{
		var project = CreateProject (out var directory, iPhoneOnly ? "<integer>1</integer>" : "<integer>2</integer>");
		var properties = new Dictionary<string, string> ();
		if (property.Length > 0)
			properties [property] = value;
		var result = DotNet.Execute ("build", project, properties, assert_success: false, target: "RunIPadAppOnMac");
		Assert.That (result.ExitCode, Is.Not.EqualTo (0), "invalid invocation must fail");
		var expectedError = property switch {
			"RuntimeIdentifier" => "RunIPadAppOnMac requires an iOS device build.",
			"FailBuild" => "Intentional build failure.",
			"" => "RunIPadAppOnMac requires an app that supports iPad.",
			_ => "RunIPadAppOnMac requires an iOS application project.",
		};
		Assert.That (result.StandardOutput + result.StandardError, Does.Contain (expectedError), "validation error");
		Assert.That (File.Exists (Path.Combine (directory, "build-ran.txt")), Is.EqualTo (iPhoneOnly), "only device family validation requires building");
		Assert.That (File.Exists (Path.Combine (directory, "wrapper-path.txt")), Is.False, "must not prepare or launch a wrapper");
	}
}
