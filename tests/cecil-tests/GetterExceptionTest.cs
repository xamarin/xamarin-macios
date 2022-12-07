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
			"You_Should_Not_Call_base_In_This_Method"
		};

		bool IsMemberObsolete (ICustomAttributeProvider member)
		{
			if (member is null || !member.HasCustomAttributes)
				return false;

			return member.CustomAttributes.Any ((m) =>
					m.AttributeType.Name == "ObsoleteAttribute" ||
					m.AttributeType.Name == "AdviceAttribute" ||
					m.AttributeType.Name == "ObsoletedOSPlatformAttribute");
		}

		bool VerifyIfGetterThrowsException (MethodDefinition methodDefinition, out string exceptionMessage)
		{
			exceptionMessage = string.Empty;
			if (methodDefinition?.Body is null)
				return false;

			foreach (Instruction? inst in methodDefinition.Body.Instructions) {
				if (inst.OpCode == OpCodes.Newobj) {
					string? baseType = ((inst.Operand as MemberReference)
						?.DeclaringType as TypeDefinition)
						?.BaseType.Name;

					if (baseType != null && baseType.Equals ("Exception")) {
						string? exceptionType = (inst.Operand as MemberReference)?.DeclaringType.Name;
						if (exceptionType != null && !exceptionsToSkip.Contains (exceptionType)) {
							exceptionMessage = exceptionType;
							return true;
						}
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

			if (assembly != null) {
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

			} else {
				Assert.Ignore ($"{assemblyPath} could not be found (might be disabled in build.)");
			}
		}
	}
}
