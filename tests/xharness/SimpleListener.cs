using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace xharness
{
	public abstract class SimpleListener : IDisposable
	{
		StreamWriter output_writer;

		protected ManualResetEvent stopped = new ManualResetEvent (false);
		protected ManualResetEvent connected = new ManualResetEvent (false);

		public IPAddress Address { get; set; }
		public int Port { get; set; }
		public Log Log { get; set; }
		public LogStream TestLog { get; set; }
		public bool AutoExit { get; set; }

		public abstract void Initialize ();
		protected abstract void Start ();
		protected abstract void Stop ();

		public StreamWriter OutputWriter {
			get {
				return output_writer;
			}
		}

		protected void Connected (string remote)
		{
			Log.WriteLine ("Connection from {0} saving logs to {1}", remote, TestLog.FullPath);
			connected.Set ();

			if (output_writer == null) {
				output_writer = TestLog.GetWriter ();

				// a few extra bits of data only available from this side
				output_writer.WriteLine ($"[Local Date/Time:\t{DateTime.Now}]");
				output_writer.WriteLine ($"[Remote Address:\t{remote}]");
			}
		}


		public void StartAsync ()
		{
			var t = new Thread (Start)
			{
				IsBackground = true,
			};
			t.Start ();
		}

		public bool WaitForConnection (TimeSpan ts)
		{
			return connected.WaitOne (ts);
		}

		public bool WaitForCompletion (TimeSpan ts)
		{
			return stopped.WaitOne (ts);
		}

		public void Cancel ()
		{
			try {
				// wait a second just in case more data arrives.
				if (!stopped.WaitOne (TimeSpan.FromSeconds (1)))
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

