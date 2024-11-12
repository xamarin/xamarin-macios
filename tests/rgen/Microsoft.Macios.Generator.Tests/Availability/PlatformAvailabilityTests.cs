using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class PlatformAvailabilityTests {
	[Theory]
	[InlineData (ApplePlatform.iOS, "ios")]
	[InlineData (ApplePlatform.MacOSX, "macos")]
	[InlineData (ApplePlatform.TVOS, "tvos")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst")]
	public void SupportPlatform (ApplePlatform platform, string attributePlatformName)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new SupportedOSPlatformData (attributePlatformName);
		builder.Add (attrData);
		var availability = builder.ToImmutable ();
		Assert.Equal (availability.SupportedVersion, new Version ());
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "macos12.0")]
	[InlineData (ApplePlatform.MacOSX, "ios16.0")]
	[InlineData (ApplePlatform.TVOS, "ios16.0")]
	[InlineData (ApplePlatform.MacCatalyst, "macos14.0")]
	public void SupportPlatformWrongPlatformAttribute (ApplePlatform platform, string attributePlatformName)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new SupportedOSPlatformData (attributePlatformName);
		builder.Add (attrData);
		var availability = builder.ToImmutable ();
		Assert.Null (availability.SupportedVersion);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "ios16.0", "16.0")]
	[InlineData (ApplePlatform.MacOSX, "macos14.0", "14.0")]
	[InlineData (ApplePlatform.TVOS, "tvos12.0", "12.0")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst13.1", "13.1")]
	public void SupportPlatformWithPlatformAttributeWithVersion (ApplePlatform platform, string attributePlatformName,
		string version)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new SupportedOSPlatformData (attributePlatformName);
		builder.Add (attrData);
		var expectedVersion = Version.Parse (version);
		var availability = builder.ToImmutable ();
		Assert.Equal (expectedVersion, availability.SupportedVersion);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "ios16.0", "16.0")]
	[InlineData (ApplePlatform.MacOSX, "macos14.0", "14.0")]
	[InlineData (ApplePlatform.TVOS, "tvos12.0", "12.0")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst13.1", "13.1")]
	public void SupportPlatformAndThenAddHigherVersion (ApplePlatform platform, string attributePlatformName,
		string version)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		// build two attr data, one with the platformName to support the platform, then a second one to override
		// the value
		var allVersion = new SupportedOSPlatformData (platform.AsString ().ToLower ());
		var specificVersion = new SupportedOSPlatformData (attributePlatformName);
		builder.Add (allVersion);
		builder.Add (specificVersion);
		var expectedVersion = Version.Parse (version);
		// should be most specific version
		var availability = builder.ToImmutable ();
		Assert.Equal (expectedVersion, availability.SupportedVersion);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, new [] { "12.0", "13.0", "13.1", "15.0", "16.0" }, "16.0")]
	[InlineData (ApplePlatform.MacOSX, new [] { "9.0", "10.0", "11.5", "14.0" }, "14.0")]
	[InlineData (ApplePlatform.TVOS, new [] { "8.0", "9.0", "12.0" }, "12.0")]
	[InlineData (ApplePlatform.MacCatalyst, new [] { "10.0", "11.0", "13.1" }, "13.1")]
	public void SupportPlatformSeveralVersionsPickHigherVersion (ApplePlatform platform, string [] versions,
		string expectedVersion)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		// the version should always be the same no matter in which order we add the support, lets shuffle the input
		Random.Shared.Shuffle (versions);
		foreach (var version in versions) {
			var attrData = new SupportedOSPlatformData ($"{platform.AsString ().ToLower ()}{version}");
			builder.Add (attrData);
		}

		var expected = Version.Parse (expectedVersion);
		var availability = builder.ToImmutable ();
		Assert.Equal (expected, availability.SupportedVersion);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "ios")]
	[InlineData (ApplePlatform.MacOSX, "macos")]
	[InlineData (ApplePlatform.TVOS, "tvos")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst")]
	public void UnsupportPlatform (ApplePlatform platform, string attributePlatformName)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new UnsupportedOSPlatformData (attributePlatformName);
		builder.Add (attrData);
		var defaultVersion = new Version ();
		var availability = builder.ToImmutable ();
		Assert.Contains (defaultVersion, availability.UnsupportedVersions.Keys);
		Assert.Single (availability.UnsupportedVersions);
		Assert.Null (availability.UnsupportedVersions [defaultVersion]);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "ios", "Not present on iOS")]
	[InlineData (ApplePlatform.MacOSX, "macos", "Use the macOS enum instead of iOS")]
	[InlineData (ApplePlatform.TVOS, "tvos", "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst", "Use the UIKit version instead of iOS")]
	public void UnsupportPlatformWithMessage (ApplePlatform platform, string attributePlatformName, string message)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new UnsupportedOSPlatformData (attributePlatformName, message);
		builder.Add (attrData);
		var defaultVersion = new Version ();
		var availability = builder.ToImmutable ();
		Assert.Contains (defaultVersion, availability.UnsupportedVersions.Keys);
		Assert.Single (availability.UnsupportedVersions);
		Assert.Equal (message, availability.UnsupportedVersions [defaultVersion]);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "16.0", "Not present on iOS")]
	[InlineData (ApplePlatform.MacOSX, "12.0", "Use the macOS enum instead of iOS")]
	[InlineData (ApplePlatform.TVOS, "9.0", "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, "13.1", "Use the UIKit version instead of iOS")]
	public void UnsupportVersionAndPlatform (ApplePlatform platform, string attributeVersion, string? message)
	{
		// add the version, then unsupport the platform and assert that we indeed just unsupported the platform
		var builder = PlatformAvailability.CreateBuilder (platform);
		var versionAttrData =
			new UnsupportedOSPlatformData ($"{platform.AsString ().ToLower ()}{attributeVersion}", message);
		var platformAttrData = new UnsupportedOSPlatformData (platform.AsString ().ToLower (), message);
		builder.Add (versionAttrData);
		builder.Add (platformAttrData);
		var defaultVersion = new Version ();
		var availability = builder.ToImmutable ();
		Assert.Contains (defaultVersion, availability.UnsupportedVersions.Keys);
		Assert.Single (availability.UnsupportedVersions);
		Assert.Equal (message, availability.UnsupportedVersions [defaultVersion]);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "Not present on iOS")]
	[InlineData (ApplePlatform.MacOSX, "Use the macOS enum instead of iOS")]
	[InlineData (ApplePlatform.TVOS, "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, "Use the UIKit version instead of iOS")]
	public void SupportAndUnsupportPlatform (ApplePlatform platform, string? message)
	{
		// support and unsupport the platform. We should ALWAYS unsupport the platform no matter the order
		// in which we add the attr data. This is because unsupported is the most restrictive 
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new object [] {
			new SupportedOSPlatformData (platform.AsString ().ToLower ()),
			new UnsupportedOSPlatformData (platform.AsString ().ToLower (), message)
		};
		// add the attr data in any order. We should always have the platfor unsupported
		Random.Shared.Shuffle (attrData);
		foreach (object data in attrData) {
			switch (data) {
			case UnsupportedOSPlatformData unsupportedPlatformData:
				builder.Add (unsupportedPlatformData);
				break;
			case SupportedOSPlatformData supportedOsPlatformData:
				builder.Add (supportedOsPlatformData);
				break;
			}
		}

		var availability = builder.ToImmutable ();
		Assert.Null (availability.SupportedVersion);
		Assert.Single (availability.UnsupportedVersions);
		Assert.Contains (new Version (), availability.UnsupportedVersions.Keys);
		Assert.Equal (message, availability.UnsupportedVersions [new Version ()]);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, new [] { "12.0", "13.0", "13.1", "15.0", "16.0" }, "Unsupporte on iOS")]
	[InlineData (ApplePlatform.MacOSX, new [] { "9.0", "10.0", "11.5", "14.0" }, "UseAppKit enum")]
	[InlineData (ApplePlatform.TVOS, new [] { "8.0", "9.0", "12.0" }, "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, new [] { "10.0", "11.0", "13.1" }, "Use the UIKit version instead")]
	public void UnsupportSeveralVersionAndPlatform (ApplePlatform platform, string [] versions, string? message)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		// create a list with the attr data + a platform unsupported one and ensure that we just
		// have it unsupported
		var unsupportedVersionList = new List<UnsupportedOSPlatformData> {
			new UnsupportedOSPlatformData (platform.AsString ().ToLower (), message)
		};
		foreach (var v in versions) {
			unsupportedVersionList.Add (
				new UnsupportedOSPlatformData ($"{platform.AsString ().ToLower ()}{v}", message));
		}

		var unsupportedAttrs = unsupportedVersionList.ToArray ();
		// we should always have the same result no matter the order
		Random.Shared.Shuffle (unsupportedAttrs);
		foreach (var attrData in unsupportedAttrs) {
			builder.Add (attrData);
		}

		var availability = builder.ToImmutable ();
		Assert.Single (availability.UnsupportedVersions);
		Assert.Contains (new Version (), availability.UnsupportedVersions.Keys);
		Assert.Equal (message, availability.UnsupportedVersions [new Version ()]);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, new [] { "12.0", "13.0", "13.1", "15.0", "16.0" }, "Unsupporte on iOS")]
	[InlineData (ApplePlatform.MacOSX, new [] { "9.0", "10.0", "11.5", "14.0" }, "UseAppKit enum")]
	[InlineData (ApplePlatform.TVOS, new [] { "8.0", "9.0", "12.0" }, "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, new [] { "10.0", "11.0", "13.1" }, "Use the UIKit version instead")]
	public void UnsupportedSeveralVersions (ApplePlatform platform, string [] versions, string? message)
	{
		// we should have all the versions that are unsupported since we did not unsupport the platform
		var builder = PlatformAvailability.CreateBuilder (platform);
		// create a list with the attr data + a platform unsupported one and ensure that we just
		// have it unsupported

		var unsupportedAttrs =
			versions.Select (v => new UnsupportedOSPlatformData ($"{platform.AsString ().ToLower ()}{v}", message))
				.ToArray ();
		// we should always have the same result no matter the order
		Random.Shared.Shuffle (unsupportedAttrs);
		foreach (var attrData in unsupportedAttrs) {
			builder.Add (attrData);
		}

		var availability = builder.ToImmutable ();
		// assert that the version is present
		foreach (var v in versions) {
			var currentVersion = Version.Parse (v);
			Assert.Contains (currentVersion, availability.UnsupportedVersions.Keys);
		}
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "ios")]
	[InlineData (ApplePlatform.MacOSX, "macos")]
	[InlineData (ApplePlatform.TVOS, "tvos")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst")]
	public void ObsoletePlatform (ApplePlatform platform, string attributePlatformName)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new ObsoletedOSPlatformData (attributePlatformName);
		builder.Add (attrData);
		var defaultVersion = new Version ();
		var availability = builder.ToImmutable ();
		Assert.Contains (defaultVersion, availability.ObsoletedVersions.Keys);
		Assert.Single (availability.ObsoletedVersions);
		Assert.Null (availability.ObsoletedVersions [defaultVersion].Message);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, "ios", "Not present on iOS")]
	[InlineData (ApplePlatform.MacOSX, "macos", "Use the macOS enum instead of iOS")]
	[InlineData (ApplePlatform.TVOS, "tvos", "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, "maccatalyst", "Use the UIKit version instead of iOS")]
	public void ObsoletePlatformWithMessage (ApplePlatform platform, string attributePlatformName, string message)
	{
		var builder = PlatformAvailability.CreateBuilder (platform);
		var attrData = new ObsoletedOSPlatformData (attributePlatformName, message);
		builder.Add (attrData);
		var defaultVersion = new Version ();
		var availability = builder.ToImmutable ();
		Assert.Contains (defaultVersion, availability.ObsoletedVersions.Keys);
		Assert.Single (availability.ObsoletedVersions);
		Assert.Equal (message, availability.ObsoletedVersions [defaultVersion].Message);
	}


	[Theory]
	[InlineData (ApplePlatform.iOS, "16.0", "Not present on iOS")]
	[InlineData (ApplePlatform.MacOSX, "12.0", "Use the macOS enum instead of iOS")]
	[InlineData (ApplePlatform.TVOS, "9.0", "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, "13.1", "Use the UIKit version instead of iOS")]
	public void ObsoleteVersionAndPlatform (ApplePlatform platform, string attributeVersion, string? message)
	{
		// add the version, then obsolete the platform and assert that we indeed just unsupported the platform
		var platformName = platform.AsString ().ToLower ();
		var builder = PlatformAvailability.CreateBuilder (platform);
		var versionAttrData = new ObsoletedOSPlatformData ($"{platformName}{attributeVersion}", message);
		var platformAttrData = new ObsoletedOSPlatformData (platformName, message);
		builder.Add (versionAttrData);
		builder.Add (platformAttrData);
		var defaultVersion = new Version ();
		var availability = builder.ToImmutable ();
		Assert.Contains (defaultVersion, availability.ObsoletedVersions.Keys);
		Assert.Single (availability.ObsoletedVersions);
		Assert.Equal (message, availability.ObsoletedVersions [defaultVersion].Message);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, new [] { "12.0", "13.0", "13.1", "15.0", "16.0" }, "Unsupporte on iOS")]
	[InlineData (ApplePlatform.MacOSX, new [] { "9.0", "10.0", "11.5", "14.0" }, "UseAppKit enum")]
	[InlineData (ApplePlatform.TVOS, new [] { "8.0", "9.0", "12.0" }, "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, new [] { "10.0", "11.0", "13.1" }, "Use the UIKit version instead")]
	public void ObsoleteSeveralVersionAndPlatform (ApplePlatform platform, string [] versions, string? message)
	{
		var platformName = platform.AsString ().ToLower ();
		var builder = PlatformAvailability.CreateBuilder (platform);
		// create a list with the attr data + a platform unsupported one and ensure that we just
		// have it unsupported
		var obsoleteVersionList = new List<ObsoletedOSPlatformData> { new(platformName, message) };
		foreach (var v in versions) {
			obsoleteVersionList.Add (new ObsoletedOSPlatformData ($"{platformName}{v}", message));
		}

		var obsoleteAttrs = obsoleteVersionList.ToArray ();
		// we should always have the same result no matter the order
		Random.Shared.Shuffle (obsoleteAttrs);
		foreach (var attrData in obsoleteAttrs) {
			builder.Add (attrData);
		}

		var availability = builder.ToImmutable ();
		Assert.Single (availability.ObsoletedVersions);
		Assert.Contains (new Version (), availability.ObsoletedVersions.Keys);
		Assert.Equal (message, availability.ObsoletedVersions [new Version ()].Message);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS, new [] { "12.0", "13.0", "13.1", "15.0", "16.0" }, "Unsupporte on iOS")]
	[InlineData (ApplePlatform.MacOSX, new [] { "9.0", "10.0", "11.5", "14.0" }, "UseAppKit enum")]
	[InlineData (ApplePlatform.TVOS, new [] { "8.0", "9.0", "12.0" }, "Not present in TVOS")]
	[InlineData (ApplePlatform.MacCatalyst, new [] { "10.0", "11.0", "13.1" }, "Use the UIKit version instead")]
	public void ObsoleteSeveralVersions (ApplePlatform platform, string [] versions, string? message)
	{
		var platformName = platform.AsString ().ToLower ();
		var builder = PlatformAvailability.CreateBuilder (platform);
		// create a list with the attr data + a platform unsupported one and ensure that we just
		// have it unsupported
		var obsoleteAttrs =
			versions.Select (v => new ObsoletedOSPlatformData ($"{platformName}{v}", message)).ToArray ();
		// we should always have the same result no matter the order
		Random.Shared.Shuffle (obsoleteAttrs);
		foreach (var attrData in obsoleteAttrs) {
			builder.Add (attrData);
		}

		// assert that the version is present
		var availability = builder.ToImmutable ();
		foreach (var v in versions) {
			var currentVersion = Version.Parse (v);
			Assert.Contains (currentVersion, availability.ObsoletedVersions.Keys);
		}
	}
}
