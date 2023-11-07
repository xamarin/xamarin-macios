using System;

namespace Xamarin.iOS.UnitTests {
	public class TestRunSelector {
		public string Assembly { get; set; }
		public string Value { get; set; }
		public TestRunSelectorType Type { get; set; }
		public bool Include { get; set; }
	}
}
