/*
 * Shared unit tests between XI and XM.
 **/

using System;
using System.IO;

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
			var plist = Path.Combine (Configuration.SdkRoot, "Versions.plist");
			var xml = new System.Xml.XmlDocument ();
			xml.Load (plist);
			var version = xml.SelectSingleNode ("//dict/key[text()='MonoVersion']")?.NextSibling?.InnerText;
			Assert.DoesNotThrow (() => Version.Parse (version), "version");
		}
	}
}

