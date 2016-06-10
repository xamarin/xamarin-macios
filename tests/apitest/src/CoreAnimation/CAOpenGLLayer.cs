using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;
using MonoMac.OpenGL;
using MonoMac.ObjCRuntime;
#else
using AppKit;
using Foundation;
using CoreAnimation;
using OpenGL;
using ObjCRuntime;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class CAOpenGLLayerTest
	{		
		[Test]
		public void SubclassedTest ()
		{
			using (var layer = new OpenGLLayer ()) {
				Messaging.IntPtr_objc_msgSend (layer.Handle, Selector.GetHandle ("copyCGLPixelFormatForDisplayMask:"));
			}
		}
	}

	public partial class OpenGLLayer : CAOpenGLLayer
	{
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