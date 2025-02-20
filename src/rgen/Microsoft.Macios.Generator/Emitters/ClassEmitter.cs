// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Generator.Formatters;
using Microsoft.Macios.Generator.IO;
using ObjCBindings;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;
using Property = Microsoft.Macios.Generator.DataModel.Property;

namespace Microsoft.Macios.Generator.Emitters;

class ClassEmitter : ICodeEmitter {
	public string GetSymbolName (in Binding binding) => binding.Name;

	public IEnumerable<string> UsingStatements => [
		"System",
		"System.Drawing",
		"System.Diagnostics",
		"System.ComponentModel",
		"System.Threading.Tasks",
		"System.Runtime.Versioning",
		"System.Runtime.InteropServices",
		"System.Diagnostics.CodeAnalysis",
		"ObjCRuntime",
	];


	void EmitDefaultConstructors (in BindingContext bindingContext, TabbedWriter<StringWriter> classBlock, bool disableDefaultCtor)
	{

		if (!disableDefaultCtor) {
			classBlock.AppendGeneratedCodeAttribute ();
			classBlock.AppendDesignatedInitializer ();
			classBlock.WriteRaw (
$@"[Export (""init"")]
public {bindingContext.Changes.Name} () : base (NSObjectFlag.Empty)
{{
	if (IsDirectBinding)
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
	else
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
}}
");
			classBlock.WriteLine ();
		}

		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.WriteLine ($"protected {bindingContext.Changes.Name} (NSObjectFlag t) : base (t) {{}}");

		classBlock.WriteLine ();
		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.WriteLine ($"protected internal {bindingContext.Changes.Name} (NativeHandle handle) : base (handle) {{}}");
	}

	/// <summary>
	/// Emit the code for all the field properties in the class. The code will add any necessary backing fields and
	/// will return all properties that are notifications.
	/// </summary>
	/// <param name="className">The current class name.</param>
	/// <param name="properties">All properties of the class, the method will filter those that are fields.</param>
	/// <param name="classBlock">Current class block.</param>
	/// <param name="notificationProperties">An immutable array with all the properties that are marked as notifications
	/// and that need a helper class to be generated.</param>
	void EmitFields (string className, in ImmutableArray<Property> properties, TabbedWriter<StringWriter> classBlock,
		out ImmutableArray<Property> notificationProperties)
	{
		var notificationsBuilder = ImmutableArray.CreateBuilder<Property> ();
		foreach (var property in properties.OrderBy (p => p.Name)) {
			if (!property.IsField)
				continue;

			classBlock.WriteLine ();
			// a field should always have a getter, if it does not, we do not generate the property
			var getter = property.GetAccessor (AccessorKind.Getter);
			if (getter is null)
				continue;

			// provide a backing variable for the property if and only if we are dealing with a reference type
			if (property.IsReferenceType) {
				classBlock.WriteLine (FieldPropertyBackingVariable (property).ToString ());
			}

			classBlock.WriteLine ();
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
				propertyBlock.AppendMemberAvailability (getter.Value.SymbolAvailability);
				using (var getterBlock = propertyBlock.CreateBlock ("get", block: true)) {
					// fields with a reference type have a backing fields, while value types do not
					if (property.IsReferenceType) {
						getterBlock.WriteRaw (
$@"if ({backingField} is null)
	{backingField} = {FieldConstantGetter (property)}
return {backingField};
");
					} else {
						// directly return the call from the getter
						getterBlock.WriteLine ($"return {FieldConstantGetter (property)}");
					}
				}

				var setter = property.GetAccessor (AccessorKind.Setter);
				if (setter is null)
					// we are done with the current property
					continue;

				propertyBlock.WriteLine (); // add space between getter and setter since we have the attrs
				propertyBlock.AppendMemberAvailability (setter.Value.SymbolAvailability);
				using (var setterBlock = propertyBlock.CreateBlock ("set", block: true)) {
					if (property.IsReferenceType) {
						// set the backing field
						setterBlock.WriteLine ($"{backingField} = value;");
					}
					// call the native code
					setterBlock.WriteLine ($"{FieldConstantSetter (property, "value")}");
				}
			}
		}
		notificationProperties = notificationsBuilder.ToImmutable ();
	}

	void EmitProperties (in BindingContext context, TabbedWriter<StringWriter> classBlock)
	{

		// use the binding context to decide if we need to insert the ui thread check
		var uiThreadCheck = (context.NeedsThreadChecks)
			? EnsureUiThread (context.RootContext.CurrentPlatform) : null;

		foreach (var property in context.Changes.Properties.OrderBy (p => p.Name)) {
			if (property.IsField)
				// ignore fields
				continue;
			// use the factory to generate all the needed invocations for the current 
			var invocations = GetInvocations (property);

			// we expect to always at least have a getter
			var getter = property.GetAccessor (AccessorKind.Getter);
			if (getter is null)
				continue;

			classBlock.WriteLine ();
			classBlock.AppendMemberAvailability (property.SymbolAvailability);
			classBlock.AppendGeneratedCodeAttribute (optimizable: true);

			using (var propertyBlock = classBlock.CreateBlock (property.ToDeclaration ().ToString (), block: true)) {
				// be very verbose with the availability, makes the life easier to the dotnet analyzer
				propertyBlock.AppendMemberAvailability (getter.Value.SymbolAvailability);
				using (var getterBlock = propertyBlock.CreateBlock ("get", block: true)) {
					if (uiThreadCheck is not null) {
						getterBlock.WriteLine (uiThreadCheck.ToString ());
						getterBlock.WriteLine ();
					}
					// depending on the property definition, we might need a temp variable to store
					// the return value
					if (property.UseTempReturn) {
						var (tempVar, tempDeclaration) = GetReturnValueAuxVariable (property.ReturnType);
						getterBlock.WriteRaw (
$@"{tempDeclaration}
if (IsDirectBinding) {{
	{tempVar} = {invocations.Getter.Send}
}} else {{
	{tempVar} = {invocations.Getter.SendSuper}
}}
return {tempVar};
");
					} else {
						getterBlock.WriteRaw (
$@"if (IsDirectBinding) {{
	return {invocations.Getter.Send}
}} else {{
	return {invocations.Getter.SendSuper}
}}
");
					}
				}

				var setter = property.GetAccessor (AccessorKind.Setter);
				if (setter is null)
					// we are done with the current property
					continue;

				propertyBlock.WriteLine (); // add space between getter and setter since we have the attrs
				propertyBlock.AppendMemberAvailability (setter.Value.SymbolAvailability);
				using (var setterBlock = propertyBlock.CreateBlock ("set", block: true)) {
					if (uiThreadCheck is not null) {
						setterBlock.WriteLine (uiThreadCheck.ToString ());
						setterBlock.WriteLine ();
					}
					setterBlock.WriteLine ("throw new NotImplementedException();");
				}
			}
		}
	}

	void EmitNotifications (in ImmutableArray<Property> properties, TabbedWriter<StringWriter> classBlock)
	{
		// to be implemented, do not throw or tests will fail.
	}

	/// <summary>
	/// Emit the selector fields for the current class. The method will add the fields to the binding context so that
	/// they can be used later.
	/// </summary>
	/// <param name="bindingContext">The current binding context.</param>
	/// <param name="classBlock">The current class block.</param>
	void EmitSelectorFields (in BindingContext bindingContext, TabbedWriter<StringWriter> classBlock)
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
			if (getter is not null) {
				var selector = getter.Value.GetSelector (property)!;
				var selectorName = selector.GetSelectorFieldName ();
				if (bindingContext.SelectorNames.TryAdd (selector, selectorName)) {
					EmitField (selector, selectorName);
				}
			}

			var setter = property.GetAccessor (AccessorKind.Setter);
			if (setter is not null) {
				var selector = setter.Value.GetSelector (property)!;
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

	public bool TryEmit (in BindingContext bindingContext, [NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		if (bindingContext.Changes.BindingType != BindingType.Class) {
			diagnostics = [Diagnostic.Create (
					Diagnostics
						.RBI0000, // An unexpected error occurred while processing '{0}'. Please fill a bug report at https://github.com/xamarin/xamarin-macios/issues/new.
					null,
					bindingContext.Changes.FullyQualifiedSymbol)];
			return false;
		}

		// namespace declaration
		bindingContext.Builder.WriteLine ();
		bindingContext.Builder.WriteLine ($"namespace {string.Join (".", bindingContext.Changes.Namespace)};");
		bindingContext.Builder.WriteLine ();

		// register the class only if we are not dealing with a static class
		var bindingData = (BindingTypeData<Class>) bindingContext.Changes.BindingInfo;
		// Registration depends on the class name. If the binding data contains a name, we use that one, else
		// we use the name of the class
		var registrationName = bindingData.Name ?? bindingContext.Changes.Name;

		if (!bindingContext.Changes.IsStatic) {
			bindingContext.Builder.WriteLine ($"[Register (\"{registrationName}\", true)]");
		}
		var modifiers = $"{string.Join (' ', bindingContext.Changes.Modifiers)} ";
		using (var classBlock = bindingContext.Builder.CreateBlock ($"{(string.IsNullOrWhiteSpace (modifiers) ? string.Empty : modifiers)}class {bindingContext.Changes.Name}", true)) {
			// emit the fields for the selectors before we register the class or anything
			EmitSelectorFields (bindingContext, classBlock);

			if (!bindingContext.Changes.IsStatic) {
				classBlock.AppendGeneratedCodeAttribute (optimizable: true);
				classBlock.WriteLine ($"static readonly NativeHandle {ClassPtr} = Class.GetHandle (\"{registrationName}\");");
				classBlock.WriteLine ();
				classBlock.WriteLine ($"public override NativeHandle ClassHandle => {ClassPtr};");
				classBlock.WriteLine ();

				EmitDefaultConstructors (bindingContext: bindingContext,
					classBlock: classBlock,
					disableDefaultCtor: bindingData.Flags.HasFlag (Class.DisableDefaultCtor));
			}

			EmitFields (bindingContext.Changes.Name, bindingContext.Changes.Properties, classBlock,
				out var notificationProperties);
			EmitProperties (bindingContext, classBlock);

			// emit the notification helper classes, leave this for the very bottom of the class
			EmitNotifications (notificationProperties, classBlock);
			classBlock.WriteLine ("// TODO: add binding code here");
		}
		return true;
	}
}
