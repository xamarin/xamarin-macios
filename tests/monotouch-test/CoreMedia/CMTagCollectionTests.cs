//
// Unit tests for CMTag
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2024 Microsoft Corp. All rights reserved.
//
using System;

using CoreFoundation;
using CoreMedia;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

using Xamarin.Utils;

using MonoTouchFixtures.AVFoundation;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMTagCollectionTests {
		[Test]
		public void GetTypeIdTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.AreNotEqual (0, CMTagCollection.GetTypeId (), "GetTypeId");
		}

		[Test]
		public void CreateTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create ();
				Assert.AreEqual (0, (int) tagCollection.Count, "Count A");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty A");
			}

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				Assert.AreEqual (1, (int) tagCollection.Count, "Count B");
				Assert.IsFalse (tagCollection.IsEmpty, "IsEmpty B");
			}

			{
				using var tagCollection = CMTagCollection.Create (new CMTag [] { CMTag.MediaTypeVideo });
				Assert.AreEqual (1, (int) tagCollection.Count, "Count C");
				Assert.IsFalse (tagCollection.IsEmpty, "IsEmpty C");
			}

			{
				using var tagCollection = CMTagCollection.Create ((CMTag []) null);
				Assert.AreEqual (0, (int) tagCollection.Count, "Count D");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty D");
			}
		}

		[Test]
		public void CreateTest_OSStatus ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (out var status);
				Assert.AreEqual (CMTagCollectionError.Success, status, "Status A");
				Assert.AreEqual (0, (int) tagCollection.Count, "Count A");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty A");
			}

			{
				using var tagCollection = CMTagCollection.Create (out var status, CMTag.MediaTypeVideo);
				Assert.AreEqual (CMTagCollectionError.Success, status, "Status B");
				Assert.AreEqual (1, (int) tagCollection.Count, "Count B");
				Assert.IsFalse (tagCollection.IsEmpty, "IsEmpty B");
			}

			{
				using var tagCollection = CMTagCollection.Create (out var status, new CMTag [] { CMTag.MediaTypeVideo });
				Assert.AreEqual (CMTagCollectionError.Success, status, "Status C");
				Assert.AreEqual (1, (int) tagCollection.Count, "Count C");
				Assert.IsFalse (tagCollection.IsEmpty, "IsEmpty C");
			}

			{
				using var tagCollection = CMTagCollection.Create (out var status, (CMTag []) null);
				Assert.AreEqual (CMTagCollectionError.Success, status, "Status D");
				Assert.AreEqual (0, (int) tagCollection.Count, "Count D");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty D");
			}
		}

		[Test]
		public void CreateMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.CreateMutable (out var status);
				Assert.AreEqual (CMTagCollectionError.Success, status, "Status A");
				Assert.AreEqual (0, (int) tagCollection.Count, "Count A");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty A");
			}

			{
				using var tagCollection = CMTagCollection.CreateMutable (1, out var status);
				Assert.AreEqual (CMTagCollectionError.Success, status, "Status B");
				Assert.AreEqual (0, (int) tagCollection.Count, "Count B");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty B");
			}

			{
				using var tagCollection = CMTagCollection.CreateMutable (-1, out var status);
				Assert.AreEqual (CMTagCollectionError.ParamErr, status, "Status C");
				Assert.IsNull (tagCollection, "Null C");
			}

			{
				using var tagCollection = CMTagCollection.CreateMutable ();
				Assert.AreEqual (0, (int) tagCollection.Count, "Count D");
				Assert.IsTrue (tagCollection.IsEmpty, "IsEmpty D");
			}
		}

		[Test]
		public void CopyTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual (1, (int) tagCollection.Count, "Count A");
			Assert.IsFalse (tagCollection.IsEmpty, "IsEmpty A");

			using var copy = tagCollection.Copy (out var status);
			Assert.AreEqual (1, (int) copy.Count, "Count B");
			Assert.IsFalse (copy.IsEmpty, "IsEmpty B");
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status B");
		}

		[Test]
		public void CreateMutableCopyTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual (1, (int) tagCollection.Count, "Count A");
			Assert.IsFalse (tagCollection.IsEmpty, "IsEmpty A");

			using var copy = tagCollection.CreateMutableCopy (out var status);
			Assert.AreEqual (1, (int) copy.Count, "Count B");
			Assert.IsFalse (copy.IsEmpty, "IsEmpty B");
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status B");
		}

		[Test]
		public void ToStringTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual ("CMTagCollection{\n{category:'mdia' value:'vide' <OSType>}\n}", tagCollection.ToString (), "ToString");
		}

		[Test]
		public void ContainsTagTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual (true, tagCollection.ContainsTag (CMTag.MediaTypeVideo), "MediaTypeVideo");
			Assert.AreEqual (false, tagCollection.ContainsTag (CMTag.MediaTypeAudio), "MediaTypeAudio");
		}

		[Test]
		public void ContainsTagCollectionTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio);
			Assert.AreEqual (true, tagCollection1.ContainsTagCollection (tagCollection2), "1");
			Assert.AreEqual (false, tagCollection2.ContainsTagCollection (tagCollection1), "2");

			Assert.Throws<ArgumentNullException> (() => tagCollection1.ContainsTagCollection (null), "Null");
		}

		[Test]
		public void ContainsTagsTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.AreEqual (true, tagCollection.ContainsTags (CMTag.MediaTypeVideo), "MediaTypeVideo");
			Assert.AreEqual (true, tagCollection.ContainsTags (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio), "MediaTypeVideo+MediaTypeAudio");
			Assert.AreEqual (false, tagCollection.ContainsTags (CMTag.MediaTypeVideo, CMTag.PackingTypeNone), "MediaTypeVideo+PackingTypeNone");
			Assert.AreEqual (false, tagCollection.ContainsTags (CMTag.PackingTypeNone), "PackingTypeNone");
		}

		[Test]
		public void ContainsCategoryTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.AreEqual (false, tagCollection.ContainsCategory (CMTagCategory.ProjectionType), "ProjectionType");
			Assert.AreEqual (true, tagCollection.ContainsCategory (CMTagCategory.MediaType), "MediaType");
		}

		[Test]
		public void GetCountTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.AreEqual (0, (int) tagCollection.GetCount (CMTagCategory.ProjectionType), "ProjectionType");
			Assert.AreEqual (2, (int) tagCollection.GetCount (CMTagCategory.MediaType), "MediaType");
		}

		[Test]
		public void TagsTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var tags = tagCollection.Tags;
			Assert.AreEqual (2, tags.Length, "Length");
		}

		[Test]
		public void GetTagsTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var tags = tagCollection.GetTags (out var status);
			Assert.AreEqual (2, tags.Length, "Length");
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
		}

		[Test]
		public void GetTags2Test ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var tags = new CMTag [1];
			var status = tagCollection.GetTags (tags, tags.Length, out var tagsCopied);
			Assert.AreEqual (1, tags.Length, "Length");
			Assert.AreEqual (1, (int) tagsCopied, "Tags Copied");
			Assert.AreEqual (CMTagCollectionError.ExhaustedBufferSize, status, "Status");
			Assert.IsTrue (tags [0].IsValid, "Tags[0].IsValid");

			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (tags, tags.Length + 1, out tagsCopied), "AOORE");
			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (tags, -1, out tagsCopied), "AOORE 2");
		}

		[Test]
		public void GetTagsForCategoryTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = tagCollection.GetTags (CMTagCategory.MediaType, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (2, tags.Length, "Length");
		}

		[Test]
		public void GetTagsForCategory2Test ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = new CMTag [1];
			var status = tagCollection.GetTags (CMTagCategory.MediaType, tags, tags.Length, out var tagsCopied);
			Assert.AreEqual (1, tags.Length, "Length");
			Assert.AreEqual (1, (int) tagsCopied, "Tags Copied");
			Assert.AreEqual (CMTagCollectionError.ExhaustedBufferSize, status, "Status");
			Assert.IsTrue (tags [0].IsValid, "Tags[0].IsValid");

			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (CMTagCategory.MediaType, tags, tags.Length + 1, out var tagsCopied), "AOORE");
			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (CMTagCategory.MediaType, tags, -1, out var tagsCopied), "AOORE 2");
		}

		[Test]
		public void GetCount_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			Assert.AreEqual (2, (int) tagCollection.GetCount ((v) => v.Category == CMTagCategory.MediaType), "Count");
		}

		[Test]
		public void GetTags_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var count = tagCollection.GetCount ((v) => v.Category == CMTagCategory.MediaType);
			Assert.AreEqual (2, (int) count, "Count");
		}

		[Test]
		public void GetTags2_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = new CMTag [1];
			var status = tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType, tags, tags.Length, out var tagsCopied);
			Assert.AreEqual (1, tags.Length, "Length");
			Assert.AreEqual (1, (int) tagsCopied, "Tags Copied");
			Assert.AreEqual (CMTagCollectionError.ExhaustedBufferSize, status, "Status");
			Assert.IsTrue (tags [0].IsValid, "Tags[0].IsValid");

			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType, tags, tags.Length + 1, out tagsCopied), "AOORE");
			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType, tags, -1, out tagsCopied), "AOORE 2");
		}

		[Test]
		public void CreateWithCopyOfTags_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection2 = tagCollection1.CreateWithCopyOfTags (out var status, CMTagCategory.MediaType);
			Assert.AreEqual (2, (int) tagCollection2.Count, "Count");
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
		}

		[Test]
		public void ApplyTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var counter = 0;
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			tagCollection.Apply ((v) => {
				counter++;
			});
			Assert.AreEqual ((int) tagCollection.Count, counter, "Counter");
		}

		[Test]
		public void ApplyUntilTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var counter = 0;
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tag = tagCollection.ApplyUntil ((v) => {
				counter++;
				return false;
			});
			Assert.AreEqual ((int) tagCollection.Count, counter, "Counter A");
			Assert.IsFalse (tag.IsValid, "IsValid A");

			counter = 0;
			tag = tagCollection.ApplyUntil ((v) => {
				counter++;
				if (v.Category == CMTagCategory.PackingType)
					return true;
				return false;
			});
			Assert.That (counter, Is.GreaterThan (0), "Counter B1");
			Assert.That (counter, Is.LessThanOrEqualTo ((int) tagCollection.Count), "Counter B2");
			Assert.IsTrue (tag.IsValid, "IsValid B");
			Assert.IsTrue (CMTag.Equals (tag, CMTag.PackingTypeNone), "Equals B");
		}

		[Test]
		public void Intersect ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.Intersect (tagCollection1, tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (1, (int) tagCollection.Count, "Count");
			Assert.IsTrue (CMTag.Equals (CMTag.MediaTypeAudio, tagCollection.Tags [0]), "Tag #0");
		}


		[Test]
		public void Intersect_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.Intersect (tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (1, (int) tagCollection.Count, "Count");
			Assert.IsTrue (CMTag.Equals (CMTag.MediaTypeAudio, tagCollection.Tags [0]), "Tag #0");
		}

		[Test]
		public void Union ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.Union (tagCollection1, tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (3, (int) tagCollection.Count, "Count");
		}


		[Test]
		public void Union_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.Union (tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (3, (int) tagCollection.Count, "Count");
		}

		[Test]
		public void Subtract ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.Subtract (tagCollection1, tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (1, (int) tagCollection.Count, "Count");
			Assert.IsTrue (CMTag.Equals (CMTag.MediaTypeVideo, tagCollection.Tags [0]), "Tag #0");
		}

		[Test]
		public void Subtract_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.Subtract (tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (1, (int) tagCollection.Count, "Count");
			Assert.IsTrue (CMTag.Equals (CMTag.MediaTypeVideo, tagCollection.Tags [0]), "Tag #0");
		}

		[Test]
		public void ExclusiveOr ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.ExclusiveOr (tagCollection1, tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (2, (int) tagCollection.Count, "Count");
		}

		[Test]
		public void ExclusiveOr_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.ExclusiveOr (tagCollection2, out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.AreEqual (2, (int) tagCollection.Count, "Count");
		}

		[Test]
		public void AddTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual (CMTagCollectionError.ParamErr, tagCollection.Add (CMTag.MediaTypeAudio), "Add");
		}

		[Test]
		public void AddMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.CreateMutable ();
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.Add (CMTag.MediaTypeAudio), "Add 1");
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.Add (CMTag.MediaTypeAudio), "Add 2");
			Assert.AreEqual (1, (int) tagCollection.Count, "Count");
		}

		[Test]
		public void RemoveTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual (CMTagCollectionError.ParamErr, tagCollection.Remove (CMTag.MediaTypeAudio), "Remove");
		}

		[Test]
		public void RemoveMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.CreateMutable ();
			Assert.AreEqual (CMTagCollectionError.TagNotFound, tagCollection.Remove (CMTag.MediaTypeAudio), "Remove 1");
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.Add (CMTag.MediaTypeAudio), "Add 1");
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.Remove (CMTag.MediaTypeAudio), "Remove 2");
			Assert.AreEqual (0, (int) tagCollection.Count, "Count");
			Assert.AreEqual (CMTagCollectionError.TagNotFound, tagCollection.Remove (CMTag.MediaTypeAudio), "Remove 3");
		}

		[Test]
		public void RemoveAllTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.AreEqual (CMTagCollectionError.ParamErr, tagCollection.RemoveAllTags (), "Remove");
		}

		[Test]
		public void RemoveAllMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.CreateMutable ();
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.Add (CMTag.MediaTypeAudio), "Add 1");
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.Add (CMTag.MediaTypeVideo), "Add 2");
			Assert.AreEqual (CMTagCollectionError.Success, tagCollection.RemoveAllTags (), "RemoveAll");
			Assert.AreEqual (0, (int) tagCollection.Count, "Count");
		}

		[Test]
		public void AddCollection ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			Assert.Throws<ArgumentNullException> (() => tagCollection1.Add ((CMTagCollection) null), "Add null");
			Assert.AreEqual (CMTagCollectionError.ParamErr, tagCollection1.Add (tagCollection2), "Add");
		}

		[Test]
		public void AddTags ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.Throws<ArgumentNullException> (() => tagCollection.Add ((CMTag []) null), "Add null");
			Assert.AreEqual (CMTagCollectionError.ParamErr, tagCollection.Add (CMTag.MediaTypeAudio, CMTag.PackingTypeNone), "Add");
		}

		[Test]
		public void Dictionary ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var roundTrip = new Action<CMTagCollection, string> ((collection, message) => {
				var dict = collection.ToDictionary ();
				var deserializedCollection = CMTagCollection.Create (dict, out var status);
				Assert.AreEqual (CMTagCollectionError.Success, status, $"{message}: Status");
				// if the union of the original and deserialized collection has the same number of tags as the original collection, then the original and deserialized collections are identical.
				var union = collection.Union (deserializedCollection, out status);
				Assert.AreEqual (CMTagCollectionError.Success, status, $"{message}: Status 2");
				Assert.AreEqual (collection.Count, union.Count, "Count");
			});

			Assert.Multiple (() => {
				roundTrip (CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio), "Create");
				var tagCollection = CMTagCollection.CreateMutable ();
				tagCollection.Add (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
				roundTrip (tagCollection, "CreateMutable");
			});
		}

		[Test]
		public void Data ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var roundTrip = new Action<CMTagCollection, string> ((collection, message) => {
				var data = collection.ToData ();
				var deserializedCollection = CMTagCollection.Create (data, out var status);
				Assert.AreEqual (CMTagCollectionError.Success, status, $"{message}: Status");
				// if the union of the original and deserialized collection has the same number of tags as the original collection, then the original and deserialized collections are identical.
				var union = collection.Union (deserializedCollection, out status);
				Assert.AreEqual (CMTagCollectionError.Success, status, $"{message}: Status 2");
				Assert.AreEqual (collection.Count, union.Count, "Count");
			});

			Assert.Multiple (() => {
				roundTrip (CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio), "Create");
				var tagCollection = CMTagCollection.CreateMutable ();
				tagCollection.Add (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
				roundTrip (tagCollection, "CreateMutable");
			});
		}
	}
}
