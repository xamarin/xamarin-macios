//
// SCNSceneSource.cs: extensions to SCNSceneSource
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.SceneKit
{
	public partial class SCNSceneSource {

		public NSObject GetEntryWithIdentifier<T> (string uid)
		{
			return GetEntryWithIdentifier (uid, new Class (typeof (T)));
		}

		public string [] GetIdentifiersOfEntries<T> ()
		{
			return NSArray.StringArrayFromHandle (Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("identifiersOfEntriesWithClass:"), new Class (typeof(T)).Handle));
		}
	}
}
