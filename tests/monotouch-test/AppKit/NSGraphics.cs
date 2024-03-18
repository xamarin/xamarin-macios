#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSGraphicsTest {
#if !XAMCORE_5_0
		[Test]
		public void BestDepth ()
		{
			bool exactMatch = false;
			var rv = NSGraphics.BestDepth (NSColorSpace.DeviceRGB, 8, 8, false, ref exactMatch);
			Assert.AreEqual (NSWindowDepth.TwentyfourBitRgb, rv, "BestDepth");
			Assert.IsTrue (exactMatch, "ExactMatch");
		}
#endif

		[Test]
		public void GetBestDepth ()
		{
			var rv = NSGraphics.GetBestDepth (NSColorSpace.DeviceRGB, 8, 8, false, out var exactMatch);
			Assert.AreEqual (NSWindowDepth.TwentyfourBitRgb, rv, "GetBestDepth");
			Assert.IsTrue (exactMatch, "ExactMatch");
		}
	}
}

#endif // __MACOS__
