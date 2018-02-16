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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AVFoundation {

#if !MONOMAC
	// Convience enum for native string values - AVAudioSession.h
	public enum AVAudioSessionCategory {
		Ambient,
		SoloAmbient,
		Playback,
		Record,
		PlayAndRecord,
		AudioProcessing,
		MultiRoute
	}

	public partial class AVAudioSession : NSObject {
#if !XAMCORE_2_0
		[Obsolete ("Please use the static SharedInstance property as this type is not meant to be created")]
		[CompilerGenerated]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Export ("init")]
		public AVAudioSession () : base (NSObjectFlag.Empty)
		{
			if (IsDirectBinding) {
				Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle ("init"));
			} else {
				Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("init"));
			}
		}

		[Obsolete ("Use 'SetActive(bool, out NSError)' instead.")]
		public bool SetActive (bool beActive, NSError outError)
		{
			// Effectively discarded due to original binding error
			return SetActive (beActive, out outError);
		}
#endif

		public NSError SetActive (bool beActive)
		{
			NSError outError;
			if (SetActive (beActive, out outError))
				return null;
			return outError;
		}

#if !TVOS && !WATCH
		public NSError SetActive (bool active, AVAudioSessionSetActiveOptions options)
		{
			NSError outError;
			if (SetActive (active, options, out outError))
				return null;
			return outError;
		}
#endif
		
#if !XAMCORE_2_0
		[Obsolete ("Use 'SetCategory(bool, out NSError)' instead.")]
		public bool SetCategory (NSString theCategory, NSError outError)
		{
			// Effectively discarded due to original binding error
			return SetCategory (theCategory, out outError);
		}
#endif

		public NSError SetCategory (NSString theCategory)
		{
			NSError outError;
			
			if (SetCategory (theCategory, out outError))
				return null;
			return outError;
		}

		internal NSString CategoryToToken (AVAudioSessionCategory category)
		{
			switch (category){
			case AVAudioSessionCategory.Ambient:
				return AVAudioSession.CategoryAmbient;
			case AVAudioSessionCategory.SoloAmbient:
				return AVAudioSession.CategorySoloAmbient;
			case AVAudioSessionCategory.Playback:
				return AVAudioSession.CategoryPlayback;
			case AVAudioSessionCategory.Record:
				return AVAudioSession.CategoryRecord;
			case AVAudioSessionCategory.PlayAndRecord:
				return AVAudioSession.CategoryPlayAndRecord;
#if !TVOS && !WATCH
			case AVAudioSessionCategory.AudioProcessing:
				return AVAudioSession.CategoryAudioProcessing;
#endif
			case AVAudioSessionCategory.MultiRoute:
				return AVAudioSession.CategoryMultiRoute;
			}
			return null;
		}
		
		public NSError SetCategory (AVAudioSessionCategory category)
		{
			return SetCategory (CategoryToToken (category));
		}

		public NSError SetCategory (AVAudioSessionCategory category, AVAudioSessionCategoryOptions options)
		{
			NSError error;
			if (SetCategory (CategoryToToken (category), options, out error))
				return null;
			return error;
		}

#if !XAMCORE_2_0
		[Obsolete ("Use 'SetPreferredSampleRate(bool, out NSError)' on iOS 6.0 instead.")]
		public bool SetPreferredHardwareSampleRate (double sampleRate, NSError outError)
		{
			// Effectively discarded due to original binding error
			return SetPreferredHardwareSampleRate (sampleRate, out outError);
		}

		[Obsolete ("Use 'SetPreferredIOBufferDuration(bool, out NSError)' instead.")]
		public bool SetPreferredIOBufferDuration (double duration, NSError outError)
		{
			// Effectively discarded due to original binding error
			return SetPreferredIOBufferDuration (duration, out outError);
		}
#endif
	}
#endif
}
