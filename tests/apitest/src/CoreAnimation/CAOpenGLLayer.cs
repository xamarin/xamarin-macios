using System;
using NUnit.Framework;

using AppKit;
using Foundation;
using CoreAnimation;
using OpenGL;
using ObjCRuntime;

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