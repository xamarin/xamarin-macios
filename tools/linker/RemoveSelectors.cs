using System;
using System.Collections.Generic;

using Mono.Linker;
using Mono.Linker.Steps;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	public class RemoveSelectors : IStep {

		public void Process (LinkContext context)
		{
			var profile = (Profile.Current as BaseProfile);

			AssemblyDefinition assembly;
			if (!context.TryGetLinkedAssembly (profile.ProductAssembly, out assembly))
				return;

			// skip this if we're not linking monotouch, e.g. --linkskip=monotouch
			if (context.Annotations.GetAction (assembly) != AssemblyAction.Link)
				return;

			foreach (TypeDefinition type in assembly.MainModule.Types) {
				ProcessType (type, (DerivedLinkContext) context);
			}
		}

		void ProcessType (TypeDefinition type, DerivedLinkContext context)
		{
			if (type.IsNSObject (context)) {
				ProcessNSObject (type);
			} else if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (nested, context);
			}
		}

		void ProcessNSObject (TypeDefinition type)
		{
			var selectors = PopulateSelectors (type);
			if (selectors is null)
				return;

			foreach (var method in CollectMethods (type))
				CheckSelectorUsage (method, selectors);
			// nested types can also use the selectors, see bug #1516 for Appearance support
			if (type.HasNestedTypes) {
				foreach (TypeDefinition nested in type.NestedTypes) {
					foreach (var nm in CollectMethods (nested))
						CheckSelectorUsage (nm, selectors);
				}
			}

			if (selectors.Count == 0)
				return;

			PatchStaticConstructor (type, selectors);
			RemoveUnusedSelectors (type, selectors);
		}

		static void CheckSelectorUsage (MethodDefinition method, HashSet<FieldDefinition> selectors)
		{
			if (!method.HasBody)
				return;

			foreach (Instruction instruction in method.Body.Instructions) {
				switch (instruction.OpCode.OperandType) {
				case OperandType.InlineTok:
				case OperandType.InlineField:
					var field = (instruction.Operand as FieldReference)?.Resolve ();
					if (field is null)
						continue;

					if (selectors.Contains (field))
						selectors.Remove (field);

					break;
				}
			}
		}

		void PatchStaticConstructor (TypeDefinition type, HashSet<FieldDefinition> selectors)
		{
			var cctor = type.GetTypeConstructor ();
			if (cctor is null || !cctor.HasBody)
				return;

			var instructions = cctor.Body.Instructions;

			for (int i = 0; i < instructions.Count; i++) {
				var instruction = instructions [i];
				if (!IsCreateSelector (instruction, selectors))
					continue;

				instructions.RemoveAt (i--);
				instructions.RemoveAt (i--);
				instructions.RemoveAt (i--);
			}
		}

		bool IsCreateSelector (Instruction instruction, HashSet<FieldDefinition> selectors)
		{
			if (instruction.OpCode != OpCodes.Stsfld)
				return false;

			var field = (instruction.Operand as FieldReference)?.Resolve ();
			if (field is null)
				return false;

			if (!selectors.Contains (field))
				return false;

			instruction = instruction.Previous;
			if (instruction is null)
				return false;

			if (instruction.OpCode != OpCodes.Call)
				return false;

			if (!IsRegisterSelector (instruction.Operand as MethodReference))
				return false;

			instruction = instruction.Previous;
			if (instruction is null)
				return false;

			if (instruction.OpCode != OpCodes.Ldstr)
				return false;

			return true;
		}

		bool IsRegisterSelector (MethodReference method)
		{
			if (method is null)
				return false;

			if (method.Name != "GetHandle" && method.Name != "sel_registerName")
				return false;

			if (!method.DeclaringType.Is (Namespaces.ObjCRuntime, "Selector"))
				return false;

			return true;
		}

		static void RemoveUnusedSelectors (TypeDefinition type, HashSet<FieldDefinition> selectors)
		{
			var fields = type.Fields;

			for (int i = 0; i < fields.Count; i++)
				if (selectors.Contains (fields [i]))
					fields.RemoveAt (i--);
		}

		static HashSet<FieldDefinition> PopulateSelectors (TypeDefinition type)
		{
			if (!type.HasFields)
				return null;

			HashSet<FieldDefinition> selectors = null;

			foreach (FieldDefinition field in type.Fields) {
				if (!IsSelector (field))
					continue;

				if (selectors is null)
					selectors = new HashSet<FieldDefinition> ();

				selectors.Add (field);
			}

			return selectors;
		}

		static bool IsSelector (FieldDefinition field)
		{
			if (!field.IsStatic)
				return false;

			if (!field.FieldType.Is ("System", "IntPtr"))
				return false;

			if (!field.Name.StartsWith ("sel", StringComparison.Ordinal))
				return false;

			return true;
		}

		static IEnumerable<MethodDefinition> CollectMethods (TypeDefinition type)
		{
			if (type.HasMethods) {
				foreach (MethodDefinition method in type.Methods) {
					if (method.IsConstructor && method.IsStatic)
						continue;
					yield return method;
				}
			}
		}
	}
}
