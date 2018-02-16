
using System;
using System.IO;

namespace Foundation {
#if !XAMCORE_2_0
	public partial class NSUrlSession {

		NSUrlDownloadSessionResponse GetCompletionHandler (NSUrlSessionResponse completionHandler)
		{
			if (completionHandler == null)
				return null;
			// we can't return an NSData so we return null - just in case existing code did not use it (e.g. only error)
			return delegate (NSUrl location, NSUrlResponse response, NSError error) {
				completionHandler (null, response, error);
			};
		}

		[Obsolete ("Use the override that accept an 'NSUrlDownloadSessionResponse' parameter.")]
		public virtual NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request, NSUrlSessionResponse completionHandler)
		{
			return CreateDownloadTask (request, GetCompletionHandler (completionHandler));
		}

		[Obsolete ("Use the override that accept an 'NSUrlDownloadSessionResponse' parameter.")]
		public virtual NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url, NSUrlSessionResponse completionHandler)
		{
			return CreateDownloadTask (url, GetCompletionHandler (completionHandler));
		}

		[Obsolete ("Use the override that accept an 'NSUrlDownloadSessionResponse' parameter.")]
		public virtual NSUrlSessionDownloadTask CreateDownloadTaskFromResumeData (NSData resumeData, NSUrlSessionResponse completionHandler)
		{
			return CreateDownloadTaskFromResumeData (resumeData, GetCompletionHandler (completionHandler));
		}
	}
#endif

	public partial class NSUrlSessionDownloadTaskRequest : IDisposable {
		string tmpfile;

#if !XAMCORE_2_0
		[Obsolete ("Use the Location property")]
		public NSData Data { get; set; }
#endif

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

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected void Dispose (bool disposing)
		{
			if (tmpfile != null) {
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
