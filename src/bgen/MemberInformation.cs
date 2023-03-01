using System;
using System.Reflection;
using Foundation;

#nullable enable

public class MemberInformation {
	Generator Generator;
	AttributeManager AttributeManager { get { return Generator.AttributeManager; } }
	public readonly MemberInfo mi;
	public readonly Type type;
	public readonly Type? category_extension_type;
	internal readonly WrapPropMemberInformation? wpmi;
	public readonly bool is_abstract;
	public readonly bool is_protected;
	public readonly bool is_internal;
	public readonly bool is_unified_internal;
	public readonly bool is_override;
	public readonly bool is_new;
	public readonly bool is_sealed;
	public readonly bool is_static;
	public readonly bool is_thread_static;
	public readonly bool is_autorelease;
	public readonly bool is_wrapper;
	public readonly bool is_forced;
	public readonly bool ignore_category_static_warnings;
	public readonly bool is_basewrapper_protocol_method;
	public readonly bool has_inner_wrap_attribute;
	public readonly ThreadCheck threadCheck;
	public bool is_unsafe;
	public bool is_virtual_method;
	public bool is_export;
	public bool is_category_extension;
	public bool is_variadic;
	public bool is_interface_impl;
	public bool is_extension_method;
	public bool is_appearance;
	public bool is_model;
	public bool is_ctor;
	public bool is_return_release;
	public bool is_type_sealed;
	public bool protocolize;
	public string? selector;
	public string? wrap_method;
	public string is_forced_owns;
	public bool is_bindAs => Generator.HasBindAsAttribute (mi);

	public MethodInfo? Method { get { return mi as MethodInfo; } }
	public PropertyInfo? Property { get { return mi as PropertyInfo; } }

	MemberInformation (Generator generator, IMemberGatherer gather, MemberInfo mi, Type type, bool is_interface_impl, bool is_extension_method, bool is_appearance, bool is_model)
	{
		Generator = generator;
		var methodInfo = mi as MethodInfo;

		is_ctor = mi is MethodInfo && mi.Name == "Constructor";
		is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (mi) && mi.DeclaringType == type;
		is_protected = AttributeManager.HasAttribute<ProtectedAttribute> (mi);
		is_internal = mi.IsInternal (generator);
		is_unified_internal = AttributeManager.HasAttribute<UnifiedInternalAttribute> (mi);
		is_override = AttributeManager.HasAttribute<OverrideAttribute> (mi) || !Generator.MemberBelongsToType (mi.DeclaringType, type);
		is_new = AttributeManager.HasAttribute<NewAttribute> (mi);
		is_sealed = AttributeManager.HasAttribute<SealedAttribute> (mi);
		is_static = AttributeManager.HasAttribute<StaticAttribute> (mi);
		is_thread_static = AttributeManager.HasAttribute<IsThreadStaticAttribute> (mi);
		is_autorelease = AttributeManager.HasAttribute<AutoreleaseAttribute> (mi);
		is_wrapper = !AttributeManager.HasAttribute<SyntheticAttribute> (mi.DeclaringType);
		is_type_sealed = AttributeManager.HasAttribute<SealedAttribute> (mi.DeclaringType);
		is_return_release = methodInfo is not null && AttributeManager.HasAttribute<ReleaseAttribute> (methodInfo.ReturnParameter);
		is_forced = Generator.HasForcedAttribute (mi, out is_forced_owns);

		var tsa = AttributeManager.GetCustomAttribute<ThreadSafeAttribute> (mi);
		// if there's an attribute then it overrides the parent (e.g. type attribute) or namespace default
		if (tsa is not null) {
			threadCheck = tsa.Safe ? ThreadCheck.Off : ThreadCheck.On;
		} else {
			threadCheck = ThreadCheck.Default; // will be based on the type decision
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
		if (mi.DeclaringType != type) {
			for (var baseType = ReflectionExtensions.GetBaseType (type, generator); baseType != Generator.TypeManager.System_Object; baseType = ReflectionExtensions.GetBaseType (baseType, generator)) {
				foreach (var baseMethod in gather.GetTypeContractMethods (baseType)) {
					if (baseMethod.DeclaringType != baseType && baseMethod == mi) {
						// We found a case, we need to flag it as new.
						is_new = true;
					}
				}
			}
		}

	}

	public MemberInformation (Generator generator, IMemberGatherer gather, MethodInfo mi, Type type,
		Type? categoryExtensionType, bool isInterfaceImpl = false, bool isExtensionMethod = false,
		bool isAppearance = false, bool isModel = false, string? selector = null,
		bool isBaseWrapperProtocolMethod = false)
		: this (generator, gather, mi, type, isInterfaceImpl, isExtensionMethod, isAppearance, isModel)
	{
		is_basewrapper_protocol_method = isBaseWrapperProtocolMethod;
		foreach (ParameterInfo pi in mi.GetParameters ())
			if (pi.ParameterType.IsSubclassOf (Generator.TypeManager.System_Delegate))
				is_unsafe = true;

		if (!is_unsafe && mi.ReturnType.IsSubclassOf (Generator.TypeManager.System_Delegate))
			is_unsafe = true;

		if (selector is not null) {
			this.selector = selector;
			if (!is_sealed && !is_wrapper) {
				is_export = !isExtensionMethod;
				is_virtual_method = !is_ctor;
			}
		} else {
			object [] attr = AttributeManager.GetCustomAttributes<ExportAttribute> (mi);
			if (attr.Length != 1) {
				attr = AttributeManager.GetCustomAttributes<BindAttribute> (mi);
				if (attr.Length != 1) {
					attr = AttributeManager.GetCustomAttributes<WrapAttribute> (mi);
					if (attr.Length != 1)
						throw new BindingException (1012, true, type, mi.Name);

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
					is_export = !isExtensionMethod;
				}
			}
		}

		this.category_extension_type = categoryExtensionType;
		if (categoryExtensionType is not null) {
			is_category_extension = true;
#if NET
			ignore_category_static_warnings = is_internal || type.IsInternal (generator);
#else
			ignore_category_static_warnings = is_internal || type.IsInternal (generator) || Generator.AttributeManager.GetCustomAttribute<CategoryAttribute> (type).AllowStaticMembers;
#endif
		}

		if (is_static || is_category_extension || isInterfaceImpl || isExtensionMethod || is_type_sealed)
			is_virtual_method = false;
	}

	public MemberInformation (Generator generator, IMemberGatherer gather, PropertyInfo pi, Type type, bool is_interface_impl = false)
		: this (generator, gather, pi, type, is_interface_impl, false, false, false)
	{
		if (pi.PropertyType.IsSubclassOf (Generator.TypeManager.System_Delegate))
			is_unsafe = true;

		var export = Generator.GetExportAttribute (pi, out wrap_method);
		if (export is not null)
			selector = export.Selector;

		if (wrap_method is not null) {
			var wrapAtt = Generator.AttributeManager.GetCustomAttribute<WrapAttribute> (pi);
			is_virtual_method = wrapAtt?.IsVirtual ?? false;
		} else if (is_interface_impl || is_type_sealed)
			is_virtual_method = false;
		else
			is_virtual_method = !is_static;

		// Properties can have WrapAttribute on getter/setter so we need to check for this
		// but only if no Export is already found on property level.
		if (export is null) {
			wpmi = new WrapPropMemberInformation (pi, generator);
			has_inner_wrap_attribute = wpmi.HasWrapOnGetter || wpmi.HasWrapOnSetter;

			// Wrap can only be used either at property level or getter/setter level at a given time.
			if (wrap_method is not null && has_inner_wrap_attribute)
				throw new BindingException (1063, true, pi.DeclaringType, pi.Name);
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
#if NET
			mods += "virtual ";
#else
			mods += "abstract ";
#endif
		} else if (is_virtual_method && !is_type_sealed) {
			mods += is_override ? "override " : "virtual ";
		}

		return mods;
	}
}
