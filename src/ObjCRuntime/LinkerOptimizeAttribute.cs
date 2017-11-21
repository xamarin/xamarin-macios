//
// LinkerOptimize.cs: Apply this to methods to tell the linker to optimize them
//

using System;
using System.Runtime.InteropServices;

namespace XamCore.ObjCRuntime {

	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field, AllowMultiple = false)]
	public class LinkerOptimizeAttribute : Attribute { }
}
