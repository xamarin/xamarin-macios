// 
// AudioSettings.cs: Implements strongly typed access for AV audio settings
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, 2014 Xamarin Inc.
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

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using AudioToolbox;

namespace AVFoundation {

	// Should be called AVAudioSetting but AVAudioSetting has been already used by keys class
	[iOS (6,0)]
	public class AudioSettings : DictionaryContainer
	{
#if !COREBUILD
		public AudioSettings ()
			: base (new NSMutableDictionary ())
		{
		}

		public AudioSettings (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public AudioFormatType? Format {
			set {
				SetNumberValue (AVAudioSettings.AVFormatIDKey, (int?) value);
			}
			get {
				return (AudioFormatType?)GetInt32Value (AVAudioSettings.AVFormatIDKey);
			}
		}

#if XAMCORE_2_0
	public double?
#else
		public float?
#endif
			SampleRate {
			set {
				SetNumberValue (AVAudioSettings.AVSampleRateKey, value);
			}
			get {
#if XAMCORE_2_0
				return GetDoubleValue (AVAudioSettings.AVSampleRateKey);
#else
				return GetFloatValue (AVAudioSettings.AVSampleRateKey);
#endif
			}
		}			

		public int? NumberChannels {
			set {
				SetNumberValue (AVAudioSettings.AVNumberOfChannelsKey, value);
			}
			get {
				return GetInt32Value (AVAudioSettings.AVNumberOfChannelsKey);
			}
		}

		public int? LinearPcmBitDepth {
			set {
				if (!(value == 8 || value == 16 || value == 24 || value == 32))
					throw new ArgumentOutOfRangeException ("value must be of 8, 16, 24 or 32");

				SetNumberValue (AVAudioSettings.AVLinearPCMBitDepthKey, value);
			}
			get {
				return GetInt32Value (AVAudioSettings.AVLinearPCMBitDepthKey);
			}
		}

		public bool? LinearPcmBigEndian {
			set {
				SetBooleanValue (AVAudioSettings.AVLinearPCMIsBigEndianKey, value);
			}
			get {
				return GetBoolValue (AVAudioSettings.AVLinearPCMIsBigEndianKey);
			}
		}

		public bool? LinearPcmFloat {
			set {
				SetBooleanValue (AVAudioSettings.AVLinearPCMIsFloatKey, value);
			}
			get {
				return GetBoolValue (AVAudioSettings.AVLinearPCMIsFloatKey);
			}
		}

		public bool? LinearPcmNonInterleaved {
			set {
				SetBooleanValue (AVAudioSettings.AVLinearPCMIsNonInterleaved, value);
			}
			get {
				return GetBoolValue (AVAudioSettings.AVLinearPCMIsNonInterleaved);
			}
		}

		public AVAudioQuality? AudioQuality {
			set {
				SetNumberValue (AVAudioSettings.AVEncoderAudioQualityKey, (nint?) (long?) value);
			}
			get {
				return (AVAudioQuality?) (long?) GetNIntValue (AVAudioSettings.AVEncoderAudioQualityKey);
			}
		}

		public AVAudioQuality? SampleRateConverterAudioQuality {
			set {
				SetNumberValue (AVAudioSettings.AVSampleRateConverterAudioQualityKey, (nint?) (long?) value);
			}
			get {
				return (AVAudioQuality?) (long?) GetNIntValue (AVAudioSettings.AVSampleRateConverterAudioQualityKey);
			}
		}

		public int? EncoderBitRate {
			set {
				SetNumberValue (AVAudioSettings.AVEncoderBitRateKey, value);
			}
			get {
				return GetInt32Value (AVAudioSettings.AVEncoderBitRateKey);
			}			
		}

		public int? EncoderBitRatePerChannel {
			set {
				SetNumberValue (AVAudioSettings.AVEncoderBitRatePerChannelKey, value);
			}
			get {
				return GetInt32Value (AVAudioSettings.AVEncoderBitRatePerChannelKey);
			}			
		}			

		public int? EncoderBitDepthHint {
			set {
				if (value < 8 || value > 32)
					throw new ArgumentOutOfRangeException ("value is required to be between 8 and 32");

				SetNumberValue (AVAudioSettings.AVEncoderBitDepthHintKey, value);
			}
			get {
				return GetInt32Value (AVAudioSettings.AVEncoderBitDepthHintKey);
			}			
		}

		[iOS (7,0)]
		public AVAudioBitRateStrategy? BitRateStrategy {
			set {
				NSString v = null;
				switch (value){
				case AVAudioBitRateStrategy.Constant:
					v = AVAudioSettings._Constant;
					break;
				case AVAudioBitRateStrategy.LongTermAverage:
					v = AVAudioSettings._LongTermAverage;
					break;
				case AVAudioBitRateStrategy.VariableConstrained:
					v = AVAudioSettings._VariableConstrained;
					break;
				case AVAudioBitRateStrategy.Variable:
					v = AVAudioSettings._Variable;
					break;
#if XAMCORE_2_0
				case null:
					break;
				default:
					throw new ArgumentOutOfRangeException ("value");
#endif
				}
				SetStringValue (AVAudioSettings.AVEncoderBitRateStrategyKey, v);
			}
			get {
				var k = GetNSStringValue (AVAudioSettings.AVEncoderBitRateStrategyKey);
				if (k == AVAudioSettings._Constant)
					return AVAudioBitRateStrategy.Constant;
				if (k == AVAudioSettings._LongTermAverage)
					return AVAudioBitRateStrategy.LongTermAverage;
				if (k == AVAudioSettings._VariableConstrained)
					return AVAudioBitRateStrategy.VariableConstrained;
				if (k == AVAudioSettings._Variable)
					return AVAudioBitRateStrategy.Variable;
				return null;
			}
		}

		[iOS (7,0)]
		public AVSampleRateConverterAlgorithm? SampleRateConverterAlgorithm {
			get {
				var k = GetNSStringValue (AVAudioSettings.AVSampleRateConverterAlgorithmKey);
				if (k == AVAudioSettings.AVSampleRateConverterAlgorithm_Normal)
					return AVSampleRateConverterAlgorithm.Normal;
				if (k == AVAudioSettings.AVSampleRateConverterAlgorithm_Mastering)
					return AVSampleRateConverterAlgorithm.Mastering;
				return null;
			}
			set {
				NSString v = null;
				switch (value){
				case AVSampleRateConverterAlgorithm.Mastering:
					v = AVAudioSettings.AVSampleRateConverterAlgorithm_Mastering;
					break;
				case AVSampleRateConverterAlgorithm.Normal:
					v = AVAudioSettings.AVSampleRateConverterAlgorithm_Normal;
					break;
#if XAMCORE_2_0
				case null:
					break;
				default:
					throw new ArgumentOutOfRangeException ("value");
#endif
				}
				SetStringValue (AVAudioSettings.AVSampleRateConverterAlgorithmKey, v);
			}
		}

		[iOS (7,0)]
		public AVAudioQuality? EncoderAudioQualityForVBR {
			get {
				return (AVAudioQuality?) (long?) GetNIntValue (AVAudioSettings.AVEncoderAudioQualityForVBRKey);
			}
			set {
				SetNumberValue (AVAudioSettings.AVEncoderAudioQualityForVBRKey, (nint?) (long?) value);
			}
		}
		
		public AudioChannelLayout ChannelLayout {
			set {
				SetNativeValue (AVAudioSettings.AVChannelLayoutKey, value == null ? null : value.AsData ());
			}
		}

#endif
	}
}

