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

