//
// AUGraph.cs: AUGraph wrapper class
//
// Authors:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2010 Novell, Inc.
// Copyright 2012 Xamarin Inc
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
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

using AudioToolbox;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioUnit {
	public enum AUGraphError // Implictly cast to OSType
	{
		OK = 0,
		NodeNotFound = -10860,
		InvalidConnection = -10861,
		OutputNodeError = -10862,
		CannotDoInCurrentContext = -10863,
		InvalidAudioUnit = -10864,

		// Values returned & shared with other error enums
		FormatNotSupported = -10868,
		InvalidElement = -10877,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("tvos14.0", "Use 'AVAudioEngine' instead.")]
	[ObsoletedOSPlatform ("macos11.0", "Use 'AVAudioEngine' instead.")]
	[ObsoletedOSPlatform ("ios14.0", "Use 'AVAudioEngine' instead.")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use 'AVAudioEngine' instead.")]
#else
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'AVAudioEngine' instead.")]
	[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'AVAudioEngine' instead.")]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'AVAudioEngine' instead.")]
#endif
	public class AUGraph : DisposableObject {
		readonly GCHandle gcHandle;

		[Preserve (Conditional = true)]
		internal AUGraph (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
			gcHandle = GCHandle.Alloc (this);
		}

		static IntPtr Create ()
		{
			IntPtr handle;
			int err;
			unsafe {
				err = NewAUGraph (&handle);
			}
			if (err != 0)
				throw new InvalidOperationException (String.Format ("Cannot create new AUGraph. Error code: {0}", err));
			return handle;
		}

		public AUGraph ()
			: this (Create (), true)
		{
		}

		public static AUGraph? Create (out int errorCode)
		{
			IntPtr handle;
			unsafe {
				errorCode = NewAUGraph (&handle);
			}

			if (errorCode != 0)
				return null;

			return new AUGraph (handle, true);
		}

		public bool IsInitialized {
			get {
				byte b;
				unsafe {
					return AUGraphIsInitialized (Handle, &b) == AUGraphError.OK && b != 0;
				}
			}
		}

		public bool IsOpen {
			get {
				byte b;
				unsafe {
					return AUGraphIsOpen (Handle, &b) == AUGraphError.OK && b != 0;
				}
			}
		}

		public bool IsRunning {
			get {
				byte b;
				unsafe {
					return AUGraphIsRunning (Handle, &b) == AUGraphError.OK && b != 0;
				}
			}
		}

		public AudioUnitStatus AddRenderNotify (RenderDelegate callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			AudioUnitStatus error = AudioUnitStatus.OK;
#if NET
			unsafe {
				if (graphUserCallbacks.Count == 0)
					error = (AudioUnitStatus) AUGraphAddRenderNotify (Handle, &renderCallback, GCHandle.ToIntPtr (gcHandle));
			}
#else
			if (graphUserCallbacks.Count == 0)
				error = (AudioUnitStatus) AUGraphAddRenderNotify (Handle, renderCallback, GCHandle.ToIntPtr (gcHandle));
#endif

			if (error == AudioUnitStatus.OK)
				graphUserCallbacks.Add (callback);
			return error;
		}

		public AudioUnitStatus RemoveRenderNotify (RenderDelegate callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));
			if (!graphUserCallbacks.Contains (callback))
				throw new ArgumentException ("Cannot unregister a callback that has not been registered");

			AudioUnitStatus error = AudioUnitStatus.OK;
#if NET
			unsafe {
				if (graphUserCallbacks.Count == 0)
					error = (AudioUnitStatus) AUGraphRemoveRenderNotify (Handle, &renderCallback, GCHandle.ToIntPtr (gcHandle));
			}
#else
			if (graphUserCallbacks.Count == 0)
				error = (AudioUnitStatus) AUGraphRemoveRenderNotify (Handle, renderCallback, GCHandle.ToIntPtr (gcHandle));
#endif

			graphUserCallbacks.Remove (callback); // Remove from list even if there is an error
			return error;
		}

		HashSet<RenderDelegate> graphUserCallbacks = new HashSet<RenderDelegate> ();

#if !NET
		static CallbackShared? _static_CallbackShared;
		static CallbackShared static_CallbackShared {
			get {
				if (_static_CallbackShared is null)
					_static_CallbackShared = new CallbackShared (renderCallback);
				return _static_CallbackShared;
			}
		}
#endif

#if NET
		[UnmanagedCallersOnly]
		static unsafe AudioUnitStatus renderCallback (IntPtr inRefCon,
					AudioUnitRenderActionFlags* _ioActionFlags,
					AudioTimeStamp* _inTimeStamp,
					uint _inBusNumber,
					uint _inNumberFrames,
					IntPtr _ioData)
#else
		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus renderCallback (IntPtr inRefCon,
					ref AudioUnitRenderActionFlags _ioActionFlags,
					ref AudioTimeStamp _inTimeStamp,
					uint _inBusNumber,
					uint _inNumberFrames,
					IntPtr _ioData)
#endif
		{
			// getting audiounit instance
			var handler = GCHandle.FromIntPtr (inRefCon);
			var inst = handler.Target as AUGraph;
			var renderers = inst?.graphUserCallbacks;

			if (renderers is null)
				return AudioUnitStatus.InvalidParameter;

			if (renderers.Count != 0) {
				using (var buffers = new AudioBuffers (_ioData)) {
					foreach (RenderDelegate renderer in renderers) {
#if NET
						renderer (*_ioActionFlags, *_inTimeStamp, _inBusNumber, _inNumberFrames, buffers);
#else
						renderer (_ioActionFlags, _inTimeStamp, _inBusNumber, _inNumberFrames, buffers);
#endif
					}
					return AudioUnitStatus.OK;
				}
			}

			return AudioUnitStatus.InvalidParameter;
		}

		public void Open ()
		{
			int err = AUGraphOpen (Handle);
			if (err != 0)
				throw new InvalidOperationException (String.Format ("Cannot open AUGraph. Error code: {0}", err));
		}

		public int TryOpen ()
		{
			int err = AUGraphOpen (Handle);
			return err;
		}

		public int AddNode (AudioComponentDescription description)
		{
			AUGraphError err;
			int node;
			unsafe {
				err = AUGraphAddNode (Handle, &description, &node);
			}
			if (err != 0)
				throw new ArgumentException (String.Format ("Error code: {0}", err));

			return node;
		}

		public AUGraphError RemoveNode (int node)
		{
			return AUGraphRemoveNode (Handle, node);
		}

		public AUGraphError GetCPULoad (out float averageCPULoad)
		{
			averageCPULoad = default;
			unsafe {
				return AUGraphGetCPULoad (Handle, (float*) Unsafe.AsPointer<float> (ref averageCPULoad));
			}
		}

		public AUGraphError GetMaxCPULoad (out float maxCPULoad)
		{
			maxCPULoad = default;
			unsafe {
				return AUGraphGetMaxCPULoad (Handle, (float*) Unsafe.AsPointer<float> (ref maxCPULoad));
			}
		}

		public AUGraphError GetNode (uint index, out int node)
		{
			node = default;
			unsafe {
				return AUGraphGetIndNode (Handle, index, (int*) Unsafe.AsPointer<int> (ref node));
			}
		}

		public AUGraphError GetNodeCount (out int count)
		{
			count = default;
			unsafe {
				return AUGraphGetNodeCount (Handle, (int*) Unsafe.AsPointer<int> (ref count));
			}
		}

		public AudioUnit GetNodeInfo (int node)
		{
			AUGraphError error;
			var unit = GetNodeInfo (node, out error);

			if (error != AUGraphError.OK)
				throw new ArgumentException (String.Format ("Error code:{0}", error));

			if (unit is null)
				throw new InvalidOperationException ("Can not get object instance");

			return unit;
		}

		public AudioUnit? GetNodeInfo (int node, out AUGraphError error)
		{
			IntPtr ptr;
			unsafe {
				error = AUGraphNodeInfo (GetCheckedHandle (), node, null, &ptr);
			}

			if (error != AUGraphError.OK || ptr == IntPtr.Zero)
				return null;

			return new AudioUnit (ptr, false);
		}

		// AudioComponentDescription struct in only correctly fixed for unified
		// Following current Api behaviour of returning an AudioUnit instead of an error
		public AudioUnit? GetNodeInfo (int node, out AudioComponentDescription cd, out AUGraphError error)
		{
			IntPtr ptr;
			cd = default;
			unsafe {
				error = AUGraphNodeInfo (GetCheckedHandle (), node, (AudioComponentDescription*) Unsafe.AsPointer<AudioComponentDescription> (ref cd), &ptr);
			}

			if (error != AUGraphError.OK || ptr == IntPtr.Zero)
				return null;

			return new AudioUnit (ptr, false);
		}

		public AUGraphError GetNumberOfInteractions (out uint interactionsCount)
		{
			interactionsCount = default;
			unsafe {
				return AUGraphGetNumberOfInteractions (Handle, (uint*) Unsafe.AsPointer<uint> (ref interactionsCount));
			}
		}

		public AUGraphError GetNumberOfInteractions (int node, out uint interactionsCount)
		{
			interactionsCount = default;
			unsafe {
				return AUGraphCountNodeInteractions (Handle, node, (uint*) Unsafe.AsPointer<uint> (ref interactionsCount));
			}
		}

		/*
				// TODO: requires AudioComponentDescription type to be fixed
				public AUGraphError GetNodeInfo (int node, out AudioComponentDescription description, out AudioUnit audioUnit)
				{
					IntPtr au;
					var res = AUGraphNodeInfo (handle, node, out description, out au);
					if (res != AUGraphError.OK) {
						audioUnit = null;
						return res;
					}

					audioUnit = new AudioUnit (au);
					return res;
				}
		*/
		public AUGraphError ConnnectNodeInput (int sourceNode, uint sourceOutputNumber, int destNode, uint destInputNumber)
		{
			return AUGraphConnectNodeInput (Handle,
							  sourceNode, sourceOutputNumber,
							  destNode, destInputNumber);
		}

		public AUGraphError DisconnectNodeInput (int destNode, uint destInputNumber)
		{
			return AUGraphDisconnectNodeInput (Handle, destNode, destInputNumber);
		}

		Dictionary<uint, RenderDelegate>? nodesCallbacks;
#if !NET
		static readonly CallbackShared CreateRenderCallback = RenderCallbackImpl;
#endif

		public AUGraphError SetNodeInputCallback (int destNode, uint destInputNumber, RenderDelegate renderDelegate)
		{
			if (nodesCallbacks is null)
				nodesCallbacks = new Dictionary<uint, RenderDelegate> ();

			nodesCallbacks [destInputNumber] = renderDelegate;

			var cb = new AURenderCallbackStruct ();
			unsafe {
#if NET
				cb.Proc = &RenderCallbackImpl;
#else
				cb.Proc = Marshal.GetFunctionPointerForDelegate (CreateRenderCallback);
#endif
				cb.ProcRefCon = GCHandle.ToIntPtr (gcHandle);
				return AUGraphSetNodeInputCallback (Handle, destNode, destInputNumber, &cb);
			}
		}
#if NET
		[UnmanagedCallersOnly]
		static unsafe AudioUnitStatus RenderCallbackImpl (IntPtr clientData, AudioUnitRenderActionFlags* actionFlags, AudioTimeStamp* timeStamp, uint busNumber, uint numberFrames, IntPtr data)
#else

		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus RenderCallbackImpl (IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
#endif
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var au = gch.Target as AUGraph;
			if (au?.nodesCallbacks is null)
				return AudioUnitStatus.InvalidParameter;

			if (!au.nodesCallbacks.TryGetValue (busNumber, out var callback))
				return AudioUnitStatus.InvalidParameter;

			using (var buffers = new AudioBuffers (data)) {
#if NET
				return callback (*actionFlags, *timeStamp, busNumber, numberFrames, buffers);
#else
				return callback (actionFlags, timeStamp, busNumber, numberFrames, buffers);
#endif
			}
		}

		public AUGraphError ClearConnections ()
		{
			return AUGraphClearConnections (Handle);
		}

		public AUGraphError Start ()
		{
			return AUGraphStart (Handle);
		}

		public AUGraphError Stop ()
		{
			return AUGraphStop (Handle);
		}

		public AUGraphError Initialize ()
		{
			return AUGraphInitialize (Handle);
		}

		public bool Update ()
		{
			byte isUpdated;
			unsafe {
				return AUGraphUpdate (Handle, &isUpdated) == AUGraphError.OK && isUpdated != 0;
			}
		}

		// Quote from Learning CoreAudio Book:
		// The CAShow() function logs (to standard output) a list of all the nodes in the graph, 
		// along with the connections between them and the stream format used in each of those connections
		public void LogAllNodes ()
		{
			CAShow (GetCheckedHandle ());
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns) {
				AUGraphUninitialize (Handle);
				AUGraphClose (Handle);
				DisposeAUGraph (Handle);
			}

			if (gcHandle.IsAllocated)
				gcHandle.Free ();

			base.Dispose (disposing);
		}

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "NewAUGraph")]
		unsafe static extern int /* OSStatus */ NewAUGraph (IntPtr* outGraph);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "AUGraphOpen")]
		static extern int /* OSStatus */ AUGraphOpen (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphAddNode (IntPtr inGraph, AudioComponentDescription* inDescription, int* /* AUNode = SInt32* */ outNode);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphRemoveNode (IntPtr inGraph, int /* AUNode = SInt32 */ inNode);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphGetNodeCount (IntPtr inGraph, int* /* UInt32* */ outNumberOfNodes);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphGetIndNode (IntPtr inGraph, uint /* UInt32 */ inIndex, int* /* AUNode = SInt32* */ outNode);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphNodeInfo (IntPtr inGraph, int /* AUNode = SInt32 */ inNode, AudioComponentDescription* outDescription, IntPtr* outAudioUnit);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphClearConnections (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphConnectNodeInput (IntPtr inGraph, int /* AUNode = SInt32 */ inSourceNode, uint /* UInt32 */ inSourceOutputNumber, int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphDisconnectNodeInput (IntPtr inGraph, int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphGetNumberOfInteractions (IntPtr inGraph, uint* /* UInt32* */ outNumInteractions);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphCountNodeInteractions (IntPtr inGraph, int /* AUNode = SInt32 */ inNode, uint* /* UInt32* */ outNumInteractions);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphInitialize (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
#if NET
		static unsafe extern int AUGraphAddRenderNotify (IntPtr inGraph, delegate* unmanaged<IntPtr, AudioUnitRenderActionFlags*, AudioTimeStamp*, uint, uint, IntPtr, AudioUnitStatus> inCallback, IntPtr inRefCon);
#else
		static extern int AUGraphAddRenderNotify (IntPtr inGraph, CallbackShared inCallback, IntPtr inRefCon);
#endif

#if NET
		[DllImport (Constants.AudioToolboxLibrary)]
		static unsafe extern int AUGraphRemoveRenderNotify (IntPtr inGraph, delegate* unmanaged<IntPtr, AudioUnitRenderActionFlags*, AudioTimeStamp*, uint, uint, IntPtr, AudioUnitStatus> inCallback, IntPtr inRefCon);
#else
		[DllImport (Constants.AudioToolboxLibrary)]
		static extern int AUGraphRemoveRenderNotify (IntPtr inGraph, CallbackShared inCallback, IntPtr inRefCon);
#endif

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphStart (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphStop (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphUninitialize (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "AUGraphClose")]
		static extern int /* OSStatus */ AUGraphClose (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern int /* OSStatus */ DisposeAUGraph (IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphIsOpen (IntPtr inGraph, byte* outIsOpen);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphIsInitialized (IntPtr inGraph, byte* outIsInitialized);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphIsRunning (IntPtr inGraph, byte* outIsRunning);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphGetCPULoad (IntPtr inGraph, float* /* Float32* */ outAverageCPULoad);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphGetMaxCPULoad (IntPtr inGraph, float* /* Float32* */ outMaxLoad);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphSetNodeInputCallback (IntPtr inGraph, int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber, AURenderCallbackStruct* inInputCallback);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe static extern AUGraphError AUGraphUpdate (IntPtr inGraph, byte* outIsUpdated);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern void CAShow (IntPtr handle);
	}
}
