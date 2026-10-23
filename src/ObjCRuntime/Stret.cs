//
// Stret.cs: Code to determine if a function is a stret function or not.
// 
// This file is shared between the product assemblies and the generator.
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc. 
//

// There's some unreachable code in this file, which is somewhat on purpose to
// avoid too much #if spaghetti code - instead of using several #if
// conditions, the code declares boolean constant in some #if conditions (but
// only some, others have those same booleans as static fields), which means
// that the C# compiler will detect unreachable code when those #if conditions
// declare constants instead of static fields. The advantage of using
// constants is that the C# compiler will automatically remove unreachable
// code (so the warning could also be fixed by always declaring static fields,
// but at the cost of more IL).
#pragma warning disable 162 // Unreachable code detected.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if !BGENERATOR
using Generator = System.Object;
#endif


#nullable enable

namespace ObjCRuntime {
	class Stret {
		/// <summary>Determines whether a return type requires x86_64 stret calling conventions.</summary>
		/// <param name="returnType">The return type to inspect.</param>
		/// <param name="generator">The generator context.</param>
		/// <returns><see langword="true"/> if stret calling conventions are required; otherwise, <see langword="false"/>.</returns>
		public static bool X86_64NeedStret (Type returnType, Generator? generator)
		{
			ThrowIfNull (returnType, nameof (returnType));
#if BGENERATOR
			ThrowIfNull (generator, nameof (generator));
#endif

			Type t = returnType;

			if (!t.IsValueType || t.IsEnum || IsBuiltInType (t))
				return false;

			var fieldTypes = new List<Type> ();
			return GetValueTypeSize (t, fieldTypes, generator) > 16;
		}

		// IL2070: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicFields', 'DynamicallyAccessedMemberTypes.NonPublicFields' in call to 'System.Type.GetFields(BindingFlags)'. The parameter 'type' of method 'ObjCRuntime.Stret.GetValueTypeSize(Type, List<Type>, Boolean, Object)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2070", Justification = "Computing the size of a struct is safe, because the trimmer can't remove fields that would affect the size of a marshallable struct (it could affect marshalling behavior).")]
		internal static int GetValueTypeSize (Type type, List<Type> fieldTypes, Generator? generator)
		{
			ThrowIfNull (type, nameof (type));
			ThrowIfNull (fieldTypes, nameof (fieldTypes));
#if BGENERATOR
			ThrowIfNull (generator, nameof (generator));
#endif

			int size = 0;
			int maxElementSize = 1;

			if (type.IsExplicitLayout) {
				// Find the maximum of "field size + field offset" for each field.
				foreach (var field in type.GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
#if BGENERATOR
					var fieldOffset = generator.AttributeManager.GetCustomAttribute<FieldOffsetAttribute> (field);
#else
					var fieldOffset = (FieldOffsetAttribute?) Attribute.GetCustomAttribute (field, typeof (FieldOffsetAttribute));
#endif
					if (fieldOffset is null)
						throw new Exception ($"Unable to find a FieldOffset attribute on field {GetTypeName (field.DeclaringType)}.{field.Name}");
					var elementSize = 0;
					GetValueTypeSize (type, field.FieldType, fieldTypes, ref elementSize, ref maxElementSize, generator);
					size = Math.Max (size, elementSize + fieldOffset.Value);
				}
			} else {
				GetValueTypeSize (type, type, fieldTypes, ref size, ref maxElementSize, generator);
			}

			if (size % maxElementSize != 0)
				size += (maxElementSize - size % maxElementSize);

			return size;
		}

		static int AlignAndAdd (Type original_type, int size, int add, ref int max_element_size)
		{
			max_element_size = Math.Max (max_element_size, add);
			if (size % add != 0)
				size += add - size % add;
			return size += add;
		}

		static bool IsBuiltInType (Type type)
		{
			ThrowIfNull (type, nameof (type));
			return IsBuiltInType (type, out var _);
		}

		internal static bool IsBuiltInType (Type type, out int type_size)
		{
			ThrowIfNull (type, nameof (type));
			type_size = 0;

			if (type.IsNested)
				return false;

			if (type.Namespace == "ObjCRuntime") {
				switch (type.Name) {
				case "NativeHandle":
					type_size = 8;
					return true;
				}
				return false;
			} else if (type.Namespace == "System.Runtime.InteropServices") {
				switch (type.Name) {
				case "NFloat":
					type_size = 8;
					return true;
				}
				return false;
			}

			if (type.Namespace != "System")
				return false;

			switch (type.Name) {
			case "Char":
			case "Boolean":
			case "SByte":
			case "Byte":
				type_size = 1;
				return true;
			case "Int16":
			case "UInt16":
				type_size = 2;
				return true;
			case "Single":
			case "Int32":
			case "UInt32":
				type_size = 4;
				return true;
			case "Double":
			case "Int64":
			case "UInt64":
				type_size = 8;
				return true;
			case "IntPtr":
			case "UIntPtr":
			case "nuint":
			case "nint":
				type_size = 8;
				return true;
			case "Void":
				return true;
			}

			return false;
		}

		// IL2070: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicFields', 'DynamicallyAccessedMemberTypes.NonPublicFields' in call to 'System.Type.GetFields(BindingFlags)'. The parameter 'type' of method 'ObjCRuntime.Stret.GetValueTypeSize(Type, Type, List<Type>, Boolean, Int32&, Int32&, Object)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2070", Justification = "Computing the size of a struct is safe, because the trimmer can't remove fields that would affect the size of a marshallable struct (it could affect marshalling behavior).")]
		static void GetValueTypeSize (Type original_type, Type type, List<Type> field_types, ref int size, ref int max_element_size, Generator? generator)
		{
			ThrowIfNull (original_type, nameof (original_type));
			ThrowIfNull (type, nameof (type));
			ThrowIfNull (field_types, nameof (field_types));
#if BGENERATOR
			ThrowIfNull (generator, nameof (generator));
#endif

			// FIXME:
			// SIMD types are not handled correctly here (they need 16-bit alignment).
			// However we don't annotate those types in any way currently, so first we'd need to 
			// add the proper attributes so that the generator can distinguish those types from other types.

			if (IsBuiltInType (type, out var type_size) && type_size > 0) {
				field_types.Add (type);
				size = AlignAndAdd (original_type, size, type_size, ref max_element_size);
				return;
			}

			// composite struct
			foreach (var field in type.GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
#if BGENERATOR
				var marshalAs = generator.AttributeManager.GetCustomAttribute<MarshalAsAttribute> (field);
#else
				var marshalAs = (MarshalAsAttribute?) Attribute.GetCustomAttribute (field, typeof (MarshalAsAttribute));
#endif
				if (marshalAs is null) {
					GetValueTypeSize (original_type, field.FieldType, field_types, ref size, ref max_element_size, generator);
					continue;
				}

				var multiplier = 1;
				switch (marshalAs.Value) {
				case UnmanagedType.ByValArray:
					var types = new List<Type> ();
					var elementType = field.FieldType.GetElementType ();
					if (elementType is null)
						throw new Exception ($"Unable to find the element type for field {GetTypeName (field.DeclaringType)}.{field.Name}");
					GetValueTypeSize (original_type, elementType, types, ref type_size, ref max_element_size, generator);
					multiplier = marshalAs.SizeConst;
					break;
				case UnmanagedType.U1:
				case UnmanagedType.I1:
					type_size = 1;
					break;
				case UnmanagedType.U2:
				case UnmanagedType.I2:
					type_size = 2;
					break;
				case UnmanagedType.U4:
				case UnmanagedType.I4:
				case UnmanagedType.R4:
					type_size = 4;
					break;
				case UnmanagedType.U8:
				case UnmanagedType.I8:
				case UnmanagedType.R8:
					type_size = 8;
					break;
				default:
					throw new Exception ($"Unhandled MarshalAs attribute: {marshalAs.Value} on field {GetTypeName (field.DeclaringType)}.{field.Name}");
				}
				field_types.Add (field.FieldType);
				size = AlignAndAdd (original_type, size, type_size, ref max_element_size);
				size += (multiplier - 1) * size;
			}
		}

		static void ThrowIfNull ([NotNull] object? value, string parameterName)
		{
#if NET
			ArgumentNullException.ThrowIfNull (value, parameterName);
#else
			if (value is null)
				throw new ArgumentNullException (parameterName);
#endif
		}

		static string GetTypeName (Type? type)
		{
			if (type is null)
				return "<unknown>";

			return type.FullName ?? type.Name ?? "<unknown>";
		}

#if BGENERATOR
		/// <summary>Determines whether the specified return type requires stret calling conventions.</summary>
		/// <param name="returnType">The return type to inspect.</param>
		/// <param name="generator">The generator context.</param>
		/// <returns><see langword="true"/> if stret calling conventions are required; otherwise, <see langword="false"/>.</returns>
		public static bool NeedStret (Type returnType, Generator generator)
		{
			return X86_64NeedStret (returnType, generator);
		}
#endif // BGENERATOR
	}
}
