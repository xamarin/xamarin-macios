//
// Unit tests for CMTagCollectionVideoOutputPreset

using System;

using AVFoundation;
using CoreMedia;
using Foundation;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMTagCollectionVideoOutputPresetTest {
		[Test]
		public void Create ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			using var tagCollection = CMTagCollectionVideoOutputPreset.Monoscopic.Create (out var status);
			Assert.AreEqual (CMTagCollectionError.Success, status, "Status");
			Assert.IsNotNull (tagCollection, "TagCollection");
		}
	}
}
