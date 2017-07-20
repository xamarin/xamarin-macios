//
// NSFileProviderItemIdentifierExtensions.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using XamCore.Foundation;

namespace XamCore.FileProvider {
	public partial class NSFileProviderItemIdentifierExtensions {

		public static NSString[] GetConstants (this NSFileProviderItemIdentifier[] self)
		{
			if (self == null)
				throw new ArgumentNullException (nameof (self));

			var array = new NSString [self.Length];
			for (int n = 0; n < self.Length; n++)
				array [n] = self [n].GetConstant ();
			return array;
		}
	}
}
#endif
