//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2015 Xamarin, Inc.
//

#if !TVOS && !MONOMAC

using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.MediaPlayer {

	// MPVolumeSettings.h
	public static class MPVolumeSettings {
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint="MPVolumeSettingsAlertShow")]
		public extern static void AlertShow ();

		[DllImport (Constants.MediaPlayerLibrary, EntryPoint="MPVolumeSettingsAlertHide")]
		public extern static void AlertHide ();

		// note: sizeof (BOOL) is 1 like C, i.e. it's not a Win32 BOOL (4 bytes)
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint="MPVolumeSettingsAlertIsVisible")]
		[return: MarshalAs (UnmanagedType.I1)]
		public extern static /* BOOL */ bool AlertIsVisible ();
	}
}

#endif
