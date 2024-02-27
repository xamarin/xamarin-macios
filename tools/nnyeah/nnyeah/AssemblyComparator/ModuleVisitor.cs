using System;
using System.IO;
using Mono.Cecil;
using System.Collections.Generic;

#nullable enable

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	public class ModuleVisitor {
		ModuleDefinition Module;
		bool PublicOnly;
		public ModuleVisitor (ModuleDefinition module, bool publicOnly)
		{
			Module = module;
			PublicOnly = publicOnly;
		}

		public ModuleVisitor (Stream stm, bool publicOnly)
			: this (ModuleDefinition.ReadModule (stm), publicOnly)
		{
		}


		public void Visit ()
		{
			VisitTypes (Module.Types);
		}

		void VisitTypes (IEnumerable<TypeDefinition> types)
		{
			foreach (var type in types) {
				VisitType (type);
			}
		}

		void VisitType (TypeDefinition type)
		{
			if (PublicOnly && !type.IsPublic)
				return;
			TypeVisited.Invoke (this, new TypeVisitedEventArgs (VisitKind.Start, type));

			if (type.NestedTypes.Count > 0)
				VisitTypes (type.NestedTypes);
			VisitFields (type);
			VisitEvents (type);
			VisitProperties (type);
			VisitMethods (type);

			TypeVisited.Invoke (this, new TypeVisitedEventArgs (VisitKind.End, type));
		}

		void VisitFields (TypeDefinition type)
		{
			foreach (var field in type.Fields)
				VisitField (type, field);
		}

		void VisitField (TypeDefinition type, FieldDefinition field)
		{
			if (PublicOnly && !field.IsPublic)
				return;

			FieldVisited.Invoke (this, new MemberVisitedEventArgs<FieldDefinition> (VisitKind.Start, type, field));
			FieldVisited.Invoke (this, new MemberVisitedEventArgs<FieldDefinition> (VisitKind.End, type, field));
		}

		void VisitProperties (TypeDefinition type)
		{
			foreach (var prop in type.Properties) {
				VisitProperty (type, prop);
			}
		}

		void VisitProperty (TypeDefinition type, PropertyDefinition prop)
		{
			if (PublicOnly && !(PublicGetterExists (prop) || PublicSetterExists (prop)))
				return;
			PropertyVisited (this, new MemberVisitedEventArgs<PropertyDefinition> (VisitKind.Start, type, prop));
			PropertyVisited (this, new MemberVisitedEventArgs<PropertyDefinition> (VisitKind.End, type, prop));
		}

		bool PublicGetterExists (PropertyDefinition prop)
		{
			return MethodExistsAndIsPublic (prop.GetMethod);
		}

		bool PublicSetterExists (PropertyDefinition prop)
		{
			return MethodExistsAndIsPublic (prop.SetMethod);
		}

		bool MethodExistsAndIsPublic (MethodDefinition? meth)
		{
			return meth is not null && meth.IsPublic;
		}

		void VisitEvents (TypeDefinition type)
		{
			foreach (var evt in type.Events) {
				VisitEvent (type, evt);
			}
		}

		void VisitEvent (TypeDefinition type, EventDefinition evt)
		{
			var method = evt.AddMethod ?? evt.RemoveMethod ?? evt.InvokeMethod;
			if (method is null)
				return;
			if (PublicOnly && !method.IsPublic)
				return;

			EventVisited.Invoke (this, new MemberVisitedEventArgs<EventDefinition> (VisitKind.Start, type, evt));
			EventVisited.Invoke (this, new MemberVisitedEventArgs<EventDefinition> (VisitKind.End, type, evt));
		}

		void VisitMethods (TypeDefinition type)
		{
			foreach (var method in type.Methods) {
				VisitMethod (type, method);
			}
		}

		void VisitMethod (TypeDefinition type, MethodDefinition method)
		{
			if (PublicOnly && !method.IsPublic)
				return;
			MethodVisited (this, new MemberVisitedEventArgs<MethodDefinition> (VisitKind.Start, type, method));
			MethodVisited (this, new MemberVisitedEventArgs<MethodDefinition> (VisitKind.End, type, method));
		}

		public EventHandler<TypeVisitedEventArgs> TypeVisited = (s, e) => { };
		public EventHandler<MemberVisitedEventArgs<MethodDefinition>> MethodVisited = (s, e) => { };
		public EventHandler<MemberVisitedEventArgs<FieldDefinition>> FieldVisited = (s, e) => { };
		public EventHandler<MemberVisitedEventArgs<EventDefinition>> EventVisited = (s, e) => { };
		public EventHandler<MemberVisitedEventArgs<PropertyDefinition>> PropertyVisited = (s, e) => { };
	}
}
