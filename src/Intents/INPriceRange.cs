//
// INPriceRange extensions and syntax sugar
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using XamCore.Foundation;

namespace XamCore.Intents {
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
#endif // XAMCORE_2_0
