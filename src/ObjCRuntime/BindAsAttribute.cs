using System;

#nullable enable

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
		}
		public Type Type;
		public Type? OriginalType;
#if BGENERATOR
		Type? nullable;
		Type? GetNullable (Generator generator)
		{
			if (nullable is null)
				nullable = Type.IsArray ? generator.TypeManager.GetUnderlyingNullableType (Type.GetElementType ()!) : generator.TypeManager.GetUnderlyingNullableType (Type);
			return nullable;
		}

		internal bool IsNullable (Generator generator)
		{
			return GetNullable (generator) is not null;
		}
		internal bool IsValueType (Generator generator)
		{
			return IsNullable (generator) ? GetNullable (generator)!.IsValueType : Type.IsValueType;

		}
#endif
	}
}
