#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using Foundation;

namespace apitest {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSStepperCellTests {
		NSStepperCell cell;

		[SetUp]
		public void SetUp ()
		{
			cell = new NSStepperCell ();
		}

		[Test]
		public void NSStepperCell_ShouldSetMinValue ()
		{
			var minValue = cell.MinValue;
			cell.MinValue = 3.14159;

			Assert.IsTrue (cell.MinValue != minValue, "NSStepperCell_ShouldSetMinValue - Failed to set the MinValue property");
		}

		[Test]
		public void NSStepperCell_ShouldSetMaxValue ()
		{
			var maxValue = cell.MaxValue;
			cell.MaxValue = 3.14159;

			Assert.IsTrue (cell.MinValue != maxValue, "NSStepperCell_ShouldSetMaxValue - Failed to set the MaxValue property");
		}
		[Test]
		public void NSStepperCell_ShouldSetIncrement ()
		{
			var increment = cell.Increment;
			cell.Increment = 3.14159;

			Assert.IsTrue (cell.Increment != increment, "NSStepperCell_ShouldSetIncrement - Failed to set the Increment property");
		}
		[Test]
		public void NSStepperCell_ShouldSetValueWraps ()
		{
			var valueWraps = cell.ValueWraps;
			cell.ValueWraps = !valueWraps;

			Assert.IsTrue (cell.ValueWraps != valueWraps, "NSStepperCell_ShouldSetValueWraps - Failed to set the ValueWraps property");
		}
		[Test]
		public void NSStepperCell_ShouldSetAutoRepeat ()
		{
			var autoRepeat = cell.Autorepeat;
			cell.Autorepeat = !autoRepeat;

			Assert.IsTrue (cell.Autorepeat != autoRepeat, "NSStepperCell_ShouldSetAutoRepeat - Failed to set the Autorepeat property");
		}
	}
}

#endif // __MACOS__
