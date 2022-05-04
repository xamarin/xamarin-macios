using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using System.Xml.Serialization;

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
	}
}
