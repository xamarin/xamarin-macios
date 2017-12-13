//
// MPSkipIntervalCommand
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.MediaPlayer {
#if XAMCORE_2_0
	public partial class MPSkipIntervalCommand {
		public double[] PreferredIntervals {
			get {
				NSArray a = _PreferredIntervals;
				if (a == null)
					return null;

                return NSArray.ArrayFromHandle<double> (a.Handle, input => {
					return new NSNumber (input).DoubleValue;
				});
			}
			set {
				if (value == null)
					_PreferredIntervals = null;
				else {
					NSObject [] nsoa = new NSObject [value.Length];
					for (int i = 0; i < value.Length; i++)
						nsoa [i] = new NSNumber (value [i]);
					_PreferredIntervals = NSArray.FromNSObjects (nsoa);
				}
			}
		}
	}
#endif
}