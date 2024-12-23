using System;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

readonly partial struct SymbolAvailability {
	/// <summary>
	/// Create a new symbol availability builder.
	/// </summary>
	/// <returns></returns>
	public static Builder CreateBuilder () => new ();

	/// <summary>
	/// A writable SymbolAvailability accessor.
	/// </summary>
	public sealed class Builder {
		readonly Dictionary<ApplePlatform, PlatformAvailability.Builder> platforms = new ();

		internal Builder () { }

		/// <summary>
		/// Returns the PlatformAvailability for the given platform. If we did not have a builder for the
		/// platform, a new one is created and added to the dictionary.
		/// </summary>
		/// <param name="platform">The platforms whose builder we want to retrieve.</param>
		/// <returns>A PlatformAvailability.Builder for the given platform.</returns>
		PlatformAvailability.Builder GetBuilder (ApplePlatform platform)
		{
			if (platforms.TryGetValue (platform, out var builder))
				return builder;

			builder = PlatformAvailability.CreateBuilder (platform);
			platforms [platform] = builder;
			return builder;
		}

		/// <summary>
		/// Add a new obsoleted version of the platform to the availability struct.
		/// </summary>
		/// <param name="platform">Platforms whose availability we are updating.</param>
		/// <param name="version">The supported versions to add.</param>
		/// <param name="message">Optional obsolete message.</param>
		/// <param name="url">Optional documentation url.</param>
		internal void AddObsoletedVersion (ApplePlatform platform, Version version, string? message, string? url)
		{
			if (!supportedPlatforms.Contains (platform))
				return;

			var builder = GetBuilder (platform);
			builder.AddObsoletedVersion (version, message, url);
		}

		/// <summary>
		/// Adds a new obsoleted version to the SymbolAvailability.
		/// </summary>
		/// <param name="obsoletedOsPlatform">The data of a ObsoleteOSPlatformAttribute.</param>
		public void Add (ObsoletedOSPlatformData obsoletedOsPlatform)
		{
			if (!supportedPlatforms.Contains (obsoletedOsPlatform.Platform))
				return;

			var builder = GetBuilder (obsoletedOsPlatform.Platform);
			builder.Add (obsoletedOsPlatform);
		}


		/// <summary>
		/// Add a new supported version to the SymbolAvailability.
		/// </summary>
		/// <param name="platform">Platforms whose availability we are updating.</param>
		/// <param name="version">The supported versions to add.</param>
		internal void AddSupportedVersion (ApplePlatform platform, Version version)
		{
			if (!supportedPlatforms.Contains (platform))
				return;
			var builder = GetBuilder (platform);
			builder.AddSupportedVersion (version);
		}

		/// <summary>
		/// Add a new supported version to the SymbolAvailability.
		/// </summary>
		/// <param name="supportedPlatform">The data of a SupportedOSPlatformAttribute.</param>
		public void Add (SupportedOSPlatformData supportedPlatform)
		{
			if (!supportedPlatforms.Contains (supportedPlatform.Platform))
				return;

			var builder = GetBuilder (supportedPlatform.Platform);
			builder.Add (supportedPlatform);
		}

		/// <summary>
		/// Adds a new version to the list of unsupported versions. If the platform is unsupported, the version is ignored.
		/// </summary>
		/// <param name="platform">Platforms whose availability we are updating.</param>
		/// <param name="version">The new unsupported version.</param>
		/// <param name="message">The optional message of the unsupported version.</param>
		internal void AddUnsupportedVersion (ApplePlatform platform, Version version, string? message)
		{
			if (!supportedPlatforms.Contains (platform))
				return;

			var builder = GetBuilder (platform);
			builder.AddUnsupportedVersion (version, message);
		}

		/// <summary>
		/// Add a new unsuspported version to the SymbolAvailability.
		/// </summary>
		/// <param name="unsupportedPlatform">The data of a UnsupportedOSPlatformAttribute.</param>
		public void Add (UnsupportedOSPlatformData unsupportedPlatform)
		{
			if (!supportedPlatforms.Contains (unsupportedPlatform.Platform))
				return;

			var builder = GetBuilder (unsupportedPlatform.Platform);
			builder.Add (unsupportedPlatform);
		}

		/// <summary>
		/// Clear all the versions that have been added to the platform availability.
		/// </summary>
		public void Clear () => platforms.Clear ();

		/// <summary>
		/// Create the immutable structure from the builder data.
		/// </summary>
		/// <returns>A new readonly structure that contains the symbol availability.</returns>
		public SymbolAvailability ToImmutable ()
		{
			var dict = new Dictionary<ApplePlatform, PlatformAvailability?> ();
			foreach (var platform in supportedPlatforms) {
				dict [platform] = platforms.ContainsKey (platform)
					? platforms [platform].ToImmutable ()
					: null;
			}

			return new (dict);
		}
	}
}
