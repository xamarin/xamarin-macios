// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;

namespace Microsoft.Macios.Generator.Emitters;

static partial class BindingSyntaxFactory {

	internal static (string? Getter, string? Setter) GetObjCMessageSendMethods (in Property property,
		bool isSuper = false, bool isStret = false)
	{
		if (property.IsProperty) {
			// the getter and the setter depend of the accessors that have been set for the property, we do not want
			// to calculate things that we won't use. The export data used will also depend if the getter/setter has a
			// export attribute attached
			var getter = property.GetAccessor (AccessorKind.Getter);
			string? getterMsgSend = null;
			if (getter is not null) {
				var getterExportData = getter.Value.ExportPropertyData ?? property.ExportPropertyData;
				if (getterExportData is not null) {
					getterMsgSend = GetObjCMessageSendMethodName (getterExportData.Value, property.ReturnType, [],
						isSuper, isStret);
				}
			}

			var setter = property.GetAccessor (AccessorKind.Setter);
			string? setterMsgSend = null;
			if (setter is not null) {
				var setterExportData = setter.Value.ExportPropertyData ?? property.ExportPropertyData;
				if (setterExportData is not null) {
					setterMsgSend = GetObjCMessageSendMethodName (setterExportData.Value, TypeInfo.Void,
						[property.ValueParameter], isSuper, isStret);
				}
			}

			return (Getter: getterMsgSend, Setter: setterMsgSend);
		}

		return default;
	}
	
	internal static (StatementSyntax Send, StatementSyntax SendSupper) GetGetterInvocations (in Property property, 
		string? selector, string? sendMethod, string? superSendMethod)
	{
		// if any of the methods is null, return a throw statement for both
		if (selector is null || sendMethod is null || superSendMethod is null) {
			return (ThrowNotImplementedException (), ThrowNotImplementedException ());
		}
		
		// the invocations depend on the property requiring a temp return variable or not
		if (property.UseTempReturn) {
			return (
				Send: ThrowNotImplementedException (), 
				SendSupper: ThrowNotImplementedException ()
			);
		} 
		// this is the simplest case, we just need to call the method and return the result, for that we
		// use the MessagingInvocation method for each of the methods
		return (
			Send: GetterInvocation (MessagingInvocation (sendMethod, selector, []), property),
			SendSupper: GetterInvocation (MessagingInvocation (superSendMethod, selector, []), property)
		);

#pragma warning disable format
		// helper internal function that returns the expression based on the property return type and uses the passed
		// message send expression
		ExpressionStatementSyntax GetterInvocation (InvocationExpressionSyntax objMsgSend, in Property property)
			=> property.ReturnType switch {
				// string[]? => CFArray.StringArrayFromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (class_ptr, Selector.GetHandle ("selector")), false);
				{ IsArray: true, Name: "string", IsNullable: true } =>
					ExpressionStatement (StringArrayFromHandle ([Argument (objMsgSend), BoolArgument (false)])),

				// string[] => CFArray.StringArrayFromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (class_ptr, Selector.GetHandle ("selector")), false)!;
				{ IsArray: true, Name: "string", IsNullable: false } =>
					ExpressionStatement (SuppressNullableWarning (StringArrayFromHandle ([Argument (objMsgSend), BoolArgument (false)]))),

				// string => CFString.FromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle ("tunnelRemoteAddress")), false);
				{ SpecialType: SpecialType.System_String, IsNullable: true } =>
					ExpressionStatement (StringFromHandle ([Argument (objMsgSend), BoolArgument (false)])),

				// string => CFString.FromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle ("tunnelRemoteAddress")), false)!;
				{ SpecialType: SpecialType.System_String, IsNullable: false } =>
					ExpressionStatement (SuppressNullableWarning (StringFromHandle ([Argument (objMsgSend), BoolArgument (false)]))),

				// general case, just return the result of the send message
				_ => ExpressionStatement (objMsgSend),
			}; 
#pragma warning restore format
	}

	internal static PropertyInvocations GetInvocations (in Property property)
	{
		// retrieve the objc_msgSend methods
		var (getter, setter) = GetObjCMessageSendMethods (property, isStret: property.ReturnType.NeedsStret);
		var (superGetter, supperSetter) = GetObjCMessageSendMethods (property, isSuper: true, isStret: property.ReturnType.NeedsStret);
		var getterSelector = property.GetAccessor (AccessorKind.Getter)?.GetSelector (property);
		var setterSelector = property.GetAccessor (AccessorKind.Getter)?.GetSelector (property);
		
		return new () {
			Getter = GetGetterInvocations (property, getterSelector, getter, superGetter),
			Setter = (ThrowNotImplementedException (), ThrowNotImplementedException ()),
		};
	}
}
