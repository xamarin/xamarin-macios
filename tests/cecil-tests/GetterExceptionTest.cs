using System;
using Mono.Cecil;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Mono.Cecil.Cil;
using Xamarin.Utils;
using Xamarin.Tests;

namespace Cecil.Tests
{
	[TestFixture]
	public class GetterExceptionTest
	{
		Dictionary<string, AssemblyDefinition> assemblyCache = new ();

		[OneTimeSetUp]
		public void SetUp ()
		{
			foreach (string assemblyPath in Helper.NetPlatformAssemblies) {
				var assembly = Helper.GetAssembly (assemblyPath);
				assemblyCache.Add (assemblyPath, assembly);
			}
		}

		[OneTimeTearDown]
		public void TearDown ()
		{
			assemblyCache.Clear ();
		}

		bool IsMemberObsolete(ICustomAttributeProvider member)
		{
			if (member is null || !member.HasCustomAttributes)
				return false;

			return member.CustomAttributes.Any((m) =>
					m.AttributeType.Name == "ObsoleteAttribute" ||
					m.AttributeType.Name == "AdviceAttribute" ||
					m.AttributeType.Name == "ObsoletedOSPlatformAttribute");
		}

		bool VerifyIfGetterThrowsException (MethodDefinition methodDefinition)
		{
			if (methodDefinition is null || methodDefinition.Body is null)
				return false;

			foreach (var inst in methodDefinition.Body.Instructions) {
				if (inst.OpCode == OpCodes.Throw) {
					return true;
				}
			}

			return false;
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void TestForAssembliesWithGetterExceptions (string assemblyPath)
		{
			Dictionary<string, string> propertiesWithGetterExceptions = new ();

			if (assemblyCache.TryGetValue (assemblyPath, out var cache)) {
				foreach (TypeDefinition type in cache.MainModule.Types) {
					foreach (PropertyDefinition property in type.Properties) {
						if (!IsMemberObsolete(property) && VerifyIfGetterThrowsException(property.GetMethod))
							propertiesWithGetterExceptions [type.Name] = property.Name;
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