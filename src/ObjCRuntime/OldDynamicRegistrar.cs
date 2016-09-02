//
// OldDynamicRegistrar.cs: The old dynamic registrar. This is obsolete code, and will be removed once the bugs have been ironed out of the new registrar.
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. 
//

//#define VERBOSE_REGISTRAR

#if IOS
#if !XAMCORE_2_0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

using MonoTouchException = global::MonoTouch.RuntimeException;

namespace XamCore.Registrar {
	internal class OldDynamicRegistrar : IDynamicRegistrar {
		Dictionary <IntPtr, Type> type_map;
		Dictionary <IntPtr, LazyMapEntry> lazy_map;
		Dictionary <Type, Dictionary <IntPtr, MethodDescription>> method_map;
		protected object lock_obj = new object ();
		List<Assembly> assemblies;
		bool class_map;
		HashSet <Type> custom_type_map;

		public OldDynamicRegistrar ()
		{
			type_map = new Dictionary<IntPtr, Type> (Runtime.IntPtrEqualityComparer);
			method_map = new Dictionary <Type, Dictionary <IntPtr, MethodDescription>> (Runtime.TypeEqualityComparer);
			assemblies = new List<Assembly> ();
			custom_type_map = new HashSet <Type> (Runtime.TypeEqualityComparer);
		}

		public void SetAssemblyRegistered (string assembly)
		{
			class_map = true;
		}

		public Dictionary<IntPtr, LazyMapEntry> GetRegistrationMap (int initial_capacity)
		{
			if (lazy_map == null)
				lazy_map = new Dictionary<IntPtr, LazyMapEntry> (initial_capacity, Runtime.IntPtrEqualityComparer);
			return lazy_map;
		}

		public void RegisterAssembly (Assembly a)
		{
			try {
				if (class_map)
					return;

				if (assemblies.Contains (a))
					return;
				assemblies.Add (a);

				foreach (Type type in a.GetTypes ()) {
					if (type != typeof (NSObject) && !type.IsSubclassOf (typeof (NSObject)))
						continue;

					if (Attribute.IsDefined (type, typeof (ModelAttribute), false))
						continue;

					try {
						Class.Register (type);
					} catch (Exception ex) {
						Console.Error.WriteLine ("Could not load the class '{0}' for registration: {1}", type.FullName, ex.Message);
					}
				}
			}
			catch (Exception e) {
				Console.Error.WriteLine ("Could not load '{0}' for registration: {1}", a.GetName ().Name, e);
				if (Runtime.Arch == XamCore.ObjCRuntime.Arch.SIMULATOR) {
					// this should not happen anymore but, in case I'm wrong, we'll still display something useful to the user
					// only show this if we're executing on the simulator, otherwise it's simply confusing
					Console.Error.WriteLine ("This could be due to an outdated assembly kept by the simulator, location: {0}", a.Location);
				}
			}
		}

		public IEnumerable<Assembly> GetAssemblies ()
		{
			return assemblies;
		}

		public Dictionary <IntPtr, MethodDescription> GetMethods (Type t)
		{
			Dictionary<IntPtr, MethodDescription> methods;
			
			lock (lock_obj) {
				if (method_map.TryGetValue (t, out methods))
					return methods;
				
				return RegisterMethods (t);
			}
		}
		
		/*
		Type must have been previously registered.
		*/
		public bool IsCustomType (Type type)
		{
			lock (lock_obj)
				return custom_type_map.Contains (type);
		}

		// This method is used internally by the -mapinject feature in mtouch.
		public void RegisterMethods (Type type, Dictionary<IntPtr, MethodDescription> methods)
		{
			lock (lock_obj)
				method_map [type] = methods;
		}
		
		public UnmanagedMethodDescription GetMethodDescriptionAndObject (Type type, IntPtr sel, IntPtr obj, ref IntPtr mthis)
		{
			throw new NotSupportedException (string.Format ("The old registrars do not support generic types (type: {0} selector: {1})", type.FullName, Selector.GetName (sel)));
		}

		public UnmanagedMethodDescription GetMethodDescription (Type type, IntPtr selptr)
		{
#if DEBUG
			Console.WriteLine ("Looking up method: {0} on {1}", new Selector (selptr).Name, new Class (type).Name);
#endif
			var methods = Class.GetMethods (type);
			
			MethodDescription method;
			if (methods.TryGetValue (selptr, out method)) {
#if DEBUG
				Console.WriteLine ("Found method: {0} on {1}", method.method, method.method.DeclaringType);
#endif
				return method.GetUnmanagedDescription ();
			}
#if DEBUG
			Console.WriteLine ("Failed to find selector");
#endif
			throw new Exception (String.Format ("Failed to find selector {0} on {1}", new Selector (selptr).Name, type.FullName));
		}
		
		public Type Lookup (IntPtr klass, bool throw_on_error)
		{
			IntPtr original_class = klass;

			// FAST PATH
			Type type;
			lock (lock_obj) {
				if (type_map.TryGetValue (klass, out type))
					return type;
				
				LazyMapEntry entry;
				if (lazy_map != null && lazy_map.TryGetValue (klass, out entry)) {
					type = Type.GetType (entry.Typename);
					type_map [klass] = type;
					AddCustomType (type);

					lazy_map.Remove (klass);
					return type;
				}
				
				// TODO:  When we type walk we currently populate the type map
				// from the walk point with the target, we should gather some
				// stats here, and see how many times there is a intermediate class
				// and see if we should populate them in the map as well
				IntPtr orig_klass = klass;
				
				do {
					IntPtr kls = Class.class_getSuperclass (klass);

					if (kls == IntPtr.Zero)
						throw new MonoTouchException ("Could not walk the type hierarchy of {2} (0x{3}): Can't get the superclass of {0} (0x{1})", new Class (klass).Name, klass.ToString ("x"), new Class (original_class).Name, original_class.ToString ("x"));
					
					if (type_map.TryGetValue (kls, out type)) {
						type_map [orig_klass] = type;
						return type;
					}
					
					if (lazy_map != null && lazy_map.TryGetValue (kls, out entry)) {
						type = Type.GetType (entry.Typename);
						type_map [kls] = type;
						type_map [orig_klass] = type;
						lazy_map.Remove (kls);
						return type;
					}
					
					klass = kls;
				} while (true);
			}
		}

		public IntPtr Register (Type type) {
			RegisterAttribute attr = (RegisterAttribute) Attribute.GetCustomAttribute (type, typeof (RegisterAttribute), false);
			string name = attr == null ? type.FullName : attr.Name ?? type.FullName;
			bool is_wrapper = attr == null ? false : attr.IsWrapper;
			return Register (type, name, is_wrapper);
		}

		public void Register (Type type, ref List<Exception> exceptions)
		{
			Register (type);
		}
		
		unsafe IntPtr Register (Type type, string name, bool is_wrapper) {
			IntPtr parent = IntPtr.Zero;
			IntPtr handle = IntPtr.Zero;
			
			lock (lock_obj) {
				handle = Class.objc_getClass (name);
				
				if (handle != IntPtr.Zero) {
					if (!type_map.ContainsKey (handle)) {
						type_map [handle] = type;
					}
					return handle;
				}
				
				/*FIXME pick a more suitable exception type */
				/*FIXME try to guess the name of the missing library - quite trivial for monotouch.dll*/
				if (is_wrapper) {
					if (Runtime.Arch == Arch.DEVICE) {
						// types decorated with [Model] attribute are not registered (see registrar.cs and regression from #769)
						// a missing [Model] attribute will cause this error on devices (e.g. bug #4864)
						if (!Attribute.IsDefined (type, typeof (ModelAttribute), false))
							throw new Exception (string.Format ("Wrapper type '{0}' is missing its native ObjectiveC class '{1}'.", type.FullName, name));
					} else {
						/*On simulator this is a common issue since we eagerly register all types. This is an issue with unlinked
						monotouch.dll since we don't link all frameworks most of the time.
						*/
						return IntPtr.Zero;
					}
				}
				
				Dictionary <IntPtr, MethodDescription> methods = new Dictionary <IntPtr, MethodDescription> (Runtime.IntPtrEqualityComparer);
				
				Type parent_type = type.BaseType;
				string parent_name = null;
				while (Attribute.IsDefined (parent_type, typeof (ModelAttribute), false))
					parent_type = parent_type.BaseType;
				RegisterAttribute parent_attr = (RegisterAttribute) Attribute.GetCustomAttribute (parent_type, typeof (RegisterAttribute), false);
				parent_name = parent_attr == null ? parent_type.FullName : parent_attr.Name ?? parent_type.FullName;
				parent = Class.objc_getClass (parent_name);
				if (parent == IntPtr.Zero && parent_type.Assembly != NSObject.PlatformAssembly) {
					bool parent_is_wrapper = parent_attr == null ? false : parent_attr.IsWrapper;
					// Its possible as we scan that we might be derived from a type that isn't reigstered yet.
					Register (parent_type, parent_name, parent_is_wrapper);
					parent = Class.objc_getClass (parent_name);
				}
				if (parent == IntPtr.Zero) {
					// This spams mtouch, we need a way to differentiate from mtouch's (ab)use
					// Console.WriteLine ("CRITICAL WARNING: Falling back to NSObject for type {0} reported as {1}", type, parent_type);
					parent = Class.objc_getClass ("NSObject");
				}
				handle = Class.objc_allocateClassPair (parent, name, IntPtr.Zero);
				
				Class.class_addIvar (handle, "__monoObjectGCHandle", (IntPtr) Marshal.SizeOf (typeof (Int32)), (byte) 4, "i");
				
				foreach (PropertyInfo prop in type.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
					ConnectAttribute cattr = (ConnectAttribute) Attribute.GetCustomAttribute (prop, typeof (ConnectAttribute));
					if (cattr != null) {
						string ivar_name = cattr.Name ?? prop.Name;
						Class.class_addIvar (handle, ivar_name, (IntPtr) Marshal.SizeOf (typeof (IntPtr)), (byte) Math.Log (Marshal.SizeOf (typeof (IntPtr)), 2), "@");
					}
					
					var exportAtt = (ExportAttribute) Attribute.GetCustomAttribute (prop, typeof (ExportAttribute));
					if (exportAtt != null) {
						var m = prop.GetGetMethod (true);
						if (m != null) {
							var ea = exportAtt.ToGetter (prop);
							RegisterMethod (m, ea, type, handle, false);
							var sel = Selector.GetHandle (ea.Selector);
							methods [sel] = new MethodDescription (m, ea.ArgumentSemantic);
						}
						m = prop.GetSetMethod (true);
						if (m != null) {
							var ea = exportAtt.ToSetter (prop);
							RegisterMethod (m, ea, type, handle, false);
							var sel = Selector.GetHandle (ea.Selector);
							methods [sel] = new MethodDescription (m, ea.ArgumentSemantic);
						}
						
						// http://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtPropertyIntrospection.html
						int count = 0;
						var props = new Class.objc_attribute_prop [3];
						props [count++] = new Class.objc_attribute_prop { name = "T", value = TypeConverter.ToNative (prop.PropertyType) };
						switch (exportAtt.ArgumentSemantic) {
						case ArgumentSemantic.Copy:
							props [count++] = new Class.objc_attribute_prop { name = "C", value = "" };
							break;
						case ArgumentSemantic.Retain:
							props [count++] = new Class.objc_attribute_prop { name = "&", value = "" };
							break;
						}
						props [count++] = new Class.objc_attribute_prop { name = "V", value = exportAtt.Selector };
						
						Class.class_addProperty (handle, exportAtt.Selector, props, count);
						
#if DEBUG_REGISTER
						Console.WriteLine ("[PROPERTY] Registering {0} of type {2} ({3}) on {1}", exportAtt.Selector, type, prop.PropertyType, TypeConverter.ToNative (prop.PropertyType));
#endif
					}
				}
				
				Class.class_addMethod (handle, Selector.GetHandle (Selector.Release), Method.ReleaseTrampoline, "v@:");
				Class.class_addMethod (handle, Selector.GetHandle (Selector.Retain), Method.RetainTrampoline, "@@:");
				Class.class_addMethod (handle, Selector.GetHandle ("xamarinGetGCHandle"), Method.GetGCHandleTrampoline, "i@:");
				Class.class_addMethod (handle, Selector.GetHandle ("xamarinSetGCHandle:"), Method.SetGCHandleTrampoline, "v@:i");
				
				var method_interface_map = SharedDynamic.PrepareInterfaceMethodMapping (type);
				foreach (MethodInfo minfo in type.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
					ExportAttribute ea = (ExportAttribute) Attribute.GetCustomAttribute (minfo.GetBaseDefinition (), typeof (ExportAttribute));
					
					if (ea == null)
						ea = GetMappedExportAttribute (method_interface_map, minfo);

					if (ea == null)
						continue;
					
					if (minfo.IsGenericMethod || minfo.IsGenericMethodDefinition) {
						Console.WriteLine ("The registrar found an exported generic method: '{0}.{1}'. Exporting generic methods is not supported.", minfo.DeclaringType.FullName, minfo.Name);
						continue;
					}
					
					bool is_conforms_to_protocol;
					bool is_model = false;
					
					is_conforms_to_protocol = minfo.DeclaringType.Assembly == NSObject.PlatformAssembly && minfo.DeclaringType.Name == "NSObject" && minfo.Name == "ConformsToProtocol";
					
					if (!is_conforms_to_protocol)
						is_model = minfo.IsVirtual && ((minfo.DeclaringType != type && minfo.DeclaringType.Assembly == NSObject.PlatformAssembly) || (Attribute.IsDefined (minfo.DeclaringType, typeof (ModelAttribute), false)));
					
					if (is_model)
						continue;
					
					RegisterMethod (minfo, ea, type, handle, false);
					
					var sel = Selector.GetHandle (ea.Selector ?? minfo.Name);
					
					methods [sel] = new MethodDescription (minfo, ea.ArgumentSemantic);
				}
				
				bool default_ctor_found = false;
				foreach (ConstructorInfo cinfo in type.GetConstructors (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
					if (!default_ctor_found && !cinfo.IsStatic && cinfo.GetParameters ().Length == 0) {
#if DEBUG_REGISTER
						Console.WriteLine ("[CTOR] Registering {0}[0x{1:x}|{2}] on {3} -> ({4})", "init", (int) Selector.Init, Method.Signature (cinfo), type, cinfo);
#endif
						default_ctor_found = true;
						Class.class_addMethod (handle, Selector.GetHandle ("init"), Method.ConstructorTrampoline, Method.Signature (cinfo));
						methods [Selector.GetHandle ("init")] = new MethodDescription (cinfo, ArgumentSemantic.Assign);
					}

					ExportAttribute ea = (ExportAttribute) Attribute.GetCustomAttribute (cinfo, typeof (ExportAttribute));
					if (ea == null)
						continue;
					
					IntPtr sel = Selector.GetHandle (ea.Selector);
					
					Class.class_addMethod (handle, sel, Method.ConstructorTrampoline, Method.Signature (cinfo));
#if DEBUG_REGISTER
					Console.WriteLine ("[CTOR] Registering {0}[0x{1:x}|{2}] on {3} -> ({4})", ea.Selector, (int) sel, Method.Signature (cinfo), type, cinfo);
#endif
					methods [sel] = new MethodDescription (cinfo, ea.ArgumentSemantic);
				}
				
				Class.objc_registerClassPair (handle);
				
				type_map [handle] = type;
				method_map [type] = methods;
				AddCustomType (type);

				return handle;
			}
		}

		static ExportAttribute GetMappedExportAttribute (Dictionary<MethodBase, List<MethodBase>> method_interface_map, MethodBase method)
		{
			List<MethodBase> iface_methods;

			if (method_interface_map == null || method_interface_map.Count == 0)
				return null;

			if (!method_interface_map.TryGetValue (method, out iface_methods))
				return null;

			if (iface_methods.Count > 2)
				throw Shared.GetMT4127 (method, iface_methods);

			return (ExportAttribute)Attribute.GetCustomAttribute (iface_methods [0], typeof(ExportAttribute));
		}

		public virtual void AddCustomType (Type type)
		{
			custom_type_map.Add (type);
		}
				
		Dictionary <IntPtr, MethodDescription> RegisterMethods (Type type) {
			// Caller must hold the obj_lock.
			Dictionary <IntPtr, MethodDescription> methods = new Dictionary <IntPtr, MethodDescription> (Runtime.IntPtrEqualityComparer);
			
			foreach (PropertyInfo pinfo in type.GetProperties (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
				ExportAttribute ea = (ExportAttribute) Attribute.GetCustomAttribute (pinfo, typeof (ExportAttribute));
				if (ea == null)
					continue;
				
				MethodInfo g = pinfo.GetGetMethod (true);
				if (g != null) {
					IntPtr selector = Selector.GetHandle (ea.ToGetter (pinfo).Selector);
#if DEBUG_POPULATE
					Console.WriteLine ("[GETTER] Registering {0}[0x{1:x}|{2}] on {3} -> ({4})", ea.Selector, (int) selector, Method.Signature (g), type, pinfo);
#endif
					methods [selector] = new MethodDescription (g, ea.ArgumentSemantic);
				}
				MethodInfo s = pinfo.GetSetMethod (true);
				if (s != null) {
					IntPtr selector = Selector.GetHandle (ea.ToSetter (pinfo).Selector);
#if DEBUG_POPULATE
					Console.WriteLine ("[SETTER] Registering {0}[0x{1:x}|{2}] on {3} -> ({4})", ea.Selector, (int) selector, Method.Signature (s), type, pinfo);
#endif
					methods [selector] = new MethodDescription (s, ea.ArgumentSemantic);
				}
			}
			
			var method_interface_map = SharedDynamic.PrepareInterfaceMethodMapping (type);
			foreach (MethodInfo minfo in type.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
				ExportAttribute ea = (ExportAttribute) Attribute.GetCustomAttribute (minfo.GetBaseDefinition (), typeof (ExportAttribute));
				
				if (ea == null)
					ea = GetMappedExportAttribute (method_interface_map, minfo);

				if (ea == null)
					continue;

				IntPtr selector = Selector.GetHandle (ea.Selector ?? minfo.Name);
				
				MethodDescription md;
				if (!methods.TryGetValue (selector, out md)) {
#if DEBUG_POPULATE
					Console.WriteLine ("[METHOD] Registering {0}[0x{1:x}|{2}] from {3} -> ({4} on {5})", ea.Selector, (int) selector, Method.Signature (minfo), type, minfo, minfo.DeclaringType);
#endif
					methods.Add (selector, new MethodDescription (minfo, ea.ArgumentSemantic));
					continue;
				}
				
				//
				// More than one method can exist for hidden methods. Choose one closest to
				// the caller type
				//
				if (minfo.DeclaringType.IsSubclassOf (md.method.DeclaringType)) {
#if DEBUG_POPULATE
					Console.WriteLine ("[METHOD] Re-registering {0}[0x{1:x}|{2}] from {3} -> ({4} on {5})", ea.Selector, (int) selector, Method.Signature (minfo), type, minfo, minfo.DeclaringType);
#endif
					methods [selector] = new MethodDescription (minfo, ea.ArgumentSemantic);
				}
			}
			
			bool default_ctor_found = false;
			foreach (ConstructorInfo cinfo in type.GetConstructors (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
				if (!default_ctor_found && !cinfo.IsStatic && cinfo.GetParameters ().Length == 0) {
					default_ctor_found = true;
					methods [Selector.GetHandle ("init")] = new MethodDescription (cinfo, ArgumentSemantic.Assign);
				}
				ExportAttribute ea = (ExportAttribute) Attribute.GetCustomAttribute (cinfo, typeof (ExportAttribute));
				if (ea == null)
					continue;
				if (ea.Selector == null)
					throw new Exception ("Constructor's must have a Export attribute with the selector specified");
				IntPtr selector = Selector.GetHandle (ea.Selector);
#if DEBUG_POPULATE
				Console.WriteLine ("[CTOR] Registering {0}[0x{1:x}|{2}] on {3} -> ({4})", ea.Selector, (int) selector, Method.Signature (cinfo), type, cinfo);
#endif
				methods [selector] = new MethodDescription (cinfo, ea.ArgumentSemantic);
			}
			
			method_map [type] = methods;
			
			return methods;
		}

		public static bool TypeContainsDouble (Type t) {
			foreach (FieldInfo field in t.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				if (field.FieldType == typeof (double) || field.FieldType == typeof (float))
					return true;
				if (field.FieldType.IsValueType && !field.FieldType.IsEnum && field.FieldType.Assembly != typeof (object).Assembly)
					if (TypeContainsDouble (field.FieldType))
						return true;
			}
			
			return false;
		}

		public void RegisterMethod (Type type, MethodInfo minfo, ExportAttribute ea)
		{
			RegisterMethod (minfo, ea, type, Class.GetHandle (type), true);
		}
		
		public void RegisterMethod (MethodInfo minfo, ExportAttribute ea, Type type, IntPtr handle, bool update_map) {
			IntPtr reg_handle = IntPtr.Zero;
			IntPtr tramp = IntPtr.Zero;
			IntPtr sel = Selector.GetHandle (ea.Selector ?? minfo.Name);
			string signature = Method.Signature (minfo);
			Type return_type = minfo.ReturnType;
			
			reg_handle = minfo.IsStatic ? Class.object_getClass (handle) : handle;
			
			if (return_type.IsValueType && !return_type.IsEnum && return_type.Assembly != typeof (object).Assembly && (Runtime.Arch == Arch.DEVICE || Marshal.SizeOf (return_type) > 8)) {
				if (Runtime.Arch == Arch.SIMULATOR) {
					if (TypeContainsDouble (return_type))
						tramp = minfo.IsStatic ? Method.X86_DoubleABI_StaticStretTrampoline : Method.X86_DoubleABI_StretTrampoline;
					else
						tramp = minfo.IsStatic ? Method.StaticStretTrampoline : Method.StretTrampoline;
				} else {
					tramp = minfo.IsStatic ? Method.StaticStretTrampoline : Method.StretTrampoline;
				}
			} else if (return_type.IsValueType && !return_type.IsEnum && return_type.Assembly != typeof (object).Assembly && Runtime.Arch == Arch.SIMULATOR && Marshal.SizeOf (return_type) > 4) {
				// for instance CGSize...
				tramp = minfo.IsStatic ? Method.StaticLongTrampoline : Method.LongTrampoline;
			} else {
				switch (signature [0]) {
				case 'Q':
				case 'q':
					tramp = minfo.IsStatic ? Method.StaticLongTrampoline : Method.LongTrampoline;
					break;
				case 'f':
					tramp = minfo.IsStatic ? Method.StaticSingleTrampoline : Method.SingleTrampoline;
					break;
				case 'd':
					tramp = minfo.IsStatic ? Method.StaticDoubleTrampoline : Method.DoubleTrampoline;
					break;
				default:
					tramp = minfo.IsStatic ? Method.StaticTrampoline : Method.Trampoline;
					break;
				}
			}
			
#if DEBUG_REGISTER
			Console.WriteLine ("[METHOD] Registering {0}[0x{1:x}|{2}] on {3} -> ({4}) tramp: 0x{5}", ea.Selector, (int) sel, Method.Signature (minfo), type, minfo, tramp.ToString ("x"));
#endif
			Class.class_addMethod (reg_handle, sel, tramp, Method.Signature (minfo));
			
			if (update_map) {
				Dictionary<IntPtr, MethodDescription> methods;
				lock (lock_obj) {
					if (!method_map.TryGetValue (type, out methods)) {
						methods = new Dictionary<IntPtr, MethodDescription> (Runtime.IntPtrEqualityComparer);
						method_map.Add (type, methods);
					}
					
					methods[sel] = new MethodDescription (minfo, ea.ArgumentSemantic);
				}
			}
		}

		public string ComputeSignature (MethodInfo minfo, bool isBlockSignature)
		{
			return Method.Signature (minfo);
		}
	}
}

#endif
#endif // IOS
