using System;
using System.Linq;
using System.Runtime.CompilerServices;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class TaggedPointerTest {

		[Test]
		public void TaggedPointersArentDisposed ()
		{
#if NET10_0_OR_GREATER
			var taggedPointersDisposedByDefault = false;
#else
			var taggedPointersDisposedByDefault = true;
#endif
			Assert.AreEqual (taggedPointersDisposedByDefault, ThrowsObjectDisposedExceptions (), "Default behavior");
		}

		[Test]
		public void MemoryUsage ()
		{
			var taggedStringValue = "a";
			var objA = new NSString (taggedStringValue);
			var objB = new NSString (taggedStringValue);
			Assert.AreEqual (objA.Handle, objB.Handle, "Pointer equality for tagged pointers");

			var cwt = new ConditionalWeakTable<string, NSObject> ();
			var count = 1000;
			for (var i = 0; i < count; i++) {
				cwt.Add (i.ToString (), new NSString (taggedStringValue));
			}
			GC.Collect ();
			Assert.That (cwt.Count (), Is.LessThan (count), "At least some objects should have gotten garbage collected.");
		}
	}
}
