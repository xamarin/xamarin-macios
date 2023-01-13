using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using AudioToolbox;

#nullable enable

namespace AVFoundation {

	public partial class AVSpeechUtterance {

		public AVSpeechUtterance (string speechString, bool isSsmlRepresentation = false) : base (NSObjectFlag.Empty)
		{
			if (isSsmlRepresentation) {
#if IOS || __MACCATALYST__ || TVOS
				if (SystemVersion.CheckiOS (16, 0)) {
#elif WATCH
				if (SystemVersion.CheckwatchOS (9, 0)) {
#else
				if (SystemVersion.CheckmacOS (13, 0)) {
#endif
					InitializeHandle (_InitFromSsmlRepresentation (speechString), "initWithSSMLRepresentation:");
				} else {
					throw new PlatformNotSupportedException ("'isSsmlRepresentation = true' is not supported in this version of the OS.");
				}
			} else {
				InitializeHandle (_InitFromString (speechString), "initWithString:");
			}
		}

	}

}
