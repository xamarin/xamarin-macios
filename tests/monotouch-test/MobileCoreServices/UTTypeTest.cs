//
// Unit tests for UTType
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012, 2015 Xamarin Inc. All rights reserved.
//

using System;

#if XAMCORE_2_0
using Foundation;
using MobileCoreServices;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.MobileCoreServices;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MobileCoreServices {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UTTypeTest {
		
		[Test]
		public void NSStringConstants ()
		{
			Assert.NotNull (UTType.ExportedTypeDeclarationsKey, "ExportedTypeDeclarationsKey");
			Assert.NotNull (UTType.ImportedTypeDeclarationsKey, "ImportedTypeDeclarationsKey");
			Assert.NotNull (UTType.IdentifierKey, "IdentifierKey");
			Assert.NotNull (UTType.TagSpecificationKey, "TagSpecificationKey");
			Assert.NotNull (UTType.ConformsToKey, "ConformsToKey");
			Assert.NotNull (UTType.DescriptionKey, "DescriptionKey");
			Assert.NotNull (UTType.IconFileKey, "IconFileKey");
			Assert.NotNull (UTType.ReferenceURLKey, "ReferenceURLKey");
			Assert.NotNull (UTType.VersionKey, "VersionKey");

			Assert.NotNull (UTType.TagClassFilenameExtension, "TagClassFilenameExtension");
			Assert.NotNull (UTType.TagClassMIMEType, "TagClassMIMEType");

			Assert.NotNull (UTType.Item, "Item");
			Assert.NotNull (UTType.Content, "Content");
			Assert.NotNull (UTType.CompositeContent, "CompositeContent");
			Assert.NotNull (UTType.Application, "Application");
			Assert.NotNull (UTType.Message, "Message");
			Assert.NotNull (UTType.Contact, "Contact");
			Assert.NotNull (UTType.Archive, "Archive");
			Assert.NotNull (UTType.DiskImage, "DiskImage");

			Assert.NotNull (UTType.Data, "Data");
			Assert.NotNull (UTType.Directory, "Directory");
			Assert.NotNull (UTType.Resolvable, "Resolvable");
			Assert.NotNull (UTType.SymLink, "SymLink");
			Assert.NotNull (UTType.MountPoint, "MountPoint");
			Assert.NotNull (UTType.AliasFile, "AliasFile");
			Assert.NotNull (UTType.AliasRecord, "AliasRecord");
			Assert.NotNull (UTType.URL, "URL");
			Assert.NotNull (UTType.FileURL, "FileURL");

			Assert.NotNull (UTType.Text, "Text");
			Assert.NotNull (UTType.PlainText, "PlainText");
			Assert.NotNull (UTType.UTF8PlainText, "UTF8PlainText");
			Assert.NotNull (UTType.UTF16ExternalPlainText, "UTF16ExternalPlainText");
			Assert.NotNull (UTType.UTF16PlainText, "UTF16PlainText");
			Assert.NotNull (UTType.RTF, "RTF");
			Assert.NotNull (UTType.HTML, "HTML");
			Assert.NotNull (UTType.XML, "XML");
			Assert.NotNull (UTType.SourceCode, "SourceCode");
			Assert.NotNull (UTType.CSource, "CSource");
			Assert.NotNull (UTType.ObjectiveCSource, "ObjectiveCSource");
			Assert.NotNull (UTType.CPlusPlusSource, "CPlusPlusSource");
			Assert.NotNull (UTType.ObjectiveCPlusPlusSource, "ObjectiveCPlusPlusSource");
			Assert.NotNull (UTType.CHeader, "CHeader");
			Assert.NotNull (UTType.CPlusPlusHeader, "CPlusPlusHeader");
			Assert.NotNull (UTType.JavaSource, "JavaSource");

			Assert.NotNull (UTType.PDF, "PDF");
			Assert.NotNull (UTType.RTFD, "RTFD");
			Assert.NotNull (UTType.FlatRTFD, "FlatRTFD");
			Assert.NotNull (UTType.TXNTextAndMultimediaData, "TXNTextAndMultimediaData");
			Assert.NotNull (UTType.WebArchive, "WebArchive");

			Assert.NotNull (UTType.Image, "Image");
			Assert.NotNull (UTType.JPEG, "JPEG");
			Assert.NotNull (UTType.JPEG2000, "JPEG2000");
			Assert.NotNull (UTType.TIFF, "TIFF");
			Assert.NotNull (UTType.GIF, "GIF");
			Assert.NotNull (UTType.PNG, "PNG");
			Assert.NotNull (UTType.QuickTimeImage, "QuickTimeImage");
			Assert.NotNull (UTType.AppleICNS, "AppleICNS");
			Assert.NotNull (UTType.BMP, "BMP");
			Assert.NotNull (UTType.ICO, "ICO");

			Assert.NotNull (UTType.AudiovisualContent, "AudiovisualContent");
			Assert.NotNull (UTType.Movie, "Movie");
			Assert.NotNull (UTType.Video, "Video");
			Assert.NotNull (UTType.Audio, "Audio");
			Assert.NotNull (UTType.QuickTimeMovie, "QuickTimeMovie");
			Assert.NotNull (UTType.MPEG, "MPEG");
			Assert.NotNull (UTType.MPEG4, "MPEG4");
			Assert.NotNull (UTType.MP3, "MP3");
			Assert.NotNull (UTType.MPEG4Audio, "MPEG4Audio");
			Assert.NotNull (UTType.AppleProtectedMPEG4Audio, "AppleProtectedMPEG4Audio");

			Assert.NotNull (UTType.Folder, "Folder");
			Assert.NotNull (UTType.Volume, "Volume");
			Assert.NotNull (UTType.Package, "Package");
			Assert.NotNull (UTType.Bundle, "Bundle");
			Assert.NotNull (UTType.Framework, "Framework");

			Assert.NotNull (UTType.ApplicationBundle, "ApplicationBundle");
			Assert.NotNull (UTType.ApplicationFile, "ApplicationFile");

			Assert.NotNull (UTType.VCard, "VCard");

			Assert.NotNull (UTType.InkText, "InkText");

			if (TestRuntime.CheckXcodeVersion (7, 0))
				Assert.NotNull (UTType.SwiftSource, "SwiftSource");
		}

		[Test]
		public void GetPreferredTag ()
		{
			Assert.NotNull (UTType.GetPreferredTag (UTType.PDF, UTType.TagClassFilenameExtension), "GetPreferredTag");
		}

		[Test]
		public void GetDeclaration ()
		{
			Assert.NotNull (UTType.GetDeclaration (UTType.PDF));
		}

		[Test]
		public void GetDeclaringBundleURL ()
		{
			Assert.NotNull (UTType.GetDeclaringBundleURL (UTType.PDF));
		}

		[Test]
		public void CreatePreferredIdentifier ()
		{
			string[] extensions = new [] { ".html", ".css", ".jpg", ".js", ".otf" };
			// random failure reported in #36708 (on some iPad2 only)
			for (int i=0; i < 100; i++) {
				foreach (var ext in extensions) {
					var result = UTType.CreatePreferredIdentifier (UTType.TagClassMIMEType, ext, null);
					Assert.NotNull (result, ext + i.ToString ());
				}
			}
		}

		[Test]
		public void Equals ()
		{
			Assert.True (UTType.Equals (null, null), "null-null");
			Assert.False (UTType.Equals (null, UTType.PDF), "null-PDF");
			Assert.False (UTType.Equals (UTType.PDF, null), "PDF-null");
			Assert.True (UTType.Equals (UTType.PDF, UTType.PDF), "PDF-PDF");
		}
	}
}
