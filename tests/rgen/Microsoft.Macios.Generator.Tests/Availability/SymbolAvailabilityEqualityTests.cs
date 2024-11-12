using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class SymbolAvailabilityEqualityTests {
	readonly SymbolAvailability.Builder leftBuilder = SymbolAvailability.CreateBuilder ();
	readonly SymbolAvailability.Builder rightBuilder = SymbolAvailability.CreateBuilder ();

	[Fact]
	public void EqualEmpty ()
	{
		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.True (left.Equals (right));
		Assert.True (right.Equals (left));
		Assert.True (left == right);
		Assert.False (left != right);
	}

	[Fact]
	public void EqualDifferentiOS ()
	{
		leftBuilder.Add (new SupportedOSPlatformData ("ios12.0"));
		rightBuilder.Add (new SupportedOSPlatformData ("ios13.0"));

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentiOSWithNull ()
	{
		leftBuilder.Add (new SupportedOSPlatformData ("ios12.0"));

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentTvOS ()
	{
		var iOSSupport = new SupportedOSPlatformData ("ios12.0");
		leftBuilder.Add (iOSSupport);
		leftBuilder.Add (new SupportedOSPlatformData ("tvos13.0"));
		rightBuilder.Add (iOSSupport);
		rightBuilder.Add (new SupportedOSPlatformData ("tvos"));

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentTvOSWithNull ()
	{
		var iOSSupport = new SupportedOSPlatformData ("ios12.0");
		leftBuilder.Add (iOSSupport);
		leftBuilder.Add (new SupportedOSPlatformData ("tvos13.0"));
		rightBuilder.Add (iOSSupport);

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentMacCatalyst ()
	{
		var iOSSupport = new SupportedOSPlatformData ("ios12.0");
		var tvOSSupport = new SupportedOSPlatformData ("tvos13.0");
		leftBuilder.Add (iOSSupport);
		leftBuilder.Add (tvOSSupport);
		leftBuilder.Add (new SupportedOSPlatformData ("maccatalyst13.0"));
		rightBuilder.Add (iOSSupport);
		rightBuilder.Add (tvOSSupport);
		rightBuilder.Add (new SupportedOSPlatformData ("maccatalyst"));

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentMacCatalystWithNull ()
	{
		var iOSSupport = new SupportedOSPlatformData ("ios12.0");
		var tvOSSupport = new SupportedOSPlatformData ("tvos13.0");
		leftBuilder.Add (iOSSupport);
		leftBuilder.Add (tvOSSupport);
		leftBuilder.Add (new SupportedOSPlatformData ("maccatalyst13.0"));
		rightBuilder.Add (iOSSupport);
		rightBuilder.Add (tvOSSupport);

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentMacOS ()
	{
		var iOSSupport = new SupportedOSPlatformData ("ios12.0");
		var tvOSSupport = new SupportedOSPlatformData ("tvos13.0");
		var maccatalystSupport = new SupportedOSPlatformData ("maccatalyst13.0");
		leftBuilder.Add (iOSSupport);
		leftBuilder.Add (tvOSSupport);
		leftBuilder.Add (maccatalystSupport);
		leftBuilder.Add (new SupportedOSPlatformData ("macos11.0"));
		rightBuilder.Add (iOSSupport);
		rightBuilder.Add (tvOSSupport);
		rightBuilder.Add (maccatalystSupport);
		rightBuilder.Add (new SupportedOSPlatformData ("macos12.0"));

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}

	[Fact]
	public void EqualDifferentMacOSWithNull ()
	{
		var iOSSupport = new SupportedOSPlatformData ("ios12.0");
		var tvOSSupport = new SupportedOSPlatformData ("tvos13.0");
		var maccatalystSupport = new SupportedOSPlatformData ("maccatalyst13.0");
		leftBuilder.Add (iOSSupport);
		leftBuilder.Add (tvOSSupport);
		leftBuilder.Add (maccatalystSupport);
		leftBuilder.Add (new SupportedOSPlatformData ("macos11.0"));
		rightBuilder.Add (iOSSupport);
		rightBuilder.Add (tvOSSupport);
		rightBuilder.Add (maccatalystSupport);

		var left = leftBuilder.ToImmutable ();
		var right = rightBuilder.ToImmutable ();
		Assert.False (left.Equals (right));
		Assert.False (right.Equals (left));
		Assert.False (left == right);
		Assert.True (left != right);
	}
}
