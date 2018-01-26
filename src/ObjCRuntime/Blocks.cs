//
// Block support
//
// Copyright 2010, Novell, Inc.
// Copyright 2011 - 2013 Xamarin Inc
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
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

// http://clang.llvm.org/docs/Block-ABI-Apple.html

namespace XamCore.ObjCRuntime {

	[StructLayout (LayoutKind.Sequential)]
#if !XAMCORE_2_0
	public 
#endif
	struct BlockDescriptor {
		public IntPtr reserved;
		public IntPtr size;
		public IntPtr copy_helper;
		public IntPtr dispose;
		public IntPtr signature;
	}

	struct XamarinBlockDescriptor {
		public BlockDescriptor descriptor;
		public volatile int ref_count;
		// followed by variable-length string (the signature)
	}

	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct BlockLiteral {
#pragma warning disable 169
#if XAMCORE_2_0
		IntPtr isa;
		BlockFlags flags;
		int reserved;
		IntPtr invoke;
		IntPtr block_descriptor;
		IntPtr local_handle;
		IntPtr global_handle;
#else
		public IntPtr isa;
		public BlockFlags flags;
		public int reserved;
		public IntPtr invoke;
		public IntPtr block_descriptor;
		public IntPtr local_handle;
		public IntPtr global_handle;
#endif
#pragma warning restore 169
#if !COREBUILD
		static IntPtr block_class = Class.GetHandle ("__NSStackBlock");

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_get_block_descriptor ();

		void SetupBlock (Delegate trampoline, Delegate userDelegate, bool safe)
		{
			// We need to get the signature of the target method, so that we can compute
			// the ObjC signature correctly (the generated method that's actually
			// invoked by native code does not have enough type information to compute
			// the correct signature).
			// This attribute might not exist for third-party libraries created
			// with earlier versions of Xamarin.iOS, so make sure to cope with
			// the attribute not being available.

			// This logic is mirrored in CoreOptimizeGeneratedCode.ProcessSetupBlock and must be
			// updated if anything changes here.
			var userDelegateType = trampoline.GetType ().GetCustomAttribute<UserDelegateTypeAttribute> ()?.UserDelegateType;
			bool blockSignature;
			MethodInfo userMethod;
			if (userDelegateType != null) {
				userMethod = userDelegateType.GetMethod ("Invoke");
				blockSignature = true;
			} else {
				userMethod = trampoline.Method;
				blockSignature = false;
			}

			var signature = Runtime.ComputeSignature (userMethod, blockSignature);
			SetupBlockImpl (trampoline, userDelegate, safe, signature);
		}

		// This method is not to be called manually by user code.
		// This is enforced by making it private. If the SetupBlock optimization is enabled,
		// the linker will make it public so that it's callable from optimized user code.
		unsafe void SetupBlockImpl (Delegate trampoline, Delegate userDelegate, bool safe, string signature)
		{
			isa = block_class;
			invoke = Marshal.GetFunctionPointerForDelegate (trampoline);
			object delegates;
			if (safe) {
				delegates = new Tuple<Delegate, Delegate> (trampoline, userDelegate);
			} else {
				delegates = userDelegate;
			}
			local_handle = (IntPtr) GCHandle.Alloc (delegates);
			global_handle = IntPtr.Zero;
			flags = BlockFlags.BLOCK_HAS_COPY_DISPOSE | BlockFlags.BLOCK_HAS_SIGNATURE;

			/* FIXME: support stret blocks */

			// we allocate one big block of memory, the first part is the BlockDescriptor, 
			// the second part is the signature string (no need to allocate a second time
			// for the signature if we can avoid it). One descriptor is allocated for every 
			// Block; this is potentially something the static registrar can fix, since it
			// should know every possible trampoline signature.
			var bytes = System.Text.Encoding.UTF8.GetBytes (signature);
			var desclen = sizeof (XamarinBlockDescriptor) + bytes.Length + 1 /* null character */;
			var descptr = Marshal.AllocHGlobal (desclen);

			block_descriptor = descptr;
			var xblock_descriptor = (XamarinBlockDescriptor *) block_descriptor;
			xblock_descriptor->descriptor = * (BlockDescriptor *) xamarin_get_block_descriptor ();
			xblock_descriptor->descriptor.signature = descptr + sizeof (BlockDescriptor) + 4 /* signature_length */;
			xblock_descriptor->ref_count = 1;
			Marshal.Copy (bytes, 0, xblock_descriptor->descriptor.signature, bytes.Length);
			Marshal.WriteByte (xblock_descriptor->descriptor.signature + bytes.Length, 0); // null terminate string
		}

		// trampoline must be static, and someone else needs to keep a ref to it
		[EditorBrowsable (EditorBrowsableState.Never)]
		public void SetupBlockUnsafe (Delegate trampoline, Delegate userDelegate)
		{
			SetupBlock (trampoline, userDelegate, safe: false);
		}

		// trampoline must be static, but it's not necessary to keep a ref to it
		public void SetupBlock (Delegate trampoline, Delegate userDelegate)
		{
			if (trampoline == null)
				throw new ArgumentNullException (nameof (trampoline));

#if !MONOMAC
			// Check that:
			// * The trampoline is static
			// * The trampoline's method has a [MonoPInvokeCallback] attribute
			// * The delegate in the [MonoPInvokeCallback] has the right signature
			//
			// WARNING: the XAMARIN_IOS_SKIP_BLOCK_CHECK will be removed in a future version, 
			//          if you find you need it, please file a bug with a test case and we'll 
			//          make sure your scenario works without the environment variable before removing it.
			if (Runtime.Arch == Arch.SIMULATOR && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_IOS_SKIP_BLOCK_CHECK"))) {
				// It should be enough to run this check in the simulator
				var method = trampoline.Method;
				if (!method.IsStatic)
					throw new ArgumentException ($"The method {method.DeclaringType.FullName}.{method.Name} is not static.", nameof (trampoline));
				var attrib = method.GetCustomAttribute<MonoPInvokeCallbackAttribute> (false);
				if (attrib == null)
					throw new ArgumentException ($"The method {method.DeclaringType.FullName}.{method.Name} does not have a [MonoPInvokeCallback] attribute.", nameof (trampoline));

				Type delegateType = attrib.DelegateType;
				var signatureMethod = delegateType.GetMethod ("Invoke");
				if (method.ReturnType != signatureMethod.ReturnType)
					throw new ArgumentException ($"The method {method.DeclaringType.FullName}.{method.Name}'s return type ({method.ReturnType.FullName}) does not match the return type of the delegate in its [MonoPInvokeCallback] attribute ({signatureMethod.ReturnType.FullName}).", nameof (trampoline));

				var parameters = method.GetParameters ();
				var signatureParameters = signatureMethod.GetParameters ();
				if (parameters.Length != signatureParameters.Length)
					throw new ArgumentException ($"The method {method.DeclaringType.FullName}.{method.Name}'s parameter count ({parameters.Length}) does not match the parameter count of the delegate in its [MonoPInvokeCallback] attribute ({signatureParameters.Length}).", nameof (trampoline));

				for (int i = 0; i < parameters.Length; i++) {
					if (parameters [i].ParameterType != signatureParameters [i].ParameterType)
						throw new ArgumentException ($"The method {method.DeclaringType.FullName}.{method.Name}'s parameter #{i + 1}'s type ({parameters [i].ParameterType.FullName}) does not match the corresponding parameter type of the delegate in its [MonoPInvokeCallback] attribute ({signatureParameters [i].ParameterType.FullName}).", nameof (trampoline));
				}
			}
#endif
			SetupBlock (trampoline, userDelegate, safe: true);
		}

		public void CleanupBlock ()
		{
			GCHandle.FromIntPtr (local_handle).Free ();
			var xblock_descriptor = (XamarinBlockDescriptor *) block_descriptor;
#pragma warning disable 420
			// CS0420: A volatile field references will not be treated as volatile
			// Documentation says: "A volatile field should not normally be passed using a ref or out parameter, since it will not be treated as volatile within the scope of the function. There are exceptions to this, such as when calling an interlocked API."
			// So ignoring the warning, since it's a documented exception.
			var rc = Interlocked.Decrement (ref xblock_descriptor->ref_count);
#pragma warning restore 420

			if (rc == 0)
				Marshal.FreeHGlobal (block_descriptor);
		}

		public object Target {
			get {
				var handle = global_handle != IntPtr.Zero ? global_handle : local_handle;
				var target = GCHandle.FromIntPtr (handle).Target;
				var tuple = target as Tuple<Delegate, Delegate>;
				if (tuple != null)
					return tuple.Item2;
				return target;
			}
		}

		public T GetDelegateForBlock<T> () where T: class
		{
			return (T) (object) Runtime.GetDelegateForBlock (invoke, typeof (T));
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public static bool IsManagedBlock (IntPtr block)
		{
			if (block == IntPtr.Zero)
				throw new ArgumentNullException ("block");

			BlockLiteral* literal = (BlockLiteral *) block;
			BlockDescriptor* descriptor = (BlockDescriptor *) xamarin_get_block_descriptor ();
			return descriptor->copy_helper == ((BlockDescriptor *) literal->block_descriptor)->copy_helper;
		}

		internal static IntPtr GetBlockForDelegate (MethodInfo minfo, object @delegate, string signature)
		{
			if (@delegate == null)
				return IntPtr.Zero;

			if (!(@delegate is Delegate))
				throw ErrorHelper.CreateError (8016, "Unable to convert delegate to block for the return value for the method {0}.{1}, because the input isn't a delegate, it's a {1}. Please file a bug at http://bugzilla.xamarin.com.", minfo.DeclaringType.FullName, minfo.Name, @delegate.GetType ().FullName);
				
			var baseMethod = minfo.GetBaseDefinition ();

			var delegateProxies = baseMethod.ReturnTypeCustomAttributes.GetCustomAttributes (typeof (DelegateProxyAttribute), false);
			if (delegateProxies.Length == 0)
				throw ErrorHelper.CreateError (8011, "Unable to locate the delegate to block conversion attribute ([DelegateProxy]) for the return value for the method {0}.{1}. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name);
			
			var delegateProxy = (DelegateProxyAttribute) delegateProxies [0];
			if (delegateProxy.DelegateType == null)
				throw ErrorHelper.CreateError (8012, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: DelegateType is null. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name);

			var delegateProxyField = delegateProxy.DelegateType.GetField ("Handler", BindingFlags.NonPublic | BindingFlags.Static);
			if (delegateProxyField == null)
				throw ErrorHelper.CreateError (8013, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: DelegateType ({2}) specifies a type without a 'Handler' field. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name, delegateProxy.DelegateType.FullName);

			var handlerDelegate = delegateProxyField.GetValue (null);
			if (handlerDelegate == null)
				throw ErrorHelper.CreateError (8014, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: The DelegateType's ({2}) 'Handler' field is null. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name, delegateProxy.DelegateType.FullName);

			if (!(handlerDelegate is Delegate))
				throw ErrorHelper.CreateError (8015, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: The DelegateType's ({2}) 'Handler' field is not a delegate, it's a {3}. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name, delegateProxy.DelegateType.FullName, handlerDelegate.GetType ().FullName);
			
			// We now have the information we need to create the block.
			// Note that we must create a heap-allocated block, so we 
			// start off by creating a stack-allocated block, and then
			// call _Block_copy, which will create a heap-allocated block
			// with the proper reference count.
			BlockLiteral block = new BlockLiteral ();
			if (signature == null) {
				block.SetupBlock ((Delegate) handlerDelegate, (Delegate) @delegate);
			} else {
				block.SetupBlockImpl ((Delegate) handlerDelegate, (Delegate) @delegate, true, signature);
			}
			var rv = _Block_copy (ref block);
			block.CleanupBlock ();
			return rv;
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern IntPtr _Block_copy (ref BlockLiteral block);
#endif
	}
		
	[Flags]
#if XAMCORE_3_0
	internal
#else
	public
#endif
	enum BlockFlags : int {
		BLOCK_REFCOUNT_MASK =     (0xffff),
		BLOCK_NEEDS_FREE =        (1 << 24),
		BLOCK_HAS_COPY_DISPOSE =  (1 << 25),
		BLOCK_HAS_CTOR =          (1 << 26), /* Helpers have C++ code. */
		BLOCK_IS_GC =             (1 << 27),
		BLOCK_IS_GLOBAL =         (1 << 28),
		BLOCK_HAS_DESCRIPTOR =    (1 << 29), // This meaning was deprecated 
		BLOCK_HAS_STRET =         (1 << 29),
		BLOCK_HAS_SIGNATURE =     (1 << 30),
	}
}
