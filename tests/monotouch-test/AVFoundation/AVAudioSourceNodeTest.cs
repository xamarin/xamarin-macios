using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using AudioToolbox;
using AVFoundation;
using Foundation;

namespace MonoTouchFixtures.AVFoundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAudioSourceNodeTest {
#if __WATCHOS__
		[SetUp]
		public void SetUp ()
		{
			// Looks like this test broke in the watchOS simulator, so just skip it there.
			TestRuntime.AssertNotSimulator ();
		}
#endif

		[Test]
		public void SourceNodeCallback ()
		{
			var callbackEvent = new TaskCompletionSource<bool> ();
			SourceNodeCallbackTest (callbackEvent, () => {
#if XAMCORE_5_0
				var handler = new AVAudioSourceNodeRenderHandler ((ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, AudioBuffers buffers) => SourceHandler (ref isSilence, ref timestamp, frameCount, buffers, callbackEvent));
#else
				var handler = new AVAudioSourceNodeRenderHandler3 ((ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, AudioBuffers buffers) => SourceHandler (ref isSilence, ref timestamp, frameCount, buffers, callbackEvent));
#endif
				return new AVAudioSourceNode (handler);
			});
		}

		[Test]
		public void SourceNodeCallbackRaw ()
		{
			var callbackEvent = new TaskCompletionSource<bool> ();
			SourceNodeCallbackTest (callbackEvent, () => {
				var handler = new AVAudioSourceNodeRenderHandlerRaw ((IntPtr isSilence, IntPtr timestamp, uint frameCount, IntPtr buffers) => SourceHandlerRaw (isSilence, timestamp, frameCount, buffers, callbackEvent));
				return new AVAudioSourceNode (handler);
			});
		}

#if !XAMCORE_5_0
		[Test]
		public void SourceNodeCallback2 ()
		{
			var callbackEvent = new TaskCompletionSource<bool> ();
			SourceNodeCallbackTest (callbackEvent, () => {
#if NET
				var handler = new AVAudioSourceNodeRenderHandler ((ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers buffers) => SourceHandler (ref isSilence, ref timestamp, frameCount, buffers, callbackEvent));
#else
				var handler = new AVAudioSourceNodeRenderHandler2 ((ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers buffers) => SourceHandler (ref isSilence, ref timestamp, frameCount, buffers, callbackEvent));
#endif
				return new AVAudioSourceNode (handler);
			});
		}
#endif

		void SourceNodeCallbackTest (TaskCompletionSource<bool> callbackEvent, Func<AVAudioSourceNode> createSourceNode)
		{
			TestRuntime.AssertNotVirtualMachine ();
			TestRuntime.AssertNotSimulator (); // broke in Xcode 16.2 beta 2 https://github.com/xamarin/maccore/issues/2956

#if __MACOS__
			var defaultCaptureDevice = AVCaptureDevice.GetDefaultDevice (AVMediaTypes.Audio);
			if (defaultCaptureDevice is null)
				Assert.Ignore ("The current system doesn't have a microphone.");
#else
			using var session = AVAudioSession.SharedInstance ();
			if (!session.InputAvailable)
				Assert.Ignore ("The current system doesn't have a microphone.");

			session.SetCategory (AVAudioSessionCategory.PlayAndRecord, AVAudioSessionCategoryOptions.DefaultToSpeaker, out var categoryError);
			Assert.IsNull (categoryError, "Category Error");
#if !__WATCHOS__
			session.SetPreferredSampleRate (48000, out var sampleRateError);
			Assert.IsNull (sampleRateError, "Sample Rate Error");
			if (session.MaximumInputNumberOfChannels == 0)
				Assert.Ignore ("The current system doesn't support any input channels");
			session.SetPreferredInputNumberOfChannels (1, out var inputChannelCountError);
			Assert.IsNull (inputChannelCountError, "Input Channel Count Error");
#endif // !__WATCHOS__
			session.SetActive (true);
#endif // __MACOS__

			using var engine = new AVAudioEngine ();

			try {
				var inputNode = engine.InputNode;
				var inputFormat = inputNode.GetBusOutputFormat (0);
				if (inputFormat.SampleRate == 0)
					Assert.Ignore ("The current system can't record audio.");

				var SourceNode = createSourceNode ();
				engine.AttachNode (SourceNode);
				engine.Connect (SourceNode, engine.MainMixerNode, inputFormat);
				engine.StartAndReturnError (out var error);

				Assert.IsNull (error, "Start error");
				Assert.True (callbackEvent.Task.Wait (TimeSpan.FromSeconds (5)), "Called back");
			} finally {
				engine.Stop ();
			}
		}

		int SourceHandler (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, AudioBuffers outputData, TaskCompletionSource<bool> evt)
		{
			try {
				Assert.AreEqual (1, outputData.Count, "Count");
				Assert.That (((IntPtr) outputData.Handle).ToInt64 (), Is.GreaterThan (1024), "Valid handle");
				Assert.That (outputData [0].DataByteSize, Is.GreaterThan (1023), "Valid data size");
				Assert.That (outputData [0].NumberChannels, Is.GreaterThan (0), "NumberChannels");
				Assert.That (frameCount, Is.GreaterThan (0), "FrameCount");

				evt.TrySetResult (true);
			} catch (Exception e) {
				evt.SetException (e);
			}
			return 0;
		}

		static void LogMe (string message)
		{
			Console.WriteLine ($"STDOUT: {message}");
			Console.Error.WriteLine ($"STDERR: {message}");
			TestRuntime.NSLog ($"NSLOG: {message}");
			File.AppendAllText ("/tmp/logme.txt", $"{DateTime.Now} {message}\n");
		}

		unsafe int SourceHandlerRaw (IntPtr isSilence, IntPtr timestamp, uint frameCount, IntPtr outputData, TaskCompletionSource<bool> evt)
		{
			int rv = 0;
			try {
				byte* isSilencePtr = (byte*) isSilence;
				bool isSilenceBool = (*isSilencePtr) != 0;
				var buffers = new AudioBuffers (outputData);
				rv = SourceHandler (ref isSilenceBool, ref Unsafe.AsRef<AudioTimeStamp> ((void*) timestamp), frameCount, buffers, evt);
				*isSilencePtr = isSilenceBool ? (byte) 1 : (byte) 0;
			} catch (Exception e) {
				evt.SetException (e);
			}
			return rv;
		}
	}
}
