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

#if !MONOMAC

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace AddressBook {
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public enum ABRecordType : uint /* uint32_t */ {
		Person = 0,
		Group = 1,
		Source = 2
	}

	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public enum ABPropertyType : uint /* uint32_t */ {
		Invalid         = 0,
		String          = 0x1,
		Integer         = 0x2,
		Real            = 0x3,
		DateTime        = 0x4,
		Dictionary      = 0x5,
		MultiString     = ABMultiValue.Mask | String,
		MultiInteger    = ABMultiValue.Mask | Integer,
		MultiReal       = ABMultiValue.Mask | Real,
		MultiDateTime   = ABMultiValue.Mask | DateTime,
		MultiDictionary = ABMultiValue.Mask | Dictionary,
	}

	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public abstract class ABRecord : INativeObject, IDisposable {

		public const int InvalidRecordId = -1;
		public const int InvalidPropertyId = -1;

		IntPtr handle;

		internal ABRecord (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		public static ABRecord FromHandle (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;
			return FromHandle (handle, null, false);
		}
		
		internal static ABRecord FromHandle (IntPtr handle, ABAddressBook addressbook, bool owns = true)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("handle");
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

		~ABRecord ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero)
				CFObject.CFRelease (handle);
			handle = IntPtr.Zero;
			AddressBook = null;
		}

		void AssertValid ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException ("");
		}

		internal ABAddressBook AddressBook {
			get; set;
		}

		public IntPtr Handle {
			get {
				AssertValid ();
				return handle;
			}
			internal set {
				handle = value;				
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static int ABRecordGetRecordID (IntPtr record);
		public int Id {
			get {return ABRecordGetRecordID (Handle);}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABRecordType ABRecordGetRecordType (IntPtr record);
		public ABRecordType Type {
			get {return ABRecordGetRecordType (Handle);}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABRecordCopyCompositeName (IntPtr record);
		public override string ToString ()
		{
			using (var s = new NSString (ABRecordCopyCompositeName (Handle)))
				return s.ToString ();
		}

		// TODO: Should SetValue/CopyValue/RemoveValue be public?

		[DllImport (Constants.AddressBookLibrary)]
		extern static bool ABRecordSetValue (IntPtr record, int /* ABPropertyID = int32_t */ property, IntPtr value, out IntPtr error);
		internal void SetValue (int property, IntPtr value)
		{
			IntPtr error;
			if (!ABRecordSetValue (Handle, property, value, out error))
				throw CFException.FromCFError (error);
		}

		internal void SetValue (int property, NSObject value)
		{
			SetValue (property, value == null ? IntPtr.Zero : value.Handle);
		}

		internal void SetValue (int property, string value)
		{
			using (NSObject v = value == null ? null : new NSString (value))
				SetValue (property, v);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABRecordCopyValue (IntPtr record, int /* ABPropertyID = int32_t */ property);
		internal IntPtr CopyValue (int property)
		{
			return ABRecordCopyValue (Handle, property);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static bool ABRecordRemoveValue (IntPtr record, int /* ABPropertyID = int32_t */ property, out IntPtr error);
		internal void RemoveValue (int property)
		{
			IntPtr error;
			bool r = ABRecordRemoveValue (Handle, property, out error);
			if (!r && error != IntPtr.Zero)
				throw CFException.FromCFError (error);
		}

		internal T PropertyTo<T> (int id)
			where T : NSObject
		{
			IntPtr value = CopyValue (id);
			if (value == IntPtr.Zero)
				return null;
			return (T) Runtime.GetNSObject (value);
		}

		internal string PropertyToString (int id)
		{
			IntPtr value = CopyValue (id);
			if (value == IntPtr.Zero)
				return null;
			using (var s = new NSString (value))
				return s.ToString ();
		}
	}
}

#endif // !MONOMAC
