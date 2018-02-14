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
	public class ApiTest
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
			// This test ensures that all methods that call optimizable methods have an [BindingImpl (BindingImplOptions.Optimizable)] attribute.
			// Current optimizable methods:
			// * BlockLiteral.SetupBlock
			// * BlockLiteral.SetupBlockImpl
			// * Runtime.get_DynamicRegistrationSupported

			var dll = Configuration.GetBaseLibrary (profile);
			var asm = AssemblyDefinition.ReadAssembly (dll);

			var failures = new List<string> ();

			foreach (var type in asm.MainModule.GetTypes ()) {
				foreach (var method in type.Methods) {
					if (!method.HasBody)
						continue;
					var mustBeOptimizable = false;
					MethodReference mr = null;
					foreach (var instr in method.Body.Instructions) {
						mr = instr.Operand as MethodReference;
						if (mr == null)
							continue;
						switch (mr.DeclaringType.Name) {
						case "BlockLiteral":
							if (type == mr.DeclaringType)
								continue; // Calls within BlockLiteral without the optimizable attribute is allowed 
							
							switch (mr.Name) {
							case "SetupBlock":
							case "SetupBlockUnsafe":
								mustBeOptimizable = true;
								break;
							}
							break;
						case "Runtime":
							switch (mr.Name) {
							case "get_DynamicRegistrationSupported":
								mustBeOptimizable = true;
								break;
							}
							break;
						}
						if (mustBeOptimizable)
							break;
					}
					if (!mustBeOptimizable)
						continue;

					// Ensure it has a [BindingImpl (BindingImplOptions.Optimizable)] attribute.
					var isOptimizable = IsOptimizable (method);
					if (!isOptimizable)
						failures.Add ($"The method {method.FullName} calls {mr.FullName}, but it does not have a [BindingImpl (BindingImplOptions.Optimizable)] attribute.");
				}
			}

			if (failures.Count > 0)
				Assert.Fail ($"All methods calling optimizable API must be optimizable\n\t{string.Join ("\n\t", failures)}");
		}

		bool IsOptimizable (MethodDefinition method)
		{
			var isOptimizable = IsOptimizableProvider (method);
			if (!isOptimizable && (method.IsGetter || method.IsSetter)) {
				var property = method.DeclaringType.Properties.FirstOrDefault ((v) => v.GetMethod == method || v.SetMethod == method);
				if (IsOptimizableProvider (property))
					return true;
			}
			foreach (var ca in method.CustomAttributes) {
				if (ca.AttributeType.Name != "BindingImplAttribute")
					continue;
				var value = (int) ca.ConstructorArguments [0].Value;
				if ((value & 0x2) == 0x2) // BindingImplOptions.Optimizable = 2
					return true;
			}
			return false;
		}

		bool IsOptimizableProvider (ICustomAttributeProvider provider)
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
