// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace AssemblyPreparerTests;

public class InlineDlfcnMethodsStepTests : BaseClass {
	[Test]
	[TestCase (ApplePlatform.MacCatalyst, false)]
	[TestCase (ApplePlatform.iOS, false)]
	[TestCase (ApplePlatform.TVOS, false)]
	[TestCase (ApplePlatform.MacOSX, true)]
	public void MarkedTest (ApplePlatform platform, bool isCoreCLR)
	{
		var code = @"
		using System;
		using CoreAnimation;
		using Foundation;
		using ObjCRuntime;

		class MyClass : NSObject {
			void GetIntPtr ()
			{
				Console.WriteLine (Dlfcn.GetIntPtr (0, ""NativeSymbol""));
			}
		}";

		AssertPrepare (platform, isCoreCLR, code, out var assemblyDefinition);

		var type = assemblyDefinition.MainModule.Types.Single (v => v.Name == "MyClass");
		var platformReference = assemblyDefinition.MainModule.AssemblyReferences.Single (v => v.Name == $"Microsoft.{platform.AsString ()}");
		var platformAssembly = assemblyDefinition.MainModule.AssemblyResolver.Resolve (platformReference);
		var dlfcn = platformAssembly.MainModule.Types.Single (v => v.Name == "Dlfcn");

		var cctor = type.GetStaticConstructor ();
		Assert.That (cctor, Is.Null, "No static constructor should be needed.");

		void AssertHasDlfcnPInvokeCall (MethodDefinition method)
		{
			var instructions = method.Body.Instructions;
			var call = instructions.FirstOrDefault (v => v.OpCode == OpCodes.Call && v.Operand is MethodReference mr && mr.DeclaringType.FullName == dlfcn.FullName);
			Assert.That (call, Is.Not.Null, $"Expected a call to Dlfcn in {method}");
			var resolvedMethod = ((MethodReference) call.Operand).Resolve ();
			Assert.That (resolvedMethod, Is.Not.Null, $"Expected the call to resolve to a method in Dlfcn for {method}");
			Assert.That (resolvedMethod.PInvokeInfo, Is.Null, $"Expected the method to not be a PInvoke method for {method}");
		}

		Assert.Multiple (() => {
			AssertHasDlfcnPInvokeCall (type.Methods.Single (v => v.Name == "GetIntPtr"));
		});
	}

	[Test]
	[TestCase (ApplePlatform.MacCatalyst, false)]
	[TestCase (ApplePlatform.iOS, false)]
	[TestCase (ApplePlatform.TVOS, false)]
	[TestCase (ApplePlatform.MacOSX, true)]
	public void HotReloadCompatibleBuildLeavesReloadableAssemblyUnmodified (ApplePlatform platform, bool isCoreCLR)
	{
		// A [Field] symbol referenced via Dlfcn, exactly the shape that compatibility-mode inlining
		// (the default for Debug builds) would normally collect as a native symbol.
		var code = @"
		using System;
		using CoreAnimation;
		using Foundation;
		using ObjCRuntime;

		class MyClass : NSObject {
			[Field (""NativeSymbol"", ""__Internal"")]
			static IntPtr MyField {
				get { return Dlfcn.GetIntPtr (0, ""NativeSymbol""); }
			}
		}";

		// The test assembly is a reloadable (Copy) assembly, and we're building for Hot Reload compatibility,
		// so the assembly must be left byte-unmodified: no inlining, no generated P/Invokes or helper members.
		// Use compatibility mode (the Debug default) so the referenced [Field] symbol is eligible for collection.
		var modified = AssertPrepare (platform, isCoreCLR, RegistrarMode.Dynamic, code, out var assemblyDefinition, out var preparer, hotReloadCompatibleBuild: true, testAssemblyTrimMode: "copy", inlineDlfcnMethods: "compatibility");
		Assert.That (modified, Is.False, "The reloadable assembly must not be modified.");

		var type = assemblyDefinition.MainModule.Types.Single (v => v.Name == "MyClass");

		// The Dlfcn call must remain an ordinary call into the platform assembly's Dlfcn type (not inlined into a
		// generated P/Invoke in the user assembly).
		var call = type.Methods
			.Where (m => m.HasBody)
			.SelectMany (m => m.Body.Instructions)
			.FirstOrDefault (v => v.OpCode == OpCodes.Call && v.Operand is MethodReference mr && mr.DeclaringType.Name == "Dlfcn" && mr.Name == "GetIntPtr");
		Assert.That (call, Is.Not.Null, "Expected the original call to Dlfcn.GetIntPtr to be preserved.");

		// No generated Dlfcn helper type should have been added to the user assembly.
		var generatedDlfcn = assemblyDefinition.MainModule.GetTypes ().FirstOrDefault (v => v.Name == "Dlfcn");
		Assert.That (generatedDlfcn, Is.Null, "No Dlfcn helper type should have been generated in the reloadable assembly.");

		// Even though we didn't inline the call, the referenced native symbol must still be collected so the
		// native linker keeps it alive (GenerateReferencesStep turns RequiredSymbols into native references).
		var requiredSymbol = preparer.Configuration.DerivedLinkContext.RequiredSymbols.Find ("NativeSymbol");
		Assert.That (requiredSymbol, Is.Not.Null, "The referenced native symbol must still be collected for native linking.");
	}

	[Test]
	public void HotReloadCompatibleBuildSkipsUntrimmedPlatformAssembly ()
	{
		var code = @"
		using System;
		using Foundation;
		using ObjCRuntime;

		class MyClass : NSObject {
			[Field (""NativeSymbol"", ""__Internal"")]
			static IntPtr MyField => Dlfcn.GetIntPtr (0, ""NativeSymbol"");
		}";

		// Strict mode ensures the platform's Dlfcn calls would collect symbols if it were scanned.
		var modified = AssertPrepare (ApplePlatform.iOS, false, RegistrarMode.Dynamic, code, out _, out var preparer,
			hotReloadCompatibleBuild: true, testAssemblyTrimMode: "copy", inlineDlfcnMethods: "strict",
			extraConfig: "AreAnyAssembliesTrimmed=false\nPublishTrimmed=false");

		Assert.That (modified, Is.False, "The user assembly should remain unmodified.");
		var fieldSymbols = preparer.Configuration.DerivedLinkContext.RequiredSymbols
			.Where (v => v.Type == SymbolType.Field)
			.Select (v => v.Name);
		Assert.That (fieldSymbols, Is.EquivalentTo (new [] { "NativeSymbol" }), "Only the user assembly's Dlfcn symbol should be collected.");
	}

	[Test]
	public void HotReloadCompatibleBuildStillInlinesTrimmedPlatformAssembly ()
	{
		AssertPrepare (ApplePlatform.iOS, false, RegistrarMode.Dynamic, "class MyClass {}", out _, out var preparer,
			hotReloadCompatibleBuild: true, testAssemblyTrimMode: "copy", inlineDlfcnMethods: "strict");

		Assert.That (preparer.Configuration.InlinedDlfcnFields.TryGetValue ("Microsoft.iOS", out var inlinedFields), Is.True);
		Assert.That (inlinedFields, Is.Not.Empty, "The linked platform assembly should still be inlined.");
	}

	[Test]
	public void NonHotReloadBuildStillInlinesUntrimmedPlatformAssembly ()
	{
		AssertPrepare (ApplePlatform.iOS, false, RegistrarMode.Dynamic, "class MyClass {}", out _, out var preparer,
			testAssemblyTrimMode: "copy", inlineDlfcnMethods: "strict",
			extraConfig: "AreAnyAssembliesTrimmed=false\nPublishTrimmed=false");

		Assert.That (preparer.Configuration.InlinedDlfcnFields.TryGetValue ("Microsoft.iOS", out var inlinedFields), Is.True);
		Assert.That (inlinedFields, Is.Not.Empty, "The platform assembly should still be inlined when Hot Reload compatibility is disabled.");
	}
}
