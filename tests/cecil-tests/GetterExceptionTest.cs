using System;
using Mono.Cecil;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Mono.Cecil.Cil;
using Xamarin.Utils;
using Xamarin.Tests;

namespace Cecil.Tests {
	[TestFixture]
	public class GetterExceptionTest {
		readonly HashSet<string> exceptionsToSkip = new ()
		{
			"AudioQueueException",
			"ModelNotImplementedException",
			"You_Should_Not_Call_base_In_This_Method",
		};

		bool IsMemberObsolete (ICustomAttributeProvider member)
			 => member?.CustomAttributes?.Any ((m) =>
					m.AttributeType.Name == "ObsoleteAttribute" ||
					m.AttributeType.Name == "AdviceAttribute" ||
					m.AttributeType.Name == "ObsoletedOSPlatformAttribute") == true;

		bool VerifyIfGetterThrowsException (MethodDefinition methodDefinition, out string exceptionMessage)
		{
			exceptionMessage = string.Empty;
			if (!methodDefinition.HasBody)
				return false;

			foreach (Instruction? inst in methodDefinition.Body.Instructions) {
				if (inst?.OpCode == OpCodes.Newobj && inst?.Operand is MemberReference reference) {
					TypeReference? baseType = (reference.DeclaringType as TypeDefinition)?.BaseType;
					if (baseType is not null && baseType.Is ("System", "Exception") &&
						!exceptionsToSkip.Contains (reference.DeclaringType.Name)) {
						exceptionMessage = reference.DeclaringType.Name;
						return true;
					}
				}
			}

			return false;
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblies))]
		[Test]
		public void TestForAssembliesWithGetterExceptions (string assemblyPath)
		{
			Dictionary<string, string> propertiesWithGetterExceptions = new ();
			AssemblyDefinition assembly = Helper.GetAssembly (assemblyPath);

			foreach (TypeDefinition type in assembly.MainModule.Types) {
				foreach (PropertyDefinition property in type.Properties) {
					if (!IsMemberObsolete (property) && property.GetMethod != null &&
						VerifyIfGetterThrowsException (property.GetMethod, out string exceptionConstructed))
						propertiesWithGetterExceptions [type.Name] = $"{property.Name} Exception: {exceptionConstructed}";
				}
			}

			Assert.AreEqual (0,
				propertiesWithGetterExceptions.Count (),
				$"Exceptions found in Getters: {string.Join (Environment.NewLine, propertiesWithGetterExceptions)}");
		}
	}
}
