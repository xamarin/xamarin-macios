using System;
using System.Reflection;

#nullable enable

public static class CustomAttributeDataExtensions {
#if !NET
	static readonly Type roCustomAttributeDataType;
	static readonly PropertyInfo attributeTypeProperty;

	static CustomAttributeDataExtensions ()
	{
		roCustomAttributeDataType = typeof (MetadataLoadContext).Assembly.GetType ("System.Reflection.TypeLoading.RoCustomAttributeData");
		attributeTypeProperty = roCustomAttributeDataType.GetProperty ("AttributeType")
								?? throw new InvalidOperationException ("Could not retrieve property 'AttributeType'.");
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
