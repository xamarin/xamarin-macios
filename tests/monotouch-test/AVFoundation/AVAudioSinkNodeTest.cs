using System;
using System.Runtime.InteropServices;
using System.Threading;

using NUnit.Framework;

using AudioToolbox;
using AudioUnit;
using AVFoundation;
using Foundation;

namespace MonoTouchFixtures.AVFoundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAudioSinkNodeTest {
		[Test]
		public void SinkNodeCallback ()
		{
			var callbackEvent = new ManualResetEvent (false);
			SinkNodeCallbackTest (callbackEvent, () => {
#if XAMCORE_5_0
				var handler = new AVAudioSinkNodeReceiverHandler ((ts, n, buffers) => SinkHandler (ts, n, buffers, callbackEvent));
#else
				var handler = new AVAudioSinkNodeReceiverHandler ((AudioTimeStamp ts, uint n, ref AudioBuffers buffers) => SinkHandler (ts, n, ref buffers, callbackEvent));
#endif
				return new AVAudioSinkNode (handler);
			});
		}

		[Test]
		public void SinkNodeCallbackRaw ()
		{
			var callbackEvent = new ManualResetEvent (false);
			SinkNodeCallbackTest (callbackEvent, () => {
				var handler = new AVAudioSinkNodeReceiverHandlerRaw ((ts, n, buffers) => SinkHandlerRaw (ts, n, buffers, callbackEvent));
				return new AVAudioSinkNode (handler);
			});
		}

#if !XAMCORE_5_0
		[Test]
		public void SinkNodeCallback2 ()
		{
			var callbackEvent = new ManualResetEvent (false);
			SinkNodeCallbackTest (callbackEvent, () => {
				var handler = new AVAudioSinkNodeReceiverHandler2 ((ts, n, buffers) => SinkHandler2 (ts, n, buffers, callbackEvent));
				return new AVAudioSinkNode (handler);
			});
		}
#endif

		void SinkNodeCallbackTest (ManualResetEvent callbackEvent, Func<AVAudioSinkNode> createSinkNode)
		{
			TestRuntime.AssertNotVirtualMachine ();

#if !__MACOS__
			using var session = AVAudioSession.SharedInstance ();
			session.SetCategory (AVAudioSessionCategory.Record);
			session.SetPreferredSampleRate (48000, out var sampleRateError);
			session.SetPreferredInputNumberOfChannels (1, out var inputChannelCountError);
			session.SetActive (true);
#endif

			using var engine = new AVAudioEngine ();

			try {
				var inputNode = engine.InputNode;
				var inputFormat = inputNode.GetBusOutputFormat (0);
				var outputFormat = inputFormat;
				var sinkNode = createSinkNode ();
				engine.AttachNode (sinkNode);
				engine.Connect (inputNode, sinkNode, inputFormat);
				engine.StartAndReturnError (out var error);

				Assert.IsNull (error, "Start error");
				Assert.True (callbackEvent.WaitOne (TimeSpan.FromSeconds (5)), "Called back");
			} finally {
				engine.Stop ();
			}
		}

		int SinkHandler (AudioTimeStamp ts, uint n, ref AudioBuffers buffers, ManualResetEvent evt)
		{
			return SinkHandler2 (ts, n, buffers, evt);
		}

		int SinkHandler2 (AudioTimeStamp ts, uint n, AudioBuffers buffers, ManualResetEvent evt)
		{
			var data = new float [n];

			int nCh = buffers.Count;
			for (int i = 0; i < nCh; i++) {
				var ptr = buffers [i].Data;
				Marshal.Copy (ptr, data, 0, (int) n);
			}

			evt.Set ();

			return 0;
		}

		unsafe int SinkHandlerRaw (IntPtr ts_ptr, uint n, IntPtr buffers_ptr, ManualResetEvent evt)
		{
			var ts = *(AudioTimeStamp*) ts_ptr;
			var buffers = new AudioBuffers (buffers_ptr);
			return SinkHandler2 (ts, n, buffers, evt);
		}
	}
}
