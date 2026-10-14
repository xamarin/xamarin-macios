// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoTouch.Tuner;

namespace AssemblyPreparerTests;

public class ClassHandleRewriterStepTests : BaseClass {
	[TestCase (false)]
	[TestCase (true)]
	public void RedirectedClassHandlesAreSaved (bool redirectClassHandles)
	{
		var code = @"
		using Foundation;
		using ObjCRuntime;

		public class MyClass : NSObject {
			static NativeHandle class_ptr = Class.GetHandle (""MyClass"");
			public override NativeHandle ClassHandle => class_ptr;
		}
		";

		using var preparer = CreatePreparer (ApplePlatform.iOS, false, p => {
			p.Registrar = RegistrarMode.Static;
			p.Optimizations.RedirectClassHandles = redirectClassHandles;
		}, code, out var testInfo);
		var context = preparer.Configuration.DerivedLinkContext;
		new LoadAssembliesStep ().Process (context);
		new ClassHandleRewriterStep ().Process (context);

		var assembly = context.GetAssemblies ().Single (v => v.Name.Name == "Test");
		Assert.That (preparer.Configuration.ModifiedAssemblies.Contains (assembly), Is.EqualTo (redirectClassHandles), "Modified user assembly");

		new SaveAssembliesStep ().Process (context);
		using var savedAssembly = AssemblyDefinition.ReadAssembly (testInfo.OutputPath);
		var type = savedAssembly.MainModule.Types.Single (v => v.Name == "MyClass");
		Assert.That (type.Fields.Any (v => v.Name == "class_ptr"), Is.EqualTo (!redirectClassHandles), "Saved class handle field");
		var getter = type.Methods.Single (v => v.Name == "get_ClassHandle");
		var field = getter.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldsfld).Operand as FieldReference;
		Assert.That (field?.DeclaringType.FullName, Is.EqualTo (redirectClassHandles ? "ObjCRuntime.Runtime/ClassHandles" : "MyClass"), "Saved class handle reference");
	}
}
