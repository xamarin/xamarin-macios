// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Mono.Cecil.Rocks;

namespace AssemblyPreparerTests;

public class ApplyPreserveAttributeTests : BaseClass {
	[Test]
	[TestCase (ApplePlatform.MacCatalyst, false)]
	[TestCase (ApplePlatform.iOS, false)]
	[TestCase (ApplePlatform.TVOS, false)]
	[TestCase (ApplePlatform.MacOSX, true)]
	public void ExplicitInterfaceMembers (ApplePlatform platform, bool isCoreCLR)
	{
		var code = @"
		using System;
		using Foundation;

		interface IMyInterface {
			int MyProperty { get; set; }
			event EventHandler MyEvent;
		}

		[Preserve (AllMembers = true)]
		class MyClass : IMyInterface {
			int IMyInterface.MyProperty { get; set; }
			event EventHandler IMyInterface.MyEvent {
				add {}
				remove {}
			}
		}
		";

		AssertPrepare (platform, isCoreCLR, code, out var assemblyDefinition);

		var moduleConstructor = assemblyDefinition.MainModule.Types.Single (v => v.Name == "<Module>").GetStaticConstructor ();
		Assert.That (moduleConstructor, Is.Not.Null, "Module constructor");
		var signatures = moduleConstructor.CustomAttributes
			.Where (v => v.AttributeType.Name == "DynamicDependencyAttribute")
			.Where (v => v.ConstructorArguments.Count == 2)
			.Where (v => v.ConstructorArguments [0].Value is string)
			.Where (v => ((TypeReference) v.ConstructorArguments [1].Value).FullName == "MyClass")
			.Select (v => (string) v.ConstructorArguments [0].Value)
			.ToArray ();

		Assert.That (signatures, Does.Contain ("IMyInterface#MyProperty"), "Property");
		Assert.That (signatures, Does.Contain ("IMyInterface#MyEvent"), "Event");
		Assert.That (signatures, Has.None.Contains ("IMyInterface."), "Unescaped explicit interface member");
	}
}
