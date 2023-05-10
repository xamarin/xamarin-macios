
using System;
using System.IO;

namespace Foundation {
	public partial class NSUrlSessionDownloadTaskRequest : IDisposable {
		string tmpfile;

		partial void Initialize ()
		{
			// Location points to a temporary file on disk which is deleted
			// which is deleted when returning from the delegate callback.
			//
			// iOS docs recommend to open the file for reading or moving
			// it, but we're left with only moving it, since iOS will
			// call 'unlink' to remove the file upon return, which means
			// that even if you've opened the file, you can only access it
			// using the file handle, and we've exposed the file name to
			// the user (which would still be unusable).
			//
			// So instead move the file so that iOS doesn't delete it,
			// expose the new filename, and keep it alive as long as this
			// object is alive.
			//
			// See bug #31427.

			tmpfile = Path.GetTempFileName ();
			File.Delete (tmpfile);
			File.Move (Location.Path, tmpfile);
			Location = NSUrl.FromFilename (tmpfile);
		}

		~NSUrlSessionDownloadTaskRequest ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected void Dispose (bool disposing)
		{
			if (tmpfile is not null) {
				try {
					File.Delete (tmpfile);
				} catch {
					// We don't care if we can't delete the tmp file.
				}
				tmpfile = null;
			}
		}
	}
}
