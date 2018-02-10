//
// DynamicRegistrar.cs: The dynamic registrar
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc. 
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif

namespace Registrar {
	// Somewhere to put shared code between the old and the new dynamic registrars.
	// Putting code in either of those classes will increase the executable size,
	// since unused code will be pulled in by the linker.
	static class SharedDynamic {
		public static Dictionary<MethodBase, List<MethodBase>> PrepareInterfaceMethodMapping (Type type)
		{
			Dictionary<MethodBase, List<MethodBase>> rv = null;
			var ifaces = type.FindInterfaces ((v, o) =>
			                                  {
				var attribs = v.GetCustomAttributes (typeof(ProtocolAttribute), true);
				return attribs != null && attribs.Length > 0;
			}, null);

			foreach (var iface in ifaces) {
				var map = type.GetInterfaceMap (iface);
				for (int i = 0; i < map.InterfaceMethods.Length; i++) {
					var ifaceMethod = map.InterfaceMethods [i];
					var impl = map.TargetMethods [i];

					if (SharedDynamic.GetOneAttribute<ExportAttribute> (ifaceMethod) == null)
						continue;

					List<MethodBase> list;
					if (rv == null) {
						rv = new Dictionary<MethodBase, List<MethodBase>> ();
						rv [impl] = list = new List<MethodBase> ();
					} else if (!rv.TryGetValue (impl, out list)) {
						rv [impl] = list = new List<MethodBase> ();
					}
					list.Add (ifaceMethod);
				}
			}

			return rv;
		}
		
		public static T GetOneAttribute<T> (ICustomAttributeProvider provider) where T : Attribute
		{
			var attribs = provider.GetCustomAttributes (typeof (T), false);
			if (attribs.Length == 0)
				return null;
			else if (attribs.Length == 1)
				return (T) attribs [0];
			var member = provider as MemberInfo;
			if (member != null)
				throw new AmbiguousMatchException (string.Format ("The member '{0}' contains more than one '{1}'", member.Name, typeof (T).FullName));
			var parameter = provider as ParameterInfo;
			if (parameter != null)
				throw new AmbiguousMatchException (string.Format ("The parameter '{0}' contains more than one '{1}'", parameter.Name, typeof (T).FullName));

			throw new AmbiguousMatchException (string.Format ("The member '{0}' contains more than one '{1}'", provider, typeof (T).FullName));
		}
	}

	class DynamicRegistrar : Registrar {
		Dictionary<IntPtr, ObjCType> type_map;
		Dictionary <string, object> registered_assemblies; // Use Dictionary instead of HashSet to avoid pulling in System.Core.dll.

		// custom_type_map can be accessed from multiple threads, and at the
		// same time mutated by the registrar, so any accesses needs to be locked
		// so that it's not queried and mutated at the same time from multiple threads.
		// Note that the registrar is already making sure it's not _mutated_ from
		// multiple threads at the same time.
		Dictionary <Type, object> custom_type_map; // Use Dictionary instead of HashSet to avoid pulling in System.Core.dll.

		protected object lock_obj = new object ();

		public DynamicRegistrar ()
		{
			type_map = new Dictionary<IntPtr, ObjCType> (Runtime.IntPtrEqualityComparer);
			custom_type_map = new Dictionary <Type, object> (Runtime.TypeEqualityComparer);
		}

		protected override bool SkipRegisterAssembly (Assembly assembly)
		{
			return registered_assemblies != null && registered_assemblies.ContainsKey (GetAssemblyName (assembly));
		}

		public void SetAssemblyRegistered (string assembly)
		{
			if (registered_assemblies == null)
				registered_assemblies = new Dictionary<string, object> ();
			registered_assemblies.Add (assembly, null);
		}

		protected override bool ContainsPlatformReference (Assembly assembly)
		{
			var aname = assembly.GetName ().Name;

			if (aname == CompatAssemblyName || aname == DualAssemblyName)
				return true;

			foreach (var ar in assembly.GetReferencedAssemblies ()) {
				if (ar.Name == CompatAssemblyName || ar.Name == DualAssemblyName)
					return true;
			}
			return false;
		}

		/*
		Type must have been previously registered.
		*/
		public bool IsCustomType (Type type)
		{
			lock (custom_type_map)
				return custom_type_map.ContainsKey (type);
		}

		protected override bool IsSimulatorOrDesktop {
			get {
#if MONOMAC
				return true;
#else
				return Runtime.Arch == Arch.SIMULATOR;
#endif
			}
		}

		protected override bool Is64Bits {
			get {
				return IntPtr.Size == 8;
			}
		}

		protected override bool IsDualBuildImpl {
			get {
				return NSObject.PlatformAssembly.GetName ().Name == 
#if MONOMAC
					"Xamarin.Mac";
#elif TVOS
					"Xamarin.TVOS";
#elif WATCH
					"Xamarin.WatchOS";
#elif IOS
					"Xamarin.iOS";
#else
	#error unknown platform
					"unknown platform";
#endif
			}
		}

		public void RegisterMethod (Type type, MethodInfo minfo, ExportAttribute ea)
		{
			if (!IsNSObject (type))
				throw new ArgumentException (string.Format ("Cannot register methods on '{0}'; it does not inherit from NSObject.", type.FullName));

			if (!minfo.IsStatic && type != minfo.DeclaringType)
				throw new ArgumentException (string.Format ("Cannot register the instance method '{0}' on the type '{1}'. The type to connect to ('{2}') must match the method's type ('{1}').", minfo.Name, type.FullName, minfo.DeclaringType.FullName));

			List<Exception> exceptions = null;
			var objcType = RegisterType (type, ref exceptions);
			var method = new ObjCMethod (this, objcType, minfo);
			if (method.SetExportAttribute (ea, ref exceptions)) {
				if (exceptions == null) {
					objcType.Add (method, ref exceptions);
					if (exceptions == null)
						RegisterMethod (method);
				}
			}

			if (exceptions != null)
				throw exceptions.Count == 1 ? exceptions [0] : new AggregateException (exceptions);
		}

		protected override IEnumerable<MethodBase> FindMethods (Type type, string name)
		{
			foreach (var method in type.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				if (method.Name == name)
					yield return method;
		}

		protected override PropertyInfo FindProperty (Type type, string name)
		{
			return type.GetProperty (name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public override Type FindType (Type relative, string @namespace, string name)
		{
			foreach (var type in relative.Assembly.GetTypes ()) {
				if (type.Namespace == @namespace && type.Name == name)
					return type;
			}
			return null;
		}

		protected override int GetValueTypeSize (Type type)
		{
			return Marshal.SizeOf (type);
		}

		protected override bool IsCorlibType (Type type)
		{
			return type.Assembly == typeof(object).Assembly;
		}

		protected override IEnumerable<MethodBase> CollectConstructors (Type type)
		{
			return type.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		protected override IEnumerable<MethodBase> CollectMethods (Type type)
		{
			return type.GetMethods (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		}

		protected override IEnumerable<PropertyInfo> CollectProperties (Type type)
		{
			return type.GetProperties (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		}

		protected override IEnumerable<Type> CollectTypes (Assembly assembly)
		{
			Trace ("Assembly {0} has {1} types", assembly, assembly.GetTypes ().Length);
			return assembly.GetTypes ();
		}

		protected override BindAsAttribute GetBindAsAttribute (PropertyInfo property)
		{
			return property?.GetCustomAttribute<BindAsAttribute> (false);
		}

		protected override BindAsAttribute GetBindAsAttribute (MethodBase method, int parameter_index)
		{
			ICustomAttributeProvider provider;

			if (method == null)
				return null;

			var minfo = method as MethodInfo;
			if (minfo != null) {
				minfo = minfo.GetBaseDefinition ();
				if (parameter_index == -1) {
					provider = minfo.ReturnTypeCustomAttributes;
				} else {
					provider = minfo.GetParameters () [parameter_index];
				}
			} else {
				var cinfo = method as ConstructorInfo;
				if (parameter_index == -1) {
					throw ErrorHelper.CreateError (99, $"Internal error: can't get the BindAs attribute for the return value of a constructor ({GetDescriptiveMethodName (method)}). Please file a bug report with a test case (https://bugzilla.xamarin.com).");
				} else {
					provider = cinfo.GetParameters () [parameter_index];
				}
			}

			return SharedDynamic.GetOneAttribute<BindAsAttribute> (provider);
		}

		public override Type GetNullableType (Type type)
		{
			if (!type.IsGenericType)
				return null;
			if (type.GetGenericTypeDefinition () != typeof (Nullable<>))
				return null;
			return type.GetGenericArguments () [0];
		}

		protected override ConnectAttribute GetConnectAttribute (PropertyInfo property)
		{
			return SharedDynamic.GetOneAttribute<ConnectAttribute> (property);
		}

		protected override ExportAttribute GetExportAttribute (MethodBase method)
		{
			MethodInfo minfo = method as MethodInfo;
			if (minfo != null)
				return SharedDynamic.GetOneAttribute<ExportAttribute> (minfo.GetBaseDefinition ());

			ConstructorInfo cinfo = method as ConstructorInfo;
			if (cinfo != null)
				return SharedDynamic.GetOneAttribute<ExportAttribute> (cinfo);

			return null;
		}

		protected override Dictionary<MethodBase, List<MethodBase>> PrepareMethodMapping (Type type)
		{
			return SharedDynamic.PrepareInterfaceMethodMapping (type);
		}

		protected override ExportAttribute GetExportAttribute (PropertyInfo property)
		{
			return SharedDynamic.GetOneAttribute<ExportAttribute> (GetBasePropertyInTypeHierarchy (property) ?? property);
		}

		public override RegisterAttribute GetRegisterAttribute (Type type)
		{
			return SharedDynamic.GetOneAttribute<RegisterAttribute> (type);
		}

		protected override ProtocolAttribute GetProtocolAttribute (Type type)
		{
			return SharedDynamic.GetOneAttribute<ProtocolAttribute> (type);
		}

		protected override IEnumerable<ProtocolMemberAttribute> GetProtocolMemberAttributes (Type type)
		{
			foreach (var attrib in type.GetCustomAttributes (false)) {
				var pmAttrib = attrib as ProtocolMemberAttribute;
				if (pmAttrib != null)
					yield return pmAttrib;
			}
		}

		protected override List<AvailabilityBaseAttribute> GetAvailabilityAttributes (Type obj)
		{
			// No need to implement this until GetSDKVersion is implemented.
			return null;
		}

		protected override Version GetSDKVersion ()
		{
			// As far as I can tell we can't figure this out at runtime,
			// we'd have to store the information at build time somewhere
			// and then fetch it here. Punt for now.
			// This method will not be called unless GetAvailabilityAttributes
			// returns something, so having an exception here (for now) is not a problem.
			throw new NotImplementedException ();
		}

		protected override string PlatformName {
			get {
#if __TVOS__
				return "tvOS";
#elif __WATCHOS__
				return "watchOS";
#elif __IOS__
				return "iOS";
#elif MONOMAC
				return "Mac";
#else
	#error No platform
#endif
			}
		}

		protected override Type GetSystemVoidType ()
		{
			return typeof (void);
		}

		protected override Type MakeByRef (Type type)
		{
			return type.MakeByRefType ();
		}

		protected override CategoryAttribute GetCategoryAttribute (Type type)
		{
			return SharedDynamic.GetOneAttribute<CategoryAttribute> (type);
		}

		protected override Type GetProtocolAttributeWrapperType (Type type)
		{
			var attr = SharedDynamic.GetOneAttribute<ProtocolAttribute> (type);
			return attr == null ? null : attr.WrapperType;
		}

		protected override IList<AdoptsAttribute> GetAdoptsAttributes (Type type)
		{
			return (AdoptsAttribute[]) type.GetCustomAttributes (typeof (AdoptsAttribute), false);
		}

		protected override string GetAssemblyName (Assembly assembly)
		{
			return assembly.GetName ().Name;
		}

		protected override Type GetBaseType (Type type)
		{
			return type.BaseType;
		}

		protected override MethodBase GetBaseMethod (MethodBase method)
		{
			return ((MethodInfo) method).GetBaseDefinition ();
		}

		protected override Type GetElementType (Type type)
		{
			return type.GetElementType ();
		}

		protected override Type GetEnumUnderlyingType (Type type)
		{
			return Enum.GetUnderlyingType (type);
		}

		protected override string GetFieldName (FieldInfo field)
		{
			return field.Name;
		}

		protected override IEnumerable<FieldInfo> GetFields (Type type)
		{
			return type.GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		protected override Type GetFieldType (FieldInfo field)
		{
			return field.FieldType;
		}

		protected override MethodBase GetGetMethod (PropertyInfo property)
		{
			return property.GetGetMethod (true);
		}

		protected override MethodBase GetSetMethod (PropertyInfo property)
		{
			return property.GetSetMethod (true);
		}

		protected override string GetMethodName (MethodBase method)
		{
			return method.Name;
		}

		protected override void GetNamespaceAndName (Type type, out string @namespace, out string name)
		{
			@namespace = type.Namespace;
			name = type.Name;
		}

		protected override Type[] GetParameters (MethodBase method)
		{
			var parameters = method.GetParameters ();
			var types = new Type [parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
				types [i] = parameters [i].ParameterType;

			return types;
		}

		protected override string GetParameterName (MethodBase method, int parameter_index)
		{
			return method.GetParameters () [parameter_index].Name;
		}

		protected override string GetPropertyName (PropertyInfo property)
		{
			return property.Name;
		}

		protected override Type GetPropertyType (PropertyInfo property)
		{
			return property.PropertyType;
		}

		protected override Type GetReturnType (MethodBase method)
		{
			var minfo = method as MethodInfo;
			if (minfo != null)
				return minfo.ReturnType;

			throw ErrorHelper.CreateError (0, "Cannot get the return type of a {0}", method.GetType ().Name);
		}
			
		protected override string GetTypeFullName (Type type)
		{
			return type.FullName;
		}

		protected override bool VerifyIsConstrainedToNSObject (Type type, out Type constrained_type)
		{
			constrained_type = null;

			if (!type.IsGenericType && !(type.IsGenericType && !type.ContainsGenericParameters) && !type.IsGenericParameter && !type.IsGenericTypeDefinition)
				return true;

			if (type.IsGenericParameter) {
				if (typeof (NSObject).IsAssignableFrom (type)) {
					// First look for a more specific constraint
					var constraints = type.GetGenericParameterConstraints ();
					foreach (var constraint in constraints) {
						if (constraint.IsSubclassOf (typeof (NSObject))) {
							constrained_type = constraint;
							return true;
						}
					}
					// Fallback to NSObject.
					constrained_type = typeof(NSObject);
					return true;
				}
				return false;
			}

			if (type.IsGenericTypeDefinition) {
				var rv = true;
				var args = type.GetGenericArguments ();
				var constrs = new Type [args.Length];
				for (int i = 0; i < args.Length; i++) {
					Type constr;
					rv &= VerifyIsConstrainedToNSObject (args [i], out constr);
					constrs [i] = constr;
				}
				constrained_type = type.MakeGenericType (constrs);
				return true;
			}

			return false;
		}

		protected override Exception CreateException (int code, Exception innerException, MethodBase method, string message, params object[] args)
		{
			// There doesn't seem to be a way to find the source code location
			// for the method using System.Reflection.
			return ErrorHelper.CreateError (code, innerException, message, args);
		}

		protected override Exception CreateException (int code, Exception innerException, Type type, string message, params object [] args)
		{
			// There doesn't seem to be a way to find the source code location
			// for the method using System.Reflection.
			return ErrorHelper.CreateError (code, innerException, message, args);
		}

		protected override string GetAssemblyQualifiedName (Type type)
		{
			return type.AssemblyQualifiedName;
		}

		protected override bool HasReleaseAttribute (MethodBase method)
		{
			var mi = method as MethodInfo;
			if (mi == null)
				return false;
			return mi.ReturnTypeCustomAttributes.IsDefined (typeof (ReleaseAttribute), false);
		}

		public static bool HasThisAttributeImpl (MethodBase method)
		{
			var mi = method as MethodInfo;
			if (mi == null)
				return false;
			return mi.IsDefined (typeof (System.Runtime.CompilerServices.ExtensionAttribute), false);
		}

		protected override bool HasThisAttribute (MethodBase method)
		{
			return HasThisAttributeImpl (method);
		}

		protected override string GetTypeName (Type type)
		{
			return type.Name;
		}

		protected override bool HasModelAttribute (Type type)
		{
			return type.IsDefined (typeof (ModelAttribute), false);
		}

		protected override bool IsArray (Type type, out int rank)
		{
			if (!type.IsArray) {
				rank = 0;
				return false;
			}
			rank = type.GetArrayRank ();
			return true;
		}

		protected override bool IsByRef (Type type)
		{
			return type.IsByRef;
		}

		protected override bool IsConstructor (MethodBase method)
		{
			return method is ConstructorInfo;
		}

		protected override bool IsGenericType (Type type)
		{
			return type.IsGenericType || type.IsGenericTypeDefinition || type.IsGenericParameter;
		}

		protected override bool IsGenericMethod (MethodBase method)
		{
			return method.IsGenericMethod || method.IsGenericMethodDefinition;
		}

		protected override Type GetGenericTypeDefinition (Type type)
		{
			return type.GetGenericTypeDefinition ();
		}

		protected override bool IsDelegate (Type type)
		{
			return type.IsSubclassOf (typeof (System.Delegate));
		}

		protected override bool IsNullable (Type type)
		{
			if (!type.IsGenericType)
				return false;

			return type.GetGenericTypeDefinition () == typeof (Nullable<>);
		}

		protected override bool IsEnum (Type type, out bool isNativeEnum)
		{
			isNativeEnum = false;
			if (type.IsEnum)
				isNativeEnum = IsDualBuildImpl && type.IsDefined (typeof (NativeAttribute), false);
			return type.IsEnum;
		}

		protected override bool IsInterface (Type type)
		{
			return type.IsInterface;
		}

		protected override bool IsINativeObject (Type type)
		{
			return typeof (INativeObject).IsAssignableFrom (type);
		}

		protected override bool IsNSObject (Type type)
		{
			return type == typeof (NSObject) || type.IsSubclassOf (typeof (NSObject));
		}

		protected override bool IsStatic (FieldInfo field)
		{
			return field.IsStatic;
		}

		protected override bool IsStatic (MethodBase method)
		{
			return method.IsStatic;
		}
		
		protected override bool IsStatic (PropertyInfo property)
		{
			return IsStaticProperty (property);
		}
	
		protected override bool IsValueType (Type type)
		{
			return type.IsValueType;
		}

		protected override bool IsVirtual (MethodBase method)
		{
			return method.IsVirtual;
		}

		protected override Type[] GetInterfaces (Type type)
		{
			return type.GetInterfaces ();
		}

		protected override bool TryGetAttribute (Type type, string attributeNamespace, string attributeType, out object attribute)
		{
			var attribs = type.GetCustomAttributes (false);

			attribute = null;

			if (attribs.Length == 0)
				return false;

			foreach (var obj in attribs) {
				var t = obj.GetType ();
				if (t.Namespace == attributeNamespace && t.Name == attributeType) {
					if (attribute != null)
						throw new AmbiguousMatchException (string.Format ("The type '{0}' contains more than one '{1}.{2}'", type.FullName, attributeNamespace, attributeType));
					attribute = obj;
				}
			}

			return attribute != null;
		}

		protected override void ReportError (int code, string message, params object[] args)
		{
			Console.WriteLine (message, args);
		}

		Class.objc_attribute_prop [] GetPropertyAttributes (ObjCProperty property, out int count, bool isProtocol)
		{
			// http://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtPropertyIntrospection.html
			var props = new Class.objc_attribute_prop [5];
			count = 0;

			props [count++] = new Class.objc_attribute_prop { name = "T", value = ToSignature (property.PropertyType, property, true) };
			switch (property.ArgumentSemantic) {
			case ArgumentSemantic.Copy:
				props [count++] = new Class.objc_attribute_prop { name = "C", value = "" };
				break;
			case ArgumentSemantic.Retain:
				props [count++] = new Class.objc_attribute_prop { name = "&", value = "" };
				break;
			}
			if (!isProtocol)
				props [count++] = new Class.objc_attribute_prop { name = "V", value = property.Selector };

			if (property.IsReadOnly)
				props [count++] = new Class.objc_attribute_prop { name = "R", value = string.Empty };

			props [count++] = new Class.objc_attribute_prop { name = "N", value = string.Empty }; // nonatomic
			return props;
		}

		protected override void OnRegisterProtocol (ObjCType type)
		{
			var protocol = Protocol.objc_getProtocol (type.ProtocolName);

			if (protocol != IntPtr.Zero) {
				type.Handle = protocol;
				if (!type_map.ContainsKey (protocol))
					type_map [protocol] = type;
				return;
			}

			protocol = Protocol.objc_allocateProtocol (type.ProtocolName);

			if (type.Protocols != null) {
				foreach (var proto in type.Protocols) {
					if (proto.ProtocolName == "JSExport") {
#if MONOMAC
						const string msg = "Detected a protocol ({0}) inheriting from the JSExport protocol while using the dynamic registrar. It is not possible to export protocols to JavaScriptCore dynamically; the static registrar must be used (add '--registrar:static' to the additional mmp arguments in the project's Mac Build options to select the static registrar).";
#else
						const string msg = "Detected a protocol ({0}) inheriting from the JSExport protocol while using the dynamic registrar. It is not possible to export protocols to JavaScriptCore dynamically; the static registrar must be used (add '--registrar:static' to the additional mtouch arguments in the project's iOS Build options to select the static registrar).";
#endif
						ErrorHelper.Warning (4147, msg, GetTypeFullName (type.Type));
					}
					Protocol.protocol_addProtocol (protocol, proto.Handle);
				}
			}

			if (type.Properties != null) {
				foreach (var property in type.Properties) {
					int count;
					var props = GetPropertyAttributes (property, out count, true);
					// Only required instance properties are added (the rest of the logic is commented out in the public source at least,
					// see file objc4-647/runtime/objc-runtime-old.mm in Apple's open source code). Still add all properties in case Apple
					// implements their missing bits.
					Protocol.protocol_addProperty (protocol, property.Selector, props, count, !property.IsOptional, !property.IsStatic);
					// The properties need to be added as methods as well.
					var propertyType = ToSignature (property.PropertyType, property, false);
					Protocol.protocol_addMethodDescription (protocol, Selector.GetHandle (property.GetterSelector), propertyType + "@:", !property.IsOptional, !property.IsStatic);
					if (!property.IsReadOnly)
						Protocol.protocol_addMethodDescription (protocol, Selector.GetHandle (property.SetterSelector), "v@:" + propertyType, !property.IsOptional, !property.IsStatic);
				}
			}

			if (type.Methods != null) {
				foreach (var method in type.Methods) {
					Protocol.protocol_addMethodDescription (protocol, Selector.GetHandle (method.Selector), method.Signature, !method.IsOptional, !method.IsStatic);
				}
			}

			Protocol.objc_registerProtocol (protocol);
			type_map [protocol] = type;

			Trace ("   [DYNAMIC PROTOCOL] Registered the protocol {0} for {1}", type.ProtocolName, type.Type.FullName);
		}

		protected override void OnRegisterCategory (ObjCType type, ref List<Exception> exceptions)
		{
			if (type.Methods != null) {
				foreach (var method in type.Methods) {
					if (!RegisterMethod (method)) {
						AddException (ref exceptions, ErrorHelper.CreateError (4155, "Cannot register the method '{0}.{1}' with the selector '{2}' as a category method on '{3}' because Objective-C already has an implementation for this selector.",
							GetTypeFullName (type.Type), method.MethodName, method.Selector, type.ExportedName));
					}
				}
			}
		}

		protected override void OnReloadType (ObjCType type)
		{
			if (type.Handle != IntPtr.Zero)
				return;

			type.Handle = Class.GetHandle (type.ExportedName);
		}

		protected override void OnRegisterType (ObjCType type)
		{
			type.Handle = Class.GetHandle (type.ExportedName);

			if (type.Handle != IntPtr.Zero) {
				if (!type_map.ContainsKey (type.Handle))
					type_map [type.Handle] = type;
				return;
			}

			/*FIXME try to guess the name of the missing library - quite trivial for monotouch.dll*/
			// types decorated with [Model] attribute are not registered (see registrar.cs and regression from #769)
			if (type.IsWrapper && !type.IsModel) {
				if (!IsSimulatorOrDesktop) {
					// This can happen when Apple introduces new types and puts them as base types for already
					// existing types. We can't throw any exceptions in that case, since the derived class
					// can still be used in older iOS versions.

					// Hardcode these types to ignore any loading errors.
					// We could also look at the AvailabilityAttribute, but it would require us
					// to not link it away anymore like we currently do.

#if !COREBUILD && !MONOMAC && !WATCH
					var major = -1;
					switch (type.Name) {
					case "PKObject":
					case "CBAttribute":
					case "CBPeer":
						major = 8;
						break;
					case "GKGameCenterViewController":
						major = 6;
						break;
					case "CBManager":
					case "GKBasePlayer":
						major = 10;
						break;
					}
					if ((major > 0) && !UIDevice.CurrentDevice.CheckSystemVersion (major, 0))
						return;
#endif

					// a missing [Model] attribute will cause this error on devices (e.g. bug #4864)
					throw ErrorHelper.CreateError (8005, "Wrapper type '{0}' is missing its native ObjectiveC class '{1}'.", type.Type.FullName, type.Name);
				} else {
					/*On simulator this is a common issue since we eagerly register all types. This is an issue with unlinked
					monotouch.dll since we don't link all frameworks most of the time. */
					return;
				}
			}

			if (type.IsFakeProtocol)
				return;

			var super = type.SuperType;

			type.Handle = Class.objc_allocateClassPair (super.Handle, type.ExportedName, IntPtr.Zero);

			if (type.Properties != null) {
				foreach (var property in type.Properties) {
					int count;
					var props = GetPropertyAttributes (property, out count, false);
					Class.class_addProperty (type.Handle, property.Selector, props, count);
				}
			}

			if (type.Fields != null) {
				foreach (var field in type.Fields.Values)
					Class.class_addIvar (type.Handle, field.Name, new IntPtr (field.Size), field.Alignment, field.FieldType);
			}

			if (type.Methods != null) {
				foreach (var method in type.Methods)
					RegisterMethod (method);
			}

			if (type.Protocols != null) {
				foreach (var protocol in type.Protocols) {
					Class.class_addProtocol (type.Handle, protocol.Handle);
				}
			}

			Class.objc_registerClassPair (type.Handle);
			type_map [type.Handle] = type;
			AddCustomType (type.Type);

			Trace ("   [DYNAMIC CLASS] Registered the class {0} for {1}", type.ExportedName, type.Type.FullName);
		}

		public void AddCustomType (Type type)
		{
			lock (custom_type_map)
				custom_type_map [type] = null;
		}

		public void GetMethodDescriptionAndObject (Type type, IntPtr selector, bool is_static, IntPtr obj, ref IntPtr mthis, IntPtr desc)
		{
			var sel = new Selector (selector);
			var res = GetMethodNoThrow (type, type, sel.Name, is_static);
			if (res == null)
				throw ErrorHelper.CreateError (8006, "Failed to find the selector '{0}' on the type '{1}'", sel.Name, type.FullName);

			if (res.IsInstanceCategory) {
				mthis = IntPtr.Zero;
			} else {
				var nsobj = Runtime.GetNSObject (obj, Runtime.MissingCtorResolution.ThrowConstructor1NotFound, true);
				mthis = ObjectWrapper.Convert (nsobj);
				if (res.Method.ContainsGenericParameters) {
					res.WriteUnmanagedDescription (desc, FindClosedMethod (nsobj.GetType (), res.Method));
					return;
				}
			}

			res.WriteUnmanagedDescription (desc);
		}

		internal static MethodInfo FindClosedMethod (Type closed_type, MethodBase open_method)
		{
			// FIXME: I think it should be handled before getting here (but it's safer here for now)
			if (!open_method.ContainsGenericParameters)
				return (MethodInfo) open_method;

			// First we need to find the type that declared the open method.
			Type declaring_closed_type = closed_type;
			do {
				if (declaring_closed_type.IsGenericType && declaring_closed_type.GetGenericTypeDefinition () == open_method.DeclaringType) {
					closed_type = declaring_closed_type;
					break;
				}
				declaring_closed_type = declaring_closed_type.BaseType;
			} while (declaring_closed_type != null);

			// Find the closed method.
			foreach (var mi in closed_type.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
				if (mi.MetadataToken == open_method.MetadataToken) {
					return mi;
				}
			}

			throw ErrorHelper.CreateError (8003, "Failed to find the closed generic method '{0}' on the type '{1}'.", open_method.Name, closed_type.FullName);
		}

		public void GetMethodDescription (Type type, IntPtr selector, bool is_static, IntPtr desc)
		{
			var sel = new Selector (selector);
			var res = GetMethodNoThrow (type, type, sel.Name, is_static);
			if (res == null)
				throw ErrorHelper.CreateError (8006, "Failed to find the selector '{0}' on the type '{1}'", sel.Name, type.FullName);
			if (type.IsGenericType && res.Method is ConstructorInfo)
				throw ErrorHelper.CreateError (4133, "Cannot construct an instance of the type '{0}' from Objective-C because the type is generic.", type.FullName);

			res.WriteUnmanagedDescription (desc);
		}

		ObjCMethod GetMethodNoThrow (Type original_type, Type type, string selector, bool is_static)
		{
			var objcType = RegisterType (type);
			
			if (objcType == null)
				throw ErrorHelper.CreateError (4142, "Failed to register the type '{0}'", type.FullName);

			ObjCMember member = null;
			
			if (type.BaseType != typeof (object) && !objcType.TryGetMember (selector, is_static, out member))
				return GetMethodNoThrow (original_type, type.BaseType, selector, is_static);
			
			var method = member as ObjCMethod;
			
			if (method == null)
				throw ErrorHelper.CreateError (8007, "Cannot get the method descriptor for the selector '{0}' on the type '{1}', because the selector does not correspond to a method", selector, original_type.FullName);
			
			return method;
		}

		public Type Lookup (IntPtr @class, bool throw_on_error)
		{
			ObjCType type;
			IntPtr original_class = @class;
			bool lockTaken = false;

			try {
				LockRegistrar (ref lockTaken);

				do {
					if (type_map.TryGetValue (@class, out type))
						return type.Type;

					bool is_custom_type;
					var tp = Class.FindType (@class, out is_custom_type);
					if (tp != null) {
						type = RegisterType (tp);
						if (is_custom_type)
							AddCustomType (tp);
						return tp;
					}

					@class = Class.class_getSuperclass (@class);
				} while (@class != IntPtr.Zero);
			} finally {
				if (lockTaken)
					UnlockRegistrar ();
			}

			throw ErrorHelper.CreateError (4143, "The ObjectiveC class '{0}' could not be registered, it does not seem to derive from any known ObjectiveC class (including NSObject).", Marshal.PtrToStringAuto (Class.class_getName (original_class)));
		}

		bool RegisterMethod (ObjCMethod method)
		{
			IntPtr reg_handle;
			IntPtr tramp;

			reg_handle = (method.IsStatic && !method.IsCategoryInstance) ? Class.object_getClass (method.DeclaringType.Handle) : method.DeclaringType.Handle;

			switch (method.Trampoline) {
			case Trampoline.Constructor:
				tramp = Method.ConstructorTrampoline;
				break;
			case Trampoline.Double:
				tramp = Method.DoubleTrampoline;
				break;
			case Trampoline.Long:
				tramp = Method.LongTrampoline;
				break;
			case Trampoline.Normal:
				tramp = Method.Trampoline;
				break;
			case Trampoline.Release:
				tramp = Method.ReleaseTrampoline;
				break;
			case Trampoline.Retain:
				tramp = Method.RetainTrampoline;
				break;
			case Trampoline.Single:
				tramp = Method.SingleTrampoline;
				break;
			case Trampoline.Static:
				tramp = Method.StaticTrampoline;
				break;
			case Trampoline.StaticDouble:
				tramp = Method.StaticDoubleTrampoline;
				break;
			case Trampoline.StaticLong:
				tramp = Method.StaticLongTrampoline;
				break;
			case Trampoline.StaticSingle:
				tramp = Method.StaticSingleTrampoline;
				break;
			case Trampoline.StaticStret:
				tramp = Method.StaticStretTrampoline;
				break;
			case Trampoline.Stret:
				tramp = Method.StretTrampoline;
				break;
			case Trampoline.X86_DoubleABI_StaticStretTrampoline:
				tramp = Method.X86_DoubleABI_StaticStretTrampoline;
				break;
			case Trampoline.X86_DoubleABI_StretTrampoline:
				tramp = Method.X86_DoubleABI_StretTrampoline;
				break;
#if MONOMAC
			case Trampoline.CopyWithZone1:
				tramp = Method.CopyWithZone1;
				break;
			case Trampoline.CopyWithZone2:
				tramp = Method.CopyWithZone2;
				break;
#endif
			case Trampoline.GetGCHandle:
				tramp = Method.GetGCHandleTrampoline;
				break;
			case Trampoline.SetGCHandle:
				tramp = Method.SetGCHandleTrampoline;
				break;
			default:
				throw ErrorHelper.CreateError (4144, "Cannot register the method '{0}.{1}' since it does not have an associated trampoline. Please file a bug report at http://bugzilla.xamarin.com", method.DeclaringType.Type.FullName, method.Name);
			}

			return Class.class_addMethod (reg_handle, Selector.GetHandle (method.Selector), tramp, method.Signature);
		}

		static MethodInfo GetPropertyMethod (PropertyInfo property)
		{
			if (property.CanRead)
				return property.GetGetMethod (true);

			return property.GetSetMethod (true);
		}

		static bool IsStaticProperty (PropertyInfo property)
		{
			return GetPropertyMethod (property).IsStatic;
		}

		static bool IsVirtualProperty (PropertyInfo property)
		{
			return GetPropertyMethod (property).IsVirtual;
		}

		static PropertyInfo GetBasePropertyInTypeHierarchy (PropertyInfo property)
		{
			if (IsStaticProperty (property) || !IsVirtualProperty (property))
				return property;

			var @base = property.DeclaringType.BaseType;
			while (@base != null) {
				var base_property = TryMatchProperty (@base, property);
				if (base_property != null)
					return GetBasePropertyInTypeHierarchy (base_property) ?? base_property;
				
				@base = @base.BaseType;
			}
			
			return null;
		}
		
		static PropertyInfo TryMatchProperty (Type type, PropertyInfo property)
		{
			foreach (var candidate in type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				if (PropertyMatch (candidate, property))
					return candidate;
			
			return null;
		}
		
		static bool PropertyMatch (PropertyInfo candidate, PropertyInfo property)
		{
			if (candidate.Name != property.Name)
				return false;
			
			if (candidate.CanRead) {
				if (!property.CanRead)
					return false;
				if (!MethodMatch (candidate.GetGetMethod (true), property.GetGetMethod (true)))
					return false;
			} else if (property.CanRead) {
				return false;
			}
			
			if (candidate.CanWrite) {
				if (!property.CanWrite)
					return false;
				if (!MethodMatch (candidate.GetSetMethod (true), property.GetSetMethod (true)))
					return false;
			} else if (property.CanWrite) {
				return false;
			}
			
			return true;
		}

		static bool MethodMatch (MethodInfo candidate, MethodInfo method)
		{
			if (!candidate.IsVirtual)
				return false;
			
			if (candidate.Name != method.Name)
				return false;
			
			if (!TypeMatch (candidate.ReturnType, method.ReturnType))
				return false;

			var cparams = candidate.GetParameters ();
			var mparams = method.GetParameters ();
			if (cparams.Length != mparams.Length)
				return false;
			
			for (int i = 0; i < cparams.Length; i++)
				if (!TypeMatch (cparams [i].ParameterType, mparams [i].ParameterType))
					return false;
			
			return true;
		}

		static bool TypeMatch (Type a, Type b)
		{
			return a == b;
		}

		public IntPtr Register (Type type)
		{
			List<Exception> exceptions = null;
			var objctype = RegisterType (type, ref exceptions);
			if (exceptions != null && exceptions.Count > 0)
				throw new AggregateException (exceptions);
			if (objctype == null)
				return IntPtr.Zero;
			return objctype.Handle;
		}

		public void Register (Type type, ref List<Exception> exceptions)
		{
			RegisterType (type, ref exceptions);
		}

		public string ComputeSignature (MethodInfo method, bool isBlockSignature)
		{
			return base.ComputeSignature (method.DeclaringType, method, isBlockSignature: isBlockSignature);
		}
	}
}

