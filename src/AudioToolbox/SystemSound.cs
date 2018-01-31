// 
// SystemSound.cs: AudioServices system sound
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2009 Novell, Inc
// Copyright 2012 Xamarin Inc.
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;

namespace XamCore.AudioToolbox {

	enum SystemSoundId : uint { // UInt32 SystemSoundID
		Vibrate = 0x00000FFF,
	}

	public class SystemSound : INativeObject, IDisposable {
#if MONOMAC
		// TODO:
#else
		public static readonly SystemSound Vibrate = new SystemSound ((uint) SystemSoundId.Vibrate, false);
#endif

		uint soundId;
		bool ownsHandle;

		Action completionRoutine;
		GCHandle gc_handle;
		static readonly Action<SystemSoundId, IntPtr> SoundCompletionCallback = SoundCompletionShared;

		internal SystemSound (uint soundId, bool ownsHandle)
		{
			this.soundId = soundId;
			this.ownsHandle = ownsHandle;
		}

		public SystemSound (uint soundId) : this (soundId, false) {}
			

		~SystemSound ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				AssertNotDisposed ();
				return (IntPtr) soundId;
			}
		}

		public bool IsUISound {
			get {
				uint out_size = sizeof (uint);
				uint data;

				var res = AudioServices.AudioServicesGetProperty (AudioServicesPropertyKey.IsUISound, sizeof (AudioServicesPropertyKey), ref soundId, out out_size, out data);
				if (res != AudioServicesError.None)
					throw new ArgumentException (res.ToString ());

				return data == 1;
			}

			set {
				uint data = value ? (uint)1 : 0;

				var res = AudioServices.AudioServicesSetProperty (AudioServicesPropertyKey.IsUISound, sizeof (AudioServicesPropertyKey), ref soundId, sizeof (uint), ref data);
				if (res != AudioServicesError.None)
					throw new ArgumentException (res.ToString ());
			}
		}

		public bool CompletePlaybackIfAppDies {
			get {
				uint out_size = sizeof (uint);
				uint data;

				var res = AudioServices.AudioServicesGetProperty (AudioServicesPropertyKey.CompletePlaybackIfAppDies, sizeof (AudioServicesPropertyKey), ref soundId, out out_size, out data);
				if (res != AudioServicesError.None)
					throw new ArgumentException (res.ToString ());

				return data == 1;
			}

			set {
				uint data = value ? (uint)1 : 0;

				var res = AudioServices.AudioServicesSetProperty (AudioServicesPropertyKey.CompletePlaybackIfAppDies, sizeof (AudioServicesPropertyKey), ref soundId, sizeof (uint), ref data);
				if (res != AudioServicesError.None)
					throw new ArgumentException (res.ToString ());
			}
		}

		void AssertNotDisposed ()
		{
			if (soundId == 0)
				throw new ObjectDisposedException ("SystemSound");
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			Cleanup (false);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioServicesError AudioServicesDisposeSystemSoundID (uint soundId);

		void Cleanup (bool checkForError)
		{
			if (soundId == 0 || !ownsHandle)
				return;

			if (gc_handle.IsAllocated) {
				gc_handle.Free ();
			}

			if (completionRoutine != null) {
				RemoveSystemSoundCompletion ();
			}

			var error = AudioServicesDisposeSystemSoundID (soundId);
			var oldId = soundId;
			soundId = 0;
			if (checkForError && error != AudioServicesError.None) {
				throw new InvalidOperationException (string.Format ("Error while disposing SystemSound with ID {0}: {1}",
							oldId, error.ToString()));
			}
		}

		public void Close ()
		{
			Cleanup (true);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern void AudioServicesPlayAlertSound (uint inSystemSoundID);
		public void PlayAlertSound ()
		{
			AssertNotDisposed ();
			AudioServicesPlayAlertSound (soundId);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern void AudioServicesPlaySystemSound(uint inSystemSoundID);
		public void PlaySystemSound ()
		{
			AssertNotDisposed ();
			AudioServicesPlaySystemSound (soundId);
		}

		static unsafe readonly Action<IntPtr> static_action = TrampolineAction;

		[MonoPInvokeCallback (typeof (Action<IntPtr>))]
		static unsafe void TrampolineAction (IntPtr blockPtr)
		{
			var block = (BlockLiteral *) blockPtr;
			var del = (Action) (block->Target);
			if (del != null)
				del ();
		}

		[iOS (9,0)][Mac (10,11)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void PlayAlertSound (Action onCompletion)
		{
			if (onCompletion == null)
				throw new ArgumentNullException (nameof (onCompletion));
			
			AssertNotDisposed ();

			unsafe {
				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_action, onCompletion);

				AudioServicesPlayAlertSoundWithCompletion (soundId, block_ptr_handler);

				block_ptr_handler->CleanupBlock ();
			}

		}

		[iOS (9,0)][Mac (10,11)]
		public Task PlayAlertSoundAsync ()
		{
                        var tcs = new TaskCompletionSource<bool> ();
                        PlayAlertSound(() => {
                                tcs.SetResult (true);
                        });
                        return tcs.Task;
		}

		[iOS (9,0)][Mac (10,11)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void PlaySystemSound (Action onCompletion)
		{
			if (onCompletion == null)
				throw new ArgumentNullException (nameof (onCompletion));
			
			AssertNotDisposed ();
			unsafe {
				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_action, onCompletion);

				AudioServicesPlaySystemSoundWithCompletion (soundId, block_ptr_handler);

				block_ptr_handler->CleanupBlock ();
			}
		}

		[iOS (9,0)][Mac (10,11)]
		public Task PlaySystemSoundAsync ()
		{
                        var tcs = new TaskCompletionSource<bool> ();
                        PlaySystemSound(() => {
                                tcs.SetResult (true);
                        });
                        return tcs.Task;
		}

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.AudioToolboxLibrary)]
		static unsafe extern void AudioServicesPlayAlertSoundWithCompletion (uint inSystemSoundID, BlockLiteral * inCompletionBlock);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.AudioToolboxLibrary)]
		static unsafe extern void AudioServicesPlaySystemSoundWithCompletion (uint inSystemSoundID, BlockLiteral * inCompletionBlock);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioServicesError AudioServicesCreateSystemSoundID (IntPtr fileUrl, out uint soundId);

		public SystemSound (NSUrl fileUrl)
		{
			var error = AudioServicesCreateSystemSoundID (fileUrl.Handle, out soundId);
			if (error != AudioServicesError.None)
				throw new InvalidOperationException (string.Format ("Could not create system sound ID for url {0}; error={1}",
							fileUrl, error));
			ownsHandle = true;
		}
			
		public static SystemSound FromFile (NSUrl fileUrl)
		{
			uint soundId;
			var error = AudioServicesCreateSystemSoundID (fileUrl.Handle, out soundId);
			if (error != AudioServicesError.None)
				return null;
			return new SystemSound (soundId, true);
		}

		public static SystemSound FromFile (string filename)
		{
			using (var url = new NSUrl (filename)){
				uint soundId;
				var error = AudioServicesCreateSystemSoundID (url.Handle, out soundId);
				if (error != AudioServicesError.None)
					return null;
				return new SystemSound (soundId, true);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern AudioServicesError AudioServicesAddSystemSoundCompletion (uint soundId, IntPtr runLoop, IntPtr runLoopMode, Action<SystemSoundId, IntPtr> completionRoutine, IntPtr clientData);

		[MonoPInvokeCallback (typeof (Action<SystemSoundId, IntPtr>))]
		static void SoundCompletionShared (SystemSoundId id, IntPtr clientData)
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var ss = (SystemSound) gch.Target;

			ss.completionRoutine ();
		}

		public AudioServicesError AddSystemSoundCompletion (Action routine, CFRunLoop runLoop = null)
		{
			if (gc_handle.IsAllocated)
				throw new ArgumentException ("Only single completion routine is supported");

			gc_handle = GCHandle.Alloc (this);
			completionRoutine = routine;

			return AudioServicesAddSystemSoundCompletion (soundId,
			                                              runLoop == null ? IntPtr.Zero : runLoop.Handle,
			                                              IntPtr.Zero, // runLoopMode should be enum runLoopMode == null ? IntPtr.Zero : runLoopMode.Handle,
			                                              SoundCompletionCallback, GCHandle.ToIntPtr (gc_handle));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern void AudioServicesRemoveSystemSoundCompletion (uint soundId);

		public void RemoveSystemSoundCompletion ()
		{
			completionRoutine = null;
			AudioServicesRemoveSystemSoundCompletion (soundId);
		}
	}
}
