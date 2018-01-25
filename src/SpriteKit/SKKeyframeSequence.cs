//
// SKKeyframeSequence helpers
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013,2015 Xamarin Inc.
//

using System;
using System.Collections.Generic;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif
using Foundation;
using ObjCRuntime;

namespace SpriteKit {
#if XAMCORE_2_0 || !MONOMAC
	public partial class SKKeyframeSequence {

		[DesignatedInitializer]
		public SKKeyframeSequence (NSObject [] values, NSNumber [] times) :
			this (values, NSArray.FromNSObjects (times))
		{
		}

		[DesignatedInitializer]
		public SKKeyframeSequence (NSObject [] values, float [] times) :
			this (values, Convert (times))
		{
		}

		[DesignatedInitializer]
		public SKKeyframeSequence (NSObject [] values, double [] times) :
			this (values, Convert (times))
		{
		}

		static NSNumber[] Convert (float [] values)
		{
			if (values == null)
				return null;
			NSNumber[] array = new NSNumber [values.Length];
			for (int i = 0 ; i < array.Length; i++)
				array [i] = NSNumber.FromFloat (values [i]);
			return array;
		}

		static NSNumber[] Convert (double [] values)
		{
			if (values == null)
				return null;
			NSNumber[] array = new NSNumber [values.Length];
			for (int i = 0 ; i < array.Length; i++)
				array [i] = NSNumber.FromDouble (values [i]);
			return array;
		}
	}
#endif
}