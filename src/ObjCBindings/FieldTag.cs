using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Base class use to flag a FieldAttribute usage. Each FieldAttribute must have a flag attached to it so that the 
	/// binding generator analyzer can verify the binding definition.
	/// </summary>
	[Experimental ("APL0003")]
	public abstract class FieldTag { }

	/// <summary>
	/// Field flag that states that the field is used as a Enum value.
	/// </summary>
	[Experimental ("APL0003")]
	public sealed class EnumValue : FieldTag { }
}
