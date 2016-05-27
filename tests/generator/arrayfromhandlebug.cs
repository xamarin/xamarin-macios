using System;
using System.Runtime.InteropServices;

#if !XAMCORE_2_0
#if MONOMAC
using MonoMac.Foundation;
#else
using MonoTouch.Foundation;
#endif
#else
using Foundation;
#endif

namespace AudioUnit {
	// @interface AUParameterNode : NSObject
	[BaseType (typeof(NSObject))]
	interface AUParameterNode
	{
	}

	// @interface AUParameterGroup : AUParameterNode <NSSecureCoding>
	[BaseType (typeof(AUParameterNode))]
	interface AUParameterGroup : INSSecureCoding
	{
		[Export ("allParameters")]
		AUParameter[] AllParameters { get; }
	}

	// @interface AUParameter : AUParameterNode <NSSecureCoding>
	[BaseType (typeof(AUParameterNode))]
	interface AUParameter : INSSecureCoding
	{
	}

	// Commenting this out "fixes" the problem
	[BaseType (typeof(NSObject))]
	interface AudioUnit
	{
	}
}
