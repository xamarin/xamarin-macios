//
// UIBezierPath.cs: Extra methods for UIBezierPath
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2014-2015, Xamarin Inc
//

using System;
using System.Collections;
using ObjCRuntime;
using Foundation;

namespace UIKit {
	public partial class UIBezierPath {

		// from AppKit/NSBezierPath.cs
		public unsafe void GetLineDash (out nfloat [] pattern, out nfloat phase)
		{
			nint length;

			//Call the internal method with null to get the length of the pattern array
			_GetLineDash (IntPtr.Zero, out length, out phase);

			pattern = new nfloat [length];
			fixed (nfloat* ptr = pattern)
				_GetLineDash ((IntPtr) ptr, out length, out phase);
		}

		public void SetLineDash (nfloat [] values, nfloat phase)
		{
			if (values is null) {
				SetLineDash (IntPtr.Zero, 0, phase);
				return;
			}
			unsafe {
				fixed (nfloat* fp = &values [0]) {
					SetLineDash ((IntPtr) fp, values.Length, phase);
				}
			}
		}

		public void SetLineDash (nfloat [] values, nint offset, nint count, nfloat phase)
		{
			if (offset + count > values.Length)
				throw new ArgumentException ("the provided offset and count would access data beyond the values array");

			unsafe {
				fixed (nfloat* fp = &values [offset]) {
					SetLineDash ((IntPtr) fp, count, phase);
				}
			}
		}
	}

}
