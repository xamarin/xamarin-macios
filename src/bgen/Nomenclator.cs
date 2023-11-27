using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

// Noun. nomenclator (plural nomenclators) An assistant who specializes in providing timely and spatially relevant
// reminders of the names of persons and other socially important information
public class Nomenclator {
	readonly Dictionary<string, bool> skipGeneration = new ();
	readonly HashSet<string> repeatedDelegateApiNames = new ();
	readonly Dictionary<Type, int> trampolinesGenericVersions = new ();

	readonly AttributeManager attributeManager;

	public Nomenclator (AttributeManager attributeManager)
	{
		this.attributeManager = attributeManager;
	}

	public string GetDelegateName (MethodInfo mi)
	{
		Attribute a = attributeManager.GetCustomAttribute<DelegateNameAttribute> (mi);
		if (a is not null)
			return ((DelegateNameAttribute) a).Name;

		a = attributeManager.GetCustomAttribute<EventArgsAttribute> (mi);
		if (a is null)
			throw new BindingException (1006, true, mi.DeclaringType!.FullName, mi.Name);

		ErrorHelper.Warning (1102, mi.DeclaringType!.FullName, mi.Name);
		return ((EventArgsAttribute) a).ArgName;
	}

	public string GetEventName (MethodInfo mi)
	{
		var a = attributeManager.GetCustomAttribute<EventNameAttribute> (mi);
		return a is null ? mi.Name : a.EvtName;
	}

	public string GetDelegateApiName (MethodInfo mi)
	{
		var apiName = attributeManager.GetCustomAttribute<DelegateApiNameAttribute> (mi);

		if (repeatedDelegateApiNames.Contains (mi.Name) && apiName is null)
			throw new BindingException (1043, true, mi.Name);
		if (apiName is null) {
			repeatedDelegateApiNames.Add (mi.Name);
			return mi.Name;
		}

		if (repeatedDelegateApiNames.Contains (apiName.Name))
			throw new BindingException (1044, true, apiName.Name);

		return apiName.Name;
	}

	public string GetEventArgName (MethodInfo mi)
	{
		if (mi.GetParameters ().Length == 1)
			return "EventArgs";

		var a = attributeManager.GetCustomAttribute<EventArgsAttribute> (mi);
		if (a is null)
			throw new BindingException (1004, true, mi.DeclaringType!.FullName, mi.Name, mi.GetParameters ().Length);

		var ea = (EventArgsAttribute) a;
		if (ea.ArgName.EndsWith ("EventArgs", StringComparison.Ordinal))
			throw new BindingException (1005, true, mi.DeclaringType!.FullName, mi.Name);

		if (ea.SkipGeneration) {
			skipGeneration [ea.FullName ? ea.ArgName : ea.ArgName + "EventArgs"] = true;
		}

		if (ea.FullName)
			return ea.ArgName;

		return ea.ArgName + "EventArgs";
	}

	public bool WasEventArgGenerated (string eaclass)
		=> skipGeneration.ContainsKey (eaclass);

	public string GetTrampolineName (Type t)
	{
		var trampolineName = t.Name.Replace ("`", "Arity");
		if (t.IsGenericType) {
			var gdef = t.GetGenericTypeDefinition ();

			if (!trampolinesGenericVersions.ContainsKey (gdef))
				trampolinesGenericVersions.Add (gdef, 0);

			trampolineName = trampolineName + "V" + trampolinesGenericVersions [gdef]++;
		}
		return trampolineName;
	}

	public void ForgetDelegateApiNames ()
		=> repeatedDelegateApiNames.Clear ();
	
	public string GetGeneratedTypeName (Type type)
	{
		var bindOnType = attributeManager.GetCustomAttributes<BindAttribute> (type);
		if (bindOnType.Length > 0)
			return bindOnType [0].Selector;
		if (type.IsGenericTypeDefinition)
			return type.Name.Substring (0, type.Name.IndexOf ('`'));
		return type.Name;
	}
}
