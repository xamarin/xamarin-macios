using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Cecil.Tests {
	public partial class SetHandleTest {
		static string GetLocation (MethodDefinition? method)
		{
			if (method?.DebugInformation?.HasSequencePoints == true) {
				var seq = method.DebugInformation.SequencePoints [0];
				return seq.Document.Url + ":" + seq.StartLine + ": ";
			}
			return string.Empty;
		}

		static bool VerifyMethod (MethodDefinition method, IList<Instruction> instructions, out string? reason)
		{
			reason = null;

			foreach (var instr in instructions) {
				if (instr.OpCode != OpCodes.Call)
					continue;

				var target = instr.Operand as MethodReference;
				if (target is null) {
					reason = $"Call instructions calling null?";
					return false;
				}
				if (target.Name != "set_Handle")
					continue;

				// A class can call its own Handle property
				if (target.DeclaringType == method.DeclaringType)
					continue;

				if (target.DeclaringType.Is ("Foundation", "NSObject")) {
					reason = $"Call to NSObject.Handle setter.";
					return false;
				}

				if (target.DeclaringType.Is ("ObjCRuntime", "DisposableObject")) {
					reason = $"Call to DisposableObject.Handle setter.";
					return false;
				}

				Console.WriteLine ($"Another Handle property? {target.DeclaringType} called in {method.RenderMethod ()}");
			}

			return true;
		}

		[Test]
		public void NobodyCallsHandleSetter ()
		{
			Configuration.IgnoreIfAnyIgnoredPlatforms ();

			var failures = new Dictionary<string, FailureWithMessageAndLocation> ();
			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				foreach (var method in info.Assembly.EnumerateMethods (v => v.HasBody)) {
					if (!VerifyMethod (method, method.Body.Instructions, out var failureReason)) {
						var msg = $"{method.RenderMethod ()} Failed NSObject.Handle setter verification: {failureReason}";
						// Uncomment this to make finding and fixing known failures easier
						// Console.WriteLine ($"{GetLocation (method)}{msg}");
						failures [method.RenderMethod ()] = new FailureWithMessageAndLocation (msg, GetLocation (method));
					}
				}
			}

			Helper.AssertFailures (failures,
				knownFailuresNobodyCallsHandleSetter,
				nameof (knownFailuresNobodyCallsHandleSetter),
				"Don't call NSObject.Handle's setter directly, use InitializeHandle instead.", (v) => $"{v.Location}: {v.Message}");
		}
	}
}
