using System;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif
using System.Runtime.InteropServices;

#if !COREBUILD
using OpenTK;
#endif

#if XAMCORE_2_0
using ObjCRuntime;
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
#if MONOMAC
using MonoMac.ObjCRuntime;
#else
using MonoTouch.ObjCRuntime;
#endif
#endif

namespace XamCore.AVFoundation {

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

		public static bool operator ==(AVAudio3DVectorOrientation left, AVAudio3DVectorOrientation right)
		{
			return left.Equals (right);
		}
		public static bool operator !=(AVAudio3DVectorOrientation left, AVAudio3DVectorOrientation right)
		{
			return !left.Equals (right);
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is AVAudio3DVectorOrientation))
				return false;
			
			return this.Equals((AVAudio3DVectorOrientation)obj);
		}

		public bool Equals(AVAudio3DVectorOrientation other)
		{
			return Forward == other.Forward && Up == other.Up;
		}
		
		public override int GetHashCode ()
		{
			return Forward.GetHashCode () ^ Up.GetHashCode ();
		}
#endif
	}
	
	public struct AVAudio3DAngularOrientation {

		public float Yaw, Pitch, Roll;

		public override string ToString ()
		{
			return String.Format ("(Yaw={0},Pitch={1},Roll={2})", Yaw, Pitch, Roll);
		}

		public static bool operator ==(AVAudio3DAngularOrientation left, AVAudio3DAngularOrientation right)
		{
			return (left.Yaw == right.Yaw &&
				left.Pitch == right.Pitch &&
				left.Roll == right.Roll);
		}
		public static bool operator !=(AVAudio3DAngularOrientation left, AVAudio3DAngularOrientation right)
		{
			return (left.Yaw != right.Yaw ||
				left.Pitch != right.Pitch ||
				left.Roll != right.Roll);
			
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is AVAudio3DAngularOrientation))
				return false;
			
			return this.Equals((AVAudio3DAngularOrientation)obj);
		}

		public bool Equals(AVAudio3DAngularOrientation other)
		{
			return this == other;
		}
		
		public override int GetHashCode ()
		{
			return Yaw.GetHashCode () ^ Pitch.GetHashCode () ^ Roll.GetHashCode ();
		}
	}

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
	}

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
	}

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
	}

#if !COREBUILD
	public static partial class AVMetadataIdentifiers {
	}
#endif

	public static class AVUtilities {

		[DllImport (Constants.AVFoundationLibrary)]
		static extern /* CGRect */ RectangleF AVMakeRectWithAspectRatioInsideRect (/* CGSize */ SizeF aspectRatio, /* CGRect */ RectangleF boundingRect);

		[Mac (10,7)][iOS (4,0)]
		public static RectangleF WithAspectRatio (this RectangleF self, SizeF aspectRatio)
		{
			return AVMakeRectWithAspectRatioInsideRect (aspectRatio, self);
		}
	}
}
