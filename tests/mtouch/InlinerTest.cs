using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Tests;

namespace Xamarin.Linker {

	public static partial class Extensions {

		// note: direct check, no inheritance
		public static bool Is (this TypeReference type, string @namespace, string name)
		{
			return ((type is not null) && (type.Name == name) && (type.Namespace == @namespace));
		}
	}

	public abstract class InlinerTest {

		// note: not every candidate should be handled by the linker 
		// most of them are _naturally_ eliminated by not being used/marked
		static bool DisplayCandidates = false;

		protected abstract string Assembly { get; }

		AssemblyDefinition assembly;

		protected virtual AssemblyDefinition AssemblyDefinition {
			get {
				if (assembly is null)
					assembly = AssemblyDefinition.ReadAssembly (Assembly);
				return assembly;
			}
		}

		protected string ListMethods (HashSet<string> h)
		{
			string result = Path.GetFileName (Assembly);
			if (h.Count == 0)
				return $" {result}: success";
			return result + "\n" + h.Aggregate ((arg1, arg2) => arg1 + "\n" + arg2);
		}

		bool IsCandidateForInlining (MethodDefinition m)
		{
			// candidates must be methods
			if (m.IsConstructor)
				return false;
			// must have a body (IL)
			if (!m.HasBody)
				return false;
			// must be static or not virtual (can't be overrriden)
			if (!m.IsStatic && m.IsVirtual)
				return false;
			// the body must not have exception handlers or variables
			var b = m.Body;
			return !(b.HasExceptionHandlers || b.HasVariables || b.InitLocals);
		}

		/// <summary>
		/// We look for candidates, without parameters, that only do `return true;`.
		/// E.g. Such a static method can be inlined by replacing the `call` with a `ldc.i4.1` instruction
		/// We must ensure that the list of methods we inline remains unchanged in the BCL we ship.
		/// </summary>
		protected void NoParameterReturnOnlyConstant (Code code, HashSet<string> list)
		{
			if (DisplayCandidates)
				Console.WriteLine ($"### NoParameterReturnOnlyConstant {code}: {Path.GetFileName (Assembly)}");

			var ad = AssemblyDefinition.ReadAssembly (Assembly);
			foreach (var t in ad.MainModule.Types) {
				if (!t.HasMethods)
					continue;
				foreach (var m in t.Methods) {
					if (!IsCandidateForInlining (m))
						continue;
					if (m.HasParameters)
						continue;
					var b = m.Body;
					if (b.Instructions.Count != 2)
						continue;
					var ins = b.Instructions [0];
					if (code == ins.OpCode.Code) {
						var s = m.ToString ();
						list.Remove (s);
						if (DisplayCandidates)
							Console.WriteLine ($"* `{s}`");
					}
				}
			}
		}

		/// <summary>
		/// We look for candidates, without parameters and return value, that does nothing (only `ret`).
		/// E.g. Such a static method can be inlined by replacing the `call` with a nop` instruction.
		/// We must ensure that the list of methods we inline remains unchanged in the BCL we ship.
		/// </summary>
		protected void NoParameterNoReturnNoCode (HashSet<string> list)
		{
			if (DisplayCandidates)
				Console.WriteLine ($"### ReturnOnly: {Path.GetFileName (Assembly)}");

			foreach (var t in AssemblyDefinition.MainModule.Types) {
				if (!t.HasMethods)
					continue;
				foreach (var m in t.Methods) {
					if (!IsCandidateForInlining (m))
						continue;
					if (m.HasParameters)
						continue;
					if (!m.ReturnType.Is ("System", "Void"))
						continue;
					var b = m.Body;
					if (b.Instructions.Count == 1) {
						var s = m.ToString ();
						list.Remove (s);
						if (DisplayCandidates)
							Console.WriteLine ($"* `{s}`");
					}
				}
			}
		}
	}

	[TestFixture]
	public class MscorlibInlinerTest : InlinerTest {

		protected override string Assembly {
			get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "mono", "Xamarin.iOS", "mscorlib.dll"); }
		}

		[Test]
		public void True ()
		{
			// list MUST be kept in sync with InlinerSubStep.cs
			var h = new HashSet<string> {
				"System.Boolean System.Security.SecurityManager::CheckElevatedPermissions()",
			};
			NoParameterReturnOnlyConstant (Code.Ldc_I4_1, h);
			Assert.That (h, Is.Empty, ListMethods (h));
		}

		[Test]
		public void Nop ()
		{
			// this list MUST be kept in sync with InlinerSubStep.cs
			var h = new HashSet<string> {
				"System.Void System.Security.SecurityManager::EnsureElevatedPermissions()",
			};
			NoParameterNoReturnNoCode (h);
			Assert.That (h, Is.Empty, ListMethods (h));
		}
	}
}
