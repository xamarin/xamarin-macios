//
// NSEntityDescription.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.CoreData
{
	public partial class NSEntityDescription
	{
		[iOS (9,0), Mac (10,11)]
		public NSObject[][] UniquenessConstraints {
			get { return NSArray.FromArrayOfArray (_UniquenessConstraints); }
			set { _UniquenessConstraints = NSArray.From (value); }
		}
	}
}
