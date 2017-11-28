/*
 * Shared unit tests between XI and XM.
 **/

using System;
using System.IO;
using System.Xml;
using System.Text;

using NUnit.Framework;

namespace Xamarin.Tests
{
	[TestFixture]
	public class ProductTests
	{
		[Test]
		public void MonoVersion ()
		{
			// Verify that the mono version is in the Versions.plist, and that it's a parsable version number.
			var settings = new XmlReaderSettings () {
				XmlResolver = null,
				DtdProcessing = DtdProcessing.Parse
			};

			var plist = Path.Combine (Configuration.SdkRoot, "Versions.plist");
			var xml = new XmlDocument ();
			using (var sr = new StreamReader (plist, Encoding.UTF8, true))
			using (var reader = XmlReader.Create (sr, settings)) {
				xml.Load (reader);
				var version = xml.SelectSingleNode ("//dict/key[text()='MonoVersion']")?.NextSibling?.InnerText;
				Assert.DoesNotThrow (() => Version.Parse (version), "version");
			}
		}
	}
}

