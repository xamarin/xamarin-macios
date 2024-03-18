//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2015 Xamarin, Inc.
//

#if !TVOS && !MONOMAC && !WATCH

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace MediaPlayer {

	// MPVolumeSettings.h
	public static class MPVolumeSettings {
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios11.3", "Use 'MPVolumeView' to present volume controls.")]
#else
		[Deprecated (PlatformName.iOS, 11, 3, message: "Use 'MPVolumeView' to present volume controls.")]
#endif
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint = "MPVolumeSettingsAlertShow")]
		public extern static void AlertShow ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios11.3", "Use 'MPVolumeView' to present volume controls.")]
#else
		[Deprecated (PlatformName.iOS, 11, 3, message: "Use 'MPVolumeView' to present volume controls.")]
#endif
		[DllImport (Constants.MediaPlayerLibrary, EntryPoint = "MPVolumeSettingsAlertHide")]
		public extern static void AlertHide ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios11.3", "Use 'MPVolumeView' to present volume controls.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'MPVolumeView' to present volume controls.")]
#else
		[Deprecated (PlatformName.iOS, 11, 3, message: "Use 'MPVolumeView' to present volume controls.")]
#endif
		[DllImport (Constants.MediaPlayerLibrary)]
		extern static /* BOOL */ byte MPVolumeSettingsAlertIsVisible ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios11.3", "Use 'MPVolumeView' to present volume controls.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'MPVolumeView' to present volume controls.")]
#else
		[Deprecated (PlatformName.iOS, 11, 3, message: "Use 'MPVolumeView' to present volume controls.")]
#endif
		public static bool AlertIsVisible ()
		{
			return MPVolumeSettingsAlertIsVisible () != 0;
		}
	}
}

#endif
