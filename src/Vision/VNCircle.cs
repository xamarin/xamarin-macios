//
// VNCircle.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace Vision {
	public partial class VNCircle {

		public static VNCircle? CreateUsingRadius (VNPoint center, double radius)
		{
			var handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle ("alloc"));
#if NET
			handle = Messaging.NativeHandle_objc_msgSend_NativeHandle_Double (handle, Selector.GetHandle ("initWithCenter:radius:"), center.Handle, radius);
#else
			handle = Messaging.IntPtr_objc_msgSend_IntPtr_Double (handle, Selector.GetHandle ("initWithCenter:radius:"), center.Handle, radius);
#endif
			return Runtime.GetNSObject<VNCircle> (handle, true);
		}

		public static VNCircle? CreateUsingDiameter (VNPoint center, double diameter)
		{
			var handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle ("alloc"));
#if NET
			handle = Messaging.NativeHandle_objc_msgSend_NativeHandle_Double (handle, Selector.GetHandle ("initWithCenter:diameter:"), center.Handle, diameter);
#else
			handle = Messaging.IntPtr_objc_msgSend_IntPtr_Double (handle, Selector.GetHandle ("initWithCenter:diameter:"), center.Handle, diameter);
#endif
			return Runtime.GetNSObject<VNCircle> (handle, true);
		}
	}
}
