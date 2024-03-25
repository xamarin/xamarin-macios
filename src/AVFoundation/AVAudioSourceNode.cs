using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AudioToolbox;
using ObjCRuntime;

namespace AVFoundation {

	// This is messy...
	// * The good delegate is:
	//   - AVAudioSourceNodeRenderHandler in XAMCORE_5_0
	//   - AVAudioSourceNodeRenderHandler3 otherwise
	// * There are two legacy delegates:
	//   - AVAudioSourceNodeRenderHandler in .NET and legacy Xamarin
	//   - AVAudioSourceNodeRenderHandler2 in legacy Xamarin.
	//
	// History of mistakes:
	//
	// 1. We first bound this using AVAudioSourceNodeRenderHandler (legacy Xamarin version), which was wrong.
	// 2. We then found the mistake, and bound it as AVAudioSourceNodeRenderHandler2 in legacy Xamarin, removed the initial version in .NET, and named it AVAudioSourceNodeRenderHandler in .NET.
	//    * However, we failed to write tests, so this delegate was broken too.
	// 3. Finally we got a customer report, and realized the new delegate was broken too. So now there are two broken delegates, and one working (AVAudioSourceNodeRenderHandler3 in .NET and legacy Xamarin, named as AVAudioSourceNodeRenderHandler in XAMCORE_5_0).
	//    * And tests were added too.
	//
	// Note: broken = made to work with a workaround, which makes this even messier.
	//

	/// <summary>The delegate that will be called in a callback from <see cref="T:AudioToolbox.AVAudioSourceNode" />.</summary>
	/// <returns>An OSStatus result code. Return 0 to indicate success.</returns>
	/// <param name="isSilence">Indicates whether the supplied audio data only contains silence.</param>
	/// <param name="timestamp">The timestamp the audio renders (HAL time).</param>
	/// <param name="frameCount">The number of frames of audio to supply.</param>
	/// <param name="outputData">The <see cref="T:AudioToolbox.AudioBuffers" /> that contains the supplied audio data when the callback returns.</param>
#if XAMCORE_5_0
	public delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, AudioBuffers outputData);
#else
	public delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler3 (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, AudioBuffers outputData);
#endif

#if !XAMCORE_5_0
#if NET
	[EditorBrowsable (EditorBrowsableState.Never)]
	public delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);
#else
	[EditorBrowsable (EditorBrowsableState.Never)]
	public delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler (bool isSilence, AudioToolbox.AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);
	[EditorBrowsable (EditorBrowsableState.Never)]
	public delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler2 (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);
#endif // NET
#endif // XAMCORE_5_0

	public partial class AVAudioSourceNode {
#if !XAMCORE_5_0 && NET
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the overload that takes a delegate that does not take a 'ref AudioBuffers' instead. Assigning a value to the 'inputData' parameter in the callback has no effect.")]
		public AVAudioSourceNode (AVAudioSourceNodeRenderHandler renderHandler)
			: this (GetHandler (renderHandler))
		{
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the overload that takes a delegate that does not take a 'ref AudioBuffers' instead. Assigning a value to the 'inputData' parameter in the callback has no effect.")]
		public AVAudioSourceNode (AVAudioFormat format, AVAudioSourceNodeRenderHandler renderHandler)
			: this (format, GetHandler (renderHandler))
		{
		}
#endif // !XAMCORE_5_0

#if !NET
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the overload that takes a delegate that does not take a 'ref AudioBuffers' instead. Assigning a value to the 'inputData' parameter in the callback has no effect.")]
		public AVAudioSourceNode (AVAudioSourceNodeRenderHandler2 renderHandler)
			: this (GetHandler (renderHandler))
		{
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the overload that takes a delegate that does not take a 'ref AudioBuffers' instead. Assigning a value to the 'inputData' parameter in the callback has no effect.")]
		public AVAudioSourceNode (AVAudioFormat format, AVAudioSourceNodeRenderHandler2 renderHandler)
			: this (format, GetHandler (renderHandler))
		{
		}
#endif // !NET

		/// <summary>Creates an <see cref="T:AudioToolbox.AVAudioSourceNode" /> with the specified callback to render audio.</summary>
		/// <param name="renderHandler">The callback that will be called to supply audio data.</param>
#if XAMCORE_5_0
		public AVAudioSourceNode (AVAudioSourceNodeRenderHandler renderHandler)
#else
		public AVAudioSourceNode (AVAudioSourceNodeRenderHandler3 renderHandler)
#endif // XAMCORE_5_0
			: this (GetHandler (renderHandler))
		{
		}

		/// <summary>Creates an <see cref="T:AudioToolbox.AVAudioSourceNode" /> with the specified callback to render audio.</summary>
		/// <param name="format">The format of the PCM audio data the callback supplies.</param>
		/// <param name="renderHandler">The callback that will be called to supply audio data.</param>
#if XAMCORE_5_0
		public AVAudioSourceNode (AVAudioFormat format, AVAudioSourceNodeRenderHandler renderHandler)
#else
		public AVAudioSourceNode (AVAudioFormat format, AVAudioSourceNodeRenderHandler3 renderHandler)
#endif // XAMCORE_5_0
			: this (format, GetHandler (renderHandler))
		{
		}

#if !NET
		static AVAudioSourceNodeRenderHandlerRaw GetHandler (AVAudioSourceNodeRenderHandler renderHandler)
		{
			AVAudioSourceNodeRenderHandlerRaw rv = (IntPtr isSilence, IntPtr timestamp, uint frameCount, IntPtr outputData) => {
				unsafe {
					byte* isSilencePtr = (byte*) isSilence;
					bool isSilenceBool = (*isSilencePtr) != 0;
					AudioTimeStamp timestampValue = *(AudioTimeStamp*) timestamp;
					var buffers = new AudioBuffers (outputData);
					return renderHandler (isSilenceBool, timestampValue, frameCount, ref buffers);
				}
			};
			return rv;
		}
#endif // !NET

#if !XAMCORE_5_0
#if NET
		static AVAudioSourceNodeRenderHandlerRaw GetHandler (AVAudioSourceNodeRenderHandler renderHandler)
#else
		static AVAudioSourceNodeRenderHandlerRaw GetHandler (AVAudioSourceNodeRenderHandler2 renderHandler)
#endif // NET
		{
			AVAudioSourceNodeRenderHandlerRaw rv = (IntPtr isSilence, IntPtr timestamp, uint frameCount, IntPtr outputData) => {
				unsafe {
					byte* isSilencePtr = (byte*) isSilence;
					bool isSilenceBool = (*isSilencePtr) != 0;
					var buffers = new AudioBuffers (outputData);
					var rv = renderHandler (ref isSilenceBool, ref Unsafe.AsRef<AudioTimeStamp> ((void*) timestamp), frameCount, ref buffers);
					*isSilencePtr = isSilenceBool.AsByte ();
					return rv;
				}
			};
			return rv;
		}
#endif // !XAMCORE_5_0

#if XAMCORE_5_0
		static AVAudioSourceNodeRenderHandlerRaw GetHandler (AVAudioSourceNodeRenderHandler renderHandler)
#else
		static AVAudioSourceNodeRenderHandlerRaw GetHandler (AVAudioSourceNodeRenderHandler3 renderHandler)
		{
#endif // !XAMCORE_5_0
			AVAudioSourceNodeRenderHandlerRaw rv = (IntPtr isSilence, IntPtr timestamp, uint frameCount, IntPtr outputData) => {
				unsafe {
					byte* isSilencePtr = (byte*) isSilence;
					bool isSilenceBool = (*isSilencePtr) != 0;
					var buffers = new AudioBuffers (outputData);
					var rv = renderHandler (ref isSilenceBool, ref Unsafe.AsRef<AudioTimeStamp> ((void*) timestamp), frameCount, buffers);
					*isSilencePtr = isSilenceBool.AsByte ();
					return rv;
				}
			};
			return rv;
		}
	}
}
