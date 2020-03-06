using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Logging;

namespace Xharness.Listeners
{
	public abstract class SimpleListener : IDisposable
	{
		ILog output_writer;
		string xml_data;

		TaskCompletionSource<bool> stopped = new TaskCompletionSource<bool> ();
		TaskCompletionSource<bool> connected = new TaskCompletionSource<bool> ();

		public IPAddress Address { get; set; }
		public int Port { get; set; }
		public ILog Log { get; set; }
		public ILog TestLog { get; set; }
		public bool AutoExit { get; set; }
		public bool XmlOutput { get; set; }

		public Task ConnectedTask {
			get { return connected.Task; }
		}

		public abstract void Initialize ();
		protected abstract void Start ();
		protected abstract void Stop ();

		public ILog OutputWriter {
			get {
				return output_writer;
			}
		}

		protected void Connected (string remote)
		{
			Log.WriteLine ("Connection from {0} saving logs to {1}", remote, TestLog.FullPath);
			connected.TrySetResult (true);

			if (output_writer == null) {
				output_writer = TestLog;
				// a few extra bits of data only available from this side
				var local_data =
$@"[Local Date/Time:	{DateTime.Now}]
[Remote Address:	{remote}]";
				if (XmlOutput) {
					// This can end up creating invalid xml randomly:
					// https://github.com/xamarin/maccore/issues/827
					//xml_data = local_data;
				} else {
					output_writer.WriteLine (local_data);
				}
			}
		}

		protected void Finished (bool early_termination = false)
		{
			if (stopped.TrySetResult (early_termination)) {
				if (early_termination) {
					Log.WriteLine ("Tests were terminated before completion");
				} else {
					Log.WriteLine ("Tests have finished executing");
				}
				if (xml_data != null) {
					output_writer.WriteLine ($"<!-- \n{xml_data}\n -->");
					output_writer.Flush ();
					xml_data = null;
				}
			}
		}


		public void StartAsync ()
		{
			var t = new Thread (() => {
				try {
					Start ();
				} catch (Exception e) {
					Console.WriteLine ($"{GetType ().Name}: an exception occurred in processing thread: {e}");
				}
			})
			{
				IsBackground = true,
			};
			t.Start ();
		}

		public bool WaitForCompletion (TimeSpan ts)
		{
			return stopped.Task.Wait (ts);
		}

		public Task CompletionTask {
			get {
				return stopped.Task;
			}
		}

		public void Cancel ()
		{
			connected.TrySetCanceled ();
			try {
				// wait a second just in case more data arrives.
				if (!stopped.Task.Wait (TimeSpan.FromSeconds (1)))
					Stop ();
			} catch {
				// We might have stopped already, so just ignore any exceptions.
			}
		}

#region IDisposable Support
		protected virtual void Dispose (bool disposing)
		{
			if (output_writer != null)
				output_writer.Dispose ();
		}

		public void Dispose ()
		{
			Dispose (true);
		}
#endregion

	}
}

