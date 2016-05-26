using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace xharness
{
	public abstract class SimpleListener : IDisposable
	{
		FileStream output_stream;

		protected ManualResetEvent stopped = new ManualResetEvent (false);
		protected ManualResetEvent connected = new ManualResetEvent (false);

		public IPAddress Address { get; set; }
		public int Port { get; set; }
		public string LogPath { get; set; }
		public string LogFile { get; set; } = DateTime.UtcNow.Ticks.ToString () + ".log";
		public bool AutoExit { get; set; }

		public abstract void Initialize ();
		protected abstract void Start ();
		protected abstract void Stop ();

		public string LogFilePath {
			get {
				return Path.Combine (LogPath, LogFile);
			}
		}

		public FileStream OutputStream {
			get {
				return output_stream;
			}
		}

		protected void Connected (string remote)
		{
			Console.WriteLine ("Connection from {0} saving logs to {1}", remote, LogFilePath);
			connected.Set ();

			if (output_stream != null) {
				output_stream.Flush ();
				output_stream.Dispose ();
			}

			var fs = new FileStream (LogFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
			// a few extra bits of data only available from this side
			string header = String.Format ("[Local Date/Time:\t{1}]{0}[Remote Address:\t{2}]{0}",
				Environment.NewLine, DateTime.Now, remote);
			byte [] array = Encoding.UTF8.GetBytes (header);
			fs.Write (array, 0, array.Length);
			fs.Flush ();
			output_stream = fs;
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
			if (output_stream != null)
				output_stream.Dispose ();
		}

		public void Dispose ()
		{
			Dispose (true);
		}
#endregion

	}
}

