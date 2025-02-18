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

namespace AudioToolbox {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class InstrumentInfo {

		// defines, not NSString, are used for the key names
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public const string NameKey = "name";
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public const string MSBKey = "MSB";
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public const string LSBKey = "LSB";
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public const string ProgramKey = "program";

		internal InstrumentInfo (NSDictionary d)
		{
			Dictionary = d;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string Name {
			get { return Dictionary [NameKey].ToString (); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MSB {
			get { return (Dictionary [MSBKey] as NSNumber)!.Int32Value; }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int LSB {
			get { return (Dictionary [LSBKey] as NSNumber)!.Int32Value; }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int Program {
			get { return (Dictionary [ProgramKey] as NSNumber)!.Int32Value; }
		}

		// some API likely wants the [CF|NS]Dictionary
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		unsafe extern static OSStatus CopyNameFromSoundBank (/* CFURLRef */ IntPtr inURL, /* CFStringRef */ IntPtr* outName);

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

			IntPtr name = IntPtr.Zero;
			OSStatus error;
			unsafe {
				error = CopyNameFromSoundBank (url.Handle, &name);
			}
			var result = CFString.FromHandle (name);
			return (error != 0) ? null : result;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus CopyInstrumentInfoFromSoundBank (/* CFURLRef */ IntPtr inURL, /* CFSArrayRef */ IntPtr* outInstrumentInfo);

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
			OSStatus error;
			unsafe {
				error = CopyInstrumentInfoFromSoundBank (url.Handle, &array);
			}
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
