using System;
using System.Collections.Generic;
using Microsoft.MaciOS.Nnyeah;
using Mono.Cecil;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class ComparingVisitor {
		ModuleDefinition EarlierModule, LaterModule;
		bool PublicOnly;

		public ComparingVisitor (ModuleDefinition earlierModule, ModuleDefinition laterModule, bool publicOnly)
		{
			EarlierModule = earlierModule;
			LaterModule = laterModule;
			PublicOnly = publicOnly;
		}

		public void Visit ()
		{
			var earlierElements = ModuleElements.Import (EarlierModule, PublicOnly);
			if (earlierElements is null)
				throw new Exception (Errors.E0007);
			var laterElements = ModuleElements.Import (LaterModule, PublicOnly);
			if (laterElements is null)
				throw new Exception (Errors.E0007);
			var reworker = new TypeReworker (EarlierModule);
			VisitTypes (reworker, earlierElements, laterElements);
		}

		void VisitTypes (TypeReworker reworker, ModuleElements earlier, ModuleElements later)
		{
			foreach (var typeName in earlier.Types.Keys) {
				if (!later.Types.TryGetValue (typeName, out var laterElems)) {
					TypeNotFound.Invoke (this, new TypeNotFoundEventArgs (typeName));
					continue;
				} else {
					TypeFound.Invoke (this, new TypeFoundEventArgs (typeName, laterElems.DeclaringType.ToString ()));
				}
				if (!earlier.Types.TryGetValue (typeName, out var earlierElems)) {
					throw new Exception (Errors.E0007);
				}
				VisitAllMembers (reworker, earlierElems, laterElems);
			}
		}

		void VisitAllMembers (TypeReworker reworker, TypeElements earlier, TypeElements later)
		{
			VisitMembers (reworker, earlier.Methods, later.Methods);
			VisitMembers (reworker, earlier.Fields, later.Fields);
			VisitMembers (reworker, earlier.Events, later.Events);
			VisitMembers (reworker, earlier.Properties, later.Properties);
		}

		void VisitMembers<T> (TypeReworker reworker,
			List<TypeElement<T>> earlier, List<TypeElement<T>> later) where T : IMemberDefinition
		{
			foreach (var earlierElem in earlier) {
				VisitMember (reworker, earlierElem, later);
			}
		}

		void VisitMember<T> (TypeReworker reworker, TypeElement<T> elem,
			List<TypeElement<T>> later) where T : IMemberDefinition
		{
			foreach (var late in later) {
				if (elem.Signature == late.Signature) {
					FireElementFound (elem, late);
					return;
				}
			}
			var remappedSig = RemappedSignature (reworker, elem.Element);
			if (remappedSig == elem.Signature) {
				FireElementNotFound (elem);
				return;
			}
			foreach (var late in later) {
				if (remappedSig == late.Signature) {
					FireElementFound (elem, late);
					return;
				}
			}
			FireElementNotFound (elem);
		}

		void FireElementNotFound<T> (TypeElement<T> earlier) where T : IMemberDefinition
		{
			if (earlier is TypeElement<FieldDefinition> field) {
				FieldNotFound.Invoke (this, new FieldNotFoundEventArgs (field.Signature));
			} else if (earlier is TypeElement<MethodDefinition> method) {
				MethodNotFound.Invoke (this, new MethodNotFoundEventArgs (method.Signature));
			} else if (earlier is TypeElement<EventDefinition> @event) {
				EventNotFound.Invoke (this, new EventNotFoundEventArgs (@event.Signature));
			} else if (earlier is TypeElement<PropertyDefinition> property) {
				PropertyNotFound.Invoke (this, new PropertyNotFoundEventArgs (property.Signature));
			}
		}

		void FireElementFound<T> (TypeElement<T> earlier, TypeElement<T> later) where T : IMemberDefinition
		{
			if (earlier is TypeElement<FieldDefinition> field) {
				FieldFound.Invoke (this, new FieldFoundEventArgs (field.Signature, later.Signature));
			} else if (earlier is TypeElement<MethodDefinition> method) {
				MethodFound.Invoke (this, new MethodFoundEventArgs (method.Signature, later.Signature));
			} else if (earlier is TypeElement<EventDefinition> @event) {
				EventFound.Invoke (this, new EventFoundEventArgs (@event.Signature, later.Signature));
			} else if (earlier is TypeElement<PropertyDefinition> property) {
				PropertyFound.Invoke (this, new PropertyFoundEventArgs (earlier.Signature, later.Signature));
			}
		}

		static string RemappedSignature<T> (TypeReworker reworker, T elem) where T : IMemberDefinition
		{
			switch (elem) {
			case MethodDefinition method:
				return reworker.ReworkMethod (method).ToString ();
			case FieldDefinition field:
				return reworker.ReworkField (field).ToString ();
			case EventDefinition @event:
				return reworker.ReworkEvent (@event).ToString ();
			case PropertyDefinition property:
				return reworker.ReworkProperty (property).ToString ();
			default:
				throw new ArgumentException (nameof (elem));
			}
		}

		public EventHandler<TypeNotFoundEventArgs> TypeNotFound = (s, e) => { };
		public EventHandler<TypeFoundEventArgs> TypeFound = (s, e) => { };

		public EventHandler<MethodNotFoundEventArgs> MethodNotFound = (s, e) => { };
		public EventHandler<MethodFoundEventArgs> MethodFound = (s, e) => { };

		public EventHandler<FieldNotFoundEventArgs> FieldNotFound = (s, e) => { };
		public EventHandler<FieldFoundEventArgs> FieldFound = (s, e) => { };

		public EventHandler<EventNotFoundEventArgs> EventNotFound = (s, e) => { };
		public EventHandler<EventFoundEventArgs> EventFound = (s, e) => { };

		public EventHandler<PropertyNotFoundEventArgs> PropertyNotFound = (s, e) => { };
		public EventHandler<PropertyFoundEventArgs> PropertyFound = (s, e) => { };
	}
}
