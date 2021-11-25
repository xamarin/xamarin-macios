//
// NEHotspotEapSettings.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#nullable enable

#if !MONOMAC
using System;
using Foundation;

namespace NetworkExtension {
	public partial class NEHotspotEapSettings {

		public NEHotspotConfigurationEapType [] SupportedEapTypes {
			get {
				return NSArray.EnumsFromHandle<NEHotspotConfigurationEapType> (_SupportedEapTypes)!;
			}
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));

				var ret = NSArray.From (value, value.Length);
				_SupportedEapTypes = ret.Handle;
			}
		}
	}
}
#endif
