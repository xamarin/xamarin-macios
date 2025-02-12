//
// INPriceRange extensions and syntax sugar
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if IOS
using System;
using Foundation;

#nullable enable

namespace Intents {
	public enum INPriceRangeOption {
		Maximum,
		Minimum,
	}

	public partial class INPriceRange {

		public INPriceRange (INPriceRangeOption option, NSDecimalNumber price, string currencyCode)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case INPriceRangeOption.Maximum:
				InitializeHandle (InitWithMaximumPrice (price, currencyCode));
				break;
			case INPriceRangeOption.Minimum:
				InitializeHandle (InitWithMinimumPrice (price, currencyCode));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
#endif // IOS
