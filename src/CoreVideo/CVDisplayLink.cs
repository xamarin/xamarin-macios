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
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using OpenGL;

namespace CoreVideo {
	public class CVDisplayLink : INativeObject, IDisposable {
		IntPtr handle;
		GCHandle callbackHandle;
		
		public CVDisplayLink (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new Exception ("Invalid parameters to display link creation");

			CVDisplayLinkRetain (handle);
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CVDisplayLink (IntPtr handle, bool owns)
		{
			if (!owns)
				CVDisplayLinkRetain (handle);

			this.handle = handle;
		}

		~CVDisplayLink ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVDisplayLinkRetain (IntPtr handle);
		
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVDisplayLinkRelease (IntPtr handle);
		
		protected virtual void Dispose (bool disposing)
		{
			if (callbackHandle.IsAllocated) {
				callbackHandle.Free();
			}

			if (handle != IntPtr.Zero){
				CVDisplayLinkRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkCreateWithActiveCGDisplays (out IntPtr displayLinkOut);
		public CVDisplayLink ()
		{
			IntPtr displayLinkOut;
		
			CVReturn ret = CVDisplayLinkCreateWithActiveCGDisplays (out displayLinkOut);
	
			if (ret != CVReturn.Success)
				throw new Exception ("CVDisplayLink returned: " + ret);
	
			this.handle = displayLinkOut;
		}		

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetCurrentCGDisplay (IntPtr displayLink, int /* CGDirectDisplayID = uint32_t */ displayId);
		public CVReturn SetCurrentDisplay (int displayId)
		{
			return CVDisplayLinkSetCurrentCGDisplay (this.handle, displayId);
		}     
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext (IntPtr displayLink, IntPtr cglContext, IntPtr cglPixelFormat);
		public CVReturn SetCurrentDisplay (CGLContext cglContext, CGLPixelFormat cglPixelFormat)
		{
			return CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext (this.handle, cglContext.Handle, cglPixelFormat.Handle);
		}     

		[DllImport (Constants.CoreVideoLibrary)]
		extern static int /* CGDirectDisplayID = uint32_t */ CVDisplayLinkGetCurrentCGDisplay (IntPtr displayLink);
		public int GetCurrentDisplay ()
		{
			return CVDisplayLinkGetCurrentCGDisplay (this.handle);
		}
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkStart (IntPtr displayLink);
		public CVReturn Start ()
		{
			return CVDisplayLinkStart (this.handle);
		}		     
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkStop (IntPtr displayLink);
		public CVReturn Stop ()
		{
			return CVDisplayLinkStop (this.handle);
		}		     
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVTime CVDisplayLinkGetNominalOutputVideoRefreshPeriod (IntPtr displayLink);
		public CVTime NominalOutputVideoRefreshPeriod {
			get {
				return CVDisplayLinkGetNominalOutputVideoRefreshPeriod (this.handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVTime CVDisplayLinkGetOutputVideoLatency (IntPtr displayLink);
		public CVTime OutputVideoLatency {
			get {
				return CVDisplayLinkGetOutputVideoLatency (this.handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static double CVDisplayLinkGetActualOutputVideoRefreshPeriod (IntPtr displayLink);
		public double ActualOutputVideoRefreshPeriod {
			get {
				return CVDisplayLinkGetActualOutputVideoRefreshPeriod (this.handle);
			}
		}
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static bool CVDisplayLinkIsRunning (IntPtr displayLink);
		public bool IsRunning {
			get {
				return CVDisplayLinkIsRunning (this.handle);
			}
		}
			
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkGetCurrentTime (IntPtr displayLink, out CVTimeStamp outTime);
		public CVReturn GetCurrentTime (out CVTimeStamp outTime)
		{
			CVReturn ret = CVDisplayLinkGetCurrentTime (this.Handle, out outTime);
				
			return ret;
		}
		
		public delegate CVReturn DisplayLinkOutputCallback (CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut);	
		delegate CVReturn CVDisplayLinkOutputCallback (IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr displayLinkContext);		
	  
		static CVDisplayLinkOutputCallback static_OutputCallback = new CVDisplayLinkOutputCallback (OutputCallback);
			
	#if !MONOMAC
		[MonoPInvokeCallback (typeof (CVDisplayLinkOutputCallback))]
	#endif
		static CVReturn OutputCallback (IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr displayLinkContext)
		{
			GCHandle callbackHandle = GCHandle.FromIntPtr (displayLinkContext);
			DisplayLinkOutputCallback func = (DisplayLinkOutputCallback) callbackHandle.Target;
			CVDisplayLink delegateDisplayLink = new CVDisplayLink(displayLink, false);
			return func (delegateDisplayLink, ref inNow, ref inOutputTime, flagsIn, ref flagsOut);
		}
	  
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVDisplayLinkSetOutputCallback (IntPtr displayLink, CVDisplayLinkOutputCallback function, IntPtr userInfo);
		public CVReturn SetOutputCallback (DisplayLinkOutputCallback callback)
		{
			callbackHandle = GCHandle.Alloc (callback);
			CVReturn ret = CVDisplayLinkSetOutputCallback (this.Handle, static_OutputCallback, GCHandle.ToIntPtr (callbackHandle));
				
			return ret;
		}
	}
}

#endif // MONOMAC

