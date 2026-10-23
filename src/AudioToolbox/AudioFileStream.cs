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

using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using CoreFoundation;

using AudioFileStreamID = System.IntPtr;

namespace AudioToolbox {

	/// <summary>An enumeration whose values flag whether a <see cref="AudioToolbox.AudioFileStreamProperty" /> is cached.</summary>
	///     <remarks>To be added.</remarks>
	[Flags]
	[NativeName ("AudioFileStreamPropertyFlags")]
	public enum AudioFileStreamPropertyFlag { // UInt32 in AudioFileStream_PropertyListenerProc
		/// <summary>To be added.</summary>
		PropertyIsCached = 1,
		/// <summary>To be added.</summary>
		CacheProperty = 2,
	}

	/// <summary>An enumeration whose values indicate the status following calls to the <see cref="AudioToolbox.AudioFileStream.Seek(System.Int64,out System.Int64,out System.Boolean)" /> or <see cref="AudioToolbox.AudioFileStream.ParseBytes(System.Byte[],System.Int32,System.Int32,System.Boolean)" /> methods.</summary>
	///     <remarks>To be added.</remarks>
	public enum AudioFileStreamStatus { // Implictly cast to OSType
		/// <summary>To be added.</summary>
		Ok = 0,
		/// <summary>To be added.</summary>
		UnsupportedFileType = 0x7479703f,
		/// <summary>To be added.</summary>
		UnsupportedDataFormat = 0x666d743f,
		/// <summary>To be added.</summary>
		UnsupportedProperty = 0x7074793f,
		/// <summary>To be added.</summary>
		BadPropertySize = 0x2173697a,
		/// <summary>To be added.</summary>
		NotOptimized = 0x6f70746d,
		/// <summary>To be added.</summary>
		InvalidPacketOffset = 0x70636b3f,
		/// <summary>To be added.</summary>
		InvalidFile = 0x6474613f,
		/// <summary>To be added.</summary>
		ValueUnknown = 0x756e6b3f,
		/// <summary>To be added.</summary>
		DataUnavailable = 0x6d6f7265,
		/// <summary>To be added.</summary>
		IllegalOperation = 0x6e6f7065,
		/// <summary>To be added.</summary>
		UnspecifiedError = 0x7768743f,
		/// <summary>To be added.</summary>
		DiscontinuityCantRecover = 0x64736321,
	}

	/// <summary>An enumeration whose values represent properties of <see cref="AudioToolbox.AudioFileStream" />.</summary>
	///     <remarks>To be added.</remarks>
	public enum AudioFileStreamProperty { // UInt32 AudioFileStreamPropertyID
		/// <summary>To be added.</summary>
		ReadyToProducePackets = 0x72656479,
		/// <summary>To be added.</summary>
		FileFormat = 0x66666d74,
		/// <summary>To be added.</summary>
		DataFormat = 0x64666d74,
		/// <summary>To be added.</summary>
		FormatList = 0x666c7374,
		/// <summary>To be added.</summary>
		MagicCookieData = 0x6d676963,
		/// <summary>To be added.</summary>
		AudioDataByteCount = 0x62636e74,
		/// <summary>To be added.</summary>
		AudioDataPacketCount = 0x70636e74,
		/// <summary>To be added.</summary>
		MaximumPacketSize = 0x70737a65,
		/// <summary>To be added.</summary>
		DataOffset = 0x646f6666,
		/// <summary>To be added.</summary>
		ChannelLayout = 0x636d6170,
		/// <summary>To be added.</summary>
		PacketToFrame = 0x706b6672,
		/// <summary>To be added.</summary>
		FrameToPacket = 0x6672706b,
		/// <summary>To be added.</summary>
		PacketToByte = 0x706b6279,
		/// <summary>To be added.</summary>
		ByteToPacket = 0x6279706b,
		/// <summary>To be added.</summary>
		PacketTableInfo = 0x706e666f,
		/// <summary>To be added.</summary>
		PacketSizeUpperBound = 0x706b7562,
		/// <summary>To be added.</summary>
		AverageBytesPerPacket = 0x61627070,
		/// <summary>To be added.</summary>
		BitRate = 0x62726174,
		/// <summary>To be added.</summary>
		InfoDictionary = 0x696e666f,
	}

	/// <summary>Provides data for the <see cref="AudioToolbox.AudioFileStream.PropertyFound" /> event.</summary>
	///     <remarks>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/StreamingAudio/">StreamingAudio</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class PropertyFoundEventArgs : EventArgs {
		/// <param name="propertyID">To be added.</param>
		///         <param name="ioFlags">To be added.</param>
		///         <summary>Initializes a new instance of the PropertyFoundEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public PropertyFoundEventArgs (AudioFileStreamProperty propertyID, AudioFileStreamPropertyFlag ioFlags)
		{
			Property = propertyID;
			Flags = ioFlags;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileStreamProperty Property { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileStreamPropertyFlag Flags { get; set; }

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public override string ToString ()
		{
			return String.Format ("AudioFileStreamProperty ({0})", Property);
		}
	}

	/// <summary>Provides data for the <see cref="AudioToolbox.AudioFileStream.PacketDecoded" /> event.</summary>
	///     <remarks>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/StreamingAudio/">StreamingAudio</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class PacketReceivedEventArgs : EventArgs {
		/// <param name="numberOfBytes">To be added.</param>
		///         <param name="inputData">To be added.</param>
		///         <param name="packetDescriptions">To be added.</param>
		///         <summary>Initializes a new instance of the PacketReceivedEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public PacketReceivedEventArgs (int numberOfBytes, IntPtr inputData, AudioStreamPacketDescription []? packetDescriptions)
		{
			this.Bytes = numberOfBytes;
			this.InputData = inputData;
			this.PacketDescriptions = packetDescriptions;
		}
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int Bytes { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public IntPtr InputData { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioStreamPacketDescription []? PacketDescriptions { get; private set; }

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public override string ToString ()
		{
			return String.Format ("Packet (Bytes={0} InputData={1} PacketDescriptions={2}", Bytes, InputData, PacketDescriptions?.Length ?? -1);
		}
	}

	/// <summary>Process partial audio files.</summary>
	///     <remarks>
	///       <para>
	/// 	You use AudioFileStream when you want to decode audio content
	/// 	that does not live in an local file or if you want to decode
	/// 	it in chunks.  New data is fed into the AudioFileStream using
	/// 	one of the ParseBytes method and decoded audio is provided on
	/// 	the PacketDecoded event (or the OnPacketDecoded virtual
	/// 	method) and information about the stream is raised on the
	/// 	PropertyFound event (or the OnPropertyFound virtual method).
	///       </para>
	///       <para>
	/// 	This can be used to parse audio files when you are streaming audio from the network for example.
	///       </para>
	///       <para>
	/// 	The methods and properties in this class update the <see cref="AudioToolbox.AudioFileStream.LastError" />
	/// 	property to track any potential errors during parsing, but
	/// 	without throwing an exception.
	///
	///       </para>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/StreamingAudio/">StreamingAudio</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioFileStream : IDisposable {
		IntPtr handle;
		GCHandle gch;

		~AudioFileStream ()
		{
			Dispose (false);
		}

		/// <summary>Releases the resources used by the AudioFileStream object.</summary>
		///         <remarks>
		///           <para>The Dispose method releases the resources used by the AudioFileStream class.</para>
		///           <para>Calling the Dispose method when the application is finished using the AudioFileStream ensures that all external resources used by this managed object are released as soon as possible.  Once developers have invoked the Dispose method, the object is no longer useful and developers should no longer make any calls to it.  For more information on releasing resources see ``Cleaning up Unmananaged Resources'' at https://msdn.microsoft.com/en-us/library/498928w2.aspx</para>
		///         </remarks>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <summary>Closes (and disposes the audio stream).</summary>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public void Close ()
		{
			Dispose ();
		}

		/// <include file="../../docs/api/AudioToolbox/AudioFileStream.xml" path="/Documentation/Docs[@DocId='M:AudioToolbox.AudioFileStream.Dispose(System.Boolean)']/*" />
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

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static unsafe OSStatus AudioFileStreamOpen (
			IntPtr clientData,
			delegate* unmanaged<IntPtr, AudioFileStreamID, AudioFileStreamProperty, AudioFileStreamPropertyFlag*, void> propertyListenerProc,
			delegate* unmanaged<IntPtr, int, int, IntPtr, IntPtr, void> packetsProc,
			AudioFileType fileTypeHint,
			IntPtr* file_id);

		[UnmanagedCallersOnly]
		static void InPackets (IntPtr clientData, int numberBytes, int numberPackets, IntPtr inputData, IntPtr packetDescriptions)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var afs = handle.Target as AudioFileStream;

			var desc = AudioFile.PacketDescriptionFrom (numberPackets, packetDescriptions);
			afs!.OnPacketDecoded (numberBytes, inputData, desc);
		}

		/// <summary>This event is raised when a packet has been decoded.</summary>
		///         <remarks>To be added.</remarks>
		public EventHandler<PacketReceivedEventArgs>? PacketDecoded;
		/// <param name="numberOfBytes">The number of bytes available in the decoded packet.</param>
		///         <param name="inputData">Pointer to the decoded data.</param>
		///         <param name="packetDescriptions">A description of the packets decoded.</param>
		///         <summary>Invoked when a packet has been decoded.</summary>
		///         <remarks>The default implementation raises the PacketDecoded event.</remarks>
		protected virtual void OnPacketDecoded (int numberOfBytes, IntPtr inputData, AudioStreamPacketDescription []? packetDescriptions)
		{
			var p = PacketDecoded;
			if (p is not null)
				p (this, new PacketReceivedEventArgs (numberOfBytes, inputData, packetDescriptions));
		}

		/// <summary>This event is raised when a property has been found on the decoded data.</summary>
		///         <remarks>The most interesting property that is raised is AudioFileStreamProperty.ReadyToProducePackets;   When this property is parsed there is enough information to create the output queue.   The MagicCookie and the StreamBasicDescription contain the information necessary to create a working instance of the OutputAudioQueue.</remarks>
		public EventHandler<PropertyFoundEventArgs>? PropertyFound;
		/// <param name="propertyID">The property that has been found.</param>
		///         <param name="ioFlags" />
		///         <summary>Invoked when a property is found.</summary>
		///         <remarks>The default implementation merely raises the PropetyFound event.</remarks>
		protected virtual void OnPropertyFound (AudioFileStreamProperty propertyID, ref AudioFileStreamPropertyFlag ioFlags)
		{
			var p = PropertyFound;
			if (p is not null) {
				var pf = new PropertyFoundEventArgs (propertyID, ioFlags);
				p (this, pf);
				ioFlags = pf.Flags;
			}
		}

		[UnmanagedCallersOnly]
		static unsafe void PropertyListener (IntPtr clientData, AudioFileStreamID audioFileStream, AudioFileStreamProperty propertyID, AudioFileStreamPropertyFlag* ioFlags)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var afs = handle.Target as AudioFileStream;

			var localFlags = *ioFlags;
			afs!.OnPropertyFound (propertyID, ref localFlags);
			*ioFlags = localFlags;
		}

		/// <param name="fileTypeHint">Hint about the audio file type.</param>
		///         <summary>Creates a new instance of this object.</summary>
		///         <remarks>To be added.</remarks>
		public AudioFileStream (AudioFileType fileTypeHint)
		{
			IntPtr h;
			gch = GCHandle.Alloc (this);
			var code = 0;
			unsafe {
				code = AudioFileStreamOpen (GCHandle.ToIntPtr (gch), &PropertyListener, &InPackets, fileTypeHint, &h);
			}
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

		/// <param name="size">The number of bytes to parse from the provided block.</param>
		///         <param name="data">A pointer to the audio data to decode.</param>
		///         <param name="discontinuity">True if this invocation to ParseBytes is contiguous to the previous one, false otherwise.</param>
		///         <summary>Parse and decode the block of data provided.</summary>
		///         <returns>Parse status.</returns>
		///         <remarks>
		///           <para>The OnPacketDecoded/OnProperty found methods are invoked as data is parsed.   If you have not subclassed this class, you can alternatively hook up to the PacketDecoded and PropertyFound events to receive parsing notifications.</para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public AudioFileStreamStatus ParseBytes (int size, IntPtr data, bool discontinuity)
		{
			if (data == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			return LastError = AudioFileStreamParseBytes (handle, size, data, discontinuity ? (uint) 1 : (uint) 0);
		}

		/// <param name="bytes">The buffer that contains the audio data to decode.</param>
		///         <param name="discontinuity">True if this invocation to ParseBytes is contiguous to the previous one, false otherwise.</param>
		///         <summary>Parse and decode the array of bytes provided.</summary>
		///         <returns>Parsing status.</returns>
		///         <remarks>
		///           <para> 
		/// 	    The OnPacketDecoded/OnProperty found methods are
		/// 	    invoked as data is parsed.  If you have not subclassed
		/// 	    this class, you can alternatively hook up to the
		/// 	    PacketDecoded and PropertyFound events to receive parsing
		/// 	    notifications.  
		/// 	  </para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <param name="bytes">Buffer containing the data.</param>
		///         <param name="offset">First byte within the array that contains the data to decode.</param>
		///         <param name="count">Number of bytes to parse.</param>
		///         <param name="discontinuity">True if this invocation to ParseBytes is contiguous to the previous one, false otherwise.</param>
		///         <summary>Parses and decode a portion of the array of bytes provided.</summary>
		///         <returns>The status from parsing the buffer.</returns>
		///         <remarks>
		///           <para>The OnPacketDecoded/OnProperty found methods are invoked as data is parsed.   If you have not subclassed this class, you can alternatively hook up to the PacketDecoded and PropertyFound events to receive parsing notifications.</para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <param name="packetOffset">The offset of the packet to map.</param>
		///         <param name="dataByteOffset">Upon return, the data byte offset in the audio file stream. </param>
		///         <param name="isEstimate">On return, the value will be true if the byte offset is an estimate.</param>
		///         <summary>Maps the absolute file offset for the specified packetOffset.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public AudioFileStreamStatus Seek (long packetOffset, out long dataByteOffset, out bool isEstimate)
		{
			int v = 0;
			dataByteOffset = 0;
			unsafe {
				fixed (long* dataByteOffsetPtr = &dataByteOffset)
					LastError = AudioFileStreamSeek (handle, packetOffset, dataByteOffsetPtr, &v);
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
				fixed (int* outPropertyDataSizePtr = &outPropertyDataSize)
					rv = AudioFileStreamGetPropertyInfo (inAudioFileStream, inPropertyID, outPropertyDataSizePtr, &writable);
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

		/// <param name="property">Property ID to fetch.</param>
		///         <param name="dataSize">The expected size of the property (must match the underlying assumption for the size).</param>
		///         <param name="outPropertyData">Must point to a buffer that can hold dataSize bytes.</param>
		///         <summary>Low-level routine used to fetch arbitrary property values from the underlying AudioFileStream object.</summary>
		///         <returns>True on success.</returns>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public bool GetProperty (AudioFileStreamProperty property, ref int dataSize, IntPtr outPropertyData)
		{
			if (outPropertyData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outPropertyData));
			unsafe {
				fixed (int* dataSizePtr = &dataSize)
					return AudioFileStreamGetProperty (handle, property, dataSizePtr, outPropertyData) == 0;
			}
		}

		/// <param name="property">Property ID to fetch.</param>
		///         <param name="size">The size in bytes of the property.</param>
		///         <summary>Low-level routine used to fetch arbitrary property values from the underlying AudioFileStream object.</summary>
		///         <returns>If the return value from this method is different that IntPtr.Zero, the value pointed to contains the value of the property.</returns>
		///         <remarks>
		///           <para>
		/// 	    This method will query the underlying AudioFileStream
		/// 	    object for the size of the specified property and allocate
		/// 	    the memory needed for it using Marshal.AllocHGlobal
		/// 	    method.   
		/// 	  </para>
		///           <para>
		/// 	    You are responsible for releasing the memory allocated by this method by calling Marshal.FreeHGlobal. 
		/// 	  </para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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
				fixed (int* sizePtr = &size)
					LastError = AudioFileStreamGetProperty (handle, property, sizePtr, buffer);
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

		unsafe T? GetProperty<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> (AudioFileStreamProperty property) where T : unmanaged
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

		/// <param name="property">The property to set.</param>
		///         <param name="dataSize">The size of the data to set.</param>
		///         <param name="propertyData">Pointer to the property data.</param>
		///         <summary>Low-level property setting API.   Use the exposed managed properties instead.</summary>
		///         <returns>true if the operation successful.</returns>
		///         <remarks>Most properties have been exposed with C# properties, there should be no need to call this directly, unless new properties are introduced that are not bound by MonoTouch.</remarks>
		public bool SetProperty (AudioFileStreamProperty property, int dataSize, IntPtr propertyData)
		{
			if (propertyData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (propertyData));
			LastError = AudioFileStreamSetProperty (handle, property, dataSize, propertyData);
			return LastError == 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileStreamStatus AudioFileStreamClose (AudioFileStreamID inAudioFileStream);

		/// <summary>Set to true once the file stream parser has read enough of the headers to be able to produce audio packets.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public bool ReadyToProducePackets {
			get {
				return GetInt (AudioFileStreamProperty.ReadyToProducePackets) == 1;
			}
		}

		/// <summary>The audio file type for the audio stream.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public AudioFileType FileType {
			get {
				return (AudioFileType) GetInt (AudioFileStreamProperty.FileFormat);
			}
		}

		/// <summary>Format of the data</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		[Advice ("Use 'DataFormat' instead.")]
		public AudioStreamBasicDescription StreamBasicDescription {
			get {
				return DataFormat;
			}
		}

		/// <summary>Format of the data (as an AudioStreamBasicDescription</summary>
		///         <value>.</value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public AudioStreamBasicDescription DataFormat {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileStreamProperty.DataFormat) ?? default (AudioStreamBasicDescription);
			}
		}

		/// <summary>List of formats supported by the audio stream.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    Some formats (like AAC) that support multiple encodings
		/// 	    will set this property to the available audio formats.
		/// 	    You would typically use one of the returned
		/// 	    AudioStreamBasicDescription descriptions to create an
		/// 	    <see cref="AudioToolbox.AudioQueue" />.
		///
		/// 	  </para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <summary>Contains information about the valid frames in the audio file stream (their start and end).</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public AudioFilePacketTableInfo? PacketTableInfo {
			get {
				return GetProperty<AudioFilePacketTableInfo> (AudioFileStreamProperty.PacketTableInfo);
			}
		}

		/// <summary>The magic cookie for this file.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    Some file formats require that the magic cookie is written
		/// 	    before data can be written, use this property to retrieve
		/// 	    the magic cookie for this file stream.
		/// 	  </para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <summary>For streams whose length is known, the number of audio byte data.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    After the audio stream header is parsed, it is possible to determine the number of bytes of audio data.  For streaming formats, this value is not specified.
		/// 	  </para>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public long DataByteCount {
			get {
				return GetLong (AudioFileStreamProperty.AudioDataByteCount);
			}
		}

		/// <summary>The number of audio packets on the audio file stream.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public long DataPacketCount {
			get {
				return GetLong (AudioFileStreamProperty.AudioDataPacketCount);
			}
		}

		/// <summary>Maximum packet size for data on the audio file stream</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public int MaximumPacketSize {
			get {
				return GetInt (AudioFileStreamProperty.MaximumPacketSize);
			}
		}

		/// <summary>Offset of the audio date from the beginning of the audio file stream.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public long DataOffset {
			get {
				return GetLong (AudioFileStreamProperty.DataOffset);
			}
		}

		/// <summary>The channel layout for the audio stream.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <param name="packet">Packet number to map.</param>
		///         <summary>Maps a packet number to an audio frame number in the audio file stream.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <param name="frame">The audio frame number.</param>
		///         <param name="frameOffsetInPacket">The frame offset in the packet.</param>
		///         <summary>Returns the packet number and the frame offset in the packet (on the out parameter) corresponding to the requested audio frame.</summary>
		///         <returns>The packet number that corresponds to the specified frame.</returns>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <param name="packet">Packet number.</param>
		///         <param name="isEstimate">On return, the value will be true if the byte offset is an estimate.</param>
		///         <summary>Maps a packet number to a byte number in the audio file stream.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <param name="byteval">The location in the file.</param>
		///         <param name="byteOffsetInPacket">Return value, byte offset within the packet.</param>
		///         <param name="isEstimate">Return value, whether the return is an estimate or not.</param>
		///         <summary>Maps a position in the file to an audio packet.</summary>
		///         <returns>The packet number that corresponds to this byte in the file.</returns>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
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

		/// <summary>The stream's bit rate in bits per second.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public int BitRate {
			get {
				return GetInt (AudioFileStreamProperty.BitRate);
			}
		}

		/// <summary>The largest possible packet size.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public int PacketSizeUpperBound {
			get {
				return GetInt (AudioFileStreamProperty.PacketSizeUpperBound);
			}
		}

		/// <summary>Average bytes per packet.   This value is precise for audio files with constant bit rates or audio files that have a packet index, otherwise it is a computed average.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This updates the <see cref="AudioToolbox.AudioFileStream.LastError" /> property.
		/// 	  </para>
		///         </remarks>
		public double AverageBytesPerPacket {
			get {
				return GetDouble (AudioFileStreamProperty.AverageBytesPerPacket);
			}
		}

		/// <summary>Contains the latest error code set by one of the methods in AudioFileStream.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		/// 	  Accessing some properties and methods set this value when
		/// 	  accessed, you can use this during debugging to identify
		/// 	  problems in your code.
		/// 	</remarks>
		public AudioFileStreamStatus LastError { get; private set; }
	}
}
