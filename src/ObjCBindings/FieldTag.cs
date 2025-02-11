using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Field flag that states that the field is used as a Enum value.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum EnumValue {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,
	}
}
