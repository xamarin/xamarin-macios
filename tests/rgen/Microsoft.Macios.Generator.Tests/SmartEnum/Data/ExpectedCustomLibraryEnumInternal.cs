// <auto-generated />

#nullable enable

using Foundation;
using ObjCBindings;
using ObjCRuntime;
using System;
using System.Runtime.Versioning;

namespace CustomLibrary;

[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
static public partial class CustomLibraryEnumInternalExtensions
{

	static IntPtr[] values = new IntPtr [3];

	[Field ("None", "__Internal")]
	internal unsafe static IntPtr None
	{
		get
		{
			fixed (IntPtr *storage = &values [0])
				return Dlfcn.CachePointer (Libraries.__Internal.Handle, "None", storage);
		}
	}

	[Field ("Medium", "__Internal")]
	internal unsafe static IntPtr Medium
	{
		get
		{
			fixed (IntPtr *storage = &values [1])
				return Dlfcn.CachePointer (Libraries.__Internal.Handle, "Medium", storage);
		}
	}

	[Field ("High", "__Internal")]
	internal unsafe static IntPtr High
	{
		get
		{
			fixed (IntPtr *storage = &values [2])
				return Dlfcn.CachePointer (Libraries.__Internal.Handle, "High", storage);
		}
	}

	public static NSString? GetConstant (this CustomLibraryEnumInternal self)
	{
		IntPtr ptr = IntPtr.Zero;
		switch ((int) self)
		{
			case 0: // None
				ptr = None;
				break;
			case 1: // Medium
				ptr = Medium;
				break;
			case 2: // High
				ptr = High;
				break;
		}
		return (NSString?) Runtime.GetNSObject (ptr);
	}

	public static CustomLibraryEnumInternal GetValue (NSString constant)
	{
		if (constant is null)
			throw new ArgumentNullException (nameof (constant));
		if (constant.IsEqualTo (None))
			return CustomLibraryEnumInternal.None;
		if (constant.IsEqualTo (Medium))
			return CustomLibraryEnumInternal.Medium;
		if (constant.IsEqualTo (High))
			return CustomLibraryEnumInternal.High;
		throw new NotSupportedException ($"The constant {constant} has no associated enum value on this platform.");
	}

	internal static NSString?[]? ToConstantArray (this CustomLibraryEnumInternal[]? values)
	{
		if (values is null)
			return null;
		var rv = new global::System.Collections.Generic.List<NSString?> ();
		for (var i = 0; i < values.Length; i++) {
			var value = values [i];
			rv.Add (value.GetConstant ());
		}
		return rv.ToArray ();
	}

	internal static CustomLibraryEnumInternal[]? ToEnumArray (this NSString[]? values)
	{
		if (values is null)
			return null;
		var rv = new global::System.Collections.Generic.List<CustomLibraryEnumInternal> ();
		for (var i = 0; i < values.Length; i++) {
			var value = values [i];
			rv.Add (GetValue (value));
		}
		return rv.ToArray ();
	}
}