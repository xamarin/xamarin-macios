//
// Copyright (c) Microsoft Corporation.
// Licensed under MIT License.
//

#nullable enable

namespace CarPlay {

	public partial class CPNowPlayingSportsClock {

		/// <summary>This enum is used to select how to initialize a new <see cref="CPNowPlayingSportsClock" /> instance.</summary>
		[SupportedOSPlatform ("ios18.4")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public enum CPNowPlayingSportsClockTimeOption {
			/// <summary>The <c>time</c> parameter passed to the constructor is elapsed time.</summary>
			ElapsedTime,
			/// <summary>The <c>time</c> parameter passed to the constructor is remaining time.</summary>
			RemainingTime,
		}

		/// <summary>Create a new <see cref="CPNowPlayingSportsClock" /> instance with the specified <c>time</c> and <c>paused</c> values.</summary>
		/// <param name="time">The elapsed or remaining time of the clock.</param>
		/// <param name="paused">If the clock is paused or not.</param>
		/// <param name="type">Use this option to specify whether the <paramref name="time" /> parameter refers to elapsed time or remaining time.</param>
		public CPNowPlayingSportsClock (double time, bool paused, CPNowPlayingSportsClockTimeOption type)
			: base (NSObjectFlag.Empty)
		{
			switch (type) {
			case CPNowPlayingSportsClockTimeOption.ElapsedTime:
				InitializeHandle (_InitWithElapsedTime (time, paused), "initWithElapsedTime:paused:");
				break;
			case CPNowPlayingSportsClockTimeOption.RemainingTime:
				InitializeHandle (_InitWithRemainingTime (time, paused), "initWithRemainingTime:paused:");
				break;
			default:
				throw new ArgumentException (nameof (type));
			}
		}
	}
}
