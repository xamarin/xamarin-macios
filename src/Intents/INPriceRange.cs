//
// INPriceRange extensions and syntax sugar
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && IOS
using System;
using Foundation;

namespace Intents {
	public enum INPriceRangeOption {
		Maximum,
		Minimum
	}

	public partial class INPriceRange {

		public INPriceRange (INPriceRangeOption option, NSDecimalNumber price, string currencyCode)
		{
			if (option == INPriceRangeOption.Maximum)
				Handle = InitWithMaximumPrice (price, currencyCode);
			else
				Handle = InitWithMinimumPrice (price, currencyCode);
		}
	}
}
#endif // XAMCORE_2_0 && IOS
