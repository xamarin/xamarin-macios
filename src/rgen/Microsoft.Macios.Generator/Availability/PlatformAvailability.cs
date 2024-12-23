using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

/// <summary>
/// Structure that represents the availability os a symbol in a specific platform. This
/// structure provides a union of all the Supported, Unsupported and Obsoleted attributes in a symbol
/// for a specific platform.
/// </summary>
readonly partial struct PlatformAvailability : IEquatable<PlatformAvailability> {

	/// <summary>
	///  The default version that is used to represents that a platform is fully supported/unsupported.
	/// </summary>
	static readonly Version defaultVersion = new ();

	/// <summary>
	/// Targeted platform.
	/// </summary>
	public ApplePlatform Platform { get; }

	/// <summary>
	/// The supported version of the platform. If null, that means that the user did not add a SupportedOSPlatform
	/// attribute. 
	/// </summary>
	public Version? SupportedVersion { get; }

	readonly SortedDictionary<Version, string?> unsupported = new ();

	/// <summary>
	/// Dictionary that contains all the obsoleted versions and their optional data.
	/// </summary>
	public readonly IReadOnlyDictionary<Version, string?> UnsupportedVersions => unsupported;

	readonly SortedDictionary<Version, (string? Message, string? Url)> obsoleted = new ();

	/// <summary>
	/// The Dictionary which contains all the unsupported versions and their optional data.
	/// </summary>
	public readonly IReadOnlyDictionary<Version, (string? Message, string? Url)> ObsoletedVersions => obsoleted;


	/// <summary>
	/// Returns if a version is the default version.
	/// </summary>
	/// <param name="version">Version being tested.</param>
	/// <returns>True if the version is default.</returns>
	public static bool IsDefaultVersion (Version version) => version == defaultVersion;

	PlatformAvailability (ApplePlatform platform, Version? supportedVersion,
		SortedDictionary<Version, string?> unsupportedVersions,
		SortedDictionary<Version, (string? Message, string? Url)> obsoletedVersions)
	{
		Platform = platform;
		SupportedVersion = supportedVersion;
		unsupported = new (unsupportedVersions);
		obsoleted = new (obsoletedVersions);
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="other">The availability to copy.</param>
	public PlatformAvailability (PlatformAvailability other)
	{
		Platform = other.Platform;
		SupportedVersion = other.SupportedVersion;
		// Important: the default copy constructor of a record won't do this. It will use the same ref, not
		// something we want to do because it will mean that two records will modify the same collection
		unsupported = new (other.unsupported);
		obsoleted = new (other.obsoleted);
	}

	/// <summary>
	/// Merge the current platform availability with that of a parent symbol. This allows to ensure that
	/// the right availability for the symbol is present.
	///
	/// The logic is as follows:
	///
	///	1. We always try to pik the most restrictive supported version.
	/// 2. Merge unsupported versions, keep the childs message if the version is present in the parent.
	/// 3. Merge the obsoleted versions, heep the childs message and url if the version is present in the parent.
	/// </summary>
	/// <param name="parent">The parent platform availability.</param>
	/// <remarks>Merging two platform availabilities with diff platforms will result in a copy of the childs
	/// availability.</remarks>
	public PlatformAvailability MergeWithParent (PlatformAvailability? parent)
	{
		if (parent is null || Platform != parent.Value.Platform)
			// we cant merge different platforms, do return a copy of this
			return new (this);

		// create a builder that will be used to create the merged result
		var builder = new Builder (Platform);

		// the decisions we are going to make are as follows
		// 1. If the parent has unsupported versions we will add them to the merge. If the kid has them, we will
		// add them too
		foreach (var (version, message) in parent.Value.unsupported) {
			builder.AddUnsupportedVersion (version, message);
		}

		foreach (var (version, message) in unsupported) {
			builder.AddUnsupportedVersion (version, message);
		}

		// 2. if supported in the platform, we will always pick the larges version between
		//    the two. Now, because we might be unsupported

		var supportedVersion = (parent.Value.SupportedVersion > SupportedVersion)
			? parent.Value.SupportedVersion
			: SupportedVersion;
		if (supportedVersion is not null)
			builder.AddSupportedVersion (supportedVersion);

		// similar to the unsupported versions, if the parent has obsolete ones, we will add them
		foreach (var (version, obsoleteInfo) in parent.Value.obsoleted) {
			builder.AddObsoletedVersion (version, obsoleteInfo.Message, obsoleteInfo.Url);
		}

		foreach (var (version, obsoleteInfo) in obsoleted) {
			builder.AddObsoletedVersion (version, obsoleteInfo.Message, obsoleteInfo.Url);
		}

		return builder.ToImmutable ();
	}

	/// <inheritdoc />
	public bool Equals (PlatformAvailability other)
	{
		var obsoleteComparer = new DictionaryComparer<Version, (string?, string?)> ();
		var unsupportedComparer = new DictionaryComparer<Version, string?> ();

		return Platform == other.Platform &&
			   Equals (SupportedVersion, other.SupportedVersion) &&
			   unsupportedComparer.Equals (unsupported, other.unsupported) &&
			   obsoleteComparer.Equals (obsoleted, other.obsoleted) && Platform == other.Platform;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is PlatformAvailability other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (unsupported, obsoleted, (int) Platform, SupportedVersion);
	}

	public static bool operator == (PlatformAvailability left, PlatformAvailability right)
	{
		return left.Equals (right);
	}

	public static bool operator != (PlatformAvailability left, PlatformAvailability right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("{ ");
		sb.Append ($"Platform: {Platform} ");
		sb.Append ($"Supported: '{SupportedVersion?.ToString ()}' ");
		sb.Append ("Unsupported: [");
		sb.AppendJoin (", ", unsupported.Select (v => $"'{v.Key}': '{v.Value?.ToString () ?? "null" }'"));
		sb.Append ("], Obsoleted: [");
		sb.AppendJoin (", ", obsoleted.Select (v => $"'{v.Key}': ('{v.Value.Message?.ToString () ?? "null"}', '{v.Value.Url?.ToString () ?? "null"}')"));
		sb.Append ("] }");
		return sb.ToString ();
	}
}
