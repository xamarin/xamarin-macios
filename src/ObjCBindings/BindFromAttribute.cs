using System;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using ObjCRuntime;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Attribute to bind from a specific type.
	/// </summary>
	[Experimental ("APL0003")]
	[AttributeUsage (AttributeTargets.ReturnValue | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public class BindFromAttribute : Attribute {

		/// <summary>
		/// Initializes a new instance of the <see cref="BindFromAttribute"/> class.
		/// </summary>
		public BindFromAttribute (Type type)
		{
			Type = type;
		}

		/// <summary>
		/// The type to bind from.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// The original type that was bound from.
		/// </summary>
		public Type? OriginalType { get; set; } = null;
	}
}
