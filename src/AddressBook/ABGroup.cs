// 
// ABGroup.cs: Implements the managed ABGroup
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//     
// Copyright (C) 2009 Novell, Inc
// Copyright (C) 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

#nullable enable

#if !MONOMAC

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AddressBook {
	static class ABGroupProperty {

		public static int Name { get; private set; }

		static ABGroupProperty ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			Name = Dlfcn.GetInt32 (Libraries.AddressBook.Handle, "kABGroupNameProperty");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABGroup : ABRecord, IEnumerable<ABRecord> {

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABGroupCreate ();

		public ABGroup ()
			: base (ABGroupCreate (), true)
		{
			InitConstants.Init ();
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABGroupCreateInSource (IntPtr source);

		public ABGroup (ABRecord source)
			: base (IntPtr.Zero, true)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			Handle = ABGroupCreateInSource (source.Handle);
		}

		[Preserve (Conditional = true)]
		internal ABGroup (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal ABGroup (NativeHandle handle, ABAddressBook addressbook)
			: base (handle, false)
		{
			AddressBook = addressbook;
		}

		public string? Name {
			get { return PropertyToString (ABGroupProperty.Name); }
			set { SetValue (ABGroupProperty.Name, value); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABGroupCopySource (IntPtr group);

		public ABRecord? Source {
			get {
				var h = ABGroupCopySource (Handle);
				if (h == IntPtr.Zero)
					return null;

				return FromHandle (h, null);
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		unsafe extern static byte ABGroupAddMember (IntPtr group, IntPtr person, IntPtr* error);
		public void Add (ABRecord person)
		{
			if (person is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (person));
			IntPtr error;
			unsafe {
				if (ABGroupAddMember (Handle, person.Handle, &error) == 0)
					throw CFException.FromCFError (error);
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABGroupCopyArrayOfAllMembers (IntPtr group);

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<ABRecord> GetEnumerator ()
		{
			var cfArrayRef = ABGroupCopyArrayOfAllMembers (Handle);
			IEnumerable<ABRecord>? e = null;
			if (cfArrayRef == IntPtr.Zero)
				e = new ABRecord [0];
			else
				e = NSArray.ArrayFromHandle (cfArrayRef, h => ABRecord.FromHandle (h, AddressBook));
			return e.GetEnumerator ();
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABGroupCopyArrayOfAllMembersWithSortOrdering (IntPtr group, ABPersonSortBy sortOrdering);

		public ABRecord [] GetMembers (ABPersonSortBy sortOrdering)
		{
			var cfArrayRef = ABGroupCopyArrayOfAllMembersWithSortOrdering (Handle, sortOrdering);
			if (cfArrayRef == IntPtr.Zero)
				return new ABRecord [0];
			return NSArray.ArrayFromHandle (cfArrayRef, h => ABRecord.FromHandle (h, AddressBook));
		}

		[DllImport (Constants.AddressBookLibrary)]
		unsafe extern static byte ABGroupRemoveMember (IntPtr group, IntPtr member, IntPtr* error);
		public void Remove (ABRecord member)
		{
			if (member is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (member));
			IntPtr error;
			unsafe {
				if (ABGroupRemoveMember (Handle, member.Handle, &error) == 0)
					throw CFException.FromCFError (error);
			}
		}
	}
}

#endif // !MONOMAC
