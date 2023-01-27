using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#nullable enable

namespace bgen {

	// from: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
	public enum NullableFlag : byte {
		Oblivious = 0,
		NotAnnotated = 1,
		Annotated = 2,
	}

	public class NullabilityInfo {
		public bool IsNullable { get; set; }
		public Type? Type { get; set; }
		public NullabilityInfo? ElementType { get; set; }
		public NullabilityInfo []? GenericTypeArguments { get; set; }
	}
	
	// helper class that allows to check the custom attrs in a method,property or field
	// in order for the generator to support ? in the bindings definition. This class should be
	// replaced by NullabilityInfoContext from the dotnet6 in the future. The API can be maintained (hopefully).
	public static class GeneratorNullabilityInfoContext {

		static readonly string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
		static readonly string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";
		
		public static NullabilityInfo Create (PropertyInfo propertyInfo)
			=> Create(propertyInfo.PropertyType, propertyInfo.DeclaringType, propertyInfo.CustomAttributes);

		public static NullabilityInfo Create (FieldInfo fieldInfo)
			=> Create (fieldInfo.FieldType, fieldInfo.DeclaringType, fieldInfo.CustomAttributes);

		public static NullabilityInfo Create (ParameterInfo parameterInfo)
			=> Create (parameterInfo.ParameterType, parameterInfo.Member, parameterInfo.CustomAttributes);

		static NullabilityInfo Create (Type memberType, MemberInfo? declaringType,
			IEnumerable<CustomAttributeData>? customAttributes, int depth = 0)
		{
			var info = new NullabilityInfo { Type = memberType };

			if (memberType.IsArray) {
				info.ElementType = Create (memberType.GetElementType ()!, declaringType, customAttributes, depth + 1);
			}

			if (memberType.IsGenericType) {
				// we need to get the nullability type of each of the generics, we use an array to make sure
				// order is kept
				var genericArguments = memberType.GetGenericArguments ();
				info.GenericTypeArguments = new NullabilityInfo[genericArguments.Length];
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
				info.IsNullable = nullableType != null;
				info.Type = nullableType ?? memberType;
				return info;
			}

			// at this point, we  have to use the attributes (metadata) added by the compiler to decide if we have a
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
			
			var nullable = customAttributes?.FirstOrDefault(
				x => x.AttributeType.FullName == NullableAttributeName);

			NullableFlag flag = NullableFlag.Oblivious;
			if (nullable is not  null && nullable.ConstructorArguments.Count == 1)
			{
				var attributeArgument = nullable.ConstructorArguments[0];
				if (attributeArgument.ArgumentType == typeof(byte[]))
				{
					var args = (ReadOnlyCollection<CustomAttributeTypedArgument>) attributeArgument.Value!;
					if (args.Count > 0 && args[depth].ArgumentType == typeof(byte)) {
						flag = (NullableFlag) args [depth].Value;

					}
				} else if (attributeArgument.ArgumentType == typeof(byte)) {
					flag = (NullableFlag) attributeArgument.Value!;
				}

				if (flag != NullableFlag.Annotated) {
					info.IsNullable = false;
					info.Type = memberType;
					return info;
				}

				info.Type = memberType;
				info.IsNullable = true;
				return info;
			}

			// we are using the context of the declaring type to decide if the type is nullable
			for (var type = declaringType; type != null; type = type.DeclaringType)
			{
				var context = type.CustomAttributes
					.FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeName);
				if (context is null ||
				    context.ConstructorArguments.Count != 1 ||
				    context.ConstructorArguments [0].ArgumentType != typeof (byte)) continue;
				if (NullableFlag.Annotated == (NullableFlag) context.ConstructorArguments [0].Value!) {
					info.Type = memberType;
					info.IsNullable = true;
					return info;
				}
			}

			return info;
		}
	}
}
