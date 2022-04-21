//
// FSEvents.cs: binding for CoreService's FSEvents API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc

#nullable enable

#if MONOMAC

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
#if !NET
		[Mac (10,9)]
#endif
		MarkSelf = 0x00000020,
#if !NET
		[Mac (10,13)]
#endif
		UseExtendedData = 0x00000040,
#if !NET
		[Mac (11,0)]
#else
		[SupportedOSPlatform ("macos11.0")]
#endif
		FullHistory = 0x00000080,
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
#if !NET
		[Mac (10,14)]
#endif
		ItemCloned = 0x00400000,
	}

	public struct FSEvent
	{
		public ulong Id { get; internal set; }
		public string? Path { get; internal set; }
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

			return (Guid)Marshal.PtrToStructure (uuidRef, typeof (Guid))!;
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
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool FSEventsPurgeEventsForDeviceUpToEventId (ulong device, ulong eventId);

		public static bool PurgeEventsForDeviceUpToEventId (ulong device, ulong eventId)
		{
			return FSEventsPurgeEventsForDeviceUpToEventId (device, eventId);
		}
	}

	struct FSEventStreamContext {
		nint version; /* CFIndex: only valid value is zero */
		internal IntPtr Info; /* void * __nullable */
		IntPtr Retain; /* CFAllocatorRetainCallBack __nullable */
#if NET
		internal unsafe delegate* unmanaged<IntPtr, void> Release; /* CFAllocatorReleaseCallBack __nullable */
#else
		internal FSEventStream.ReleaseContextCallback Release; /* CFAllocatorReleaseCallBack __nullable */
#endif
		IntPtr CopyDescription; /* CFAllocatorCopyDescriptionCallBack __nullable */
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

	public class FSEventStream : NativeObject
	{
		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamRetain (IntPtr handle);

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamRelease (IntPtr handle);

		protected internal override void Retain ()
		{
			FSEventStreamRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			FSEventStreamRelease (GetCheckedHandle ());
		}

		delegate void FSEventStreamCallback (IntPtr handle, IntPtr userData, nint numEvents,
 			IntPtr eventPaths, IntPtr eventFlags, IntPtr eventIds);

		[DllImport (Constants.CoreServicesLibrary)]
		unsafe static extern IntPtr FSEventStreamCreate (IntPtr allocator,
#if NET
			delegate* unmanaged<IntPtr, IntPtr, nint, IntPtr, IntPtr, IntPtr, void> callback,
#else
			FSEventStreamCallback callback,
#endif
			ref FSEventStreamContext context, IntPtr pathsToWatch,
			ulong sinceWhen, double latency, FSEventStreamCreateFlags flags);

		public FSEventStream (CFAllocator? allocator, NSArray pathsToWatch,
			ulong sinceWhenId, TimeSpan latency, FSEventStreamCreateFlags flags)
		{
			if (pathsToWatch is null)
				throw new ArgumentNullException (nameof (pathsToWatch));

			var gch = GCHandle.Alloc (this);

			var context = default (FSEventStreamContext);
			context.Info = GCHandle.ToIntPtr (gch);
#if NET
			unsafe {
				context.Release = &FreeGCHandle;
			}
#else
			context.Release = releaseContextCallback;
#endif

			IntPtr handle;
			unsafe {
				handle = FSEventStreamCreate (
					allocator.GetHandle (),
#if NET
					&EventsCallback,
#else
					eventsCallback,
#endif
					ref context, pathsToWatch.Handle,
					sinceWhenId, latency.TotalSeconds, flags | (FSEventStreamCreateFlags)0x1 /* UseCFTypes */);
			}

			InitializeHandle (handle);
		}

		public FSEventStream (string [] pathsToWatch, TimeSpan latency, FSEventStreamCreateFlags flags)
			: this (null, NSArray.FromStrings (pathsToWatch), FSEvent.SinceNowId, latency, flags)
		{
		}

#if !NET
		static readonly FSEventStreamCallback eventsCallback = EventsCallback;

		static readonly ReleaseContextCallback releaseContextCallback = FreeGCHandle;
		internal delegate void ReleaseContextCallback (IntPtr info);
#endif

#if NET
		[UnmanagedCallersOnly]
#endif
		static void FreeGCHandle (IntPtr gchandle)
		{
			GCHandle.FromIntPtr (gchandle).Free ();
		}

#if NET
		[UnmanagedCallersOnly]
#endif
		static void EventsCallback (IntPtr handle, IntPtr userData, nint numEvents,
			IntPtr eventPaths, IntPtr eventFlags, IntPtr eventIds)
		{
			if (numEvents == 0) {
				return;
			}

			var events = new FSEvent[numEvents];
			var pathArray = new CFArray (eventPaths, false);

			for (int i = 0; i < events.Length; i++) {
				events[i].Flags = (FSEventStreamEventFlags)(uint)Marshal.ReadInt32 (eventFlags, i * 4);
				events[i].Id = (uint)Marshal.ReadInt64 (eventIds, i * 8);
				events[i].Path = CFString.FromHandle (pathArray.GetValue (i));
			}

			var instance = GCHandle.FromIntPtr (userData).Target as FSEventStream;
			instance?.OnEvents (events);
		}

		public event FSEventStreamEventsHandler? Events;

		protected virtual void OnEvents (FSEvent [] events)
		{
			var handler = Events;
			if (handler is not null) {
				handler (this, new FSEventStreamEventsArgs (events));
			}
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern IntPtr FSEventStreamCopyDescription (IntPtr handle);

		public string? Description {
			get {
				if (Handle == IntPtr.Zero) {
					return null;
				}

				return CFString.FromHandle (FSEventStreamCopyDescription (Handle), true);
			}
		}

		public override string? ToString ()
		{
			return Description;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamShow (IntPtr handle);

		public void Show ()
		{
			FSEventStreamShow (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreServicesLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool FSEventStreamStart (IntPtr handle);

		public bool Start ()
		{
			return FSEventStreamStart (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamStop (IntPtr handle);

		public void Stop ()
		{
			FSEventStreamStop (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamScheduleWithRunLoop (IntPtr handle,
			IntPtr runLoop, IntPtr runLoopMode);

		public void ScheduleWithRunLoop (CFRunLoop runLoop, NSString runLoopMode)
		{
			FSEventStreamScheduleWithRunLoop (GetCheckedHandle (), runLoop.Handle, runLoopMode.Handle);
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

		public string? []? PathsBeingWatched {
			get {
				var cfarray = FSEventStreamCopyPathsBeingWatched (GetCheckedHandle ());
				if (cfarray == IntPtr.Zero)
					return Array.Empty<string> ();
				return CFArray.StringArrayFromHandle (cfarray, true);
			}
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern uint FSEventStreamFlushAsync (IntPtr handle);

		public uint FlushAsync ()
		{
			return FSEventStreamFlushAsync (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamFlushSync (IntPtr handle);

		public void FlushSync ()
		{
			FSEventStreamFlushSync (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern void FSEventStreamInvalidate (IntPtr handle);

		public void Invalidate ()
		{
			FSEventStreamInvalidate (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreServicesLibrary)]
		static extern ulong FSEventStreamGetLatestEventId (IntPtr handle);

		public ulong LatestEventId {
			get {
				return FSEventStreamGetLatestEventId (GetCheckedHandle ());
			}
		}
	}
}

#endif // MONOMAC
