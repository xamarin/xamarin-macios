// 
// BackingField.cs:
//
//  Helper to convert between an IntPtr and a non-'generator'd managed 
//  representation with intelligent caching of the managed rep.  
//  This allows repeated calls to a property getter to avoid recreating new
//  instances of the managed representation:
//
//    ABNewPersonViewController a = ...;
//    var p = a.DisplayedPerson;  // OK; may create a new instance.
//    p = a.DisplayedPerson;      // OK; returns previous version IFF the
//                                //     handle hasn't changed in the meantime.
//    p = GetPerson();
//    a.DisplayedPerson = p;      // OK; flushes handle
//
// This can be supported by following a simple pattern:
//
//   class ABNewPersonViewController {
//      ABPerson displayedPerson;
//      public ABPerson DisplayedPerson {
//        get {
//			MarkDirty ();
//			return BackingField.Get (ref displayedPerson, _DisplayedPerson, h => new ABPerson (h));
//		}
//        set {
//			_DisplayedPerson = BackingField.Save (ref displayedPerson, value);
//			MarkDirty ();
//		  }
//      }
//   }
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
// Copyright 2011 Xamarin Inc. All rights reserved
//

#if IOS

using System;

using ObjCRuntime;

namespace ObjCRuntime {

	static class BackingField
	{
		public static T Get<T> (ref T value, IntPtr handle, Converter<IntPtr, T> c)
			where T : class, INativeObject, IDisposable
		{
			if (handle == IntPtr.Zero) {
				if (value is not null)
					value.Dispose ();
				return value = null;
			}
			if (value is not null) {
				if (handle == value.Handle)
					return value;
				value.Dispose ();
			}
			return value = c (handle);
		}

		public static IntPtr Save<T> (ref T value, T newValue)
			where T : class, INativeObject, IDisposable
		{
			if (object.ReferenceEquals (value, newValue))
				return value is null ? IntPtr.Zero : value.Handle;
			if (value is not null)
				value.Dispose ();
			value = newValue;
			return value is null ? IntPtr.Zero : value.Handle;
		}
	}
}

#endif // IOS
