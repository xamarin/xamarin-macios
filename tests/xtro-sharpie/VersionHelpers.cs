using System;
using Clang;
using static Extrospection.Helpers;

namespace Extrospection
{
	public static class VersionHelpers
	{
		public static Version Convert (VersionTuple version)
		{
			uint major = version.Major;
			uint minor = version.Minor.HasValue ? version.Minor.Value : 0;
			if (version.Subminor.HasValue)
				return new Version ((int)major, (int)minor, (int)version.Subminor.Value);
			else
				return new Version ((int)major, (int)minor);
		}

		public static bool VersionTooOldToCare (VersionTuple version)
		{
			switch (Helpers.Platform) {
			case Platforms.iOS:
				return version.Major < 6;
			case Platforms.macOS:
				return version.Minor < 7;
			case Platforms.tvOS:
				return version.Major < 9;
			case Platforms.watchOS:
				return version.Major < 2;
			default:
				throw new InvalidOperationException ($"Unknown platform {Platform} in VersionTooOldToCare");
			}
		}
	}
}
