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

using System;
using System.Collections.Generic;
using System.Reflection;
#if !BGENERATOR
using Generator = System.Object;
#endif
using System.Runtime.InteropServices;

using Foundation;

namespace ObjCRuntime {
	class Stret {
		static bool IsHomogeneousAggregateSmallEnough_Armv7k (Type t, int members)
		{
			// https://github.com/llvm-mirror/clang/blob/82f6d5c9ae84c04d6e7b402f72c33638d1fb6bc8/lib/CodeGen/TargetInfo.cpp#L5516-L5519
			return members <= 4;
		}

		static bool IsHomogeneousAggregateBaseType_Armv7k (Type t, Generator generator)
		{
			// https://github.com/llvm-mirror/clang/blob/82f6d5c9ae84c04d6e7b402f72c33638d1fb6bc8/lib/CodeGen/TargetInfo.cpp#L5500-L5514
#if BGENERATOR
			if (t == generator.TypeManager.System_Float || t == generator.TypeManager.System_Double || t == generator.TypeManager.System_nfloat)
				return true;
#else
			if (t == typeof (float) || t == typeof (double) || t == typeof (nfloat))
				return true;
#endif

			return false;
		}

		static bool IsHomogeneousAggregate_Armv7k (List<Type> fieldTypes, Generator generator)
		{
			// Very simplified version of https://github.com/llvm-mirror/clang/blob/82f6d5c9ae84c04d6e7b402f72c33638d1fb6bc8/lib/CodeGen/TargetInfo.cpp#L4051
			// since C# supports a lot less types than clang does.

			if (fieldTypes.Count == 0)
				return false;

			if (!IsHomogeneousAggregateSmallEnough_Armv7k (fieldTypes [0], fieldTypes.Count))
				return false;

			if (!IsHomogeneousAggregateBaseType_Armv7k (fieldTypes [0], generator))
				return false;

			for (int i = 1; i < fieldTypes.Count; i++) {
				if (fieldTypes [0] != fieldTypes [i])
					return false;
			}

			return true;
		}

		public static bool ArmNeedStret (Type returnType, Generator generator)
		{
			bool has32bitArm;
#if BGENERATOR
			has32bitArm = generator.CurrentPlatform != PlatformName.TvOS && generator.CurrentPlatform != PlatformName.MacOSX;
#elif MONOMAC || __TVOS__
			has32bitArm = false;
#else
			has32bitArm = true;
#endif
			if (!has32bitArm)
				return false;

			Type t = returnType;

			if (!t.IsValueType || t.IsEnum || IsBuiltInType (t))
				return false;

			var fieldTypes = new List<Type> ();
			var size = GetValueTypeSize (t, fieldTypes, false, generator);

			bool isWatchOS;
#if BGENERATOR
			isWatchOS = generator.CurrentPlatform == PlatformName.WatchOS;
#elif __WATCHOS__
			isWatchOS = true;
#else
			isWatchOS = false;
#endif

			if (isWatchOS) {
				// According to clang watchOS passes arguments bigger than 16 bytes by reference.
				// https://github.com/llvm-mirror/clang/blob/82f6d5c9ae84c04d6e7b402f72c33638d1fb6bc8/lib/CodeGen/TargetInfo.cpp#L5248-L5250
				// https://github.com/llvm-mirror/clang/blob/82f6d5c9ae84c04d6e7b402f72c33638d1fb6bc8/lib/CodeGen/TargetInfo.cpp#L5542-L5543
				if (size <= 16)
					return false;

				// Except homogeneous aggregates, which are not stret either.
				if (IsHomogeneousAggregate_Armv7k (fieldTypes, generator))
					return false;
			}

			bool isiOS;
#if BGENERATOR
			isiOS = generator.CurrentPlatform == PlatformName.iOS;
#elif __IOS__
			isiOS = true;
#else
			isiOS = false;
#endif

			if (isiOS) {
				if (size <= 4 && fieldTypes.Count == 1) {
					switch (fieldTypes [0].FullName) {
					case "System.Char":
					case "System.Byte":
					case "System.SByte":
					case "System.UInt16":
					case "System.Int16":
					case "System.UInt32":
					case "System.Int32":
					case "System.IntPtr":
					case "System.UIntPtr":
					case "System.nuint":
					case "System.nint":
						return false;
						// floating-point types are stret
					}
				}
			}

			return true;
		}

		public static bool X86NeedStret (Type returnType, Generator generator)
		{
			Type t = returnType;

			if (!t.IsValueType || t.IsEnum || IsBuiltInType (t))
				return false;

			var fieldTypes = new List<Type> ();
			var size = GetValueTypeSize (t, fieldTypes, false, generator);

			if (size > 8)
				return true;

			if (fieldTypes.Count == 3)
				return true;

			return false;
		}

		public static bool X86_64NeedStret (Type returnType, Generator generator)
		{
			Type t = returnType;

			if (!t.IsValueType || t.IsEnum || IsBuiltInType (t))
				return false;

			var fieldTypes = new List<Type> ();
			return GetValueTypeSize (t, fieldTypes, true, generator) > 16;
		}

		static int GetValueTypeSize (Type type, List<Type> fieldTypes, bool is_64_bits, Generator generator)
		{
			int size = 0;
			int maxElementSize = 1;

			if (type.IsExplicitLayout) {
				// Find the maximum of "field size + field offset" for each field.
				foreach (var field in type.GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
#if BGENERATOR
					var fieldOffset = generator.AttributeManager.GetCustomAttribute<FieldOffsetAttribute> (field);
#else
					var fieldOffset = (FieldOffsetAttribute) Attribute.GetCustomAttribute (field, typeof (FieldOffsetAttribute));
#endif
					var elementSize = 0;
					GetValueTypeSize (type, field.FieldType, fieldTypes, is_64_bits, ref elementSize, ref maxElementSize, generator);
					size = Math.Max (size, elementSize + fieldOffset.Value);
				}
			} else {
				GetValueTypeSize (type, type, fieldTypes, is_64_bits, ref size, ref maxElementSize, generator);
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
			return IsBuiltInType (type, true /* doesn't matter */, out var _);
		}

		static bool IsBuiltInType (Type type, bool is_64_bits, out int type_size)
		{
			type_size = 0;

			if (type.IsNested)
				return false;

#if NET
			if (type.Namespace == "ObjCRuntime") {
				switch (type.Name) {
				case "NativeHandle":
					type_size = is_64_bits ? 8 : 4;
					return true;
				}
				return false;
			} else if (type.Namespace == "System.Runtime.InteropServices") {
				switch (type.Name) {
				case "NFloat":
					type_size = is_64_bits ? 8 : 4;
					return true;
				}
				return false;
			}
#endif

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
#if !NET
			case "nfloat":
#endif
			case "nuint":
			case "nint":
				type_size = is_64_bits ? 8 : 4;
				return true;
			case "Void":
				return true;
			}

			return false;
		}

		static void GetValueTypeSize (Type original_type, Type type, List<Type> field_types, bool is_64_bits, ref int size, ref int max_element_size, Generator generator)
		{
			// FIXME:
			// SIMD types are not handled correctly here (they need 16-bit alignment).
			// However we don't annotate those types in any way currently, so first we'd need to 
			// add the proper attributes so that the generator can distinguish those types from other types.

			if (IsBuiltInType (type, is_64_bits, out var type_size) && type_size > 0) {
				field_types.Add (type);
				size = AlignAndAdd (original_type, size, type_size, ref max_element_size);
				return;
			}

			// composite struct
			foreach (var field in type.GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
#if BGENERATOR
				var marshalAs = generator.AttributeManager.GetCustomAttribute<MarshalAsAttribute> (field);
#else
				var marshalAs = (MarshalAsAttribute) Attribute.GetCustomAttribute (field, typeof (MarshalAsAttribute));
#endif
				if (marshalAs is null) {
					GetValueTypeSize (original_type, field.FieldType, field_types, is_64_bits, ref size, ref max_element_size, generator);
					continue;
				}

				var multiplier = 1;
				switch (marshalAs.Value) {
				case UnmanagedType.ByValArray:
					var types = new List<Type> ();
					GetValueTypeSize (original_type, field.FieldType.GetElementType (), types, is_64_bits, ref type_size, ref max_element_size, generator);
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
					throw new Exception ($"Unhandled MarshalAs attribute: {marshalAs.Value} on field {field.DeclaringType.FullName}.{field.Name}");
				}
				field_types.Add (field.FieldType);
				size = AlignAndAdd (original_type, size, type_size, ref max_element_size);
				size += (multiplier - 1) * size;
			}
		}

		public static bool NeedStret (Type returnType, Generator generator)
		{
			if (X86NeedStret (returnType, generator))
				return true;

			if (X86_64NeedStret (returnType, generator))
				return true;

			if (ArmNeedStret (returnType, generator))
				return true;

			return false;
		}
	}
}
