using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using AudioUnit;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {
	public partial class AVSpeechSynthesisProviderAudioUnit {
		/// <summary>Create a new <see cref="AVSpeechSynthesisProviderAudioUnit" /> instance.</summary>
		/// <param name="componentDescription">A description of the component to create.</param>
		/// <param name="options">Any options for the returned audio unit.</param>
		/// <param name="error">The error if an error occurred, null otherwise.</param>
		/// <returns>A new <see cref="AVSpeechSynthesisProviderAudioUnit" /> instance if successful, null otherwise.</returns>
		public static AVSpeechSynthesisProviderAudioUnit? Create (AudioComponentDescription componentDescription, AudioComponentInstantiationOptions options, out NSError? error)
		{
			var rv = new AVSpeechSynthesisProviderAudioUnit (NSObjectFlag.Empty);
			rv.InitializeHandle (rv._InitWithComponentDescription (componentDescription, options, out error), string.Empty, false);
			if (rv.Handle == IntPtr.Zero) {
				rv.Dispose ();
				return null;
			}
			return rv;
		}
	}
}
