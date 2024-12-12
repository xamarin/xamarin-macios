using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Flags to be used on methods that will generate constructors in the binding class.
	/// </summary>
	[Flags]
	public enum Constructor : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Map to Objective-C/clang use of __attribute__((objc_designated_initializer)).
		/// </summary>
		DesignatedInitializer = 1 << 2,
	}

	/// <summary>
	/// Flgs to be used in general bound methods.
	/// </summary>
	[Flags]
	public enum Method : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Method access a variable number of args.
		/// </summary>
		IsVariadic = 1 << 2,

		/// <summary>
		/// Instruct the generator to avoid the generation of any method decorated with it in a Model.
		/// </summary>
		IgnoredInDelegate = 1 << 3,

		/// <summary>
		/// Make a method support native (Objective-C) exceptions. Instead of calling objc_msgSend directly, the invocation 
		/// will go through a custom trampoline which catches ObjectiveC exceptions and marshals them into managed exceptions.
		/// </summary>
		MarshalNativeExceptions = 1 << 4,

	}

	/// <summary>
	/// Flags to be used on properties.
	/// </summary>
	[Flags]
	public enum Property : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,

		/// <summary>
		/// The backing field for a property to be annotated with the .NET [ThreadStatic] attribute.
		/// </summary>
		IsThreadStaticAttribute = 1 << 2,

		/// <summary>
		/// Generate a notification for the property.
		/// </summary>
		Notification = 1 << 3,
	}
}

