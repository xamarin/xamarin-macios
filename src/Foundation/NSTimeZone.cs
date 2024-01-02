using System;
using System.Collections.ObjectModel;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

	public partial class NSTimeZone {

		static ReadOnlyCollection<string> known_time_zone_names;

		// avoid exposing an array - it's too easy to break
		public static ReadOnlyCollection<string> KnownTimeZoneNames {
			get {
				if (known_time_zone_names is null)
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
