using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Foundation;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSRangeTest {

		// Validates the NSRange IEquatable implementation
		[TestCase (1, 1, 1, 1, true)]
		[TestCase (2, 1, 1, 1, false)]
		[TestCase (1, 2, 1, 1, false)]
		[TestCase (1, 1, 2, 1, false)]
		[TestCase (1, 1, 1, 2, false)]
		public void IEquatableImplementation (int start1, int len1, int start2, int len2, bool expected)
		{
			var left = new NSRange (start1, len1);
			var right = new NSRange (start2, len2);

			Assert.AreEqual (expected, left.Equals (right));

			if (expected) {
				Assert.AreEqual (left.GetHashCode (), right.GetHashCode ());
			}
		}

	}
}
