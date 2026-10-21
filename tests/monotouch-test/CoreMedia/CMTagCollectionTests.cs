//
// Unit tests for CMTag
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2024 Microsoft Corp. All rights reserved.
//

using System.Linq;

using CoreMedia;

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

			Assert.That (CMTagCollection.GetTypeId (), Is.Not.EqualTo (0), "GetTypeId");
		}

		[Test]
		public void CreateTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create ();
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count A");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty A");
			}

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count B");
				Assert.That (tagCollection.IsEmpty, Is.False, "IsEmpty B");
			}

			{
				using var tagCollection = CMTagCollection.Create (new CMTag [] { CMTag.MediaTypeVideo });
				Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count C");
				Assert.That (tagCollection.IsEmpty, Is.False, "IsEmpty C");
			}

			{
				using var tagCollection = CMTagCollection.Create ((CMTag []) null);
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count D");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty D");
			}
		}

		[Test]
		public void CreateTest_OSStatus ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (out var status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status A");
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count A");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty A");
			}

			{
				using var tagCollection = CMTagCollection.Create (out var status, CMTag.MediaTypeVideo);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status B");
				Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count B");
				Assert.That (tagCollection.IsEmpty, Is.False, "IsEmpty B");
			}

			{
				using var tagCollection = CMTagCollection.Create (out var status, new CMTag [] { CMTag.MediaTypeVideo });
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status C");
				Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count C");
				Assert.That (tagCollection.IsEmpty, Is.False, "IsEmpty C");
			}

			{
				using var tagCollection = CMTagCollection.Create (out var status, (CMTag []) null);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status D");
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count D");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty D");
			}
		}

		[Test]
		public void CreateMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.CreateMutable (out var status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status A");
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count A");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty A");
			}

			{
				using var tagCollection = CMTagCollection.CreateMutable (1, out var status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status B");
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count B");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty B");
			}

			{
				using var tagCollection = CMTagCollection.CreateMutable (-1, out var status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.ParamErr), "Status C");
				Assert.That (tagCollection, Is.Null, "Null C");
			}

			{
				using var tagCollection = CMTagCollection.CreateMutable ();
				Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count D");
				Assert.That (tagCollection.IsEmpty, Is.True, "IsEmpty D");
			}
		}

		[Test]
		public void CopyTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count A");
			Assert.That (tagCollection.IsEmpty, Is.False, "IsEmpty A");

			using var copy = tagCollection.Copy (out var status);
			Assert.That ((int) copy.Count, Is.EqualTo (1), "Count B");
			Assert.That (copy.IsEmpty, Is.False, "IsEmpty B");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status B");
		}

		[Test]
		public void CreateMutableCopyTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count A");
			Assert.That (tagCollection.IsEmpty, Is.False, "IsEmpty A");

			using var copy = tagCollection.CreateMutableCopy (out var status);
			Assert.That ((int) copy.Count, Is.EqualTo (1), "Count B");
			Assert.That (copy.IsEmpty, Is.False, "IsEmpty B");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status B");
		}

		[Test]
		public void ToStringTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That (tagCollection.ToString (), Is.EqualTo ("CMTagCollection{\n{category:'mdia' value:'vide' <OSType>}\n}"), "ToString");
		}

		[Test]
		public void ContainsTagTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That (tagCollection.ContainsTag (CMTag.MediaTypeVideo), Is.EqualTo (true), "MediaTypeVideo");
			Assert.That (tagCollection.ContainsTag (CMTag.MediaTypeAudio), Is.EqualTo (false), "MediaTypeAudio");
		}

		[Test]
		public void ContainsTagCollectionTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio);
			Assert.That (tagCollection1.ContainsTagCollection (tagCollection2), Is.EqualTo (true), "1");
			Assert.That (tagCollection2.ContainsTagCollection (tagCollection1), Is.EqualTo (false), "2");

			Assert.Throws<ArgumentNullException> (() => tagCollection1.ContainsTagCollection (null), "Null");
		}

		[Test]
		public void ContainsTagsTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.That (tagCollection.ContainsTags (CMTag.MediaTypeVideo), Is.EqualTo (true), "MediaTypeVideo");
			Assert.That (tagCollection.ContainsTags (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio), Is.EqualTo (true), "MediaTypeVideo+MediaTypeAudio");
			Assert.That (tagCollection.ContainsTags (CMTag.MediaTypeVideo, CMTag.PackingTypeNone), Is.EqualTo (false), "MediaTypeVideo+PackingTypeNone");
			Assert.That (tagCollection.ContainsTags (CMTag.PackingTypeNone), Is.EqualTo (false), "PackingTypeNone");
		}

		[Test]
		public void ContainsCategoryTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.That (tagCollection.ContainsCategory (CMTagCategory.ProjectionType), Is.EqualTo (false), "ProjectionType");
			Assert.That (tagCollection.ContainsCategory (CMTagCategory.MediaType), Is.EqualTo (true), "MediaType");
		}

		[Test]
		public void GetCountTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.That ((int) tagCollection.GetCount (CMTagCategory.ProjectionType), Is.EqualTo (0), "ProjectionType");
			Assert.That ((int) tagCollection.GetCount (CMTagCategory.MediaType), Is.EqualTo (2), "MediaType");
		}

		[Test]
		public void TagsTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var tags = tagCollection.Tags;
			Assert.That (tags.Length, Is.EqualTo (2), "Length");
		}

		[Test]
		public void GetTagsTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var tags = tagCollection.GetTags (out var status);
			Assert.That (tags.Length, Is.EqualTo (2), "Length");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
		}

		[Test]
		public void GetTags2Test ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var tags = new CMTag [1];
			var status = tagCollection.GetTags (tags, tags.Length, out var tagsCopied);
			Assert.That (tags.Length, Is.EqualTo (1), "Length");
			Assert.That ((int) tagsCopied, Is.EqualTo (1), "Tags Copied");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.ExhaustedBufferSize), "Status");
			Assert.That (tags [0].IsValid, Is.True, "Tags[0].IsValid");

			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (tags, tags.Length + 1, out tagsCopied), "AOORE");
			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (tags, -1, out tagsCopied), "AOORE 2");
		}

		[Test]
		public void GetTagsForCategoryTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = tagCollection.GetTags (CMTagCategory.MediaType, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That (tags.Length, Is.EqualTo (2), "Length");
		}

		[Test]
		public void GetTagsForCategory2Test ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = new CMTag [1];
			var status = tagCollection.GetTags (CMTagCategory.MediaType, tags, tags.Length, out var tagsCopied);
			Assert.That (tags.Length, Is.EqualTo (1), "Length");
			Assert.That ((int) tagsCopied, Is.EqualTo (1), "Tags Copied");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.ExhaustedBufferSize), "Status");
			Assert.That (tags [0].IsValid, Is.True, "Tags[0].IsValid");

			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (CMTagCategory.MediaType, tags, tags.Length + 1, out var tagsCopied), "AOORE");
			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags (CMTagCategory.MediaType, tags, -1, out var tagsCopied), "AOORE 2");
		}

		[Test]
		public void GetCount_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			Assert.That ((int) tagCollection.GetCount ((v) => v.Category == CMTagCategory.MediaType), Is.EqualTo (2), "Count");
		}

		[Test]
		public void GetTags_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType);
			Assert.That (tags.Length, Is.EqualTo (2), "Length");
			Assert.That (tags.Count (tag => CMTag.Equals (tag, CMTag.MediaTypeVideo)), Is.EqualTo (1), "Video tag count");
			Assert.That (tags.Count (tag => CMTag.Equals (tag, CMTag.MediaTypeAudio)), Is.EqualTo (1), "Audio tag count");
		}

		[Test]
		public void GetTags2_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			var tags = new CMTag [1];
			var status = tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType, tags, tags.Length, out var tagsCopied);
			Assert.That (tags.Length, Is.EqualTo (1), "Length");
			Assert.That ((int) tagsCopied, Is.EqualTo (1), "Tags Copied");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.ExhaustedBufferSize), "Status");
			Assert.That (tags [0].IsValid, Is.True, "Tags[0].IsValid");

			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType, tags, tags.Length + 1, out tagsCopied), "AOORE");
			Assert.Throws<ArgumentOutOfRangeException> (() => tagCollection.GetTags ((v) => v.Category == CMTagCategory.MediaType, tags, -1, out tagsCopied), "AOORE 2");
		}

		[Test]
		public void CreateWithCopyOfTags_Filter ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection2 = tagCollection1.CreateWithCopyOfTags (out var status, CMTagCategory.MediaType);
			Assert.That ((int) tagCollection2.Count, Is.EqualTo (2), "Count");
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
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
			Assert.That (counter, Is.EqualTo ((int) tagCollection.Count), "Counter");
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
			Assert.That (counter, Is.EqualTo ((int) tagCollection.Count), "Counter A");
			Assert.That (tag.IsValid, Is.False, "IsValid A");

			counter = 0;
			tag = tagCollection.ApplyUntil ((v) => {
				counter++;
				if (v.Category == CMTagCategory.PackingType)
					return true;
				return false;
			});
			Assert.That (counter, Is.GreaterThan (0), "Counter B1");
			Assert.That (counter, Is.LessThanOrEqualTo ((int) tagCollection.Count), "Counter B2");
			Assert.That (tag.IsValid, Is.True, "IsValid B");
			Assert.That (CMTag.Equals (tag, CMTag.PackingTypeNone), Is.True, "Equals B");
		}

		[TestCase ("Count")]
		[TestCase ("GetTags")]
		[TestCase ("Apply")]
		[TestCase ("ApplyUntil")]
		public void EnumerationKeepsCollectionAlive (string operation)
		{
			TestRuntime.AssertXcodeVersion (15, 0);
			var collection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			var reference = new WeakReference (collection);
			var handle = collection.Handle;
			var alive = true;
			var count = 0;
			CMTagCollectionTagFilterFunction callback = tag => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				alive &= reference.IsAlive;
				count++;
				return true;
			};

			// An independent native retain makes a lifetime regression fail without crashing.
			TestRuntime.CFRetain (handle);
			try {
				switch (operation) {
				case "Count":
					Assert.That ((int) collection.GetCount (callback), Is.EqualTo (2), "Count");
					break;
				case "GetTags":
					var tags = new CMTag [2];
					Assert.That (collection.GetTags (callback, tags, tags.Length, out var copied), Is.EqualTo (CMTagCollectionError.Success), "Status");
					Assert.That ((int) copied, Is.EqualTo (2), "Copied");
					break;
				case "Apply":
					collection.Apply (tag => { callback (tag); });
					break;
				case "ApplyUntil":
					Assert.That (collection.ApplyUntil (callback).IsValid, Is.True, "Tag");
					break;
				}
				Assert.That (alive, Is.True, "Collection must remain alive during enumeration");
				Assert.That (count, Is.EqualTo (operation == "ApplyUntil" ? 1 : 2), "Callbacks");
			} finally {
				TestRuntime.CFRelease (handle);
			}
		}

		[Test]
		public void Intersect ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.Intersect (tagCollection1, tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count");
			Assert.That (CMTag.Equals (CMTag.MediaTypeAudio, tagCollection.Tags [0]), Is.True, "Tag #0");
		}


		[Test]
		public void Intersect_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.Intersect (tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count");
			Assert.That (CMTag.Equals (CMTag.MediaTypeAudio, tagCollection.Tags [0]), Is.True, "Tag #0");
		}

		[Test]
		public void Union ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.Union (tagCollection1, tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (3), "Count");
		}


		[Test]
		public void Union_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.Union (tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (3), "Count");
		}

		[Test]
		public void Subtract ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.Subtract (tagCollection1, tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count");
			Assert.That (CMTag.Equals (CMTag.MediaTypeVideo, tagCollection.Tags [0]), Is.True, "Tag #0");
		}

		[Test]
		public void Subtract_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.Subtract (tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count");
			Assert.That (CMTag.Equals (CMTag.MediaTypeVideo, tagCollection.Tags [0]), Is.True, "Tag #0");
		}

		[Test]
		public void ExclusiveOr ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = CMTagCollection.ExclusiveOr (tagCollection1, tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (2), "Count");
		}

		[Test]
		public void ExclusiveOr_Instance ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			using var tagCollection = tagCollection1.ExclusiveOr (tagCollection2, out var status);
			Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), "Status");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (2), "Count");
		}

		[Test]
		public void AddTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That (tagCollection.Add (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.ParamErr), "Add");
		}

		[Test]
		public void AddMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.CreateMutable ();
			Assert.That (tagCollection.Add (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.Success), "Add 1");
			Assert.That (tagCollection.Add (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.Success), "Add 2");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (1), "Count");
		}

		[Test]
		public void RemoveTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That (tagCollection.Remove (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.ParamErr), "Remove");
		}

		[Test]
		public void RemoveMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.CreateMutable ();
			Assert.That (tagCollection.Remove (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.TagNotFound), "Remove 1");
			Assert.That (tagCollection.Add (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.Success), "Add 1");
			Assert.That (tagCollection.Remove (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.Success), "Remove 2");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count");
			Assert.That (tagCollection.Remove (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.TagNotFound), "Remove 3");
		}

		[Test]
		public void RemoveAllTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
			Assert.That (tagCollection.RemoveAllTags (), Is.EqualTo (CMTagCollectionError.ParamErr), "Remove");
		}

		[Test]
		public void RemoveAllMutableTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.CreateMutable ();
			Assert.That (tagCollection.Add (CMTag.MediaTypeAudio), Is.EqualTo (CMTagCollectionError.Success), "Add 1");
			Assert.That (tagCollection.Add (CMTag.MediaTypeVideo), Is.EqualTo (CMTagCollectionError.Success), "Add 2");
			Assert.That (tagCollection.RemoveAllTags (), Is.EqualTo (CMTagCollectionError.Success), "RemoveAll");
			Assert.That ((int) tagCollection.Count, Is.EqualTo (0), "Count");
		}

		[Test]
		public void AddCollection ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			// Trying to modify a non-mutable collection
			using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio, CMTag.PackingTypeNone);
			Assert.Throws<ArgumentNullException> (() => tagCollection1.Add ((CMTagCollection) null), "Add null");
			Assert.That (tagCollection1.Add (tagCollection2), Is.EqualTo (CMTagCollectionError.ParamErr), "Add");
		}

		[Test]
		public void AddTags ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
			Assert.Throws<ArgumentNullException> (() => tagCollection.Add ((CMTag []) null), "Add null");
			Assert.That (tagCollection.Add (CMTag.MediaTypeAudio, CMTag.PackingTypeNone), Is.EqualTo (CMTagCollectionError.ParamErr), "Add");
		}

		[Test]
		public void Dictionary ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var roundTrip = new Action<CMTagCollection, string> ((collection, message) => {
				var dict = collection.ToDictionary ();
				var deserializedCollection = CMTagCollection.Create (dict, out var status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), $"{message}: Status");
				// if the union of the original and deserialized collection has the same number of tags as the original collection, then the original and deserialized collections are identical.
				var union = collection.Union (deserializedCollection, out status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), $"{message}: Status 2");
				Assert.That (union.Count, Is.EqualTo (collection.Count), "Count");
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
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), $"{message}: Status");
				// if the union of the original and deserialized collection has the same number of tags as the original collection, then the original and deserialized collections are identical.
				var union = collection.Union (deserializedCollection, out status);
				Assert.That (status, Is.EqualTo (CMTagCollectionError.Success), $"{message}: Status 2");
				Assert.That (union.Count, Is.EqualTo (collection.Count), "Count");
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
