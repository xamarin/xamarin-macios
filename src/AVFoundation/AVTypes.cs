using System;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif
using System.Runtime.InteropServices;

#if !COREBUILD
#if NET
using Vector3 = global::System.Numerics.Vector3;
#else
using Vector3 = global::OpenTK.Vector3;
#endif // NET
#endif // !COREBUILD
using CoreGraphics;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVAudio3DVectorOrientation {
#if !COREBUILD
		public Vector3 Forward, Up;

		public AVAudio3DVectorOrientation (Vector3 forward, Vector3 up)
		{
			Forward = forward;
			Up = up;
		}

		public override string ToString ()
		{
			return String.Format ("({0}:{1})", Forward, Up);
		}

		public static bool operator == (AVAudio3DVectorOrientation left, AVAudio3DVectorOrientation right)
		{
			return left.Equals (right);
		}
		public static bool operator != (AVAudio3DVectorOrientation left, AVAudio3DVectorOrientation right)
		{
			return !left.Equals (right);
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is AVAudio3DVectorOrientation))
				return false;

			return this.Equals ((AVAudio3DVectorOrientation) obj);
		}

		public bool Equals (AVAudio3DVectorOrientation other)
		{
			return Forward == other.Forward && Up == other.Up;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (Forward, Up);
		}
#endif
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVAudio3DAngularOrientation {

		public float Yaw, Pitch, Roll;

		public override string ToString ()
		{
			return String.Format ("(Yaw={0},Pitch={1},Roll={2})", Yaw, Pitch, Roll);
		}

		public static bool operator == (AVAudio3DAngularOrientation left, AVAudio3DAngularOrientation right)
		{
			return (left.Yaw == right.Yaw &&
				left.Pitch == right.Pitch &&
				left.Roll == right.Roll);
		}
		public static bool operator != (AVAudio3DAngularOrientation left, AVAudio3DAngularOrientation right)
		{
			return (left.Yaw != right.Yaw ||
				left.Pitch != right.Pitch ||
				left.Roll != right.Roll);

		}

		public override bool Equals (object? obj)
		{
			if (!(obj is AVAudio3DAngularOrientation))
				return false;

			return this.Equals ((AVAudio3DAngularOrientation) obj);
		}

		public bool Equals (AVAudio3DAngularOrientation other)
		{
			return this == other;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (Yaw, Pitch, Roll);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVCaptureWhiteBalanceGains {
		public float RedGain, GreenGain, BlueGain;

		public AVCaptureWhiteBalanceGains (float redGain, float greenGain, float blueGain)
		{
			RedGain = redGain;
			GreenGain = greenGain;
			BlueGain = blueGain;
		}

		public override string ToString ()
		{
			return String.Format ("(RedGain={0},GreenGain={1},BlueGain={2})", RedGain, GreenGain, BlueGain);
		}

		public static bool operator == (AVCaptureWhiteBalanceGains left, AVCaptureWhiteBalanceGains right)
		{
			return (left.RedGain == right.RedGain &&
				left.GreenGain == right.GreenGain &&
				left.BlueGain == right.BlueGain);
		}

		public static bool operator != (AVCaptureWhiteBalanceGains left, AVCaptureWhiteBalanceGains right)
		{
			return (left.RedGain != right.RedGain ||
				left.GreenGain != right.GreenGain ||
				left.BlueGain != right.BlueGain);
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is AVCaptureWhiteBalanceGains))
				return false;

			return this.Equals ((AVCaptureWhiteBalanceGains) obj);
		}

		public bool Equals (AVCaptureWhiteBalanceGains other)
		{
			return this == other;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (RedGain, GreenGain, BlueGain);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVCaptureWhiteBalanceChromaticityValues {
		public float X, Y;

		public AVCaptureWhiteBalanceChromaticityValues (float x, float y)
		{
			X = x;
			Y = y;
		}

		public override string ToString ()
		{
			return String.Format ("({0},{1})", X, Y);
		}

		public static bool operator == (AVCaptureWhiteBalanceChromaticityValues left, AVCaptureWhiteBalanceChromaticityValues right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator != (AVCaptureWhiteBalanceChromaticityValues left, AVCaptureWhiteBalanceChromaticityValues right)
		{
			return left.X != right.X || left.Y != right.Y;
		}

		public override bool Equals (object? obj)
		{
			if (!(obj is AVCaptureWhiteBalanceChromaticityValues))
				return false;

			return this.Equals ((AVCaptureWhiteBalanceChromaticityValues) obj);
		}

		public bool Equals (AVCaptureWhiteBalanceChromaticityValues other)
		{
			return this == other;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (X, Y);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVCaptureWhiteBalanceTemperatureAndTintValues {
		public float Temperature, Tint;

		public AVCaptureWhiteBalanceTemperatureAndTintValues (float temperature, float tint)
		{
			Temperature = temperature;
			Tint = tint;
		}
		public override string ToString ()
		{
			return String.Format ("(Temperature={0},Tint={1})", Temperature, Tint);
		}

		public static bool operator == (AVCaptureWhiteBalanceTemperatureAndTintValues left, AVCaptureWhiteBalanceTemperatureAndTintValues right)
		{
			return left.Temperature == right.Temperature && left.Tint == right.Tint;
		}

		public static bool operator != (AVCaptureWhiteBalanceTemperatureAndTintValues left, AVCaptureWhiteBalanceTemperatureAndTintValues right)
		{
			return left.Temperature != right.Temperature || left.Tint != right.Tint;

		}

		public override bool Equals (object? obj)
		{
			if (!(obj is AVCaptureWhiteBalanceTemperatureAndTintValues))
				return false;

			return this.Equals ((AVCaptureWhiteBalanceTemperatureAndTintValues) obj);
		}

		public bool Equals (AVCaptureWhiteBalanceTemperatureAndTintValues other)
		{
			return this == other;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (Temperature, Tint);
		}
	}

#if !COREBUILD
	public static partial class AVMetadataIdentifiers {
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
#if !WATCH
	public static class AVUtilities {

		[DllImport (Constants.AVFoundationLibrary)]
		static extern /* CGRect */ CGRect AVMakeRectWithAspectRatioInsideRect (/* CGSize */ CGSize aspectRatio, /* CGRect */ CGRect boundingRect);

		public static CGRect WithAspectRatio (this CGRect self, CGSize aspectRatio)
		{
			return AVMakeRectWithAspectRatioInsideRect (aspectRatio, self);
		}
	}
#endif

#if MONOMAC || !NET

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS]
	[NoWatch]
	[NoTV]
#endif
#if !MONOMAC
	[Obsolete ("This API is not available on this platform.")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVSampleCursorSyncInfo {
		[MarshalAs (UnmanagedType.I1)]
		public bool IsFullSync;

		[MarshalAs (UnmanagedType.I1)]
		public bool IsPartialSync;

		[MarshalAs (UnmanagedType.I1)]
		public bool IsDroppable;
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS]
	[NoWatch]
	[NoTV]
#endif
#if !MONOMAC
	[Obsolete ("This API is not available on this platform.")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVSampleCursorDependencyInfo {
		[MarshalAs (UnmanagedType.I1)]
		public bool IndicatesWhetherItHasDependentSamples;

		[MarshalAs (UnmanagedType.I1)]
		public bool HasDependentSamples;

		[MarshalAs (UnmanagedType.I1)]
		public bool IndicatesWhetherItDependsOnOthers;

		[MarshalAs (UnmanagedType.I1)]
		public bool DependsOnOthers;

		[MarshalAs (UnmanagedType.I1)]
		public bool IndicatesWhetherItHasRedundantCoding;

		[MarshalAs (UnmanagedType.I1)]
		public bool HasRedundantCoding;
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS]
	[NoWatch]
	[NoTV]
#endif
#if !MONOMAC
	[Obsolete ("This API is not available on this platform.")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVSampleCursorStorageRange {
		public long Offset;
		public long Length;
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS]
	[NoWatch]
	[NoTV]
#endif
#if !MONOMAC
	[Obsolete ("This API is not available on this platform.")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVSampleCursorChunkInfo {
		public long SampleCount;

		[MarshalAs (UnmanagedType.I1)]
		public bool HasUniformSampleSizes;

		[MarshalAs (UnmanagedType.I1)]
		public bool HasUniformSampleDurations;

		[MarshalAs (UnmanagedType.I1)]
		public bool HasUniformFormatDescriptions;
	}
#endif

#if MONOMAC || __MACCATALYST__

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVSampleCursorAudioDependencyInfo {
		[MarshalAs (UnmanagedType.I1)]
		public bool IsIndependentlyDecodable;

		public nint PacketRefreshCount;
	}
#endif

#if MONOMAC

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
#endif
	[Native]
	public enum AVCaptionUnitsType : long {
		Unspecified = 0,
		Cells,
		Percent,
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVCaptionDimension {
		public nfloat Value;
		nuint units;

		public AVCaptionUnitsType Units {
			get => (AVCaptionUnitsType) (long) units;
			set => units = (nuint) (long) value;
		}

		[DllImport (Constants.AVFoundationLibrary)]
		static extern AVCaptionDimension AVCaptionDimensionMake (nfloat dimension, /* AVCaptionUnitsType */ nuint units);

		public static AVCaptionDimension Create (nfloat dimension, AVCaptionUnitsType units)
			=> AVCaptionDimensionMake (dimension, (nuint) (long)units);
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVCaptionPoint {
		public AVCaptionDimension X;
		public AVCaptionDimension Y;

		[DllImport (Constants.AVFoundationLibrary)]
		static extern AVCaptionPoint AVCaptionPointMake (AVCaptionDimension x, AVCaptionDimension y);

		public static AVCaptionPoint Create (AVCaptionDimension x, AVCaptionDimension y)
			=> AVCaptionPointMake (x,y);
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AVCaptionSize {
		public AVCaptionDimension Width;
		public AVCaptionDimension Height;

		[DllImport (Constants.AVFoundationLibrary)]
		static extern AVCaptionSize AVCaptionSizeMake (AVCaptionDimension width, AVCaptionDimension height);

		public static AVCaptionSize Create (AVCaptionDimension width, AVCaptionDimension height)
			=> AVCaptionSizeMake (width, height);
	}

#endif

}
