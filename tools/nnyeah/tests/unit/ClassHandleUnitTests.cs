using System.Threading.Tasks;
using NUnit.Framework;
using Mono.Cecil;
using System.Linq;
using System.IO;
using Mono.Cecil.Cil;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	[TestFixture]
	public class ClassHandleUnitTests {
		static ReaderParameters ReaderParameters {
			get {
				var legacyPlatform = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
				var netPlatform = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);

				// We must use a resolver here as the types will be Resolved()'ed later
				return new ReaderParameters { AssemblyResolver = new NNyeahAssemblyResolver (legacyPlatform, netPlatform) };
			}
		}

		static async Task<string> CompileTypeForTest (string code)
		{
			return await TestRunning.BuildTemporaryLibrary (code);
		}

		Reworker? CreateReworker (string modulePath)
		{
			var readerParameters = ReaderParameters;
			var resolver = (readerParameters.AssemblyResolver as NNyeahAssemblyResolver)!;
			var stm = new FileStream (modulePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var moduleToEdit = ModuleDefinition.ReadModule (stm, readerParameters);
			var typeAndModuleMap = new TypeAndModuleMap (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS),
				Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), resolver);

			var moduleContainer = new ModuleContainer (moduleToEdit, typeAndModuleMap.XamarinModule,
				typeAndModuleMap.MicrosoftModule);

			return Reworker.CreateReworker (stm, moduleContainer, typeAndModuleMap.TypeMap);
		}

		ModuleDefinition? GetReworkedModule (string inModule)
		{
			var reworker = CreateReworker (inModule);
			if (reworker is null)
				return null;

			var outStm = new MemoryStream ();
			reworker.Rework (outStm);
			outStm.Seek (0, SeekOrigin.Begin);
			return ModuleDefinition.ReadModule (outStm, ReaderParameters);
		}

		[Test]
		public async Task ChangeClassHandleUsage ()
		{
			var pathToModule = await CompileTypeForTest (@"
using System;
using Foundation;
public class Handlicious : NSObject {
    public Handlicious (IntPtr p) : base (p) { }
    public IntPtr DoAThing () {
        return ClassHandle;
    }
}
");

			var editedModule = GetReworkedModule (pathToModule);
			Assert.IsNotNull (editedModule, "edited module is null (oops)");

			var type = editedModule!.Types.First (t => t.Name == "Handlicious");

			var method = type.Methods.First (m => m.Name == "DoAThing");

			for (int i = 0; i < method.Body.Instructions.Count; i++) {
				var instr = method.Body.Instructions [i];
				if (instr.Operand is null)
					continue;
				if (instr.Operand.ToString ()!.Contains ("get_ClassHandle")) {
					var nextInstr = method.Body.Instructions [i + 1];
					Assert.AreEqual (OpCodes.Call, nextInstr.OpCode, "wrong opcode");
					Assert.IsTrue (nextInstr.Operand.ToString ()!.Contains ("get_Handle"), "wrong operand");
					return;
				}
			}
			Assert.Fail ("didn't find instruction");
		}

	}
}

