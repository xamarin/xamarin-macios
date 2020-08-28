//
// VNCircle.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;

namespace Vision {
	public partial class VNCircle {

		public static VNCircle CreateUsingRadius (VNPoint center, double radius)
		{
			var handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle ("alloc"));
			handle = Messaging.IntPtr_objc_msgSend_IntPtr_Double (handle, Selector.GetHandle ("initWithCenter:radius:"), center.Handle, radius);
			return Runtime.GetNSObject<VNCircle> (handle, true);
		}

		public static VNCircle CreateUsingDiameter (VNPoint center, double diameter)
		{
			var handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle ("alloc"));
			handle = Messaging.IntPtr_objc_msgSend_IntPtr_Double (handle, Selector.GetHandle ("initWithCenter:diameter:"), center.Handle, diameter);
			return Runtime.GetNSObject<VNCircle> (handle, true);
		}
	}
}
