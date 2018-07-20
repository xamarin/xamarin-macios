//
// Author: Kenneth J. Pouncey
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using ObjCRuntime;

namespace OpenGL {

	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
	public enum CGLErrorCode : uint
	{
		NoError            = 0,		/* no error */
		BadAttribute       = 10000,	/* invalid pixel format attribute  */
		BadProperty        = 10001,	/* invalid renderer property       */
		BadPixelFormat     = 10002,	/* invalid pixel format            */
		BadRendererInfo    = 10003,	/* invalid renderer info           */
		BadContext         = 10004,	/* invalid context                 */
		BadDrawable        = 10005,	/* invalid drawable                */
		BadDisplay         = 10006,	/* invalid graphics device         */
		BadState           = 10007,	/* invalid context state           */
		BadValue           = 10008,	/* invalid numerical value         */
		BadMatch           = 10009,	/* invalid share context           */
		BadEnumeration     = 10010,	/* invalid enumerant               */
		BadOffScreen       = 10011,	/* invalid offscreen drawable      */
		BadFullScreen      = 10012,	/* invalid offscreen drawable      */
		BadWindow          = 10013,	/* invalid window                  */
		BadAddress         = 10014,	/* invalid pointer                 */
		BadCodeModule      = 10015,	/* invalid code module             */
		BadAlloc           = 10016,	/* invalid memory allocation       */
		BadConnection      = 10017 	/* invalid CoreGraphics connection */

	}

	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
	public enum CGLPixelFormatAttribute : uint
	{
		AllRenderers = 1,
		DoubleBuffer = 5,
		Stereo = 6,
		AuxBuffers = 7,
		ColorSize = 8,
		AlphaSize = 11,
		DepthSize = 12,
		StencilSize = 13,
		AccumSize = 14,
		MinimumPolicy = 51,
		MaximumPolicy = 52,
		OffScreen = 53,
		FullScreen = 54,
		SampleBuffers = 55,
		Samples = 56,
		AuxDepthStencil = 57,
		ColorFloat = 58,
		Multisample = 59,
		Supersample = 60,
		SampleAlpha = 61,
		RendererID = 70,
		SingleRenderer = 71,
		NoRecovery = 72,
		Accelerated = 73,
		ClosestPolicy = 74,
		Robust = 75,
		BackingStore = 76,
		MPSafe = 78,
		Window = 80,
		MultiScreen = 81,
		Compliant = 83,
		ScreenMask = 84,
		PixelBuffer = 90,
		RemotePixelBuffer = 91,
		AllowOfflineRenderers = 96,
		AcceleratedCompute = 97,
		VirtualScreenCount = 128
	}

} 
