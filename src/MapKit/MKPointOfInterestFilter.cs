#if !WATCH // doesn't show up in watch headers
#if !MONOMAC
using System;
using ObjCRuntime;
using System.Runtime.Versioning;

#nullable enable

namespace MapKit {

	public enum  MKPointOfInterestFilterType {
		Including,
		Excluding,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MKPointOfInterestFilter {
		public MKPointOfInterestFilter (MKPointOfInterestCategory[] categories) : this (categories, MKPointOfInterestFilterType.Including)
		{
		}

		public MKPointOfInterestFilter (MKPointOfInterestCategory[] categories, MKPointOfInterestFilterType type)
		{
			// two different `init*` would share the same C# signature
			switch (type) {
			case MKPointOfInterestFilterType.Including:
				InitializeHandle (InitIncludingCategories (categories));
				break;
			case MKPointOfInterestFilterType.Excluding:
				InitializeHandle (InitExcludingCategories (categories));
				break;
			default:
				throw new ArgumentException (nameof (type));
			}
		}
	}
}
#endif
#endif // !WATCH
