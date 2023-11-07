using System;

using Foundation;
using AppKit;
using ProtoBuf;
using System.IO;

namespace Protobuf_Test {
	public partial class AppDelegate : NSApplicationDelegate {
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			ProtoTest.DoTest ();
		}
	}

	static class ProtoTest {
		[ProtoContract]
		class Person {
			[ProtoMember (1)]
			public int Id { get; set; }
			[ProtoMember (2)]
			public string Name { get; set; }
			[ProtoMember (3)]
			public Address Address { get; set; }
		}
		[ProtoContract]
		class Address {
			[ProtoMember (1)]
			public string Line1 { get; set; }
			[ProtoMember (2)]
			public string Line2 { get; set; }
		}

		public static void DoTest ()
		{
			var person = new Person {
				Id = 12345,
				Name = "Fred",
				Address = new Address {
					Line1 = "Flat 1",
					Line2 = "The Meadows"
				}
			};
			using (var file = File.Create ("/tmp/person.bin")) {
				Serializer.Serialize (file, person);
			}

			Person newPerson;
			using (var file = File.OpenRead ("/tmp/person.bin")) {
				newPerson = Serializer.Deserialize<Person> (file);
			}

			var fileName = "../../../../../TestResult.txt";
			if (File.Exists (fileName))
				File.Delete (fileName);

			using (TextWriter writer = File.CreateText (fileName)) {
				writer.WriteLine (newPerson.Name == "Fred" ? "Test Passed" : "Test Failed");
			}

			Environment.Exit (0);
		}
	}
}
