// 
// IOSurface.cs
//
// Copyright 2016 Microsoft 
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

using System;
using System.Runtime.InteropServices;

using CoreFoundation;

using ObjCRuntime;

namespace IOSurface {
	public partial class IOSurface {

		// kern_return_t
		// See bug #59201 
		public int Lock (IOSurfaceLockOptions options, ref int seed)
		{
			unsafe {
				fixed (int* p = &seed) {
					return _Lock (options, (IntPtr) p);
				}
			}
		}

		// kern_return_t
		// See bug #59201 
		public int Lock (IOSurfaceLockOptions options)
		{
			return _Lock (options, IntPtr.Zero);
		}

		// kern_return_t
		// See bug #59201 
		public int Unlock (IOSurfaceLockOptions options, ref int seed)
		{
			unsafe {
				fixed (int* p = &seed) {
					return _Unlock (options, (IntPtr) p);
				}
			}
		}

		// kern_return_t
		// See bug #59201 
		public int Unlock (IOSurfaceLockOptions options)
		{
			return _Unlock (options, IntPtr.Zero);
		}

#if !MONOMAC
		// kern_return_t
		public int SetPurgeable (IOSurfacePurgeabilityState newState, ref IOSurfacePurgeabilityState oldState)
		{
			unsafe {
				fixed (IOSurfacePurgeabilityState* p = &oldState) {
					return _SetPurgeable (newState, (IntPtr) p);
				}
			}
		}

		public int SetPurgeable (IOSurfacePurgeabilityState newState)
		{
			return _SetPurgeable (newState, IntPtr.Zero);
		}
#endif
	}
}
