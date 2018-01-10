//
// Helper methods for NSDistributedNotificationCenter
//
// Author:
//   Miguel de Icaza
//
// Copyright 2011 Xamarin Inc
//
#if !MONOMAC && !XAMCORE_3_0

using System;

using ObjCRuntime;

namespace Foundation {

	[Obsolete ("This is not available in iOS.")]
	public partial class NSDistributedNotificationCenter {

		[Obsolete ("This is not available in iOS.")]
		public void AddObserver (NSObject observer, Selector aSelector, string aName, string anObject)
		{
		}

		[Obsolete ("This is not available in iOS.")]
		public void RemoveObserver (NSObject observer, string aName, string anObject)
		{
		}
	}
}

#endif
