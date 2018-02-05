//
// SCNAnimatable.cs: helper methods
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !WATCH

using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreAnimation;

namespace SceneKit
{
	public partial class SCNAnimatable {
		
		public void AddAnimation (CAAnimation animation, string key = null)
		{
			NSString nskey = key == null ? null : new NSString (key);
			
			AddAnimation (animation, nskey);
			if (nskey != null)
				nskey.Dispose ();
		}
	}
}

#endif
