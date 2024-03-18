using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#nullable enable

namespace bgen {

	// extension shared between NET and !NET make our lives a little easier
	public static class NullabilityInfoExtensions {
		public static bool IsNullable (this NullabilityInfo self)
			=> self.ReadState == NullabilityState.Nullable;
	}
#if !NET

	// from: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
	public enum NullabilityState : byte {
		Unknown = 0,
		NotNull = 1,
		Nullable = 2,
	}

	public class NullabilityInfo {
		public NullabilityState ReadState { get; set; } = NullabilityState.NotNull;
		public Type? Type { get; set; }
		public NullabilityInfo? ElementType { get; set; }
		public NullabilityInfo []? GenericTypeArguments { get; set; }
	}

	// helper class that allows to check the custom attrs in a method,property or field
	// in order for the generator to support ? in the bindings definition. This class should be
	// replaced by NullabilityInfoContext from the dotnet6 in the future. The API can be maintained (hopefully).
	public class NullabilityInfoContext {

		static readonly string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
		static readonly string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";

		public NullabilityInfoContext () { }

		public NullabilityInfo Create (PropertyInfo propertyInfo)
			=> Create (propertyInfo.PropertyType, propertyInfo.DeclaringType, propertyInfo.CustomAttributes);

		public NullabilityInfo Create (FieldInfo fieldInfo)
			=> Create (fieldInfo.FieldType, fieldInfo.DeclaringType, fieldInfo.CustomAttributes);

		public NullabilityInfo Create (ParameterInfo parameterInfo)
			=> Create (parameterInfo.ParameterType, parameterInfo.Member, parameterInfo.CustomAttributes);

		static NullabilityInfo Create (Type memberType, MemberInfo? declaringType,
			IEnumerable<CustomAttributeData>? customAttributes, int depth = 0)
		{
			var info = new NullabilityInfo { Type = memberType };

			if (memberType.IsByRef) {
				var e = memberType.GetElementType ();
				info = Create (e, declaringType, customAttributes);
				// override the returned type because it is returning e and not ref e
				info.Type = memberType;
				return info;
			}

			if (memberType.IsArray) {
				info.ElementType = Create (memberType.GetElementType ()!, declaringType, customAttributes, depth + 1);
			}

			if (memberType.IsGenericType) {
				// we need to get the nullability type of each of the generics, we use an array to make sure
				// order is kept
				var genericArguments = memberType.GetGenericArguments ();
				info.GenericTypeArguments = new NullabilityInfo [genericArguments.Length];
				for (int i = 0; i < genericArguments.Length; i++) {
					info.GenericTypeArguments [i] = Create (genericArguments [i], declaringType,
						customAttributes, depth + (i + 1)); // the depth can be complicated
				}
			}

			// there are two possible cases we have to take care of:
			//
			// 1. ValueType: If we are dealing with a value type the compiler converts it to Nullable<ValueType>
			// 2. ReferenceType: Ret types do not use an interface, but a custom attr is used in the signature

			if (memberType.IsValueType) {
				var nullableType = Nullable.GetUnderlyingType (memberType);
				info.ReadState = nullableType is not null ? NullabilityState.Nullable : NullabilityState.NotNull;
				info.Type = memberType;
				return info;
			}

			// at this point, we have to use the attributes (metadata) added by the compiler to decide if we have a
			// nullable type or not from a ref type. From https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
			//
			// The byte[] is constructed as follows:
			// Reference type: the nullability (0, 1, or 2), followed by the representation of the type arguments in order including containing types
			// Nullable value type: the representation of the type argument only
			// Non-generic value type: skipped
			// 	Generic value type: 0, followed by the representation of the type arguments in order including containing types
			// Array: the nullability (0, 1, or 2), followed by the representation of the element type
			// Tuple: the representation of the underlying constructed type
			// 	Type parameter reference: the nullability (0, 1, or 2, with 0 for unconstrained type parameter)

			// interesting case when we have constrains from a generic method
			if ((customAttributes?.Count () ?? 0) == 0 && memberType.CustomAttributes is not null)
				customAttributes = memberType.CustomAttributes;

			var nullable = customAttributes?.FirstOrDefault (
				x => x.AttributeType.FullName == NullableAttributeName);

			var flag = NullabilityState.Unknown;
			if (nullable is not null && nullable.ConstructorArguments.Count == 1) {
				var attributeArgument = nullable.ConstructorArguments [0];
				if (attributeArgument.ArgumentType == typeof (byte [])) {
					var args = (ReadOnlyCollection<CustomAttributeTypedArgument>) attributeArgument.Value!;
					if (args.Count > 0 && args [depth].ArgumentType == typeof (byte)) {
						flag = (NullabilityState) args [depth].Value;

					}
				} else if (attributeArgument.ArgumentType == typeof (byte)) {
					flag = (NullabilityState) attributeArgument.Value!;
				}

				info.Type = memberType;
				info.ReadState = flag;
				return info;
			}

			// we are using the context of the declaring type to decide if the type is nullable
			for (var type = declaringType; type is not null; type = type.DeclaringType) {
				var context = type.CustomAttributes
					.FirstOrDefault (x => x.AttributeType.FullName == NullableContextAttributeName);
				if (context is null ||
					context.ConstructorArguments.Count != 1 ||
					context.ConstructorArguments [0].ArgumentType != typeof (byte))
					continue;
				if (NullabilityState.Nullable == (NullabilityState) context.ConstructorArguments [0].Value!) {
					info.Type = memberType;
					info.ReadState = NullabilityState.Nullable;
					return info;
				}
			}

			// we need to consider the generic constrains
			if (!memberType.IsGenericParameter)
				return info;

			// if we do have a custom null atr in any of them, use it
			if (memberType.GenericParameterAttributes.HasFlag (GenericParameterAttributes.NotNullableValueTypeConstraint))
				info.ReadState = NullabilityState.NotNull;

			return info;
		}
	}
#endif

}
