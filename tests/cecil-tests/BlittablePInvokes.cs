using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class BlittablePInvokes {
		struct MethodBlitResult {
			public MethodBlitResult (bool isBlittable)
			{
				IsBlittable = isBlittable;
				Result = new StringBuilder ();
			}
			public bool IsBlittable;
			public StringBuilder Result;
		}

		struct TypeAndIndex {
			public TypeAndIndex (TypeReference type, int index)
			{
				Type = type;
				Index = index;
			}
			public TypeReference Type;
			public int Index;
		}
		struct BlitAndReason {
			public BlitAndReason (bool isBlittable, string reason)
			{
				IsBlittable = isBlittable;
				Reason = reason;
			}
			public bool IsBlittable;
			public string Reason;
		}

		[Ignore ("work in progress - there are 4 failures, mostly due to delegates")]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))]
		public void CheckForNonBlittablePInvokes (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var pinvokes = AllPInvokes (assembly).Where (IsPInvokeOK);
			Assert.IsTrue (pinvokes.Count () > 0);

			var blitCache = new Dictionary<string, BlitAndReason> ();
			var results = pinvokes.Select (pi => IsMethodBlittable (assembly, pi, blitCache)).Where (r => !r.IsBlittable);
			if (results.Count () > 0) {
				var failString = new StringBuilder ();
				failString.Append ($"There is an issue with {results.Count ()} pinvokes in {assembly.Name} ({info.Path}):\n");
				foreach (var sb in results.Select (r => r.Result)) {
					failString.Append (sb.ToString ());
				}
				Assert.Fail (failString.ToString ());
			}

		}

		MethodBlitResult IsMethodBlittable (AssemblyDefinition assembly, MethodReference method, Dictionary<string, BlitAndReason> blitCache)
		{
			var result = new MethodBlitResult (true);
			var localResult = new StringBuilder ();
			var types = TypesFromMethod (method);
			foreach (var typeIndex in types) {
				if (!IsTypeBlittable (assembly, typeIndex.Type, localResult, blitCache)) {
					if (result.IsBlittable) {
						result.IsBlittable = false;
						result.Result.Append ($"    The P/Invoke {method.FullName} has been marked as non-blittable for the following reasons:\n");
					}
					if (typeIndex.Index < 0) {
						result.Result.Append ($"        The return type is");
					} else {
						result.Result.Append ($"        Parameter index {typeIndex.Index} is");
					}
					result.Result.Append ($" {typeIndex.Type}: {localResult.ToString ()}\n");
				}
			}
			return result;
		}

		IEnumerable<TypeAndIndex> TypesFromMethod (MethodReference method)
		{
			if (method.ReturnType is not null)
				yield return new TypeAndIndex (method.ReturnType, -1);
			var i = 0;
			foreach (var parameter in method.Parameters)
				yield return new TypeAndIndex (parameter.ParameterType, i++);
		}

		bool IsTypeBlittable (AssemblyDefinition assembly, TypeReference type, StringBuilder result, Dictionary<string, BlitAndReason> blitCache)
		{
			if (blitCache.TryGetValue (type.Name, out var cachedResult)) {
				if (!cachedResult.IsBlittable)
					result.Append ($" {cachedResult.Reason}");
				return cachedResult.IsBlittable;
			}
			if (IsBlittableTypesWeLike (type)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			if (IsBlittablePointer (type)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			var localResult = new StringBuilder ();
			if (IsBlittableValueType (assembly, type, localResult, blitCache)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			result.Append (localResult);
			blitCache [type.Name] = new BlitAndReason (false, result.ToString ());
			return false;
		}


		static HashSet<string> typesWeLike = new HashSet<string> () {
			"System.Void",
			"System.IntPtr",
			"System.UIntPtr",
			"ObjCRuntime.NativeHandle",
			"System.Byte",
			"System.SByte",
			"System.Int16",
			"System.UInt16",
			"System.Int32",
			"System.UInt32",
			"System.Int64",
			"System.UInt64",
			"System.Single",
			"System.Double",
			"System.Runtime.InteropServices.NFloat",
			"System.Runtime.InteropServices.NFloat&",
		};

		bool IsBlittableTypesWeLike (TypeReference t)
		{
			return typesWeLike.Contains (t.ToString ());
		}

		bool IsBlittablePointer (TypeReference type)
		{
			return type.IsPointer || type.IsFunctionPointer;
		}


		bool IsBlittableValueType (AssemblyDefinition assembly, TypeReference type, StringBuilder result, Dictionary<string, BlitAndReason> blitCache)
		{
			TypeDefinition? typeDefinition = type.Resolve ();
			if (typeDefinition is null) {
				result.Append ($" {type.FullName}: Unable to load type.");
				return false;
			}
			if (!typeDefinition.IsValueType) {
				// handy for debugging
				// change the true to false to get more information
				// than you'll probably need about the typeDefinition
				var other = true ? "" : $"IsByReference {typeDefinition.IsByReference} IsPointer {typeDefinition.IsPointer} IsSentinel {typeDefinition.IsSentinel} IsArray {typeDefinition.IsArray} IsGenericParameter {typeDefinition.IsGenericParameter} IsRequiredModifier {typeDefinition.IsRequiredModifier} IsOptionalModifier {typeDefinition.IsOptionalModifier} IsPinned {typeDefinition.IsPinned} IsFunctionPointer {typeDefinition.IsFunctionPointer} IsPrimitive {typeDefinition.IsPrimitive}";
				result.Append ($" {type.Name}: Type is not a value type.\n{other}\n");
				return false;
			}
			if (typeDefinition.IsEnum) {
				return true;
			}
			var allBlittable = true;
			// if we get here then this is a struct. We can presume
			// that a struct will be blittable until we know otherwise
			// this will prevent infinite recursion
			blitCache [type.Name] = new BlitAndReason (true, "");
			var fieldsResult = new StringBuilder ();

			// if we're here, this is a struct
			// a struct is blittable if and only if all the
			// non-static fields are blittable.
			foreach (var f in typeDefinition.Fields) {
				if (f.IsStatic)
					continue;
				var localResult = new StringBuilder ();
				if (!IsTypeBlittable (assembly, f.FieldType, localResult, blitCache)) {
					if (!allBlittable)
						fieldsResult.Append ($" {type.Name}:");
					fieldsResult.Append ($" ({f.Name}: {localResult})");
					allBlittable = false;
				}
			}
			if (!allBlittable) {
				result.Append (fieldsResult);
				blitCache [type.Name] = new BlitAndReason (false, fieldsResult.ToString ());
			}
			return allBlittable;
		}

		IEnumerable<MethodDefinition> AllPInvokes (AssemblyDefinition assembly)
		{
			return assembly.EnumerateMethods (method =>
				(method.Attributes & MethodAttributes.PInvokeImpl) != 0);
		}

		static bool IsPInvokeOK (MethodDefinition method)
		{
			var fullName = method.FullName;
			switch (fullName) {
			default:
				return true;
			}
		}
	}
}
