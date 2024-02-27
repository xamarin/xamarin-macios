using System.Reflection;

#nullable enable

class WrapPropMemberInformation {
	public bool HasWrapOnGetter { get => WrapGetter is not null; }
	public bool HasWrapOnSetter { get => WrapSetter is not null; }
	public string? WrapGetter { get; private set; }
	public string? WrapSetter { get; private set; }

	public WrapPropMemberInformation (PropertyInfo pi, Generator generator)
	{
		WrapGetter = generator.AttributeManager.GetCustomAttribute<WrapAttribute> (pi.GetMethod)?.MethodName;
		WrapSetter = generator.AttributeManager.GetCustomAttribute<WrapAttribute> (pi.SetMethod)?.MethodName;
	}
}
