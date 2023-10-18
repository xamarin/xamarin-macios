#nullable enable

#if IOS && !__MACCATALYST__

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using PassKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PassKit {

	public partial class PKPayLaterView {

#if !NET
		delegate void PKPayLaterValidateAmountCompletionHandler (IntPtr block, byte eligible);
		static PKPayLaterValidateAmountCompletionHandler static_ValidateAmount = TrampolineValidateAmount;

		[MonoPInvokeCallback (typeof (PKPayLaterValidateAmountCompletionHandler))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineValidateAmount (IntPtr block, byte eligible)
		{
			var del = BlockLiteral.GetTarget<Action<bool>> (block);
			if (del is not null) {
				del (eligible != 0);
			}
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void ValidateAmount (NSDecimalNumber amount, string currencyCode, Action<bool> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, byte, void> trampoline = &TrampolineValidateAmount;
				using var block = new BlockLiteral (trampoline, callback, typeof (PKPayLaterView), nameof (TrampolineValidateAmount));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ValidateAmount, callback);
#endif
				var nsCurrencyCodePtr = NSString.CreateNative (currencyCode);
				try {
					PKPayLaterValidateAmount (amount.Handle, nsCurrencyCodePtr, &block);
				} finally {
					NSString.ReleaseNative (nsCurrencyCodePtr);
				}
			}
		}

		[DllImport (Constants.PassKitLibrary)]
		unsafe static extern void PKPayLaterValidateAmount (IntPtr /* NSDecimalNumber */ amount, IntPtr /* NSString */  currencyCode, BlockLiteral* callback);
	}
}

#endif
