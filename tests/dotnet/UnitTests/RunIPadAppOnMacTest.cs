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

	string CreateProject (out string directory, string deviceFamilies = "<integer>2</integer>")
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
					<OutputType>Exe</OutputType>
					<RuntimeIdentifier>ios-arm64</RuntimeIdentifier>
				</PropertyGroup>
				<Import Project="Sdk.targets" Sdk="Microsoft.NET.Sdk" />
				<!-- Create an unsigned fixture bundle instead of compiling an app or requiring signing credentials. -->
				<Target Name="Build" DependsOnTargets="_ComputeTargetFrameworkMoniker">
					<Error Condition="'$(FailBuild)' == 'true'" Text="Intentional build failure." />
					<PropertyGroup>
						<_AppBundleName>Test App</_AppBundleName>
						<_AppBundlePath>$(MSBuildProjectDirectory)/input/Test App.app/</_AppBundlePath>
						<_AppBundleManifestPath>$(_AppBundlePath)Info.plist</_AppBundleManifestPath>
					</PropertyGroup>
					<MakeDir Directories="$(_AppBundlePath)" />
					<Copy SourceFiles="Info.plist" DestinationFiles="$(_AppBundleManifestPath)" />
					<WriteLinesToFile File="$(_AppBundlePath)Test App" Lines="fixture executable" Overwrite="true" />
					<WriteLinesToFile File="build-ran.txt" Lines="Build" Overwrite="true" />
				</Target>
				<Target Name="RecordWrapper" AfterTargets="_PrepareIPadAppOnMac">
					<WriteLinesToFile File="wrapper-path.txt" Lines="$(_IPadAppOnMacBundle)" Overwrite="true" />
					<WriteLinesToFile File="wrapper-files.txt" Lines="@(FileWrites->'%(FullPath)')" Overwrite="true" />
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
