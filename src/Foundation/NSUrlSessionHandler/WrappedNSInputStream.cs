using System;
using System.IO;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	class WrappedNSInputStream : NSInputStream
	{
		NSStreamStatus status;
		CFRunLoopSource source;
		readonly Stream stream;
		bool notifying;

		public WrappedNSInputStream (Stream inputStream)
		{
			status = NSStreamStatus.NotOpen;
			stream = inputStream;
			source = new CFRunLoopSource (Handle, false);
		}

		public override NSStreamStatus Status => status;

		public override void Open ()
		{
			status = NSStreamStatus.Open;
			Notify (CFStreamEventType.OpenCompleted);
		}

		public override void Close ()
		{
			status = NSStreamStatus.Closed;
		}

		public override nint Read (IntPtr buffer, nuint len)
		{
			var sourceBytes = new byte [len];
			var read = stream.Read (sourceBytes, 0, (int)len);
			Marshal.Copy (sourceBytes, 0, buffer, (int)len);

			if (notifying)
				return read;

			notifying = true;
			if (stream.CanSeek && stream.Position == stream.Length) {
				Notify (CFStreamEventType.EndEncountered);
				status = NSStreamStatus.AtEnd;
			}
			notifying = false;

			return read;
		}

		public override bool HasBytesAvailable ()
		{
			return true;
		}

		protected override bool GetBuffer (out IntPtr buffer, out nuint len)
		{
			// Just call the base implemention (which will return false)
			return base.GetBuffer (out buffer, out len);
		}

		// NSInvalidArgumentException Reason: *** -propertyForKey: only defined for abstract class.  Define -[System_Net_Http_NSUrlSessionHandler_WrappedNSInputStream propertyForKey:]!
		protected override NSObject? GetProperty (NSString key)
		{
			return null;
		}

		protected override bool SetProperty (NSObject? property, NSString key)
		{
			return false;
		}

		protected override bool SetCFClientFlags (CFStreamEventType inFlags, IntPtr inCallback, IntPtr inContextPtr)
		{
			// Just call the base implementation, which knows how to handle everything.
			return base.SetCFClientFlags (inFlags, inCallback, inContextPtr);
		}

#if NET
		public override void Schedule (NSRunLoop aRunLoop, NSString nsMode)
#else
		public override void Schedule (NSRunLoop aRunLoop, string mode)
#endif
		{
			var cfRunLoop = aRunLoop.GetCFRunLoop ();
#if !NET
			var nsMode = new NSString (mode);
#endif

			cfRunLoop.AddSource (source, nsMode);

			if (notifying)
				return;

			notifying = true;
			Notify (CFStreamEventType.HasBytesAvailable);
			notifying = false;
		}

#if NET
		public override void Unschedule (NSRunLoop aRunLoop, NSString nsMode)
#else
		public override void Unschedule (NSRunLoop aRunLoop, string mode)
#endif
		{
			var cfRunLoop = aRunLoop.GetCFRunLoop ();
#if !NET
			var nsMode = new NSString (mode);
#endif

			cfRunLoop.RemoveSource (source, nsMode);
		}

		protected override void Dispose (bool disposing)
		{
			stream?.Dispose ();
		}
	}
}
