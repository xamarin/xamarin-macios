using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ClassRedirector;
using Mono.Linker;

#nullable enable

namespace ClassRedirector {
#if NET
	public class Rewriter {
		const string runtimeName = "ObjCRuntime.Runtime";
		const string classHandleName = "ObjCRuntime.Runtime/ClassHandles";
		const string mtClassMapName = "ObjCRuntime.Runtime/MTClassMap";
		const string nativeHandleName = "ObjCRuntime.NativeHandle";
		const string initClassHandlesName = "InitializeClassHandles";
		const string classPtrName = "class_ptr";
		CSToObjCMap map;
		string pathToXamarinAssembly;
		Dictionary<string, FieldDefinition> csTypeToFieldDef = new Dictionary<string, FieldDefinition> ();
		IEnumerable<AssemblyDefinition> assemblies;
		AssemblyDefinition xamarinAssembly;
		Xamarin.Tuner.DerivedLinkContext linkContext;

		public Rewriter (CSToObjCMap map, IEnumerable<AssemblyDefinition> assembliesToPatch, Xamarin.Tuner.DerivedLinkContext? linkContext)
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
			if (linkContext is null) {
				throw new Exception ("Rewriter needs a valid link context.");
			} else {
				this.linkContext = linkContext;
			}

		}

		public string Process ()
		{
			Dictionary<string, FieldDefinition> classMap;
			try {
				classMap = CreateClassHandles ();
			} catch (Exception e) {
				// if this throws, no changes are made to the assemblies
				// so it's safe to log it on the far side.
				return e.Message;
			}
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

			var processor = initMethod.Body.GetILProcessor ();

			var mtClassMapDef = LocateMTClassMap (module);
			if (mtClassMapDef is null)
				throw new Exception ($"Unable to find {mtClassMapName} in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");

			var nativeHandle = LocateNativeHandle (module);
			if (nativeHandle is null)
				throw new Exception ($"Unable to find {nativeHandleName} in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");

			var nativeHandleOpImplicit = FindOpImplicit (nativeHandle);
			if (nativeHandleOpImplicit is null)
				throw new Exception ($"Unable to find implicit cast in {nativeHandleName}");

			if (map.Count () == 0)
				return classMap;

			foreach (var nameIndexPair in map) {
				var csName = nameIndexPair.Key;
				var nameIndex = nameIndexPair.Value;
				var fieldDef = AddPublicStaticField (classHandles, nameIndex.ObjCName, nativeHandle);
				AddInitializer (nativeHandleOpImplicit, processor, mtClassMapDef, nameIndex.MapIndex, fieldDef);
				classMap [csName] = fieldDef;
			}

			MarkForSave (xamarinAssembly);
			return classMap;
		}

		MethodDefinition? FindOpImplicit (TypeDefinition nativeHandle)
		{
			return nativeHandle.Methods.FirstOrDefault (m => m.Name == "op_Implicit" && m.ReturnType == nativeHandle &&
				m.Parameters.Count == 1 && m.Parameters [0].ParameterType == nativeHandle.Module.TypeSystem.IntPtr);
		}

		void AddInitializer (MethodReference nativeHandleOpImplicit, ILProcessor il, TypeDefinition mtClassMapDef, int index, FieldDefinition fieldDef)
		{
			// Assuming that we have a method that looks like this:
			// internal static unsafe void InitializeClassHandles (MTClassMap* map)
			// {
			// }
			// We should have a compiled method that looks like this:
			// nop
			// ret
			//
			// For each handle that we define, we should add the following instructions:
			// ldarg.0
			// ldc.i4 index
			// conv.i
			// sizeof ObjCRuntime.Runtime.MTClassMap
			// mul
			// add
			// ldfld ObjCRuntime.Runtime.MTClassMap.handle
			// call ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)
			// stsfld fieldDef
			var handleRef = mtClassMapDef.Fields.First (f => f.Name == "handle");
			var last = il.Body.Instructions.Last ();
			il.InsertBefore (last, Instruction.Create (OpCodes.Ldarg_0));
			il.InsertBefore (last, Instruction.Create (OpCodes.Ldc_I4, index));
			il.InsertBefore (last, Instruction.Create (OpCodes.Conv_I));
			il.InsertBefore (last, Instruction.Create (OpCodes.Sizeof, mtClassMapDef));
			il.InsertBefore (last, Instruction.Create (OpCodes.Mul));
			il.InsertBefore (last, Instruction.Create (OpCodes.Add));
			il.InsertBefore (last, Instruction.Create (OpCodes.Ldfld, handleRef));
			il.InsertBefore (last, Instruction.Create (OpCodes.Call, nativeHandleOpImplicit));
			il.InsertBefore (last, Instruction.Create (OpCodes.Stsfld, fieldDef));
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

		TypeDefinition? LocateMTClassMap (ModuleDefinition module)
		{
			return AllTypes (module).FirstOrDefault (t => t.FullName == mtClassMapName);
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
				if (classMap.TryGetValue (cl.FullName, out var classPtrField)) {
					dirty = true;
					// if this doesn't throw, it will
					// always change the contents of an
					// assembly
					PatchClassPtrUsage (cl, classPtrField);
				}
			}
			return dirty;
		}

		void PatchClassPtrUsage (TypeDefinition cl, FieldDefinition classPtrField)
		{
			var class_ptr = cl.Fields.FirstOrDefault (f => f.Name == classPtrName);
			if (class_ptr is null) {
				throw new Exception ($"Error processing class {cl.FullName} - no {classPtrName} field.");
			}

			// step 1: remove the field
			cl.Fields.Remove (class_ptr);

			// step 2: remove init code from cctor
			RemoveCCtorInit (cl, class_ptr);

			// step 3: patch every method
			PatchMethods (cl, class_ptr, classPtrField);
		}

		void PatchMethods (TypeDefinition cl, FieldDefinition classPtr, FieldDefinition classPtrField)
		{
			foreach (var method in cl.Methods) {
				PatchMethod (method, classPtr, classPtrField);
			}
		}

		void PatchMethod (MethodDefinition method, FieldDefinition classPtr, FieldDefinition classPtrField)
		{
			var il = method.Body.GetILProcessor ();
			for (int i = 0; i < method.Body.Instructions.Count; i++) {
				var instr = method.Body.Instructions [i];
				if (instr.OpCode == OpCodes.Ldsfld && instr.Operand == classPtr) {
					il.Replace (instr, Instruction.Create (OpCodes.Ldsfld, method.Module.ImportReference (classPtrField)));
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
			var annotations = linkContext.Annotations;
			var action = annotations.GetAction (assembly);
			if (action == AssemblyAction.Copy)
				annotations.SetAction (assembly, AssemblyAction.Save);
		}
	}
#endif
}

