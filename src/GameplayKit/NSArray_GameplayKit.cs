//
// NSArray_GameplayKit.cs: Generic extensions to NSArray
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 || !MONOMAC
using System;
using Foundation;
using ObjCRuntime;

namespace GameplayKit {
	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	public static class NSArray_GameplayKit {

		[Export ("shuffledArrayWithRandomSource:")]
		public static T [] GetShuffledArray<T> (this NSArray This, GKRandomSource randomSource) where T : class, INativeObject
		{
			if (randomSource == null)
				throw new ArgumentNullException (nameof (randomSource));
			return NSArray.ArrayFromHandle<T> (Messaging.IntPtr_objc_msgSend_IntPtr (This.Handle, Selector.GetHandle ("shuffledArrayWithRandomSource:"), randomSource.Handle));
		}

		[Export ("shuffledArray")]
		public static T [] GetShuffledArray<T> (this NSArray This) where T : class, INativeObject
		{
			return NSArray.ArrayFromHandle<T> (Messaging.IntPtr_objc_msgSend (This.Handle, Selector.GetHandle ("shuffledArray")));
		}
	}
}
#endif
