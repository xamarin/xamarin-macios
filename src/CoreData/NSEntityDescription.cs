//
// NSEntityDescription.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreData
{
	public partial class NSEntityDescription
	{
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public NSObject[][] UniquenessConstraints {
			get { return NSArray.FromArrayOfArray (_UniquenessConstraints); }
			set { _UniquenessConstraints = NSArray.From (value); }
		}
	}
}
