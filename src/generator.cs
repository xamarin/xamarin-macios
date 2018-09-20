//
// This is the binding generator for the MonoTouch API, it uses the
// contract in API.cs to generate the binding.
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011-2015 Xamarin, Inc.
//
//
// This generator produces various */*.g.cs files based on the
// interface-based type description on this file, see the 
// embedded `MonoTouch.UIKit' namespace here for an example
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// TODO:
//   * Add support for wrapping "ref" and "out" NSObjects (WrappedTypes)
//     Typically this is necessary for things like NSError.
//

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;
using System.Text;
using System.ComponentModel;

using ObjCRuntime;
using Foundation;
using Xamarin.Utils;

public static class GeneratorExtensions
{
	public static StreamWriter Write (this StreamWriter sw, char c, int count)
	{
		for (int i = 0; i < count; i++)
			sw.Write (c);
		return sw;
	}
}

public static class ReflectionExtensions {
	public static BaseTypeAttribute GetBaseTypeAttribute (Type type)
	{
		return AttributeManager.GetCustomAttribute<BaseTypeAttribute> (type);
	}

	public static Type GetBaseType (Type type)
	{
		BaseTypeAttribute bta = GetBaseTypeAttribute (type);
		Type base_type = bta != null ?  bta.BaseType : TypeManager.System_Object;

		return base_type;
	}

	public static List <PropertyInfo> GatherProperties (this Type type) {
		return type.GatherProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
	}

	//
	// Returs true if the specified method info or property info is not
	// available in the current platform (because it has the attribute
	// [Unavailable (ThisPlatform) or becasue the shorthand versions
	// of [NoiOS] or [NoMac] are applied.
	//
	// This needs to merge, because we might have multiple attributes in
	// use, for example, the availability (iOS (6,0)) and the fact that this
	// is not available on Mac (NoMac).
	//
	public static bool IsUnavailable (this ICustomAttributeProvider provider)
	{
		return AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (provider)
			.Any (attr => attr.AvailabilityKind == AvailabilityKind.Unavailable &&
				attr.Platform == Generator.CurrentPlatform);
	}
	
	public static AvailabilityBaseAttribute GetAvailability (this ICustomAttributeProvider attrProvider, AvailabilityKind availabilityKind)
	{
		return AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (attrProvider)
			.FirstOrDefault (attr =>
				attr.AvailabilityKind == availabilityKind &&
					attr.Platform == Generator.CurrentPlatform
			);
	}

	public static List <PropertyInfo> GatherProperties (this Type type, BindingFlags flags) {
		List <PropertyInfo> properties = new List <PropertyInfo> (type.GetProperties (flags));

		if (Generator.IsPublicMode)
			return properties;

		Type parent_type = GetBaseType (type);
		string owrap;
		string nwrap;

		if (parent_type != TypeManager.NSObject) {
			if (AttributeManager.HasAttribute<ModelAttribute> (parent_type)) {
				foreach (PropertyInfo pinfo in parent_type.GetProperties (flags)) {
					bool toadd = true;
					var modelea = Generator.GetExportAttribute (pinfo, out nwrap);

					if (modelea == null)
						continue;

					foreach (PropertyInfo exists in properties) {
						var origea = Generator.GetExportAttribute (exists, out owrap);
						if (origea.Selector == modelea.Selector)
							toadd = false;
					}

					if (toadd)
						properties.Add (pinfo);
				}
			}
		}

		return properties;
	}

	public static List <MethodInfo> GatherMethods (this Type type) {
		return type.GatherMethods (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
	}

	public static bool IsInternal (this MemberInfo mi)
	{
		return AttributeManager.HasAttribute<InternalAttribute> (mi)
			|| (Generator.UnifiedAPI && AttributeManager.HasAttribute<UnifiedInternalAttribute> (mi));
	}

	public static bool IsUnifiedInternal (this MemberInfo mi)
	{
		return (Generator.UnifiedAPI && AttributeManager.HasAttribute<UnifiedInternalAttribute> (mi));
	}

	public static bool IsInternal (this PropertyInfo pi)
	{
		return AttributeManager.HasAttribute<InternalAttribute> (pi)
			|| (Generator.UnifiedAPI && AttributeManager.HasAttribute<UnifiedInternalAttribute> (pi));
	}

	public static bool IsInternal (this Type type)
	{
		return AttributeManager.HasAttribute<InternalAttribute> (type)
			|| (Generator.UnifiedAPI && AttributeManager.HasAttribute<UnifiedInternalAttribute> (type));
	}
	
	public static List <MethodInfo> GatherMethods (this Type type, BindingFlags flags) {
		List <MethodInfo> methods = new List <MethodInfo> (type.GetMethods (flags));

		if (Generator.IsPublicMode)
			return methods;

		Type parent_type = GetBaseType (type);

		if (parent_type != TypeManager.NSObject) {
			if (AttributeManager.HasAttribute<ModelAttribute> (parent_type))
				foreach (MethodInfo minfo in parent_type.GetMethods ())
					if (AttributeManager.HasAttribute<ExportAttribute> (minfo))
						methods.Add (minfo);
		}

		return methods;
	}
}

// Fixes bug 27430 - btouch doesn't escape identifiers with the same name as C# keywords
public static class StringExtensions
{
	public static string GetSafeParamName (this string paramName)
	{
		if (paramName == null)
			return paramName;

		return IsValidIdentifier (paramName) ? paramName : "@" + paramName;
	}

	// Since we're building against the iOS assemblies and there's no code generation there,
	// I'm bringing the implementation from:
	// mono/mcs/class//System/Microsoft.CSharp/CSharpCodeGenerator.cs
	static bool IsValidIdentifier (string identifier)
	{
		if (identifier == null || identifier.Length == 0)
			return false;

		if (keywordsTable == null)
			FillKeywordTable ();

		if (keywordsTable.Contains (identifier))
			return false;

		if (!is_identifier_start_character (identifier [0]))
			return false;

		for (int i = 1; i < identifier.Length; i ++)
			if (! is_identifier_part_character (identifier [i]))
				return false;

		return true;
	}

	static bool is_identifier_start_character (char c)
	{
		return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == '@' || Char.IsLetter (c);
	}

	static bool is_identifier_part_character (char c)
	{
		return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= '0' && c <= '9') || Char.IsLetter (c);
	}

	static void FillKeywordTable ()
	{
		lock (keywords) {
			if (keywordsTable == null) {
				keywordsTable = new Hashtable ();
				foreach (string keyword in keywords) {
					keywordsTable.Add (keyword, keyword);
				}
			}
		}
	}

	private static Hashtable keywordsTable;
	private static string[] keywords = new string[] {
		"abstract","event","new","struct","as","explicit","null","switch","base","extern",
		"this","false","operator","throw","break","finally","out","true",
		"fixed","override","try","case","params","typeof","catch","for",
		"private","foreach","protected","checked","goto","public",
		"unchecked","class","if","readonly","unsafe","const","implicit","ref",
		"continue","in","return","using","virtual","default",
		"interface","sealed","volatile","delegate","internal","do","is",
		"sizeof","while","lock","stackalloc","else","static","enum",
		"namespace",
		"object","bool","byte","float","uint","char","ulong","ushort",
		"decimal","int","sbyte","short","double","long","string","void",
		"partial", "yield", "where"
	};
}

//
// Used to encapsulate flags about types in either the parameter or the return value
// For now, it only supports the [PlainString] attribute on strings.
//
public class MarshalInfo {
	public bool PlainString;
	public Type Type;
	public bool IsOut;
	public EnumMode EnumMode;

 	// This is set on a string parameter if the argument parameters are set to
 	// Copy.   This means that we can do fast string passing.
	public bool ZeroCopyStringMarshal;

	public bool IsAligned;

	// Used for parameters
	public MarshalInfo (MethodInfo mi, ParameterInfo pi)
	{
		PlainString = AttributeManager.HasAttribute<PlainStringAttribute> (pi);
		Type = pi.ParameterType;
		ZeroCopyStringMarshal = (Type == TypeManager.System_String) && PlainString == false && !AttributeManager.HasAttribute<DisableZeroCopyAttribute> (pi) && Generator.SharedGenerator.type_wants_zero_copy;
		if (ZeroCopyStringMarshal && AttributeManager.HasAttribute<DisableZeroCopyAttribute> (mi))
			ZeroCopyStringMarshal = false;
		IsOut = TypeManager.IsOutParameter (pi);
	}

	// Used to return values
	public MarshalInfo (MethodInfo mi)
	{
		PlainString = AttributeManager.HasAttribute<PlainStringAttribute> (AttributeManager.GetReturnTypeCustomAttributes (mi));
		Type = mi.ReturnType;
	}

	public static bool UseString (MethodInfo mi, ParameterInfo pi)
	{
		return new MarshalInfo (mi, pi).PlainString;
	}

	public static implicit operator MarshalInfo (MethodInfo mi)
	{
		return new MarshalInfo (mi);
	}
}

public class Tuple<A,B> {
	public Tuple (A a, B b)
	{
		Item1 = a;
		Item2 = b;
	}
	public A Item1;
	public B Item2;
}
//
// Encapsulates the information necessary to create a block delegate
//
// FIXME: We do not really need this class, we should just move all this
// pre-processing to the generation stage, instead of decoupling it in two places.
//
// The Name is the internal generated name we use for the delegate
// The Parameters is used for the internal delegate signature
// The Invoke contains the invocation steps necessary to invoke the method
//
public class TrampolineInfo {
	public string UserDelegate, DelegateName, TrampolineName, Parameters, Convert, Invoke, ReturnType, DelegateReturnType, ReturnFormat, Clear, OutReturnType, PostConvert;
	public string UserDelegateTypeAttribute;
	public Type Type;
	
	public TrampolineInfo (string userDelegate, string delegateName, string trampolineName, string pars, string convert, string invoke, string returnType, string delegateReturnType, string returnFormat, string clear, string postConvert, Type type)
	{
		UserDelegate = userDelegate;
		DelegateName = delegateName;
		Parameters = pars;
		TrampolineName = trampolineName;
		Convert = convert;
		Invoke = invoke;
		ReturnType = returnType;
		DelegateReturnType = delegateReturnType;
		ReturnFormat = returnFormat;
		Clear = clear;
		PostConvert = postConvert;
		this.Type = type;

		TrampolineName = "Invoke";
	}

	// Name for the static class generated that contains the Objective-C to C# block bridge
	public string StaticName {
		get {
			return "S" + DelegateName;
		}
	}
	
	// Name for the class generated that allows C# to invoke an Objective-C block
	public string NativeInvokerName {
		get {
			return "NI" + DelegateName;
		}
	}
}

//
// This class is used to generate a graph of the type hierarchy of the
// generated types and required by the UIApperance support to determine
// which types need to have Appearance methods created
//
public class GeneratedType {
	static Dictionary<Type,GeneratedType> knownTypes = new Dictionary<Type,GeneratedType> ();

	public static GeneratedType Lookup (Type t)
	{
		if (knownTypes.ContainsKey (t))
			return knownTypes [t];
		var n = new GeneratedType (t);
		knownTypes [t] = n;
		return n;
	}
	
	public GeneratedType (Type t)
	{
		Type = t;
		foreach (var iface in Type.GetInterfaces ()){
			if (iface.Name == "UIAppearance" || iface.Name == "IUIAppearance")
				ImplementsAppearance = true;
		}
		var btype = ReflectionExtensions.GetBaseType (Type);
		if (btype != TypeManager.System_Object){
			Parent = btype;
			// protected against a StackOverflowException - bug #19751
			// it does not protect against large cycles (but good against copy/paste errors)
			if (Parent == Type)
				throw new BindingException (1030, true, "{0} cannot have [BaseType(typeof({1}))] as it creates a circular dependency", Type, Parent);
			ParentGenerated = Lookup (Parent);

			// If our parent had UIAppearance, we flag this class as well
			if (ParentGenerated.ImplementsAppearance)
				ImplementsAppearance = true;
			ParentGenerated.Children.Add (this);
		}

		if (AttributeManager.HasAttribute<CategoryAttribute> (t))
			ImplementsAppearance = false;
	}
	public Type Type;
	public List<GeneratedType> Children = new List<GeneratedType> (1);
	public Type Parent;
	public GeneratedType ParentGenerated;
	public bool ImplementsAppearance;

	List<MemberInfo> appearance_selectors;
	
	public List<MemberInfo> AppearanceSelectors {
		get {
			if (appearance_selectors == null)
				appearance_selectors = new List<MemberInfo> ();
			return appearance_selectors;
		}
	}
}

public interface IMemberGatherer {
	IEnumerable<MethodInfo> GetTypeContractMethods (Type source);
}

class WrapPropMemberInformation
{
	public bool HasWrapOnGetter { get => WrapGetter != null; }
	public bool HasWrapOnSetter { get => WrapSetter != null; }
	public string WrapGetter { get; private set; }
	public string WrapSetter { get; private set; }

	public WrapPropMemberInformation (PropertyInfo pi)
	{
		WrapGetter = AttributeManager.GetCustomAttribute<WrapAttribute> (pi?.GetMethod)?.MethodName;
		WrapSetter = AttributeManager.GetCustomAttribute<WrapAttribute> (pi?.SetMethod)?.MethodName;
	}
}


public class MemberInformation
{
	public readonly MemberInfo mi;
	public readonly Type type;
	public readonly Type category_extension_type;
	internal readonly WrapPropMemberInformation wpmi;
	public readonly bool is_abstract, is_protected, is_internal, is_unified_internal, is_override, is_new, is_sealed, is_static, is_thread_static, is_autorelease, is_wrapper, is_forced;
	public readonly bool is_type_sealed, ignore_category_static_warnings, is_basewrapper_protocol_method;
	public readonly bool has_inner_wrap_attribute;
	public readonly Generator.ThreadCheck threadCheck;
	public bool is_unsafe, is_virtual_method, is_export, is_category_extension, is_variadic, is_interface_impl, is_extension_method, is_appearance, is_model, is_ctor;
	public bool is_return_release;
	public bool protocolize;
	public string selector, wrap_method, is_forced_owns;
	public bool is_bindAs => Generator.HasBindAsAttribute (mi);

	public MethodInfo method { get { return (MethodInfo) mi; } }
	public PropertyInfo property { get { return (PropertyInfo) mi; } }

	MemberInformation (IMemberGatherer gather, MemberInfo mi, Type type, bool is_interface_impl, bool is_extension_method, bool is_appearance, bool is_model)
	{
		var method = mi as MethodInfo;

		is_ctor = mi is MethodInfo && mi.Name == "Constructor";
		is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (mi) && mi.DeclaringType == type;
		is_protected = AttributeManager.HasAttribute<ProtectedAttribute> (mi);
		is_internal = mi.IsInternal ();
		is_unified_internal = (Generator.UnifiedAPI && AttributeManager.HasAttribute<UnifiedInternalAttribute> (mi));
		is_override = AttributeManager.HasAttribute<OverrideAttribute> (mi) || !Generator.MemberBelongsToType (mi.DeclaringType, type);
		is_new = AttributeManager.HasAttribute<NewAttribute> (mi);
		is_sealed = AttributeManager.HasAttribute<SealedAttribute> (mi);
		is_static = AttributeManager.HasAttribute<StaticAttribute> (mi);
		is_thread_static = AttributeManager.HasAttribute<IsThreadStaticAttribute> (mi);
		is_autorelease = AttributeManager.HasAttribute<AutoreleaseAttribute> (mi);
		is_wrapper = !AttributeManager.HasAttribute<SyntheticAttribute> (mi.DeclaringType);
		is_type_sealed = AttributeManager.HasAttribute<SealedAttribute> (mi.DeclaringType);
		is_return_release = method != null && AttributeManager.HasAttribute<ReleaseAttribute> (AttributeManager.GetReturnTypeCustomAttributes (method));
		is_forced = Generator.HasForcedAttribute (mi, out is_forced_owns);

		var tsa = AttributeManager.GetCustomAttribute<ThreadSafeAttribute> (mi);
		// if there's an attribute then it overrides the parent (e.g. type attribute) or namespace default
		if (tsa != null) {
			threadCheck = tsa.Safe ? Generator.ThreadCheck.Off : Generator.ThreadCheck.On;
		} else {
			threadCheck = Generator.ThreadCheck.Default; // will be based on the type decision
		}
		this.is_interface_impl = is_interface_impl;
		this.is_extension_method = is_extension_method;
		this.type = type;
		this.is_appearance = is_appearance;
		this.is_model = is_model;
		this.mi = mi;
		
		if (is_interface_impl || is_extension_method || is_type_sealed) {
			is_abstract = false;
			is_virtual_method = false;
		}

		// To avoid a warning, we should determine whether we should insert a "new" in the 
		// declaration.  If this is an inlined method, then we need to see if this was
		// also inlined in any of the base classes.
		if (mi.DeclaringType != type){
			for (var baseType = ReflectionExtensions.GetBaseType (type); baseType != null && baseType != TypeManager.System_Object; baseType = ReflectionExtensions.GetBaseType (baseType)){
				foreach (var baseMethod in gather.GetTypeContractMethods (baseType)){
					if (baseMethod.DeclaringType != baseType && baseMethod ==  mi){
						// We found a case, we need to flag it as new.
						is_new = true;
					}
				}
			}
		}
		
	}

	public MemberInformation (IMemberGatherer gather, MethodInfo mi, Type type, Type category_extension_type, bool is_interface_impl = false, bool is_extension_method = false, bool is_appearance = false, bool is_model = false, string selector = null, bool isBaseWrapperProtocolMethod = false)
	: this (gather, (MemberInfo)mi, type, is_interface_impl, is_extension_method, is_appearance, is_model)
	{
		is_basewrapper_protocol_method = isBaseWrapperProtocolMethod;
		foreach (ParameterInfo pi in mi.GetParameters ())
			if (pi.ParameterType.IsSubclassOf (TypeManager.System_Delegate))
				is_unsafe = true;

		if (!is_unsafe &&  mi.ReturnType.IsSubclassOf (TypeManager.System_Delegate))
			is_unsafe = true;

		if (selector != null) {
			this.selector = selector;
			if (!is_sealed && !is_wrapper) {
				is_export = !is_extension_method;
				is_virtual_method = !is_ctor;
			}
		} else {
			object [] attr = AttributeManager.GetCustomAttributes<ExportAttribute> (mi);
			if (attr.Length != 1) {
				attr = AttributeManager.GetCustomAttributes<BindAttribute> (mi);
				if (attr.Length != 1) {
					attr = AttributeManager.GetCustomAttributes<WrapAttribute> (mi);
					if (attr.Length != 1)
						throw new BindingException (1012, true, "No Export or Bind attribute defined on {0}.{1}", type, mi.Name);

					var wrapAtt = (WrapAttribute) attr [0];
					wrap_method = wrapAtt.MethodName;
					is_virtual_method = wrapAtt.IsVirtual;
				} else {
					BindAttribute ba = (BindAttribute) attr [0];
					this.selector = ba.Selector;
					is_virtual_method = ba.Virtual;
				}
			} else {
				ExportAttribute ea = (ExportAttribute) attr [0];
				this.selector = ea.Selector;
				is_variadic = ea.IsVariadic;

				if (!is_sealed || !is_wrapper) {
					is_virtual_method = !is_ctor;
					is_export = !is_extension_method;
				}
			}
		}

		this.category_extension_type = category_extension_type;
		if (category_extension_type != null) {
			is_category_extension = true;
			ignore_category_static_warnings = is_internal || type.IsInternal () || AttributeManager.GetCustomAttribute<CategoryAttribute> (type).AllowStaticMembers;
		}

		if (is_static || is_category_extension || is_interface_impl || is_extension_method || is_type_sealed)
			is_virtual_method = false;
	}

	public MemberInformation (IMemberGatherer gather, PropertyInfo pi, Type type, bool is_interface_impl = false)
	: this (gather, (MemberInfo)pi, type, is_interface_impl, false, false, false)
	{
		if (pi.PropertyType.IsSubclassOf (TypeManager.System_Delegate))
			is_unsafe = true;

		var export = Generator.GetExportAttribute (pi, out wrap_method);
		if (export != null)
			selector = export.Selector;

		if (wrap_method != null) {
			var wrapAtt = AttributeManager.GetCustomAttribute <WrapAttribute> (pi);
			is_virtual_method = wrapAtt?.IsVirtual ?? false;
		}
		else if (is_interface_impl || is_type_sealed)
			is_virtual_method = false;
		else
			is_virtual_method = !is_static;

		// Properties can have WrapAttribute on getter/setter so we need to check for this
		// but only if no Export is already found on property level.
		if (export is null) {
			wpmi = new WrapPropMemberInformation (pi);
			has_inner_wrap_attribute = wpmi.HasWrapOnGetter || wpmi.HasWrapOnSetter;

			// Wrap can only be used either at property level or getter/setter level at a given time.
			if (wrap_method != null && has_inner_wrap_attribute)
				throw new BindingException (1063, true, $"The 'WrapAttribute' can only be used at the property or at getter/setter level at a given time. Property: '{pi.DeclaringType}.{pi.Name}'");
		}
	}

	public string GetVisibility ()
	{
		if (is_interface_impl || is_extension_method)
			return "public";

		var mod = is_protected ? "protected" : null;
		mod += is_internal ? "internal" : null;
		if (string.IsNullOrEmpty (mod))
			mod = "public";
		return mod;
	}

	public string GetModifiers ()
	{
		string mods = "";

		mods += is_unsafe ? "unsafe " : null;
		mods += is_new ? "new " : "";

		if (is_sealed) {
			mods += "";
		} else if (is_static || is_category_extension || is_extension_method) {
			mods += "static ";
		} else if (is_abstract) {
			mods += "abstract ";
		} else if (is_virtual_method) {
			mods += is_override ? "override " : "virtual ";
		}

	    return mods;
	}
}

public class NamespaceManager
{
	public string Prefix { get; private set; }

	// Where the core messaging lives
	public string CoreObjCRuntime { get; private set; }

	// Where user-overrideable messaging may live
	public string ObjCRuntime { get; private set; }

	public string Messaging { get; private set; }

	public ICollection<string> StandardNamespaces { get; private set; }
	public ICollection<string> UINamespaces { get; private set; }
	public ICollection<string> ImplicitNamespaces { get; private set; }
	public ICollection<string> NamespacesThatConflictWithTypes { get; private set; }

	public NamespaceManager (string prefix, string customObjCRuntimeNS, bool skipSystemDrawing)
	{
		Prefix = prefix;

		CoreObjCRuntime = Get ("ObjCRuntime");
		ObjCRuntime = String.IsNullOrEmpty (customObjCRuntimeNS)
			? CoreObjCRuntime
			: customObjCRuntimeNS;

		Messaging = ObjCRuntime + ".Messaging";

		StandardNamespaces = new HashSet<string> {
			Get ("Foundation"),
			Get ("ObjCRuntime"),
			Get ("CoreGraphics")
		};

		UINamespaces = new HashSet<string> ();
		if (Frameworks.HaveAppKit)
			UINamespaces.Add (Get ("AppKit"));
		if (Frameworks.HaveUIKit)
			UINamespaces.Add (Get ("UIKit"));
		if (Frameworks.HaveTwitter)
			UINamespaces.Add (Get ("Twitter"));
		if (Frameworks.HaveGameKit && Generator.CurrentPlatform != PlatformName.MacOSX && Generator.CurrentPlatform != PlatformName.WatchOS)
			UINamespaces.Add (Get ("GameKit"));
		if (Frameworks.HaveNewsstandKit)
			UINamespaces.Add (Get ("NewsstandKit"));
		if (Frameworks.HaveiAd)
			UINamespaces.Add (Get ("iAd"));
		if (Frameworks.HaveQuickLook)
			UINamespaces.Add (Get ("QuickLook"));
		if (Frameworks.HaveEventKitUI)
			UINamespaces.Add (Get ("EventKitUI"));
		if (Frameworks.HaveAddressBookUI)
			UINamespaces.Add (Get ("AddressBookUI"));
		if (Frameworks.HaveMapKit && Generator.CurrentPlatform != PlatformName.MacOSX && Generator.CurrentPlatform != PlatformName.TvOS && Generator.CurrentPlatform != PlatformName.WatchOS)
			UINamespaces.Add (Get ("MapKit"));
		if (Frameworks.HaveMessageUI)
			UINamespaces.Add (Get ("MessageUI"));
		if (Frameworks.HavePhotosUI)
			UINamespaces.Add (Get ("PhotosUI"));
		if (Frameworks.HaveHealthKitUI)
			UINamespaces.Add (Get ("HealthKitUI"));

		ImplicitNamespaces = new HashSet<string> ();
		ImplicitNamespaces.Add ("System");
		ImplicitNamespaces.Add ("System.Runtime.InteropServices");
		ImplicitNamespaces.Add ("System.Diagnostics");
		ImplicitNamespaces.Add ("System.ComponentModel");
		ImplicitNamespaces.Add ("System.Threading.Tasks");
		ImplicitNamespaces.Add (Get ("CoreFoundation"));
		ImplicitNamespaces.Add (Get ("Foundation"));
		ImplicitNamespaces.Add (Get ("ObjCRuntime"));
		ImplicitNamespaces.Add (Get ("CoreGraphics"));
		ImplicitNamespaces.Add (Get ("SceneKit"));

		if (Frameworks.HaveAudioUnit)
			ImplicitNamespaces.Add (Get ("AudioUnit"));
		if (Frameworks.HaveContacts && Generator.UnifiedAPI)
			ImplicitNamespaces.Add (Get ("Contacts"));
		if (Frameworks.HaveCoreAnimation)
			ImplicitNamespaces.Add (Get ("CoreAnimation"));
		if (Frameworks.HaveCoreLocation)
			ImplicitNamespaces.Add (Get ("CoreLocation"));
		if (Frameworks.HaveCoreVideo)
			ImplicitNamespaces.Add (Get ("CoreVideo"));
		if (Frameworks.HaveCoreMedia)
			ImplicitNamespaces.Add (Get ("CoreMedia"));
		if (Frameworks.HaveSecurity && Generator.CurrentPlatform != PlatformName.WatchOS)
			ImplicitNamespaces.Add (Get ("Security"));
		if (Frameworks.HaveAVFoundation)
			ImplicitNamespaces.Add (Get ("AVFoundation"));
		if (Frameworks.HaveOpenGL)
			ImplicitNamespaces.Add (Get ("OpenGL"));
		if (Frameworks.HaveQTKit)
			ImplicitNamespaces.Add (Get ("QTKit"));
		if (Frameworks.HaveAppKit)
			ImplicitNamespaces.Add (Get ("AppKit"));
		if (Frameworks.HaveCloudKit && Generator.CurrentPlatform != PlatformName.WatchOS && Generator.CurrentPlatform != PlatformName.TvOS && Generator.CurrentPlatform != PlatformName.iOS)
			ImplicitNamespaces.Add (Get ("CloudKit"));
		if (Frameworks.HaveCoreMotion && Generator.CurrentPlatform != PlatformName.WatchOS && Generator.CurrentPlatform != PlatformName.TvOS && Generator.CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add (Get ("CoreMotion"));
		if (Frameworks.HaveMapKit && Generator.CurrentPlatform != PlatformName.WatchOS && Generator.CurrentPlatform != PlatformName.TvOS && Generator.CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add (Get ("MapKit"));
		if (Frameworks.HaveUIKit)
			ImplicitNamespaces.Add (Get ("UIKit"));
		if (Frameworks.HaveNewsstandKit)
			ImplicitNamespaces.Add (Get ("NewsstandKit"));
		if (Frameworks.HaveGLKit && Generator.CurrentPlatform != PlatformName.WatchOS && Generator.CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add (Get ("GLKit"));
		if (Frameworks.HaveQuickLook && Generator.CurrentPlatform != PlatformName.WatchOS && Generator.CurrentPlatform != PlatformName.TvOS && Generator.CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add (Get ("QuickLook"));
		if (Frameworks.HaveAddressBook)
			ImplicitNamespaces.Add (Get ("AddressBook"));

		if (Frameworks.HaveModelIO && !(Generator.CurrentPlatform == PlatformName.MacOSX && !Generator.UnifiedAPI))
			ImplicitNamespaces.Add (Get ("ModelIO"));
		if (Frameworks.HaveMetal && !(Generator.CurrentPlatform == PlatformName.MacOSX && !Generator.UnifiedAPI))
			ImplicitNamespaces.Add (Get ("Metal"));

		if (Frameworks.HaveCoreImage)
			ImplicitNamespaces.Add (Get ("CoreImage"));
		if (Frameworks.HavePhotos)
			ImplicitNamespaces.Add (Get ("Photos"));
		if (Frameworks.HaveMediaPlayer && Generator.UnifiedAPI)
			ImplicitNamespaces.Add (Get ("MediaPlayer"));
		if (Frameworks.HaveMessages)
			ImplicitNamespaces.Add (Get ("Messages"));
		if (Frameworks.HaveGameplayKit)
			ImplicitNamespaces.Add (Get ("GameplayKit"));
		if (Frameworks.HaveSpriteKit)
			ImplicitNamespaces.Add (Get ("SpriteKit"));
		if (Frameworks.HaveFileProvider && Generator.UnifiedAPI)
			ImplicitNamespaces.Add (Get ("FileProvider"));

		// These are both types and namespaces
		NamespacesThatConflictWithTypes = new HashSet<string> {
			Get ("AudioUnit")
		};


		if (!skipSystemDrawing)
			ImplicitNamespaces.Add ("System.Drawing");
	}

	public string Get (string nsWithoutPrefix)
	{
		if (String.IsNullOrEmpty (Prefix))
			return nsWithoutPrefix;
		return Prefix + "." + nsWithoutPrefix;
	}

	public string [] Get (IEnumerable<string> nsWithoutPrefix)
	{
		return nsWithoutPrefix.Select (ns => Get (ns)).ToArray ();
	}

	public string [] Get (params string [] nsWithoutPrefix)
	{
		return Get ((IEnumerable<string>)nsWithoutPrefix);
	}
}

public enum EnumMode {
	Compat, Bit32, Bit64, NativeBits
}

public partial class Frameworks {
	static HashSet<string> frameworks;
	static bool GetValue (string framework)
	{
		if (frameworks == null) {
			switch (Generator.CurrentPlatform) {
			case PlatformName.iOS:
				frameworks = iosframeworks;
				break;
			case PlatformName.WatchOS:
				frameworks = watchosframeworks;
				break;
			case PlatformName.TvOS:
				frameworks = tvosframeworks;
				break;
			case PlatformName.MacOSX:
				frameworks = macosframeworks;
				break;
			default:
				throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (https://bugzilla.xamarin.com) with a test case.", Generator.CurrentPlatform);
			}
		}

		return frameworks.Contains (framework);
	}
}

public partial class Generator : IMemberGatherer {
	internal static bool IsPublicMode;

	static NamespaceManager ns;
	Dictionary<Type,IEnumerable<string>> selectors = new Dictionary<Type,IEnumerable<string>> ();
	Dictionary<Type,bool> need_static = new Dictionary<Type,bool> ();
	Dictionary<Type,bool> need_abstract = new Dictionary<Type,bool> ();
	Dictionary<string,int> selector_use = new Dictionary<string, int> ();
	Dictionary<string,string> selector_names = new Dictionary<string,string> ();
	Dictionary<string,string> send_methods = new Dictionary<string,string> ();
	List<MarshalType> marshal_types = new List<MarshalType> ();
	Dictionary<Type,TrampolineInfo> trampolines = new Dictionary<Type,TrampolineInfo> ();
	Dictionary<Type,int> trampolines_generic_versions = new Dictionary<Type,int> ();
	Dictionary<Type,Type> notification_event_arg_types = new Dictionary<Type,Type> ();
	Dictionary<string, string> libraries = new Dictionary<string, string> (); // <LibraryName, libraryPath>

	List<Tuple<string, ParameterInfo[]>> async_result_types = new List<Tuple <string, ParameterInfo[]>> ();
	HashSet<string> async_result_types_emitted = new HashSet<string> ();

	//
	// This contains delegates that are referenced in the source and need to be generated.
	//
	Dictionary<string,MethodInfo> delegate_types = new Dictionary<string,MethodInfo> ();

	public bool Compat { get { return !UnifiedAPI; } }

	public static PlatformName CurrentPlatform { get { return BindingTouch.CurrentPlatform; } }

	public static string ApplicationClassName {
		get {
			switch (CurrentPlatform) {
			case PlatformName.iOS:
			case PlatformName.WatchOS:
			case PlatformName.TvOS:
				return "UIApplication";
			case PlatformName.MacOSX:
				return "NSApplication";
			default:
				throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
			}
		}
	}

	// Static version of the above (!Compat) field, set on each Go invocation, needed because some static
	// helper methods need to access this.   This is the exact opposite of Compat.
	public static bool UnifiedAPI { get { return BindingTouch.Unified; } }
	public static int XamcoreVersion {
		get {
			switch (Generator.CurrentPlatform) {
			case PlatformName.MacOSX:
			case PlatformName.iOS:
				return UnifiedAPI ? 2 : 1;
			case PlatformName.TvOS:
			case PlatformName.WatchOS:
				return 3;
			default:
				return 4;
			}
		}
	}
	Type [] types, strong_dictionaries;
	bool debug;
	bool external;
	StreamWriter sw, m;
	int indent;

	static public NamespaceManager NamespaceManager {
		get { return ns; }
	}

	public class MarshalType {
		public Type Type;
		public string Encoding;
		public string ParameterMarshal;
		public string CreateFromRet;
		public bool HasCustomCreate;
		public string ClosingCreate;

		public MarshalType (Type t, string encode = null, string fetch = null, string create = null, string closingCreate = ")")
		{
			Type = t;
			Encoding = encode ?? "IntPtr";
			ParameterMarshal = fetch ?? "{0}.Handle";
			CreateFromRet = create ?? String.Format ("new global::{0} (", t.FullName);
			HasCustomCreate = create != null;
			ClosingCreate = closingCreate;
		}

		//
		// When you use this constructor, the marshaling defaults to:
		// Marshal type like this:
		//   Encoding = IntPtr
		//   Getting the underlying representation: using the .Handle property
		//   Intantiating the object: creates a new object by passing the handle to the type.
		//
		public static implicit operator MarshalType (Type type)
		{
			return new MarshalType (type);
		}
	}

	public bool LookupMarshal (Type t, out MarshalType res)
	{
		res = null;
		// quick out for common (and easy to detect) cases
		if (t.IsArray || t.IsByRef || t.IsPrimitive)
			return false;
		foreach (var mt in marshal_types){
			// full name is required because some types (e.g. CVPixelBuffer) are now also in core.dll
			if (mt.Type.FullName == t.FullName) {
				res = mt;
				return true;
			}
		}
		return false;
	}

	//
	// Properties and definitions to support binding third-party Objective-C libraries
	//
	string is_direct_binding_value; // An expression that calculates the IsDirectBinding value. Might not be a constant expression. This will be added to every constructor for a type.
	bool? is_direct_binding; // If a constant value for IsDirectBinding is known, it's stored here. Will be null if no constant value is known.

	// Whether to use ZeroCopy for strings, defaults to false
	public bool ZeroCopyStrings;

	public static bool BindThirdPartyLibrary = true;
	public bool InlineSelectors;
	public string BaseDir { get { return basedir; } set { basedir = value; }}
	string basedir;
	HashSet<string> generated_files = new HashSet<string> ();
	public Type CoreNSObject = TypeManager.NSObject;
	public Type SampleBufferType = TypeManager.CMSampleBuffer;

	static string CoreImageMap {
		get {
			switch (CurrentPlatform) {
			case PlatformName.iOS:
			case PlatformName.WatchOS:
			case PlatformName.TvOS:
				return "CoreImage";
			case PlatformName.MacOSX:
				return "Quartz";
			default:
				throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
			}
		}
	}

	static string CoreServicesMap {
		get {
			switch (CurrentPlatform) {
			case PlatformName.iOS:
			case PlatformName.WatchOS:
			case PlatformName.TvOS:
				return "MobileCoreServices";
			case PlatformName.MacOSX:
				return "CoreServices";
			default:
				throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
			}
		}
	}

	static string PDFKitMap {
		get {
			switch (CurrentPlatform) {
			case PlatformName.iOS:
				return "PDFKit";
			case PlatformName.MacOSX:
				return "Quartz";
			default:
				throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (https://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
			}
		}
	}

	//
	// We inject thread checks to MonoTouch.UIKit types, unless there is a [ThreadSafe] attribuet on the type.
	// Set on every call to Generate
	//
	bool type_needs_thread_checks;

	//
	// If set, the members of this type will get zero copy
	// 
	internal bool type_wants_zero_copy;
	
	//
	// Used by the public binding generator to populate the
	// class with types that do not exist
	//
	public void RegisterMethodName (string method_name)
	{
		send_methods [method_name] = method_name;
	}

	//
	// Helpers
	//
	string MakeSig (MethodInfo mi, bool stret, bool aligned = false, EnumMode enum_mode = EnumMode.Compat ) { return MakeSig ("objc_msgSend", stret, mi, aligned, enum_mode); }
	string MakeSuperSig (MethodInfo mi, bool stret, bool aligned = false, EnumMode enum_mode = EnumMode.Compat) { return MakeSig ("objc_msgSendSuper", stret, mi, aligned, enum_mode); }

	public IEnumerable<string> GeneratedFiles {
		get {
			return generated_files;
		}
	}

	bool IsNativeType (Type pt)
	{
		return (pt == TypeManager.System_Int32 || pt == TypeManager.System_Int64 || pt == TypeManager.System_Byte || pt == TypeManager.System_Int16);
	}

	public string PrimitiveType (Type t, bool formatted = false, EnumMode enum_mode = EnumMode.Compat)
	{
		if (t == TypeManager.System_Void)
			return "void";

		if (t.IsEnum) {
			var enumType = t;

			t = TypeManager.GetUnderlyingEnumType (t);

			if (UnifiedAPI && AttributeManager.HasAttribute<NativeAttribute> (enumType)) {
				if (t != TypeManager.System_Int64 && t != TypeManager.System_UInt64)
					throw new BindingException (1026, true,
						"`{0}`: Enums attributed with [{1}] must have an underlying type of `long` or `ulong`",
					    enumType.FullName, "NativeAttribute");

				if (enum_mode == EnumMode.Bit32) {
					if (t == TypeManager.System_Int64) {
						t = TypeManager.System_Int32;
					} else if (t == TypeManager.System_UInt64) {
						t = TypeManager.System_UInt32;
					}
				} else if (enum_mode == EnumMode.Bit64) {
					// Nothing to do
				} else {
					throw new BindingException (1029, "Internal error: invalid enum mode for type '{0}'", t.FullName);
				}
			}
		}

		if (t == TypeManager.System_Int32)
			return "int";
		if (t == TypeManager.System_Int16)
			return "short";
		if (t == TypeManager.System_Byte)
			return "byte";
		if (t == TypeManager.System_Float)
			return "float";
		if (t == TypeManager.System_Boolean)
			return "bool";

		return formatted ? FormatType (null, t) : t.Name;
	}

	// Is this a wrapped type of NSObject from the MonoTouch/MonoMac binding world?
	public bool IsWrappedType (Type t)
	{
		if (t.IsInterface) 
			return true;
		if (CoreNSObject != null)
			return t.IsSubclassOf (CoreNSObject) || t == CoreNSObject; 
		return false;
	}

	public bool IsArrayOfWrappedType (Type t)
	{
		return t.IsArray && IsWrappedType (t.GetElementType ());
	}
	

	// Is this type something that derives from DictionaryContainerType (or an interface marked up with StrongDictionary)
	public bool IsDictionaryContainerType (Type t)
	{
		return t.IsSubclassOf (TypeManager.DictionaryContainerType) || (t.IsInterface && AttributeManager.HasAttribute<StrongDictionaryAttribute> (t));
	}

	//
	// Returns the type that we use to marshal the given type as a string
	// for example "UIView" -> "IntPtr"
	string ParameterGetMarshalType (MarshalInfo mai, bool formatted = false)
	{
		if (mai.IsAligned)
			return "IntPtr";

		if (mai.Type.IsEnum)
			return PrimitiveType (mai.Type, formatted, mai.EnumMode);

		if (IsWrappedType (mai.Type))
			return mai.Type.IsByRef ? "ref IntPtr" : "IntPtr";

		if (IsNativeType (mai.Type))
			return PrimitiveType (mai.Type, formatted);

		if (mai.Type == TypeManager.System_String){
			if (mai.PlainString)
				return "string";

			// We will do NSString
			return "IntPtr";
		} 

		MarshalType mt;
		if (LookupMarshal (mai.Type, out mt))
			return mt.Encoding;
		
		if (mai.Type.IsValueType)
			return PrimitiveType (mai.Type, formatted);

		// Arrays are returned as NSArrays
		if (mai.Type.IsArray)
			return "IntPtr";

		//
		// Pass "out ValueType" directly
		//
		if (mai.Type.IsByRef && mai.Type.GetElementType ().IsValueType){
			Type elementType = mai.Type.GetElementType ();

			return (mai.IsOut ? "out " : "ref ") + (formatted ? FormatType (null, elementType) : elementType.Name);
		}

		if (mai.Type.IsSubclassOf (TypeManager.System_Delegate)){
			return "IntPtr";
		}

		if (IsDictionaryContainerType(mai.Type)){
			return "IntPtr";
		}
		
		//
		// Edit the table in the "void Go ()" routine
		//
		
		if (mai.Type.IsByRef && mai.Type.GetElementType ().IsValueType == false)
			return "ref IntPtr";
		
		if (mai.Type.IsGenericParameter)
			return "IntPtr";

		throw new BindingException (1017, true, "Do not know how to make a signature for {0}", mai.Type);
	}

	static bool IsProtocolInterface (Type type, bool checkPrefix = true)
	{
		// for subclassing the type (from the binding files) is not yet prefixed by an `I`
		if (checkPrefix && type.Name [0] != 'I')
			return false;

		if (AttributeManager.HasAttribute<ProtocolAttribute> (type))
			return true;

		var protocol = type.Assembly.GetType (type.Namespace + "." + type.Name.Substring (1), false);
		if (protocol == null)
			return false;

		return AttributeManager.HasAttribute<ProtocolAttribute> (protocol);
	}

	static string FindProtocolInterface (Type type, MemberInfo pi)
	{
		var isArray = type.IsArray;
		var declType = isArray ? type.GetElementType () : type;

		if (!AttributeManager.HasAttribute<ProtocolAttribute> (declType))
			throw new BindingException (1034, true, "The [Protocolize] attribute is set on the member {0}.{1}, but the member's type ({2}) is not a protocol.",
				pi.DeclaringType, pi.Name, declType);

		return "I" + type.Name;
	}

	public static BindAsAttribute GetBindAsAttribute (ICustomAttributeProvider cu)
	{
		BindAsAttribute rv;
		if (cu != null && (rv = AttributeManager.GetCustomAttribute<BindAsAttribute> (cu)) != null)
			return rv;

		var minfo = cu as MethodInfo;
		if (minfo?.ReturnParameter != null && (rv = AttributeManager.GetCustomAttribute<BindAsAttribute> (minfo.ReturnParameter)) != null)
			return rv;

		return null;
	}
	public static bool HasBindAsAttribute (ICustomAttributeProvider cu)
	{
		return GetBindAsAttribute (cu) != null;
	}

	static bool IsSetter (MethodInfo mi) => mi.IsSpecialName && mi.Name.StartsWith ("set_", StringComparison.Ordinal);
	static string GetBindAsExceptionString (string box, string retType, string containerType, string container, params ICustomAttributeProvider [] providers)
	{
		Type declaringType = null;
		string memberName = null;
		foreach (var provider in providers) {
			if (provider is MemberInfo member) {
				declaringType = member.DeclaringType;
				memberName = member.Name;
			} else if (provider is ParameterInfo parameter) {
				declaringType = parameter.Member.DeclaringType;
				memberName = parameter.Member.Name;
				break;
			}
		}

		if (declaringType != null && memberName != null)
			memberName = declaringType.FullName + "." + memberName;

		return $"Could not {box} type {retType} from {containerType} {container} used on member {memberName} decorated with [BindAs].";
	}
	bool IsMemberInsideProtocol (Type type) => IsProtocol (type) || IsModel (type);

	bool IsSmartEnum (Type type)
	{
		if (!type.IsEnum)
			return false;
		// First check if the smart enum candidate still holds the FieldAtttribute data
		if (type.GetFields ().Any (f => AttributeManager.GetCustomAttribute<FieldAttribute> (f) != null))
			return true;
		// If the above fails it's possible that it comes from another dll (like X.I.dll) so we look for the [Enum]Extensions class existence
		return type.Assembly.GetType (type.FullName + "Extensions") != null;
	}

	static Dictionary<Type,string> nsvalue_create_map;
	static Dictionary<Type,string> NSValueCreateMap {
		get {
			if (nsvalue_create_map == null) {
				nsvalue_create_map = new Dictionary<Type, string> ();
				nsvalue_create_map [TypeManager.CGAffineTransform] = "CGAffineTransform";
				nsvalue_create_map [TypeManager.NSRange] = "Range";
				nsvalue_create_map [TypeManager.CGVector] = "CGVector";
				nsvalue_create_map [TypeManager.SCNMatrix4] = "SCNMatrix4";
				nsvalue_create_map [TypeManager.CLLocationCoordinate2D] = "MKCoordinate";
				nsvalue_create_map [TypeManager.SCNVector3] = "Vector";
				nsvalue_create_map [TypeManager.SCNVector4] = "Vector";

				if (UnifiedAPI) {
					nsvalue_create_map [TypeManager.CoreGraphics_CGPoint] = "CGPoint";
					nsvalue_create_map [TypeManager.CoreGraphics_CGRect] = "CGRect";
					nsvalue_create_map [TypeManager.CoreGraphics_CGSize] = "CGSize";
				}

				if (Frameworks.HaveUIKit) {
					nsvalue_create_map [TypeManager.UIEdgeInsets] = "UIEdgeInsets";
					nsvalue_create_map [TypeManager.UIOffset] = "UIOffset";
					nsvalue_create_map [TypeManager.NSDirectionalEdgeInsets] = "DirectionalEdgeInsets";
				}

				if (TypeManager.MKCoordinateSpan != null)
					nsvalue_create_map [TypeManager.MKCoordinateSpan] = "MKCoordinateSpan";

				if (Frameworks.HaveCoreMedia) {
					nsvalue_create_map [TypeManager.CMTimeRange] = "CMTimeRange";
					nsvalue_create_map [TypeManager.CMTime] = "CMTime";
					nsvalue_create_map [TypeManager.CMTimeMapping] = "CMTimeMapping";
				}

				if (Frameworks.HaveCoreAnimation)
					nsvalue_create_map [TypeManager.CATransform3D] = "CATransform3D";
			}
			return nsvalue_create_map;
		}
	}

	string GetToBindAsWrapper (MemberInformation minfo = null, ParameterInfo pi = null)
	{
		BindAsAttribute attrib = null;
		Type originalType = null;
		string temp = null;
		var declaringType = minfo?.mi?.DeclaringType ?? pi?.Member?.DeclaringType;

		if (IsMemberInsideProtocol (declaringType))
			throw new BindingException (1050, true, "[BindAs] cannot be used inside Protocol or Model types. Type: {0}", declaringType.Name);

		if (pi == null) {
			attrib = GetBindAsAttribute (minfo.mi);
			var property = minfo.mi as PropertyInfo;
			var method = minfo.mi as MethodInfo;
			originalType = method?.ReturnType ?? property?.PropertyType;
		} else {
			attrib = GetBindAsAttribute (pi);
			originalType = pi.ParameterType;
		}

		if (originalType.IsByRef)
			throw new BindingException (1048, true, $"Unsupported type 'ref/out {originalType.Name.Replace ("&", string.Empty)}' decorated with [BindAs]");

		var retType = TypeManager.GetUnderlyingNullableType (attrib.Type) ?? attrib.Type;
		var isNullable = attrib.IsNullable;
		var isValueType = retType.IsValueType;
		var isEnum = retType.IsEnum;
		var parameterName = pi != null ? pi.Name.GetSafeParamName () : "value";
		var denullify = isNullable ? ".Value" : string.Empty;
		var nullCheck = isNullable ? $"{parameterName} == null ? null : " : string.Empty;

		if (isNullable || !isValueType)
			temp = string.Format ("{0} == null ? null : ", parameterName);

		if (originalType == TypeManager.NSNumber) {
			var enumCast = isEnum ? $"(int)" : string.Empty;
			temp = string.Format ("{3}new NSNumber ({2}{1}{0});", denullify, parameterName, enumCast, nullCheck);
		}
		else if (originalType == TypeManager.NSValue) {
			var typeStr = string.Empty;
			if (!NSValueCreateMap.TryGetValue (retType, out typeStr)) {
				// HACK: These are problematic for X.M due to we do not ship System.Drawing for Full profile
				if (retType.Name == "RectangleF" || retType.Name == "SizeF" || retType.Name == "PointF")
					typeStr = retType.Name;
				else
					throw new BindingException (1049, true, GetBindAsExceptionString ("box", retType.Name, originalType.Name, "container", minfo?.mi, pi));
			}
			temp = string.Format ("{3}NSValue.From{0} ({2}{1});", typeStr, denullify, parameterName, nullCheck);
		} else if (originalType == TypeManager.NSString && IsSmartEnum (retType)) {
			temp = isNullable ? $"{parameterName} == null ? null : " : string.Empty;
			temp += $"{FormatType (retType.DeclaringType, retType)}Extensions.GetConstant ({parameterName}{denullify});";
		} else if (originalType.IsArray && originalType.GetArrayRank () == 1) {
			var arrType = originalType.GetElementType ();
			var arrRetType = TypeManager.GetUnderlyingNullableType (retType.GetElementType ()) ?? retType.GetElementType ();
			var valueConverter = string.Empty;

			if (arrType == TypeManager.NSString && !isNullable) {
				valueConverter = isNullable ? "o == null ? null : " : string.Empty;
				valueConverter += $"{FormatType (retType.DeclaringType, arrRetType)}Extensions.GetConstant ({(isNullable ? "o.Value" : "o")}), {parameterName});";
			} else if (arrType == TypeManager.NSNumber && !isNullable) {
				var cast = arrRetType.IsEnum ? "(int)" : string.Empty;
				valueConverter = $"new NSNumber ({cast}o{denullify}), {parameterName});";
			} else if (arrType == TypeManager.NSValue && !isNullable) {
				var typeStr = string.Empty;
				if (!NSValueCreateMap.TryGetValue (arrRetType, out typeStr)) {
					if (arrRetType.Name == "RectangleF" || arrRetType.Name == "SizeF" || arrRetType.Name == "PointF")
						typeStr = retType.Name;
					else
						throw new BindingException (1049, true, GetBindAsExceptionString ("box", arrRetType.Name, originalType.Name, "array", minfo?.mi, pi));
				}
				valueConverter = $"NSValue.From{typeStr} (o{denullify}), {parameterName});";
			} else
				throw new BindingException (1048, true, "Unsupported type {0} decorated with [BindAs]", isNullable ? arrRetType.Name + "?[]" : retType.Name);
			temp = $"NSArray.FromNSObjects (o => {valueConverter}";
		} else
			throw new BindingException (1048, true, "Unsupported type {0} decorated with [BindAs]", retType.Name);

		return temp;
	}

	static Dictionary<Type,string> nsnumber_return_map;
	static Dictionary<Type,string> NSNumberReturnMap {
		get {
			if (nsnumber_return_map == null) {
				nsnumber_return_map = new Dictionary<Type, string> {
					{ TypeManager.System_Boolean, ".BoolValue" },
					{ TypeManager.System_Byte, ".ByteValue" },
					{ TypeManager.System_Double, ".DoubleValue" },
					{ TypeManager.System_Float, ".FloatValue" },
					{ TypeManager.System_Int16, ".Int16Value" },
					{ TypeManager.System_Int32, ".Int32Value" },
					{ TypeManager.System_Int64, ".Int64Value" },
					{ TypeManager.System_SByte, ".SByteValue" },
					{ TypeManager.System_UInt16, ".UInt16Value" },
					{ TypeManager.System_UInt32, ".UInt32Value" },
					{ TypeManager.System_UInt64, ".UInt64Value" },
				};
				if (UnifiedAPI) {
					nsnumber_return_map [TypeManager.System_nfloat] = ".NFloatValue";
					nsnumber_return_map [TypeManager.System_nint] = ".NIntValue";
					nsnumber_return_map [TypeManager.System_nuint] = ".NUIntValue";
				}
			}
			return nsnumber_return_map;
		}
	}

	static Dictionary<Type,string> nsvalue_return_map;
	static Dictionary<Type,string> NSValueReturnMap {
		get {
			if (nsvalue_return_map == null) {
				nsvalue_return_map = new Dictionary<Type, string> {
					{ TypeManager.CGAffineTransform, ".CGAffineTransformValue" },
					{ TypeManager.NSRange, ".RangeValue" },
					{ TypeManager.CGVector, ".CGVectorValue" },
					{ TypeManager.SCNMatrix4, ".SCNMatrix4Value" },
					{ TypeManager.CLLocationCoordinate2D, ".CoordinateValue" },
					{ TypeManager.SCNVector3, ".Vector3Value" },
					{ TypeManager.SCNVector4, ".Vector4Value" }
				};

				if (UnifiedAPI) {
					nsvalue_return_map [TypeManager.CoreGraphics_CGPoint] = ".CGPointValue";
					nsvalue_return_map [TypeManager.CoreGraphics_CGRect] = ".CGRectValue";
					nsvalue_return_map [TypeManager.CoreGraphics_CGSize] = ".CGSizeValue";
				}

				if (Frameworks.HaveUIKit) {
					nsvalue_return_map [TypeManager.UIEdgeInsets] = ".UIEdgeInsetsValue";
					nsvalue_return_map [TypeManager.UIOffset] = ".UIOffsetValue";
					nsvalue_return_map [TypeManager.NSDirectionalEdgeInsets] = ".DirectionalEdgeInsetsValue";
				}

				if (TypeManager.MKCoordinateSpan != null)
					nsvalue_return_map [TypeManager.MKCoordinateSpan] = ".CoordinateSpanValue";

				if (Frameworks.HaveCoreMedia) {
					nsvalue_return_map [TypeManager.CMTimeRange] =  ".CMTimeRangeValue";
					nsvalue_return_map [TypeManager.CMTime] = ".CMTimeValue";
					nsvalue_return_map [TypeManager.CMTimeMapping] = ".CMTimeMappingValue";
				}

				if (Frameworks.HaveCoreMedia)
					nsvalue_return_map [TypeManager.CATransform3D] = ".CATransform3DValue";
			}
			return nsvalue_return_map;
		}
	}

	string GetFromBindAsWrapper (MemberInformation minfo, out string suffix)
	{
		var declaringType = minfo.mi.DeclaringType;
		if (IsMemberInsideProtocol (declaringType))
			throw new BindingException (1050, true, "[BindAs] cannot be used inside Protocol or Model types. Type: {0}", declaringType.Name);

		suffix = string.Empty;

		var attrib = GetBindAsAttribute (minfo.mi);
		var nullableRetType = TypeManager.GetUnderlyingNullableType (attrib.Type);
		var isNullable = nullableRetType != null;
		var retType = isNullable ? nullableRetType : attrib.Type;
		var isValueType = retType.IsValueType;
		var append = string.Empty;
		var property = minfo.mi as PropertyInfo;
		var method = minfo.mi as MethodInfo;
		var originalReturnType = method?.ReturnType ?? property?.PropertyType;

		if (originalReturnType == TypeManager.NSNumber) {
			if (!NSNumberReturnMap.TryGetValue (retType, out append)) {
				if (retType.IsEnum) {
					var enumType = TypeManager.GetUnderlyingEnumType (retType);
					if (!NSNumberReturnMap.TryGetValue (enumType, out append))
						throw new BindingException (1049, true, GetBindAsExceptionString ("unbox", retType.Name, originalReturnType.Name, "container", minfo.mi));
				}
				else
					throw new BindingException (1049, true, GetBindAsExceptionString ("unbox", retType.Name, originalReturnType.Name, "container", minfo.mi));
			}
			if (isNullable)
				append = $"?{append}";
		} else if (originalReturnType == TypeManager.NSValue) {
			if (!NSValueReturnMap.TryGetValue (retType, out append)) {
				// HACK: These are problematic for X.M due to we do not ship System.Drawing for Full profile
				if (retType.Name == "RectangleF" || retType.Name == "SizeF" || retType.Name == "PointF")
					append = $".{retType.Name}Value";
				else
					throw new BindingException (1049, true, GetBindAsExceptionString ("unbox", retType.Name, originalReturnType.Name, "container", minfo.mi));
			}
			if (isNullable)
				append = $"?{append}";
		} else if (originalReturnType == TypeManager.NSString && IsSmartEnum (retType)) {
			append = $"{FormatType (retType.DeclaringType, retType)}Extensions.GetValue (";
			suffix = ")";
		} else if (originalReturnType.IsArray && originalReturnType.GetArrayRank () == 1) {
			var arrType = originalReturnType.GetElementType ();
			var nullableElementType = TypeManager.GetUnderlyingNullableType (retType.GetElementType ());
			var arrIsNullable = nullableElementType != null;
			var arrRetType = arrIsNullable ? nullableElementType : retType.GetElementType ();
			var valueFetcher = string.Empty;
			if (arrType == TypeManager.NSString && !arrIsNullable)
				append = $"ptr => {{\n\tusing (var str = Runtime.GetNSObject<NSString> (ptr)) {{\n\t\treturn {FormatType (arrRetType.DeclaringType, arrRetType)}Extensions.GetValue (str);\n\t}}\n}}";
			else if (arrType == TypeManager.NSNumber && !arrIsNullable) {
				if (NSNumberReturnMap.TryGetValue (arrRetType, out valueFetcher) || arrRetType.IsEnum) {
					var getterStr = string.Format ("{0}{1}", arrIsNullable ? "?" : string.Empty, arrRetType.IsEnum ? ".Int32Value" : valueFetcher);
					append = string.Format ("ptr => {{\n\tusing (var num = Runtime.GetNSObject<NSNumber> (ptr)) {{\n\t\treturn ({1}) num{0};\n\t}}\n}}", getterStr, FormatType (arrRetType.DeclaringType, arrRetType));
				}
				else
					throw new BindingException (1049, true, GetBindAsExceptionString ("unbox", retType.Name, arrType.Name, "array", minfo.mi));
			} else if (arrType == TypeManager.NSValue && !arrIsNullable) {
				if (arrRetType.Name == "RectangleF" || arrRetType.Name == "SizeF" || arrRetType.Name == "PointF")
					valueFetcher = $"{(arrIsNullable ? "?" : string.Empty)}.{arrRetType.Name}Value";
				else if (!NSValueReturnMap.TryGetValue (arrRetType, out valueFetcher))
					throw new BindingException (1049, true, GetBindAsExceptionString ("unbox", retType.Name, arrType.Name, "array", minfo.mi));

				append = string.Format ("ptr => {{\n\tusing (var val = Runtime.GetNSObject<NSValue> (ptr)) {{\n\t\treturn val{0};\n\t}}\n}}", valueFetcher);
			} else
				throw new BindingException (1048, true, "Unsupported type {0} decorated with [BindAs]", arrIsNullable ? arrRetType.Name + "?[]" : retType.Name);
		} else
			throw new BindingException (1048, true, "Unsupported type {0} decorated with [BindAs]", retType.Name);
		return append;
	}

	public static bool HasForcedAttribute (ICustomAttributeProvider cu, out string owns)
	{
		var att = AttributeManager.GetCustomAttribute<ForcedTypeAttribute> (cu);

		if (att == null) {
			var mi = cu as MethodInfo;
			if (mi != null)
				att = AttributeManager.GetCustomAttribute<ForcedTypeAttribute> (mi.ReturnParameter);
		}

		if (att == null) {
			owns = "false";
			return false;
		}

		owns = att.Owns ? "true" : "false";
		return true;
	}

	public string MakeTrampolineName (Type t)
	{
		var trampoline_name = t.Name.Replace ("`", "Arity");
		if (t.IsGenericType) {
			var gdef = t.GetGenericTypeDefinition ();

			if (!trampolines_generic_versions.ContainsKey (gdef))
				trampolines_generic_versions.Add (gdef, 0);

			trampoline_name = trampoline_name + "V" + trampolines_generic_versions [gdef]++;
		}
		return trampoline_name;
	}

	//
	// MakeTrampoline: processes a delegate type and registers a TrampolineInfo with all the information
	// necessary to create trampolines that allow Objective-C blocks to call C# code, and C# code to call
	// Objective-C blocks.
	//
	// @t: A delegate type
	// 
	// This probably should use MarshalInfo to find the correct way of turning
	// the native types into managed types instead of hardcoding the limited
	// values we know about here
	//
	public TrampolineInfo MakeTrampoline (Type t)
	{
		if (trampolines.ContainsKey (t)){
			return trampolines [t];
		} 

		var mi = t.GetMethod ("Invoke");
		var pars = new StringBuilder ();
		var convert = new StringBuilder ();
		var invoke = new StringBuilder ();
		var clear = new StringBuilder  ();
		var postConvert = new StringBuilder ();
		string returntype;
		var returnformat = "return {0};";

		if (mi.ReturnType.IsArray && IsWrappedType (mi.ReturnType.GetElementType())) {
			returntype = "IntPtr";
			returnformat = "return NSArray.FromNSObjects({0}).Handle;";
		}
		else if (IsWrappedType (mi.ReturnType)) {
			returntype = "IntPtr";
			returnformat = "return {0} != null ? {0}.Handle : IntPtr.Zero;";
		} else if (mi.ReturnType == TypeManager.System_String) {
			returntype = "IntPtr";
			returnformat = "return NSString.CreateNative ({0}, true);";
		} else if (IsNativeEnum (mi.ReturnType)) {
			returntype = "IntPtr";
			returnformat = "return (IntPtr) {0};";
		} else {
			returntype = FormatType (mi.DeclaringType, mi.ReturnType);
		}
		
		pars.Append ("IntPtr block");
		var parameters = mi.GetParameters ();
		foreach (var pi in parameters){
			string isForcedOwns;
			var isForced = HasForcedAttribute (pi, out isForcedOwns);

			pars.Append (", ");
			if (pi != parameters [0])
				invoke.Append (", ");
			
			if (IsWrappedType (pi.ParameterType)){
				pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
				if (IsProtocolInterface (pi.ParameterType)) {
					invoke.AppendFormat (" Runtime.GetINativeObject<{1}> ({0}, false)", pi.Name.GetSafeParamName (), pi.ParameterType);
				} else if (isForced) {
					invoke.AppendFormat (" Runtime.GetINativeObject<{1}> ({0}, {2})", pi.Name.GetSafeParamName (), RenderType (pi.ParameterType), isForcedOwns);
				} else {
					invoke.AppendFormat (" Runtime.GetNSObject<{1}> ({0})", pi.Name.GetSafeParamName (), RenderType (pi.ParameterType));
				}
				continue;
			}

			if (Frameworks.HaveCoreMedia) {
				// special case (false) so it needs to be before the _real_ INativeObject check
				if (pi.ParameterType == SampleBufferType){
					pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
					invoke.AppendFormat ("{0} == IntPtr.Zero ? null : new CMSampleBuffer ({0}, false)", pi.Name.GetSafeParamName ());
					continue;
				}
			}

			if (Frameworks.HaveAudioToolbox) {
				if (pi.ParameterType == TypeManager.AudioBuffers){
					pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
					invoke.AppendFormat ("new global::{0}AudioToolbox.AudioBuffers ({1})", Generator.UnifiedAPI ? "" : "MonoTouch.", pi.Name.GetSafeParamName ());
					continue;
				}
			}

			if (TypeManager.INativeObject.IsAssignableFrom (pi.ParameterType)) {
				pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
				invoke.AppendFormat ("new {0} ({1})", pi.ParameterType, pi.Name.GetSafeParamName ());
				continue;
			}

			if (pi.ParameterType.IsByRef){
				var nt = pi.ParameterType.GetElementType ();
				if (pi.IsOut){
					clear.AppendFormat ("{0} = {1};", pi.Name.GetSafeParamName (), nt.IsValueType ? "default (" + FormatType (null, nt) + ")" : "null");
				}
				if (nt.IsValueType){
					string marshal = string.Empty;
					if (nt == TypeManager.System_Boolean)
						marshal = "[System.Runtime.InteropServices.MarshalAs (System.Runtime.InteropServices.UnmanagedType.I1)] ";
					string fnt;
					string invoke_name;
					string arg_byref = pi.IsOut ? "out " : "ref ";
					var nullable = TypeManager.GetUnderlyingNullableType (nt);
					if (nullable != null) {
						fnt = "IntPtr";
						invoke_name = $"__xamarin_nullified__{pi.Position}";
						arg_byref = String.Empty;
						convert.AppendLine ($"{nullable.Name}? {invoke_name} = null;");
						convert.AppendLine ("if (value != IntPtr.Zero)");
						convert.Append ($"\t{invoke_name} = * ({nullable.Name} *) value;");
					} else {
						fnt = FormatType (null, nt);
						invoke_name = pi.Name.GetSafeParamName ();
					}
					pars.AppendFormat ("{3}{0}{1} {2}", arg_byref, fnt, pi.Name.GetSafeParamName (), marshal);
					invoke.AppendFormat ("{0} {1}", pi.IsOut ? "out" : "ref", invoke_name);
					continue;
				} else if (pi.ParameterType.IsByRef) {
					var pname = pi.Name.GetSafeParamName ();
					var refname = $"__xamarin_pref{pi.Position}";
					convert.Append ($"var {refname} = Runtime.GetINativeObject<{RenderType (nt)}> ({pname}, false);");
					pars.Append ($"ref IntPtr {pname}");
					postConvert.Append ($"error = {refname}.GetHandle ();");
					invoke.Append ($"ref {refname}");
					continue;
				}
			} else if (!Compat && IsNativeEnum (pi.ParameterType)) {
				Type underlyingEnumType = TypeManager.GetUnderlyingEnumType (pi.ParameterType);
				pars.AppendFormat ("{0} {1}", GetNativeEnumType (pi.ParameterType), pi.Name.GetSafeParamName ());
				invoke.AppendFormat ("({1}) ({2}) {0}", pi.Name.GetSafeParamName (), FormatType (null, pi.ParameterType), FormatType (null, underlyingEnumType));
				continue;
			} else if (pi.ParameterType.IsValueType){
				pars.AppendFormat ("{0} {1}", FormatType (null, pi.ParameterType), pi.Name.GetSafeParamName ());
				invoke.AppendFormat ("{0}", pi.Name.GetSafeParamName ());
				continue;
			}
		
			if (pi.ParameterType == TypeManager.System_String_Array){
				pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
				invoke.AppendFormat ("NSArray.StringArrayFromHandle ({0})", pi.Name.GetSafeParamName ());
				continue;
			}
			if (pi.ParameterType == TypeManager.System_String){
				pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
				invoke.AppendFormat ("NSString.FromHandle ({0})", pi.Name.GetSafeParamName ());
				continue;
			}

			if (pi.ParameterType.IsArray){
				Type et = pi.ParameterType.GetElementType ();
				if (IsWrappedType (et)){
					pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
					invoke.AppendFormat ("NSArray.ArrayFromHandle<{0}> ({1})", FormatType (null, et), pi.Name.GetSafeParamName ());
					continue;
				}
			}

			if (pi.ParameterType.IsSubclassOf (TypeManager.System_Delegate)){
				if (!delegate_types.ContainsKey (pi.ParameterType.Name)){
					delegate_types [pi.ParameterType.FullName] = pi.ParameterType.GetMethod ("Invoke");
				}
				if (AttributeManager.HasAttribute<BlockCallbackAttribute> (pi)) {
					pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
					invoke.AppendFormat ("NID{0}.Create ({1})", MakeTrampolineName (pi.ParameterType), pi.Name.GetSafeParamName ());
					// The trampoline will eventually be generated in the final loop
				} else {
					if (!AttributeManager.HasAttribute<CCallbackAttribute> (pi)) {
						if (t.FullName.StartsWith ("System.Action`", StringComparison.Ordinal) || t.FullName.StartsWith ("System.Func`", StringComparison.Ordinal)) {
							ErrorHelper.Show (new BindingException (1116, "The parameter '{0}' in the delegate '{1}' does not have a [CCallback] or [BlockCallback] attribute. Defaulting to [CCallback]. Declare a custom delegate instead of using System.Action / System.Func and add the attribute on the corresponding parameter.", pi.Name.GetSafeParamName (), t.FullName));
						} else {
							ErrorHelper.Show (new BindingException (1115, "The parameter '{0}' in the delegate '{1}' does not have a [CCallback] or [BlockCallback] attribute. Defaulting to [CCallback].", pi.Name.GetSafeParamName (), t.FullName));
						}
					}
					pars.AppendFormat ("IntPtr {0}", pi.Name.GetSafeParamName ());
					invoke.AppendFormat ("({0}) Marshal.GetDelegateForFunctionPointer ({1}, typeof ({0}))", pi.ParameterType, pi.Name.GetSafeParamName ());
				}
				continue;
			}
			
			throw new BindingException (1001, true, $"Do not know how to make a trampoline for {pi.ParameterType.FullName}");
		}

		var rt = mi.ReturnType;
		var rts = IsNativeEnum (rt) ? "var" : rt.ToString ();
		var trampoline_name = MakeTrampolineName (t);
		var ti = new TrampolineInfo (userDelegate: FormatType (null, t),
					     delegateName: "D" + trampoline_name,
					     trampolineName: "T" + trampoline_name,
					     pars: pars.ToString (),
		                             convert: convert.ToString (),
					     invoke: invoke.ToString (),
					     returnType: returntype,
		                             delegateReturnType: rts,
					     returnFormat: returnformat,
					     clear: clear.ToString (),
		                             postConvert: postConvert.ToString (),
					     type: t);
					     

		ti.UserDelegateTypeAttribute = FormatType (null, t);
		trampolines [t] = ti;
			
		return ti;
	}
	
	//
	// Returns the actual way in which the type t must be marshalled
	// for example "UIView foo" is generated as  "foo.Handle"
	//
	public string MarshalParameter (MethodInfo mi, ParameterInfo pi, bool null_allowed_override, EnumMode enum_mode, PropertyInfo propInfo = null)
	{
		if (pi.ParameterType.IsByRef && pi.ParameterType.GetElementType ().IsValueType == false){
			return "ref " + pi.Name + "Value";
		}

		if (HasBindAsAttribute (pi))
			return string.Format ("nsb_{0} == null ? IntPtr.Zero : nsb_{0}.Handle", pi.Name);
		if (propInfo != null && HasBindAsAttribute (propInfo))
			return string.Format ("nsb_{0} == null ? IntPtr.Zero : nsb_{0}.Handle", propInfo.Name);

		if (IsWrappedType (pi.ParameterType)){
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return String.Format ("{0} == null ? IntPtr.Zero : {0}.Handle", pi.Name.GetSafeParamName ());
			return pi.Name.GetSafeParamName () + ".Handle";
		}
		
		if (enum_mode != EnumMode.Compat && enum_mode != EnumMode.NativeBits && pi.ParameterType.IsEnum)
			return "(" + PrimitiveType (pi.ParameterType, enum_mode: enum_mode) + ")" + pi.Name.GetSafeParamName ();

		if (enum_mode == EnumMode.NativeBits && IsNativeEnum (pi.ParameterType) && !Compat)
			return "(" + GetNativeEnumType (pi.ParameterType) + ") (" + PrimitiveType (TypeManager.GetUnderlyingEnumType (pi.ParameterType)) + ") " + pi.Name.GetSafeParamName ();
		
		if (IsNativeType (pi.ParameterType))
			return pi.Name.GetSafeParamName ();

		if (pi.ParameterType == TypeManager.System_String){
			var mai = new MarshalInfo (mi, pi);
			if (mai.PlainString)
				return pi.Name.GetSafeParamName ();
			else {
				bool allow_null = null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi);
				
				if (mai.ZeroCopyStringMarshal){
					if (allow_null)
						return String.Format ("{0} == null ? IntPtr.Zero : (IntPtr)(&_s{0})", pi.Name);
					else
						return String.Format ("(IntPtr)(&_s{0})", pi.Name);
				} else {
					return "ns" + pi.Name;
				}
			}
		}

		if (pi.ParameterType.IsValueType)
			return pi.Name.GetSafeParamName ();

		MarshalType mt;
		if (LookupMarshal (pi.ParameterType, out mt)){
			string access = String.Format (mt.ParameterMarshal, pi.Name.GetSafeParamName ());
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return String.Format ("{0} == null ? IntPtr.Zero : {1}", pi.Name.GetSafeParamName (), access);
			return access;
		}

		if (pi.ParameterType.IsArray){
			//Type etype = pi.ParameterType.GetElementType ();

			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return String.Format ("nsa_{0} == null ? IntPtr.Zero : nsa_{0}.Handle", pi.Name);
			return "nsa_" + pi.Name + ".Handle";
		}

		//
		// Handle (out ValueType foo)
		//
		if (pi.ParameterType.IsByRef) {
			var et = pi.ParameterType.GetElementType ();
			var nullable = TypeManager.GetUnderlyingNullableType (et);
			if (nullable != null) {
				return "converted";
			} else if (et.IsValueType)
				return (TypeManager.IsOutParameter (pi) ? "out " : "ref ") + pi.Name.GetSafeParamName ();
		}

		if (pi.ParameterType.IsSubclassOf (TypeManager.System_Delegate)){
			return String.Format ("(IntPtr) block_ptr_{0}", pi.Name);
		}

		if (IsDictionaryContainerType(pi.ParameterType)){
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return String.Format ("{0} == null ? IntPtr.Zero : {0}.Dictionary.Handle", pi.Name.GetSafeParamName ());
			return pi.Name.GetSafeParamName () + ".Dictionary.Handle";
		}

		if (pi.ParameterType.IsGenericParameter) {
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return string.Format ("{0} == null ? IntPtr.Zero : {0}.Handle", pi.Name.GetSafeParamName ());
			return pi.Name.GetSafeParamName () + ".Handle";
		}

		// This means you need to add a new MarshalType in the method "Go"
		throw new BindingException (1002, true, "Unknown kind {0} in method '{1}.{2}'", pi.ParameterType.FullName, mi.DeclaringType.FullName, mi.Name.GetSafeParamName ());
	}

	public bool ParameterNeedsNullCheck (ParameterInfo pi, MethodInfo mi, PropertyInfo propInfo = null)
	{
		if (pi.ParameterType.IsByRef)
			return false;

		if (AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
			return false;

		if (IsSetter (mi)) {
			if (AttributeManager.HasAttribute<NullAllowedAttribute> (mi)) {
				return false;
			}
		}

		var bindAsAtt = GetBindAsAttribute (pi) ?? GetBindAsAttribute (propInfo);
		if (bindAsAtt != null)
			return bindAsAtt.IsNullable || !bindAsAtt.IsValueType;

		if (IsWrappedType (pi.ParameterType))
			return true;

		return !pi.ParameterType.IsValueType;
	}

	public BindAttribute GetBindAttribute (MethodInfo mi)
	{
		return AttributeManager.GetCustomAttribute<BindAttribute> (mi);
	}

	public static bool HasAttribute<T> (ICustomAttributeProvider i, Attribute [] attributes) where T: Attribute
	{
		if (attributes == null)
			return AttributeManager.HasAttribute<T> (i);
		else
			foreach (var a in attributes)
				if (a.GetType () == typeof (T))
					return true;
		return false;
	}

	public static bool ShouldMarshalNativeExceptions (MethodInfo mi)
	{
		// [MarshalNativeExceptions] should work on a property and inside the get / set
		// If we have it directly on our method, good enough
		if (AttributeManager.HasAttribute<MarshalNativeExceptionsAttribute> (mi))
			return true;

		// Else look up to see if we are part of a property and look for the attribute there
		PropertyInfo owningProperty = mi.DeclaringType.GetProperties ()
			.FirstOrDefault(prop =>  prop.GetSetMethod() == mi ||
					prop.GetGetMethod() == mi);
		if (owningProperty != null && AttributeManager.HasAttribute<MarshalNativeExceptionsAttribute> (owningProperty))
			return true;

		return false;
	}

	public bool IsTarget (ParameterInfo pi)
	{
		var is_target = AttributeManager.HasAttribute<TargetAttribute> (pi);
		if (is_target && UnifiedAPI) {
			throw new BindingException (1031, true,
				"The [Target] attribute is not supported for the Unified API (found on the member '{0}.{1}'). " +
				"For Objective-C categories, create an api definition interface with the [Category] attribute instead.",
				pi.Member.DeclaringType.FullName, pi.Member.Name.GetSafeParamName ());
		}
		return is_target;
	}
	
	//
	// Makes the method name for a objcSend call
	//
	string MakeSig (string send, bool stret, MethodInfo mi, bool aligned, EnumMode enum_mode = EnumMode.Compat)
	{
		var sb = new StringBuilder ();
		
		if (ShouldMarshalNativeExceptions (mi))
			sb.Append ("xamarin_");
		
		try {
			sb.Append (ParameterGetMarshalType (new MarshalInfo (mi) { IsAligned = aligned, EnumMode = enum_mode } ));
		} catch (BindingException ex) {
			throw new BindingException (ex.Code, ex.Error, ex,  "{0} in method `{1}'", ex.Message, mi.Name);
		}

		sb.Append ("_");
		sb.Append (send);
		if (stret)
			sb.Append ("_stret");
		
		foreach (var pi in mi.GetParameters ()){
			if (IsTarget (pi))
				continue;
			sb.Append ("_");
			try {
				sb.Append (ParameterGetMarshalType (new MarshalInfo (mi, pi) { EnumMode = enum_mode }).Replace (' ', '_'));
			} catch (BindingException ex) {
				throw new BindingException (ex.Code, ex.Error, ex, "{0} in parameter `{1}' from {2}.{3}", ex.Message, pi.Name.GetSafeParamName (), mi.DeclaringType, mi.Name);
			}
		}

		var marshalDirective = AttributeManager.GetCustomAttribute<MarshalDirectiveAttribute> (mi);
		if (marshalDirective != null) {
			if (!string.IsNullOrEmpty (marshalDirective.NativePrefix))
				sb.Insert (0, marshalDirective.NativePrefix);
			if (!string.IsNullOrEmpty (marshalDirective.NativeSuffix))
				sb.Append (marshalDirective.NativeSuffix);
		}

		return sb.ToString ();
	}

	void RegisterMethod (bool need_stret, MethodInfo mi, string method_name, bool aligned, EnumMode enum_mode = EnumMode.Compat)
	{
		if (send_methods.ContainsKey (method_name))
			return;
		send_methods [method_name] = method_name;

		var b = new StringBuilder ();
		int n = 0;
		
		foreach (var pi in mi.GetParameters ()){
			if (IsTarget (pi))
				continue;

			b.Append (", ");

			try {
				b.Append (ParameterGetMarshalType (new MarshalInfo (mi, pi) { EnumMode = enum_mode }, true));
			} catch (BindingException ex) {
				throw new BindingException (ex.Code, ex.Error, ex, "{0} in parameter {1} of {2}.{3}", ex.Message, pi.Name.GetSafeParamName (), mi.DeclaringType, mi.Name);
			}
			b.Append (" ");
			b.Append ("arg" + (++n));
		}

		string entry_point;
		if (method_name.IndexOf ("objc_msgSendSuper", StringComparison.Ordinal) != -1) {
			entry_point = need_stret ? "objc_msgSendSuper_stret" : "objc_msgSendSuper";
		} else
			entry_point = need_stret ? "objc_msgSend_stret" : "objc_msgSend";

		var marshalDirective = AttributeManager.GetCustomAttribute<MarshalDirectiveAttribute> (mi);
		if (marshalDirective != null && marshalDirective.Library != null) {
			print (m, "\t\t[DllImport (\"{0}\", EntryPoint=\"{1}\")]", marshalDirective.Library, method_name);
		} else if (method_name.StartsWith ("xamarin_", StringComparison.Ordinal)) {
			print (m, "\t\t[DllImport (\"__Internal\", EntryPoint=\"{0}\")]", method_name);
		} else {
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"{0}\")]", entry_point);
		}

		print (m, "\t\tpublic extern static {0} {1} ({3}IntPtr receiver, IntPtr selector{2});",
		       need_stret ? "void" : ParameterGetMarshalType (new MarshalInfo (mi) { EnumMode = enum_mode }, true), method_name, b.ToString (),
		       need_stret ? (aligned ? "IntPtr" : "out " + FormatTypeUsedIn (ns.CoreObjCRuntime, mi.ReturnType)) + " retval, " : "");
	}

	bool IsNativeEnum (Type type)
	{
		return type.IsEnum && AttributeManager.HasAttribute<NativeAttribute> (type);
	}

	// nint or nuint
	string GetNativeEnumType (Type type)
	{
		var underlyingEnumType = TypeManager.GetUnderlyingEnumType (type);
		if (TypeManager.System_Int64 == underlyingEnumType) {
			return "nint";
		} else if (TypeManager.System_UInt64 == underlyingEnumType) {
			return "nuint";
		} else {
			throw new BindingException (1029, "Internal error: invalid enum type '{0}'", type);
		}
	}

	bool HasNativeEnumInSignature (MethodInfo mi)
	{
		if (Compat)
			return false;

		if (IsNativeEnum (mi.ReturnType))
			return true;
		
		foreach (var p in mi.GetParameters ())
			if (IsNativeEnum (p.ParameterType))
				return true;

		return false;
	}

	void DeclareInvoker (MethodInfo mi)
	{
		if (AttributeManager.HasAttribute<WrapAttribute> (mi))
			return;

		try {
			if (Compat) {
				var returnType = mi.ReturnType;
				bool arm_stret = Stret.ArmNeedStret (returnType);
				bool is_aligned = AttributeManager.HasAttribute<AlignAttribute> (mi);
				RegisterMethod (arm_stret, mi, MakeSig (mi, arm_stret, arm_stret && is_aligned), arm_stret && is_aligned);
				RegisterMethod (arm_stret, mi, MakeSuperSig (mi, arm_stret, arm_stret && is_aligned), arm_stret && is_aligned);

				bool x86_stret = Stret.X86NeedStret (returnType);
				if (x86_stret != arm_stret){
					RegisterMethod (x86_stret, mi, MakeSig (mi, x86_stret, x86_stret && is_aligned), x86_stret && is_aligned);
					RegisterMethod (x86_stret, mi, MakeSuperSig (mi, x86_stret, x86_stret && is_aligned), x86_stret && is_aligned);
				}
			} else {
				EnumMode[] modes;
				if (HasNativeEnumInSignature (mi)) {
					modes = new EnumMode[] { EnumMode.Bit32, EnumMode.Bit64 };
				} else {
					modes = new EnumMode[] { EnumMode.Bit32 };
				}
				foreach (var mode in modes) {
					// arm64 never requires stret, so we'll always need the non-stret variants
					RegisterMethod (false, mi, MakeSig (mi, false, enum_mode: mode), false, mode);
					RegisterMethod (false, mi, MakeSuperSig (mi, false, enum_mode: mode), false, mode);

					if (CheckNeedStret (mi)) {
						RegisterMethod (true, mi, MakeSig (mi, true, enum_mode: mode), false, mode);
						RegisterMethod (true, mi, MakeSuperSig (mi, true, enum_mode: mode), false, mode);

						if (AttributeManager.HasAttribute<AlignAttribute> (mi)) {
							RegisterMethod (true, mi, MakeSig (mi, true, true, mode), true, mode);
							RegisterMethod (true, mi, MakeSuperSig (mi, true, true, mode), true, mode);
						}
					}
				}
			}
		} catch (BindingException ex) {
			throw ex;
		}
	}
	static char [] invalid_selector_chars = new char [] { '*', '^', '(', ')' };

	public static ExportAttribute GetExportAttribute (MemberInfo mo)
	{
		string dummy;
		return GetExportAttribute (mo, out dummy);
	}

	//
	// Either we have an [Export] attribute, or we have a [Wrap] one
	//
	public static ExportAttribute GetExportAttribute (MemberInfo mo, out string wrap)
	{
		wrap = null;
		object [] attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (mo);
		if (attrs.Length == 0) {
			attrs = AttributeManager.GetCustomAttributes<WrapAttribute> (mo);
			if (attrs.Length != 0) {
				wrap = ((WrapAttribute) attrs [0]).MethodName;
				return null;
			}
			PropertyInfo pi = mo as PropertyInfo;
			if (pi != null && pi.CanRead) {
				var getter = pi.GetGetMethod (true);
				attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (getter);
			}
			if (attrs.Length == 0)
				return null;
		}
		
		var export = (ExportAttribute) attrs [0];

		if (string.IsNullOrEmpty (export.Selector))
			throw new BindingException (1024, true, "No selector specified for member '{0}.{1}'", mo.DeclaringType.FullName, mo.Name);

		if (export.Selector.IndexOfAny (invalid_selector_chars) != -1){
			Console.Error.WriteLine ("Export attribute contains invalid selector name: {0}", export.Selector);
			Environment.Exit (1);
		}
		
		return export;
	}

	public static ExportAttribute GetSetterExportAttribute (PropertyInfo pinfo)
	{
		var ea = AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo.GetSetMethod ());
		if (ea != null && ea.Selector != null)
			return ea;
		return AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo)?.ToSetter (pinfo);
	}

	public static ExportAttribute GetGetterExportAttribute (PropertyInfo pinfo)
	{
		var ea = AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo.GetGetMethod ());
		if (ea != null && ea.Selector != null)
			return ea;
		return AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo).ToGetter (pinfo);
	}

	public static Generator SharedGenerator;
	
	public Generator (NamespaceManager nsm, bool is_public_mode, bool external, bool debug, Type [] types, Type [] strong_dictionaries)
	{
		ns = nsm;
		Generator.IsPublicMode = is_public_mode;
		this.external = external;
		this.debug = debug;
		this.types = types;
		this.strong_dictionaries = strong_dictionaries;
		basedir = ".";
		SharedGenerator = this;
	}

	bool SkipGenerationOfType (Type t)
	{
		if (t.IsUnavailable ())
			return true;

		if (Compat)
			return AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (t)
				.Any (attr => attr.AvailabilityKind == AvailabilityKind.Introduced &&
					attr.Platform == Generator.CurrentPlatform &&
					attr.Architecture == PlatformArchitecture.Arch64);

		return false;
	}

	public void Go ()
	{
		marshal_types.Add (new MarshalType (TypeManager.NSObject, create: "Runtime.GetNSObject ("));
		marshal_types.Add (new MarshalType (TypeManager.Selector, create: "Selector.FromHandle ("));
		marshal_types.Add (new MarshalType (TypeManager.BlockLiteral, "BlockLiteral", "{0}", "THIS_IS_BROKEN"));
		if (TypeManager.MusicSequence != null)
			marshal_types.Add (new MarshalType (TypeManager.MusicSequence, create: "global::AudioToolbox.MusicSequence.Lookup ("));
		marshal_types.Add (TypeManager.CGColor);
		marshal_types.Add (TypeManager.CGPath);
		marshal_types.Add (TypeManager.CGGradient);
		marshal_types.Add (TypeManager.CGContext);
		marshal_types.Add (TypeManager.CGPDFDocument);
		marshal_types.Add (TypeManager.CGPDFPage);
		marshal_types.Add (TypeManager.CGImage);
		marshal_types.Add (TypeManager.Class);
		marshal_types.Add (TypeManager.CFRunLoop);
		marshal_types.Add (TypeManager.CGColorSpace);
		marshal_types.Add (TypeManager.DispatchQueue);
		marshal_types.Add (TypeManager.Protocol);
		if (Frameworks.HaveCoreMidi)
			marshal_types.Add (TypeManager.MidiEndpoint);
		if (Frameworks.HaveCoreMedia) {
			marshal_types.Add (TypeManager.CMTimebase);
			marshal_types.Add (TypeManager.CMClock);
		}
		marshal_types.Add (TypeManager.NSZone);
		if (Frameworks.HaveOpenGL) {
			marshal_types.Add (TypeManager.CGLContext);
			marshal_types.Add (TypeManager.CGLPixelFormat);
			marshal_types.Add (TypeManager.CVImageBuffer);
		}
		if (Frameworks.HaveMediaToolbox)
			marshal_types.Add (new MarshalType (TypeManager.MTAudioProcessingTap, create: (NamespaceManager.Get ("MediaToolbox") + ".MTAudioProcessingTap.FromHandle(")));
		if (Frameworks.HaveAddressBook) {
			marshal_types.Add (TypeManager.ABAddressBook);
			marshal_types.Add (new MarshalType (TypeManager.ABPerson, create: "(ABPerson) ABRecord.FromHandle("));
			marshal_types.Add (new MarshalType (TypeManager.ABRecord, create: "ABRecord.FromHandle("));
		}
		if (Frameworks.HaveCoreVideo) {
			// owns `false` like ptr ctor https://github.com/xamarin/xamarin-macios/blob/6f68ab6f79c5f1d96d2cbb1e697330623164e46d/src/CoreVideo/CVBuffer.cs#L74-L90
			marshal_types.Add (new MarshalType (TypeManager.CVPixelBuffer, create: "Runtime.GetINativeObject<CVPixelBuffer> (", closingCreate: ", false)"));
		}
		marshal_types.Add (TypeManager.CGLayer);
		if (Frameworks.HaveCoreMedia)
			marshal_types.Add (TypeManager.CMSampleBuffer);

		if (Frameworks.HaveCoreVideo) {
			marshal_types.Add (TypeManager.CVImageBuffer);
			marshal_types.Add (TypeManager.CVPixelBufferPool);
		}
		if (Frameworks.HaveAudioUnit)
			marshal_types.Add (TypeManager.AudioComponent);
		if (Frameworks.HaveCoreMedia) {
			marshal_types.Add (new MarshalType (TypeManager.CMFormatDescription, create: "CMFormatDescription.Create ("));
			marshal_types.Add (TypeManager.CMAudioFormatDescription);
			marshal_types.Add (TypeManager.CMVideoFormatDescription);
		}
		if (Frameworks.HaveAudioUnit)
			marshal_types.Add (TypeManager.AudioUnit);
		marshal_types.Add (TypeManager.SecIdentity);
		marshal_types.Add (TypeManager.SecTrust);
		marshal_types.Add (TypeManager.SecAccessControl);
		marshal_types.Add (TypeManager.AudioBuffers);
		if (Frameworks.HaveAudioUnit) {
			marshal_types.Add (TypeManager.AURenderEventEnumerator);
		}

		m = GetOutputStream ("ObjCRuntime", "Messaging");
		Header (m);
		print (m, "namespace {0} {{", ns.ObjCRuntime);
		print (m, "\t{0}partial class Messaging {{", Compat ? "public " : String.Empty);

		if (BindThirdPartyLibrary){
			print (m, "\t\tstatic internal System.Reflection.Assembly this_assembly = typeof (Messaging).Assembly;\n");
			print (m, "\t\tconst string LIBOBJC_DYLIB = \"/usr/lib/libobjc.dylib\";\n");
			// IntPtr_objc_msgSend[Super]: for init
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSend (IntPtr receiever, IntPtr selector);");
			send_methods ["IntPtr_objc_msgSend"] = "IntPtr_objc_msgSend";
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSendSuper (IntPtr receiever, IntPtr selector);");
			send_methods ["IntPtr_objc_msgSendSuper"] = "IntPtr_objc_msgSendSuper";
			// IntPtr_objc_msgSendSuper_IntPtr: for initWithCoder:
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
			send_methods ["IntPtr_objc_msgSend_IntPtr"] = "IntPtr_objc_msgSend_IntPtr";
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
			send_methods ["IntPtr_objc_msgSendSuper_IntPtr"] = "IntPtr_objc_msgSendSuper_IntPtr";
		}

		Array.Sort (types, (a, b) => string.CompareOrdinal (a.FullName, b.FullName));

		foreach (Type t in types){
			if (SkipGenerationOfType (t))
				continue;

			// We call lookup to build the hierarchy graph
			GeneratedType.Lookup (t);
			
			var tselectors = new List<string> ();
			
			foreach (var pi in GetTypeContractProperties (t)){
				if (pi.IsUnavailable ())
					continue;

				if (AttributeManager.HasAttribute<IsThreadStaticAttribute> (pi) && !AttributeManager.HasAttribute<StaticAttribute> (pi))
					throw new BindingException (1008, true, "[IsThreadStatic] is only valid on properties that are also [Static]");

				string wrapname;
				var export = GetExportAttribute (pi, out wrapname);
				if (export == null){
					if (wrapname != null)
						continue;

					// Let properties with the [Field] attribute through as well.
					if (AttributeManager.HasAttribute<FieldAttribute> (pi))
						continue;


					if (AttributeManager.HasAttribute<CoreImageFilterPropertyAttribute> (pi))
						continue;

					if (AttributeManager.HasAttribute<WrapAttribute> (pi.GetGetMethod ()) || AttributeManager.HasAttribute<WrapAttribute> (pi.GetSetMethod ()))
						continue;

					throw new BindingException (1018, true, "No [Export] attribute on property {0}.{1}", t.FullName, pi.Name);
				}
				if (AttributeManager.HasAttribute<StaticAttribute> (pi))
					need_static [t] = true;

				bool is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (pi) && pi.DeclaringType == t;
				
				if (pi.CanRead){
					MethodInfo getter = pi.GetGetMethod ();
					BindAttribute ba = GetBindAttribute (getter);

					if (!is_abstract)
						tselectors.Add (ba != null ? ba.Selector : export.Selector);
					DeclareInvoker (getter);
				}
				
				if (pi.CanWrite){
					MethodInfo setter = pi.GetSetMethod ();
					BindAttribute ba = GetBindAttribute (setter);
					var notImpl = AttributeManager.HasAttribute<NotImplementedAttribute> (setter);
					
					if (!is_abstract && !notImpl)
						tselectors.Add (ba != null ? ba.Selector : GetSetterExportAttribute (pi).Selector);
					DeclareInvoker (setter);
				}
			}
			
			foreach (var mi in GetTypeContractMethods (t)){
				// Skip properties
				if (mi.IsSpecialName)
					continue;

				if (mi.IsUnavailable ())
					continue;

				bool seenAbstract = false;
				bool seenDefaultValue = false;
				bool seenNoDefaultValue = false;

				foreach (var attr in AttributeManager.GetCustomAttributes<System.Attribute> (mi)) {
					string selector = null;
					ExportAttribute ea = attr as ExportAttribute;
					BindAttribute ba = attr as BindAttribute;
					if (ea != null){
						selector = ea.Selector;
					} else if (ba != null){
						selector = ba.Selector;
					} else if (attr is StaticAttribute){
						need_static [t] = true;
						continue;
					} else if (attr is InternalAttribute || attr is UnifiedInternalAttribute || attr is ProtectedAttribute){
						continue;
					} else if (attr is NeedsAuditAttribute) {
						continue;
					} else if (attr is FactoryAttribute){
						continue;
					} else  if (attr is AbstractAttribute){
						if (mi.DeclaringType == t)
							need_abstract [t] = true;
						seenAbstract = true;
						continue;
					} else if (attr is DefaultValueAttribute || attr is DefaultValueFromArgumentAttribute) {
						seenDefaultValue = true;
						continue;
					} else if (attr is NoDefaultValueAttribute) {
						seenNoDefaultValue = true;
						continue;
					} else if (attr is SealedAttribute || attr is EventArgsAttribute || attr is DelegateNameAttribute || attr is EventNameAttribute || attr is IgnoredInDelegateAttribute || attr is ObsoleteAttribute || attr is NewAttribute || attr is PostGetAttribute || attr is NullAllowedAttribute || attr is CheckDisposedAttribute || attr is SnippetAttribute || attr is AppearanceAttribute || attr is ThreadSafeAttribute || attr is AutoreleaseAttribute || attr is EditorBrowsableAttribute || attr is AdviceAttribute || attr is OverrideAttribute || attr is DelegateApiNameAttribute || attr is ForcedTypeAttribute)
						continue;
					else if (attr is MarshalNativeExceptionsAttribute)
						continue;
					else if (attr is WrapAttribute)
						continue;
					else if (attr is AsyncAttribute)
						continue;
					else if (attr is DesignatedInitializerAttribute)
						continue;
					else if (attr is DesignatedDefaultCtorAttribute)
						continue;
					else if (attr is AvailabilityBaseAttribute)
						continue;
					else if (attr is RequiresSuperAttribute)
						continue;
					else {
						switch (attr.GetType ().Name) {
						case "PreserveAttribute":
						case "CompilerGeneratedAttribute":
						case "ManualAttribute":
						case "MarshalDirectiveAttribute":
						case "BindingImplAttribute":
							continue;
						default:
							throw new BindingException (1007, true, "Unknown attribute {0} on {1}.{2}", attr.GetType (), mi.DeclaringType, mi.Name);
						}
					}

					if (selector == null)
						throw new BindingException (1009, true, "No selector specified for method `{0}.{1}'", mi.DeclaringType, mi.Name);
					
					tselectors.Add (selector);
					if (selector_use.ContainsKey (selector)){
						selector_use [selector]++;
					} else
						selector_use [selector] = 1;
				}

				if (seenNoDefaultValue && seenAbstract)
					throw new BindingException (1019, true, "Cannot use [NoDefaultValue] on abstract method `{0}.{1}'", mi.DeclaringType, mi.Name);
				else if (seenNoDefaultValue && seenDefaultValue)
					throw new BindingException (1019, true, "Cannot use both [NoDefaultValue] and [DefaultValue] on method `{0}.{1}'", mi.DeclaringType, mi.Name);

				DeclareInvoker (mi);
			}

			foreach (var pi in t.GatherProperties (BindingFlags.Instance | BindingFlags.Public)){
				if (pi.IsUnavailable ())
					continue;

				if (AttributeManager.HasAttribute<AbstractAttribute> (pi) && pi.DeclaringType == t)
					need_abstract [t] = true;
			}
			
			selectors [t] = tselectors.Distinct ();
		}

		foreach (Type t in types){
			if (SkipGenerationOfType (t))
				continue;

			Generate (t);
		}

		//DumpChildren (0, GeneratedType.Lookup (TypeManager.NSObject));
		
		print (m, "\t}\n}");
		m.Close ();

		// Generate strong argument types
		GenerateStrongDictionaryTypes ();
		
		// Generate the event arg mappings
		if (notification_event_arg_types.Count > 0)
			GenerateEventArgsFile ();

		if (delegate_types.Count > 0)
			GenerateIndirectDelegateFile ();

		if (trampolines.Count > 0)
			GenerateTrampolines ();

		if (libraries.Count > 0)
			GenerateLibraryHandles ();
	}

	static string GenerateNSValue (string propertyToCall)
	{
		return "using (var nsv = Runtime.GetNSObject<NSValue> (value))\n\treturn nsv." + propertyToCall + ";";
	}

	static string GenerateNSNumber (string cast, string propertyToCall)
	{
		return "using (var nsn = Runtime.GetNSObject<NSNumber> (value))\n\treturn " + cast + "nsn." + propertyToCall + ";";
	}

	static Type GetCorrectGenericType (Type type)
	{
		if (UnifiedAPI)
			return type;

		if (type != null && type.IsGenericType) {
			// for compat we expose NSSet/NSDictionary instead of NSSet<TKey>/NSDictionary<TKey,TValue>
			var bt = type.GetGenericTypeDefinition ();

			switch (bt.FullName) {
			case "MonoTouch.Foundation.NSDictionary`2":
			case "MonoMac.Foundation.NSDictionary`2":
				return bt.Assembly.GetType (bt.Namespace + ".NSDictionary");
			case "MonoTouch.Foundation.NSMutableDictionary`2":
			case "MonoMac.Foundation.NSMutableDictionary`2":
				return bt.Assembly.GetType (bt.Namespace + ".NSMutableDictionary");
			case "MonoTouch.Foundation.NSSet`1":
			case "MonoMac.Foundation.NSSet`1":
				return bt.Assembly.GetType (bt.Namespace + ".NSSet");
			case "MonoTouch.Foundation.NSMutableSet`1":
			case "MonoMac.Foundation.NSMutableSet`1":
				return bt.Assembly.GetType (bt.Namespace + ".NSMutableSet");
			case "MonoTouch.Foundation.NSMutableArray`1":
			case "MonoMac.Foundation.NSMutableArray`1":
				return bt.Assembly.GetType (bt.Namespace + ".NSMutableArray");
			default:
				return type;
			}
		}
		return type;
	}

	void GenerateIndirectDelegateFile ()
	{
		sw = GetOutputStream (null, "SupportDelegates");
		Header (sw);
		RenderDelegates (delegate_types);
		sw.Close ();
	}

	void GenerateTrampolines ()
	{
		sw = GetOutputStream ("ObjCRuntime", "Trampolines");

		Header (sw);
		print ("namespace {0} {{", ns.CoreObjCRuntime); indent++;
		print ("");
		print_generated_code ();
		print ("static partial class Trampolines {"); indent++;

		print ("");
		print ("[DllImport (\"/usr/lib/libobjc.dylib\")]");
		print ("static extern IntPtr _Block_copy (IntPtr ptr);");

		while (trampolines.Count > 0){
			var queue = trampolines.Values.ToArray ();
			trampolines.Clear ();
				
			GenerateTrampolinesForQueue (queue);
		}
		indent--; print ("}");
		indent--; print ("}");
		sw.Close ();
	}

	Dictionary<Type,bool> generated_trampolines = new Dictionary<Type,bool> ();
	
	void GenerateTrampolinesForQueue (TrampolineInfo [] queue)
	{
		Array.Sort (queue, (a, b) => string.CompareOrdinal (a.Type.FullName, b.Type.FullName));
		foreach (var ti in queue) {
			if (generated_trampolines.ContainsKey (ti.Type))
				continue;
			generated_trampolines [ti.Type] = true;
			
			var mi = ti.Type.GetMethod ("Invoke");
			var parameters = mi.GetParameters ();

			print ("");
			print ("[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]");
			print ("[UserDelegateType (typeof ({0}))]", ti.UserDelegate);
			print ("internal delegate {0} {1} ({2});", ti.ReturnType, ti.DelegateName, ti.Parameters);
			print ("");
			print ("//\n// This class bridges native block invocations that call into C#\n//");
			print ("static internal class {0} {{", ti.StaticName); indent++;
			print ("static internal readonly {0} Handler = {1};", ti.DelegateName, ti.TrampolineName);
			print ("");
			print ("[MonoPInvokeCallback (typeof ({0}))]", ti.DelegateName);
			print ("static unsafe {0} {1} ({2}) {{", ti.ReturnType, ti.TrampolineName, ti.Parameters);
			indent++;
			print ("var descriptor = (BlockLiteral *) block;");
			print ("var del = ({0}) (descriptor->Target);", ti.UserDelegate);
			bool is_void = ti.ReturnType == "void";
			// FIXME: right now we only support 'null' when the delegate does not return a value
			// otherwise we will need to know the default value to be returned (likely uncommon)
			if (is_void) {
				print ("if (del != null)");
				indent++;
				print ("del ({0});", ti.Invoke);
				indent--;
				if (ti.Clear.Length > 0){
					print ("else");
					indent++;
					print (ti.Clear);
					indent--;
				}
			} else {
				if (ti.Convert.Length > 0)
					print (ti.Convert);
				print ("{0} retval = del ({1});", ti.DelegateReturnType, ti.Invoke);
				if (ti.PostConvert.Length > 0)
					print (ti.PostConvert);
				print (ti.ReturnFormat, "retval");
			}
			indent--;
			print ("}"); 
			indent--;
			print ("}} /* class {0} */", ti.StaticName);

			//
			// Now generate the class that allows us to invoke a Objective-C block from C#
			//
			print ("");
			print ("internal class {0} {{", ti.NativeInvokerName);
			indent++;
			print ("IntPtr blockPtr;");
			print ("{0} invoker;", ti.DelegateName);
			print ("");
			print ("[Preserve (Conditional=true)]");
			print_generated_code ();
			print ("public unsafe {0} (BlockLiteral *block)", ti.NativeInvokerName);
			print ("{"); indent++;
			print ("blockPtr = _Block_copy ((IntPtr) block);", ns.CoreObjCRuntime);
			print ("invoker = block->GetDelegateForBlock<{0}> ();", ti.DelegateName);
			indent--; print ("}");
			print ("");
			print ("[Preserve (Conditional=true)]");
			print_generated_code ();
			print ("~{0} ()", ti.NativeInvokerName);
			print ("{"); indent++;
			print ("Runtime.ReleaseBlockOnMainThread (blockPtr);", ns.CoreObjCRuntime);
			indent--; print ("}");
			print ("");
			print ("[Preserve (Conditional=true)]");
			print_generated_code ();
			print ("public unsafe static {0} Create (IntPtr block)\n{{", ti.UserDelegate); indent++;
			print ("if (block == IntPtr.Zero)"); indent++;
			print ("return null;"); indent--;
			print ("if (BlockLiteral.IsManagedBlock (block)) {"); indent++;
			print ("var existing_delegate = ((BlockLiteral *) block)->Target as {0};", ti.UserDelegate);
			print ("if (existing_delegate != null)"); indent++;
			print ("return existing_delegate;"); indent--;
			indent--; print ("}"); 
			print ("return new {0} ((BlockLiteral *) block).Invoke;", ti.NativeInvokerName);
			indent--;print ("}");
			print ("");
			var string_pars = new StringBuilder ();
			MakeSignatureFromParameterInfo (false, string_pars, mi, declaringType: null, parameters: parameters);
			print ("[Preserve (Conditional=true)]");
			print_generated_code ();
			print ("unsafe {0} Invoke ({1})", FormatType (null, mi.ReturnType), string_pars.ToString ());
			print ("{"); indent++;
			string cast_a = "", cast_b = "";
			bool use_temp_return;

			GenerateArgumentChecks (mi, true);

			StringBuilder args, convs, disposes, by_ref_processing, by_ref_init;
			GenerateTypeLowering (mi,
					      null_allowed_override: true,
					      enum_mode: EnumMode.NativeBits,
					      args: out args,
					      convs: out convs,
					      disposes: out disposes,
					      by_ref_processing: out by_ref_processing,
					      by_ref_init: out by_ref_init);

			if (by_ref_init.Length > 0)
				print (by_ref_init.ToString ());

			use_temp_return = mi.ReturnType != TypeManager.System_Void;
			if (use_temp_return)
				GetReturnsWrappers (mi, null, null, out cast_a, out cast_b);

			if (convs.Length > 0)
				print (convs.ToString ());
			print ("{0}{1}invoker (blockPtr{2}){3};",
			       use_temp_return ? "var ret = " : "",
			       cast_a,
			       args.ToString (),
			       cast_b);
			if (disposes.Length > 0)
				print (disposes.ToString ());
			if (by_ref_processing.Length > 0)
				print (sw, by_ref_processing.ToString ());
			if (use_temp_return)
				print ("return ret;");
			indent--; print ("}");
			indent--;
			print ("}} /* class {0} */", ti.NativeInvokerName);
		}
	}

	// We need to check against the user using just UIKit (and friends) in the FieldAttribute
	// so we need to reflect the libraries contained in our Constants class and do the mapping
	// we will return the system library path if found
	bool IsNotSystemLibrary (string library_name)
	{
		string library_path = null;
		return TryGetLibraryPath (library_name, ref library_path);
	}

	bool TryGetLibraryPath (string library_name, ref string library_path)
	{
		var libSuffixedName = $"{library_name}Library";
		var constType = TypeManager.Constants;
		var field = constType.GetFields (BindingFlags.Public | BindingFlags.Static).FirstOrDefault (f => f.Name == libSuffixedName);
		library_path = (string) (field?.GetRawConstantValue ());
		return library_path == null;
	}

	void GenerateLibraryHandles ()
	{
		sw = GetOutputStream ("ObjCRuntime", "Libraries");
		
		Header (sw);
		print ("namespace {0} {{", ns.CoreObjCRuntime); indent++;
		print_generated_code ();
		print ("static partial class Libraries {"); indent++;
		foreach (var library_info in libraries.OrderBy (v => v.Key, StringComparer.Ordinal)) {
			var library_name = library_info.Key;
			var library_path = library_info.Value;
			print ("static public class {0} {{", library_name.Replace (".", string.Empty)); indent++;
			if (BindThirdPartyLibrary && library_name == "__Internal") {
				print ("static public readonly IntPtr Handle = Dlfcn.dlopen (null, 0);");
			} else if (BindThirdPartyLibrary && library_path != null && IsNotSystemLibrary (library_name)) {
				print ($"static public readonly IntPtr Handle = Dlfcn.dlopen (\"{library_path}\", 0);");
			} else {
				print ("static public readonly IntPtr Handle = Dlfcn.dlopen (Constants.{0}Library, 0);", library_name);
			}
			indent--; print ("}");
		}
		indent--; print ("}");
		indent--; print ("}");
		sw.Close ();
	}

	//
	// Processes the various StrongDictionaryAttribute interfaces
	//
	void GenerateStrongDictionaryTypes ()
	{
		foreach (var dictType in strong_dictionaries){
			if (dictType.IsUnavailable ())
				continue;
			var sa = AttributeManager.GetCustomAttribute<StrongDictionaryAttribute> (dictType);
			var keyContainerType = sa.TypeWithKeys;
			string suffix = sa.Suffix;

			using (var sw = GetOutputStreamForType (dictType)) {
				string typeName = dictType.Name;
				this.sw = sw;
				Header (sw);

				print ("namespace {0} {{", dictType.Namespace);
				indent++;
				PrintPlatformAttributes (dictType);
				print ("public partial class {0} : DictionaryContainer {{", typeName);
				indent++;
				sw.WriteLine ("#if !COREBUILD");
				print ("[Preserve (Conditional = true)]");
				print ("public {0} () : base (new NSMutableDictionary ()) {{}}\n", typeName);
				print ("[Preserve (Conditional = true)]");
				print ("public {0} (NSDictionary dictionary) : base (dictionary) {{}}\n", typeName);

				foreach (var pi in dictType.GatherProperties ()){
					if (pi.IsUnavailable ())
						continue;
					
					string keyname;
					var attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (pi);
					if (attrs.Length == 0)
						keyname = keyContainerType + "." + pi.Name + suffix;
					else {
						keyname = attrs [0].Selector;
						if (keyname.IndexOf ('.') == -1)
							keyname = keyContainerType + "." + keyname;
					}

					PrintPlatformAttributes (pi);
					string modifier = pi.IsInternal () ? "internal" : "public";
					
					print (modifier + " {0}{1} {2} {{",
					       FormatType (dictType, pi.PropertyType),
					       pi.PropertyType.IsValueType ? "?" : "", // Add the nullable
					       pi.Name);

					string getter = null, setter = null;
					Type fetchType = pi.PropertyType;
					string castToUnderlying = "";
					string castToEnum = "";

					if (pi.PropertyType.IsEnum){
						fetchType = TypeManager.GetUnderlyingEnumType (pi.PropertyType);
						castToUnderlying = "(" + fetchType + "?)";
						castToEnum = "(" + FormatType (dictType, pi.PropertyType) + "?)";
					}
					if (pi.PropertyType.IsValueType){
						if (pi.PropertyType == TypeManager.System_Boolean){
							getter = "{1} GetBoolValue ({0})";
							setter = "SetBooleanValue ({0}, {1}value)";
						} else if (fetchType == TypeManager.System_Int32){
							getter = "{1} GetInt32Value ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeManager.System_nint){
							getter = "{1} GetNIntValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType ==  TypeManager.System_Int64){
							getter = "{1} GetLongValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (pi.PropertyType == TypeManager.System_Float){
							getter = "{1} GetFloatValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (pi.PropertyType == TypeManager.System_Double){
							getter = "{1} GetDoubleValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeManager.System_UInt32){
							getter = "{1} GetUInt32Value ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeManager.System_nuint){
							getter = "{1} GetNUIntValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (UnifiedAPI && fetchType == TypeManager.CoreGraphics_CGRect){
							getter = "{1} GetCGRectValue ({0})";
							setter = "SetCGRectValue ({0}, {1}value)";
						} else if (UnifiedAPI && fetchType == TypeManager.CoreGraphics_CGSize){
							getter = "{1} GetCGSizeValue ({0})";
							setter = "SetCGSizeValue ({0}, {1}value)";
						} else if (UnifiedAPI && fetchType == TypeManager.CoreGraphics_CGPoint){
							getter = "{1} GetCGPointValue ({0})";
							setter = "SetCGPointValue ({0}, {1}value)";
						} else if (Frameworks.HaveCoreMedia && fetchType == TypeManager.CMTime){
							getter = "{1} GetCMTimeValue ({0})";
							setter = "SetCMTimeValue ({0}, {1}value)";
						} else {
							throw new BindingException (1031, true,
										    "Limitation: can not automatically create strongly typed dictionary for " +
										    "({0}) the value type of the {1}.{2} property", pi.PropertyType, dictType, pi.Name);
						}
					} else {
						if (pi.PropertyType.IsArray){
							var elementType = pi.PropertyType.GetElementType ();
							if (IsWrappedType (elementType)){
								getter = "GetArray<" + FormatType (dictType, elementType) + "> ({0})";
								setter = "SetArrayValue ({0}, value)";
							} else if (elementType.IsEnum){
								// Handle arrays of enums as arrays of NSNumbers, casted to the enum
								var enumTypeStr = FormatType (null, elementType);
								getter = "GetArray<" + enumTypeStr +
									"> ({0}, (ptr)=> {{\n\tusing (var num = Runtime.GetNSObject<NSNumber> (ptr)){{\n\t\treturn (" +
									enumTypeStr + ") num.Int32Value;\n\t}}\n}})";
								setter = "SetArrayValue<" + enumTypeStr + "> ({0}, value)";
							} else if (elementType == TypeManager.System_String){
								getter = "GetArray<string> ({0}, (ptr)=>NSString.FromHandle (ptr))";
								setter = "SetArrayValue ({0}, value)";
							} else {
								throw new BindingException (1033, true,
											    "Limitation: can not automatically create strongly typed dictionary for arrays of " +
											    "({0}) the type of the {1}.{2} property", pi.PropertyType, dictType, pi.Name);
							}
						} else if (pi.PropertyType ==  TypeManager.NSString){
							getter = "GetNSStringValue ({0})";
							setter = "SetStringValue ({0}, value)";
						} else if (pi.PropertyType == TypeManager.System_String){
							getter = "GetStringValue ({0})";
							setter = "SetStringValue ({0}, value)";
						} else if (pi.PropertyType.Name.StartsWith ("NSDictionary", StringComparison.Ordinal)){
							if (pi.PropertyType.IsGenericType) {
								var genericParameters = pi.PropertyType.GetGenericArguments ();
								// we want to keep {0} for later yet add the params for the template.
								getter = $"GetNSDictionary <{FormatType (dictType, genericParameters [0])}, {FormatType (dictType, genericParameters [1])}> " + "({0})";  
							} else {
								getter = "GetNSDictionary ({0})";
							}
							setter = "SetNativeValue ({0}, value)";
						} else if (IsDictionaryContainerType (pi.PropertyType) || AttributeManager.HasAttribute<StrongDictionaryAttribute> (pi)) {
							var strType = pi.PropertyType.Name;
							getter = "GetStrongDictionary<" + strType + ">({0})";
							setter = "SetNativeValue ({0}, value.Dictionary)";
						} else if (IsWrappedType (pi.PropertyType)){
							getter = "Dictionary [{0}] as " + pi.PropertyType;
							setter = "SetNativeValue ({0}, value)";
						} else {
							throw new BindingException (1031, true,
										    "Limitation: can not automatically create strongly typed dictionary for " +
										    "({0}) the type of the {1}.{2} property", pi.PropertyType, dictType, pi.Name);
						}
					}

					var og = getter;
					try {
					getter = String.Format (getter, keyname, castToEnum);
					} catch {
						Console.WriteLine ("OOPS: g={0} k={1} c={2}", og, keyname, castToEnum);
						throw;
					}
					setter = String.Format (setter, keyname, castToUnderlying);
					if (pi.CanRead){
						indent++;
						print ("get {"); indent++;
						print ("return {0};", getter);
						indent--; print ("}");
						indent--;
					}
					if (pi.CanWrite){
						if (setter == null)
							throw new BindingException (1032, true, "No support for setters in StrongDictionary classes for type {0} in {1}.{2}", pi.PropertyType, dictType, pi.Name);
						indent++;
						print ("set {"); indent++;
						print ("{0};", setter);
						indent--; print ("}");
						indent--;
					}
					print ("}\n");
						
				}
				sw.WriteLine ("#endif");
				indent--;
				print ("}");
				indent--;
				print ("}");
			}
		}
	}
	
	void GenerateEventArgsFile ()
	{
		sw = GetOutputStream ("ObjCRuntime", "EventArgs");

		Header (sw);
		foreach (Type eventType in notification_event_arg_types.Keys){
			// Do not generate events for stuff with no arguments
			if (eventType == null)
				continue;
		
			if (eventType.Namespace != null) {
				print ("namespace {0} {{", eventType.Namespace);
				indent++;
			}

			print ("public partial class {0} : NSNotificationEventArgs {{", eventType.Name); indent++;
			print ("public {0} (NSNotification notification) : base (notification) \n{{\n}}\n", eventType.Name);
			int i = 0;
			foreach (var prop in eventType.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)){
				if (prop.IsUnavailable ())
					continue;
				var attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (prop);
				if (attrs.Length == 0)
					throw new BindingException (1010, true, "No Export attribute on {0}.{1} property", eventType, prop.Name);

				var is_internal = prop.IsInternal ();
				var export = attrs [0];
				var use_export_as_string_constant = export.ArgumentSemantic != ArgumentSemantic.None;
				var null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (prop);
				var nullable_type = prop.PropertyType.IsValueType && null_allowed;
				var propertyType = prop.PropertyType;
				var propNamespace = prop.DeclaringType.Namespace;
				var probe_presence = AttributeManager.HasAttribute<ProbePresenceAttribute> (prop);

				string kn = "k" + (i++);
				if (use_export_as_string_constant){
					print ("{0} {1}{2} {3} {{\n\tget {{\n",
					       is_internal ? "internal" : "public",
					       propertyType,
					       nullable_type ? "?" : "",
					       prop.Name);
					indent += 2;
					print ("IntPtr value;");
					print ("using (var str = new NSString (\"{0}\")){{", export.Selector);
					kn = "str.Handle";
					indent++;
				} else {
					var lib = propNamespace.Substring (propNamespace.IndexOf ('.') + 1);
					print ("[Field (\"{0}\", \"{1}\")]", export.Selector, lib);
					print ("static IntPtr {0};", kn);
					print ("");
					// linker will remove the attributes (but it's useful for testing)
					print_generated_code ();
					print ("{0} {1}{2} {3} {{",
					       is_internal ? "internal" : "public",
					       propertyType,
					       nullable_type ? "?" : "",
					       prop.Name);
					indent++;
					print ("get {");
					indent++;
					print ("IntPtr value;");
					print ("if ({0} == IntPtr.Zero)", kn);
					indent++;
					var libname = BindThirdPartyLibrary ? "__Internal" : lib;
					print ("{0} = {1}.Dlfcn.GetIntPtr (Libraries.{2}.Handle, \"{3}\");", kn, ns.CoreObjCRuntime, libname, export.Selector);
					indent--;
				}
				if (null_allowed || probe_presence){
					if (probe_presence)
						print ("if (Notification.UserInfo == null)\n\treturn false;");
					else
						print ("if (Notification.UserInfo == null)\n\treturn null;");
				}
				print ("value = Notification.UserInfo.LowlevelObjectForKey ({0});", kn);
				if (use_export_as_string_constant){
					indent--;
					print ("}");
				} else
					print ("");
						
				if (probe_presence)
					print ("return value != IntPtr.Zero;");
				else {
					if (null_allowed)
						print ("if (value == IntPtr.Zero)\n\treturn null;");
					else if (propertyType.IsArray)
						print ("if (value == IntPtr.Zero)\n\treturn new {0} [0];", RenderType (propertyType.GetElementType ()));
					else
						print ("if (value == IntPtr.Zero)\n\treturn default({0});", RenderType (propertyType));

					var fullname = propertyType.FullName;

					if (propertyType.IsArray && IsWrappedType (propertyType.GetElementType ())) {
						print ("return NSArray.ArrayFromHandle<{0}> (value);", RenderType (propertyType.GetElementType ()));
					} else if (IsWrappedType (propertyType)){
						print ("return Runtime.GetNSObject<{0}> (value);", RenderType (propertyType));
					} else if (propertyType == TypeManager.System_Double)
						print (GenerateNSNumber ("", "DoubleValue"));
					else if (propertyType == TypeManager.System_Float)
						print (GenerateNSNumber ("", "FloatValue"));
					else if (fullname == "System.Drawing.PointF")
						print (GenerateNSValue ("PointFValue"));
					else if (fullname == "System.Drawing.SizeF")
						print (GenerateNSValue ("SizeFValue"));
					else if (fullname == "System.Drawing.RectangleF")
						print (GenerateNSValue ("RectangleFValue"));
					else if (fullname == ns.Get ("CoreGraphics.CGPoint"))
						print (GenerateNSValue ("CGPointValue"));
					else if (fullname == ns.Get ("CoreGraphics.CGSize"))
						print (GenerateNSValue ("CGSizeValue"));
					else if (fullname == ns.Get ("CoreGraphics.CGRect"))
						print (GenerateNSValue ("CGRectValue"));
					else if (propertyType == TypeManager.System_String)
						print ("return NSString.FromHandle (value);");
					else if (propertyType == TypeManager.NSString)
						print ("return new NSString (value);");
					else if (propertyType == TypeManager.System_String_Array){
						print ("return NSArray.StringArrayFromHandle (value);");
					} else {
						Type underlying = propertyType.IsEnum ? TypeManager.GetUnderlyingEnumType (propertyType) : propertyType;
						string cast = propertyType.IsEnum ? "(" + propertyType.FullName + ") " : "";
					
						if (underlying == TypeManager.System_Int32)
							print (GenerateNSNumber (cast, "Int32Value"));
						else if (underlying == TypeManager.System_UInt32)
							print (GenerateNSNumber (cast, "UInt32Value"));
						else if (underlying == TypeManager.System_Int64)
							print (GenerateNSNumber (cast, "Int64Value"));
						else if (underlying == TypeManager.System_UInt64)
							print (GenerateNSNumber (cast, "UInt64Value"));
						else if (underlying == TypeManager.System_Int16)
							print (GenerateNSNumber (cast, "Int16Value"));
						else if (underlying == TypeManager.System_UInt16)
							print (GenerateNSNumber (cast, "UInt16Value"));
						else if (underlying == TypeManager.System_SByte)
							print (GenerateNSNumber (cast, "SByteValue"));
						else if (underlying == TypeManager.System_Byte)
							print (GenerateNSNumber (cast, "ByteValue"));
						else if (underlying == TypeManager.System_Boolean)
							print (GenerateNSNumber (cast, "BoolValue"));
						else if (underlying == TypeManager.System_nint)
							print (GenerateNSNumber (cast, "NIntValue"));
						else if (underlying == TypeManager.System_nuint)
							print (GenerateNSNumber (cast, "NUIntValue"));
						else
							throw new BindingException (1011, true, "Do not know how to extract type {0}/{1} from an NSDictionary", propertyType, underlying);
					}
				}
				indent -= 2;
				print ("\t}\n}\n");
			}

			indent--; print ("}");

			if (eventType.Namespace != null) {
				indent--;
				print ("}");
			}
		}
		sw.Close ();
	}
	
		
	public void DumpChildren (int level, GeneratedType gt)
	{
		string prefix = new string ('\t', level);
		Console.WriteLine ("{2} {0} - {1}", gt.Type.Name, gt.ImplementsAppearance ? "APPEARANCE" : "", prefix);
		foreach (var c in (from s in gt.Children .OrderBy ( s => s.Type.FullName, StringComparer.Ordinal) select s))
			DumpChildren (level+1, c);
	}
	
	// this attribute allows the linker to be more clever in removing unused code in bindings - without risking breaking user code
	// only generate those for monotouch now since we can ensure they will be linked away before reaching the devices
	public void GeneratedCode (StreamWriter sw, int tabs)
	{
		for (int i=0; i < tabs; i++)
			sw.Write ('\t');
		sw.WriteLine ("[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
	}

	static void WriteIsDirectBindingCondition (StreamWriter sw, ref int tabs, bool? is_direct_binding, string is_direct_binding_value, Func<string> trueCode, Func<string> falseCode)
	{
		if (is_direct_binding_value != null)
			sw.Write ('\t', tabs).WriteLine ("IsDirectBinding = {0};", is_direct_binding_value);
		
		// If we don't know the IsDirectBinding value, we need the condition
		if (!is_direct_binding.HasValue) {
			sw.Write ('\t', tabs).WriteLine ("if (IsDirectBinding) {");
			tabs++;
		}

		// We need the true part if we don't know, or if we know it's true
		if (is_direct_binding != false) {
			var code = trueCode ();
			if (!string.IsNullOrEmpty (code))
				sw.Write ('\t', tabs).WriteLine (code);
		}
		
		if (!is_direct_binding.HasValue)
			sw.Write ('\t', tabs - 1).WriteLine ("} else {");

		// We need the false part if we don't know, or if we know it's false
		if (is_direct_binding != true) {
			var code = falseCode ();
			if (!string.IsNullOrEmpty (code))
				sw.Write ('\t', tabs).WriteLine (code);
		}
		
		if (!is_direct_binding.HasValue) {
			tabs--;
			sw.Write ('\t', tabs).WriteLine ("}");
		}
	}
	
	public void print_generated_code ()
	{
		GeneratedCode (sw, indent);
	}

	public void print (string format)
	{
		print (sw, format);
	}

	public void print (string format, params object [] args)
	{
		print (sw, format, args);
	}

	public void print (StreamWriter w, string format)
	{
		if (indent < 0)
			throw new InvalidOperationException ("Indent is a negative value.");

		string[] lines = format.Split (new char [] { '\n' });
		string lwsp = new string ('\t', indent);
		
		for (int i = 0; i < lines.Length; i++)
			w.WriteLine (lwsp + lines[i]);
	}

	public void print (StreamWriter w, string format, params object [] args)
	{
		string[] lines = String.Format (format, args).Split (new char [] { '\n' });
		string lwsp = new string ('\t', indent);
		
		for (int i = 0; i < lines.Length; i++)
			w.WriteLine (lwsp + lines[i]);
	}

	public void print (StreamWriter w, IEnumerable e)
	{
		foreach (var a in e)
			w.WriteLine (a);
	}

	public void PrintPlatformAttributes (MemberInfo mi)
	{
		if (mi == null)
			return;

		foreach (var availability in AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (mi))
			print (availability.ToString ());
	}

	public void PrintPlatformAttributesIfInlined (MemberInformation minfo)
	{
		if (minfo == null)
			return;

		// check if it is an inlined property (e.g. from a protocol)
		bool isInlined = minfo.type != minfo.property.DeclaringType;

		// we must avoid duplication of availability so we will only print
		// if the property has no Availability
		bool propHasNoInfo = !AttributeManager.HasAttribute<AvailabilityBaseAttribute> (minfo.property)
						                                     && (minfo.property.GetGetMethod () == null || !AttributeManager.HasAttribute<AvailabilityBaseAttribute> (minfo.property.GetGetMethod ()));

		if (isInlined && propHasNoInfo)
			PrintPlatformAttributes (minfo.property.DeclaringType);
	}

	public string SelectorField (string s, bool ignore_inline_directive = false)
	{
		string name;
		
		if (InlineSelectors && !ignore_inline_directive)
			return "Selector.GetHandle (\"" + s + "\")";

		if (selector_names.TryGetValue (s, out name))
			return name;
		
		StringBuilder sb = new StringBuilder ();
		bool up = true;
		sb.Append ("sel");
		
		foreach (char c in s){
			if (up && c != ':'){
				sb.Append (Char.ToUpper (c));
				up = false;
			} else if (c == ':') {
				// Selectors can differ only by colons.
				// Example 'mountDeveloperDiskImageWithError:' and 'mountDeveloperDiskImage:WithError:' (from Xamarin.Hosting)
				// This also showed up in a bug report: http://bugzilla.xamarin.com/show_bug.cgi?id=2626
				// So make sure we create a different name for those in C# as well, otherwise the generated code won't build.
				up = true;
				sb.Append ('_');
			} else
				sb.Append (c);
		}
		if (!InlineSelectors)
			sb.Append ("Handle");
		name = sb.ToString ();
		selector_names [s] = name;
		return name;
	}

	public string FormatType (Type usedIn, string @namespace, string name)
	{
		string tname;
		if ((usedIn != null && @namespace == usedIn.Namespace) || ns.StandardNamespaces.Contains (@namespace))
			tname = name;
		else
			tname = "global::" + @namespace + "." + name;

		return tname;
	}

	// This will return the needed ObjC name for class_ptr lookup
	public string FormatCategoryClassName (BaseTypeAttribute bta)
	{
		object[] attribs;

		// If we are binding a third party library we need to check for the RegisterAttribute
		// its Name property will contain the propper name we are looking for
		if (BindThirdPartyLibrary) {
			attribs = AttributeManager.GetCustomAttributes<RegisterAttribute> (bta.BaseType);
			if (attribs.Length > 0) {
				var register = (RegisterAttribute)attribs [0];
				return register.Name;
			} else {
				// this will do for categories of third party classes defined in ApiDefinition.cs
				attribs = AttributeManager.GetCustomAttributes<BaseTypeAttribute> (bta.BaseType);
				if (attribs.Length > 0) {
					var baseT = (BaseTypeAttribute)attribs [0];
					if (baseT.Name != null)
						return baseT.Name;
				}
			}
		} else {
			// If we are binding a category inside Xamarin.iOS the selector name will come in
			// the Name property of the BaseTypeAttribute of the base type if changed from the ObjC name
			attribs = AttributeManager.GetCustomAttributes<BaseTypeAttribute> (bta.BaseType);
			if (attribs.Length > 0) {
				var baseT = (BaseTypeAttribute) attribs [0];
				if (baseT.Name != null)
					return baseT.Name;
			}
		}

		var objcClassName = FormatType (null, bta.BaseType);

		if (objcClassName.Contains ("global::"))
			objcClassName = objcClassName.Substring (objcClassName.LastIndexOf ('.') + 1);

		return objcClassName;
	}

	public string FormatType (Type usedIn, Type type)
	{
		return FormatTypeUsedIn (usedIn == null ? null : usedIn.Namespace, type);
	}

	public string FormatType (Type usedIn, Type type, bool protocolized)
	{
		return FormatTypeUsedIn (usedIn?.Namespace, type, protocolized);
	}

	public string FormatTypeUsedIn (string usedInNamespace, Type type, bool protocolized = false)
	{
		type = GetCorrectGenericType (type);

		if (type == TypeManager.System_Void)
			return "void";
		if (type == TypeManager.System_SByte)
			return "sbyte";
		if (type == TypeManager.System_Int32)
			return "int";
		if (type == TypeManager.System_Int16)
			return "short";
		if (type == TypeManager.System_Int64)
			return "long";
		if (type == TypeManager.System_Byte)
			return "byte";
		if (type == TypeManager.System_UInt16)
			return "ushort";
		if (type == TypeManager.System_UInt32)
			return "uint";
		if (type == TypeManager.System_UInt64)
			return "ulong";
		if (type == TypeManager.System_Byte)
			return "byte";
		if (type == TypeManager.System_Float)
			return "float";
		if (type == TypeManager.System_Double)
			return "double";
		if (type == TypeManager.System_Boolean)
			return "bool";
		if (type == TypeManager.System_String)
			return "string";
		if (type == TypeManager.System_nfloat)
			return "nfloat";
		if (type == TypeManager.System_nint)
			return "nint";
		if (type == TypeManager.System_nuint)
			return "nuint";
		if (type == TypeManager.System_Boolean)
			return "bool";

		if (type.IsArray)
			return FormatTypeUsedIn (usedInNamespace, type.GetElementType ()) + "[" + new string (',', type.GetArrayRank () - 1) + "]";

		var interfaceTag = protocolized == true ? "I" : "";
		string tname;
		if ((usedInNamespace != null && type.Namespace == usedInNamespace) || ns.StandardNamespaces.Contains (type.Namespace) || string.IsNullOrEmpty (type.FullName))
			tname = interfaceTag + type.Name;
		else
			tname = $"global::{type.Namespace}.{interfaceTag}{type.Name}";

		var targs = type.GetGenericArguments ();
		if (targs.Length > 0) {
			var isNullable = TypeManager.GetUnderlyingNullableType (type) != null;
			if (isNullable)
				return FormatTypeUsedIn (usedInNamespace, targs [0]) + "?";
			return RemoveArity (tname) + "<" + string.Join (", ", targs.Select (l => FormatTypeUsedIn (usedInNamespace, l)).ToArray ()) + ">";
		}

		return tname;
	}

	static string RemoveArity (string typeName)
	{
		var arity = typeName.IndexOf ('`');
		return arity > 0 ? typeName.Substring (0, arity) : typeName;
	}

	//
	// Makes the public signature for an exposed method
	//
	public string MakeSignature (MemberInformation minfo)
	{
		return MakeSignature (minfo, false, minfo.method.GetParameters ());
	}

	//
	// Makes the public signature for an exposed method, taking into account if PreserveAttribute is needed
	//
	public string MakeSignature (MemberInformation minfo, bool alreadyPreserved)
	{
		return MakeSignature (minfo, false, minfo.method.GetParameters (), "", alreadyPreserved);
	}

	public string GetAsyncName (MethodInfo mi)
	{
		var attr = AttributeManager.GetCustomAttribute<AsyncAttribute> (mi);
		if (attr.MethodName != null)
			return attr.MethodName;
		return mi.Name + "Async";
	}

	public bool IsModel (Type type)
	{
		return AttributeManager.HasAttribute<ModelAttribute> (type) && !AttributeManager.HasAttribute<SyntheticAttribute> (type);
	}

	public bool IsProtocol (Type type)
	{
		return AttributeManager.HasAttribute<ProtocolAttribute> (type);
	}

	public static bool Protocolize (ICustomAttributeProvider provider)
	{
		if (!UnifiedAPI)
			return false;

		var attribs = AttributeManager.GetCustomAttributes<ProtocolizeAttribute> (provider);
		if (attribs == null || attribs.Length == 0)
			return false;

		var attrib = attribs [0];
		return Generator.XamcoreVersion >= attrib.Version;
	}

	public string MakeSignature (MemberInformation minfo, bool is_async, ParameterInfo[] parameters, string extra = "", bool alreadyPreserved = false)
	{
		var mi = minfo.method;
		var category_class = minfo.category_extension_type;
		StringBuilder sb = new StringBuilder ();
		string name = minfo.is_ctor ? GetGeneratedTypeName (mi.DeclaringType) : is_async ? GetAsyncName (mi) : mi.Name;

		// Some codepaths already write preservation info
		PrintAttributes (minfo.mi, preserve:!alreadyPreserved, advice:true, bindAs:true, requiresSuper: true);

		if (!minfo.is_ctor && !is_async){
			var prefix = "";
			if (UnifiedAPI && !BindThirdPartyLibrary){
				var hasReturnTypeProtocolize = Protocolize (AttributeManager.GetReturnTypeCustomAttributes (minfo.method));
				if (hasReturnTypeProtocolize) {
					if (!IsProtocol (minfo.method.ReturnType)) {
						ErrorHelper.Show (new BindingException (1108, false, "The [Protocolize] attribute is applied to the return type of the method {0}.{1}, but the return type ({2}) isn't a model and can thus not be protocolized. Please remove the [Protocolize] attribute.", minfo.method.DeclaringType, minfo.method, minfo.method.ReturnType.FullName));
					} else {
						prefix = "I";
					}
				}
				if (minfo.method.ReturnType.IsArray) {
					var et = minfo.method.ReturnType.GetElementType ();
					if (IsModel (et))
						ErrorHelper.Show (new BindingException (1109, false, "The return type of the method {0}.{1} exposes a model ({2}). Please expose the corresponding protocol type instead ({3}.I{4}).", minfo.method.DeclaringType, minfo.method.Name, et, et.Namespace, et.Name));
				}
				if (IsModel (minfo.method.ReturnType) && !hasReturnTypeProtocolize)
					ErrorHelper.Show (new BindingException (1107, false, "The return type of the method {0}.{1} exposes a model ({2}). Please expose the corresponding protocol type instead ({3}.I{4}).", minfo.method.DeclaringType, minfo.method.Name, minfo.method.ReturnType, minfo.method.ReturnType.Namespace, minfo.method.ReturnType.Name));
			}

			if (minfo.is_bindAs) {
				if (IsMemberInsideProtocol (minfo.mi.DeclaringType))
					throw new BindingException (1050, true, "[BindAs] cannot be used inside Protocol or Model types. Type: {0}", minfo.mi.DeclaringType.Name);

				var bindAsAttrib = GetBindAsAttribute (minfo.mi);
				sb.Append (prefix + FormatType (bindAsAttrib.Type.DeclaringType, GetCorrectGenericType (bindAsAttrib.Type)));
			} else
				sb.Append (prefix + FormatType (mi.DeclaringType, GetCorrectGenericType (mi.ReturnType)));

			sb.Append (" ");
		}
		// Unified internal methods automatically get a _ appended
		if (minfo.is_extension_method && minfo.method.IsSpecialName) {
			if (name.StartsWith ("get_", StringComparison.Ordinal))
				name = "Get" + name.Substring (4);
			else if (name.StartsWith ("set_", StringComparison.Ordinal))
				name = "Set" + name.Substring (4);
		}
		sb.Append (name);
		if (minfo.is_unified_internal)
			sb.Append ("_");
		sb.Append (" (");

		bool comma = false;
		if (minfo.is_extension_method) {
			sb.Append ("this ");
			sb.Append ("I" + mi.DeclaringType.Name);
			sb.Append (" This");
			comma = true;
		} else if (category_class != null){
			sb.Append ("this ");
//			Console.WriteLine ("Gto {0} and {1}", mi.DeclaringType, category_class);
			sb.Append (FormatType (mi.DeclaringType, category_class));
			sb.Append (" This");
			comma = true;
		}
		MakeSignatureFromParameterInfo (comma, sb, mi, minfo.type, parameters);
		sb.Append (extra);
		sb.Append (")");
		return sb.ToString ();
	}

	//
	// Renders the parameters in @parameters in a format suitable for a method declaration.
	// The result is place into the provided string builder 
	// 
	public void MakeSignatureFromParameterInfo (bool comma, StringBuilder sb, MemberInfo mi, Type declaringType, ParameterInfo [] parameters)
	{
		int parCount = parameters.Length;
		for (int pari = 0; pari < parCount; pari++){
			var pi = parameters [pari];

			if (comma)
				sb.Append (", ");
			comma = true;

			// Format nicely the type, as succinctly as possible
			Type parType = GetCorrectGenericType (pi.ParameterType);
			if (parType.IsSubclassOf (TypeManager.System_Delegate)){
				var ti = MakeTrampoline (parType);
				sb.AppendFormat ("[BlockProxy (typeof ({0}.Trampolines.{1}))]", ns.CoreObjCRuntime, ti.NativeInvokerName);
			}

			if (AttributeManager.HasAttribute<TransientAttribute> (pi))
				sb.Append ("[Transient] ");
			
			if (parType.IsByRef){
				string reftype = TypeManager.IsOutParameter (pi) ? "out " : "ref ";
				sb.Append (reftype);
				parType = parType.GetElementType ();
			}
			// note: check for .NET `params` on the bindings, which generates a `ParamArrayAttribute` or the old (not really required) custom `ParamsAttribute`
			if (pari == parCount - 1 && parType.IsArray && (AttributeManager.HasAttribute<ParamsAttribute> (pi) || AttributeManager.HasAttribute<ParamArrayAttribute> (pi))) {
				sb.Append ("params ");
			}
			var protocolized = false;
			if (!BindThirdPartyLibrary && Protocolize (pi)) {
				if (!AttributeManager.HasAttribute<ProtocolAttribute> (parType)) {
					Console.WriteLine ("Protocolized attribute for type that does not have a [Protocol] for {0}'s parameter {1}", mi, pi);
				}
				protocolized = true;
			}

			var bindAsAtt = GetBindAsAttribute (pi);
			if (bindAsAtt != null) {
				PrintBindAsAttribute (pi, sb);
				sb.Append (FormatType (bindAsAtt.Type.DeclaringType, bindAsAtt.Type, protocolized));
			}
			else
				sb.Append (FormatType (declaringType, parType, protocolized));

			sb.Append (" ");
			sb.Append (pi.Name.GetSafeParamName ());
		}
	}

	void Header (StreamWriter w)
	{
		print (w, "//\n// Auto-generated from generator.cs, do not edit\n//");
		print (w, "// We keep references to objects, so warning 414 is expected\n");
		print (w, "#pragma warning disable 414\n");
		print (w, ns.ImplicitNamespaces.OrderByDescending (n => n.StartsWith ("System", StringComparison.Ordinal)).ThenBy (n => n.Length).Select (n => "using " + n + ";"));
		print (w, "");
	}

	//
	// Given a method info that has a return value, produce the text necessary on
	// both sides of a call to wrap the result into user-facing MonoTouch types.
	//
	// @mi: the method info, should not have a returntype of void
	// @cast_a: left side to generate
	// @cast_b: right side to generate
	//
	void GetReturnsWrappers (MethodInfo mi, MemberInformation minfo, Type declaringType, out string cast_a, out string cast_b, EnumMode enum_mode = EnumMode.Compat, StringBuilder postproc = null)
	{
		cast_a = cast_b = "";
		if (mi.ReturnType == TypeManager.System_Void){
			throw new ArgumentException ("the provided Method has a void return type, it should never call this method");
		}
		
		MarshalInfo mai = new MarshalInfo (mi);
		MarshalType mt;

		if (IsNativeEnum (mi.ReturnType) && enum_mode == EnumMode.Bit32) {
			// Check if we got UInt32.MaxValue, which should probably be UInt64.MaxValue (if the enum
			// in question actually has that value at least).
			var type = TypeManager.GetUnderlyingEnumType (mi.ReturnType) == TypeManager.System_UInt64 ? "ulong" : "long";
			var itype = type == "ulong" ? "uint" : "int";
			var value = type == "ulong" ? (object) ulong.MaxValue : (object) long.MaxValue;
			if (mi.ReturnType.IsEnumDefined (value)) {
				postproc.AppendFormat ("if (({0}) ret == ({0}) {2}.MaxValue) ret = ({1}) {0}.MaxValue;", type, FormatType (mi.DeclaringType, mi.ReturnType), itype);
				if (type == "long")
					postproc.AppendFormat ("else if (({0}) ret == ({0}) {2}.MinValue) ret = ({1}) {0}.MinValue;", type, FormatType (mi.DeclaringType, mi.ReturnType), itype);
			} else {
				cast_a = "(" + FormatType (mi.DeclaringType, mi.ReturnType) + ") ";
				cast_b = "";
			}
		} if (mi.ReturnType.IsEnum){
			cast_a = "(" + FormatType (mi.DeclaringType, mi.ReturnType) + ") ";
			cast_b = "";
		} else if (LookupMarshal (mai.Type, out mt)){
			if (mt.HasCustomCreate) {
				cast_a = mt.CreateFromRet;
				cast_b = mt.ClosingCreate;
			} else { // we need to gather the ptr and store it inside IntPtr ret;
				cast_a = string.Empty;
				cast_b = string.Empty;
			}
		} else if (IsWrappedType (mi.ReturnType)){
			// protocol support means we can return interfaces and, as far as .NET knows, they might not be NSObject
			if (IsProtocolInterface (mi.ReturnType)) {
				cast_a = " Runtime.GetINativeObject<" + FormatType (mi.DeclaringType, mi.ReturnType) + "> (";
				cast_b = ", false)";
			} else if (minfo != null && minfo.protocolize) {
				cast_a = " Runtime.GetINativeObject<" + FormatType (mi.DeclaringType, mi.ReturnType.Namespace, FindProtocolInterface (mi.ReturnType, mi)) + "> (";
				cast_b = ", false)";
			} else if (minfo != null && minfo.is_forced) {
				cast_a = " Runtime.GetINativeObject<" + FormatType (declaringType, GetCorrectGenericType (mi.ReturnType)) + "> (";
				cast_b = $", {minfo.is_forced_owns})";
			} else if (minfo != null && minfo.is_bindAs) {
				var bindAs = GetBindAsAttribute (minfo.mi);
				var nullableBindAsType = TypeManager.GetUnderlyingNullableType (bindAs.Type);
				var isNullable = nullableBindAsType != null;
				var bindAsType = TypeManager.GetUnderlyingNullableType (bindAs.Type) ?? bindAs.Type;
				var formattedBindAsType = FormatType (declaringType, GetCorrectGenericType (bindAs.Type));
				string suffix;
				var wrapper = GetFromBindAsWrapper (minfo, out suffix);
				var formattedReturnType = FormatType (declaringType, GetCorrectGenericType (mi.ReturnType));
				if (mi.ReturnType == TypeManager.NSString) {
					if (isNullable) {
						print ("IntPtr retvaltmp;");
						cast_a = "((retvaltmp = ";
						cast_b = $") == IntPtr.Zero ? default ({formattedBindAsType}) : ({wrapper}Runtime.GetNSObject<{formattedReturnType}> (retvaltmp)){suffix})";
					} else {
						cast_a = $"{wrapper}Runtime.GetNSObject<{formattedReturnType}> (";
						cast_b = $"){suffix}";
					}
				} else {
					var enumCast = (bindAsType.IsEnum && !minfo.type.IsArray) ? $"({formattedBindAsType}) " : string.Empty;
					print ("IntPtr retvaltmp;");
					cast_a = "((retvaltmp = ";
					cast_b = $") == IntPtr.Zero ? default ({formattedBindAsType}) : ({enumCast}Runtime.GetNSObject<{formattedReturnType}> (retvaltmp){wrapper})){suffix}";
				}
			} else {
				cast_a = " Runtime.GetNSObject<" + FormatType (declaringType, GetCorrectGenericType (mi.ReturnType)) + "> (";
				cast_b = ")";
			}
		} else if (mi.ReturnType.IsGenericParameter) {
			cast_a = " Runtime.GetINativeObject<" + mi.ReturnType.Name + "> (";
			cast_b = ", false)";
		} else if (mai.Type == TypeManager.System_String && !mai.PlainString){
			cast_a = "NSString.FromHandle (";
			cast_b = ")";
		} else if (mi.ReturnType.IsSubclassOf (TypeManager.System_Delegate)){
			cast_a = "";
			cast_b = "";
		} else if (mai.Type.IsArray){
			Type etype = mai.Type.GetElementType ();
			if (minfo != null && minfo.is_bindAs) {
				var bindAsT = GetBindAsAttribute (minfo.mi).Type.GetElementType ();
				var suffix = string.Empty;
				print ("IntPtr retvalarrtmp;");
				cast_a = "((retvalarrtmp = ";
				cast_b = ") == IntPtr.Zero ? null : (";
				cast_b += $"NSArray.ArrayFromHandleFunc <{FormatType (bindAsT.DeclaringType, bindAsT)}> (retvalarrtmp, {GetFromBindAsWrapper (minfo, out suffix)})" + suffix;
				cast_b += "))";
			} else if (etype == TypeManager.System_String) {
				cast_a = "NSArray.StringArrayFromHandle (";
				cast_b = ")";
			} else if (minfo != null && minfo.protocolize) {
				cast_a = "NSArray.ArrayFromHandle<global::" + etype.Namespace + ".I" + etype.Name + ">(";
				cast_b = ")";
			} else {
				if (NamespaceManager.NamespacesThatConflictWithTypes.Contains (NamespaceManager.Get(etype.Namespace)))
					cast_a = "NSArray.ArrayFromHandle<global::" + etype + ">(";
				else
					cast_a = "NSArray.ArrayFromHandle<" + FormatType (mi.DeclaringType, etype) + ">(";
				cast_b = ")";
			}
		}
	}

	void GenerateInvoke (bool stret, bool supercall, MethodInfo mi, MemberInformation minfo, string selector, string args, bool assign_to_temp, Type category_type, bool aligned, EnumMode enum_mode = EnumMode.Compat)
	{
		string target_name = (category_type == null && !minfo.is_extension_method) ? "this" : "This";
		string handle = supercall ? ".SuperHandle" : ".Handle";
		
		// If we have supercall == false, we can be a Bind methdo that has a [Target]
		if (supercall == false && !minfo.is_static){
			foreach (var pi in mi.GetParameters ()){
				if (IsTarget (pi)){
					if (pi.ParameterType == TypeManager.System_String){
						var mai = new MarshalInfo (mi, pi);
						
						if (mai.PlainString)
							ErrorHelper.Show (new BindingException (1101, false, "Trying to use a string as a [Target]"));

						if (mai.ZeroCopyStringMarshal){
							target_name = "(IntPtr)(&_s" + pi.Name + ")";
							handle = "";
						} else {
							target_name = "ns" + pi.Name;
							handle = "";
						}
					} else
						target_name = pi.Name.GetSafeParamName ();
					break;
				}
			}
		}
		
		string sig = supercall ? MakeSuperSig (mi, stret, aligned, enum_mode) : MakeSig (mi, stret, aligned, enum_mode);

		sig = "global::" + ns.Messaging + "." + sig;

		string selector_field;
		if (minfo.is_interface_impl || minfo.is_extension_method) {
			var tmp = InlineSelectors;
			InlineSelectors = true;
			selector_field = SelectorField (selector);
			InlineSelectors = tmp;
		} else {
			selector_field = SelectorField (selector);
		}


		if (stret){
			string ret_val = aligned ? "aligned_ret" : "out ret";
			if (minfo.is_static)
				print ("{0} ({5}, class_ptr, {3}{4});", sig, "/*unusued*/", "/*unusued*/", selector_field, args, ret_val);
			else
				print ("{0} ({5}, {1}{2}, {3}{4});", sig, target_name, handle, selector_field, args, ret_val);

			if (aligned)
				print ("aligned_assigned = true;");
		} else {
			bool returns = mi.ReturnType != TypeManager.System_Void && mi.Name != "Constructor";
			string cast_a = "", cast_b = "";
			StringBuilder postproc = new StringBuilder ();

			if (returns)
				GetReturnsWrappers (mi, minfo, mi.DeclaringType, out cast_a, out cast_b, enum_mode, postproc);
			else if (mi.Name == "Constructor") {
				cast_a = "InitializeHandle (";
				cast_b = ", \"" + selector + "\")";
			}

			if (minfo.is_static)
				print ("{0}{1}{2} (class_ptr, {5}{6}){7};",
				       returns ? (assign_to_temp ? "ret = " : "return ") : "",
				       cast_a, sig, target_name, 
				       "/*unusued3*/", //supercall ? "Super" : "",
					   selector_field, args, cast_b);
			else
				print ("{0}{1}{2} ({3}{4}, {5}{6}){7};",
				       returns ? (assign_to_temp ? "ret = " : "return ") : "",
				       cast_a, sig, target_name,
				       handle,
					   selector_field, args, cast_b);

			if (postproc.Length > 0)
				print (postproc.ToString ());
		}
	}
	
	void GenerateInvoke (bool supercall, MethodInfo mi, MemberInformation minfo, string selector, string[] args, bool assign_to_temp, Type category_type)
	{
		if (!Compat) {
			GenerateNewStyleInvoke (supercall, mi, minfo, selector, args, assign_to_temp, category_type);
			return;
		}

		var returnType = mi.ReturnType;
		bool arm_stret = Stret.ArmNeedStret (returnType);
		bool x86_stret = Stret.X86NeedStret (returnType);
		bool aligned = AttributeManager.HasAttribute<AlignAttribute> (mi);

		if (CurrentPlatform == PlatformName.MacOSX) {
			GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && x86_stret);
			return;
		}
		
		bool need_two_paths = arm_stret != x86_stret;
		if (need_two_paths){
			print ("if (Runtime.Arch == Arch.DEVICE){");
			indent++;
			GenerateInvoke (arm_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && arm_stret);
			indent--;
			print ("} else {");
			indent++;
			GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && arm_stret);
			indent--;
			print ("}");
		} else {
			GenerateInvoke (arm_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && arm_stret);
		}
	}

	void GenerateNewStyleInvoke (bool supercall, MethodInfo mi, MemberInformation minfo, string selector, string[] args, bool assign_to_temp, Type category_type)
	{
		var returnType = mi.ReturnType;
		bool arm_stret = Stret.ArmNeedStret (returnType);
		bool x86_stret = Stret.X86NeedStret (returnType);
		bool x64_stret = Stret.X86_64NeedStret (returnType);
		bool dual_enum = HasNativeEnumInSignature (mi);
		bool is_stret_multi = arm_stret || x86_stret || x64_stret;
		bool need_multi_path = is_stret_multi || dual_enum;
		bool aligned = AttributeManager.HasAttribute<AlignAttribute> (mi);
		int index64 = dual_enum ? 1 : 0;

		if (CurrentPlatform == PlatformName.MacOSX) {
			if (need_multi_path) {
				print ("if (IntPtr.Size == 8) {");
				indent++;
				GenerateInvoke (x64_stret, supercall, mi, minfo, selector, args[index64], assign_to_temp, category_type, aligned && x64_stret, EnumMode.Bit64);
				indent--;
				print ("} else {");
				indent++;
				GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && x86_stret, EnumMode.Bit32);
				indent--;
				print ("}");
			} else {
				GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && x86_stret);
			}
			return;
		}

		if (need_multi_path) {
			if (is_stret_multi) {
				print ("if (Runtime.Arch == Arch.DEVICE) {");
				indent++;
				print ("if (IntPtr.Size == 8) {");
				indent++;
				GenerateInvoke (false, supercall, mi, minfo, selector, args [index64], assign_to_temp, category_type, false, EnumMode.Bit64);
				indent--;
				print ("} else {");
				indent++;
				GenerateInvoke (arm_stret, supercall, mi, minfo, selector, args [0], assign_to_temp, category_type, aligned && arm_stret, EnumMode.Bit32);
				indent--;
				print ("}");
				indent--;
				print ("} else if (IntPtr.Size == 8) {");
			} else {
				print ("if (IntPtr.Size == 8) {");
			}
			indent++;
			GenerateInvoke (x64_stret, supercall, mi, minfo, selector, args[index64], assign_to_temp, category_type, aligned && x64_stret, EnumMode.Bit64);
			indent--;
			print ("} else {");
			indent++;
			GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, aligned && x86_stret, EnumMode.Bit32);
			indent--;
			print ("}");
		} else {
			GenerateInvoke (false, supercall, mi, minfo, selector, args[0], assign_to_temp, category_type, false);
		}
	}

	static char [] newlineTab = new char [] { '\n', '\t' };
	
	void Inject<T> (MethodInfo method) where T: SnippetAttribute
	{
		var snippets = AttributeManager.GetCustomAttributes<T> (method);
		if (snippets.Length == 0)
			return;
		foreach (SnippetAttribute snippet in snippets)
			Inject (snippet);
	}

	void Inject (SnippetAttribute snippet)
	{
		if (snippet.Code == null)
			return;
		var lines = snippet.Code.Split (newlineTab);
		foreach (var l in lines){
			if (l.Length == 0)
				continue;
			print (l);
		}
	}
	
	[Flags]
	public enum BodyOption {
		None = 0x0,
		NeedsTempReturn = 0x1,
		CondStoreRet    = 0x3,
		MarkRetDirty    = 0x5,
		StoreRet        = 0x7,
	}

	public enum ThreadCheck {
		Default, // depends on the namespace
		Off,
		On,
	}
	
	//
	// generates the code to marshal a string from C# to Objective-C:
	//
	// @probe_null: determines whether null is allowed, and
	// whether we need to generate code to handle this
	//
	// @must_copy: determines whether to create a new NSString, necessary
	// for NSString properties that are flagged with "retain" instead of "copy"
	//
	// @prefix: prefix to prepend on each line
	//
	// @property: the name of the property
	//
	public string GenerateMarshalString (bool probe_null, bool must_copy)
	{
		if (must_copy){
			return "var ns{0} = NSString.CreateNative ({1});\n";
		}
		return
			ns.CoreObjCRuntime + ".NSStringStruct _s{0}; Console.WriteLine (\"" + CurrentMethod + ": Marshalling: {{1}}\", {1}); \n" +
			"_s{0}.ClassPtr = " + ns.CoreObjCRuntime + ".NSStringStruct.ReferencePtr;\n" +
			"_s{0}.Flags = 0x010007d1; // RefCount=1, Unicode, InlineContents = 0, DontFreeContents\n" +
			"_s{0}.UnicodePtr = _p{0};\n" + 
			"_s{0}.Length = " + (probe_null ? "{1} == null ? 0 : {1}.Length;" : "{1}.Length;\n");
	}
	
	public string GenerateDisposeString (bool probe_null, bool must_copy)
	{
		if (must_copy){
			return "NSString.ReleaseNative (ns{0});\n";
		} else 
			return "if (_s{0}.Flags != 0x010007d1) throw new Exception (\"String was retained, not copied\");";
	}

	List<string> CollectFastStringMarshalParameters (MethodInfo mi)
	{
		List<string> stringParameters = null;
		
		foreach (var pi in mi.GetParameters ()){
 			var mai = new MarshalInfo (mi, pi);

 			if (mai.ZeroCopyStringMarshal){
 				if (stringParameters == null)
 					stringParameters = new List<string>();
				stringParameters.Add (pi.Name.GetSafeParamName ());
 			}
 		}
		return stringParameters;
	}

	AvailabilityBaseAttribute GetIntroduced (Type type, string methodName)
	{
		if (type == null)
			return null;

		var prop = type.GetProperties ()
			.Where (pi => pi.Name == methodName)
			.FirstOrDefault ();

		if (prop != null)
			return prop.GetAvailability (AvailabilityKind.Introduced);

		return GetIntroduced (ReflectionExtensions.GetBaseType (type), methodName);
	}

	AvailabilityBaseAttribute GetIntroduced (MethodInfo mi, PropertyInfo pi)
	{
		return mi.GetAvailability (AvailabilityKind.Introduced) ?? pi.GetAvailability (AvailabilityKind.Introduced);
	}

	//
	// Generates the code necessary to lower the MonoTouch-APIs to something suitable
	// to be passed to Objective-C.
	//
	// This turns things like strings into NSStrings, NSObjects into the Handle object,
	// INativeObjects into calling the handle and so on.
	//
	// The result is delivered as a StringBuilder that is used to prepare the marshaling
	// and then one that must be executed after the method has been invoked to cleaup the
	// results
	//
	// @mi: input parameter, contains the method info with the C# signature to generate lowering for
	// @null_allowed_override: this is suitable for applying [NullAllowed] at the property level,
	// and is a convenient override to pass down to this method.   
	//
	// The following are written into, these are expected to be fresh StringBuilders:
	// @args: arguments that should be passed to native
	// @convs: conversions to perform before the invocation
	// @disposes: dispose operations to perform after the invocation
	// @by_ref_processing
	void GenerateTypeLowering (MethodInfo mi, bool null_allowed_override, EnumMode enum_mode, out StringBuilder args, out StringBuilder convs, out StringBuilder disposes, out StringBuilder by_ref_processing, out StringBuilder by_ref_init, PropertyInfo propInfo = null)
	{
		args = new StringBuilder ();
		convs = new StringBuilder ();
		disposes = new StringBuilder ();
		by_ref_processing = new StringBuilder();
		by_ref_init = new StringBuilder ();
		
		foreach (var pi in mi.GetParameters ()){
			MarshalInfo mai = new MarshalInfo (mi, pi);

			if (!IsTarget (pi)){
				// Construct invocation
				args.Append (", ");
				args.Append (MarshalParameter (mi, pi, null_allowed_override, enum_mode, propInfo));

				if (pi.ParameterType.IsByRef) {
					var et = pi.ParameterType.GetElementType ();
					var nullable = TypeManager.GetUnderlyingNullableType (et);
					if (nullable != null) {
						convs.Append ("var converted = IntPtr.Zero;\n");
						convs.Append ($"var v = default ({FormatType (mi.DeclaringType, nullable)});\n");
						convs.Append ("if (value.HasValue) {\n");
						convs.Append ("\tv = value.Value;\n");
						convs.Append ("\tconverted = new IntPtr (&v);\n");
						convs.Append ("}\n");
					}
				}
			}

			// Construct conversions
			if (mai.Type == TypeManager.System_String && !mai.PlainString){
				bool probe_null = null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi);

				convs.AppendFormat (GenerateMarshalString (probe_null, !mai.ZeroCopyStringMarshal), pi.Name, pi.Name.GetSafeParamName ());
				disposes.AppendFormat (GenerateDisposeString (probe_null, !mai.ZeroCopyStringMarshal), pi.Name);
			} else if (mai.Type.IsArray){
				Type etype = mai.Type.GetElementType ();
				if (HasBindAsAttribute (pi)) {
					convs.AppendFormat ("var nsb_{0} = {1}\n", pi.Name, GetToBindAsWrapper (null, pi));
					disposes.AppendFormat ("\nnsb_{0}?.Dispose ();", pi.Name);
				} else if (HasBindAsAttribute (propInfo)) {
					disposes.AppendFormat ("\nnsb_{0}?.Dispose ();", propInfo.Name);
				} else if (etype == TypeManager.System_String) {
					if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi)) {
						convs.AppendFormat ("var nsa_{0} = {1} == null ? null : NSArray.FromStrings ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("if (nsa_{0} != null)\n\tnsa_{0}.Dispose ();\n", pi.Name);
					} else {
						convs.AppendFormat ("var nsa_{0} = NSArray.FromStrings ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("nsa_{0}.Dispose ();\n", pi.Name);
					}
				} else {
					if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi)) {
						convs.AppendFormat ("var nsa_{0} = {1} == null ? null : NSArray.FromNSObjects ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("if (nsa_{0} != null)\n\tnsa_{0}.Dispose ();\n", pi.Name);
					} else {
						convs.AppendFormat ("var nsa_{0} = NSArray.FromNSObjects ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("nsa_{0}.Dispose ();\n", pi.Name);
					}
				}
			} else if (mai.Type.IsSubclassOf (TypeManager.System_Delegate)){
				string trampoline_name = MakeTrampoline (pi.ParameterType).StaticName;
				string extra = "";
				bool null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (pi);
				
				convs.AppendFormat ("BlockLiteral *block_ptr_{0};\n", pi.Name);
				convs.AppendFormat ("BlockLiteral block_{0};\n", pi.Name);
				if (null_allowed){
					convs.AppendFormat ("if ({0} == null){{\n", pi.Name.GetSafeParamName ());
					convs.AppendFormat ("\tblock_ptr_{0} = null;\n", pi.Name);
					convs.AppendFormat ("}} else {{\n");
					extra = "\t";
				}
				convs.AppendFormat (extra + "block_{0} = new BlockLiteral ();\n", pi.Name);
				convs.AppendFormat (extra + "block_ptr_{0} = &block_{0};\n", pi.Name);
				convs.AppendFormat (extra + "block_{0}.SetupBlockUnsafe (Trampolines.{1}.Handler, {2});\n", pi.Name, trampoline_name, pi.Name.GetSafeParamName ());
				if (null_allowed)
					convs.AppendFormat ("}}\n");

				if (null_allowed){
					disposes.AppendFormat ("if (block_ptr_{0} != null)\n", pi.Name);
				}
				disposes.AppendFormat (extra + "block_ptr_{0}->CleanupBlock ();\n", pi.Name);
			} else if (pi.ParameterType.IsGenericParameter) {
//				convs.AppendFormat ("{0}.Handle", pi.Name.GetSafeParamName ());
			} else if (HasBindAsAttribute (pi)) {
				convs.AppendFormat ("var nsb_{0} = {1}\n", pi.Name, GetToBindAsWrapper (null, pi));
			} else {
				if (mai.Type.IsClass && !mai.Type.IsByRef && 
					(mai.Type != TypeManager.Selector && mai.Type != TypeManager.Class && mai.Type != TypeManager.System_String && !TypeManager.INativeObject.IsAssignableFrom (mai.Type)))
					throw new BindingException (1020, true, "Unsupported type {0} used on exported method {1}.{2} -> {3}'", mai.Type, mi.DeclaringType, mi.Name, mai.Type.IsByRef);
			}

			// Handle ByRef
			if (mai.Type.IsByRef && mai.Type.GetElementType ().IsValueType == false){
				string isForcedOwns;
				var isForced = HasForcedAttribute (pi, out isForcedOwns);
				by_ref_init.AppendFormat ("IntPtr {0}Value = IntPtr.Zero;\n", pi.Name.GetSafeParamName ());

				by_ref_processing.AppendLine();
				if (mai.Type.GetElementType () == TypeManager.System_String){
					by_ref_processing.AppendFormat("{0} = {0}Value != IntPtr.Zero ? NSString.FromHandle ({0}Value) : null;", pi.Name.GetSafeParamName ());
				} else if (pi.ParameterType.GetElementType ().IsArray) {
					by_ref_processing.AppendFormat ("{0} = {0}Value != IntPtr.Zero ? NSArray.ArrayFromHandle<{1}> ({0}Value) : null;", pi.Name.GetSafeParamName (), RenderType (mai.Type.GetElementType ().GetElementType ()));
				} else if (isForced) {
					by_ref_processing.AppendFormat("{0} = {0}Value != IntPtr.Zero ? Runtime.GetINativeObject<{1}> ({0}Value, {2}) : null;", pi.Name.GetSafeParamName (), RenderType (mai.Type.GetElementType ()), isForcedOwns);
				} else {
					by_ref_processing.AppendFormat("{0} = {0}Value != IntPtr.Zero ? Runtime.GetNSObject<{1}> ({0}Value) : null;", pi.Name.GetSafeParamName (), RenderType (mai.Type.GetElementType ()));
				}
			}
		}
	}

	void GenerateArgumentChecks (MethodInfo mi, bool null_allowed_override, PropertyInfo propInfo = null)
	{
		if (null_allowed_override)
			return;

		foreach (var pi in mi.GetParameters ()) {
			var needs_null_check = ParameterNeedsNullCheck (pi, mi, propInfo);
			if (!needs_null_check)
				continue;

			if (UnifiedAPI && !BindThirdPartyLibrary) {
				if (!mi.IsSpecialName && IsModel (pi.ParameterType) && !Protocolize (pi))
					ErrorHelper.Show (new BindingException (1106, false, "The parameter {2} in the method {0}.{1} exposes a model ({3}). Please expose the corresponding protocol type instead ({4}.I{5}).",
						mi.DeclaringType, mi.Name, pi.Name, pi.ParameterType, pi.ParameterType.Namespace, pi.ParameterType.Name));
			}
			
			if (Protocolize (pi)) {
				print ("if ({0} != null){{", pi.Name.GetSafeParamName ());
				print ("\tif (!({0} is NSObject))\n", pi.Name.GetSafeParamName ());
				print ("\t\tthrow new ArgumentException (\"The object passed of type \" + {0}.GetType () + \" does not derive from NSObject\");", pi.Name.GetSafeParamName ());
				if (needs_null_check){
					print ("} else {");
					print ("\tthrow new ArgumentNullException (\"{0}\");", pi.Name.GetSafeParamName ());
				}
				print ("}");
			} else {
				print ("if ({0} == null)", pi.Name.GetSafeParamName ());
				print ("\tthrow new ArgumentNullException (\"{0}\");", pi.Name.GetSafeParamName ());
			}
		}
	}
		
	// undecorated code is assumed to be iOS 2.0
	static AvailabilityBaseAttribute iOSIntroducedDefault = new IntroducedAttribute (PlatformName.iOS, 2, 0);

	string CurrentMethod;

	void GenerateThreadCheck ()
	{
		switch (CurrentPlatform) {
			case PlatformName.iOS:
			case PlatformName.WatchOS:
			case PlatformName.TvOS:
				print ("global::{0}.UIApplication.EnsureUIThread ();", ns.Get ("UIKit"));
				break;
			case PlatformName.MacOSX:
				print ("global::{0}.NSApplication.EnsureUIThread ();", ns.Get ("AppKit"));
				break;
			default:
				throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
		}
	}

	// Stret.NeedStret is shared between generator and X.I dll so in order to wrap the exception
	// into a BindingException we need to set the try/catch here so we can provide a better message. Bugzilla ref 51212.
	bool CheckNeedStret (MethodInfo mi)
	{
		try {
			return Stret.NeedStret (mi.ReturnType);
		}
		catch (TypeLoadException ex) {
			throw new BindingException (0001, true, $"The .NET runtime could not load the {mi.ReturnType.Name} type. Message: {ex.Message}");
		}
	}

	//
	// The NullAllowed can be applied on a property, to avoid the ugly syntax, we allow it on the property
	// So we need to pass this as `null_allowed_override',   This should only be used by setters.
	//
	public void GenerateMethodBody (MemberInformation minfo, MethodInfo mi, string sel, bool null_allowed_override, string var_name, BodyOption body_options, PropertyInfo propInfo = null)
	{
		var type = minfo.type;
		var category_type = minfo.category_extension_type;
		var is_appearance = minfo.is_appearance;
		var dual_enum = HasNativeEnumInSignature (mi);
		TrampolineInfo trampoline_info = null;
		EnumMode[] enum_modes;
		if (dual_enum) {
			enum_modes = new EnumMode[] { EnumMode.Bit32, EnumMode.Bit64 };
		} else {
			enum_modes = new EnumMode[] { EnumMode.Bit32 };
		}

		CurrentMethod = String.Format ("{0}.{1}", type.Name, mi.Name);

		// Warn about [Static] used in a member of [Category]
		var hasStaticAtt = AttributeManager.HasAttribute<StaticAttribute> (mi);
		if (category_type != null && hasStaticAtt && !minfo.ignore_category_static_warnings) {
			var baseTypeAtt = AttributeManager.GetCustomAttribute<BaseTypeAttribute> (minfo.type);
			ErrorHelper.Show (new BindingException (1117, "The member '{0}' is decorated with [Static] and its container class {1} is decorated with [Category] this leads to hard to use code. Please inline {0} into {2} class.", mi.Name, type.FullName, baseTypeAtt?.BaseType.FullName));
		}

		indent++;
		// if the namespace/type needs it and if the member is NOT marked as safe (don't check)
		// if the namespace/type does NOT need it and if the member is marked as NOT safe (do check)
		if (type_needs_thread_checks ? (minfo.threadCheck != ThreadCheck.Off) : (minfo.threadCheck == ThreadCheck.On))
			GenerateThreadCheck ();

		Inject<PrologueSnippetAttribute> (mi);

		GenerateArgumentChecks (mi, null_allowed_override, propInfo);

		// Collect all strings that can be fast-marshalled
		List<string> stringParameters = CollectFastStringMarshalParameters (mi);

		StringBuilder[] args2 = null, convs2 = null, disposes2 = null, by_ref_processing2 = null, by_ref_init2 = null;
		args2 = new StringBuilder[enum_modes.Length];
		convs2 = new StringBuilder[enum_modes.Length];
		disposes2 = new StringBuilder[enum_modes.Length];
		by_ref_processing2 = new StringBuilder[enum_modes.Length];
		by_ref_init2 = new StringBuilder[enum_modes.Length];
		for (int i = 0; i < enum_modes.Length; i++) {
			GenerateTypeLowering (mi, null_allowed_override, enum_modes [i], out args2[i], out convs2[i], out disposes2[i], out by_ref_processing2[i], out by_ref_init2[i], propInfo);
		}

		// sanity check
		if (enum_modes.Length > 1) {
			bool sane = true;
			sane &= convs2 [0].ToString () == convs2 [1].ToString ();
			sane &= disposes2 [0].ToString () == disposes2 [1].ToString ();
			sane &= by_ref_processing2 [0].ToString () == by_ref_processing2 [1].ToString ();
			sane &= by_ref_init2 [0].ToString () == by_ref_init2 [1].ToString ();
			if (!sane)
				throw new BindingException (1028, "Internal sanity check failed, please file a bug report (http://bugzilla.xamarin.com) with a test case.");
		}

		var convs = convs2 [0];
		var disposes = disposes2 [0];
		var by_ref_processing = by_ref_processing2 [0];
		var by_ref_init = by_ref_init2 [0];
		var argsArray = new string[args2.Length];
		for (int i = 0; i < args2.Length; i++)
			argsArray [i] = args2 [i].ToString ();

		if (by_ref_init.Length > 0)
			print (by_ref_init.ToString ());
				      
 		if (stringParameters != null){
 			print ("fixed (char * {0}){{",
			       stringParameters.Select (name => "_p" + name + " = " + name).Aggregate ((first,second) => first + ", " + second));
 			indent++;
 		}

		if (propInfo != null && IsSetter (mi) && HasBindAsAttribute (propInfo)) {
			convs.AppendFormat ("var nsb_{0} = {1}\n", propInfo.Name, GetToBindAsWrapper (minfo));
		}

		if (convs.Length > 0)
			print (sw, convs.ToString ());

		Inject<PreSnippetAttribute> (mi);
		var align = AttributeManager.GetCustomAttribute<AlignAttribute> (mi);

		PostGetAttribute [] postget = null;
		// [PostGet] are not needed (and might not be available) when generating methods inside Appearance types
		// However we want them even if ImplementsAppearance is true (i.e. the original type needs them)
		if (!is_appearance) {
			postget = AttributeManager.GetCustomAttributes<PostGetAttribute> (mi);
			if (postget.Length == 0 && propInfo != null)
				postget = AttributeManager.GetCustomAttributes<PostGetAttribute> (propInfo);

			if (postget != null && postget.Length == 0)
				postget = null;
		}

		// Types inside marshal_types that does not have a custom create: needs a IntPtr zero check before they return see Bug 28271
		MarshalType marshalType;
		bool needsPtrZeroCheck = LookupMarshal (mi.ReturnType, out marshalType) && !marshalType.HasCustomCreate;

		bool use_temp_return  =
			minfo.is_return_release ||
			(mi.Name != "Constructor" && (CheckNeedStret (mi) || disposes.Length > 0 || postget != null) && mi.ReturnType != TypeManager.System_Void) ||
			(AttributeManager.HasAttribute<FactoryAttribute> (mi)) ||
			((body_options & BodyOption.NeedsTempReturn) == BodyOption.NeedsTempReturn) ||
			(mi.ReturnType.IsSubclassOf (TypeManager.System_Delegate)) ||
			(AttributeManager.HasAttribute<ProxyAttribute> (AttributeManager.GetReturnTypeCustomAttributes (mi))) ||
			(!Compat && IsNativeEnum (mi.ReturnType)) ||
			(mi.Name != "Constructor" && by_ref_processing.Length > 0 && mi.ReturnType != TypeManager.System_Void) ||
			needsPtrZeroCheck;

		if (use_temp_return) {
			if (mi.ReturnType.IsSubclassOf (TypeManager.System_Delegate)) {
				print ("IntPtr ret;");
				trampoline_info = MakeTrampoline (mi.ReturnType);
			} else if (align != null) {
				print ("{0} ret = default({0});", FormatType (mi.DeclaringType, mi.ReturnType));
				print ("IntPtr ret_alloced = Marshal.AllocHGlobal (Marshal.SizeOf (typeof ({0})) + {1});", FormatType (mi.DeclaringType, mi.ReturnType), align.Align);
				print ("IntPtr aligned_ret = new IntPtr (((nint) (ret_alloced + {0}) >> {1}) << {1});", align.Align - 1, align.Bits);
				print ("bool aligned_assigned = false;");
			} else if (minfo.protocolize) {
				print ("{0} ret;", FormatType (mi.DeclaringType, mi.ReturnType.Namespace, FindProtocolInterface (mi.ReturnType, mi)));
			} else if (needsPtrZeroCheck) {
				print ("IntPtr ret;");
			} else if (minfo.is_bindAs) {
				var bindAsAttrib = GetBindAsAttribute (minfo.mi);
				print ("{0} ret;", FormatType (bindAsAttrib.Type.DeclaringType, GetCorrectGenericType (bindAsAttrib.Type)));
			} else
				print ("{0} ret;", FormatType (mi.DeclaringType, GetCorrectGenericType (mi.ReturnType))); //  = new {0} ();"
		}
		
		bool needs_temp = use_temp_return || disposes.Length > 0;
		if (minfo.is_virtual_method || mi.Name == "Constructor"){
			//print ("if (this.GetType () == TypeManager.{0}) {{", type.Name);
			if (external || minfo.is_interface_impl || minfo.is_extension_method) {
				if (dual_enum) {
					print ("if (IntPtr.Size == 8) {");
					indent++;
					GenerateInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type);
					indent--;
					print ("} else {");
					indent++;
					GenerateInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type);
					indent--;
					print ("}");
				} else {
					GenerateInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type);
				}
			} else {
				var may_throw = ShouldMarshalNativeExceptions (mi);
				var null_handle = may_throw && mi.Name == "Constructor";
				if (null_handle) {
					print ("try {");
					indent++;
				}

				WriteIsDirectBindingCondition (sw, ref indent, is_direct_binding,
							       mi.Name == "Constructor" ? is_direct_binding_value : null, // We only need to print the is_direct_binding value in constructors
				                               () => { GenerateInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type); return null; },
				                               () => { GenerateInvoke (true, mi, minfo, sel, argsArray, needs_temp, category_type); return null; }
							      );
				
				if (null_handle) {
					indent--;
					print ("} catch {");
					indent++;
					print ("Handle = IntPtr.Zero;");
					print ("throw;");
					indent--;
					print ("}");
				}
			}
		} else {
			GenerateInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type);
		}
		
		if (minfo.is_return_release) {
			if (!needsPtrZeroCheck)
				print ("{0}.void_objc_msgSend (ret.Handle, Selector.GetHandle (Selector.Release));", ns.Messaging);
			else {
				// We must create the managed wrapper before calling Release on it
				// FIXME: https://trello.com/c/1ukS9TbL/43-introduce-common-object-type-for-all-unmanaged-types-which-will-correctly-implement-idisposable-and-inativeobject
				// We should consider using return INativeObject<T> (ptr, bool); here at some point
				print ("global::{0} relObj = ret == IntPtr.Zero ? null : new global::{0} (ret);", mi.ReturnType.FullName);
				print ("if (relObj != null) global::{0}.void_objc_msgSend (relObj.Handle, Selector.GetHandle (Selector.Release));", ns.Messaging);
			}
		}
		
		Inject<PostSnippetAttribute> (mi);

		if (disposes.Length > 0)
			print (sw, disposes.ToString ());
		if ((body_options & BodyOption.StoreRet) == BodyOption.StoreRet) {
			// nothing to do
		} else if ((body_options & BodyOption.CondStoreRet) == BodyOption.CondStoreRet) {
			// nothing to do
		} else if ((body_options & BodyOption.MarkRetDirty) == BodyOption.MarkRetDirty) {
			print ("MarkDirty ();");
			print ("{0} = ret;", var_name);
		}

		if ((postget != null) && (postget.Length > 0)) {
			print ("#pragma warning disable 168");
			for (int i = 0; i < postget.Length; i++) {
				if (IsDisableForNewRefCount (postget [i], type))
					continue;


				bool version_check = false;
				if (CurrentPlatform != PlatformName.MacOSX) {
					// bug #7742: if this code, e.g. existing in iOS 2.0, 
					// tries to call a property available since iOS 5.0, 
					// then it will fail when executing in iOS 4.3
					var postget_avail = GetIntroduced (type, postget [i].MethodName);
					if (postget_avail != null) {
						var caller_avail = GetIntroduced (mi, propInfo) ?? iOSIntroducedDefault;
						if (caller_avail.Version < postget_avail.Version) {
							version_check = true;
							print ("var postget{0} = {4}.UIDevice.CurrentDevice.CheckSystemVersion ({1},{2}) ? {3} : null;",
								i,
								postget_avail.Version.Major,
								postget_avail.Version.Minor,
								postget [i].MethodName,
								ns.Get ("UIKit"));
						}
					}
				}
				if (!version_check)
					print ("var postget{0} = {1};", i, postget [i].MethodName);
			}
			print ("#pragma warning restore 168");
		}
		
		if (AttributeManager.HasAttribute<FactoryAttribute> (mi))
			print ("ret.Release (); // Release implicit ref taken by GetNSObject");
		if (by_ref_processing.Length > 0)
			print (sw, by_ref_processing.ToString ());
		if (use_temp_return) {
			if (AttributeManager.HasAttribute<ProxyAttribute> (AttributeManager.GetReturnTypeCustomAttributes (mi)))
				print ("ret.SetAsProxy ();");

			if (mi.ReturnType.IsSubclassOf (TypeManager.System_Delegate)) {
				print ("return global::{0}.Trampolines.{1}.Create (ret);", ns.CoreObjCRuntime, trampoline_info.NativeInvokerName);
			} else if (align != null) {
				print ("if (aligned_assigned)");
				indent++;
				print ("unsafe {{ ret = *({0} *) aligned_ret; }}", FormatType (mi.DeclaringType, mi.ReturnType));
				indent--;
				print ("Marshal.FreeHGlobal (ret_alloced);");
				print ("return ret;");
			} else if (needsPtrZeroCheck) {
				if (minfo.is_return_release)
					print ("return relObj;");
				else {
					// FIXME: https://trello.com/c/1ukS9TbL/43-introduce-common-object-type-for-all-unmanaged-types-which-will-correctly-implement-idisposable-and-inativeobject
					// We should consider using return INativeObject<T> (ptr, bool); here at some point
					print ("return ret == IntPtr.Zero ? null : new global::{0} (ret);", mi.ReturnType.FullName);
				}
			} else {
				print ("return ret;");
			}
		}
		if (minfo.is_ctor)
			WriteMarkDirtyIfDerived (sw, mi.DeclaringType);
		if (stringParameters != null){
			indent--;
			print ("}");
		}
		indent--;
	}


	static PropertyInfo GetProperty (PostGetAttribute @this, Type type)
	{
		if (type == null || type == TypeManager.System_Object)
			return null;

		var props = type.GetProperties ();
		foreach (var pi in props) {
			if (pi.Name != @this.MethodName)
				continue;
			return pi;
		}
		return GetProperty (@this, ReflectionExtensions.GetBaseType (type));
	}

	static bool IsDisableForNewRefCount (PostGetAttribute @this, Type type)
	{
		PropertyInfo p = GetProperty (@this, type);
		var ea = AttributeManager.GetCustomAttributes<ExportAttribute> (p);
		var sem = ea [0].ArgumentSemantic;
		return (sem != ArgumentSemantic.Assign && sem != ArgumentSemantic.Weak); // also cover UnsafeUnretained
	}

	public IEnumerable<MethodInfo> GetTypeContractMethods (Type source)
	{
		if (source.IsEnum)
			yield break;
		foreach (var method in source.GatherMethods (BindingFlags.Public | BindingFlags.Instance))
			yield return method;
		foreach (var parent in source.GetInterfaces ()){
			// skip case where the interface implemented comes from an already built assembly (e.g. monotouch.dll)
			// e.g. Dispose won't have an [Export] since it's present to satisfy System.IDisposable
			if (parent.FullName != "System.IDisposable") {
				foreach (var method in parent.GatherMethods (BindingFlags.Public | BindingFlags.Instance)) {
					yield return method;
				}
			}
		}
	}

	public IEnumerable<PropertyInfo> GetTypeContractProperties (Type source)
	{
		foreach (var prop in source.GatherProperties ())
			yield return prop;
		foreach (var parent in source.GetInterfaces ()){
			// skip case where the interface implemented comes from an already built assembly (e.g. monotouch.dll)
			// e.g. the Handle property won't have an [Export] since it's present to satisfyINativeObject
			if (parent.Name != "INativeObject") {
				foreach (var prop in parent.GatherProperties ())
					yield return prop;
			}
		}
	}

	//
	// This is used to determine if the memberType is in the declaring type or in any of the
	// inherited versions of the type.   We use this now, since we support inlining protocols
	//
	public static bool MemberBelongsToType (Type memberType, Type hostType)
	{
		if (memberType == hostType)
			return true;
		// we also need to inline the base type, e.g. UITableViewDelegate must bring UIScrollViewDelegate
		if (MemberBelongToInterface (memberType, hostType)) {
			return true;
		}
		return false;
	}

	static bool MemberBelongToInterface (Type memberType, Type intf)
	{
		if (memberType == intf)
			return true;
		foreach (var p in intf.GetInterfaces ()) {
			if (memberType == p)
				return true;
			if (MemberBelongToInterface (memberType, ReflectionExtensions.GetBaseType (p)))
				return true;
		}
		return false;
	}
	
	Dictionary<string,object> generatedEvents = new Dictionary<string,object> ();
	Dictionary<string,object> generatedDelegates = new Dictionary<string,object> ();

	bool DoesTypeNeedBackingField (Type type) {
		return IsWrappedType (type) || (type.IsArray && IsWrappedType (type.GetElementType ()));
	}

	bool DoesPropertyNeedBackingField (PropertyInfo pi) {
		return DoesTypeNeedBackingField (pi.PropertyType) && !AttributeManager.HasAttribute<TransientAttribute> (pi);
	}
	
	bool DoesPropertyNeedDirtyCheck (PropertyInfo pi, ExportAttribute ea) 
	{
		switch (ea.ArgumentSemantic) {
		case ArgumentSemantic.Copy:
		case ArgumentSemantic.Retain: // same as Strong
		case ArgumentSemantic.None:
			return DoesPropertyNeedBackingField (pi);
		default: // Assign (same as UnsafeUnretained) or Weak
			return false;
		}
	}

	void PrintPropertyAttributes (PropertyInfo pi)
	{
		foreach (var oa in AttributeManager.GetCustomAttributes<ObsoleteAttribute> (pi)) {
			print ("[Obsolete (\"{0}\", {1})]", oa.Message, oa.IsError ? "true" : "false");
			print ("[EditorBrowsable (EditorBrowsableState.Never)]");
		}

		foreach (var ba in AttributeManager.GetCustomAttributes<DebuggerBrowsableAttribute> (pi)) 
			print ("[DebuggerBrowsable (DebuggerBrowsableState.{0})]", ba.State);

		foreach (var da in AttributeManager.GetCustomAttributes<DebuggerDisplayAttribute> (pi)) {
			var narg = da.Name != null ? string.Format (", Name = \"{0}\"", da.Name) : string.Empty;
			var targ = da.Type != null ? string.Format (", Type = \"{0}\"", da.Type) : string.Empty;
			print ("[DebuggerDisplay (\"{0}\"{1}{2})]", da.Value, narg, targ);
		}
		foreach (var oa in AttributeManager.GetCustomAttributes<OptionalImplementationAttribute> (pi)) {
			print ("[DebuggerBrowsable (DebuggerBrowsableState.Never)]");
		}

		PrintPlatformAttributes (pi);

		foreach (var sa in AttributeManager.GetCustomAttributes<ThreadSafeAttribute> (pi))
			print (sa.Safe ? "[ThreadSafe]" : "[ThreadSafe (false)]");
	}

	void GenerateProperty (Type type, PropertyInfo pi, List<string> instance_fields_to_clear_on_dispose, bool is_model, bool is_interface_impl = false)
	{
		string wrap;
		var export = GetExportAttribute (pi, out wrap);
		var minfo = new MemberInformation (this, pi, type, is_interface_impl);
		bool use_underscore = minfo.is_unified_internal;
		var mod = minfo.GetVisibility ();
		minfo.protocolize = Protocolize (pi);

		if (UnifiedAPI) { // We have some sub-optimal bindings in compat that trigger this exception, so we just don't fix those
			// So we don't hide the get or set of a parent property with the same name, we need to see if we have a parent declaring the same property
			PropertyInfo parentBaseType = GetParentTypeWithSameNamedProperty (ReflectionExtensions.GetBaseTypeAttribute(type), pi.Name);

			// If so, we're not static, and we can't both read and write, but they can
			if (!minfo.is_static && !(pi.CanRead && pi.CanWrite) && (parentBaseType != null && parentBaseType.CanRead && parentBaseType.CanWrite)) {
				// Make sure the selector matches, sanity check that we aren't hiding something of a different type
				// We skip this for wrap'ed properties, as those get complicated to resolve the correct export
				if (wrap == null &&
					((pi.CanRead && (GetGetterExportAttribute (pi).Selector != GetGetterExportAttribute (parentBaseType).Selector)) ||
					 pi.CanWrite && (GetSetterExportAttribute (pi).Selector != GetSetterExportAttribute (parentBaseType).Selector))) {
					throw new BindingException (1035, true, "The property {0} on class {1} is hiding a property from a parent class {2} but the selectors do not match.", pi.Name, type, parentBaseType.DeclaringType);
				}
				// Then let's not write out our copy, since we'll reduce visibility
				return;
			}
		}

		if (UnifiedAPI && !BindThirdPartyLibrary) {
			var elType = pi.PropertyType.IsArray ? pi.PropertyType.GetElementType () : pi.PropertyType;

			if (IsModel (elType) && !minfo.protocolize) {
				ErrorHelper.Show (new BindingException (1110, false, "The property {0}.{1} exposes a model ({2}). Please expose the corresponding protocol type instead ({3}.I{4}).", pi.DeclaringType, pi.Name, pi.PropertyType, pi.PropertyType.Namespace, pi.PropertyType.Name));
			}
		}

		if (wrap != null){
			print_generated_code ();
			PrintPropertyAttributes (pi);
			PrintAttributes (pi, preserve:true, advice:true);
			print ("{0} {1}{2} {3}{4} {{",
			       mod,
			       minfo.GetModifiers (),
			       (minfo.protocolize ? "I" : "") + FormatType (pi.DeclaringType, GetCorrectGenericType (pi.PropertyType)),
					pi.Name.GetSafeParamName (),
			       use_underscore ? "_" : "");
			indent++;
			if (pi.CanRead) {
				PrintAttributes (pi, platform:true);
				PrintAttributes (pi.GetGetMethod (), platform:true, preserve:true, advice:true);
				print ("get {");
				indent++;

				if (IsDictionaryContainerType (pi.PropertyType)) {
					print ("var src = {0} != null ? new NSMutableDictionary ({0}) : null;", wrap);
					print ("return src == null ? null : new {0}(src);", FormatType (pi.DeclaringType, pi.PropertyType));
				} else {
					if (IsArrayOfWrappedType (pi.PropertyType))
						print ("return NSArray.FromArray<{0}>({1} as NSArray);", FormatType (pi.DeclaringType, pi.PropertyType.GetElementType ()), wrap);
					else if (pi.PropertyType.IsValueType)
						print ("return ({0}) ({1});", FormatType (pi.DeclaringType, pi.PropertyType), wrap);
					else
						print ("return {0} as {1}{2};", wrap, minfo.protocolize ? "I" : String.Empty, FormatType (pi.DeclaringType, pi.PropertyType));
				}
				indent--;
				print ("}");
			}
			if (pi.CanWrite) {
				PrintAttributes (pi, platform:true);
				PrintAttributes (pi.GetSetMethod (), platform:true, preserve:true, advice:true);
				print ("set {");
				indent++;

				var is_protocol_wrapper = UnifiedAPI && IsProtocolInterface (pi.PropertyType, false);

				if (minfo.protocolize || is_protocol_wrapper){
					print ("var rvalue = value as NSObject;");
					print ("if (value != null && rvalue == null)");
					print ("\tthrow new ArgumentException (\"The object passed of type \" + value.GetType () + \" does not derive from NSObject\");");
				}
							
				if (IsDictionaryContainerType (pi.PropertyType))
					print ("{0} = value == null ? null : value.Dictionary;", wrap);
				else {
					if (IsArrayOfWrappedType (pi.PropertyType))
						print ("{0} = NSArray.FromNSObjects (value);", wrap);
					else 
						print ("{0} = {1}value;", wrap, minfo.protocolize || is_protocol_wrapper ? "r" : "");
				}
				indent--;
				print ("}");
			}

			indent--;
			print ("}\n");
			return;
		}

		string var_name = null;
		
		if (wrap == null) {
			// [Model] has properties that only throws, so there's no point in adding unused backing fields
			if (!is_model && DoesPropertyNeedBackingField (pi) && !is_interface_impl && !minfo.is_static && !DoesPropertyNeedDirtyCheck (pi, export)) {
				var_name = string.Format ("__mt_{0}_var{1}", pi.Name, minfo.is_static ? "_static" : "");

				print_generated_code ();

				if (minfo.is_thread_static)
					print ("[ThreadStatic]");
				print ("{1}object {0};", var_name, minfo.is_static ? "static " : "");

				if (!minfo.is_static && !is_interface_impl){
					instance_fields_to_clear_on_dispose.Add (var_name);
				}
			}
		}

		print_generated_code ();
		PrintPropertyAttributes (pi);

		// when we inline properties (e.g. from a protocol)
		// we must look if the type has an [Availability] attribute
		PrintPlatformAttributesIfInlined (minfo);

		PrintAttributes (pi, preserve:true, advice:true, bindAs:true);

		string propertyTypeName;
		if (minfo.protocolize) {
			propertyTypeName = FindProtocolInterface (pi.PropertyType, pi);
		} else if (minfo.is_bindAs) {
			var bindAsAttrib = GetBindAsAttribute (minfo.mi);
			propertyTypeName = FormatType (bindAsAttrib.Type.DeclaringType, GetCorrectGenericType (bindAsAttrib.Type));
		} else {
			propertyTypeName = FormatType (pi.DeclaringType, GetCorrectGenericType (pi.PropertyType));
		}

		print ("{0} {1}{2} {3}{4} {{",
		       mod,
		       minfo.GetModifiers (),
			   propertyTypeName,
				pi.Name.GetSafeParamName (),
		       use_underscore ? "_" : "");
		indent++;

		if (wrap != null) {
			if (pi.CanRead) {
				PrintPlatformAttributes (pi);
				PrintPlatformAttributes (pi.GetGetMethod ());
				print ("get {{ return {0} as {1}; }}", wrap, FormatType (pi.DeclaringType, GetCorrectGenericType (pi.PropertyType)));
			}
			if (pi.CanWrite) {
				PrintPlatformAttributes (pi);
				PrintPlatformAttributes (pi.GetSetMethod ());
				print ("set {{ {0} = value; }}", wrap);
			}
			indent--;
			print ("}\n");
			return;			
		} else if (minfo.has_inner_wrap_attribute) {
			// If property getter or setter has its own WrapAttribute we let the user do whatever their heart desires
			if (pi.CanRead) {
				PrintAttributes (pi, platform: true);
				PrintAttributes (pi.GetGetMethod (), platform: true, preserve: true, advice: true);
				print ("get {");
				indent++;

				print ($"return {minfo.wpmi.WrapGetter};");

				indent--;
				print ("}");
			}
			if (pi.CanWrite) {
				var setter = pi.GetSetMethod ();
				var not_implemented_attr = AttributeManager.GetCustomAttribute<NotImplementedAttribute> (setter);

				PrintAttributes (pi, platform: true);
				PrintAttributes (setter, platform: true, preserve: true, advice: true, notImplemented: true);
				print ("set {");
				indent++;

				if (not_implemented_attr != null)
					print ("throw new NotImplementedException ({0});", not_implemented_attr.Message == null ? "" : $@"""{not_implemented_attr.Message}""");
				else
					print ($"{minfo.wpmi.WrapSetter};");

				indent--;
				print ("}");
			}
			indent--;
			print ("}\n");
			return;
		}

		if (pi.CanRead){
			var getter = pi.GetGetMethod ();
			var ba = GetBindAttribute (getter);
			string sel = ba != null ? ba.Selector : export.Selector;

			PrintAttributes (pi, platform:true);

			if (!minfo.is_sealed || !minfo.is_wrapper) {
				PrintDelegateProxy (pi.GetGetMethod ());
				PrintExport (minfo, sel, export.ArgumentSemantic);
			}

			PrintAttributes (pi.GetGetMethod(), platform:true, preserve:true, advice:true, notImplemented:true);
			if (minfo.is_abstract){
				print ("get; ");
			} else {
				print ("get {");
				if (debug)
					print ("Console.WriteLine (\"In {0}\");", pi.GetGetMethod ());
				if (is_model)
					print ("\tthrow new ModelNotImplementedException ();");
				else {
					if (minfo.is_autorelease) {
						indent++;
						print ("using (var autorelease_pool = new NSAutoreleasePool ()) {");
					}
					if (is_interface_impl || !DoesPropertyNeedBackingField (pi)) {
						GenerateMethodBody (minfo, getter, sel, false, null, BodyOption.None, pi);
					} else if (minfo.is_static) {
						GenerateMethodBody (minfo, getter, sel, false, var_name, BodyOption.StoreRet, pi);
					} else {
						if (DoesPropertyNeedDirtyCheck (pi, export))
							GenerateMethodBody (minfo, getter, sel, false, var_name, BodyOption.CondStoreRet, pi);
						else
							GenerateMethodBody (minfo, getter, sel, false, var_name, BodyOption.MarkRetDirty, pi);
					}
					if (minfo.is_autorelease) {
						print ("}");
						indent--;
					}
				}
				print ("}\n");
			}
		}
		if (pi.CanWrite){
			var setter = pi.GetSetMethod ();
			var ba = GetBindAttribute (setter);
			bool null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (pi) || AttributeManager.HasAttribute<NullAllowedAttribute> (setter);
			var not_implemented_attr = AttributeManager.GetCustomAttribute<NotImplementedAttribute> (setter);
			string sel;

			if (ba == null) {
				sel = GetSetterExportAttribute (pi)?.Selector;
			} else {
				sel = ba.Selector;
			}

			PrintBlockProxy (pi.PropertyType);
			PrintAttributes (pi, platform:true);

			if (not_implemented_attr == null && (!minfo.is_sealed || !minfo.is_wrapper))
				PrintExport (minfo, sel, export.ArgumentSemantic);

			PrintAttributes (pi.GetSetMethod (), platform:true, preserve:true, advice:true, notImplemented:true);
			if (minfo.is_abstract){
				print ("set; ");
			} else {
				print ("set {");
				if (debug)
					print ("Console.WriteLine (\"In {0}\");", pi.GetSetMethod ());

				// If we're doing a setter for a weak property that is protocolized event back
				// we need to put in a check to verify you aren't stomping the "internal underscore"
				// generated delegate. We check CheckForEventAndDelegateMismatches global to disable the checks
				if (!BindThirdPartyLibrary && pi.Name.StartsWith ("Weak", StringComparison.Ordinal)) {
					string delName = pi.Name.Substring(4);
					if (SafeIsProtocolizedEventBacked (delName, type))
						print ("\t{0}.EnsureDelegateAssignIsNotOverwritingInternalDelegate ({1}, value, {2});", ApplicationClassName, string.IsNullOrEmpty (var_name) ? "null" : var_name, GetDelegateTypePropertyName (delName));
				}

				if (not_implemented_attr != null){
					print ("\tthrow new NotImplementedException ({0});", not_implemented_attr.Message == null ? "" : "\"" + not_implemented_attr.Message + "\"");
				} else if (is_model)
					print ("\tthrow new ModelNotImplementedException ();");
				else {
					GenerateMethodBody (minfo, setter, sel, null_allowed, null, BodyOption.None, pi);
					if (!minfo.is_static && !is_interface_impl && DoesPropertyNeedBackingField (pi)) {
						if (!DoesPropertyNeedDirtyCheck (pi, export)) {
							print ("\tMarkDirty ();");
							print ("\t{0} = value;", var_name);
						}
					}
				}
				print ("}");
			}
		}
		indent--;
		print ("}}\n", pi.Name.GetSafeParamName ());
	}

	class AsyncMethodInfo : MemberInformation {
		public ParameterInfo[] async_initial_params, async_completion_params;
		public bool has_nserror, is_void_async, is_single_arg_async;
		public MethodInfo MethodInfo;
		
		public AsyncMethodInfo (IMemberGatherer gather, Type type, MethodInfo mi, Type category_extension_type, bool is_extension_method) : base (gather, mi, type, category_extension_type, false, is_extension_method)
		{
			this.MethodInfo = mi;
			this.async_initial_params = Generator.DropLast (mi.GetParameters ());

			var lastType = mi.GetParameters ().Last ().ParameterType;
			if (!lastType.IsSubclassOf (TypeManager.System_Delegate))
				throw new BindingException (1036, true, "The last parameter in the method '{0}.{1}' must be a delegate (it's '{2}').", mi.DeclaringType.FullName, mi.Name, lastType.FullName);
			var cbParams = lastType.GetMethod ("Invoke").GetParameters ();
			async_completion_params = cbParams;

			// ?!? this fails: cbParams.Last ().ParameterType.Name == TypeManager.NSError
			if (cbParams.Length > 0 && cbParams.Last ().ParameterType.Name == "NSError") {
				has_nserror = true;
				cbParams = Generator.DropLast (cbParams);
			}
			if (cbParams.Length == 0)
				is_void_async = true;
			if (cbParams.Length == 1)
				is_single_arg_async = true;
		}

		public string GetUniqueParamName (string suggestion)
		{
			while (true) {
				bool next = false;

				foreach (var pi in async_completion_params) {
					if (pi.Name == suggestion) {
						next = true;
						break;
					}
				}

				if (!next)
					return suggestion;

				suggestion = "_" + suggestion;
			}
		}

	}

	public static T[] DropLast<T> (T[] arr)
	{
		T[] res = new T [arr.Length - 1];
		Array.Copy (arr, res, res.Length);
		return res;
	}

	string GetReturnType (AsyncMethodInfo minfo)
	{
		if (minfo.is_void_async)
			return "Task";
		var ttype = GetAsyncTaskType (minfo);
		if (UnifiedAPI && minfo.has_nserror && (ttype == "bool"))
			ttype = "Tuple<bool,NSError>";
		return "Task<" + ttype + ">";
	}

	string GetAsyncTaskType (AsyncMethodInfo minfo)
	{
		if (minfo.is_single_arg_async)
			return FormatType (minfo.type, minfo.async_completion_params [0].ParameterType);

		var attr = AttributeManager.GetCustomAttribute<AsyncAttribute> (minfo.mi);
		if (attr.ResultTypeName != null)
			return attr.ResultTypeName;
		if (attr.ResultType != null)
			return FormatType (minfo.type, attr.ResultType);

		Console.WriteLine ("{0}", minfo.MethodInfo.GetParameters ().Last ().ParameterType);
		throw new BindingException (1023, true, "Async method {0} with more than one result parameter in the callback by neither ResultTypeName or ResultType", minfo.mi);
	}

	string GetInvokeParamList (ParameterInfo [] parameters, bool suffix = true)
	{
		StringBuilder sb = new StringBuilder ();
		bool comma = false;
		foreach (var pi in parameters) {
			if (comma) {
				sb.Append (", ");
			}
			comma = true;
			sb.Append (pi.Name.GetSafeParamName ());
			if (suffix)
				sb.Append ('_');
		}
		return sb.ToString ();
	}

	//
	// The kind of Async method we generate
	// We typically generate a single one, but if the
	// async method has a non-void return, we generate the second with the out parameter
	// 
	enum AsyncMethodKind {
		// Plain Async method, original method return void
		Plain,

		// Async method generated when we had a return type from the method
		// ie: [Async] string XyZ (Action completion), the "string" is the
		// result
		WithResultOutParameter,
	}
	
	void PrintAsyncHeader (AsyncMethodInfo minfo, AsyncMethodKind asyncKind)
	{
		print_generated_code ();
		string extra = "";

		if (asyncKind == AsyncMethodKind.WithResultOutParameter) {
			if (minfo.method.GetParameters ().Count () > 1)
				extra = ", ";
			extra += "out " + FormatType (minfo.MethodInfo.DeclaringType, minfo.MethodInfo.ReturnType) + " " + minfo.GetUniqueParamName ("result");
		}

		// async wrapper don't have to be abstract (it's counter productive)
		var modifier = minfo.GetModifiers ().Replace ("abstract ", String.Empty);

		print ("{0} {1}{2} {3}",
		       minfo.GetVisibility (),
		       modifier,
		       GetReturnType (minfo),
		       MakeSignature (minfo, true, minfo.async_initial_params, extra),
		       minfo.is_abstract ? ";" : "");
	}

	void GenerateAsyncMethod (MemberInformation original_minfo, AsyncMethodKind asyncKind)
	{
		var mi = original_minfo.method;
		var minfo = new AsyncMethodInfo (this, original_minfo.type, mi, original_minfo.category_extension_type, original_minfo.is_extension_method);
		var is_void = mi.ReturnType == TypeManager.System_Void;

		// Print a error if any of the method parameters or handler parameters is ref/out, it should not be asyncified.
		if (minfo.async_initial_params != null) {
			foreach (var param in minfo.async_initial_params) {
				if (param.ParameterType.IsByRef) {
					throw new BindingException (1062, true, $"The member '{original_minfo.type.Name}.{mi.Name}' contains ref/out parameters and must not be decorated with [Async].");
				}
			}
		}
		foreach (var param in minfo.async_completion_params) {
			if (param.ParameterType.IsByRef) {
				throw new BindingException (1062, true, $"The member '{original_minfo.type.Name}.{mi.Name}' contains ref/out parameters and must not be decorated with [Async].");
			}
		}

		PrintMethodAttributes (minfo);

		PrintAsyncHeader (minfo, asyncKind);

		print ("{");
		indent++;

		var ttype = "bool";
		var tuple = false;
		if (!minfo.is_void_async) {
			ttype = GetAsyncTaskType (minfo);
			tuple = (UnifiedAPI && minfo.has_nserror && (ttype == "bool"));
			if (tuple)
				ttype = "Tuple<bool,NSError>";
		}
		print ("var tcs = new TaskCompletionSource<{0}> ();", ttype);
		bool ignoreResult = !is_void &&
			asyncKind == AsyncMethodKind.Plain &&
			AttributeManager.GetCustomAttribute<AsyncAttribute> (mi).PostNonResultSnippet == null;
		print ("{6}{5}{4}{0}({1}{2}({3}) => {{",
			mi.Name,
			GetInvokeParamList (minfo.async_initial_params, false),
			minfo.async_initial_params.Length > 0 ? ", " : "",
			GetInvokeParamList (minfo.async_completion_params),
			minfo.is_extension_method || minfo.is_category_extension ? "This." : string.Empty,
			is_void || ignoreResult ? string.Empty : minfo.GetUniqueParamName ("result") + " = ",
			is_void || ignoreResult ? string.Empty : (asyncKind == AsyncMethodKind.WithResultOutParameter ? string.Empty : "var ")
		);

		indent++;

		int nesting_level = 1;
		if (minfo.has_nserror && !tuple) {
			var var_name = minfo.async_completion_params.Last ().Name.GetSafeParamName ();;
			print ("if ({0}_ != null)", var_name);
			print ("\ttcs.SetException (new NSErrorException({0}_));", var_name);
			print ("else");
			++nesting_level; ++indent;
		}

		if (minfo.is_void_async)
			print ("tcs.SetResult (true);");
		else if (tuple) {
			var cond_name = minfo.async_completion_params [0].Name;
			var var_name = minfo.async_completion_params.Last ().Name;
			print ("tcs.SetResult (new Tuple<bool,NSError> ({0}_, {1}_));", cond_name, var_name);
		} else if (minfo.is_single_arg_async)
			print ("tcs.SetResult ({0}_);", minfo.async_completion_params [0].Name);
		else
			print ("tcs.SetResult (new {0} ({1}));",
				GetAsyncTaskType (minfo),
				GetInvokeParamList (minfo.has_nserror ? DropLast (minfo.async_completion_params) : minfo.async_completion_params));
		indent -= nesting_level;
		print ("});");
		var attr = AttributeManager.GetCustomAttribute<AsyncAttribute> (mi);
		if (asyncKind == AsyncMethodKind.Plain && !string.IsNullOrEmpty (attr.PostNonResultSnippet))
			print (attr.PostNonResultSnippet);
		print ("return tcs.Task;");
		indent--;
		print ("}\n");

		
		if (attr.ResultTypeName != null) {
			if (minfo.has_nserror)
				async_result_types.Add (new Tuple<string, ParameterInfo[]> (attr.ResultTypeName, DropLast (minfo.async_completion_params)));
			else
				async_result_types.Add (new Tuple<string, ParameterInfo[]> (attr.ResultTypeName, minfo.async_completion_params));
		}
	}

	void PrintMethodAttributes (MemberInformation minfo)
	{
		MethodInfo mi = minfo.method;

		foreach (var oa in AttributeManager.GetCustomAttributes<ObsoleteAttribute> (mi)) {
			print ("[Obsolete (\"{0}\", {1})]",
			       oa.Message, oa.IsError ? "true" : "false");
			print ("[EditorBrowsable (EditorBrowsableState.Never)]");

		}

		foreach (var sa in AttributeManager.GetCustomAttributes<ThreadSafeAttribute> (mi)) 
			print (sa.Safe ? "[ThreadSafe]" : "[ThreadSafe (false)]");
		
		foreach (var ea in AttributeManager.GetCustomAttributes<EditorBrowsableAttribute> (mi)) {
			if (ea.State == EditorBrowsableState.Always) {
				print ("[EditorBrowsable]");
			} else {
				print ("[EditorBrowsable (EditorBrowsableState.{0})]", ea.State);
			}
		}

		if (minfo.is_return_release)
			print ("[return: ReleaseAttribute ()]");

		PrintPlatformAttributes (minfo.method);
		// when we inline methods (e.g. from a protocol) 
		if (minfo.type != minfo.method.DeclaringType) {
			// we must look if the type has an [Availability] attribute
			PrintPlatformAttributes (minfo.method.DeclaringType);
		}

		foreach (var di in AttributeManager.GetCustomAttributes<DesignatedInitializerAttribute> (mi)) {
			print ("[DesignatedInitializer]");
			break;
		}
	}


	void GenerateMethod (Type type, MethodInfo mi, bool is_model, Type category_extension_type, bool is_appearance, bool is_interface_impl = false, bool is_extension_method = false, string selector = null, bool isBaseWrapperProtocolMethod = false)
	{
		var minfo = new MemberInformation (this, mi, type, category_extension_type, is_interface_impl, is_extension_method, is_appearance, is_model, selector, isBaseWrapperProtocolMethod);
		GenerateMethod (minfo);
	}

	void PrintDelegateProxy (MemberInformation minfo)
	{
		PrintDelegateProxy (minfo.method);
	}

	void PrintDelegateProxy (MethodInfo mi)
	{
		if (mi.ReturnType.IsSubclassOf (TypeManager.System_Delegate)) {
			var ti = MakeTrampoline (mi.ReturnType);
			print ("[return: DelegateProxy (typeof ({0}.Trampolines.{1}))]", ns.CoreObjCRuntime, ti.StaticName);
		}
	}

	void PrintBlockProxy (Type type)
	{
		type = GetCorrectGenericType (type);
		if (type.IsSubclassOf (TypeManager.System_Delegate)) {
			var ti = MakeTrampoline (type);
			print ("[param: BlockProxy (typeof ({0}.Trampolines.{1}))]", ns.CoreObjCRuntime, ti.NativeInvokerName);
		}
	}

	void PrintExport (MemberInformation minfo)
	{
		if (minfo.is_export)
			print ("[Export (\"{0}\"{1})]", minfo.selector, minfo.is_variadic ? ", IsVariadic = true" : string.Empty);
	}

	void PrintExport (MemberInformation minfo, ExportAttribute ea)
	{
		PrintExport (minfo, ea.Selector, ea.ArgumentSemantic);
	}

	void PrintExport (MemberInformation minfo, string sel, ArgumentSemantic semantic)
	{
		bool output_semantics = semantic != ArgumentSemantic.None;
		// it does not make sense on every properties, depending on the their types
		if (output_semantics && (minfo.mi is PropertyInfo)) {
			var t = minfo.property.PropertyType;
			output_semantics = !t.IsPrimitive || t == TypeManager.System_IntPtr;
		}
		
		if (output_semantics)
			print ("[Export (\"{0}\", ArgumentSemantic.{1})]", sel, semantic);
		else
			print ("[Export (\"{0}\")]", sel);
	}

	void GenerateMethod (MemberInformation minfo)
	{
		var mi = minfo.method;

		// skip if we provide a manual implementation that would conflict with the generated code
		if (AttributeManager.HasAttribute<ManualAttribute> (mi))
			return;

		foreach (ParameterInfo pi in mi.GetParameters ())
			if (AttributeManager.HasAttribute<RetainAttribute> (pi)) {
				print ("#pragma warning disable 168");
				print ("{0} __mt_{1}_{2};", pi.ParameterType, mi.Name, pi.Name);
				print ("#pragma warning restore 168");
			}

		int argCount = 0;
		if (minfo.is_export && !minfo.is_variadic) {
			foreach (char c in minfo.selector){
				if (c == ':')
					argCount++;
			}
			if (minfo.method.GetParameters ().Length != argCount) {
				ErrorHelper.Show (new BindingException (1105, false, "Potential selector/argument mismatch [Export (\"{0}\")] has {1} arguments and {2} has {3} arguments",
					minfo.selector, argCount, minfo.method, minfo.method.GetParameters ().Length));
			}
		}

		PrintDelegateProxy (minfo);
		PrintExport (minfo);

		if (!minfo.is_interface_impl) {
			PrintMethodAttributes (minfo);
		}

		var mod = minfo.GetVisibility ();

		var is_abstract = minfo.is_abstract;
		print_generated_code ();
		print ("{0} {1}{2}{3}",
		       mod,
		       minfo.GetModifiers (),
		       MakeSignature (minfo),
		       is_abstract ? ";" : "");


		if (!is_abstract){
			if (minfo.is_ctor) {
				indent++;
				print (": {0}", minfo.wrap_method == null ? "base (NSObjectFlag.Empty)" : minfo.wrap_method);
				indent--;
			}

			print ("{");
			if (debug)
				print ("\tConsole.WriteLine (\"In {0}\");", mi);
					
			if (minfo.is_model)
				print ("\tthrow new You_Should_Not_Call_base_In_This_Method ();");
			else if (minfo.wrap_method != null) {
				if (!minfo.is_ctor) {
					indent++;

					string ret = mi.ReturnType == TypeManager.System_Void ? null : "return ";
					print ("{0}{1}{2};", ret, minfo.is_extension_method ? "This." : "", minfo.wrap_method);
					indent--;
				}
			} else {
				if (minfo.is_autorelease) {
					indent++;
					print ("using (var autorelease_pool = new NSAutoreleasePool ()) {");
				}
				GenerateMethodBody (minfo, minfo.method, minfo.selector, false, null, BodyOption.None, null);
				if (minfo.is_autorelease) {
					print ("}");
					indent--;
				}
			}
			print ("}\n");
		}

		if (AttributeManager.HasAttribute<AsyncAttribute> (mi)) {
			// We do not want Async methods inside internal wrapper classes, they are useless
			// internal sealed class FooWrapper : BaseWrapper, IMyFooDelegate
			// Also we do not want Async members inside [Model] classes
			if (minfo.is_basewrapper_protocol_method || minfo.is_model)
				return;

			GenerateAsyncMethod (minfo, AsyncMethodKind.Plain);

			// Generate the overload with the out parameter
			if (minfo.method.ReturnType != TypeManager.System_Void){
				GenerateAsyncMethod (minfo, AsyncMethodKind.WithResultOutParameter);
			}
		}
	}
	
	public string GetGeneratedTypeName (Type type)
	{
		var bindOnType = AttributeManager.GetCustomAttributes<BindAttribute> (type);
		if (bindOnType.Length > 0)
			return bindOnType [0].Selector;
		else if (type.IsGenericTypeDefinition)
			return type.Name.Substring (0, type.Name.IndexOf ('`'));
		else
			return type.Name;
	}

	void RenderDelegates (Dictionary<string,MethodInfo> delegateTypes)
	{
		// Group the delegates by namespace
		var groupedTypes = from fullname in delegateTypes.Keys
			where fullname != "System.Action"
			let p = fullname.LastIndexOf ('.')
			let ns = p == -1 ? String.Empty : fullname.Substring (0, p)
			group fullname by ns into g
			select new {Namespace = g.Key, Fullname=g};
		
		foreach (var group in groupedTypes.OrderBy (v => v.Namespace, StringComparer.Ordinal)) {
			if (group.Namespace != null) {
				print ("namespace {0} {{", group.Namespace);
				indent++;
			}

			foreach (var deltype in group.Fullname.OrderBy (v => v, StringComparer.Ordinal)) {
				int p = deltype.LastIndexOf ('.');
				var shortName = deltype.Substring (p+1);
				var mi = delegateTypes [deltype];

				if (shortName.StartsWith ("Func<", StringComparison.Ordinal))
					continue;

				var del = mi.DeclaringType;

				if (AttributeManager.HasAttribute (mi.DeclaringType, "MonoNativeFunctionWrapper"))
					print ("[MonoNativeFunctionWrapper]\n");

				print ("public delegate {0} {1} ({2});",
				       RenderType (mi.ReturnType),
				       shortName,
				       RenderParameterDecl (mi.GetParameters ()));
			}

			if (group.Namespace != null) {
				indent--;
				print ("}\n");
			}
		}
	}

	IEnumerable<MethodInfo> SelectProtocolMethods (Type type, bool? @static = null, bool? required = null)
	{
		var list = type.GetMethods (BindingFlags.Public | BindingFlags.Instance);

		foreach (var m in list) {
			if (m.IsSpecialName)
				continue;

			if (m.Name == "Constructor")
				continue;

			var attrs = AttributeManager.GetCustomAttributes<Attribute> (m);
			AvailabilityBaseAttribute availabilityAttribute = null;
			bool hasExportAttribute = false;
			bool hasStaticAttribute = false;
			
			foreach (var a in attrs){
				if (a is AvailabilityBaseAttribute){
					availabilityAttribute = a as AvailabilityBaseAttribute;
				} else if (a is ExportAttribute)
					hasExportAttribute = true;
				else if (a is StaticAttribute)
					hasStaticAttribute = true;
			}
			if (availabilityAttribute != null && m.IsUnavailable ())
				continue;

			if (!hasExportAttribute)
				continue;
			
			if (@static.HasValue && @static.Value != hasStaticAttribute)
				continue;

			if (required.HasValue && required.Value != IsRequired (m, attrs))
				continue;
			
			yield return m;
		}
	}
	
	IEnumerable<PropertyInfo> SelectProtocolProperties (Type type, bool? @static = null, bool? required = null)
	{
		var list = type.GetProperties (BindingFlags.Public | BindingFlags.Instance);

		foreach (var p in list) {
			var attrs = AttributeManager.GetCustomAttributes<Attribute> (p);
			AvailabilityBaseAttribute availabilityAttribute = null;
			bool hasExportAttribute = false;
			bool hasStaticAttribute = false;

			foreach (var a in attrs){
				if (a is AvailabilityBaseAttribute){
					availabilityAttribute = a as AvailabilityBaseAttribute;
				} else if (a is ExportAttribute)
					hasExportAttribute = true;
				else if (a is StaticAttribute)
					hasStaticAttribute = true;
			}
			if (availabilityAttribute != null && p.IsUnavailable ())
				continue;
			
			if (!hasExportAttribute) {
				var getter = p.GetGetMethod ();
				if (p.CanRead && !AttributeManager.HasAttribute<ExportAttribute> (getter)) {
					if (!AttributeManager.HasAttribute<NotImplementedAttribute> (getter))
						continue;
				}
				var setter = p.GetSetMethod ();
				if (p.CanWrite && !AttributeManager.HasAttribute<ExportAttribute> (setter)) {
					if (!AttributeManager.HasAttribute<NotImplementedAttribute> (setter))
						continue;
				}
			}

			if (@static.HasValue && @static.Value != hasStaticAttribute)
				continue;

			if (required.HasValue && required.Value != IsRequired (p, attrs))
				continue;
			
			yield return p;
		}
	}

	// If a type comes from the assembly with the api definition we're processing.
	bool IsApiType (Type type)
	{
		return type.Assembly == types [0].Assembly;
	}

	bool IsRequired (MemberInfo provider, Attribute [] attributes = null)
	{
		var type = provider.DeclaringType;
		if (IsApiType (type)){
			return HasAttribute<AbstractAttribute> (provider, attributes);
		}
		if (type.IsInterface)
			return true;

		return false;
	}

	void GenerateProtocolTypes (Type type, string class_visibility, string TypeName, string protocol_name, ProtocolAttribute protocolAttribute)
	{
		var allProtocolMethods = new List<MethodInfo> ();
		var allProtocolProperties = new List<PropertyInfo> ();
		var ifaces = (IEnumerable<Type>) type.GetInterfaces ().Concat (new Type [] { ReflectionExtensions.GetBaseType (type) }).OrderBy (v => v.FullName, StringComparer.Ordinal);
		
		if (type.Namespace != null) {
			print ("namespace {0} {{", type.Namespace);
			indent++;
		}
		
		ifaces = ifaces.Where ((v) => IsProtocolInterface (v, false));

		allProtocolMethods.AddRange (SelectProtocolMethods (type));
		allProtocolProperties.AddRange (SelectProtocolProperties (type));

		var requiredInstanceMethods = allProtocolMethods.Where ((v) => IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v)).ToList ();
		var optionalInstanceMethods = allProtocolMethods.Where ((v) => !IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v));
		var requiredInstanceProperties = allProtocolProperties.Where ((v) => IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v)).ToList ();
		var optionalInstanceProperties = allProtocolProperties.Where ((v) => !IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v));
		var requiredInstanceAsyncMethods = requiredInstanceMethods.Where (m => AttributeManager.HasAttribute<AsyncAttribute> (m)).ToList ();

		PrintAttributes (type, platform:true, preserve:true, advice:true);
		print ("[Protocol (Name = \"{1}\", WrapperType = typeof ({0}Wrapper){2}{3})]", 
		       TypeName, 
		       protocol_name, 
		       protocolAttribute.IsInformal ? ", IsInformal = true" : string.Empty, 
		       protocolAttribute.FormalSince != null ? $", FormalSince = \"{protocolAttribute.FormalSince}\"" : string.Empty);

		var sb = new StringBuilder ();

		foreach (var mi in allProtocolMethods) {
			var attrib = GetExportAttribute (mi);
			sb.Clear ();
			sb.Append ("[ProtocolMember (");
			sb.Append ("IsRequired = ").Append (IsRequired (mi) ? "true" : "false");
			sb.Append (", IsProperty = false");
			sb.Append (", IsStatic = ").Append (AttributeManager.HasAttribute<StaticAttribute> (mi) ? "true" : "false");
			sb.Append (", Name = \"").Append (mi.Name).Append ("\"");
			sb.Append (", Selector = \"").Append (attrib.Selector).Append ("\"");
			if (mi.ReturnType != TypeManager.System_Void) {
				var retType = GetCorrectGenericType (mi.ReturnType);
				sb.Append (", ReturnType = typeof (").Append (RenderType (retType)).Append (")");
				if (retType.IsSubclassOf (TypeManager.System_Delegate)) {
					var ti = MakeTrampoline (retType);
					sb.Append ($", ReturnTypeDelegateProxy = typeof ({ns.CoreObjCRuntime}.Trampolines.{ti.StaticName})");
				}
			}
			var parameters = mi.GetParameters ();
			if (parameters != null && parameters.Length > 0) {
				sb.Append (", ParameterType = new Type [] { ");
				for (int i = 0; i < parameters.Length; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append ("typeof (");
					var pt = parameters [i].ParameterType;
					if (pt.IsByRef)
						pt = pt.GetElementType ();
					sb.Append (RenderType (GetCorrectGenericType (pt)));
					sb.Append (")");
				}
				sb.Append (" }");
				sb.Append (", ParameterByRef = new bool [] { ");
				for (int i = 0; i < parameters.Length; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append (parameters [i].ParameterType.IsByRef ? "true" : "false");
				}
				sb.Append (" }");

				var blockProxies = new string [parameters.Length];
				var anyblockProxy = false;
				for (int i = 0; i < parameters.Length; i++) {
					var parType = GetCorrectGenericType (parameters [i].ParameterType);
					if (parType.IsSubclassOf (TypeManager.System_Delegate)) {
						var ti = MakeTrampoline (parType);
						blockProxies [i] = $"typeof ({ns.CoreObjCRuntime}.Trampolines.{ti.NativeInvokerName})";
						anyblockProxy = true;
					}
				}
				if (anyblockProxy) {
					sb.Append (", ParameterBlockProxy = new Type [] { ");
					for (int i = 0; i < blockProxies.Length; i++) {
						if (i > 0)
							sb.Append (", ");
						sb.Append (blockProxies [i] == null ? "null" : blockProxies [i]);
					}
					sb.Append (" }");
				}

				if (attrib.IsVariadic)
					sb.Append (", IsVariadic = true");
			}
			sb.Append (")]");
			print (sb.ToString ());
		}
		foreach (var pi in allProtocolProperties) {
			var attrib = GetExportAttribute (pi);
			sb.Clear ();
			sb.Append ("[ProtocolMember (");
			sb.Append ("IsRequired = ").Append (IsRequired (pi) ? "true" : "false");
			sb.Append (", IsProperty = true");
			sb.Append (", IsStatic = ").Append (AttributeManager.HasAttribute<StaticAttribute> (pi) ? "true" : "false");
			sb.Append (", Name = \"").Append (pi.Name).Append ("\"");
			sb.Append (", Selector = \"").Append (attrib.Selector).Append ("\"");
			sb.Append (", PropertyType = typeof (").Append (RenderType (pi.PropertyType)).Append(")");
			if (pi.CanRead && !AttributeManager.HasAttribute<NotImplementedAttribute> (pi.GetGetMethod ())) {
				var ea = GetGetterExportAttribute (pi);
				var ba = GetBindAttribute (pi.GetGetMethod ());
				sb.Append (", GetterSelector = \"").Append (ba != null ? ba.Selector : ea.Selector).Append ("\"");
			}
			if (pi.CanWrite && !AttributeManager.HasAttribute<NotImplementedAttribute> (pi.GetSetMethod ())) {
				var ea = GetSetterExportAttribute (pi);
				var ba = GetBindAttribute (pi.GetSetMethod ());
				sb.Append (", SetterSelector = \"").Append (ba != null ? ba.Selector : ea.Selector).Append ("\"");
			}
			sb.Append (", ArgumentSemantic = ArgumentSemantic.").Append (attrib.ArgumentSemantic);
			// Check for block/delegate proxies
			var propType = GetCorrectGenericType (pi.PropertyType);
			if (propType.IsSubclassOf (TypeManager.System_Delegate)) {
				var ti = MakeTrampoline (propType);
				if (pi.SetMethod != null)
					sb.Append ($", ParameterBlockProxy = new Type [] {{ typeof ({ns.CoreObjCRuntime}.Trampolines.{ti.NativeInvokerName}) }}");
				if (pi.GetMethod != null)
					sb.Append ($", ReturnTypeDelegateProxy = typeof ({ns.CoreObjCRuntime}.Trampolines.{ti.StaticName})");
			}
			sb.Append (")]");
			print (sb.ToString ());
		}

		print ("{0} interface I{1} : INativeObject, IDisposable{2}", class_visibility, TypeName, ifaces.Count () > 0 ? ", " : string.Empty);
		indent++;
		sb.Clear ();
		foreach (var iface in ifaces) {
			string iname = iface.Name;
			// if the [Protocol] interface is defined in the binding itself it won't start with an 'I'
			// but if it's a reference to something inside an already built assembly, 
			// e.g. monotouch.dll, then the 'I' will be present
			if (sb.Length > 0)
				sb.Append (", ");
			sb.Append (iface.Namespace).Append ('.');
			if (iface.Assembly.GetName ().Name == "temp")
				sb.Append ('I');
			else if (iface.IsClass)
				sb.Append ('I');
			sb.AppendLine (iname);
		}
		if (sb.Length > 0)
			print (sb.ToString ());
		indent--;

		print ("{");
		indent++;
		foreach (var mi in requiredInstanceMethods) {
			if (AttributeManager.HasAttribute<StaticAttribute> (mi))
				continue;

			var minfo = new MemberInformation (this, mi, type, null);
			var mod = string.Empty;

			PrintMethodAttributes (minfo);
			PrintPlatformAttributes (mi);
			print_generated_code ();
			PrintDelegateProxy (minfo);
			PrintExport (minfo);
			print ("[Preserve (Conditional = true)]");
			if (minfo.is_unsafe)
				mod = "unsafe ";
			print ("{0}{1};", mod, MakeSignature (minfo, true));
			print ("");
		}

		foreach (var pi in requiredInstanceProperties) {
			var minfo = new MemberInformation (this, pi, type);
			var mod = string.Empty;
			minfo.is_export = true;

			print ("[Preserve (Conditional = true)]");

			if (minfo.is_unsafe)
				mod = "unsafe ";

			print ("{0}{1} {2} {{", mod, FormatType (type, pi.PropertyType), pi.Name, pi.CanRead ? "get;" : string.Empty, pi.CanWrite ? "set;" : string.Empty);
			indent++;
			if (pi.CanRead) {
				var ea = GetGetterExportAttribute (pi);
				var getMethod = pi.GetGetMethod ();
				// there can be a [Bind] there that override the selector name to be used
				// e.g. IMTLTexture.FramebufferOnly
				var ba = GetBindAttribute (getMethod);
				PrintDelegateProxy (getMethod);
				if (!AttributeManager.HasAttribute<NotImplementedAttribute> (getMethod)) {
					if (ba != null)
						PrintExport (minfo, ba.Selector, ea.ArgumentSemantic);
					else
						PrintExport (minfo, ea);
				}
				PrintAttributes (getMethod, notImplemented: true);
				print ("get;");
			}
			if (pi.CanWrite) {
				var setMethod = pi.GetSetMethod ();
				PrintBlockProxy (pi.PropertyType);
				PrintAttributes (setMethod, notImplemented:true);
				if (!AttributeManager.HasAttribute<NotImplementedAttribute> (setMethod))
					PrintExport (minfo, GetSetterExportAttribute (pi));
				print ("set;");
			}
			indent--;
			print ("}");
			print ("");
		}
		indent--;
		print ("}");
		print ("");

		// avoid (for unified) all the metadata for empty static classes, we can introduce them later when required
		bool include_extensions = false;
		if (UnifiedAPI) {
			include_extensions = optionalInstanceMethods.Any () || optionalInstanceProperties.Any () || requiredInstanceAsyncMethods.Any ();
		} else {
			include_extensions = true;
		}
		if (include_extensions) {
			// extension methods
			PrintAttributes (type, preserve:true, advice:true);
			print ("{1} static partial class {0}_Extensions {{", TypeName, class_visibility);
			indent++;
			foreach (var mi in optionalInstanceMethods)
				GenerateMethod (type, mi, false, null, false, false, true);

			// Generate Extension Methods of required [Async] decorated methods (we already do optional) 
			foreach (var ami in requiredInstanceAsyncMethods) {
				var minfo = new MemberInformation (this, ami, type, null, is_extension_method: true);
				GenerateAsyncMethod (minfo, AsyncMethodKind.Plain);

				// Generate the overload with the out parameter
				if (minfo.method.ReturnType != TypeManager.System_Void)
					GenerateAsyncMethod (minfo, AsyncMethodKind.WithResultOutParameter);
			}

			// C# does not support extension properties, so create Get* and Set* accessors instead.
			foreach (var pi in optionalInstanceProperties) {
				var attrib = GetExportAttribute (pi);
				var getter = pi.GetGetMethod ();
				if (getter != null) {
					PrintAttributes (pi, preserve:true, advice:true);
					GenerateMethod (type, getter, false, null, false, false, true, attrib.ToGetter(pi).Selector);
				}
				var setter = pi.GetSetMethod ();
				if (setter != null) {
					PrintAttributes (pi, preserve:true, advice:true);
					GenerateMethod (type, setter, false, null, false, false, true, attrib.ToSetter(pi).Selector);
				}
			}
			indent--;
			print ("}");
			print ("");
		}

		// Add API from base interfaces we also need to implement.
		foreach (var iface in ifaces) {
			requiredInstanceMethods.AddRange (SelectProtocolMethods (iface, @static: false, required: true));
			requiredInstanceProperties.AddRange (SelectProtocolProperties (iface, @static: false, required: true));
		}

		PrintPreserveAttribute (type);
		print ("internal sealed class {0}Wrapper : BaseWrapper, I{0} {{", TypeName);
		indent++;
		// ctor (IntPtr, bool)
		print ("[Preserve (Conditional = true)]");
		print ("public {0}Wrapper (IntPtr handle, bool owns)", TypeName);
		print ("\t: base (handle, owns)");
		print ("{");
		print ("}");
		print ("");
		// Methods
		foreach (var mi in requiredInstanceMethods) {
			GenerateMethod (type, mi, false, null, false, true, isBaseWrapperProtocolMethod:true);
		}
		foreach (var pi in requiredInstanceProperties) {
			GenerateProperty (type, pi, null, false, true);
		}
		indent--;
		print ("}");

		if (type.Namespace != null) {
			indent--;
			print ("}");
		}
	}

	bool ConformToNSCoding (Type type)
	{
		foreach (var intf in type.GetInterfaces ()) {
			if (intf.Name == "NSCoding" || intf.Name == "INSCoding")
				return true;
		}
		// [BaseType (x)] might implement NSCoding... and this means we need the .ctor(NSCoder)
		var attrs = AttributeManager.GetCustomAttributes<BaseTypeAttribute> (type);
		if (attrs == null || attrs.Length == 0)
			return false;
		return ConformToNSCoding (attrs [0].BaseType);
	}

	StreamWriter GetOutputStream (string @namespace, string name)
	{
		var dir = basedir;

		if (!string.IsNullOrEmpty (@namespace))
			dir = Path.Combine (dir, @namespace.Replace (ns.Prefix + ".", string.Empty));

		var filename = Path.Combine (dir, name + ".g.cs");
		var counter = 2;
		while (generated_files.Contains (filename)) {
			filename = Path.Combine (dir, name + counter.ToString () + ".g.cs");
			counter++;
		}
		generated_files.Add (filename);
		if (!Directory.Exists (dir))
			Directory.CreateDirectory (dir);

		return new StreamWriter (filename);
	}


	StreamWriter GetOutputStreamForType (Type type)
	{
		if (type.Namespace == null)
			ErrorHelper.Show (new BindingException (1103, false, "'{0}' does not live under a namespace; namespaces are a highly recommended .NET best practice", type.FullName));

		var tn = GetGeneratedTypeName (type);
		if (type.IsGenericType)
			tn = tn + "_" + type.GetGenericArguments ().Length.ToString ();
		return GetOutputStream (type.Namespace, tn);
	}

	void WriteMarkDirtyIfDerived (StreamWriter sw, Type type)
	{
		if (type.Name == "CALayer" && ns.Get ("CoreAnimation") == type.Namespace) {
			sw.WriteLine ("\t\t\tMarkDirtyIfDerived ();");
		}
	}

	// Function to check if PreserveAttribute is present and
	// generate/print the same attribute as in bindings
	public void PrintPreserveAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<PreserveAttribute> (mi);
		if (p == null)
			return;
		
		if (p.AllMembers)
			print ("[Preserve (AllMembers = true)]");
		else if (p.Conditional)
			print ("[Preserve (Conditional = true)]");
		else
			print ("[Preserve]");
	}

	// Function to check if PrintAdviceAttribute is present and
	// generate/print the same attribute as in bindings
	public void PrintAdviceAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<AdviceAttribute> (mi);
		if (p == null)
			return;

		print ($"[Advice ({Quote (p.Message)})]");
	}

	public void PrintRequiresSuperAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<RequiresSuperAttribute> (mi);
		if (p == null)
			return;

		print ("[RequiresSuper]");
	}

	public void PrintNotImplementedAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<NotImplementedAttribute> (mi);
		if (p == null)
			return;

		print ($"[NotImplemented ({Quote (p.Message)})]");
	}

	public void PrintBindAsAttribute (ICustomAttributeProvider mi, StringBuilder sb = null)
	{
		var p = GetBindAsAttribute (mi);
		if (p == null)
			return;

		var property = mi as PropertyInfo;
		var method = mi as MethodInfo;
		var param = mi as ParameterInfo;
		var originalType = method?.ReturnType ?? property?.PropertyType;
		originalType = originalType ?? param?.ParameterType;

		var declaringType = (mi as MemberInfo)?.DeclaringType ?? param.Member.DeclaringType;
		var pReturn = method != null ? "return: " : string.Empty;

		var attribstr = $"[{pReturn}BindAs (typeof ({FormatType (declaringType, p.Type)}), OriginalType = typeof ({FormatType (declaringType, originalType)}))]";

		if (sb != null)
			sb.Append ($"{attribstr} ");
		else
			print (attribstr);
	}

	public void PrintAttributes (ICustomAttributeProvider mi, bool platform = false, bool preserve = false, bool advice = false, bool notImplemented = false, bool bindAs = false, bool requiresSuper = false)
	{
		if (platform)
			PrintPlatformAttributes (mi as MemberInfo);
		if (preserve)
			PrintPreserveAttribute (mi);
		if (advice)
			PrintAdviceAttribute (mi);
		if (notImplemented)
			PrintNotImplementedAttribute (mi);
		if (bindAs)
			PrintBindAsAttribute (mi);
		if (requiresSuper)
			PrintRequiresSuperAttribute (mi);
	}

	public void ComputeLibraryName (FieldAttribute fieldAttr, Type type, string propertyName, out string library_name, out string library_path)
	{
		library_path = null;

		if (fieldAttr != null && fieldAttr.LibraryName != null) {
			// Remapped
			library_name = fieldAttr.LibraryName;
			if (library_name [0] == '+') {
				switch (library_name) {
				case "+CoreImage":
					library_name = CoreImageMap;
					break;
				case "+CoreServices":
					library_name = CoreServicesMap;
					break;
				case "+PDFKit":
					library_name = "PdfKit";
					library_path = PDFKitMap;
					break;
				}
			} else {
				// we get something in LibraryName from FieldAttribute so we asume
				// it is a path to a library, so we save the path and change library name
				// to a valid identifier if needed
				library_path = library_name;
				library_name = Path.GetFileName (library_name);
				if (library_name.Contains ("."))
					library_name = library_name.Replace (".", string.Empty);
			}
		} else if (BindThirdPartyLibrary) {
			// User should provide a LibraryName
			throw new BindingException (1042, true, $"Missing '[Field (LibraryName=value)]' for {propertyName} (e.g.\"__Internal\")");
		} else {
			library_name = type.Namespace;

			// note: not every binding namespace will start with ns.Prefix (e.g. MonoTouch.)
			if (!String.IsNullOrEmpty (ns.Prefix) && library_name.StartsWith (ns.Prefix, StringComparison.Ordinal)) {
				library_name = library_name.Substring (ns.Prefix.Length + 1);
				library_name = library_name.Replace (".", string.Empty); // Remove dots from namespaces
			}
		}

		if (!libraries.ContainsKey (library_name))
			libraries.Add (library_name, library_path);
	}

	public static string GetSelector (MemberInfo mi)
	{
		object [] attr = AttributeManager.GetCustomAttributes<ExportAttribute> (mi);
		if (attr.Length != 1){
			attr = AttributeManager.GetCustomAttributes<BindAttribute> (mi);
			if (attr.Length != 1) {
				return null;
			}
			else {
				BindAttribute ba = (BindAttribute) attr [0];
				return ba.Selector;
			 }
		} else {
			ExportAttribute ea = (ExportAttribute) attr [0];
			return ea.Selector;
		}
	}

	static string GetAssemblyName ()
	{
		if (BindThirdPartyLibrary)
			return Path.GetFileNameWithoutExtension (BindingTouch.outfile);
		return BindingTouch.TargetFramework.Identifier;
	}

	public void Generate (Type type)
	{
		if (!Compat && ZeroCopyStrings) {
			ErrorHelper.Show (new BindingException (1027, "Support for ZeroCopy strings is not implemented. Strings will be marshalled as NSStrings."));
			ZeroCopyStrings = false;
		}

		type_wants_zero_copy = AttributeManager.HasAttribute<ZeroCopyStringsAttribute> (type) || ZeroCopyStrings;
		var tsa = AttributeManager.GetCustomAttribute<ThreadSafeAttribute> (type);
		// if we're inside a special namespace then default is non-thread safe, otherwise default is thread safe
		if (ns.UINamespaces.Contains (type.Namespace)) {
			// Any type inside these namespaces requires, by default, a thread check
			// unless it has a [ThreadSafe] or [ThreadSafe (true)] attribute
			type_needs_thread_checks = tsa == null || !tsa.Safe;
		} else {
			// Any type outside these namespaces do NOT require a thread check
			// unless it has a [ThreadSafe (false)] attribute
			type_needs_thread_checks = tsa != null && !tsa.Safe;
		}

		string TypeName = GetGeneratedTypeName (type);
		indent = 0;
		var instance_fields_to_clear_on_dispose = new List<string> ();
		var gtype = GeneratedType.Lookup (type);
		var appearance_selectors = gtype.ImplementsAppearance ? gtype.AppearanceSelectors : null;

		using (var sw = GetOutputStreamForType (type)) {
			this.sw = sw;
			bool is_category_class = AttributeManager.HasAttribute<CategoryAttribute> (type);
			bool is_static_class = AttributeManager.HasAttribute<StaticAttribute> (type) || is_category_class;
			bool is_partial = AttributeManager.HasAttribute<PartialAttribute> (type);
			var model = AttributeManager.GetCustomAttribute<ModelAttribute> (type);
			bool is_model = model != null;
			var protocol = AttributeManager.GetCustomAttribute<ProtocolAttribute> (type);
			bool is_protocol = protocol != null;
			bool is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (type);
			bool is_sealed = AttributeManager.HasAttribute<SealedAttribute> (type);
			string class_visibility = type.IsInternal () ? "internal" : "public";

			var default_ctor_visibility = AttributeManager.GetCustomAttribute<DefaultCtorVisibilityAttribute> (type);
			BaseTypeAttribute bta = ReflectionExtensions.GetBaseTypeAttribute(type);
			Type base_type = bta != null ?  bta.BaseType : TypeManager.System_Object;
			string objc_type_name = bta != null ? (bta.Name != null ? bta.Name : TypeName) : TypeName;
			string register_name = objc_type_name;

			if (is_model) {
				if (!string.IsNullOrEmpty (model.Name)) {
					register_name = model.Name;
				} else if (model.AutoGeneratedName) {
					register_name = StringUtils.SanitizeObjectiveCName (GetAssemblyName () + "__" + type.FullName);
				}
			}
			Header (sw);
			
			if (is_protocol) {
				if (is_static_class)
					throw new BindingException (1025, true, "[Static] and [Protocol] are mutually exclusive ({0})", type.FullName);
				if (is_model && base_type == TypeManager.System_Object)
					ErrorHelper.Show (new BindingException (1060, "The {0} protocol is decorated with [Model], but not [BaseType]. Please verify that [Model] is relevant for this protocol; if so, add [BaseType] as well, otherwise remove [Model].", type.FullName));

				GenerateProtocolTypes (type, class_visibility, TypeName, protocol.Name ?? objc_type_name, protocol);
			}

			if (!is_static_class && bta == null && is_protocol)
				return;

			if (type.Namespace != null) {
				print ("namespace {0} {{", type.Namespace);
				indent++;
			}

			bool core_image_filter = false;
			string class_mod = null;

			is_direct_binding_value = null;
			is_direct_binding = null;
			if (BindThirdPartyLibrary)
				is_direct_binding_value = string.Format ("GetType ().Assembly == global::{0}.this_assembly", ns.Messaging);
			if (is_static_class || is_category_class || is_partial) {
				base_type = TypeManager.System_Object;
				if (!is_partial)
					class_mod = "static ";
			} else {
				if (is_protocol) {
					var pName = !string.IsNullOrEmpty (protocol.Name) ? $"Name = \"{protocol.Name}\"" : string.Empty;
					print ("[Protocol({0}{1}{2})]", pName, (!string.IsNullOrEmpty (pName) && protocol.IsInformal) ? ", " : string.Empty, protocol.IsInformal ? "IsInformal = true" : string.Empty);
				}
				core_image_filter = AttributeManager.HasAttribute<CoreImageFilterAttribute> (type);
				if (!type.IsEnum && !core_image_filter) {
					if (is_model || AttributeManager.HasAttribute<SyntheticAttribute> (type)) {
						is_direct_binding = false;
						is_direct_binding_value = "false";
					}
					print ("[Register(\"{0}\", {1})]", register_name, is_direct_binding == false ? "false" : "true");
				}
				if (is_abstract || need_abstract.ContainsKey (type))
					class_mod = "abstract ";
				else if (is_sealed)
					class_mod = "sealed ";
			} 
			
			if (is_sealed && is_direct_binding == null) {
				is_direct_binding = true;
				is_direct_binding_value = "true";
			}
				
			if (is_model){
				if (is_category_class)
					ErrorHelper.Show (new BindingException (1022, true, "Category classes can not use the [Model] attribute"));
				print ("[Model]");
			}

			PrintAttributes (type, platform:true, preserve:true, advice:true);

			if (type.IsEnum) {
				GenerateEnum (type);
				return;
			}

			if (core_image_filter) {
				GenerateFilter (type);
				return;
			}

			// Order of this list is such that the base type will be first,
			// followed by the protocol interface, followed by any other
			// interfaces in ascending order
			var implements_list = new List<string> ();

			foreach (var protocolType in type.GetInterfaces ()) {
				if (!AttributeManager.HasAttribute<ProtocolAttribute> (protocolType)) {
					string nonInterfaceName = protocolType.Name.Substring (1);
					if (protocolType.Name[0] == 'I' && types.Any (x => x.Name.Contains(nonInterfaceName)))
						if (protocolType.Name.Contains ("MKUserLocation"))	// We can not fix MKUserLocation without breaking API, and we don't want warning forever in build until then...
							ErrorHelper.Show (new BindingException (1111, false, "Interface '{0}' on '{1}' is being ignored as it is not a protocol. Did you mean '{2}' instead?", protocolType, type, nonInterfaceName));
					continue;
				}

				// A protocol this class implements. We need to implement the corresponding interface for the protocol.
				string pname = protocolType.Name;
				// the extra 'I' is only required for the bindings being built, if it comes from something already
				// built (e.g. monotouch.dll) then the interface will alreadybe prefixed
				if (protocolType.Assembly.GetName ().Name == "temp")
					pname = "I" + pname;
				var iface = FormatType (type, protocolType.Namespace, pname);
				if (!implements_list.Contains (iface))
					implements_list.Add (iface);
			}

			implements_list.Sort (StringComparer.Ordinal);

			if (is_protocol)
				implements_list.Insert (0, "I" + type.Name);

			if (base_type != TypeManager.System_Object && TypeName != "NSObject" && !is_category_class)
				implements_list.Insert (0, FormatType (type, base_type));

			if (type.IsNested){
				var nestedName = type.FullName.Substring (type.Namespace.Length+1);
				var container = nestedName.Substring (0, nestedName.IndexOf ('+'));

				print ("partial class {0} {{", container);
				indent++;
			}

			var class_name = TypeName;
			var where_list = string.Empty;
			if (type.IsGenericType) {
				class_name += "<";
				var gargs = type.GetGenericArguments ();
				for (int i = 0; i < gargs.Length; i++) {
					if (i > 0)
						class_name += ", ";
					class_name += FormatType (type, gargs [i]);

					where_list += "\n\t\twhere " + gargs [i].Name + " : ";
					var constraints = gargs [i].GetGenericParameterConstraints ();
					if (constraints.Length > 0) {
						var comma = string.Empty;
						if (IsProtocol (constraints [0])) {
							where_list += "NSObject";
							comma = ", ";
						}
						
						for (int c = 0; c < constraints.Length; c++) {
							where_list += comma + FormatType (type, constraints [c]);
							comma = ", ";
						}
					} else {
						where_list += "NSObject";
					}
				}
				class_name += ">";
				if (where_list.Length > 0)
					where_list += " ";
			}

			print ("{0} unsafe {1}partial class {2} {3} {4}{{",
			       class_visibility,
			       class_mod,
			       class_name,
			       implements_list.Count == 0 ? string.Empty : ": " + string.Join (", ", implements_list),
			       where_list);

			indent++;
			
			if (!is_model && !is_partial) {
				foreach (var ea in selectors [type].OrderBy (s => s, StringComparer.Ordinal)) {
					var selectorField = SelectorField (ea, true);
					if (!InlineSelectors) {
						selectorField = selectorField.Substring (0, selectorField.Length - 6 /* Handle */);
						print_generated_code ();
						print ("const string {0} = \"{1}\";", selectorField, ea);
						print ("static readonly IntPtr {0} = Selector.GetHandle (\"{1}\");", SelectorField (ea), ea);
					}
				}
			}
			print ("");

			// Regular bindings (those that are not-static) or categories need this
			if (!(is_static_class || is_partial) || is_category_class){
				if (is_category_class)
					objc_type_name = FormatCategoryClassName (bta);
				
				if (!is_model) {
					print_generated_code ();
					print ("static readonly IntPtr class_ptr = Class.GetHandle (\"{0}\");\n", objc_type_name);
				}
			}
			
			if (!is_static_class && !is_partial){
				if (!is_model && !external) {
					print ("public {1} IntPtr ClassHandle {{ get {{ return class_ptr; }} }}\n", objc_type_name, TypeName == "NSObject" ? "virtual" : "override");
				}

				var ctor_visibility = "public";
				var disable_default_ctor = false;
				if (default_ctor_visibility != null) {
					switch (default_ctor_visibility.Visibility) {
					case Visibility.Public:
						break; // default
					case Visibility.Internal: 
						ctor_visibility = "internal";
						break;
					case Visibility.Protected:
						ctor_visibility = "protected";
						break;
					case Visibility.ProtectedInternal:
						ctor_visibility = "protected internal";
						break;
					case Visibility.Private:
						ctor_visibility = string.Empty;
						break;
					case Visibility.Disabled:
						disable_default_ctor = true;
						break;
					}
				}
				
				if (TypeName != "NSObject"){
					var initSelector = (InlineSelectors || BindThirdPartyLibrary) ? "Selector.GetHandle (\"init\")" : "Selector.Init";
					var initWithCoderSelector = (InlineSelectors || BindThirdPartyLibrary) ? "Selector.GetHandle (\"initWithCoder:\")" : "Selector.InitWithCoder";
					string v = UnifiedAPI && class_mod == "abstract " ? "protected" : ctor_visibility;
					if (external) {
						if (!disable_default_ctor) {
							GeneratedCode (sw, 2);
							foreach (var ta in AttributeManager.GetCustomAttributes<DesignatedDefaultCtorAttribute> (type)) {
								sw.WriteLine ("\n\n[DesignatedInitializer]");
								break;
							}
							sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
							sw.WriteLine ("\t\t[Export (\"init\")]");
							sw.WriteLine ("\t\t{0} {1} () : base (NSObjectFlag.Empty)", v, TypeName);
							sw.WriteLine ("\t\t{");
							if (is_direct_binding_value != null)
								sw.WriteLine ("\t\t\tIsDirectBinding = {0};", is_direct_binding_value);
							if (debug)
								sw.WriteLine ("\t\t\tConsole.WriteLine (\"{0}.ctor ()\");", TypeName);
							sw.WriteLine ("\t\t\tInitializeHandle (global::{1}.IntPtr_objc_msgSend (this.Handle, global::{2}.{0}), \"init\");", initSelector, ns.Messaging, ns.CoreObjCRuntime);
							sw.WriteLine ("\t\t\t");
							sw.WriteLine ("\t\t}");
						}
					} else {
						if (!disable_default_ctor) {
							GeneratedCode (sw, 2);
							foreach (var ta in AttributeManager.GetCustomAttributes<DesignatedDefaultCtorAttribute> (type)) {
								sw.WriteLine ("\t\t[DesignatedInitializer]");
								break;
							}
							sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
							sw.WriteLine ("\t\t[Export (\"init\")]");
							sw.WriteLine ("\t\t{0} {1} () : base (NSObjectFlag.Empty)", v, TypeName);
							sw.WriteLine ("\t\t{");
							var indentation = 3;
							WriteIsDirectBindingCondition (sw, ref indentation, is_direct_binding, is_direct_binding_value,
							                               () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSend (this.Handle, global::{2}.{0}), \"init\");", initSelector, ns.Messaging, ns.CoreObjCRuntime),
							                               () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSendSuper (this.SuperHandle, global::{2}.{0}), \"init\");", initSelector, ns.Messaging, ns.CoreObjCRuntime));

							WriteMarkDirtyIfDerived (sw, type);
							sw.WriteLine ("\t\t}");
							sw.WriteLine ();
						}
						// old monotouch.dll (and MonoMac.dll, XamMac.dll) always included this .ctor even if the
						// type did not conform to NSCopying. That made the .ctor throw a (native) exception and crash
						var nscoding = ConformToNSCoding (type);
						if (Compat || nscoding) {
							// for compatibility we continue to include the .ctor(NSCoder) in the compat assemblies
							// but we make it throw an InvalidOperationException if the type does not implement NSCoding
							// because it's easier to catch (and won't crash on devices)
							GeneratedCode (sw, 2);
							sw.WriteLine ("\t\t[DesignatedInitializer]");
							sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
							sw.WriteLine ("\t\t[Export (\"initWithCoder:\")]");
							sw.WriteLine ("\t\t{0} {1} (NSCoder coder) : base (NSObjectFlag.Empty)", UnifiedAPI ? v : "public", TypeName);
							sw.WriteLine ("\t\t{");
							if (nscoding) {
								if (debug)
									sw.WriteLine ("\t\t\tConsole.WriteLine (\"{0}.ctor (NSCoder)\");", TypeName);
								sw.WriteLine ();
								var indentation = 3;
								WriteIsDirectBindingCondition (sw, ref indentation, is_direct_binding, is_direct_binding_value,
								                               () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSend_IntPtr (this.Handle, {0}, coder.Handle), \"initWithCoder:\");", initWithCoderSelector, ns.Messaging),
								                               () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, {0}, coder.Handle), \"initWithCoder:\");", initWithCoderSelector, ns.Messaging));
								WriteMarkDirtyIfDerived (sw, type);
							} else {
								sw.WriteLine ("\t\t\tthrow new InvalidOperationException (\"Type does not conform to NSCoding\");");
							}
							sw.WriteLine ("\t\t}");
							sw.WriteLine ();
						}
					}
					if (!is_sealed) {
						GeneratedCode (sw, 2);
						sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
						sw.WriteLine ("\t\t{0} {1} (NSObjectFlag t) : base (t)", UnifiedAPI ? "protected" : "public", TypeName);
						sw.WriteLine ("\t\t{");
						if (is_direct_binding_value != null)
							sw.WriteLine ("\t\t\tIsDirectBinding = {0};", is_direct_binding_value);
						WriteMarkDirtyIfDerived (sw, type);
						sw.WriteLine ("\t\t}");
						sw.WriteLine ();
					}
					GeneratedCode (sw, 2);
					sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
					sw.WriteLine ("\t\t{0} {1} (IntPtr handle) : base (handle)", UnifiedAPI ? "protected internal" : "public", TypeName);
					sw.WriteLine ("\t\t{");
					if (is_direct_binding_value != null)
						sw.WriteLine ("\t\t\tIsDirectBinding = {0};", is_direct_binding_value);
					WriteMarkDirtyIfDerived (sw, type);
					sw.WriteLine ("\t\t}");
					sw.WriteLine ();
				}
			}
			
			var bound_methods = new HashSet<MemberInformation> (); // List of methods bound on the class itself (not via protocols)
			var generated_methods = new List<MemberInformation> (); // All method that have been generated
			foreach (var mi in GetTypeContractMethods (type).OrderByDescending (m => m.Name == "Constructor").ThenBy (m => m.Name, StringComparer.Ordinal)) {
				if (mi.IsSpecialName || (mi.Name == "Constructor" && type != mi.DeclaringType))
					continue;

				if (mi.IsUnavailable ())
					continue;

				if (appearance_selectors != null && AttributeManager.HasAttribute<AppearanceAttribute> (mi))
					appearance_selectors.Add (mi);
				
				var minfo = new MemberInformation (this, mi, type, is_category_class ? bta.BaseType : null, is_model: is_model);

				if (type == mi.DeclaringType || type.IsSubclassOf (mi.DeclaringType)) {
					// not an injected protocol method.
					bound_methods.Add (minfo);
				} else {
					// don't inject a protocol method if the class already
					// implements the same method.
					if (bound_methods.Contains (minfo))
						continue;

					var protocolsThatHaveThisMethod = GetTypeContractMethods (type).Where (x => { var sel = GetSelector (x); return sel != null && sel == minfo.selector; } );
					if (protocolsThatHaveThisMethod.Count () > 1) {
						// If multiple protocols have this method and we haven't generated a copy yet
						if (generated_methods.Any (x => x.selector == minfo.selector))
							continue;

						// Verify all of the versions have the same arguments / return value
						// And just generate the first one (us)
						var methodParams = mi.GetParameters ();
						foreach (var duplicateMethod in protocolsThatHaveThisMethod) {
							if (mi.ReturnType != duplicateMethod.ReturnType)
								throw new BindingException (1038, true, "The selector {0} on type {1} is found multiple times with different return types.", mi.Name, type.Name);

							if (methodParams.Length != duplicateMethod.GetParameters().Length)
								throw new BindingException (1039, true, "The selector {0} on type {1} is found multiple times with different argument length {2} : {3}.", minfo.selector, type.Name, mi.GetParameters ().Length, duplicateMethod.GetParameters ().Length);
						}

						int i = 0;
						foreach (var param in methodParams) {
							foreach (var duplicateMethod in protocolsThatHaveThisMethod) {
								var duplicateParam = duplicateMethod.GetParameters ()[i];
								if (param.IsOut != duplicateParam.IsOut)
									throw new BindingException (1040, true, "The selector {0} on type {1} is found multiple times with different argument out states on argument {2}.", minfo.selector, type.Name, i);
								if (param.ParameterType != duplicateParam.ParameterType)
									throw new BindingException (1041, true, "The selector {0} on type {1} is found multiple times with different argument types on argument {2} - {3} : {4}.", minfo.selector, type.Name, i, param.ParameterType, duplicateParam.ParameterType);
							}
							i++;
						}
					}
				}

				generated_methods.Add (minfo);
				GenerateMethod (minfo);
			}

			var field_exports = new List<PropertyInfo> ();
			var notifications = new List<PropertyInfo> ();
			var bound_properties = new List<string> (); // List of properties bound on the class itself (not via protocols)
			var generated_properties = new List<string> (); // All properties that have been generated

			foreach (var pi in GetTypeContractProperties (type).OrderBy (p => p.Name, StringComparer.Ordinal)) {

				if (pi.IsUnavailable ())
					continue;

				if (AttributeManager.HasAttribute<FieldAttribute> (pi)) {
					field_exports.Add (pi);

					if (AttributeManager.HasAttribute<NotificationAttribute> (pi))
						notifications.Add (pi);
					continue;
				}

				if (appearance_selectors != null && AttributeManager.HasAttribute<AppearanceAttribute> (pi))
					appearance_selectors.Add (pi);

				if (type == pi.DeclaringType || type.IsSubclassOf (pi.DeclaringType)) {
					// not an injected protocol property.
					bound_properties.Add (pi.Name);
				} else {
					// don't inject a protocol property if the class already
					// implements the same property.
					if (bound_properties.Contains (pi.Name))
						continue;

					var protocolsThatHaveThisProp = GetTypeContractProperties (type).Where (x => x.Name == pi.Name);
					if (protocolsThatHaveThisProp.Count () > 1) {
						// If multiple protocols have this property and we haven't generated a copy yet
						if (generated_properties.Contains (pi.Name))
							continue;

						// If there is a version that has get and set
						if (protocolsThatHaveThisProp.Any (x => x.CanRead && x.CanWrite)) {
							// Skip if we are not it
							if (!(pi.CanRead && pi.CanWrite))
								continue;
						}
						else {
							// Verify all of the versions have the same get/set abilities since there is no universal get/set version
							// And just generate the first one (us)
							if (!protocolsThatHaveThisProp.All (x => x.CanRead == pi.CanRead && x.CanWrite == pi.CanWrite))
								throw new BindingException (1037, true, "The selector {0} on type {1} is found multiple times with both read only and write only versions, with no read/write version.", pi.Name, type.Name);
						}
					}
				}

				generated_properties.Add (pi.Name);
				GenerateProperty (type, pi, instance_fields_to_clear_on_dispose, is_model);
			}
			
			if (field_exports.Count != 0){
				foreach (var field_pi in field_exports.OrderBy (f => f.Name, StringComparer.Ordinal)) {
					var fieldAttr = AttributeManager.GetCustomAttribute<FieldAttribute> (field_pi);
					ComputeLibraryName (fieldAttr, type, field_pi.Name, out string library_name, out string library_path);

					bool is_unified_internal = field_pi.IsUnifiedInternal ();

					string fieldTypeName;
					string smartEnumTypeName = null;
					if (IsSmartEnum (field_pi.PropertyType)) {
						fieldTypeName = FormatType (TypeManager.NSString.DeclaringType, TypeManager.NSString);
						smartEnumTypeName = FormatType (field_pi.DeclaringType, field_pi.PropertyType);
					} else
						fieldTypeName = FormatType (field_pi.DeclaringType, field_pi.PropertyType);

					// Value types we dont cache for now, to avoid Nullable<T>
					if (!field_pi.PropertyType.IsValueType || smartEnumTypeName != null) {
						print_generated_code ();
						PrintPreserveAttribute (field_pi);
						print ("static {0} _{1};", fieldTypeName, field_pi.Name);
					}

					PrintAttributes (field_pi, preserve:true, advice:true);
					print ("[Field (\"{0}\",  \"{1}\")]", fieldAttr.SymbolName, library_path ?? library_name);
					PrintPlatformAttributes (field_pi);
					if (AttributeManager.HasAttribute<AdvancedAttribute> (field_pi)) {
						print ("[EditorBrowsable (EditorBrowsableState.Advanced)]");
					}
					// Check if this is a notification and print Advice to use our Notification API
					if (AttributeManager.HasAttribute<NotificationAttribute> (field_pi))
						print ($"[Advice (\"Use {type.Name}.Notifications.Observe{GetNotificationName (field_pi)} helper method instead.\")]");

					print ("{0} static {1} {2}{3} {{", field_pi.IsInternal () ? "internal" : "public",
						smartEnumTypeName ?? fieldTypeName,
						field_pi.Name,
						is_unified_internal ? "_" : "");
					indent++;

					PrintAttributes (field_pi, platform:true);
					PrintAttributes (field_pi.GetGetMethod (), preserve:true, advice:true);
					print ("get {");
					indent++;
					if (field_pi.PropertyType == TypeManager.NSString){
						print ("if (_{0} == null)", field_pi.Name);
						indent++;
						print ("_{0} = Dlfcn.GetStringConstant (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
						indent--;
						print ("return _{0};", field_pi.Name);
					} else if (field_pi.PropertyType.Name == "NSArray"){
						print ("if (_{0} == null)", field_pi.Name);
						indent++;
						print ("_{0} = Runtime.GetNSObject<NSArray> (Dlfcn.GetIndirect (Libraries.{2}.Handle, \"{1}\"));", field_pi.Name, fieldAttr.SymbolName, library_name);
						indent--;
						print ("return _{0};", field_pi.Name);
					} else if (field_pi.PropertyType == TypeManager.System_Int32){
						print ("return Dlfcn.GetInt32 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeManager.System_UInt32) {
						print ("return Dlfcn.GetUInt32 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeManager.System_Double){
						print ("return Dlfcn.GetDouble (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeManager.System_Float){
						print ("return Dlfcn.GetFloat (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeManager.System_IntPtr){
						print ("return Dlfcn.GetIntPtr (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType.FullName == "System.Drawing.SizeF"){
						print ("return Dlfcn.GetSizeF (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeManager.System_Int64){
						print ("return Dlfcn.GetInt64 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeManager.System_UInt64) {
						print ("return Dlfcn.GetUInt64 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else
						//
						// Handle various blittable value types here
						//
						if (Frameworks.HaveCoreMedia && Frameworks.HaveAVFoundation && (field_pi.PropertyType == TypeManager.CMTime ||
						   field_pi.PropertyType == TypeManager.AVCaptureWhiteBalanceGains)) {
						print ("return *(({3} *) Dlfcn.dlsym (Libraries.{2}.Handle, \"{1}\"));", field_pi.Name, fieldAttr.SymbolName, library_name,
						       FormatType (type, field_pi.PropertyType.Namespace, field_pi.PropertyType.Name));
					} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.System_nint) {
						print ("return Dlfcn.GetNInt (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.System_nuint) {
						print ("return Dlfcn.GetNUInt (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.System_nfloat) {
						print ("return Dlfcn.GetNFloat (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.CoreGraphics_CGSize){
						print ("return Dlfcn.GetCGSize (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType.IsEnum) {
						var btype = field_pi.PropertyType.GetEnumUnderlyingType ();
						if (smartEnumTypeName != null) {
							print ("if (_{0} == null)", field_pi.Name);
							indent++;
							print ("_{0} = Dlfcn.GetStringConstant (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
							indent--;
							print ($"return {smartEnumTypeName}Extensions.GetValue (_{field_pi.Name});");
						} else if (UnifiedAPI && IsNativeEnum (field_pi.PropertyType)) {
							if (btype == TypeManager.System_nint || btype == TypeManager.System_Int64)
								print ($"return ({fieldTypeName}) (long) Dlfcn.GetNInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");" );
							else if (btype == TypeManager.System_nuint || btype == TypeManager.System_UInt64)
								print ($"return ({fieldTypeName}) (ulong) Dlfcn.GetNUInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else
								throw new BindingException (1014, true, $"Unsupported type for Fields: {fieldTypeName} for '{field_pi}'.");
						} else {
							if (btype == TypeManager.System_Int32)
								print ($"return ({fieldTypeName}) Dlfcn.GetInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else if (btype == TypeManager.System_UInt32)
								print ($"return ({fieldTypeName}) Dlfcn.GetUInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else if (btype == TypeManager.System_Int64)
								print ($"return ({fieldTypeName}) Dlfcn.GetInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else if (btype == TypeManager.System_UInt64)
								print ($"return ({fieldTypeName}) Dlfcn.GetUInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else
								throw new BindingException (1014, true, $"Unsupported type for Fields: {fieldTypeName} for '{field_pi}'.");
						}
					} else {
						if (field_pi.PropertyType == TypeManager.System_String)
							throw new BindingException (1013, true, "Unsupported type for Fields (string), you probably meant NSString");
						else
							throw new BindingException (1014, true, $"Unsupported type for Fields: {fieldTypeName} for '{field_pi}'.");
					}
					
					indent--;
					print ("}");

					if (field_pi.CanWrite) {
						PrintAttributes (field_pi, platform:true);
						PrintAttributes (field_pi.GetSetMethod (), preserve:true, advice:true);
						print ("set {");
						indent++;
						if (field_pi.PropertyType == TypeManager.System_Int32) {
							print ("Dlfcn.SetInt32 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.System_UInt32) {
							print ("Dlfcn.SetUInt32 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.System_Double) {
							print ("Dlfcn.SetDouble (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.System_Float) {
							print ("Dlfcn.SetFloat (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.System_IntPtr) {
							print ("Dlfcn.SetIntPtr (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType.FullName == "System.Drawing.SizeF") {
							print ("Dlfcn.SetSizeF (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.System_Int64) {
							print ("Dlfcn.SetInt64 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.System_UInt64) {
							print ("Dlfcn.SetUInt64 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeManager.NSString){
							print ("Dlfcn.SetString (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType.Name == "NSArray"){
							print ("Dlfcn.SetArray (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.System_nint) {
							print ("Dlfcn.SetNInt (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.System_nuint) {
							print ("Dlfcn.SetNUInt (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.System_nfloat) {
							print ("Dlfcn.SetNFloat (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (UnifiedAPI && field_pi.PropertyType == TypeManager.CoreGraphics_CGSize) {
							print ("Dlfcn.SetCGSize (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType.IsEnum) {
							var btype = field_pi.PropertyType.GetEnumUnderlyingType ();
							if (smartEnumTypeName != null)
								print ($"Dlfcn.SetString (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", value.GetConstant ());");
							else if (UnifiedAPI && IsNativeEnum (field_pi.PropertyType)) {
								if (btype == TypeManager.System_nint || (BindThirdPartyLibrary && btype == TypeManager.System_Int64))
									print ($"Dlfcn.SetNInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (nint) (long) value);");
								else if (btype == TypeManager.System_nuint || (BindThirdPartyLibrary && btype == TypeManager.System_UInt64))
									print ($"Dlfcn.SetNUInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (nuint) (ulong) value);");
								else
									throw new BindingException (1021, true, "Unsupported type for read/write Fields: {0} for {1}.{2}", fieldTypeName, field_pi.DeclaringType.FullName, field_pi.Name);
							} else {
								if (btype == TypeManager.System_Int32)
									print ($"Dlfcn.SetInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (int) value);");
								else if (btype == TypeManager.System_UInt32)
									print ($"Dlfcn.SetUInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (uint) value);");
								else if (btype == TypeManager.System_Int64)
									print ($"Dlfcn.SetInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (long) value);");
								else if (btype == TypeManager.System_UInt64)
									print ($"Dlfcn.SetUInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (ulong) value);");
								else
									throw new BindingException (1021, true, "Unsupported type for read/write Fields: {0} for {1}.{2}", fieldTypeName, field_pi.DeclaringType.FullName, field_pi.Name);
							}
						} else
							throw new BindingException (1021, true, "Unsupported type for read/write Fields: {0} for {1}.{2}", fieldTypeName, field_pi.DeclaringType.FullName, field_pi.Name);
						indent--;
						print ("}");
					}
					indent--;
					print ("}");
				}
			}

			var eventArgTypes = new Dictionary<string,ParameterInfo[]> ();

			if (bta != null && bta.Events != null){
				if (bta.Delegates == null)
					throw new BindingException (1015, true, "In class {0} You specified the Events property, but did not bind those to names with Delegates", type.FullName);
				
				print ("//");
				print ("// Events and properties from the delegate");
				print ("//\n");

				if (bta.Events.Length != bta.Delegates.Length)
					throw new BindingException (1023, true, "The number of events and delegates must match for `{0}`", type.FullName);

				int delidx = 0;
				foreach (var dtype in bta.Events) {
					string delName = bta.Delegates [delidx++];
					delName = delName.StartsWith ("Weak", StringComparison.Ordinal) ? delName.Substring(4) : delName;

					// Here's the problem:
					//    If you have two or more types in an inheritence structure in the binding that expose events
					//    they can fight over who's generated delegate gets used if you use the events at both level.
					//    e.g. NSComboxBox.SelectionChanged (on NSComboxBox) and NSComboBox.EditingEnded (on NSTextField)
					// We solve this under Unified when the delegate is protocalized (and leave Classic how it always has been)
					// To handle this case, we do two things:
					//    1) We have to ensure that the same internal delegate is uses for base and derived events, so they
					//     aren't stepping on each other toes. This means we always instance up the leaf class's internal delegate
					//    2) We have to have an some for of "inheritance" in the generated delegates, so the
					//     base class can use this derived type (and have the events it expects on it) We do this via inhertiance and
					//     use of the protocal interfaces.
					//     See xamcore/test/apitest/DerivedEventTest.cs for specific tests

					// Does this delegate qualify for this treatment. We fall back on the old "wrong" codegen if not.
					bool isProtocolizedEventBacked = IsProtocolizedEventBacked (delName, type);

					// The name of the the generated virtual property that specifies the type of the leaf most internal delegate
					string delegateTypePropertyName = GetDelegateTypePropertyName (delName);

					// The name of the method to instance up said internal delegate
					string delegateCreationMethodName = "CreateInternalEvent" + delName + "Type";

					// If we're not the base class in our inheritance tree to define this delegate, we need to override instead
					bool shouldOverride = HasParentWithSameNamedDelegate (bta, delName);

					// The name of the protocol in question
					string interfaceName = GenerateInterfaceTypeName (bta, delName, dtype.Name);

					bool hasKeepRefUntil = bta.KeepRefUntil != null;

					if (isProtocolizedEventBacked) {
						// The generated virtual type property and creation virtual method
						string generatedTypeOverrideType = shouldOverride ? "override" :  "virtual";
						print ("internal {0} Type {1}", generatedTypeOverrideType, delegateTypePropertyName);
						print ("{");
						print ("	get {{ return typeof (_{0}); }}", dtype.Name);
						print ("}\n");

						print ("internal {0} _{1} {2} ({3})", generatedTypeOverrideType, interfaceName, delegateCreationMethodName, hasKeepRefUntil ? "object oref" : "");
						print ("{");
						print ("	return (_{0})(new _{1}({2}));", interfaceName, dtype.Name, hasKeepRefUntil ? "oref" : "");
						print ("}\n");
					}

					if (!hasKeepRefUntil)
						print ("{0}_{1} Ensure{1} ()", isProtocolizedEventBacked ? "internal " : "", dtype.Name);
					else {
						print ("static System.Collections.ArrayList instances;");
						print ("{0}_{1} Ensure{1} (object oref)", isProtocolizedEventBacked ? "internal " : "", dtype.Name);
					}
					
					print ("{"); indent++;

					if (isProtocolizedEventBacked) {
						// If our delegate not null and it isn't the same type as our property
						//   - We're in one of two cases: The user += an Event and then assigned their own delegate or the inverse
						//   - One of them isn't being called anymore no matter what. Throw an exception.
						if (!BindThirdPartyLibrary) {
							print ("if ({0} != null)", delName);
							print ("\t{0}.EnsureEventAndDelegateAreNotMismatched ({1}, {2});", ApplicationClassName, delName, delegateTypePropertyName);
						}

						print ("_{0} del = {1} as _{0};", dtype.Name, delName);

						print ("if (del == null){");
						indent++;
						if (!hasKeepRefUntil)
							print ("del = (_{0}){1} ();", dtype.Name, delegateCreationMethodName);
						else {
							string oref = "new object[] {\"oref\"}";
							print ("del = (_{0}){1} ({2});", dtype.Name, delegateCreationMethodName, oref);
							print ("if (instances == null) instances = new System.Collections.ArrayList ();");
							print ("if (!instances.Contains (this)) instances.Add (this);");
						}
						print ("{0} = (I{1})del;", delName, dtype.Name);
						indent--;
						print ("}");
						print ("return del;");
					}
					else {
						print ("var del = {0};", delName);
						print ("if (del == null || (!(del is _{0}))){{", dtype.Name);
						print ("\tdel = new _{0} ({1});", dtype.Name, bta.KeepRefUntil == null ? "" : "oref");
						if (hasKeepRefUntil) {
							print ("\tif (instances == null) instances = new System.Collections.ArrayList ();");
							print ("\tif (!instances.Contains (this)) instances.Add (this);");
						}
						print ("\t{0} = del;", delName);
						print ("}");
						print ("return (_{0}) del;", dtype.Name);
					}
					indent--; print ("}\n");

					var noDefaultValue = new List<MethodInfo> ();
					
					print ("#pragma warning disable 672");
					print ("[Register]");
					if (isProtocolizedEventBacked)
						print ("internal class _{0} : {1}I{2} {{ ", dtype.Name, shouldOverride ? "_" + interfaceName + ", " : "NSObject, ", dtype.Name);
					else
						print ("sealed class _{0} : {1} {{ ", dtype.Name, RenderType (dtype));


					indent++;
					if (hasKeepRefUntil){
						print ("object reference;");
						print ("public _{0} (object reference) {{ this.reference = reference; IsDirectBinding = false; }}\n", dtype.Name);
					} else 
						print ("public _{0} () {{ IsDirectBinding = false; }}\n", dtype.Name);
						

					string shouldOverrideDelegateString = isProtocolizedEventBacked ? "" : "override ";

					string previous_miname = null;
					int miname_count = 0;
					foreach (var mi in dtype.GatherMethods ().OrderBy (m => m.Name, StringComparer.Ordinal)) {
						if (ShouldSkipEventGeneration (mi))
							continue;
						
						var pars = mi.GetParameters ();
						int minPars = bta.Singleton ? 0 : 1;

						if (AttributeManager.HasAttribute<NoDefaultValueAttribute> (mi))
							noDefaultValue.Add (mi);

						if (pars.Length < minPars)
							throw new BindingException (1003, true, "The delegate method {0}.{1} needs to take at least one parameter", dtype.FullName, mi.Name);
						
						var sender = pars.Length == 0 ? "this" : pars [0].Name;

						var miname = PascalCase (mi.Name);
						if (miname == previous_miname) {
							// overloads, add a numbered suffix (it's internal)
							previous_miname = miname;
							miname += (++miname_count).ToString ();
						} else
							previous_miname = miname;

						if (mi.ReturnType == TypeManager.System_Void){
							if (bta.Singleton || mi.GetParameters ().Length == 1)
								print ("internal EventHandler {0};", miname);
							else
								print ("internal EventHandler<{0}> {1};", GetEventArgName (mi), miname);
						} else
							print ("internal {0} {1};", GetDelegateName (mi), miname);

						print ("[Preserve (Conditional = true)]");
						if (isProtocolizedEventBacked)
							print ("[Export (\"{0}\")]", FindSelector (dtype, mi));

						print ("public {0}{1} {2} ({3})", shouldOverrideDelegateString, RenderType (mi.ReturnType), mi.Name, RenderParameterDecl (pars));
						print ("{"); indent++;

						if (mi.Name == bta.KeepRefUntil)
							print ("instances.Remove (reference);");
						
						if (mi.ReturnType == TypeManager.System_Void){
							string eaname;

							if (debug)
								print ("Console.WriteLine (\"Method {0}.{1} invoked\");", dtype.Name, mi.Name);
							if (pars.Length != minPars){
								eaname = GetEventArgName (mi);
								if (!generatedEvents.ContainsKey (eaname) && !eventArgTypes.ContainsKey (eaname)){
									eventArgTypes.Add (eaname, pars);
									generatedEvents.Add (eaname, pars);
								}
							} else
								eaname = "<NOTREACHED>";
							
							if (bta.Singleton || mi.GetParameters ().Length == 1)
								print ("EventHandler handler = {0};", PascalCase (miname));
							else
								print ("EventHandler<{0}> handler = {1};", GetEventArgName (mi), miname);

							print ("if (handler != null){");
							indent++;
							string eventArgs;
							if (pars.Length == minPars)
								eventArgs = "EventArgs.Empty";
							else {
								print ("var args = new {0} ({1});", eaname, RenderArgs (pars.Skip (minPars), true));
								eventArgs = "args";
							}

							print ("handler ({0}, {1});", sender, eventArgs);
							if (pars.Length != minPars && MustPullValuesBack (pars.Skip (minPars))){
								foreach (var par in pars.Skip (minPars)){
									if (!par.ParameterType.IsByRef)
										continue;

									print ("{0} = args.{1};", par.Name, GetPublicParameterName (par));
								}
							}
							if (AttributeManager.HasAttribute<CheckDisposedAttribute> (mi)) {
								var arg = RenderArgs (pars.Take (1));
								print ("if ({0}.Handle == IntPtr.Zero)", arg);
								print ("\tthrow new ObjectDisposedException (\"{0}\", \"The object was disposed on the event, you should not call Dispose() inside the handler\");", arg);
							}
							indent--;
							print ("}");
						} else {
							var delname = GetDelegateName (mi);

							if (!generatedDelegates.ContainsKey (delname) && !delegate_types.ContainsKey (delname)){
								generatedDelegates.Add (delname, null);
								delegate_types.Add (type.Namespace + "." + delname, mi);
							}
							if (debug)
								print ("Console.WriteLine (\"Method {0}.{1} invoked\");", dtype.Name, mi.Name);

							print ("{0} handler = {1};", delname, PascalCase (miname));
							print ("if (handler != null)");
							print ("	return handler ({0}{1});",
							       sender,
							       pars.Length == minPars ? "" : String.Format (", {0}", RenderArgs (pars.Skip (1))));

							if (AttributeManager.HasAttribute<NoDefaultValueAttribute> (mi))
								print ("throw new You_Should_Not_Call_base_In_This_Method ();");
							else {
								var def = GetDefaultValue (mi);
								if ((def is string) && ((def as string) == "null") && mi.ReturnType.IsValueType)
									print ("throw new Exception (\"No event handler has been added to the {0} event.\");", mi.Name);
								else {
									foreach (var j in pars){
										if (j.ParameterType.IsByRef && j.IsOut){
											print ("{0} = null;", j.Name.GetSafeParamName ());
										}
									}
										
									print ("return {0};", def);
								}
							}
						}
						
						indent--;
						print ("}\n");
					}

					if (noDefaultValue.Count > 0) {
						string selRespondsToSelector = "Selector.GetHandle (\"respondsToSelector:\")";

						if (!InlineSelectors) {
							foreach (var mi in noDefaultValue) {
								var export = AttributeManager.GetCustomAttribute<ExportAttribute> (mi);
								print ("static IntPtr sel{0}Handle = Selector.GetHandle (\"{1}\");", mi.Name, export.Selector);
							}
							print ("static IntPtr selRespondsToSelector = " + selRespondsToSelector + ";");
							selRespondsToSelector = "selRespondsToSelector";
						}

						print ("[Preserve (Conditional = true)]");
						print ("public override bool RespondsToSelector (Selector sel)");
						print ("{");
						++indent;
						print ("if (sel == null)");
						++indent;
						print ("return false;");
						--indent;
						print ("IntPtr selHandle = sel == null ? IntPtr.Zero : sel.Handle;");
						foreach (var mi in noDefaultValue.OrderBy (m => m.Name, StringComparer.Ordinal)) {
							if (InlineSelectors) {
								var export = AttributeManager.GetCustomAttribute<ExportAttribute> (mi);
								print ("if (selHandle.Equals (Selector.GetHandle (\"{0}\")))", export.Selector);
							} else {
								print ("if (selHandle.Equals (sel{0}Handle))", mi.Name);
							}
							++indent;
							print ("return {0} != null;", PascalCase (mi.Name));
							--indent;
						}
						print ("return global::" + ns.Messaging + ".bool_objc_msgSendSuper_IntPtr (SuperHandle, " + selRespondsToSelector + ", selHandle);");
						--indent;
						print ("}");

						// Make sure we generate the required signature in Messaging only if needed 
						// bool_objc_msgSendSuper_IntPtr: for respondsToSelector:
						if (!send_methods.ContainsKey ("bool_objc_msgSendSuper_IntPtr")) {
							print (m, "[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
							print (m, "public extern static bool bool_objc_msgSendSuper_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
							RegisterMethodName ("bool_objc_msgSendSuper_IntPtr");
						}
					}

					indent--;
					print ("}");

					print ("#pragma warning restore 672");
				}
				print ("");

				string prev_miname = null;
				int minameCount = 0;
				repeatedDelegateApiNames.Clear ();
				// Now add the instance vars and event handlers
				foreach (var dtype in bta.Events.OrderBy (d => d.Name, StringComparer.Ordinal)) {
					foreach (var mi in dtype.GatherMethods ().OrderBy (m => m.Name, StringComparer.Ordinal)) {
						if (ShouldSkipEventGeneration (mi))
							continue;

						string ensureArg = bta.KeepRefUntil == null ? "" : "this";

						var miname = PascalCase (mi.Name);
						if (miname == prev_miname) {
							// overloads, add a numbered suffix (it's internal)
							prev_miname = miname;
							miname += (++minameCount).ToString ();
						} else
							prev_miname = miname;
						
						if (mi.ReturnType == TypeManager.System_Void){
							foreach (var oa in AttributeManager.GetCustomAttributes<ObsoleteAttribute> (mi))
								print ("[Obsolete (\"{0}\", {1})]", oa.Message, oa.IsError ? "true" : "false");

							if (bta.Singleton && mi.GetParameters ().Length == 0 || mi.GetParameters ().Length == 1)
								print ("public event EventHandler {0} {{", CamelCase (GetEventName (mi)));
							else 
								print ("public event EventHandler<{0}> {1} {{", GetEventArgName (mi), CamelCase (GetEventName (mi)));
							print ("\tadd {{ Ensure{0} ({1}).{2} += value; }}", dtype.Name, ensureArg, miname);
							print ("\tremove {{ Ensure{0} ({1}).{2} -= value; }}", dtype.Name, ensureArg, miname);
							print ("}\n");
						} else {
							print ("public {0} {1} {{", GetDelegateName (mi), CamelCase (GetDelegateApiName (mi)));
							print ("\tget {{ return Ensure{0} ({1}).{2}; }}", dtype.Name, ensureArg, miname);
							print ("\tset {{ Ensure{0} ({1}).{2} = value; }}", dtype.Name, ensureArg, miname);
							print ("}\n");
						}
					}
				}
			}

			//
			// Do we need a dispose method?
			//
			if (!is_static_class){
				var disposeAttr = AttributeManager.GetCustomAttributes<DisposeAttribute> (type);
				if (disposeAttr.Length > 0 || instance_fields_to_clear_on_dispose.Count > 0){
					print_generated_code ();
					print ("protected override void Dispose (bool disposing)");
					print ("{");
					indent++;
					if (disposeAttr.Length > 0){
						var snippet = disposeAttr [0];
						Inject (snippet);
					}
					
					print ("base.Dispose (disposing);");
					
					if (instance_fields_to_clear_on_dispose.Count > 0) {
						print ("if (Handle == IntPtr.Zero) {");
						indent++;
						foreach (var field in instance_fields_to_clear_on_dispose.OrderBy (f => f, StringComparer.Ordinal))
							print ("{0} = null;", field);
						indent--;
						print ("}");
					}
					
					indent--;
					print ("}");
				}
			}

			//
			// Appearance class
			//
			var gt = GeneratedType.Lookup (type);
			if (gt.ImplementsAppearance){
				var parent_implements_appearance = gt.Parent != null && gt.ParentGenerated.ImplementsAppearance;
				string base_class;
				
				if (parent_implements_appearance){
					var parent = GetGeneratedTypeName (gt.Parent);
					base_class = "global::" + gt.Parent.FullName + "." + parent + "Appearance";
				} else
					base_class = "UIAppearance";

				string appearance_type_name = TypeName + "Appearance";
				print ("public partial class {0} : {1} {{", appearance_type_name, base_class);
				indent++;
				print ("protected internal {0} (IntPtr handle) : base (handle) {{}}", appearance_type_name);

				if (appearance_selectors != null){
					var currently_ignored_fields = new List<string> ();
					
					foreach (MemberInfo mi in appearance_selectors.OrderBy (m => m.Name, StringComparer.Ordinal)) {
						if (mi is MethodInfo)
							GenerateMethod (type, mi as MethodInfo,
									is_model: false,
									category_extension_type: is_category_class ? base_type : null,
									is_appearance: true);
						else
							GenerateProperty (type, mi as PropertyInfo, currently_ignored_fields, false);
					}
				}
				
				indent--;
				print ("}\n");
				print ("public static {0}{1} Appearance {{", parent_implements_appearance ? "new " : "", appearance_type_name);
				indent++;
				print ("get {{ return new {0} (global::{1}.IntPtr_objc_msgSend (class_ptr, {2})); }}", appearance_type_name, ns.Messaging, InlineSelectors ? ns.CoreObjCRuntime + ".Selector.GetHandle (\"appearance\")" : "UIAppearance.SelectorAppearance");
				indent--;
				print ("}\n");
				print ("public static {0}{1} GetAppearance<T> () where T: {2} {{", parent_implements_appearance ? "new " : "", appearance_type_name, TypeName);
				indent++;
				print ("return new {0} (global::{1}.IntPtr_objc_msgSend (Class.GetHandle (typeof (T)), {2}));", appearance_type_name, ns.Messaging, InlineSelectors ? ns.CoreObjCRuntime + ".Selector.GetHandle (\"appearance\")" : "UIAppearance.SelectorAppearance");
				indent--;
				print ("}\n");
				print ("public static {0}{1} AppearanceWhenContainedIn (params Type [] containers)", parent_implements_appearance ? "new " : "", appearance_type_name);
				print ("{");
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (class_ptr, containers));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance (UITraitCollection traits) {{", parent_implements_appearance ? "new " : "", appearance_type_name);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (class_ptr, traits));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance (UITraitCollection traits, params Type [] containers) {{", parent_implements_appearance ? "new " : "", appearance_type_name);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (class_ptr, traits, containers));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance<T> (UITraitCollection traits) where T: {2} {{", parent_implements_appearance ? "new " : "", appearance_type_name, TypeName);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (Class.GetHandle (typeof (T)), traits));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance<T> (UITraitCollection traits, params Type [] containers) where T: {2}{{", parent_implements_appearance ? "new " : "", appearance_type_name, TypeName);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (Class.GetHandle (typeof (T)), containers));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("");
			}
			//
			// Notification extensions
			//
			if (notifications.Count > 0){
				print ("\n");
				print ("//");
				print ("// Notifications");
				print ("//");
			
				print ("public static partial class Notifications {\n");
				foreach (var property in notifications.OrderBy (p => p.Name, StringComparer.Ordinal)) {
					string notification_name = GetNotificationName (property);
					string notification_center = GetNotificationCenter (property);

					foreach (var notification_attribute in AttributeManager.GetCustomAttributes<NotificationAttribute> (property)) {
						Type event_args_type = notification_attribute.Type;
						string event_name = event_args_type == null ? "NSNotificationEventArgs" : event_args_type.FullName;

						if (event_args_type != null)
							notification_event_arg_types [event_args_type] = event_args_type;
						print ("\tpublic static NSObject Observe{0} (EventHandler<{1}> handler)", notification_name, event_name);
						print ("\t{");
						print ("\t\treturn {0}.AddObserver ({1}, notification => handler (null, new {2} (notification)));", notification_center, property.Name, event_name);
						print ("\t}");
						print ("\tpublic static NSObject Observe{0} (NSObject objectToObserve, EventHandler<{1}> handler)", notification_name, event_name);
						print ("\t{");
						print ("\t\treturn {0}.AddObserver ({1}, notification => handler (null, new {2} (notification)), objectToObserve);", notification_center, property.Name, event_name);
						print ("\t}");
					}
				}
				print ("\n}");
			}

			indent--;
			print ("}} /* class {0} */", TypeName);
			
			//
			// Copy delegates from the API files into the output if they were declared there
			//
			var rootAssembly = types [0].Assembly;
			foreach (var deltype in trampolines.Keys.OrderBy (d => d.Name, StringComparer.Ordinal)) {
				if (deltype.Assembly != rootAssembly)
					continue;

				// This formats the delegate 
				delegate_types [deltype.FullName] = deltype.GetMethod ("Invoke");
			}

			if (eventArgTypes.Count > 0){
				print ("\n");
				print ("//");
				print ("// EventArgs classes");
				print ("//");
			}
			// Now add the EventArgs classes
			foreach (var eaclass in eventArgTypes.Keys.OrderBy (e => e, StringComparer.Ordinal)) {
				if (skipGeneration.ContainsKey (eaclass)){
					continue;
				}
				int minPars = bta.Singleton ? 0 : 1;
				
				var pars = eventArgTypes [eaclass];

				print ("public partial class {0} : EventArgs {{", eaclass); indent++;
				print ("public {0} ({1})", eaclass, RenderParameterDecl (pars.Skip (1), true));
				print ("{");
				indent++;
				foreach (var p in pars.Skip (minPars).OrderBy (p => p.Name, StringComparer.Ordinal)) {
					print ("this.{0} = {1};", GetPublicParameterName (p), p.Name);
				}
				indent--;
				print ("}");
				
				// Now print the properties
				foreach (var p in pars.Skip (minPars).OrderBy (p => p.Name, StringComparer.Ordinal)) {
					var bareType = p.ParameterType.IsByRef ? p.ParameterType.GetElementType () : p.ParameterType;

					print ("public {0} {1} {{ get; set; }}", RenderType (bareType), GetPublicParameterName (p));
				}
				indent--; print ("}\n");
			}

			if (async_result_types.Count > 0) {
				print ("\n");
				print ("//");
				print ("// Async result classes");
				print ("//");
			}

			foreach (var async_type in async_result_types.OrderBy (t => t.Item1, StringComparer.Ordinal)) {
				if (async_result_types_emitted.Contains (async_type.Item1))
					continue;
				async_result_types_emitted.Add (async_type.Item1);

				print ("public partial class {0} {{", async_type.Item1); indent++;

				StringBuilder ctor = new StringBuilder ();

				bool comma = false;
				foreach (var pi in async_type.Item2) {
					print ("public {0} {1} {{ get; set; }}",
						FormatType (type, pi.ParameterType),
						Capitalize (pi.Name.GetSafeParamName ()));

					if (comma)
						ctor.Append (", ");
					comma = true;
					ctor.Append (FormatType (type, pi.ParameterType)).Append (" ").Append (pi.Name.GetSafeParamName ());
				}

				print ("\npartial void Initialize ();");

				print ("\npublic {0} ({1}) {{", async_type.Item1, ctor); indent++;
				foreach (var pi in async_type.Item2) {
					print ("this.{0} = {1};", Capitalize (pi.Name.GetSafeParamName ()), pi.Name.GetSafeParamName ());
				}
				print ("Initialize ();");
				indent--; print ("}");

				indent--; print ("}\n");
			}
			async_result_types.Clear ();

			if (type.IsNested){
				indent--;
				print ("}");
			}
			if (type.Namespace != null) {
				indent--;
				print ("}");
			}
		}
	}

	static string GetDelegateTypePropertyName (string delName)
	{
		return "GetInternalEvent" + delName + "Type";
	}

	static bool SafeIsProtocolizedEventBacked (string propertyName, Type type)
	{
		return CoreIsProtocolizedEventBacked (propertyName, type, false);
	}

	static bool IsProtocolizedEventBacked (string propertyName, Type type)
	{
		return CoreIsProtocolizedEventBacked (propertyName, type, true);
	}

	static bool CoreIsProtocolizedEventBacked (string propertyName, Type type, bool shouldThrowOnNotFound)
	{
		PropertyInfo pi = type.GetProperty(propertyName);
		BaseTypeAttribute bta = ReflectionExtensions.GetBaseTypeAttribute (type);
		if (pi == null || bta == null) {
			if (shouldThrowOnNotFound) {
 				if (propertyName == "Delegate" && bta.Delegates.Count () > 0) {
					var delegates = new List <string> (bta.Delegates);
					// grab all the properties that have the Wrap attr
					var propsAttrs = from p in type.GetProperties ()
						let attrs = AttributeManager.GetCustomAttributes<WrapAttribute> (p)
						where p.Name != "Delegate" && attrs.Length > 0
						select new { Name = p.Name, Attributes = Array.ConvertAll (attrs, item => item.MethodName) };

					var props = new List <string> ();
					foreach (var p in propsAttrs) {
						if (delegates.Intersect (p.Attributes).Any ()) {
							// add quoates since we are only using this info for the exception message
							props.Add (String.Format ("'{0}'", p.Name));
						}
					}
					if (props.Count () == 1)
						throw new BindingException (1112, true,
							"Property {0} should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work.", props[0], false);
					else if (props.Count () > 1)
						throw new BindingException (1112, true,
							"Properties {0} should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work.",
							String.Join (", ", props.ToArray ()), false);
					else
						throw new BindingException (1113, true,
							"BaseType.Delegates were set but no properties could be found. Do ensure that the WrapAttribute is used on the right properties.", false);
				} else
					throw new BindingException (1114, "Binding error: test unable to find property: {0} on {1}", propertyName, type, false);
			} else
				return false;
		}

		return Protocolize (pi) && bta.Events != null && bta.Events.Any (x => x.Name == pi.PropertyType.Name);
	}

	string FindSelector (Type type, MethodInfo mi)
	{
		Type currentType = type;
		do
		{
			// avoid AmbiguousMatchException when GetMethod is used.
			var parameters = mi.GetParameters ().Select ((arg) => arg.ParameterType).ToArray ();
			MethodInfo method = currentType.GetMethod (mi.Name, parameters);
			if (method != null) {
				string wrap;
				ExportAttribute export = Generator.GetExportAttribute (method, out wrap);
				if (export != null)
					return export.Selector;
			}
			BaseTypeAttribute bta = ReflectionExtensions.GetBaseTypeAttribute (currentType);

			if (bta == null)
				break;
			currentType = bta.BaseType;
		}
		while (currentType != null);
		throw new BindingException (1035, true, "Unable to find selector for {0} on {1} on self or base class", mi, type);
	}

	string GenerateInterfaceTypeName (BaseTypeAttribute bta, string delName, string currentTypeName)
	{
		bool shouldOverride = HasParentWithSameNamedDelegate (bta, delName);
		if (shouldOverride) {
			Type parentType = GetParentTypeWithSameNamedDelegate (bta, delName);
			PropertyInfo parentProperty = parentType.GetProperty (delName);
			return parentProperty.PropertyType.Name;
		}
		return currentTypeName;
	}

	bool ShouldSkipEventGeneration (MethodInfo mi)
	{
		// Skip property getter/setters
		if (mi.IsSpecialName && (mi.Name.StartsWith ("get_", StringComparison.Ordinal) || mi.Name.StartsWith ("set_", StringComparison.Ordinal)))
			return true;

		if (mi.IsUnavailable ())
			return true;

		// Skip those methods marked to be ignored by the developer
		return AttributeManager.HasAttribute<IgnoredInDelegateAttribute> (mi);
	}

	// Safely strips away any Weak from the beginning of either delegate and returns if they match
	static bool CompareTwoDelegateNames (string lhsDel, string rhsDel)
	{
		lhsDel = lhsDel.StartsWith ("Weak", StringComparison.Ordinal) ? lhsDel.Substring (4): lhsDel;
		rhsDel = rhsDel.StartsWith ("Weak", StringComparison.Ordinal) ? rhsDel.Substring (4): rhsDel;
		return lhsDel == rhsDel;
	}

	Type GetParentTypeWithSameNamedDelegate (BaseTypeAttribute bta, string delegateName)
	{
		Type currentType = bta.BaseType;
		while (currentType != null && currentType != TypeManager.NSObject)
		{
			BaseTypeAttribute currentBta = ReflectionExtensions.GetBaseTypeAttribute (currentType);
			if (currentBta != null && currentBta.Events != null) {
				int delidx = 0;
				foreach (var v in currentBta.Events) {
					string currentDelName = currentBta.Delegates [delidx++];
					if (CompareTwoDelegateNames (currentDelName, delegateName))
						return currentType;
				}
			}
			currentType = currentType.BaseType;
		}
		return null;
	}

	bool HasParentWithSameNamedDelegate (BaseTypeAttribute bta, string delegateName)
	{
		return GetParentTypeWithSameNamedDelegate (bta, delegateName) != null;
	}

	// TODO: If we ever have an API with nested properties of the same name more than
	// 2 deep, we'll need to have this return a list of PropertyInfo and comb through them all.
	PropertyInfo GetParentTypeWithSameNamedProperty (BaseTypeAttribute bta, string propertyName)
	{
		if (bta == null)
			return null;

		Type currentType = bta.BaseType;
		while (currentType != null && currentType != TypeManager.NSObject)
		{
			PropertyInfo prop = currentType.GetProperty (propertyName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			if (prop != null)
				return prop;
			currentType = currentType.BaseType;
		}
		return null;
	}

	static string Capitalize (string str)
	{
		if (str.StartsWith ("@", StringComparison.Ordinal))
			return char.ToUpper (str[1]) + str.Substring (2);
	
		return char.ToUpper (str[0]) + str.Substring (1);
	}

	string GetNotificationCenter (PropertyInfo pi)
	{
		var a = AttributeManager.GetCustomAttributes<NotificationAttribute> (pi);
		var str =  a [0].NotificationCenter;
		if (str == null)
			str = "NSNotificationCenter.DefaultCenter";
		return str;
	}
		
	string GetNotificationName (PropertyInfo pi)
	{
		// TODO: fetch the NotificationAttribute, see if there is an override there.
		var name = pi.Name;
		if (name.EndsWith ("Notification", StringComparison.Ordinal))
			return name.Substring (0, name.Length-"Notification".Length);
		return name;
	}

	Type GetNotificationArgType (PropertyInfo pi)
	{
		return AttributeManager.GetCustomAttributes<NotificationAttribute> (pi) [0].Type;
	}
	
	//
	// Support for the automatic delegate/event generation
	//
	string RenderParameterDecl (IEnumerable<ParameterInfo> pi)
	{
		return RenderParameterDecl (pi, false);
	}
	
	string RenderParameterDecl (IEnumerable<ParameterInfo> pi, bool removeRefTypes)
	{
		return String.Join (", ", pi.Select (p=>RenderSingleParameter(p, removeRefTypes) + " " + p.Name.GetSafeParamName ()).ToArray ());
	}

	string RenderSingleParameter (ParameterInfo p, bool removeRefTypes)
	{
		var protocolized = Protocolize (p);
		var prefix = protocolized ? "I" : "";

		if (p.ParameterType.IsByRef)
			return (removeRefTypes ? "" : (p.IsOut ? "out " : "ref ")) + prefix + RenderType (p.ParameterType.GetElementType ());
		else
			return prefix + RenderType (p.ParameterType);
						     
	}

	string GetPublicParameterName (ParameterInfo pi)
	{
		var attrs = AttributeManager.GetCustomAttributes<EventNameAttribute> (pi);
		if (attrs.Length == 0)
			return CamelCase (pi.Name).GetSafeParamName ();

		var a = attrs [0];
		return CamelCase (a.EvtName).GetSafeParamName ();
	}
	
	string RenderArgs (IEnumerable<ParameterInfo> pi)
	{
		return RenderArgs (pi, false);
	}
	
	string RenderArgs (IEnumerable<ParameterInfo> pi, bool removeRefTypes)
	{
		return String.Join (", ", pi.Select (p => (p.ParameterType.IsByRef ? (removeRefTypes ? "" : (p.IsOut ? "out " : "ref ")) : "")+ p.Name.GetSafeParamName ()).ToArray ());
	}

	bool MustPullValuesBack (IEnumerable<ParameterInfo> parameters)
	{
		return parameters.Any (pi => pi.ParameterType.IsByRef);
	}
	
	string CamelCase (string ins)
	{
		return Char.ToUpper (ins [0]) + ins.Substring (1);
	}

	string PascalCase (string ins)
	{
		return Char.ToLower (ins [0]) + ins.Substring (1);
	}

	Dictionary<string,bool> skipGeneration = new Dictionary<string,bool> ();
	string GetEventName (MethodInfo mi)
	{
		var a = AttributeManager.GetCustomAttribute<EventNameAttribute> (mi);
		if (a == null)
			return mi.Name;
		var ea = (EventNameAttribute) a;
		
		return ea.EvtName;
	}

	HashSet<string> repeatedDelegateApiNames = new HashSet<string> ();
	string GetDelegateApiName (MethodInfo mi)
	{
		var a = AttributeManager.GetCustomAttribute<DelegateApiNameAttribute> (mi);

		if (repeatedDelegateApiNames.Contains (mi.Name) && a == null)
			throw new BindingException (1043, true, $"Repeated overload {mi.Name} and no [DelegateApiNameAttribute] provided to generate property name on host class.");
		if (a == null) {
			repeatedDelegateApiNames.Add (mi.Name);
			return mi.Name;
		}

		var apiName = (DelegateApiNameAttribute) a;
		if (repeatedDelegateApiNames.Contains (apiName.Name))
			throw new BindingException (1044, true, $"Repeated name '{apiName.Name}' provided in [DelegateApiNameAttribute]");

		return apiName.Name;
	}

	string GetEventArgName (MethodInfo mi)
	{
		if (mi.GetParameters ().Length == 1)
			return "EventArgs";
		
		var a = AttributeManager.GetCustomAttribute<EventArgsAttribute> (mi);
		if (a == null)
			throw new BindingException (1004, true, "The delegate method {0}.{1} is missing the [EventArgs] attribute (has {2} parameters)", mi.DeclaringType.FullName, mi.Name, mi.GetParameters ().Length);

		var ea = (EventArgsAttribute) a;
		if (ea.ArgName.EndsWith ("EventArgs", StringComparison.Ordinal))
			throw new BindingException (1005, true, "EventArgs in {0}.{1} attribute should not include the text `EventArgs' at the end", mi.DeclaringType.FullName, mi.Name);
		
		if (ea.SkipGeneration){
			skipGeneration [ea.FullName ? ea.ArgName : ea.ArgName + "EventArgs"] = true;
		}
		
		if (ea.FullName)
			return ea.ArgName;

		return ea.ArgName + "EventArgs";
	}

	string GetDelegateName (MethodInfo mi)
	{
		Attribute a = AttributeManager.GetCustomAttribute<DelegateNameAttribute> (mi);
		if (a != null)
			return ((DelegateNameAttribute) a).Name;

		a = AttributeManager.GetCustomAttribute<EventArgsAttribute> (mi);
		if (a == null)
			throw new BindingException (1006, true, "The delegate method {0}.{1} is missing the [DelegateName] attribute (or EventArgs)", mi.DeclaringType.FullName, mi.Name);

		ErrorHelper.Show (new BindingException (1102, false, "Using the deprecated 'EventArgs' for a delegate signature in {0}.{1}, please use 'DelegateName' instead.", mi.DeclaringType.FullName, mi.Name));
		return ((EventArgsAttribute) a).ArgName;
	}
	
	object GetDefaultValue (MethodInfo mi)
	{
		Attribute a = AttributeManager.GetCustomAttribute<DefaultValueAttribute> (mi);
		if (a == null){
			a = AttributeManager.GetCustomAttribute<DefaultValueFromArgumentAttribute> (mi);
			if (a != null){
				var fvfa = (DefaultValueFromArgumentAttribute) a;
				return fvfa.Argument;
			}
			
			throw new BindingException (1016, true, "The delegate method {0}.{1} is missing the [DefaultValue] attribute", mi.DeclaringType.FullName, mi.Name);
		}
		var def = ((DefaultValueAttribute) a).Default;
		if (def == null)
			return "null";

		var type = def as Type;
		if (type != null && (
			type.FullName == "System.Drawing.PointF" ||
			type.FullName == "System.Drawing.SizeF" ||
			type.FullName == "System.Drawing.RectangleF" ||
			type.FullName == ns.Get ("CoreGraphics.CGPoint") ||
			type.FullName == ns.Get ("CoreGraphics.CGSize") ||
			type.FullName == ns.Get ("CoreGraphics.CGRect")))
			return type.FullName + ".Empty";

		if (def is bool)
			return (bool) def ? "true" : "false";

		if (mi.ReturnType.IsEnum) {
			if (def is string)
				return def;
			var name = mi.ReturnType.GetEnumName (def);
			if (string.IsNullOrEmpty (name)) {
				return "(" + mi.ReturnType.FullName + ") " + def;
			} else {
				return mi.ReturnType.FullName + "." + name;
			}
		}

		return def;
	}
	
	string RenderType (Type t)
	{
		t = GetCorrectGenericType (t);

		if (!t.IsEnum){
			switch (Type.GetTypeCode (t)){
			case TypeCode.Char:
				return "char";
			case TypeCode.String:
				return "string";
			case TypeCode.Int32:
				return "int";
			case TypeCode.UInt32:
				return "uint";
			case TypeCode.Int64:
				return "long";
			case TypeCode.UInt64:
				return "ulong";
			case TypeCode.Single:
				return "float";
			case TypeCode.Double:
				return "double";
			case TypeCode.Decimal:
				return "decimal";
			case TypeCode.SByte:
				return "sbyte";
			case TypeCode.Byte:
				return "byte";
			case TypeCode.Boolean:
				return "bool";
			}
		}
		
		if (t == TypeManager.System_Void)
			return "void";

		string ns = t.Namespace;
		if (NamespaceManager.ImplicitNamespaces.Contains (ns) || t.IsGenericType) {
			var targs = t.GetGenericArguments ();
			if (targs.Length == 0)
				return t.Name;
			return $"global::{t.Namespace}." + RemoveArity (t.Name) + "<" + string.Join (", ", targs.Select (l => FormatTypeUsedIn (null, l)).ToArray ()) + ">";
		}
		if (NamespaceManager.NamespacesThatConflictWithTypes.Contains (NamespaceManager.Get(ns)))
			return "global::" + t.FullName;
		if (t.Name == t.Namespace)
			return "global::" + t.FullName;
		else
			return t.FullName;
		
	}

	public static string Quote (string s)
	{
		if (s == null)
			return String.Empty;
		if (s == string.Empty)
			return @"""""";

		return $"@\"{s.Replace ("\"", "\"\"")}\"";
	}

	// Format a provider for error messages
	public static string FormatProvider (ICustomAttributeProvider provider)
	{
		if (provider is Type type) {
			return type.FullName;
		} else if (provider is MemberInfo mi) {
			return mi.DeclaringType.FullName + "." + mi.Name;
		} else {
			return provider?.ToString ();
		}
	}
}
