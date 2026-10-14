// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

using Extrospection;
using Mono.Cecil;

namespace Xamarin.Tests;

[TestFixture]
public class NativeNameTests {
	static CustomAttribute CreateAttribute (ModuleDefinition module, string name, string? nativeName)
	{
		var attributeType = new TypeReference ("ObjCRuntime", name, module, module);
		var constructor = new MethodReference (".ctor", module.TypeSystem.Void, attributeType) { HasThis = true };
		var attribute = new CustomAttribute (constructor);
		if (nativeName is not null) {
			constructor.Parameters.Add (new ParameterDefinition (module.TypeSystem.String));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (module.TypeSystem.String, nativeName));
		}
		return attribute;
	}

	[TestCase (true, null)]
	[TestCase (false, null)]
	[TestCase (true, "LegacyNativeName")]
	[TestCase (false, "LegacyNativeName")]
	public void NativeNameTakesPrecedence (bool nativeFirst, string? legacyNativeName)
	{
		using var module = ModuleDefinition.CreateModule ("NativeNameTests", ModuleKind.Dll);
		var type = new TypeDefinition ("Tests", "ManagedEnum", TypeAttributes.Public);
		var native = CreateAttribute (module, "NativeAttribute", legacyNativeName);
		var nativeName = CreateAttribute (module, "NativeNameAttribute", "NativeEnum");
		type.CustomAttributes.Add (nativeFirst ? native : nativeName);
		type.CustomAttributes.Add (nativeFirst ? nativeName : native);

		Assert.That (Helpers.GetName (type), Is.EqualTo ("NativeEnum"));
	}

	[TestCase (null, "ManagedEnum")]
	[TestCase ("LegacyNativeName", "LegacyNativeName")]
	public void NativeNameFallback (string? nativeName, string expected)
	{
		using var module = ModuleDefinition.CreateModule ("NativeNameTests", ModuleKind.Dll);
		var type = new TypeDefinition ("Tests", "ManagedEnum", TypeAttributes.Public);
		type.CustomAttributes.Add (CreateAttribute (module, "NativeAttribute", nativeName));

		Assert.That (Helpers.GetName (type), Is.EqualTo (expected));
	}
}
