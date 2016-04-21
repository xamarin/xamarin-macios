//
// Release attribute
//
using System;

namespace XamCore.ObjCRuntime {
	[AttributeUsage (AttributeTargets.ReturnValue)]
#if XAMCORE_2_0
	sealed
#endif
	public class ReleaseAttribute : Attribute {
	}
}
	
