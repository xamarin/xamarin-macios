// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Mono.Cecil.Rocks;

namespace AssemblyPreparerTests;

public class MarkNSObjectsTests : BaseClass {
	[TestCase (ApplePlatform.MacCatalyst, true)]
	[TestCase (ApplePlatform.iOS, false)]
	[TestCase (ApplePlatform.TVOS, false)]
	[TestCase (ApplePlatform.MacOSX, true)]
	public void StaticRegistrarPreservesUserMembers (ApplePlatform platform, bool isCoreCLR)
	{
		var code = @"
		using System;
		using Foundation;

		public class BaseClass : NSObject {
			[Export (""func:"")]
			public virtual IntPtr Func (out NSObject value) { value = null; return IntPtr.Zero; }
		}

		public class Outer {
			public class Middle {
				public class DerivedClass : BaseClass {
					public override IntPtr Func (out NSObject value) { value = null; return IntPtr.Zero; }
				}
			}
		}
		";

		var modified = AssertPrepare (platform, isCoreCLR, RegistrarMode.Static, code, out var assembly);
		using (assembly) {
			Assert.That (modified, Is.True, "User assembly saved");
			var moduleConstructor = assembly.MainModule.Types.Single (v => v.Name == "<Module>").GetStaticConstructor ();
			Assert.That (moduleConstructor, Is.Not.Null, "Module constructor saved");
			var dependencies = moduleConstructor.CustomAttributes
				.Where (v => v.AttributeType.Name == "DynamicDependencyAttribute")
				.Where (v => v.ConstructorArguments.Count == 2 && v.ConstructorArguments [0].Value is string)
				.Select (v => $"{((TypeReference) v.ConstructorArguments [1].Value).FullName}:{v.ConstructorArguments [0].Value}")
				.ToArray ();
			Assert.That (dependencies, Does.Contain ("BaseClass:Func"), "Base method preserved");
			Assert.That (dependencies, Does.Contain ("Outer/Middle/DerivedClass:Func"), "Override preserved");

			var outer = assembly.MainModule.Types.Single (v => v.Name == "Outer");
			var middle = outer.NestedTypes.Single (v => v.Name == "Middle");
			var derived = middle.NestedTypes.Single (v => v.Name == "DerivedClass");
			var constructor = derived.GetStaticConstructor ();
			Assert.That (constructor, Is.Not.Null, "Nested type constructor saved");
			var parentTypes = constructor.CustomAttributes
				.Where (v => v.AttributeType.Name == "DynamicDependencyAttribute")
				.Where (v => v.ConstructorArguments.Count == 3)
				.Select (v => (string) v.ConstructorArguments [1].Value)
				.ToArray ();
			Assert.That (parentTypes, Does.Contain ("Outer"), "Outer parent preserved");
			Assert.That (parentTypes, Does.Contain ("Outer.Middle"), "Immediate parent preserved");
		}
	}
}
