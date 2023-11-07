#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSOpenGLPixelFormatTests {
		[Test]
		public void NSOpenGLPixelFormatAttributesShouldPassWith0Terminate ()
		{
			var _attribs = new object [] { NSOpenGLPixelFormatAttribute.DoubleBuffer,
				NSOpenGLPixelFormatAttribute.DepthSize,
				24,
				NSOpenGLPixelFormatAttribute.OpenGLProfile,
				NSOpenGLProfile.Version3_2Core,
				0
			};
			NSOpenGLPixelFormat pixelFormat = new NSOpenGLPixelFormat (_attribs);

			Assert.NotNull (pixelFormat);
		}

		[Test]
		public void NSOpenGLPixelFormatAttributesShouldWorkWithEmptyAttributes ()
		{
			var _attribs = new object [] {
			};
			NSOpenGLPixelFormat pixelFormat = new NSOpenGLPixelFormat (_attribs);

			Assert.NotNull (pixelFormat);
		}

		[Test]
		public void NSOpenGLPixelFormatAttributesShouldPassWithout0Terminate ()
		{
			var _attribs = new object [] { NSOpenGLPixelFormatAttribute.DoubleBuffer,
				NSOpenGLPixelFormatAttribute.DepthSize,
				24,
				NSOpenGLPixelFormatAttribute.OpenGLProfile,
				NSOpenGLProfile.Version3_2Core,
			};
			Assert.DoesNotThrow (() => { NSOpenGLPixelFormat pixelFormat = new NSOpenGLPixelFormat (_attribs); });
		}

		[Test]
		public void NSOpenGLPixelFormatAttributesOpenGLProfileShouldThrowForMissingValue ()
		{
			var missingValue = new object [] { NSOpenGLPixelFormatAttribute.DoubleBuffer,
				NSOpenGLPixelFormatAttribute.DepthSize,
				24,
				NSOpenGLPixelFormatAttribute.OpenGLProfile
			};
			Assert.Throws<ArgumentException> (() => { NSOpenGLPixelFormat pixelFormat = new NSOpenGLPixelFormat (missingValue); });
		}

		[Test]
		public void NSOpenGLPixelFormatAttributesShouldThrowForInvalidArgument ()
		{
			var _attribs = new object [] { NSOpenGLPixelFormatAttribute.DoubleBuffer,
				NSOpenGLPixelFormatAttribute.DepthSize,
				24,
				45,
				NSOpenGLPixelFormatAttribute.OpenGLProfile,
				NSOpenGLProfile.Version3_2Core,
				0
			};

			Assert.Throws<ArgumentException> (() => { NSOpenGLPixelFormat pixelFormat = new NSOpenGLPixelFormat (_attribs); });
		}
	}
}
#endif // __MACOS__
