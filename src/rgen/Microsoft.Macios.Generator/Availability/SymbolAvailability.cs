using System;
using System.Collections.Generic;

namespace Microsoft.Macios.Generator.Availability;

/// <summary>
/// Readonly structure that describes the availability of a symbol in the supported platforms of the SDK.
/// </summary>
readonly partial struct SymbolAvailability : IEquatable<SymbolAvailability> {
	/// <summary>
	/// iOS platform availability. If null, the default versions are used.
	/// </summary>
	public PlatformAvailability? iOS { get; }

	/// <summary>
	/// TvOS platform availability. If null, the default versions are used.
	/// </summary>
	public PlatformAvailability? TvOS { get; }

	/// <summary>
	/// MacCatalyst platform availability. If null, the default versions are used.
	/// </summary>
	public PlatformAvailability? MacCatalyst { get; }

	/// <summary>
	/// MacOS platform availability. If null, the default versions are used.
	/// </summary>
	public PlatformAvailability? MacOSX { get; }

	SymbolAvailability (PlatformAvailability? iOsAvailability, PlatformAvailability? tvOsAvailability,
		PlatformAvailability? macCatalystAvailability, PlatformAvailability? macOSXAvailability)
	{
		iOS = iOsAvailability;
		TvOS = tvOsAvailability;
		MacCatalyst = macCatalystAvailability;
		MacOSX = macOSXAvailability;
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="other">Symbol availability to copy,</param>
	public SymbolAvailability (SymbolAvailability other)
	{
		iOS = other.iOS;
		TvOS = other.TvOS;
		MacCatalyst = other.MacCatalyst;
		MacOSX = other.MacOSX;
	}

	/// <summary>
	/// Returns an IEnumerable of all the platform availabilities of the symbol. You can use the
	/// PlatformAvailability.Platform property to determine the platform.
	/// </summary>
	public IEnumerable<PlatformAvailability> PlatformAvailabilities {
		get {
			foreach (var platform in new [] { iOS, TvOS, MacCatalyst, MacOSX }) {
				if (platform is not null)
					yield return platform.Value;
			}
		}
	}

	/// <summary>
	/// Merged the current platform symbol availabilities to those of a parent symbol. This allows to ensure that
	/// we have all the information from both the parent and the child.
	///
	/// The logic is as follows:
	/// 1. If both params are null, return null.
	/// 2. If one of the params is not null, but the other is, return the one that is not null.
	/// 3. If both objects are not null, return the merged between the child and the parent.
	/// </summary>
	/// <param name="childData">The childs platform availability.</param>
	/// <param name="parentData"></param>
	/// <returns>The result of merging both platform availabilities.</returns>
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

	/// <summary>
	/// Merge the current symbol availability with that of a parent symbol.
	/// </summary>
	/// <param name="parent">The parents symbol availability.</param>
	/// <returns>The result of mergeing the current iOS, Tv, Catalyst and MacOS availabiliti4es.</returns>
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

	/// <inheritdoc />
	public bool Equals (SymbolAvailability other)
	{
		return iOS == other.iOS && TvOS == other.TvOS && MacCatalyst == other.MacCatalyst && MacOSX == other.MacOSX;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is SymbolAvailability other && Equals (other);
	}

	/// <inheritdoc />
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
