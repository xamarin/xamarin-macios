// 
// StoreProductParameters.cs: Implements strongly typed access for SKStoreProductParameters
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, Xamarin Inc.
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

#if !MONOMAC

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace StoreKit {

	[iOS (6,0)]
	public partial class StoreProductParameters : DictionaryContainer
	{
#if !COREBUILD
		public StoreProductParameters (int iTunesItemIdentifier)
			: this ()
		{
			ITunesItemIdentifier = iTunesItemIdentifier;
		}

		// TODO: What is real iTunes Store item identifier length
		public int? ITunesItemIdentifier {
			set {
				SetNumberValue (SKStoreProductParameterKey.ITunesItemIdentifier, value);
			}
			get {
				return GetInt32Value (SKStoreProductParameterKey.ITunesItemIdentifier);
			}
		}

		public string AffiliateToken {
			get {
				return GetStringValue (SKStoreProductParameterKey.AffiliateToken);
			}
			set {
				SetStringValue (SKStoreProductParameterKey.AffiliateToken, value);
			}
		}

		public string CampaignToken {
			get {
				return GetStringValue (SKStoreProductParameterKey.CampaignToken);
			}
			set {
				SetStringValue (SKStoreProductParameterKey.CampaignToken, value);
			}
		}
#endif
	}
}

#endif // !MONOMAC
