//
// Release attribute
//
using System;

namespace ObjCRuntime {
	[AttributeUsage (AttributeTargets.ReturnValue)]
	public sealed class ReleaseAttribute : Attribute {
	}
}

