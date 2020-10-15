using System.IO;
using NUnit.Framework;
using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	class CustomSymbolStripTask : SymbolStripTaskBase
	{
		public new string GenerateFullPathToTool ()
		{
			return base.GenerateFullPathToTool ();
		}
	}

	[TestFixture]
	public class SymbolStripTaskTests : TestBase
	{
		[Test (Description = "Xambug #34687")]
		public void GenerateFullPathToTool ()
		{
			var task = CreateTask<CustomSymbolStripTask> ();
			Assert.IsTrue (File.Exists (task.GenerateFullPathToTool ()), "#1");
		}
	}
}

