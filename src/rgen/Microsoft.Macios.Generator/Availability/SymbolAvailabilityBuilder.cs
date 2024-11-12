using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

readonly partial struct SymbolAvailability {
	public static Builder CreateBuilder () => new ();

	public sealed class Builder {
		readonly Dictionary<ApplePlatform, PlatformAvailability.Builder> platforms = new ();

		readonly HashSet<ApplePlatform> supportedPlatforms =
			[ApplePlatform.iOS, ApplePlatform.TVOS, ApplePlatform.MacOSX, ApplePlatform.MacCatalyst];

		internal Builder () { }

		PlatformAvailability.Builder GetBuilder (ApplePlatform platform)
		{
			if (platforms.TryGetValue (platform, out var builder))
				return builder;

			builder = PlatformAvailability.CreateBuilder (platform);
			platforms [platform] = builder;
			return builder;
		}

		public void Add (ObsoletedOSPlatformData obsoletedOsPlatform)
		{
			if (!supportedPlatforms.Contains (obsoletedOsPlatform.Platform))
				return;

			var builder = GetBuilder (obsoletedOsPlatform.Platform);
			builder.Add (obsoletedOsPlatform);
		}

		public void Add (SupportedOSPlatformData supportedPlatform)
		{
			if (!supportedPlatforms.Contains (supportedPlatform.Platform))
				return;

			var builder = GetBuilder (supportedPlatform.Platform);
			builder.Add (supportedPlatform);
		}

		public void Add (UnsupportedOSPlatformData unsupportedPlatform)
		{
			if (!supportedPlatforms.Contains (unsupportedPlatform.Platform))
				return;

			var builder = GetBuilder (unsupportedPlatform.Platform);
			builder.Add (unsupportedPlatform);
		}

		public void Clear () => platforms.Clear ();

		public SymbolAvailability ToImmutable ()
		{
			PlatformAvailability? iOS =
				platforms.ContainsKey (ApplePlatform.iOS) ? platforms [ApplePlatform.iOS].ToImmutable () : null;
			PlatformAvailability? tvOS =
				platforms.ContainsKey (ApplePlatform.TVOS) ? platforms [ApplePlatform.TVOS].ToImmutable () : null;
			PlatformAvailability? macCatalyst =
				platforms.ContainsKey (ApplePlatform.MacCatalyst)
					? platforms [ApplePlatform.MacCatalyst].ToImmutable ()
					: null;
			PlatformAvailability? macOS =
				platforms.ContainsKey (ApplePlatform.MacOSX) ? platforms [ApplePlatform.MacOSX].ToImmutable () : null;
			return new (iOS, tvOS, macCatalyst, macOS);
		}
	}
}
