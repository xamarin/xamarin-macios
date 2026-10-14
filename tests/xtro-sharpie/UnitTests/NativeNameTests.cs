// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

using Extrospection;
using Mono.Cecil;
using Sharpie.Bind;

namespace Xamarin.Tests;

[TestFixture]
[NonParallelizable]
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

	[Test]
	public void EnumsWithDifferentNativeNamesDoNotCollide ()
	{
		using var module = ModuleDefinition.CreateModule ("NativeNameTests", ModuleKind.Dll);
		var enumType = new TypeReference ("System", "Enum", module, module.TypeSystem.CoreLibrary);
		var first = new TypeDefinition ("FirstFramework", "PathStatus", TypeAttributes.Public, enumType);
		var second = new TypeDefinition ("SecondFramework", "PathStatus", TypeAttributes.Public, enumType);
		first.CustomAttributes.Add (CreateAttribute (module, "NativeNameAttribute", "native_path_status_t"));
		second.CustomAttributes.Add (CreateAttribute (module, "NativeAttribute", null));
		var check = new EnumCheck (new BindingResult ());
		Log.On (first.Namespace).Clear ();
		Log.On (second.Namespace).Clear ();
		try {
			check.VisitManagedType (first);
			check.VisitManagedType (second);

			Assert.That (Log.On (first.Namespace), Is.Empty);
			Assert.That (Log.On (second.Namespace), Is.Empty);
		} finally {
			Log.On (first.Namespace).Clear ();
			Log.On (second.Namespace).Clear ();
		}
	}
}
