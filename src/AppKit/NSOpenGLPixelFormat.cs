//
// Copyright 2010, Novell, Inc.
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

#if !__MACCATALYST__

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreImage;
using CoreAnimation;

#nullable enable

namespace AppKit {
	public partial class NSOpenGLPixelFormat {
		static IntPtr selInitWithAttributes = Selector.GetHandle ("initWithAttributes:");

		public NSOpenGLPixelFormat (NSOpenGLPixelFormatAttribute [] attribs) : base (NSObjectFlag.Empty)
		{
			if (attribs is null)
				throw new ArgumentNullException ("attribs");

			unsafe {
				NSOpenGLPixelFormatAttribute [] copy = new NSOpenGLPixelFormatAttribute [attribs.Length + 1];
				Array.Copy (attribs, 0, copy, 0, attribs.Length);

				fixed (NSOpenGLPixelFormatAttribute* pArray = copy) {
					if (IsDirectBinding) {
						Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selInitWithAttributes, new IntPtr ((void*) pArray));
					} else {
						Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, selInitWithAttributes, new IntPtr ((void*) pArray));
					}
				}

			}
		}

		public NSOpenGLPixelFormat (uint [] attribs) : base (NSObjectFlag.Empty)
		{
			if (attribs is null)
				throw new ArgumentNullException ("attribs");

			unsafe {
				uint [] copy = new uint [attribs.Length + 1];
				Array.Copy (attribs, 0, copy, 0, attribs.Length);

				fixed (uint* pArray = copy) {
					if (IsDirectBinding) {
						InitializeHandle (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selInitWithAttributes, new IntPtr ((void*) pArray)));
					} else {
						InitializeHandle (ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, selInitWithAttributes, new IntPtr ((void*) pArray)));
					}
				}

			}
		}

		static uint [] ConvertToAttributes (object [] args)
		{

			var list = new List<uint> ();
			for (int i = 0; i < args.Length; i++) {
				if (args [i] is NSOpenGLPixelFormatAttribute) {
					NSOpenGLPixelFormatAttribute v = (NSOpenGLPixelFormatAttribute) args [i];
					switch (v) {
					case NSOpenGLPixelFormatAttribute.AllRenderers:
					case NSOpenGLPixelFormatAttribute.DoubleBuffer:
					case NSOpenGLPixelFormatAttribute.Stereo:
					case NSOpenGLPixelFormatAttribute.MinimumPolicy:
					case NSOpenGLPixelFormatAttribute.MaximumPolicy:
					case NSOpenGLPixelFormatAttribute.OffScreen:
					case NSOpenGLPixelFormatAttribute.FullScreen:
					case NSOpenGLPixelFormatAttribute.SingleRenderer:
					case NSOpenGLPixelFormatAttribute.NoRecovery:
					case NSOpenGLPixelFormatAttribute.Accelerated:
					case NSOpenGLPixelFormatAttribute.ClosestPolicy:
					case NSOpenGLPixelFormatAttribute.Robust:
					case NSOpenGLPixelFormatAttribute.BackingStore:
					case NSOpenGLPixelFormatAttribute.Window:
					case NSOpenGLPixelFormatAttribute.MultiScreen:
					case NSOpenGLPixelFormatAttribute.Compliant:
					case NSOpenGLPixelFormatAttribute.PixelBuffer:

					// Not listed in the docs, but header file implies it
					case NSOpenGLPixelFormatAttribute.RemotePixelBuffer:
					case NSOpenGLPixelFormatAttribute.AuxDepthStencil:
					case NSOpenGLPixelFormatAttribute.ColorFloat:
					case NSOpenGLPixelFormatAttribute.Multisample:
					case NSOpenGLPixelFormatAttribute.Supersample:
					case NSOpenGLPixelFormatAttribute.SampleAlpha:
					case NSOpenGLPixelFormatAttribute.AllowOfflineRenderers:
					case NSOpenGLPixelFormatAttribute.AcceleratedCompute:
					case NSOpenGLPixelFormatAttribute.MPSafe:
						list.Add ((uint) v);
						break;

					case NSOpenGLPixelFormatAttribute.AuxBuffers:
					case NSOpenGLPixelFormatAttribute.ColorSize:
					case NSOpenGLPixelFormatAttribute.AlphaSize:
					case NSOpenGLPixelFormatAttribute.DepthSize:
					case NSOpenGLPixelFormatAttribute.StencilSize:
					case NSOpenGLPixelFormatAttribute.AccumSize:
					case NSOpenGLPixelFormatAttribute.RendererID:
					case NSOpenGLPixelFormatAttribute.ScreenMask:
					case NSOpenGLPixelFormatAttribute.OpenGLProfile:

					// not listed in the docs, but header file implies it
					case NSOpenGLPixelFormatAttribute.SampleBuffers:
					case NSOpenGLPixelFormatAttribute.Samples:
					case NSOpenGLPixelFormatAttribute.VirtualScreenCount:
						list.Add ((uint) (int) v);
						i++;
						if (i >= args.Length)
							throw new ArgumentException ("Attribute " + v + " needs a value");
						list.Add ((uint) (int) args [i]);
						break;
					}
				} else if (args [i] is int && (int) args [i] == 0 && i == args.Length - 1)
					list.Add (0);
				else
					throw new ArgumentException ($"The specified attribute is not of type NSOpenGLPixelFormatAttribute: {args [i]}");
			}

			if (args.Length == 0 || !(args [args.Length - 1] is int) || ((int) args [args.Length - 1]) != 0)
				list.Add (0);

			return list.ToArray ();
		}

		public NSOpenGLPixelFormat (params object [] attribs) : this (ConvertToAttributes (attribs))
		{
			if (attribs is null)
				throw new ArgumentNullException ("attribs");

			unsafe {
				var copy = new NSOpenGLPixelFormatAttribute [attribs.Length + 1 /* null termination */];
				for (int i = 0; i < attribs.Length; i++) {
					var input = attribs [i];
					if (input is NSOpenGLPixelFormatAttribute) {
						copy [i] = (NSOpenGLPixelFormatAttribute) input;
					} else {
						copy [i] = (NSOpenGLPixelFormatAttribute) (int) input;
					}
				}

				fixed (NSOpenGLPixelFormatAttribute* pArray = copy) {
					if (IsDirectBinding) {
						Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selInitWithAttributes, new IntPtr ((void*) pArray));
					} else {
						Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, selInitWithAttributes, new IntPtr ((void*) pArray));
					}
				}

			}
		}
	}
}
#endif // !__MACCATALYST__
