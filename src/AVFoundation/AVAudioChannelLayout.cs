//
// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
using Foundation;
using ObjCRuntime;
using AudioToolbox;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AVFoundation {
	public partial class AVAudioChannelLayout {
		[ThreadStatic] 
		static IntPtr handleToLayout;

		static IntPtr CreateLayoutPtr (AudioChannelLayout layout)
		{
			int size;
			handleToLayout = layout.ToBlock (out size);
			return handleToLayout;
		}

		[DesignatedInitializer]
		public AVAudioChannelLayout (AudioChannelLayout layout) : this ((nint) CreateLayoutPtr (layout))
		{
			Marshal.FreeHGlobal (handleToLayout);
		}

		public AudioChannelLayout Layout {
			get {
				return AudioChannelLayout.FromHandle (_Layout);
			}
		}
		
		public static bool operator == (AVAudioChannelLayout a, AVAudioChannelLayout b)
		{
			return a.Equals (b);
		}
		
		public static bool operator != (AVAudioChannelLayout a, AVAudioChannelLayout b)
		{
			return !a.Equals (b);
		}
	}
}
