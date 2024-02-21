using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public class GenericPInvokesTest {
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))]
		public void CheckSetupBlockUnsafeUsage (AssemblyInfo info)
		{
			// We should not call BlockLiteral.SetupBlockUnsafe in our code at all.
			// All our code should use the function pointer syntax for block creation:
			//     var block = new BlockLiteral (&function, nameof (<type where function is defined), nameof (function))

			var assembly = info.Assembly;
			var callsToSetupBlock = AllSetupBlocks (assembly);
			Assert.That (callsToSetupBlock.Select (v => v.FullName), Is.Empty, "No calls at all to BlockLiteral.SetupBlockUnsafe");
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))]
		public void CheckAllPInvokes (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var pinvokes = AllPInvokes (assembly).Where (IsPInvokeOK);
			Assert.IsTrue (pinvokes.Count () > 0);

			var failures = pinvokes.Where (ContainsGenerics).ToList ();
			var failingMethods = ListOfFailingMethods (failures);

			Assert.IsTrue (failures.Count () == 0,
				$"There are {failures.Count ()} pinvoke methods that contain generics. This will not work in .NET 7 and above (see https://github.com/xamarin/xamarin-macios/issues/11771 ):{failingMethods}");
		}

		string ListOfFailingMethods (IEnumerable<MethodDefinition> methods)
		{
			var list = new StringBuilder ();
			foreach (var method in methods) {
				list.Append ('\n').Append (method.FullName);
			}
			return list.ToString ();
		}

		static bool ContainsGenerics (MethodDefinition method)
		{
			return method.ContainsGenericParameter;
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

		IEnumerable<MethodDefinition> AllSetupBlocks (AssemblyDefinition assembly)
		{
			return assembly.EnumerateMethods (method => {
				if (!method.HasBody)
					return false;
				return method.Body.Instructions.Any (IsCallToSetupBlockUnsafe);
			});
		}

		static bool IsCallToSetupBlockUnsafe (Instruction instr)
		{
			if (!IsCall (instr))
				return false;

			var operand = instr.Operand;
			if (!(operand is MethodReference mr))
				return false;

			if (!mr.DeclaringType.Is ("ObjCRuntime", "BlockLiteral"))
				return false;

			if (mr.Name != "SetupBlockUnsafe")
				return false;

			if (!mr.ReturnType.Is ("System", "Void"))
				return false;

			if (!mr.HasParameters || mr.Parameters.Count != 2)
				return false;

			if (!mr.Parameters [0].ParameterType.Is ("System", "Delegate") || !mr.Parameters [1].ParameterType.Is ("System", "Delegate"))
				return false;

			return true;
		}

		static bool IsCall (Instruction instr)
		{
			return instr.OpCode == OpCodes.Call ||
				instr.OpCode == OpCodes.Calli;
		}
	}
}
