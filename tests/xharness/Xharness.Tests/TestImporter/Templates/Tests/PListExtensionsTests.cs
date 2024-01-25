using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

using NUnit.Framework;

namespace Xharness.Tests.TestImporter.Templates.Tests {

	public class PListExtensionsTests {
		private XmlDocument plist;

		[SetUp]
		public void SetUp ()
		{
			plist = CreateResultSample ();
		}

		/// <summary>
		/// Creates a sample pList to be used with the tests and returns the temp file in which it was stored.
		/// </summary>
		/// <returns>The path where the sample plist can be found.</returns>
		XmlDocument CreateResultSample ()
		{
			var name = GetType ().Assembly.GetManifestResourceNames ()
				.FirstOrDefault (a => a.EndsWith ("Info.plist", StringComparison.Ordinal));
			var tempPath = Path.GetTempFileName ();
			var byteOrderMarkUtf8 = Encoding.UTF8.GetString (Encoding.UTF8.GetPreamble ()); // I hate BOM
			using var sampleStream = new StreamReader (GetType ().Assembly.GetManifestResourceStream (name));

			// create the document with the plist and return it
			var doc = new XmlDocument ();
			var settings = new XmlReaderSettings () {
				XmlResolver = null,
				DtdProcessing = DtdProcessing.Parse,
			};
			using var reader = XmlReader.Create (sampleStream, settings);
			doc.Load (reader);
			return doc;
		}

		[Test]
		public void SetMinimumOSVersion ()
		{
			var version = "MyMinVersion";
			plist.SetMinimumOSVersion (version);
			Assert.AreEqual (version, plist.GetMinimumOSVersion ());
		}

		[Test]
		public void SetNullMinimumOSVersion () => Assert.Throws<ArgumentNullException> (() => plist.SetMinimumOSVersion (null));

		[Test]
		public void SetMinimummacOSVersion ()
		{
			var version = "MyMaccMinVersion";
			plist.SetMinimummacOSVersion (version);
			Assert.AreEqual (version, plist.GetMinimummacOSVersion ());
		}

		[Test]
		public void SetNullMinimummacOSVersion () => Assert.Throws<ArgumentNullException> (() => plist.SetMinimummacOSVersion (null));

		[Test]
		public void SetCFBundleDisplayName ()
		{
			var displayName = "MySuperApp";
			plist.SetCFBundleDisplayName (displayName);
			Assert.AreEqual (displayName, plist.GetCFBundleDisplayName ());
		}

		[Test]
		public void SetNullCFBundleDisplayName () => Assert.Throws<ArgumentNullException> (() => plist.SetCFBundleDisplayName (null));

		[Test]
		public void SetCFBundleIdentifier ()
		{
			var bundleIdentifier = "my.company.super.app";
			plist.SetCFBundleIdentifier (bundleIdentifier);
			Assert.AreEqual (bundleIdentifier, plist.GetCFBundleIdentifier ());
		}

		[Test]
		public void SetNullCFBundleIdentifier () => Assert.Throws<ArgumentNullException> (() => plist.SetCFBundleIdentifier (null));

		[Test]
		public void SetCFBundleName ()
		{
			var bundleName = "MySuper.app";
			plist.SetCFBundleName (bundleName);
			Assert.AreEqual (bundleName, plist.GetCFBundleName ());
		}

		[Test]
		public void SetNullCFBundleName () => Assert.Throws<ArgumentNullException> (() => plist.SetCFBundleName (null));
	}
}
