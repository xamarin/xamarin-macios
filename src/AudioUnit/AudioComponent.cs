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
using ObjCRuntime;
using AudioToolbox;
using CoreFoundation;
using Foundation;

namespace AudioUnit
{

#if !COREBUILD
	// keys are not constants and had to be found in AudioToolbox.framework/Headers/AudioComponent.h
	[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
	public partial class ResourceUsageInfo : DictionaryContainer {
		static NSString userClientK = new NSString ("iokit.user-client");
		static NSString globalNameK = new NSString ("mach-lookup.global-name");
		static NSString networkClientK = new NSString ("network.client");
		static NSString exceptionK = new NSString ("temporary-exception.files.all.read-write");

		public ResourceUsageInfo () : base () {}

		public ResourceUsageInfo (NSDictionary dic) : base (dic) {}

		public string[] IOKitUserClient { 
			get {
				var array = GetNativeValue<NSArray> (userClientK);
				if (array == null )
					return null;
				return NSArray.StringArrayFromHandle (array.Handle);
			} 
			set {
				if (value == null)
					RemoveValue (userClientK);
				else
					SetArrayValue (userClientK, value);
			}
		}

		public string[] MachLookUpGlobalName { 
			get {
				var array = GetNativeValue<NSArray> (globalNameK);
				if (array == null)
					return null;
				return NSArray.StringArrayFromHandle (array.Handle);
			} 
			set {
				if (value == null)
					RemoveValue (globalNameK);	
				else
					SetArrayValue (globalNameK, value);
			}
		}

		public bool? NetworkClient { 
			get {
				return GetBoolValue (networkClientK);
			} 
			set {
				SetBooleanValue (networkClientK, value);
			}
		}

		public bool? TemporaryExceptionReadWrite { 
			get {
				return GetBoolValue (exceptionK);
			} 
			set {
				SetBooleanValue (exceptionK, value);
			}
		}
	}

	// keys are not constants and had to be found in AudioToolbox.framework/Headers/AudioComponent.h
	[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
	public partial class AudioComponentInfo : DictionaryContainer {
		static NSString typeK = new NSString ("type");
		static NSString subtypeK = new NSString ("subtype");
		static NSString manufacturerK = new NSString ("manufacturer");
		static NSString nameK = new NSString ("name");
		static NSString versionK = new NSString ("version");
		static NSString factoryFunctionK = new NSString ("factoryFunction");
		static NSString sandboxSafeK = new NSString ("sandboxSafe");
		static NSString resourceUsageK = new NSString ("resourceUsage");
		static NSString tagsK = new NSString ("tags");

		public AudioComponentInfo () : base () {}

		public AudioComponentInfo (NSDictionary dic) : base (dic) {}

		public string Type { 
			get {
				return GetStringValue (typeK);
			} 
			set {
				SetStringValue (typeK, value);
			}
		}

		public string Subtype { 
			get {
				return GetStringValue (subtypeK);
			} 
			set {
				SetStringValue (subtypeK, value);
			}
		}

		public string Manufacturer { 
			get {
				return GetStringValue (manufacturerK);
			} 
			set {
				SetStringValue (manufacturerK, value);
			}
		}

		public string Name { 
			get {
				return GetStringValue (nameK);
			} 
			set {
				SetStringValue (nameK, value);
			}
		}

		public nuint? Version { 
			get {
				return GetNUIntValue (versionK);
			} 
			set {
				SetNumberValue (versionK, value);
			}
		}

		public string FactoryFunction { 
			get {
				return GetStringValue (factoryFunctionK);
			} 
			set {
				SetStringValue (factoryFunctionK, value);
			}
		}

		public bool? SandboxSafe { 
			get {
				return GetBoolValue (sandboxSafeK);
			} 
			set {
				SetBooleanValue (sandboxSafeK, value);
			}
		}

		public ResourceUsageInfo ResourceUsage { 
			get {
				return GetStrongDictionary<ResourceUsageInfo> (resourceUsageK);
			} 
			set {
				SetNativeValue (resourceUsageK, value?.Dictionary, true);
			}
		}

		public string[] Tags { 
			get {
				var array = GetNativeValue<NSArray> (tagsK);
				if (array == null)
					return null;
				return NSArray.StringArrayFromHandle (array.Handle);
			} 
			set {
				if (value == null)
					RemoveValue (tagsK);	
				else
					SetArrayValue (tagsK, value);
			}
		}
	}

#endif


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
		[Obsolete ("Use the overload that accept a ref to the 'AudioComponentDescription' parameter.")]
		public static AudioComponent FindNextComponent (AudioComponent cmp, AudioComponentDescription cd)
		{
			return FindNextComponent (cmp, ref cd);
		}

		[Obsolete ("Use the overload that accept a ref to the 'AudioComponentDescription' parameter.")]
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
		public UIKit.UIImage GetIcon (float desiredPointSize)
		{
			return new UIKit.UIImage (AudioComponentGetIcon (handle, desiredPointSize));
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
		public AppKit.NSImage GetIcon ()
		{
			return new AppKit.NSImage (AudioComponentGetIcon (handle));
		}
#endif

#if IOS || MONOMAC
		[NoWatch, NoTV, Mac (10,13, onlyOn64: true), iOS (11,0)]
		[DllImport (Constants.AudioUnitLibrary)]
		static extern int /* OSStatus */ AudioUnitExtensionSetComponentList (IntPtr /* CFString */ extensionIdentifier, /* CFArrayRef */ IntPtr audioComponentInfo);

		[NoWatch, NoTV, Mac (10,13, onlyOn64: true), iOS (11,0)]
		[DllImport (Constants.AudioUnitLibrary)]
		static extern /* CFArrayRef */ IntPtr AudioUnitExtensionCopyComponentList (IntPtr /* CFString */ extensionIdentifier);

		[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
		public AudioComponentInfo[] ComponentList {
			get {
				using (var cfString = new CFString (Name)) {
					var cHandle = AudioUnitExtensionCopyComponentList (cfString.Handle);
					if (cHandle == IntPtr.Zero)
						return null;
					using (var nsArray = Runtime.GetNSObject<NSArray> (cHandle, owns: true)) {
						if (nsArray == null)
							return null;
						// make things easier for developers since we do not know how to have an implicit conversion from NSObject to AudioComponentInfo
						var dics = NSArray.FromArray <NSDictionary> (nsArray);
						var result = new AudioComponentInfo [dics.Length];
						for (var i = 0; i < result.Length; i++) {
							result [i] = new AudioComponentInfo (dics[i]);
						}
						return result;
					}
				}
			}
			set {
				if (value == null)
					throw new ArgumentNullException	(nameof	(value));
				using (var cfString = new CFString (Name)) {
					var dics = new NSDictionary [value.Length];
					for (var i = 0; i < value.Length; i++) {
						dics [i] = value [i].Dictionary;
					}
					using (var array = NSArray.FromNSObjects (dics)) {
						var result = (AudioConverterError) AudioUnitExtensionSetComponentList (cfString.Handle, array.Handle);
						switch (result) {
						case AudioConverterError.None:
							return;
						default:
							throw new InvalidOperationException ($"ComponentList could not be set, error {result.ToString ()}");

						}
					}
				}
			}
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
