//
// FSEvents.cs: binding for CoreService's FSEvents API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc

#if MONOMAC

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace CoreServices
{
	// FSEvents.h: typedef UInt32                          FSEventStreamCreateFlags;
	[Flags]
	public enum FSEventStreamCreateFlags : uint {
		None  = 0x00000000,
		/*UseCFTypes = 0x00000001,*/
		NoDefer = 0x00000002,
		WatchRoot = 0x00000004,
		IgnoreSelf = 0x00000008,
		FileEvents = 0x00000010,
		[Mac (10,9)]
		MarkSelf = 0x00000020,
		[Mac (10,13)]
		UseExtendedData = 0x00000040,
	}

	// FSEvents.h: typedef UInt32                          FSEventStreamEventFlags;
	[Flags]
	public enum FSEventStreamEventFlags : uint {
		None = 0x00000000,
		MustScanSubDirs = 0x00000001,
		UserDropped = 0x00000002,
		KernelDropped = 0x00000004,
		EventIdsWrapped = 0x00000008,
		HistoryDone = 0x00000010,
		RootChanged = 0x00000020,
		Mount = 0x00000040,
		Unmount = 0x00000080,
		ItemCreated = 0x00000100,
		ItemRemoved = 0x00000200,
		ItemInodeMetaMod = 0x00000400,
		ItemRenamed = 0x00000800,
		ItemModified = 0x00001000,
		ItemFinderInfoMod = 0x00002000,
		ItemChangeOwner = 0x00004000,
		ItemXattrMod = 0x00008000,
		ItemIsFile = 0x00010000,
		ItemIsDir = 0x00020000,
		ItemIsSymlink = 0x00040000,
		OwnEvent  = 0x00080000,
		ItemIsHardlink = 0x00100000,
		ItemIsLastHardlink = 0x00200000,
		[Mac (10,14, onlyOn64: true)]
		ItemCloned = 0x00400000,
	}

	public struct FSEvent
	{
		public ulong Id { get; internal set; }
		public string Path { get; internal set; }
		public FSEventStreamEventFlags Flags { get; internal set; }

		public override string ToString ()
		{
			return String.Format ("[FSEvent: Id={0}, Path={1}, Flags={2}]", Id, Path, Flags);
		}

		public const ulong SinceNowId = UInt64.MaxValue;

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr FSEventsCopyUUIDForDevice (ulong device);

		public static Guid GetUuidForDevice (ulong device)
		{
			if (device <= 0) {
				return Guid.Empty;
			}

			var uuidRef = FSEventsCopyUUIDForDevice (device);
			if (uuidRef == IntPtr.Zero) {
				return Guid.Empty;
			}

			return (Guid)Marshal.PtrToStructure (uuidRef, typeof (Guid));
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern ulong FSEventsGetCurrentEventId ();

		public static ulong CurrentEventId {
			get { return FSEventsGetCurrentEventId (); }
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern ulong FSEventsGetLastEventIdForDeviceBeforeTime (
			ulong device, double timeInSecondsSinceEpoch);

		public static ulong GetLastEventIdForDeviceBeforeTime (ulong device, double timeInSecondsSinceEpoch)
		{
			return FSEventsGetLastEventIdForDeviceBeforeTime (device, timeInSecondsSinceEpoch);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern bool FSEventsPurgeEventsForDeviceUpToEventId (ulong device, ulong eventId);

		public static bool PurgeEventsForDeviceUpToEventId (ulong device, ulong eventId)
		{
			return FSEventsPurgeEventsForDeviceUpToEventId (device, eventId);
		}
	}

	public delegate void FSEventStreamEventsHandler (object sender, FSEventStreamEventsArgs args);

	public sealed class FSEventStreamEventsArgs : EventArgs
	{
		public FSEvent [] Events { get; private set; }

		internal FSEventStreamEventsArgs (FSEvent [] events)
		{
			Events = events;
		}
	}

	public class FSEventStream : INativeObject, IDisposable
	{
		IntPtr handle;
		FSEventStreamCallback eventsCallback;

		public IntPtr Handle {
			get { return handle; }
		}

		~FSEventStream ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamRelease (IntPtr handle);

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				FSEventStreamRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		void CheckDisposed ()
		{
			if (handle == IntPtr.Zero) {
				throw new ObjectDisposedException ("this");
			}
		}

		delegate void FSEventStreamCallback (IntPtr handle, IntPtr userData, nint numEvents,
 			IntPtr eventPaths, IntPtr eventFlags, IntPtr eventIds);

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr FSEventStreamCreate (IntPtr allocator,
			FSEventStreamCallback callback, IntPtr context, IntPtr pathsToWatch,
			ulong sinceWhen, double latency, FSEventStreamCreateFlags flags);

		public FSEventStream (CFAllocator allocator, NSArray pathsToWatch,
			ulong sinceWhenId, TimeSpan latency, FSEventStreamCreateFlags flags)
		{
			if (pathsToWatch == null) {
				throw new ArgumentNullException ("pathsToWatch");
			}

			eventsCallback = new FSEventStreamCallback (EventsCallback);

			handle = FSEventStreamCreate (
				allocator ==  null ? IntPtr.Zero : allocator.Handle,
				eventsCallback, IntPtr.Zero, pathsToWatch.Handle,
				sinceWhenId, latency.TotalSeconds, flags | (FSEventStreamCreateFlags)0x1 /* UseCFTypes */);

			if (handle == IntPtr.Zero) {
				throw new Exception ("Unable to create FSEventStream");
			}
		}

		public FSEventStream (string [] pathsToWatch, TimeSpan latency, FSEventStreamCreateFlags flags)
			: this (null, NSArray.FromStrings (pathsToWatch), FSEvent.SinceNowId, latency, flags)
		{
		}

		void EventsCallback (IntPtr handle, IntPtr userData, nint numEvents,
			IntPtr eventPaths, IntPtr eventFlags, IntPtr eventIds)
		{
			if (numEvents == 0) {
				return;
			}

			var events = new FSEvent[numEvents];
			var pathArray = new CFArray (eventPaths);

			for (int i = 0; i < events.Length; i++) {
				events[i].Flags = (FSEventStreamEventFlags)(uint)Marshal.ReadInt32 (eventFlags, i * 4);
				events[i].Id = (uint)Marshal.ReadInt64 (eventIds, i * 8);
				using (var cfstr = new CFString (pathArray.GetValue (i))) {
					events[i].Path = cfstr.ToString ();
				}
			}

			OnEvents (events);
		}

		public event FSEventStreamEventsHandler Events;

		protected virtual void OnEvents (FSEvent [] events)
		{
			var handler = Events;
			if (handler != null) {
				handler (this, new FSEventStreamEventsArgs (events));
			}
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr FSEventStreamCopyDescription (IntPtr handle);

		public string Description {
			get {
				if (handle == IntPtr.Zero) {
					return null;
				}

				var strPtr = FSEventStreamCopyDescription (handle);
				if (strPtr == IntPtr.Zero) {
					return null;
				}

				using (var str = new CFString (strPtr, true)) {
					return str.ToString ();
				}
			}
		}

		public override string ToString ()
		{
			return Description;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamShow (IntPtr handle);

		public void Show ()
		{
			CheckDisposed ();
			FSEventStreamShow (handle);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern bool FSEventStreamStart (IntPtr handle);

		public bool Start ()
		{
			CheckDisposed ();
			return FSEventStreamStart (handle);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamStop (IntPtr handle);

		public void Stop ()
		{
			CheckDisposed ();
			FSEventStreamStop (handle);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamScheduleWithRunLoop (IntPtr handle,
			IntPtr runLoop, IntPtr runLoopMode);

		public void ScheduleWithRunLoop (CFRunLoop runLoop, NSString runLoopMode)
		{
			CheckDisposed ();
			FSEventStreamScheduleWithRunLoop (handle, runLoop.Handle, runLoopMode.Handle);
		}

		public void ScheduleWithRunLoop (CFRunLoop runLoop)
		{
			ScheduleWithRunLoop (runLoop, CFRunLoop.ModeDefault);
		}

		public void ScheduleWithRunLoop (NSRunLoop runLoop, NSString runLoopMode)
		{
			ScheduleWithRunLoop (runLoop.GetCFRunLoop (), runLoopMode);
		}

		public void ScheduleWithRunLoop (NSRunLoop runLoop)
		{
			ScheduleWithRunLoop (runLoop.GetCFRunLoop (), CFRunLoop.ModeDefault);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr FSEventStreamCopyPathsBeingWatched (IntPtr handle);

		public string [] PathsBeingWatched {
			get {
				CheckDisposed ();
				var cfarray = new CFArray (FSEventStreamCopyPathsBeingWatched (handle), true);
				var paths = new string[cfarray.Count];
				for (int i = 0; i < paths.Length; i++) {
					using (var cfstr = new CFString (cfarray.GetValue (i), true)) {
						paths[i] = cfstr.ToString ();
					}
				}
				return paths;
			}
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern uint FSEventStreamFlushAsync (IntPtr handle);

		public uint FlushAsync ()
		{
			CheckDisposed ();
			return FSEventStreamFlushAsync (handle);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamFlushSync (IntPtr handle);

		public void FlushSync ()
		{
			CheckDisposed ();
			FSEventStreamFlushSync (handle);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamInvalidate (IntPtr handle);

		public void Invalidate ()
		{
			CheckDisposed ();
			FSEventStreamInvalidate (handle);
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern ulong FSEventStreamGetLatestEventId (IntPtr handle);

		public ulong LatestEventId {
			get {
				CheckDisposed ();
				return FSEventStreamGetLatestEventId (handle);
			}
		}
	}
}

#endif // MONOMAC
