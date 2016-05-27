// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MonoMac.AppKit;

// Test
// * application use XML serialization
// * the [Xml*Attribute] also serve as a [Preserve] attribute
//
// Requirement
// * Link All must be enabled

namespace Xamarin.Mac.Linker.Test {

	public class SerializeMe {

		[XmlAttribute]
		public int SetMe { get; set; }

		public SerializeMe ()
		{
			SetMe = 1;
		}
	}

	class XmlSerialization {

		const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
			"<SerializeMe xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" SetMe=\"2\">" +
			"</SerializeMe>";

		static void Main (string[] args)
		{
			NSApplication.Init ();

			Test.EnsureLinker (true);

			bool success = false;

			try {
				using (var sr = new StringReader (xml))
				using (var xr = new XmlTextReader (sr)) {
					var xs = new XmlSerializer (typeof (SerializeMe));
					SerializeMe item = xs.Deserialize (xr) as SerializeMe;
					success = item.SetMe == 2;
				}
			}
			finally {
				Test.Log.WriteLine ("{0}\tXmlSerialization", success ? "[PASS]" : "[FAIL]");
				Test.Terminate ();
			}
		}
	}
}