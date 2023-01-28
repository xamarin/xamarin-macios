//
// Unit tests for NSFileCoordinator
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FileCoordinatorTest {

		NSUrl GetUrl ()
		{
			return new NSUrl (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "FileCoordinatorTest.txt"));
		}

		bool fileop;

		void FileOp (NSUrl url)
		{
			fileop = true;
		}

		[Test]
		public void CoordinateRead ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				fileop = false;
				fc.CoordinateRead (url, NSFileCoordinatorReadingOptions.WithoutChanges, out err, FileOp);
				Assert.True (fileop, "fileop/sync");
				Assert.Null (err, "NSError");
			}
		}

		[Test]
		public void CoordinateRead_Null ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				// NULL is not documented by Apple but it crash the app with:
				// NSFileCoordinator: A surprising server error was signaled. Details: Connection invalid
				Assert.Throws<ArgumentNullException> (() => fc.CoordinateRead (url, NSFileCoordinatorReadingOptions.WithoutChanges, out err, null));
			}
		}

		[Test]
		public void CoordinateWrite ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				fileop = false;
				fc.CoordinateWrite (url, NSFileCoordinatorWritingOptions.ForDeleting, out err, FileOp);
				Assert.True (fileop, "fileop/sync");
				Assert.Null (err, "NSError");
			}
		}

		[Test]
		public void CoordinateWrite_Null ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				// NULL is not documented by Apple but it crash the app with:
				// NSFileCoordinator: A surprising server error was signaled. Details: Connection invalid
				Assert.Throws<ArgumentNullException> (() => fc.CoordinateWrite (url, NSFileCoordinatorWritingOptions.ForDeleting, out err, null));
			}
		}

		void FileOp (NSUrl url1, NSUrl url2)
		{
			fileop = true;
		}

		[Test]
		public void CoordinateReadWrite ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				fileop = false;
				fc.CoordinateReadWrite (url, NSFileCoordinatorReadingOptions.WithoutChanges, url, NSFileCoordinatorWritingOptions.ForDeleting, out err, FileOp);
				Assert.True (fileop, "fileop/sync");
				Assert.Null (err, "NSError");
			}
		}

		[Test]
		public void CoordinateReadWrite_Null ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				// NULL is not documented by Apple but it crash the app with:
				// NSFileCoordinator: A surprising server error was signaled. Details: Connection invalid
				Assert.Throws<ArgumentNullException> (() => fc.CoordinateReadWrite (url, NSFileCoordinatorReadingOptions.WithoutChanges, url, NSFileCoordinatorWritingOptions.ForDeleting, out err, null));
			}
		}

		[Test]
		public void CoordinateWriteWrite ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				fileop = false;
				fc.CoordinateWriteWrite (url, NSFileCoordinatorWritingOptions.ForMoving, url, NSFileCoordinatorWritingOptions.ForDeleting, out err, FileOp);
				Assert.True (fileop, "fileop/sync");
				Assert.Null (err, "NSError");
			}
		}

		[Test]
		public void CoordinateWriteWrite_Null ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				// NULL is not documented by Apple but it crash the app with:
				// NSFileCoordinator: A surprising server error was signaled. Details: Connection invalid
				Assert.Throws<ArgumentNullException> (() => fc.CoordinateWriteWrite (url, NSFileCoordinatorWritingOptions.ForMoving, url, NSFileCoordinatorWritingOptions.ForDeleting, out err, null));
			}
		}

		void Action ()
		{
			fileop = true;
		}

		[Test]
		public void CoordinateBatch_Action ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				fileop = false;
#if NET
				fc.CoordinateBatch (new NSUrl [] { url }, NSFileCoordinatorReadingOptions.WithoutChanges, new NSUrl [] { url }, NSFileCoordinatorWritingOptions.ForDeleting, out err, Action);
#else
				fc.CoordinateBatc (new NSUrl [] { url }, NSFileCoordinatorReadingOptions.WithoutChanges, new NSUrl [] { url }, NSFileCoordinatorWritingOptions.ForDeleting, out err, Action);
#endif
				Assert.True (fileop, "fileop/sync");
				Assert.Null (err, "NSError");
			}
		}

		[Test]
		public void CoordinateBatch_Action_Null ()
		{
			using (var url = GetUrl ())
			using (var fc = new NSFileCoordinator ()) {
				NSError err;
				// NULL is not documented by Apple but it crash the app with:
				// NSFileCoordinator: A surprising server error was signaled. Details: Connection invalid
#if NET
				Assert.Throws<ArgumentNullException> (() => fc.CoordinateBatch (new NSUrl [] { url }, NSFileCoordinatorReadingOptions.WithoutChanges, new NSUrl [] { url }, NSFileCoordinatorWritingOptions.ForDeleting, out err, null));
#else
				Assert.Throws<ArgumentNullException> (() => fc.CoordinateBatc (new NSUrl [] { url }, NSFileCoordinatorReadingOptions.WithoutChanges, new NSUrl [] { url }, NSFileCoordinatorWritingOptions.ForDeleting, out err, null));
#endif
			}
		}
	}
}
