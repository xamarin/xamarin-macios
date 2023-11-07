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
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace SceneKit {
	public partial class SCNSceneSource {

		public NSObject? GetEntryWithIdentifier<T> (string uid)
		{
			return GetEntryWithIdentifier (uid, new Class (typeof (T)));
		}

		public string [] GetIdentifiersOfEntries<T> ()
		{
			return CFArray.StringArrayFromHandle (Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("identifiersOfEntriesWithClass:"), new Class (typeof (T)).Handle))!;
		}
	}
}
