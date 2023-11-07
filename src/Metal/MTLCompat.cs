#if !NET
using System;

using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;

using NativeHandle = System.IntPtr;

#nullable enable

namespace Metal {

	public partial class MTLSharedTextureHandle {

		[Obsolete ("Type is not meant to be created by user code.")]
		public MTLSharedTextureHandle () { }
	}

#if MONOMAC
	public unsafe static partial class MTLDevice_Extensions {
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLCounterSet[] GetIMTLCounterSets (this IMTLDevice This)
		{
			return NSArray.ArrayFromHandle<IMTLCounterSet>(global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (This.Handle, Selector.GetHandle ("counterSets")));
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLCounterSampleBuffer? CreateIMTLCounterSampleBuffer (this IMTLDevice This, MTLCounterSampleBufferDescriptor descriptor, out NSError? error)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			var errorValue = NativeHandle.Zero;

			var rv = global::ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_ref_IntPtr (This.Handle, Selector.GetHandle ("newCounterSampleBufferWithDescriptor:error:"), descriptor.Handle, &errorValue);
			var ret = Runtime.GetINativeObject<IMTLCounterSampleBuffer> (rv, owns: false);
			error = Runtime.GetNSObject<NSError> (errorValue);

			return ret;
		}
	}

	public static partial class MTLComputeCommandEncoder_Extensions {
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void SampleCounters (this IMTLComputeCommandEncoder This, IMTLCounterSampleBuffer sampleBuffer, nuint sampleIndex, bool barrier)
		{
			if (sampleBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sampleBuffer));
			global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_UIntPtr_bool (This.Handle, Selector.GetHandle ("sampleCountersInBuffer:atSampleIndex:withBarrier:"), sampleBuffer.Handle, (UIntPtr) sampleIndex, barrier ? (byte) 1 : (byte) 0);
		}
	}
#endif // MONOMAC
}
#endif // !NET
