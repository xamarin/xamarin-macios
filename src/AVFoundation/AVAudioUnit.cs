// 
// AVAudioUnit.cs: Support for AVAudioUnit
//
// Authors: Miguel de Icaza (marek.safar@gmail.com)
//     
// Copyright 2014 Xamarin Inc.
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

namespace XamCore.AVFoundation {

	public partial class AVAudioUnit {
		public AudioComponentDescription AudioComponentDescription {
			get {
#if !XAMCORE_2_0
				return new AudioComponentDescription (_AudioComponentDescription);
#endif
			}
		}

			       
	}
}

