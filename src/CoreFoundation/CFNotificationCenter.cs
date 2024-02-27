//
// CFNotificaionCenter.cs: Implements CoreFoundation notifications
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//    James Clancey (james.clancey@xamarin.com)
//
// Copyright 2014-2015 Xamarin Inc
//
// At this point, there are only three notification centers, and no new instances
// can be created, so this optimized for that.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using CFNotificationCenterRef = global::System.IntPtr;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System.Collections.Generic;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	[Native] // CFIndex
	public enum CFNotificationSuspensionBehavior : long {
		Drop = 1,
		Coalesce = 2,
		Hold = 3,
		DeliverImmediately = 4
	}

	//
	// Encapsulates all the information needed to maintain a notification, so we do not expose the 
	// inner details of how to remove a notification, and also to get a reliable removal of the
	// notification callback.
	//
	// This is needed because the API itself is not great.
	//
	public class CFNotificationObserverToken {
		internal CFNotificationObserverToken (string stringName)
		{
			this.stringName = stringName;
		}

		internal IntPtr centerHandle;
		internal IntPtr nameHandle;
		internal IntPtr observedObject;
		internal string stringName;
		internal Action<string, NSDictionary?>? listener;
	}


	public class CFNotificationCenter : NativeObject {
		// If this becomes public for some reason, and more than three instances are created, you should revisit the lookup code
		[Preserve (Conditional = true)]
		internal CFNotificationCenter (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		static CFNotificationCenter? darwinnc;
		static CFNotificationCenter? localnc;

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFNotificationCenterRef CFNotificationCenterGetDarwinNotifyCenter ();

		static public CFNotificationCenter Darwin {
			get {
				return darwinnc ?? (darwinnc = new CFNotificationCenter (CFNotificationCenterGetDarwinNotifyCenter (), false));
			}
		}

#if MONOMAC
		static CFNotificationCenter? distributednc;

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFNotificationCenterRef CFNotificationCenterGetDistributedCenter ();

		static public CFNotificationCenter Distributed {
			get {
				return distributednc ?? (distributednc = new CFNotificationCenter (CFNotificationCenterGetDistributedCenter (), false));
			}
		}
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFNotificationCenterRef CFNotificationCenterGetLocalCenter ();

		static public CFNotificationCenter Local {
			get {
				return localnc ?? (localnc = new CFNotificationCenter (CFNotificationCenterGetLocalCenter (), false));
			}
		}

		Dictionary<string, List<CFNotificationObserverToken>> listeners = new Dictionary<string, List<CFNotificationObserverToken>> ();
		const string NullNotificationName = "NullNotificationName";
		public CFNotificationObserverToken AddObserver (string name, INativeObject objectToObserve, Action<string, NSDictionary?> notificationHandler,
								CFNotificationSuspensionBehavior suspensionBehavior = CFNotificationSuspensionBehavior.DeliverImmediately)
		{
			if (darwinnc is not null && darwinnc.Handle == Handle && name is null) {
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name), "When using the Darwin Notification Center, the value passed must not be null");
			}

			var strHandle = CFString.CreateNative (name);
			name = name ?? NullNotificationName;
			var token = new CFNotificationObserverToken (name) {
				centerHandle = Handle,
				nameHandle = strHandle,
				observedObject = objectToObserve.GetHandle (),
				listener = notificationHandler
			};

			//
			// To allow callbacks to add observers, we duplicate the list of listeners on AddObserver
			// We do the duplication on AddObserver, instead of making a copy on the notification
			// callback, as we expect the notification callback to be a more common operation
			// than the AddObserver operation
			//
			List<CFNotificationObserverToken>? listenersForName;
			lock (listeners) {
				if (!listeners.TryGetValue (name, out listenersForName)) {
					listenersForName = new List<CFNotificationObserverToken> (1);
#if NET
					unsafe {
						CFNotificationCenterAddObserver (center: Handle,
									 observer: Handle,
									 callback: &NotificationCallback,
									 name: strHandle,
									 obj: token.observedObject,
									 suspensionBehavior: (IntPtr) suspensionBehavior);
					}
#else
					CFNotificationCenterAddObserver (center: Handle,
									 observer: Handle,
									 callback: NotificationCallback,
									 name: strHandle,
									 obj: token.observedObject,
									 suspensionBehavior: (IntPtr) suspensionBehavior);
#endif
				} else
					listenersForName = new List<CFNotificationObserverToken> (listenersForName);
				listenersForName.Add (token);
				listeners [name] = listenersForName;
			}
			return token;
		}

		void notification (string? name, NSDictionary? userInfo)
		{
			List<CFNotificationObserverToken>? listenersForName;
			List<CFNotificationObserverToken>? nullNotificationListeners;
			bool hasName;
			bool hasNullNotifications;
			lock (listeners) {
				hasName = listeners.TryGetValue (name!, out listenersForName);
				hasNullNotifications = listeners.TryGetValue (NullNotificationName, out nullNotificationListeners);
			}

			// We can iterate over this list, even if the callbacks add or remove observers, because we make copies
			// on add/remove
			if (hasName) {
				foreach (var observer in listenersForName!)
					observer.listener! (name!, userInfo);
			}
			if (hasNullNotifications) {
				foreach (var observer in nullNotificationListeners!)
					observer.listener! (name!, userInfo);
			}
		}

#if !NET
		delegate void CFNotificationCallback (CFNotificationCenterRef center, IntPtr observer, IntPtr name, IntPtr obj, IntPtr userInfo);
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CFNotificationCallback))]
#endif
		static void NotificationCallback (CFNotificationCenterRef centerPtr, IntPtr observer, IntPtr name, IntPtr obj, IntPtr userInfo)
		{
			CFNotificationCenter center;

			if (darwinnc is not null && centerPtr == darwinnc.Handle)
				center = darwinnc;
			else if (localnc is not null && centerPtr == localnc.Handle)
				center = localnc;
#if MONOMAC
			else if (distributednc is not null && centerPtr == distributednc.Handle)
				center = distributednc;
#endif
			else
				return;

			center.notification (CFString.FromHandle (name), Runtime.GetNSObject<NSDictionary> (userInfo));
		}

		public void PostNotification (string notification, INativeObject objectToObserve, NSDictionary? userInfo = null, bool deliverImmediately = false, bool postOnAllSessions = false)
		{
			// The name of the notification to post.This value must not be NULL.
			if (notification is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (notification));

			var strHandle = CFString.CreateNative (notification);
			CFNotificationCenterPostNotificationWithOptions (
				center: Handle,
				name: strHandle,
				obj: objectToObserve.GetHandle (),
				userInfo: userInfo.GetHandle (),
				options: (deliverImmediately ? 1 : 0) | (postOnAllSessions ? 2 : 0));
			CFString.ReleaseNative (strHandle);
		}

		public void RemoveObserver (CFNotificationObserverToken token)
		{
			if (token is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (token));
			if (token.nameHandle == IntPtr.Zero && token.stringName != NullNotificationName)
				throw new ObjectDisposedException (nameof (token));
			if (token.centerHandle != Handle)
				throw new ArgumentException (nameof (token), "This token belongs to a different notification center");
			lock (listeners) {
				var list = listeners [token.stringName];
				List<CFNotificationObserverToken>? newList = null;
				foreach (var e in list) {
					if (e == token)
						continue;
					if (newList is null)
						newList = new List<CFNotificationObserverToken> ();
					newList.Add (e);
				}
				if (newList is not null) {
					listeners [token.stringName] = newList;
					return;
				} else
					listeners.Remove (token.stringName);
			}
			CFNotificationCenterRemoveObserver (Handle, this.Handle, name: token.nameHandle, obj: token.observedObject);
			NSString.ReleaseNative (token.nameHandle);
			token.nameHandle = IntPtr.Zero;
		}

		public void RemoveEveryObserver ()
		{
			lock (listeners) {
				var keys = new string [listeners.Keys.Count];
				listeners.Keys.CopyTo (keys, 0);
				foreach (var key in keys) {
					var l = listeners [key];
					var copy = new List<CFNotificationObserverToken> (l);
					foreach (var e in copy)
						RemoveObserver (e);
				}
				listeners.Clear ();
			}
		}


#if NET
		[DllImport (Constants.CoreFoundationLibrary)]
		static extern unsafe void CFNotificationCenterAddObserver (CFNotificationCenterRef center, IntPtr observer,
									   delegate* unmanaged<CFNotificationCenterRef, IntPtr, IntPtr, IntPtr, IntPtr, void> callback, IntPtr name, IntPtr obj,
									   /* CFNotificationSuspensionBehavior */ IntPtr suspensionBehavior);
#else
		[DllImport (Constants.CoreFoundationLibrary)]
		static extern unsafe void CFNotificationCenterAddObserver (CFNotificationCenterRef center, IntPtr observer,
									   CFNotificationCallback callback, IntPtr name, IntPtr obj,
									   /* CFNotificationSuspensionBehavior */ IntPtr suspensionBehavior);
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern unsafe void CFNotificationCenterPostNotificationWithOptions (CFNotificationCenterRef center, IntPtr name, IntPtr obj, IntPtr userInfo, int options);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern unsafe void CFNotificationCenterRemoveObserver (CFNotificationCenterRef center, IntPtr observer, IntPtr name, IntPtr obj);

	}
}
