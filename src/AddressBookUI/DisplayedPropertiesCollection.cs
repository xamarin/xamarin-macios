// 
// ABPeoplePickerNavigationController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

using AddressBook;
using Foundation;
using ObjCRuntime;

namespace AddressBookUI {

	delegate T ABFunc<T> ();

#if NET
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class DisplayedPropertiesCollection : ICollection<ABPersonProperty> {

		ABFunc<NSNumber []?> g;
		Action<NSNumber []?> s;

		internal DisplayedPropertiesCollection (ABFunc<NSNumber []?> g, Action<NSNumber []?> s)
		{
			this.g = g;
			this.s = s;
		}

		public int Count {
			get { return g ()!.Length; }
		}

		bool ICollection<ABPersonProperty>.IsReadOnly {
			get { return false; }
		}

		public void Add (ABPersonProperty item)
		{
			List<NSNumber> values;
			var dp = g ();
			if (dp is not null)
				values = new List<NSNumber> (dp);
			else
				values = new List<NSNumber> ();
			values.Add (new NSNumber (ABPersonPropertyId.ToId (item)));
			s (values.ToArray ());
		}

		public void Clear ()
		{
			s (new NSNumber [0]);
		}

		public bool Contains (ABPersonProperty item)
		{
			int id = ABPersonPropertyId.ToId (item);
			var values = g ();
			if (values is null)
				return false;

			for (int i = 0; i < values.Length; ++i)
				if (values [i].Int32Value == id)
					return true;
			return false;
		}

		public void CopyTo (ABPersonProperty [] array, int arrayIndex)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException ("arrayIndex");
			if (arrayIndex > array.Length)
				throw new ArgumentException ("index larger than largest valid index of array");
			if (array.Length - arrayIndex < Count)
				throw new ArgumentException ("Destination array cannot hold the requested elements!");

			var e = GetEnumerator ();
			while (e.MoveNext ())
				array [arrayIndex++] = e.Current;
		}

		public bool Remove (ABPersonProperty item)
		{
			var dp = g ();
			if (dp is null)
				return false;
			var id = ABPersonPropertyId.ToId (item);
			var values = new List<NSNumber> (dp);
			bool found = false;
			for (int i = values.Count - 1; i >= 0; --i)
				if (values [i].Int32Value == id) {
					values.RemoveAt (i);
					found = true;
				}
			if (found)
				s (values.ToArray ());
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<ABPersonProperty> GetEnumerator ()
		{
			var values = g ();
			if (values is null) {
				yield break;
			}
			for (int i = 0; i < values.Length; ++i)
				yield return ABPersonPropertyId.ToPersonProperty (values [i].Int32Value);
		}
	}
}
