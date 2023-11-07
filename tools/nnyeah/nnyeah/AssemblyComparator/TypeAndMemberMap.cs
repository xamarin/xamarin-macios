using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	[XmlRoot]
	public class TypeAndMemberMap {
		public List<string> TypesNotPresent { get; init; } = new List<string> ();
		public Dictionary<string, TypeDefinition> TypeMap { get; init; } = new Dictionary<string, TypeDefinition> ();

		public List<string> MethodsNotPresent { get; init; } = new List<string> ();
		public Dictionary<string, MethodDefinition> MethodMap { get; init; } = new Dictionary<string, MethodDefinition> ();

		public List<string> FieldsNotPresent { get; init; } = new List<string> ();
		public Dictionary<string, FieldDefinition> FieldMap { get; init; } = new Dictionary<string, FieldDefinition> ();

		public List<string> EventsNotPresent { get; init; } = new List<string> ();
		public Dictionary<string, EventDefinition> EventMap { get; init; } = new Dictionary<string, EventDefinition> ();

		public List<string> PropertiesNotPresent { get; init; } = new List<string> ();
		public Dictionary<string, PropertyDefinition> PropertyMap { get; init; } = new Dictionary<string, PropertyDefinition> ();

		public TypeAndMemberMap ()
		{
		}

		public bool TypeIsNotPresent (string typeName)
		{
			return TypesNotPresent.Contains (typeName);
		}

		public bool TryGetMappedType (string typeName, [NotNullWhen (returnValue: true)] out TypeDefinition? result)
		{
			return TypeMap.TryGetValue (typeName, out result);
		}

		public bool MemberIsNotPresent (string member)
		{
			return MethodsNotPresent.Contains (member) ||
				FieldsNotPresent.Contains (member) ||
				EventsNotPresent.Contains (member) ||
				PropertiesNotPresent.Contains (member);
		}

		public bool TryGetMappedMember (string memberName, [NotNullWhen (returnValue: true)] out IMemberDefinition? member)
		{
			if (MethodMap.TryGetValue (memberName, out var method)) {
				member = method;
				return true;
			} else if (FieldMap.TryGetValue (memberName, out var field)) {
				member = field;
				return true;
			} else if (EventMap.TryGetValue (memberName, out var @event)) {
				member = @event;
				return true;
			} else if (PropertyMap.TryGetValue (memberName, out var property)) {
				member = property;
				return true;
			}
			member = null;
			return false;
		}
	}
}
