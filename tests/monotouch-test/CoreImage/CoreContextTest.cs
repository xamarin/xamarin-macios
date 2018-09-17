//
// Unit tests for CIContext
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;

#if XAMCORE_2_0
using Foundation;
using CoreImage;
using CoreGraphics;
using CoreVideo;
using ObjCRuntime;
#if MONOMAC
using AppKit;
using OpenGL;
#else
using UIKit;
using OpenGLES;
#endif
#else
using MonoTouch.CoreImage;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreVideo;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreImage {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CIContextTest {
		
		[Test]
		public void CreateRefCount ()
		{
			// Bug #7117
			
			CIImage img = new CIImage (CIColor.FromRgb (0.5f, 0.5f, 0.5f));
			Selector retainCount = new Selector ("retainCount");

#if MONOMAC
			using (var ctx = new CIContext (null)) {
#else
			using (var ctx = CIContext.Create ()) {
#endif
				using (var v = ctx.CreateCGImage (img, new RectangleF (0, 0, 5, 5))) {
					int rc = Messaging.int_objc_msgSend (v.Handle, retainCount.Handle);
					Assert.AreEqual (1, rc, "CreateCGImage #a1");
				}

				using (var v = ctx.CreateCGImage (img, new RectangleF (0, 0, 32, 32), CIImage.FormatARGB8, null)) {
					int rc = Messaging.int_objc_msgSend (v.Handle, retainCount.Handle);
					Assert.AreEqual (1, rc, "CreateCGImage #b1");
				}

#if !MONOMAC // CreateCGImage returning null on mac
				using (var v = ctx.CreateCGImage (img, new RectangleF (0, 0, 5, 5), CIFormat.ARGB8, null)) {
					int rc = Messaging.int_objc_msgSend (v.Handle, retainCount.Handle);
					Assert.AreEqual (1, rc, "CreateCGImage #c1");
				}
#endif
			}
		}

#if !MONOMAC // No EAGLContext for Mac
		[Test]
		public void FromContext_13983 ()
		{
			using (var ctx = new EAGLContext (EAGLRenderingAPI.OpenGLES2))
			using (var ci = CIContext.FromContext (ctx)) {
				Assert.NotNull (ci);
				if (TestRuntime.CheckXcodeVersion (7, 0))
					Assert.That (ci.WorkingColorSpace.Model, Is.EqualTo (CGColorSpaceModel.RGB), "WorkingColorSpace");
			}
		}

		[Test]
		public void Render_Colorspace ()
		{
			using (var ctx = new EAGLContext (EAGLRenderingAPI.OpenGLES2))
			using (var ci = CIContext.FromContext (ctx))
			using (var cv = new CVPixelBuffer (1, 1, CVPixelFormatType.CV24RGB))
			using (CIImage img = new CIImage (CIColor.FromRgb (0.5f, 0.5f, 0.5f))) {
				// that one "null allowed" was undocumented
				ci.Render (img, cv, RectangleF.Empty, null);
			}
		}
#endif
	}
}

#endif // !__WATCHOS__
