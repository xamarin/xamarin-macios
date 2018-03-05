using System;
using System.IO;

using NUnit.Framework;

namespace Xamarin.Linker
{
	[TestFixture]
	public partial class Preservation
	{
		[Test]
		public void PreserveParameterInfoInXml ()
		{
			using (var mtouch = new MTouchTool ()) {
				var xml = Path.Combine (mtouch.CreateTemporaryDirectory (), "extra.xml");
				File.WriteAllText (xml, @"
<linker>
  <assembly fullname=""mscorlib"">
    <type fullname=""System.Reflection.ParameterInfo"" />
  </assembly>
</linker>");
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.XmlDefinitions = new string [] { xml };
				mtouch.CreateTemporaryApp ();
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
			}
		}
	}
}
