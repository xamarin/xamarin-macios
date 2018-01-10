// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
using Foundation;
using ObjCRuntime;
using System;
#if XAMCORE_2_0 && !MONOMAC

namespace AVFoundation {
	public enum AVAudioDataSourceLocation {
		Unknown, Upper, Lower
	}
	
	public enum AVAudioDataSourceOrientation {
		Unknown, Top, Bottom, Front, Back, Left, Right
	}

	public enum AVAudioDataSourcePolarPattern {
		Unknown, Omnidirectional, Cardioid, Subcardioid
	}
	
	public partial class AVAudioSessionDataSourceDescription {
		static internal AVAudioDataSourceLocation ToLocation (NSString l)
		{
			if (l == AVAudioSession.LocationLower_)
				return AVAudioDataSourceLocation.Lower;
			else if (l == AVAudioSession.LocationUpper_)
				return AVAudioDataSourceLocation.Upper;
			else
				return AVAudioDataSourceLocation.Unknown;
		}

		static internal AVAudioDataSourceOrientation ToOrientation (NSString o)
		{
			if (o == AVAudioSession.OrientationTop_)
				return AVAudioDataSourceOrientation.Top;
			if (o == AVAudioSession.OrientationBottom_)
				return AVAudioDataSourceOrientation.Bottom;
			if (o == AVAudioSession.OrientationFront_)
				return AVAudioDataSourceOrientation.Front;
			if (o == AVAudioSession.OrientationBack_)
				return AVAudioDataSourceOrientation.Back;
			return AVAudioDataSourceOrientation.Unknown;
		}
		
		static internal AVAudioDataSourcePolarPattern ToPolarPattern (NSString p)
		{
			if (p == AVAudioSession.PolarPatternOmnidirectional_)
				return AVAudioDataSourcePolarPattern.Omnidirectional;
			if (p == AVAudioSession.PolarPatternCardioid_)
				return AVAudioDataSourcePolarPattern.Cardioid;
			if (p == AVAudioSession.PolarPatternSubcardioid_)
				return AVAudioDataSourcePolarPattern.Subcardioid;
			return AVAudioDataSourcePolarPattern.Unknown;
		}
		
		static internal NSString ToToken (AVAudioDataSourcePolarPattern p)
		{
			switch (p){
			case AVAudioDataSourcePolarPattern.Omnidirectional:
				return AVAudioSession.PolarPatternOmnidirectional_;
			case AVAudioDataSourcePolarPattern.Cardioid:
				return AVAudioSession.PolarPatternCardioid_;
			case AVAudioDataSourcePolarPattern.Subcardioid:
				return AVAudioSession.PolarPatternSubcardioid_;
			default:
				return null;
			}
		}
		
		public AVAudioDataSourceLocation Location {
			get {
				return ToLocation (Location_);
			}
		}

		public AVAudioDataSourceOrientation Orientation {
			get {
				return ToOrientation (Orientation_);
			}
		}

#if !WATCH
		public AVAudioDataSourcePolarPattern []SupportedPolarPatterns {
			get {
				var x = SupportedPolarPatterns_;
				int n = x.Length;
				var r = new AVAudioDataSourcePolarPattern [n];
				for (int i = 0; i < n; i++)
					r [i] = ToPolarPattern (x [i]);
				return r;
			}
		}

		public AVAudioDataSourcePolarPattern SelectedPolarPattern {
			get {
				return ToPolarPattern (SelectedPolarPattern_);
			}
		}

		public AVAudioDataSourcePolarPattern PreferredPolarPattern {
			get {
				return ToPolarPattern (PreferredPolarPattern_);
			}
		}
		
		public bool SetPreferredPolarPattern (AVAudioDataSourcePolarPattern pattern, out NSError outError)
		{
			return SetPreferredPolarPattern_ (ToToken (pattern), out outError);
		}
#endif
	}
}
#endif
