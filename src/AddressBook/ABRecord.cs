// 
// ABRecord.cs: Implements the managed ABRecord
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc.
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
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AddressBook {

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
	public class ABRecord : NativeObject {

		public const int InvalidRecordId = -1;
		public const int InvalidPropertyId = -1;

		[Preserve (Conditional = true)]
		internal ABRecord (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public static ABRecord? FromHandle (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;
			return FromHandle (handle, null, false);
		}

		internal static ABRecord FromHandle (IntPtr handle, ABAddressBook? addressbook, bool owns = true)
		{
			if (handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handle));
			// TODO: does ABGroupCopyArrayOfAllMembers() have Create or Get
			// semantics for the array elements?
			var type = ABRecordGetRecordType (handle);
			ABRecord rec;

			switch (type) {
			case ABRecordType.Person:
				rec = new ABPerson (handle, owns);
				break;
			case ABRecordType.Group:
				rec = new ABGroup (handle, owns);
				break;
			case ABRecordType.Source:
				rec = new ABSource (handle, owns);
				break;
			default:
				throw new NotSupportedException ("Could not determine record type.");
			}

			rec.AddressBook = addressbook;
			return rec;
		}

		protected override void Dispose (bool disposing)
		{
			AddressBook = null;
			base.Dispose (disposing);
		}

		internal ABAddressBook? AddressBook {
			get; set;
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static int ABRecordGetRecordID (IntPtr record);
		public int Id {
			get { return ABRecordGetRecordID (Handle); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABRecordType ABRecordGetRecordType (IntPtr record);
		public ABRecordType Type {
			get { return ABRecordGetRecordType (Handle); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABRecordCopyCompositeName (IntPtr record);
		public override string? ToString ()
		{
			return CFString.FromHandle (ABRecordCopyCompositeName (Handle));
		}

		// TODO: Should SetValue/CopyValue/RemoveValue be public?

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABRecordSetValue (IntPtr record, int /* ABPropertyID = int32_t */ property, IntPtr value, out IntPtr error);
		internal void SetValue (int property, IntPtr value)
		{
			IntPtr error;
			if (!ABRecordSetValue (Handle, property, value, out error))
				throw CFException.FromCFError (error);
		}

		internal void SetValue (int property, NSObject? value)
		{
			SetValue (property, value.GetHandle ());
		}

		internal void SetValue (int property, string? value)
		{
			var valueHandle = CFString.CreateNative (value);
			try {
				SetValue (property, valueHandle);
			} finally {
				CFString.ReleaseNative (valueHandle);
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABRecordCopyValue (IntPtr record, int /* ABPropertyID = int32_t */ property);
		internal IntPtr CopyValue (int property)
		{
			return ABRecordCopyValue (Handle, property);
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABRecordRemoveValue (IntPtr record, int /* ABPropertyID = int32_t */ property, out IntPtr error);
		internal void RemoveValue (int property)
		{
			IntPtr error;
			bool r = ABRecordRemoveValue (Handle, property, out error);
			if (!r && error != IntPtr.Zero)
				throw CFException.FromCFError (error);
		}

		internal T? PropertyTo<T> (int id)
			where T : NSObject
		{
			IntPtr value = CopyValue (id);
			return (T?) Runtime.GetNSObject (value);
		}

		internal string? PropertyToString (int id)
		{
			return CFString.FromHandle (CopyValue (id));
		}
	}
}

#endif // !MONOMAC
