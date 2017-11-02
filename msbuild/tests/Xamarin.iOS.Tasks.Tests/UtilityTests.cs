using NUnit.Framework;

using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class UtilityTests
	{
		[Test]
		public void TestAbsoluteToRelativePath ()
		{
			string rpath;

			rpath = PathUtils.AbsoluteToRelative ("/Users/user/source/Project", "/Users/user/Source/Project/Info.plist");
			Assert.AreEqual ("Info.plist", rpath, "#1");
		}
	}
}
