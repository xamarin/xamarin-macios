using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	public class DisposableTempDirectory : IDisposable {
		// If directories are deleted at process startup (for previous
		// processes), or when the DisposableTempDirectory instance is
		// disposed/collected. Deleting at process startup is useful when
		// debugging tests: you can examine temporary files after the test has
		// completed.
		static readonly bool delete_on_launch;
		// If paths are deterministic (the nth request always gets the same
		// path). Deterministic paths are useful when debugging tests:
		// temporary files end up in the same location every time you run a
		// test from the IDE.
		static readonly bool deterministic;
		static int counter;
		static readonly string root;

		static DisposableTempDirectory ()
		{
			root = Path.Combine (Path.GetTempPath (), "nnyeah");
#if DEBUG
			// Default to the helpful values when running unit tests in the IDE.
			// This is only something we want possible in DEBUG mode
			if (Assembly.GetEntryAssembly () is null) {
				delete_on_launch = true;
				deterministic = true;
				var assemblyPath = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location) ?? throw new NotSupportedException ();
				root = Path.Combine (assemblyPath, "nnyeah-tests");
			}
			if (delete_on_launch && Directory.Exists (root))
				Directory.Delete (root, true);
#endif
			Directory.CreateDirectory (root);
		}

		public DisposableTempDirectory (string? directoryName = null, bool prependGuid = true)
		{
			var unique = deterministic ? Interlocked.Increment (ref counter).ToString () : Guid.NewGuid ().ToString ();
			if (directoryName is not null)
				directoryName = unique + directoryName;

			directoryName = directoryName ?? unique;

			this.DirectoryPath = Path.Combine (root, directoryName);

			Directory.CreateDirectory (this.DirectoryPath);
		}

		#region IDisposable implementation

		~DisposableTempDirectory ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!delete_on_launch)
				RemoveTempDirectoryAndContents ();
		}

		#endregion

		void RemoveTempDirectoryAndContents ()
		{
			Directory.Delete (DirectoryPath, true);
		}

		public string DirectoryPath { get; private set; }
	}
}
