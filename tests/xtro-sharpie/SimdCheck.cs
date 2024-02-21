using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	class SimdCheck : BaseVisitor {
		bool very_strict = false;
		bool strict = false;

		// A dictionary of native type -> managed type mapping.
		class NativeSimdInfo {
			public string Managed;
			public string InvalidManaged;
		}

		static Dictionary<string, NativeSimdInfo> type_mapping = new Dictionary<string, NativeSimdInfo> () {
			{ "matrix_double2x2", new NativeSimdInfo { Managed ="MatrixDouble2x2", InvalidManaged = "Matrix2d" }},
			{ "matrix_double3x3", new NativeSimdInfo { Managed = "MatrixDouble3x3", InvalidManaged = "Matrix3d" }},
			{ "matrix_double4x4", new NativeSimdInfo { Managed = "MatrixDouble4x4", InvalidManaged = "Matrix4d" }},
			{ "matrix_float2x2", new NativeSimdInfo { Managed = "MatrixFloat2x2", InvalidManaged = "Matrix2", }},
			{ "matrix_float3x3", new NativeSimdInfo { Managed = "MatrixFloat3x3", InvalidManaged = "Matrix3", }},
			{ "matrix_float4x3", new NativeSimdInfo { Managed = "MatrixFloat4x3", }},
			{ "matrix_float4x4", new NativeSimdInfo { Managed = "MatrixFloat4x4", InvalidManaged = "Matrix4", }},
			{ "simd_quatd", new NativeSimdInfo { Managed = "Quaternion4d", }},
			{ "simd_quatf", new NativeSimdInfo { Managed = "Quaternion4", }},
			{ "vector_double2", new NativeSimdInfo { Managed = "Vector2d", }},
			{ "vector_double3", new NativeSimdInfo { Managed = "Vector3d", }},
			{ "vector_double4", new NativeSimdInfo { Managed = "Vector4d", }},
			{ "vector_float2", new NativeSimdInfo { Managed = "Vector2", }},
			{ "vector_float3", new NativeSimdInfo { Managed = "Vector3", }},
			{ "vector_float4", new NativeSimdInfo { Managed = "Vector4", }},
			{ "vector_int2", new NativeSimdInfo { Managed = "Vector2i", }},
			{ "vector_int3", new NativeSimdInfo { Managed = "Vector3i", }},
			{ "vector_int4", new NativeSimdInfo { Managed = "Vector4i", }},
			{ "vector_uint2", new NativeSimdInfo { Managed = "Vector2i", }},
			{ "vector_uint3", new NativeSimdInfo { Managed = "Vector3i", }},
			{ "vector_uint4", new NativeSimdInfo { Managed = "Vector4i", }},
			// simd_doubleX is typedefed to vector_doubleX
			{ "simd_double2", new NativeSimdInfo { Managed = "Vector2d" }},
			// simd_floatX is typedefed to vector_floatX
			{ "simd_float2", new NativeSimdInfo { Managed = "Vector2" }},
			{ "simd_float3", new NativeSimdInfo { Managed = "Vector3" }},
			{ "simd_float4", new NativeSimdInfo { Managed = "Vector4" }},
			{ "simd_float3x3", new NativeSimdInfo { Managed = "MatrixFloat3x3", InvalidManaged = "Matrix3" }},
			{ "simd_float4x4", new NativeSimdInfo { Managed = "MatrixFloat4x4", InvalidManaged = "Matrix4" }},
			// The native definition is two 'vector_float2' fields.
			// The managed definition matches this (two 'Vector2' fields), and should work fine.
			{ "GKQuad", new NativeSimdInfo { Managed = "GKQuad" }},
			// The native definition is two 'vector_float3' fields.
			// In this case each element uses 16 bytes (4 floats) due to padding.
			// The managed definition is two Vector3 fields, and does *not*
			// match the native definition (missing the padding).
			// It still works because we're marshalling this struct manually ([MarshalDirective]).
			{ "GKBox", new NativeSimdInfo { Managed = "GKBox", }},
			// The native definition is 'vector_float3 points[3]' - an array of three vector_float3.
			// In this case each element uses 16 bytes (4 floats) due to padding.
			// The managed definition is just an array of Vector3, but luckily
			// it's a private field, so we might be able to improve this later. Right now we're marshalling
			// this struct manually ([MarshalDirective]), so managed code should get correct
			// results.
			{ "GKTriangle", new NativeSimdInfo { Managed = "GKTriangle", }},
			// This is a 'vector_int4, represented by a Vector4i in managed code,
			// which means it's matching the native definition.
			{ "MDLVoxelIndex", new NativeSimdInfo { Managed = "MDLVoxelIndex" }},
			// In managed code this is struct of two Vector4, so it's matching the native definition.
			{ "MDLVoxelIndexExtent", new NativeSimdInfo { Managed = "MDLVoxelIndexExtent" }},
			// In managed code this is a struct of two Vector3, so it's *not* matching
			// the native definition. However, since we're manually marshalling this type
			// (using [MarshalDirective]), managed code doesn't get incorrect results.
			{ "MDLAxisAlignedBoundingBox", new NativeSimdInfo { Managed = "MDLAxisAlignedBoundingBox", }},
			{ "MPSAxisAlignedBoundingBox", new NativeSimdInfo { Managed = "MPSAxisAlignedBoundingBox" }},
			// The managed definition is identical to the native definition
			{ "MPSImageHistogramInfo", new NativeSimdInfo { Managed = "MPSImageHistogramInfo" }},
		};

		static Dictionary<string, bool> managed_simd_types; // bool: invalid_for_simd

		static SimdCheck ()
		{
			managed_simd_types = new Dictionary<string, bool> ();
			foreach (var kvp in type_mapping) {
				managed_simd_types [kvp.Value.Managed] = false;
				if (!string.IsNullOrEmpty (kvp.Value.InvalidManaged))
					managed_simd_types [kvp.Value.InvalidManaged] = true;
			}
		}

		class ManagedSimdInfo {
			public MethodDefinition Method;
			public bool ContainsInvalidMappingForSimd;
		}
		Dictionary<string, ManagedSimdInfo> managed_methods = new Dictionary<string, ManagedSimdInfo> ();

		public override void VisitManagedMethod (MethodDefinition method)
		{
			var type = method.DeclaringType;

			if (!type.IsNested && type.IsNotPublic)
				return;

			if (type.IsNested && (type.IsNestedPrivate || type.IsNestedAssembly || type.IsNestedFamilyAndAssembly))
				return;

			if (method.IsPrivate || method.IsAssembly || method.IsFamilyAndAssembly)
				return; // Don't care about non-visible types

			if (type.Namespace == "Simd" || type.Namespace.StartsWith ("OpenTK", StringComparison.Ordinal))
				return; // We're assuming everything in the Simd and OpenTK namespaces can be ignored (the former because it's correctly written, the latter because it doesn't map to native simd types).

			if (method.HasCustomAttributes && method.CustomAttributes.Where ((v) => v.Constructor.DeclaringType.Name == "ExtensionAttribute").Any ())
				return; // Extension methods can't be mapped.

			var invalid_simd_type = false;
			var contains_simd_types = ContainsSimdTypes (method, ref invalid_simd_type);

			var key = method.GetName ();
			if (key is null) {
				if (method.IsObsolete ())
					return; // Don't care about obsolete API.

				if (contains_simd_types && very_strict) {
					// We can't map this method to a native function.
					var framework = method.DeclaringType.Namespace;
					Log.On (framework).Add ($"!missing-simd-native-signature! {method}");
				}
				return;
			}

			ManagedSimdInfo existing;
			if (managed_methods.TryGetValue (key, out existing)) {
				if (very_strict) {
					var sorted = Helpers.Sort (existing.Method, method);
					var framework = sorted.Item1.DeclaringType.Namespace;
					Log.On (framework).Add ($"!duplicate-type-mapping! same key '{key}' for both '{sorted.Item1.FullName}' and '{sorted.Item2.FullName}'");
				}
			} else {
				managed_methods [key] = new ManagedSimdInfo {
					Method = method,
					ContainsInvalidMappingForSimd = invalid_simd_type
				};
			}
		}

		bool ContainsSimdTypes (MethodDefinition method, ref bool invalid_for_simd)
		{
			if (IsSimdType (method.ReturnType, ref invalid_for_simd))
				return true;

			if (method.HasParameters) {
				foreach (var param in method.Parameters)
					if (IsSimdType (param.ParameterType, ref invalid_for_simd))
						return true;
			}

			return false;
		}

		bool IsSimdType (TypeReference td, ref bool invalid_for_simd)
		{
			return managed_simd_types.TryGetValue (td.Name, out invalid_for_simd);
		}

		bool ContainsSimdTypes (ObjCMethodDecl decl, ref string simd_type, ref bool requires_marshal_directive)
		{
			if (IsSimdType (decl, decl.ReturnQualType, ref simd_type, ref requires_marshal_directive))
				return true;

			var is_simd_type = false;
			foreach (var param in decl.Parameters)
				is_simd_type |= IsSimdType (decl, param.QualType, ref simd_type, ref requires_marshal_directive);

			return is_simd_type;
		}

		bool IsExtVector (Decl decl, QualType type, ref string simd_type)
		{
			var rv = false;
			var t = type.CanonicalQualType.Type;

			// Unpoint the type
			var pointerType = t as Clang.Ast.PointerType;
			if (pointerType is not null)
				t = pointerType.PointeeQualType.Type;

			if (t.Kind == TypeKind.ExtVector) {
				rv = true;
			} else {
				var r = (t as RecordType)?.Decl;
				if (r is not null) {
					foreach (var f in r.Fields) {
						var qt = f.QualType.CanonicalQualType.Type;
						if (qt.Kind == TypeKind.ExtVector) {
							rv = true;
							break;
						}
						var at = qt as ConstantArrayType;
						if (at is not null) {
							if (at.ElementType.Type.Kind == TypeKind.ExtVector) {
								rv = true;
								break;
							}
						}
					}
				}
			}

			var typeName = type.ToString ();

			if (!rv && typeName.Contains ("simd")) {
				var framework = Helpers.GetFramework (decl);
				Log.On (framework).Add ($"!unknown-simd-type! Could not detect that {typeName} is a Simd type, but its name contains 'simd'. Something needs fixing in SimdCheck.cs");
			}

			if (rv)
				simd_type = typeName;

			return rv;
		}

		bool IsSimdType (Decl decl, QualType type, ref string simd_type, ref bool requires_marshal_directive)
		{
			var str = Undecorate (type.ToString ());

			if (type_mapping.TryGetValue (str, out var info)) {
				requires_marshal_directive = true;
				simd_type = str;
				return true;
			}

			if (IsExtVector (decl, type, ref simd_type)) {
				var framework = Helpers.GetFramework (decl);
				Log.On (framework).Add ($"!unknown-simd-type-mapping! The Simd type {simd_type} does not have a mapping to a managed type. Please add one in SimdCheck.cs");
			}

			return false;
		}

		string Undecorate (string native_name)
		{
			const string _const = "const ";
			if (native_name.StartsWith (_const, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (_const.Length));

			const string _struct = "struct ";
			if (native_name.StartsWith (_struct, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (_struct.Length));

			const string _nsrefinedforswift = "NS_REFINED_FOR_SWIFT ";
			if (native_name.StartsWith (_nsrefinedforswift, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (_nsrefinedforswift.Length));

			const string _nsreturnsinnerpointer = "NS_RETURNS_INNER_POINTER ";
			if (native_name.StartsWith (_nsreturnsinnerpointer, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (_nsreturnsinnerpointer.Length));

			const string _Nonnull = " _Nonnull";
			if (native_name.EndsWith (_Nonnull, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (0, native_name.Length - _Nonnull.Length));

			const string _Nullable = " _Nullable";
			if (native_name.EndsWith (_Nullable, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (0, native_name.Length - _Nullable.Length));

			const string _star = " *";
			if (native_name.EndsWith (_star, StringComparison.Ordinal))
				return Undecorate (native_name.Substring (0, native_name.Length - _star.Length));

			return native_name;
		}

		bool HasMarshalDirective (ICustomAttributeProvider provider)
		{
			if (provider?.HasCustomAttributes != true)
				return false;

			foreach (var ca in provider.CustomAttributes)
				if (ca.Constructor.DeclaringType.Name == "MarshalDirective")
					return true;

			return false;
		}

		void CheckMarshalDirective (MethodDefinition method, string simd_type)
		{
			if (!method.HasBody)
				return;

			if (method.IsObsolete ())
				return;

			// The [MarshalDirective] attribute isn't copied to the generated code,
			// so instead apply some heuristics and detect calls to the xamarin_simd__ P/Invoke,
			// and if there are any, then assume the method binding has a [MarshalDirective].
			var body = method.Body;
			var anyCalls = false;
			foreach (var i in body.Instructions) {
				switch (i.OpCode.Code) {
				case Mono.Cecil.Cil.Code.Call:
				case Mono.Cecil.Cil.Code.Calli:
				case Mono.Cecil.Cil.Code.Callvirt:
					var mr = i.Operand as MethodReference;
					if (mr is not null) {
						if (mr.Name.StartsWith ("xamarin_simd__", StringComparison.Ordinal))
							return;
						if (mr.Name.StartsWith ("xamarin_vector_float3__", StringComparison.Ordinal))
							return;
					}
					anyCalls = true;
					break;
				default:
					break;
				}
			}

			// If the method doesn't call anywhere, it can't be broken.
			// For instance if the method just throws an exception.
			if (!anyCalls)
				return;

			var framework = method.DeclaringType.Namespace;
			Log.On (framework).Add ($"!wrong-simd-missing-marshaldirective! {method}: simd type: {simd_type}");
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework is null)
				return;

			var simd_type = string.Empty;
			var requires_marshal_directive = false;
			var native_simd = ContainsSimdTypes (decl, ref simd_type, ref requires_marshal_directive);

			ManagedSimdInfo info;
			managed_methods.TryGetValue (decl.GetName (), out info);
			var method = info?.Method;

			if (!native_simd) {
				if (method is not null) {
					// The managed method uses types that were incorrectly used in place of the correct Simd types,
					// but the native method doesn't use the native Simd types. This means the binding is correct.
				} else {
					// Neither the managed nor the native method have anything to do with Simd types.
				}
				return;
			}

			if (method is null) {
				// Could not map the native method to a managed method.
				// This needs investigation, to see why the native method couldn't be mapped.

				// Check if this is new API, in which case it probably couldn't be mapped because we haven't bound it.
				var is_new = false;
				var attrs = decl.Attrs.ToList ();
				var parentClass = decl.DeclContext as Decl;
				if (parentClass is not null)
					attrs.AddRange (parentClass.Attrs);

				foreach (var attr in attrs) {
					var av_attr = attr as AvailabilityAttr;
					if (av_attr is null)
						continue;
					if (av_attr.Platform.Name != "ios")
						continue;
					if (av_attr.Introduced.Major >= 11) {
						is_new = true;
						break;
					}
				}
				if (is_new && !very_strict)
					return;
				if (!strict)
					return;
				Log.On (framework).Add ($"!missing-simd-managed-method! {decl}: could not find a managed method for the native method {decl.GetName ()} (selector: {decl.Selector}). Found the simd type '{simd_type}' in the native signature.");
				return;
			}

			if (!info.ContainsInvalidMappingForSimd) {
				// The managed method does not have any types that are incorrect for Simd.
				if (requires_marshal_directive)
					CheckMarshalDirective (method, simd_type);
				return;
			}

			if (method.IsObsolete ()) {
				// We have a potentially broken managed method, but it's obsolete. That's fine.
				return;
			}

			if (requires_marshal_directive)
				CheckMarshalDirective (method, simd_type);

			// We have a potentially broken managed method. This needs fixing/investigation.
			Log.On (framework).Add ($"!unknown-simd-type-in-signature! {method}: the native signature has a simd type ({simd_type}), while the corresponding managed method is using an incorrect (non-simd) type.");
		}
	}
}
