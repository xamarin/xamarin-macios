//
// AudioConverter.cs: AudioConverter wrapper class
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2013 Xamarin Inc.
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

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioToolbox {
	public enum AudioConverterError // Impliclty cast to OSStatus in AudioConverter.h
	{
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		FormatNotSupported = 0x666d743f, // 'fmt?'
		/// <summary>To be added.</summary>
		OperationNotSupported = 0x6f703f3f, // 'op??'
		/// <summary>To be added.</summary>
		PropertyNotSupported = 0x70726f70, // 'prop'
		/// <summary>To be added.</summary>
		InvalidInputSize = 0x696e737a, // 'insz'
		/// <summary>To be added.</summary>
		InvalidOutputSize = 0x6f74737a, // 'otsz'
		/// <summary>To be added.</summary>
		UnspecifiedError = 0x77686174, // 'what'
		/// <summary>To be added.</summary>
		BadPropertySizeError = 0x2173697a, // '!siz'
		/// <summary>To be added.</summary>
		RequiresPacketDescriptionsError = 0x21706b64, // '!pkd'
		/// <summary>To be added.</summary>
		InputSampleRateOutOfRange = 0x21697372, // '!isr'
		/// <summary>To be added.</summary>
		OutputSampleRateOutOfRange = 0x216f7372, // '!osr'
		/// <summary>To be added.</summary>
		HardwareInUse = 0x68776975, // 'hwiu'
		/// <summary>To be added.</summary>
		NoHardwarePermission = 0x7065726d, // 'perm'
		/// <summary>To be added.</summary>
		AudioFormatUnsupported = 0x21646174, // '!dat' From http://lists.apple.com/archives/coreaudio-api/2009/Feb/msg00082.html
	}

	public enum AudioConverterSampleRateConverterComplexity // typedef UInt32 AudioConverterPropertyID
	{
		/// <summary>Represents lowest quality sample rate.</summary>
		Linear = 0x6c696e65, // 'line'
		/// <summary>Represents normal quality sample rate.</summary>
		Normal = 0x6e6f726d, // 'norm'
		/// <summary>Represents mastering quality sample rate.</summary>
		Mastering = 0x62617473, // 'bats'
	}

	public enum AudioConverterQuality // typedef UInt32 AudioConverterPropertyID
	{
		/// <summary>Represents maximum quality.</summary>
		Max = 0x7F,
		/// <summary>Represents high quality.</summary>
		High = 0x60,
		/// <summary>Represents medium quality.</summary>
		Medium = 0x40,
		/// <summary>Represents low quality.</summary>
		Low = 0x20,
		/// <summary>Represents minimum quality.</summary>
		Min = 0
	}

	public enum AudioConverterPrimeMethod // typedef UInt32 AudioConverterPropertyID
	{
		/// <summary>Represents primes with both leading and trailing input frames.</summary>
		Pre = 0,
		/// <summary>Represents primes with trailing input frames only.</summary>
		Normal = 1,
		/// <summary>Represents primes with both leading and trailing input frames to be silence.</summary>
		None = 2
	}

	[Flags]
#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	public enum AudioConverterOptions : uint {
		None = 0,
		Unbuffered = 1 << 16,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioConverterPrimeInfo {
		/// <summary>The number of leading input frames.</summary>
		///         <remarks>
		///         </remarks>
		public int LeadingFrames;
		/// <summary>The number of trailing input frames.</summary>
		///         <remarks>
		///         </remarks>
		public int TrailingFrames;
	}

	public delegate AudioConverterError AudioConverterComplexInputData (ref int numberDataPackets, AudioBuffers data,
		ref AudioStreamPacketDescription []? dataPacketDescription);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioConverter : DisposableObject {
#if !NET
		delegate AudioConverterError AudioConverterComplexInputDataShared (IntPtr inAudioConverter, ref int ioNumberDataPackets, IntPtr ioData,
			IntPtr outDataPacketDescription, IntPtr inUserData);
		static readonly AudioConverterComplexInputDataShared ComplexInputDataShared = FillComplexBufferShared;
#endif

		IntPtr packetDescriptions;
		int packetDescriptionSize;

		public event AudioConverterComplexInputData? InputData;

		[Preserve (Conditional = true)]
		internal AudioConverter (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		/// <summary>The size in bytes of the smallest buffer of input data.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint MinimumInputBufferSize {
			get {
				return GetUIntProperty (AudioConverterPropertyID.MinimumInputBufferSize);
			}
		}

		/// <summary>The size in bytes of the smallest buffer of output data.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint MinimumOutputBufferSize {
			get {
				return GetUIntProperty (AudioConverterPropertyID.MinimumOutputBufferSize);
			}
		}

		/// <summary>The size in bytes of the largest single packet of data in the input format.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint MaximumInputPacketSize {
			get {
				return GetUIntProperty (AudioConverterPropertyID.MaximumInputPacketSize);
			}
		}

		/// <summary>The size in bytes of the largest single packet of data in the output format.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint MaximumOutputPacketSize {
			get {
				return GetUIntProperty (AudioConverterPropertyID.MaximumOutputPacketSize);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint CalculateInputBufferSize {
			get {
				return GetUIntProperty (AudioConverterPropertyID.CalculateInputBufferSize);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint CalculateOutputBufferSize {
			get {
				return GetUIntProperty (AudioConverterPropertyID.CalculateOutputBufferSize);
			}
		}

		/// <summary>The initial sub-sample position of the sample rate converter.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public double SampleRateConverterInitialPhase {
			get {
				return GetDoubleProperty (AudioConverterPropertyID.SampleRateConverterInitialPhase);
			}
			set {
				SetProperty (AudioConverterPropertyID.SampleRateConverterInitialPhase, value);
			}
		}

		/// <summary>The sample rate converter algorithm.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioConverterSampleRateConverterComplexity SampleRateConverterComplexity {
			get {
				return (AudioConverterSampleRateConverterComplexity) GetUIntProperty (AudioConverterPropertyID.SampleRateConverterComplexity);
			}
			set {
				SetProperty (AudioConverterPropertyID.SampleRateConverterComplexity, (uint) value);
			}
		}

		/// <summary>The rendering quality of the sample rate converter.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioConverterQuality SampleRateConverterQuality {
			get {
				return (AudioConverterQuality) GetUIntProperty (AudioConverterPropertyID.SampleRateConverterQuality);
			}
			set {
				SetProperty (AudioConverterPropertyID.SampleRateConverterQuality, (uint) value);
			}
		}

		/// <summary>Rendering quality of the converter codec.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioConverterQuality CodecQuality {
			get {
				return (AudioConverterQuality) GetUIntProperty (AudioConverterPropertyID.CodecQuality);
			}
			set {
				SetProperty (AudioConverterPropertyID.CodecQuality, (uint) value);
			}
		}

		/// <summary>The priming information for converter's priming method.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioConverterPrimeMethod PrimeMethod {
			get {
				return (AudioConverterPrimeMethod) GetUIntProperty (AudioConverterPropertyID.PrimeMethod);
			}
			set {
				SetProperty (AudioConverterPropertyID.PrimeMethod, (uint) value);
			}
		}

		/// <summary>The priming method.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public unsafe AudioConverterPrimeInfo PrimeInfo {
			get {
				AudioConverterPrimeInfo value;
				var size = sizeof (AudioConverterPrimeInfo);
				var res = AudioConverterGetProperty (Handle, AudioConverterPropertyID.PrimeInfo, ref size, out value);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());

				return value;
			}
		}

		/// <summary>Input to Output channel mapping.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public int []? ChannelMap {
			get {
				return GetArray<int> (AudioConverterPropertyID.ChannelMap, sizeof (int));
			}
		}

		/// <summary>Gets or sets a magic cookie that is used for compression.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public byte []? CompressionMagicCookie {
			get {
				int size;
				bool writable;
				if (AudioConverterGetPropertyInfo (Handle, AudioConverterPropertyID.CompressionMagicCookie, out size, out writable) != AudioConverterError.None)
					return null;

				var cookie = new byte [size];
				if (AudioConverterGetProperty (Handle, AudioConverterPropertyID.CompressionMagicCookie, ref size, cookie) != AudioConverterError.None)
					return null;

				return cookie;
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				var res = AudioConverterSetProperty (Handle, AudioConverterPropertyID.CompressionMagicCookie, value.Length, value);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());
			}
		}

		/// <summary>Gets or sets a magic cookie that is used for decompression.</summary>
		///         <value>
		///         </value>
		///         <remarks>If the audio data format has a magic cookie associated with it, you must add this information to appropriately decompress the data.</remarks>
		public byte []? DecompressionMagicCookie {
			get {
				int size;
				bool writable;
				if (AudioConverterGetPropertyInfo (Handle, AudioConverterPropertyID.DecompressionMagicCookie, out size, out writable) != AudioConverterError.None)
					return null;

				var cookie = new byte [size];
				if (AudioConverterGetProperty (Handle, AudioConverterPropertyID.DecompressionMagicCookie, ref size, cookie) != AudioConverterError.None)
					return null;

				return cookie;
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				var res = AudioConverterSetProperty (Handle, AudioConverterPropertyID.DecompressionMagicCookie, value.Length, value);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());
			}
		}

		/// <summary>The number of bits per second to aim for when encoding data.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public uint EncodeBitRate {
			get {
				return GetUIntProperty (AudioConverterPropertyID.EncodeBitRate);
			}
			set {
				SetProperty (AudioConverterPropertyID.EncodeBitRate, value);
			}
		}

		/// <summary>An an output sample rate.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public double EncodeAdjustableSampleRate {
			get {
				return GetDoubleProperty (AudioConverterPropertyID.EncodeAdjustableSampleRate);
			}
			set {
				SetProperty (AudioConverterPropertyID.EncodeAdjustableSampleRate, value);
			}
		}

		/// <summary>Input audio channels layout.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioChannelLayout? InputChannelLayout {
			get {
				int size;
				bool writable;
				if (AudioConverterGetPropertyInfo (Handle, AudioConverterPropertyID.InputChannelLayout, out size, out writable) != AudioConverterError.None)
					return null;

				IntPtr ptr = Marshal.AllocHGlobal (size);
				var res = AudioConverterGetProperty (Handle, AudioConverterPropertyID.InputChannelLayout, ref size, ptr);
				var layout = res == AudioConverterError.None ? new AudioChannelLayout (ptr) : null;
				Marshal.FreeHGlobal (ptr);
				return layout;
			}
		}

		/// <summary>Output audio channels layout.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioChannelLayout? OutputChannelLayout {
			get {
				int size;
				bool writable;
				if (AudioConverterGetPropertyInfo (Handle, AudioConverterPropertyID.OutputChannelLayout, out size, out writable) != AudioConverterError.None)
					return null;

				IntPtr ptr = Marshal.AllocHGlobal (size);
				var res = AudioConverterGetProperty (Handle, AudioConverterPropertyID.OutputChannelLayout, ref size, ptr);
				var layout = res == AudioConverterError.None ? new AudioChannelLayout (ptr) : null;
				Marshal.FreeHGlobal (ptr);
				return layout;
			}
		}

		/// <summary>All applicable bit rates based on current settings.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioValueRange []? ApplicableEncodeBitRates {
			get {
				return GetAudioValueRange (AudioConverterPropertyID.ApplicableEncodeBitRates);
			}
		}

		/// <summary>All available bit rates for the input format.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioValueRange []? AvailableEncodeBitRates {
			get {
				return GetAudioValueRange (AudioConverterPropertyID.AvailableEncodeBitRates);
			}
		}

		/// <summary>All applicable sample rates based on current settings.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioValueRange []? ApplicableEncodeSampleRates {
			get {
				return GetAudioValueRange (AudioConverterPropertyID.ApplicableEncodeSampleRates);
			}
		}

		/// <summary>All applicable sample rates based on current settings.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioValueRange []? AvailableEncodeSampleRates {
			get {
				return GetAudioValueRange (AudioConverterPropertyID.AvailableEncodeSampleRates);
			}
		}

		/// <summary>All audio channel layouts for the input format.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public AudioChannelLayoutTag []? AvailableEncodeChannelLayoutTags {
			get {
				return GetArray<AudioChannelLayoutTag> (AudioConverterPropertyID.AvailableEncodeChannelLayoutTags, sizeof (AudioChannelLayoutTag));
			}
		}

		/// <summary>Completely filled output audio description.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property can be used to obtain converter filled <see cref="T:AudioToolbox.AudioStreamBasicDescription" /> for output audio stream.</remarks>
		public unsafe AudioStreamBasicDescription CurrentOutputStreamDescription {
			get {
				int size;
				bool writable;
				var res = AudioConverterGetPropertyInfo (Handle, AudioConverterPropertyID.CurrentOutputStreamDescription, out size, out writable);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());

				IntPtr ptr = Marshal.AllocHGlobal (size);
				res = AudioConverterGetProperty (Handle, AudioConverterPropertyID.CurrentOutputStreamDescription, ref size, ptr);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());

				var asbd = *(AudioStreamBasicDescription*) ptr;
				Marshal.FreeHGlobal (ptr);
				return asbd;
			}
		}

		/// <summary>Completely filled input audio description.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property can be used to obtain converter filled <see cref="T:AudioToolbox.AudioStreamBasicDescription" /> for input audio stream.</remarks>
		public unsafe AudioStreamBasicDescription CurrentInputStreamDescription {
			get {
				int size;
				bool writable;
				var res = AudioConverterGetPropertyInfo (Handle, AudioConverterPropertyID.CurrentInputStreamDescription, out size, out writable);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());

				IntPtr ptr = Marshal.AllocHGlobal (size);
				res = AudioConverterGetProperty (Handle, AudioConverterPropertyID.CurrentInputStreamDescription, ref size, ptr);
				if (res != AudioConverterError.None)
					throw new ArgumentException (res.ToString ());

				var asbd = *(AudioStreamBasicDescription*) ptr;
				Marshal.FreeHGlobal (ptr);
				return asbd;
			}
		}

		/// <summary>The source bit depth to preserve.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public int BitDepthHint {
			get {
				return (int) GetUIntProperty (AudioConverterPropertyID.PropertyBitDepthHint);
			}
			set {
				SetProperty (AudioConverterPropertyID.PropertyBitDepthHint, value);
			}
		}

		/// <summary>All the data formats produced by the converter encoder.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public unsafe AudioFormat []? FormatList {
			get {
				return GetArray<AudioFormat> (AudioConverterPropertyID.PropertyFormatList, sizeof (AudioFormat));
			}
		}

#if !MONOMAC
		/// <summary>The underlying codec supports resumption following an interruption.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public bool CanResumeFromInterruption {
			get {
				return GetUIntProperty (AudioConverterPropertyID.CanResumeFromInterruption) != 0;
			}
		}
#endif

		public static AudioConverter? Create (AudioStreamBasicDescription sourceFormat, AudioStreamBasicDescription destinationFormat)
		{
			AudioConverterError res;
			return Create (sourceFormat, destinationFormat, out res);
		}

		public static AudioConverter? Create (AudioStreamBasicDescription sourceFormat, AudioStreamBasicDescription destinationFormat, out AudioConverterError error)
		{
			IntPtr ptr = new IntPtr ();
			unsafe {
				error = AudioConverterNew (&sourceFormat, &destinationFormat, &ptr);
			}
			if (error != AudioConverterError.None)
				return null;

			return new AudioConverter (ptr, true);
		}

		public static AudioConverter? Create (AudioStreamBasicDescription sourceFormat, AudioStreamBasicDescription destinationFormat, AudioClassDescription [] descriptions)
		{
			if (descriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptions));

			IntPtr ptr = new IntPtr ();
			unsafe {
				fixed (AudioClassDescription* descriptionsPtr = descriptions) {
					var res = AudioConverterNewSpecific (&sourceFormat, &destinationFormat, descriptions.Length, descriptionsPtr, &ptr);
					if (res != AudioConverterError.None)
						return null;
				}
			}

			return new AudioConverter (ptr, true);
		}

		/// <summary>Create a new AudioConverter with the specified options.</summary>
		/// <param name="sourceFormat">The format of the source audio to be converted.</param>
		/// <param name="destinationFormat">The format to convert the source audio to.</param>
		/// <param name="options">Any <see cref="AudioConverterOptions" /> to use.</param>
		/// <param name="error">In case of failure, will contain the error code for the failure. Otherwise the value <see cref="AudioConverterError.None" /> will be returned.</param>
		/// <returns>A new AudioConverter instance, or null in case of failure.</returns>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public static AudioConverter? Create (AudioStreamBasicDescription sourceFormat, AudioStreamBasicDescription destinationFormat, AudioConverterOptions options, out AudioConverterError error)
		{
			IntPtr ptr = default (IntPtr);
			unsafe {
				error = AudioConverterNewWithOptions (&sourceFormat, &destinationFormat, options, &ptr);
			}
			if (error != AudioConverterError.None)
				return null;
			return new AudioConverter (ptr, true);
		}

		/// <summary>Create a new AudioConverter with the specified options.</summary>
		/// <param name="sourceFormat">The format of the source audio to be converted.</param>
		/// <param name="destinationFormat">The format to convert the source audio to.</param>
		/// <param name="options">Any <see cref="AudioConverterOptions" /> to use.</param>
		/// <returns>A new AudioConverter instance, or null in case of failure.</returns>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public static AudioConverter? Create (AudioStreamBasicDescription sourceFormat, AudioStreamBasicDescription destinationFormat, AudioConverterOptions options)
		{
			return Create (sourceFormat, destinationFormat, options, out var _);
		}

		/// <summary>All valid converter input formats.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public static AudioFormatType []? DecodeFormats {
			get {
				return GetFormats (AudioFormatProperty.DecodeFormatIDs);
			}
		}

		/// <summary>All valid converter output formats.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public static AudioFormatType []? EncodeFormats {
			get {
				return GetFormats (AudioFormatProperty.EncodeFormatIDs);
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns)
				AudioConverterDispose (Handle);

			if (packetDescriptions != IntPtr.Zero) {
				Marshal.FreeHGlobal (packetDescriptions);
				packetDescriptions = IntPtr.Zero;
			}

			base.Dispose (disposing);
		}

		public AudioConverterError ConvertBuffer (byte [] input, byte [] output)
		{
			if (input is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (input));
			if (output is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (output));

			int outSize = output.Length;
			unsafe {
				fixed (byte* inputPtr = input) {
					fixed (byte* outputPtr = output) {
						return AudioConverterConvertBuffer (Handle, input.Length, inputPtr, &outSize, outputPtr);
					}
				}
			}
		}

		public AudioConverterError ConvertComplexBuffer (int numberPCMFrames, AudioBuffers inputData, AudioBuffers outputData)
		{
			if (inputData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (inputData));
			if (outputData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputData));

			return AudioConverterConvertComplexBuffer (Handle, numberPCMFrames, (IntPtr) inputData, (IntPtr) outputData);
		}

		public AudioConverterError FillComplexBuffer (ref int outputDataPacketSize,
			AudioBuffers outputData, AudioStreamPacketDescription [] packetDescription, AudioConverterComplexInputData newInputDataHandler)
		{
			if (outputData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputData));

			if (newInputDataHandler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newInputDataHandler));

			return FillComplexBuffer (ref outputDataPacketSize, outputData, packetDescription, new Tuple<AudioConverter, AudioConverterComplexInputData?> (this, newInputDataHandler));
		}

		public AudioConverterError FillComplexBuffer (ref int outputDataPacketSize,
			AudioBuffers outputData, AudioStreamPacketDescription [] packetDescription)
		{
			if (outputData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputData));

			return FillComplexBuffer (ref outputDataPacketSize, outputData, packetDescription, new Tuple<AudioConverter, AudioConverterComplexInputData?> (this, null));
		}

		AudioConverterError FillComplexBuffer (ref int outputDataPacketSize,
			AudioBuffers outputData, AudioStreamPacketDescription [] packetDescription, Tuple<AudioConverter, AudioConverterComplexInputData?> instanceData)
		{
			var this_handle = GCHandle.Alloc (instanceData);

			try {
				var this_ptr = GCHandle.ToIntPtr (this_handle);
#if NET
				unsafe {
					var packetSize = outputDataPacketSize;
					int* packetSizePtr = &packetSize;
					if (packetDescription is null) {
						var returnOne = AudioConverterFillComplexBuffer (Handle, &FillComplexBufferShared, this_ptr, (IntPtr) packetSizePtr, (IntPtr) outputData, IntPtr.Zero);
						outputDataPacketSize = packetSize;
						return returnOne;
					}

					fixed (AudioStreamPacketDescription* pdesc = packetDescription) {
						var returnTwo = AudioConverterFillComplexBuffer (Handle, &FillComplexBufferShared, this_ptr, (IntPtr) packetSizePtr, (IntPtr) outputData, (IntPtr) pdesc);
						outputDataPacketSize = packetSize;
						return returnTwo;
					}
				}
#else
				if (packetDescription is null)
					return AudioConverterFillComplexBuffer (Handle, ComplexInputDataShared, this_ptr, ref outputDataPacketSize, (IntPtr) outputData, IntPtr.Zero);

				unsafe {
					fixed (AudioStreamPacketDescription* pdesc = packetDescription) {
						return AudioConverterFillComplexBuffer (Handle, ComplexInputDataShared, this_ptr, ref outputDataPacketSize, (IntPtr) outputData, (IntPtr) pdesc);
					}
				}
#endif
			} finally {
				this_handle.Free ();
			}
		}

		//
		// outDataPacketDescription should be `ref IntPtr' but using IntPtr we get easier access to pointer address
		//
#if NET
		[UnmanagedCallersOnly]
		static AudioConverterError FillComplexBufferShared (IntPtr inAudioConverter, IntPtr ioNumberDataPacketsPtr, IntPtr ioData,
															IntPtr outDataPacketDescription, IntPtr inUserData)
#else
		[MonoPInvokeCallback (typeof (AudioConverterComplexInputDataShared))]
		static AudioConverterError FillComplexBufferShared (IntPtr inAudioConverter, ref int ioNumberDataPackets, IntPtr ioData,
															IntPtr outDataPacketDescription, IntPtr inUserData)
#endif
		{
			var handler = GCHandle.FromIntPtr (inUserData);
			var instanceData = handler.Target as Tuple<AudioConverter, AudioConverterComplexInputData?>;

			if (instanceData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (instanceData));

			var inst = instanceData.Item1;
			var callback = instanceData.Item2;

			// Invoke event handler with an argument
			// since callback is not provided, must come from the old FillComplexBuffer call
			if (callback is null && inst.InputData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException ("InputData");
			// Check if subscribed to event and provided a callback, error out if true
			else if (callback is not null && inst.InputData is not null)
				throw new InvalidOperationException ("Please either only subscribe to InputData event or provide newInputDataHandler in FillComplexBuffer, using both is unsuported.");

			using (var buffers = new AudioBuffers (ioData)) {
				//
				// Callback is supposed to fill outDataPacketDescription when outDataPacketDescription is not NULL
				// Using 0-size array as marker because the size of pre-allocated memory is not known
				//
				var data = outDataPacketDescription == IntPtr.Zero ? null : new AudioStreamPacketDescription [0];
#if NET
				// tricky - this in !NET this is an argument
				// in NET it's a local so all the other code
				// flows
				var ioNumberDataPackets = Marshal.ReadInt32 (ioNumberDataPacketsPtr);
				var res = inst.InputData is not null ?
					inst.InputData (ref ioNumberDataPackets, buffers, ref data) :
					callback! (ref ioNumberDataPackets, buffers, ref data);
				Marshal.WriteInt32 (ioNumberDataPacketsPtr, ioNumberDataPackets);
#else
				var res = inst.InputData is not null ?
					inst.InputData (ref ioNumberDataPackets, buffers, ref data) :
					callback! (ref ioNumberDataPackets, buffers, ref data);
#endif

				if (outDataPacketDescription != IntPtr.Zero) {
					if (ioNumberDataPackets > 0) {
						if (data is null || data.Length == 0)
							throw new ArgumentException ("ref argument outDataPacketDescription has to be set");

						//
						// Apple doc says the output buffer has to be pre-allocated using last argument to 
						// AudioConverterFillComplexBuffer but even if NULL is passed and convertor requires
						// packet description outDataPacketDescription is not NULL and it such case we will
						// write at some unknown pointer location, similar situation happens when initialization
						// size does not match data size
						//
						int size = Marshal.SizeOf<AudioStreamPacketDescription> ();
						// Clear our buffer if it's not big enough
						if (inst.packetDescriptionSize < data.Length && inst.packetDescriptions != IntPtr.Zero) {
							Marshal.FreeHGlobal (inst.packetDescriptions);
							inst.packetDescriptions = IntPtr.Zero;
						}
						// Create a new buffer if we don't already have one
						if (inst.packetDescriptions == IntPtr.Zero) {
							inst.packetDescriptionSize = data.Length;
							inst.packetDescriptions = Marshal.AllocHGlobal (data.Length * size);
						}
						unsafe {
							fixed (void* source = data) {
								Buffer.MemoryCopy (source, (void*) inst.packetDescriptions, inst.packetDescriptionSize * size, data.Length * size);
							}
						}
						Marshal.WriteIntPtr (outDataPacketDescription, inst.packetDescriptions);
					} else {
						Marshal.WriteIntPtr (outDataPacketDescription, IntPtr.Zero);
					}
				}

				return res;
			}
		}

#if NET
		public delegate void PrepareCompletionCallback (AudioConverterError status);

		[UnmanagedCallersOnly]
		static void PrepareCompletionHandler (IntPtr block, int status)
		{
			var del = BlockLiteral.GetTarget<PrepareCompletionCallback> (block);
			if (del is not null)
				del ((AudioConverterError) status);
		}

		/// <summary>Optimizes any subsequent creation of audio converters in this process.</summary>
		/// <param name="flags">Reserved; always pass 0.</param>
		/// <param name="ioReserved">Reserved; always pass IntPtr.Zero.</param>
		/// <param name="completionCallback">Optional callback to invoke when the preparation is complete.</param>
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe static void Prepare (uint flags = 0, IntPtr ioReserved = default (IntPtr), PrepareCompletionCallback? completionCallback = null)
		{
			if (completionCallback is null) {
				AudioConverterPrepare (flags, ioReserved, null);
			} else {
				delegate* unmanaged<IntPtr, int, void> trampoline = &PrepareCompletionHandler;
				using var block = new BlockLiteral (trampoline, completionCallback, typeof (AudioConverter), nameof (PrepareCompletionHandler));
				AudioConverterPrepare (flags, ioReserved, &block);
			}
		}

		/// <summary>Optimizes any subsequent creation of audio converters in this process.</summary>
		/// <param name="completionCallback">Callback to invoke when the preparation is complete.</param>
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		public unsafe static void Prepare (PrepareCompletionCallback completionCallback)
		{
			Prepare (0, IntPtr.Zero, completionCallback);
		}
#endif // NET

		public AudioConverterError Reset ()
		{
			return AudioConverterReset (Handle);
		}

		unsafe static AudioFormatType []? GetFormats (AudioFormatProperty prop)
		{
			int size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (prop, 0, null, &size) != 0)
				return null;

			var elementSize = sizeof (AudioFormatType);
			var elementCount = size / elementSize;
			var data = new AudioFormatType [elementCount];
			fixed (AudioFormatType* ptr = data) {
				var res = AudioFormatPropertyNative.AudioFormatGetProperty (prop, 0, null, &size, (IntPtr) ptr);
				if (res != 0)
					return null;

				elementCount = size / elementSize;
				Array.Resize (ref data, elementCount);
				return data;
			}
		}

		uint GetUIntProperty (AudioConverterPropertyID propertyID)
		{
			uint value;
			var size = sizeof (uint);
			var res = AudioConverterGetProperty (Handle, propertyID, ref size, out value);
			if (res != AudioConverterError.None)
				throw new ArgumentException (res.ToString ());

			return value;
		}

		double GetDoubleProperty (AudioConverterPropertyID propertyID)
		{
			double value;
			var size = sizeof (double);
			var res = AudioConverterGetProperty (Handle, propertyID, ref size, out value);
			if (res != AudioConverterError.None)
				throw new ArgumentException (res.ToString ());

			return value;
		}

		unsafe AudioValueRange []? GetAudioValueRange (AudioConverterPropertyID prop)
		{
			return GetArray<AudioValueRange> (prop, sizeof (AudioValueRange));
		}

		unsafe T []? GetArray<T> (AudioConverterPropertyID prop, int elementSize) where T : unmanaged
		{
			int size;
			bool writable;
			if (AudioConverterGetPropertyInfo (Handle, prop, out size, out writable) != AudioConverterError.None)
				return null;

			if (size == 0)
				return Array.Empty<T> ();

			var data = new T [size / elementSize];

			fixed (T* ptr = data) {
				var res = AudioConverterGetProperty (Handle, prop, ref size, (IntPtr) ptr);
				if (res != 0)
					return null;

				Array.Resize (ref data, size / elementSize);
				return data;
			}
		}

		void SetProperty (AudioConverterPropertyID propertyID, uint value)
		{
			var res = AudioConverterSetProperty (Handle, propertyID, sizeof (uint), ref value);
			if (res != AudioConverterError.None)
				throw new ArgumentException (res.ToString ());
		}

		void SetProperty (AudioConverterPropertyID propertyID, int value)
		{
			var res = AudioConverterSetProperty (Handle, propertyID, sizeof (int), ref value);
			if (res != AudioConverterError.None)
				throw new ArgumentException (res.ToString ());
		}

		void SetProperty (AudioConverterPropertyID propertyID, double value)
		{
			var res = AudioConverterSetProperty (Handle, propertyID, sizeof (double), ref value);
			if (res != AudioConverterError.None)
				throw new ArgumentException (res.ToString ());
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterNew (AudioStreamBasicDescription* inSourceFormat, AudioStreamBasicDescription* inDestinationFormat, IntPtr* outAudioConverter);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterNewSpecific (AudioStreamBasicDescription* inSourceFormat, AudioStreamBasicDescription* inDestinationFormat,
			int inNumberClassDescriptions, AudioClassDescription* inClassDescriptions, IntPtr* outAudioConverter);

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern void AudioConverterPrepare (uint inFlags, IntPtr ioReserved, BlockLiteral* block);

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern /* OSStatus */ AudioConverterError AudioConverterNewWithOptions (
				/* const AudioStreamBasicDescription * */ AudioStreamBasicDescription* inSourceFormat,
				/* const AudioStreamBasicDescription * */ AudioStreamBasicDescription* inDestinationFormat,
				/* AudioConverterOptions */ AudioConverterOptions inOptions,
				/* AudioConverterRef __nullable * __nonnull */ IntPtr* outAudioConverter);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioConverterError AudioConverterDispose (IntPtr inAudioConverter);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioConverterError AudioConverterReset (IntPtr inAudioConverter);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioConverterError AudioConverterConvertComplexBuffer (IntPtr inAudioConverter, int inNumberPCMFrames,
			IntPtr inInputData, IntPtr outOutputData);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int* ioPropertyDataSize, uint* outPropertyData);

		unsafe static AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, ref int ioPropertyDataSize, out uint outPropertyData)
		{
			outPropertyData = default (uint);
			return AudioConverterGetProperty (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref ioPropertyDataSize), (uint*) Unsafe.AsPointer<uint> (ref outPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int* ioPropertyDataSize, int* outPropertyData);

		unsafe static AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, ref int ioPropertyDataSize, out int outPropertyData)
		{
			outPropertyData = default (int);
			return AudioConverterGetProperty (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref ioPropertyDataSize), (int*) Unsafe.AsPointer<int> (ref outPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int* ioPropertyDataSize, double* outPropertyData);

		unsafe static AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, ref int ioPropertyDataSize, out double outPropertyData)
		{
			outPropertyData = default (double);
			return AudioConverterGetProperty (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref ioPropertyDataSize), (double*) Unsafe.AsPointer<double> (ref outPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int* ioPropertyDataSize, byte* outPropertyData);

		unsafe static AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, ref int ioPropertyDataSize, byte [] outPropertyData)
		{
			fixed (byte* outPropertyDataPtr = outPropertyData)
				return AudioConverterGetProperty (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref ioPropertyDataSize), outPropertyDataPtr);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int* ioPropertyDataSize, AudioConverterPrimeInfo* outPropertyData);

		unsafe static AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, ref int ioPropertyDataSize, out AudioConverterPrimeInfo outPropertyData)
		{
			outPropertyData = default (AudioConverterPrimeInfo);
			return AudioConverterGetProperty (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref ioPropertyDataSize), (AudioConverterPrimeInfo*) Unsafe.AsPointer<AudioConverterPrimeInfo> (ref outPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int* ioPropertyDataSize, IntPtr outPropertyData);

		unsafe static AudioConverterError AudioConverterGetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, ref int ioPropertyDataSize, IntPtr outPropertyData)
		{
			return AudioConverterGetProperty (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref ioPropertyDataSize), outPropertyData);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterGetPropertyInfo (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, int* outSize, byte* outWritable);

		static AudioConverterError AudioConverterGetPropertyInfo (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, out int outSize, out bool outWritable)
		{
			byte writable = 0;
			outSize = 0;
			AudioConverterError rv;
			unsafe {
				rv = AudioConverterGetPropertyInfo (inAudioConverter, inPropertyID, (int*) Unsafe.AsPointer<int> (ref outSize), &writable);
			}
			outWritable = writable != 0;
			return rv;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int inPropertyDataSize, uint* inPropertyData);

		unsafe static AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int inPropertyDataSize, ref uint inPropertyData)
		{
			return AudioConverterSetProperty (inAudioConverter, inPropertyID, inPropertyDataSize, (uint*) Unsafe.AsPointer<uint> (ref inPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int inPropertyDataSize, int* inPropertyData);

		unsafe static AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int inPropertyDataSize, ref int inPropertyData)
		{
			return AudioConverterSetProperty (inAudioConverter, inPropertyID, inPropertyDataSize, (int*) Unsafe.AsPointer<int> (ref inPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int inPropertyDataSize, double* inPropertyData);

		unsafe static AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, int inPropertyDataSize, ref double inPropertyData)
		{
			return AudioConverterSetProperty (inAudioConverter, inPropertyID, inPropertyDataSize, (double*) Unsafe.AsPointer<double> (ref inPropertyData));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID,
			int inPropertyDataSize, byte* inPropertyData);

		unsafe static AudioConverterError AudioConverterSetProperty (IntPtr inAudioConverter, AudioConverterPropertyID inPropertyID, int inPropertyDataSize, byte [] inPropertyData)
		{
			fixed (byte* inPropertyDataPtr = inPropertyData)
				return AudioConverterSetProperty (inAudioConverter, inPropertyID, inPropertyDataSize, inPropertyDataPtr);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AudioConverterError AudioConverterConvertBuffer (IntPtr inAudioConverter, int inInputDataSize, byte* inInputData,
			int* ioOutputDataSize, byte* outOutputData);

		[DllImport (Constants.AudioToolboxLibrary)]
#if NET
		static unsafe extern AudioConverterError AudioConverterFillComplexBuffer (IntPtr inAudioConverter,
			delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, AudioConverterError> inInputDataProc, IntPtr inInputDataProcUserData,
			IntPtr ioOutputDataPacketSize, IntPtr outOutputData,
			IntPtr outPacketDescription);
#else
		static extern AudioConverterError AudioConverterFillComplexBuffer (IntPtr inAudioConverter,
			AudioConverterComplexInputDataShared inInputDataProc, IntPtr inInputDataProcUserData,
			ref int ioOutputDataPacketSize, IntPtr outOutputData,
			IntPtr outPacketDescription);
#endif
	}

	enum AudioConverterPropertyID // typedef UInt32 AudioConverterPropertyID
	{
		MinimumInputBufferSize = 0x6d696273, // 'mibs'
		MinimumOutputBufferSize = 0x6d6f6273, // 'mobs'
											  // Deprecated
											  // MaximumInputBufferSize		= 0x78696273, // 'xibs'
		MaximumInputPacketSize = 0x78697073, // 'xips'
		MaximumOutputPacketSize = 0x786f7073, // 'xops'
		CalculateInputBufferSize = 0x63696273, // 'cibs'
		CalculateOutputBufferSize = 0x636f6273, // 'cobs'

		// TODO: Format specific
		// InputCodecParameters         = 'icdp'
		// OutputCodecParameters        = 'ocdp'

		// Deprecated
		// SampleRateConverterAlgorithm = 'srci'
		SampleRateConverterComplexity = 0x73726361, // 'srca'
		SampleRateConverterQuality = 0x73726371, // 'srcq'
		SampleRateConverterInitialPhase = 0x73726370, // 'srcp'
		CodecQuality = 0x63647175, // 'cdqu'
		PrimeMethod = 0x70726d6d, // 'prmm'
		PrimeInfo = 0x7072696d, // 'prim'
		ChannelMap = 0x63686d70, // 'chmp'
		DecompressionMagicCookie = 0x646d6763, // 'dmgc'
		CompressionMagicCookie = 0x636d6763, // 'cmgc'
		EncodeBitRate = 0x62726174, // 'brat'
		EncodeAdjustableSampleRate = 0x616a7372, // 'ajsr'
		InputChannelLayout = 0x69636c20, // 'icl '
		OutputChannelLayout = 0x6f636c20, // 'ocl '
		ApplicableEncodeBitRates = 0x61656272, // 'aebr'
		AvailableEncodeBitRates = 0x76656272, // 'vebr'
		ApplicableEncodeSampleRates = 0x61657372, // 'aesr'
		AvailableEncodeSampleRates = 0x76657372, // 'vesr'
		AvailableEncodeChannelLayoutTags = 0x6165636c, // 'aecl'
		CurrentOutputStreamDescription = 0x61636f64, // 'acod'
		CurrentInputStreamDescription = 0x61636964, // 'acid'
		PropertySettings = 0x61637073, // 'acps'	// TODO
		PropertyBitDepthHint = 0x61636264, // 'acbd'
		PropertyFormatList = 0x666c7374, // 'flst'
		CanResumeFromInterruption = 0x63726669, // 'crfi'
	}
}
