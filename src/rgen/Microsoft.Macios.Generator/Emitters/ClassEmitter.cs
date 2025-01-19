// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Formatters;
using ObjCBindings;
using Property = Microsoft.Macios.Generator.DataModel.Property;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;

namespace Microsoft.Macios.Generator.Emitters;

class ClassEmitter : ICodeEmitter {
	public string GetSymbolName (in CodeChanges codeChanges) => codeChanges.Name;

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


	void EmitDefaultConstructors (in BindingContext bindingContext, TabbedStringBuilder classBlock, bool disableDefaultCtor)
	{

		if (!disableDefaultCtor) {
			classBlock.AppendGeneratedCodeAttribute ();
			classBlock.AppendDesignatedInitializer ();
			classBlock.AppendRaw (
$@"[Export (""init"")]
public {bindingContext.Changes.Name} () : base (NSObjectFlag.Empty)
{{
	if (IsDirectBinding)
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
	else
		InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.Selector.GetHandle (""init"")), ""init"");
}}
");
			classBlock.AppendLine ();
		}

		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.AppendLine ($"protected {bindingContext.Changes.Name} (NSObjectFlag t) : base (t) {{}}");

		classBlock.AppendLine ();
		classBlock.AppendGeneratedCodeAttribute ();
		classBlock.AppendEditorBrowsableAttribute (EditorBrowsableState.Advanced);
		classBlock.AppendLine ($"protected internal {bindingContext.Changes.Name} (NativeHandle handle) : base (handle) {{}}");
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
	void EmitFields (string className, in ImmutableArray<Property> properties, TabbedStringBuilder classBlock,
		out ImmutableArray<Property> notificationProperties)
	{
		var notificationsBuilder = ImmutableArray.CreateBuilder<Property> ();
		foreach (var property in properties.OrderBy (p => p.Name)) {
			if (!property.IsField)
				continue;

			classBlock.AppendLine ();
			// a field should always have a getter, if it does not, we do not generate the property
			var getter = property.GetAccessor (AccessorKind.Getter);
			if (getter is null)
				continue;

			// provide a backing variable for the property if and only if we are dealing with a reference type
			if (property.IsReferenceType) {
				classBlock.AppendLine (FieldPropertyBackingVariable (property).ToString ());
			}

			classBlock.AppendLine ();
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

				// be very verbose with the availability, makes the life easier to the dotnet analyzer
				propertyBlock.AppendMemberAvailability (getter.Value.SymbolAvailability);
				using (var getterBlock = propertyBlock.CreateBlock ("get", block: true)) {
					getterBlock.AppendLine ("throw new NotImplementedException ();");
				}

				var setter = property.GetAccessor (AccessorKind.Setter);
				if (setter is null)
					// we are done with the current property
					continue;

				propertyBlock.AppendLine (); // add space between getter and setter since we have the attrs
				propertyBlock.AppendMemberAvailability (setter.Value.SymbolAvailability);
				using (var setterBlock = propertyBlock.CreateBlock ("set", block: true)) {
					setterBlock.AppendLine ("throw new NotImplementedException ();");
				}
			}
		}
		notificationProperties = notificationsBuilder.ToImmutable ();
	}

	void EmitNotifications (in ImmutableArray<Property> properties, TabbedStringBuilder classBlock)
	{
		// to be implemented, do not throw or tests will fail.
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
		bindingContext.Builder.AppendLine ();
		bindingContext.Builder.AppendLine ($"namespace {string.Join (".", bindingContext.Changes.Namespace)};");
		bindingContext.Builder.AppendLine ();

		// register the class only if we are not dealing with a static class
		var bindingData = (BindingTypeData<Class>) bindingContext.Changes.BindingInfo;
		// Registration depends on the class name. If the binding data contains a name, we use that one, else
		// we use the name of the class
		var registrationName = bindingData.Name ?? bindingContext.Changes.Name;

		if (!bindingContext.Changes.IsStatic) {
			bindingContext.Builder.AppendLine ($"[Register (\"{registrationName}\", true)]");
		}
		var modifiers = $"{string.Join (' ', bindingContext.Changes.Modifiers)} ";
		using (var classBlock = bindingContext.Builder.CreateBlock ($"{(string.IsNullOrWhiteSpace (modifiers) ? string.Empty : modifiers)}class {bindingContext.Changes.Name}", true)) {
			if (!bindingContext.Changes.IsStatic) {
				classBlock.AppendGeneratedCodeAttribute (optimizable: true);
				classBlock.AppendLine ($"static readonly NativeHandle class_ptr = Class.GetHandle (\"{registrationName}\");");
				classBlock.AppendLine ();
				classBlock.AppendLine ("public override NativeHandle ClassHandle => class_ptr;");
				classBlock.AppendLine ();

				EmitDefaultConstructors (bindingContext: bindingContext,
					classBlock: classBlock,
					disableDefaultCtor: bindingData.Flags.HasFlag (Class.DisableDefaultCtor));
			}

			EmitFields (bindingContext.Changes.Name, bindingContext.Changes.Properties, classBlock,
				out var notificationProperties);

			// emit the notification helper classes, leave this for the very bottom of the class
			EmitNotifications (notificationProperties, classBlock);
			classBlock.AppendLine ("// TODO: add binding code here");
		}
		return true;
	}
}
