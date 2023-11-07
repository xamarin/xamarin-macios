using System;

using Mono.Cecil;
using Mono.Cecil.Cil;

using NUnit.Framework;

namespace GeneratorTests {
	public static class Asserts {
		public static void DoesNotThrowExceptions (MethodReference method, string message)
		{
			var instructions = method.Resolve ().Body.Instructions;
			foreach (var ins in instructions) {
				if (ins.OpCode.FlowControl != FlowControl.Throw)
					continue;

				Assert.Fail ($"The method '{method.FullName}' unexpectedly throws an exception at offset {ins.Offset}: {message}\n\t{string.Join ("\n\t", instructions)}");
			}
		}
	}
}
