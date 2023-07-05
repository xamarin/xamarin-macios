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
		string? outputDirectory = null;
		Dictionary<string, FieldDefinition> csTypeToFieldDef = new Dictionary<string, FieldDefinition> ();
		IEnumerable<AssemblyDefinition> assemblies;
		AssemblyDefinition xamarinAssembly;
		Xamarin.Tuner.DerivedLinkContext linkContext;
		Action <int, string> logFunc;

		public Rewriter (CSToObjCMap map, IEnumerable<AssemblyDefinition> assembliesToPatch, Xamarin.Tuner.DerivedLinkContext? linkContext, Action<int, string> logFunc)
		{
logFunc (0, $"---------------------------------------------------------");
logFunc (0, $"Redirector: making rewriter. Map has {map.Count} entries.");
logFunc (0, $"Redirector: looking to patch {assembliesToPatch.Count ()} assemblies.");
foreach (var assembly in assembliesToPatch)
	logFunc (0, $"Redirector: {assembly.Name}");
			this.map = map;
			this.assemblies = assembliesToPatch;
			var xasm = assembliesToPatch.Select (assem => assem.MainModule).FirstOrDefault (ContainsNativeHandle)?.Assembly;
			if (xasm is null) {
logFunc (0, "Redirector: null xamarin assembly.");
				throw new Exception ("Unable to find Xamarin assembly.");
			} else {
				xamarinAssembly = xasm;
				pathToXamarinAssembly = xamarinAssembly.MainModule.FileName;
			}
			if (linkContext is null) {
Console.WriteLine ("no link context.");
				throw new Exception ("Rewriter needs a valid link context.");
			} else {
				this.linkContext = linkContext;
			}
			this.logFunc = (n, str) => {
				logFunc (n, str);
				HardAppendToLog (str);
			}; 
		}

		public static void HardAppendToLog (string str)
		{
			using var writer = System.IO.File.AppendText ("/Users/stevehawley/rewriterlog.txt");
			var date = DateTime.Now;
			writer.WriteLine (date.ToString () + " " + str);
		}

		public string Process ()
		{
			Dictionary<string, FieldDefinition> classMap;
			try {
logFunc (0, "Redirector: making class handles.");
				classMap = CreateClassHandles ();
			} catch (Exception e) {
logFunc (0, $"Redirector: caught and exception {e.Message}");
				// if this throws, no changes are made to the assemblies
				// so it's safe to log it on the far side.
				return e.Message;
			}
			PatchClassPtrUsage (classMap);
logFunc (0, $"---------------------------------------------------------");
			return "";
		}

		Dictionary<string, FieldDefinition> CreateClassHandles ()
		{
			var classMap = new Dictionary<string, FieldDefinition> ();
			var module = xamarinAssembly.MainModule;

			var classHandles = LocateClassHandles (module);
			if (classHandles is null) {
logFunc (0, $"Redirector: unable to find ClassHandles in {module.Name}");
				throw new Exception ($"Unable to find {classHandleName} type in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
			}

logFunc (0, "Found class handles.");
			var initMethod = classHandles.Methods.FirstOrDefault (m => m.Name == initClassHandlesName);
			if (initMethod is null)
				throw new Exception ($"Unable to find {initClassHandlesName} method in {classHandles.Name}");

logFunc (0, "Found initializer.");
			var processor = initMethod.Body.GetILProcessor ();

			var mtClassMapDef = LocateMTClassMap (module);
			if (mtClassMapDef is null) {
logFunc (0, $"Redirector: unable to find {mtClassMapName} in module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
				throw new Exception ($"Unable to find {mtClassMapName} in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
}

logFunc (0, "Found mtClassMapDef.");
			var nativeHandle = LocateNativeHandle (module);
			if (nativeHandle is null) {
logFunc (0, $"Unable to find {nativeHandleName} in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
				throw new Exception ($"Unable to find {nativeHandleName} in Module {module.Name} File {module.FileName}, assembly {xamarinAssembly.Name}");
			}

logFunc (0, "Found native handle.");
			var nativeHandleOpImplicit = FindOpImplicit (nativeHandle);
			if (nativeHandleOpImplicit is null) {
logFunc (0, $"Redirector: Unable to find implicit cast in {nativeHandleName}");
				throw new Exception ($"Unable to find implicit cast in {nativeHandleName}");
}

logFunc (0, "Found native handle opImplicit.");
			if (map.Count () == 0)
				return classMap;

			foreach (var nameIndexPair in map) {
				var csName = nameIndexPair.Key;
				var nameIndex = nameIndexPair.Value;
logFunc (0, $"Adding public field for ${nameIndex.ObjCName}");
				var fieldDef = AddPublicStaticField (classHandles, nameIndex.ObjCName, nativeHandle);
				AddInitializer (nativeHandleOpImplicit, processor, mtClassMapDef, nameIndex.MapIndex, fieldDef);
				classMap [csName] = fieldDef;
logFunc (0, $"Mapped {csName} to ${nameIndex.ObjCName}");
			}

			MarkForSave (xamarinAssembly);
logFunc (0, "Marked Xamarin assembly for save.");
			return classMap;
		}

		MethodDefinition? FindOpImplicit (TypeDefinition nativeHandle)
		{
			return nativeHandle.Methods.FirstOrDefault (m => m.Name == "op_Implicit" && m.ReturnType == nativeHandle &&
				m.Parameters.Count == 1 && m.Parameters [0].ParameterType == nativeHandle.Module.TypeSystem.IntPtr);
		}

		void AddInitializer (MethodReference nativeHandleOpImplicit, ILProcessor il, TypeDefinition mtClassMapDef, int index, FieldDefinition fieldDef)
		{
logFunc (0, $"Redirector: adding code to the initializer for {fieldDef.Name}.");
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
logFunc (0, $"Patching class handle usage in module {module.Name}");
				if (PatchClassPtrUsage (classMap, module)) {
logFunc (0, $"Marking assembly for save ${assem.Name}");
					MarkForSave (assem);
				} else {
					logFunc (0, $"For some reason {assem.Name} wasn't modified.");
				}
			}
		}

		// returns true if the assembly was changed.
		bool PatchClassPtrUsage (Dictionary<string, FieldDefinition> classMap, ModuleDefinition module)
		{
logFunc (0, $"Patching class ptr usage in {module.Name}");
			var dirty = false;
var allTypes = AllTypes (module);
logFunc (0, $"There are {allTypes.Count ()} types in {module.Name}");
			foreach (var cl in AllTypes (module)) {
if (module.Name == "MyClassRedirectApp.dll" && cl.FullName == "MySimpleApp.SomeObj") {
logFunc (0, "----special case, dumping classMap on MySimpleApp.SomeObj access---");
foreach (var kvp in classMap) {
logFunc (0, $"key: {kvp.Key} val: {kvp.Value} key == cl.FullName: {(kvp.Key == cl.FullName)}");
}
logFunc (0, "----end special case classMap dump----");
}
				if (classMap.TryGetValue (cl.FullName, out var classPtrField)) {
logFunc (0, $"class {cl.FullName} is in the class map.");
					dirty = true;
					// if this doesn't throw, it will
					// always change the contents of an
					// assembly
					PatchClassPtrUsage (cl, classPtrField);
				} else {
logFunc (0, $"{cl.FullName} was not in the class map.");
				}
			}
logFunc (0, $"after patching assembly {module.Name} is dirty: {dirty}");
			return dirty;
		}

		void PatchClassPtrUsage (TypeDefinition cl, FieldDefinition classPtrField)
		{
logFunc (0, $"patching class_ptr using in {cl.Name}.");
			var class_ptr = cl.Fields.FirstOrDefault (f => f.Name == classPtrName);
			if (class_ptr is null) {
logFunc (0, $"didn't get a class ptr in {cl.Name}.");
				throw new Exception ($"Error processing class {cl.FullName} - no {classPtrName} field.");
			}

			// step 1: remove the field
			cl.Fields.Remove (class_ptr);
logFunc (0, $"removed class_ptr field in {cl.Name}");

			// step 2: remove init code from cctor
			RemoveCCtorInit (cl, class_ptr);

			// step 3: patch every method
			PatchMethods (cl, class_ptr, classPtrField);
		}

		void PatchMethods (TypeDefinition cl, FieldDefinition classPtr, FieldDefinition classPtrField)
		{
logFunc (0, "Patching methods.");
			foreach (var method in cl.Methods) {
				PatchMethod (method, classPtr, classPtrField);
			}
		}

		void PatchMethod (MethodDefinition method, FieldDefinition classPtr, FieldDefinition classPtrField)
		{
logFunc (0, $"Patching {method.Name}");
			var il = method.Body.GetILProcessor ();
			for (int i = 0; i < method.Body.Instructions.Count; i++) {
				var instr = method.Body.Instructions [i];
				if (instr.OpCode == OpCodes.Ldsfld && instr.Operand == classPtr) {
logFunc (0, "Replaced reference to class_ptr");
					il.Replace (instr, Instruction.Create (OpCodes.Ldsfld, method.Module.ImportReference (classPtrField)));
				}
			}
		}

		void RemoveCCtorInit (TypeDefinition cl, FieldDefinition class_ptr)
		{
logFunc (0, "Removing CCtorInit.");
			var cctor = cl.Methods.FirstOrDefault (m => m.Name == ".cctor");
			if (cctor is null)
				return; // no static init - should never happen, but we can deal.
logFunc (0, "found the cctor.");

			var il = cctor.Body.GetILProcessor ();
			Instruction? stsfld = null;
			int i = 0;
			for (; i < il.Body.Instructions.Count; i++) {
				var instr = il.Body.Instructions [i];
				// look for
				// stsfld class_ptr
				if (instr.OpCode == OpCodes.Stsfld && instr.Operand == class_ptr) {
					stsfld = instr;
logFunc (0, "found stsfld instruction in cctor.");
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
logFunc (0, "isGetClassHandle and isLdStr");
				il.RemoveAt (i);
				il.RemoveAt (i - 1);
				il.RemoveAt (i - 2);
			} else if (isGetClassHandle) {
				// don't know how the string got on the stack, so at least get rid of the
logFunc (0, "isGetClassHandle but not isLdStr");
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
logFunc (0, "removing cctor entirely");
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

		string ToOutputFileName (string pathToInputFileName)
		{
			return Path.Combine (outputDirectory, Path.GetFileName (pathToInputFileName));
		}

		void MarkForSave (AssemblyDefinition assembly)
		{
			var annotations = linkContext.Annotations;
			var action = annotations.GetAction (assembly);
logFunc (0, $"action for that assembly is {action.ToString ()}");
			if (action != AssemblyAction.Save) {
logFunc (0, $"marking assembly action as Save");
				annotations.SetAction (assembly, AssemblyAction.Save);
			}
		}
	}
#endif
}

