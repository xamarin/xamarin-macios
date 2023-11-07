using System.Threading.Tasks;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Microsoft.MaciOS.Nnyeah.Tests {

	[TestFixture]
	public class ConstructorTransformTests {
		static ConstructorTransforms CreateTestTransform (TypeDefinition type)
		{
			var legacyPlatform = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
			var netPlatform = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);

			var readerParams = ReworkerHelper.ReaderParameters;
			var legacyAssembly = ModuleDefinition.ReadModule (legacyPlatform, readerParams);
			var netAssembly = ModuleDefinition.ReadModule (netPlatform, readerParams);

			var legacyNSObject = legacyAssembly.Types.First (t => t.FullName == "Foundation.NSObject");

			var nativeHandle = netAssembly.GetType ("ObjCRuntime.NativeHandle");
			return new ConstructorTransforms (
				nativeHandle,
				netAssembly.TypeSystem.Boolean,
				nativeHandle.Resolve ().GetMethods ().First (m => m.FullName == "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)"),
				warningIssued: null, transformed: null
			);
		}

		[Test]
		public async Task DerivedFromNSObject ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p) : base (p) { }
}");
			CreateTestTransform (type).ReworkAsNeeded (type);

			var ctor = type.GetConstructors ().First ();
			Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters [0].ParameterType.FullName);
			Assert.AreEqual (ctor.FullName, "System.Void Foo::.ctor(ObjCRuntime.NativeHandle)");
		}

		[Test]
		public async Task DerivedFromNSObjectWithBool ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p, bool b) : base (p, b) { }
}");
			CreateTestTransform (type).ReworkAsNeeded (type);

			var ctor = type.GetConstructors ().First ();
			Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters [0].ParameterType.FullName);
			Assert.AreEqual ("System.Boolean", ctor.Parameters [1].ParameterType.FullName);
		}

		// [Test] - https://github.com/xamarin/xamarin-macios/issues/15133
		public async Task InvokeCtor ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p) : base (p) { }
	public Foo Create () => new Foo (IntPtr.Zero);
}");
			CreateTestTransform (type).ReworkAsNeeded (type);

			var ctor = type.GetConstructors ().First ();
			Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters [0].ParameterType.FullName);
		}

		[Test]
		public async Task DerivedFromNSObjectDerived ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class BaseFoo : NSObject {
	public BaseFoo (IntPtr p) : base (p) { }
}
public class Foo : BaseFoo {
	public Foo (IntPtr p) : base (p) { }
}
");
			CreateTestTransform (type).ReworkAsNeeded (type);

			var ctor = type.GetConstructors ().First ();
			Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters [0].ParameterType.FullName);
		}

		[Test]
		public async Task NotNSObjectJustIntPtr ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo {
	public Foo (IntPtr p) { }
}");

			CreateTestTransform (type).ReworkAsNeeded (type);

			var ctor = type.GetConstructors ().First ();
			Assert.AreEqual ("System.IntPtr", ctor.Parameters [0].ParameterType.FullName);
		}

		void AssertInstruction (Instruction instruction, string value)
		{
			// Instruction.ToString() has a offset such as "IL_0010:" so use contain to sidestep
			Assert.True (instruction.ToString ().Contains (value), $"Instruction was: {instruction}");
		}

		bool IsNewObjInstruction (Instruction i) => i.OpCode.Code == Mono.Cecil.Cil.Code.Newobj;

		void AssertInstructionBeforeNewobj (IList<Instruction> instructions, string instructionValue)
		{
			foreach (var createInstruction in instructions.Where (IsNewObjInstruction)) {
				var createInstructionIndex = instructions.IndexOf (createInstruction);
				var instructionBeforeCreate = instructions [createInstructionIndex - 1];
				AssertInstruction (instructionBeforeCreate, instructionValue);
			}
		}

		void AssertNativeHandleCtorCalled (IList<Instruction> instructions)
		{
			foreach (var createInstruction in instructions.Where (IsNewObjInstruction)) {
				// Some calls might have ,System.Boolean suffix, so don't add final ')'
				AssertInstruction (createInstruction, "::.ctor(ObjCRuntime.NativeHandle");
			}
		}

		void AssertIntPtrCtorCalled (IList<Instruction> instructions)
		{
			foreach (var createInstruction in instructions.Where (IsNewObjInstruction)) {
				AssertInstruction (createInstruction, "::.ctor(System.IntPtr)");
			}
		}

		[Test]
		public async Task DerivedFromNSObjectInvocation ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p) : base (p) { }
	public static Foo Create () {
		var ptr = IntPtr.Zero;
		return new Foo (ptr);
	}
}
");
			CreateTestTransform (type).ReworkAsNeeded (type);
			var instructions = type.GetMethods ().First (m => m.Name == "Create").Body.Instructions;
			AssertInstructionBeforeNewobj (instructions, "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
			AssertNativeHandleCtorCalled (instructions);
		}

		[Test]
		public async Task CallingAnotherIntPtr ()
		{
			var pathToModule = await TestRunning.BuildTemporaryLibrary (@"
using System;
using Foundation;
	public class Buzz : NSObject {
		public Buzz (IntPtr p) : base (p) { } 
	}
	public class Foo {
		Buzz f;
		public Foo () {
			f = new Buzz (IntPtr.Zero); 
		}
	}
");
			// As this uses remapping called from Reworker, we have to call the entire machinery
			var editedModule = ReworkerHelper.GetReworkedModule (pathToModule);
			var type = editedModule!.GetType ("Foo");

			var instructions = type.GetConstructors ().First ().Body.Instructions;
			AssertInstructionBeforeNewobj (instructions, "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
			AssertNativeHandleCtorCalled (instructions);
		}

		[Test]
		public async Task PrivateClass ()
		{
			var pathToModule = await TestRunning.BuildTemporaryLibrary (@"
using System;
using Foundation;
    class Foo : NSObject {
    	class Bar : NSObject {
			public Bar (IntPtr p) : base (p) { }
		}
    	public NSObject Create () {
			return new Bar (IntPtr.Zero); 
		}
	}
");
			// As this uses remapping called from Reworker, we have to call the entire machinery
			var editedModule = ReworkerHelper.GetReworkedModule (pathToModule);
			var type = editedModule!.GetType ("Foo");
			var instructions = type.GetMethods ().First (m => m.Name == "Create").Body.Instructions;
			AssertInstructionBeforeNewobj (instructions, "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
			AssertNativeHandleCtorCalled (instructions);
		}

		[Test]
		public async Task NotDerivedFromNSObjectInvocation ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
public class Foo {
	public Foo (IntPtr p) { }
	public static Foo Create () {
		var ptr = IntPtr.Zero;
		return new Foo (ptr);
	}
}
");
			CreateTestTransform (type).ReworkAsNeeded (type);

			var instructions = type.GetMethods ().First (m => m.Name == "Create").Body.Instructions;
			// This is the instruction normally before the newobj
			AssertInstructionBeforeNewobj (instructions, "ldloc.0");
			AssertIntPtrCtorCalled (instructions);
		}

		[Test]
		public async Task DerivedFromNSObjectMultipleInvocations ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p) : base (p) { }
	public static Foo Create () {
		var x = new Foo (IntPtr.Zero);
		var y = new Foo (IntPtr.Zero);
		var z = new Foo (IntPtr.Zero);
		return z;
	}
}
");
			CreateTestTransform (type).ReworkAsNeeded (type);
			var instructions = type.GetMethods ().First (m => m.Name == "Create").Body.Instructions;
			AssertInstructionBeforeNewobj (instructions, "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
			AssertNativeHandleCtorCalled (instructions);
		}

		[Test]
		public async Task DerivedFromNSObjectInvocationWithBool ()
		{
			var type = await ReworkerHelper.CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p, bool b) : base (p, b) { }
	public static Foo Create () {
		var ptr = IntPtr.Zero;
		return new Foo (ptr, true);
	}
}
");
			CreateTestTransform (type).ReworkAsNeeded (type);

			var create = type.GetMethods ().First (m => m.Name == "Create");
			var instructions = create.Body.Instructions;

			var createInstruction = instructions.First (IsNewObjInstruction);
			var createInstructionIndex = instructions.IndexOf (createInstruction);

			AssertInstruction (instructions [createInstructionIndex - 3], "stloc");
			AssertInstruction (instructions [createInstructionIndex - 2], "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
			AssertInstruction (instructions [createInstructionIndex - 1], "ldloc");
			AssertNativeHandleCtorCalled (instructions);
		}


		[Test]
		public async Task OneReferenceThanks ()
		{
			var pathToModule = await TestRunning.BuildTemporaryLibrary (@"
using System;
using Foundation;
public class Foo : NSObject {
	public Foo (IntPtr p) : base (p) { }
}");
			var editedModule = ReworkerHelper.GetReworkedModule (pathToModule)!;
			var totalMSModules = editedModule.AssemblyReferences.Count ((nameRef => nameRef.Name == "Microsoft.macOS"));
			Assert.AreEqual (1, totalMSModules, "More than one Microsoft.macOS reference");
		}
	}
}
