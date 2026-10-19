// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

#if !__TVOS__

namespace WidgetKit {

	/// <summary>Requests updates to the timelines of the app's widgets.</summary>
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[UnsupportedOSPlatform ("tvos")]
	public static class WidgetCenter {

		/// <summary>Requests new timelines for all widgets of the specified kind.</summary>
		/// <param name="kind">The kind string specified by the widget's configuration.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="kind" /> argument is <see langword="null" />.</exception>
		public static void ReloadTimelines (string kind)
		{
			ArgumentNullException.ThrowIfNull (kind);
			StoreKit.XamarinSwiftFunctions.ReloadTimelines (kind);
		}

		/// <summary>Requests new timelines for all of the app's widgets.</summary>
		public static void ReloadAllTimelines ()
		{
			StoreKit.XamarinSwiftFunctions.ReloadAllTimelines ();
		}
	}
}

#endif // !__TVOS__
