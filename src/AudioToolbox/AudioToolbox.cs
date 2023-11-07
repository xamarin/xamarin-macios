// 
// AudioToolbox.cs: functions defined in AudioToolbox.h
//
// Copyright 2013-2014 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

using OSStatus = System.Int32;

namespace AudioToolbox {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class InstrumentInfo {

		// defines, not NSString, are used for the key names
		public const string NameKey = "name";
		public const string MSBKey = "MSB";
		public const string LSBKey = "LSB";
		public const string ProgramKey = "program";

		internal InstrumentInfo (NSDictionary d)
		{
			Dictionary = d;
		}

		public string Name {
			get { return Dictionary [NameKey].ToString (); }
		}

		public int MSB {
			get { return (Dictionary [MSBKey] as NSNumber)!.Int32Value; }
		}

		public int LSB {
			get { return (Dictionary [LSBKey] as NSNumber)!.Int32Value; }
		}

		public int Program {
			get { return (Dictionary [ProgramKey] as NSNumber)!.Int32Value; }
		}

		// some API likely wants the [CF|NS]Dictionary
		public NSDictionary Dictionary { get; private set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class SoundBank {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus CopyNameFromSoundBank (/* CFURLRef */ IntPtr inURL, /* CFStringRef */ ref IntPtr outName);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static string? GetName (NSUrl url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			string? result = null;
			IntPtr name = IntPtr.Zero;
			var error = CopyNameFromSoundBank (url.Handle, ref name);
			if (name != IntPtr.Zero) {
				using (NSString s = new NSString (name))
					result = s.ToString ();
			}
			return (error != 0) ? null : result;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus CopyInstrumentInfoFromSoundBank (/* CFURLRef */ IntPtr inURL, /* CFSArrayRef */ ref IntPtr outInstrumentInfo);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static InstrumentInfo []? GetInstrumentInfo (NSUrl url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			InstrumentInfo []? result = null;
			IntPtr array = IntPtr.Zero;
			var error = CopyInstrumentInfoFromSoundBank (url.Handle, ref array);
			if (array != IntPtr.Zero) {
				var dicts = NSArray.ArrayFromHandle<NSDictionary> (array);
				result = new InstrumentInfo [dicts.Length];
				for (int i = 0; i < dicts.Length; i++)
					result [i] = new InstrumentInfo (dicts [i]);
				CFObject.CFRelease (array);
			}
			return (error != 0) ? null : result;
		}
	}
}
