using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Cecil.Tests {
	public class ConstructorTest {
		static bool IsMatch (MethodDefinition ctor, params (string Namespace, string Name) [] parameterTypes)
		{
			if (!ctor.IsConstructor)
				return false;
			if (ctor.IsStatic)
				return false;
			if (!ctor.HasParameters)
				return false;
			var parameters = ctor.Parameters;
			if (parameters.Count != parameterTypes.Length)
				return false;
			for (var i = 0; i < parameters.Count; i++) {
				if (!parameters [i].ParameterType.Is (parameterTypes [i].Namespace, parameterTypes [i].Name))
					return false;
			}
			return true;
		}

		static MethodDefinition? GetConstructor (TypeDefinition type, params (string Namespace, string Name) [] parameterTypes)
			=> GetConstructor (type, out _, parameterTypes);

		static MethodDefinition? GetConstructor (TypeDefinition type, out string? selectorName, params (string Namespace, string Name) [] parameterTypes)
		{
			selectorName = null;
			foreach (var ctor in type.Methods) {
				if (IsMatch (ctor, parameterTypes)) {
					// retrieve the selectorName if possible
					var exportAttr = ctor.CustomAttributes.FirstOrDefault (attr => attr.AttributeType.Is ("Foundation", "ExportAttribute"));
					if (exportAttr is not null) {
						selectorName = (string) exportAttr.ConstructorArguments [0].Value!;
					}
					return ctor;
				}
			}
			return null;
		}

		static string GetLocation (MethodDefinition? method)
		{
			if (method?.DebugInformation?.HasSequencePoints == true) {
				var seq = method.DebugInformation.SequencePoints [0];
				return seq.Document.Url + ":" + seq.StartLine + ": ";
			}
			return string.Empty;
		}

		static bool IsFunctionEnd (IList<Instruction> instructions, int index)
		{
			if (instructions.Count == index + 1 && instructions [index].OpCode == OpCodes.Ret)
				return true;
			if (instructions.Count == index + 2 && instructions [index].OpCode == OpCodes.Newobj && ((MethodReference) instructions [index].Operand).Resolve ().Parameters.Count == 0 && instructions [index + 1].OpCode == OpCodes.Throw)
				return true;
			if (instructions.Count == index + 3 && instructions [index].OpCode == OpCodes.Ldstr && instructions [index + 1].OpCode == OpCodes.Newobj && ((MethodReference) instructions [index + 1].Operand).Resolve ().Parameters.Count == 1 && instructions [index + 2].OpCode == OpCodes.Throw)
				return true;
			return false;
		}

		static bool VerifyInstructions (MethodDefinition method, IList<Instruction> instructions, out string? reason)
		{
			reason = null;

			// base (owns)
			// optional additional statements (either statement, not both):
			//     IsDirectBinding = false;
			//     MarkDirtyIfDerived ()
			if (instructions.Count >= 3 &&
				instructions [0].OpCode == OpCodes.Ldarg_0 &&
				instructions [1].OpCode == OpCodes.Ldarg_1 &&
				instructions [2].OpCode == OpCodes.Call) {
				var targetMethod = (instructions [2].Operand as MethodReference)!.Resolve ();
				if (!targetMethod.IsConstructor) {
					reason = $"Calls another method which is not a constructor: {targetMethod.FullName}";
					return false;
				}
				var isChainedCtorCall = targetMethod.DeclaringType == method.DeclaringType || targetMethod.DeclaringType == method.DeclaringType.BaseType;
				if (!isChainedCtorCall) {
					reason = $"Calls unknown (unchained) constructor: {targetMethod.FullName}";
					return false;
				}

				if (IsFunctionEnd (instructions, 3))
					return true;

				if (instructions [3].OpCode == OpCodes.Ldarg_0 && instructions [4].OpCode == OpCodes.Ldc_I4_0 && instructions [5].OpCode == OpCodes.Call) {
					targetMethod = (instructions [5].Operand as MethodReference)!.Resolve ();
					if (targetMethod.Name != "set_IsDirectBinding") {
						reason = $"Calls unknown method: {targetMethod.FullName}";
						return false;
					}

					if (IsFunctionEnd (instructions, 6))
						return true;
				}

				if (instructions [3].OpCode == OpCodes.Ldarg_0 && instructions [4].OpCode == OpCodes.Call) {
					targetMethod = (instructions [4].Operand as MethodReference)!.Resolve ();
					if (targetMethod.Name != "MarkDirtyIfDerived") {
						reason = $"Calls unknown method: {targetMethod.FullName}";
						return false;
					}

					if (IsFunctionEnd (instructions, 5))
						return true;
				}
			}

			// base (handle, owns|false)
			if (instructions.Count >= 4 &&
				instructions [0].OpCode == OpCodes.Ldarg_0 &&
				instructions [1].OpCode == OpCodes.Ldarg_1 &&
				(instructions [2].OpCode == OpCodes.Ldarg_2 || instructions [2].OpCode == OpCodes.Ldc_I4_0) &&
				instructions [3].OpCode == OpCodes.Call) {
				var targetMethod = (instructions [3].Operand as MethodReference)!.Resolve ();
				if (!targetMethod.IsConstructor) {
					reason = $"Calls another method which is not a constructor (2): {targetMethod.FullName}";
					return false;
				}
				var isChainedCtorCall = targetMethod.DeclaringType.Resolve () == method.DeclaringType.Resolve () || targetMethod.DeclaringType.Resolve () == method.DeclaringType.BaseType.Resolve ();
				if (!isChainedCtorCall) {
					reason = $"Calls unknown (unchained) constructor (2): {targetMethod.FullName}";
					return false;
				}

				if (IsFunctionEnd (instructions, 4))
					return true;
			}

			// base (handle, owns|false, validate: false|true)
			if (instructions.Count >= 5 &&
				instructions [0].OpCode == OpCodes.Ldarg_0 &&
				instructions [1].OpCode == OpCodes.Ldarg_1 &&
				(instructions [2].OpCode == OpCodes.Ldarg_2 || instructions [2].OpCode == OpCodes.Ldc_I4_0) &&
				(instructions [3].OpCode == OpCodes.Ldc_I4_0 || instructions [3].OpCode == OpCodes.Ldc_I4_1) &&
				instructions [4].OpCode == OpCodes.Call) {
				var targetMethod = (instructions [4].Operand as MethodReference)!.Resolve ();
				if (!targetMethod.IsConstructor) {
					reason = $"Calls another method which is not a constructor (2): {targetMethod.FullName}";
					return false;
				}
				var isChainedCtorCall = targetMethod.DeclaringType == method.DeclaringType || targetMethod.DeclaringType == method.DeclaringType.BaseType;
				if (!isChainedCtorCall) {
					reason = $"Calls unknown (unchained) constructor (2): {targetMethod.FullName}";
					return false;
				}

				if (IsFunctionEnd (instructions, 5))
					return true;
			}

			if (reason is null)
				reason = $"Sequence of instructions didn't match any known sequence.";

			return false;
		}

		static bool VerifyConstructor (MethodDefinition? ctor, out string? failureReason)
		{
			failureReason = null;
			// There's nothing wrong with a constructor that doesn't exist
			if (ctor is null)
				return true;

			// Verify that the constructor only does valid stuff
			if (!VerifyInstructions (ctor, ctor.Body.Instructions, out failureReason)) {
				Console.WriteLine (ctor.FullName);
				foreach (var instr in ctor.Body.Instructions)
					Console.WriteLine (instr);

				return false;
			}

			return true;
		}

		public static bool ImplementsINativeObject (TypeDefinition? type)
		{
			if (type is null)
				return false;

			foreach (var id in type.Interfaces) {
				if (id.InterfaceType.Name == "INativeObject") {
					return true;
				}
			}

			return ImplementsINativeObject (type.BaseType?.Resolve ());
		}

		public static bool SubclassesNSObject (TypeDefinition? type)
		{
			if (type is null)
				return false;

			if (type.Namespace == "Foundation" && type.Name == "NSObject")
				return true;

			return SubclassesNSObject (type.BaseType?.Resolve ());
		}

		static bool IsVisible (TypeDefinition type)
		{
			if (type.IsNested) {
				if (!IsVisible (type.DeclaringType))
					return false;
				return type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamilyOrAssembly;
			} else {
				return type.IsPublic;
			}
		}

		static bool IsVisible (MethodDefinition method)
		{
			if (!IsVisible (method.DeclaringType))
				return false;
			return method.IsPublic || method.IsFamilyOrAssembly || method.IsFamily;
		}

		static bool IsPublic (MethodDefinition method)
		{
			if (!IsVisible (method.DeclaringType))
				return false;
			return method.IsPublic;
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void INativeObjectIntPtrConstructorDoesNotOwnHandle (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var failures = new List<string> ();
			foreach (var dll in Configuration.GetBaseLibraryImplementations (platform)) {
				using (var ad = AssemblyDefinition.ReadAssembly (dll, new ReaderParameters (ReadingMode.Deferred) { ReadSymbols = true })) {
					foreach (var type in ad.MainModule.Types) {
						// Skip classes we know aren't (properly) reference counted.
						switch (type.Name) {
						case "Selector": // not really refcounted
						case "Class": // not really refcounted
						case "Protocol": // not really refcounted
						case "AURenderEventEnumerator": // this class shouldn't really be an INativeObject in the first place
						case "AudioBuffers": // this class shouldn't really be an INativeObject in the first place
						case "AVAudioChannelLayout": // has a private IntPtr constructor which is a void* in native code (i.e. not a mistake).
						case "VNVideoProcessorFrameRateCadence": // has a nint (i.e. IntPtr) constructor (framerate) - not a mistake
						case "NSMutableOrderedSet`1":
						case "NSMutableOrderedSet": // has a nint (i.e. IntPtr) constructor (capacity) - not a mistake
						case "NSMutableSet`1":
						case "NSMutableSet": // has a nint (i.e. IntPtr) constructor (capacity) - not a mistake
						case "NSMutableString": // has a nint (i.e. IntPtr) constructor (capacity) - not a mistake
						case "NSNumber": // has a nint (i.e. IntPtr) constructor - not a mistake
						case "NSConditionLock": // has a nint (i.e. IntPtr) constructor (condition) - not a mistake
						case "NSScrubberProportionalLayout": // has a nint (i.e. IntPtr) constructor (numberOfVisibleItems) - not a mistake
						case "NSIndexSet": // has a nuint (i.e. UIntPtr) constructor (index) - not a mistake
						case "NSWindow": // has an actual IntPtr constructor (initWithWindowRef:) - not a mistake
							continue;
						}

						// Find classes that implement INativeObject, but doesn't subclass NSObject.
						if (!type.IsClass)
							continue;

						// Does type implement INativeObject?
						if (!ImplementsINativeObject (type))
							continue;

						var isNSObjectSubclass = SubclassesNSObject (type);

						// Find the constructors constructors we care about
						var intptrCtor = GetConstructor (type, out var intptrCtorSelector, ("System", "IntPtr"));
						var intptrBoolCtor = GetConstructor (type, ("System", "IntPtr"), ("System", "Boolean"));
						var nativeHandleCtor = GetConstructor (type, ("ObjCRuntime", "NativeHandle"));
						var nativeHandleBoolCtor = GetConstructor (type, ("ObjCRuntime", "NativeHandle"), ("System", "Boolean"));

						if (intptrCtor is not null && intptrCtorSelector == "init:") {
							if (IsVisible (intptrCtor)) {
								var msg = $"{type}: (IntPtr) constructor found. It should not exist.";
								Console.WriteLine ($"{GetLocation (intptrCtor)}{msg}");
								failures.Add (msg);
							} else {
								var msg = $"{type}: private (IntPtr) constructor found. It should probably not exist. If it should, add an exception to this test.";
								failures.Add ($"{GetLocation (intptrCtor)}{msg}");
							}
						}

						if (intptrBoolCtor is not null) {
							var msg = $"{type}: (IntPtr, bool) constructor found. It should not exist.";
							Console.WriteLine ($"{GetLocation (intptrBoolCtor)}{msg}");
							failures.Add (msg);
						}

						if (nativeHandleCtor is not null) {
							if (IsPublic (nativeHandleCtor)) {
								var msg = $"{type}: public (NativeHandle) constructor found. If it exists it should not be public.";
								Console.WriteLine ($"{GetLocation (nativeHandleCtor)}{msg}");
								failures.Add (msg);
							}
						}

						if (nativeHandleBoolCtor is not null) {
							if (IsPublic (nativeHandleBoolCtor)) {
								var msg = $"{type}: public (NativeHandle, bool) constructor found. If it exists it should not be public.";
								Console.WriteLine ($"{GetLocation (nativeHandleBoolCtor)}{msg}");
								failures.Add (msg);
							}
						}

						var skipILVerification = isNSObjectSubclass;
						switch (type.Name) {
						case "CGPDFObject": // root class
						case "SecKeyChain": // root class
						case "NSObject": // NSObject is a base class and needs custom constructor logic
						case "NSZone": // root class
						case "ABAddressBook": // needs a custom ctor implementation
						case "CFSocket": // needs a custom ctor implementation
						case "NWBrowser": // needs a custom ctor implementation
						case "NWListener": // needs a custom ctor implementation
						case "CFNotificationCenter": // needs a custom ctor implementation
						case "AUGraph": // needs a custom ctor implementation
						case "ABMultiValue`1": // has a custom ctor implementation
						case "NWPathMonitor": // has a custom ctor implementation
							skipILVerification = true;
							break;
						}

						if (!skipILVerification) {
							if (!VerifyConstructor (nativeHandleCtor, out var failureReason)) {
								var msg = $"{type}: (NativeHandle) ctor failed IL verification: {failureReason}";
								Console.WriteLine ($"{GetLocation (nativeHandleCtor)}{msg}");
								failures.Add (msg);
							}

							if (!VerifyConstructor (nativeHandleBoolCtor, out failureReason)) {
								var msg = $"{type}: (NativeHandle, bool) ctor failed IL verification: {failureReason}";
								Console.WriteLine ($"{GetLocation (nativeHandleBoolCtor)}{msg}");
								failures.Add (msg);
							}
						}
					}
				}
			}
			Assert.That (failures, Is.Empty, "No failures");
		}

		bool IsConditionallyPreserved (MethodDefinition ctor)
		{
			if (!ctor.HasCustomAttributes)
				return false;

			foreach (var ca in ctor.CustomAttributes) {
				if (ca.AttributeType.Name != "PreserveAttribute")
					continue;
				if (!ca.HasFields)
					continue;
				foreach (var field in ca.Fields) {
					if (field.Name != "Conditional")
						continue;
					return (bool) field.Argument.Value;
				}
			}

			return false;
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void NativeObjectIntPtrBoolConstructorIsPreserved (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var failures = new List<string> ();
			foreach (var dll in Configuration.GetBaseLibraryImplementations (platform)) {
				using (var ad = AssemblyDefinition.ReadAssembly (dll, new ReaderParameters (ReadingMode.Deferred) { ReadSymbols = true })) {
					foreach (var type in ad.MainModule.Types) {
						switch (type.Name) {
						case "AudioBuffers": // this class shouldn't really be an INativeObject in the first place
							continue;
						}
						// Find classes that implement INativeObject, but doesn't subclass NSObject.
						if (!type.IsClass)
							continue;

						// Skip abstract classes, any subclasses should preserve their ctor (which will make the linker mark this class' ctor as well).
						if (type.IsAbstract)
							continue;

						// Does type implement INativeObject?
						if (!ImplementsINativeObject (type))
							continue;

						if (SubclassesNSObject (type))
							continue;

						// Find the IntPtr/NativeHandle, Boolean constructor
						var intptrBoolCtor = GetConstructor (type, ("System", "IntPtr"), ("System", "Boolean"));
						var nativeHandleBoolCtor = GetConstructor (type, ("ObjCRuntime", "NativeHandle"), ("System", "Boolean"));

						if (intptrBoolCtor is not null && !IsConditionallyPreserved (intptrBoolCtor)) {
							var msg = $"{type}: The (IntPtr, bool) constructor is not conditionally preserved.";
							Console.WriteLine ($"{GetLocation (intptrBoolCtor)}{msg}");
							failures.Add (msg);
						}

						if (nativeHandleBoolCtor is not null && !IsConditionallyPreserved (nativeHandleBoolCtor)) {
							var msg = $"{type}: The (NativeHandle, bool) constructor is not conditionally preserved.";
							Console.WriteLine ($"{GetLocation (nativeHandleBoolCtor)}{msg}");
							failures.Add (msg);
						}
					}
				}
			}
			Assert.That (failures, Is.Empty, "No failures");
		}
	}
}
