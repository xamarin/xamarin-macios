// 
// AVAudioSessionPortDescription.cs
//
// Authors: Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2015 Xamarin Inc.
//

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using AudioToolbox;

#if !MONOMAC
namespace AVFoundation {
	public partial class AVAudioSessionPortDescription {
#if !XAMCORE_3_0
		[Obsolete ("Use 'DataSourceDescriptions' instead.")]
		public virtual AVAudioSessionChannelDescription [] DataSources {
			get {
				throw new InvalidOperationException ("Call DataSourceDescriptions instead.");
			}
		}
#endif	       
	}
}
#endif

