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
		public override bool Equals (object  obj)
		{
			if (this == null){
				return (obj == null);
			}
			if (!(obj is NSObject))
				return false;
			return IsEqual ((NSObject)obj);
		}

		public static bool operator == (AVAudioFormat a, AVAudioFormat b)
		{
			return a.Equals (b);
		}
		
		public static bool operator != (AVAudioFormat a, AVAudioFormat b)
		{
			return !a.Equals (b);
		}

		public override int GetHashCode ()
		{
			return (int) ChannelCount;
		}
		
	}
}
