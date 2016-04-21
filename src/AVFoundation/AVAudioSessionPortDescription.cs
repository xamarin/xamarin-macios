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
using XamCore.AudioToolbox;

#if !MONOMAC
namespace XamCore.AVFoundation {
	public partial class AVAudioSessionPortDescription {
#if !XAMCORE_3_0
		[Obsolete ("Use DataSourceDescriptions instead")]
		public virtual AVAudioSessionChannelDescription [] DataSources {
			get {
				throw new InvalidOperationException ("Call DataSourceDescriptions instead.");
			}
		}
#endif	       
	}
}
#endif

