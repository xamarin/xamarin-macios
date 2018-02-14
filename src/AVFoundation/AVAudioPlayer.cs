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
using Foundation;
using ObjCRuntime;
using AudioToolbox;
using System;

namespace AVFoundation {
#if !XAMCORE_2_0
	[Advice ("Use 'AudioSettings' instead.")]
	public class AVAudioPlayerSettings {
		NSDictionary dict;
		
		internal AVAudioPlayerSettings (NSDictionary dictionary)
		{
			dict = dictionary;
		}

		public AudioChannelLayout AudioChannelLayout {
			get {
				var data = dict.ObjectForKey (AVAudioSettings.AVChannelLayoutKey) as NSData;
				if (data == null)
					return new AudioChannelLayout ();
				return AudioChannelLayout.FromHandle (data.Bytes);
			}
		}

		public int EncoderBitRateKey {
			get {
				var rate = dict.ObjectForKey (AVAudioSettings.AVEncoderBitRateKey) as NSNumber;
				return rate == null ? 0 : rate.Int32Value;
			}
		}

		public AudioFormatType AudioFormat {
			get {
				var ft = dict.ObjectForKey (AVAudioSettings.AVFormatIDKey) as NSNumber;
				return (AudioFormatType) (ft == null ? -1 : ft.Int32Value);
			}
		}

		public int NumberChannels {
			get {
				var n =  dict.ObjectForKey (AVAudioSettings.AVNumberOfChannelsKey) as NSNumber;
				return n == null ? 1 : n.Int32Value;
			}
		}

		public float SampleRate {
			get {
				var r = dict.ObjectForKey (AVAudioSettings.AVSampleRateKey) as NSNumber;
				return r == null ? 0 : r.FloatValue;
			}
		}
		
		public static implicit operator NSDictionary (AVAudioPlayerSettings settings)
		{
			return settings.dict;
		}
	}
#endif

#if !WATCH
	public partial class AVAudioPlayer {

		public static AVAudioPlayer FromUrl (NSUrl url, out NSError error)
		{
			unsafe {
				IntPtr errhandle;
				IntPtr ptrtohandle = (IntPtr) (&errhandle);

				var ap = new AVAudioPlayer (url, ptrtohandle);
				if (ap.Handle == IntPtr.Zero){
					error = (NSError) Runtime.GetNSObject (errhandle);
					return null;
				} else
					error = null;
				return ap;
			}
		}

		public static AVAudioPlayer FromUrl (NSUrl url)
		{
			unsafe {
				var ap = new AVAudioPlayer (url, IntPtr.Zero);
				if (ap.Handle == IntPtr.Zero)
					return null;

				return ap;
			}
		}

		public static AVAudioPlayer FromData (NSData data, out NSError error)
		{
			unsafe {
				IntPtr errhandle;
				IntPtr ptrtohandle = (IntPtr) (&errhandle);

				var ap = new AVAudioPlayer (data, ptrtohandle);
				if (ap.Handle == IntPtr.Zero){
					error = (NSError) Runtime.GetNSObject (errhandle);
					return null;
				} else
					error = null;
				return ap;
			}
		}

		public static AVAudioPlayer FromData (NSData data)
		{
			unsafe {
				var ap = new AVAudioPlayer (data, IntPtr.Zero);
				if (ap.Handle == IntPtr.Zero)
					return null;

				return ap;
			}
		}

#if !XAMCORE_2_0
		[Obsolete ("This method had an invalid signature in MonoMac 1.0.3, use 'AVAudioPlayer.FromUrl' instead.")]
		public AVAudioPlayer (NSUrl url, NSError error) : this (url, IntPtr.Zero)
		{
			
		}

		[Obsolete ("This method had an invalid signature in MonoMac 1.0.3, use 'AVAudioPlayer.FromData' instead.")]
		public AVAudioPlayer (NSData data, NSError error) : this (data, IntPtr.Zero)
		{
			
		}

		[Advice ("Use SoundSettings")]
		public AVAudioPlayerSettings Settings {
			get {
				return new AVAudioPlayerSettings (WeakSettings);
			}
		}

		[Advice ("This method was incorrectly named, use 'PlayAtTime' instead.")]
		public bool PlayAtTimetime (double time)
		{
			return PlayAtTime (time);
		}
#endif
	}
#endif // !WATCH
}
