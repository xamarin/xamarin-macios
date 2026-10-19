//
// Runtime.cs: Mac/iOS shared runtime code
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2013 Xamarin Inc.

// #define LOG_TRIMMABLE_TYPEMAP

#nullable enable

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using CoreFoundation;
using Registrar;

#if MONOMAC
using AppKit;
#endif

namespace ObjCRuntime {

	/// <summary>Provides information about the runtime.</summary>
	public partial class Runtime {
#if !COREBUILD
#pragma warning disable 8618 // "Non-nullable field '...' must contain a non-null value when exiting constructor. Consider declaring the field as nullable.": we make sure through other means that these will never be null
		static Dictionary<IntPtrTypeValueTuple, Delegate> block_to_delegate_cache;
		static Dictionary<Type, ConstructorInfo> intptr_ctor_cache;
		static Dictionary<Type, ConstructorInfo> intptr_bool_ctor_cache;
		internal static Dictionary<IntPtr, Dictionary<IntPtr, bool>> protocol_cache;
		static ConditionalWeakTable<Delegate, BlockCollector> block_lifetime_table;

		static List<object> delegates;
		static List<Assembly> assemblies;
		static Dictionary<IntPtr, GCHandle> object_map;
		static Dictionary<IntPtr, bool> usertype_cache;
		static object lock_obj;
		static IntPtr NSObjectClass;
		static bool initialized;

		internal static IntPtrEqualityComparer IntPtrEqualityComparer;
		internal static TypeEqualityComparer TypeEqualityComparer;
		internal static UInt64EqualityComparer UInt64EqualityComparer;

		internal static DynamicRegistrar Registrar;
#pragma warning restore 8618

		internal const uint INVALID_TOKEN_REF = 0xFFFFFFFF;

#pragma warning disable 649 // Field 'X' is never assigned to, and will always have its default value
		internal unsafe struct MTRegistrationMap {
			public IntPtr product_hash;
			public MTAssembly* assemblies;
			public MTClassMap* map;
			public MTFullTokenReference* full_token_references;
			public MTManagedClassMap* skipped_map;
			public MTProtocolWrapperMap* protocol_wrapper_map;
			public MTProtocolMap protocol_map;
			public int assembly_count;
			public int map_count;
			public int full_token_reference_count;
			public int skipped_map_count;
			public int protocol_wrapper_count;
			public int protocol_count;
		}
#pragma warning restore 649

#if __TVOS__
		internal const string PlatformName = "tvOS";
		internal const string ProductName = "Microsoft." + PlatformName;
#elif __MACCATALYST__
		internal const string PlatformName = "Mac Catalyst";
		internal const string ProductName = "Microsoft.MacCatalyst";
#elif __IOS__
		internal const string PlatformName = "iOS";
		internal const string ProductName = "Microsoft." + PlatformName;
#elif __MACOS__
		internal const string PlatformName = "macOS";
		internal const string ProductName = "Microsoft." + PlatformName;
#else
#error Undetermined platform name
#endif
		internal const string AssemblyName = ProductName + ".dll";

		static Thread? mainThread;

		[Flags]
		internal enum MTTypeFlags : uint {
			None = 0,
			CustomType = 1,
			UserType = 2,
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal unsafe struct MTFullTokenReference {
			public uint assembly_index;
			public uint module_token;
			public uint token;
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal struct MTClassMap {
			public IntPtr handle;
			public uint type_reference;
			public MTTypeFlags flags;
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal struct MTManagedClassMap {
			public uint skipped_reference; // implied token type: TypeDef
			public uint actual_reference; // implied token type: TypeDef
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal struct MTProtocolWrapperMap {
			public uint protocol_token;
			public uint wrapper_token;
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal unsafe struct MTProtocolMap {
			public uint* protocol_tokens;
			public IntPtr* protocols;
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal unsafe struct MTAssembly {
			public IntPtr name;
			public IntPtr mvid;
		}

		/* Keep Delegates, Trampolines and InitializationOptions in sync with monotouch-glue.m */
#pragma warning disable 649 // Field 'X' is never assigned to, and will always have its default value
		internal struct Trampolines {
			public IntPtr tramp;
			public IntPtr stret_tramp;
			public IntPtr fpret_single_tramp;
			public IntPtr fpret_double_tramp;
			public IntPtr release_tramp;
			public IntPtr retain_tramp;
			public IntPtr static_tramp;
			public IntPtr ctor_tramp;
			public IntPtr static_fpret_single_tramp;
			public IntPtr static_fpret_double_tramp;
			public IntPtr static_stret_tramp;
			public IntPtr long_tramp;
			public IntPtr static_long_tramp;
#if MONOMAC
			public IntPtr copy_with_zone_1;
			public IntPtr copy_with_zone_2;
#endif
			public IntPtr get_gchandle_tramp;
			public IntPtr set_gchandle_tramp;
			public IntPtr get_gchandle_flags_tramp;
			public IntPtr set_gchandle_flags_tramp;
			public IntPtr retainWeakReference_tramp;
			public IntPtr get_nsobject_data_tramp;
		}
#pragma warning restore 649

		[Flags]
		internal enum InitializationFlags : int {
			IsPartialStaticRegistrar = 0x01,
			IsManagedStaticRegistrar = 0x02,
			IsTrimmableStaticRegistrar = 0x04,
			/* unused				= 0x08,*/
			IsSimulator = 0x10,
			IsCoreCLR = 0x20,
			IsNativeAOT = 0x40,
		}

		[StructLayout (LayoutKind.Sequential)]
		internal unsafe struct InitializationOptions {
			public int Size;
			public InitializationFlags Flags;
			public Delegates* Delegates;
			public Trampolines* Trampolines;
			public MTRegistrationMap* RegistrationMap;
			public MarshalObjectiveCExceptionMode MarshalObjectiveCExceptionMode;
			public MarshalManagedExceptionMode MarshalManagedExceptionMode;
			IntPtr AssemblyLocations;

			public IntPtr xamarin_objc_msgsend;
			public IntPtr xamarin_objc_msgsend_super;
			public IntPtr xamarin_objc_msgsend_stret;
			public IntPtr xamarin_objc_msgsend_super_stret;
			public IntPtr unhandled_exception_handler;
			public IntPtr reference_tracking_begin_end_callback;
			public IntPtr reference_tracking_is_referenced_callback;
			public IntPtr reference_tracking_tracked_object_entered_finalization;

			public bool IsSimulator {
				get {
					return (Flags & InitializationFlags.IsSimulator) == InitializationFlags.IsSimulator;
				}
			}
		}

		internal static unsafe InitializationOptions* options;

#if !XAMCORE_5_0
		[Obsolete ("Class-handle redirection is no longer supported.", true)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static class ClassHandles {
			internal static unsafe void InitializeClassHandles (IntPtr* map)
			{
			}

			internal static unsafe void SetHandle (int index, NativeHandle* handle, IntPtr* map)
			{
			}
		}
#endif

		[BindingImpl (BindingImplOptions.Optimizable)]
		internal unsafe static bool IsCoreCLR {
			get {
				// The linker may turn calls to this property into a constant
				return (options->Flags.HasFlag (InitializationFlags.IsCoreCLR));
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		internal unsafe static bool IsNativeAOT {
			get {
				// The linker may turn calls to this property into a constant
				return (options->Flags.HasFlag (InitializationFlags.IsNativeAOT));
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		internal unsafe static bool IsManagedStaticRegistrar {
			get {
				// The linker may turn calls to this property into a constant
				return (options->Flags.HasFlag (InitializationFlags.IsManagedStaticRegistrar));
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		internal unsafe static bool IsTrimmableStaticRegistrar {
			get {
				// The linker may turn calls to this property into a constant
				return options->Flags.HasFlag (InitializationFlags.IsTrimmableStaticRegistrar);
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		[FeatureSwitchDefinition ("ObjCRuntime.Runtime.HotReloadCompatible")]
		internal static bool HotReloadCompatible => AppContext.TryGetSwitch ("ObjCRuntime.Runtime.HotReloadCompatible", out var value) && value;

		/// <summary>If dynamic registration is supported.</summary>
		/// <value>If dynamic registration is supported.</value>
		/// <remarks>
		///   <para>At build time the managed linker can in some cases determine whether dynamic registration is required or not, and if not, it can optimize away the code to support dynamic registration (to minimize app size). If support for dynamic registration has been removed by the linker, this property will return false.</para>
		/// </remarks>
		/// <related type="article" href="https://docs.microsoft.com/en-us/xamarin/cross-platform/macios/optimizations#remove-the-dynamic-registrar">Build optimizations (removal of the dynamic registrar)</related>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static bool DynamicRegistrationSupported {
			get {
				// The linker may turn calls to this property into a constant
				return true;
			}
		}

		// The linker may turn calls to this property into a constant
		[BindingImpl (BindingImplOptions.Optimizable)]
		internal static bool UseNSUrlSessionHandler => AppContext.TryGetSwitch ("System.Net.Http.NativeHandler.UseNSUrlSessionHandler", out bool isDefault) ? isDefault : true;

		// The linker may turn calls to this property into a constant
		[BindingImpl (BindingImplOptions.Optimizable)]
		internal static bool UseCFNetworkHandler => AppContext.TryGetSwitch ("System.Net.Http.NativeHandler.UseCFNetworkHandler", out bool isDefault) ? isDefault : false;

		// Issue #25861: when false (the default), NSObjects created via alloc/init are
		// added to the object_map only after 'init' has completed. This way a native
		// 'init' that frees the alloc'd handle (and returns a different one) can't leave
		// a stale pointer to freed memory in the map, which could later be clobbered when
		// the memory is reused by another object. Set this AppContext switch to true to
		// restore the previous behavior (register the object right after 'alloc').
		internal static bool RegisterObjectsBeforeInit => AppContext.TryGetSwitch ("ObjCRuntime.Runtime.RegisterObjectsBeforeInit", out var value) && value;

		// The linker may turn calls to this property into a constant
		/// <summary>Determines whether the debug builds will enforce that calls done to AppKit/UIKit APIs are only issued from the UI thread.</summary>
		/// <remarks>
		///   <para>
		///     On debug builds, the runtime will enforce that calls made to
		///     AppKit/UIKit APIs are only done from the main thread. This is
		///     useful to spot code that could inadvertently use AppKit/UIKit from
		///     a non-UI thread which can corrupt state and could lead to
		///     very hard to debug problems.
		///   </para>
		///   <para>
		///     But sometimes it might be useful to disable this check,
		///     either because you can ensure that AppKit/UIKit is not in use at
		///     this point or because the APIs in question might have later been
		///     relaxed or made thread safe by Apple.
		///   </para>
		/// </remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		internal static bool CheckForIllegalCrossThreadCalls => AppContext.TryGetSwitch ("ObjCRuntime.Runtime.CheckForIllegalCrossThreadCalls", out bool enabled) ? enabled : true;

		internal static bool Initialized {
			get { return initialized; }
		}

#if MONOMAC
		[DllImport (Constants.libcLibrary)]
		unsafe static extern int _NSGetExecutablePath (byte* buf, int* bufsize);

		unsafe static int _NSGetExecutablePath (byte [] buf, ref int bufsize)
		{
			fixed (byte* bufptr = buf)
			fixed (int* bufsizePtr = &bufsize)
				return _NSGetExecutablePath (bufptr, bufsizePtr);
		}
#endif

		[Preserve] // called from native - nativeaot-bridge.m and coreclr-bridge.m.
		[UnmanagedCallersOnly (EntryPoint = "xamarin_objcruntime_runtime_nativeaotinitialize")]
		unsafe static void SafeInitialize (InitializationOptions* options, IntPtr* exception_gchandle)
		{
			*exception_gchandle = IntPtr.Zero;
			try {
				Initialize (options);
			} catch (Exception e) {
				*exception_gchandle = AllocGCHandle (e);
				try {
					Runtime.NSLog ($"Failed to initialize the runtime: {e}");
				} catch {
					// Ignore any exceptions here, we do absolutely not want to leak exceptions to native code (which will crash the process),
					// and if the call to Runtime.NSLog went wrong, then something is seriously wrong, so it's likely nothing is safe to do.
				}
			}
		}

		[Preserve] // called from native - runtime.m.
		[BindingImpl (BindingImplOptions.Optimizable)] // To inline the Runtime.DynamicRegistrationSupported code if possible.
		unsafe static void Initialize (InitializationOptions* options)
		{
#if PROFILE
			var watch = new Stopwatch ();
#endif
			if (options->Size != Marshal.SizeOf<InitializationOptions> ()) {
				var msg = $"Version mismatch between the native {ProductName} runtime and {AssemblyName}. Please reinstall {ProductName}.";
				NSLog (msg);
				throw ErrorHelper.CreateError (8001, msg);
			}

			if (System.Runtime.GCSettings.IsServerGC) {
				var msg = $".NET for {PlatformName} does not support server garbage collection.";
				NSLog (msg);
				throw ErrorHelper.CreateError (8057, msg);
			}

			IntPtrEqualityComparer = new IntPtrEqualityComparer ();
			TypeEqualityComparer = new TypeEqualityComparer ();
			UInt64EqualityComparer = new UInt64EqualityComparer ();

			Runtime.options = options;
			delegates = new List<object> ();
			object_map = new Dictionary<IntPtr, GCHandle> (IntPtrEqualityComparer);
			usertype_cache = new Dictionary<IntPtr, bool> (IntPtrEqualityComparer);
			intptr_ctor_cache = new Dictionary<Type, ConstructorInfo> (TypeEqualityComparer);
			intptr_bool_ctor_cache = new Dictionary<Type, ConstructorInfo> (TypeEqualityComparer);
			block_lifetime_table = new ConditionalWeakTable<Delegate, BlockCollector> ();
			lock_obj = new object ();

			mainThread = Thread.CurrentThread;

#if NET11_0_OR_GREATER
			if (IsTrimmableStaticRegistrar)
				TypeMaps.Initialize ();
#endif

			NSObjectClass = NSObject.InitializeObject ();

			if (DynamicRegistrationSupported) {
				Registrar = new DynamicRegistrar ();
				protocol_cache = new Dictionary<IntPtr, Dictionary<IntPtr, bool>> (IntPtrEqualityComparer);
			}
			RegisterDelegates (options);
			Class.InitializeClass (options);
			InitializePlatform (options);

			IsARM64CallingConvention = GetIsARM64CallingConvention (); // Can only be done after Runtime.Arch is set (i.e. InitializePlatform has been called).

			objc_exception_mode = options->MarshalObjectiveCExceptionMode;
			managed_exception_mode = options->MarshalManagedExceptionMode;

			if (IsCoreCLR)
				InitializeCoreCLRBridge (options);

			initialized = true;
#if PROFILE
			Console.WriteLine ("Runtime.Initialize completed in {0} ms", watch.ElapsedMilliseconds);
#endif
		}

		/// <summary>Assertion to ensure that this call is being done from the UI thread.</summary>
		/// <remarks>
		///   <para>
		///     This method is used internally to ensure that
		///     accesses done to AppKit/UIKit classes and methods are only
		///     performed from the main thread. This is necessary because
		///     the AppKit/UIKit API is not thread-safe and accessing it from
		///     multiple threads will corrupt the application state and will
		///     likely lead to a crash that is hard to identify.
		///   </para>
		///   <para>
		///     This thread check is only done in debug builds.
		///     Release builds have this feature disabled.
		///   </para>
		/// </remarks>
		internal static void EnsureUIThread ()
		{
#if MONOMAC
			var checkForIllegalCrossThreadCalls = AppKit.NSApplication.CheckForIllegalCrossThreadCalls;
#else
			var checkForIllegalCrossThreadCalls = UIKit.UIApplication.CheckForIllegalCrossThreadCalls;
#endif

			if (!checkForIllegalCrossThreadCalls)
				return;

			if (mainThread == Thread.CurrentThread)
				return;

#if MONOMAC
			throw new AppKit.AppKitThreadAccessException ();
#else
			throw new UIKit.UIKitThreadAccessException ();
#endif
		}

#if MONOMAC
		/// <summary>Raised when an assembly is about to be registered with the Objective-C runtime, allowing an application to control whether the assembly's types are registered.</summary>
		/// <remarks>This event is only available on macOS. Set the event argument's <see cref="ObjCRuntime.AssemblyRegistrationEventArgs.Register" /> property to <see langword="false" /> to skip registering the assembly.</remarks>
		public static event AssemblyRegistrationHandler? AssemblyRegistration;

		static bool OnAssemblyRegistration (AssemblyName assembly_name)
		{
			if (AssemblyRegistration is not null) {
				var args = new AssemblyRegistrationEventArgs {
					Register = true,
					AssemblyName = assembly_name,
				};
				AssemblyRegistration (null, args);
				return args.Register;
			}
			return true;
		}
#endif
		static MarshalObjectiveCExceptionMode objc_exception_mode;
		static MarshalManagedExceptionMode managed_exception_mode;

		/// <summary>Raised when an Objective-C exception is about to be marshalled into managed code, allowing the application to choose how the exception is handled.</summary>
		/// <remarks>Handlers can inspect the native exception and set the marshalling mode on the event arguments to control whether a managed exception is thrown.</remarks>
		public static event MarshalObjectiveCExceptionHandler? MarshalObjectiveCException;
		/// <summary>Raised when a managed exception is about to be marshalled into an Objective-C exception, allowing the application to choose how the exception is handled.</summary>
		/// <remarks>Handlers can inspect the managed exception and set the marshalling mode on the event arguments to control how the exception is surfaced to native code.</remarks>
		public static event MarshalManagedExceptionHandler? MarshalManagedException;

		static MarshalObjectiveCExceptionMode OnMarshalObjectiveCException (IntPtr exception_handle, sbyte throwManagedAsDefault)
		{
			if (throwManagedAsDefault != 0 && MarshalObjectiveCException is null)
				return MarshalObjectiveCExceptionMode.ThrowManagedException;

			if (MarshalObjectiveCException is not null) {
				var exception = GetNSObject<NSException> (exception_handle);
				if (exception is null)
					return objc_exception_mode;
				var args = new MarshalObjectiveCExceptionEventArgs (
					exception,
					(throwManagedAsDefault != 0) ? MarshalObjectiveCExceptionMode.ThrowManagedException : objc_exception_mode
				);

				MarshalObjectiveCException (null, args);
				return args.ExceptionMode;
			}
			return objc_exception_mode;
		}

		static MarshalManagedExceptionMode OnMarshalManagedException (IntPtr exception_handle)
		{
			if (MarshalManagedException is not null) {
				if (GCHandle.FromIntPtr (exception_handle).Target is Exception exception) {
					var args = new MarshalManagedExceptionEventArgs (exception, managed_exception_mode);
					MarshalManagedException (null, args);
					return args.ExceptionMode;
				}
			}
			return managed_exception_mode;
		}

		static IntPtr GetFunctionPointer (Delegate d)
		{
			delegates.Add (d);
			return Marshal.GetFunctionPointerForDelegate (d);
		}

		// value_handle: GCHandle to a (smart) enum value
		// returns: a handle to a native NSString *
		static IntPtr ConvertSmartEnumToNSString (IntPtr value_handle)
		{
			var value = GetGCHandleTarget (value_handle)!;
			var smart_type = value.GetType ();
			if (!Registrar.IsSmartEnum (smart_type, out var getConstantMethod, out var getValueMethod))
				throw ErrorHelper.CreateError (8024, $"Could not find a valid extension type for the smart enum '{smart_type.FullName}'. Please file a bug at https://github.com/dotnet/macios/issues/new.");
			var rv = (NSString?) ((MethodInfo) getConstantMethod).Invoke (null, new object [] { value });
			if (rv is null)
				return IntPtr.Zero;
			rv.DangerousRetain ().DangerousAutorelease ();
#pragma warning disable RBI0014
			return rv.Handle;
#pragma warning restore RBI0014
		}


		// value: native NSString *
		// returns: GCHandle to a (smart) enum value. Caller must free the GCHandle.
		static IntPtr ConvertNSStringToSmartEnum (IntPtr value, IntPtr type)
		{
			var smart_type = (Type) GetGCHandleTarget (type)!;
			var str = GetNSObject<NSString> (value)!;
			if (!Registrar.IsSmartEnum (smart_type, out var getConstantMethod, out var getValueMethod))
				throw ErrorHelper.CreateError (8024, $"Could not find a valid extension type for the smart enum '{smart_type.FullName}'. Please file a bug at https://github.com/dotnet/macios/issues/new.");
			var rv = ((MethodInfo) getValueMethod).Invoke (null, new object [] { str });
			return AllocGCHandle (rv);
		}

		#region Wrappers for delegate callbacks
		static void RegisterAssembly (IntPtr a)
		{
			RegisterAssembly ((Assembly) GetGCHandleTarget (a)!);
		}

		static void RegisterEntryAssembly (IntPtr a)
		{
			RegisterEntryAssembly ((Assembly) GetGCHandleTarget (a)!);
		}

		static void ThrowNSException (IntPtr ns_exception)
		{
			throw new ObjCException (new NSException (ns_exception));
		}

		static void RethrowManagedException (IntPtr exception_gchandle)
		{
			var e = (Exception) GCHandle.FromIntPtr ((IntPtr) exception_gchandle).Target!;
			System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture (e).Throw ();
		}

		static IntPtr CreateNSException (IntPtr ns_exception)
		{
			Exception ex;
			ex = new ObjCException (Runtime.GetNSObject<NSException> (ns_exception)!);
			return AllocGCHandle (ex);
		}

		static Exception CreateRuntimeException (int code, string message)
		{
			return ErrorHelper.CreateError (code, message);
		}

		static IntPtr CreateRuntimeException (int code, IntPtr message)
		{
			var ex = ErrorHelper.CreateError (code, Marshal.PtrToStringAuto (message)!);
			return AllocGCHandle (ex);
		}

		static IntPtr UnwrapNSException (IntPtr exc_handle)
		{
			var obj = GCHandle.FromIntPtr (exc_handle).Target;
			var exc = obj as ObjCException;
			var nsexc = exc?.NSException;
			if (nsexc is not null) {
				return nsexc.DangerousRetain ().DangerousAutorelease ().Handle;
			} else {
				return IntPtr.Zero;
			}
		}

		static IntPtr GetBlockWrapperCreator (IntPtr method, int parameter)
		{
			if (IsNativeAOT)
				throw Runtime.CreateNativeAOTNotSupportedException ();

			return AllocGCHandle (GetBlockWrapperCreator ((MethodInfo) GetGCHandleTarget (method)!, parameter));
		}

		static IntPtr CreateBlockProxy (IntPtr method, IntPtr block)
		{
			return AllocGCHandle (CreateBlockProxy ((MethodInfo) GetGCHandleTarget (method)!, block));
		}

		static IntPtr CreateDelegateProxy (IntPtr method, IntPtr @delegate, IntPtr signature, uint token_ref)
		{
			return BlockLiteral.GetBlockForDelegate ((MethodInfo) GetGCHandleTarget (method)!, GetGCHandleTarget (@delegate), token_ref, Marshal.PtrToStringAuto (signature));
		}

		static IntPtr GetExceptionMessage (IntPtr exception_gchandle)
		{
			var exc = (Exception) GetGCHandleTarget (exception_gchandle)!;
			return Marshal.StringToHGlobalAuto (exc.Message);
		}

		static void PrintException (Exception exc, bool isInnerException, StringBuilder sb)
		{
			if (isInnerException)
				sb.AppendLine (" --- inner exception ---");
			sb.Append (exc.Message).Append (" (").Append (exc.GetType ().FullName).AppendLine (")");
			var trace = exc.StackTrace;
			if (!string.IsNullOrEmpty (trace))
				sb.AppendLine (trace);
		}

		static IntPtr PrintAllExceptions (IntPtr exception_gchandle)
		{
			var str = new StringBuilder ();
			try {
				var exc = (Exception?) GetGCHandleTarget (exception_gchandle);

				if (exc is null) {
					str.Append ($"Unable to print exception handle 0x{exception_gchandle.ToString ("x")}: null exception");
				} else {
					int counter = 0;
					do {
						PrintException (exc, counter > 0, str);
						exc = exc.InnerException;
					} while (counter < 10 && exc is not null);
				}
			} catch (Exception exception) {
				str.Append ("Failed to print exception: ").Append (exception);
			}

			return Marshal.StringToHGlobalAuto (str.ToString ());
		}

		// This method will register all assemblies referenced by the entry assembly.
		// For XM it will also register all assemblies loaded in the current appdomain.
		//
		// NOTE: the linker will remove this method when the dynamic registrar has been optimized away (RemoveCode.cs)
		// and as such cannot be renamed without updating the linker
		internal static void RegisterEntryAssembly (Assembly? entry_assembly)
		{
			var assemblies = new List<Assembly> ();

			assemblies.Add (NSObject.PlatformAssembly); // make sure our platform assembly comes first
														// Recursively get all assemblies referenced by the entry assembly.
			if (entry_assembly is not null) {
				var register_entry_assembly = true;
#if MONOMAC
				register_entry_assembly = OnAssemblyRegistration (entry_assembly.GetName ());
#endif
				if (register_entry_assembly)
					CollectReferencedAssemblies (assemblies, entry_assembly);
			} else {
				NSLog ("Could not find the entry assembly.");
			}

#if MONOMAC
			// Add all assemblies already loaded
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies ()) {
				if (!OnAssemblyRegistration (a.GetName ()))
					continue;

				if (!assemblies.Contains (a))
					assemblies.Add (a);
			}
#endif

			foreach (var a in assemblies)
				RegisterAssembly (a);
		}

		// IL2075: Using member 'System.Reflection.Assembly.GetReferencedAssemblies()' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Assembly references might be removed.
		[UnconditionalSuppressMessage ("", "IL2026", Justification = "We only want assemblies that survived trimming, so this is effectively trimmer-safe.")]
		static void CollectReferencedAssemblies (List<Assembly> assemblies, Assembly assembly)
		{
			assemblies.Add (assembly);
			foreach (var rf in assembly.GetReferencedAssemblies ()) {
#if MONOMAC
				if (!OnAssemblyRegistration (rf))
					continue;
#endif
				try {
					var a = Assembly.Load (rf);
					if (!assemblies.Contains (a))
						CollectReferencedAssemblies (assemblies, a);
				} catch (FileNotFoundException fefe) {
					// that's more important for XI because device builds don't go thru this step
					// and we can end up with simulator-only failures - bug #29211
					NSLog ($"Could not find `{fefe.FileName}` referenced by assembly `{assembly.FullName}`.");
				}
			}
		}

		internal static IEnumerable<Assembly> GetAssemblies ()
		{
			return Registrar.GetAssemblies ();
		}

		internal static string ComputeSignature (MethodInfo method, bool isBlockSignature)
		{
			return Registrar.ComputeSignature (method, isBlockSignature);
		}

		/// <param name="a">The assembly to process.</param>
		/// <summary>Registers all of the classes in the specified assembly.</summary>
		/// <remarks>
		///   <para>This iterates over all the types that derive from <see cref="NSObject" /> in the specified assembly and registers them with the runtime.</para>
		/// </remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterAssembly (Assembly a)
		{
			if (a is null)
				throw new ArgumentNullException (nameof (a));

			if (!DynamicRegistrationSupported)
				throw ErrorHelper.CreateError (8026, "Runtime.RegisterAssembly is not supported when the dynamic registrar has been linked away.");
#if MONOMAC
			var attributes = a.GetCustomAttributes (typeof (RequiredFrameworkAttribute), false);

			foreach (var attribute in attributes) {
				var requiredFramework = (RequiredFrameworkAttribute) attribute;
				string libPath;
				string libName = requiredFramework.Name;

				if (libName.Contains (".dylib")) {
					libPath = ResourcesPath!;
				} else {
					libPath = FrameworksPath!;
					libPath = Path.Combine (libPath, libName);
					libName = libName.Replace (".frameworks", "");
				}
				libPath = Path.Combine (libPath, libName);

				if (Dlfcn.dlopen (libPath, 0) == IntPtr.Zero)
					throw new Exception ($"Unable to load required framework: '{requiredFramework.Name}'",
						new Exception (Dlfcn.dlerror ()));
			}

			attributes = a.GetCustomAttributes (typeof (DelayedRegistrationAttribute), false);
			foreach (var attribute in attributes) {
				var delayedRegistration = (DelayedRegistrationAttribute) attribute;
				if (delayedRegistration.Delay)
					return;
			}
#endif

			if (assemblies is null) {
				assemblies = new List<Assembly> ();
				Class.Register (typeof (NSObject));
			}

			if (assemblies.Contains (a))
				return;

			assemblies.Add (a);

#if PROFILE
			var watch = new Stopwatch ();
			watch.Start ();
#endif

			Registrar.RegisterAssembly (a);

#if PROFILE
			watch.Stop ();
			Console.WriteLine ("RegisterAssembly ({0}) completed in {1} ms", a.FullName, watch.ElapsedMilliseconds);
#endif
		}

		static IntPtr GetClass (IntPtr klass)
		{
			return AllocGCHandle (new Class (klass));
		}

		static IntPtr GetSelector (IntPtr sel)
		{
			return AllocGCHandle (new Selector (sel));
		}

		static void GetMethodForSelector (IntPtr cls, IntPtr sel, sbyte is_static, IntPtr desc)
		{
			// This is called by the old registrar code.
			Registrar.GetMethodDescription (Class.Lookup (cls), sel, is_static != 0, desc);
		}

		internal static bool HasNSObject (NativeHandle ptr)
		{
			return TryGetNSObject (ptr, evenInFinalizerQueue: false) is not null;
		}

		internal static nint HasNSObject (IntPtr ptr)
		{
			var rv = TryGetNSObject (ptr, evenInFinalizerQueue: false) is not null;
			return (sbyte) (rv ? 1 : 0);
		}

		static IntPtr GetHandleForINativeObject (IntPtr ptr)
		{
			return ((INativeObject) GetGCHandleTarget (ptr)!).Handle;
		}

		static void UnregisterNSObject (IntPtr native_obj, IntPtr managed_obj)
		{
			NativeObjectHasDied (native_obj, GetGCHandleTarget (managed_obj) as NSObject);
		}

		static unsafe IntPtr GetMethodFromToken (uint token_ref)
		{
			var method = Class.ResolveMethodTokenReference (token_ref);
			if (method is not null)
				return AllocGCHandle (method);

			return IntPtr.Zero;
		}

		static unsafe IntPtr GetGenericMethodFromToken (IntPtr obj, uint token_ref)
		{
			var method = Class.ResolveMethodTokenReference (token_ref);
			if (method is null)
				return IntPtr.Zero;

			var nsobj = GetGCHandleTarget (obj) as NSObject;
			return AllocGCHandle (FindClosedMethodForObject (nsobj, method));
		}

		static MethodInfo FindClosedMethodForObject (NSObject? nsobj, MethodBase method)
		{
			if (nsobj is null)
				throw ErrorHelper.CreateError (8023, $"An instance object is required to construct a closed generic method for the open generic method: {method.DeclaringType!.FullName}.{method.Name}. {Constants.PleaseFileBugReport}");

			return FindClosedMethod (nsobj.GetType (), method);
		}

		static IntPtr TryGetOrConstructNSObjectWrapped (IntPtr ptr)
		{
			return AllocGCHandle (GetNSObject (ptr, MissingCtorResolution.Ignore, true));
		}

		static IntPtr GetINativeObject_Dynamic (IntPtr ptr, sbyte owns, IntPtr type_ptr)
		{
			/*
			 * This method is called from marshalling bridge (dynamic mode).
			 */
			var type = (System.Type) GetGCHandleTarget (type_ptr)!;
			return AllocGCHandle (GetINativeObject (ptr, owns != 0, type, null));
		}

		static IntPtr GetINativeObject_Static (IntPtr ptr, sbyte owns, uint iface_token, uint implementation_token)
		{
			/* 
			 * This method is called from generated code from the static registrar.
			 */

			var iface = Class.ResolveTypeTokenReference (iface_token)!;
			var type = Class.ResolveTypeTokenReference (implementation_token);
			return AllocGCHandle (GetINativeObject (ptr, owns != 0, iface, type));
		}

		unsafe static IntPtr GetNSObjectWithType (IntPtr ptr, IntPtr type_ptr, int* createdPtr)
		{
			var type = (System.Type) GetGCHandleTarget (type_ptr)!;
			var rv = AllocGCHandle (GetNSObject (ptr, type, MissingCtorResolution.ThrowConstructor1NotFound, true, true, out var created));
			*createdPtr = created ? 1 : 0;
			return rv;
		}

		static void Dispose (IntPtr gchandle)
		{
			((IDisposable?) GetGCHandleTarget (gchandle))?.Dispose ();
		}

		static nint IsParameterTransient (IntPtr info, int parameter)
		{
			var minfo = GetGCHandleTarget (info) as MethodInfo;
			if (minfo is null)
				return 0; // might be a ConstructorInfo (bug #15583), but we don't care about that (yet at least).
			minfo = minfo.GetBaseDefinition ();
			var parameters = minfo.GetParameters ();
			if (parameters.Length <= parameter)
				return 0;
			var rv = parameters [parameter].IsDefined (typeof (TransientAttribute), false);
			return rv ? 1 : 0;
		}

		static nint IsParameterOut (IntPtr info, int parameter)
		{
			var minfo = GetGCHandleTarget (info) as MethodInfo;
			if (minfo is null)
				return 0; // might be a ConstructorInfo (bug #15583), but we don't care about that (yet at least).
			minfo = minfo.GetBaseDefinition ();
			var parameters = minfo.GetParameters ();
			if (parameters.Length <= parameter)
				return 0;
			var rv = parameters [parameter].IsOut;
			return rv ? 1 : 0;
		}

		unsafe static void GetMethodAndObjectForSelector (IntPtr klass, IntPtr sel, sbyte is_static, IntPtr obj, IntPtr* mthisPtr, IntPtr desc)
		{
			IntPtr mthis = *mthisPtr;
			Registrar.GetMethodDescriptionAndObject (Class.Lookup (klass), sel, is_static != 0, obj, ref mthis, desc);
			*mthisPtr = mthis;
		}

		// If inner_exception_gchandle is provided, it will be freed.
		static IntPtr CreateProductException (int code, IntPtr inner_exception_gchandle, IntPtr utf8Message)
		{
			Exception? inner_exception = null;
			if (inner_exception_gchandle != IntPtr.Zero) {
				GCHandle gchandle = GCHandle.FromIntPtr (inner_exception_gchandle);
				inner_exception = (Exception?) gchandle.Target;
				gchandle.Free ();
			}
			var msg = Marshal.PtrToStringAuto (utf8Message)!;
			Exception ex = ErrorHelper.CreateError (code, inner_exception, msg);
			return AllocGCHandle (ex);
		}

		static IntPtr TypeGetFullName (IntPtr type)
		{
			return Marshal.StringToHGlobalAuto (((Type) GetGCHandleTarget (type)!).FullName);
		}

		static IntPtr GetObjectTypeFullName (IntPtr gchandle)
		{
			var obj = GetGCHandleTarget (gchandle);
			if (obj is null)
				return IntPtr.Zero;
			return Marshal.StringToHGlobalAuto (obj.GetType ().FullName);
		}

		static IntPtr LookupManagedTypeName (IntPtr klass)
		{
			return Marshal.StringToHGlobalAuto (Class.Lookup (klass)?.FullName);
		}
		#endregion

		// IL2075: this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicMethods' in call to 'System.Type.GetMethod(String)'. The return value of method 'ObjCRuntime.BlockProxyAttribute.Type.get' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2075", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		static MethodInfo? GetBlockProxyAttributeMethod (MethodInfo method, int parameter)
		{
			var attrs = method.GetParameters () [parameter].GetCustomAttributes (typeof (BlockProxyAttribute), true);
			if (attrs.Length == 1) {
				try {
					var attr = attrs [0] as BlockProxyAttribute;
					return attr?.Type?.GetMethod ("Create");
				} catch {
					return null;
				}
			}
			return null;
		}

		internal static ProtocolMemberAttribute? GetProtocolMemberAttribute (Type type, string selector, MethodInfo method)
		{
			var memberAttributes = type.GetCustomAttributes<ProtocolMemberAttribute> ();
			if (memberAttributes is null)
				return null;

			foreach (var attrib in memberAttributes) {
				if (attrib.IsStatic != method.IsStatic)
					continue;

				if (attrib.Selector != selector)
					continue;

				if (!attrib.IsProperty) {
					var methodParameters = method.GetParameters ();
					if ((attrib.ParameterType?.Length ?? 0) != methodParameters.Length)
						continue;
					var notApplicable = false;
					for (int i = 0; i < methodParameters.Length; i++) {
						var paramType = methodParameters [i].ParameterType;
						var isByRef = paramType.IsByRef;
						if (isByRef)
							paramType = paramType.GetElementType ();
						if (isByRef != attrib.ParameterByRef! [i]) {
							notApplicable = true;
							break;
						}
						if (paramType != attrib.ParameterType! [i]) {
							notApplicable = true;
							break;
						}
					}
					if (notApplicable)
						continue;
				}

				return attrib;
			}

			return null;
		}

		//
		// Returns a MethodInfo that represents the method that can be used to turn
		// a the block in the given method at the given parameter into a strongly typed
		// delegate
		//
		// Note that the code in this method doesn't work with NativeAOT, so assert that never happens by throwing an exception in that case
		// IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.Interfaces' in call to 'System.Type.GetInterfaces()'. The return value of method 'System.Reflection.MemberInfo.DeclaringType.get' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to."),
		// IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicMethods' in call to 'System.Type.GetMethod(String, BindingFlags, Binder, Type[], ParameterModifier[])'. The return value of method 'System.Reflection.Assembly.GetType(String, Boolean)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to."),
		[UnconditionalSuppressMessage ("", "IL2075", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		// IL2026: Using member 'System.Reflection.Assembly.GetType(String, Boolean)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Types might be removed."),
		[UnconditionalSuppressMessage ("", "IL2026", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		// IL2065: Value passed to implicit 'this' parameter of method 'System.Type.GetMethod(String)' can not be statically determined and may not meet 'DynamicallyAccessedMembersAttribute' requirements."),
		[UnconditionalSuppressMessage ("", "IL2065", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		// IL2072: ObjCRuntime.Runtime.GetBlockWrapperCreator(MethodInfo, Int32): 'interfaceType' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicMethods', 'DynamicallyAccessedMemberTypes.NonPublicMethods' in call to 'System.Type.GetInterfaceMap(Type)'. The return value of method 'System.Type.GetInterfaces()' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2072", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		static MethodInfo? GetBlockWrapperCreator (MethodInfo method, int parameter)
		{
			// Note that the code in this method doesn't work with NativeAOT, so assert that never happens by throwing an exception in that case
			if (IsNativeAOT)
				throw Runtime.CreateNativeAOTNotSupportedException ();

			// A mirror of this method is also implemented in StaticRegistrar:FindBlockProxyCreatorMethod
			// If this method is changed, that method will probably have to be updated too (tests!!!)
			MethodInfo first = method;
			MethodInfo? last = null;
			Type []? extensionParameters = null;

			while (method != last) {
				last = method;
				var createMethod = GetBlockProxyAttributeMethod (method, parameter);
				if (createMethod is not null)
					return createMethod;
				method = method.GetBaseDefinition ();
			}

			string? selector = null;

			// Might be the implementation of an interface method, so find the corresponding
			// MethodInfo for the interface, and check for BlockProxy attributes there as well.
			foreach (var iface in method.DeclaringType!.GetInterfaces ()) {
				if (!iface.IsDefined (typeof (ProtocolAttribute), false))
					continue;

				var map = method.DeclaringType.GetInterfaceMap (iface);
				for (int i = 0; i < map.TargetMethods.Length; i++) {
					if (map.TargetMethods [i] == first) {
						var createMethod = GetBlockProxyAttributeMethod (map.InterfaceMethods [i], parameter);
						if (createMethod is not null)
							return createMethod;
					}
				}

				// We store the BlockProxy type in the ProtocolMemberAttribute, so check those.
				// We may run into binding assemblies built with earlier versions of the generator,
				// which means we can't rely on finding the BlockProxy attribute in the ProtocolMemberAttribute.
				if (selector is null)
					selector = GetExportAttribute (method)?.Selector ?? string.Empty;
				if (!string.IsNullOrEmpty (selector)) {
					var attrib = GetProtocolMemberAttribute (iface, selector, method);
					if (attrib is not null && attrib.ParameterBlockProxy!.Length > parameter && attrib.ParameterBlockProxy [parameter] is not null)
						return attrib.ParameterBlockProxy [parameter]!.GetMethod ("Create");
				}

				// Might be an implementation of an optional protocol member.
				// We look that up on the corresponding extension method.
				string extensionName = string.Empty;
				if (!string.IsNullOrEmpty (iface.Namespace))
					extensionName = iface.Namespace + ".";
				extensionName += iface.Name.Substring (1) + "_Extensions";
				var extensionType = iface.Assembly.GetType (extensionName, false);
				if (extensionType is not null) {
					if (extensionParameters is null) {
						var methodParameters = method.GetParameters ();
						extensionParameters = new Type [methodParameters.Length + 1];
						for (int i = 0; i < methodParameters.Length; i++)
							extensionParameters [i + 1] = methodParameters [i].ParameterType;
					}
					extensionParameters [0] = iface;
					var extensionMethod = extensionType.GetMethod (method.Name, BindingFlags.Public | BindingFlags.Static, null, extensionParameters, null);
					if (extensionMethod is not null) {
						var createMethod = GetBlockProxyAttributeMethod (extensionMethod, parameter + 1);
						if (createMethod is not null)
							return createMethod;
					}
				}
			}

			throw new RuntimeException (8009, true, $"Unable to locate the block to delegate conversion method for the method {method.DeclaringType.FullName}.{method.Name}'s parameter #{parameter + 1}. {Constants.PleaseFileBugReport}");
		}

		//
		// Called from the runtime, since it is too hard to use the unmanaged API
		// Given a MethodInfo, invoke it, passing the given block
		//
		// Used to call the Create(IntPtr) method on the proxy classes that turn
		// objective c blocks into strongly typed delegates.
		//
		// The block will be kept alive until the delegate is collected by the GC.
		[EditorBrowsable (EditorBrowsableState.Never)]
		static Delegate? CreateBlockProxy (MethodInfo method, IntPtr block)
		{
			var del = (Delegate?) method.Invoke (null, new object [] { block });
			if (del is not null) {
				ReleaseBlockWhenDelegateIsCollected (block, del);
			} else {
				ReleaseBlockOnMainThread (block);
			}
			return del;
		}

		internal static T GetDelegateForBlock<T> (IntPtr methodPtr) where T : System.MulticastDelegate
		{
			// We do not care if there is a race condition and we initialize two caches
			// since the worst that can happen is that we end up with an extra
			// delegate->function pointer.
			var pair = new IntPtrTypeValueTuple (methodPtr, typeof (T));
			lock (lock_obj) {
				if (block_to_delegate_cache is null)
					block_to_delegate_cache = new Dictionary<IntPtrTypeValueTuple, Delegate> ();

				if (block_to_delegate_cache.TryGetValue (pair, out var cachedValue))
					return (T) cachedValue;
			}

			var val = Marshal.GetDelegateForFunctionPointer<T> (methodPtr);

			lock (lock_obj) {
				block_to_delegate_cache [pair] = val;
			}
			return val;
		}

		unsafe static MethodBase FindMethod (IntPtr typeptr, IntPtr methodptr, int paramCount, IntPtr* paramptr)
		{
			var type = Type.GetType (Marshal.PtrToStringAuto (typeptr)!)!;
			var methodName = Marshal.PtrToStringAuto (methodptr)!;
			var parameterTypes = new string [paramCount];
			for (int i = 0; i < paramCount; i++)
				parameterTypes [i] = Marshal.PtrToStringAuto (paramptr [i])!;

			MethodBase [] methods;
			if (methodName == ".ctor") {
				methods = type.GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			} else {
				methods = type.GetMethods (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}

			foreach (var m in methods) {
				if (m.Name != methodName)
					continue;
				var parameters = m.GetParameters ();
				if (parameters.Length != paramCount)
					continue;
				bool found = true;
				for (int i = 0; i < paramCount; i++) {
					var paramType = parameters [i].ParameterType;

					var ptaqn = paramType.AssemblyQualifiedName;
					if (ptaqn is null)
						continue;
					// the condensed string representation needs some fixup if there are generics used
					if (paramType.IsGenericType) {
						int s = 0;
						while ((s = ptaqn.IndexOf (", Version=", s, StringComparison.OrdinalIgnoreCase)) != -1) {
							var e = ptaqn.IndexOf (']', s);
							ptaqn = (e == -1) ? ptaqn.Substring (0, s) : ptaqn.Remove (s, e - s);
						}
					}
					if (paramType.Name != parameterTypes [i] && !ptaqn.StartsWith (parameterTypes [i], StringComparison.Ordinal)) {
						found = false;
						break;
					}
				}
				if (!found)
					continue;

				// Found the method.
				return m;
			}

			throw ErrorHelper.CreateError (8002, "Could not find the method '{methodName}' in the type '{type.FullName}'.");
		}

		internal static void UnregisterNSObject (IntPtr ptr)
		{
			lock (lock_obj) {
				if (object_map.Remove (ptr, out var value))
					value.Free ();
			}
		}

		// Ownership-aware variant of UnregisterNSObject: only removes the entry if it
		// still refers to `managed` (or is dead). This avoids clobbering an entry that
		// another object created after reusing a freed native pointer (issue #25861).
		internal static void UnregisterNSObject (IntPtr ptr, NSObject managed)
		{
			lock (lock_obj) {
				if (object_map.TryGetValue (ptr, out var value)) {
					if (value.Target is null || object.ReferenceEquals (value.Target, managed)) {
						object_map.Remove (ptr);
						value.Free ();
					}
				}
			}
		}

		internal static void NativeObjectHasDied (IntPtr ptr, NSObject? managed_obj)
		{
			lock (lock_obj) {
				if (object_map.TryGetValue (ptr, out var wr)) {
					if (managed_obj is null || wr.Target == (object) managed_obj) {
						object_map.Remove (ptr);
						wr.Free ();
					} else if (wr.Target is null) {
						// We can remove null entries, and free the corresponding GCHandle
						object_map.Remove (ptr);
						wr.Free ();
					}

				}

				if (managed_obj is not null)
					managed_obj.ClearHandle ();
			}
		}

		// Completes deferred object_map registration (issue #25861): when 'onlyIfNeeded' is
		// true, registers the object only if the pointer isn't already present, and leaves
		// any live existing entry untouched. Dead entries are removed so they don't prevent
		// registration of a new object for a reused native pointer.
		internal static void RegisterNSObject (NSObject obj, IntPtr ptr, bool onlyIfNeeded = false)
		{
			GCHandle handle;
			if (Runtime.IsCoreCLR) {
				handle = CreateTrackingGCHandle (obj, ptr);
			} else {
				handle = GCHandle.Alloc (obj, GCHandleType.WeakTrackResurrection);
			}

			lock (lock_obj) {
				if (onlyIfNeeded) {
					if (object_map.TryGetValue (ptr, out var existing)) {
						if (existing.Target is not null) {
							// Already registered; don't touch the existing entry, just free
							// the handle we speculatively allocated.
							handle.Free ();
							return;
						}

						// The weak target was collected without the native object dying.
						// Remove the stale entry before registering the replacement.
						object_map.Remove (ptr);
						existing.Free ();
					}
				} else {
					if (object_map.Remove (ptr, out var existing))
						existing.Free ();
				}
				object_map [ptr] = handle;
#pragma warning disable RBI0014
				obj.Handle = ptr;
#pragma warning restore RBI0014
			}
		}

		// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
		//
		// IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicProperties' in call to 'System.Type.GetProperties()'. The return value of method 'System.Reflection.MemberInfo.DeclaringType.get' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2075", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		internal static PropertyInfo? FindPropertyInfo (MethodInfo accessor)
		{
			// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
			if (IsNativeAOT)
				throw Runtime.CreateNativeAOTNotSupportedException ();

			if (!accessor.IsSpecialName)
				return null;

			foreach (var pi in accessor.DeclaringType!.GetProperties ()) {
				if (pi.GetGetMethod () == accessor)
					return pi;
				if (pi.GetSetMethod () == accessor)
					return pi;
			}

			return null;
		}

		internal static ExportAttribute? GetExportAttribute (MethodInfo method)
		{
			var attrib = method.GetCustomAttribute<ExportAttribute> ();
			if (attrib is null) {
				var pinfo = FindPropertyInfo (method);
				if (pinfo is not null)
					attrib = pinfo.GetCustomAttribute<ExportAttribute> ();
			}
			return attrib;
		}

		static NSObject? IgnoreConstructionError (IntPtr ptr, IntPtr klass, Type type)
		{
			return null;
		}

		internal enum MissingCtorResolution {
			ThrowConstructor1NotFound,
			ThrowConstructor2NotFound,
			Ignore,
		}

		static void MissingCtor (IntPtr ptr, IntPtr klass, Type type, MissingCtorResolution resolution, IntPtr sel, RuntimeMethodHandle method_handle)
		{
			if (resolution == MissingCtorResolution.Ignore)
				return;

			if (klass == IntPtr.Zero)
				klass = Class.GetClassForObject (ptr);

			var msg = new StringBuilder ();
			msg.Append ("Failed to marshal the Objective-C object 0x");
			msg.Append (ptr.ToString ("x"));
			msg.Append (" (type: ");
			msg.Append (new Class (klass).Name);
			msg.Append ("). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type '");
			msg.Append (type.FullName);
			msg.Append ("' does not have a constructor that takes ");

			switch (resolution) {
			case MissingCtorResolution.ThrowConstructor1NotFound:
				msg.Append ("one NativeHandle argument");
				break;
			case MissingCtorResolution.ThrowConstructor2NotFound:
				msg.Append ("two (NativeHandle, bool) arguments");
				break;
			}

			msg.Append (").");

			if (sel != IntPtr.Zero || method_handle.Value != IntPtr.Zero) {
				AppendAdditionalInformation (msg, sel, method_handle);
			}

			throw ErrorHelper.CreateError (8027, msg.ToString ());
		}

		static void CannotCreateManagedInstanceOfGenericType (IntPtr ptr, IntPtr klass, Type type, MissingCtorResolution resolution, IntPtr sel, RuntimeMethodHandle method_handle)
		{
			if (resolution == MissingCtorResolution.Ignore)
				return;

			if (klass == IntPtr.Zero)
				klass = Class.GetClassForObject (ptr);

			var msg = new StringBuilder ();
			msg.AppendFormat (Xamarin.Bundler.Errors.MX8056 /* Failed to marshal the Objective-C object 0x{0} (type: {1}). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance of generic type {2}. */, ptr.ToString ("x"), new Class (klass).Name, type.FullName);

			if (sel != IntPtr.Zero || method_handle.Value != IntPtr.Zero) {
				AppendAdditionalInformation (msg, sel, method_handle);
			}

			throw ErrorHelper.CreateError (8056, msg.ToString ());
		}

		static void AppendAdditionalInformation (StringBuilder msg, IntPtr sel, RuntimeMethodHandle method_handle)
		{
			msg.AppendLine ();
			msg.AppendLine ("Additional information:");
			if (sel != IntPtr.Zero)
				msg.Append ("\tSelector: ").Append (Selector.GetName (sel)).AppendLine ();
			if (method_handle.Value != IntPtr.Zero) {
				try {
					var method = MethodBase.GetMethodFromHandle (method_handle);
					msg.Append ($"\tMethod: ");
					if (method is not null) {
						// there's no good built-in function to format a MethodInfo :/
						msg.Append (method.DeclaringType?.FullName ?? string.Empty);
						msg.Append (".");
						msg.Append (method.Name);
						msg.Append ("(");
						var parameters = method.GetParameters ();
						for (var i = 0; i < parameters.Length; i++) {
							if (i > 0)
								msg.Append (", ");
							msg.Append (parameters [i].ParameterType.FullName);
						}
						msg.Append (")");
					} else {
						msg.Append ($"Unable to resolve RuntimeMethodHandle 0x{method_handle.Value.ToString ("x")}");
					}
					msg.AppendLine ();
				} catch (Exception ex) {
					msg.Append ($"\tMethod: Unable to resolve RuntimeMethodHandle 0x{method_handle.Value.ToString ("x")}: {ex.Message}");
				}
			}
		}

		static NSObject? ConstructNSObject (IntPtr ptr, IntPtr klass, MissingCtorResolution missingCtorResolution)
		{
			Type type = Class.Lookup (klass);

			if (type is not null) {
				return ConstructNSObject<NSObject> (ptr, type, missingCtorResolution);
			} else {
				return new NSObject (ptr);
			}
		}

		internal static T? ConstructNSObject<T> (IntPtr ptr) where T : NSObject
		{
			return ConstructNSObject<T> (ptr, typeof (T), MissingCtorResolution.ThrowConstructor1NotFound);
		}

		// The 'selector' and 'method' arguments are only used in error messages.
		static T? ConstructNSObject<T> (IntPtr ptr, Type type, MissingCtorResolution missingCtorResolution) where T : NSObject
		{
			return ConstructNSObject<T> (ptr, type, missingCtorResolution, IntPtr.Zero, default (RuntimeMethodHandle));
		}

		// The 'selector' and 'method' arguments are only used in error messages.
		static T? ConstructNSObject<T> (IntPtr ptr, Type type, MissingCtorResolution missingCtorResolution, IntPtr sel, RuntimeMethodHandle method_handle) where T : NSObject, INSObjectFactory
		{
			if (type is null)
				throw new ArgumentNullException (nameof (type));

			if (Runtime.IsTrimmableStaticRegistrar) {
				var lookupType = type;
				if (typeof (T) == type && type.IsGenericType) {
					var inst = ConstructNSObjectViaFactoryMethod (ptr);
					if (inst is not null) {
#if LOG_TRIMMABLE_TYPEMAP
						Runtime.NSLog ($"ConstructNSObject<{typeof (T).FullName}> (0x{@ptr:X}, {type}) created '{inst.GetType ().FullName}' instance using static interface factory method.");
#endif
						return inst;
					}
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructNSObject<{typeof (T).FullName}> (0x{@ptr:X}, {type}) failed to create instance using static interface factory method.");
#endif
					if (HotReloadCompatible) {
						var reflectionCtor = GetIntPtrConstructor (type);
						if (reflectionCtor is not null) {
							var argument = reflectionCtor.GetParameters () [0].ParameterType == typeof (IntPtr) ? (object) ptr : new NativeHandle (ptr);
							return (T?) reflectionCtor.Invoke ([argument]);
						}
					}
					CannotCreateManagedInstanceOfGenericType (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
					return null;
				}

				if (TypeMaps.TryGetNSObjectProxyAttribute (lookupType, out var proxyAttribute)) {
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructNSObject<{typeof (T).FullName}> (0x{@ptr:X}, {type}) found proxy attribute for lookup type '{lookupType.FullName}'");
#endif
					var instance = (T?) (object?) proxyAttribute.CreateObject (ptr);
					if (instance is not null)
						return instance;
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructNSObject<{typeof (T).FullName}> (0x{@ptr:X}, {type}) proxy attribute didn't create instance?");
#endif
				}

#if LOG_TRIMMABLE_TYPEMAP
				Runtime.NSLog ($"ConstructNSObject<{typeof (T).FullName}> (0x{@ptr:X}, {type}) did not find type '{lookupType.FullName}' in proxy map");
#endif
				MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
				return null;
			}

			if (Runtime.IsManagedStaticRegistrar) {
				T? instance = default;
				var nativeHandle = new NativeHandle (ptr);

				// We want to create an instance of `type` and if we have the chance to use the factory method
				// on the generic type, we will prefer it to using the lookup table.
				if (typeof (T) == type
					&& typeof (T) != typeof (NSObject)
					&& !(typeof (T).IsInterface || typeof (T).IsAbstract)) {
					instance = ConstructNSObjectViaFactoryMethod (nativeHandle);
				}

				// Generic types can only be instantiated through the factory method and if that failed, we can't
				// fall back to the lookup tables and we need to stop here.
				if (type.IsGenericType && instance is null) {
					CannotCreateManagedInstanceOfGenericType (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
					return null;
				}

				// If we couldn't create an instance of T through the factory method, we'll use the lookup table
				// based on the RuntimeTypeHandle.
				//
				// This isn't possible for generic types - we don't know the type arguments at compile time
				// (otherwise we would be able to create an instance of T through the factory method).
				// For non-generic types, we can call the NativeHandle constructor based on the RuntimeTypeHandle.

				if (instance is null) {
					instance = RegistrarHelper.ConstructNSObject<T> (type, nativeHandle);
				}

				if (instance is null) {
					// If we couldn't create an instance using the lookup table either, it means `type` doesn't contain
					// a suitable constructor.
					MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
				}

				return instance;
			}

			var ctor = GetIntPtrConstructor (type);

			if (ctor is null) {
				MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
				return null;
			}

			var ctorArguments = new object [1];
			if (ctor.GetParameters () [0].ParameterType == typeof (IntPtr)) {
				ctorArguments [0] = ptr;
			} else {
				ctorArguments [0] = new NativeHandle (ptr);
			}

			var obj = ctor.Invoke (ctorArguments);
			if (obj is T rv)
				return rv;

			throw new InvalidCastException ($"Unable to cast object of type '{obj.GetType ().FullName}' to type '{typeof (T).FullName}'.");

			// It isn't possible to call T._Xamarin_ConstructNSObject (...) directly from the parent function. For some
			// types, the app crashes with a SIGSEGV:
			//
			//   error: * Assertion at /Users/runner/work/1/s/src/mono/mono/mini/mini-generic-sharing.c:2283, condition `m_class_get_vtable (info->klass)' not met
			//
			// When the same call is made from a separate function, it works fine.
			static T? ConstructNSObjectViaFactoryMethod (NativeHandle handle)
				=> T._Xamarin_ConstructNSObject (handle) as T;
		}

		// The generic argument T is only used to cast the return value.
		static T? ConstructINativeObject<T> (IntPtr ptr, bool owns, Type type, Type target_type, MissingCtorResolution missingCtorResolution, IntPtr sel, RuntimeMethodHandle method_handle) where T : INativeObject
		{
			if (type is null)
				throw new ArgumentNullException (nameof (type));

			if (type.IsByRef)
				type = type.GetElementType ()!;

			if (Runtime.IsTrimmableStaticRegistrar) {
				if (typeof (T) == type && type.IsGenericType) {
					var inst = ConstructINativeObjectViaFactoryMethod (ptr, owns);
					if (inst is not null) {
#if LOG_TRIMMABLE_TYPEMAP
						Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) created '{inst.GetType ().FullName}' instance using static interface factory method.");
#endif
						return inst;
					}
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) failed to create instance using static interface factory method.");
#endif
					if (HotReloadCompatible) {
						if (type.IsSubclassOf (typeof (NSObject))) {
							var nsObjectCtor = GetIntPtrConstructor (type);
							if (nsObjectCtor is not null) {
								var handle = nsObjectCtor.GetParameters () [0].ParameterType == typeof (IntPtr) ? (object) ptr : new NativeHandle (ptr);
								var instance = (T?) nsObjectCtor.Invoke ([handle]);
								if (instance is not null && owns)
									Runtime.TryReleaseINativeObject (instance);
								return instance;
							}
						}

						var reflectionCtor = GetIntPtr_BoolConstructor (type);
						if (reflectionCtor is not null) {
							var handle = reflectionCtor.GetParameters () [0].ParameterType == typeof (IntPtr) ? (object) ptr : new NativeHandle (ptr);
							return (T?) reflectionCtor.Invoke ([handle, owns]);
						}
					}
					CannotCreateManagedInstanceOfGenericType (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
					return default (T);
				}

				if (TypeMaps.TryCreateInstanceUsingProxyTypeAttribute<T> (type, ptr, owns, out var proxyInstanceA)) {
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) created instance of type '{proxyInstanceA.GetType ()}' using proxy type attribute for '{type.FullName}'");
#endif
					return proxyInstanceA;
				}

				if (type != target_type) {
					if (TypeMaps.TryCreateInstanceUsingProxyTypeAttribute<T> (target_type, ptr, owns, out var proxyInstanceB)) {
#if LOG_TRIMMABLE_TYPEMAP
						Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) created instance '{proxyInstanceB.GetType ()}' using proxy type attribute for '{target_type.FullName}' [2]");
#endif
						return proxyInstanceB;
					}
				}

				if (TypeMaps.TryGetProtocolProxyAttribute (target_type, out var protocolProxyAttribute)) {
					var rv = protocolProxyAttribute.CreateObject (ptr, owns);
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) found proxy attribute of type '{protocolProxyAttribute.GetType ()}', and created object of type '{(rv?.GetType ()?.FullName ?? "null")}'");
#endif
					return (T?) (object?) rv;
				}

				if (TypeMaps.INativeObjectProxyTypes.TryGetValue (target_type, out var inativeObjectProxyType)) {
#if LOG_TRIMMABLE_TYPEMAP
					Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) found in INativeObject proxy map");
#endif
					var attrib = inativeObjectProxyType.GetCustomAttribute<INativeObjectProxyAttribute> ();
					if (attrib is null)
						throw new InvalidOperationException ($"Type '{inativeObjectProxyType.FullName}' is expected to have an INativeObjectProxyAttribute."); // TODO: better exception
					return (T?) (object?) attrib.CreateObject (ptr, owns);
				}

#if LOG_TRIMMABLE_TYPEMAP
				Runtime.NSLog ($"ConstructINativeObject<{typeof (T).FullName}> (0x{@ptr:X}, {owns}, {type}, {target_type}) did not find type '{target_type.FullName}' in any map");
#endif

				MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
				return default (T);
			}

			if (Runtime.IsManagedStaticRegistrar) {
				var nativeHandle = new NativeHandle (ptr);
				T? instance = default (T);

				// We want to create an instance of `type` and if we have the chance to use the factory method
				// on the generic type, we will prefer it to using the lookup table.
				if (typeof (T) == type
					&& typeof (T) != typeof (INativeObject)
					&& typeof (T) != typeof (NSObject)
					&& !(typeof (T).IsInterface || typeof (T).IsAbstract)) {
					instance = ConstructINativeObjectViaFactoryMethod (nativeHandle, owns);
				}

				// Generic types can only be instantiated through the factory method and if that failed, we can't
				// fall back to the lookup tables and we need to stop here.
				if (type.IsGenericType && instance is null) {
					CannotCreateManagedInstanceOfGenericType (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
					return default (T);
				}

				// If we couldn't create an instance of T through the factory method, we'll use the lookup table
				// based on the RuntimeTypeHandle.
				//
				// This isn't possible for generic types - we don't know the type arguments at compile time
				// (otherwise we would be able to create an instance of T through the factory method).
				// For non-generic types, we can call the NativeHandle constructor based on the RuntimeTypeHandle.

				// If type is an NSObject, we prefer the NSObject lookup table
				if (instance is null && type != typeof (NSObject) && type.IsSubclassOf (typeof (NSObject))) {
					instance = (T?) (INativeObject?) RegistrarHelper.ConstructNSObject<T> (type, nativeHandle);
					if (instance is not null && owns) {
						Runtime.TryReleaseINativeObject (instance);
					}
				}

				if (instance is null && type != typeof (INativeObject)) {
					instance = RegistrarHelper.ConstructINativeObject<T> (type, nativeHandle, owns);
				}

				if (instance is null) {
					// If we couldn't create an instance using the lookup table either, it means `type` doesn't contain
					// a suitable constructor.
					MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
				}

				return instance;
			}

			var ctor = GetIntPtr_BoolConstructor (type);

			if (ctor is null) {
				MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution, sel, method_handle);
				return default (T);
			}

			var ctorArguments = new object [2];
			if (ctor.GetParameters () [0].ParameterType == typeof (IntPtr)) {
				ctorArguments [0] = ptr;
			} else {
				ctorArguments [0] = new NativeHandle (ptr);
			}
			ctorArguments [1] = owns;

			return (T?) ctor.Invoke (ctorArguments);

			// It isn't possible to call T._Xamarin_ConstructINativeObject (...) directly from the parent function. For some
			// types, the app crashes with a SIGSEGV:
			//
			//   error: * Assertion at /Users/runner/work/1/s/src/mono/mono/mini/mini-generic-sharing.c:2283, condition `m_class_get_vtable (info->klass)' not met
			//
			// When the same call is made from a separate function, it works fine.
			static T? ConstructINativeObjectViaFactoryMethod (NativeHandle nativeHandle, bool owns)
			{
				var rv = T._Xamarin_ConstructINativeObject (nativeHandle, owns);
				if (rv is T t)
					return t;
				return default (T);
			}
		}

		static IntPtr CreateNSObject (IntPtr type_gchandle, IntPtr handle, NSObject.Flags flags)
		{
			return NSObject.CreateNSObject (type_gchandle, handle, flags);
		}

		// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
		//
		// IL2070: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicConstructors', 'DynamicallyAccessedMemberTypes.NonPublicConstructors' in call to 'System.Type.GetConstructors(BindingFlags)'. The parameter 'type' of method 'ObjCRuntime.Runtime.GetIntPtrConstructor(Type)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2070", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		static ConstructorInfo? GetIntPtrConstructor (Type type)
		{
			// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
			if (IsNativeAOT)
				throw CreateNativeAOTNotSupportedException ();

			lock (intptr_ctor_cache) {
				if (intptr_ctor_cache.TryGetValue (type, out var rv))
					return rv;
			}
			var ctors = type.GetConstructors (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			ConstructorInfo? backupConstructor = null;
			for (int i = 0; i < ctors.Length; ++i) {
				var param = ctors [i].GetParameters ();
				if (param.Length != 1)
					continue;
				if (param [0].ParameterType == typeof (IntPtr)) {
					backupConstructor = ctors [i];
					continue;
				}

				if (param [0].ParameterType != typeof (NativeHandle))
					continue;

				lock (intptr_ctor_cache)
					intptr_ctor_cache [type] = ctors [i];
				return ctors [i];
			}

			if (backupConstructor is not null) {
				const string p1 = "an ObjCRuntime.NativeHandle parameter";
				const string p2 = "an System.IntPtr parameter";
				string p3 = typeof (IntPtr).FullName!;
				string p4 = typeof (NativeHandle).FullName!;
				NSLog ($"The type {type.FullName} does not have a constructor that takes {p1} but a constructor that takes {p2} was found (and will be used instead). It's highly recommended to change the signature of the {p3} constructor to be {p4}.");
				lock (intptr_ctor_cache)
					intptr_ctor_cache [type] = backupConstructor;
				return backupConstructor;
			}

			return null;
		}

		// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
		//
		// IL2070: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicConstructors', 'DynamicallyAccessedMemberTypes.NonPublicConstructors' in call to 'System.Type.GetConstructors(BindingFlags)'. The parameter 'type' of method 'ObjCRuntime.Runtime.GetIntPtr_BoolConstructor(Type)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2070", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		static ConstructorInfo? GetIntPtr_BoolConstructor (Type type)
		{
			// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
			if (IsNativeAOT)
				throw CreateNativeAOTNotSupportedException ();

			lock (intptr_bool_ctor_cache) {
				if (intptr_bool_ctor_cache.TryGetValue (type, out var rv))
					return rv;
			}
			var ctors = type.GetConstructors (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			ConstructorInfo? backupConstructor = null;
			for (int i = 0; i < ctors.Length; ++i) {
				var param = ctors [i].GetParameters ();
				if (param.Length != 2)
					continue;

				if (param [1].ParameterType != typeof (bool))
					continue;

				if (param [0].ParameterType == typeof (IntPtr)) {
					backupConstructor = ctors [i];
					continue;
				}

				if (param [0].ParameterType != typeof (NativeHandle))
					continue;

				lock (intptr_bool_ctor_cache)
					intptr_bool_ctor_cache [type] = ctors [i];
				return ctors [i];
			}

			if (backupConstructor is not null) {
				const string p1 = "two (ObjCRuntime.NativeHandle, bool) arguments";
				const string p2 = "two (System.IntPtr, bool) parameters";
				const string p3 = "(System.IntPtr, bool)";
				const string p4 = "(ObjCRuntime.NativeHandle, bool)";
				NSLog ($"The type {type.FullName} does not have a constructor that takes {p1} but a constructor that takes {p2} was found (and will be used instead). It's highly recommended to change the signature of the {p3} constructor to be {p4}.");
				lock (intptr_bool_ctor_cache)
					intptr_bool_ctor_cache [type] = backupConstructor;
				return backupConstructor;
			}

			return null;
		}

		/// <param name="ptr">A pointer to an unmanaged NSObject or any class that derives from the Objective-C NSObject class.</param>
		/// <summary>Looks up an existing wrapper object for an unmanaged <see cref="IntPtr" />.</summary>
		/// <returns>If a managed wrapper exists for the specified <see cref="IntPtr" />, that wrapper is returned, otherwise null.</returns>
		public static NSObject? TryGetNSObject (IntPtr ptr)
		{
			return TryGetNSObject (ptr, evenInFinalizerQueue: false);
		}

		internal static NSObject? TryGetNSObject (IntPtr ptr, bool evenInFinalizerQueue)
		{
			lock (lock_obj) {
				if (object_map.TryGetValue (ptr, out var reference)) {
					var target = (NSObject?) reference.Target;
					if (target is null)
						return null;

					if (target.InFinalizerQueue) {
						if (!evenInFinalizerQueue) {
							// Don't return objects that's been queued for finalization unless requested to.
							return null;
						}

						if (target.IsDirectBinding && !target.IsRegisteredToggleRef) {
							// This is a non-toggled direct binding, which is safe to re-create.
							// We get here if the native object is still around, while the GC
							// has collected the managed object, and then we end up needing
							// a managed wrapper again before we've completely cleaned up 
							// the existing managed wrapper (which is the one we just found).
							// Returning null here will cause us to re-create the managed wrapper.
							// See bug #37670 for a real-world scenario.
							return null;
						}
					}

					return target;
				}
			}

			return null;
		}

		/// <summary>Remove an object from the object map.</summary>
		/// <remarks>
		///   Removing an object from the object map can be necessary for managed wrapper objects that are singletons or not really Objective-C classes, but constant pointers.
		///   For instance, the constant empty string (@"") is just a singleton pointer which can't be retained/released (retaining/releasing does nothing).
		///   The problem arises when the managed wrapper for such a native handle is disposed, then subsequently it re-surfaces, and now we end up finding a
		///   disposed object in the object map.
		/// </remarks>
		internal static bool RemoveFromObjectMap (NSObject obj)
		{
			var handle = obj.GetHandle ();
			if (handle == NativeHandle.Zero)
				return false;

			lock (lock_obj) {
				if (!object_map.TryGetValue (handle, out var reference))
					return false;

				if (!object.ReferenceEquals (reference.Target, obj))
					return false;

				object_map.Remove (handle);
				reference.Free ();
			}

			return true;
		}

		public static NSObject? GetNSObject (NativeHandle ptr)
		{
			return GetNSObject ((IntPtr) ptr, MissingCtorResolution.ThrowConstructor1NotFound);
		}

		/// <summary>Wraps an unmanaged <see cref="NativeHandle" /> into a fully typed <see cref="NSObject" />, or returns an existing wrapper object if one already exists.</summary>
		/// <param name="ptr">A pointer to an unmanaged <see cref="NSObject" /> or any class that derives from the Objective-C NSObject class.</param>
		/// <param name="owns">Pass true if the caller has a reference to the native object, and wants to give it to the managed wrapper instance. Otherwise pass false (and the native object will be retained if needed).</param>
		/// <returns>An instance of a class that derives <see cref="NSObject" />.</returns>
		/// <remarks>
		///   <para>The runtime create an instance of the most derived managed class.</para>
		/// </remarks>
		public static NSObject? GetNSObject (NativeHandle ptr, bool owns)
		{
			return GetNSObject ((IntPtr) ptr, owns, MissingCtorResolution.ThrowConstructor1NotFound);
		}

		/// <param name="ptr">A pointer to an unmanaged NSObject or any class that derives from the Objective-C NSObject class.</param>
		/// <summary>Wraps an unmanaged IntPtr into a fully typed NSObject, or returns an existing wrapper object if one already exists.</summary>
		/// <returns>An instance of a class that derives from Foundation.NSObject.</returns>
		/// <remarks>
		///   <para>The runtime create an instance of the most derived class.</para>
		/// </remarks>
		public static NSObject? GetNSObject (IntPtr ptr)
		{
			return GetNSObject (ptr, MissingCtorResolution.ThrowConstructor1NotFound);
		}

		internal static NSObject? GetNSObject (IntPtr ptr, MissingCtorResolution missingCtorResolution, bool evenInFinalizerQueue = false)
		{
			return GetNSObject (ptr, false, missingCtorResolution, evenInFinalizerQueue);
		}

		internal static NSObject? GetNSObject (IntPtr ptr, bool owns, MissingCtorResolution missingCtorResolution, bool evenInFinalizerQueue = false)
		{
			if (ptr == IntPtr.Zero)
				return null;

			var o = TryGetNSObject (ptr, evenInFinalizerQueue);

			// Fallback for issue #25861: a user type still executing its own 'init' isn't in
			// the object_map yet (deferred until 'init' completes), but carries its gchandle
			// in a native ivar. Safe here because GetNSObject is only called for Objective-C
			// objects. See TryGetNSObjectFromIvar for why this can't live in TryGetNSObject.
			o ??= TryGetNSObjectFromIvar (ptr, evenInFinalizerQueue);

			if (o is not null) {
				if (owns)
					o.DangerousRelease ();
				return o;
			}

			o = ConstructNSObject (ptr, Class.GetClassForObject (ptr), missingCtorResolution);
			if (owns)
				NSObject.DangerousRelease (ptr);
			return o;
		}

		/// <summary>Wraps an unmanaged <see cref="IntPtr" /> into a fully typed <see cref="NSObject" />, or returns an existing wrapper object if one already exists.</summary>
		/// <typeparam name="T">Type to wrap the native object as.</typeparam>
		/// <param name="ptr">A pointer to an unmanaged <see cref="NSObject" /> or any class that derives from the Objective-C <see cref="NSObject" /> class.</param>
		/// <returns>An instance of the class <typeparamref name="T" />.</returns>
		/// <remarks>
		///   <para>Returns an instance of the <typeparamref name="T" /> class even if the native object is not in the class hierarchy of <typeparamref name="T" /> (no type checks).</para>
		///   <para>This method will fail if there already is a managed wrapper of a different and incompatible type for the native object.</para>
		/// </remarks>
		static public T? GetNSObject<T> (IntPtr ptr) where T : NSObject
		{
			return GetNSObject<T> (ptr, IntPtr.Zero, default (RuntimeMethodHandle), false, false);
		}

		/// <summary>Wraps an unmanaged <see cref="IntPtr" /> into a fully typed <see cref="NSObject" />, or returns an existing wrapper object if one already exists.</summary>
		/// <typeparam name="T">Type to wrap the native object as.</typeparam>
		/// <param name="ptr">A pointer to an unmanaged <see cref="NSObject" /> or any class that derives from the Objective-C <see cref="NSObject" /> class.</param>
		/// <returns>An instance of the class <typeparamref name="T" />.</returns>
		/// <remarks>
		///   <para>Contrary to <see cref="GetNSObject{T}(IntPtr)" />, this method will throw an exception if the native object's type is not compatible with <typeparamref name="T" />.</para>
		///   <para>This method will fail if there already is a managed wrapper of a different and incompatible type for the native object.</para>
		/// </remarks>
		// Not doing "GetNSObject(IntPtr, bool checked)" because that signature very much looks like the second parameter is an "owns" parameter, a common signatures in similar methods.
		public static T? GetNSObjectChecked<T> (IntPtr ptr) where T : NSObject
		{
			return GetNSObject<T> (ptr, IntPtr.Zero, default (RuntimeMethodHandle), false, true);
		}

		static T? GetNSObject<T> (IntPtr ptr, IntPtr sel, RuntimeMethodHandle method_handle) where T : NSObject
		{
			return GetNSObject<T> (ptr, sel, method_handle, false, false);
		}

		static T? GetNSObject<T> (IntPtr ptr, IntPtr sel, RuntimeMethodHandle method_handle, bool evenInFinalizerQueue) where T : NSObject
		{
			return GetNSObject<T> (ptr, sel, method_handle, evenInFinalizerQueue, false);
		}

		static T? GetNSObject<T> (IntPtr ptr, IntPtr sel, RuntimeMethodHandle method_handle, bool evenInFinalizerQueue, bool typeSafe) where T : NSObject
		{
			if (ptr == IntPtr.Zero)
				return null;

			var obj = TryGetNSObject (ptr, evenInFinalizerQueue: evenInFinalizerQueue);

			// Fallback for issue #25861: resolve a user type still executing its own 'init'
			// (not yet in the object_map) via its native gchandle ivar. See the non-generic
			// GetNSObject for why this is safe here.
			obj ??= TryGetNSObjectFromIvar (ptr, evenInFinalizerQueue);

			// First check if we got an object of the expected type
			if (obj is T o)
				return o;

			if (typeSafe && obj is not null)
				throw new InvalidCastException ($"Unable to cast object of type '{obj.GetType ().FullName}' to type '{typeof (T).FullName}'.");

			// We either didn't find an object, or it was of the wrong type, so we need to create a new instance.

			// Try to get the managed type that correspond to this exact native type
			IntPtr p = Class.GetClassForObject (ptr);
			// If unknown then we'll get the Class that Lookup to NSObject even if this is not NSObject.
			// We can use this condition to fallback on the provided (generic argument) type
			Type target_type;
			if (p != NSObjectClass) {
				target_type = Class.Lookup (p);
				if (target_type == typeof (NSObject))
					target_type = typeof (T);
				else if (typeof (T).IsGenericType)
					target_type = typeof (T);
				else if (target_type.IsSubclassOf (typeof (T))) {
					// do nothing, this is fine.
				} else if (Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("isKindOfClass:"), Class.GetHandle (typeof (T))) != 0) {
					// If the instance itself claims it's an instance of the provided (generic argument) type,
					// then we believe the instance. See bug #20692 for a test case.
					target_type = typeof (T);
				}
			} else {
				target_type = typeof (NSObject);
			}

			if (typeSafe) {
				obj = ConstructNSObject<NSObject> (ptr, target_type, MissingCtorResolution.ThrowConstructor1NotFound, sel, method_handle);
				if (obj is T o2)
					return o2;
				throw new InvalidCastException ($"Unable to cast object of type '{obj?.GetType ().FullName}' to type '{typeof (T).FullName}'.");
			}

			return ConstructNSObject<T> (ptr, target_type, MissingCtorResolution.ThrowConstructor1NotFound, sel, method_handle);
		}

		/// <typeparam name="T">Type to wrap the native object as.</typeparam>
		/// <param name="ptr">A pointer to an unmanaged NSObject or any class that derives from the Objective-C NSObject class.</param>
		/// <param name="owns">Pass true if the caller has a reference to the native object, and wants to give it to the managed wrapper instance. Otherwise pass false (and the native object will be retained).</param>
		/// <summary>Wraps an unmanaged IntPtr into a fully typed NSObject, or returns an existing wrapper object if one already exists.</summary>
		/// <returns>An instance of the T class.</returns>
		/// <remarks>
		///   <para>Returns an instance of the T class even if the native object is not in the class hierarchy of T (no type checks).</para>
		///   <para>This method will fail if there already is a managed wrapper of a different (and incompatible) type for the native object.</para>
		/// </remarks>
		static public T? GetNSObject<T> (IntPtr ptr, bool owns) where T : NSObject
		{
			var obj = GetNSObject<T> (ptr);
			if (owns)
				obj?.DangerousRelease ();
			return obj;
		}

		//
		// This method is an ugly hack.
		//
		// Existing apps (may) work even if GetNSObject doesn't return the expected type. Example:
		// https://bugzilla.xamarin.com/show_bug.cgi?id=13518 - put bluntly: we return some type
		// the caller doesn't expect, the caller does not verify the type and continues happily,
		// violating type safety.
		//
		// Sometimes this works (#13518), sometimes it doesn't (#14075).
		//
		// This GetNSObject overload takes an additional @target_type, which determines the type
		// to create if there is no wrapper type for the actual type of the native object. Note
		// in particular that we do not check in the other code paths if the type we return is
		// compatible with @target_type.
		//
		// FIXME: this hack should become redundant once we've implement support for generic
		// NSObject subclasses (the test case in #13518 should work even with type checks).
		//

		// The 'selector' and 'method' arguments are only used in error messages.
		static NSObject? GetNSObject (IntPtr ptr, Type target_type, MissingCtorResolution missingCtorResolution, bool evenInFinalizerQueue, bool createNewInstanceIfWrongType, out bool created)
		{
			created = false;

			if (ptr == IntPtr.Zero)
				return null;

			var o = TryGetNSObject (ptr, evenInFinalizerQueue);

			if (o is not null) {
				if (!createNewInstanceIfWrongType) {
					// We don't care if we found an instance of the wrong type or not, so just return whatever we got.
					return o;
				}

				// if our target type is a byref type, get the element type, otherwise the IsAssignableFrom method doesn't work as expected.
				var acceptibleTargetType = target_type;
				if (acceptibleTargetType.IsByRef)
					acceptibleTargetType = acceptibleTargetType.GetElementType ()!;
				if (acceptibleTargetType.IsAssignableFrom (o.GetType ())) {
					// We found an instance of an acceptable type! We're done here.
					return o;
				}

				// We found an instance of the wrong type, and we're asked to not return that.
				// So fall through to create a new instance instead.
			}

			// Try to get the managed type that correspond to this exact native type
			IntPtr p = Class.GetClassForObject (ptr);
			// If unknown then we'll get the Class that Lookup to NSObject even if this is not NSObject.
			// We can use this condition to fallback on the provided (generic argument) type

			if (p != NSObjectClass) {
				var dynamic_type = Class.Lookup (p);
				if (dynamic_type == typeof (NSObject)) {
					// nothing to do
				} else if (dynamic_type.IsSubclassOf (target_type)) {
					target_type = dynamic_type;
				} else if (Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("isKindOfClass:"), Class.GetHandle (target_type)) != 0) {
					// nothing to do
				} else {
					target_type = dynamic_type;
				}
			} else {
				target_type = typeof (NSObject);
			}

			created = true;
			return ConstructNSObject<NSObject> (ptr, target_type, MissingCtorResolution.ThrowConstructor1NotFound);
		}

		static Type LookupINativeObjectImplementation (IntPtr ptr, Type target_type, Type? implementation = null, bool forced_type = false)
		{
			if (!typeof (NSObject).IsAssignableFrom (target_type)) {
				// If we're not expecting an NSObject, we can't do a dynamic lookup of the type of ptr,
				// since we do not know if the object actually supports dynamic type lookup (it can be an
				// INativeObject which is just a managed wrapper around a native handle). Example: bug #24438,
				// where we're asked to lookup a CGLContext (we crash if we call Class.GetClassForObject on
				// such a pointer).
				implementation = target_type;
			} else {
				// Lookup the ObjC type of the ptr and see if we can use it.
				var p = Class.GetClassForObject (ptr);

				if (p == NSObjectClass) {
					if (implementation is null)
						implementation = target_type;
				} else {
					// only throw if we're not forcing the type we want to expose
					var runtime_type = Class.Lookup (p, throw_on_error: !forced_type);
					// Check if the runtime type can actually be used.
					if (target_type.IsAssignableFrom (runtime_type)) {
						implementation = runtime_type;
					} else if (implementation is null) {
						implementation = target_type;
					}
				}
			}

			var interface_check_type = implementation;
			// https://github.com/dotnet/runtime/issues/39068
			if (interface_check_type.IsByRef)
				interface_check_type = interface_check_type.GetElementType ();

			if (interface_check_type!.IsInterface)
				implementation = FindProtocolWrapperType (implementation);

			return implementation!;
		}

		/// <summary>Wraps a native object pointer in a managed object of the specified type.</summary>
		/// <param name="ptr">A pointer to the native object.</param>
		/// <param name="owns">Pass <see langword="true"/> if the caller transfers ownership of a native reference to the managed wrapper; otherwise pass <see langword="false"/>.</param>
		/// <param name="target_type">The managed type the native object should be wrapped as.</param>
		/// <returns>A managed object implementing <paramref name="target_type"/>, or <see langword="null"/> if <paramref name="ptr"/> is <see cref="IntPtr.Zero"/>.</returns>
		/// <remarks>This method may return an existing wrapper if one has already been created for the native object.</remarks>
		public static INativeObject? GetINativeObject (IntPtr ptr, bool owns, Type target_type)
		{
			return GetINativeObject (ptr, owns, target_type, null);
		}

		// this method is identical in behavior to the generic one.
		static INativeObject? GetINativeObject (IntPtr ptr, bool owns, Type target_type, Type? implementation)
		{
			return GetINativeObject (ptr, owns, target_type, implementation, IntPtr.Zero, default (RuntimeMethodHandle));
		}

		// this method is identical in behavior to the generic one.
		static INativeObject? GetINativeObject (IntPtr ptr, bool owns, Type target_type, Type? implementation, IntPtr sel, RuntimeMethodHandle method_handle)
		{
			if (ptr == IntPtr.Zero)
				return null;

			var o = TryGetNSObject (ptr, evenInFinalizerQueue: false);
			if (o is not null && target_type.IsAssignableFrom (o.GetType ())) {
				// found an existing object with the right type.
				return o;
			}

			if (o is not null) {
				var interface_check_type = target_type;
				// https://github.com/dotnet/runtime/issues/39068
				if (interface_check_type.IsByRef)
					interface_check_type = interface_check_type.GetElementType ()!;
				// found an existing object, but with an incompatible type.
				if (!interface_check_type.IsInterface) {
					// if the target type is another class, there's nothing we can do.
					throw new InvalidCastException ($"Unable to cast object of type '{o.GetType ().FullName}' to type '{target_type.FullName}'.");
				}
			}

			// Lookup the ObjC type of the ptr and see if we can use it.
			implementation = LookupINativeObjectImplementation (ptr, target_type, implementation);

			if (implementation.IsSubclassOf (typeof (NSObject))) {
				if (o is not null) {
					// We already have an instance of an NSObject-subclass for this ptr.
					// Creating another will break the one-to-one assumption we have between
					// native objects and NSObject instances.
					throw ErrorHelper.CreateError (8004, $"Cannot create an instance of {implementation.FullName} for the native object 0x{ptr:x} (of type '{Class.class_getName (Class.GetClassForObject (ptr))}'), because another instance already exists for this native object (of type {o.GetType ().FullName}).");
				}
				return ConstructNSObject<NSObject> (ptr, implementation!, MissingCtorResolution.ThrowConstructor1NotFound, sel, method_handle);
			}

			return ConstructINativeObject<INativeObject> (ptr, owns, implementation, target_type, MissingCtorResolution.ThrowConstructor2NotFound, sel, method_handle);
		}

		// this method is identical in behavior to the non-generic one.
		/// <typeparam name="T">The type of the object to return. This can also be an interface corresponding to an Objective-C protocol.</typeparam>
		/// <param name="ptr">A pointer to a native object.</param>
		/// <param name="owns">Pass true if the caller has a reference to the native object, and wants to give it to the managed wrapper instance. Otherwise pass false (and the native object will be retained).</param>
		/// <summary>Wraps an native IntPtr with a managed object of the specified type.</summary>
		/// <returns>An instance of a class implementing the specified type.</returns>
		/// <remarks>
		///   <para>Returns an instance of the specified type even if the native object is not in the class hierarchy of type (there are no type checks).</para>
		/// </remarks>
		public static T? GetINativeObject<T> (IntPtr ptr, bool owns) where T : INativeObject
		{
			return GetINativeObject<T> (ptr, false, owns);
		}

		public static T? GetINativeObject<T> (IntPtr ptr, bool forced_type, bool owns) where T : INativeObject
		{
			return GetINativeObject<T> (ptr, forced_type, null, owns);
		}

		internal static T? GetINativeObject<T> (IntPtr ptr, bool forced_type, Type? implementation, bool owns) where T : INativeObject
		{
			return GetINativeObject<T> (ptr, forced_type, implementation, owns, IntPtr.Zero, default (RuntimeMethodHandle));
		}

		static T? GetINativeObject<T> (IntPtr ptr, bool forced_type, Type? implementation, bool owns, IntPtr sel, RuntimeMethodHandle method_handle) where T : INativeObject
		{
			if (ptr == IntPtr.Zero)
				return default (T);

			var o = TryGetNSObject (ptr, evenInFinalizerQueue: false);
			if (o is T t) {
				// found an existing object with the right type.
				if (owns)
					TryReleaseINativeObject (t);
				return t;
			}

			// If forced type is true, we ignore any existing instances if the managed type of the existing instance isn't compatible with T.
			// This may end up creating multiple managed wrapper instances for the same native handle,
			// which is not optimal, but sometimes the alternative can be worse :/
			if (o is not null && !forced_type) {
				// found an existing object, but with an incompatible type.
				if (!typeof (T).IsInterface && typeof (NSObject).IsAssignableFrom (typeof (T))) {
					// if the target type is another NSObject subclass, there's nothing we can do.
					throw new InvalidCastException ($"Unable to cast object of type '{o.GetType ().FullName}' to type '{typeof (T).FullName}'.");
				}
			}

			// Lookup the ObjC type of the ptr and see if we can use it.
			implementation = LookupINativeObjectImplementation (ptr, typeof (T), implementation, forced_type: forced_type);

			if (implementation.IsSubclassOf (typeof (NSObject))) {
				if (o is not null && !forced_type) {
					// We already have an instance of an NSObject-subclass for this ptr.
					// Creating another will break the one-to-one assumption we have between
					// native objects and NSObject instances.
					throw ErrorHelper.CreateError (8004, $"Cannot create an instance of {implementation.FullName} for the native object 0x{ptr:x} (of type '{Class.class_getName (Class.GetClassForObject (ptr))}'), because another instance already exists for this native object (of type {o.GetType ().FullName}).");
				}
				if (!Runtime.IsManagedStaticRegistrar && !Runtime.IsTrimmableStaticRegistrar) {
					// For other registrars other than managed-static the generic parameter of ConstructNSObject is used
					// only to cast the return value so we can safely pass NSObject here to satisfy the constraints of the
					// generic parameter.
					var rv = (T?) (INativeObject?) ConstructNSObject<NSObject> (ptr, implementation, MissingCtorResolution.ThrowConstructor1NotFound, sel, method_handle);
					if (owns)
						TryReleaseINativeObject (rv);
					return rv;
				}
			}

			return ConstructINativeObject<T> (ptr, owns, implementation, typeof (T), MissingCtorResolution.ThrowConstructor2NotFound, sel, method_handle);
		}

		internal static void TryReleaseINativeObject (INativeObject? obj)
		{
			if (obj is null)
				return;

			if (obj is NativeObject nobj) {
				nobj.Release ();
				return;
			}

			if (obj is NSObject nsobj) {
				nsobj.DangerousRelease ();
				return;
			}

			throw ErrorHelper.CreateError (8045, Xamarin.Bundler.Errors.MX8045 /* Unable to call release on an instance of the type {0}" */, obj.GetType ().FullName);
		}

		static Type? FindProtocolWrapperType (Type? type)
		{
			if (type is null)
				return null;

			// https://github.com/dotnet/runtime/issues/39068
			if (type.IsByRef)
				type = type.GetElementType ()!;

			if (!type.IsInterface)
				return null;

			// Check if the static registrar knows about this protocol
			if (IsManagedStaticRegistrar) {
				var rv = RegistrarHelper.FindProtocolWrapperType (type);
				if (rv is not null)
					return rv;
			} else if (IsTrimmableStaticRegistrar) {
				if (TypeMaps.ProtocolWrapperTypes.TryGetValue (type, out var protocolWrapperType))
					return protocolWrapperType;
			} else {
				unsafe {
					var map = options->RegistrationMap;
					if (map is not null) {
						var token = Class.GetTokenReference (type, throw_exception: false);
						if (token != INVALID_TOKEN_REF) {
							var wrapper_token = xamarin_find_protocol_wrapper_type (token);
							if (wrapper_token != INVALID_TOKEN_REF)
								return Class.ResolveTypeTokenReference (wrapper_token);
						}
					}
				}
			}

			// need to look up the type from the ProtocolAttribute.
			var a = type.GetCustomAttributes (typeof (Foundation.ProtocolAttribute), false);

			var attr = (Foundation.ProtocolAttribute?) (a.Length > 0 ? a [0] : null);
			if (attr is null || attr.WrapperType is null)
				throw ErrorHelper.CreateError (4125, $"The registrar found an invalid interface '{type.FullName}': " +
					"The interface must have a Protocol attribute specifying its wrapper type.");
			return attr.WrapperType;
		}

		[DllImport ("__Internal")]
		extern static uint xamarin_find_protocol_wrapper_type (uint token_ref);

		/// <param name="protocol">Name of the Objective-C protocol.</param>
		/// <summary>Returns the handle of the Objective-C protocol descriptor for the given protocol name.</summary>
		/// <returns>The protocol handle for the given protocol name.</returns>
		/// <remarks>
		///   <para>This is the equivalent of the objc_getProtocol function call.</para>
		/// </remarks>
		public static IntPtr GetProtocol (string protocol)
		{
			return Protocol.objc_getProtocol (protocol);
		}

		internal static IntPtr GetProtocolForType (Type type)
		{
			// Check if the trimmable static registrar knows about this protocol
			if (IsTrimmableStaticRegistrar) {
				if (TypeMaps.TryGetProtocolProxyAttribute (type, out var attrib)) {
#if LOG_TRIMMABLE_TYPEMAP
					NSLog ($"GetProtocolForType ({type.FullName}) found protocol proxy attribute");
#endif
					var protocolName = attrib.GetProtocolName ();
					return Protocol.objc_getProtocol (protocolName);
				}

#if LOG_TRIMMABLE_TYPEMAP
				NSLog ($"GetProtocolForType ({type.FullName}) NOT found in protocol proxy map");
#endif

				return IntPtr.Zero;
			}

			// Check if the static registrar knows about this protocol
			unsafe {
				var map = options->RegistrationMap;
				if (map is not null && map->protocol_count > 0) {
					var token = Class.GetTokenReference (type);
					var tokens = map->protocol_map.protocol_tokens;
					for (int i = 0; i < map->protocol_count; i++) {
						if (tokens [i] == token)
							return map->protocol_map.protocols [i];
					}
				}
			}

			if (type.IsInterface) {
				var pa = type.GetCustomAttribute<ProtocolAttribute> (false);
				if (pa is not null) {
					var handle = Protocol.objc_getProtocol (pa.Name);
					if (handle != IntPtr.Zero)
						return handle;
				}
			}

			throw new ArgumentException ($"'{type.FullName}' is an unknown protocol");
		}

		internal static bool IsUserType (IntPtr self)
		{
			if (!TryGetIsUserType (self, out var result, out var error_message))
				throw new InvalidOperationException ($"Unable to get the class of the pointer 0x{self.ToString ("x")}: {error_message}");
			return result;
		}

		internal static bool TryGetIsUserType (IntPtr self, out bool isUserType, [NotNullWhen (false)] out string? error_message)
		{
			isUserType = false;
			if (!Class.TryGetClass (self, out var cls, out error_message))
				return false;

			lock (usertype_cache) {
				ref var result = ref CollectionsMarshal.GetValueRefOrAddDefault (usertype_cache, cls, out var exists);
				if (!exists)
					result = SlowIsUserType (cls);
				isUserType = result;
				return true;
			}
		}

		[DllImport ("__Internal")]
		extern static byte xamarin_is_user_type (IntPtr cls);

		[BindingImpl (BindingImplOptions.Optimizable)]
		static bool SlowIsUserType (IntPtr cls)
		{
			unsafe {
				if (options->RegistrationMap is not null && options->RegistrationMap->map_count > 0) {
					var map = options->RegistrationMap->map;
					var idx = Class.FindMapIndex (map, 0, options->RegistrationMap->map_count - 1, cls);
					if (idx >= 0)
						return (map [idx].flags & MTTypeFlags.UserType) == MTTypeFlags.UserType;
					// If using the partial static registrar, we need to continue
					// If full static registrar, we can return false, as long as the dynamic registrar is not supported
					if (!DynamicRegistrationSupported && (options->Flags & InitializationFlags.IsPartialStaticRegistrar) != InitializationFlags.IsPartialStaticRegistrar)
						return false;
				}
			}

			return xamarin_is_user_type (cls) != 0;
		}

		/// <param name="type">Connect to the selector on this type.</param>
		/// <param name="method">Method that will be called when Objective-C sends a message to the specified selector.</param>
		/// <param name="selector">Selector to connect to.</param>
		/// <summary>This call allows the specified method in this method to respond to message invocations on the specified selector.</summary>
		/// <remarks>
		///   <para>The method must be declared on an NSObject-derived class.</para>
		///   <para>Developers can use this method to dynamically reconfigure which methods on a class should respond to which Objective-C selectors.</para>
		/// </remarks>
		public static void ConnectMethod (Type type, MethodInfo method, Selector selector)
		{
			if (selector is null)
				throw new ArgumentNullException (nameof (selector));

			ConnectMethod (type, method, new ExportAttribute (selector.Name));
		}

		/// <param name="type">Connect to the selector on this type.</param>
		/// <param name="method">Method that will be called when Objective-C sends a message to the specified selector.</param>
		/// <param name="export">An export attribute that specifies the selector to connect to.</param>
		/// <summary>This call allows the specified method in this method to respond to message invocations on the specified selector.</summary>
		/// <remarks>
		///   <para>The method must be declared on an NSObject-derived class.</para>
		///   <para>Developers can use this method to dynamically reconfigure which methods on a class should respond to which Objective-C selectors.</para>
		/// </remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void ConnectMethod (Type type, MethodInfo method, ExportAttribute export)
		{
			if (type is null)
				throw new ArgumentNullException (nameof (type));

			if (method is null)
				throw new ArgumentNullException (nameof (method));

			if (export is null)
				throw new ArgumentNullException (nameof (export));

			if (!DynamicRegistrationSupported)
				throw ErrorHelper.CreateError (8026, "Runtime.ConnectMethod is not supported when the dynamic registrar has been linked away.");

			Registrar.RegisterMethod (type, method, export);
		}

		/// <param name="method">Method that will be called when Objective-C sends a message to the specified selector.</param>
		/// <param name="selector">Selector to connect to.</param>
		/// <summary>This call allows the specified method in this method to respond to message invocations on the specified selector.</summary>
		/// <remarks>
		///   <para>The method must be declared on an NSObject-derived class.</para>
		///   <para>Developers can use this method to dynamically reconfigure which methods on a class should respond to which Objective-C selectors.</para>
		/// </remarks>
		public static void ConnectMethod (MethodInfo method, Selector selector)
		{
			if (method is null)
				throw new ArgumentNullException (nameof (method));

			ConnectMethod (method.DeclaringType!, method, selector);
		}

		[DllImport ("__Internal")]
		extern static void xamarin_log (IntPtr s);

		[DllImport (Constants.libcLibrary)]
		extern static nint write (int filedes, byte [] buf, nint nbyte);

		internal static void NSLog (string value)
		{
			try {
				using var valuePtr = new TransientString (value, TransientString.Encoding.Unicode);
				xamarin_log (valuePtr);
			} catch {
				// Append a newline like NSLog does
				if (!value.EndsWith ('\n'))
					value += "\n";
				// Don't use Console.WriteLine, since that brings in a lot of supporting code and may bloat apps.
				var utf8 = Encoding.UTF8.GetBytes (value);
				write (2 /* STDERR */, utf8, utf8.Length);
				// Ignore any errors writing to stderr (might happen on devices if the developer tools haven't been mounted, but xamarin_log should always work on devices).
			}
		}

		internal static string ToFourCCString (uint value)
		{
			return ToFourCCString (unchecked((int) value));
		}

		internal static string ToFourCCString (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value,
			});
		}

		// Retain the input if it's either an NSObject or a NativeObject.
		static void RetainNativeObject (IntPtr gchandle)
		{
			var obj = GetGCHandleTarget (gchandle);
			RetainNativeObject ((INativeObject?) obj);
		}

		// Retain the input if it's either an NSObject or a NativeObject.
		static NativeHandle RetainNativeObject (INativeObject? obj)
		{
			if (obj is null)
				return NativeHandle.Zero;
			if (obj is NSObject nsobj)
				RetainNSObject (nsobj);
			else if (obj is NativeObject nobj)
				nobj.Retain ();
			return obj.GetHandle ();
		}

		internal static NativeHandle RetainNSObject (NSObject? obj)
		{
			if (obj is null)
				return NativeHandle.Zero;
			obj.DangerousRetain ();
#pragma warning disable RBI0014
			return obj.GetHandle ();
#pragma warning restore RBI0014
		}

		/// <summary>Retain and autorelease the given object, then return the object's handle.</summary>
		/// <param name="obj">The object to retain and autorelease.</param>
		/// <returns>The object's handle (retained and autorelease).</returns>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static NativeHandle RetainAndAutoreleaseNSObject (NSObject? obj)
		{
			if (obj is null)
				return NativeHandle.Zero;
			obj.DangerousRetain ();
			obj.DangerousAutorelease ();
#pragma warning disable RBI0014
			return obj.GetHandle ();
#pragma warning restore RBI0014
		}

		/// <summary>Retain and autorelease the given object, then return the object's handle.</summary>
		/// <param name="obj">The object to retain and autorelease.</param>
		/// <returns>The object's handle (retained and autorelease).</returns>
		/// <remarks>If the given object is not an `NSObject`, then the handle won't be retained/autoreleased.</remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static NativeHandle RetainAndAutoreleaseNativeObject (INativeObject? obj)
		{
			if (obj is null)
				return NativeHandle.Zero;
			if (obj is NSObject nsobj) {
				nsobj.DangerousRetain ();
				nsobj.DangerousAutorelease ();
			}
			return obj.GetHandle ();
		}

		/// <summary>Retain and autorelease the given object, then return the object's handle.</summary>
		/// <param name="obj">The object to retain and autorelease.</param>
		/// <returns>The object's handle (retained and autorelease).</returns>
		/// <remarks>The behavior is undefined if the handle's type doesn't support the 'retain' and 'autorelease' selectors.</remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
		internal static NativeHandle RetainAndAutoreleaseHandle (INativeObject? obj)
		{
			if (obj is null)
				return NativeHandle.Zero;
			var rv = RetainAndAutoreleaseHandle (obj.GetHandle ());
			GC.KeepAlive (obj);
			return rv;
		}

		/// <summary>Retain and autorelease the given handle, then return the handle.</summary>
		/// <param name="handle">The handle to retain and autorelease.</param>
		/// <returns>The handle (retained and autorelease).</returns>
		/// <remarks>The behavior is undefined if the handle's type doesn't support the 'retain' and 'autorelease' selectors.</remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
		internal static NativeHandle RetainAndAutoreleaseHandle (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return NativeHandle.Zero;
			NSObject.DangerousRetain (handle);
			NSObject.DangerousAutorelease (handle);
			return handle;
		}

		static IntPtr CopyAndAutorelease (IntPtr ptr)
		{
			ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("copy"));
			NSObject.DangerousAutorelease (ptr);
			return ptr;
		}

		// Check if the input is an NSObject, and in that case retain it (and return true)
		// This way the caller knows if it can call 'autorelease' on our input.
		static nint AttemptRetainNSObject (IntPtr gchandle)
		{
			var obj = GetGCHandleTarget (gchandle) as NSObject;
			obj?.DangerousRetain ();
			var rv = obj is not null;
			return rv ? 1 : 0;
		}
#endif // !COREBUILD

		static Version? SystemVersion;

		internal static bool CheckSystemVersion (int major, int minor, string systemVersion)
		{
			return CheckSystemVersion (major, minor, 0, systemVersion);
		}

		internal static bool CheckSystemVersion (int major, int minor, int build, string systemVersion)
		{
			if (SystemVersion is null) {
				if (!Version.TryParse (systemVersion, out SystemVersion))
					SystemVersion = new Version (2, 0, 0);
			}

			if (SystemVersion.Major > major)
				return true;
			else if (SystemVersion.Major < major)
				return false;

			if (SystemVersion.Minor > minor)
				return true;
			else if (SystemVersion.Minor < minor)
				return false;

			return (SystemVersion.Build == -1) || (SystemVersion.Build >= build);
		}

		internal unsafe static IntPtr CloneMemory (IntPtr source, long length)
		{
			var rv = Marshal.AllocHGlobal ((IntPtr) length);
			Buffer.MemoryCopy ((void*) source, (void*) rv, length, length);
			return rv;
		}

		// This function will try to compare a native UTF8 string to a managed string without creating a temporary managed string for the native UTF8 string.
		// Currently this only works if the UTF8 string only contains single-byte characters.
		// If any multi-byte characters are found, the native utf8 string is converted to a managed string, and then normal managed comparison is done.
		internal static bool StringEquals (IntPtr utf8, string? str)
		{
			if (str is null)
				return utf8 == IntPtr.Zero;

			if (utf8 == IntPtr.Zero)
				return false;

			// The vast majority of strings we compare fall within the single-byte UTF8 range, so optimize for this
			unsafe {
				byte* c = (byte*) utf8;
				for (int i = 0; i < str.Length; i++) {
					byte b = c [i];
					if (b > 0x7F) {
						// This string is a multibyte UTF8 string, so go the slow route and convert it to a managed string before comparison
						return string.Equals (Marshal.PtrToStringUTF8 (utf8), str, StringComparison.Ordinal);
					}
					if (b != (short) str [i])
						return false;
					if (b == 0)
						return false;
				}
				return c [str.Length] == 0;
			}
		}

		// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
		//
		// IL2070: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.All' in call to 'System.Type.GetMemberWithSameMetadataDefinitionAs(MemberInfo)'. The parameter 'closed_type' of method 'ObjCRuntime.Runtime.FindClosedMethod(Type, MethodBase)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2070", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		internal static MethodInfo FindClosedMethod (Type closed_type, MethodBase open_method)
		{
			// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception in that case
			if (IsNativeAOT)
				throw Runtime.CreateNativeAOTNotSupportedException ();

			// FIXME: I think it should be handled before getting here (but it's safer here for now)
			if (!open_method.ContainsGenericParameters)
				return (MethodInfo) open_method;

			// First we need to find the type that declared the open method.
			var declaring_closed_type = closed_type;
			do {
				if (declaring_closed_type.IsGenericType && declaring_closed_type.GetGenericTypeDefinition () == open_method.DeclaringType) {
					closed_type = declaring_closed_type;
					break;
				}
				declaring_closed_type = declaring_closed_type.BaseType;
			} while (declaring_closed_type is not null);

			// Find the closed method.
			if (closed_type.GetMemberWithSameMetadataDefinitionAs (open_method) is MethodInfo mi)
				return mi;

			throw ErrorHelper.CreateError (8003, $"Failed to find the closed generic method '{open_method.Name}' on the type '{closed_type.FullName}'.");
		}

		internal static MethodInfo FindClosedMethod (object instance, RuntimeTypeHandle open_type_handle, RuntimeMethodHandle open_method_handle)
		{
			var closed_type = instance.GetType ()!;
			var open_type = Type.GetTypeFromHandle (open_type_handle)!;
			closed_type = FindClosedTypeInHierarchy (open_type, closed_type)!;
			var closedMethod = MethodBase.GetMethodFromHandle (open_method_handle, closed_type.TypeHandle)!;
			return (MethodInfo) closedMethod;
		}

		static Type? FindClosedTypeInHierarchy (Type open_type, Type? closed_type)
		{
			if (closed_type is null)
				return null;

			var closed_type_definition = closed_type;
			if (closed_type_definition.IsGenericType)
				closed_type_definition = closed_type_definition.GetGenericTypeDefinition ();
			if (closed_type_definition == open_type)
				return closed_type;
			return FindClosedTypeInHierarchy (open_type, closed_type.BaseType);
		}

		internal static Type FindClosedParameterType (object instance, RuntimeTypeHandle open_type_handle, RuntimeMethodHandle open_method_handle, int parameter)
		{
			var closed_type = instance.GetType ()!;
			var open_type = Type.GetTypeFromHandle (open_type_handle)!;
			closed_type = FindClosedTypeInHierarchy (open_type, closed_type)!;
			var closedMethod = MethodBase.GetMethodFromHandle (open_method_handle, closed_type.TypeHandle)!;
			var parameters = closedMethod.GetParameters ();
			return parameters [parameter].ParameterType.GetElementType ()!; // FIX NAMING
		}

		// Caches the closed generic helper method per (open helper method, closed instance type), so
		// that the reflection cost (FindClosedTypeInHierarchy + MakeGenericMethod) is only paid once
		// per instantiation instead of on every call.
		//
		// The cache lives in a nested type (rather than as a field initializer on Runtime) so its
		// ConcurrentDictionary instantiation is constructed by the nested type's static constructor
		// - which only runs the first time InvokeGenericRegistrarTrampoline actually accesses it.
		// InvokeGenericRegistrarTrampoline is only ever reachable under a Hot-Reload-compatible
		// build, so in every other configuration (e.g. Release/NativeAOT) the trampoline is trimmed
		// and this nested type - together with the ConcurrentDictionary instantiation - is trimmed
		// with it, keeping it out of Runtime's static constructor and out of the app.
		static class ClosedGenericRegistrarTrampolines {
			internal static readonly ConcurrentDictionary<(RuntimeMethodHandle OpenImplMethod, RuntimeTypeHandle ClosedInstanceType), MethodInfo> Cache = new ();
		}

		// Invokes a relocated generic registrar trampoline helper.
		//
		// When the trimmable static registrar relocates the registrar trampolines into a companion
		// assembly (for Hot Reload, so the user assembly stays unmodified), the trampoline for a
		// method declared in a generic type can't be relocated as-is: the static UnmanagedCallersOnly
		// callback doesn't know the generic parameters of the instance, and we can't add the
		// virtual-dispatch proxy to the user type (that would modify the user assembly). Instead we
		// emit a *generic* helper method ('open_impl' below) into the companion assembly whose body
		// performs the type-parameter-aware conversions, and the static callback dispatches to it
		// here: we close the generic helper over the instance's actual generic arguments and invoke
		// it. This reflection-based dispatch is only ever used under a Hot-Reload-compatible build,
		// which always runs under the JIT (never NativeAOT), so MakeGenericMethod is safe.
		//
		// 'args' holds all the arguments of the helper: the instance, followed by the native
		// arguments, followed by the trailing 'out IntPtr exception_gchandle' slot. The callback
		// allocates the array with that extra slot so that no copy is needed here. This method
		// invokes the closed helper and returns its (boxed) native return value. Any failure is
		// caught and reported through 'exception_gchandle' so no exception escapes the
		// UnmanagedCallersOnly boundary.
		internal static object? InvokeGenericRegistrarTrampoline (object instance, RuntimeTypeHandle open_user_type_handle, RuntimeMethodHandle open_impl_method_handle, object? [] args, out IntPtr exception_gchandle)
		{
			// This method uses reflection (MakeGenericMethod + MethodInfo.Invoke), which requires
			// dynamic code and isn't supported by NativeAOT. It's only ever reached under a
			// Hot-Reload-compatible build, which always runs under the JIT (never NativeAOT). The
			// IsNativeAOT check is optimizable, so the linker folds it to a constant and removes the
			// reflection code below as unreachable in a NativeAOT build - which also elides the
			// IL2060/IL3050 trimmer/AOT warnings that MakeGenericMethod would otherwise produce.
			if (IsNativeAOT)
				throw CreateNativeAOTNotSupportedException ();

			exception_gchandle = IntPtr.Zero;
			try {
				var closed_instance_type = instance.GetType ();
				var cache_key = (open_impl_method_handle, closed_instance_type.TypeHandle);
				MethodInfo closed_impl;
				if (ClosedGenericRegistrarTrampolines.Cache.TryGetValue (cache_key, out var cached_impl)) {
					closed_impl = cached_impl;
				} else {
					var open_user_type = Type.GetTypeFromHandle (open_user_type_handle);
					if (open_user_type is null)
						throw new InvalidOperationException ("Could not resolve the open generic type of a relocated registrar trampoline.");
					var closed_user_type = FindClosedTypeInHierarchy (open_user_type, closed_instance_type);
					if (closed_user_type is null)
						throw new InvalidOperationException ($"Could not find the type '{open_user_type.FullName}' in the type hierarchy of '{closed_instance_type.FullName}'.");
					if (MethodBase.GetMethodFromHandle (open_impl_method_handle) is not MethodInfo open_impl)
						throw new InvalidOperationException ("Could not resolve the generic helper method of a relocated registrar trampoline.");
					closed_impl = open_impl.MakeGenericMethod (closed_user_type.GetGenericArguments ());
					ClosedGenericRegistrarTrampolines.Cache.TryAdd (cache_key, closed_impl);
				}

				// The trailing 'out IntPtr exception_gchandle' slot is pre-initialized to IntPtr.Zero so
				// that the (success) code path, which leaves the byref output untouched, reports "no
				// exception".
				var exception_slot = args.Length - 1;
				args [exception_slot] = IntPtr.Zero;
				var rv = closed_impl.Invoke (null, args);
				exception_gchandle = (IntPtr) (args [exception_slot] ?? IntPtr.Zero);
				return rv;
			} catch (Exception e) {
				exception_gchandle = AllocGCHandle (e);
				return null;
			}
		}

		// This method might be called by the generated code from the managed static registrar.
		static void TraceCaller (string message)
		{
			var caller = new System.Diagnostics.StackFrame (1);
			NSLog ($"{caller?.GetMethod ()?.ToString ()}: {message}");
		}

		static void GCCollect ()
		{
			GC.Collect ();
		}

		/// <param name="block">The block to release.</param>
		/// <summary>Calls _Block_release on the specified block on the main thread.</summary>
		/// <remarks>
		///   <para>Developers should not call this method, it's called by generated binding code.</para>
		/// </remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DllImport ("__Internal", EntryPoint = "xamarin_release_block_on_main_thread")]
		public static extern void ReleaseBlockOnMainThread (IntPtr block);

		// This method will release the specified block, but not while the delegate is still alive.
		[EditorBrowsable (EditorBrowsableState.Never)]
		static Delegate ReleaseBlockWhenDelegateIsCollected (IntPtr block, Delegate @delegate)
		{
			if (block == IntPtr.Zero)
				return @delegate;

			if (@delegate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (@delegate));

			if (block_lifetime_table.TryGetValue (@delegate, out var existingCollector)) {
				existingCollector.Add (block);
			} else {
				block_lifetime_table.Add (@delegate, new BlockCollector (block));
			}
			return @delegate;
		}

		enum NXByteOrder /* unspecified in header, means most likely int */ {
			Unknown,
			LittleEndian,
			BigEndian,
		}

		[StructLayout (LayoutKind.Sequential)]
		struct NXArchInfo {
			IntPtr name; // const char *
			public int CpuType; // cpu_type_t -> integer_t -> int
			public int CpuSubType; // cpu_subtype_t -> integer_t -> int
			public NXByteOrder ByteOrder;
			IntPtr description; // const char *

			public string Name {
				get { return Marshal.PtrToStringUTF8 (name)!; }
			}

			public string Description {
				get { return Marshal.PtrToStringUTF8 (description)!; }
			}
		}

		[DllImport (Constants.libSystemLibrary)]
		static unsafe extern NXArchInfo* NXGetLocalArchInfo ();

		/// <summary>Gets whether the current process uses ARM64 calling conventions.</summary>
		/// <remarks>This is initialized during runtime startup.</remarks>
		public static bool IsARM64CallingConvention;

		[BindingImpl (BindingImplOptions.Optimizable)]
		static bool GetIsARM64CallingConvention ()
		{
			unsafe {
				return NXGetLocalArchInfo ()->Name.StartsWith ("arm64", StringComparison.OrdinalIgnoreCase);
			}
		}

		// Get the GCHandle from the IntPtr value and get the wrapped object.
		internal static object? GetGCHandleTarget (IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				return null;
			return GCHandle.FromIntPtr (ptr).Target;
		}

		// Returns the gchandle stored in the native object's gchandle ivar (user types
		// only; INVALID_GCHANDLE/zero otherwise).
		[DllImport ("__Internal")]
		static extern IntPtr xamarin_get_gchandle (IntPtr obj);

		internal static IntPtr GetGCHandleForObject (IntPtr ptr)
		{
			return xamarin_get_gchandle (ptr);
		}

		// Fallback lookup for issue #25861: a user-type object that hasn't been added to
		// the object_map yet (e.g. it's still executing its own 'init') can still be
		// resolved via the gchandle stored in its native ivar. Returns null for direct
		// bindings (which have no ivar) and for objects without a managed reference.
		internal static NSObject? TryGetNSObjectFromIvar (IntPtr ptr)
		{
			var gchandle = xamarin_get_gchandle (ptr);
			if (gchandle == IntPtr.Zero)
				return null;
			return GetGCHandleTarget (gchandle) as NSObject;
		}

		// Guarded ivar fallback for issue #25861: resolve a user type that's still executing
		// its own 'init' (and so isn't in the object_map yet) via its native gchandle ivar,
		// only returning it if it still owns 'ptr' and isn't queued for finalization (unless
		// asked otherwise). MUST only be called with an Objective-C object pointer: it sends
		// the xamarinGetGCHandle message, which crashes on non-Objective-C native handles.
		// That's why this isn't folded into the general TryGetNSObject (which can be called
		// with arbitrary native handles, e.g. from GetINativeObject); it's only safe from
		// GetNSObject/GetNSObject<T>, which are only ever called for Objective-C objects.
		static NSObject? TryGetNSObjectFromIvar (IntPtr ptr, bool evenInFinalizerQueue)
		{
			var fromIvar = TryGetNSObjectFromIvar (ptr);
			if (fromIvar is not null && fromIvar.Handle == ptr && (evenInFinalizerQueue || !fromIvar.InFinalizerQueue))
				return fromIvar;
			return null;
		}

		// Allocate a GCHandle and return the IntPtr to it.
		internal static IntPtr AllocGCHandle (object? value)
		{
			return AllocGCHandle (value, GCHandleType.Normal);
		}

		// Allocate a GCHandle and return the IntPtr to it.
		internal static IntPtr AllocGCHandle (object? value, GCHandleType type)
		{
			return GCHandle.ToIntPtr (GCHandle.Alloc (value, type));
		}

#if __MACCATALYST__
		static string? _iOSSupportVersion;
		internal static string iOSSupportVersion {
			get {
				if (_iOSSupportVersion is null) {
					// This is how Apple does it: https://github.com/llvm/llvm-project/blob/62ec4ac90738a5f2d209ed28c822223e58aaaeb7/lldb/source/Host/macosx/objcxx/HostInfoMacOSX.mm#L100-L105
					using var dict = NSMutableDictionary.FromFile ("/System/Library/CoreServices/SystemVersion.plist");
					using var str = (NSString) "iOSSupportVersion";
					using var obj = dict.ObjectForKey (str);
					_iOSSupportVersion = obj?.ToString () ?? "";
				}
				return _iOSSupportVersion;
			}
		}
#endif

		// Takes a GCHandle (as an IntPtr) for an exception, frees the GCHandle, and throws the exception.
		// If the IntPtr does not represent a valid GCHandle, then the function just returns.
		// This method must be public, because the generator can generate calls to it (thus third-party binding libraries may need it).
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static void ThrowException (IntPtr gchandle)
		{
			if (gchandle == IntPtr.Zero)
				return;
			var handle = GCHandle.FromIntPtr (gchandle);
			var exc = handle.Target as Exception;
			handle.Free ();

			if (exc is null)
				return;

			throw exc;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public static ulong ConvertNativeEnumValueToManaged (nuint value, bool targetTypeHasMaxValue)
		{
			return (ulong) value;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public static long ConvertNativeEnumValueToManaged (nint value, bool targetTypeHasMaxValue)
		{
			return (long) value;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public static nint ConvertManagedEnumValueToNative (long value)
		{
			return (nint) value;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public static nuint ConvertManagedEnumValueToNative (ulong value)
		{
			return (nuint) value;
		}

		/// <summary>Gets the original working directory used when the application process was launched.</summary>
		/// <value>The original working directory, or <see langword="null"/> if unavailable.</value>
		/// <remarks>This value is provided by the native runtime and may differ from the current working directory.</remarks>
		public static string? OriginalWorkingDirectory {
			get {
				return Marshal.PtrToStringUTF8 (xamarin_get_original_working_directory_path ());
			}
		}

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_get_original_working_directory_path ();

		static nint InvokeConformsToProtocol (IntPtr gchandle, IntPtr handle, IntPtr protocol)
		{
			var obj = GetGCHandleTarget (gchandle) as NSObject;
			if (obj is null) {
				// conformsToProtocol is special, we might get here before a managed instance has been created
				// for a native instance, in which case we need to create the managed instance (which GetNSObject will do)
				obj = GetNSObject (handle);
				if (obj is null)
					return 0;
			}
			var rv = obj.ConformsToProtocol (protocol);
			return rv ? 1 : 0;
		}

		static IntPtr LookupUnmanagedFunction (IntPtr assembly, IntPtr symbol, int id, IntPtr objcClassName)
		{
			return RegistrarHelper.LookupUnmanagedFunction (assembly, Marshal.PtrToStringAuto (symbol), id, Marshal.PtrToStringAuto (objcClassName));
		}

		// This option is turned on by setting _ValidateObjectPointers property to true in the project file.
		static bool validate_object_pointers;
		internal static bool ValidateObjectPointers {
			get => validate_object_pointers;
			set => validate_object_pointers = value;
		}

		/// <summary>Allocate unmanaged zeroed memory of the specified struct.</summary>
		/// <remarks>Call <see cref="NativeMemory.Free" /> to free the returned pointer.</remarks>
		internal unsafe static T* AllocZeroed<T> () where T : unmanaged
		{
			return (T*) NativeMemory.AllocZeroed ((nuint) sizeof (T));
		}
	}


	internal class IntPtrEqualityComparer : IEqualityComparer<IntPtr> {
		public bool Equals (IntPtr x, IntPtr y)
		{
			return x == y;
		}
		public int GetHashCode (IntPtr obj)
		{
			return obj.GetHashCode ();
		}
	}

	internal class UInt64EqualityComparer : IEqualityComparer<ulong> {
		public bool Equals (ulong x, ulong y)
		{
			return x == y;
		}
		public int GetHashCode (ulong obj)
		{
			return obj.GetHashCode ();
		}
	}

	internal class TypeEqualityComparer : IEqualityComparer<Type> {
		public bool Equals (Type? x, Type? y)
		{
			return (object?) x == (object?) y;
		}
		public int GetHashCode (Type? obj)
		{
			if (obj is null)
				return 0;
			return obj.GetHashCode ();
		}
	}

	internal class StringEqualityComparer : IEqualityComparer<string> {
		public bool Equals (string? x, string? y)
		{
			return string.Equals (x, y, StringComparison.Ordinal);
		}
		public int GetHashCode (string? obj)
		{
			return obj?.GetHashCode () ?? 0;
		}
	}

	internal class RuntimeTypeHandleEqualityComparer : IEqualityComparer<RuntimeTypeHandle> {
		public bool Equals (RuntimeTypeHandle x, RuntimeTypeHandle y)
		{
			return x.Equals (y);
		}
		public int GetHashCode (RuntimeTypeHandle obj)
		{
			return obj.GetHashCode ();
		}
	}

	internal struct IntPtrTypeValueTuple : IEquatable<IntPtrTypeValueTuple> {
		static readonly IEqualityComparer<IntPtr> item1Comparer = Runtime.IntPtrEqualityComparer;
		static readonly IEqualityComparer<Type> item2Comparer = Runtime.TypeEqualityComparer;

		public readonly IntPtr Item1;
		public readonly Type Item2;

		public IntPtrTypeValueTuple (IntPtr item1, Type item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public bool Equals (IntPtrTypeValueTuple other)
		{
			return item1Comparer.Equals (Item1, other.Item1) &&
				item2Comparer.Equals (Item2, other.Item2);
		}

		public override bool Equals (object? obj)
		{
			if (obj is IntPtrTypeValueTuple vt)
				return Equals (vt);

			return false;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (item1Comparer, item2Comparer);
		}

		public static bool operator == (IntPtrTypeValueTuple left, IntPtrTypeValueTuple right)
		{
			return left.Equals (right);
		}

		public static bool operator != (IntPtrTypeValueTuple left, IntPtrTypeValueTuple right)
		{
			return !left.Equals (right);
		}
	}
}
