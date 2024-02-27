#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using Foundation;
using CoreAnimation;
using OpenGL;
using ObjCRuntime;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CAOpenGLLayerTest {
#if !DYNAMIC_REGISTRAR
		[Ignore ("https://github.com/xamarin/xamarin-macios/issues/10284")]
#endif
		[Test]
		public void SubclassedTest ()
		{
			TestRuntime.AssertNotVirtualMachine ();

			using (var layer = new OpenGLLayer ()) {
				Messaging.IntPtr_objc_msgSend (layer.Handle, Selector.GetHandle ("copyCGLPixelFormatForDisplayMask:"));
			}
		}
	}

	public partial class OpenGLLayer : CAOpenGLLayer {
		public override CGLPixelFormat CopyCGLPixelFormatForDisplayMask (uint mask)
		{
			var attribs = new object [] {
				CGLPixelFormatAttribute.Accelerated,
				CGLPixelFormatAttribute.DoubleBuffer,
				CGLPixelFormatAttribute.ColorSize, 24,
				CGLPixelFormatAttribute.DepthSize, 16 };

			CGLPixelFormat pixelFormat = new CGLPixelFormat (attribs);
			return pixelFormat;
		}
	}
}
#endif // __MACOS__
