using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjCRuntime;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {

	// Guide to when this test fails.
	//
	// What does this test do?
	// It ensures that every type in a p/invoke is blittable. This means that
	//    the number of allowed types gets cut down to a narrow set: pointers and
	//    scalars.
	// What are common failing types?
	//    1. string
	//    2. delegate
	//    3. ref/out types
	//    4. structs with non-blittable types
	//    5. arrays
	// How do I fix these?
	//    1. use a TransientString from ObjCRuntime. This is an IDisposable type
	//       that will allocate an unmanaged string in the appropriate encoding.
	//    Former typical case:
	//    extern static void SomePinvoke (string value);
	//    SomePinvoke (valueStr);
	//    Fix:
	//    extern void SomePinvoke (IntPtr value);
	//    using var valueTransient = new TransientString (valueStr);
	//    SomePinvoke (valueTransient);
	//    NOTE: You need to understand the lifespace of the string. If the call
	//    holds onto the pointer forever instead of making a copy, you can't use
	//    TransientString. Instead use Marshal to allocate the string once.
	//
	//    2. Don't use the delegate type as declared, instead use
	//    delegate* unamanged<...>
	//    Former typical case:
	//    delegate void SomeCall (int a, IntPtr someHandle);
	//    extern static void AssignSomeCallback (SomeCall f, IntPtr someHandle);
	//    [MonoPInvokeCallback (typeof (SomeCall))]
	//    static void SomeCallImpl (int a, IntPtr someHandle)
	//    { /* ... */ }
	//    public void SetSomeCallback (SomeCall callMe)
	//    {
	//         var handle = GCHandle.Alloc (callMe);
	//         AssignSomeCallback (SomeCallImpl, someHandle);
	//    }
	//    Fix:
	//    #if NET
	//    // note that this call is now unsafe in addition to the delegate*
	//    extern unsafe static void AssignSomeCallback (delegate* unmanaged<int, IntPtr, void>, IntPtr someHandle);
	//    #else
	//    ...old pinvoke goes here...
	//    #endif
	//    // you have to change the callback decoration. You *must* have a single
	//    // callback implementation that calls out to the actual callback because
	//    // the UnmanagedCallersOnly is strict. If you try to call it from C# you
	//    // get a compiler error.
	//    #if NET
	//    [UnmanagedCallersOnly]
	//    #else   
	//    [MonoPInvokeCallback (typeof (SomeCall))]
	//    #endif
	//    static void SomeCallImpl (int a, IntPtr someHandle)
	//    { /* ... */ }
	//    public void SetSomeCallback (SomeCall callMe)
	//    {
	//        var handle = GCHandle.Alloc (callMe);
	//    #if NET
	//        // calling code is now also unsafe because of the & operator
	//        unsafe {
	//            AssignSomeCallback (&SomeCallImpl, someHandle);
	//        }
	//    #else
	//        AssignSomeCallback (SomeCallImpl, someHandle);
	//    #endif
	//
	//    3. ref/out types - these are disallowed. Replace them with pointers and
	//       make the pinvoke unsafe. Can you use an IntPtr instead of unsafe? Yes.
	//       BUT you lose type safety. I'd rather have typed pointers than untyped
	//       if I have a choice.
	//
	//    extern static void TPGetCoordinates (NativeHandle obj, out int x, out int y);
	//    public Point GetCoordinates ()
	//    {
	//         int x = 0, y = 0;
	//         TPGetCoordinates (this.GetHandle (), out x, out y);
	//         return new Point (x, y);
	//    }
	//    Fix:
	//    #if NET
	//    extern unsafe static void TPGetCoordinates (NativeHandle obj, int* x, int* y);
	//    public Point GetCoordingates ()
	//    {
	//        int x = 0, y = 0;
	//    #if NET
	//        unsafe {
	//            TPGetCoordinates (this.GetHandle (), &x, &y);
	//        }
	//    #else
	//        TPGetCoordinates (this.GetHandle (), out x, out y);
	//    #endif
	//        return new Point (x, y);
	//    }
	//
	//    4. structs with non-blittable types. I don't think I ran into any of these
	//       *except* for structs with delegate callbacks. In that case, you need to
	//       rewrite the type to have delegate* unmanaged<> *AND* change the callback
	//       implementation to have [UnmanagedCallersOnly]
	//
	//    5. arrays. These are uncommon and fall into two broad categories:
	//       an array of blittable types and and array of non-blittable types.
	//       for the first you can change the array type in the pinvoke to be a pointer:
	//       extern static void SomeCall (int[] arr, int length);
	//    becomes:
	//       extern unsafe static void SomeCall (int* arr, int length);
	//    and then use a fixed block to get a pointer to the first element.
	//    for non-blittable types, you need to allocate a block of memory that can
	//    hold the entire array and then marshal the type into each element.
	//    On call return, you need to free each element and free the array UNLESS
	//    the call does NOT make a copy of each element.
	//    The only case that I ran into that had an array of non-blittables was
	//    arrays of strings.
	//    Because of this, there are two helpers in TransientString:
	//    IntPtr AllocStringArray (string? []? arr, Encoding encoding = Encoding.Auto);
	//    and:
	//    FreeStringArray (IntPtr arr, int count);
	//    both can handle a null array or null strings in the array.
	[TestFixture]
	public class BlittablePInvokes {
		struct MethodBlitResult {
			public MethodBlitResult (bool isBlittable)
			{
				IsBlittable = isBlittable;
				Result = new StringBuilder ();
			}
			public bool IsBlittable;
			public StringBuilder Result;
		}

		struct TypeAndIndex {
			public TypeAndIndex (TypeReference type, int index)
			{
				Type = type;
				Index = index;
			}
			public TypeReference Type;
			public int Index;
		}
		struct BlitAndReason {
			public BlitAndReason (bool isBlittable, string reason)
			{
				IsBlittable = isBlittable;
				Reason = reason;
			}
			public bool IsBlittable;
			public string Reason;
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))]
		public void CheckForNonBlittablePInvokes (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var pinvokes = AllPInvokes (assembly).Where (IsPInvokeOK);
			Assert.IsTrue (pinvokes.Count () > 0);

			var blitCache = new Dictionary<string, BlitAndReason> ();
			var results = pinvokes.Select (pi => IsMethodBlittable (assembly, pi, blitCache)).Where (r => !r.IsBlittable);
			if (results.Count () > 0) {
				var failString = new StringBuilder ();
				failString.Append ($"There is an issue with {results.Count ()} pinvokes in {assembly.Name} ({info.Path}):\n");
				foreach (var sb in results.Select (r => r.Result)) {
					failString.Append (sb.ToString ());
				}
				failString.Append ("In the file tests/cecil-tests/BlittablePInvokes.cs, read the guide carefully.");
				Assert.Fail (failString.ToString ());
			}

		}

		MethodBlitResult IsMethodBlittable (AssemblyDefinition assembly, MethodReference method, Dictionary<string, BlitAndReason> blitCache)
		{
			var result = new MethodBlitResult (true);
			var localResult = new StringBuilder ();
			var types = TypesFromMethod (method);
			foreach (var typeIndex in types) {
				if (!IsTypeBlittable (assembly, typeIndex.Type, localResult, blitCache)) {
					if (result.IsBlittable) {
						result.IsBlittable = false;
						result.Result.Append ($"    The P/Invoke {method.FullName} has been marked as non-blittable for the following reasons:\n");
					}
					if (typeIndex.Index < 0) {
						result.Result.Append ($"        The return type is");
					} else {
						result.Result.Append ($"        Parameter index {typeIndex.Index} is");
					}
					result.Result.Append ($" {typeIndex.Type}: {localResult.ToString ()}\n");
				}
			}
			return result;
		}

		IEnumerable<TypeAndIndex> TypesFromMethod (MethodReference method)
		{
			if (method.ReturnType is not null)
				yield return new TypeAndIndex (method.ReturnType, -1);
			var i = 0;
			foreach (var parameter in method.Parameters)
				yield return new TypeAndIndex (parameter.ParameterType, i++);
		}

		bool IsTypeBlittable (AssemblyDefinition assembly, TypeReference type, StringBuilder result, Dictionary<string, BlitAndReason> blitCache)
		{
			if (blitCache.TryGetValue (type.Name, out var cachedResult)) {
				if (!cachedResult.IsBlittable)
					result.Append ($" {cachedResult.Reason}");
				return cachedResult.IsBlittable;
			}
			if (IsBlittableTypesWeLike (type)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			if (IsBlittablePointer (type)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			var localResult = new StringBuilder ();
			if (IsBlittableValueType (assembly, type, localResult, blitCache)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			result.Append (localResult);
			blitCache [type.Name] = new BlitAndReason (false, result.ToString ());
			return false;
		}


		static HashSet<string> typesWeLike = new HashSet<string> () {
			"System.Void",
			"System.IntPtr",
			"System.UIntPtr",
			"ObjCRuntime.NativeHandle",
			"System.Byte",
			"System.SByte",
			"System.Int16",
			"System.UInt16",
			"System.Int32",
			"System.UInt32",
			"System.Int64",
			"System.UInt64",
			"System.Single",
			"System.Double",
			"System.Runtime.InteropServices.NFloat",
			"System.Runtime.InteropServices.NFloat&",
		};

		bool IsBlittableTypesWeLike (TypeReference t)
		{
			return typesWeLike.Contains (t.ToString ());
		}

		bool IsBlittablePointer (TypeReference type)
		{
			return type.IsPointer || type.IsFunctionPointer;
		}


		bool IsBlittableValueType (AssemblyDefinition assembly, TypeReference type, StringBuilder result, Dictionary<string, BlitAndReason> blitCache)
		{
			TypeDefinition? typeDefinition = type.Resolve ();
			if (typeDefinition is null) {
				result.Append ($" {type.FullName}: Unable to load type.");
				return false;
			}
			if (!typeDefinition.IsValueType) {
				// handy for debugging
				// change the true to false to get more information
				// than you'll probably need about the typeDefinition
				var other = true ? "" : $"IsByReference {typeDefinition.IsByReference} IsPointer {typeDefinition.IsPointer} IsSentinel {typeDefinition.IsSentinel} IsArray {typeDefinition.IsArray} IsGenericParameter {typeDefinition.IsGenericParameter} IsRequiredModifier {typeDefinition.IsRequiredModifier} IsOptionalModifier {typeDefinition.IsOptionalModifier} IsPinned {typeDefinition.IsPinned} IsFunctionPointer {typeDefinition.IsFunctionPointer} IsPrimitive {typeDefinition.IsPrimitive}";
				result.Append ($" {type.Name}: Type is not a value type.\n{other}\n");
				return false;
			}
			if (typeDefinition.IsEnum) {
				return true;
			}
			var allBlittable = true;
			// if we get here then this is a struct. We can presume
			// that a struct will be blittable until we know otherwise
			// this will prevent infinite recursion
			blitCache [type.Name] = new BlitAndReason (true, "");
			var fieldsResult = new StringBuilder ();

			// if we're here, this is a struct
			// a struct is blittable if and only if all the
			// non-static fields are blittable.
			foreach (var f in typeDefinition.Fields) {
				if (f.IsStatic)
					continue;
				var localResult = new StringBuilder ();
				if (!IsTypeBlittable (assembly, f.FieldType, localResult, blitCache)) {
					if (!allBlittable)
						fieldsResult.Append ($" {type.Name}:");
					fieldsResult.Append ($" ({f.Name}: {localResult})");
					allBlittable = false;
				}
			}
			if (!allBlittable) {
				result.Append (fieldsResult);
				blitCache [type.Name] = new BlitAndReason (false, fieldsResult.ToString ());
			}
			return allBlittable;
		}

		IEnumerable<MethodDefinition> AllPInvokes (AssemblyDefinition assembly)
		{
			return assembly.EnumerateMethods (method =>
				(method.Attributes & MethodAttributes.PInvokeImpl) != 0);
		}

		static bool IsPInvokeOK (MethodDefinition method)
		{
			var fullName = method.FullName;
			switch (fullName) {
			case "System.IntPtr ObjCRuntime.Selector::GetHandle(System.String)":
#if !NET8_0_OR_GREATER
			case "System.Boolean CoreFoundation.CFReadStream::CFReadStreamSetClient(System.IntPtr,System.IntPtr,CoreFoundation.CFStream/CFStreamCallback,System.IntPtr)":
			case "System.Boolean CoreFoundation.CFWriteStream::CFWriteStreamSetClient(System.IntPtr,System.IntPtr,CoreFoundation.CFStream/CFStreamCallback,System.IntPtr)":
#endif
				return false;
			default:
				return true;
			}
		}

		[Test]
		public void CheckForBlockLiterals ()
		{
			var failures = new HashSet<(string Message, string Location)> ();

			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				var assembly = info.Assembly;
				foreach (var type in assembly.EnumerateTypes ()) {
					foreach (var method in type.EnumerateMethods (m => m.HasBody)) {
						var body = method.Body;
						foreach (var instr in body.Instructions) {
							switch (instr.OpCode.Code) {
							case Code.Call:
							case Code.Calli:
							case Code.Callvirt:
								break;
							default:
								continue;
							}

							var targetMethod = instr.Operand as MethodReference;
							Assert.IsNotNull (targetMethod, "Null operand"); // If this ever fails, the code needs to be updated.

							if (!targetMethod!.DeclaringType.Is ("ObjCRuntime", "BlockLiteral"))
								continue;

							switch (targetMethod.Name) {
							case "SetupBlock":
							case "SetupBlockUnsafe":
								break;
							default:
								continue;
							}

							var location = method.RenderLocation (instr);
							var message = $"The call to {targetMethod.Name} in {method.AsFullName ()} must be converted to new Block syntax.";
							failures.Add (new (message, location));
						}
					}
				}
			}

			var newFailures = failures.Where (v => !knownFailuresBlockLiterals.Contains (v.Message)).ToArray ();
			var fixedFailures = knownFailuresBlockLiterals.Except (failures.Select (v => v.Message).ToHashSet ());

			var printKnownFailures = newFailures.Any () || fixedFailures.Any ();
			if (printKnownFailures) {
				Console.WriteLine ("Printing all failures as known failures because they seem out of date:");
				Console.WriteLine ("\t\tstatic HashSet<string> knownFailuresBlockLiterals = new HashSet<string> {");
				foreach (var failure in failures.OrderBy (v => v))
					Console.WriteLine ($"\t\t\t\"{failure.Message}\",");
				Console.WriteLine ("\t\t};");
			}

			if (newFailures.Any ()) {
				// Print any new failures with the local path for easy navigation (depending on the terminal and/or IDE you might just click on the path to open the corresponding file).
				Console.WriteLine ($"Printing {newFailures.Count ()} new failures with local paths for easy navigation:");
				foreach (var failure in newFailures.OrderBy (v => v))
					Console.WriteLine ($"    {failure.Location}: {failure.Message}");
			}

			Assert.IsEmpty (newFailures, "Failures");
			Assert.IsEmpty (fixedFailures, "Known failures that aren't failing anymore - remove these from the list of known failures");
		}

		static HashSet<string> knownFailuresBlockLiterals = new HashSet<string> {
			"The call to SetupBlock in ObjCRuntime.BlockLiteral.GetBlockForDelegate(System.Reflection.MethodInfo, System.Object, System.UInt32, System.String) must be converted to new Block syntax.",
			"The call to SetupBlock in ObjCRuntime.BlockLiteral.SetupBlock(System.Delegate, System.Delegate) must be converted to new Block syntax.",
			"The call to SetupBlock in ObjCRuntime.BlockLiteral.SetupBlockUnsafe(System.Delegate, System.Delegate) must be converted to new Block syntax.",
		};

		[Test]
		public void CheckForMonoPInvokeCallback ()
		{
			var failures = new HashSet<(string Message, string Location)> ();

			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				var assembly = info.Assembly;
				foreach (var type in assembly.EnumerateTypes ()) {
					foreach (var method in type.EnumerateMethods (m => m.HasCustomAttributes)) {
						foreach (var ca in method.CustomAttributes) {
							if (ca.AttributeType.Name != "MonoPInvokeCallbackAttribute")
								continue;

							var location = method.RenderLocation ();
							var message = $"The method {method.AsFullName ()} has a MonoPInvokeCallback attribute and must be converted to an UnmanagedCallersOnly method.";
							failures.Add (new (message, location));

							break;
						}
					}
				}
			}

			var newFailures = failures.Where (v => !knownFailuresMonoPInvokeCallback.Contains (v.Message)).ToArray ();
			var fixedFailures = knownFailuresMonoPInvokeCallback.Except (failures.Select (v => v.Message).ToHashSet ());

			if (newFailures.Any ()) {
				// Print any new failures with the local path for easy navigation (depending on the terminal and/or IDE you might just click on the path to open the corresponding file).
				Console.WriteLine ($"Printing {newFailures.Count ()} failures with local path for easy navigation:");
				foreach (var failure in newFailures.OrderBy (v => v))
					Console.WriteLine ($"    {failure.Location}: {failure.Message}");
			}

			Assert.IsEmpty (newFailures, "New Failures");
			Assert.IsEmpty (fixedFailures, "Known failures that aren't failing anymore - remove these from the list of known failures");
		}

		static HashSet<string> knownFailuresMonoPInvokeCallback = new HashSet<string> {
#if !NET8_0_OR_GREATER
			"The method CoreFoundation.CFStream.OnCallback(System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to an UnmanagedCallersOnly method.",
#endif
		};
	}
}
