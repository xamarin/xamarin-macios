// 
// ABMultiValue.cs: Implements the managed ABMultiValue
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

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0)]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	static class ABMultiValue {
		public const uint Mask = (1 << 8);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueCopyValueAtIndex")]
		public static extern IntPtr CopyValueAtIndex (IntPtr multiValue, nint index);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueCopyLabelAtIndex")]
		public static extern IntPtr CopyLabelAtIndex (IntPtr multiValue, nint index);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueGetIdentifierAtIndex")]
		public static extern int /* ABMultiValueIdentifier */ GetIdentifierAtIndex (IntPtr multiValue, nint index);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueCopyArrayOfAllValues")]
		public static extern IntPtr CopyArrayOfAllValues (IntPtr multiValue);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueGetCount")]
		public static extern nint GetCount (IntPtr multiValue);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueGetFirstIndexOfValue")]
		public static extern nint GetFirstIndexOfValue (IntPtr multiValue, IntPtr value);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueGetIndexForIdentifier")]
		public static extern nint GetIndexForIdentifier (IntPtr multiValue, int /* ABMultiValueIdentifier */ identifier);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueGetPropertyType")]
		public static extern ABPropertyType GetPropertyType (IntPtr multiValue);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueCreateMutable")]
		public static extern IntPtr CreateMutable (ABPropertyType type);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueCreateMutableCopy")]
		public static extern IntPtr CreateMutableCopy (IntPtr multiValue);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueAddValueAndLabel")]
		public unsafe static extern byte AddValueAndLabel (IntPtr multiValue, IntPtr value, IntPtr label, int* /* int32_t */ outIdentifier);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueReplaceValueAtIndex")]
		public static extern byte ReplaceValueAtIndex (IntPtr multiValue, IntPtr value, nint index);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueReplaceLabelAtIndex")]
		public static extern byte ReplaceLabelAtIndex (IntPtr multiValue, IntPtr value, nint index);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueInsertValueAndLabelAtIndex")]
		public unsafe static extern byte InsertValueAndLabelAtIndex (IntPtr multiValue, IntPtr value, IntPtr label, nint index, int* /* int32_t */ outIdentifier);

		[DllImport (Constants.AddressBookLibrary, EntryPoint = "ABMultiValueRemoveValueAndLabelAtIndex")]
		public static extern byte RemoveValueAndLabelAtIndex (IntPtr multiValue, nint index);
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public struct ABMultiValueEntry<T> {
		ABMultiValue<T> self;
		nint index;

		internal ABMultiValueEntry (ABMultiValue<T> self, nint index)
		{
			this.self = self;
			this.index = index;
		}

		internal void AssertValid ()
		{
			if (self is null)
				throw new InvalidOperationException ();
		}

		public bool IsReadOnly {
			get { return self.IsReadOnly; }
		}

		NativeHandle ToIntPtr (T value)
		{
			var mutable = self as ABMutableMultiValue<T>;
			if (mutable is null)
				throw CreateNotSupportedException ();
			return mutable.toNative (value);
		}

		static Exception CreateNotSupportedException ()
		{
			return new NotSupportedException ("ABMultiValue record is read-only. " +
					"To update properties, use an ABMutableMultiValue<T>. " +
					"See ABMultiValue<T>.ToMutableMultiValue().");
		}

		public T Value {
			get {
				AssertValid ();
				return self.toManaged (ABMultiValue.CopyValueAtIndex (self.Handle, index));
			}
			set {
				if (IsReadOnly)
					throw CreateNotSupportedException ();
				AssertValid ();
				if (ABMultiValue.ReplaceValueAtIndex (self.Handle, ToIntPtr (value), index) == 0)
					throw new ArgumentException ("Value cannot be set");
			}
		}

		public NSString? Label {
			get {
				AssertValid ();
				return Runtime.GetNSObject<NSString> (ABMultiValue.CopyLabelAtIndex (self.Handle, index));
			}
			set {
				if (IsReadOnly)
					throw CreateNotSupportedException ();
				AssertValid ();
				ABMultiValue.ReplaceLabelAtIndex (self.Handle, value.GetHandle (), index);
			}
		}

		public int Identifier {
			get {
				AssertValid ();
				return ABMultiValue.GetIdentifierAtIndex (self.Handle, index);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABMultiValue<T> : NativeObject, IEnumerable<ABMultiValueEntry<T>> {
		internal Converter<NativeHandle, T> toManaged;
		internal Converter<T, NativeHandle> toNative;

		[Preserve (Conditional = true)]
		internal ABMultiValue (NativeHandle handle, bool owns)
			: this (handle,
					v => (T) (object) Runtime.GetNSObject (v)!,
					v => ((INativeObject?) v).GetHandle (), owns)
		{
			if (!typeof (NSObject).IsAssignableFrom (typeof (T)))
				throw new InvalidOperationException ("T must be an NSObject!");
		}

		internal ABMultiValue (NativeHandle handle, Converter<NativeHandle, T> toManaged, Converter<T, NativeHandle> toNative, bool owns)
			: base (handle, owns)
		{
			if (toManaged is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (toManaged));
			if (toNative is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (toNative));

			this.toManaged = toManaged;
			this.toNative = toNative;
		}

		public virtual bool IsReadOnly {
			get {
				GetCheckedHandle ();
				return true;
			}
		}

		public ABPropertyType PropertyType {
			get { return ABMultiValue.GetPropertyType (Handle); }
		}

		public T [] GetValues ()
		{
			return NSArray.ArrayFromHandle (ABMultiValue.CopyArrayOfAllValues (Handle), toManaged)
				?? Array.Empty<T> ();
		}

		public nint Count {
			get {
				return ABMultiValue.GetCount (Handle);
			}
		}

		public ABMultiValueEntry<T> this [nint index] {
			get {
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException ();
				return new ABMultiValueEntry<T> (this, index);
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<ABMultiValueEntry<T>> GetEnumerator ()
		{
			nint c = Count;
			for (nint i = 0; i < c; ++i)
				yield return this [i];
		}

		public nint GetFirstIndexOfValue (NSObject value)
		{
			return ABMultiValue.GetFirstIndexOfValue (Handle, value.Handle);
		}

		public nint GetIndexForIdentifier (int identifier)
		{
			return ABMultiValue.GetIndexForIdentifier (Handle, identifier);
		}

		public ABMutableMultiValue<T> ToMutableMultiValue ()
		{
			return new ABMutableMultiValue<T> (ABMultiValue.CreateMutableCopy (Handle), toManaged, toNative);
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABMutableMultiValue<T> : ABMultiValue<T> {
		[Preserve (Conditional = true)]
		internal ABMutableMultiValue (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal ABMutableMultiValue (NativeHandle handle, Converter<NativeHandle, T> toManaged, Converter<T, NativeHandle> toNative)
			: base (handle, toManaged, toNative, false)
		{
		}

		public override bool IsReadOnly {
			get {
				GetCheckedHandle ();
				return false;
			}
		}

		public unsafe bool Add (T value, NSString? label)
		{
			int _;
			return ABMultiValue.AddValueAndLabel (Handle,
						toNative (value),
						label.GetHandle (),
						&_) != 0;
		}

		public unsafe bool Insert (nint index, T value, NSString? label)
		{
			int _;
			return ABMultiValue.InsertValueAndLabelAtIndex (Handle,
					toNative (value),
					label.GetHandle (),
					index,
					&_) != 0;
		}

		public bool RemoveAt (nint index)
		{
			return ABMultiValue.RemoveValueAndLabelAtIndex (Handle, index) != 0;
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABMutableDateMultiValue : ABMutableMultiValue<NSDate> {
		public ABMutableDateMultiValue ()
			: base (ABMultiValue.CreateMutable (ABPropertyType.MultiDateTime), true)
		{
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABMutableDictionaryMultiValue : ABMutableMultiValue<NSDictionary> {
		public ABMutableDictionaryMultiValue ()
			: base (ABMultiValue.CreateMutable (ABPropertyType.MultiDictionary), true)
		{
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABMutableStringMultiValue : ABMutableMultiValue<string> {
		public ABMutableStringMultiValue ()
			: base (ABMultiValue.CreateMutable (ABPropertyType.MultiString),
					ABPerson.ToString, CFString.CreateNative)
		{
		}
	}
}

#endif // !MONOMAC
