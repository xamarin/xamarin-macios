using System;
using System.Linq;
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
			session.SetCategory (AVAudioSessionCategory.PlayAndRecord, AVAudioSessionCategoryOptions.DefaultToSpeaker, out var categoryError);
			Assert.IsNull (categoryError, "Category Error");
			session.SetPreferredSampleRate (48000, out var sampleRateError);
			Assert.IsNull (sampleRateError, "Sample Rate Error");
			session.SetPreferredInputNumberOfChannels (1, out var inputChannelCountError);
			Assert.IsNull (inputChannelCountError, "Input Channel Count Error");
			session.SetActive (true);

			Assert.IsTrue (session.InputAvailable, "Input Available");

			Console.WriteLine ($"AVAudioSession: {session}");
			Console.WriteLine ($"Mode: {session.Mode}");
			Console.WriteLine ($"Preferred Input: {session.PreferredInput}");
			var availableInputs = session.AvailableInputs;
			Console.WriteLine ($"Available Inputs: {availableInputs.Length}");
			foreach (var ai in availableInputs)
				Console.WriteLine ($"    {ai}");
			Console.WriteLine ($"Available Categories: {string.Join (", ", session.AvailableCategories)}");
			Console.WriteLine ($"Available Modes: {string.Join (", ", session.AvailableModes)}");
#endif

			using var engine = new AVAudioEngine ();

			try {
				var inputNode = engine.InputNode;
				var inputFormat = inputNode.GetBusOutputFormat (0);
				Console.WriteLine ($"Input Format:");
				Console.WriteLine ($"    Sample Rate: {inputFormat.SampleRate}");
				Console.WriteLine ($"    ChannelCount: {inputFormat.ChannelCount}");
				Console.WriteLine ($"    ChannelLayout: {inputFormat.ChannelLayout}");
				Console.WriteLine ($"    FormatDescription: {inputFormat.FormatDescription}");
				Console.WriteLine ($"    FormatDescription.MediaType: {inputFormat.FormatDescription?.MediaType}");
				Console.WriteLine ($"    FormatDescription.MediaSubType: {inputFormat.FormatDescription?.MediaSubType}");
				Console.WriteLine ($"    FormatDescription.AudioStreamBasicDescription: {inputFormat.FormatDescription?.AudioStreamBasicDescription}");
				Console.WriteLine ($"    FormatDescription.AudioChannelLayout: {inputFormat.FormatDescription?.AudioChannelLayout}");
				Console.WriteLine ($"    FormatDescription.AudioFormats: {inputFormat.FormatDescription?.AudioFormats}");
				Console.WriteLine ($"    FormatDescription.AudioMostCompatibleFormat: {inputFormat.FormatDescription?.AudioMostCompatibleFormat}");
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
