//
// Link All Xml Serialization Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Foundation;

using NUnit.Framework;

namespace LinkAll.Serialization.Xml {

	[Serializable]
	[XmlRoot ("rsp", IsNullable = false)]
	public class XmlResult<T> {
		[XmlAttribute ("stat")]
		public int StatusCode { get; set; }

		[XmlElement ("photos")]
		[XmlElement ("photosets")]
		public T Result { get; set; }
	}

	[Serializable]
	[XmlRoot ("rsp")]
	public class XmlField<T> {
		[XmlElement ("photos")]
		[XmlElement ("photosets")]
		public T Result;
	}

#if NET
	[Ignore ("https://github.com/dotnet/runtime/issues/41389")]
#endif
	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class XmlSerializationTest {

		[Test]
		public void GenericProperty_Bug5543 ()
		{
			XmlResult<string> r = new XmlResult<string> ();
			r.Result = "5543";
			r.StatusCode = 10;

			var serializer = new XmlSerializer (typeof (XmlResult<string>));
			using (var ms = new MemoryStream ()) {
				serializer.Serialize (ms, r);
				r.Result = String.Empty;
				r.StatusCode = -1;

				ms.Position = 0;
				XmlResult<string> back = (XmlResult<string>) serializer.Deserialize (ms);

				Assert.That (back.Result, Is.EqualTo ("5543"), "Result");
				Assert.That (back.StatusCode, Is.EqualTo (10), "StatusCode");
			}
		}

		[Test]
		public void GenericField ()
		{
			XmlField<string> f = new XmlField<string> ();
			f.Result = "5543";
			// not valid for serialization but the linker should not have issue with that
		}
	}
}
