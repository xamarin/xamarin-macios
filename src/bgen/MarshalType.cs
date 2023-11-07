using System;

#nullable enable

public class MarshalType {
	public Type Type { get; }
	public string Encoding { get; }
	public string ParameterMarshal { get; }
	public string CreateFromRet { get; }
	public string? ClosingCreate { get; }

	public MarshalType (Type t, string? encode = null, string? fetch = null, string? create = null, string? closingCreate = ")")
	{
		Type = t;
		Encoding = encode ?? Generator.NativeHandleType;
		ParameterMarshal = fetch ?? "{0}.Handle";
		if (create is null) {
			CreateFromRet = $"Runtime.GetINativeObject<global::{t.FullName}> (";
			ClosingCreate = ", false)!";
		} else {
			CreateFromRet = create;
			ClosingCreate = closingCreate;
		}
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
		return new (type);
	}
}
