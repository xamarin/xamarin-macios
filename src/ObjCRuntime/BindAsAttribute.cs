using System;
#if BGENERATOR
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;
#endif
namespace ObjCRuntime {
	//
	// BindAsAttribute
	//
	// The BindAsAttribute allows binding NSNumber, NSValue and NSString(enums) into more accurate C# types.
	// It can be used in methods, parameters and properties. The only restriction is that your member must not
	// be inside a [Protocol] or [Model] interface.
	//
	// For example:
	//
	//	[return: BindAs (typeof (bool?))]
	//	[Export ("shouldDrawAt:")]
	//	NSNumber ShouldDraw ([BindAs (typeof (CGRect))] NSValue rect);
	//
	// Would output:
	//
	//	[Export ("shouldDrawAt:")]
	//	bool? ShouldDraw (CGRect rect) { ... }
	//
	// Internally we will do the bool? <-> NSNumber and CGRect <-> NSValue conversions.
	//
	// BindAs also supports arrays of NSNumber|NSValue|NSString(enums)
	//
	// For example:
	//
	//	[BindAs (typeof (CAScroll []))]
	//	[Export ("supportedScrollModes")]
	//	NSString [] SupportedScrollModes { get; set; }
	//
	// Would output:
	//
	//	[Export ("supportedScrollModes")]
	//	CAScroll [] SupportedScrollModes { get; set; }
	//
	// CAScroll is a NSString backed enum, we will fetch the right NSString value and handle the type conversion.
	//
	[AttributeUsage (AttributeTargets.ReturnValue | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public class BindAsAttribute : Attribute {
		public BindAsAttribute (Type type)
		{
			Type = type;
#if BGENERATOR
			var nullable = type.IsArray ? TypeManager.GetUnderlyingNullableType (type.GetElementType ()) : TypeManager.GetUnderlyingNullableType (type);
			IsNullable = nullable != null;
			IsValueType = IsNullable ? nullable.IsValueType : type.IsValueType;
#endif
		}
		public Type Type;
		public Type OriginalType;
#if BGENERATOR
		internal readonly bool IsNullable;
		internal readonly bool IsValueType;
#endif
	}
}
