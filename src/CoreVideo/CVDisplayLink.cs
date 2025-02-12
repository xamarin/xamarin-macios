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

#nullable enable

namespace CoreVideo {
	[SupportedOSPlatform ("macos")]
	public class CVDisplayLink : NativeObject {
		GCHandle callbackHandle;

		[Preserve (Conditional=true)]
		internal CVDisplayLink (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern CVReturn CVDisplayLinkCreateWithCGDisplay (uint displayId, IntPtr* displayLink);

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
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

		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public static CVDisplayLink? CreateFromDisplayId (uint displayId)
			=> CreateFromDisplayId (displayId, out var _);

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern CVReturn CVDisplayLinkCreateWithCGDisplays (uint* displayArray, nint count, IntPtr* displayLink);

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
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

		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public static CVDisplayLink? CreateFromDisplayIds (uint[] displayIds)
			=> CreateFromDisplayIds (displayIds, out var _);

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern CVReturn CVDisplayLinkCreateWithOpenGLDisplayMask (uint mask, IntPtr* displayLinkOut);

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
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

		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public static CVDisplayLink? CreateFromOpenGLMask (uint mask)
			=> CreateFromOpenGLMask (mask, out var _);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVDisplayLinkRetain (IntPtr handle);
		
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVDisplayLinkRelease (IntPtr handle);
		
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		protected internal override void Retain ()
		{
			CVDisplayLinkRetain (GetCheckedHandle ());
		}

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
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
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		unsafe extern static CVReturn CVDisplayLinkCreateWithActiveCGDisplays (IntPtr* displayLinkOut);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
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

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVDisplayLink ()
			: base (Create (), true)
		{
		}		

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetCurrentCGDisplay (IntPtr displayLink, int /* CGDirectDisplayID = uint32_t */ displayId);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVReturn SetCurrentDisplay (int displayId)
		{
			return CVDisplayLinkSetCurrentCGDisplay (Handle, displayId);
		}     
			
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext (IntPtr displayLink, IntPtr cglContext, IntPtr cglPixelFormat);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVReturn SetCurrentDisplay (CGLContext cglContext, CGLPixelFormat cglPixelFormat)
		{
			return CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext (Handle, cglContext.Handle, cglPixelFormat.Handle);
		}     

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static int /* CGDirectDisplayID = uint32_t */ CVDisplayLinkGetCurrentCGDisplay (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public int GetCurrentDisplay ()
		{
			return CVDisplayLinkGetCurrentCGDisplay (Handle);
		}
			
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkStart (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVReturn Start ()
		{
			return CVDisplayLinkStart (Handle);
		}		     
			
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkStop (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVReturn Stop ()
		{
			return CVDisplayLinkStop (Handle);
		}		     
			
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVTime CVDisplayLinkGetNominalOutputVideoRefreshPeriod (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVTime NominalOutputVideoRefreshPeriod {
			get {
				return CVDisplayLinkGetNominalOutputVideoRefreshPeriod (Handle);
			}
		}

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVTime CVDisplayLinkGetOutputVideoLatency (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVTime OutputVideoLatency {
			get {
				return CVDisplayLinkGetOutputVideoLatency (Handle);
			}
		}

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static double CVDisplayLinkGetActualOutputVideoRefreshPeriod (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public double ActualOutputVideoRefreshPeriod {
			get {
				return CVDisplayLinkGetActualOutputVideoRefreshPeriod (Handle);
			}
		}
			
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static byte CVDisplayLinkIsRunning (IntPtr displayLink);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public bool IsRunning {
			get {
				return CVDisplayLinkIsRunning (Handle) != 0;
			}
		}
			
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static CVReturn CVDisplayLinkGetCurrentTime (IntPtr displayLink, CVTimeStamp* outTime);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
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
	  
		[UnmanagedCallersOnly]
		static unsafe CVReturn OutputCallback (IntPtr displayLink, CVTimeStamp* inNow, CVTimeStamp* inOutputTime, CVOptionFlags flagsIn, CVOptionFlags* flagsOut, IntPtr displayLinkContext)
		{
			GCHandle callbackHandle = GCHandle.FromIntPtr (displayLinkContext);
			DisplayLinkOutputCallback func = (DisplayLinkOutputCallback) callbackHandle.Target!;
			CVDisplayLink delegateDisplayLink = new CVDisplayLink(displayLink, false);
			return func (delegateDisplayLink,
				ref System.Runtime.CompilerServices.Unsafe.AsRef<CVTimeStamp> (inNow),
				ref System.Runtime.CompilerServices.Unsafe.AsRef<CVTimeStamp> (inOutputTime),
				flagsIn, ref System.Runtime.CompilerServices.Unsafe.AsRef<CVOptionFlags> (flagsOut));
		}

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreVideoLibrary)]
		extern static unsafe CVReturn CVDisplayLinkSetOutputCallback (IntPtr displayLink, delegate* unmanaged<IntPtr, CVTimeStamp*, CVTimeStamp*, CVOptionFlags, CVOptionFlags *, IntPtr, CVReturn> function, IntPtr userInfo);

		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[SupportedOSPlatform ("macos")]
		public CVReturn SetOutputCallback (DisplayLinkOutputCallback callback)
		{
			callbackHandle = GCHandle.Alloc (callback);
			unsafe {
				CVReturn ret = CVDisplayLinkSetOutputCallback (this.Handle, &OutputCallback, GCHandle.ToIntPtr (callbackHandle));
				return ret;
			}
		}

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreVideoLibrary)]
		static extern nuint CVDisplayLinkGetTypeID ();

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public static nuint GetTypeId ()
			=> CVDisplayLinkGetTypeID ();

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern int CVDisplayLinkTranslateTime (IntPtr displayLink, CVTimeStamp inTime, CVTimeStamp* outTime);

		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use 'NSView.GetDisplayLink', 'NSWindow.GetDisplayLink' or 'NSScreen.GetDisplayLink' instead.")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public bool TryTranslateTime (CVTimeStamp inTime, ref CVTimeStamp outTime)
		{
			unsafe {
				return CVDisplayLinkTranslateTime (this.Handle, inTime, (CVTimeStamp *) Unsafe.AsPointer<CVTimeStamp> (ref outTime)) == 0;
			}
		}
	}
}

#endif // MONOMAC
