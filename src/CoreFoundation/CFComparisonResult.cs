using Foundation;
using ObjCRuntime;

namespace CoreFoundation {
	[Native]
	public enum CFComparisonResult : long {
		LessThan = -1,
		EqualTo = 0,
		GreaterThan = 1,
	}
}
