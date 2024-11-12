using System;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

readonly partial struct SymbolAvailability : IEquatable<SymbolAvailability> {
	public PlatformAvailability? iOS { get; }

	public PlatformAvailability? TvOS { get; }

	public PlatformAvailability? MacCatalyst { get; }

	public PlatformAvailability? MacOSX { get; }

	public SymbolAvailability () { }

	SymbolAvailability (PlatformAvailability? iOsAvailability, PlatformAvailability? tvOsAvailability,
		PlatformAvailability? macCatalystAvailability, PlatformAvailability? macOSXAvailability)
	{
		iOS = iOsAvailability;
		TvOS = tvOsAvailability;
		MacCatalyst = macCatalystAvailability;
		MacOSX = macOSXAvailability;
	}

	public SymbolAvailability (SymbolAvailability other)
	{
		iOS = other.iOS;
		TvOS = other.TvOS;
		MacCatalyst = other.MacCatalyst;
		MacOSX = other.MacOSX;
	}

	public IEnumerable<PlatformAvailability> PlatformAvailabilities {
		get {
			foreach (var platform in new [] { iOS, TvOS, MacCatalyst, MacOSX }) {
				if (platform is not null)
					yield return platform.Value;
			}
		}
	}

	static PlatformAvailability? Merge (PlatformAvailability? childData, PlatformAvailability? parentData)
	{
		switch ((childData, parentData)) {
		case (null, null):
			break;
		case (not null, null):
			// copy of the child
			return new PlatformAvailability (childData.Value);
		case (null, not null):
			// copy or the parent
			return new PlatformAvailability (parentData.Value);
		case (not null, not null):
			// merge of the child with the parent
			return childData.Value.MergeWithParent (parentData.Value);
		}

		return null;
	}

	public SymbolAvailability MergeWithParent (SymbolAvailability? parent)
	{
		if (parent is null)
			return new SymbolAvailability (this);

		return new (
			Merge (iOS, parent.Value.iOS),
			Merge (TvOS, parent.Value.TvOS),
			Merge (MacCatalyst, parent.Value.MacCatalyst),
			Merge (MacOSX, parent.Value.MacOSX)
		);
	}

	public bool Equals (SymbolAvailability other)
	{
		return iOS == other.iOS && TvOS == other.TvOS && MacCatalyst == other.MacCatalyst && MacOSX == other.MacOSX;
	}

	public override bool Equals (object? obj)
	{
		return obj is SymbolAvailability other && Equals (other);
	}

	public override int GetHashCode ()
	{
		return HashCode.Combine (iOS, TvOS, MacCatalyst, MacOSX);
	}

	public static bool operator == (SymbolAvailability left, SymbolAvailability right)
	{
		return left.Equals (right);
	}

	public static bool operator != (SymbolAvailability left, SymbolAvailability right)
	{
		return !left.Equals (right);
	}
}
