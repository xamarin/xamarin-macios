using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace xharness
{
	public class SimpleFileListener : SimpleListener
	{
		Thread processor_thread;
		bool cancel;

		protected override void Stop ()
		{
			cancel = true;
			processor_thread.Join ();
			processor_thread = null;
			stopped.Set ();
		}

		public override void Initialize ()
		{
			processor_thread = new Thread (Processing);
		}

		protected override void Start ()
		{
			processor_thread.Start ();
		}

		void Processing ()
		{
			var path = TestLog.FullPath;
			Connected ("?");
			using (var fs = new BlockingFileStream (path) { Listener = this }) {
				using (var reader = new StreamReader (fs)) {
					string line;
					while ((line = reader.ReadLine ()) != null) {
						if (line.StartsWith ("[Runner executing:", StringComparison.Ordinal)) {
							Console.WriteLine ("Tests have started executing");
						} else if (line.StartsWith ("Tests run: ", StringComparison.Ordinal)) {
							Console.WriteLine ("Tests have finished executing");
							stopped.Set ();
							return;
						}
					}
				}
			}
		}

		class BlockingFileStream : FileStream
		{
			public SimpleFileListener Listener;

			long last_position;

			public BlockingFileStream (string path)
				: base (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
			{
			}

			public override int Read (byte [] array, int offset, int count)
			{
				while (last_position == base.Length && !Listener.cancel)
					Thread.Sleep (25);
				var rv = base.Read (array, offset, count);
				last_position += rv;
				return rv;
			}
		}
	}
}

