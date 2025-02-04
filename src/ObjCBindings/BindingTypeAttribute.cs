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
	[AttributeUsage (System.AttributeTargets.Enum, AllowMultiple = false)]
	public class BindingTypeAttribute : Attribute {

		/// <summary>
		/// Indicates the name of the binding type. This is the name that will be used by the registrar to make the
		/// class available in the ObjC runtime. The default value is string.Empty, in that case the generator
		/// will use the name of the C# class.
		/// </summary>
		public string Name { get; set; } = string.Empty;
	}

	[Experimental ("APL0003")]
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	public class BindingTypeAttribute<T> : Attribute where T : Enum {

		/// <summary>
		/// Indicates the name of the binding type. This is the name that will be used by the registrar to make the
		/// class available in the ObjC runtime. The default value is string.Empty, in that case the generator
		/// will use the name of the C# class.
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Get/Set the export configuration flags.
		/// </summary >
		public T? Flags { get; set; } = default (T);

		/// <summary>
		/// Get/set the visibility of the default constructor for a core image filter.
		/// </summary >
		public MethodAttributes DefaultCtorVisibility { get; set; }

		/// <summary>
		/// Get/set the visibility of the IntPtr constructor for a core image filter.
		/// </summary >
		public MethodAttributes IntPtrCtorVisibility { get; set; }

		/// <summary>
		///Get/set the visibility of the string constructor for a core image filter.
		/// </summary >
		public MethodAttributes StringCtorVisibility { get; set; }

		/// <summary>
		/// Creates a binding type attribute with the default flag value;
		/// </summary>
		public BindingTypeAttribute () {}

		/// <summary>
		/// Creates a binding type attribute with the specified flags.
		/// </summary>
		public BindingTypeAttribute (T flags)
		{
			Flags = flags;
		}

		/// <summary>
		/// Creates a binding type attribute with the specified flags.
		/// </summary>
		public BindingTypeAttribute (string name)
		{
			Name = name;
		}

		/// <summary>
		/// Creates a binding type attribute with the specified flags.
		/// </summary>
		public BindingTypeAttribute (string name, T flags)
		{
			Name = name;
			Flags = flags;
		}
	}
}
