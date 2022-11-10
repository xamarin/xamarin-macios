#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;
using SpriteKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SKSceneTests {
		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureMavericks ();
		}

		[Test]
		public void SKScene_InitWithSize ()
		{
			if (IntPtr.Size != 8) // SpriteKit is 64-bit only on mac
				return;

			SKNode c = new SKNode ();
			//SKScene c = new SKScene (new CGSize (50, 50));
			Assert.IsNotNull (c);
		}

		[Test]
		public void SKScene_InitWithSizeSuper ()
		{
			if (IntPtr.Size != 8) // SpriteKit is 64-bit only on mac
				return;

			MyScene c = new MyScene (new CGSize (50, 50));
			Assert.IsNotNull (c);
		}

		class MyScene : SKScene {
			public MyScene (CGSize size) : base (size)
			{
			}
		}
	}
}
#endif // __MACOS__
