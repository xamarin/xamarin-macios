// 
// MPMediaItem.cs: 
//
// Authors:
//   Geoff Norton.
//   Miguel de Icaza
//     
// Copyright 2011 Novell, Inc
// Copyright 2011-2012, 2014-2015 Xamarin, Inc
//

#if !TVOS && !MONOMAC

using System;
using Foundation; 
using ObjCRuntime;

#if XAMCORE_2_0
using xint = System.nuint;
#else
using xint = System.Int32;
#endif

namespace MediaPlayer {

	public partial class MPMediaQuery
	{
		public MPMediaItem GetItem (xint index)
		{
			using (var array = new NSArray (Messaging.IntPtr_objc_msgSend (Handle, Selector.GetHandle ("items"))))
				return array.GetItem<MPMediaItem> (index);
		}

		public MPMediaQuerySection GetSection (xint index)
		{
			using (var array = new NSArray (Messaging.IntPtr_objc_msgSend (Handle, Selector.GetHandle ("itemSections"))))
				return array.GetItem<MPMediaQuerySection> (index);
		}

		public MPMediaItemCollection GetCollection (xint index)
		{
			using (var array = new NSArray (Messaging.IntPtr_objc_msgSend (Handle, Selector.GetHandle ("collections"))))
				return array.GetItem<MPMediaItemCollection> (index);
		}
	}
}

#endif // !TVOS
