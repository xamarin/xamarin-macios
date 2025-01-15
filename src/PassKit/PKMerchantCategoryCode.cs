using System;
using System.Runtime.Versioning;

using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if !TVOS

namespace PassKit {
	/*
	 * PKMerchantCategoryCode is defined like this:
	 *
	 *   typedef SInt16 PKMerchantCategoryCode NS_TYPED_EXTENSIBLE_ENUM NS_REFINED_FOR_SWIFT API_AVAILABLE(macos(15.0), ios(18.0), watchos(11.0));
	 *   extern PKMerchantCategoryCode const PKMerchantCategoryCodeNone API_AVAILABLE(macos(15.0), ios(18.0), watchos(11.0));
	 *
	 * In other words: like a strongly typed enum, just with 'short' as the backing type instead of 'NSString'.
	 *
	 * Since we can't model this as an enum in C# (because the values aren't constant), instead create a custom struct with a short field.
	 */

	/// <summary>The four-digit type, in ISO 18245 format, that represents the type of goods or service a merchant provides for a transaction.</summary>
#if NET
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (15, 0), iOS (18, 0), NoTV, MacCatalyst (18, 0)]
#endif
	public struct PKMerchantCategoryCode {
		short value;

#if !COREBUILD
		/// <summary>A <see cref="PKMerchantCategoryCode" /> representing no merchant code.</summary>
		public static PKMerchantCategoryCode None { get => new PKMerchantCategoryCode (PKMerchantCategoryCodeValues.None); }
#endif

		/// <summary>Create a <see cref="PKMerchantCategoryCode" /> for the specified merchant code.</summary>
		/// <param name="code">The 16-bit merchant code.</param>
		public PKMerchantCategoryCode (short code)
		{
			value = code;
		}

		/// <summary>Get the 16-bit value for this <see cref="PKMerchantCategoryCode" />.</summary>
		public short Value {
			get {
				return this.Value;
			}
		}

		/// <summary>Get the 16-bit value for a <see cref="PKMerchantCategoryCode" />.</summary>
		public static explicit operator short (PKMerchantCategoryCode code)
		{
			return code.Value;
		}

		/// <summary>Convert a 16-bit value to a <see cref="PKMerchantCategoryCode" />.</summary>
		public static explicit operator PKMerchantCategoryCode (short code)
		{
			return new PKMerchantCategoryCode (code);
		}
	}
}
#endif // !TVOS
