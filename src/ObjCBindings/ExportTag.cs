using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCBindings {

	/// <summary>
	/// Flags to be used on methods that will generate constructors in the binding class.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
	public enum Constructor : Int64 {
		/// <summary>
		/// Use the default values.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Map to Objective-C/clang use of __attribute__((objc_designated_initializer)).
		/// </summary>
		DesignatedInitializer = 1 << 2,

		/// <summary>
		/// Flags the object as being thread safe.
		/// </summary>
		IsThreadSafe = 1 << 3,

		/// <summary>
		/// If this flag is applied to a property, we do not generate a NSString for
		/// marshalling the property.
		/// </summary>
		PlainString = 1 << 4,
	}

	/// <summary>
	/// Flgs to be used in general bound methods.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
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


		/// <summary>
		/// Instruct the generator to use a custom marshal directive for the method. When this flag is applied the 
		/// following name parameters must be provided:
		/// - NativePrefix: The prefix to be used in the native method name.
		/// - NativeSuffix: The suffix to be used in the native method name.
		/// - Library: The library to be used in the custom marshal directive.
		/// </summary>
		CustomMarshalDirective = 1 << 5,

		/// <summary>
		/// Flags the object as being thread safe.
		/// </summary>
		IsThreadSafe = 1 << 6,

		/// <summary>
		/// If this flag is applied to a property, we do not generate a NSString for
		/// marshalling the property.
		/// </summary>
		PlainString = 1 << 7,
	}

	/// <summary>
	/// Flags to be used on properties.
	/// </summary>
	[Flags]
	[Experimental ("APL0003")]
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

		/// <summary>
		/// Make a method support native (Objective-C) exceptions. Instead of calling objc_msgSend directly, the invocation 
		/// will go through a custom trampoline which catches ObjectiveC exceptions and marshals them into managed exceptions.
		/// </summary>
		MarshalNativeExceptions = 1 << 4,

		/// <summary>
		/// Instruct the generator to use a custom marshal directive for the method. When this flag is applied the 
		/// following name parameters must be provided:
		/// - NativePrefix: The prefix to be used in the native method name.
		/// - NativeSuffix: The suffix to be used in the native method name.
		/// - Library: The library to be used in the custom marshal directive.
		/// </summary>
		CustomMarshalDirective = 1 << 5,

		/// <summary>
		/// Apply to strings parameters that are merely retained or assigned,
		/// not copied this is an exception as it is advised in the coding
		/// standard for Objective-C to avoid this, but a few properties do use
		/// this.  Use this falg for properties flagged with `retain' or
		/// `assign', which look like this:
		///
		/// @property (retain) NSString foo;
		/// @property (assign) NSString assigned;
		///
		/// This forced the generator to create an NSString before calling the
		/// API instead of using the fast string marshalling code.
		/// </summary>
		DisableZeroCopy = 1 << 6,

		/// <summary>
		/// Flags the object as being thread safe.
		/// </summary>
		IsThreadSafe = 1 << 7,

		/// <summary>
		/// If this falgs is applied to a property, we do not generate a
		/// backing field.   See bugzilla #3359 and Assistly 7032 for some
		/// background information
		/// </summary>
		Transient = 1 << 8,

		/// <summary>
		/// If this flag is applied to a property, we do not generate a NSString for
		/// marshalling the property.
		/// </summary>
		PlainString = 1 << 9,

		/// <summary>
		/// If this flag is applied to a property, the generator will consider the property to be
		/// part of a CoreImage filter and will generate the property as a CoreImage filter property.
		/// </summary>
		CoreImageFilterProperty = 1 << 10,

	}
}

