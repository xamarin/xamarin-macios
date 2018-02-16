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

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using AudioToolbox;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace AudioUnit
{
	public enum AUGraphError // Implictly cast to OSType
	{
		OK = 0,
		NodeNotFound 				= -10860,
		InvalidConnection 			= -10861,
		OutputNodeError				= -10862,
		CannotDoInCurrentContext	= -10863,
		InvalidAudioUnit			= -10864,

		// Values returned & shared with other error enums
		FormatNotSupported			= -10868,
		InvalidElement				= -10877,		
	}

	public class AUGraph : INativeObject, IDisposable
	{
		readonly GCHandle gcHandle;
		IntPtr handle;

		[Preserve (Conditional = true)]
		internal AUGraph (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (this.handle);

			gcHandle = GCHandle.Alloc (this);
		}

		internal AUGraph (IntPtr ptr)
		{
			handle = ptr;
			
#if !XAMCORE_2_0
			int err = AUGraphAddRenderNotify(handle, oldRenderCallback, GCHandle.ToIntPtr(gcHandle));
			if (err != 0)
				throw new ArgumentException(String.Format("Error code: {0}", err));
#endif

			gcHandle = GCHandle.Alloc (this);
		}

#if !XAMCORE_2_0
#pragma warning disable 612
		public event EventHandler<AudioGraphEventArgs> RenderCallback;
#pragma warning restore 612

		[Obsolete ("Use 'Handle' property instead.")]
		public IntPtr Handler {
			get {
				return handle;
			}
		}
#endif

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public AUGraph ()
		{
			int err = NewAUGraph (ref handle);
			if (err != 0)
				throw new InvalidOperationException (String.Format ("Cannot create new AUGraph. Error code: {0}", err));

			gcHandle = GCHandle.Alloc (this);
		}

		public static AUGraph Create (out int errorCode)
		{
			IntPtr ret = IntPtr.Zero;
			errorCode = NewAUGraph (ref ret);

			if (errorCode != 0)
				return null;

			return new AUGraph (ret);
		}

		public bool IsInitialized {
			get {
				bool b;
				return AUGraphIsInitialized (handle, out b) == AUGraphError.OK && b;
			}
		}

		public bool IsOpen {
			get {
				bool b;
				return AUGraphIsOpen (handle, out b) == AUGraphError.OK && b;
			}
		}

		public bool IsRunning {
			get {
				bool b;
				return AUGraphIsRunning (handle, out b) == AUGraphError.OK && b;
			}
		}

#if !XAMCORE_2_0
		// callback funtion should be static method and be attatched a MonoPInvokeCallback attribute.        
#pragma warning disable 612
		[MonoPInvokeCallback(typeof(AudioUnit.AURenderCallback))]
		static int oldRenderCallback(IntPtr inRefCon,
					  ref AudioUnitRenderActionFlags _ioActionFlags,
					  ref AudioTimeStamp _inTimeStamp,
					  int _inBusNumber,
					  int _inNumberFrames,
					  AudioBufferList _ioData)
		{
			// getting audiounit instance
			var handler = GCHandle.FromIntPtr(inRefCon);
			var inst = (AUGraph)handler.Target;
			
			// invoke event handler with an argument
			if (inst.RenderCallback != null){
				var args = new AudioGraphEventArgs(
					_ioActionFlags,
					_inTimeStamp,
					_inBusNumber,
					_inNumberFrames,
					_ioData);
				inst.RenderCallback(inst, args);
			}

			return 0; // noerror
		}
#pragma warning restore 612
#endif

		public AudioUnitStatus AddRenderNotify (RenderDelegate callback)
		{
			if (callback == null)
				throw new ArgumentException ("Callback can not be null");

			AudioUnitStatus error = AudioUnitStatus.OK;
			if (graphUserCallbacks.Count == 0)
				error = (AudioUnitStatus)AUGraphAddRenderNotify (handle, renderCallback, GCHandle.ToIntPtr(gcHandle));

			if (error == AudioUnitStatus.OK)
				graphUserCallbacks.Add (callback);
			return error;
		}

		public AudioUnitStatus RemoveRenderNotify (RenderDelegate callback)
		{
			if (callback == null)
				throw new ArgumentException ("Callback can not be null");
			if (!graphUserCallbacks.Contains (callback))
				throw new ArgumentException ("Cannot unregister a callback that has not been registered");

			AudioUnitStatus error = AudioUnitStatus.OK;
			if (graphUserCallbacks.Count == 0)
				error = (AudioUnitStatus)AUGraphRemoveRenderNotify (handle, renderCallback, GCHandle.ToIntPtr (gcHandle));

			graphUserCallbacks.Remove (callback); // Remove from list even if there is an error
			return error;
		}

		HashSet<RenderDelegate> graphUserCallbacks = new HashSet<RenderDelegate> ();

		[MonoPInvokeCallback(typeof(CallbackShared))]
		static AudioUnitStatus renderCallback(IntPtr inRefCon,
					ref AudioUnitRenderActionFlags _ioActionFlags,
					ref AudioTimeStamp _inTimeStamp,
					uint _inBusNumber,
					uint _inNumberFrames,
					IntPtr _ioData)
		{
			// getting audiounit instance
			var handler = GCHandle.FromIntPtr (inRefCon);
			var inst = (AUGraph)handler.Target;
			HashSet<RenderDelegate> renderers = inst.graphUserCallbacks;

			if (renderers.Count != 0) {
				using (var buffers = new AudioBuffers (_ioData)) {
					foreach (RenderDelegate renderer in renderers)
						renderer (_ioActionFlags, _inTimeStamp, _inBusNumber, _inNumberFrames, buffers);
					return AudioUnitStatus.OK;
				}
			}

			return AudioUnitStatus.InvalidParameter;
		}

		public void Open ()
		{ 
			int err = AUGraphOpen (handle);
			if (err != 0)
				throw new InvalidOperationException (String.Format ("Cannot open AUGraph. Error code: {0}", err));
		}

		public int TryOpen ()
		{ 
			int err = AUGraphOpen (handle);
			return err;
		}
		
		public int AddNode (AudioComponentDescription description)
		{
			int node;
#if XAMCORE_2_0
			var err = AUGraphAddNode (handle, ref description, out node);
#else
			var err = AUGraphAddNode (handle, description, out node);
#endif
			if (err != 0)
				throw new ArgumentException (String.Format ("Error code: {0}", err));
			
			return node;
		}

		public AUGraphError RemoveNode (int node)
		{
			return AUGraphRemoveNode (handle, node);
		}

		public AUGraphError GetCPULoad (out float averageCPULoad)
		{
			return AUGraphGetCPULoad (handle, out averageCPULoad);
		}

		public AUGraphError GetMaxCPULoad (out float maxCPULoad)
		{
			return AUGraphGetMaxCPULoad (handle, out maxCPULoad);
		}

		public AUGraphError GetNode (uint index, out int node)
		{
			return AUGraphGetIndNode (handle, index, out node);
		}

		public AUGraphError GetNodeCount (out int count)
		{
			return AUGraphGetNodeCount (handle, out count);
		}
		
		public AudioUnit GetNodeInfo (int node)
		{
			AUGraphError error;
			var unit = GetNodeInfo (node, out error);

			if (error != AUGraphError.OK)
				throw new ArgumentException (String.Format ("Error code:{0}", error));

			if (unit == null)
				throw new InvalidOperationException ("Can not get object instance");

			return unit;
		}

		public AudioUnit GetNodeInfo (int node, out AUGraphError error)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AUGraph");

			IntPtr ptr;
			error = AUGraphNodeInfo (handle, node, IntPtr.Zero, out ptr);

			if (error != AUGraphError.OK || ptr == IntPtr.Zero)
				return null;

			return new AudioUnit (ptr);
		}

#if XAMCORE_2_0
		// AudioComponentDescription struct in only correctly fixed for unified
		// Following current Api behaviour of returning an AudioUnit instead of an error
		public AudioUnit GetNodeInfo (int node, out AudioComponentDescription cd, out AUGraphError error)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AUGraph");

			IntPtr ptr;
			error = AUGraphNodeInfo (Handle, node, out cd, out ptr);

			if (error != AUGraphError.OK || ptr == IntPtr.Zero)
				return null;

			return new AudioUnit (ptr);
		}
#endif // XAMCORE_2_0

		public AUGraphError GetNumberOfInteractions (out uint interactionsCount)
		{
			return AUGraphGetNumberOfInteractions (handle, out interactionsCount);
		}

		public AUGraphError GetNumberOfInteractions (int node, out uint interactionsCount)
		{
			return AUGraphCountNodeInteractions (handle, node, out interactionsCount);
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
			return AUGraphConnectNodeInput (handle,
							  sourceNode, sourceOutputNumber,                                    
							  destNode, destInputNumber);
		}

		public AUGraphError DisconnectNodeInput (int destNode, uint destInputNumber)
		{
			return AUGraphDisconnectNodeInput (handle, destNode, destInputNumber);
		}

		Dictionary<uint, RenderDelegate> nodesCallbacks;
		static readonly CallbackShared CreateRenderCallback = RenderCallbackImpl;

		public AUGraphError SetNodeInputCallback (int destNode, uint destInputNumber, RenderDelegate renderDelegate)
		{
			if (nodesCallbacks == null)
				nodesCallbacks = new Dictionary<uint, RenderDelegate> ();

			nodesCallbacks [destInputNumber] = renderDelegate;

			var cb = new AURenderCallbackStruct ();
			cb.Proc = CreateRenderCallback;
			cb.ProcRefCon = GCHandle.ToIntPtr (gcHandle);
			return AUGraphSetNodeInputCallback (handle, destNode, destInputNumber, ref cb);
		}

		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus RenderCallbackImpl (IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var au = (AUGraph) gch.Target;

			RenderDelegate callback;
			if (!au.nodesCallbacks.TryGetValue (busNumber, out callback))
				return AudioUnitStatus.InvalidParameter;

			using (var buffers = new AudioBuffers (data)) {
				return callback (actionFlags, timeStamp, busNumber, numberFrames, buffers);
			}
		}

		public AUGraphError ClearConnections ()
		{
			return AUGraphClearConnections (handle);
		}

		public AUGraphError Start()
		{
			return AUGraphStart (handle);
		}

		public AUGraphError Stop()
		{
			return AUGraphStop (handle);
		}
		
		public AUGraphError Initialize()
		{
			return AUGraphInitialize (handle);
		}

		public bool Update ()
		{
			bool isUpdated;
			return AUGraphUpdate (handle, out isUpdated) == AUGraphError.OK && isUpdated;
		}

		// Quote from Learning CoreAudio Book:
		// The CAShow() function logs (to standard output) a list of all the nodes in the graph, 
		// along with the connections between them and the stream format used in each of those connections
		public void LogAllNodes ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AUGraph");
			
			CAShow (Handle);
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			if (handle != IntPtr.Zero){
				AUGraphUninitialize (handle);
				AUGraphClose (handle);
				DisposeAUGraph (handle);
				
				gcHandle.Free();
				handle = IntPtr.Zero;
			}
		}

		~AUGraph ()
		{
			Dispose (false);
		}
			
		[DllImport(Constants.AudioToolboxLibrary, EntryPoint = "NewAUGraph")]
		static extern int /* OSStatus */ NewAUGraph(ref IntPtr outGraph);

		[DllImport(Constants.AudioToolboxLibrary, EntryPoint = "AUGraphOpen")]
		static extern int /* OSStatus */ AUGraphOpen(IntPtr inGraph);

		[DllImport(Constants.AudioToolboxLibrary)]
#if XAMCORE_2_0
		static extern AUGraphError AUGraphAddNode(IntPtr inGraph, ref AudioComponentDescription inDescription, out int /* AUNode = SInt32* */ outNode);
#else
		static extern AUGraphError AUGraphAddNode(IntPtr inGraph, AudioComponentDescription inDescription, out int /* AUNode = SInt32* */ outNode);
#endif

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphRemoveNode (IntPtr inGraph, int /* AUNode = SInt32 */ inNode);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphGetNodeCount (IntPtr inGraph, out int /* UInt32* */ outNumberOfNodes);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphGetIndNode (IntPtr inGraph, uint /* UInt32 */ inIndex, out int /* AUNode = SInt32* */ outNode);
	
		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphNodeInfo(IntPtr inGraph, int /* AUNode = SInt32 */ inNode, IntPtr outDescription, out IntPtr outAudioUnit);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphNodeInfo(IntPtr inGraph, int /* AUNode = SInt32 */ inNode, out AudioComponentDescription outDescription, out IntPtr outAudioUnit);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphClearConnections (IntPtr inGraph);
	
		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphConnectNodeInput (IntPtr inGraph, int /* AUNode = SInt32 */ inSourceNode, uint /* UInt32 */ inSourceOutputNumber, int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphDisconnectNodeInput (IntPtr inGraph, int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphGetNumberOfInteractions (IntPtr inGraph, out uint /* UInt32* */ outNumInteractions);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphCountNodeInteractions (IntPtr inGraph, int /* AUNode = SInt32 */ inNode, out uint /* UInt32* */ outNumInteractions);
	
		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphInitialize (IntPtr inGraph);

#if !XAMCORE_2_0
#pragma warning disable 612
		[DllImport(Constants.AudioToolboxLibrary, EntryPoint = "AUGraphAddRenderNotify")]
		static extern int AUGraphAddRenderNotify (IntPtr inGraph, AudioUnit.AURenderCallback inCallback, IntPtr inRefCon );
#pragma warning restore 612
#endif

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern int AUGraphAddRenderNotify (IntPtr inGraph, CallbackShared inCallback, IntPtr inRefCon );

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern int AUGraphRemoveRenderNotify (IntPtr inGraph, CallbackShared inCallback, IntPtr inRefCon );

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphStart (IntPtr inGraph);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphStop (IntPtr inGraph);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphUninitialize (IntPtr inGraph);

		[DllImport(Constants.AudioToolboxLibrary, EntryPoint = "AUGraphClose")]
		static extern int /* OSStatus */ AUGraphClose(IntPtr inGraph);
	
		[DllImport(Constants.AudioToolboxLibrary)]
		static extern int /* OSStatus */ DisposeAUGraph(IntPtr inGraph);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphIsOpen (IntPtr inGraph, out bool outIsOpen);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphIsInitialized (IntPtr inGraph, out bool outIsInitialized);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphIsRunning (IntPtr inGraph, out bool outIsRunning);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphGetCPULoad (IntPtr inGraph, out float /* Float32* */ outAverageCPULoad);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphGetMaxCPULoad (IntPtr inGraph, out float /* Float32* */ outMaxLoad);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphSetNodeInputCallback (IntPtr inGraph, int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber, ref AURenderCallbackStruct inInputCallback);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern AUGraphError AUGraphUpdate (IntPtr inGraph, out bool outIsUpdated);

		[DllImport(Constants.AudioToolboxLibrary)]
		static extern void CAShow (IntPtr handle);
	}
}
