using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class MarshalAsTest {
		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblyDefinitions))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void TestAssembly (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var failedMethods = new List<string> ();
			List<string>? failures = null;
			var checkedTypes = new List<TypeReference> ();
			foreach (var m in assembly.EnumerateMethods ((m) => m.HasPInvokeInfo)) {
				failures = null;
				checkedTypes.Clear ();
				if (!CheckMarshalAs (checkedTypes, m, ref failures)) {
					failedMethods.Add ($"Found {failures!.Count} issues with {m.FullName}:\n\t{string.Join ("\n\t", failures)}");
				}
			}

			Assert.That (failedMethods, Is.Empty, "Methods with bool return type / parameters and no MarshalAs attribute.");
		}

		static void AddFailure (ref List<string>? failures, string failure)
		{
			if (failures is null)
				failures = new List<string> ();

			failures.Add (failure);
			Console.WriteLine (failure);
		}

		bool CheckMarshalAs (List<TypeReference> checkedTypes, MethodDefinition method, ref List<string>? failures)
		{
			var rv = true;

			if (!CheckMarshalAs (checkedTypes, method, method.MethodReturnType.ReturnType, method.MethodReturnType, ref failures)) {
				AddFailure (ref failures, $"The {GetTypeName (method.ReturnType)} return type does not have a MarshalAs attribute: {method.FullName}");
				rv = false;
			}

			if (method.HasParameters) {
				for (var i = 0; i < method.Parameters.Count; i++) {
					var param = method.Parameters [i];
					var paramType = param.ParameterType;
					if (paramType.IsByReference)
						paramType = paramType.GetElementType ();
					if (!CheckMarshalAs (checkedTypes, method, paramType, param, ref failures)) {
						AddFailure (ref failures, $"The {GetTypeName (paramType)} parameter #{i + 1} ({param.Name}) does not have a MarshalAs attribute: {method.FullName}");
						rv = false;
					}
				}
			}

			return rv;
		}

		bool CheckMarshalAsDelegate (List<TypeReference> checkedTypes, MethodDefinition method, TypeReference type, IMarshalInfoProvider provider, ref List<string>? failures)
		{
			var invokeMethod = type.Resolve ().Methods.Single (v => v.Name == "Invoke");
			if (!CheckMarshalAs (checkedTypes, invokeMethod, ref failures)) {
				AddFailure (ref failures, $"For the delegate type {type.FullName}");
				return false;
			}
			return true;
		}

		bool CheckValueType (List<TypeReference> checkedTypes, MethodDefinition method, TypeReference tr, ref List<string>? failures)
		{
			var rv = true;

			var type = tr.Resolve ();

			// System.Runtime.InteropServices.NFloat is in a custom assembly in .NET 6, so add a special case.
			if (type is null && tr.FullName == "System.Runtime.InteropServices.NFloat")
				return true;

			if (type is null)
				throw new Exception ($"Unable to resolve {tr.FullName}");

			if (type.IsEnum)
				return true;

			if ((type.Attributes & TypeAttributes.ExplicitLayout) == TypeAttributes.ExplicitLayout)
				return true;

			foreach (var field in type.Fields) {
				if (field.IsStatic)
					continue;

				if (!CheckMarshalAs (checkedTypes, method, field.FieldType, field, ref failures)) {
					AddFailure (ref failures, $"The {GetTypeName (field.FieldType)} field '{field.Name}' in {tr.FullName} does not have a MarshalAs attribute. Original method: {method.FullName}");
					rv = false;
				}
			}
			return rv;
		}

		static string GetTypeName (TypeReference type)
		{
			return type.Resolve ().FullName;
		}

		static bool IsDelegate (TypeReference tr)
		{
			var t = tr.Resolve ();
			if (t is null)
				return false;

			var baseType = t.BaseType;
			return baseType.Namespace == "System" && baseType.Name == "MulticastDelegate";
		}

		bool CheckMarshalAs (List<TypeReference> checkedTypes, MethodDefinition method, TypeReference type, IMarshalInfoProvider provider, ref List<string>? failures)
		{
			if (checkedTypes.Contains (type))
				return true;
			checkedTypes.Add (type);

			if (type.IsValueType && !type.IsPrimitive)
				return CheckValueType (checkedTypes, method, type, ref failures);

			if (IsDelegate (type))
				return CheckMarshalAsDelegate (checkedTypes, method, type, provider, ref failures);

			if (provider.HasMarshalInfo)
				return true;

			// boolean or char type without MarshalAs directive
			if (type.Namespace == "System") {
				switch (type.Name) {
				case "Boolean":
				case "Char":
					return false;
				}
			}

			return true;
		}
	}
}
