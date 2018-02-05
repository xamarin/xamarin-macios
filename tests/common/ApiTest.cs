// Tests are common to both mtouch and mmp
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Tests;

namespace Xamarin.ApiTest
{
	[TestFixture]
	public class BlockLiteral
	{
		[Test]
#if MONOTOUCH
		[TestCase (Profile.iOS)]
		[TestCase (Profile.watchOS)]
		[TestCase (Profile.tvOS)]
#else
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.macOSFull)]
#endif
		public void AlwaysOptimizable (Profile profile)
		{
			// This test ensures that all methods that call BlockLiteral.SetupBlock[Impl] have an [BindingImpl (BindingImplOptions.Optimizable)] attribute.

			var dll = Configuration.GetBaseLibrary (profile);
			var asm = AssemblyDefinition.ReadAssembly (dll);

			var failures = new List<string> ();

			foreach (var type in asm.MainModule.GetTypes ()) {
				if (type.Name == "BlockLiteral")
					continue;

				foreach (var method in type.Methods) {
					if (!method.HasBody)
						continue;
					var callsSetupBlock = false;
					foreach (var instr in method.Body.Instructions) {
						var mr = instr.Operand as MethodReference;
						if (mr == null)
							continue;
						if (mr.DeclaringType.Name != "BlockLiteral")
							continue;
						if (mr.Name != "SetupBlock" && mr.Name != "SetupBlockUnsafe")
							continue;
						callsSetupBlock = true;
						break;
					}
					if (!callsSetupBlock)
						continue;

					// This method calls BlockLiteral.SetupBlock. Ensure it has a [BindingImpl (BindingImplOptions.Optimizable)] attribute.
					var isOptimizable = IsOptimizable (method);
					if (!isOptimizable && (method.IsGetter || method.IsSetter)) {
						var property = method.DeclaringType.Properties.FirstOrDefault ((v) => v.GetMethod == method || v.SetMethod == method);
						isOptimizable = IsOptimizable (property);
					}
					foreach (var ca in method.CustomAttributes) {
						if (ca.AttributeType.Name != "BindingImplAttribute")
							continue;
						var value = (int) ca.ConstructorArguments [0].Value;
						if ((value & 0x2) == 0x2) { // BindingImplOptions.Optimizable = 2
							isOptimizable = true;
							break;
						}
					}
					if (!isOptimizable)
						failures.Add ($"The method {method.FullName} calls BlockLiteral.SetupBlock[Unsafe], but it does not have a [BindingImpl (BindingImplOptions.Optimizable)] attribute.");
				}
			}

			CollectionAssert.IsEmpty (failures, "All methods calling SetupBlock[Unsafe] must be optimizable");
		}

		bool IsOptimizable (ICustomAttributeProvider provider)
		{
			foreach (var ca in provider.CustomAttributes) {
				if (ca.AttributeType.Name != "BindingImplAttribute")
					continue;
				var value = (int) ca.ConstructorArguments [0].Value;
				if ((value & 0x2) == 0x2) // BindingImplOptions.Optimizable = 2
					return true;
			}

			return false;
		}
	}
}
