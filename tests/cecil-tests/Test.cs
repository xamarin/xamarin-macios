using System;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class Test {

		[TestCaseSource (typeof(Helper), "PlatformAssemblies")]
		// ref: https://github.com/xamarin/xamarin-macios/pull/7760
		public void IdentifyBackingFieldAssignation (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null)
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
			// look inside all .cctor (static constructor) insde `assemblyName`
			foreach (var m in Helper.FilterMethods (assembly!, (m) => m.IsStatic && m.IsConstructor)) {
				foreach (var ins in m.Body.Instructions) {
					if (ins.OpCode != OpCodes.Stsfld)
						continue;
					if (!(ins.Operand is FieldDefinition f))
						continue;
					var name = f.Name;
					if ((name [0] != '<') || !name.EndsWith (">k__BackingField"))
						continue;
					// filter valid usage
					// it's fine if the returned value is constant (won't ever change during execution)
					// there should be a comment in the source that confirm this behaviour
					switch (m.DeclaringType.FullName) {
					case "CoreFoundation.OSLog":
						if (name == "<Default>k__BackingField")
							break;
						goto default;
					case "Vision.VNUtils":
						if (name == "<NormalizedIdentityRect>k__BackingField")
							break;
						goto default;
					default:
						Assert.Fail ($"Unaudited {m.DeclaringType.FullName} -> {name}");
						break;
					}
				}
			}
		}
	}
}
