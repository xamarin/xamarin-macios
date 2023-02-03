using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

//
// This class is used to generate a graph of the type hierarchy of the
// generated types and required by the UIApperance support to determine
// which types need to have Appearance methods created
//
public class GeneratedType {
	public GeneratedType (Type t, GeneratedTypes root)
	{
		Root = root;
		Type = t;
		var generator = root.Generator;
		foreach (var iface in Type.GetInterfaces ()) {
			if (iface.Name == "UIAppearance" || iface.Name == "IUIAppearance")
				ImplementsAppearance = true;
		}
		var btype = ReflectionExtensions.GetBaseType (Type, generator);
		if (btype != generator.TypeManager.System_Object) {
			Parent = btype;
			// protected against a StackOverflowException - bug #19751
			// it does not protect against large cycles (but good against copy/paste errors)
			if (Parent == Type)
				throw new BindingException (1030, true, Type, Parent);
			ParentGenerated = Root.Lookup (Parent);

			// If our parent had UIAppearance, we flag this class as well
			if (ParentGenerated.ImplementsAppearance)
				ImplementsAppearance = true;
			ParentGenerated.Children.Add (this);
		}

		if (generator.AttributeManager.HasAttribute<CategoryAttribute> (t))
			ImplementsAppearance = false;
	}
	public GeneratedTypes Root;
	public Type Type;
	public List<GeneratedType> Children = new (1);
	public Type? Parent;
	public GeneratedType? ParentGenerated;
	public bool ImplementsAppearance;

	List<MemberInfo>? appearance_selectors;

	public List<MemberInfo> AppearanceSelectors {
		get { return appearance_selectors ??= new (); }
	}
}
