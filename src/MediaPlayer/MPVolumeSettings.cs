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
using Foundation;
using ObjCRuntime;

namespace MediaPlayer {

	// MPVolumeSettings.h
	public static class MPVolumeSettings {
		[Deprecated (PlatformName.iOS, 11,3, message: "Use 'MPVolumeView' to present volume controls.")]
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint="MPVolumeSettingsAlertShow")]
		public extern static void AlertShow ();

		[Deprecated (PlatformName.iOS, 11,3, message: "Use 'MPVolumeView' to present volume controls.")]
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint="MPVolumeSettingsAlertHide")]
		public extern static void AlertHide ();

		// note: sizeof (BOOL) is 1 like C, i.e. it's not a Win32 BOOL (4 bytes)
		[Deprecated (PlatformName.iOS, 11,3, message: "Use 'MPVolumeView' to present volume controls.")]
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint="MPVolumeSettingsAlertIsVisible")]
		[return: MarshalAs (UnmanagedType.I1)]
		public extern static /* BOOL */ bool AlertIsVisible ();
	}
}

#endif
