//
// AudioComponent.cs: AudioComponent wrapper class
//
// Author:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2011, 2012 Xamarin Inc
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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;
using XamCore.AudioToolbox;
using XamCore.CoreFoundation;
using XamCore.Foundation;

namespace XamCore.AudioUnit
{
	public class AudioComponent : INativeObject {
#if !COREBUILD
		internal IntPtr handle;

		public IntPtr Handle { get { return handle; } }

		internal AudioComponent(IntPtr handle)
		{ 
			this.handle = handle;
		}
			
		public AudioUnit CreateAudioUnit ()
		{
			return new AudioUnit (this);
		}

#if !XAMCORE_2_0
		[Obsolete ("Use the overload that accept a ref to the AudioComponentDescription parameter")]
		public static AudioComponent FindNextComponent (AudioComponent cmp, AudioComponentDescription cd)
		{
			return FindNextComponent (cmp, ref cd);
		}

		[Obsolete ("Use the overload that accept a ref to the AudioComponentDescription parameter")]
		public static AudioComponent FindComponent (AudioComponentDescription cd)
		{
			return FindNextComponent (null, ref cd);
		}

		public static AudioComponent FindNextComponent (AudioComponent cmp, ref AudioComponentDescription cd)
		{
			var handle = cmp == null ? IntPtr.Zero : cmp.Handle;
			var cdn = new AudioComponentDescriptionNative (cd);
			handle = AudioComponentFindNext (handle, ref cdn);
			cdn.CopyTo (cd);
			return  (handle != IntPtr.Zero) ? new AudioComponent (handle) : null;
		}
#else
		public static AudioComponent FindNextComponent (AudioComponent cmp, ref AudioComponentDescription cd)
		{
			var handle = cmp == null ? IntPtr.Zero : cmp.Handle;
			handle = AudioComponentFindNext (handle, ref cd);
			return  (handle != IntPtr.Zero) ? new AudioComponent (handle) : null;
		}
#endif
		public static AudioComponent FindComponent (ref AudioComponentDescription cd)
		{
			return FindNextComponent (null, ref cd);
		}

		public static AudioComponent FindComponent (AudioTypeOutput output)
		{
			var cd = AudioComponentDescription.CreateOutput (output);
			return FindComponent (ref cd);
		}

		public static AudioComponent FindComponent (AudioTypeMusicDevice musicDevice)
		{
			var cd = AudioComponentDescription.CreateMusicDevice (musicDevice);
			return FindComponent (ref cd);
		}
		
		public static AudioComponent FindComponent (AudioTypeConverter conveter)
		{
			var cd = AudioComponentDescription.CreateConverter (conveter);
			return FindComponent (ref cd);
		}
		
		public static AudioComponent FindComponent (AudioTypeEffect effect)
		{
			var cd = AudioComponentDescription.CreateEffect (effect);
			return FindComponent (ref cd);
		}
		
		public static AudioComponent FindComponent (AudioTypeMixer mixer)
		{
			var cd = AudioComponentDescription.CreateMixer (mixer);
			return FindComponent (ref cd);
		}
		
		public static AudioComponent FindComponent (AudioTypePanner panner)
		{
			var cd = AudioComponentDescription.CreatePanner (panner);
			return FindComponent (ref cd);
		}
		
		public static AudioComponent FindComponent (AudioTypeGenerator generator)
		{
			var cd = AudioComponentDescription.CreateGenerator (generator);
			return FindComponent (ref cd);
		}

		[DllImport(Constants.AudioUnitLibrary)]
#if XAMCORE_2_0
		static extern IntPtr AudioComponentFindNext (IntPtr inComponent, ref AudioComponentDescription inDesc);
#else
		static extern IntPtr AudioComponentFindNext (IntPtr inComponent, ref AudioComponentDescriptionNative inDesc);
#endif
		
		[DllImport(Constants.AudioUnitLibrary, EntryPoint = "AudioComponentCopyName")]
		static extern int /* OSStatus */ AudioComponentCopyName (IntPtr component, out IntPtr cfstr);
		
		public string Name {
			get {
				IntPtr r;
				if (AudioComponentCopyName (handle, out r) == 0)
					return CFString.FetchString (r);
				return null;
			}
		}
	
#if XAMCORE_2_0
		[DllImport (Constants.AudioUnitLibrary)]
		static extern int /* OSStatus */ AudioComponentGetDescription (IntPtr component, out AudioComponentDescription desc);

		public AudioComponentDescription? Description {
			get {
				AudioComponentDescription desc;

				if (AudioComponentGetDescription (handle, out desc) == 0)
					return desc;

				return null;
			}
		}
#else
		[DllImport (Constants.AudioUnitLibrary)]
		static extern int /* OSStatus */ AudioComponentGetDescription (IntPtr component, out AudioComponentDescriptionNative desc);
		public AudioComponentDescription Description {
			get {
				AudioComponentDescriptionNative desc;

				if (AudioComponentGetDescription (handle, out desc) == 0)
					return new AudioComponentDescription (desc);

				return null;
			}
		}
#endif

		[DllImport(Constants.AudioUnitLibrary)]
		static extern int /* OSStatus */ AudioComponentGetVersion (IntPtr component, out int /* UInt32* */ version);

		public Version Version {
			get {
				int ret;
				if (AudioComponentGetVersion (handle, out ret) == 0)
					return new Version (ret >> 16, (ret >> 8) & 0xff, ret & 0xff);

				return null;
			}
		}

#if !MONOMAC
		[iOS (7,0)]
		[DllImport(Constants.AudioUnitLibrary)]
		static extern IntPtr AudioComponentGetIcon (IntPtr comp, float /* float */ desiredPointSize);

		[iOS (7,0)]
		public XamCore.UIKit.UIImage GetIcon (float desiredPointSize)
		{
			return new XamCore.UIKit.UIImage (AudioComponentGetIcon (handle, desiredPointSize));
		}

		[iOS (7,0)]
		[DllImport(Constants.AudioUnitLibrary)]
		static extern double AudioComponentGetLastActiveTime (IntPtr comp);

		[iOS (7,0)]
		public double LastActiveTime {
			get {
				return AudioComponentGetLastActiveTime (handle);
			}
		}
#else
		// extern NSImage * __nullable AudioComponentGetIcon (AudioComponent __nonnull comp) __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)]
		[DllImport (Constants.AudioUnitLibrary)]
		static extern IntPtr AudioComponentGetIcon (IntPtr comp);

		[Mac (10,11)]
		public XamCore.AppKit.NSImage GetIcon ()
		{
			return new XamCore.AppKit.NSImage (AudioComponentGetIcon (handle));
		}
#endif
#endif // !COREBUILD
    }

#if !COREBUILD
	public static class AudioComponentValidationParameter {
//		#define kAudioComponentValidationParameter_ForceValidation		 "ForceValidation"
		public static NSString ForceValidation = new NSString ("ForceValidation");

//		#define kAudioComponentValidationParameter_TimeOut				"TimeOut"
		public static NSString TimeOut = new NSString ("TimeOut");
	}

	public static class AudioComponentConfigurationInfo {
//		#define kAudioComponentConfigurationInfo_ValidationResult	"ValidationResult"
		public static NSString ValidationResult = new NSString ("ValidationResult");
	}
#endif
}