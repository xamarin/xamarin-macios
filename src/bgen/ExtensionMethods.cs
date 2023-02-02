using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Foundation;
using ObjCRuntime;

#nullable enable

public static class GeneratorExtensions {

	public static StreamWriter Write (this StreamWriter sw, char c, int count)
	{
		for (int i = 0; i < count; i++)
			sw.Write (c);
		return sw;
	}

}

public static class ReflectionExtensions {
	public static BaseTypeAttribute? GetBaseTypeAttribute (Type type, Generator generator)
	{
		return generator.AttributeManager.GetCustomAttribute<BaseTypeAttribute> (type);
	}

	public static Type GetBaseType (Type type, Generator generator)
	{
		var bta = GetBaseTypeAttribute (type, generator);
		var baseType = bta is not null ? bta.BaseType : generator.TypeManager.System_Object;

		return baseType;
	}

	public static List<PropertyInfo> GatherProperties (this Type type, Generator generator)
	{
		return type.GatherProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, generator);
	}

	//
	// Returns true if the specified method info or property info is not
	// available in the current platform (because it has the attribute
	// [Unavailable (ThisPlatform) or because the shorthand versions
	// of [NoiOS] or [NoMac] are applied.
	//
	// This needs to merge, because we might have multiple attributes in
	// use, for example, the availability (iOS (7,0)) and the fact that this
	// is not available on Mac (NoMac).
	//
	public static bool IsUnavailable (this ICustomAttributeProvider provider, Generator generator)
	{
		var attributes = generator.AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (provider);
		var platform = generator.CurrentPlatform;
		return IsUnavailable (attributes, platform);
	}

	public static bool IsUnavailable (AvailabilityBaseAttribute [] attributes, PlatformName platform)
	{
		if (attributes.Any (attr => attr.AvailabilityKind == AvailabilityKind.Unavailable && attr.Platform == platform))
			return true;

		if (platform == PlatformName.MacCatalyst) {
			// If we're targetting Mac Catalyst, and we don't have any availability information for Mac Catalyst,
			// then use the availability for iOS
			var anyCatalyst = attributes.Any (v => v.Platform == PlatformName.MacCatalyst);
			if (!anyCatalyst)
				return IsUnavailable (attributes, PlatformName.iOS);
		}

		return false;
	}

	public static AvailabilityBaseAttribute? GetAvailability (this ICustomAttributeProvider attrProvider, AvailabilityKind availabilityKind, Generator generator)
	{
		return generator.AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (attrProvider)
			.FirstOrDefault (attr =>
				attr.AvailabilityKind == availabilityKind &&
					attr.Platform == generator.CurrentPlatform
			);
	}

	public static List<PropertyInfo> GatherProperties (this Type type, BindingFlags flags, Generator generator)
	{
		List<PropertyInfo> properties = new List<PropertyInfo> (type.GetProperties (flags));

		if (generator.IsPublicMode)
			return properties;

		Type parentType = GetBaseType (type, generator);
		if (parentType != generator.TypeManager.NSObject) {
			if (generator.AttributeManager.HasAttribute<ModelAttribute> (parentType)) {
				foreach (PropertyInfo pinfo in parentType.GetProperties (flags)) {
					bool toadd = true;
					var modelea = generator.GetExportAttribute (pinfo, out _);

					if (modelea == null)
						continue;

					foreach (PropertyInfo exists in properties) {
						var origea = generator.GetExportAttribute (exists, out _);
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

	public static List<MethodInfo> GatherMethods (this Type type, Generator generator)
	{
		return type.GatherMethods (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, generator);
	}

	public static bool IsInternal (this MemberInfo mi, Generator generator)
	{
		return generator.AttributeManager.HasAttribute<InternalAttribute> (mi)
			|| (generator.AttributeManager.HasAttribute<UnifiedInternalAttribute> (mi));
	}

	public static bool IsUnifiedInternal (this MemberInfo mi, Generator generator)
	{
		return (generator.AttributeManager.HasAttribute<UnifiedInternalAttribute> (mi));
	}

	public static bool IsInternal (this PropertyInfo pi, Generator generator)
	{
		return generator.AttributeManager.HasAttribute<InternalAttribute> (pi)
			|| (generator.AttributeManager.HasAttribute<UnifiedInternalAttribute> (pi));
	}

	public static bool IsInternal (this Type type, Generator generator)
	{
		return generator.AttributeManager.HasAttribute<InternalAttribute> (type)
			|| (generator.AttributeManager.HasAttribute<UnifiedInternalAttribute> (type));
	}

	public static List<MethodInfo> GatherMethods (this Type type, BindingFlags flags, Generator generator)
	{
		List<MethodInfo> methods = new List<MethodInfo> (type.GetMethods (flags));

		if (generator.IsPublicMode)
			return methods;

		Type parentType = GetBaseType (type, generator);

		if (parentType != generator.TypeManager.NSObject) {
			if (generator.AttributeManager.HasAttribute<ModelAttribute> (parentType))
				foreach (MethodInfo minfo in parentType.GetMethods ())
					if (generator.AttributeManager.HasAttribute<ExportAttribute> (minfo))
						methods.Add (minfo);
		}

		return methods;
	}
}

// Fixes bug 27430 - btouch doesn't escape identifiers with the same name as C# keywords
public static class StringExtensions {
	public static string? GetSafeParamName (this string? paramName)
	{
		if (paramName is null)
			return paramName;

		if (!IsValidIdentifier (paramName, out var hasIllegalChars)) {
			return hasIllegalChars ? null : "@" + paramName;
		}
		return paramName;
	}

	// Since we're building against the iOS assemblies and there's no code generation there,
	// I'm bringing the implementation from:
	// mono/mcs/class//System/Microsoft.CSharp/CSharpCodeGenerator.cs
	static bool IsValidIdentifier (string? identifier, out bool hasIllegalChars)
	{
		hasIllegalChars = false;
		if (identifier is null || identifier.Length == 0)
			return false;

		if (keywordsTable is null)
			FillKeywordTable ();

		if (keywordsTable!.Contains (identifier))
			return false;

		if (!is_identifier_start_character (identifier [0])) {
			// if we are dealing with a number, we are ok, we can prepend @, else we have a problem
			hasIllegalChars = !Char.IsNumber (identifier [0]);
			return false;
		}

		for (int i = 1; i < identifier.Length; i++)
			if (!is_identifier_part_character (identifier [i])) {
				hasIllegalChars = true;
				return false;
			}

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
			if (keywordsTable is null) {
				keywordsTable = new Hashtable ();
				foreach (string keyword in keywords) {
					keywordsTable.Add (keyword, keyword);
				}
			}
		}
	}

	static Hashtable? keywordsTable;

	static string [] keywords = new string [] {
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
