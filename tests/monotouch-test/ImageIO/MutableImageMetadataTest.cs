//
// CGMutableImageMetadata
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ImageIO;
using ObjCRuntime;
using NUnit.Framework;

using CGImageProperties = ImageIO.CGImageProperties;

namespace MonoTouchFixtures.ImageIO {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableImageMetadataTest {

		NSString nspace = CGImageMetadataTagNamespaces.Exif;
		NSString prefix = CGImageMetadataTagPrefixes.Exif;
		NSString name = new NSString ("tagName");
		NSString path = new NSString ("exif:Flash.Fired");

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var meta = new CGMutableImageMetadata ()) {
				Console.WriteLine (meta);
				NSError err;
				Assert.True (meta.RegisterNamespace (CGImageMetadataTagNamespaces.Exif, CGImageMetadataTagPrefixes.Exif, out err), "RegisterNamespace");
				Assert.Null (err, "NSError");

				// nothing to see at this stage
				using (var data = meta.CreateXMPData ()) {
					Assert.Null (data, "CreateXMPData-1");
				}

				using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, true)) {
					Assert.True (meta.SetTag (null, path, tag), "SetTag");
				}

				// now we're talking
				using (var data = meta.CreateXMPData ()) {
					Assert.NotNull (data, "CreateXMPData-2");
				}

				Assert.True (meta.SetValue (null, path, false), "SetValue");

				Assert.True (meta.SetValueMatchingImageProperty (CGImageProperties.ExifDictionary, CGImageProperties.ExifDateTimeOriginal, (NSDate) DateTime.Now), "SetValueMatchingImageProperty");
			}
		}
	}
}
