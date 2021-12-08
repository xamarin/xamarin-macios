#if !XAMCORE_4_0
using System;

using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Metal {

	public partial class MTLSharedTextureHandle {

		[Obsolete ("Type is not meant to be created by user code.")]
		public MTLSharedTextureHandle () {}
	}

#if MONOMAC
	public static partial class MTLDevice_Extensions {
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLCounterSet[] GetIMTLCounterSets (this IMTLDevice This)
		{
			return NSArray.ArrayFromHandle<IMTLCounterSet>(global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (This.Handle, Selector.GetHandle ("counterSets")));
		}

#if !NET
		[Introduced (PlatformName.MacOSX, 10,15, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("macos10.15")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLCounterSampleBuffer? CreateIMTLCounterSampleBuffer (this IMTLDevice This, MTLCounterSampleBufferDescriptor descriptor, out NSError? error)
		{
			if (descriptor == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			var errorValue = NativeHandle.Zero;

#if NET
			var rv = global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend_NativeHandle_ref_NativeHandle (This.Handle, Selector.GetHandle ("newCounterSampleBufferWithDescriptor:error:"), descriptor.Handle, ref errorValue);
#else
			var rv = global::ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_ref_IntPtr (This.Handle, Selector.GetHandle ("newCounterSampleBufferWithDescriptor:error:"), descriptor.Handle, ref errorValue);
#endif

			var ret = Runtime.GetINativeObject<IMTLCounterSampleBuffer> (rv, owns: false);
			error = Runtime.GetNSObject<NSError> (errorValue);

			return ret;
		}
	}

	public static partial class MTLComputeCommandEncoder_Extensions {
#if !NET
		[Introduced (PlatformName.MacOSX, 10,15, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("macos10.15")]
#endif

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void SampleCounters (this IMTLComputeCommandEncoder This, IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier)
		{
			if (sampleBuffer == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sampleBuffer));
#if NET
			global::ObjCRuntime.Messaging.void_objc_msgSend_NativeHandle_UIntPtr_bool (This.Handle, Selector.GetHandle ("sampleCountersInBuffer:atSampleIndex:withBarrier:"), sampleBuffer.Handle, (UIntPtr) sampleIndex, barrier);
#else
			global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_UIntPtr_bool (This.Handle, Selector.GetHandle ("sampleCountersInBuffer:atSampleIndex:withBarrier:"), sampleBuffer.Handle, (UIntPtr) sampleIndex, barrier);
#endif
		}
	}
#endif // MONOMAC
}
#endif // !XAMCORE_4_0
