// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class PlatformAvailabilityMergeTests {
	readonly PlatformAvailability.Builder parentBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
	readonly PlatformAvailability.Builder childBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
	readonly Version unsupportedPlatform = new Version ();

	[Fact]
	public void MergeDifferentPlatforms ()
	{
		var wrongParentBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.MacOSX);
		// add data in the child to ensure we do have a real copy
		childBuilder.AddSupportedVersion (new Version ());
		childBuilder.AddUnsupportedVersion (new Version (12, 0), "Unsupported version");
		childBuilder.AddObsoletedVersion (new Version (11, 0), null, null);
		var child = childBuilder.ToImmutable ();
		var wrongParent = wrongParentBuilder.ToImmutable ();

		var merged = child.MergeWithParent (wrongParent);
		Assert.Equal (child.UnsupportedVersions.Count, merged.UnsupportedVersions.Count);
		Assert.Equal (child.ObsoletedVersions.Count, merged.ObsoletedVersions.Count);
		Assert.Equal (child.SupportedVersion, merged.SupportedVersion);
		Assert.Equal (child.Platform, merged.Platform);
	}

	[Fact]
	public void MergeNullParent ()
	{
		// add data in the child to ensure we do have a real copy
		childBuilder.AddSupportedVersion (new Version ());
		childBuilder.AddUnsupportedVersion (new Version (12, 0), "Unsupported version");
		childBuilder.AddObsoletedVersion (new Version (11, 0), null, null);
		var child = childBuilder.ToImmutable ();
		var merged = child.MergeWithParent (null);
		Assert.Equal (child.UnsupportedVersions.Count, merged.UnsupportedVersions.Count);
		Assert.Equal (child.ObsoletedVersions.Count, merged.ObsoletedVersions.Count);
		Assert.Equal (child.SupportedVersion, merged.SupportedVersion);
		Assert.Equal (child.Platform, merged.Platform);
	}

	[Fact]
	public void MergeWithParentNoParentData ()
	{
		// example:
		// public class Test {
		//
		//    [UnsupportedOSPlatform ("ios")]
		//    public void Test ();
		// }
		var unsupportedVersion = new Version (12, 0, 0);
		childBuilder.AddUnsupportedVersion (unsupportedVersion, "Unsupported version");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Single (merged.UnsupportedVersions);
		Assert.Contains (unsupportedVersion, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeWithParentUnsupportedParent ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios")]
		// public class Test {
		// 
		//    public void Test ();
		// }
		parentBuilder.AddUnsupportedVersion (unsupportedPlatform, "Unsupported version");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Single (merged.UnsupportedVersions);
		Assert.Contains (unsupportedPlatform, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeWithParentUnsupportedAndChildUnsupported ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios11.0")]
		// public class Test {
		//
		//    [UnsupportedOSPlatform ("ios12.0")]
		//    public void Test ();
		// }
		var unsupportedParentVersion = new Version (11, 0, 0);
		parentBuilder.AddUnsupportedVersion (unsupportedParentVersion, "Unsupported version");
		var unsupportedChildVersion = new Version (12, 0, 0);
		childBuilder.AddUnsupportedVersion (unsupportedChildVersion, "Unsupported version");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Equal (2, merged.UnsupportedVersions.Count);
		// both unsupported versions should appear
		Assert.Contains (unsupportedParentVersion, merged.UnsupportedVersions);
		Assert.Contains (unsupportedChildVersion, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeUnsupportedPlatformParentUnsupportedChildVersion ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios")]
		// public class Test {
		//
		//    [UnsupportedOSPlatform ("ios12.0")]
		//    public void Test ();
		// }
		parentBuilder.AddUnsupportedVersion (unsupportedPlatform, "Unsupported platform");
		var unsupportedChildVersion = new Version (12, 0, 0);
		childBuilder.AddUnsupportedVersion (unsupportedChildVersion, "Unsupported version");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Single (merged.UnsupportedVersions);
		Assert.Contains (unsupportedPlatform, merged.UnsupportedVersions);
		Assert.DoesNotContain (unsupportedChildVersion, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeWithParentUnsupportedVersionUnsupportedPlatformChild ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios12.0")]
		// public class Test {
		//
		//    [UnsupportedOSPlatform ("ios")]
		//    public void Test ();
		// }
		var unsupportedParentVersion = new Version (12, 0, 0);
		parentBuilder.AddUnsupportedVersion (unsupportedParentVersion, "Unsupported version");
		childBuilder.AddUnsupportedVersion (unsupportedPlatform, "Unsupported platform");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Single (merged.UnsupportedVersions);
		Assert.Contains (unsupportedPlatform, merged.UnsupportedVersions);
		Assert.DoesNotContain (unsupportedParentVersion, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeWithParentLowerParentVersion ()
	{
		// example:
		// [SupportedOSPlatform ("ios11.0")
		// public class Test {
		//     [SupportedOSPlatform ("ios12.0"]
		//     public void Test ();
		// }
		var supportedParentVersion = new Version (11, 0, 0);
		parentBuilder.AddSupportedVersion (supportedParentVersion);
		var supportedChildVersion = new Version (12, 0, 0);
		childBuilder.AddSupportedVersion (supportedChildVersion);
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.NotNull (merged.SupportedVersion);
		// always pick the most restrictive one
		Assert.Equal (supportedChildVersion, merged.SupportedVersion);
	}

	[Fact]
	public void MergeWithParentHigherParentVersion ()
	{
		// example:
		// [SupportedOSPlatform ("ios12.0")
		// public class Test {
		//     [SupportedOSPlatform ("ios11.0"]
		//     public void Test ();
		// }
		var supportedParentVersion = new Version (12, 0, 0);
		parentBuilder.AddSupportedVersion (supportedParentVersion);
		var supportedChildVersion = new Version (11, 0, 0);
		childBuilder.AddSupportedVersion (supportedChildVersion);
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.NotNull (merged.SupportedVersion);
		// always pick the most restrictive one
		Assert.Equal (supportedParentVersion, merged.SupportedVersion);
	}

	[Fact]
	public void MergeWithParentUnsupportedChild ()
	{
		// example:
		// [SupportedOSPlatform ("ios12.0")
		// public class Test {
		//     [UnsupportedOSPlatform ("ios"]
		//     public void Test ();
		// }
		var supportedParentVersion = new Version (12, 0, 0);
		parentBuilder.AddSupportedVersion (supportedParentVersion);
		childBuilder.AddUnsupportedVersion (unsupportedPlatform, "Unsupported platform");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Null (merged.SupportedVersion);
	}

	[Fact]
	public void MergeWithParentUnsupportedParentAndChild ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios11.0")
		// public class Test {
		//     [UnsupportedOSPlatform ("ios12.0"]
		//     public void Test ();
		// }
		var unsupportedParentVersion = new Version (11, 0, 0);
		parentBuilder.AddUnsupportedVersion (unsupportedParentVersion, "Unsupported version");
		var unsupportedChildVersion = new Version (12, 0, 0);
		childBuilder.AddUnsupportedVersion (unsupportedChildVersion, "Unsupported version");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Equal (2, merged.UnsupportedVersions.Count);
		Assert.Contains (unsupportedParentVersion, merged.UnsupportedVersions);
		Assert.Contains (unsupportedChildVersion, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeWithParentUnsupportedEmptyChild ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios11.0")
		// public class Test {
		//
		//     public void Test ();
		// }
		var unsupportedParentVersion = new Version (11, 0, 0);
		parentBuilder.AddUnsupportedVersion (unsupportedParentVersion, "Unsupported platform");
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Single (merged.UnsupportedVersions);
		Assert.Contains (unsupportedParentVersion, merged.UnsupportedVersions);
	}

	[Fact]
	public void MergeWithUnsupportedParentChildSameVersion ()
	{
		// example:
		// [UnsupportedOSPlatform ("ios11.0")
		// public class Test {
		//
		//     [UnsupportedOSPlatform ("ios11.0", "This was an error.")
		//     public void Test ();
		// }
		var childMsg = "This was an error.";
		var unsupportedParentVersion = new Version (11, 0, 0);
		parentBuilder.AddUnsupportedVersion (unsupportedParentVersion, null);
		childBuilder.AddUnsupportedVersion (unsupportedParentVersion, childMsg);
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Single (merged.UnsupportedVersions);
		Assert.Contains (unsupportedParentVersion, merged.UnsupportedVersions);
		Assert.Equal (childMsg, merged.UnsupportedVersions [unsupportedParentVersion]);
	}
}
