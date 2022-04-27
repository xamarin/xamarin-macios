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
		public SerializableDictionary<string, string> TypeMap { get; init; } = new SerializableDictionary<string, string> ();

		public List<string> MethodsNotPresent { get; init; } = new List<string> ();
		public SerializableDictionary<string, string> MethodMap { get; init; } = new SerializableDictionary<string, string> ();

		public List<string> FieldsNotPresent { get; init; } = new List<string> ();
		public SerializableDictionary<string, string> FieldMap { get; init; } = new SerializableDictionary<string, string> ();

		public List<string> EventsNotPresent { get; init; } = new List<string> ();
		public SerializableDictionary<string, string> EventMap { get; init; } = new SerializableDictionary<string, string> ();

		public List<string> PropertiesNotPresent { get; init; } = new List<string> ();
		public SerializableDictionary<string, string> PropertyMap { get; init; } = new SerializableDictionary<string, string> ();

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
