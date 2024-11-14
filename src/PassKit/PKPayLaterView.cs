#nullable enable

#if IOS && !__MACCATALYST__

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Foundation;
using ObjCRuntime;
using PassKit;

namespace PassKit {

	/// <summary>The delegate that is called when <see cref="PKPayLaterView.ValidateAmount(System.Decimal,System.String,PassKit.PKPayLaterViewValidateAmountCallback)" /> has determined whether the Pay Later Merchandising information is valid.</summary>
	/// <param name="eligible">True if the Pay Later Merchandising information is valid, false otherwise.</param>
	public delegate void PKPayLaterViewValidateAmountCallback (bool eligible);

	public partial class PKPayLaterView {
		[UnmanagedCallersOnly]
		static void TrampolineValidateAmount (IntPtr block, byte eligible)
		{
			var del = BlockLiteral.GetTarget<PKPayLaterViewValidateAmountCallback> (block);
			if (del is not null) {
				del (eligible != 0);
			}
		}

		/// <summary>Checks whether the Pay Later Merchandising information is valid for the specified amount and currency.</summary>
		/// <param name="amount">The amount to check for.</param>
		/// <param name="currencyCode">The ISO 4217 currency code to use.</param>
		/// <param name="callback">The delegate that will be called with the result.</param>
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void ValidateAmount (NSDecimalNumber amount, string currencyCode, PKPayLaterViewValidateAmountCallback callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
				delegate* unmanaged<IntPtr, byte, void> trampoline = &TrampolineValidateAmount;
				using var block = new BlockLiteral (trampoline, callback, typeof (PKPayLaterView), nameof (TrampolineValidateAmount));
				var nsCurrencyCodePtr = NSString.CreateNative (currencyCode);
				try {
					PKPayLaterValidateAmount (amount.Handle, nsCurrencyCodePtr, &block);
				} finally {
					NSString.ReleaseNative (nsCurrencyCodePtr);
				}
			}
		}

		/// <summary>Checks whether the Pay Later Merchandising information is valid for the specified amount and currency.</summary>
		/// <param name="amount">The amount to check for.</param>
		/// <param name="currencyCode">The ISO 4217 currency code to use.</param>
		/// <param name="callback">The delegate that will be called with the result.</param>
		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void ValidateAmount (decimal amount, string currencyCode, PKPayLaterViewValidateAmountCallback callback)
		{
			using var decimalAmount = new NSDecimalNumber ((NSDecimal) amount);
			ValidateAmount (decimalAmount, currencyCode, callback);
		}

		[SupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[DllImport (Constants.PassKitLibrary)]
		unsafe static extern void PKPayLaterValidateAmount (IntPtr /* NSDecimalNumber */ amount, IntPtr /* NSString */  currencyCode, BlockLiteral* callback);
	}
}

#endif
