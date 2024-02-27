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
#if HAS_OPENGLES
using OpenGLES;
#endif
#endif

using NUnit.Framework;

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
				using (var v = ctx.CreateCGImage (img, new CGRect (0, 0, 5, 5))) {
					int rc = Messaging.int_objc_msgSend (v.Handle, retainCount.Handle);
					Assert.AreEqual (1, rc, "CreateCGImage #a1");
				}

				using (var v = ctx.CreateCGImage (img, new CGRect (0, 0, 32, 32), CIImage.FormatARGB8, null)) {
					int rc = Messaging.int_objc_msgSend (v.Handle, retainCount.Handle);
					Assert.AreEqual (1, rc, "CreateCGImage #b1");
				}

#if !MONOMAC // CreateCGImage returning null on mac
				using (var v = ctx.CreateCGImage (img, new CGRect (0, 0, 5, 5), CIFormat.ARGB8, null)) {
					int rc = Messaging.int_objc_msgSend (v.Handle, retainCount.Handle);
					Assert.AreEqual (1, rc, "CreateCGImage #c1");
				}
#endif
			}
		}

#if HAS_OPENGLES
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
				ci.Render (img, cv, CGRect.Empty, null);
			}
		}
#endif // HAS_OPENGLES
	}
}

#endif // !__WATCHOS__
