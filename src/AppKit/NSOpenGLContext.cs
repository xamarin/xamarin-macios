using System;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif

using ObjCRuntime;
using Foundation;

namespace AppKit {
	
	public partial class NSOpenGLContext {

		unsafe void SetValue (int /* GLint */ val, NSOpenGLContextParameter par)
		{
			int *p = &val;
			SetValues ((IntPtr) p, par);
		}
			
		unsafe int /* GLint */ GetValue (NSOpenGLContextParameter par)
		{
			int ret;
			int *p = &ret;
			GetValues ((IntPtr)p, par);

			return ret;
		}

#if !NO_SYSTEM_DRAWING
		unsafe public Rectangle SwapRectangle {
			get {
				Rectangle ret;
				GetValues ((IntPtr) (&ret), NSOpenGLContextParameter.SwapRectangle);
				return ret;
			}
			set {
				SetValues ((IntPtr) (&value), NSOpenGLContextParameter.SwapRectangle);
			}
		}
#endif

		public bool SwapRectangleEnabled {
			get {
				return GetValue (NSOpenGLContextParameter.SwapRectangleEnable) != 0;
			}
			set {
				SetValue (value ? 1 : 0, NSOpenGLContextParameter.SwapRectangleEnable);
			}
		}

		public bool RasterizationEnabled {
			get {
				return GetValue (NSOpenGLContextParameter.RasterizationEnable) != 0;
			}
			set {
				SetValue (value ? 1 : 0, NSOpenGLContextParameter.RasterizationEnable);
			}
		}

		public bool SwapInterval {
			get {
				return GetValue (NSOpenGLContextParameter.SwapInterval) != 0;
			}
			set {
				SetValue (value ? 1 : 0, NSOpenGLContextParameter.SwapInterval);
			}
		}

		public NSSurfaceOrder SurfaceOrder {
			get {
				switch (GetValue (NSOpenGLContextParameter.SurfaceOrder)){
				case -1:
					return NSSurfaceOrder.BelowWindow;
				default:
					return NSSurfaceOrder.AboveWindow;
				}
			}
			set {
				SetValue (value == NSSurfaceOrder.BelowWindow ? -1 : 1, NSOpenGLContextParameter.SurfaceOrder);
			}
		}

		public bool SurfaceOpaque {
			get {
				return GetValue (NSOpenGLContextParameter.SurfaceOpacity) != 0;
			}
			set {
				SetValue (value ? 1 : 0, NSOpenGLContextParameter.SurfaceOpacity);
			}
		}

		public bool StateValidation {
			get {
				return GetValue (NSOpenGLContextParameter.StateValidation) != 0;
			}
			set {
				SetValue (value ? 1 : 0, NSOpenGLContextParameter.StateValidation);
			}
		}
	}
}
