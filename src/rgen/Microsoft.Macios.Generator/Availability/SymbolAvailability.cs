using System;
using System.Collections.Generic;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

/// <summary>
/// Readonly structure that describes the availability of a symbol in the supported platforms of the SDK.
/// </summary>
readonly partial struct SymbolAvailability : IEquatable<SymbolAvailability> {
	static readonly HashSet<ApplePlatform> supportedPlatforms =
		[ApplePlatform.iOS, ApplePlatform.TVOS, ApplePlatform.MacOSX, ApplePlatform.MacCatalyst];

	readonly SortedDictionary<ApplePlatform, PlatformAvailability?> availabilities;

	SymbolAvailability (Dictionary<ApplePlatform, PlatformAvailability?> platforms)
	{
		// copy the dict, do not assign
		availabilities = new SortedDictionary<ApplePlatform, PlatformAvailability?> (platforms);
	}

	SymbolAvailability (IEnumerable<KeyValuePair<ApplePlatform, PlatformAvailability?>> platforms)
	{
		availabilities = new SortedDictionary<ApplePlatform, PlatformAvailability?> ();
		foreach (var (key, value) in platforms) {
			availabilities.Add (key, value);
		}
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="other">Symbol availability to copy,</param>
	public SymbolAvailability (SymbolAvailability other)
	{
		availabilities = other.availabilities;
	}

	/// <summary>
	/// Returns an IEnumerable of all the platform availabilities of the symbol. You can use the
	/// PlatformAvailability.Platform property to determine the platform.
	/// </summary>
	public IEnumerable<PlatformAvailability> PlatformAvailabilities {
		get {
			foreach (var platform in availabilities.Values) {
				if (platform is not null)
					yield return platform.Value;
			}
		}
	}

	/// <summary>
	/// Readonly indexer that allows to access the availability of a given platform.
	/// </summary>
	/// <param name="platform">The platform whose availability we want to retrieve.</param>
	public PlatformAvailability? this [ApplePlatform platform] {
		get {
			return (availabilities.ContainsKey (platform)) ? availabilities [platform] : null;
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

		// create the key value pairs for the supported platforms
		var merged = new List<KeyValuePair<ApplePlatform, PlatformAvailability?>> ();
		foreach (var platform in supportedPlatforms) {
			merged.Add (new (platform, Merge (this [platform], parent.Value [platform])));
		}

		return new (merged);
	}

	/// <inheritdoc />
	public bool Equals (SymbolAvailability other)
	{
		// loop over the supported platforms and ensure that the availabilities are the
		// same 
		foreach (var platform in supportedPlatforms) {
			if (this [platform] != other [platform]) {
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is SymbolAvailability other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		foreach (var platform in supportedPlatforms) {
			hashCode.Add (this [platform]);
		}

		return hashCode.ToHashCode ();
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
