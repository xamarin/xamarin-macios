using System;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace ObjCBindings {

	/// <summary>
	/// Attribute that indicates that a class or enum is a binding type. This attribute is used by the binding generator
	/// to generate all the necessary code for the binding. The attribute can be used in a class or enum and it is required.
	///
	/// If the attribute is used in a class, the class must be partial otherwise the generator will fail.
	/// </summary>
	[Experimental ("APL0003")]
	[AttributeUsage (AttributeTargets.Class | System.AttributeTargets.Enum, AllowMultiple = false)]
	public class BindingTypeAttribute : Attribute {

		/// <summary>
		/// Indicates the name of the binding type. This is the name that will be used by the registrar to make the
		/// class available in the ObjC runtime. The default value is string.Empty, in that case the generator
		/// will use the name of the C# class.
		/// </summary>
		public string Name { get; set; } = string.Empty;
	}

}
