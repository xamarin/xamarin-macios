// 
// AccountStoreOptions.cs: Implements strongly typed access for Facebook
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014, Xamarin Inc.
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

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using System.Runtime.Versioning;

#nullable enable

namespace Accounts {

	// XI specific, not part of ObjC (NSString mapping)
	public enum ACFacebookAudience {
		Everyone = 1,
		Friends,
		OnlyMe
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class AccountStoreOptions : DictionaryContainer {
#if !COREBUILD
		public AccountStoreOptions ()
			: base (new NSMutableDictionary ())
		{
		}

		public AccountStoreOptions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public string? FacebookAppId {
			set {
				SetStringValue (ACFacebookKey.AppId, value);
			}
			get {
				return GetStringValue (ACFacebookKey.AppId);
			}
		}

		public void SetPermissions (ACFacebookAudience audience, params string [] permissions)
		{
			if (permissions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (permissions));
			if (permissions.Length == 0)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (nameof (permissions));

			SetArrayValue (ACFacebookKey.Permissions, permissions);

			NSString v;

			switch (audience) {
			case ACFacebookAudience.Everyone:
				v = ACFacebookAudienceValue.Everyone;
				break;
			case ACFacebookAudience.Friends:
				v = ACFacebookAudienceValue.Friends;
				break;
			case ACFacebookAudience.OnlyMe:
				v = ACFacebookAudienceValue.Friends;
				break;
			default:
				throw new ArgumentOutOfRangeException ("audience");
			}

			SetNativeValue (ACFacebookKey.Audience, v);
		}

		public string? TencentWeiboAppId {
			set {
				SetStringValue (ACTencentWeiboKey.AppId, value);
			}
			get {
				return GetStringValue (ACTencentWeiboKey.AppId);
			}
		}
#endif
	}
}
