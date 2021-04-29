//
// Runtime.CoreCLR.cs: Supporting managed code for the CoreCLR bridge
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2021 Microsoft Corp.

#if NET && !COREBUILD

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Foundation;

using MonoObjectPtr=System.IntPtr;

namespace ObjCRuntime {

	public partial class Runtime {
		// This struct must be kept in sync with the _MonoObject struct in coreclr-bridge.h
		[StructLayout (LayoutKind.Sequential)]
		struct MonoObject {
			public int ReferenceCount;
			public IntPtr GCHandle;
		}

		// Comment out the attribute to get all printfs
		[System.Diagnostics.Conditional ("UNDEFINED")]
		static void log_coreclr (string message)
		{
			NSLog (message);
		}

		// Returns a retained MonoObject. Caller must release.
		static IntPtr FindAssembly (IntPtr assembly_name)
		{
			var path = Marshal.PtrToStringAuto (assembly_name);
			var name = Path.GetFileNameWithoutExtension (path);

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
			if (loadedAssembly != null) {
				log_coreclr ($"    Loaded {loadedAssembly.GetName ().Name}");
				return GetMonoObject (loadedAssembly);
			}

			log_coreclr ($"    Found no assembly named {name}");

			throw new InvalidOperationException ($"Could not find any assemblies named {name}");
		}

		static IntPtr CreateGCHandle (IntPtr gchandle, GCHandleType type)
		{
			// It's valid to create a GCHandle to a null value.
			object obj = null;
			if (gchandle != IntPtr.Zero)
				obj = GetGCHandleTarget (gchandle);
			var rv = GCHandle.Alloc (obj, type);
			return GCHandle.ToIntPtr (rv);
		}

		static void FreeGCHandle (IntPtr gchandle)
		{
			GCHandle.FromIntPtr (gchandle).Free ();
		}

		// Returns a retained MonoObject. Caller must release.
		static IntPtr GetMonoObject (IntPtr gchandle)
		{
			return GetMonoObject (GetGCHandleTarget (gchandle));
		}

		// Returns a retained MonoObject. Caller must release.
		static IntPtr GetMonoObject (object obj)
		{
			if (obj == null)
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

			IntPtr rv = MarshalStructure (mobj);

			log_coreclr ($"GetMonoObjectImpl ({obj.GetType ()}) => 0x{rv.ToString ("x")} => GCHandle=0x{handle.ToString ("x")}");

			return rv;
		}

		static object GetMonoObjectTarget (MonoObjectPtr mobj)
		{
			if (mobj == IntPtr.Zero)
				return null;

			unsafe {
				MonoObject *monoobj = (MonoObject *) mobj;
				return GetGCHandleTarget (monoobj->GCHandle);
			}
		}

		static IntPtr MarshalStructure<T> (T value) where T: struct
		{
			var rv = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (T)));
			StructureToPtr (value, rv);
			return rv;
		}

		static void StructureToPtr (object obj, IntPtr ptr)
		{
			if (obj == null)
				return;

			Marshal.StructureToPtr (obj, ptr, false);
		}

		static IntPtr GetAssemblyName (IntPtr gchandle)
		{
			var asm = (Assembly) GetGCHandleTarget (gchandle);
			return Marshal.StringToHGlobalAuto (Path.GetFileName (asm.Location));
		}

		static void SetFlagsForNSObject (IntPtr gchandle, byte flags)
		{
			var obj = (NSObject) GetGCHandleTarget (gchandle);
			obj.FlagsInternal = (NSObject.Flags) flags;
		}

		static byte GetFlagsForNSObject (IntPtr gchandle)
		{
			var obj = (NSObject) GetGCHandleTarget (gchandle);
			return (byte) obj.FlagsInternal;
		}

		static IntPtr GetMethodDeclaringType (MonoObjectPtr mobj)
		{
			var method = (MethodBase) GetMonoObjectTarget (mobj);
			return GetMonoObject (method.DeclaringType);
		}

		static IntPtr ObjectGetType (MonoObjectPtr mobj)
		{
			var obj = GetMonoObjectTarget (mobj);
			if (obj == null) {
				log_coreclr ($"ObjectGetType (0x{mobj.ToString ("x")}) => null object");
				return IntPtr.Zero;
			}
			return GetMonoObject (obj.GetType ());
		}

		static bool IsInstance (MonoObjectPtr mobj, MonoObjectPtr mtype)
		{
			var obj = GetMonoObjectTarget (mobj);
			if (obj == null)
				return false;

			var type = (Type) GetMonoObjectTarget (mtype);
			var rv = type.IsAssignableFrom (obj.GetType ());

			log_coreclr ($"IsInstance ({obj.GetType ()}, {type})");

			return rv;
		}
	}
}

#endif // NET && !COREBUILD
