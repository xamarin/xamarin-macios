//
// Unit tests for CGImageMetadata
//

using System.IO;

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using CoreGraphics;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CGImageTest {

		CGImageMetadata SampleMetadata ()
		{
			var xmpString = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"XMP Core 5.4.0\"><rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"><rdf:Description rdf:about=\"\" xmlns:apple_desktop=\"http://ns.apple.com/namespace/1.0/\"><apple_desktop:solar>123</apple_desktop:solar></rdf:Description></rdf:RDF></x:xmpmeta>";
			var xmpPointer = Marshal.StringToHGlobalAnsi (xmpString);
			var xmpData = new NSData (xmpPointer);
			return new CGImageMetadata (xmpData);
		}

		[Test]
		public void EnumerateMetadata ()
		{
			var data = SampleMetadata ();
			var keys = new List<NSString> ();
			var tags = new List<CGImageMetadataTag> ();
			data.EnumerateTags (null, (key, tag) => {
				keys.Add (key);
				tags.Add (tags);
				return true;
			});
			Assert.AreEqual (2, keys.Count (), "key count mismatch");
			Assert.AreEqual (2, tags.Count (), "tag count mistmatch");
		}
	}
}
