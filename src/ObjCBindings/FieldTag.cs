using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// The exported constant/field is a class/interface property field.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum Field {
		/// <summary>
		/// Use the default values.
		/// </summary>
		None = 0,
	}

	/// <summary>
	/// Field flag that states that the field is used as a Enum value.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum EnumValue {
		/// <summary>
		/// Use the default values.
		/// </summary>
		None = 0,
	}
}
