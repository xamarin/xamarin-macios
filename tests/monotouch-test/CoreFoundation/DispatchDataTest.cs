using System;
using System.Text;

using Foundation;
using CoreFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DispatchDataTest {
		string testString;
		byte [] testData;

		[SetUp]
		public void SetUp ()
		{
			testString = "String used for testing";
			testData = Encoding.UTF8.GetBytes (testString);
		}

		[Test]
		public void FromByteBufferTest ()
		{
			using (var dd = DispatchData.FromByteBuffer (testData)) {
				var ddString = Encoding.UTF8.GetString (dd.ToArray ());
				Assert.AreEqual (testString, ddString);
			}
		}

		[Test]
		public void FromReadOnlySpanTest ()
		{
			var readOnlySpan = new ReadOnlySpan<byte> (testData);
			using (var dd = DispatchData.FromReadOnlySpan (readOnlySpan)) {
				var data = dd.ToArray ();
				var ddString = Encoding.UTF8.GetString (dd.ToArray ());
				Assert.AreEqual (testString, ddString);
			}
		}

	}

}
