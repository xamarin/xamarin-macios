using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	[XmlRoot]
	public class TypeAndMemberMap {
		public List<string> TypesNotPresent { get; init; } = new List<string> ();
		public UniquingStringDictionary TypeMap { get; init; } = new UniquingStringDictionary ();

		public List<string> MethodsNotPresent { get; init; } = new List<string> ();
		public UniquingStringDictionary MethodMap { get; init; } = new UniquingStringDictionary ();

		public List<string> FieldsNotPresent { get; init; } = new List<string> ();
		public UniquingStringDictionary FieldMap { get; init; } = new UniquingStringDictionary ();

		public List<string> EventsNotPresent { get; init; } = new List<string> ();
		public UniquingStringDictionary EventMap { get; init; } = new UniquingStringDictionary ();

		public List<string> PropertiesNotPresent { get; init; } = new List<string> ();
		public UniquingStringDictionary PropertyMap { get; init; } = new UniquingStringDictionary ();

		public TypeAndMemberMap ()
		{
		}

		public void ToXml (string path)
		{
			using var stm = new FileStream (path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
			ToXml (stm);
		}

		public void ToXml (Stream stm)
		{
			var serializer = new XmlSerializer (typeof (TypeAndMemberMap));
			serializer.Serialize (stm, this);
		}

		public static TypeAndMemberMap? FromXml (string path)
		{
			using var stm = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return FromXml (stm);
		}

		public static TypeAndMemberMap? FromXml (Stream stm)
		{
			var serializer = new XmlSerializer (typeof (TypeAndMemberMap));
			return (TypeAndMemberMap?)serializer.Deserialize (stm);
		}
	}
}
