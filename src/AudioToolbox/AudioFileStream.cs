// 
// AudioFile.cs:
//
// Authors:
//    Miguel de Icaza (miguel@novell.com)
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2013 Xamarin Inc.
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
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

using OSStatus = System.Int32;
using AudioFileStreamID = System.IntPtr;
using System.Runtime.Versioning;

namespace AudioToolbox {

	[Flags]
	public enum AudioFileStreamPropertyFlag { // UInt32 in AudioFileStream_PropertyListenerProc
		PropertyIsCached = 1,
		CacheProperty = 2,
	}

	public enum AudioFileStreamStatus { // Implictly cast to OSType
		Ok = 0,
		UnsupportedFileType = 0x7479703f,
		UnsupportedDataFormat = 0x666d743f,
		UnsupportedProperty = 0x7074793f,
		BadPropertySize = 0x2173697a,
		NotOptimized = 0x6f70746d,
		InvalidPacketOffset = 0x70636b3f,
		InvalidFile = 0x6474613f,
		ValueUnknown = 0x756e6b3f,
		DataUnavailable = 0x6d6f7265,
		IllegalOperation = 0x6e6f7065,
		UnspecifiedError = 0x7768743f,
		DiscontinuityCantRecover = 0x64736321,
	}

	public enum AudioFileStreamProperty { // UInt32 AudioFileStreamPropertyID
		ReadyToProducePackets = 0x72656479,
		FileFormat = 0x66666d74,
		DataFormat = 0x64666d74,
		FormatList = 0x666c7374,
		MagicCookieData = 0x6d676963,
		AudioDataByteCount = 0x62636e74,
		AudioDataPacketCount = 0x70636e74,
		MaximumPacketSize = 0x70737a65,
		DataOffset = 0x646f6666,
		ChannelLayout = 0x636d6170,
		PacketToFrame = 0x706b6672,
		FrameToPacket = 0x6672706b,
		PacketToByte = 0x706b6279,
		ByteToPacket = 0x6279706b,
		PacketTableInfo = 0x706e666f,
		PacketSizeUpperBound = 0x706b7562,
		AverageBytesPerPacket = 0x61627070,
		BitRate = 0x62726174,
		InfoDictionary = 0x696e666f,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class PropertyFoundEventArgs : EventArgs {
		public PropertyFoundEventArgs (AudioFileStreamProperty propertyID, AudioFileStreamPropertyFlag ioFlags)
		{
			Property = propertyID;
			Flags = ioFlags;
		}

		public AudioFileStreamProperty Property { get; private set; }
		public AudioFileStreamPropertyFlag Flags { get; set; }

		public override string ToString ()
		{
			return String.Format ("AudioFileStreamProperty ({0})", Property);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class PacketReceivedEventArgs : EventArgs {
		public PacketReceivedEventArgs (int numberOfBytes, IntPtr inputData, AudioStreamPacketDescription []? packetDescriptions)
		{
			this.Bytes = numberOfBytes;
			this.InputData = inputData;
			this.PacketDescriptions = packetDescriptions;
		}
		public int Bytes { get; private set; }
		public IntPtr InputData { get; private set; }
		public AudioStreamPacketDescription []? PacketDescriptions { get; private set; }

		public override string ToString ()
		{
			return String.Format ("Packet (Bytes={0} InputData={1} PacketDescriptions={2}", Bytes, InputData, PacketDescriptions?.Length ?? -1);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioFileStream : IDisposable {
		IntPtr handle;
		GCHandle gch;

		~AudioFileStream ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public void Close ()
		{
			Dispose ();
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				if (gch.IsAllocated)
					gch.Free ();
			}
			if (handle != IntPtr.Zero) {
				AudioFileStreamClose (handle);
				handle = IntPtr.Zero;
			}
		}

		delegate void AudioFileStream_PropertyListenerProc (IntPtr clientData,
								   AudioFileStreamID audioFileStream,
								   AudioFileStreamProperty propertyID,
								   ref AudioFileStreamPropertyFlag ioFlags);

		delegate void AudioFileStream_PacketsProc (IntPtr clientData,
							   int numberBytes,
							   int numberPackets,
							   IntPtr inputData,
							   IntPtr packetDescriptions);

#if NET
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static unsafe OSStatus AudioFileStreamOpen (
			IntPtr clientData,
			delegate* unmanaged<IntPtr, AudioFileStreamID, AudioFileStreamProperty, AudioFileStreamPropertyFlag*, void> propertyListenerProc,
			delegate* unmanaged<IntPtr, int, int, IntPtr, IntPtr, void> packetsProc,
			AudioFileType fileTypeHint,
			IntPtr* file_id);
#else
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileStreamOpen (
			IntPtr clientData,
			AudioFileStream_PropertyListenerProc propertyListenerProc,
			AudioFileStream_PacketsProc packetsProc,
			AudioFileType fileTypeHint,
			out IntPtr file_id);

		static readonly AudioFileStream_PacketsProc dInPackets = InPackets;
		static readonly AudioFileStream_PropertyListenerProc dPropertyListener = PropertyListener;
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (AudioFileStream_PacketsProc))]
#endif
		static void InPackets (IntPtr clientData, int numberBytes, int numberPackets, IntPtr inputData, IntPtr packetDescriptions)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var afs = handle.Target as AudioFileStream;

			var desc = AudioFile.PacketDescriptionFrom (numberPackets, packetDescriptions);
			afs!.OnPacketDecoded (numberBytes, inputData, desc);
		}

		public EventHandler<PacketReceivedEventArgs>? PacketDecoded;
		protected virtual void OnPacketDecoded (int numberOfBytes, IntPtr inputData, AudioStreamPacketDescription []? packetDescriptions)
		{
			var p = PacketDecoded;
			if (p is not null)
				p (this, new PacketReceivedEventArgs (numberOfBytes, inputData, packetDescriptions));
		}

		public EventHandler<PropertyFoundEventArgs>? PropertyFound;
		protected virtual void OnPropertyFound (AudioFileStreamProperty propertyID, ref AudioFileStreamPropertyFlag ioFlags)
		{
			var p = PropertyFound;
			if (p is not null) {
				var pf = new PropertyFoundEventArgs (propertyID, ioFlags);
				p (this, pf);
				ioFlags = pf.Flags;
			}
		}

#if NET
		[UnmanagedCallersOnly]
		static unsafe void PropertyListener (IntPtr clientData, AudioFileStreamID audioFileStream, AudioFileStreamProperty propertyID, AudioFileStreamPropertyFlag* ioFlags)
#else
		[MonoPInvokeCallback (typeof (AudioFileStream_PropertyListenerProc))]
		static void PropertyListener (IntPtr clientData, AudioFileStreamID audioFileStream, AudioFileStreamProperty propertyID, ref AudioFileStreamPropertyFlag ioFlags)
#endif
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var afs = handle.Target as AudioFileStream;

#if NET
			var localFlags = *ioFlags;
			afs!.OnPropertyFound (propertyID, ref localFlags);
			*ioFlags = localFlags;
#else
			afs!.OnPropertyFound (propertyID, ref ioFlags);
#endif
		}

		public AudioFileStream (AudioFileType fileTypeHint)
		{
			IntPtr h;
			gch = GCHandle.Alloc (this);
#if NET
			var code = 0;
			unsafe {
				code = AudioFileStreamOpen (GCHandle.ToIntPtr (gch), &PropertyListener, &InPackets, fileTypeHint, &h);
			}
#else
			var code = AudioFileStreamOpen (GCHandle.ToIntPtr (gch), dPropertyListener, dInPackets, fileTypeHint, out h);
#endif
			if (code == 0) {
				handle = h;
				return;
			}
			throw new Exception (String.Format ("Unable to create AudioFileStream, code: 0x{0:x}", code));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileStreamStatus AudioFileStreamParseBytes (
			AudioFileStreamID inAudioFileStream,
			int inDataByteSize,
			IntPtr inData,
			UInt32 inFlags);

		public AudioFileStreamStatus ParseBytes (int size, IntPtr data, bool discontinuity)
		{
			if (data == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			return LastError = AudioFileStreamParseBytes (handle, size, data, discontinuity ? (uint) 1 : (uint) 0);
		}

		public AudioFileStreamStatus ParseBytes (byte [] bytes, bool discontinuity)
		{
			if (bytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));
			unsafe {
				fixed (byte* bp = bytes) {
					return LastError = AudioFileStreamParseBytes (handle, bytes.Length, (IntPtr) bp, discontinuity ? (uint) 1 : (uint) 0);
				}
			}
		}

		public AudioFileStreamStatus ParseBytes (byte [] bytes, int offset, int count, bool discontinuity)
		{
			if (bytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));
			if (offset < 0)
				throw new ArgumentException ("offset");
			if (count < 0)
				throw new ArgumentException ("count");
			if (offset + count > bytes.Length)
				throw new ArgumentException ("offset+count");

			unsafe {
				fixed (byte* bp = bytes) {
					return LastError = AudioFileStreamParseBytes (handle, count, (IntPtr) (bp + offset), discontinuity ? (uint) 1 : (uint) 0);
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileStreamStatus AudioFileStreamSeek (AudioFileStreamID inAudioFileStream,
									long inPacketOffset,
									long* outDataByteOffset,
									int* ioFlags);

		public AudioFileStreamStatus Seek (long packetOffset, out long dataByteOffset, out bool isEstimate)
		{
			int v = 0;
			dataByteOffset = 0;
			unsafe {
				LastError = AudioFileStreamSeek (handle, packetOffset, (long *) Unsafe.AsPointer<long> (ref dataByteOffset), &v);
			}
			if (LastError != AudioFileStreamStatus.Ok) {
				isEstimate = false;
			} else {
				isEstimate = (v & 1) == 1;
			}

			return LastError;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileStreamStatus AudioFileStreamGetPropertyInfo (
			AudioFileStreamID inAudioFileStream,
			AudioFileStreamProperty inPropertyID,
			int* outPropertyDataSize,
			byte* isWritable);

		static AudioFileStreamStatus AudioFileStreamGetPropertyInfo (
			AudioFileStreamID inAudioFileStream,
			AudioFileStreamProperty inPropertyID,
			out int outPropertyDataSize,
			out bool isWritable)
		{
			byte writable;
			AudioFileStreamStatus rv;
			outPropertyDataSize = 0;
			unsafe {
				rv = AudioFileStreamGetPropertyInfo (inAudioFileStream, inPropertyID, (int *) Unsafe.AsPointer<int> (ref outPropertyDataSize), &writable);
			}
			isWritable = writable != 0;
			return rv;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileStreamStatus AudioFileStreamGetProperty (
			AudioFileStreamID inAudioFileStream,
			AudioFileStreamProperty inPropertyID,
			int* ioPropertyDataSize,
			IntPtr outPropertyData);

		public bool GetProperty (AudioFileStreamProperty property, ref int dataSize, IntPtr outPropertyData)
		{
			if (outPropertyData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outPropertyData));
			unsafe {
				return AudioFileStreamGetProperty (handle, property, (int *) Unsafe.AsPointer<int> (ref dataSize), outPropertyData) == 0;
			}
		}

		public IntPtr GetProperty (AudioFileStreamProperty property, out int size)
		{
			bool writable;

			LastError = AudioFileStreamGetPropertyInfo (handle, property, out size, out writable);
			if (LastError != 0)
				return IntPtr.Zero;

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return IntPtr.Zero;

			unsafe {
				LastError = AudioFileStreamGetProperty (handle, property, (int *) Unsafe.AsPointer<int> (ref size), buffer);
			}
			if (LastError == 0)
				return buffer;
			Marshal.FreeHGlobal (buffer);
			return IntPtr.Zero;
		}

		int GetInt (AudioFileStreamProperty property)
		{
			unsafe {
				int val = 0;
				int size = 4;
				LastError = AudioFileStreamGetProperty (handle, property, &size, (IntPtr) (&val));
				if (LastError == 0)
					return val;
				return 0;
			}
		}

		double GetDouble (AudioFileStreamProperty property)
		{
			unsafe {
				double val = 0;
				int size = 8;
				LastError = AudioFileStreamGetProperty (handle, property, &size, (IntPtr) (&val));
				if (LastError == 0)
					return val;
				return 0;
			}
		}

		long GetLong (AudioFileStreamProperty property)
		{
			unsafe {
				long val = 0;
				int size = 8;
				LastError = AudioFileStreamGetProperty (handle, property, &size, (IntPtr) (&val));
				if (LastError == 0)
					return val;
				return 0;
			}
		}

		unsafe T? GetProperty<T> (AudioFileStreamProperty property) where T : struct
		{
			int size;
			bool writable;

			LastError = AudioFileStreamGetPropertyInfo (handle, property, out size, out writable);
			if (LastError != 0)
				return null;
			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return null;
			try {
				LastError = AudioFileStreamGetProperty (handle, property, &size, buffer);
				if (LastError == 0) {
					return Marshal.PtrToStructure<T> (buffer)!;
				}

				return null;
			} finally {
				Marshal.FreeHGlobal (buffer);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileStreamStatus AudioFileStreamSetProperty (
			AudioFileStreamID inAudioFileStream,
			AudioFileStreamProperty inPropertyID,
			int inPropertyDataSize,
			IntPtr inPropertyData);

		public bool SetProperty (AudioFileStreamProperty property, int dataSize, IntPtr propertyData)
		{
			if (propertyData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (propertyData));
			LastError = AudioFileStreamSetProperty (handle, property, dataSize, propertyData);
			return LastError == 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileStreamStatus AudioFileStreamClose (AudioFileStreamID inAudioFileStream);

		public bool ReadyToProducePackets {
			get {
				return GetInt (AudioFileStreamProperty.ReadyToProducePackets) == 1;
			}
		}

		public AudioFileType FileType {
			get {
				return (AudioFileType) GetInt (AudioFileStreamProperty.FileFormat);
			}
		}

		[Advice ("Use 'DataFormat' instead.")]
		public AudioStreamBasicDescription StreamBasicDescription {
			get {
				return DataFormat;
			}
		}

		public AudioStreamBasicDescription DataFormat {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileStreamProperty.DataFormat) ?? default (AudioStreamBasicDescription);
			}
		}

		public unsafe AudioFormat []? FormatList {
			get {
				int size;
				var r = GetProperty (AudioFileStreamProperty.FormatList, out size);
				if (r == IntPtr.Zero)
					return null;

				var records = (AudioFormat*) r;
				int itemSize = sizeof (AudioFormat);
				int items = size / itemSize;
				var ret = new AudioFormat [items];

				for (int i = 0; i < items; i++)
					ret [i] = records [i];

				Marshal.FreeHGlobal (r);
				return ret;
			}
		}

		public AudioFilePacketTableInfo? PacketTableInfo {
			get {
				return GetProperty<AudioFilePacketTableInfo> (AudioFileStreamProperty.PacketTableInfo);
			}
		}

		public byte [] MagicCookie {
			get {
				int size;
				var h = GetProperty (AudioFileStreamProperty.MagicCookieData, out size);
				if (h == IntPtr.Zero)
					return Array.Empty<byte> ();

				byte [] cookie = new byte [size];
				Marshal.Copy (h, cookie, 0, size);
				Marshal.FreeHGlobal (h);

				return cookie;
			}
		}

		public long DataByteCount {
			get {
				return GetLong (AudioFileStreamProperty.AudioDataByteCount);
			}
		}

		public long DataPacketCount {
			get {
				return GetLong (AudioFileStreamProperty.AudioDataPacketCount);
			}
		}

		public int MaximumPacketSize {
			get {
				return GetInt (AudioFileStreamProperty.MaximumPacketSize);
			}
		}

		public long DataOffset {
			get {
				return GetLong (AudioFileStreamProperty.DataOffset);
			}
		}

		public AudioChannelLayout? ChannelLayout {
			get {
				int size;
				var h = GetProperty (AudioFileStreamProperty.ChannelLayout, out size);
				if (h == IntPtr.Zero)
					return null;

				var layout = AudioChannelLayout.FromHandle (h);
				Marshal.FreeHGlobal (h);

				return layout;
			}
		}

		public long PacketToFrame (long packet)
		{
			AudioFramePacketTranslation buffer;
			buffer.Packet = packet;

			unsafe {
				AudioFramePacketTranslation* p = &buffer;
				int size = sizeof (AudioFramePacketTranslation);
				LastError = AudioFileStreamGetProperty (handle, AudioFileStreamProperty.PacketToFrame, &size, (IntPtr) p);
				if (LastError == 0)
					return buffer.Frame;
				return -1;
			}
		}

		public long FrameToPacket (long frame, out int frameOffsetInPacket)
		{
			AudioFramePacketTranslation buffer;
			buffer.Frame = frame;

			unsafe {
				AudioFramePacketTranslation* p = &buffer;
				int size = sizeof (AudioFramePacketTranslation);
				LastError = AudioFileStreamGetProperty (handle, AudioFileStreamProperty.FrameToPacket, &size, (IntPtr) p);
				if (LastError == 0) {
					frameOffsetInPacket = buffer.FrameOffsetInPacket;
					return buffer.Packet;
				}
				frameOffsetInPacket = 0;
				return -1;
			}
		}

		public long PacketToByte (long packet, out bool isEstimate)
		{
			AudioBytePacketTranslation buffer;
			buffer.Packet = packet;

			unsafe {
				AudioBytePacketTranslation* p = &buffer;
				int size = sizeof (AudioBytePacketTranslation);
				LastError = AudioFileStreamGetProperty (handle, AudioFileStreamProperty.PacketToByte, &size, (IntPtr) p);
				if (LastError == 0) {
					isEstimate = (buffer.Flags & BytePacketTranslationFlags.IsEstimate) != 0;
					return buffer.Byte;
				}
				isEstimate = false;
				return -1;
			}
		}

		public long ByteToPacket (long byteval, out int byteOffsetInPacket, out bool isEstimate)
		{
			AudioBytePacketTranslation buffer;
			buffer.Byte = byteval;

			unsafe {
				AudioBytePacketTranslation* p = &buffer;
				int size = sizeof (AudioBytePacketTranslation);
				LastError = AudioFileStreamGetProperty (handle, AudioFileStreamProperty.ByteToPacket, &size, (IntPtr) p);
				if (LastError == 0) {
					isEstimate = (buffer.Flags & BytePacketTranslationFlags.IsEstimate) != 0;
					byteOffsetInPacket = buffer.ByteOffsetInPacket;
					return buffer.Packet;
				}
				byteOffsetInPacket = 0;
				isEstimate = false;
				return -1;
			}
		}

		public int BitRate {
			get {
				return GetInt (AudioFileStreamProperty.BitRate);
			}
		}

		public int PacketSizeUpperBound {
			get {
				return GetInt (AudioFileStreamProperty.PacketSizeUpperBound);
			}
		}

		public double AverageBytesPerPacket {
			get {
				return GetDouble (AudioFileStreamProperty.AverageBytesPerPacket);
			}
		}

		public AudioFileStreamStatus LastError { get; private set; }
	}
}
