using System.Threading.Tasks;
using NUnit.Framework;
using Mono.Cecil;
using System.Linq;
using System.IO;
using Mono.Cecil.Cil;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	[TestFixture]
	public class ClassHandleUnitTests {
		[Test]
		public async Task ChangeClassHandleUsage ()
		{
			var pathToModule = await TestRunning.BuildTemporaryLibrary (@"
using System;
using Foundation;
public class Handlicious : NSObject {
    public Handlicious (IntPtr p) : base (p) { }
    public IntPtr DoAThing () {
        return ClassHandle;
    }
}
");

			var editedModule = ReworkerHelper.GetReworkedModule (pathToModule);
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

		[Test]
		public async Task ChangeHandleUsage ()
		{
			var pathToModule = await TestRunning.BuildTemporaryLibrary (@"
using System;
using Foundation;
public class Handliciousness : NSObject {
    public Handliciousness (IntPtr p) : base (p) { }
    public IntPtr DoAThing () {
        return Handle;
    }
}
");

			var editedModule = ReworkerHelper.GetReworkedModule (pathToModule);
			Assert.IsNotNull (editedModule, "edited module is null (oops)");

			var type = editedModule!.Types.First (t => t.Name == "Handliciousness");

			var method = type.Methods.First (m => m.Name == "DoAThing");

			for (int i = 0; i < method.Body.Instructions.Count; i++) {
				var instr = method.Body.Instructions [i];
				if (instr.Operand is null)
					continue;
				if (instr.Operand.ToString ()!.Contains ("get_Handle")) {
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

