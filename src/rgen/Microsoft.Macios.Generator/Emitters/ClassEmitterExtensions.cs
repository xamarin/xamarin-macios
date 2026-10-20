// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Generator.Formatters;
using Microsoft.Macios.Generator.IO;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Marker interface to ensure that the EmitSelectorFields and EmitMethods are used for emitters that generate classes.
/// </summary>
interface IClassEmitter : ICodeEmitter { }

static class ClassEmitterExtensions {
	/// <summary>
	/// Emit the selector fields for the current class. The method will add the fields to the binding context so that
	/// they can be used later.
	/// </summary>
	/// <param name="self">The class emitter.</param>
	/// <param name="bindingContext">The current binding context.</param>
	/// <param name="classBlock">The current class block.</param>
	public static void EmitSelectorFields (this IClassEmitter self, in BindingContext bindingContext, TabbedWriter<StringWriter> classBlock)
	{
		// we will use the binding context to store the name of the selectors so that later other methods can
		// access them
		foreach (var method in bindingContext.Changes.Methods) {
			if (method.ExportMethodData.Selector is null)
				continue;
			var selectorName = method.ExportMethodData.GetSelectorFieldName ()!;
			if (bindingContext.SelectorNames.TryAdd (method.ExportMethodData.Selector, selectorName)) {
				EmitField (method.ExportMethodData.Selector, selectorName);
			}
		}

		// Similar to methods, but with properties is hard because we have a selector for the different 
		// accessors.
		//
		// The accessor.GetSelector method helps to simplify the logic by returning the 
		// correct selector for the accessor taking the export data from the property into account
		foreach (var property in bindingContext.Changes.Properties) {
			if (!property.IsProperty)
				// ignore fields
				continue;
			var getter = property.GetAccessor (AccessorKind.Getter);
			if (!getter.IsNullOrDefault) {
				var selector = getter.GetSelector (property)!;
				var selectorName = selector.GetSelectorFieldName ();
				if (bindingContext.SelectorNames.TryAdd (selector, selectorName)) {
					EmitField (selector, selectorName);
				}
			}

			var setter = property.GetAccessor (AccessorKind.Setter);
			if (!setter.IsNullOrDefault) {
				var selector = setter.GetSelector (property)!;
				var selectorName = selector.GetSelectorFieldName ();
				if (bindingContext.SelectorNames.TryAdd (selector, selectorName)) {
					EmitField (selector, selectorName);
				}
			}
		}
		// helper function that simply writes the necessary fields to the class block.
		void EmitField (string selector, string selectorName)
		{
			classBlock.AppendGeneratedCodeAttribute (optimizable: true);
			classBlock.WriteLine (GetSelectorStringField (selector, selectorName).ToString ());
			classBlock.WriteLine (GetSelectorHandleField (selector, selectorName).ToString ());
			// reading generated code should not be painful, add a space
			classBlock.WriteLine ();
		}
	}

	/// <summary>
	/// Emits the body for a method that does not return a value.
	/// </summary>
	/// <param name="context">Current binding context.</param>
	/// <param name="method">The method for which to generate the body.</param>
	/// <param name="invocations">The method invocations and argument transformations.</param>
	/// <param name="methodBlock">The writer for the method block.</param>
	static void EmitVoidMethodBody (in BindingContext context, in Method method, in MethodInvocations invocations, TabbedWriter<StringWriter> methodBlock)
	{
		// validate and init the needed temp variables
		foreach (var argument in invocations.Arguments) {
			methodBlock.Write (argument.Validations, verifyTrivia: false);
			methodBlock.Write (argument.Initializers, verifyTrivia: false);
		}

		// do any pre-call conversions that might be needed, for example string to NSString
		foreach (var argument in invocations.Arguments) {
			methodBlock.Write (argument.PreCallConversion, verifyTrivia: false);
		}

		// if we are dealing with a protocol or an extension method, we need to call send directly
		if (context.Changes.BindingType == BindingType.Protocol || method.IsExtension || method.SkipRegistration) {
			methodBlock.WriteRaw (
$@"{ExpressionStatement (invocations.Send)}
{ExpressionStatement (KeepAlive (method.This))}
");
		} else {
			// simply call the send or sendSuper accordingly
			methodBlock.WriteRaw (
$@"if (IsDirectBinding) {{
	{ExpressionStatement (invocations.Send)}
}} else {{
	{ExpressionStatement (invocations.SendSuper)}
}}
{ExpressionStatement (KeepAlive (method.This))}
");
		}

		// before we leave the methods, do any post operations
		foreach (var argument in invocations.Arguments) {
			methodBlock.Write (argument.PostCallConversion, verifyTrivia: false);
		}
	}

	/// <summary>
	/// Emits the body for a method that returns a value.
	/// </summary>
	/// <param name="method">The method for which to generate the body.</param>
	/// <param name="invocations">The method invocations and argument transformations.</param>
	/// <param name="methodBlock">The writer for the method block.</param>
	static void EmitReturnMethodBody (in BindingContext context, in Method method, in MethodInvocations invocations, TabbedWriter<StringWriter> methodBlock)
	{
		// similar to the void method but we need to create a temp variable to store the return value
		// and do any conversions that might be needed for the return value, for example byte to bool
		var (tempVar, tempDeclaration) = GetReturnValueAuxVariable (method.ReturnType);

		// init and validate the needed temp variables
		foreach (var argument in invocations.Arguments) {
			methodBlock.Write (argument.Validations, verifyTrivia: false);
			methodBlock.Write (argument.Initializers, verifyTrivia: false);
		}

		// perform any pre-call conversions that might be needed, for example string to NSString
		foreach (var argument in invocations.Arguments) {
			methodBlock.Write (argument.PreCallConversion, verifyTrivia: false);
		}

		// if we are dealing with a protocol or an extension method, we need to call send directly
		if (context.Changes.BindingType == BindingType.Protocol || method.IsExtension || method.SkipRegistration) {
			methodBlock.WriteRaw (
$@"{tempDeclaration}
{ExpressionStatement (invocations.Send)}
{ExpressionStatement (KeepAlive (method.This))}
");
		} else {
			methodBlock.WriteRaw (
$@"{tempDeclaration}
if (IsDirectBinding) {{
	{ExpressionStatement (invocations.Send)}
}} else {{
	{ExpressionStatement (invocations.SendSuper)}
}}
{ExpressionStatement (KeepAlive (method.This))}
");
		}
		// before returning the value, we need to do the post operations for the temp vars
		foreach (var argument in invocations.Arguments) {
			methodBlock.Write (argument.PostCallConversion, verifyTrivia: false);
		}
		methodBlock.WriteLine ($"return {tempVar};");
	}

	/// <summary>
	/// Emits the code for a given method, including its async version if applicable.
	/// </summary>
	/// <param name="self">The class emitter.</param>
	/// <param name="context">The current binding context.</param>
	/// <param name="method">The method to emit.</param>
	/// <param name="classBlock">The current class block writer.</param>
	/// <param name="uiThreadCheck">An optional UI thread check expression. If not provided, it will be created based on the context.</param>
	public static void EmitMethod (this IClassEmitter self, in BindingContext context, in Method method,
		TabbedWriter<StringWriter> classBlock, ExpressionStatementSyntax? uiThreadCheck = null)
	{

		// if not passed as an argument, we will create the ui thread check based on the context
		if (uiThreadCheck is null) {
			uiThreadCheck = (context.NeedsThreadChecks)
				? EnsureUiThread (context.RootContext.CurrentPlatform)
				: null;
		}

		classBlock.AppendMemberAvailability (method.SymbolAvailability);
		classBlock.AppendGeneratedCodeAttribute (optimizable: true);

		// append the export attribute to the method just in case it is a protocol method in a wrapper class,
		// that is when the method is not an extension method and the binding type is protocol.
		if (context.Changes.BindingType == BindingType.Protocol && !method.IsExtension) {
			classBlock.AppendExportAttribute (method.ExportMethodData);
		}

		// this is needed to support the registrar until it understands the rgen attributes. 
		if (GeneratorConfiguration.BGenCompatible && !method.SkipRegistration) {
			classBlock.AppendBgenExportAttribute (method.ExportMethodData);
		}

		using (var methodBlock = classBlock.CreateBlock (method.ToDeclaration ().ToString (), block: true)) {
			// write any possible thread check at the beginning of the method
			if (uiThreadCheck is not null) {
				methodBlock.WriteLine (uiThreadCheck.ToString ());
				methodBlock.WriteLine ();
			}

			// retrieve the method invocation via the factory, this will generate the necessary arguments
			// transformations and the invocation
			var invocations = GetInvocations (method);

			if (method.ReturnType.IsVoid) {
				EmitVoidMethodBody (context, method, invocations, methodBlock);
			} else {
				EmitReturnMethodBody (context, method, invocations, methodBlock);
			}
		}

		if (!method.GenerateAsync)
			// we cannot generate an async method
			return;

		// if the method is an async method, generate its async version
		classBlock.WriteLine ();
		classBlock.AppendMemberAvailability (method.SymbolAvailability);
		classBlock.AppendGeneratedCodeAttribute (optimizable: true);

		var asyncMethod = method.ToAsync ();
		using (var methodBlock = classBlock.CreateBlock (asyncMethod.ToDeclaration ().ToString (), block: true)) {
			// we need to create the tcs for the the async method
			var tcsType = asyncMethod.ReturnType.ToTaskCompletionSource ();
			var tcsName = Nomenclator.GetTaskCompletionSourceName ();
			methodBlock.WriteRaw (
$@"{tcsType.GetIdentifierSyntax ()} {tcsName} = new ();
{ExpressionStatement (ExecuteSyncCall (method))}
return {tcsName}.Task;
");
		}
	}

	/// <summary>
	/// Emit the code for all the methods in the class.
	/// </summary>
	/// <param name="context">The current binding context.</param>
	/// <param name="classBlock">Current class block.</param>
	public static void EmitMethods (this IClassEmitter self, in BindingContext context, TabbedWriter<StringWriter> classBlock)
	{
		var uiThreadCheck = (context.NeedsThreadChecks)
			? EnsureUiThread (context.RootContext.CurrentPlatform) : null;
		foreach (var method in context.Changes.Methods.OrderBy (m => m.Name)) {
			EmitMethod (self, context, method, classBlock, uiThreadCheck);
			classBlock.WriteLine ();
		}
	}

	/// <summary>
	/// Emit the code for all the field properties in the class. The code will add any necessary backing fields and
	/// will return all properties that are notifications.
	/// </summary>
	/// <param name="self">The class emitter.</param>
	/// <param name="className">The current class name.</param>
	/// <param name="properties">All properties of the class, the method will filter those that are fields.</param>
	/// <param name="classBlock">Current class block.</param>
	/// <param name="notificationProperties">An immutable array with all the properties that are marked as notifications
	/// and that need a helper class to be generated.</param>
	public static void EmitFields (this IClassEmitter self, string className, in ImmutableArray<Property> properties, TabbedWriter<StringWriter> classBlock,
		out ImmutableArray<Property> notificationProperties)
	{
		var notificationsBuilder = ImmutableArray.CreateBuilder<Property> ();
		foreach (var property in properties.OrderBy (p => p.Name)) {
			if (!property.IsField)
				continue;

			// a field should always have a getter, if it does not, we do not generate the property
			var getter = property.GetAccessor (AccessorKind.Getter);
			if (getter.IsNullOrDefault)
				continue;

			// provide a backing variable for the property if and only if we are dealing with a reference type
			if (property.IsReferenceType) {
				classBlock.WriteLine (FieldPropertyBackingVariable (property).ToString ());
				classBlock.WriteLine ();
			}

			classBlock.AppendMemberAvailability (property.SymbolAvailability);
			classBlock.AppendGeneratedCodeAttribute (optimizable: true);
			if (property.IsNotification) {
				// add it to the bucket so that we can later generate the necessary partial class for the 
				// notifications
				notificationsBuilder.Add (property);
				classBlock.AppendNotificationAdvice (className, property.Name);
			}

			using (var propertyBlock = classBlock.CreateBlock (property.ToDeclaration ().ToString (), block: true)) {
				// generate the accessors, we will always have a get, a set is optional depending on the type
				// if the symbol availability of the accessor is different of the one from the property, write it
				var backingField = property.BackingField;

				// be very verbose with the availability, makes the life easier to the dotnet analyzer
				propertyBlock.AppendMemberAvailability (getter.SymbolAvailability);
				using (var getterBlock = propertyBlock.CreateBlock ("get", block: true)) {
					// fields with a reference type have a backing fields, while value types do not
					if (property.IsReferenceType) {
						getterBlock.WriteRaw (
$@"if ({backingField} is null)
	{backingField} = {ExpressionStatement (FieldConstantGetter (property))}
return {backingField};
");
					} else {
						// directly return the call from the getter
						getterBlock.WriteLine ($"return {ExpressionStatement (FieldConstantGetter (property))}");
					}
				}

				var setter = property.GetAccessor (AccessorKind.Setter);
				if (!setter.IsNullOrDefault) {
					propertyBlock.WriteLine (); // add space between getter and setter since we have the attrs
					propertyBlock.AppendMemberAvailability (setter.SymbolAvailability);
					using (var setterBlock = propertyBlock.CreateBlock ("set", block: true)) {
						if (property.IsReferenceType) {
							// set the backing field
							setterBlock.WriteLine ($"{backingField} = value;");
						}

						// call the native code
						setterBlock.WriteLine ($"{ExpressionStatement (FieldConstantSetter (property, "value"))}");
					}
				}
			}
			classBlock.WriteLine ();
		}
		notificationProperties = notificationsBuilder.ToImmutable ();
	}

	/// <summary>
	/// Emits the code for a given property.
	/// </summary>
	/// <param name="self">The class emitter.</param>
	/// <param name="context">The current binding context.</param>
	/// <param name="property">The property to emit.</param>
	/// <param name="classBlock">The current class block writer.</param>
	/// <param name="uiThreadCheck">An optional UI thread check expression. If not provided, it will be created based on the context.</param>
	public static void EmitProperty (this IClassEmitter self, in BindingContext context, in Property property,
		TabbedWriter<StringWriter> classBlock, ExpressionStatementSyntax? uiThreadCheck = null)
	{

		// if not passed as an argument, we will create the ui thread check based on the context
		if (uiThreadCheck is null) {
			uiThreadCheck = (context.NeedsThreadChecks)
				? EnsureUiThread (context.RootContext.CurrentPlatform)
				: null;
		}

		if (property.IsField)
			// ignore fields
			return;
		// use the factory to generate all the needed invocations for the current 
		var invocations = GetInvocations (property);

		// we expect to always at least have a getter
		var getter = property.GetAccessor (AccessorKind.Getter);
		if (getter.IsNullOrDefault)
			return;

		// add backing variable for the property if it is needed
		if (property.NeedsBackingField) {
			classBlock.AppendGeneratedCodeAttribute (optimizable: true);
			classBlock.WriteLine ($"object? {property.BackingField} = null;");
			classBlock.WriteLine ();
		}

		classBlock.AppendMemberAvailability (property.SymbolAvailability);
		classBlock.AppendGeneratedCodeAttribute (optimizable: true);

		if (context.Changes.BindingType == BindingType.Protocol) {
			// add the export method for the property, this is needed for the protocol wrapper
			classBlock.AppendExportAttribute (property.ExportPropertyData);
		}

		using (var propertyBlock = classBlock.CreateBlock (property.ToDeclaration ().ToString (), block: true)) {
			// be very verbose with the availability, makes the life easier to the dotnet analyzer
			propertyBlock.AppendMemberAvailability (getter.SymbolAvailability);
			// if we deal with a delegate, include the attr:
			// [return: DelegateProxy (typeof ({staticBridge}))]
			if (property.ReturnType.IsDelegate)
				propertyBlock.AppendDelegateProxyReturn (property.ReturnType);
			if (context.Changes.BindingType == BindingType.Protocol && !getter.ExportPropertyData.IsNullOrDefault)
				propertyBlock.AppendExportAttribute (getter.ExportPropertyData);

			if (GeneratorConfiguration.BGenCompatible && !property.SkipRegistration) {
				var selector = getter.GetSelector (property);
				// same, needed to support the registrar until it understands the rgen attributes.
				propertyBlock.AppendBgenExportAttribute (selector);
			}

			using (var getterBlock = propertyBlock.CreateBlock ("get", block: true)) {
				if (uiThreadCheck is not null) {
					getterBlock.WriteLine (uiThreadCheck.ToString ());
					getterBlock.WriteLine ();
				}
				// depending on the property definition, we might need a temp variable to store
				// the return value
				var (tempVar, tempDeclaration) = GetReturnValueAuxVariable (property.ReturnType);
				// if the binding is a protocol, we need to call send directly
				if (context.Changes.BindingType == BindingType.Protocol || property.SkipRegistration) {
					getterBlock.WriteLine ($"{tempDeclaration}");
					getterBlock.WriteLine ($"{ExpressionStatement (invocations.Getter.Send)}");
					getterBlock.WriteLine ($"{ExpressionStatement (KeepAlive ("this"))}");
				} else {
					getterBlock.WriteRaw (
$@"{tempDeclaration}
if (IsDirectBinding) {{
	{ExpressionStatement (invocations.Getter.Send)}
}} else {{
	{ExpressionStatement (invocations.Getter.SendSuper)}
}}
{ExpressionStatement (KeepAlive ("this"))}
");
				}

				if (property.RequiresDirtyCheck || property.IsWeakDelegate) {
					getterBlock.WriteLine ("MarkDirty ();");
				}

				if (property.NeedsBackingField) {
					getterBlock.WriteLine ($"{property.BackingField} = {tempVar};");
				}

				getterBlock.WriteLine ($"return {tempVar};");
			}

			var setter = property.GetAccessor (AccessorKind.Setter);
			if (setter.IsNullOrDefault || invocations.Setter is null)
				// we are done with the current property
				return;

			propertyBlock.WriteLine (); // add space between getter and setter since we have the attrs
			propertyBlock.AppendMemberAvailability (setter.SymbolAvailability);
			// if we deal with a delegate, include the attr:
			// [param: BlockProxy (typeof ({nativeInvoker}))]
			if (property.ReturnType.IsDelegate)
				propertyBlock.AppendDelegateParameter (property.ReturnType);
			if (context.Changes.BindingType == BindingType.Protocol && !setter.ExportPropertyData.IsNullOrDefault)
				propertyBlock.AppendExportAttribute (setter.ExportPropertyData);

			if (GeneratorConfiguration.BGenCompatible && !property.SkipRegistration) {
				// same, needed to support the registrar until it understands the rgen attributes.
				var selector = setter.GetSelector (property);
				propertyBlock.AppendBgenExportAttribute (selector);
			}
			using (var setterBlock = propertyBlock.CreateBlock ("set", block: true)) {
				if (uiThreadCheck is not null) {
					setterBlock.WriteLine (uiThreadCheck.ToString ());
					setterBlock.WriteLine ();
				}
				// init the needed temp variables
				setterBlock.Write (invocations.Setter.Value.Argument.Initializers, verifyTrivia: false);
				setterBlock.Write (invocations.Setter.Value.Argument.Validations, verifyTrivia: false);
				setterBlock.Write (invocations.Setter.Value.Argument.PreCallConversion, verifyTrivia: false);

				// perform the invocation
				// if the binding is a protocol, we need to call send directly
				if (context.Changes.BindingType == BindingType.Protocol || property.SkipRegistration) {
					setterBlock.WriteLine ($"{ExpressionStatement (invocations.Setter.Value.Send)}");
					setterBlock.WriteLine ($"{ExpressionStatement (KeepAlive ("this"))}");
				} else {
					setterBlock.WriteRaw (
$@"if (IsDirectBinding) {{
	{ExpressionStatement (invocations.Setter.Value.Send)}
}} else {{
	{ExpressionStatement (invocations.Setter.Value.SendSuper)}
}}
{ExpressionStatement (KeepAlive ("this"))}
");
				}

				// perform the post delegate call conversion, this might include the GC.KeepAlive calls to keep
				// the native object alive
				setterBlock.Write (invocations.Setter.Value.Argument.PostCallConversion, verifyTrivia: false);
				// mark property as dirty if needed
				if (property.RequiresDirtyCheck || property.IsWeakDelegate) {
					setterBlock.WriteLine ("MarkDirty ();");
				}

				if (property.NeedsBackingField) {
					setterBlock.WriteLine ($"{property.BackingField} = value;");
				}
			}
		}

		// if the property is a weak delegate and has the strong delegate type set, we need to emit the
		// strong delegate property
		if (property is { IsProperty: true, IsWeakDelegate: true }
			&& !property.ExportPropertyData.StrongDelegateType.IsNullOrDefault) {
			classBlock.WriteLine ();
			var strongDelegate = property.ToStrongDelegate ();
			using (var propertyBlock =
				   classBlock.CreateBlock (strongDelegate.ToDeclaration ().ToString (), block: true)) {
				using (var getterBlock =
					   propertyBlock.CreateBlock ("get", block: true)) {
					getterBlock.WriteLine (
						$"return {property.Name} as {strongDelegate.ReturnType.WithNullable (isNullable: false).GetIdentifierSyntax ()};");
				}

				using (var setterBlock =
					   propertyBlock.CreateBlock ("set", block: true)) {
					setterBlock.WriteRaw (
$@"var rvalue = value as NSObject;
if (!(value is null) && rvalue is null) {{
	throw new ArgumentException ($""The object passed of type {{value.GetType ()}} does not derive from NSObject"");
}}
{property.Name} = rvalue;
");
				}
			}
		}
	}

	/// <summary>
	/// Emit the code for all the properties in the class.
	/// </summary>
	/// <param name="self">The class emitter.</param>
	/// <param name="context">The current binding context.</param>
	/// <param name="classBlock">Current class block.</param>
	/// <param name="strongDelegates">List of properties who generates events.</param>
	public static void EmitProperties (this IClassEmitter self, in BindingContext context,
		TabbedWriter<StringWriter> classBlock, out ImmutableArray<Property> strongDelegates)
	{
		var strongDelegatesBuilder = ImmutableArray.CreateBuilder<Property> ();
		// use the binding context to decide if we need to insert the ui thread check
		var uiThreadCheck = (context.NeedsThreadChecks)
			? EnsureUiThread (context.RootContext.CurrentPlatform) : null;

		foreach (var property in context.Changes.Properties.OrderBy (p => p.Name)) {
			if (property.IsField)
				continue;
			if (property.CreateEvents)
				strongDelegatesBuilder.Add (property);
			EmitProperty (self, in context, property, classBlock, uiThreadCheck);
			classBlock.WriteLine ();
		}
		strongDelegates = strongDelegatesBuilder.ToImmutable ();
	}

	/// <summary>
	/// Emit the default constructors for the class.
	/// </summary>
	/// <param name="className">The name of the class whose constructors are going to be generated.</param>
	/// <param name="classBlock">Current class block.</param>
	/// <param name="disableDefaultCtor">A value indicating whether to disable the default constructor.</param>
	public static void EmitDefaultNSObjectConstructors (this IClassEmitter self, string className, TabbedWriter<StringWriter> classBlock, bool disableDefaultCtor)
	{
		if (!disableDefaultCtor) {
			classBlock.WriteDocumentation (Documentation.Class.DefaultInit (className));
			classBlock.AppendGeneratedCodeAttribute ();
			classBlock.AppendDesignatedInitializer ();
			classBlock.WriteRaw (
$@"[Export (""init"")]
public {className} () : base ({NSObjectFlag}.Empty)
{{
	if (IsDirectBinding)
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
	else
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
}}
");
			classBlock.WriteLine ();
		}

		classBlock.WriteDocumentation (Documentation.Class.DefaultInitWithFlag (className));
		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.WriteLine ($"protected {className} ({NSObjectFlag} t) : base (t) {{}}");

		classBlock.WriteLine ();
		classBlock.WriteDocumentation (Documentation.Class.DefaultInitWithHandle (className));
		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.WriteLine ($"protected internal {className} ({NativeHandle} handle) : base (handle) {{}}");
		classBlock.WriteLine ();
	}
}
