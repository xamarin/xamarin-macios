//
// The rule reports
//
// !extra-null-allowed!
//		when a method parameters or return value has an [NullAllowed] attribute that is not part of the ObjC headers
//
// !missing-null-allowed!
//		when a method parameters or return value does not have an [NullAllowed] when one is present in the ObjC headers
//

namespace Extrospection {

	public class NullabilityCheck : BaseVisitor {

		// 0 for oblivious, 1 for not annotated, and 2 for annotated
		internal enum Null : byte {
			Oblivious = 0,
			NotAnnotated = 1,
			Annotated = 2,
		}

		readonly Dictionary<string, MethodDefinition> methods = new Dictionary<string, MethodDefinition> ();

		public NullabilityCheck (BindingResult bindingResult)
			: base (bindingResult)
		{
		}

		MethodDefinition? GetMethod (ObjCMethodDecl decl)
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
		static Null? GetDeclaredNullableContext (ICustomAttributeProvider cap)
		{
			if (cap.HasCustomAttributes) {
				foreach (var ca in cap.CustomAttributes) {
					if (ca.Constructor.DeclaringType.FullName != "System.Runtime.CompilerServices.NullableContextAttribute")
						continue;
					return (Null) (byte) ca.ConstructorArguments [0].Value;
				}
			}
			return null;
		}

		readonly Dictionary<TypeDefinition, Null> null_type_cache = new Dictionary<TypeDefinition, Null> ();

		// most method checks the type so it adds up fast
		Null GetNullableContext (TypeDefinition type)
		{
			if (!null_type_cache.TryGetValue (type, out var result)) {
				result = GetDeclaredNullableContext (type) ??
					(type.DeclaringType is null ? Null.Oblivious : GetNullableContext (type.DeclaringType));
				null_type_cache.Add (type, result);
			}
			return result;
		}

		internal Null GetNullableContext (MethodDefinition method)
		{
			return GetDeclaredNullableContext (method) ?? GetNullableContext (method.DeclaringType);
		}

		internal static Null [] GetNullable (ICustomAttributeProvider cap)
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
						var caa = (CustomAttributeArgument []) first.Value;
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

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl)
		{
			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(((Decl) decl.DeclContext!).IsAvailable ()))
				return;

			// don't process deprecated methods (or types)
			if (decl.IsDeprecated () || (((Decl) decl.DeclContext!).IsDeprecated ()))
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

				// if we used a type by reference (e.g. `ref float foo`), nullability won't be present
				if (pt.IsByReference)
					continue;

				// if we used a pointer to a type, nullability won't be present
				if (pt.IsPointer)
					continue;

				Null parameter_nullable;

				// if we used a nullable type (e.g. `[BindAs]`)
				// then assume it's meant as a nullable type) without additional decorations
				if (pt.FullName.StartsWith ("System.Nullable`1<", StringComparison.Ordinal)) {
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
				var nullability = p.Type.Handle.Nullability;
				switch (nullability) {
				case CXTypeNullabilityKind.CXTypeNullability_NonNull:
					if (parameter_nullable == Null.Annotated)
						Log.On (framework).Add ($"!extra-null-allowed! '{method.FullName}' has a extraneous [NullAllowed] on parameter #{i - 1}");
					break;
				case CXTypeNullabilityKind.CXTypeNullability_Nullable:
					if (parameter_nullable != Null.Annotated)
						Log.On (framework).Add ($"!missing-null-allowed! '{method.FullName}' is missing an [NullAllowed] on parameter #{i - 1}");
					break;
				case CXTypeNullabilityKind.CXTypeNullability_Unspecified:
					break;
				}

				// Check nullability of block/delegate parameter's inner parameters
				CheckBlockNullability (p.Type, pt, mp, method, framework, managed_default_nullability, $"parameter '{mp.Name}'");
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
					var property = method.FindProperty ()!;
					// also `null_resettable` will only show something (natively) on the setter (since it does not return null, but accept it)
					// in this case we'll trust xtro checking the setter only (if it exists, if not then it can't be `null_resettable`)
					if (property.SetMethod is not null)
						return;
					cap = property;
				} else {
					cap = method.MethodReturnType;
				}
				CheckBlockNullability (decl.ReturnType, mrt, cap, method, framework, managed_default_nullability, "return type");
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

			var rt = decl.ReturnType;
			var rnull = rt.Handle.Nullability;
			switch (rnull) {
			case CXTypeNullabilityKind.CXTypeNullability_NonNull:
				if (return_nullable == Null.Annotated)
					Log.On (framework).Add ($"!extra-null-allowed! '{method}' has a extraneous [NullAllowed] on return type");
				break;
			case CXTypeNullabilityKind.CXTypeNullability_Nullable:
				if (return_nullable != Null.Annotated)
					Log.On (framework).Add ($"!missing-null-allowed! '{method}' is missing an [NullAllowed] on return type");
				break;
			case CXTypeNullabilityKind.CXTypeNullability_Unspecified:
				break;
			}
		}

		void CheckBlockNullability (ClangSharp.Type nativeType, TypeReference managedType, ICustomAttributeProvider provider,
			MethodDefinition method, string framework, Null managedDefaultNullability, string location)
		{
			var funcType = GetBlockFunctionProtoType (nativeType);
			if (funcType is null)
				return;

			var definition = managedType.Resolve () ?? throw new InvalidOperationException ($"Unable to resolve callback type '{managedType}' in '{method.FullName}'.");
			if (definition.BaseType?.FullName != "System.MulticastDelegate")
				return;

			var invoke = definition.Methods.Single (v => v.Name == "Invoke");
			var parameterOffset = GetManagedParameterOffset (managedType, invoke, method, funcType.ParamTypes.Count);
			if (invoke.Parameters.Count - parameterOffset != funcType.ParamTypes.Count && !HasOmittedCompletionHandler (funcType, invoke))
				return;

			var nullable = GetNullable (provider);
			var context = GetNullableContext (invoke);
			for (var i = parameterOffset; i < invoke.Parameters.Count; i++) {
				var parameter = invoke.Parameters [i];
				var nativeIndex = i - parameterOffset;
				CheckBlockTypeNullability (funcType.ParamTypes [nativeIndex], parameter.ParameterType, parameter, context,
					managedType as GenericInstanceType, nullable, managedDefaultNullability, method, framework, $"{location} block parameter #{nativeIndex}");
			}

			CheckBlockTypeNullability (funcType.ReturnType, invoke.ReturnType, invoke.MethodReturnType, context,
				managedType as GenericInstanceType, nullable, managedDefaultNullability, method, framework, $"{location} block return type");
		}

		static bool HasOmittedCompletionHandler (FunctionProtoType nativeType, MethodDefinition managedInvoke)
		{
			if (nativeType.ParamTypes.Count != managedInvoke.Parameters.Count + 1)
				return false;

			return GetBlockFunctionProtoType (nativeType.ParamTypes [^1]) is not null;
		}

		static int GetManagedParameterOffset (TypeReference managedType, MethodDefinition managedInvoke, MethodDefinition method, int nativeParameterCount)
		{
			if (managedInvoke.Parameters.Count != nativeParameterCount + 1)
				return 0;

			var firstParameterType = managedInvoke.Parameters [0].ParameterType;
			if (firstParameterType is GenericParameter parameter && managedType is GenericInstanceType generic)
				firstParameterType = generic.GenericArguments [parameter.Position];

			return firstParameterType.FullName == method.DeclaringType.FullName ? 1 : 0;
		}

		void CheckBlockTypeNullability (ClangSharp.Type nativeType, TypeReference managedType, ICustomAttributeProvider provider,
			Null context, GenericInstanceType? delegateType, Null [] typeNullability, Null typeContext,
			MethodDefinition method, string framework, string location)
		{
			managedType = StripModifiers (managedType);
			if (managedType is ByReferenceType byReference) {
				managedType = StripModifiers (byReference.ElementType);
				// The annotation describes the referent, not the address passed to the callback.
				if (nativeType.UnqualifiedDesugaredType is not ClangSharp.PointerType pointer)
					throw new InvalidOperationException ($"Expected a native pointer for by-reference {location} in '{method.FullName}'.");
				nativeType = pointer.PointeeType;
			}

			var resolved = GetCallbackTypeNullability (managedType, GetNullable (provider), context, delegateType, typeNullability, typeContext);
			managedType = resolved.Type;
			var managedNullability = resolved.Nullability;

			if (!IsNullableValueType (managedType) && (managedType.IsValueType || managedType.IsPointer || managedType.IsFunctionPointer ||
				managedType is GenericParameter { HasNotNullableValueTypeConstraint: true }))
				return;

			switch (nativeType.Handle.Nullability) {
			case CXTypeNullabilityKind.CXTypeNullability_NonNull:
				if (managedNullability == Null.Annotated)
					Log.On (framework).Add ($"!extra-null-allowed! '{method.FullName}' has an extraneous '?' on {location}");
				break;
			case CXTypeNullabilityKind.CXTypeNullability_Nullable:
			case CXTypeNullabilityKind.CXTypeNullability_NullableResult:
				if (managedNullability != Null.Annotated)
					Log.On (framework).Add ($"!missing-null-allowed! '{method.FullName}' is missing a '?' on {location}");
				break;
			}
		}

		internal static (TypeReference Type, Null Nullability) GetCallbackTypeNullability (TypeReference managedType,
			Null [] nullable, Null context, GenericInstanceType? delegateType, Null [] typeNullability, Null typeContext)
		{
			managedType = StripModifiers (managedType);
			if (managedType is ByReferenceType byReference)
				managedType = StripModifiers (byReference.ElementType);
			var managedNullability = GetNullability (nullable, 0, context);
			if (managedType is GenericParameter parameter && delegateType is not null) {
				var position = 1;
				for (var i = 0; i < parameter.Position; i++)
					position += CountNullablePositions (delegateType.GenericArguments [i]);
				managedType = delegateType.GenericArguments [parameter.Position];
				// T uses the instantiated argument's annotation; T? remains nullable.
				if (managedNullability != Null.Annotated && CountNullablePositions (managedType) > 0)
					managedNullability = GetNullability (typeNullability, position, typeContext);
			}

			if (IsNullableValueType (managedType))
				managedNullability = Null.Annotated;
			return (managedType, managedNullability);
		}

		static Null GetNullability (Null [] nullable, int position, Null context)
		{
			if (nullable.Length == 0)
				return context;
			if (nullable.Length == 1)
				return nullable [0];
			if (position >= nullable.Length)
				throw new InvalidOperationException ("The NullableAttribute does not contain enough flags for the callback signature.");
			return nullable [position];
		}

		static TypeReference StripModifiers (TypeReference type)
		{
			while (type is IModifierType modifier)
				type = modifier.ElementType;
			return type;
		}

		static bool IsNullableValueType (TypeReference type)
		{
			return type is GenericInstanceType generic && generic.ElementType.FullName == "System.Nullable`1";
		}

		static int CountNullablePositions (TypeReference type)
		{
			type = StripModifiers (type);
			if (type is ByReferenceType byReference)
				return CountNullablePositions (byReference.ElementType);
			if (type is Mono.Cecil.ArrayType array)
				return 1 + CountNullablePositions (array.ElementType);
			if (type is GenericInstanceType generic) {
				var count = IsNullableValueType (type) ? 0 : 1;
				foreach (var argument in generic.GenericArguments)
					count += CountNullablePositions (argument);
				return count;
			}
			return type.IsValueType && type is not GenericParameter ? 0 : 1;
		}

		/// <summary>
		/// Unwraps a native type to find the FunctionProtoType inside a block pointer.
		/// Returns null if the type is not a block pointer.
		/// </summary>
		static FunctionProtoType? GetBlockFunctionProtoType (ClangSharp.Type type)
		{
			if (type.UnqualifiedDesugaredType is not BlockPointerType blockPointer)
				return null;

			// CanonicalType would also discard the signature's inner nullability annotations.
			return blockPointer.PointeeType.UnqualifiedDesugaredType as FunctionProtoType;
		}
	}
}
