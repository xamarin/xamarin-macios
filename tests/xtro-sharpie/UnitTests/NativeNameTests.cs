// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

using ClangSharp;
using ClangSharp.Interop;
using Extrospection;
using Mono.Cecil;
using Sharpie.Bind;

namespace Xamarin.Tests;

[TestFixture]
[NonParallelizable]
public class NativeNameTests {
	const string Framework = "NativeNameTests";

	static TypeDefinition CreateEnum (ModuleDefinition module, string framework, string name, TypeReference backingType, object value)
	{
		var enumType = new TypeReference ("System", "Enum", module, module.TypeSystem.CoreLibrary);
		var type = new TypeDefinition (framework, name, TypeAttributes.Public, enumType);
		module.Types.Add (type);
		type.Fields.Add (new FieldDefinition ("value__", FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName, backingType));
		type.Fields.Add (new FieldDefinition ("Value", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal, type) { Constant = value });
		return type;
	}

	static string [] Check (string declarations, params TypeDefinition [] types)
	{
		var directory = Cache.CreateTemporaryDirectory ();
		var headers = Path.Combine (directory, Framework + ".framework", "Headers");
		Directory.CreateDirectory (headers);
		var header = Path.Combine (headers, Framework + ".h");
		File.WriteAllText (header, declarations);
		var frameworks = types.Select (Helpers.GetFramework).Concat ([Framework]).Distinct ().ToArray ();
		var previousPlatform = Helpers.Platform;
		Helpers.Platform = Platforms.macOS;
		foreach (var framework in frameworks)
			Log.On (framework).Clear ();
		try {
			var check = new EnumCheck (new BindingResult ());
			foreach (var type in types)
				check.VisitManagedType (type);
			using var index = CXIndex.Create ();
			var result = CXTranslationUnit.TryParse (index, header, ["-x", "objective-c"], [],
				CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes, out var handle);
			Assert.That (result, Is.EqualTo (CXErrorCode.CXError_Success), "Parsing synthetic header");
			using var unit = TranslationUnit.GetOrCreate (handle);
			Assert.That (unit, Is.Not.Null, "Translation unit");
			Assert.That (unit.Handle.NumDiagnostics, Is.Zero, "Synthetic header diagnostics");
			check.Visit (unit.TranslationUnitDecl);
			check.EndVisit ();
			return frameworks.SelectMany (framework => Log.On (framework)).ToArray ();
		} finally {
			foreach (var framework in frameworks)
				Log.On (framework).Clear ();
			Helpers.Platform = previousPlatform;
			Directory.Delete (directory, true);
		}
	}

	static CustomAttribute CreateAttribute (ModuleDefinition module, string name, string? nativeName)
	{
		var attributeType = new TypeReference ("ObjCRuntime", name, module, module);
		if (name == "ObsoleteAttribute")
			attributeType.Namespace = "System";
		else if (name == "ObsoletedOSPlatformAttribute")
			attributeType.Namespace = "System.Runtime.Versioning";
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
		var first = CreateEnum (module, "FirstFramework", "PathStatus", module.TypeSystem.Int64, 0L);
		var second = CreateEnum (module, "SecondFramework", "PathStatus", module.TypeSystem.Int64, 1L);
		first.CustomAttributes.Add (CreateAttribute (module, "NativeAttribute", null));
		first.CustomAttributes.Add (CreateAttribute (module, "NativeNameAttribute", "native_path_status_t"));
		second.CustomAttributes.Add (CreateAttribute (module, "NativeAttribute", null));

		var messages = Check ("""
			typedef long NSInteger;
			enum native_path_status_t : NSInteger { FirstValue = 0 };
			enum PathStatus : NSInteger { SecondValue = 1 };
			""", first, second);
		Assert.That (messages, Is.Empty);
	}

	[TestCase ("ObsoleteAttribute", null, true, "NativeEnum")]
	[TestCase ("ObsoletedOSPlatformAttribute", "macos10.14", true, "NativeEnum")]
	[TestCase ("ObsoletedOSPlatformAttribute", "macos10.14", true, "nativeenum")]
	[TestCase ("ObsoletedOSPlatformAttribute", "ios12.0", false, "NativeEnum")]
	public void DeprecatedManagedEnum (string attributeName, string? platform, bool skipped, string nativeName)
	{
		using var module = ModuleDefinition.CreateModule ("NativeNameTests", ModuleKind.Dll);
		var type = CreateEnum (module, Framework, "ManagedEnum", module.TypeSystem.Int32, 0);
		type.CustomAttributes.Add (CreateAttribute (module, "NativeNameAttribute", "NativeEnum"));
		type.CustomAttributes.Add (CreateAttribute (module, attributeName, platform));
		var messages = Check ($"enum {nativeName} : int {{ NativeValue = 42 }};", type);
		if (skipped) {
			Assert.That (messages, Is.Empty);
		} else {
			Assert.That (messages, Has.Length.EqualTo (2));
			Assert.That (messages, Has.Some.StartsWith ("!missing-enum-value!"));
			Assert.That (messages, Has.Some.StartsWith ("!extra-enum-value!"));
		}
	}

	[Test]
	public void DeprecatedNativeEnum ()
	{
		using var module = ModuleDefinition.CreateModule ("NativeNameTests", ModuleKind.Dll);
		var type = CreateEnum (module, Framework, "ManagedEnum", module.TypeSystem.Int32, 0);
		type.CustomAttributes.Add (CreateAttribute (module, "NativeAttribute", null));
		type.CustomAttributes.Add (CreateAttribute (module, "NativeNameAttribute", "NativeEnum"));
		var messages = Check ("""
			enum __attribute__((availability(macos, introduced=10.0, deprecated=10.14))) NativeEnum : int { NativeValue = 42 };
			""", type);
		Assert.That (messages, Is.Empty);
	}
}
