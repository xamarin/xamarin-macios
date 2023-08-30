//
// Runtime.CoreCLR.cs: Supporting managed code for the CoreCLR bridge
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2021 Microsoft Corp.


// Uncomment the TRACK_MONOOBJECTS define to show a summary at process exit of
// the MonoObjects that were created, and if any were not freed. If there are
// leaked MonoObjects, a list of them will be printed.
// This has an equivalent variable in runtime/runtime-internal.m
// which must be set for tracking to work.
//#define TRACK_MONOOBJECTS

// Uncomment VERBOSE_LOG to enable verbose logging
// #define VERBOSE_LOG

#if NET && !COREBUILD

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ObjectiveC;
using System.Runtime.Loader;
using System.Text;

using Foundation;

using Xamarin.Bundler;

using MonoObjectPtr=System.IntPtr;

namespace ObjCRuntime {

	public partial class Runtime {
		// Keep in sync with XamarinLookupTypes in main.h
		internal enum TypeLookup {
			System_Array,
			System_String,
			System_IntPtr,
			Foundation_NSNumber,
			Foundation_NSObject,
			Foundation_NSString,
			Foundation_NSValue,
			ObjCRuntime_INativeObject,
			ObjCRuntime_NativeHandle,
		}

		// Keep in sync with XamarinExceptionType in main.h
		internal enum ExceptionType {
			System_Exception,
			System_InvalidCastException,
			System_EntryPointNotFoundException,
			System_OutOfMemoryException,
		}

		// This struct must be kept in sync with the _MonoObject struct in coreclr-bridge.h
		[StructLayout (LayoutKind.Sequential)]
		internal struct MonoObject {
			public int ReferenceCount;
			public IntPtr GCHandle;
			public IntPtr StructValue;
		}

		// This struct must be kept in sync with the _MonoMethodSignature struct in coreclr-bridge.h
		[StructLayout (LayoutKind.Sequential)]
		unsafe struct MonoMethodSignature {
			public MonoObject* Method;
			public int ParameterCount;
			public MonoObject* ReturnType;
			public MonoObject* Parameters;
		}

#if TRACK_MONOOBJECTS
		static bool? track_monoobject_with_stacktraces;
#endif

		// Define VERBOSE_LOG at the top of this file to get all printfs
		[System.Diagnostics.Conditional ("VERBOSE_LOG")]
		static void log_coreclr (string message)
		{
			NSLog (message);
		}

		// Define VERBOSE_LOG at the top of this file to get all printfs
		[System.Diagnostics.Conditional ("VERBOSE_LOG")]
		static void log_coreclr_render (string message, params object?[] argumentsToRender)
		{
			var args = new string [argumentsToRender.Length];
			for (var i = 0; i < args.Length; i++) {
				string arg;
				var obj = argumentsToRender [i];
				if (obj is null) {
					arg = "<null>";
				} else if (obj is IntPtr ptr) {
					arg = $"0x{ptr.ToString ("x")} (IntPtr)";
				} else if (obj.GetType ().IsValueType) {
					arg = $"{obj.ToString ()} ({obj.GetType ()})";
				} else if (obj is INativeObject inativeobj) {
					// Don't call ToString on an INativeObject, we may end up with infinite recursion.
					arg = $"{inativeobj.Handle.ToString ()} ({obj.GetType ()})";
				} else {
					var toString = obj.ToString () ?? string.Empty;
					// Print one line, and at most 256 characters.
					var strLength = Math.Min (256, toString.Length);
					var eol = toString.IndexOf ('\n');
					if (eol != -1 && eol < strLength)
						strLength = eol;
					if (strLength != toString.Length)
						toString = toString.Substring (0, strLength) + " [...]";

					arg = $"{toString} ({obj.GetType ()})";
				}
				args [i] = arg;
			}
			log_coreclr (string.Format (message, args));
		}

		static unsafe void InitializeCoreCLRBridge (InitializationOptions* options)
		{
			if (options->xamarin_objc_msgsend != IntPtr.Zero)
				ObjectiveCMarshal.SetMessageSendCallback (ObjectiveCMarshal.MessageSendFunction.MsgSend, options->xamarin_objc_msgsend);

			if (options->xamarin_objc_msgsend_super != IntPtr.Zero)
				ObjectiveCMarshal.SetMessageSendCallback (ObjectiveCMarshal.MessageSendFunction.MsgSendSuper, options->xamarin_objc_msgsend_super);

			if (options->xamarin_objc_msgsend_stret != IntPtr.Zero)
				ObjectiveCMarshal.SetMessageSendCallback (ObjectiveCMarshal.MessageSendFunction.MsgSendStret, options->xamarin_objc_msgsend_stret);

			if (options->xamarin_objc_msgsend_super_stret != IntPtr.Zero)
				ObjectiveCMarshal.SetMessageSendCallback (ObjectiveCMarshal.MessageSendFunction.MsgSendSuperStret, options->xamarin_objc_msgsend_super_stret);

			delegate* unmanaged<void> beginEndCallback = (delegate* unmanaged<void>) options->reference_tracking_begin_end_callback;
			delegate* unmanaged<IntPtr, int> isReferencedCallback = (delegate* unmanaged<IntPtr, int>) options->reference_tracking_is_referenced_callback;
			delegate* unmanaged<IntPtr, void> trackedObjectEnteredFinalization = (delegate* unmanaged<IntPtr, void>) options->reference_tracking_tracked_object_entered_finalization;
			ObjectiveCMarshal.Initialize (beginEndCallback, isReferencedCallback, trackedObjectEnteredFinalization, UnhandledExceptionPropagationHandler);

			AssemblyLoadContext.Default.Resolving += ResolvingEventHandler;
		}

		[DllImport ("__Internal")]
		static extern byte xamarin_locate_assembly_resource (IntPtr assembly_name, IntPtr culture, IntPtr resource, IntPtr path, nint pathlen);

		static bool xamarin_locate_assembly_resource (string assembly_name, string? culture, string resource, [NotNullWhen (true)] out string? path)
		{
			path = null;

			const int path_size = 1024;
			using var assembly_name_ptr = new TransientString (assembly_name);
			using var culture_ptr = new TransientString (culture);
			using var resource_ptr = new TransientString (resource);
			using var path_ptr = new TransientString (path_size);
			var rv = xamarin_locate_assembly_resource (assembly_name_ptr, culture_ptr, resource_ptr, path_ptr, path_size) != 0;
			if (rv)
				path = (string?) path_ptr;
			return path is not null;
		}

		static Assembly? ResolvingEventHandler (AssemblyLoadContext sender, AssemblyName assemblyName)
		{
			if (xamarin_locate_assembly_resource (assemblyName.Name!, assemblyName.CultureName, assemblyName.Name + ".dll", out var path))
				return sender.LoadFromAssemblyPath (path);
			return null;
		}

		static unsafe delegate* unmanaged<IntPtr, void> UnhandledExceptionPropagationHandler (Exception exception, RuntimeMethodHandle lastMethod, out IntPtr context)
		{
			var exceptionHandler = (delegate* unmanaged<IntPtr, void>) options->unhandled_exception_handler;
			context = AllocGCHandle (exception);
			return exceptionHandler;
		}

		// Size: 2 pointers
		internal struct TrackedObjectInfo {
			public IntPtr Handle;
			public NSObject.Flags Flags;
		}

		internal static GCHandle CreateTrackingGCHandle (NSObject obj, IntPtr handle)
		{
			var gchandle = ObjectiveCMarshal.CreateReferenceTrackingHandle (obj, out var info);

			unsafe {
				TrackedObjectInfo* tracked_info;
				fixed (void* ptr = info)
					tracked_info = (TrackedObjectInfo *) ptr;
				tracked_info->Handle = handle;
				tracked_info->Flags = obj.FlagsInternal;
				obj.tracked_object_info = tracked_info;

				log_coreclr ($"GetOrCreateTrackingGCHandle ({obj.GetType ().FullName}, 0x{handle.ToString ("x")}) => Info=0x{((IntPtr) tracked_info).ToString ("x")} Flags={tracked_info->Flags} Created new");
			}

			return gchandle;
		}

		// See  "Toggle-ref support for CoreCLR" in coreclr-bridge.m for more information.
		internal static void RegisterToggleReferenceCoreCLR (NSObject obj, IntPtr handle, bool isCustomType)
		{
			unsafe {
				TrackedObjectInfo* tracked_info = obj.tracked_object_info;
				tracked_info->Flags = obj.FlagsInternal;

				log_coreclr ($"RegisterToggleReferenceCoreCLR ({obj.GetType ().FullName}, 0x{handle.ToString ("x")}, {isCustomType}) => Info=0x{((IntPtr) tracked_info).ToString ("x")} Flags={tracked_info->Flags}");
			}

			// Make sure the GCHandle we have is a weak one for custom types.
			if (isCustomType)
				xamarin_switch_gchandle (handle, true);
		}

		static unsafe MonoObject* CreateException (ExceptionType type, IntPtr arg0)
		{
			Exception rv;
			var str0 = Marshal.PtrToStringAuto (arg0);

			switch (type) {
			case ExceptionType.System_Exception:
				rv = new System.Exception (str0);
				break;
			case ExceptionType.System_InvalidCastException:
				rv = new System.InvalidCastException (str0);
				break;
			case ExceptionType.System_EntryPointNotFoundException:
				rv = new System.EntryPointNotFoundException (str0);
				break;
			case ExceptionType.System_OutOfMemoryException:
				rv = new System.OutOfMemoryException ();
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (type));
			}

			return (MonoObject*) GetMonoObject (rv);
		}

		// Returns a retained MonoObject. Caller must release.
		static IntPtr FindAssembly (IntPtr assembly_name)
		{
			var path = Marshal.PtrToStringAuto (assembly_name);
			var name = Path.GetFileNameWithoutExtension (path);

			if (path is null)
				throw new ArgumentOutOfRangeException ($"Invalid assembly name pointer: 0x{assembly_name.ToString ("x")}");

			log_coreclr ($"Runtime.FindAssembly (0x{assembly_name.ToString ("x")} = {name})");

			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies ()) {
				log_coreclr ($"    Assembly from app domain: {asm.GetName ().Name}");
				if (asm.GetName ().Name == name) {
					log_coreclr ($"        Match!");
					return GetMonoObject (asm);
				}
			}

			log_coreclr ($"    Did not find the assembly in the app domain's loaded assemblies. Will try to load it.");

			var loadedAssembly = Assembly.LoadFrom (path);
			if (loadedAssembly is not null) {
				log_coreclr ($"    Loaded {loadedAssembly.GetName ().Name}");
				return GetMonoObject (loadedAssembly);
			}

			log_coreclr ($"    Found no assembly named {name}");

			throw new InvalidOperationException ($"Could not find any assemblies named {name}");
		}

		static unsafe void SetPendingException (MonoObject* exception_obj)
		{
			var exc = (Exception?) GetMonoObjectTarget (exception_obj);
			log_coreclr ($"Runtime.SetPendingException ({exc})");
			ObjectiveCMarshal.SetMessageSendPendingException (exc);
		}

		unsafe static sbyte IsClassOfType (MonoObject *typeobj, TypeLookup match)
		{
			var rv = IsClassOfType ((Type?) GetMonoObjectTarget (typeobj), match);
			return (sbyte) (rv ? 1 : 0);
		}

		static bool IsClassOfType (Type? type, TypeLookup match)
		{
			var rv = false;

			if (type is null)
				return false;

			switch (match) {
			case TypeLookup.System_Array:
				rv = type.IsArray;
				break;
			case TypeLookup.System_String:
				rv = type == typeof (System.String);
				break;
			case TypeLookup.System_IntPtr:
				rv = type == typeof (System.IntPtr);
				break;
			case TypeLookup.Foundation_NSNumber:
				rv = typeof (Foundation.NSNumber).IsAssignableFrom (type);
				break;
			case TypeLookup.Foundation_NSObject:
				rv = typeof (Foundation.NSObject).IsAssignableFrom (type);
				break;
			case TypeLookup.Foundation_NSString:
				rv = typeof (Foundation.NSString).IsAssignableFrom (type);
				break;
			case TypeLookup.Foundation_NSValue:
				rv = typeof (Foundation.NSValue).IsAssignableFrom (type);
				break;
			case TypeLookup.ObjCRuntime_INativeObject:
				rv = typeof (ObjCRuntime.INativeObject).IsAssignableFrom (type);
				break;
			case TypeLookup.ObjCRuntime_NativeHandle:
				rv = typeof (ObjCRuntime.NativeHandle).IsAssignableFrom (type);
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (type));
			}

			log_coreclr ($"IsClassOfType ({type}, {match}) => {rv}");

			return rv;
		}

		static unsafe MonoObject* LookupType (TypeLookup type)
		{
			Type rv;

			switch (type) {
			case TypeLookup.Foundation_NSNumber:
				rv = typeof (Foundation.NSNumber);
				break;
			case TypeLookup.Foundation_NSValue:
				rv = typeof (Foundation.NSValue);
				break;
			case TypeLookup.System_String:
				rv = typeof (string);
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (type));
			}

			log_coreclr ($"LookupType ({type}) => {rv}");

			return (MonoObject*) GetMonoObject (rv);
		}

		static unsafe MonoObject* GetElementClass (MonoObject* classobj)
		{
			var type = (Type?) GetMonoObjectTarget (classobj);
			return (MonoObject*) GetMonoObject (type?.GetElementType ());
		}

		static unsafe MonoObject* GetNullableElementType (MonoObject* typeobj)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			var elementType = type.GetGenericArguments () [0];
			return (MonoObject*) GetMonoObject (elementType);
		}

		static IntPtr CreateGCHandle (IntPtr gchandle, GCHandleType type)
		{
			// It's valid to create a GCHandle to a null value.
			object? obj = null;
			if (gchandle != IntPtr.Zero)
				obj = GetGCHandleTarget (gchandle);
			return AllocGCHandle (obj, type);
		}

		static void FreeGCHandle (IntPtr gchandle)
		{
			GCHandle.FromIntPtr (gchandle).Free ();
		}

		[DllImport ("__Internal")]
		static extern void xamarin_switch_gchandle (IntPtr obj, [MarshalAs (UnmanagedType.I1)] bool to_weak);

		// Returns a retained MonoObject. Caller must release.
		static IntPtr GetMonoObject (IntPtr gchandle)
		{
			return GetMonoObject (GetGCHandleTarget (gchandle));
		}

		// Returns a retained MonoObject. Caller must release.
		static IntPtr GetMonoObject (object? obj)
		{
			if (obj is null)
				return IntPtr.Zero;

			return GetMonoObjectImpl (obj);
		}

		// Returns a retained MonoObject. Caller must release.
		static IntPtr GetMonoObjectImpl (object obj)
		{
			var handle = AllocGCHandle (obj);

			var mobj = new MonoObject ();
			mobj.GCHandle = handle;
			mobj.ReferenceCount = 1;
			mobj.StructValue = WriteStructure (obj);

			IntPtr rv = MarshalStructure (mobj);

			log_coreclr ($"GetMonoObjectImpl ({obj.GetType ()}) => 0x{rv.ToString ("x")} => GCHandle=0x{handle.ToString ("x")}");

#if TRACK_MONOOBJECTS
			// Only capture the stack trace if requested explicitly, it has a very significant perf hit (monotouch-test is 3x slower).
			if (!track_monoobject_with_stacktraces.HasValue)
				track_monoobject_with_stacktraces = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("MONOOBJECT_TRACKING_WITH_STACKTRACES"));
			xamarin_bridge_log_monoobject (rv, track_monoobject_with_stacktraces.Value ? Environment.StackTrace : null);
#endif

			return rv;
		}

		static unsafe object? GetMonoObjectTarget (MonoObject* mobj)
		{
			return GetMonoObjectTarget ((IntPtr) mobj);
		}

		static object? GetMonoObjectTarget (MonoObjectPtr mobj)
		{
			if (mobj == IntPtr.Zero)
				return null;

			unsafe {
				MonoObject *monoobj = (MonoObject *) mobj;
				return GetGCHandleTarget (monoobj->GCHandle);
			}
		}

#if TRACK_MONOOBJECTS
		[DllImport ("__Internal", CharSet = CharSet.Auto)]
		static extern void xamarin_bridge_log_monoobject (IntPtr mono_object, string stack_trace);
#endif

		static IntPtr MarshalStructure<T> (T value) where T: struct
		{
			var rv = Marshal.AllocHGlobal (Marshal.SizeOf<T> ());
			StructureToPtr (value, rv);
			return rv;
		}

		static void StructureToPtr (object? obj, IntPtr ptr)
		{
			if (obj is null)
				return;

			var structType = obj.GetType ();
			// Unwrap enums, Marshal.StructureToPtr complains they're not blittable (https://github.com/xamarin/xamarin-macios/issues/15744)
			if (structType.IsEnum) {
				structType = Enum.GetUnderlyingType (structType);
				obj = Convert.ChangeType (obj, structType);
			}

			if (obj is bool b) {
				// Only write a single byte for bools
				Marshal.WriteByte (ptr, b ? (byte) 1 : (byte) 0);
			} else {
				Marshal.StructureToPtr (obj, ptr, false);
			}
		}

		static IntPtr WriteStructure (object obj)
		{
			if (obj is null)
				return IntPtr.Zero;

			if (!obj.GetType ().IsValueType)
				return IntPtr.Zero;

			var structType = obj.GetType ();
			// Unwrap enums
			if (structType.IsEnum) {
				structType = Enum.GetUnderlyingType (structType);
				obj = Convert.ChangeType (obj, structType);
			}

			var size = SizeOf (obj.GetType ());
			var output = Marshal.AllocHGlobal (size);

			StructureToPtr (obj, output);

			log_coreclr ($"WriteStructure ({obj}) => Pointer: 0x{output.ToString ("x")} Size: {size}");

			return output;
		}

		static IntPtr GetAssemblyName (IntPtr gchandle)
		{
			var asm = (Assembly?) GetGCHandleTarget (gchandle);
			return Marshal.StringToHGlobalAuto (asm?.GetName ()?.Name);
		}

		static IntPtr GetAssemblyLocation (IntPtr gchandle)
		{
			var asm = (Assembly?) GetGCHandleTarget (gchandle);
			return Marshal.StringToHGlobalAuto (asm?.Location);
		}

		static void SetFlagsForNSObject (IntPtr gchandle, byte flags)
		{
			var obj = (NSObject) GetGCHandleTarget (gchandle)!;
			obj.FlagsInternal = (NSObject.Flags) flags;
		}

		static byte GetFlagsForNSObject (IntPtr gchandle)
		{
			var obj = (NSObject) GetGCHandleTarget (gchandle)!;
			return (byte) obj.FlagsInternal;
		}

		static unsafe MonoObject* GetMethodDeclaringType (MonoObject *mobj)
		{
			var method = (MethodBase) GetMonoObjectTarget (mobj)!;
			return (MonoObject *) GetMonoObject (method.DeclaringType);
		}

		static IntPtr ObjectGetType (MonoObjectPtr mobj)
		{
			var obj = GetMonoObjectTarget (mobj);
			if (obj is null) {
				log_coreclr ($"ObjectGetType (0x{mobj.ToString ("x")}) => null object");
				return IntPtr.Zero;
			}
			return GetMonoObject (obj.GetType ());
		}

		unsafe static sbyte IsDelegate (MonoObject* typeobj)
		{
			var type = (Type?) GetMonoObjectTarget (typeobj);
			if (type is null)
				return 0;
			var rv = typeof (MulticastDelegate).IsAssignableFrom (type);
			log_coreclr ($"IsDelegate ({type.FullName}) => {rv}");
			return (sbyte) (rv ? 1 : 0);
		}

		static sbyte IsInstance (MonoObjectPtr mobj, MonoObjectPtr mtype)
		{
			var obj = GetMonoObjectTarget (mobj);
			if (obj is null)
				return 0;

			var type = (Type) GetMonoObjectTarget (mtype)!;
			var rv = type.IsAssignableFrom (obj.GetType ());

			log_coreclr ($"IsInstance ({obj.GetType ()}, {type})");

			return (sbyte) (rv ? 1 : 0);
		}

		static unsafe IntPtr GetMethodSignature (MonoObject* methodobj)
		{
			var method = (MethodBase) GetMonoObjectTarget (methodobj)!;
			var parameters = method.GetParameters ()!;
			var parameterCount = parameters.Length;
			var rv = Marshal.AllocHGlobal (sizeof (MonoMethodSignature) + sizeof (MonoObjectPtr) * parameterCount);

			MonoMethodSignature* signature = (MonoMethodSignature *) rv;
			signature->Method = methodobj;
			xamarin_mono_object_retain (methodobj);
			signature->ParameterCount = parameterCount;
			signature->ReturnType = (MonoObject *) GetMonoObject (GetMethodReturnType (method));

			MonoObject** mparams = &signature->Parameters;
			for (var i = 0; i < parameterCount; i++) {
				var p = parameters [i];
				mparams [i] = (MonoObject *) GetMonoObject (p.ParameterType);
			}

			return rv;
		}

		static Type? GetMethodReturnType (MethodBase method)
		{
			if (method is MethodInfo minfo)
				return minfo.ReturnType;

			// Constructors return the instance that was created, which is the declaring type of the constructor.
			if (method is ConstructorInfo cinfo)
				return cinfo.DeclaringType;

			return null;
		}

		unsafe static IntPtr ClassGetNamespace (MonoObject *typeobj)
		{
			var type = (Type?) GetMonoObjectTarget (typeobj);
			var rv = type?.Namespace;
			return Marshal.StringToHGlobalAuto (rv);
		}

		unsafe static IntPtr ClassGetName (MonoObject *typeobj)
		{
			var type = (Type?) GetMonoObjectTarget (typeobj);
			var rv = type?.Name;
			return Marshal.StringToHGlobalAuto (rv);
		}

		// This should work like mono_class_from_mono_type.
		static unsafe MonoObject* TypeToClass (MonoObject* typeobj)
		{
			var type = (Type?) GetMonoObjectTarget (typeobj);
			if (type?.IsByRef == true)
				type = type.GetElementType ();
			return (MonoObject *) GetMonoObject (type);
		}

		static unsafe int SizeOf (MonoObject* typeobj)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			return SizeOf (type);
		}

		static int SizeOf (Type type)
		{
			if (type.IsEnum) // https://github.com/dotnet/runtime/issues/12258
				type = Enum.GetUnderlyingType (type);
			return Marshal.SizeOf (type);
		}

		static unsafe MonoObject* InvokeMethod (MonoObject* methodobj, MonoObject* instanceobj, IntPtr native_parameters)
		{
			var method = (MethodBase) GetMonoObjectTarget (methodobj)!;
			var instance = GetMonoObjectTarget (instanceobj)!;
			var rv = InvokeMethod (method, instance, native_parameters);
			return (MonoObject *) GetMonoObject (rv);
		}

		// Return value: NULL or a MonoObject* that must be released with xamarin_mono_object_safe_release.
		// Any MonoObject* ref parameters must also be retained and must be released with xamarin_mono_object_release.
		static object? InvokeMethod (MethodBase method, object instance, IntPtr native_parameters)
		{
			var methodParameters = method.GetParameters ();
			var parameters = new object? [methodParameters.Length];
			var inputParameters = new object? [methodParameters.Length];
			var nativeParameters = new IntPtr [methodParameters.Length];

			// Copy native array of void* to managed array of IntPtr to make the subsequent code simpler.
			unsafe {
				IntPtr* nativeParams = (IntPtr*) native_parameters;
				for (var i = 0; i < methodParameters.Length; i++) {
					nativeParameters [i] = nativeParams [i];
				}
			}

			// Log our input
			log_coreclr ($"InvokeMethod ({method.DeclaringType!.FullName}::{method}, {(instance is null ? "<null>" : instance.GetType ().FullName)}, 0x{native_parameters.ToString ("x")})");
			for (var i = 0; i < methodParameters.Length; i++) {
				var nativeParam = nativeParameters [i];
				var p = methodParameters [i]!;
				var paramType = p.ParameterType;
				if (paramType.IsByRef)
					paramType = paramType.GetElementType ()!;
				log_coreclr ($"    Argument #{i + 1}: Type = {p.ParameterType!.FullName} IsByRef: {p.ParameterType!.IsByRef} IsOut: {p.IsOut} IsClass: {paramType.IsClass} IsInterface: {paramType.IsInterface} NativeParameter: 0x{nativeParam.ToString ("x")}");
			}

			// Process the arguments, and convert to what MethodBase.Invoke expects
			for (var i = 0; i < methodParameters.Length; i++) {
				var nativeParam = nativeParameters [i];
				var p = methodParameters [i];
				var paramType = p.ParameterType!;
				var isByRef = paramType.IsByRef;
				if (isByRef)
					paramType = paramType.GetElementType ()!;
				log_coreclr ($"    Marshalling #{i + 1}: IntPtr => 0x{nativeParam.ToString ("x")} => {p.ParameterType.FullName}");

				if (paramType == typeof (IntPtr)) {
					log_coreclr ($"        IntPtr");
					if (isByRef) {
						if (p.IsOut) {
							parameters [i] = Marshal.AllocHGlobal (IntPtr.Size);
						} else {
							parameters [i] = nativeParam == IntPtr.Zero ? IntPtr.Zero : Marshal.ReadIntPtr (nativeParam);
						}
					} else {
						parameters [i] = nativeParam == IntPtr.Zero ? IntPtr.Zero : Marshal.ReadIntPtr (nativeParam);
					}
					log_coreclr_render ("            => {0}", parameters [i]);
				} else if (paramType.IsPointer) {
					log_coreclr ($"        IsPointer nativeParam: 0x{nativeParam.ToString ("x")} ParameterType: {paramType}");
					if (nativeParam != IntPtr.Zero)
						parameters [i] = nativeParam;
					log_coreclr_render ("            => {0}", parameters [i]);
				} else if (paramType.IsClass || paramType.IsInterface || (paramType.IsValueType && IsNullable (paramType))) {
					log_coreclr ($"        IsClass/IsInterface/IsNullable IsByRef: {isByRef} IsOut: {p.IsOut} ParameterType: {paramType}");
					if (nativeParam != IntPtr.Zero) {
						unsafe {
							MonoObject* mono_obj = (MonoObject *) nativeParam;
							// dereference if it's a byref type
							if (isByRef)
								mono_obj = *(MonoObject**) mono_obj;
							// get the object
							parameters [i] = GetMonoObjectTarget (mono_obj);
						}
					}
					log_coreclr_render ("            => {0}", parameters [i]);
				} else if (paramType.IsValueType) {
					log_coreclr ($"        IsValueType IsByRef: {isByRef} IsOut: {p.IsOut} nativeParam: 0x{nativeParam.ToString ("x")} ParameterType: {paramType}");
					if (nativeParam != IntPtr.Zero) {
						// We need to unwrap nullable types and enum types to their underlying struct type.
						var structType = paramType;
						Type? enumType = null;
						if (IsNullable (structType))
							structType = Nullable.GetUnderlyingType (structType)!;
						if (structType.IsEnum) {
							enumType = structType;
							structType = Enum.GetUnderlyingType (structType)!;
						}
						// convert the pointer to the corresponding structure
						var vt = PtrToStructure (nativeParam, structType)!;
						// convert the structure to the enum type if that's what we need
						if (enumType is not null)
							vt = Enum.ToObject (enumType, vt);
						parameters [i] = vt;
					}
					log_coreclr_render ("            => {0}", parameters [i]);
				} else {
					throw ErrorHelper.CreateError (8037, Errors.MX8037 /* Don't know how to marshal the parameter of type {p.ParameterType.FullName} for parameter {p.Name} in call to {method} */, p.ParameterType.FullName, p.Name, method);
				}
			}

			// Make a copy of the array of parameters, so that we can figure out if there were any ref parameters that the method modified.
			parameters.CopyTo (inputParameters, 0);

			// Call the actual method
			log_coreclr ($"    Invoking...");

			object? rv = null;

			try {
				rv = method.Invoke (instance, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, parameters, null);
			} catch (TargetInvocationException tie)	{
				var ex = tie.InnerException ?? tie;
				// This will re-throw the original exception and preserve the stacktrace.
				ExceptionDispatchInfo.Capture (ex).Throw ();
			} catch (Exception e) {
				throw ErrorHelper.CreateError (8042, e, Errors.MX8042 /* An exception occurred while trying to invoke the function {0}: {1} */, GetMethodFullName (method), e.Message);
			}

			// Copy any byref parameters back out again
			var byrefParameterCount = 0;
			for (var i = 0; i < methodParameters.Length; i++) {
				var p = methodParameters [i];
				if (!p.IsOut && !p.ParameterType.IsByRef)
					continue;

				byrefParameterCount++;

				var parameterType = p.ParameterType.GetElementType ()!;
				var isMonoObject = parameterType.IsClass || parameterType.IsInterface || (parameterType.IsValueType && IsNullable (parameterType));

				var nativeParam = nativeParameters [i];

				log_coreclr_render ($"    Marshalling #{i + 1} back (Type: {p.ParameterType.FullName}) nativeParam: 0x{nativeParam.ToString ("x")} value: {{0}}", parameters [i]);

				if (nativeParam == IntPtr.Zero) {
					log_coreclr ($"    No output pointer was provided.");
					continue;
				}

				if (parameters [i] == inputParameters [i]) {
					log_coreclr ($"        The argument didn't change, no marshalling required");
					if (parameters [i] is not null && parameterType != typeof (IntPtr) && isMonoObject) {
						// byref parameters must be retained
						xamarin_mono_object_retain (Marshal.ReadIntPtr (nativeParam));
					}
					continue;
				}

				if (parameterType == typeof (IntPtr)) {
					Marshal.WriteIntPtr (nativeParam, (IntPtr) parameters [i]!);
					log_coreclr ($"        IntPtr");
				} else if (isMonoObject) {
					var ptr = GetMonoObject (parameters [i]);
					Marshal.WriteIntPtr (nativeParam, ptr);
					log_coreclr ($"        IsClass/IsInterface/IsNullable  MonoObject: 0x{ptr.ToString ("x")}");
				} else if (parameterType.IsValueType) {
					StructureToPtr (parameters [i], nativeParam);
					log_coreclr ($"        IsValueType");
				} else {
					throw ErrorHelper.CreateError (8038, Errors.MX8038 /* Don't know how to marshal back the parameter of type {p.ParameterType.FullName} for parameter {p.Name} in call to {method} */, p.ParameterType.FullName, p.Name, method);
				}
			}

			// we're done!
			log_coreclr_render ($"    Invoke complete with {byrefParameterCount} ref parameters and return value: {{0}}", rv);

			return rv;
		}

		static unsafe IntPtr StringToUtf8 (MonoObject* obj)
		{
			var str = (string?) GetMonoObjectTarget (obj);
			return Marshal.StringToHGlobalAuto (str);
		}

		static unsafe MonoObject* NewString (IntPtr text)
		{
			return (MonoObject *) GetMonoObject (Marshal.PtrToStringAuto (text));
		}

		static unsafe MonoObject* CreateArray (MonoObject* typeobj, ulong elements)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			var obj = Array.CreateInstance (type, (int) elements);
			return (MonoObject*) GetMonoObject (obj);
		}

		static unsafe ulong GetArrayLength (MonoObject* obj)
		{
			var array = (Array) GetMonoObjectTarget (obj)!;
			return (ulong) array.Length;
		}

		static unsafe void SetArrayObjectValue (MonoObject *arrayobj, ulong index, MonoObject *mobj)
		{
			var array = (Array) GetMonoObjectTarget (arrayobj)!;
			var obj = GetMonoObjectTarget (mobj);
			array.SetValue (obj, (long) index);
		}

		static unsafe void SetArrayStructValue (MonoObject *arrayobj, ulong index, MonoObject *typeobj, IntPtr valueptr)
		{
			var array = (Array) GetMonoObjectTarget (arrayobj)!;
			var elementType = (Type) GetMonoObjectTarget (typeobj)!;
			var obj = Box (elementType, valueptr);
			array.SetValue (obj, (long) index);
		}

		static unsafe MonoObject* GetArrayObjectValue (MonoObject* arrayobj, ulong index)
		{
			var array = (Array) GetMonoObjectTarget (arrayobj)!;
			var obj = array.GetValue ((long) index);
			return (MonoObject *) GetMonoObject (obj);
		}

		static unsafe MonoObject* Box (MonoObject* typeobj, IntPtr value)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			var rv = Box (type, value);
			return (MonoObject *) GetMonoObject (rv);
		}

		static object? Box (Type type, IntPtr value)
		{
			var structType = type;
			Type? enumType = null;

			// We can have a nullable enum value
			if (IsNullable (structType)) {
				if (value == IntPtr.Zero)
					return null;

				structType = Nullable.GetUnderlyingType (structType)!;
			}

			if (structType.IsEnum) {
				// Change to underlying enum type
				enumType = structType;
				structType = Enum.GetUnderlyingType (structType)!;
			}

			var boxed = PtrToStructure (value, structType)!;
			if (enumType is not null) {
				// Convert to enum value
				boxed = Enum.ToObject (enumType, boxed);
			}

			return boxed;
		}

		static unsafe sbyte IsNullable (MonoObject* type)
		{
			var rv = IsNullable ((Type) GetMonoObjectTarget (type)!);
			return (sbyte) (rv ? 1 : 0);
		}

		static bool IsNullable (Type type)
		{
			if (Nullable.GetUnderlyingType (type) is not null)
				return true;

			if (type.IsGenericType && type.GetGenericTypeDefinition () == typeof (Nullable<>))
				return true;

			return false;
		}

		unsafe static sbyte IsByRef (MonoObject *typeobj)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			var rv = type.IsByRef;
			return (sbyte) (rv ? 1 : 0);
		}

		unsafe static sbyte IsValueType (MonoObject *typeobj)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			var rv = type.IsValueType;
			return (sbyte) (rv ? 1 : 0);
		}

		unsafe static sbyte IsEnum (MonoObject *typeobj)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			var rv = type.IsEnum;
			return (sbyte) (rv ? 1 : 0);
		}

		static unsafe MonoObject* GetEnumBaseType (MonoObject* typeobj)
		{
			var type = (Type) GetMonoObjectTarget (typeobj)!;
			return (MonoObject*) GetMonoObject (GetEnumBaseType (type));
		}

		static Type GetEnumBaseType (Type type)
		{
			return type.GetEnumUnderlyingType ();
		}

		static object? PtrToStructure (IntPtr ptr, Type type)
		{
			if (ptr == IntPtr.Zero)
				return null;

			// Only read a single byte for bools.
			if (type == typeof (bool))
				return Marshal.ReadByte (ptr) != 0;

			return Marshal.PtrToStructure (ptr, type);
		}

		/* Managed version of mono_g_hash_table The mono_g_hash_table can be configured
		   in several ways, depending on whether the GC should track keys,
		   values or both, but in our case we only want the GC to track the
		   values (MONO_HASH_VALUE_GC).
		   Semantics:
		   * Custom compare and hash functions from native code.
		   * Keep a strong reference to the values of the hash table.
		 */
		class MonoHashTable : IEqualityComparer<IntPtr> {
			Dictionary<IntPtr, object?> Table;
			HashFunc Hash;
			EqualityFunc Compare;

			delegate uint HashFunc (IntPtr ptr);
			delegate bool EqualityFunc (IntPtr a, IntPtr b);

			public MonoHashTable (IntPtr hash_func, IntPtr compare_func)
			{
				Table = new Dictionary<IntPtr, object?> ();
				Hash = Marshal.GetDelegateForFunctionPointer<HashFunc> (hash_func);
				Compare = Marshal.GetDelegateForFunctionPointer<EqualityFunc> (compare_func);
			}

			public void Insert (IntPtr key, object? obj)
			{
				Table [key] = obj;
			}

			public object? Lookup (IntPtr key)
			{
				if (Table.TryGetValue (key, out var value))
					return value;
				return null;
			}

			bool IEqualityComparer<IntPtr>.Equals (IntPtr x, IntPtr y)
			{
				return Compare (x, y);
			}

			int IEqualityComparer<IntPtr>.GetHashCode (IntPtr obj)
			{
				unchecked {
					return (int) Hash (obj);
				}
			}
		}

		static unsafe MonoObject* CreateMonoHashTable (IntPtr hash_method, IntPtr compare_method, int type)
		{
			if (type != 2 /* MONO_HASH_VALUE_GC */)
				throw new NotSupportedException ($"Unsupported hash table type: {type}");

			return (MonoObject*) GetMonoObject (new MonoHashTable (hash_method, compare_method));
		}

		static unsafe void MonoHashTableInsert (MonoObject* tableobj, IntPtr key, MonoObject* valueobj)
		{
			var table = (MonoHashTable) GetMonoObjectTarget (tableobj)!;
			var value = GetMonoObjectTarget (valueobj);
			table.Insert (key, value);
		}

		static unsafe MonoObject* MonoHashTableLookup (MonoObject* tableobj, IntPtr key)
		{
			var dict = (MonoHashTable) GetMonoObjectTarget (tableobj)!;
			return (MonoObject*) GetMonoObject (dict.Lookup (key));
		}

		static unsafe IntPtr GetMethodFullName (MonoObject* mobj)
		{
			return Marshal.StringToHGlobalAuto (GetMethodFullName ((MethodBase) GetMonoObjectTarget (mobj)!));
		}

		[return: NotNullIfNotNull (nameof (method))]
		static string? GetMethodFullName (MethodBase? method)
		{
			if (method is null)
				return null;

			// The return value is used in error messages, so there's not a
			// specific format we have to return, it just has to be helpful.

			var returnType = (method as MethodInfo)?.ReturnType ?? typeof (void);
			var sb = new StringBuilder ();
			sb.Append (returnType.FullName);
			sb.Append (' ');
			sb.Append (method.DeclaringType!.FullName);
			sb.Append ('.');
			sb.Append (method.Name);
			sb.Append (' ');
			sb.Append ('(');
			var parameters = method.GetParameters ();
			for (var i = 0; i < parameters.Length; i++) {
				if (i > 0)
					sb.Append (", ");
				sb.Append (parameters [i].ParameterType.FullName);
			}
			sb.Append (')');
			return sb.ToString ();
		}

		[DllImport ("__Internal")]
		static extern void xamarin_mono_object_retain (IntPtr mono_object);

		[DllImport ("__Internal")]
		unsafe static extern void xamarin_mono_object_retain (MonoObject* mono_object);
	}
}

#endif // NET && !COREBUILD
