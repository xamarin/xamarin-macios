using System;

namespace ObjCRuntime {
	// These two attributes can be applied to parameters in a C# delegate
	// declaration to specify what kind of bridge needs to be provided on
	// callback.   Either a Block style setup, or a C-style setup
	//
	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false)]
	public class BlockCallbackAttribute : Attribute { }

	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false)]
	public class CCallbackAttribute : Attribute { }
}
