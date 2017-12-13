// 
// AVAudioSessionPortDescription.cs
//
// Authors: Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2015 Xamarin Inc.
//

using System;

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.AudioToolbox;

#if !MONOMAC
namespace XamCore.AVFoundation {
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

