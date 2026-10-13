// 
// ABPeoplePickerNavigationController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using System.Collections;
using System.Collections.Generic;

using AddressBook;

namespace AddressBookUI {

	delegate T ABFunc<T> ();

	/// <summary>A collection of <see cref="AddressBook.ABPersonProperty" />s returned by the <see cref="AddressBookUI.ABPeoplePickerNavigationController.DisplayedProperties" /> and <see cref="AddressBookUI.ABPersonViewController.DisplayedProperties" /> properties.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class DisplayedPropertiesCollection : ICollection<ABPersonProperty> {

		ABFunc<NSNumber []?> g;
		Action<NSNumber []?> s;

		internal DisplayedPropertiesCollection (ABFunc<NSNumber []?> g, Action<NSNumber []?> s)
		{
			this.g = g;
			this.s = s;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int Count {
			get { return g ()!.Length; }
		}

		bool ICollection<ABPersonProperty>.IsReadOnly {
			get { return false; }
		}

		/// <param name="item">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void Clear ()
		{
			s (new NSNumber [0]);
		}

		/// <param name="item">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="array">To be added.</param>
		///         <param name="arrayIndex">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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

		/// <param name="item">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
