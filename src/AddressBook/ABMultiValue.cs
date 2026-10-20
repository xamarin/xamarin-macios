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

using System.Collections;
using System.Collections.Generic;

using CoreFoundation;

namespace AddressBook {

	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
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

	/// <typeparam name="T">
	///       The type of the value to store.
	///     </typeparam>
	///     <summary>
	///       A <see cref="AddressBook.ABMultiValue{T}" /> entry.
	///     </summary>
	///     <remarks>
	///       <para>
	///         A "tuple" of
	///         (<see cref="AddressBook.ABMultiValueEntry{T}.Value" />,
	///         <see cref="AddressBook.ABMultiValueEntry{T}.Label" />,
	///         <see cref="AddressBook.ABMultiValueEntry{T}.Identifier" />).
	///       </para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
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

		/// <summary>
		///           Gets a value indicating whether the
		///           <see cref="AddressBook.ABMultiValueEntry{T}" />
		///           is read-only.
		///         </summary>
		///         <value>
		///           <see langword="true" /> if the current instance can be changed;
		///           otherwise, <see langword="false" />.
		///         </value>
		///         <remarks>
		///           <para>
		///             If <c>IsReadOnly</c> is <see langword="true" />, attempts to
		///             change the
		///             <see cref="AddressBook.ABMultiValueEntry{T}.Value" /> and
		///             <see cref="AddressBook.ABMultiValueEntry{T}.Label" />
		///             properties will result in a
		///             <see cref="System.NotSupportedException" />.
		///           </para>
		///         </remarks>
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

		/// <summary>
		///           The value of the <see cref="AddressBook.ABMultiValueEntry{T}" />.
		///         </summary>
		///         <value>
		///           A <typeparamref name="T" /> which is the value of the
		///           <see cref="AddressBook.ABMultiValueEntry{T}" />.
		///         </value>
		///         <remarks>
		///         </remarks>
		///         <exception cref="System.NotSupportedException">
		///           <see cref="AddressBook.ABMultiValueEntry{T}.IsReadOnly" />
		///           is <see langword="true" /> and the setter was invoked.
		///         </exception>
		///         <altmember cref="AddressBook.ABMultiValue{T}.IsReadOnly" />
		public T Value {
			get {
				AssertValid ();
				var rv = self.toManaged (ABMultiValue.CopyValueAtIndex (self.Handle, index));
				GC.KeepAlive (self);
				return rv;
			}
			set {
				if (IsReadOnly)
					throw CreateNotSupportedException ();
				AssertValid ();
				var rv = ABMultiValue.ReplaceValueAtIndex (self.Handle, ToIntPtr (value), index);
				GC.KeepAlive (self);
				if (rv == 0)
					throw new ArgumentException ("Value cannot be set");
			}
		}

		/// <summary>
		///           The label of the <see cref="AddressBook.ABMultiValueEntry{T}" />.
		///         </summary>
		///         <value>
		///           A <see cref="Foundation.NSString" /> which is the label
		///           of the <see cref="AddressBook.ABMultiValueEntry{T}" />.
		///         </value>
		///         <remarks>
		///         </remarks>
		///         <exception cref="System.NotSupportedException">
		///           <see cref="AddressBook.ABMultiValueEntry{T}.IsReadOnly" />
		///           is <see langword="true" /> and the setter was invoked.
		///         </exception>
		///         <altmember cref="AddressBook.ABMultiValue{T}.IsReadOnly" />
		public NSString? Label {
			get {
				AssertValid ();
				var rv = Runtime.GetNSObject<NSString> (ABMultiValue.CopyLabelAtIndex (self.Handle, index), true);
				GC.KeepAlive (self);
				return rv;
			}
			set {
				if (IsReadOnly)
					throw CreateNotSupportedException ();
				AssertValid ();
				ABMultiValue.ReplaceLabelAtIndex (self.Handle, value.GetHandle (), index);
				GC.KeepAlive (value);
				GC.KeepAlive (self);
			}
		}

		/// <summary>
		///           The identifier of the
		///           <see cref="AddressBook.ABMultiValueEntry{T}" />.
		///         </summary>
		///         <value>
		///           A <see cref="System.Int32" /> which is the identifier of the
		///           <see cref="AddressBook.ABMultiValueEntry{T}" />.
		///         </value>
		///         <remarks>
		///           Since multiple
		///           <see cref="AddressBook.ABMultiValueEntry{T}" />s within a
		///           <see cref="AddressBook.ABMultiValue{T}" /> can have the
		///           same
		///           <see cref="AddressBook.ABMultiValueEntry{T}.Value" /> and
		///           <see cref="AddressBook.ABMultiValueEntry{T}.Label" />,
		///           use <c>Identifier</c> to differentiate between entries.
		///         </remarks>
		public int Identifier {
			get {
				AssertValid ();
				var rv = ABMultiValue.GetIdentifierAtIndex (self.Handle, index);
				GC.KeepAlive (self);
				return rv;
			}
		}
	}

	/// <include file="../../docs/api/AddressBook/ABMultiValue`1.xml" path="/Documentation/Docs[@DocId='T:AddressBook.ABMultiValue`1']/*" />
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABMultiValue<T> : NativeObject, IEnumerable<ABMultiValueEntry<T>> {
		internal Converter<NativeHandle, T> toManaged;
		internal Converter<T, NativeHandle> toNative;

		[Preserve (Conditional = true)]
		internal ABMultiValue (NativeHandle handle, bool owns)
			: this (handle,
					v => (T) (object) Runtime.GetNSObject (v)!,
					v => Runtime.RetainAndAutoreleaseNativeObject ((INativeObject?) v), owns)
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

		/// <include file="../../docs/api/AddressBook/ABMultiValue`1.xml" path="/Documentation/Docs[@DocId='P:AddressBook.ABMultiValue`1.IsReadOnly']/*" />
		public virtual bool IsReadOnly {
			get {
				GetCheckedHandle ();
				return true;
			}
		}

		/// <summary>
		///           The type of the values in the collection.
		///         </summary>
		///         <value>
		///           A <see cref="AddressBook.ABPropertyType" /> specifying
		///           the type of values in the collection.
		///         </value>
		///         <remarks>
		///           <para>
		///             <see cref="AddressBook.ABPropertyType.Invalid" />
		///             is returned if the instance contains values of multiple different
		///             types or if the collection has no values.
		///           </para>
		///         </remarks>
		public ABPropertyType PropertyType {
			get { return ABMultiValue.GetPropertyType (Handle); }
		}

		/// <summary>
		///           Gets all values within the collection.
		///         </summary>
		///         <returns>
		///           A <typeparamref name="T" /> array containing all
		///           <see cref="AddressBook.ABMultiValueEntry{T}.Value" />s
		///           within the collection.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public T [] GetValues ()
		{
			return NSArray.NonNullArrayFromHandleDropNullElements (ABMultiValue.CopyArrayOfAllValues (Handle), toManaged, releaseHandle: true);
		}

		/// <summary>
		///           The number of entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </summary>
		///         <value>
		///           A <see cref="System.Int32" /> containing the number of entries in
		///           the <see cref="AddressBook.ABMultiValue{T}" />.
		///         </value>
		///         <remarks>
		///         </remarks>
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

		/// <summary>
		///           Returns an enumerator that iterates through all entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </summary>
		///         <returns>
		///           An
		///           <see cref="System.Collections.IEnumerator" />
		///           which will return all entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </returns>
		///         <remarks>
		///         </remarks>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>
		///           Returns an enumerator that iterates through all entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </summary>
		///         <returns>
		///           An
		///           <see cref="System.Collections.Generic.IEnumerator{T}" /> of <see cref="AddressBook.ABMultiValueEntry{T}" />
		///           which will return all entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public IEnumerator<ABMultiValueEntry<T>> GetEnumerator ()
		{
			nint c = Count;
			for (nint i = 0; i < c; ++i)
				yield return this [i];
		}

		/// <param name="value">
		///           A <see cref="Foundation.NSObject" /> containing
		///           the value to get the first index of.
		///         </param>
		///         <summary>
		///           Gets the first index of <paramref name="value" /> within the collection.
		///         </summary>
		///         <returns>
		///           <para>
		///           A <see cref="System.Int32" /> containing the first index of
		///           <paramref name="value" /> within the collection.
		///           If <paramref name="value" /> isn't present, <c>-1</c> is returned.
		///           </para>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public nint GetFirstIndexOfValue (NSObject value)
		{
			nint index = ABMultiValue.GetFirstIndexOfValue (Handle, value.Handle);
			GC.KeepAlive (value);
			return index;
		}

		/// <include file="../../docs/api/AddressBook/ABMultiValue`1.xml" path="/Documentation/Docs[@DocId='M:AddressBook.ABMultiValue`1.GetIndexForIdentifier(System.Int32)']/*" />
		public nint GetIndexForIdentifier (int identifier)
		{
			return ABMultiValue.GetIndexForIdentifier (Handle, identifier);
		}

		/// <summary>
		///           Returns an enumerator that iterates through all entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </summary>
		///         <returns>
		///           An <see cref="System.Collections.IEnumerator" />
		///           which will return all entries in the
		///           <see cref="AddressBook.ABMultiValue{T}" />.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public ABMutableMultiValue<T> ToMutableMultiValue ()
		{
			return new ABMutableMultiValue<T> (ABMultiValue.CreateMutableCopy (Handle), toManaged, toNative);
		}
	}

	/// <include file="../../docs/api/AddressBook/ABMutableMultiValue`1.xml" path="/Documentation/Docs[@DocId='T:AddressBook.ABMutableMultiValue`1']/*" />
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
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

		/// <summary>
		///           Gets a value indicating whether the
		///           <see cref="AddressBook.ABMutableMultiValue{T}" />
		///           is read-only.
		///         </summary>
		///         <value>
		///           Always returns <see langword="true" />.
		///         </value>
		///         <remarks>
		///         </remarks>
		public override bool IsReadOnly {
			get {
				GetCheckedHandle ();
				return false;
			}
		}

		/// <param name="value">
		///           A <typeparamref name="T" /> to add to the
		///           <see cref="AddressBook.ABMutableMultiValue{T}" />.
		///         </param>
		///         <param name="label">
		///           A <see cref="Foundation.NSString" /> to use
		///           as the label for <paramref name="value" />.
		///         </param>
		///         <summary>
		///           Add <paramref name="value" /> with the label <paramref name="label" />
		///           to a multivalue property.
		///         </summary>
		///         <returns>
		///           <see langword="true" /> if the value was added;
		///           otherwise, <see langword="false" />.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public unsafe bool Add (T value, NSString? label)
		{
			int _;
			bool result = ABMultiValue.AddValueAndLabel (Handle,
						toNative (value),
						label.GetHandle (),
						&_) != 0;
			GC.KeepAlive (label);
			return result;
		}

		public unsafe bool Insert (nint index, T value, NSString? label)
		{
			int _;
			bool result = ABMultiValue.InsertValueAndLabelAtIndex (Handle,
					toNative (value),
					label.GetHandle (),
					index,
					&_) != 0;
			GC.KeepAlive (label);
			return result;
		}

		public bool RemoveAt (nint index)
		{
			return ABMultiValue.RemoveValueAndLabelAtIndex (Handle, index) != 0;
		}
	}

	/// <summary>
	///       A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSDate" />
	///       which supports changing values.
	///     </summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABMutableDateMultiValue : ABMutableMultiValue<NSDate> {
		/// <summary>
		///           Constructs and initializes a
		///           <see cref="AddressBook.ABMutableDateMultiValue" />
		///           instance.
		///         </summary>
		///         <remarks>
		///         </remarks>
		public ABMutableDateMultiValue ()
			: base (ABMultiValue.CreateMutable (ABPropertyType.MultiDateTime), true)
		{
		}
	}

	/// <summary>
	///       A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSDictionary" />
	///       which supports changing values.
	///     </summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABMutableDictionaryMultiValue : ABMutableMultiValue<NSDictionary> {
		/// <summary>
		///           Constructs and initializes a
		///           <see cref="AddressBook.ABMutableDictionaryMultiValue" />
		///           instance.
		///         </summary>
		///         <remarks>
		///         </remarks>
		public ABMutableDictionaryMultiValue ()
			: base (ABMultiValue.CreateMutable (ABPropertyType.MultiDictionary), true)
		{
		}
	}

	/// <summary>
	///       A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSString" />
	///       which supports changing values.
	///     </summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABMutableStringMultiValue : ABMutableMultiValue<string> {
		/// <summary>
		///           Constructs and initializes a
		///           <see cref="AddressBook.ABMutableStringMultiValue" />
		///           instance.
		///         </summary>
		///         <remarks>
		///         </remarks>
		public ABMutableStringMultiValue ()
			: base (ABMultiValue.CreateMutable (ABPropertyType.MultiString),
					ABPerson.ToString, CFString.CreateNative)
		{
		}
	}
}

#endif // !MONOMAC
