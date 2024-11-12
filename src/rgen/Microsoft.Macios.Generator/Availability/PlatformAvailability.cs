using System;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

readonly partial struct PlatformAvailability : IEquatable<PlatformAvailability> {
	static readonly Version defaultVersion = new();

	public ApplePlatform Platform { get; }
	public Version? SupportedVersion { get; }

	readonly Dictionary<Version, string?> unsupported = new();
	public readonly IReadOnlyDictionary<Version, string?> UnsupportedVersions => unsupported;

	readonly Dictionary<Version, (string? Message, string? Url)> obsoleted = new();
	public readonly IReadOnlyDictionary<Version, (string? Message, string? Url)> ObsoletedVersions => obsoleted;

	public static bool IsDefaultVersion (Version version) => version == defaultVersion;

	PlatformAvailability (ApplePlatform platform, Version? supportedVersion,
		Dictionary<Version, string?> unsupportedVersions,
		Dictionary<Version, (string? Message, string? Url)> obsoletedVersions)
	{
		Platform = platform;
		SupportedVersion = supportedVersion;
		unsupported = unsupportedVersions;
		obsoleted = obsoletedVersions;
	}

	public PlatformAvailability (PlatformAvailability other)
	{
		Platform = other.Platform;
		SupportedVersion = other.SupportedVersion;
		// important, the default copy constructor of a record wont do this. It will use the same ref, not
		// something we want to do because it will mean that two records will modify the same collection
		unsupported = new(other.unsupported);
		obsoleted = new(other.ObsoletedVersions);
	}
	
	public PlatformAvailability MergeWithParent (PlatformAvailability? parent)
	{
		if (parent is null || Platform != parent.Value.Platform)
			// we cant merge different platforms, do return a copy of this
			return new(this);

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
	
	public bool Equals (PlatformAvailability other)
	{
		var obsoleteComparer = new DictionaryComparer<Version, (string?, string?)> ();
		var unsupportedComparer = new DictionaryComparer<Version, string?> ();

		return Platform == other.Platform && 
		       Equals (SupportedVersion, other.SupportedVersion) &&
		       unsupportedComparer.Equals (unsupported, other.unsupported) &&
		       obsoleteComparer.Equals (obsoleted, other.obsoleted) && Platform == other.Platform;
	}

	public override bool Equals (object? obj)
	{
		return obj is PlatformAvailability other && Equals (other);
	}

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
}
