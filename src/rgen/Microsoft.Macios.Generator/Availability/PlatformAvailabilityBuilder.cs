// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

readonly partial struct PlatformAvailability {
	/// <summary>
	/// Create a new builder for the platform.
	/// </summary>
	/// <param name="platform">The platform whose availability is going to be described.</param>
	/// <returns>A new builder for the provided platform.</returns>
	public static Builder CreateBuilder (ApplePlatform platform) => new (platform);

	/// <summary>
	/// A writable PlatformAvailability accessor.
	/// </summary>
	public sealed class Builder {
		readonly ApplePlatform platform;
		Version? supportedVersion;
		readonly SortedDictionary<Version, string?> unsupported = new ();
		readonly SortedDictionary<Version, (string? Message, string? Url)> obsoleted = new ();

		/// <summary>
		/// Create a builder for the given platform.
		/// </summary>
		/// <param name="platform">The platform whose availability we are going to build.</param>
		internal Builder (ApplePlatform platform) => this.platform = platform;

		/// <summary>
		/// Adds the version as the supported version. We want to be as strict as possible. Therefore, we will always
		/// keep the latest version. If a smaller version has been as supported version, the method will update the version
		/// else the version is ignored.
		/// </summary>
		/// <param name="version">A new supported version.</param>
		internal void AddSupportedVersion (Version version)
		{
			if (unsupported.ContainsKey (defaultVersion))
				// platform is unsupported, do nothing
				return;

			// update the supported version to the larges of the two
			if (supportedVersion is null || version > supportedVersion)
				supportedVersion = version;
		}

		/// <summary>
		/// Method that parses the data from the attribute data and add a new supported platform.
		/// </summary>
		/// <param name="supportedPlatform">The SupportedOS attribute data.</param>
		public void Add (SupportedOSPlatformData supportedPlatform)
		{
			// ignore data from platforms that we do not care
			if (supportedPlatform.Platform != platform)
				return;
			// no version is present, therefore try to set the default one
			AddSupportedVersion (supportedPlatform.Version);
		}

		/// <summary>
		/// Adds a new version to the list of unsupported versions. If the platform is unsupported, the version is ignored.
		/// </summary>
		/// <param name="version">The new unsupported version.</param>
		/// <param name="message">The optional message of the unsupported version.</param>
		internal void AddUnsupportedVersion (Version version, string? message)
		{
			// adding an unsupported version, due to the way the API is designed is more complicated. It can be
			// that a selector/member is unsupported in more than one version, so we need to keep track of that
			// the only time in which we make an exception is when we unsupported a platform (no version number)
			// if that is the case, we will remove all unsupported versions and just un-support the platform
			// ignore data from platforms that we do not care
			if (version == defaultVersion) {
				unsupported.Clear ();
				unsupported [defaultVersion] = message;
				// we are unsupporting the platform! that means if we supported it or any version, that should be
				// set back to null
				supportedVersion = null;
			} else {
				// we have a version, that does not mean we do care about that data, first we want to check
				// that we did not fully unsupported the platform, if that is the case, we ignore this version
				if (unsupported.ContainsKey (defaultVersion))
					return;
				unsupported [version] = message;
			}
		}

		/// <summary>
		/// Add a new unsupported version of the platform to the availability struct.
		/// </summary>
		/// <param name="unsupportedPlatform">An unsupported version.</param>
		public void Add (UnsupportedOSPlatformData unsupportedPlatform)
		{
			if (unsupportedPlatform.Platform != platform)
				return;
			AddUnsupportedVersion (unsupportedPlatform.Version, unsupportedPlatform.Message);
		}


		/// <summary>
		/// Add a new obsoleted version of the platform to the availability struct.
		/// </summary>
		/// <param name="version">The supported versions to add.</param>
		/// <param name="message">Optional obsolete message.</param>
		/// <param name="url">Optional documentation url.</param>
		public void AddObsoletedVersion (Version version, string? message, string? url)
		{
			if (version == defaultVersion) {
				// we are obsoleting for all versions
				obsoleted.Clear ();
				obsoleted [defaultVersion] = (message, url);
			} else {
				// we have a version, only add it if we do not obsoleted the entire platform
				if (obsoleted.ContainsKey (defaultVersion))
					return;
				// we have a version, only add it if we do not obsoleted the entire platform
				obsoleted [version] = (message, url);
			}
		}

		/// <summary>
		/// Add a new obsolete version from the ObsoletedOSPlatformAttribute.
		/// </summary>
		/// <param name="obsoletedPlatform">The data of the ObsoletedOSPlatformAttribute.</param>
		public void Add (ObsoletedOSPlatformData obsoletedPlatform)
		{
			if (obsoletedPlatform.Platform != platform)
				return;
			AddObsoletedVersion (obsoletedPlatform.Version, obsoletedPlatform.Message, obsoletedPlatform.Url);
		}

		/// <summary>
		/// Create the immutable structure from the builder data.
		/// </summary>
		/// <returns>A new readonly structure that contains the platform availability.</returns>
		public PlatformAvailability ToImmutable ()
		{
			return new PlatformAvailability (platform, supportedVersion, unsupported, obsoleted);
		}

		/// <summary>
		/// Clear all the versions that have been added to the platform availability.
		/// </summary>
		public void Clear ()
		{
			supportedVersion = null;
			unsupported.Clear ();
			obsoleted.Clear ();
		}
	}
}
