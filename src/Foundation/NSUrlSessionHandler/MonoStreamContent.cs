//
// Copied from https://github.com/mono/mono/blob/2019-02/mcs/class/System.Net.Http/System.Net.Http/StreamContent.cs.
//
// This is not a perfect solution, but the most robust and risk-free approach.
//
// The implementation depends on Mono-specific behavior, which makes SerializeToStreamAsync() cancellable.
// Unfortunately, the CoreFX implementation of HttpClient does not support this.
//
// By copying Mono's old implementation here, we ensure that we're compatible with both HttpClient implementations,
// so when we eventually adopt the CoreFX version in all of Mono's profiles, we don't regress here.
//
using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	class MonoStreamContent : HttpContent
	{
		readonly Stream content;
		readonly int bufferSize;
		readonly CancellationToken cancellationToken;
		readonly long startPosition;
		bool contentCopied;

		public MonoStreamContent (Stream content)
			: this (content, 16 * 1024)
		{
		}

		public MonoStreamContent (Stream content, int bufferSize)
		{
			if (content is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (content));

			if (bufferSize <= 0)
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (bufferSize), bufferSize, "Buffer size must be >0");

			this.content = content;
			this.bufferSize = bufferSize;

			if (content.CanSeek) {
				startPosition = content.Position;
			}
		}

		//
		// Workarounds for poor .NET API
		// Instead of having SerializeToStreamAsync with CancellationToken as public API. Only LoadIntoBufferAsync
		// called internally from the send worker can be cancelled and user cannot see/do it
		//
		internal MonoStreamContent (Stream content, CancellationToken cancellationToken)
			: this (content)
		{
			// We don't own the token so don't worry about disposing it
			this.cancellationToken = cancellationToken;
		}

		protected override Task<Stream> CreateContentReadStreamAsync ()
		{
			return Task.FromResult (content);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				content.Dispose ();
			}

			base.Dispose (disposing);
		}

		protected override Task SerializeToStreamAsync (Stream stream, TransportContext? context)
		{
			if (contentCopied) {
				if (!content.CanSeek) {
					throw new InvalidOperationException ("The stream was already consumed. It cannot be read again.");
				}

				content.Seek (startPosition, SeekOrigin.Begin);
			} else {
				contentCopied = true;
			}

			return content.CopyToAsync (stream, bufferSize, cancellationToken);
		}

#if !NET
		internal
#endif
		protected override bool TryComputeLength (out long length)
		{
			if (!content.CanSeek) {
				length = 0;
				return false;
			}
			length = content.Length - startPosition;
			return true;
		}
	}

}
