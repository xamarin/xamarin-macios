using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using MonoMac.AppKit;
using MonoMac.Foundation;

// Test
// * application roundtrip some data using DataContract
// * Adapted from iOS linkall.app, based on bug #11135
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	[DataContract(Namespace = "mb")][Flags] 
	public enum SomeTypes {
		[Preserve (AllMembers = true)][EnumMember] None = 0,
		[Preserve (AllMembers = true)][EnumMember] Image = 1,
		[Preserve (AllMembers = true)][EnumMember] Audio = 2,
		[Preserve (AllMembers = true)][EnumMember] Video = 4,
		[Preserve (AllMembers = true)][EnumMember] Document = 8
	}

	public class TestClass
	{
		public TestClass (SomeTypes types)
		{
			Types = types;
		}

		public SomeTypes Types { get; set; }
	}

	class DataContract {

		public static string ToXml<T> (T obj)
		{
			var sb = new StringBuilder();
			using (var x = XmlWriter.Create (sb, new XmlWriterSettings ())) {
				var s = new DataContractSerializer (typeof (T));
				s.WriteObject(x, obj);
			}
			return sb.ToString();
		}

		public static T FromXml<T> (string xml)
		{
			using (var r = XmlReader.Create (new StringReader (xml))) {
				var s = new DataContractSerializer (typeof (T));
				return (T) s.ReadObject (r);
			}
		}

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			var t1 = new TestClass (SomeTypes.Audio | SomeTypes.Image);
			var st = ToXml<TestClass> (t1);
			var t2 = FromXml<TestClass> (st);
			bool success = t2.Types == t1.Types;

			Test.Log.WriteLine ("[{0}]\tDataContract roundtrip", success ? "PASS" : "FAIL");

			Test.Terminate ();
		}
	}
}