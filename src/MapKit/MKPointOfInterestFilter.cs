#if !WATCH // doesn't show up in watch headers
#if XAMCORE_2_0 || !MONOMAC
using System;
using ObjCRuntime;

namespace MapKit {

	public enum  MKPointOfInterestFilterType {
		Including,
        Excluding,
	}

	public partial class MKPointOfInterestFilter {
		public MKPointOfInterestFilter (string[] categories) : this (categories, MKPointOfInterestFilterType.Including)
		{
		}

		public MKPointOfInterestFilter (string[] categories, MKPointOfInterestFilterType type)
		{
			// two different `init*` would share the same C# signature
			switch (type) {
			case MKPointOfInterestFilterType.Including:
				Handle = InitIncludingCategories (categories);
				break;
			case MKPointOfInterestFilterType.Excluding:
				Handle = InitExcludingCategories (categories);
				break;
			default:
				throw new ArgumentException ("type");
			}
		}
	}
}
#endif
#endif // !WATCH
