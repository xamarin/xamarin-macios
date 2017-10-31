//
// EAGLContext Unit Tests
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using OpenGLES;
#else
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
#endif
using OpenTK.Graphics.ES20;
using NUnit.Framework;

namespace MonoTouchFixtures.OpenGLES
{

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class EAGLContextTest
	{
		[Test]
		public void PresentRenderBufferTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var obj = new EAGLContext (EAGLRenderingAPI.OpenGLES2);

			Asserts.AreEqual (true, obj.PresentRenderBuffer ((int)RenderbufferTarget.Renderbuffer, 0), "PresentRenderBuffer");
			if (TestRuntime.CheckXcodeVersion (8, 3)) {
				Asserts.AreEqual (true, obj.PresentRenderBuffer ((int)RenderbufferTarget.Renderbuffer, 0, EAGLContext.PresentationMode.AtTime), "PresentRenderBufferAtTime");
				Asserts.AreEqual (true, obj.PresentRenderBuffer ((int)RenderbufferTarget.Renderbuffer, 0, EAGLContext.PresentationMode.AfterMinimumDuration), "PresentRenderBufferAfterMinimumDuration");
			}
		}
	}
}

#endif // !__WATCHOS__
