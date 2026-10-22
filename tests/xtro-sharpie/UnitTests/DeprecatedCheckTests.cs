// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using ClangSharp;
using ClangSharp.Interop;
using Extrospection;
using Mono.Cecil;
using Sharpie.Bind;

namespace Xamarin.Tests;

[TestFixture]
[NonParallelizable]
public class DeprecatedCheckTests {
	const string Framework = "DeprecatedCheckTests";

	static void AddAttribute (ModuleDefinition module, Mono.Cecil.ICustomAttributeProvider provider, string ns, string name, string value)
	{
		var attributeType = new TypeReference (ns, name, module, module);
		var constructor = new MethodReference (".ctor", module.TypeSystem.Void, attributeType) { HasThis = true };
		constructor.Parameters.Add (new ParameterDefinition (module.TypeSystem.String));
		var attribute = new CustomAttribute (constructor);
		attribute.ConstructorArguments.Add (new CustomAttributeArgument (module.TypeSystem.String, value));
		provider.CustomAttributes.Add (attribute);
	}

	static string [] Check (string declarations, TypeDefinition type)
	{
		var directory = Cache.CreateTemporaryDirectory ();
		var headers = Path.Combine (directory, Framework + ".framework", "Headers");
		Directory.CreateDirectory (headers);
		var header = Path.Combine (headers, Framework + ".h");
		File.WriteAllText (header, declarations);
		var previousPlatform = Helpers.Platform;
		Helpers.Platform = Platforms.iOS;
		Log.On (Framework).Clear ();
		try {
			var bindingResult = new BindingResult ();
			var check = new DeprecatedCheck (bindingResult);
			check.VisitManagedType (type);
			var visitor = new SharpieVisitor (bindingResult) { check };
			using var index = CXIndex.Create ();
			var result = CXTranslationUnit.TryParse (index, header, ["-x", "objective-c"], [],
				CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes, out var handle);
			Assert.That (result, Is.EqualTo (CXErrorCode.CXError_Success), "Parsing synthetic header");
			using var unit = TranslationUnit.GetOrCreate (handle);
			Assert.That (unit, Is.Not.Null, "Translation unit");
			Assert.That (unit.Handle.NumDiagnostics, Is.Zero, "Synthetic header diagnostics");
			visitor.Visit (unit.TranslationUnitDecl);
			visitor.EndVisit ();
			return Log.On (Framework).ToArray ();
		} finally {
			Log.On (Framework).Clear ();
			Helpers.Platform = previousPlatform;
			Directory.Delete (directory, true);
		}
	}

	[Test]
	[Combinatorial]
	public void DeprecatedSelectors (
		[Values (false, true)] bool classMethod,
		[Values (false, true)] bool property,
		[Values (null, "ios26.0", "tvos26.0")] string? deprecation)
	{
		using var module = ModuleDefinition.CreateModule (Framework, ModuleKind.Dll);
		var type = new TypeDefinition (Framework, "ManagedScreen", TypeAttributes.Public);
		module.Types.Add (type);
		AddAttribute (module, type, "ObjCRuntime", "RegisterAttribute", "UIScreen");
		var selector = property ? "mainScreen" : "deprecatedMethod";
		var attributes = MethodAttributes.Public;
		if (classMethod)
			attributes |= MethodAttributes.Static;
		var method = new MethodDefinition (property ? "get_MainScreen" : "DeprecatedMethod", attributes, module.TypeSystem.Int32);
		type.Methods.Add (method);
		AddAttribute (module, method, "Foundation", "ExportAttribute", selector);
		Mono.Cecil.ICustomAttributeProvider provider = method;
		if (property) {
			method.Attributes |= MethodAttributes.SpecialName | MethodAttributes.HideBySig;
			var managedProperty = new PropertyDefinition ("MainScreen", PropertyAttributes.None, module.TypeSystem.Int32) { GetMethod = method };
			type.Properties.Add (managedProperty);
			provider = managedProperty;
		}
		if (deprecation is not null)
			AddAttribute (module, provider, "System.Runtime.Versioning", "ObsoletedOSPlatformAttribute", deprecation);

		var declaration = property
			? $"@property ({(classMethod ? "class, " : "")}readonly) int {selector}"
			: $"{(classMethod ? "+" : "-")} (int){selector}";
		var messages = Check ($"""
			@interface UIScreen
			{declaration} __attribute__((availability(ios, introduced=2.0, deprecated=26.0)));
			@end
			""", type);

		var fullname = $"{(classMethod ? "+" : "")}UIScreen::{selector}";
		if (deprecation == "ios26.0") {
			Assert.That (messages, Is.Empty);
		} else {
			Assert.That (messages, Is.EqualTo (new [] {
				$"!deprecated-attribute-missing! {fullname} missing a [Deprecated] attribute",
			}));
		}
	}
}
