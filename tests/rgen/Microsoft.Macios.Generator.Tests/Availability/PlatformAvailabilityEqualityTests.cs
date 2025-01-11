// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class PlatformAvailabilityEqualityTests {
	[Fact]
	public void EqualDifferentPlatforms ()
	{
		var leftBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var rightBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.TVOS);
		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualSamePlatformDifferentSupportedVersion ()
	{
		var leftBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var rightBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.TVOS);
		leftBuilder.AddSupportedVersion (new Version (12, 0));
		rightBuilder.AddSupportedVersion (new Version (10, 0));
		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualsNullSupportedVersion ()
	{
		var leftBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var rightBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.TVOS);
		leftBuilder.AddSupportedVersion (new Version (12, 0));
		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualSamePlatformSameSupportedVersionDifferentUnsupportedVersion ()
	{
		var leftBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var rightBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.TVOS);
		leftBuilder.AddSupportedVersion (new Version (12, 0));
		leftBuilder.AddUnsupportedVersion (new Version (10, 0), "Unsupported");
		rightBuilder.AddSupportedVersion (new Version (12, 0));
		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualsSamePlatformSamerSupportedVersionSameUnsupportedDiffObsolete ()
	{
		var leftBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var rightBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.TVOS);
		leftBuilder.AddSupportedVersion (new Version (12, 0));
		leftBuilder.AddUnsupportedVersion (new Version (10, 0), "Unsupported");
		leftBuilder.AddObsoletedVersion (new Version (12, 0), "Obsolete", null);
		rightBuilder.AddSupportedVersion (new Version (12, 0));
		rightBuilder.AddUnsupportedVersion (new Version (10, 0), "Unsupported");
		leftBuilder.AddObsoletedVersion (new Version (12, 0), "Obsolete", "url");
		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}
}
