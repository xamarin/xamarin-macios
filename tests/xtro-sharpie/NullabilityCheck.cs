//
// The rule reports
//
// !extra-null-allowed!
//		when a method parameters or return value has an [NullAllowed] attribute that is not part of the ObjC headers
//
// !missing-null-allowed!
//		when a method parameters or return value does not have an [NullAllowed] when one is present in the ObjC headers
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class NullabilityCheck : BaseVisitor {

		// 0 for oblivious, 1 for not annotated, and 2 for annotated
		enum Null : byte {
			Oblivious = 0,
			NotAnnotated = 1,
			Annotated = 2,
		}

		static Dictionary<string, TypeDefinition> types = new Dictionary<string, TypeDefinition> ();
		static Dictionary<string, MethodDefinition> methods = new Dictionary<string, MethodDefinition> ();

		static TypeDefinition GetType (ObjCInterfaceDecl decl)
		{
			types.TryGetValue (decl.Name, out var td);
			return td;
		}

		static MethodDefinition GetMethod (ObjCMethodDecl decl)
		{
			methods.TryGetValue (decl.GetName (), out var md);
			return md;
		}

		public override void VisitManagedMethod (MethodDefinition method)
		{
			var key = method.GetName ();
			if (key is null)
				return;

			// we still have one case to fix with duplicate selectors :|
			if (!methods.ContainsKey (key))
				methods.Add (key, method);
		}

		// NullableContextAttribute is valid in metadata on type and method declarations.
		// https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md
		static Null GetNullableContext (ICustomAttributeProvider cap)
		{
			if (cap.HasCustomAttributes) {
				foreach (var ca in cap.CustomAttributes) {
					if (ca.Constructor.DeclaringType.FullName != "System.Runtime.CompilerServices.NullableContextAttribute")
						continue;
					return (Null) (byte) ca.ConstructorArguments [0].Value;
				}
			}
			return Null.Oblivious;
		}

		static Dictionary<TypeDefinition, Null> null_type_cache = new Dictionary<TypeDefinition, Null> ();

		// most method checks the type so it adds up fast
		static Null GetNullableContext (TypeDefinition type)
		{
			if (!null_type_cache.TryGetValue (type, out var result)) {
				result = GetNullableContext ((ICustomAttributeProvider) type);
				null_type_cache.Add (type, result);
			}
			return result;
		}

		static Null [] GetNullable (ICustomAttributeProvider cap)
		{
			if (cap.HasCustomAttributes) {
				foreach (var ca in cap.CustomAttributes) {
					if (ca.Constructor.DeclaringType.FullName != "System.Runtime.CompilerServices.NullableAttribute")
						continue;
					var first = ca.ConstructorArguments [0];
					// encoding is... weird
					switch (first.Type.FullName) {
					// Type is `System.Byte` and value is a `byte`
					case "System.Byte":
						return new Null [1] { (Null) (byte) first.Value };
					// Type is `System.Byte[]` and value is a `CustomAttributeArgument[]`
					// each with a `Type` of `System.Byte` and where value is a `byte`
					case "System.Byte[]":
						var caa = first.Value as CustomAttributeArgument [];
						var length = caa.Length;
						var values = new Null [length];
						for (int i = 0; i < length; i++)
							values [i] = (Null) (byte) caa [i].Value;
						return values;
					}
				}
			}
			return Array.Empty<Null> ();
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
				return;

			// don't process deprecated methods (or types)
			if (decl.IsDeprecated () || (decl.DeclContext as Decl).IsDeprecated ())
				return;

			var method = GetMethod (decl);
			// don't report missing nullability on types that are not bound - that's a different problem
			if (method is null)
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework is null)
				return;

			var t = method.DeclaringType;
			// look for [NullableContext] for defaults
			var managed_default_nullability = GetNullableContext (method);
			if (managed_default_nullability == Null.Oblivious)
				managed_default_nullability = GetNullableContext (t);

			// check parameters
			// categories have an offset of 1 for the extension method type (spotted as static types)
			int i = t.IsSealed && t.IsAbstract ? 1 : 0;
			foreach (var p in decl.Parameters) {
				var mp = method.Parameters [i++];
				// a managed `out` value does not need to be inialized, won't be null (but can be ignored)
				if (mp.IsOut)
					continue;

				var pt = mp.ParameterType;
				// if bound as `IntPtr` then nullability attributes won't be present
				if (pt.IsValueType)
					continue;

				Null parameter_nullable;

				// if we used a type by reference (e.g. `ref float foo`); or a nullable type (e.g. `[BindAs]`)
				// then assume it's meant as a nullable type) without additional decorations
				if (pt.IsByReference || pt.FullName.StartsWith ("System.Nullable`1<", StringComparison.Ordinal)) {
					parameter_nullable = Null.Annotated;
				} else {
					// check C# 8 compiler attributes
					var nullable = GetNullable (mp);
					if (nullable.Length > 1) {
						// check the type itself, TODO check the generics (don't think we have such cases yet)
						parameter_nullable = nullable [0];
					} else if (nullable.Length == 0) {
						parameter_nullable = managed_default_nullability;
					} else {
						parameter_nullable = nullable [0];
					}
				}

				// match with native and, if needed, report discrepancies
				p.QualType.Type.GetNullability (p.AstContext, out var nullability);
				switch (nullability) {
				case NullabilityKind.NonNull:
					if (parameter_nullable == Null.Annotated)
						Log.On (framework).Add ($"!extra-null-allowed! '{method.FullName}' has a extraneous [NullAllowed] on parameter #{i - 1}");
					break;
				case NullabilityKind.Nullable:
					if (parameter_nullable != Null.Annotated)
						Log.On (framework).Add ($"!missing-null-allowed! '{method.FullName}' is missing an [NullAllowed] on parameter #{i - 1}");
					break;
				case NullabilityKind.Unspecified:
					break;
				}
			}

			// with .net a constructor will always return something (or throw)
			// that's not the case in ObjC where `init*` can return `nil`
			if (method.IsConstructor)
				return;

			var mrt = method.ReturnType;
			// if bound as an `IntPtr` then the nullability will not be visible in the metadata
			if (mrt.IsValueType)
				return;

			Null return_nullable;
			// if we used a nullable type (e.g. [BindAs] then assume it's meant as a nullable type) without additional decorations
			if (mrt.FullName.StartsWith ("System.Nullable`1<", StringComparison.Ordinal)) {
				return_nullable = Null.Annotated;
			} else {
				ICustomAttributeProvider cap;
				// the managed attributes are on the property, not the special methods
				if (method.IsGetter) {
					var property = method.FindProperty ();
					// also `null_resettable` will only show something (natively) on the setter (since it does not return null, but accept it)
					// in this case we'll trust xtro checking the setter only (if it exists, if not then it can't be `null_resettable`)
					if (property.SetMethod is not null)
						return;
					cap = property;
				} else {
					cap = method.MethodReturnType;
				}
				Null [] mrt_nullable = GetNullable (cap);

				if (mrt_nullable.Length > 1) {
					// check the type itself, TODO check the generics (don't think we have such cases yet)
					return_nullable = mrt_nullable [0];
				} else if (mrt_nullable.Length == 0) {
					return_nullable = managed_default_nullability;
				} else {
					return_nullable = mrt_nullable [0];
				}
			}

			var rt = decl.ReturnQualType;
			rt.Type.GetNullability (decl.AstContext, out var rnull);
			switch (rnull) {
			case NullabilityKind.NonNull:
				if (return_nullable == Null.Annotated)
					Log.On (framework).Add ($"!extra-null-allowed! '{method}' has a extraneous [NullAllowed] on return type");
				break;
			case NullabilityKind.Nullable:
				if (return_nullable != Null.Annotated)
					Log.On (framework).Add ($"!missing-null-allowed! '{method}' is missing an [NullAllowed] on return type");
				break;
			case NullabilityKind.Unspecified:
				break;
			}
		}
	}
}
