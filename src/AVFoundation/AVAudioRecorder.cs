// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2014 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !WATCH

using Foundation;
using CoreFoundation;
using AudioToolbox;
using ObjCRuntime;
using System;

namespace AVFoundation {

#if !XAMCORE_2_0
	[Advice ("Use 'AudioSettings' instead.")]
	public class AVAudioRecorderSettings {
		public AVAudioRecorderSettings ()
		{
		}

		public AudioFormatType AudioFormat = AudioFormatType.LinearPCM;
		public float SampleRate = 44100f;
		public int NumberChannels = 1;
		public int LinearPcmBitDepth;
		public bool LinearPcmBigEndian;
		public bool LinearPcmFloat = false;
		public bool LinearPcmNonInterleaved;
		public AVAudioQuality AudioQuality = AVAudioQuality.High;
		public AVAudioQuality? SampleRateConverterAudioQuality;
		public int? EncoderBitRate;
		public int? EncoderBitRatePerChannel;
		public int? EncoderBitDepthHint;
		
		internal NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			dict.SetObject (new NSNumber ((int) AudioFormat), AVAudioSettings.AVFormatIDKey);
			dict.SetObject (new NSNumber (SampleRate), AVAudioSettings.AVSampleRateKey);
			dict.SetObject (new NSNumber (NumberChannels), AVAudioSettings.AVNumberOfChannelsKey);

			if (AudioFormat == AudioFormatType.LinearPCM){
				IntPtr thandle = CFBoolean.TrueHandle;
				IntPtr fhandle = CFBoolean.FalseHandle;
				
				if (LinearPcmBitDepth != 0){
					if (LinearPcmBitDepth == 8 || LinearPcmBitDepth == 16 || LinearPcmBitDepth == 32 || LinearPcmBitDepth == 24)
						dict.SetObject (new NSNumber (LinearPcmBitDepth), AVAudioSettings.AVLinearPCMBitDepthKey);
					else
						throw new Exception ("Invalid value for LinearPcmBitDepth, must be one of 8, 16, 24 or 32");
				}
				dict.LowlevelSetObject (LinearPcmBigEndian ? thandle : fhandle, AVAudioSettings.AVLinearPCMIsBigEndianKey.Handle);
				dict.LowlevelSetObject (LinearPcmFloat ? thandle : fhandle, AVAudioSettings.AVLinearPCMIsFloatKey.Handle);
				dict.LowlevelSetObject (LinearPcmNonInterleaved ? thandle : fhandle, AVAudioSettings.AVLinearPCMIsNonInterleaved.Handle);
			}
			dict.SetObject (new NSNumber ((int) AudioQuality), AVAudioSettings.AVEncoderAudioQualityKey);
			if (EncoderBitRate.HasValue)
				dict.SetObject (new NSNumber ((int) EncoderBitRate.Value), AVAudioSettings.AVEncoderBitRateKey);
			if (EncoderBitRatePerChannel.HasValue)
				dict.SetObject (new NSNumber ((int) EncoderBitRatePerChannel.Value), AVAudioSettings.AVEncoderBitRatePerChannelKey);
			if (EncoderBitDepthHint.HasValue){
				var n = EncoderBitDepthHint.Value;
				if (n < 8 || n > 32)
					throw new Exception ("EncoderBitDepthHint should be a value between 8 and 32");
				dict.SetObject (new NSNumber ((int) EncoderBitDepthHint.Value), AVAudioSettings.AVEncoderBitDepthHintKey);
			}
			if (SampleRateConverterAudioQuality.HasValue)
				dict.SetObject (new NSNumber ((int) SampleRateConverterAudioQuality.Value), AVAudioSettings.AVSampleRateConverterAudioQualityKey);

			return dict;
		}
	}
#endif

#if !TVOS
	public partial class AVAudioRecorder {
#if !XAMCORE_2_0
		[Obsolete ("Use static Create method as this method had an invalid signature up to MonoMac 1.4.4", true)]
		public AVAudioRecorder (NSUrl url, NSDictionary settings, NSError outError)
		{
			throw new Exception ("This constructor is no longer supported, use the AVAudioRecorder.ToUrl factory method instead");
		}
#endif
		AVAudioRecorder (NSUrl url, AudioSettings settings, out NSError error) {
			// We use this method because it allows us to out NSError but, as a side effect, it is possible for the handle to be null and we will need to check this manually (on the Create method).
			Handle = InitWithUrl (url, settings.Dictionary, out error);
		}	

		AVAudioRecorder (NSUrl url, AVAudioFormat format, out NSError error) {
			// We use this method because it allows us to out NSError but, as a side effect, it is possible for the handle to be null and we will need to check this manually (on the Create method).
			Handle = InitWithUrl (url, format, out error);
		}

		public static AVAudioRecorder Create (NSUrl url, AudioSettings settings, out NSError error)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			error = null;
			try {
				AVAudioRecorder r = new AVAudioRecorder (url, settings, out error);
				if (r.Handle == IntPtr.Zero) 
					return null;

				return r;
			} catch { 
				return null;
			}
		}

		[iOS (10,0), Mac (10,12)]
		public static AVAudioRecorder Create (NSUrl url, AVAudioFormat format, out NSError error)
		{
			if (format == null)
				throw new ArgumentNullException (nameof (format));
			error = null;
			try {
				AVAudioRecorder r = new AVAudioRecorder (url, format, out error);
				if (r.Handle == IntPtr.Zero) 
					return null;

				return r;
			} catch { 
				return null;
			}
		}

#if !XAMCORE_2_0
		[Advice ("Use 'Create' method.")]
		public static AVAudioRecorder ToUrl (NSUrl url, AVAudioRecorderSettings settings, out NSError error)
		{
			if (settings == null)
				throw new ArgumentNullException ("settings");
			
			return ToUrl (url, settings.ToDictionary (), out error);
		}
#endif

#if XAMCORE_2_0
		internal
#else
		[Advice ("Use 'Create' method.")]
		public
#endif
		static AVAudioRecorder ToUrl (NSUrl url, NSDictionary settings, out NSError error)
		{
			return Create (url, new AudioSettings (settings), out error);
		}
	}
#endif // !TVOS
}

#endif // !WATCH
