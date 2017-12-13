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
using XamCore.Foundation; 
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.CoreGraphics;

namespace XamCore.MediaPlayer {
	public partial class MPMediaItemArtwork {
#if !XAMCORE_3_0
		[Obsolete ("Use the (UIImage) constructor instead, iOS9 does not allow creating an empty instance.")]
		public MPMediaItemArtwork ()
		{

		}
#endif
	}
}

#endif
