using System;
using System.IO;
using System.Threading;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Listeners {
	public class SimpleFileListener : SimpleListener {
		Thread processor_thread;
		bool cancel;
		public string Path { get; private set; }

		public SimpleFileListener (string path, ILog log, ILog testLog, bool xmlOutput)
			: base (log, testLog, xmlOutput)
		{
			Path = path ?? throw new ArgumentNullException (nameof (path));
		}

		protected override void Stop ()
		{
			cancel = true;
			processor_thread.Join ();
			processor_thread = null;
			Finished (true);
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
			Connected ("N/A");
			using (var fs = new BlockingFileStream (Path) { Listener = this }) {
				using (var reader = new StreamReader (fs)) {
					string line;
					while ((line = reader.ReadLine ()) != null) {
						OutputWriter.WriteLine (line);
						if (line.StartsWith ("[Runner executing:", StringComparison.Ordinal)) {
							Log.WriteLine ("Tests have started executing");
						} else if (!XmlOutput && line.StartsWith ("Tests run: ", StringComparison.Ordinal)) {
							Log.WriteLine ("Tests have finished executing");
							Finished ();
							return;
						} else if (XmlOutput && line == "<!-- the end -->") {
							Log.WriteLine ("Tests have finished executing");
							Finished ();
							return;
						}
					}
				}
			}
		}

		class BlockingFileStream : FileStream {
			public SimpleFileListener Listener;

			long last_position;

			public BlockingFileStream (string path)
				: base (path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)
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

