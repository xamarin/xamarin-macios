//
// Runtime.cs: Mac/iOS shared runtime code
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2013 Xamarin Inc.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Foundation;
using Registrar;

#if MONOMAC
using AppKit;
#endif

#if !COREBUILD && (XAMARIN_APPLETLS || XAMARIN_NO_TLS)
#if !MMP && !MTOUCH && !MTOUCH_TEST
using Mono.Security.Interface;
#endif
#endif

namespace ObjCRuntime {
	
	public partial class Runtime {
#if !COREBUILD
#if XAMCORE_2_0
		internal const bool IsUnifiedBuild = true;
#else
		internal const bool IsUnifiedBuild = false;
#endif

		static Dictionary<IntPtrTypeValueTuple,Delegate> block_to_delegate_cache;

		static List <object> delegates;
		static List <Assembly> assemblies;
		static Dictionary <IntPtr, WeakReference> object_map;
		static object lock_obj;
		static IntPtr NSObjectClass;
		static bool initialized;

		internal static IntPtrEqualityComparer IntPtrEqualityComparer;
		internal static TypeEqualityComparer TypeEqualityComparer;

		internal static DynamicRegistrar Registrar;

		internal unsafe struct MTRegistrationMap {
			public IntPtr assembly;
			public MTClassMap *map;
			public IntPtr full_token_references; /* array of MTFullTokenReference */
			public MTManagedClassMap* skipped_map;
			public int assembly_count;
			public int map_count;
			public int custom_type_count;
			public int full_token_reference_count;
			public int skipped_map_count;
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal struct MTClassMap {
			public IntPtr handle;
			public uint type_reference;
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		internal struct MTManagedClassMap
		{
			public uint skipped_reference; // implied token type: TypeDef
			public uint index; // index into MTRegistrationMap.map
		}

		/* Keep Delegates, Trampolines and InitializationOptions in sync with monotouch-glue.m */
		internal struct Trampolines {
			public IntPtr tramp;
			public IntPtr stret_tramp;
			public IntPtr fpret_single_tramp;
			public IntPtr fpret_double_tramp;
			public IntPtr release_tramp;
			public IntPtr retain_tramp;
			public IntPtr static_tramp;
			public IntPtr ctor_tramp;
			public IntPtr x86_double_abi_stret_tramp;
			public IntPtr static_fpret_single_tramp;
			public IntPtr static_fpret_double_tramp;
			public IntPtr static_stret_tramp;
			public IntPtr x86_double_abi_static_stret_tramp;
			public IntPtr long_tramp;
			public IntPtr static_long_tramp;
#if MONOMAC
			public IntPtr copy_with_zone_1;
			public IntPtr copy_with_zone_2;
#endif
			public IntPtr get_gchandle_tramp;
			public IntPtr set_gchandle_tramp;
		}

		[Flags]
		internal enum InitializationFlags : int {
			/* unused               = 0x01 */
			/* unused				= 0x02,*/
			DynamicRegistrar		= 0x04,
			/* unused				= 0x08,*/
			IsSimulator				= 0x10,
		}

#if MONOMAC
		/* This enum must always match the identical enum in runtime/xamarin/main.h */
		internal enum LaunchMode : int {
			App = 0,
			Extension = 1,
			Embedded = 2,
		}
#endif

		[StructLayout (LayoutKind.Sequential)]
		internal unsafe struct InitializationOptions {
			public int Size;
			public InitializationFlags Flags;
			public Delegates *Delegates;
			public Trampolines *Trampolines;
			public MTRegistrationMap *RegistrationMap;
			public MarshalObjectiveCExceptionMode MarshalObjectiveCExceptionMode;
			public MarshalManagedExceptionMode MarshalManagedExceptionMode;
#if MONOMAC
			public LaunchMode LaunchMode;
			public IntPtr EntryAssemblyPath; /* char * */
#endif
			IntPtr AssemblyLocations;

			public bool IsSimulator {
				get {
					return (Flags & InitializationFlags.IsSimulator) == InitializationFlags.IsSimulator;
				}
			}
		}

		internal static unsafe InitializationOptions* options;

		internal static bool Initialized {
			get { return initialized; }
		}
			
#if MONOMAC
		[DllImport (Constants.libcLibrary)]
		static extern int _NSGetExecutablePath (byte[] buf, ref int bufsize);
#endif

		[Preserve] // called from native - runtime.m.
		unsafe static void Initialize (InitializationOptions* options)
		{
#if PROFILE
			var watch = new Stopwatch ();
#endif
			if (options->Size != Marshal.SizeOf (typeof (InitializationOptions))) {
				string msg = "Version mismatch between the native " + ProductName + " runtime and " + AssemblyName + ". Please reinstall " + ProductName + ".";
				Console.Error.WriteLine (msg);
#if MONOMAC
				try {
					// Print out where Xamarin.Mac.dll and the native runtime was loaded from.
					Console.Error.WriteLine (AssemblyName + " was loaded from {0}", typeof (nint).Assembly.Location);

					var sym2 = Dlfcn.dlsym (Dlfcn.RTLD.Default, "xamarin_initialize");
					Dlfcn.Dl_info info2;
					if (Dlfcn.dladdr (sym2, out info2) == 0) {
						Console.Error.WriteLine ("The native runtime was loaded from {0}", Marshal.PtrToStringAuto (info2.dli_fname));
					} else if (Dlfcn.dlsym (Dlfcn.RTLD.MainOnly, "xamarin_initialize") != IntPtr.Zero) {
						byte[] buf = new byte [128];
						int length = buf.Length;
						if (_NSGetExecutablePath (buf, ref length) == -1) {
							Array.Resize (ref buf, length);
							length = buf.Length;
							if (_NSGetExecutablePath (buf, ref length) != 0) {
								Console.Error.WriteLine ("Could not find out where the native runtime was loaded from.");
								buf = null;
							}
						}
						if (buf != null) {
							var strlength = 0;
							for (int i = 0; i < buf.Length && buf [i] != 0; i++)
								strlength++;
							Console.Error.WriteLine ("The native runtime was loaded from {0}", System.Text.Encoding.UTF8.GetString (buf, 0, strlength));
						}
					} else {
						Console.Error.WriteLine ("Could not find out where the native runtime was loaded from.");
					}
				} catch {
					// Just ignore any exceptions, the above code is just a debug help, and if it fails,
					// any exception show to the user will likely confuse more than help
				}
#endif
				throw ErrorHelper.CreateError (8001, msg);
			}

			if (IntPtr.Size != sizeof (nint)) {
				string msg = string.Format ("Native type size mismatch between " + AssemblyName + " and the executing architecture. " + AssemblyName + " was built for {0}-bit, while the current process is {1}-bit.", 
					IntPtr.Size == 4 ? "64" : "32", IntPtr.Size == 4 ? "32" : "64");
				Console.Error.WriteLine (msg);
				throw ErrorHelper.CreateError (8010, msg);
			}

			IntPtrEqualityComparer = new IntPtrEqualityComparer ();
			TypeEqualityComparer = new TypeEqualityComparer ();

			Runtime.options = options;
			delegates = new List<object> ();
			object_map = new Dictionary <IntPtr, WeakReference> (IntPtrEqualityComparer);
			lock_obj = new object ();

			NSObjectClass = NSObject.Initialize ();

			Registrar = new DynamicRegistrar ();
			RegisterDelegates (options);
			Class.Initialize (options);
			InitializePlatform (options);

#if !XAMMAC_SYSTEM_MONO
			UseAutoreleasePoolInThreadPool = true;
#endif

			objc_exception_mode = options->MarshalObjectiveCExceptionMode;
			managed_exception_mode = options->MarshalManagedExceptionMode;

			initialized = true;
#if PROFILE
			Console.WriteLine ("Runtime.Initialize completed in {0} ms", watch.ElapsedMilliseconds);
#endif
		}

#if !XAMMAC_SYSTEM_MONO
		static bool has_autoreleasepool_in_thread_pool;
		public static bool UseAutoreleasePoolInThreadPool {
			get {
				return has_autoreleasepool_in_thread_pool;
			}
			set {
				System.Threading._ThreadPoolWaitCallback.SetDispatcher (value ? new Func<Func<bool>, bool> (ThreadPoolDispatcher) : null);
				has_autoreleasepool_in_thread_pool = value;
			}
		}

		static bool ThreadPoolDispatcher (Func<bool> callback)
		{
			using (var pool = new NSAutoreleasePool ())
				return callback ();
		}
#endif

#if MONOMAC
		public static event AssemblyRegistrationHandler AssemblyRegistration;

		static bool OnAssemblyRegistration (AssemblyName assembly_name)
		{
			if (AssemblyRegistration != null) {
				var args = new AssemblyRegistrationEventArgs
				{
					Register = true,
					AssemblyName = assembly_name
				};
				AssemblyRegistration (null, args);
				return args.Register;
			}
			return true;
		}
#endif
		static MarshalObjectiveCExceptionMode objc_exception_mode;
		static MarshalManagedExceptionMode managed_exception_mode;

		public static event MarshalObjectiveCExceptionHandler MarshalObjectiveCException;
		public static event MarshalManagedExceptionHandler MarshalManagedException;

		static MarshalObjectiveCExceptionMode OnMarshalObjectiveCException (IntPtr exception_handle, bool throwManagedAsDefault)
		{
			if (throwManagedAsDefault && MarshalObjectiveCException == null)
				return MarshalObjectiveCExceptionMode.ThrowManagedException;
			
			if (MarshalObjectiveCException != null) {
				var exception = GetNSObject<NSException> (exception_handle);
				var args = new MarshalObjectiveCExceptionEventArgs ()
				{
					Exception = exception,
					ExceptionMode = throwManagedAsDefault ? MarshalObjectiveCExceptionMode.ThrowManagedException : objc_exception_mode,
				};

				MarshalObjectiveCException (null, args);
				return args.ExceptionMode;
			}
			return objc_exception_mode;
		}

		static MarshalManagedExceptionMode OnMarshalManagedException (int exception_handle)
		{
			if (MarshalManagedException != null) {
				var exception = GCHandle.FromIntPtr (new IntPtr (exception_handle)).Target as Exception;
				var args = new MarshalManagedExceptionEventArgs ()
				{
					Exception = exception,
					ExceptionMode = managed_exception_mode,
				};
				MarshalManagedException (null, args);
				return args.ExceptionMode;
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
			var value = GCHandle.FromIntPtr (value_handle).Target;
			var smart_type = value.GetType ();
			MethodBase getConstantMethod, getValueMethod;
			if (!Registrar.IsSmartEnum (smart_type, out getConstantMethod, out getValueMethod))
				throw ErrorHelper.CreateError (8024, $"Could not find a valid extension type for the smart enum '{smart_type.FullName}'. Please file a bug at https://bugzilla.xamarin.com.");
			var rv = (NSString) ((MethodInfo) getConstantMethod).Invoke (null, new object [] { value });
			if (rv == null)
				return IntPtr.Zero;
			rv.DangerousRetain ().DangerousAutorelease ();
			return rv.Handle;
		}


		// value: native NSString *
		// returns: GCHandle to a (smart) enum value. Caller must free the GCHandle.
		static IntPtr ConvertNSStringToSmartEnum (IntPtr value, IntPtr type)
		{
			var smart_type = (Type) ObjectWrapper.Convert (type);
			var str = GetNSObject<NSString> (value);
			MethodBase getConstantMethod, getValueMethod;
			if (!Registrar.IsSmartEnum (smart_type, out getConstantMethod, out getValueMethod))
				throw ErrorHelper.CreateError (8024, $"Could not find a valid extension type for the smart enum '{smart_type.FullName}'. Please file a bug at https://bugzilla.xamarin.com.");
			var rv = ((MethodInfo) getValueMethod).Invoke (null, new object [] { str });
			return GCHandle.ToIntPtr (GCHandle.Alloc (rv));
		}

#region Wrappers for delegate callbacks
		static void RegisterNSObject (IntPtr managed_obj, IntPtr native_obj)
		{
			RegisterNSObject ((NSObject) ObjectWrapper.Convert (managed_obj), native_obj);
		}

		static void RegisterAssembly (IntPtr a)
		{
			RegisterAssembly ((Assembly) ObjectWrapper.Convert (a));
		}

		static void RegisterEntryAssembly (IntPtr a)
		{
			RegisterEntryAssembly ((Assembly) ObjectWrapper.Convert (a));
		}

		static void ThrowNSException (IntPtr ns_exception)
		{
#if MONOMAC
			throw new ObjCException (new NSException (ns_exception));
#else
			throw new MonoTouchException (new NSException (ns_exception));
#endif
		}

		static void RethrowManagedException (uint exception_gchandle)
		{
			var e = (Exception) GCHandle.FromIntPtr ((IntPtr) exception_gchandle).Target;
			System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture (e).Throw ();
		}

		static int CreateNSException (IntPtr ns_exception)
		{
			Exception ex;
#if MONOMAC
			ex = new ObjCException (Runtime.GetNSObject<NSException> (ns_exception));
#else
			ex = new MonoTouchException (Runtime.GetNSObject<NSException> (ns_exception));
#endif
			return GCHandle.ToIntPtr (GCHandle.Alloc (ex)).ToInt32 ();
		}

		static IntPtr UnwrapNSException (int exc_handle)
		{
			var obj = GCHandle.FromIntPtr (new IntPtr (exc_handle)).Target;
#if MONOMAC
			var exc = obj as ObjCException;
#else
			var exc = obj as MonoTouchException;
#endif
			if (exc != null) {
				return exc.NSException.DangerousRetain ().DangerousAutorelease ().Handle;
			} else {
				return IntPtr.Zero;
			}
		}

		static IntPtr GetBlockWrapperCreator (IntPtr method, int parameter)
		{
			return ObjectWrapper.Convert (GetBlockWrapperCreator ((MethodInfo) ObjectWrapper.Convert (method), parameter));
		}

		static IntPtr CreateBlockProxy (IntPtr method, IntPtr block)
		{
			return ObjectWrapper.Convert (CreateBlockProxy ((MethodInfo) ObjectWrapper.Convert (method), block));
		}
			
		static IntPtr CreateDelegateProxy (IntPtr method, IntPtr @delegate, IntPtr signature)
		{
			return BlockLiteral.GetBlockForDelegate ((MethodInfo) ObjectWrapper.Convert (method), ObjectWrapper.Convert (@delegate), Marshal.PtrToStringAuto (signature));
		}

		static unsafe Assembly GetEntryAssembly ()
		{
			var asm = Assembly.GetEntryAssembly ();
#if MONOMAC
			if (asm == null)
				asm = Assembly.LoadFile (Marshal.PtrToStringAuto (options->EntryAssemblyPath));
#endif
			return asm;
		}

		// This method will register all assemblies referenced by the entry assembly.
		// For XM it will also register all assemblies loaded in the current appdomain.
		internal static void RegisterAssemblies ()
		{
#if PROFILE
			var watch = new Stopwatch ();
#endif

			RegisterEntryAssembly (GetEntryAssembly ());

#if PROFILE
			Console.WriteLine ("RegisterAssemblies completed in {0} ms", watch.ElapsedMilliseconds);
#endif
		}

		// This method will register all assemblies referenced by the entry assembly.
		// For XM it will also register all assemblies loaded in the current appdomain.
		//
		// NOTE: the (XI) linker remove this method for device builds (RemoveCode.cs) and as such cannot be renamed
		// without updating mtouch
		internal static void RegisterEntryAssembly (Assembly entry_assembly)
		{
			var assemblies = new List<Assembly> ();

			assemblies.Add (NSObject.PlatformAssembly); // make sure our platform assembly comes first
			// Recursively get all assemblies referenced by the entry assembly.
			if (entry_assembly != null) {
				var register_entry_assembly = true;
#if MONOMAC
				register_entry_assembly = OnAssemblyRegistration (entry_assembly.GetName ());
#endif
				if (register_entry_assembly)
					CollectReferencedAssemblies (assemblies, entry_assembly);
			} else {
				Console.WriteLine ("Could not find the entry assembly.");
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

		static void CollectReferencedAssemblies (List<Assembly> assemblies, Assembly assembly)
		{
			assemblies.Add (assembly);
			foreach (var rf in assembly.GetReferencedAssemblies ()) {
#if MONOMAc
				if (!OnAssemblyRegistration (rf))
					continue;
#endif
				try {
					var a = Assembly.Load (rf);
					if (!assemblies.Contains (a))
						CollectReferencedAssemblies (assemblies, a);
				}
				catch (FileNotFoundException fefe) {
					// that's more important for XI because device builds don't go thru this step
					// and we can end up with simulator-only failures - bug #29211
					NSLog ("Could not find `{0}` referenced by assembly `{1}`.", fefe.FileName, assembly.FullName);
#if MONOMAC
					if (!NSApplication.IgnoreMissingAssembliesDuringRegistration)
						throw;
#endif
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

		public static void RegisterAssembly (Assembly a)
		{
			if (a == null)
				throw new ArgumentNullException ("a");

#if MONOMAC
			var attributes = a.GetCustomAttributes (typeof (RequiredFrameworkAttribute), false);

			foreach (var attribute in attributes) {
				var requiredFramework = (RequiredFrameworkAttribute)attribute;
				string libPath;
				string libName = requiredFramework.Name;

				if (libName.Contains (".dylib")) {
					libPath = ResourcesPath;
				}
				else {
					libPath = FrameworksPath;
					libPath = Path.Combine (libPath, libName);
					libName = libName.Replace (".frameworks", "");
				}
				libPath = Path.Combine (libPath, libName);

				if (Dlfcn.dlopen (libPath, 0) == IntPtr.Zero)
					throw new Exception (string.Format ("Unable to load required framework: '{0}'", requiredFramework.Name),
						new Exception (Dlfcn.dlerror()));
			}

			attributes = a.GetCustomAttributes (typeof (DelayedRegistrationAttribute), false);
			foreach (var attribute in attributes) {
				var delayedRegistration = (DelayedRegistrationAttribute) attribute;
				if (delayedRegistration.Delay)
					return;
			}
#endif

			if (assemblies == null) {
				assemblies = new List <Assembly> ();
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
			return ObjectWrapper.Convert (new Class (klass));
		}

		static IntPtr GetSelector (IntPtr sel)
		{
			return ObjectWrapper.Convert (new Selector (sel));
		}

		static void GetMethodForSelector (IntPtr cls, IntPtr sel, bool is_static, IntPtr desc)
		{
			// This is called by the old registrar code.
			Registrar.GetMethodDescription (Class.Lookup (cls), sel, is_static, desc);
		}

		static IntPtr GetNSObjectWrapped (IntPtr ptr)
		{
			return ObjectWrapper.Convert (TryGetNSObject (ptr, true));
		}

		static bool HasNSObject (IntPtr ptr)
		{
			return TryGetNSObject (ptr) != null;
		}

		static IntPtr GetHandleForINativeObject (IntPtr ptr)
		{
			return ((INativeObject) ObjectWrapper.Convert (ptr)).Handle;
		}

		static void UnregisterNSObject (IntPtr native_obj, IntPtr managed_obj) 
		{
			NativeObjectHasDied (native_obj, ObjectWrapper.Convert (managed_obj) as NSObject);
		}

		static unsafe IntPtr GetMethodFromToken (uint token_ref)
		{
			var method = Class.ResolveTokenReference (token_ref, 0x06000000);

			var mb = method as MethodBase;
			if (method != null && mb == null)
				throw ErrorHelper.CreateError (8022, $"Expected the token reference 0x{token_ref:X} to be a method, but it's a {method.GetType ().Name}. Please file a bug report at http://bugzilla.xamarin.com.");
			
			if (method != null)
				return ObjectWrapper.Convert (method);

			return IntPtr.Zero;
		}

		static unsafe IntPtr GetGenericMethodFromToken (IntPtr obj, uint token_ref)
		{
			var method = Class.ResolveTokenReference (token_ref, 0x06000000);
			if (method == null)
				return IntPtr.Zero;

			var mb = method as MethodBase;
			if (mb == null)
				throw ErrorHelper.CreateError (8022, $"Expected the token reference 0x{token_ref:X} to be a method, but it's a {method.GetType ().Name}. Please file a bug report at http://bugzilla.xamarin.com.");

			var nsobj = ObjectWrapper.Convert (obj) as NSObject;
			if (nsobj == null)
				throw ErrorHelper.CreateError (8023, $"An instance object is required to construct a closed generic method for the open generic method: {mb.DeclaringType.FullName}.{mb.Name} (token reference: 0x{token_ref:X}). Please file a bug report at http://bugzilla.xamarin.com.");

			return ObjectWrapper.Convert (DynamicRegistrar.FindClosedMethod (nsobj.GetType (), mb));
		}

		static IntPtr TryGetOrConstructNSObjectWrapped (IntPtr ptr)
		{
			return ObjectWrapper.Convert (GetNSObject (ptr, MissingCtorResolution.Ignore, true));
		}

		static IntPtr GetINativeObject_Dynamic (IntPtr ptr, bool owns, IntPtr type_ptr)
		{
			/*
			 * This method is called from marshalling bridge (dynamic mode).
			 */
			// It doesn't work to use System.Type in the signature, we get garbage.
			var type = (System.Type) ObjectWrapper.Convert (type_ptr);
			return ObjectWrapper.Convert (GetINativeObject (ptr, owns, type));
		}
			
		static IntPtr GetINativeObject_Static (IntPtr ptr, bool owns, string typename, string ifacename)
		{
			/* 
			 * This method is called from generated code from the static registrar.
			 */

			var iface = Type.GetType (ifacename, true);
			var type = Type.GetType (typename, true);
			return ObjectWrapper.Convert (GetINativeObject (ptr, owns, iface, type));
		}

		static IntPtr GetNSObjectWithType (IntPtr ptr, IntPtr type_ptr, out bool created)
		{
			// It doesn't work to use System.Type in the signature, we get garbage.
			var type = (System.Type) ObjectWrapper.Convert (type_ptr);
			return ObjectWrapper.Convert (GetNSObject (ptr, type, MissingCtorResolution.ThrowConstructor1NotFound, true, out created));
		}

		static void Dispose (IntPtr mobj)
		{
			((IDisposable) ObjectWrapper.Convert (mobj)).Dispose ();
		}

		static bool IsParameterTransient (IntPtr info, int parameter)
		{
			var minfo = ObjectWrapper.Convert (info) as MethodInfo;
			if (minfo == null)
				return false; // might be a ConstructorInfo (bug #15583), but we don't care about that (yet at least).
			minfo = minfo.GetBaseDefinition ();
			var parameters = minfo.GetParameters ();
			if (parameters == null || parameters.Length <= parameter)
				return false;
			return parameters [parameter].IsDefined (typeof(TransientAttribute), false);
		}

		static bool IsParameterOut (IntPtr info, int parameter)
		{
			var minfo = ObjectWrapper.Convert (info) as MethodInfo;
			if (minfo == null)
				return false; // might be a ConstructorInfo (bug #15583), but we don't care about that (yet at least).
			minfo = minfo.GetBaseDefinition ();
			var parameters = minfo.GetParameters ();
			if (parameters == null || parameters.Length <= parameter)
				return false;
			return parameters [parameter].IsOut;
		}

		static void GetMethodAndObjectForSelector (IntPtr klass, IntPtr sel, bool is_static, IntPtr obj, ref IntPtr mthis, IntPtr desc)
		{
			Registrar.GetMethodDescriptionAndObject (Class.Lookup (klass), sel, is_static, obj, ref mthis, desc);
		}

		static int CreateProductException (int code, string msg)
		{
			var ex = ErrorHelper.CreateError (code, msg);
			return GCHandle.ToIntPtr (GCHandle.Alloc (ex, GCHandleType.Normal)).ToInt32 ();
		}

		static IntPtr TypeGetFullName (IntPtr type) 
		{	
			return Marshal.StringToHGlobalAuto (((Type) ObjectWrapper.Convert (type)).FullName);
		}

		static IntPtr LookupManagedTypeName (IntPtr klass)
		{
			return Marshal.StringToHGlobalAuto (Class.LookupFullName (klass));
		}
#endregion

		static MethodInfo GetBlockProxyAttributeMethod (MethodInfo method, int parameter)
		{
			var attrs = method.GetParameters () [parameter].GetCustomAttributes (typeof (BlockProxyAttribute), true);
			if (attrs.Length == 1) {
				try {
					var attr = attrs [0] as BlockProxyAttribute;
					return attr.Type.GetMethod ("Create");
				} catch {
					return null;
				}
			}
			return null;
		}

		//
		// Returns a MethodInfo that represents the method that can be used to turn
		// a the block in the given method at the given parameter into a strongly typed
		// delegate
		//
		[EditorBrowsable (EditorBrowsableState.Never)]
#if XAMCORE_2_0
		internal
#else
		public 
#endif
		static MethodInfo GetBlockWrapperCreator (MethodInfo method, int parameter)
		{
			MethodInfo first = method;
			MethodInfo last = null;
			Type[] extensionParameters = null;

			while (method != last){
				last = method;
				var createMethod = GetBlockProxyAttributeMethod (method, parameter);
				if (createMethod != null)
					return createMethod;
				method = method.GetBaseDefinition ();
			}

			// Might be the implementation of an interface method, so find the corresponding
			// MethodInfo for the interface, and check for BlockProxy attributes there as well.
			foreach (var iface in method.DeclaringType.GetInterfaces ()) {
				if (!iface.IsDefined (typeof (ProtocolAttribute), false))
					continue;

				var map = method.DeclaringType.GetInterfaceMap (iface);
				for (int i = 0; i < map.TargetMethods.Length; i++) {
					if (map.TargetMethods [i] == first) {
						var createMethod = GetBlockProxyAttributeMethod (map.InterfaceMethods [i], parameter);
						if (createMethod != null)
							return createMethod;
					}
				}

				// Might be an implementation of an optional protocol member.
				// We look that up on the corresponding extension method.
				string extensionName = string.Empty;
				if (!string.IsNullOrEmpty (iface.Namespace))
					extensionName = iface.Namespace + ".";
				extensionName +=iface.Name.Substring (1) + "_Extensions";
				var extensionType = iface.Assembly.GetType (extensionName, false);
				if (extensionType != null) {
					if (extensionParameters == null) {
						var methodParameters = method.GetParameters ();
						extensionParameters = new Type [methodParameters.Length + 1];
						for (int i = 0; i < methodParameters.Length; i++)
							extensionParameters [i + 1] = methodParameters [i].ParameterType;
					}
					extensionParameters [0] = iface;
					var extensionMethod = extensionType.GetMethod (method.Name, BindingFlags.Public | BindingFlags.Static, null, extensionParameters, null);
					if (extensionMethod != null) {
						var createMethod = GetBlockProxyAttributeMethod (extensionMethod, parameter + 1);
						if (createMethod != null)
							return createMethod;
					}
				}
			}

			throw new RuntimeException (8009, true, "Unable to locate the block to delegate conversion method for the method {0}.{1}'s parameter #{2}. Please file a bug at http://bugzilla.xamarin.com.",
				method.DeclaringType.FullName, method.Name, parameter + 1);
		}

		//
		// Called from the runtime, since it is too hard to use the unmanaged API
		// Given a MethodInfo, invoke it, passing the given block
		//
		// Used to call the Create(IntPtr) method on the proxy classes that turn
		// objective c blocks into strongly typed delegates.
		//
		[EditorBrowsable (EditorBrowsableState.Never)]
#if XAMCORE_2_0
		internal
#else
		public 
#endif
		static Delegate CreateBlockProxy (MethodInfo method, IntPtr block)
		{
			NSObject.DangerousRetain (block);
			return (Delegate) method.Invoke (null, new object [] { block } );
		}

#if XAMCORE_2_0
		internal
#else
		public
#endif
		static Delegate GetDelegateForBlock (IntPtr methodPtr, Type type)
		{
			if (block_to_delegate_cache == null)
				block_to_delegate_cache = new Dictionary<IntPtrTypeValueTuple, Delegate> ();

			// We do not care if there is a race condition and we initialize two caches
			// since the worst that can happen is that we end up with an extra
			// delegate->function pointer.
			Delegate val;
			var pair = new IntPtrTypeValueTuple (methodPtr, type);
			lock (block_to_delegate_cache) {
				if (block_to_delegate_cache.TryGetValue (pair, out val))
					return val;
			}

			val = Marshal.GetDelegateForFunctionPointer (methodPtr, type);

			lock (block_to_delegate_cache){
				block_to_delegate_cache [pair] = val;
			}
			return val;
		}

		unsafe static MethodBase FindMethod (IntPtr typeptr, IntPtr methodptr, int paramCount, IntPtr* paramptr)
		{
			var type = Type.GetType (Marshal.PtrToStringAuto (typeptr));
			var methodName = Marshal.PtrToStringAuto (methodptr);
			var parameterTypes = new string [paramCount];
			for (int i = 0; i < paramCount; i++)
				parameterTypes [i] = Marshal.PtrToStringAuto (paramptr [i]);

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

			throw ErrorHelper.CreateError (8002, "Could not find the method '{0}' in the type '{1}'.", methodName, type.FullName);
		}

		internal static void UnregisterNSObject (IntPtr ptr) {
			lock (lock_obj) {
				object_map.Remove (ptr);
			}
		}
					
		static void NativeObjectHasDied (IntPtr ptr, NSObject managed_obj)
		{
			lock (lock_obj) {
				WeakReference wr;
				if (object_map.TryGetValue (ptr, out wr)) {
					if (managed_obj == null || wr.Target == (object) managed_obj)
						object_map.Remove (ptr);

				}

				if (managed_obj != null)
					managed_obj.ClearHandle ();
			}
		}
		
		internal static void RegisterNSObject (NSObject obj, IntPtr ptr) {
			lock (lock_obj) {
				object_map [ptr] = new WeakReference (obj, true);
				obj.Handle = ptr;
			}
		}


		static NSObject IgnoreConstructionError (IntPtr ptr, IntPtr klass, Type type)
		{
			return null;
		}

		internal enum MissingCtorResolution {
			ThrowConstructor1NotFound,
			ThrowConstructor2NotFound,
			Ignore,
		}

		static void MissingCtor (IntPtr ptr, IntPtr klass, Type type, MissingCtorResolution resolution)
		{
			string msg;

			if (klass == IntPtr.Zero)
				klass = Class.GetClassForObject (ptr);

			switch (resolution) {
			case MissingCtorResolution.ThrowConstructor1NotFound:
				msg = "Failed to marshal the Objective-C object 0x{0} (type: {1}). " +
					"Could not find an existing managed instance for this object, " +
					"nor was it possible to create a new managed instance " +
					"(because the type '{2}' does not have a constructor that takes one IntPtr argument).";
				break;
			case MissingCtorResolution.ThrowConstructor2NotFound:
				msg = "Failed to marshal the Objective-C object 0x{0} (type: {1}). " +
					"Could not find an existing managed instance for this object, " +
					"nor was it possible to create a new managed instance " +
					"(because the type '{2}' does not have a constructor that takes two (IntPtr, bool) arguments).";
				break;
			case MissingCtorResolution.Ignore:
			default:
				return;
			}

			throw new Exception (string.Format (msg, ptr.ToString ("x"), new Class (klass).Name, type.FullName));
		}

		static NSObject ConstructNSObject (IntPtr ptr, IntPtr klass, MissingCtorResolution missingCtorResolution)
		{
			Type type = Class.Lookup (klass);

			if (type != null) {
				return ConstructNSObject<NSObject> (ptr, type, missingCtorResolution);
			} else {
				return new NSObject (ptr);
			}
		}

		internal static T ConstructNSObject<T> (IntPtr ptr) where T: NSObject
		{
			return ConstructNSObject<T> (ptr, typeof (T), MissingCtorResolution.ThrowConstructor1NotFound);
		}

		// The generic argument T is only used to cast the return value.
		static T ConstructNSObject<T> (IntPtr ptr, Type type, MissingCtorResolution missingCtorResolution) where T: class, INativeObject
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			var ctor = GetIntPtrConstructor (type);

			if (ctor == null) {
				MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution);
				return null;
			}

			return (T) ctor.Invoke (new object[] { ptr });
		}

		// The generic argument T is only used to cast the return value.
		static T ConstructINativeObject<T> (IntPtr ptr, bool owns, Type type, MissingCtorResolution missingCtorResolution) where T : class, INativeObject
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			if (type.IsByRef)
				type = type.GetElementType ();

			var ctor = GetIntPtr_BoolConstructor (type);

			if (ctor == null)
				MissingCtor (ptr, IntPtr.Zero, type, missingCtorResolution);

			return (T) ctor.Invoke (new object[] { ptr, owns});
		}

		static ConstructorInfo GetIntPtrConstructor (Type type)
		{
			var ctors = type.GetConstructors (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			for (int i = 0; i < ctors.Length; ++i) {
				var param = ctors[i].GetParameters ();
				if (param.Length == 1 && param [0].ParameterType == typeof (IntPtr))
					return ctors [i];
			}
			return null;
		}

		static ConstructorInfo GetIntPtr_BoolConstructor (Type type)
		{
			var ctors = type.GetConstructors (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			for (int i = 0; i < ctors.Length; ++i) {
				var param = ctors[i].GetParameters ();
				if (param.Length == 2 && param [0].ParameterType == typeof (IntPtr) && param [1].ParameterType == typeof (bool))
					return ctors [i];
			}
			return null;
		}

		public static NSObject TryGetNSObject (IntPtr ptr)
		{
			return TryGetNSObject (ptr, false);
		}

		internal static NSObject TryGetNSObject (IntPtr ptr, bool evenInFinalizerQueue = false)
		{
			lock (lock_obj) {
				WeakReference reference;
				if (object_map.TryGetValue (ptr, out reference)) {
					var target = (NSObject) reference.Target;
					if (target == null)
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

		public static NSObject GetNSObject (IntPtr ptr) {
			return GetNSObject (ptr, MissingCtorResolution.ThrowConstructor1NotFound);
		}

		internal static NSObject GetNSObject (IntPtr ptr, MissingCtorResolution missingCtorResolution, bool evenInFinalizerQueue = false) {
			if (ptr == IntPtr.Zero)
				return null;

			var o = TryGetNSObject (ptr, evenInFinalizerQueue);

			if (o != null)
				return o;

			return ConstructNSObject (ptr, Class.GetClassForObject (ptr), missingCtorResolution);
		}

		static public T GetNSObject<T> (IntPtr ptr) where T : NSObject
		{
			if (ptr == IntPtr.Zero)
				return null;

			object obj = TryGetNSObject (ptr);
			T o;

			if (obj == null) {
				// Try to get the managed type that correspond to this exact native type
				IntPtr p = Class.GetClassForObject (ptr);
				// If unknown then we'll get the Class that Lookup to NSObject even if this is not NSObject.
				// We can use this condition to fallback on the provided (generic argument) type
				Type target_type;
				if (p != NSObjectClass) {
					target_type = Class.Lookup (p);
					if (target_type == typeof(NSObject))
						target_type = typeof(T);
					else if (typeof (T).IsGenericType)
						target_type = typeof(T);
					else if (target_type.IsSubclassOf (typeof(T))) {
						// do nothing, this is fine.
					} else if (Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("isKindOfClass:"), Class.GetHandle (typeof (T)))) {
						// If the instance itself claims it's an instance of the provided (generic argument) type,
						// then we believe the instance. See bug #20692 for a test case.
						target_type = typeof(T);
					}
				} else {
					target_type = typeof(NSObject);
				}
				o = ConstructNSObject<T> (ptr, target_type, MissingCtorResolution.ThrowConstructor1NotFound);
			} else {
				o = obj as T;
				if (o == null)
					throw new InvalidCastException (string.Format ("Unable to cast object of type '{0}' to type '{1}'", obj.GetType ().FullName, typeof(T).FullName));
			}

			return o;
		}

		static public T GetNSObject<T> (IntPtr ptr, bool owns) where T : NSObject
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

		static NSObject GetNSObject (IntPtr ptr, Type target_type, MissingCtorResolution missingCtorResolution, bool evenInFinalizerQueue, out bool created) {
			created = false;

			if (ptr == IntPtr.Zero)
				return null;

			var o = TryGetNSObject (ptr, evenInFinalizerQueue);

			if (o != null)
				return o;

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
				} else if (Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("isKindOfClass:"), Class.GetHandle (target_type))) {
					// nothing to do
				} else {
					target_type = dynamic_type;
				}
			} else {
				target_type = typeof(NSObject);
			}

			created = true;
			return ConstructNSObject<NSObject> (ptr, target_type, MissingCtorResolution.ThrowConstructor1NotFound);
		}

		static Type LookupINativeObjectImplementation (IntPtr ptr, Type target_type, Type implementation = null)
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
					if (implementation == null)
						implementation = target_type;
				} else {
					var runtime_type = Class.Lookup (p);
					// Check if the runtime type can actually be used.
					if (target_type.IsAssignableFrom (runtime_type)) {
						implementation = runtime_type;
					} else if (implementation == null) {
						implementation = target_type;
					}
				}
			}

			if (implementation.IsInterface)
				implementation = FindProtocolWrapperType (implementation);

			return implementation;
		}

		// this method is identical in behavior to the generic one.
		// TODO: figure out if it's possible to call the generic 
		// implementation from native code.
		static INativeObject GetINativeObject (IntPtr ptr, bool owns, Type target_type, Type implementation = null)
		{
			if (ptr == IntPtr.Zero)
				return null;

			NSObject o = TryGetNSObject (ptr);
			if (o != null && target_type.IsAssignableFrom (o.GetType ())) {
				// found an existing object with the right type.
				return o;
			}

			if (o != null) {
				// found an existing object, but with an incompatible type.
				if (!target_type.IsInterface) {
					// if the target type is another class, there's nothing we can do.
					throw new InvalidCastException (string.Format ("Unable to cast object of type '{0}' to type '{1}'.", o.GetType ().FullName, target_type.FullName));
				}
			}

			// Lookup the ObjC type of the ptr and see if we can use it.
			implementation = LookupINativeObjectImplementation (ptr, target_type, implementation);

			if (implementation.IsSubclassOf (typeof (NSObject))) {
				if (o != null) {
					// We already have an instance of an NSObject-subclass for this ptr.
					// Creating another will break the one-to-one assumption we have between
					// native objects and NSObject instances.
					throw ErrorHelper.CreateError (8004, "Cannot create an instance of {0} for the native object 0x{1} (of type '{2}'), " +
						"because another instance already exists for this native object (of type {3}).",
						implementation.FullName, ptr.ToString ("x"), Class.class_getName (Class.GetClassForObject (ptr)), o.GetType ().FullName);
				}
				return ConstructNSObject<INativeObject> (ptr, implementation, MissingCtorResolution.ThrowConstructor1NotFound);
			}

			return ConstructINativeObject<INativeObject> (ptr, owns, implementation, MissingCtorResolution.ThrowConstructor2NotFound);
		}

		// this method is identical in behavior to the non-generic one.
		public static T GetINativeObject<T> (IntPtr ptr, bool owns) where T : class, INativeObject
		{
			if (ptr == IntPtr.Zero)
				return null;

			var o = TryGetNSObject (ptr);
			var t = o as T;
			if (t != null) {
				// found an existing object with the right type.
				return t;
			}

			if (o != null) {
				// found an existing object, but with an incompatible type.
				if (!typeof (T).IsInterface && typeof(NSObject).IsAssignableFrom (typeof (T))) {
					// if the target type is another NSObject subclass, there's nothing we can do.
					throw new InvalidCastException (string.Format ("Unable to cast object of type '{0}' to type '{1}'.", o.GetType ().FullName, typeof (T).FullName));
				}
			}

			// Lookup the ObjC type of the ptr and see if we can use it.
			var implementation = LookupINativeObjectImplementation (ptr, typeof (T));

			if (implementation.IsSubclassOf (typeof (NSObject))) {
				if (o != null) {
					// We already have an instance of an NSObject-subclass for this ptr.
					// Creating another will break the one-to-one assumption we have between
					// native objects and NSObject instances.
					throw ErrorHelper.CreateError (8004, "Cannot create an instance of {0} for the native object 0x{1} (of type '{2}'), " +
						"because another instance already exists for this native object (of type {3}).",
						implementation.FullName, ptr.ToString ("x"), Class.class_getName (Class.GetClassForObject (ptr)), o.GetType ().FullName);
				}
				return (T) ConstructNSObject<T> (ptr, implementation, MissingCtorResolution.ThrowConstructor1NotFound);
			}

			return ConstructINativeObject<T> (ptr, owns, implementation, MissingCtorResolution.ThrowConstructor2NotFound);
		}

		private static Type FindProtocolWrapperType (Type type)
		{
			if (type == null || !type.IsInterface)
				return null;

			// need to look up the type from the ProtocolAttribute.
			var a = type.GetCustomAttributes (typeof (Foundation.ProtocolAttribute), false);

			var attr = (Foundation.ProtocolAttribute) (a.Length > 0 ? a [0] : null);
			if (attr == null || attr.WrapperType == null)
				throw ErrorHelper.CreateError (4125, "The registrar found an invalid interface '{0}': " +
					"The interface must have a Protocol attribute specifying its wrapper type.",
					type.FullName);
			return attr.WrapperType;
		}

		public static IntPtr GetProtocol (string protocol)
		{
			return Protocol.objc_getProtocol (protocol);
		}

		public static void ConnectMethod (Type type, MethodInfo method, Selector selector)
		{
			if (selector == null)
				throw new ArgumentNullException ("selector");

			ConnectMethod (type, method, new ExportAttribute (selector.Name));
		}
			
		public static void ConnectMethod (Type type, MethodInfo method, ExportAttribute export)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			if (method == null)
				throw new ArgumentNullException ("method");

			if (export == null)
				throw new ArgumentNullException ("export");

			Registrar.RegisterMethod (type, method, export);
		}

		public static void ConnectMethod (MethodInfo method, Selector selector)
		{
			if (method == null)
				throw new ArgumentNullException ("method");

			ConnectMethod (method.DeclaringType, method, selector);
		}

#if MONOMAC
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSLog")]
		extern static void NSLog_impl (IntPtr format, [MarshalAs (UnmanagedType.LPStr)] string s);
		static void NSLog (IntPtr format, string s)
		{
			if (PlatformHelper.CheckSystemVersion (10, 12)) {
				Console.WriteLine (s);
			} else {
				NSLog_impl (format, s);
			}
		}
#else
		[DllImport (Constants.FoundationLibrary)]
		extern static void NSLog (IntPtr format, [MarshalAs (UnmanagedType.LPStr)] string s);
#endif

#if !MONOMAC && !WATCHOS
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSLog")]
		extern static void NSLog_arm64 (IntPtr format, IntPtr p2, IntPtr p3, IntPtr p4, IntPtr p5, IntPtr p6, IntPtr p7, IntPtr p8, [MarshalAs (UnmanagedType.LPStr)] string s);
#endif

		internal static void NSLog (string format, params object[] args)
		{
			var fmt = NSString.CreateNative ("%s");
			var val = (args == null || args.Length == 0) ? format : string.Format (format, args);
#if !MONOMAC && !WATCHOS
			if (IntPtr.Size == 8 && Arch == Arch.DEVICE)
				NSLog_arm64 (fmt, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, val);
			else
#endif
				NSLog (fmt, val);
			NSString.ReleaseNative (fmt);
		}
#endif // !COREBUILD

		static int MajorVersion = -1;
		static int MinorVersion = -1;

		internal static bool CheckSystemVersion (int major, int minor, string systemVersion)
		{
			if (MajorVersion == -1) {
				string[] version = systemVersion.Split (new char[] { '.' });
				
				if (version.Length < 1 || !int.TryParse (version[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out MajorVersion))
					MajorVersion = 2;
				
				if (version.Length < 2 || !int.TryParse (version[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out MinorVersion))
					MinorVersion = 0;
			}
			
			return MajorVersion > major || (MajorVersion == major && MinorVersion >= minor);
		}

		internal static IntPtr CloneMemory (IntPtr source, nint length)
		{
			var rv = Marshal.AllocHGlobal (new IntPtr (length));
			memcpy (rv, source, length);
			return rv;
		}

		[DllImport (Constants.libSystemLibrary)]
		extern internal static void memcpy (IntPtr target, IntPtr source, nint n);

		[DllImport (Constants.libSystemLibrary)]
		unsafe extern internal static void memcpy (byte * target, byte * source, nint n);

		// This function will try to compare a native UTF8 string to a managed string without creating a temporary managed string for the native UTF8 string.
		// Currently this only works if the UTF8 string only contains single-byte characters.
		// If any multi-byte characters are found, the native utf8 string is converted to a managed string, and then normal managed comparison is done.
		internal static bool StringEquals (IntPtr utf8, string str)
		{
			// The vast majority of strings we compare fall within the single-byte UTF8 range, so optimize for this
			unsafe {
				byte* c = (byte*) utf8;
				for (int i = 0; i < str.Length; i++) {
					byte b = c [i];
					if (b > 0x7F) {
						// This string is a multibyte UTF8 string, so go the slow route and convert it to a managed string before comparison
						return string.Equals (Marshal.PtrToStringUTF8 (utf8), str);
					}
					if (b != (short) str [i])
						return false;
				}
				return c [str.Length] == 0;
			}
		}

	}
		
	internal class IntPtrEqualityComparer : IEqualityComparer<IntPtr>
	{
		public bool Equals (IntPtr x, IntPtr y)
		{
			return x == y;
		}
		public int GetHashCode (IntPtr obj)
		{
			return obj.GetHashCode ();
		}
	}

	internal class TypeEqualityComparer : IEqualityComparer<Type>
	{
		public bool Equals (Type x, Type y)
		{
			return x == y;
		}
		public int GetHashCode (Type obj)
		{
			if (obj == null)
				return 0;
			return obj.GetHashCode ();
		}
	}

	internal struct IntPtrTypeValueTuple : IEquatable<IntPtrTypeValueTuple>
	{
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

		public override bool Equals (object obj)
		{
			if (obj is IntPtrTypeValueTuple)
				return Equals ((IntPtrTypeValueTuple)obj);

			return false;
		}

		public override int GetHashCode ()
		{
			return item1Comparer.GetHashCode (Item1) ^ item2Comparer.GetHashCode (Item2);
		}

		public static bool operator == (IntPtrTypeValueTuple left, IntPtrTypeValueTuple right)
		{
			return left.Equals(right);
		}

		public static bool operator != (IntPtrTypeValueTuple left, IntPtrTypeValueTuple right)
		{
			return !left.Equals(right);
		}
	}
}
