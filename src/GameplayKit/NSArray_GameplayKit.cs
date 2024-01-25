//
// NSArray_GameplayKit.cs: Generic extensions to NSArray
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;

using Foundation;

using ObjCRuntime;

namespace GameplayKit {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public static class NSArray_GameplayKit {

		[Export ("shuffledArrayWithRandomSource:")]
		public static T [] GetShuffledArray<T> (this NSArray This, GKRandomSource randomSource) where T : class, INativeObject
		{
			if (randomSource is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (randomSource));
			return NSArray.ArrayFromHandle<T> (Messaging.IntPtr_objc_msgSend_IntPtr (This.Handle, Selector.GetHandle ("shuffledArrayWithRandomSource:"), randomSource.Handle));
		}

		[Export ("shuffledArray")]
		public static T [] GetShuffledArray<T> (this NSArray This) where T : class, INativeObject
		{
			return NSArray.ArrayFromHandle<T> (Messaging.IntPtr_objc_msgSend (This.Handle, Selector.GetHandle ("shuffledArray")));
		}
	}
}
