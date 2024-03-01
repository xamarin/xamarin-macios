using System;

namespace ObjCRuntime {
	/// <summary>
	/// This attribute is applied to delegate parameters in a delegate to specify
	/// that the delegate parameter needs an Objective-C Block-style bridge.
	/// </summary>
	/// <remarks>
	/// <seealso cref='T:ObjCRuntime.CCallbackAttribute' />
	/// </remarks>
	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false)]
	public class BlockCallbackAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="BlockCallbackAttribute"/> class.
		/// </summary>
		public CCallbackAttribute ()
		{
		}
	}
}
