// 
// CVDisplayLink.cs: Implements the managed CVDisplayLink
//
// Authors: Kenneth J. Pouncey
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

#if MONOMAC

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using OpenGL;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreVideo {
#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class CVDisplayLink : NativeObject {
		GCHandle callbackHandle;
		
#if !NET
		public CVDisplayLink (NativeHandle handle)
			: base (handle, false, true)
		{
		}
#endif

		[Preserve (Conditional=true)]
		internal CVDisplayLink (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern CVReturn CVDisplayLinkCreateWithCGDisplay (uint displayId, IntPtr* displayLink);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static CVDisplayLink? CreateFromDisplayId (uint displayId, out CVReturn error)
		{
			IntPtr handle;
			unsafe {
				error = CVDisplayLinkCreateWithCGDisplay (displayId, &handle);
			}
			if (error != 0)
				return null;

			return new CVDisplayLink (handle, true);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static CVDisplayLink? CreateFromDisplayId (uint displayId)
			=> CreateFromDisplayId (displayId, out var _);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern CVReturn CVDisplayLinkCreateWithCGDisplays (uint* displayArray, nint count, IntPtr* displayLink);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static CVDisplayLink? CreateFromDisplayIds (uint[] displayIds, out CVReturn error)
		{
			if (displayIds is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (displayIds));
			error = 0;
			IntPtr handle = IntPtr.Zero;
			unsafe {
				fixed (uint* displayArrayPtrs = displayIds) {
					error = CVDisplayLinkCreateWithCGDisplays (displayArrayPtrs, displayIds.Length, &handle);
				}
			}

			if (error != 0)
				return null;

			return new CVDisplayLink (handle, true);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static CVDisplayLink? CreateFromDisplayIds (uint[] displayIds)
			=> CreateFromDisplayIds (displayIds, out var _);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern CVReturn CVDisplayLinkCreateWithOpenGLDisplayMask (uint mask, IntPtr* displayLinkOut);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static CVDisplayLink? CreateFromOpenGLMask (uint mask, out CVReturn error)
		{
			IntPtr handle;
			unsafe {
				error = CVDisplayLinkCreateWithOpenGLDisplayMask (mask, &handle);
			}
			if (error != 0)
				return null;
			return new CVDisplayLink (handle, true);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static CVDisplayLink? CreateFromOpenGLMask (uint mask)
			=> CreateFromOpenGLMask (mask, out var _);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVDisplayLinkRetain (IntPtr handle);
		
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVDisplayLinkRelease (IntPtr handle);
		
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		protected internal override void Retain ()
		{
			CVDisplayLinkRetain (GetCheckedHandle ());
		}

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		protected internal override void Release ()
		{
			CVDisplayLinkRelease (GetCheckedHandle ());
		}

		protected override void Dispose (bool disposing)
		{
			if (callbackHandle.IsAllocated) {
				callbackHandle.Free();
			}

			base.Dispose (disposing);
		}

		[DllImport (Constants.CoreVideoLibrary)]
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		unsafe extern static CVReturn CVDisplayLinkCreateWithActiveCGDisplays (IntPtr* displayLinkOut);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		static IntPtr Create ()
		{
			CVReturn ret;
			IntPtr handle;
			unsafe {
				ret = CVDisplayLinkCreateWithActiveCGDisplays (&handle);
			}

			if (ret != CVReturn.Success)
				throw new Exception ("CVDisplayLink returned: " + ret);

			return handle;

		}

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVDisplayLink ()
			: base (Create (), true)
		{
		}		

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetCurrentCGDisplay (IntPtr displayLink, int /* CGDirectDisplayID = uint32_t */ displayId);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVReturn SetCurrentDisplay (int displayId)
		{
			return CVDisplayLinkSetCurrentCGDisplay (Handle, displayId);
		}     
			
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext (IntPtr displayLink, IntPtr cglContext, IntPtr cglPixelFormat);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVReturn SetCurrentDisplay (CGLContext cglContext, CGLPixelFormat cglPixelFormat)
		{
			return CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext (Handle, cglContext.Handle, cglPixelFormat.Handle);
		}     

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static int /* CGDirectDisplayID = uint32_t */ CVDisplayLinkGetCurrentCGDisplay (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public int GetCurrentDisplay ()
		{
			return CVDisplayLinkGetCurrentCGDisplay (Handle);
		}
			
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkStart (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVReturn Start ()
		{
			return CVDisplayLinkStart (Handle);
		}		     
			
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkStop (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVReturn Stop ()
		{
			return CVDisplayLinkStop (Handle);
		}		     
			
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVTime CVDisplayLinkGetNominalOutputVideoRefreshPeriod (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVTime NominalOutputVideoRefreshPeriod {
			get {
				return CVDisplayLinkGetNominalOutputVideoRefreshPeriod (Handle);
			}
		}

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVTime CVDisplayLinkGetOutputVideoLatency (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVTime OutputVideoLatency {
			get {
				return CVDisplayLinkGetOutputVideoLatency (Handle);
			}
		}

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static double CVDisplayLinkGetActualOutputVideoRefreshPeriod (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public double ActualOutputVideoRefreshPeriod {
			get {
				return CVDisplayLinkGetActualOutputVideoRefreshPeriod (Handle);
			}
		}
			
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static byte CVDisplayLinkIsRunning (IntPtr displayLink);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public bool IsRunning {
			get {
				return CVDisplayLinkIsRunning (Handle) != 0;
			}
		}
			
#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static CVReturn CVDisplayLinkGetCurrentTime (IntPtr displayLink, CVTimeStamp* outTime);

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVReturn GetCurrentTime (out CVTimeStamp outTime)
		{
			CVReturn ret;
			outTime = default;
			unsafe {
				ret = CVDisplayLinkGetCurrentTime (this.Handle, (CVTimeStamp *) Unsafe.AsPointer<CVTimeStamp> (ref outTime));
			}
				
			return ret;
		}
		
		public delegate CVReturn DisplayLinkOutputCallback (CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut);	
		delegate CVReturn CVDisplayLinkOutputCallback (IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr displayLinkContext);		
	  
#if NET
		[UnmanagedCallersOnly]
		static unsafe CVReturn OutputCallback (IntPtr displayLink, CVTimeStamp* inNow, CVTimeStamp* inOutputTime, CVOptionFlags flagsIn, CVOptionFlags* flagsOut, IntPtr displayLinkContext)
#else
		static CVDisplayLinkOutputCallback static_OutputCallback = new CVDisplayLinkOutputCallback (OutputCallback);
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CVDisplayLinkOutputCallback))]
#endif
		static CVReturn OutputCallback (IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr displayLinkContext)
#endif
		{
			GCHandle callbackHandle = GCHandle.FromIntPtr (displayLinkContext);
			DisplayLinkOutputCallback func = (DisplayLinkOutputCallback) callbackHandle.Target!;
			CVDisplayLink delegateDisplayLink = new CVDisplayLink(displayLink, false);
#if NET
			return func (delegateDisplayLink,
				ref System.Runtime.CompilerServices.Unsafe.AsRef<CVTimeStamp> (inNow),
				ref System.Runtime.CompilerServices.Unsafe.AsRef<CVTimeStamp> (inOutputTime),
				flagsIn, ref System.Runtime.CompilerServices.Unsafe.AsRef<CVOptionFlags> (flagsOut));
#else
			return func (delegateDisplayLink, ref inNow, ref inOutputTime, flagsIn, ref flagsOut);
#endif
		}

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static unsafe CVReturn CVDisplayLinkSetOutputCallback (IntPtr displayLink, delegate* unmanaged<IntPtr, CVTimeStamp*, CVTimeStamp*, CVOptionFlags, CVOptionFlags *, IntPtr, CVReturn> function, IntPtr userInfo);
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetOutputCallback (IntPtr displayLink, CVDisplayLinkOutputCallback function, IntPtr userInfo);
#endif

#if NET
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
#endif
		public CVReturn SetOutputCallback (DisplayLinkOutputCallback callback)
		{
			callbackHandle = GCHandle.Alloc (callback);
#if NET
			unsafe {
				CVReturn ret = CVDisplayLinkSetOutputCallback (this.Handle, &OutputCallback, GCHandle.ToIntPtr (callbackHandle));
				return ret;
			}
#else
			CVReturn ret = CVDisplayLinkSetOutputCallback (this.Handle, static_OutputCallback, GCHandle.ToIntPtr (callbackHandle));
			return ret;
#endif
				
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		static extern nuint CVDisplayLinkGetTypeID ();

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public static nuint GetTypeId ()
			=> CVDisplayLinkGetTypeID ();

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern int CVDisplayLinkTranslateTime (IntPtr displayLink, CVTimeStamp inTime, CVTimeStamp* outTime);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
#endif
		public bool TryTranslateTime (CVTimeStamp inTime, ref CVTimeStamp outTime)
		{
			unsafe {
				return CVDisplayLinkTranslateTime (this.Handle, inTime, (CVTimeStamp *) Unsafe.AsPointer<CVTimeStamp> (ref outTime)) == 0;
			}
		}
	}
}

#endif // MONOMAC
