using System;
using Mono.Cecil;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Mono.Cecil.Cil;
using Xamarin.Utils;
using Xamarin.Tests;
#nullable enable

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

		bool SubclassesException (TypeDefinition? type)
		{
			if (type is null)
				return false;

			if (type.Namespace == "System" && type.Name == "Exception")
				return true;

			return SubclassesException (type.BaseType?.Resolve ());
		}

		bool VerifyIfGetterThrowsException (MethodDefinition methodDefinition, out string exceptionMessage)
		{
			exceptionMessage = string.Empty;
			if (!methodDefinition.HasBody)
				return false;

			foreach (Instruction? inst in methodDefinition.Body.Instructions) {
				if (inst?.OpCode == OpCodes.Newobj && inst?.Operand is MemberReference reference) {
					if (!exceptionsToSkip.Contains (reference.DeclaringType.Name) &&
						SubclassesException (reference.DeclaringType as TypeDefinition)) {
						exceptionMessage = reference.DeclaringType.Name;
						return true;
					}
				}
			}

			return false;
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))]
		[Test]
		public void TestForAssembliesWithGetterExceptions (AssemblyInfo info)
		{
			Dictionary<string, string> propertiesWithGetterExceptions = new ();
			AssemblyDefinition assembly = info.Assembly;

			foreach (PropertyDefinition property in assembly.EnumerateProperties ()) {
				if (!IsMemberObsolete (property) && property.GetMethod is not null &&
					VerifyIfGetterThrowsException (property.GetMethod, out string exceptionConstructed))
					propertiesWithGetterExceptions [property.FullName] = $"Exception: {exceptionConstructed}";

			}

			Assert.AreEqual (0,
				propertiesWithGetterExceptions.Count (),
				$"Exceptions found in Getters: {string.Join (Environment.NewLine, propertiesWithGetterExceptions)}");
		}
	}
}
