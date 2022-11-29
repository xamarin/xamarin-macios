//
// MPSkipIntervalCommand
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace MediaPlayer {
	public partial class MPSkipIntervalCommand {
		public double []? PreferredIntervals {
			get {
				NSArray a = _PreferredIntervals;
				if ((a is null) || (a.Count == 0))
					return null;

				return NSArray.ArrayFromHandle<double> (a.Handle, input => {
					return new NSNumber (input).DoubleValue;
				});
			}
			set {
				if (value is null)
					_PreferredIntervals = new NSArray ();
				else {
					NSObject [] nsoa = new NSObject [value.Length];
					for (int i = 0; i < value.Length; i++)
						nsoa [i] = new NSNumber (value [i]);
					_PreferredIntervals = NSArray.FromNSObjects (nsoa);
				}
			}
		}
	}
}
