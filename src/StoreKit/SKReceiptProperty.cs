//
// SKReceiptProperty.cs: strongly typed dictionary for options in StoreKit
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2013 Xamarin Inc.
//
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.StoreKit;

#if !MONOMAC
using XamCore.UIKit;
#endif
using System;

namespace XamCore.StoreKit {
	public partial class SKReceiptProperties : DictionaryContainer {
#if !COREBUILD
		public SKReceiptProperties ()
			: base (new NSMutableDictionary ())
		{
		}

		public SKReceiptProperties (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public bool IsExpired {
			get {
				return GetInt32Value (_SKReceiptProperty.IsExpired) != 0;
			}
			set {
				SetNumberValue (_SKReceiptProperty.IsExpired, value ? 1 : 0);
			}
		}

		public bool IsRevoked {
			get {
				return GetInt32Value (_SKReceiptProperty.IsRevoked) != 0;
			}
			set {
				SetNumberValue (_SKReceiptProperty.IsRevoked, value ? 1 : 0);
			}
		}

		public bool IsVolumePurchase {
			get {
				return GetInt32Value (_SKReceiptProperty.IsVolumePurchase) != 0;
			}
			set {
				SetNumberValue (_SKReceiptProperty.IsVolumePurchase, value ? 1 : 0);
			}
		}
#endif
		
	}
}