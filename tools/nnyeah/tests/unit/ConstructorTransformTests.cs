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
        static ReaderParameters ReaderParameters {
            get {
                var legacyPlatform = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
                var netPlatform = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);
                
                // We must use a resolver here as the types will be Resolved()'ed later
                return new ReaderParameters { AssemblyResolver = new NNyeahAssemblyResolver (legacyPlatform, netPlatform) };
            }
        }

        static async Task<TypeDefinition> CompileTypeForTest (string code)
        {
            string lib = await TestRunning.BuildTemporaryLibrary (code);
            var module = ModuleDefinition.ReadModule (lib, ReaderParameters);
            return module.GetType ("Foo");
        }

        static ConstructorTransforms CreateTestTransform (TypeDefinition type) 
        {
            var legacyPlatform = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
            var netPlatform = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);
            
            var legacyAssembly = ModuleDefinition.ReadModule (legacyPlatform, ReaderParameters);
            var netAssembly = ModuleDefinition.ReadModule (netPlatform, ReaderParameters);

            var legacyNSObject = legacyAssembly.Types.First (t => t.FullName == "Foundation.NSObject");

            return new ConstructorTransforms (
                netAssembly.GetType("ObjCRuntime.NativeHandle"),
                legacyNSObject.Methods.First (m => m.FullName == "System.Void Foundation.NSObject::.ctor(System.IntPtr)"),
                legacyNSObject.Methods.First (m => m.FullName == "System.Void Foundation.NSObject::.ctor(System.IntPtr,System.Boolean)"),
                warningIssued: null, transformed: null
            );
        }
        
        [Test]
        public async Task DerivedFromNSObject ()
        {
            var type = await CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
    public Foo (IntPtr p) : base (p) { }
}");
            CreateTestTransform (type).ReworkAsNeeded (type);

            var ctor = type.GetConstructors ().First ();
            Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters[0].ParameterType.FullName);
            Assert.AreEqual (ctor.FullName, "System.Void Foo::.ctor(ObjCRuntime.NativeHandle)");
        }

        [Test]
        public async Task DerivedFromNSObjectWithBool ()
        {
            var type = await CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
    public Foo (IntPtr p, bool b) : base (p, b) { }
}");
            CreateTestTransform (type).ReworkAsNeeded (type);

            var ctor = type.GetConstructors ().First ();
            Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters[0].ParameterType.FullName);
            Assert.AreEqual ("System.Boolean", ctor.Parameters[1].ParameterType.FullName);
        }

        // [Test] - https://github.com/xamarin/xamarin-macios/issues/15133
        public async Task InvokeCtor ()
        {
            var type = await CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
    public Foo (IntPtr p) : base (p) { }
    public Foo Create () => new Foo (IntPtr.Zero);
}");
            CreateTestTransform (type).ReworkAsNeeded (type);

            var ctor = type.GetConstructors ().First ();
            Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters[0].ParameterType.FullName);
        }

        [Test]
        public async Task RefuseToProcessCtorWithBehavior ()
        {
            var type = await CompileTypeForTest (@"
using System;
using Foundation;
public class Foo : NSObject {
    public Foo (IntPtr p) : base (p) { Console.Error.WriteLine (typeof(int)); }
}");
            Assert.Throws<ConversionException> (() => CreateTestTransform (type).ReworkAsNeeded (type));
        }

        [Test]
        public async Task DerivedFromNSObjectDerived ()
        {
            var type = await CompileTypeForTest (@"
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

            var ctor = type.GetConstructors().First();
            Assert.AreEqual ("ObjCRuntime.NativeHandle", ctor.Parameters[0].ParameterType.FullName);
        }

        [Test]
        public async Task NotNSObjectJustIntPtr ()
        {
            var type = await CompileTypeForTest (@"
using System;
using Foundation;
public class Foo {
    public Foo (IntPtr p) { }
}");

            CreateTestTransform (type).ReworkAsNeeded (type);

            var ctor = type.GetConstructors ().First ();
            Assert.AreEqual ("System.IntPtr", ctor.Parameters[0].ParameterType.FullName);
        }

        void AssertInstruction (Instruction instruction, string value)
        {
            // Instruction.ToString() has a offset such as "IL_0010:" so use contain to sidestep
            Assert.True (instruction.ToString ().Contains (value), $"Instruction was: {instruction}");
        }

        void AssertInstructionBeforeNewobj (IList<Instruction> instructions, string instructionValue)
        {
            foreach (var createInstruction in instructions.Where (i => i.OpCode.Code == Mono.Cecil.Cil.Code.Newobj))
            {
                var createInstructionIndex = instructions.IndexOf (createInstruction);
                var instructionBeforeCreate = instructions[createInstructionIndex - 1];
                AssertInstruction (instructionBeforeCreate, instructionValue);
            }
        }

        [Test]
        public async Task DerivedFromNSObjectInvocation ()
        {
            var type = await CompileTypeForTest (@"
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
        }

        [Test]
        public async Task NotDerivedFromNSObjectInvocation ()
        {
            var type = await CompileTypeForTest (@"
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
        }

        [Test]
        public async Task DerivedFromNSObjectMultipleInvocations ()
        {
            var type = await CompileTypeForTest (@"
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
     }

        [Test]
        public async Task DerivedFromNSObjectInvocationWithBool ()
        {
            var type = await CompileTypeForTest (@"
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

            var createInstruction = instructions.First (i => i.OpCode.Code == Mono.Cecil.Cil.Code.Newobj);
            var createInstructionIndex = instructions.IndexOf (createInstruction);
            
            AssertInstruction (instructions[createInstructionIndex - 3], "stloc");
            AssertInstruction (instructions[createInstructionIndex - 2], "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
            AssertInstruction (instructions[createInstructionIndex - 1], "ldloc");
        } 
    }
}
