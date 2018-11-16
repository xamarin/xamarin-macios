using System;

namespace Xamarin.iOS.UnitTests.XUnit
{
	public class XUnitFilter
	{
		public string TraitName { get; }
		public string TraitValue { get; }
		public string TestCaseName { get; }
		public bool Exclude { get; }
		public XUnitFilterType FilterType { get; }

		public XUnitFilter (string testCaseName, bool exclude)
		{
			FilterType = XUnitFilterType.TypeName;
			TestCaseName = testCaseName;
			Exclude = exclude;
		}

		public XUnitFilter (string traitName, string traitValue, bool exclude)
		{
			FilterType = XUnitFilterType.Trait;
			TraitName = traitName;
			TraitValue = traitValue;
			Exclude = exclude;
		}
	}
}