using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Flags to be used on class bindings.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum Class : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Use to let the generator know that the default constructor should not be generated.
		/// </summary>
		DisableDefaultCtor = 1 << 2,
	}

	/// <summary>
	/// Flags to be used on protocol bindings.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum Protocol : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,
	}

	/// <summary>
	/// Flags to be used on protocol bindings.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum Category : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,
	}
}
