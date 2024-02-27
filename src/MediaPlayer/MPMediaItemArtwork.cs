// 
// MPMediaItemArtwork.cs: 
//
// Authors:
//   Rolf Bjarne Kvinge
//     
// Copyright 2015 Xamarin, Inc
//

#if !TVOS && !MONOMAC

using System;
using System.Collections;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

#nullable enable

namespace MediaPlayer {
	public partial class MPMediaItemArtwork {
#if !XAMCORE_3_0 && !NET
		[Obsolete ("Use the (UIImage) constructor instead, iOS9 does not allow creating an empty instance.")]
		public MPMediaItemArtwork ()
		{

		}
#endif
	}
}

#endif
