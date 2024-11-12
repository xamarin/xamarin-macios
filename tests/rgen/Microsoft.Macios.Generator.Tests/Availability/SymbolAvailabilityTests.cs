using System.Collections.Generic;
using System.Linq;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class SymbolAvailabilityTests {
	SymbolAvailability.Builder builder = SymbolAvailability.CreateBuilder ();

	[Fact]
	public void ConstructorTest ()
	{
		// assert we have no availability info for any platform
		var availability = builder.ToImmutable ();
		Assert.Empty (availability.PlatformAvailabilities);
	}

	[Fact]
	public void AddUnknownPlatformTest ()
	{
		var supportedUnknown = new SupportedOSPlatformData ("Unknown");
		builder.Add (supportedUnknown);
		var availability = builder.ToImmutable ();
		Assert.Empty (availability.PlatformAvailabilities);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS)]
	[InlineData (ApplePlatform.MacOSX)]
	[InlineData (ApplePlatform.TVOS)]
	[InlineData (ApplePlatform.MacCatalyst)]
	public void AddSupportedPlatformTest (ApplePlatform platform)
	{
		var supported = new SupportedOSPlatformData (platform.AsString ().ToLower ());
		builder.Add (supported);
		var availability = builder.ToImmutable ();
		var availablePlatforms = availability.PlatformAvailabilities.ToArray ();
		Assert.Single (availablePlatforms);
		Assert.Equal (platform, availablePlatforms [0].Platform);
		Assert.NotNull (availablePlatforms [0].SupportedVersion);
		Assert.Empty (availablePlatforms [0].UnsupportedVersions);
		Assert.Empty (availablePlatforms [0].ObsoletedVersions);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS)]
	[InlineData (ApplePlatform.MacOSX)]
	[InlineData (ApplePlatform.TVOS)]
	[InlineData (ApplePlatform.MacCatalyst)]
	public void AddUnsupportedPlatformTest (ApplePlatform platform)
	{
		var unsupported = new UnsupportedOSPlatformData (platform.AsString ().ToLower ());
		builder.Add (unsupported);
		var availability = builder.ToImmutable ();
		var availablePlatforms = availability.PlatformAvailabilities.ToArray ();
		Assert.Single (availablePlatforms);
		Assert.Equal (platform, availablePlatforms [0].Platform);
		Assert.Null (availablePlatforms [0].SupportedVersion);
		Assert.Single (availablePlatforms [0].UnsupportedVersions);
		Assert.Empty (availablePlatforms [0].ObsoletedVersions);
	}

	[Theory]
	[InlineData (ApplePlatform.iOS)]
	[InlineData (ApplePlatform.MacOSX)]
	[InlineData (ApplePlatform.TVOS)]
	[InlineData (ApplePlatform.MacCatalyst)]
	public void AddObsoletePlatformTest (ApplePlatform platform)
	{
		var obsolete = new ObsoletedOSPlatformData (platform.AsString ().ToLower ());
		builder.Add (obsolete);
		var availability = builder.ToImmutable ();
		var availablePlatforms = availability.PlatformAvailabilities.ToArray ();
		Assert.Single (availablePlatforms);
		Assert.Equal (platform, availablePlatforms [0].Platform);
		Assert.Null (availablePlatforms [0].SupportedVersion);
		Assert.Empty (availablePlatforms [0].UnsupportedVersions);
		Assert.Single (availablePlatforms [0].ObsoletedVersions);
	}


	public static IEnumerable<object []> AddSeveralPlatformTestData {
		get {
			yield return [
				new Dictionary<ApplePlatform, object []> {
					{ ApplePlatform.iOS, [ new SupportedOSPlatformData (ApplePlatform.iOS.AsString ().ToLower())] },
					{ ApplePlatform.MacOSX, [new SupportedOSPlatformData (ApplePlatform.MacOSX.AsString ().ToLower())] },
				}
			];

			yield return [
				new Dictionary<ApplePlatform, object []> {
					{ ApplePlatform.iOS, [new SupportedOSPlatformData ("ios12.0")] },
					{ ApplePlatform.MacOSX, [new SupportedOSPlatformData ("macos12.0"), new UnsupportedOSPlatformData ("macos11.0")] },
					{ ApplePlatform.TVOS, [new SupportedOSPlatformData (ApplePlatform.TVOS.AsString ().ToLower())] },
				}
			];

			yield return [
				new Dictionary<ApplePlatform, object []> {
					{ ApplePlatform.iOS, [new UnsupportedOSPlatformData (ApplePlatform.iOS.AsString ().ToLower())] },
					{ ApplePlatform.MacOSX, [new UnsupportedOSPlatformData (ApplePlatform.MacOSX.AsString ().ToLower())] },
					{ ApplePlatform.TVOS, [new UnsupportedOSPlatformData (ApplePlatform.TVOS.AsString ().ToLower())] },
					{ ApplePlatform.MacCatalyst, [new UnsupportedOSPlatformData (ApplePlatform.MacCatalyst.AsString ().ToLower())] }
				}
			];
		}
	}

	[Theory]
	[MemberData (nameof (AddSeveralPlatformTestData))]
	void AddSeveralPlatformTest (Dictionary<ApplePlatform, object []> platforms)
	{
		foreach (var (_, availabilityData) in platforms) {
			foreach (var data in availabilityData) {
				switch (data) {
				case ObsoletedOSPlatformData obsoleted:
					builder.Add (obsoleted);
					break;
				case SupportedOSPlatformData supported:
					builder.Add (supported);
					break;
				case UnsupportedOSPlatformData unsupported:
					builder.Add (unsupported);
					break;
				}
			}
		}

		var availability = builder.ToImmutable ();
		var availablePlatforms = availability.PlatformAvailabilities.ToArray ();
		Assert.Equal (platforms.Keys.Count, availablePlatforms.Length);
	}
}
