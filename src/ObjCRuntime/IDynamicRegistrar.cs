//
// IDynamicRegistrar.cs:
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 - 2014 Xamarin Inc. 
//

using System;
using System.Collections.Generic;
using System.Reflection;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Registrar {
#if !COREBUILD
	class LazyMapEntry {
		public string Typename;
		public bool IsCustomType;
	}

	interface IDynamicRegistrar {
		void RegisterAssembly (Assembly assembly);
		UnmanagedMethodDescription GetMethodDescription (Type type, IntPtr selptr);
		UnmanagedMethodDescription GetMethodDescriptionAndObject (Type type, IntPtr sel, IntPtr obj, ref IntPtr mthis);
		Type Lookup (IntPtr klass, bool throw_on_error);
		IntPtr Register (Type Type);
		void Register (Type gype, ref List<Exception> exceptions);
		Dictionary<IntPtr, LazyMapEntry> GetRegistrationMap (int initial_capacity);
		void SetAssemblyRegistered (string assembly);
		Dictionary<IntPtr, MethodDescription> GetMethods (Type type);
		void RegisterMethods (Type type, Dictionary<IntPtr, MethodDescription> methods);
		bool IsCustomType (Type type);
		void RegisterMethod (Type type, MethodInfo minfo, ExportAttribute ea);
		IEnumerable<Assembly> GetAssemblies ();
		string ComputeSignature (MethodInfo minfo);
	}
#endif
}

