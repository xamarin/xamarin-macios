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
			if ((object) a == (object) b)
				return true;
			if ((object) a == null ^ (object) b == null)
				return false;
			return a.Equals (b);
		}
		
		public static bool operator != (AVAudioFormat a, AVAudioFormat b)
		{
			if ((object) a != (object) b)
				return true;
			if ((object) a == null ^ (object) b == null)
				return true;
			return !a.Equals (b);
		}
	}
}
