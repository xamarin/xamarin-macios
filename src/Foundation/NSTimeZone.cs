using System;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;

namespace Foundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSTimeZone {
		
		static ReadOnlyCollection<string> known_time_zone_names;
		
		// avoid exposing an array - it's too easy to break
		public static ReadOnlyCollection<string> KnownTimeZoneNames {
			get {
				if (known_time_zone_names == null)
					known_time_zone_names = new ReadOnlyCollection<string> (_KnownTimeZoneNames);
				return known_time_zone_names;
			}
		}
		
		public override string ToString ()
		{
			return Name;
		}
	}
}
