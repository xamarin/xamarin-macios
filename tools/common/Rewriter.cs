using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Tuner;
using ClassRedirector;
using Mono.Linker;

#nullable enable

namespace ClassRedirector {
#if NET
	public class Rewriter {
		// Rewriter exists to do three things:
		// 0. For each class identified by the static registrar,
		//    write a public field that will contain its class handle and
		//    The fields and static initializer will live in
		//    ObjRuntime.Runtime.ClassHandles
		//   write a static initializer to initialize that field.
		// 1. Remove class_ptr fields and rewrite all usages of it
		//    to the static field in ClassHandles
		// 2. Strip out the static initializer from each class, and if
		//    possible, remove the static initializer itself.
		const string runtimeName = "ObjCRuntime.Runtime";
		const string classHandleName = "ObjCRuntime.Runtime/ClassHandles";
		const string nativeHandleName = "ObjCRuntime.NativeHandle";
		const string initClassHandlesName = "InitializeClassHandles";
		const string setHandleName = "SetHandle";
		const string classPtrName = "class_ptr";
		CSToObjCMap map;
		string pathToXamarinAssembly;
		Dictionary<string, FieldDefinition> csTypeToFieldDef = new Dictionary<string, FieldDefinition> ();
		IEnumerable<AssemblyDefinition> assemblies;
		AssemblyDefinition xamarinAssembly;
		Xamarin.Tuner.DerivedLinkContext derivedLinkContext;

		public Rewriter (CSToObjCMap map, IEnumerable<AssemblyDefinition> assembliesToPatch, Xamarin.Tuner.DerivedLinkContext? derivedLinkContext)
		{
			this.map = map;
			this.assemblies = assembliesToPatch;
			var xasm = assembliesToPatch.Select (assem => assem.MainModule).FirstOrDefault (ContainsNativeHandle)?.Assembly;
			if (xasm is null) {
				throw new Exception ("Unable to find Xamarin assembly.");
			} else {
				xamarinAssembly = xasm;
				pathToXamarinAssembly = xamarinAssembly.MainModule.FileName;
			}
			if (derivedLinkContext is null) {
				throw new Exception ("Rewriter needs a valid derived link context.");
			} else {
				this.derivedLinkContext = derivedLinkContext;
			}
		}

		public string Process ()
		{
			Dictionary<string, FieldDefinition> classMap;
			try {
				// make the fields and static initializer for
				// the fields. If this fails, there have been
				// no changes, so no harm, no foul, just
				// report it. The things that would
				// cause this to fail are heinously
				// catastrophic like not being able to
				// find the type NativeHandle and its
				// attendent methods. How is *anything*
				// supposed to work with that in play?
				classMap = CreateClassHandles ();
			} catch (Exception e) {
				// if this throws, no changes are made to the assemblies
				// so it's safe to log it on the far side.
				return e.Message;
			}
			// The second pass is to fix up all the usages
			// of class_ptr
			PatchClassPtrUsage (classMap);
			return "";
		}

		Dictionary<string, FieldDefinition> CreateClassHandles ()
		{
			var classMap = new Dictionary<string, FieldDefinition> ();
			var module = xamarinAssembly.MainModule;

			var classHandles = LocateClassHandles (module);
			if (classHandles is null) {
				throw new Exception ($"Unable to find {classHandleName} type in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
			}

			var initMethod = classHandles.Methods.FirstOrDefault (m => m.Name == initClassHandlesName);
			if (initMethod is null)
				throw new Exception ($"Unable to find {initClassHandlesName} method in {classHandles.Name}");
			var setHandleMethod = classHandles.Methods.FirstOrDefault (m => m.Name == setHandleName);
			if (setHandleMethod is null)
				throw new Exception ($"Unable to find {setHandleName} method in {classHandles.Name}");

			var processor = initMethod.Body.GetILProcessor ();

			var nativeHandle = LocateNativeHandle (module);
			if (nativeHandle is null) {
				throw new Exception ($"Unable to find {nativeHandleName} in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
			}

			var nativeHandleOpImplicit = FindOpImplicit (nativeHandle);
			if (nativeHandleOpImplicit is null) {
				throw new Exception ($"Unable to find implicit cast in {nativeHandleName}");
			}

			// no work to do
			if (map.Count () == 0)
				return classMap;

			processor.Clear ();
			processor.Append (Instruction.Create (OpCodes.Nop));
			foreach (var nameIndexPair in map) {
				var csName = TrimClassName (nameIndexPair.Key);
				var nameIndex = nameIndexPair.Value;
				var fieldDef = AddPublicStaticField (classHandles, nameIndex.ObjCName, nativeHandle);
				AddInitializer (processor, nameIndex.MapIndex, fieldDef, setHandleMethod);
				classMap [csName] = fieldDef;
			}
			processor.Append (Instruction.Create (OpCodes.Ret));

			MarkForSave (xamarinAssembly);
			return classMap;
		}

		static string TrimClassName (string str)
		{
			var index = str.LastIndexOf (", ");
			return index < 0 ? str : str.Remove (index);
		}

		MethodDefinition? FindOpImplicit (TypeDefinition nativeHandle)
		{
			return nativeHandle.Methods.FirstOrDefault (m => m.Name == "op_Implicit" && m.ReturnType == nativeHandle &&
				m.Parameters.Count == 1 && m.Parameters [0].ParameterType == nativeHandle.Module.TypeSystem.IntPtr);
		}

		void AddInitializer (ILProcessor il, int index, FieldDefinition staticFieldTarget, MethodDefinition setHandleMethod)
		{
			il.Append (Instruction.Create (OpCodes.Ldsflda, staticFieldTarget));
			il.Append (Instruction.Create (OpCodes.Stloc_1));
			il.Append (Instruction.Create (OpCodes.Ldc_I4, index));
			il.Append (Instruction.Create (OpCodes.Ldloc_1));
			il.Append (Instruction.Create (OpCodes.Conv_U));
			il.Append (Instruction.Create (OpCodes.Ldarg_0));
			il.Append (Instruction.Create (OpCodes.Call, setHandleMethod));
			il.Append (Instruction.Create (OpCodes.Ldc_I4, 0));
			il.Append (Instruction.Create (OpCodes.Conv_U));
			il.Append (Instruction.Create (OpCodes.Stloc_1));
		}

		FieldDefinition AddPublicStaticField (TypeDefinition inType, string fieldName, TypeReference fieldType)
		{
			var fieldDef = new FieldDefinition (fieldName, FieldAttributes.Public | FieldAttributes.Static, fieldType);
			inType.Fields.Add (fieldDef);
			return fieldDef;
		}

		bool ContainsNativeHandle (ModuleDefinition module)
		{
			return LocateNativeHandle (module) is not null;
		}

		TypeDefinition? LocateNativeHandle (ModuleDefinition module)
		{
			return AllTypes (module).FirstOrDefault (t => t.FullName == nativeHandleName);
		}

		TypeDefinition? LocateClassHandles (ModuleDefinition module)
		{
			return AllTypes (module).FirstOrDefault (t => t.FullName == classHandleName);
		}

		void PatchClassPtrUsage (Dictionary<string, FieldDefinition> classMap)
		{
			foreach (var assem in assemblies) {
				var module = assem.MainModule;
				if (PatchClassPtrUsage (classMap, module)) {
					MarkForSave (assem);
				}
			}
		}

		// returns true if the assembly was changed.
		bool PatchClassPtrUsage (Dictionary<string, FieldDefinition> classMap, ModuleDefinition module)
		{
			var dirty = false;
			foreach (var cl in AllTypes (module)) {
				// generic classes leave class_ptr references
				// behind that we (apparently) can't find
				if (cl.HasGenericParameters) {
					continue;
				}
				if (classMap.TryGetValue (cl.FullName, out var classPtrField)) {
					var madeChanges = PatchClassPtrUsage (cl, classPtrField);
					dirty = dirty || madeChanges;
				}
			}
			return dirty;
		}

		bool PatchClassPtrUsage (TypeDefinition cl, FieldDefinition classPtrField)
		{
			var class_ptr = cl.Fields.FirstOrDefault (f => f.Name == classPtrName);
			if (class_ptr is null) {
				return false;
			}
			// step 1: remove the field
			cl.Fields.Remove (class_ptr);
			// step 2: remove init code from cctor
			RemoveCCtorInit (cl, class_ptr);
			// step 3: patch every method
			PatchMethods (cl, class_ptr, classPtrField);
			return true;
		}

		void PatchMethods (TypeDefinition cl, FieldDefinition classPtr, FieldDefinition classPtrField)
		{
			foreach (var method in cl.Methods) {
				PatchMethod (method, classPtr, classPtrField);
			}
		}

		void PatchMethod (MethodDefinition method, FieldDefinition classPtr, FieldDefinition classPtrField)
		{
			var body = method.Body;
			if (body is null)
				return;
			var il = body.GetILProcessor ();
			body.SimplifyMacros ();
			for (var i = 0; i < body.Instructions.Count; i++) {
				var old = body.Instructions [i];
				if (old.OpCode == OpCodes.Ldsfld && old.Operand == classPtr) {
					var @new = Instruction.Create (OpCodes.Ldsfld, method.Module.ImportReference (classPtrField));
					PatchReferences (body, old, @new);
					il.Replace (i, @new);
				}
			}
			body.OptimizeMacros ();
		}

		static void PatchReferences (MethodBody body, Instruction old, Instruction @new)
		{
			foreach (var instruction in body.Instructions) {
				if (instruction.Operand is Instruction target && target == old) {
					instruction.Operand = @new;
				} else if (instruction.Operand is Instruction [] targets) {
					for (int i = 0; i < targets.Length; i++) {
						if (targets [i] == old) {
							@targets [i] = @new;
						}
					}
				}
			}
		}

		void RemoveCCtorInit (TypeDefinition cl, FieldDefinition class_ptr)
		{
			var cctor = cl.Methods.FirstOrDefault (m => m.Name == ".cctor");
			if (cctor is null)
				return; // no static init - should never happen, but we can deal.

			var il = cctor.Body.GetILProcessor ();
			Instruction? stsfld = null;
			int i = 0;
			for (; i < il.Body.Instructions.Count; i++) {
				var instr = il.Body.Instructions [i];
				// look for
				// stsfld class_ptr
				if (instr.OpCode == OpCodes.Stsfld && instr.Operand == class_ptr) {
					stsfld = instr;
					break;
				}
			}
			if (stsfld is null)
				return;

			// if we see:
			// ldstr "any"
			// call ObjCRuntime.GetClassHandle
			// stsfld class_ptr
			// Then we can remove all of those instructions

			var isGetClassHandle = IsGetClassHandle (il, i - 1);
			var isLdStr = IsLdStr (il, i - 2);

			if (isGetClassHandle && isLdStr) {
				il.RemoveAt (i);
				il.RemoveAt (i - 1);
				il.RemoveAt (i - 2);
			} else if (isGetClassHandle) {
				// don't know how the string got on the stack, so at least get rid of the
				// call to GetClassHandle by nopping it out. This still leaves the string on
				// the stack, so pop it.
				il.Replace (il.Body.Instructions [i - 1], Instruction.Create (OpCodes.Nop));
				// can't remove all three, so just pop the IntPtr.
				il.Replace (il.Body.Instructions [i], Instruction.Create (OpCodes.Pop));
			} else {
				// can't remove all three, so just pop the IntPtr.
				il.Replace (il.Body.Instructions [i], Instruction.Create (OpCodes.Pop));
			}

			// if we're left with exactly 1 instruction and it's a return,
			// then we can get rid of the entire method
			if (cctor.Body.Instructions.Count == 1) {
				if (cctor.Body.Instructions.Last ().OpCode == OpCodes.Ret)
					cl.Methods.Remove (cctor);
			}

			// if we're left with exactly 2 instructions and the first is a no-op
			// and the last is a return, then we can get rid of the entire method
			if (cctor.Body.Instructions.Count == 2) {
				if (cctor.Body.Instructions.Last ().OpCode == OpCodes.Ret &&
					cctor.Body.Instructions.First ().OpCode == OpCodes.Nop)
					cl.Methods.Remove (cctor);
			}
		}

		bool IsGetClassHandle (ILProcessor il, int index)
		{
			if (index < 0)
				return false;
			var instr = il.Body.Instructions [index]!;
			var operand = instr.Operand?.ToString () ?? string.Empty;
			return instr.OpCode == OpCodes.Call && operand.Contains ("Class::GetHandle");
		}

		bool IsLdStr (ILProcessor il, int index)
		{
			if (index < 0)
				return false;
			var instr = il.Body.Instructions [index]!;
			return instr.OpCode == OpCodes.Ldstr;
		}

		IEnumerable<TypeDefinition> AllTypes (ModuleDefinition module)
		{
			foreach (var type in module.Types) {
				yield return type;
				foreach (var t in AllTypes (type))
					yield return t;
			}
		}

		IEnumerable<TypeDefinition> AllTypes (TypeDefinition type)
		{
			if (type.HasNestedTypes) {
				foreach (var t in type.NestedTypes) {
					yield return t;
					foreach (var nt in AllTypes (t))
						yield return nt;
				}
			}
		}

		void MarkForSave (AssemblyDefinition assembly)
		{
			var annotations = derivedLinkContext.Annotations;
			if (!annotations.HasAction (assembly)) {
				annotations.SetAction (assembly, AssemblyAction.Link);
			} else {
				var action = annotations.GetAction (assembly);
				if (action != AssemblyAction.Link) {
					annotations.SetAction (assembly, AssemblyAction.Save);
				}
			}
		}
	}
#endif
}

