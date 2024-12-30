using System;

namespace Xharness {
	[Flags]
	public enum TestPlatform {
		None = 0,

		iOS = 1,
		tvOS = 2,
		MacCatalyst = 4,
		Mac = 8,

		All = iOS | tvOS | MacCatalyst | Mac,
	}
}
