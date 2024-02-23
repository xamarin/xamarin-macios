using System;

namespace ObjCRuntime {
	/// <summary>
	/// This attribute is applied to delegate parameters in a delegate to specify
	/// that the delegate parameter needs an C-style bridge.
	/// </summary>
	/// <remarks>
	/// <seealso cref='T:ObjCRuntime.BlockCallbackAttribute' />
	/// </remarks> 
	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false)]
	public class CCallbackAttribute : Attribute { }
}
