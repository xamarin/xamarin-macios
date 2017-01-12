//
// error.cs: Error handling code for bmac/btouch
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com
//   Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin, Inc.
//
//

using System;
using System.Collections.Generic;

using ProductException=BindingException;

// Error allocation
//
// BI0xxx	the generator itself, e.g. parameters, environment
//		BI0001	The .NET runtime could not load the {mi.ReturnType.Name} type. Message: {ex.Message}
//		BI0068	Invalid value for target framework: {0}. [same error number/message as mtouch/mmp]
//		BI0070	Invalid target framework: {0}. Valid target frameworks are: {1}. [same error number/message as mtouch/mmp]
//		BI0086  A target framework (--target-framework) must be specified when building for Xamarin.Mac. [same error number/message as mtouch/mmp]
// BI1xxx	code generation
//	BI10xx	errors
//		BI1001 Do not know how to make a trampoline for {0}
//		BI1002 Unknown kind {0} in method '{1}'
//		BI1003 The delegate method {0}.{1} needs to take at least one parameter
//		BI1004 The delegate method {0}.{1} is missing the [EventArgs] attribute (has {2} parameters)
//		BI1005 EventArgs in {0}.{1} attribute should not include the text `EventArgs' at the end
//		BI1006 The delegate method {0}.{1} is missing the [DelegateName] attribute (or EventArgs)
//		BI1007 Unknown attribute {0} on {1}
//		BI1008 [IsThreadStatic] is only valid on properties that are also [Static]
//		BI1009 No selector specified for method `{0}.{1}'
//		BI1010 No Export attribute on {0}.{1} property
//		BI1011 Do not know how to extract type {0}/{1} from an NSDictionary
//		BI1012 No Export or Bind attribute defined on {0}.{1}
//		BI1013 Unsupported type for Fields (string), you probably meant NSString
//		BI1014 Unsupported type for Fields: {0}
//		BI1015 In class {0} You specified the Events property, but did not bind those to names with Delegates
//		BI1016 The delegate method {0}.{1} is missing the [DefaultValue] attribute
//		BI1017 Do not know how to make a signature for {0}
//		BI1018 No [Export] attribute on property {0}.{1}
//		BI1019 Invalid [NoDefaultValue] attribute on method `{0}.{1}'
//		BI1020 Unsupported type {0} used on exported method {1}.{2}
//		BI1021 Unsupported type for read/write Fields: {0}
//		BI1022 Model classes can not be categories
//		BI1023 The number of Events (Type) and Delegates (string) must match for `{0}`
//		BI1024 No selector specified for property '{0}.{1}'
//		BI1025 [Static] and [Protocol] are mutually exclusive ({0})
//		BI1026 `{0}`: Enums attributed with [{1}] must have an underlying type of `long` or `ulong`
//		BI1027 Support for ZeroCopy strings is not implemented. Strings will be marshalled as NSStrings.
//		BI1028 Internal sanity check failed, please file a bug report (http://bugzilla.xamarin.com) with a test case.
//		BI1029 Internal error: invalid enum mode for type '{0}'
//		BI1030 {0} cannot have [BaseType(typeof({1}))] as it creates a circular dependency
//		BI1031 The [Target] attribute is not supported for the Unified API (found on the member '{0}.{1}'). For Objective-C categories, create an api definition interface with the [Category] attribute instead.
//		BI1034 The [Protocolize] attribute is set on the property {0}.{1}, but the property's type ({2}) is not a protocol.
//		BI1035 The property {0} on class {1} is hiding a property from a parent class {2} but the selectors do not match.
//		BI1036 The last parameter in the method '{0}.{1}' must be a delegate (it's '{2}').
//		BI1037 The selector {0} on type {1} is found multiple times with both read only and write only versions, with no read/write version.
//		BI1038 The selector {0} on type {1} is found multiple times with different return types.
//		BI1039 The selector {0} on type {1} is found multiple times with different argument length {2} : {3}.
//		BI1040 The selector {0} on type {1} is found multiple times with different argument out states on argument {2}.
//		BI1041 The selector {0} on type {1} is found multiple times with different argument types on argument {2} - {3} : {4}.
//		BI1042 Missing '[Field (LibraryName=value)]' for {field_pi.Name} (e.g."__Internal")
//		BI1043 Repeated overload {mi.Name} and no [DelegateApiNameAttribute] provided to generate property name on host class.
//		BI1044 Repeated name '{apiName.Name}' provided in [DelegateApiNameAttribute].
//		BI1045 Only a single [DefaultEnumValue] attribute can be used inside enum {type.Name}.
//		BI1046 The [Field] constant {fa.SymbolName} cannot only be used once inside enum {type.Name}.
//		BI1047 Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.
//		BI1048 Unsupported type {0} decorated with [BindAs].
//		BI1049 Could not unbox type {0} from {1} container used on {2} member decorated with [BindAs].
//	BI11xx	warnings
//		BI1101 Trying to use a string as a [Target]
//		BI1102 Using the deprecated EventArgs for a delegate signature in {0}.{1}, please use DelegateName instead
//		BI1103 '{0}' does not live under a namespace; namespaces are a highly recommended .NET best practice
//		BI1104 Could not load the referenced library '{0}': {1}.
//		BI1105 Potential selector/argument mismatch [Export ("{0}")] has {1} arguments and {2}.{3} has {4} arguments
//		BI1106 The parameter {2} in the method {0}.{1} exposes a model ({3}). Please expose the corresponding protocol type instead ({5}.I{6})
//		BI1107 The return type of the method {0}.{1} exposes a model ({2}). Please expose the corresponding protocol type instead ({3}.I{4}).
//		BI1108 The [Protocolize] attribute is applied to the return type of the method {0}.{1}, but the return type ({2}) isn't a model and can thus not be protocolized. Please remove the [Protocolize] attribute.
//		BI1109 The return type of the method {0}.{1} exposes a model ({2}). Please expose the corresponding protocol type instead ({3}.I{4}).
//		BI1110 The property {0}.{1} exposes a model ({2}). Please expose the corresponding protocol type instead ({3}.I{4}).
//		BI1111 Interface '{0}' on '{1}' is being ignored as it is not a protocol. Did you mean '{2}' instead?
//		BI1112 Property {0} should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work.
//		BI1113  BaseType.Delegates were set but no properties could be found. Do ensure that the WrapAttribute is used on the right properties.
//		BI1114 Binding error: test unable to find property: {0} on {1}.
//		BI1115 The parameter '{0}' in the delegate '{1}' does not have a [CCallback] or [BlockCallback] attribute. Defaulting to [CCallback].
//		BI1116 The parameter '{0}' in the delegate '{1}' does not have a [CCallback] or [BlockCallback] attribute. Defaulting to [CCallback]. Declare a custom delegate instead of using System.Action / System.Func and add the attribute on the corresponding parameter.
// BI2xxx	reserved
// BI3xxx	reserved
// BI4xxx	reserved
// BI5xxx	reserved
// BI6xxx	reserved
// BI7xxx	reserved
// BI8xxx	reserved
// BI9xxx	reserved

public class BindingException : Exception {
	
	public BindingException (int code, string message, params object[] args) : 
		this (code, false, message, args)
	{
	}

	public BindingException (int code, bool error, string message, params object[] args) : 
		this (code, error, null, message, args)
	{
	}

	public BindingException (int code, bool error, Exception innerException, string message, params object[] args) : 
		base (String.Format (message, args), innerException)
	{
		Code = code;
		Error = error;
	}

	public int Code { get; private set; }
	
	public bool Error { get; private set; }
	
	// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
	public override string ToString ()
	{
		 return String.Format ("{0} BI{1:0000}: {3}: {2}",
			Error ? "error" : "warning", Code, Message, BindingTouch.ToolName);
	}
}

public static class ErrorHelper {
	
	static public int Verbosity { get; set; }
	
	public static ProductException CreateError (int code, string message, params object[] args)
	{
		return new ProductException (code, true, message, args);
	}

	static public void Show (Exception e)
	{
		List<Exception> exceptions = new List<Exception> ();
		bool error = false;

		CollectExceptions (e, exceptions);

		foreach (var ex in exceptions)
			error |= ShowInternal (ex);

		if (error)
			Environment.Exit (1);
	}

	static void CollectExceptions (Exception ex, List<Exception> exceptions)
	{
#if NET_4_0
		AggregateException ae = ex as AggregateException;

		if (ae != null) {
			foreach (var ie in ae.InnerExceptions)
				CollectExceptions (ie, exceptions);
		} else {
			exceptions.Add (ex);
		}
#else
		exceptions.Add (ex);
#endif
	}

	static bool ShowInternal (Exception e)
	{
		BindingException mte = (e as BindingException);
		bool error = true;

		if (mte != null) {
			error = mte.Error;
			Console.Out.WriteLine (mte.ToString ());
			
			if (Verbosity > 1) {
				Exception ie = e.InnerException;
				if (ie != null) {
					if (Verbosity > 3) {
						Console.Error.WriteLine ("--- inner exception");
						Console.Error.WriteLine (ie);
						Console.Error.WriteLine ("---");
					} else {
						Console.Error.WriteLine ("\t{0}", ie.Message);
					}
				}
			}
			
			if (Verbosity > 2)
				Console.Error.WriteLine (e.StackTrace);
		} else {
			Console.Out.WriteLine ("error BI0000: Unexpected error - Please file a bug report at http://bugzilla.xamarin.com");
			Console.Out.WriteLine (e.ToString ());
			Console.Out.WriteLine (Environment.StackTrace);
		}
		return error;
	}
}
