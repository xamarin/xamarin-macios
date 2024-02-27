//
// Encapsulates the information necessary to create a block delegate
//
// FIXME: We do not really need this class, we should just move all this
// pre-processing to the generation stage, instead of decoupling it in two places.
//
// The Name is the internal generated name we use for the delegate
// The Parameters is used for the internal delegate signature
// The Invoke contains the invocation steps necessary to invoke the method
//

using System;

#nullable enable

public class TrampolineInfo {
	public string UserDelegate;
	public string DelegateName;
	public string Parameters;
	public string Convert;
	public string Invoke;
	public string ReturnType;
	public string DelegateReturnType;
	public string ReturnFormat;
	public string Clear;
	public string? OutReturnType;
	public string PostConvert;
	public string? UserDelegateTypeAttribute;
	public string FunctionPointerSignature;
	public Type Type;

	public TrampolineInfo (string userDelegate, string delegateName, string pars,
		string convert, string invoke, string returnType, string delegateReturnType, string returnFormat,
		string clear, string postConvert, Type type, string functionPointerSignature)
	{
		UserDelegate = userDelegate;
		DelegateName = delegateName;
		Parameters = pars;
		Convert = convert;
		Invoke = invoke;
		ReturnType = returnType;
		DelegateReturnType = delegateReturnType;
		ReturnFormat = returnFormat;
		Clear = clear;
		PostConvert = postConvert;
		Type = type;
		FunctionPointerSignature = functionPointerSignature;
	}

	// Name for the static class generated that contains the Objective-C to C# block bridge
	public string StaticName {
		get {
			return "S" + DelegateName;
		}
	}

	// Name for the class generated that allows C# to invoke an Objective-C block
	public string NativeInvokerName {
		get {
			return "NI" + DelegateName;
		}
	}
}

class TrampolineParameterInfo {
	public string Type;
	public string ParameterName;

	public TrampolineParameterInfo (string type, string parameterName)
	{
		Type = type;
		ParameterName = parameterName;
	}
}
