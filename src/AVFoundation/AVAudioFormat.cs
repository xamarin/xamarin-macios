//
// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
using Foundation;
using ObjCRuntime;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AVFoundation {
	public partial class AVAudioFormat {
		public static bool operator == (AVAudioFormat a, AVAudioFormat b)
		{
			return a.Equals (b);
		}
		
		public static bool operator != (AVAudioFormat a, AVAudioFormat b)
		{
			return !a.Equals (b);
		}
	}
}
