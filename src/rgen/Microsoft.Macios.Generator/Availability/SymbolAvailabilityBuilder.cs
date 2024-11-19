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
		/// <param name="supportedPlatform">The data of a SupportedOSPlatformAttribute.</param>
		public void Add (SupportedOSPlatformData supportedPlatform)
		{
			if (!supportedPlatforms.Contains (supportedPlatform.Platform))
				return;

			var builder = GetBuilder (supportedPlatform.Platform);
			builder.Add (supportedPlatform);
		}

		/// <summary>
		/// Add a new unsuspported verison to the SymbolAvailability.
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
