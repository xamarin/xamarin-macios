using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class SymbolAvailabilityMergeTests {
	readonly SymbolAvailability.Builder parentBuilder = SymbolAvailability.CreateBuilder ();
	readonly SymbolAvailability.Builder childBuilder = SymbolAvailability.CreateBuilder ();

	[Fact]
	public void MergeWithNull ()
	{
		// add some data to the member availability to ensure we do get a copy with all data
		var iosSupportedData = new SupportedOSPlatformData (ApplePlatform.iOS.AsString ().ToLower ());
		var tvUnsupportedData = new UnsupportedOSPlatformData ($"{ApplePlatform.TVOS.AsString ().ToLower ()}12.0");
		var tvSupportedData = new UnsupportedOSPlatformData ($"{ApplePlatform.TVOS.AsString ().ToLower ()}14.0");
		var catalystSupportedData =
			new SupportedOSPlatformData ($"{ApplePlatform.MacCatalyst.AsString ().ToLower ()}13.1");
		childBuilder.Add (iosSupportedData);
		childBuilder.Add (tvUnsupportedData);
		childBuilder.Add (tvSupportedData);
		childBuilder.Add (catalystSupportedData);
		var child = childBuilder.ToImmutable ();
		var merged = child.MergeWithParent (null);
		// validate data
		Assert.NotNull (merged.iOS);
		Assert.NotNull (merged.TvOS);
		Assert.NotNull (merged.MacCatalyst);
		Assert.Null (merged.MacOSX);
	}

	[Fact]
	public void MergeWithParentNotDataChildNotData ()
	{
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.Null (merged.iOS);
		Assert.Null (merged.TvOS);
		Assert.Null (merged.MacCatalyst);
		Assert.Null (merged.MacOSX);
	}

	[Fact]
	public void MergeWithParentWithDataChildNotData ()
	{
		var iosSupportedData = new SupportedOSPlatformData (ApplePlatform.iOS.AsString ().ToLower ());
		var tvUnsupportedData = new UnsupportedOSPlatformData ($"{ApplePlatform.TVOS.AsString ().ToLower ()}12.0");
		var tvSupportedData = new UnsupportedOSPlatformData ($"{ApplePlatform.TVOS.AsString ().ToLower ()}14.0");
		var catalystSupportedData =
			new SupportedOSPlatformData ($"{ApplePlatform.MacCatalyst.AsString ().ToLower ()}13.1");
		parentBuilder.Add (iosSupportedData);
		parentBuilder.Add (tvUnsupportedData);
		parentBuilder.Add (tvSupportedData);
		parentBuilder.Add (catalystSupportedData);
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		// we should have the parent data.
		Assert.NotNull (merged.iOS);
		Assert.NotNull (merged.TvOS);
		Assert.NotNull (merged.MacCatalyst);
		Assert.Null (merged.MacOSX);
	}

	[Fact]
	public void MergeWithParentNoDataChildData ()
	{
		var iosSupportedData = new SupportedOSPlatformData (ApplePlatform.iOS.AsString ().ToLower ());
		var tvUnsupportedData = new UnsupportedOSPlatformData ($"{ApplePlatform.TVOS.AsString ().ToLower ()}12.0");
		var tvSupportedData = new UnsupportedOSPlatformData ($"{ApplePlatform.TVOS.AsString ().ToLower ()}14.0");
		var catalystSupportedData =
			new SupportedOSPlatformData ($"{ApplePlatform.MacCatalyst.AsString ().ToLower ()}13.1");
		childBuilder.Add (iosSupportedData);
		childBuilder.Add (tvUnsupportedData);
		childBuilder.Add (tvSupportedData);
		childBuilder.Add (catalystSupportedData);
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		// validate data
		Assert.NotNull (merged.iOS);
		Assert.NotNull (merged.TvOS);
		Assert.NotNull (merged.MacCatalyst);
		Assert.Null (merged.MacOSX);
	}

	[Fact]
	public void MergeWithParentWithDataChildData ()
	{
		// create two diff availability, one for the child, one for the parent. Then ensure that the 
		// result of the merge is the merge of the two.
		var platformName = ApplePlatform.iOS.AsString ().ToLower ();
		var childAvailabilityBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var childiOSSupportedData = new SupportedOSPlatformData ($"{platformName}12.0");
		var childObsoletedData = new ObsoletedOSPlatformData ($"{platformName}14.0");
		// add it to the platform data
		childAvailabilityBuilder.Add (childiOSSupportedData);
		childAvailabilityBuilder.Add (childObsoletedData);
		// add it to the member one
		childBuilder.Add (childiOSSupportedData);
		childBuilder.Add (childObsoletedData);

		var parentAvailabilityBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var parentiOSSupportData = new SupportedOSPlatformData ($"{platformName}10.0");
		var parentObsoletedData = new ObsoletedOSPlatformData ($"{platformName}11.0");
		parentAvailabilityBuilder.Add (parentiOSSupportData);
		parentAvailabilityBuilder.Add (parentObsoletedData);
		parentBuilder.Add (parentiOSSupportData);
		parentBuilder.Add (parentObsoletedData);

		var childAvailability = childAvailabilityBuilder.ToImmutable ();
		var parentAvailability = parentAvailabilityBuilder.ToImmutable ();
		var expectedData = childAvailability.MergeWithParent (parentAvailability);
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.NotNull (merged.iOS);
		Assert.Equal (expectedData, merged.iOS);
	}

	[Fact]
	public void MergeParentDataChildDataDiffPlatforms ()
	{
		// create availability for two diff platforms, and ensure that the merge contains both
		var childPlatformName = ApplePlatform.iOS.AsString ().ToLower ();
		var childAvailabilityBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
		var childiOSSupportedData = new SupportedOSPlatformData ($"{childPlatformName}12.0");
		var childObsoletedData = new ObsoletedOSPlatformData ($"{childPlatformName}14.0");
		// add it to the platform data
		childAvailabilityBuilder.Add (childiOSSupportedData);
		childAvailabilityBuilder.Add (childObsoletedData);
		// add it to the member one
		childBuilder.Add (childiOSSupportedData);
		childBuilder.Add (childObsoletedData);

		var parentPlatformName = ApplePlatform.TVOS.AsString ().ToLower ();
		var parentAvailabilityBuilder = PlatformAvailability.CreateBuilder (ApplePlatform.TVOS);
		var parentiOSSupportData = new SupportedOSPlatformData ($"{parentPlatformName}10.0");
		var parentObsoletedData = new ObsoletedOSPlatformData ($"{parentPlatformName}11.0");
		parentAvailabilityBuilder.Add (parentiOSSupportData);
		parentAvailabilityBuilder.Add (parentObsoletedData);
		parentBuilder.Add (parentiOSSupportData);
		parentBuilder.Add (parentObsoletedData);

		var childAvailability = childAvailabilityBuilder.ToImmutable ();
		var parentAvailability = parentAvailabilityBuilder.ToImmutable ();
		var child = childBuilder.ToImmutable ();
		var parent = parentBuilder.ToImmutable ();
		var merged = child.MergeWithParent (parent);
		Assert.NotNull (merged.iOS);
		Assert.NotNull (merged.TvOS);
		Assert.Equal (childAvailability, merged.iOS);
		Assert.Equal (parentAvailability, merged.TvOS);
	}
}
