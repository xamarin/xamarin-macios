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

#nullable enable

namespace OpenGL {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("macos")]
	[ObsoletedOSPlatform ("macos10.14", "Use 'Metal' Framework instead.")]
	[NativeName ("_CGLError")]
	public enum CGLErrorCode : uint {
		/// <summary>To be added.</summary>
		NoError = 0,        /* no error */
		/// <summary>To be added.</summary>
		BadAttribute = 10000,   /* invalid pixel format attribute  */
		/// <summary>To be added.</summary>
		BadProperty = 10001,    /* invalid renderer property       */
		/// <summary>To be added.</summary>
		BadPixelFormat = 10002, /* invalid pixel format            */
		/// <summary>To be added.</summary>
		BadRendererInfo = 10003,    /* invalid renderer info           */
		/// <summary>To be added.</summary>
		BadContext = 10004, /* invalid context                 */
		/// <summary>To be added.</summary>
		BadDrawable = 10005,    /* invalid drawable                */
		/// <summary>To be added.</summary>
		BadDisplay = 10006, /* invalid graphics device         */
		/// <summary>To be added.</summary>
		BadState = 10007,   /* invalid context state           */
		/// <summary>To be added.</summary>
		BadValue = 10008,   /* invalid numerical value         */
		/// <summary>To be added.</summary>
		BadMatch = 10009,   /* invalid share context           */
		/// <summary>To be added.</summary>
		BadEnumeration = 10010, /* invalid enumerant               */
		/// <summary>To be added.</summary>
		BadOffScreen = 10011,   /* invalid offscreen drawable      */
		/// <summary>To be added.</summary>
		BadFullScreen = 10012,  /* invalid offscreen drawable      */
		/// <summary>To be added.</summary>
		BadWindow = 10013,  /* invalid window                  */
		/// <summary>To be added.</summary>
		BadAddress = 10014, /* invalid pointer                 */
		/// <summary>To be added.</summary>
		BadCodeModule = 10015,  /* invalid code module             */
		/// <summary>To be added.</summary>
		BadAlloc = 10016,   /* invalid memory allocation       */
		/// <summary>To be added.</summary>
		BadConnection = 10017,  /* invalid CoreGraphics connection */

	}

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("macos")]
	[ObsoletedOSPlatform ("macos10.14", "Use 'Metal' Framework instead.")]
	[NativeName ("_CGLPixelFormatAttribute")]
	public enum CGLPixelFormatAttribute : uint {
		/// <summary>To be added.</summary>
		AllRenderers = 1,
		/// <summary>To be added.</summary>
		DoubleBuffer = 5,
		/// <summary>To be added.</summary>
		Stereo = 6,
		/// <summary>To be added.</summary>
		AuxBuffers = 7,
		/// <summary>To be added.</summary>
		ColorSize = 8,
		/// <summary>To be added.</summary>
		AlphaSize = 11,
		/// <summary>To be added.</summary>
		DepthSize = 12,
		/// <summary>To be added.</summary>
		StencilSize = 13,
		/// <summary>To be added.</summary>
		AccumSize = 14,
		/// <summary>To be added.</summary>
		MinimumPolicy = 51,
		/// <summary>To be added.</summary>
		MaximumPolicy = 52,
		/// <summary>To be added.</summary>
		OffScreen = 53,
		/// <summary>To be added.</summary>
		FullScreen = 54,
		/// <summary>To be added.</summary>
		SampleBuffers = 55,
		/// <summary>To be added.</summary>
		Samples = 56,
		/// <summary>To be added.</summary>
		AuxDepthStencil = 57,
		/// <summary>To be added.</summary>
		ColorFloat = 58,
		/// <summary>To be added.</summary>
		Multisample = 59,
		/// <summary>To be added.</summary>
		Supersample = 60,
		/// <summary>To be added.</summary>
		SampleAlpha = 61,
		/// <summary>To be added.</summary>
		RendererID = 70,
		/// <summary>To be added.</summary>
		SingleRenderer = 71,
		/// <summary>To be added.</summary>
		NoRecovery = 72,
		/// <summary>To be added.</summary>
		Accelerated = 73,
		/// <summary>To be added.</summary>
		ClosestPolicy = 74,
		/// <summary>To be added.</summary>
		Robust = 75,
		/// <summary>To be added.</summary>
		BackingStore = 76,
		/// <summary>To be added.</summary>
		MPSafe = 78,
		/// <summary>To be added.</summary>
		Window = 80,
		/// <summary>To be added.</summary>
		MultiScreen = 81,
		/// <summary>To be added.</summary>
		Compliant = 83,
		/// <summary>To be added.</summary>
		ScreenMask = 84,
		/// <summary>To be added.</summary>
		PixelBuffer = 90,
		/// <summary>To be added.</summary>
		RemotePixelBuffer = 91,
		/// <summary>To be added.</summary>
		AllowOfflineRenderers = 96,
		/// <summary>To be added.</summary>
		AcceleratedCompute = 97,
		/// <summary>To be added.</summary>
		VirtualScreenCount = 128,
	}

}
