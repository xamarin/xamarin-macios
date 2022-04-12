using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if !NET
using ObjCRuntime;
using PlatformName = ObjCRuntime.PlatformName;
#endif

public class AttributeManager
{
	public BindingTouch BindingTouch;
	TypeManager TypeManager { get { return BindingTouch.TypeManager; } }

	public AttributeManager (BindingTouch binding_touch)
	{
		BindingTouch = binding_touch;
	}

	System.Type LookupReflectionType (string fullname, ICustomAttributeProvider provider)
	{
		switch (fullname) {
		case "AbstractAttribute":
			return typeof (AbstractAttribute);
		case "AlignAttribute":
			return typeof (AlignAttribute);
		case "AppearanceAttribute":
			return typeof (AppearanceAttribute);
		case "AsyncAttribute":
			return typeof (AsyncAttribute);
		case "AutoreleaseAttribute":
			return typeof (AutoreleaseAttribute);
		case "BaseTypeAttribute":
			return typeof (BaseTypeAttribute);
		case "BindAttribute":
			return typeof (BindAttribute);
		case "CategoryAttribute":
			return typeof (CategoryAttribute);
		case "CheckDisposedAttribute":
			return typeof (CheckDisposedAttribute);
		case "CoreImageFilterAttribute":
			return typeof (CoreImageFilterAttribute);
		case "CoreImageFilterPropertyAttribute":
			return typeof (CoreImageFilterPropertyAttribute);
		case "DefaultCtorVisibilityAttribute":
			return typeof (DefaultCtorVisibilityAttribute);
		case "DefaultEnumValueAttribute":
			return typeof (DefaultEnumValueAttribute);
		case "DefaultValueAttribute":
			return typeof (DefaultValueAttribute);
		case "DefaultValueFromArgumentAttribute":
			return typeof (DefaultValueFromArgumentAttribute);
		case "DelegateApiNameAttribute":
			return typeof (DelegateApiNameAttribute);
		case "DelegateNameAttribute":
			return typeof (DelegateNameAttribute);
		case "DesignatedInitializerAttribute":
			return typeof (DesignatedInitializerAttribute);
		case "DisableDefaultCtorAttribute":
			return typeof (DisableDefaultCtorAttribute);
		case "DisposeAttribute":
			return typeof (DisposeAttribute);
		case "ErrorDomainAttribute":
			return typeof (ErrorDomainAttribute);
		case "EventArgsAttribute":
			return typeof (EventArgsAttribute);
		case "EventNameAttribute":
			return typeof (EventNameAttribute);
		case "ForcedTypeAttribute":
			return typeof (ForcedTypeAttribute);
		case "Foundation.AdviceAttribute":
			return typeof (Foundation.AdviceAttribute);
		case "Foundation.ExportAttribute":
			return typeof (Foundation.ExportAttribute);
		case "Foundation.FieldAttribute":
			return typeof (Foundation.FieldAttribute);
		case "Foundation.ModelAttribute":
			return typeof (Foundation.ModelAttribute);
		case "Foundation.NotImplementedAttribute":
			return typeof (Foundation.NotImplementedAttribute);
		case "Foundation.PreserveAttribute":
			return typeof (Foundation.PreserveAttribute);
		case "Foundation.ProtocolAttribute":
			return typeof (Foundation.ProtocolAttribute);
		case "Foundation.RegisterAttribute":
			return typeof (Foundation.RegisterAttribute);
		case "IgnoredInDelegateAttribute":
			return typeof (IgnoredInDelegateAttribute);
		case "InternalAttribute":
			return typeof (InternalAttribute);
		case "ManualAttribute":
			return typeof (ManualAttribute);
		case "MarshalDirectiveAttribute":
			return typeof (MarshalDirectiveAttribute);
		case "MarshalNativeExceptionsAttribute":
			return typeof (MarshalNativeExceptionsAttribute);
		case "NewAttribute":
			return typeof (NewAttribute);
		case "NoDefaultValueAttribute":
			return typeof (NoDefaultValueAttribute);
		case "NoMethodAttribute":
			return typeof (NoMethodAttribute);
		case "NotificationAttribute":
			return typeof (NotificationAttribute);
		case "NullAllowedAttribute":
			return typeof (NullAllowedAttribute);
		case "ObjCRuntime.ArgumentSemantic":
			return typeof (ObjCRuntime.ArgumentSemantic);
		case "ObjCRuntime.BindAsAttribute":
			return typeof (ObjCRuntime.BindAsAttribute);
		case "ObjCRuntime.BindingImplAttribute":
			return typeof (ObjCRuntime.BindingImplAttribute);
		case "ObjCRuntime.BindingImplOptions":
			return typeof (ObjCRuntime.BindingImplOptions);
#if NET
		case "DeprecatedAttribute":
			return typeof (DeprecatedAttribute);
#else
		case "ObjCRuntime.DeprecatedAttribute":
			return typeof (ObjCRuntime.DeprecatedAttribute);
#endif
#if NET
		case "IntroducedAttribute":
			return typeof (IntroducedAttribute);
#else
		case "ObjCRuntime.IntroducedAttribute":
			return typeof (ObjCRuntime.IntroducedAttribute);
#endif
		case "ObjCRuntime.NativeAttribute":
			return typeof (ObjCRuntime.NativeAttribute);
#if NET
		case "ObsoletedAttribute":
			return typeof (ObsoletedAttribute);
#else
		case "ObjCRuntime.ObsoletedAttribute":
			return typeof (ObjCRuntime.ObsoletedAttribute);
#endif
#if !NET
		case "ObjCRuntime.PlatformArchitecture":
			return typeof (ObjCRuntime.PlatformArchitecture);
#endif
#if NET
		case "PlatformName":
			return typeof (PlatformName);
#else
		case "ObjCRuntime.PlatformName":
			return typeof (ObjCRuntime.PlatformName);
#endif
		case "ObjCRuntime.RequiresSuperAttribute":
			return typeof (ObjCRuntime.RequiresSuperAttribute);
#if NET
		case "UnavailableAttribute":
			return typeof (UnavailableAttribute);
#else
		case "ObjCRuntime.UnavailableAttribute":
			return typeof (ObjCRuntime.UnavailableAttribute);
#endif
		case "OptionalImplementationAttribute":
			return typeof (OptionalImplementationAttribute);
		case "OverrideAttribute":
			return typeof (OverrideAttribute);
		case "PostGetAttribute":
			return typeof (PostGetAttribute);
		case "PrologueSnippetAttribute":
			return typeof (PrologueSnippetAttribute);
		case "PostSnippetAttribute":
			return typeof (PostSnippetAttribute);
		case "PreSnippetAttribute":
			return typeof (PreSnippetAttribute);
		case "PrivateDefaultCtorAttribute":
			return typeof (PrivateDefaultCtorAttribute);
		case "ProtectedAttribute":
			return typeof (ProtectedAttribute);
		case "ProtocolizeAttribute":
			return typeof (ProtocolizeAttribute);
		case "SealedAttribute":
			return typeof (SealedAttribute);
		case "StaticAttribute":
			return typeof (StaticAttribute);
		case "StrongDictionaryAttribute":
			return typeof (StrongDictionaryAttribute);
		case "System.Boolean":
			return typeof (System.Boolean);
		case "System.ComponentModel.EditorBrowsableAttribute":
			return typeof (System.ComponentModel.EditorBrowsableAttribute);
		case "System.ComponentModel.EditorBrowsableState":
			return typeof (System.ComponentModel.EditorBrowsableState);
		case "System.Diagnostics.DebuggerBrowsableAttribute":
			return typeof (System.Diagnostics.DebuggerBrowsableAttribute);
		case "System.Diagnostics.DebuggerBrowsableState":
			return typeof (System.Diagnostics.DebuggerBrowsableState);
		case "System.Int32":
			return typeof (System.Int32);
		case "System.Object":
			return typeof (System.Object);
		case "System.ObsoleteAttribute":
			return typeof (System.ObsoleteAttribute);
		case "System.Runtime.InteropServices.FieldOffsetAttribute":
			return typeof (System.Runtime.InteropServices.FieldOffsetAttribute);
		case "System.Runtime.InteropServices.MarshalAsAttribute":
			return typeof (System.Runtime.InteropServices.MarshalAsAttribute);
		case "System.Runtime.InteropServices.UnmanagedType":
			return typeof (System.Runtime.InteropServices.UnmanagedType);
		case "System.String":
			return typeof (System.String);
		case "ThreadSafeAttribute":
			return typeof (ThreadSafeAttribute);
		case "TransientAttribute":
			return typeof (TransientAttribute);
		case "UnifiedInternalAttribute":
			return typeof (UnifiedInternalAttribute);
		case "Visibility":
			return typeof (Visibility);
		case "WrapAttribute":
			return typeof (WrapAttribute);
#if NET
		case "System.Runtime.Versioning.SupportedOSPlatformAttribute":
			return typeof (System.Runtime.Versioning.SupportedOSPlatformAttribute);
		case "System.Runtime.Versioning.UnsupportedOSPlatformAttribute":
			return typeof (System.Runtime.Versioning.UnsupportedOSPlatformAttribute);
#endif
		}

		switch (fullname) {
		case "ObjCRuntime.iOSAttribute":
		case "ObjCRuntime.LionAttribute":
		case "ObjCRuntime.AvailabilityAttribute":
		case "ObjCRuntime.MacAttribute":
		case "ObjCRuntime.SinceAttribute":
		case "ObjCRuntime.MountainLionAttribute":
		case "ObjCRuntime.MavericksAttribute":
			throw ErrorHelper.CreateError (1061, fullname, Generator.FormatProvider (provider));
		}

		return null;
	}

	// This method gets the System.Type for a IKVM.Reflection.Type to a System.Type.
	System.Type ConvertTypeFromMeta (Type type, ICustomAttributeProvider provider)
	{
		var rv = LookupReflectionType (type.FullName, provider);
		if (rv == null)
			throw ErrorHelper.CreateError (1055, type.AssemblyQualifiedName);
		return rv;
	}

	// This method gets the IKVM.Reflection.Type for a System.Type.
	Type ConvertTypeToMeta (System.Type type, ICustomAttributeProvider provider)
	{
		var ikvm_type_lookup = BindingTouch.IKVMTypeLookup;

		if (!ikvm_type_lookup.TryGetValue (type, out var rv)) {
			// Brute force: look everywhere.
			// Due to how types move around between assemblies in .NET 5 it gets complicated
			// to figure out which assembly each type comes from, so just look in every assembly.
			// Report a warning if we find the same type in multiple assemblies though.
			var assemblies = BindingTouch.universe.GetAssemblies ();
			foreach (var asm in assemblies) {
				var lookup = asm.GetType (type.Namespace + "." + type.Name);
				if (lookup == null)
					continue;
				if (lookup.Assembly != asm) {
					// Apparently looking for type X in assembly A can return type X from assembly B... ignore those.
					continue;
				}
				if (rv != null) {
					ErrorHelper.Warning (1119, /*"Internal error: found the same type ({0}) in multiple assemblies ({1} and {2}). Please file a bug report (https://github.com/xamarin/xamarin-macios/issues/new) with a test case.", */type.FullName, rv.AssemblyQualifiedName, lookup.AssemblyQualifiedName);
					break; // no need to report this more than once
				}
				rv = lookup;
			}
			ikvm_type_lookup [type] = rv;
		}
		if (rv == null)
			throw ErrorHelper.CreateError (1055, type.AssemblyQualifiedName);
		return rv;
	}


	static IEnumerable<System.Attribute> ConvertOldAttributes (CustomAttributeData attribute)
	{
 		switch (attribute.GetAttributeType ().Namespace) {
		case null: // Root namespace such as PlatformAvailabilityShadow.cs
		case "MonoTouch.ObjCRuntime":
		case "ObjCRuntime":
#if NET
		case "System.Runtime.Versioning":
#endif
			break;
		default:
			return Enumerable.Empty<System.Attribute> ();
		}

		switch (attribute.GetAttributeType ().Name) {
		case "SinceAttribute":
		case "iOSAttribute":
			return AttributeConversionManager.ConvertPlatformAttribute (attribute, PlatformName.iOS).Yield ();
		case "MacAttribute":
			return AttributeConversionManager.ConvertPlatformAttribute (attribute, PlatformName.MacOSX).Yield ();
		case "WatchAttribute":
			return AttributeConversionManager.ConvertPlatformAttribute (attribute, PlatformName.WatchOS).Yield ();
		case "TVAttribute":
			return AttributeConversionManager.ConvertPlatformAttribute (attribute, PlatformName.TvOS).Yield ();
		case "MacCatalystAttribute":
			return AttributeConversionManager.ConvertPlatformAttribute (attribute, PlatformName.MacCatalyst).Yield ();
		case "LionAttribute":
			return AttributeFactory.CreateNewIntroducedAttribute (PlatformName.MacOSX, 10, 7).Yield ();
		case "MountainLionAttribute":
			return AttributeFactory.CreateNewIntroducedAttribute (PlatformName.MacOSX, 10, 8).Yield ();
		case "MavericksAttribute":
			return AttributeFactory.CreateNewIntroducedAttribute (PlatformName.MacOSX, 10, 9).Yield ();
		case "NoMacAttribute":
			return AttributeFactory.CreateUnavailableAttribute (PlatformName.MacOSX).Yield ();
		case "NoiOSAttribute":
			return AttributeFactory.CreateUnavailableAttribute (PlatformName.iOS).Yield ();
		case "NoWatchAttribute":
			return AttributeFactory.CreateUnavailableAttribute (PlatformName.WatchOS).Yield ();
		case "NoTVAttribute":
			return AttributeFactory.CreateUnavailableAttribute (PlatformName.TvOS).Yield ();
		case "NoMacCatalystAttribute":
			return AttributeFactory.CreateUnavailableAttribute (PlatformName.MacCatalyst).Yield ();
		case "AvailabilityAttribute":
			return AttributeConversionManager.ConvertAvailability (attribute);
#if NET
		case "SupportedOSPlatformAttribute":
			var sarg = attribute.ConstructorArguments [0].Value as string;
			(var sp, var sv) = ParseOSPlatformAttribute (sarg);
			if (sv is null)
				return AttributeFactory.CreateNewUnspecifiedIntroducedAttribute (sp).Yield ();
			else
				return AttributeFactory.CreateNewIntroducedAttribute (sp, sv.Major, sv.Minor).Yield ();
		case "UnsupportedOSPlatformAttribute":
			var uarg = attribute.ConstructorArguments [0].Value as string;
			(var up, var uv) = ParseOSPlatformAttribute (uarg);
			// might have been available for a while...
			if (uv == null)
				return AttributeFactory.CreateUnavailableAttribute (up).Yield ();
			else
				return Enumerable.Empty<System.Attribute> ();
#endif
		default:
			return Enumerable.Empty<System.Attribute> ();
		}
	}

#if NET
	static (PlatformName, Version) ParseOSPlatformAttribute (string arg)
	{
		PlatformName name;
		int len;
		switch (arg) {
		case string s when s.StartsWith ("ios", StringComparison.Ordinal):
			name = PlatformName.iOS;
			len = "ios".Length;
			break;
		case string s when s.StartsWith ("tvos", StringComparison.Ordinal):
			name = PlatformName.TvOS;
			len = "tvos".Length;
			break;
		case string s when s.StartsWith ("watchos", StringComparison.Ordinal):
			name = PlatformName.WatchOS;
			len = "watchos".Length;
			break;
		case string s when s.StartsWith ("macos", StringComparison.Ordinal):
			name = PlatformName.MacOSX;
			len = "macos".Length;
			break;
		case string s when s.StartsWith ("maccatalyst", StringComparison.Ordinal):
			name = PlatformName.MacCatalyst;
			len = "maccatalyst".Length;
			break;
		default:
			throw new BindingException (1047, arg);
		}

		Version version = null;
		if (arg.Length > len) {
			if (!Version.TryParse (arg [len..], out version))
				throw new BindingException (1047, arg);
		}
		return (name, version);
	}
#endif

	IEnumerable<T> CreateAttributeInstance <T> (CustomAttributeData attribute, ICustomAttributeProvider provider) where T : System.Attribute
	{
		var convertedAttributes = ConvertOldAttributes (attribute);
		if (convertedAttributes.Any ())
			return convertedAttributes.OfType<T> ();

		var expectedType = ConvertTypeToMeta (typeof (T), provider);
		if (attribute.GetAttributeType () != expectedType && !IsSubclassOf (expectedType, attribute.GetAttributeType ()))
			return Enumerable.Empty<T> ();

		System.Type attribType = ConvertTypeFromMeta (attribute.GetAttributeType (), provider);

		var constructorArguments = new object [attribute.ConstructorArguments.Count];

		for (int i = 0; i < constructorArguments.Length; i++) {
			var value = attribute.ConstructorArguments [i].Value;
			switch (attribute.ConstructorArguments [i].ArgumentType.FullName) {
			case "System.Type":
				if (value != null) {
					if (attribType.Assembly == typeof (TypeManager).Assembly) {
						constructorArguments [i] = value;
					} else {
						constructorArguments [i] = System.Type.GetType (((Type) value).FullName);
					}
					if (constructorArguments [i] == null)
						throw ErrorHelper.CreateError (1056, attribType.FullName, i + 1);
				}
				break;
			default:
				constructorArguments [i] = value;
				break;
			}
		}

		var parameters = attribute.Constructor.GetParameters ();
		var ctorTypes = new System.Type [parameters.Length];
		for (int i = 0; i < ctorTypes.Length; i++) {
			var paramType = parameters [i].ParameterType;
			switch (paramType.FullName) {
			case "System.Type":
				if (attribType.Assembly == typeof (TypeManager).Assembly) {
					ctorTypes [i] = typeof (Type);
				} else {
					ctorTypes [i] = typeof (System.Type);
				}
				break;
			default:
				ctorTypes [i] = ConvertTypeFromMeta (paramType, provider);
				break;
			}
			if (ctorTypes [i] == null)
				throw ErrorHelper.CreateError (1057, attribType.FullName, i, paramType.FullName);
		}
		var ctor = attribType.GetConstructor (ctorTypes);
		if (ctor == null)
			throw ErrorHelper.CreateError (1058, attribType.FullName);
		var instance = ctor.Invoke (constructorArguments);

		for (int i = 0; i < attribute.NamedArguments.Count; i++) {
			var arg = attribute.NamedArguments [i];
			var value = arg.TypedValue.Value;
			if (arg.TypedValue.ArgumentType == TypeManager.System_String_Array) {
				var typed_values = ((IEnumerable<CustomAttributeTypedArgument>) arg.TypedValue.Value).ToArray ();
				var arr = new string [typed_values.Length];
				for (int a = 0; a < arr.Length; a++)
					arr [a] = (string) typed_values [a].Value;
				value = arr;
			} else if (arg.TypedValue.ArgumentType.FullName == "System.Type[]") {
				var typed_values = ((IEnumerable<CustomAttributeTypedArgument>) arg.TypedValue.Value).ToArray ();
				var arr = new Type [typed_values.Length];
				for (int a = 0; a < arr.Length; a++)
					arr [a] = (Type) typed_values [a].Value;
				value = arr;
			} else if (arg.TypedValue.ArgumentType.IsArray) {
				throw ErrorHelper.CreateError (1073, attribType.FullName, i + 1, arg.MemberName);
			}
			if (arg.IsField) {
				attribType.GetField (arg.MemberName).SetValue (instance, value);
			} else {
				attribType.GetProperty (arg.MemberName).SetValue (instance, value, new object [0]);
			}
		}

		return ((T) instance).Yield ();
	}

	T [] FilterAttributes<T> (IList<CustomAttributeData> attributes, ICustomAttributeProvider provider) where T : System.Attribute
	{
		if (attributes == null || attributes.Count == 0)
			return Array.Empty<T> ();

		List<T> list = null;
		for (int i = 0; i < attributes.Count; i++) {

			// special compiler attribtues not usable from C#
			switch (attributes [i].GetAttributeType ().FullName) {
			case "System.Runtime.CompilerServices.NullableAttribute":
			case "System.Runtime.CompilerServices.NullableContextAttribute":
			case "System.Runtime.CompilerServices.NativeIntegerAttribute":
				continue;
			}

			foreach (var attrib in CreateAttributeInstance<T> (attributes [i], provider)) {
				if (list == null)
					list = new List<T> ();
				list.Add (attrib);
			}
		}

		if (list != null)
			return list.ToArray ();

		return Array.Empty<T> ();
	}

	public T [] GetCustomAttributes<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		return FilterAttributes<T> (GetIKVMAttributes (provider), provider);
	}

	static IList<CustomAttributeData> GetIKVMAttributes (ICustomAttributeProvider provider)
	{
		if (provider == null)
			return null;
		if (provider is MemberInfo member)
			return member.GetCustomAttributesData ();
		if (provider is Assembly assembly)
			return assembly.GetCustomAttributesData ();
		if (provider is ParameterInfo pinfo)
			return pinfo.GetCustomAttributesData ();
		if (provider is Module module)
			return module.GetCustomAttributesData ();
		throw new BindingException (1051, true, provider.GetType ().FullName);
	}

	public static bool HasAttribute (ICustomAttributeProvider provider, string type_name)
	{
		var attribs = GetIKVMAttributes (provider);
		for (int i = 0; i < attribs.Count; i++)
			if (attribs [i].GetAttributeType ().Name == type_name)
				return true;
		return false;
	}

	public bool HasAttribute<T> (ICustomAttributeProvider provider) where T : Attribute
	{
		var attribute_type = ConvertTypeToMeta (typeof (T), provider);
		var attribs = GetIKVMAttributes (provider);
		if (attribs == null || attribs.Count == 0)
			return false;

		for (int i = 0; i < attribs.Count; i++) {
			var attrib = attribs [i];
			if (attrib.GetAttributeType () == attribute_type)
				return true;
			if (IsSubclassOf (attribute_type, attrib.GetAttributeType ()))
				return true;
		}

		return false;
	}

	public T GetCustomAttribute<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		if (provider is null)
			return null;
		var rv = GetCustomAttributes<T> (provider);
		if (rv == null || rv.Length == 0)
			return null;

		if (rv.Length == 1)
			return rv [0];

		string name = (provider as MemberInfo)?.Name;
		if (provider is ParameterInfo) {
			var pi = (ParameterInfo) provider;
			name = $"the method {pi.Member.DeclaringType.FullName}.{pi.Member.Name}'s parameter #{pi.Position} ({pi.Name})";
		} else if (provider is Type type) {
			name = $"the type {type.FullName}";
		} else if (provider is MemberInfo) {
			var mi = (MemberInfo) provider;
			name = $"the member {mi.DeclaringType.FullName}.{mi.Name}";
		} else if (provider is Assembly) {
			name = $"the assembly {((Assembly) provider).FullName}";
		} else if (provider is Module) {
			name = $"the module {((Module) provider).FullyQualifiedName}";
		} else {
			name = $"the member {provider.ToString ()}";
		}
		throw ErrorHelper.CreateError (1059, rv.Length, typeof (T).FullName, name);
	}

	public static ICustomAttributeProvider GetReturnTypeCustomAttributes (MethodInfo method)
	{
		return method.ReturnParameter;
	}

	static bool IsSubclassOf (Type base_class, Type derived_class)
	{
		return derived_class.IsSubclassOf (base_class);
	}
}

public static class AttributeConversionManager
{
	public static System.Attribute ConvertPlatformAttribute (CustomAttributeData attribute, PlatformName platform)
	{
		var constructorArguments = new object [attribute.ConstructorArguments.Count];
		for (int i = 0; i < attribute.ConstructorArguments.Count; ++i)
			constructorArguments [i] = attribute.ConstructorArguments [i].Value;

		Func<string> createErrorMessage = () => {
			var b = new System.Text.StringBuilder (" Types { ");
			for (int i = 0; i < constructorArguments.Length; ++i)
				b.Append (constructorArguments[i].GetType ().ToString () + " ");
			b.Append ("}");
			return b.ToString ();
		};

		Func<string> unknownFormatError = () => $"Unknown format for old style availability attribute {attribute.GetAttributeType ().FullName} {attribute.ConstructorArguments.Count} {createErrorMessage ()}";

		object [] ctorValues;
		System.Type [] ctorTypes;

		switch (attribute.ConstructorArguments.Count) {
		case 2:
			if (constructorArguments [0].GetType () == typeof (byte) &&
			    constructorArguments [1].GetType () == typeof (byte)) {
#if NET
				ctorValues = new object [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (string) };
#else
				ctorValues = new object [] { (byte)platform, (int)(byte)constructorArguments [0], (int)(byte)constructorArguments [1], (byte)0xff, null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
#endif
				break;
			}
			throw new NotImplementedException (unknownFormatError ());
		case 3:
			if (constructorArguments [0].GetType () == typeof (byte) &&
			    constructorArguments [1].GetType () == typeof (byte) &&
			    constructorArguments [2].GetType () == typeof (byte)) {
#if NET
				ctorValues = new object [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), typeof (string) };
#else
				ctorValues = new object [] { (byte) platform, (int)(byte)constructorArguments [0], (int)(byte)constructorArguments [1], (int)(byte)constructorArguments [2], (byte) 0xff, null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
#endif
				break;
			}
#if !NET
			if (constructorArguments [0].GetType () == typeof (byte) &&
			    constructorArguments [1].GetType () == typeof (byte) &&
			    constructorArguments [2].GetType () == typeof (bool)) {
				byte arch = (bool) constructorArguments [2] ? (byte) 2 : (byte) 0xff;
				ctorValues = new object [] { (byte)platform, (int)(byte) constructorArguments [0], (int)(byte)constructorArguments [1], arch, null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
				break;
			}
#endif
			throw new NotImplementedException (unknownFormatError ());
#if !NET
		case 4:
			if (constructorArguments [0].GetType () == typeof (byte) &&
			    constructorArguments [1].GetType () == typeof (byte) &&
			    constructorArguments [2].GetType () == typeof (byte) &&
			    constructorArguments [3].GetType () == typeof (bool)) {
				byte arch = (bool) constructorArguments [3] ? (byte) 2 : (byte) 0xff;
				ctorValues = new object [] { (byte) platform, (int) (byte) constructorArguments [0], (int)(byte) constructorArguments [1], (int)(byte) constructorArguments [2], arch, null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
				break;
			}
			if (constructorArguments [0].GetType () == typeof (byte) &&
			    constructorArguments [1].GetType () == typeof (byte) &&
			    constructorArguments [2].GetType () == typeof (byte) &&
			    constructorArguments [3].GetType () == typeof (byte) /* ObjCRuntime.PlatformArchitecture */) {
				ctorValues = new object [] { (byte) platform, (int) (byte) constructorArguments [0], (int)(byte) constructorArguments [1], (int)(byte) constructorArguments [2], constructorArguments [3], null };
				ctorTypes = new System.Type [] { AttributeFactory.PlatformEnum, typeof (int), typeof (int), typeof (int), AttributeFactory.PlatformArch, typeof (string) };
				break;
			}

			throw new NotImplementedException (unknownFormatError ());
#endif
		default:
			throw new NotImplementedException ($"Unknown count {attribute.ConstructorArguments.Count} {createErrorMessage ()}");
		}

		return AttributeFactory.CreateNewAttribute (AttributeFactory.IntroducedAttributeType, ctorTypes, ctorValues);
	}

	struct ParsedAvailabilityInfo
	{
		public PlatformName Platform;
		public int Major;
		public int Minor;

		public ParsedAvailabilityInfo (PlatformName platform, int major, int minor)
		{
			Platform = platform;
			Major = major;
			Minor = minor;
		}

		public ParsedAvailabilityInfo (PlatformName platform)
		{
			Platform = platform;
			Major = -1;
			Minor = -1;
		}
	}

	static PlatformName ParsePlatforName (string s)
	{
		switch (s) {
		case "iOS":
			return PlatformName.iOS;
		case "Mac":
			return PlatformName.MacOSX;
		case "Watch":
			return PlatformName.WatchOS;
		case "TV":
			return PlatformName.TvOS;
		default:
			return PlatformName.None;
		}
	}

	static ParsedAvailabilityInfo DetermineOldAvailabilityVersion (CustomAttributeNamedArgument arg)
	{
		string enumName = Enum.GetName (typeof (Platform), (ulong) arg.TypedValue.Value);
		if (enumName == null)
			throw new NotImplementedException ($"Unknown version format \"{enumName}\" in DetermineOldAvailabilityVersion. Are there two values | togeather?");

		string [] enumParts = enumName.Split (new char [] { '_' });
		switch (enumParts.Count ())
		{
		case 1:
			if (enumName == "None")
				return new ParsedAvailabilityInfo (PlatformName.None);
			break;
		case 2: {
			if (enumParts [1] != "Version")
				break;

			PlatformName platform = ParsePlatforName (enumParts [0]);
			if (platform == PlatformName.None)
				break;
			return new ParsedAvailabilityInfo (platform);
		}
		case 3: {
			PlatformName platform = ParsePlatforName (enumParts [0]);
			if (platform == PlatformName.None)
				break;
			int major = int.Parse (enumParts [1]);
			int minor = int.Parse (enumParts [2]);

			return new ParsedAvailabilityInfo (platform, major, minor);
		}
		}
		throw new NotImplementedException ($"Unknown version format \"{enumName}\" in DetermineOldAvailabilityVersion");
	}



	public static IEnumerable<System.Attribute> ConvertAvailability (CustomAttributeData attribute)
	{
		string message = null;
		if (attribute.NamedArguments.Any (x => x.MemberName == "Message"))
			message = (string)attribute.NamedArguments.First (x => x.MemberName == "Message").TypedValue.Value;

		foreach (var arg in attribute.NamedArguments) {
			switch (arg.MemberName) {
			case "Introduced": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateNewIntroducedAttribute (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Deprecated": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateDeprecatedAttribute (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Obsoleted": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateObsoletedAttribute (availInfo.Platform, availInfo.Major, availInfo.Minor, message: message);
				continue;
			}
			case "Unavailable": {
				ParsedAvailabilityInfo availInfo = DetermineOldAvailabilityVersion (arg);
				yield return AttributeFactory.CreateUnavailableAttribute (availInfo.Platform, message: message);
				continue;
			}
			case "Message":
				continue;
			default:
				throw new NotImplementedException ($"ConvertAvailability found unknown named argument {arg.MemberName}");
			}
		}
	}
}


static class AttributeFactory
{
	public static System.Type PlatformEnum = typeof (PlatformName);
#if !NET
	public static System.Type PlatformArch = typeof (PlatformArchitecture);
#endif

	public static System.Type IntroducedAttributeType = typeof (IntroducedAttribute);
	public static System.Type UnavailableAttributeType = typeof (UnavailableAttribute);
	public static System.Type ObsoletedAttributeType = typeof (ObsoletedAttribute);
	public static System.Type DeprecatedAttributeType = typeof (DeprecatedAttribute);

	public static System.Attribute CreateNewAttribute (System.Type attribType, System.Type [] ctorTypes, object [] ctorValues)
	{
		var ctor = attribType.GetConstructor (ctorTypes);
		if (ctor == null)
			throw ErrorHelper.CreateError (1058, attribType.FullName);

		return (System.Attribute) ctor.Invoke (ctorValues);
	}

	static Attribute CreateMajorMinorAttribute (System.Type type, PlatformName platform, int major, int minor, string message)
	{
#if NET
		var ctorValues = new object [] { (byte) platform, major, minor, message };
		var ctorTypes = new System.Type [] { PlatformEnum, typeof (int), typeof (int), typeof (string) };
#else
		var ctorValues = new object [] { (byte)platform, major, minor, (byte) 0xff, message };
		var ctorTypes = new System.Type [] { PlatformEnum, typeof (int), typeof (int), PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (type, ctorTypes, ctorValues);
	}

	static Attribute CreateUnspecifiedAttribute (System.Type type, PlatformName platform, string message)
	{
#if NET
		var ctorValues = new object [] { (byte) platform, message };
		var ctorTypes = new System.Type [] { PlatformEnum, typeof (string) };
#else
		var ctorValues = new object [] { (byte)platform, (byte) 0xff, message };
		var ctorTypes = new System.Type [] { PlatformEnum, PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (type, ctorTypes, ctorValues);
	}

	public static System.Attribute CreateNewIntroducedAttribute (PlatformName platform, int major, int minor, string message = null)
	{
		return CreateMajorMinorAttribute (IntroducedAttributeType, platform, major, minor, message);
	}

	public static System.Attribute CreateNewUnspecifiedIntroducedAttribute (PlatformName platform, string message = null)
	{
		return CreateUnspecifiedAttribute (IntroducedAttributeType, platform, message);
	}

	public static System.Attribute CreateObsoletedAttribute (PlatformName platform, int major, int minor, string message = null)
	{
		return CreateMajorMinorAttribute (ObsoletedAttributeType, platform, major, minor, message);
	}

	public static System.Attribute CreateDeprecatedAttribute (PlatformName platform, int major, int minor, string message = null)
	{
		return CreateMajorMinorAttribute (DeprecatedAttributeType, platform, major, minor, message);
	}

	public static System.Attribute CreateUnavailableAttribute (PlatformName platformName, string message = null)
	{
#if NET
		var ctorValues = new object [] { (byte)platformName, message };
		var ctorTypes = new System.Type [] { PlatformEnum, typeof (string) };
#else
		var ctorValues = new object [] { (byte)platformName, (byte) 0xff, message };
		var ctorTypes = new System.Type [] { PlatformEnum, PlatformArch, typeof (string) };
#endif
		return CreateNewAttribute (UnavailableAttributeType, ctorTypes, ctorValues);
	}
}

public static class EnumerableExtensions
{
	public static IEnumerable<T> Yield<T> (this T item)
	{
		yield return item;
	}
}

public static class CustomAttributeDataExtensions
{
#if !NET
	static Type roCustomAttributeDataType;
	static PropertyInfo attributeTypeProperty;

	static CustomAttributeDataExtensions ()
	{
		roCustomAttributeDataType = typeof(MetadataLoadContext).Assembly.GetType ("System.Reflection.TypeLoading.RoCustomAttributeData");
		attributeTypeProperty = roCustomAttributeDataType.GetProperty ("AttributeType");
	}

	public static Type GetAttributeType (this CustomAttributeData data)
	{
		// Workaround for CustomAttributeData.AttributeType not being declared as virtual in Mono
		if (data.GetType ().IsSubclassOf (roCustomAttributeDataType))
			return (Type) attributeTypeProperty.GetValue (data);
		return data.AttributeType;
	}
#else
	public static Type GetAttributeType (this CustomAttributeData data) => data.AttributeType;
#endif
}
