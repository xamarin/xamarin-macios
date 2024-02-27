//
// CGImageMetadataTag Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using ImageIO;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.ImageIO {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImageMetadataTagTest {

		NSString nspace = CGImageMetadataTagNamespaces.Exif;
		NSString prefix = CGImageMetadataTagPrefixes.Exif;
		NSString name = new NSString ("tagName");

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static int CFGetRetainCount (IntPtr handle);

		[Test]
		public void Ctor_Null ()
		{
			Assert.Throws<ArgumentNullException> (delegate { new CGImageMetadataTag (null, prefix, name, CGImageMetadataType.Default, name); }, "1");
			Assert.Throws<ArgumentNullException> (delegate { new CGImageMetadataTag (nspace, prefix, null, CGImageMetadataType.Default, name); }, "3");
			Assert.Throws<ArgumentNullException> (delegate { new CGImageMetadataTag (nspace, prefix, null, CGImageMetadataType.Default, null); }, "4");
		}

		[Test]
		public void Ctor_NSString ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var value = new NSString ("value"))
			using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, value)) {
				Assert.That (tag.Name.ToString (), Is.EqualTo ("tagName"), "Name");
				Assert.That (tag.Namespace.ToString (), Is.EqualTo ("http://ns.adobe.com/exif/1.0/"), "Namespace");
				Assert.That (tag.Prefix.ToString (), Is.EqualTo ("exif"), "Prefix");
				Assert.That (tag.Type, Is.EqualTo (CGImageMetadataType.String), "Type");
				Assert.That (tag.Value.ToString (), Is.EqualTo ("value"), "Value");
				Assert.Null (tag.GetQualifiers (), "GetQualifiers");
			}
		}

		[Test]
		public void Ctor_NSNumber ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var value = NSNumber.FromByte (255))
			using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, value)) {
				Assert.That (tag.Name.ToString (), Is.EqualTo ("tagName"), "Name");
				Assert.That (tag.Namespace.ToString (), Is.EqualTo ("http://ns.adobe.com/exif/1.0/"), "Namespace");
				Assert.That (tag.Prefix.ToString (), Is.EqualTo ("exif"), "Prefix");
				Assert.That (tag.Type, Is.EqualTo (CGImageMetadataType.String), "Type");
				Assert.That (tag.Value.ToString (), Is.EqualTo ("255"), "Value");
				Assert.Null (tag.GetQualifiers (), "GetQualifiers");
			}
		}

		[Test]
		public void Ctor_NSArray ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var value = NSArray.FromNSObjects (nspace, prefix, name))
			using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, value)) {
				Assert.That (tag.Name.ToString (), Is.EqualTo ("tagName"), "Name");
				Assert.That (tag.Namespace.ToString (), Is.EqualTo ("http://ns.adobe.com/exif/1.0/"), "Namespace");
				Assert.That (tag.Prefix.ToString (), Is.EqualTo ("exif"), "Prefix");
				Assert.That (tag.Type, Is.EqualTo (CGImageMetadataType.ArrayOrdered), "Type");
				// an NSArray before iOS 10, NSMutableArray then
				Assert.That (tag.Value, Is.InstanceOf<NSArray> (), "Value");
				Assert.Null (tag.GetQualifiers (), "GetQualifiers");
			}
		}

		[Test]
		public void Ctor_NSDictionary ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var value = NSDictionary.FromObjectAndKey (name, prefix))
			using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, value)) {
				Assert.That (tag.Name.ToString (), Is.EqualTo ("tagName"), "Name");
				Assert.That (tag.Namespace.ToString (), Is.EqualTo ("http://ns.adobe.com/exif/1.0/"), "Namespace");
				Assert.That (tag.Prefix.ToString (), Is.EqualTo ("exif"), "Prefix");
				Assert.That (tag.Type, Is.EqualTo (CGImageMetadataType.Structure), "Type");
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.That (tag.Value, Is.TypeOf<NSDictionary> (), "Value");
				} else {
					Assert.That (tag.Value, Is.TypeOf<NSMutableDictionary> (), "Value");
				}
				Assert.Null (tag.GetQualifiers (), "GetQualifiers");
			}
		}

		[Test]
		public void Ctor_Bool_True ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, true)) {
				Assert.That (CFGetRetainCount (tag.Handle), Is.EqualTo (1), "RetainCount");

				Assert.That (tag.Name.ToString (), Is.EqualTo ("tagName"), "Name");
				Assert.That (tag.Namespace.ToString (), Is.EqualTo ("http://ns.adobe.com/exif/1.0/"), "Namespace");
				Assert.That (tag.Prefix.ToString (), Is.EqualTo ("exif"), "Prefix");
				Assert.That (tag.Type, Is.EqualTo (CGImageMetadataType.String), "Type");
				Assert.That (tag.Value.ToString (), Is.EqualTo ("True"), "Value");
				Assert.Null (tag.GetQualifiers (), "GetQualifiers");
			}
		}

		[Test]
		public void Ctor_Bool_False ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			var rc = name.RetainCount;
			using (var tag = new CGImageMetadataTag (nspace, prefix, name, CGImageMetadataType.Default, false)) {
				var n = tag.Name;
				Assert.That (n.Handle, Is.EqualTo (name.Handle), "same");
				Assert.That (n.ToString (), Is.EqualTo ("tagName"), "Name");
				Assert.That (tag.Namespace.ToString (), Is.EqualTo ("http://ns.adobe.com/exif/1.0/"), "Namespace");
				Assert.That (tag.Prefix.ToString (), Is.EqualTo ("exif"), "Prefix");
				Assert.That (tag.Type, Is.EqualTo (CGImageMetadataType.String), "Type");
				Assert.That (tag.Value.ToString (), Is.EqualTo ("False"), "Value");
				Assert.Null (tag.GetQualifiers (), "GetQualifiers");
			}
		}
	}
}
