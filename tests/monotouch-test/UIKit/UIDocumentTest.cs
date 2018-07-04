//
// Unit tests for UIDocument
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.IO;
using System.Reflection;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
using MonoTouchException=ObjCRuntime.RuntimeException;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouchException=MonoTouch.RuntimeException;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	class DocumentPoker : UIDocument {
		
		static FieldInfo bkFileUrl;
		
		static DocumentPoker ()
		{
			var t = typeof (UIDocument);
			bkFileUrl = t.GetField ("__mt_FileUrl_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		
		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}
		
		public DocumentPoker (NSUrl url) : base (url)
		{
		}

		public NSUrl FileUrlBackingField {
			get {
				return (NSUrl) bkFileUrl.GetValue (this);
			}
		}
	}
	
	class MyUrl : NSUrl {
		
		public MyUrl (string url, string annotation) : base (url)
		{
			Annotation = annotation;
		}
		
		public string Annotation { get; private set; }
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DocumentTest {

		class MyDocument : UIDocument {

			private string content = "content";

			public MyDocument (NSUrl url) : base (url)
			{
			}

			public override NSObject ContentsForType (string typeName, out NSError outError)
			{
				outError = null;
				return NSData.FromString (content);
			}

			public override bool LoadFromContents (NSObject contents, string typeName, out NSError outError)
			{
				outError = null;
				content = (contents as NSData).ToString ();
				return true;
			}
		}

		MyDocument doc;
		
		string GetFileName ()
		{
			string file = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "mydocument.txt");
			if (File.Exists (file))
				File.Delete (file);
			return file;
		}

		[Test]
		public void Save ()
		{
			using (NSUrl url = NSUrl.FromFilename (GetFileName ())) {
				doc = new MyDocument (url);
				doc.Save (url, UIDocumentSaveOperation.ForCreating, OperationHandler);
			}
		}

		void OperationHandler (bool success)
		{
			Assert.True (success);
		}
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void PerformAsynchronousFileAccess_Null ()
		{
			using (NSUrl url = NSUrl.FromFilename (GetFileName ()))
			using (var doc = new MyDocument (url)) {
				// NULL value is not documented by Apple but adding a
				// [NullAllowed] would throw an Objective-C exception (bad)
				doc.PerformAsynchronousFileAccess (null);
			}
		}

		[Test]
		public void FileUrl_BackingField ()
		{
			if (DocumentPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			string file = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "uidocument.txt");
			if (File.Exists (file))
				File.Delete (file);
			
			using (NSUrl url = NSUrl.FromFilename (file))
			using (var doc = new DocumentPoker (url)) {
				Assert.NotNull (doc.FileUrlBackingField, "1a");
				Assert.AreSame (doc.FileUrl, doc.FileUrlBackingField, "2a");
				// not a big deal in this case since we can't subclass NSUrl
			}
		}

		[Test]
		[Ignore ("crash on the bots, run fines locally on sim")]
		public void NSUrl_Subclass ()
		{
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Inconclusive ("will crash runner application after test execution");
			
			string file = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "uidocument.txt");
			if (File.Exists (file))
				File.Delete (file);
			
			// interesting limitation
			using (MyUrl url2 = new MyUrl (file, "my document")) {
				// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: must pass a valid file URL to -[UIDocument initWithFileURL:]
				Assert.Throws<MonoTouchException> (delegate { 
					new DocumentPoker (url2);
				});
			}
		}

		[Test]
		public void Fields ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			// just to confirm it's not an NSUrl but an NSString
			Assert.That (UIDocument.UserActivityDocumentUrlKey.ToString (), Is.EqualTo ("NSUserActivityDocumentURL"), "NSUserActivityDocumentURLKey");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
