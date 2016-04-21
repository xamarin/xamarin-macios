#if XAMARIN_APPLETLS
//
// AsyncProtocolRequest.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc.
//
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using SD = System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace XamCore.Security.Tls
{
	delegate AsyncOperationStatus AsyncOperation (AsyncProtocolRequest asyncRequest, AsyncOperationStatus status);

	class BufferOffsetSize
	{
		public byte[] Buffer;
		public int Offset;
		public int Size;
		public int TotalBytes;
		public bool Complete;

		public int EndOffset {
			get { return Offset + Size; }
		}

		public int Remaining {
			get { return Buffer.Length - Offset - Size; }
		}

		public BufferOffsetSize (byte[] buffer, int offset, int size)
		{
			Buffer = buffer;
			Offset = offset;
			Size = size;
			Complete = false;
		}

		public void Reset ()
		{
			Offset = Size = 0;
			TotalBytes = 0;
			Complete = false;
		}

		public override string ToString ()
		{
			return string.Format ("[BufferOffsetSize: {0} {1}]", Offset, Size);
		}
	}

	enum AsyncOperationStatus {
		NotStarted,
		Initialize,
		Continue,
		Running,
		Complete,
		WantRead,
		WantWrite,
		ReadDone
	}

	class AsyncProtocolRequest
	{
		public readonly MobileAuthenticatedStream Parent;
		public readonly BufferOffsetSize UserBuffer;

		int RequestedSize;
		public int CurrentSize;
		public int UserResult;

		AsyncOperation Operation;
		int Status;

		public readonly LazyAsyncResult UserAsyncResult;

		public AsyncProtocolRequest (MobileAuthenticatedStream parent, LazyAsyncResult lazyResult, BufferOffsetSize userBuffer = null)
		{
			Parent = parent;
			UserAsyncResult = lazyResult;
			UserBuffer = userBuffer;
		}

		public bool CompleteWithError (Exception ex)
		{
			Status = (int)AsyncOperationStatus.Complete;
			if (UserAsyncResult == null)
				return true;
			if (!UserAsyncResult.InternalPeekCompleted)
				UserAsyncResult.InvokeCallback (ex);
			return false;
		}

		internal void RequestRead (int size)
		{
			var oldStatus = (AsyncOperationStatus)Interlocked.CompareExchange (ref Status, (int)AsyncOperationStatus.WantRead, (int)AsyncOperationStatus.Running);
			Parent.Debug ("RequestRead: {0} {1}", oldStatus, size);
			if (oldStatus == AsyncOperationStatus.Running)
				RequestedSize = size;
			else if (oldStatus == AsyncOperationStatus.WantRead)
				RequestedSize += size;
			else if (oldStatus != AsyncOperationStatus.WantWrite)
				throw new InvalidOperationException ();
		}

		internal void RequestWrite ()
		{
			var oldStatus = (AsyncOperationStatus)Interlocked.CompareExchange (ref Status, (int)AsyncOperationStatus.WantWrite, (int)AsyncOperationStatus.Running);
			if (oldStatus == AsyncOperationStatus.Running)
				return;
			else if (oldStatus != AsyncOperationStatus.WantRead && oldStatus != AsyncOperationStatus.WantWrite)
				throw new InvalidOperationException ();
		}

		internal void StartOperation (AsyncOperation operation)
		{
			if (Interlocked.CompareExchange (ref Status, (int)AsyncOperationStatus.Initialize, (int)AsyncOperationStatus.NotStarted) != (int)AsyncOperationStatus.NotStarted)
				throw new InvalidOperationException ();

			Operation = operation;

			if (UserAsyncResult == null) {
				StartOperation ();
				return;
			}

			ThreadPool.QueueUserWorkItem (_ => StartOperation ());
		}

		void StartOperation ()
		{
			try {
				ProcessOperation ();
				if (UserAsyncResult != null && !UserAsyncResult.InternalPeekCompleted)
					UserAsyncResult.InvokeCallback (UserResult);
			} catch (Exception ex) {
				if (UserAsyncResult == null)
					throw;
				if (!UserAsyncResult.InternalPeekCompleted)
					UserAsyncResult.InvokeCallback (ex);
			}
		}

		void ProcessOperation ()
		{
			AsyncOperationStatus status;
			do {
				status = (AsyncOperationStatus)Interlocked.Exchange (ref Status, (int)AsyncOperationStatus.Running);

				Parent.Debug ("ProcessOperation: {0}", status);

				status = ProcessOperation (status);

				var oldStatus = (AsyncOperationStatus)Interlocked.CompareExchange (ref Status, (int)status, (int)AsyncOperationStatus.Running);
				Parent.Debug ("ProcessOperation done: {0} -> {1}", oldStatus, status);

				if (oldStatus != AsyncOperationStatus.Running) {
					if (status == oldStatus || status == AsyncOperationStatus.Continue || status == AsyncOperationStatus.Complete)
						status = oldStatus;
					else
						throw new InvalidOperationException ();
				}
			} while (status != AsyncOperationStatus.Complete);
		}

		AsyncOperationStatus ProcessOperation (AsyncOperationStatus status)
		{
			if (status == AsyncOperationStatus.WantRead) {
				if (RequestedSize < 0)
					throw new InvalidOperationException ();
				else if (RequestedSize == 0)
					return AsyncOperationStatus.Continue;

				Parent.Debug ("ProcessOperation - read inner: {0}", RequestedSize);
				var ret = Parent.InnerRead (RequestedSize);
				Parent.Debug ("ProcessOperation - read inner done: {0} - {1}", RequestedSize, ret);

				if (ret < 0)
					return AsyncOperationStatus.ReadDone;

				RequestedSize -= ret;

				if (ret == 0 || RequestedSize == 0)
					return AsyncOperationStatus.Continue;
				else
					return AsyncOperationStatus.WantRead;
			} else if (status == AsyncOperationStatus.WantWrite) {
				Parent.InnerWrite ();
				return AsyncOperationStatus.Continue;
			} else if (status == AsyncOperationStatus.Initialize || status == AsyncOperationStatus.Continue) {
				Parent.Debug ("ProcessOperation - continue");
				status = Operation (this, status);
				Parent.Debug ("ProcessOperation - continue done: {0}", status);
				return status;
			} else if (status == AsyncOperationStatus.ReadDone) {
				Parent.Debug ("ProcessOperation - read done");
				status = Operation (this, status);
				Parent.Debug ("ProcessOperation - read done: {0}", status);
				return status;
			}

			throw new InvalidOperationException ();
		}
	}
}
#endif

