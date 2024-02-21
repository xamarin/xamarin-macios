using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if !NET
using ObjCRuntime;
using PlatformName = ObjCRuntime.PlatformName;
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

public class AttributeManager {

	readonly Dictionary<System.Type, Type> typeLookup = new ();

	readonly HashSet<string> ignoredAttributes = new () {
		"Microsoft.CodeAnalysis.EmbeddedAttribute",
		"System.Runtime.CompilerServices.NullableAttribute",
		"System.Runtime.CompilerServices.NullableContextAttribute",
		"System.Runtime.CompilerServices.NativeIntegerAttribute",
	};

	TypeCache TypeCache { get; }

	public AttributeManager (TypeCache typeCache)
	{
		TypeCache = typeCache;
	}

	Type LookupReflectionType (string fullname, ICustomAttributeProvider provider)
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
		case "ObjCRuntime.NativeNameAttribute":
			return typeof (ObjCRuntime.NativeNameAttribute);
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
		case "Visibility":
			return typeof (Visibility);
		case "WrapAttribute":
			return typeof (WrapAttribute);
#if NET
		case "System.Runtime.Versioning.SupportedOSPlatformAttribute":
			return typeof (System.Runtime.Versioning.SupportedOSPlatformAttribute);
		case "System.Runtime.Versioning.UnsupportedOSPlatformAttribute":
			return typeof (System.Runtime.Versioning.UnsupportedOSPlatformAttribute);
		case "System.Runtime.Versioning.ObsoletedOSPlatformAttribute":
			return typeof (System.Runtime.Versioning.ObsoletedOSPlatformAttribute);
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
		if (rv is null)
			throw ErrorHelper.CreateError (1055, type.AssemblyQualifiedName);
		return rv;
	}

	// This method gets the IKVM.Reflection.Type for a System.Type.
	Type ConvertTypeToMeta (System.Type type)
	{
		if (!typeLookup.TryGetValue (type, out var rv)) {
			// Brute force: look everywhere.
			// Due to how types move around between assemblies in .NET 5 it gets complicated
			// to figure out which assembly each type comes from, so just look in every assembly.
			// Report a warning if we find the same type in multiple assemblies though.
			var assemblies = TypeCache.Universe.GetAssemblies ();
			foreach (var asm in assemblies) {
				var typeName = type.Name;
				if (type.Namespace is not null)
					typeName = type.Namespace + "." + typeName;
				var lookup = asm.GetType (typeName);
				if (lookup is null)
					continue;
				if (lookup.Assembly != asm) {
					// Apparently looking for type X in assembly A can return type X from assembly B... ignore those.
					continue;
				}
				// we will just throw if we do find a type multiple times but if it was not injected by the compiler.
				if (rv is not null && !ignoredAttributes.Contains (rv.FullName)) {
					ErrorHelper.Warning (1119, /*"Internal error: found the same type ({0}) in multiple assemblies ({1} and {2}). Please file a bug report (https://github.com/xamarin/xamarin-macios/issues/new) with a test case.", */type.FullName, rv.AssemblyQualifiedName, lookup.AssemblyQualifiedName);
					break; // no need to report this more than once
				}
				rv = lookup;
			}
			typeLookup [type] = rv;
		}
		if (rv is null)
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
			return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (PlatformName.MacOSX, 10, 7).Yield ();
		case "MountainLionAttribute":
			return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (PlatformName.MacOSX, 10, 8).Yield ();
		case "MavericksAttribute":
			return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (PlatformName.MacOSX, 10, 9).Yield ();
		case "NoMacAttribute":
			return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (PlatformName.MacOSX).Yield ();
		case "NoiOSAttribute":
			return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (PlatformName.iOS).Yield ();
		case "NoWatchAttribute":
			return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (PlatformName.WatchOS).Yield ();
		case "NoTVAttribute":
			return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (PlatformName.TvOS).Yield ();
		case "NoMacCatalystAttribute":
			return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (PlatformName.MacCatalyst).Yield ();
		case "AvailabilityAttribute":
			return AttributeConversionManager.ConvertAvailability (attribute);
#if NET
		case "SupportedOSPlatformAttribute":
			var sarg = attribute.ConstructorArguments [0].Value as string;
			(var sp, var sv) = ParseOSPlatformAttribute (sarg);
			if (sv is null)
				return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (sp).Yield ();
			else
				return AttributeFactory.CreateNewAttribute<IntroducedAttribute> (sp, sv.Major, sv.Minor).Yield ();
		case "UnsupportedOSPlatformAttribute":
			var uarg = attribute.ConstructorArguments [0].Value as string;
			(var up, var uv) = ParseOSPlatformAttribute (uarg);
			// might have been available for a while...
			if (uv is null)
				return AttributeFactory.CreateNewAttribute<UnavailableAttribute> (up).Yield ();
			else
				return Enumerable.Empty<System.Attribute> ();
		case "ObsoletedOSPlatformAttribute":
			var oarg = attribute.ConstructorArguments [0].Value as string;
			(var op, var ov) = ParseOSPlatformAttribute (oarg);
			// might have been available for a while...
			if (ov is null)
				return AttributeFactory.CreateNewAttribute<ObsoletedAttribute> (op).Yield ();
			else
				return AttributeFactory.CreateNewAttribute<ObsoletedAttribute> (op, ov.Major, ov.Minor).Yield ();
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

	IEnumerable<T> CreateAttributeInstance<T> (CustomAttributeData attribute, ICustomAttributeProvider provider) where T : System.Attribute
	{
		var convertedAttributes = ConvertOldAttributes (attribute);
		if (convertedAttributes.Any ())
			return convertedAttributes.OfType<T> ();

		var expectedType = ConvertTypeToMeta (typeof (T));
		var attributeType = ConvertTypeToMeta (attribute.GetAttributeType ());
		// == when comparing types uses reference equality, which is what we want here.
		if (attributeType != expectedType && !attributeType.IsSubclassOf (expectedType))
			return Enumerable.Empty<T> ();

		System.Type attribType = ConvertTypeFromMeta (attributeType, provider);

		var constructorArguments = new object [attribute.ConstructorArguments.Count];

		for (int i = 0; i < constructorArguments.Length; i++) {
			var value = attribute.ConstructorArguments [i].Value;
			switch (attribute.ConstructorArguments [i].ArgumentType.FullName) {
			case "System.Type":
				if (value is not null) {
					if (attribType.Assembly == typeof (TypeCache).Assembly) {
						constructorArguments [i] = value;
					} else {
						constructorArguments [i] = System.Type.GetType (((Type) value).FullName);
					}
					if (constructorArguments [i] is null)
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
				if (attribType.Assembly == typeof (TypeCache).Assembly) {
					ctorTypes [i] = typeof (Type);
				} else {
					ctorTypes [i] = typeof (System.Type);
				}
				break;
			default:
				ctorTypes [i] = ConvertTypeFromMeta (paramType, provider);
				break;
			}
			if (ctorTypes [i] is null)
				throw ErrorHelper.CreateError (1057, attribType.FullName, i, paramType.FullName);
		}
		var ctor = attribType.GetConstructor (ctorTypes);
		if (ctor is null)
			throw ErrorHelper.CreateError (1058, attribType.FullName);
		var instance = ctor.Invoke (constructorArguments);

		for (int i = 0; i < attribute.NamedArguments.Count; i++) {
			var arg = attribute.NamedArguments [i];
			var value = arg.TypedValue.Value;
			if (arg.TypedValue.ArgumentType == TypeCache.System_String_Array) {
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
		if (attributes is null || attributes.Count == 0)
			return Array.Empty<T> ();

		List<T> list = null;
		for (int i = 0; i < attributes.Count; i++) {

			// special compiler attribtues not usable from C#
			if (ignoredAttributes.Contains (attributes [i].GetAttributeType ().FullName))
				continue;

			foreach (var attrib in CreateAttributeInstance<T> (attributes [i], provider)) {
				if (list is null)
					list = new List<T> ();
				list.Add (attrib);
			}
		}

		if (list is not null)
			return list.ToArray ();

		return Array.Empty<T> ();
	}

	public virtual T [] GetCustomAttributes<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		return FilterAttributes<T> (GetAttributes (provider), provider);
	}

	static IList<CustomAttributeData> GetAttributes (ICustomAttributeProvider provider)
		=> provider switch {
			null => null,
			MemberInfo member => member.GetCustomAttributesData (),
			Assembly assembly => assembly.GetCustomAttributesData (),
			ParameterInfo pinfo => pinfo.GetCustomAttributesData (),
			Module module => module.GetCustomAttributesData (),
			_ => throw new BindingException (1051, true, provider.GetType ().FullName)
		};

	public static bool HasAttribute (ICustomAttributeProvider provider, string type_name)
	{
		var attribs = GetAttributes (provider);
		for (int i = 0; i < attribs.Count; i++)
			if (attribs [i].GetAttributeType ().Name == type_name)
				return true;
		return false;
	}

	public virtual bool HasAttribute<T> (ICustomAttributeProvider provider) where T : Attribute
	{
		var attributeType = ConvertTypeToMeta (typeof (T));
		var attribs = GetAttributes (provider);
		if (attribs is null || attribs.Count == 0)
			return false;

		for (int i = 0; i < attribs.Count; i++) {
			var attrib = attribs [i];
			// == when comparing types uses reference equality, which is what we want here.
			var currentType = ConvertTypeToMeta (attrib.GetAttributeType ());
			if (currentType == attributeType)
				return true;
			if (currentType.IsSubclassOf (attributeType))
				return true;
		}

		return false;
	}

	public virtual T GetCustomAttribute<T> (ICustomAttributeProvider provider) where T : System.Attribute
	{
		if (provider is null)
			return null;
		var rv = GetCustomAttributes<T> (provider);
		if (rv is null || rv.Length == 0)
			return null;

		if (rv.Length == 1)
			return rv [0];

		int code;
		object [] args;
		// each type of provider has its own error. This is because each exception has its own message that 
		// must be correctly translated.
		switch (provider) {
		case ParameterInfo pi:
			code = 1083;
			args = new object [] {
				rv.Length, typeof (T).FullName, $"{pi.Member.DeclaringType.FullName}.{pi.Member.Name}", pi.Position, pi.Name
			};
			break;
		case Type type:
			code = 1084;
			args = new object [] { rv.Length, typeof (T).FullName, type.FullName };
			break;
		case MemberInfo mi:
			code = 1059;
			args = new object [] { rv.Length, typeof (T).FullName, $"{mi.DeclaringType?.FullName}.{mi.Name}" };
			break;
		case Assembly assm:
			code = 1085;
			args = new object [] { rv.Length, typeof (T).FullName, $"{assm.FullName}" };
			break;
		case Module mod:
			code = 1086;
			args = new object [] { rv.Length, typeof (T).FullName, $"{mod.FullyQualifiedName}" };
			break;
		default:
			code = 1059;
			args = new object [] { rv.Length, typeof (T).FullName, provider.ToString () };
			break;
		}
		throw ErrorHelper.CreateError (code, args);
	}

	public virtual bool HasNativeAttribute (ICustomAttributeProvider provider)
	{
		if (provider is null)
			return false;

		return HasAttribute (provider, "NativeIntegerAttribute");
	}

	public virtual bool HasAttribute<T> (ICustomAttributeProvider i, Attribute [] attributes) where T : Attribute
	{
		if (attributes is null)
			return HasAttribute<T> (i);

		foreach (var a in attributes)
			if (a.GetType () == typeof (T))
				return true;
		return false;
	}

}
