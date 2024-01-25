using System;
using System.Collections.Generic;

using Mono.Cecil;

#nullable enable

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	public class ComparingVisitor {
		ModuleDefinition EarlierModule, LaterModule;
		bool PublicOnly;
		NNyeahAssemblyResolver Resolver;

		public ComparingVisitor (ModuleDefinition earlierModule, ModuleDefinition laterModule, NNyeahAssemblyResolver resolver, bool publicOnly)
		{
			EarlierModule = earlierModule;
			LaterModule = laterModule;
			PublicOnly = publicOnly;
			Resolver = resolver;
		}

		public void Visit ()
		{
			var earlierElements = ModuleElements.Import (EarlierModule, Resolver, PublicOnly);
			if (earlierElements is null)
				throw new Exception (Errors.E0007);

			// why is PublicOnly being ignored here?
			// The reason is that in the earlier assembly, you might
			// have a constructor of the form:
			// public SomeObject (IntPtr handle) { }
			// but in the later assembly it will be:
			// protected SomeObject (IntPtr handle) { }
			// If we use PublicOnly here, this will not work.
			var laterElements = ModuleElements.Import (LaterModule, Resolver, publicOnly: false);
			if (laterElements is null)
				throw new Exception (Errors.E0007);
			var reworker = new TypeReworker (EarlierModule);
			VisitTypes (reworker, earlierElements, laterElements);
		}

		void VisitTypes (TypeReworker reworker, ModuleElements earlier, ModuleElements later)
		{
			foreach (var typeNameAndValue in earlier.Types) {
				if (!later.Types.TryGetValue (typeNameAndValue.Key, out var laterElems)) {
					TypeEvents.InvokeNotFound (this, typeNameAndValue.Key);
					continue;
				}

				TypeEvents.InvokeFound (this, typeNameAndValue.Key, laterElems.DeclaringType);

				VisitAllMembers (reworker, typeNameAndValue.Value, laterElems);
			}
		}

		void VisitAllMembers (TypeReworker reworker, TypeElements earlier, TypeElements later)
		{
			VisitMembers (reworker, earlier.Methods, later.Methods, MethodEvents);
			VisitMembers (reworker, earlier.Fields, later.Fields, FieldEvents);
			VisitMembers (reworker, earlier.Events, later.Events, EventEvents);
			VisitMembers (reworker, earlier.Properties, later.Properties, PropertyEvents);
		}

		void VisitMembers<T> (TypeReworker reworker, List<TypeElement<T>> earlier,
			List<TypeElement<T>> later, ItemEvents<T> events) where T : IMemberDefinition
		{
			foreach (var earlierElem in earlier) {
				VisitMember (reworker, earlierElem, later, events);
			}
		}

		void VisitMember<T> (TypeReworker reworker, TypeElement<T> elem,
			List<TypeElement<T>> later, ItemEvents<T> events) where T : IMemberDefinition
		{
			foreach (var late in later) {
				if (elem.Signature == late.Signature) {
					events.InvokeFound (this, elem.Signature, late.Element);
					return;
				}
			}
			var remappedSig = RemappedSignature (reworker, elem.Element);
			if (remappedSig == elem.Signature) {
				events.InvokeNotFound (this, elem.Signature);
				return;
			}
			foreach (var late in later) {
				if (remappedSig == late.Signature) {
					events.InvokeFound (this, elem.Signature, late.Element);
					return;
				}
			}
			events.InvokeNotFound (this, elem.Signature);
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

		public ItemEvents<TypeDefinition> TypeEvents { get; } = new ();
		public ItemEvents<MethodDefinition> MethodEvents { get; } = new ();
		public ItemEvents<FieldDefinition> FieldEvents { get; } = new ();
		public ItemEvents<EventDefinition> EventEvents { get; } = new ();
		public ItemEvents<PropertyDefinition> PropertyEvents { get; } = new ();
	}
}
