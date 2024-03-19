using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

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

	public static void LoadWithoutNetworkAccess (this XmlDocument doc, string filename)
	{
		using (var fs = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
			var settings = new XmlReaderSettings () {
				XmlResolver = null,
				DtdProcessing = DtdProcessing.Parse,
			};
			using (var reader = XmlReader.Create (fs, settings)) {
				doc.Load (reader);
			}
		}
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
		var baseType = bta is not null ? bta.BaseType : generator.TypeCache.System_Object;

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
		if (parentType != generator.TypeCache.NSObject) {
			if (generator.AttributeManager.HasAttribute<ModelAttribute> (parentType)) {
				foreach (PropertyInfo pinfo in parentType.GetProperties (flags)) {
					bool toadd = true;
					var modelea = generator.GetExportAttribute (pinfo, out _);

					if (modelea is null)
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
		return generator.AttributeManager.HasAttribute<InternalAttribute> (mi);
	}

	public static bool IsInternal (this PropertyInfo pi, Generator generator)
	{
		return generator.AttributeManager.HasAttribute<InternalAttribute> (pi);
	}

	public static bool IsInternal (this Type type, Generator generator)
	{
		return generator.AttributeManager.HasAttribute<InternalAttribute> (type);
	}

	public static List<MethodInfo> GatherMethods (this Type type, BindingFlags flags, Generator generator)
	{
		List<MethodInfo> methods = new List<MethodInfo> (type.GetMethods (flags));

		if (generator.IsPublicMode)
			return methods;

		Type parentType = GetBaseType (type, generator);

		if (parentType != generator.TypeCache.NSObject) {
			if (generator.AttributeManager.HasAttribute<ModelAttribute> (parentType))
				foreach (MethodInfo minfo in parentType.GetMethods ())
					if (generator.AttributeManager.HasAttribute<ExportAttribute> (minfo))
						methods.Add (minfo);
		}

		return methods;
	}
}
