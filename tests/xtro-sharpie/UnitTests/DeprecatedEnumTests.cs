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
public class DeprecatedEnumTests {
	const string Framework = "DeprecatedEnumTests";
	const string Deprecated = "__attribute__((availability(ios, introduced=10.0, deprecated=27.0)))";
	const string Missing = "!deprecated-attribute-missing! NativeEnum missing a [Deprecated] attribute";

	static void AddAttribute (TypeDefinition type, string name, string? value = null)
	{
		var module = type.Module;
		var attributeType = new TypeReference ("Tests", name, module, module);
		var constructor = new MethodReference (".ctor", module.TypeSystem.Void, attributeType) { HasThis = true };
		var attribute = new CustomAttribute (constructor);
		if (value is not null) {
			constructor.Parameters.Add (new ParameterDefinition (module.TypeSystem.String));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (module.TypeSystem.String, value));
		}
		type.CustomAttributes.Add (attribute);
	}

	static TypeDefinition CreateEnum (ModuleDefinition module, string framework = Framework)
	{
		var enumType = new TypeReference ("System", "Enum", module, module.TypeSystem.CoreLibrary);
		var type = new TypeDefinition (framework, "ManagedEnum", TypeAttributes.Public, enumType);
		module.Types.Add (type);
		AddAttribute (type, "NativeNameAttribute", "NativeEnum");
		return type;
	}

	static string [] Check (string declarations, Platforms platform = Platforms.iOS, Action<ModuleDefinition, TypeDefinition>? prepare = null,
		Action<TranslationUnitDecl>? inspect = null)
	{
		var directory = Cache.CreateTemporaryDirectory ();
		var headers = Path.Combine (directory, Framework + ".framework", "Headers");
		Directory.CreateDirectory (headers);
		var header = Path.Combine (headers, Framework + ".h");
		File.WriteAllText (header, declarations);
		using var module = ModuleDefinition.CreateModule (Framework, ModuleKind.Dll);
		var type = CreateEnum (module);
		prepare?.Invoke (module, type);
		var frameworks = module.Types.Select (Helpers.GetFramework).Concat (new [] { Framework }).Distinct ().ToArray ();
		var previousPlatform = Helpers.Platform;
		Helpers.Platform = platform;
		foreach (var framework in frameworks)
			Log.On (framework).Clear ();
		try {
			var check = new DeprecatedCheck (new BindingResult ());
			var visitor = new SharpieVisitor (check.BindingResult) { check };
			foreach (var managed in module.Types)
				check.VisitManagedType (managed);
			using var index = CXIndex.Create ();
			var result = CXTranslationUnit.TryParse (index, header, ["-x", "objective-c", "-Wno-deprecated-declarations"], [],
				CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes, out var handle);
			Assert.That (result, Is.EqualTo (CXErrorCode.CXError_Success), "Parsing synthetic header");
			using var unit = TranslationUnit.GetOrCreate (handle);
			Assert.That (unit, Is.Not.Null, "Translation unit");
			Assert.That (unit.Handle.NumDiagnostics, Is.Zero, "Synthetic header diagnostics");
			inspect?.Invoke (unit.TranslationUnitDecl);
			visitor.Visit (unit.TranslationUnitDecl);
			visitor.EndVisit ();
			return frameworks.SelectMany (framework => Log.On (framework)).ToArray ();
		} finally {
			foreach (var framework in frameworks)
				Log.On (framework).Clear ();
			Helpers.Platform = previousPlatform;
			Directory.Delete (directory, true);
		}
	}

	[TestCase ("enum " + Deprecated + " NativeEnum { Value };")]
	[TestCase ("typedef enum { Value } NativeEnum " + Deprecated + ";")]
	[TestCase ("typedef enum NativeTag { Value } NativeEnum " + Deprecated + ";")]
	[TestCase ("enum " + Deprecated + " NativeTag { Value }; typedef enum NativeTag NativeEnum;")]
	[TestCase ("typedef unsigned long NativeEnum " + Deprecated + ";")]
	[TestCase ("typedef enum NativeEnum { Value } NativeEnum " + Deprecated + ";")]
	[TestCase ("enum " + Deprecated + " NativeEnum { Value }; typedef enum NativeEnum NativeEnum " + Deprecated + ";")]
	[TestCase ("enum " + Deprecated + " NativeEnum; enum NativeEnum { Value };")]
	public void MissingDeprecation (string declarations)
	{
		Assert.That (Check (declarations), Is.EqualTo (new [] { Missing }));
	}

	[TestCase ("NS_ENUM")]
	[TestCase ("NS_OPTIONS")]
	public void AppleEnumMacros (string macro)
	{
		var messages = Check ($$"""
			#define NS_ENUM(type, name) enum name : type name; enum name : type
			#define NS_OPTIONS(type, name) NS_ENUM(type, name)
			typedef {{macro}} (unsigned long, NativeEnum) { Value = 1 } {{Deprecated}};
			""");
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[Test]
	public void TrailingAttributeBelongsToTypedef ()
	{
		var messages = Check ($"typedef enum NativeTag {{ Value }} NativeEnum {Deprecated};", inspect: unit => {
			var nativeEnum = unit.Decls.OfType<EnumDecl> ().Single ();
			var typedef = unit.Decls.OfType<TypedefDecl> ().Single (v => v.Name == "NativeEnum");
			Assert.That (nativeEnum.Attrs.GetAvailabilityAttributes (), Is.Empty, "Enum has no deprecation to check");
			Assert.That (typedef.Attrs.GetAvailabilityAttributes ().Count (), Is.EqualTo (1), "Deprecation is on typedef");
		});
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[Test]
	public void AliasUsesItsOwnFramework ()
	{
		var messages = Check ($$"""
			#line 1 "/native/OtherFramework.framework/Headers/OtherFramework.h"
			enum {{Deprecated}} NativeTag { Value };
			#line 1 "/native/DeprecatedEnumTests.framework/Headers/DeprecatedEnumTests.h"
			typedef enum NativeTag NativeEnum;
			""");
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[TestCase (Platforms.iOS, true)]
	[TestCase (Platforms.tvOS, true)]
	[TestCase (Platforms.MacCatalyst, true)]
	[TestCase (Platforms.macOS, false)]
	public void PlatformInheritance (Platforms platform, bool expected)
	{
		var messages = Check ($"typedef enum {{ Value }} NativeEnum {Deprecated};", platform);
		Assert.That (messages, Is.EqualTo (expected ? new [] { Missing } : []));
	}

	[TestCase ("maccatalyst", Platforms.MacCatalyst, true)]
	[TestCase ("macos", Platforms.MacCatalyst, false)]
	[TestCase ("tvos", Platforms.tvOS, true)]
	[TestCase ("macos", Platforms.macOS, true)]
	[TestCase ("ios", Platforms.macOS, false)]
	public void ExplicitPlatform (string nativePlatform, Platforms platform, bool expected)
	{
		var messages = Check ($"enum __attribute__((availability({nativePlatform}, introduced=14.0, deprecated=27.0))) NativeEnum {{ Value }};", platform);
		Assert.That (messages, Is.EqualTo (expected ? new [] { Missing } : []));
	}

	[TestCase ("maccatalyst", Platforms.MacCatalyst)]
	[TestCase ("tvos", Platforms.tvOS)]
	public void ExplicitIntroductionPreventsInheritance (string nativePlatform, Platforms platform)
	{
		var messages = Check ($"enum {Deprecated} __attribute__((availability({nativePlatform}, introduced=14.0))) NativeEnum {{ Value }};", platform);
		Assert.That (messages, Is.Empty);
	}

	[TestCase ("ObsoleteAttribute", null, Platforms.iOS)]
	[TestCase ("AdviceAttribute", "Use another enum.", Platforms.iOS)]
	[TestCase ("ObsoletedOSPlatformAttribute", "ios27.0", Platforms.iOS)]
	[TestCase ("UnsupportedOSPlatformAttribute", "ios", Platforms.iOS)]
	[TestCase ("ObsoletedOSPlatformAttribute", "ios27.0", Platforms.MacCatalyst)]
	[TestCase ("ObsoletedOSPlatformAttribute", "maccatalyst27.0", Platforms.MacCatalyst)]
	public void AcceptedManagedMetadata (string name, string? value, Platforms platform)
	{
		var messages = Check ($"enum {Deprecated} NativeEnum {{ Value }};", platform, (_, type) => AddAttribute (type, name, value));
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void WrongManagedPlatform ()
	{
		var messages = Check ($"enum {Deprecated} NativeEnum {{ Value }};", prepare: (_, type) => AddAttribute (type, "ObsoletedOSPlatformAttribute", "macos27.0"));
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[TestCase ("typedef enum NativeEnum { Value } NativeEnum __attribute__((availability(ios, unavailable)));")]
	[TestCase ("enum " + Deprecated + " NativeEnum { Value }; typedef enum NativeEnum NativeEnum __attribute__((availability(ios, unavailable)));")]
	[TestCase ("enum __attribute__((availability(ios, unavailable))) NativeTag { Value }; typedef enum NativeTag NativeEnum " + Deprecated + ";")]
	[TestCase ("typedef int NativeEnum;")]
	[TestCase ("enum NativeEnum { Value };")]
	[TestCase ("typedef enum NativeEnum { Value } UnrelatedAlias " + Deprecated + ";")]
	public void UnavailableAndUnannotated (string declarations)
	{
		var messages = Check (declarations);
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void UnavailableAliasDoesNotHideEnum ()
	{
		var messages = Check ($"enum {Deprecated} NativeEnum {{ Value }}; typedef enum NativeEnum UnrelatedAlias __attribute__((availability(ios, unavailable)));");
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[TestCase (true)]
	[TestCase (false)]
	public void NativeNameAndFallback (bool legacy)
	{
		var messages = Check ($"enum {Deprecated} NativeEnum {{ Value }};", prepare: (_, type) => {
			type.CustomAttributes.Clear ();
			if (legacy)
				AddAttribute (type, "NativeAttribute", "NativeEnum");
			else
				type.Name = "NativeEnum";
		});
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[Test]
	public void NativeNameTakesPrecedence ()
	{
		var messages = Check ($"enum {Deprecated} NativeEnum {{ Value }};", prepare: (_, type) => {
			type.CustomAttributes.Clear ();
			AddAttribute (type, "NativeAttribute", "LegacyName");
			AddAttribute (type, "NativeNameAttribute", "NativeEnum");
		});
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}

	[TestCase ("5.0")]
	[TestCase ("100000.0")]
	public void ExcludedDeprecationVersions (string version)
	{
		var messages = Check ($"enum __attribute__((availability(ios, introduced=2.0, deprecated={version}))) NativeEnum {{ Value }};");
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void ChecksEveryPublicMatchInTheCorrectFramework ()
	{
		var messages = Check ($"enum {Deprecated} NativeEnum {{ Value }};", prepare: (module, type) => {
			AddAttribute (type, "ObsoleteAttribute");
			CreateEnum (module);
			CreateEnum (module, "OtherFramework");
			var hidden = CreateEnum (module);
			hidden.IsPublic = false;
		});
		Assert.That (messages, Is.EqualTo (new [] { Missing }));
	}
}
