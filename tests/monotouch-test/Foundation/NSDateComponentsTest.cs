using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSDateComponentsTest {

		[Test]
		public void TestUndefinedComponent ()
		{
			// as per documentation:
			// "When a new instance of NSDateComponents is created, 
			// the date components are set to 
			// NSDateComponentUndefined." 
			// we simply test that the values are undefined
			var components = new NSDateComponents ();
			Assert.AreEqual (NSDateComponents.Undefined, components.Year, $"Year");
			Assert.AreEqual (NSDateComponents.Undefined, components.Month, "Month");
			Assert.AreEqual (NSDateComponents.Undefined, components.Day, "Day");
			Assert.AreEqual (NSDateComponents.Undefined, components.Hour, "Hour");
			Assert.AreEqual (NSDateComponents.Undefined, components.Minute, "Minute");
			Assert.AreEqual (NSDateComponents.Undefined, components.Second, "Second");
		}
	}
}
