using System;
using System.Collections.Generic;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	public class Helper {

		static Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition> ();

		// make sure we load assemblies only once into memory
		public static AssemblyDefinition GetAssembly (string assemblyName)
		{
			if (!cache.TryGetValue (assemblyName, out var ad)) {
				ad = AssemblyDefinition.ReadAssembly (assemblyName);
				cache.Add (assemblyName, ad);
			}
			return ad;
		}

		public static IEnumerable<MethodDefinition> FilterMethods (AssemblyDefinition assembly, Func<MethodDefinition, bool>? filter)
		{
			foreach (var module in assembly.Modules) {
				foreach (var type in module.Types) {
					foreach (var method in FilterMethods (type, filter))
						yield return method;
				}
			}
			yield break;
		}

		static IEnumerable<MethodDefinition> FilterMethods (TypeDefinition type, Func<MethodDefinition, bool>? filter)
		{
			if (type.HasMethods) {
				foreach (var method in type.Methods) {
					if ((filter == null) || filter (method))
						yield return method;
				}
			}
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes) {
					foreach (var method in FilterMethods (nested, filter))
						yield return method;
				}
			}
			yield break;
		}
	}
}
