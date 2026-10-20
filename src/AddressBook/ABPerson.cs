// 
// ABPerson.cs: Implements the managed ABPerson
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//     
// Copyright (C) 2009 Novell, Inc
// Copyright (C) 2012-2013 Xamarin Inc.
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

using System.Collections.Generic;

using CoreFoundation;

namespace AddressBook {

	static partial class ABPersonPropertyId {
		public static int ToId (ABPersonProperty property)
		{
			switch (property) {
			case ABPersonProperty.Address: return Address;
			case ABPersonProperty.Birthday: return Birthday;
			case ABPersonProperty.CreationDate: return CreationDate;
			case ABPersonProperty.Date: return Date;
			case ABPersonProperty.Department: return Department;
			case ABPersonProperty.Email: return Email;
			case ABPersonProperty.FirstName: return FirstName;
			case ABPersonProperty.FirstNamePhonetic: return FirstNamePhonetic;
			case ABPersonProperty.InstantMessage: return InstantMessage;
			case ABPersonProperty.JobTitle: return JobTitle;
			case ABPersonProperty.Kind: return Kind;
			case ABPersonProperty.LastName: return LastName;
			case ABPersonProperty.LastNamePhonetic: return LastNamePhonetic;
			case ABPersonProperty.MiddleName: return MiddleName;
			case ABPersonProperty.MiddleNamePhonetic: return MiddleNamePhonetic;
			case ABPersonProperty.ModificationDate: return ModificationDate;
			case ABPersonProperty.Nickname: return Nickname;
			case ABPersonProperty.Note: return Note;
			case ABPersonProperty.Organization: return Organization;
			case ABPersonProperty.Phone: return Phone;
			case ABPersonProperty.Prefix: return Prefix;
			case ABPersonProperty.RelatedNames: return RelatedNames;
			case ABPersonProperty.Suffix: return Suffix;
			case ABPersonProperty.Url: return Url;
			case ABPersonProperty.SocialProfile: return SocialProfile;
			}
			throw new NotSupportedException ("Invalid ABPersonProperty value: " + property);
		}

		public static ABPersonProperty ToPersonProperty (int id)
		{
			if (id == Address) return ABPersonProperty.Address;
			if (id == Birthday) return ABPersonProperty.Birthday;
			if (id == CreationDate) return ABPersonProperty.CreationDate;
			if (id == Date) return ABPersonProperty.Date;
			if (id == Department) return ABPersonProperty.Department;
			if (id == Email) return ABPersonProperty.Email;
			if (id == FirstName) return ABPersonProperty.FirstName;
			if (id == FirstNamePhonetic) return ABPersonProperty.FirstNamePhonetic;
			if (id == InstantMessage) return ABPersonProperty.InstantMessage;
			if (id == JobTitle) return ABPersonProperty.JobTitle;
			if (id == Kind) return ABPersonProperty.Kind;
			if (id == LastName) return ABPersonProperty.LastName;
			if (id == LastNamePhonetic) return ABPersonProperty.LastNamePhonetic;
			if (id == MiddleName) return ABPersonProperty.MiddleName;
			if (id == MiddleNamePhonetic) return ABPersonProperty.MiddleNamePhonetic;
			if (id == ModificationDate) return ABPersonProperty.ModificationDate;
			if (id == Nickname) return ABPersonProperty.Nickname;
			if (id == Note) return ABPersonProperty.Note;
			if (id == Organization) return ABPersonProperty.Organization;
			if (id == Phone) return ABPersonProperty.Phone;
			if (id == Prefix) return ABPersonProperty.Prefix;
			if (id == RelatedNames) return ABPersonProperty.RelatedNames;
			if (id == Suffix) return ABPersonProperty.Suffix;
			if (id == Url) return ABPersonProperty.Url;
			if (id == SocialProfile) return ABPersonProperty.SocialProfile;
			throw new NotSupportedException ("Invalid ABPersonPropertyId value: " + id);
		}
	}

	/// <summary>Contains keys for the components of a postal address.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonAddressKey {
	}

	/// <summary>Contains labels for dates stored in an Address Book person record.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonDateLabel {
	}

	static partial class ABPersonKindId {
		public static ABPersonKind ToPersonKind (NSNumber value)
		{
			if (object.ReferenceEquals (Organization, value))
				return ABPersonKind.Organization;
			if (object.ReferenceEquals (Person, value))
				return ABPersonKind.Person;
			return ABPersonKind.None;
		}

		public static NSNumber? FromPersonKind (ABPersonKind value)
		{
			switch (value) {
			case ABPersonKind.Organization: return Organization;
			case ABPersonKind.Person: return Person;
			}
			return null;
		}
	}

	/// <summary>A class whose static members define constant names for various social networks.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonSocialProfileService {
#if !XAMCORE_5_0
		/// <summary>Identifies the Twitter social-profile service.</summary>
		public static readonly NSString? Twitter = _Twitter;

		/// <summary>Identifies the Game Center social-profile service.</summary>
		public static readonly NSString? GameCenter = _GameCenter;

		/// <summary>Identifies the Facebook social-profile service.</summary>
		public static readonly NSString? Facebook = _Facebook;

		/// <summary>Identifies the Myspace social-profile service.</summary>
		public static readonly NSString? Myspace = _Myspace;

		/// <summary>Identifies the LinkedIn social-profile service.</summary>
		public static readonly NSString? LinkedIn = _LinkedIn;

		/// <summary>Identifies the Flickr social-profile service.</summary>
		public static readonly NSString? Flickr = _Flickr;

		/// <summary>Identifies the Sina Weibo social-profile service.</summary>
		public static readonly NSString? SinaWeibo = _SinaWeibo;
#endif
	}

	/// <summary>Contains labels for phone numbers stored in an Address Book person record.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonPhoneLabel {
	}

	/// <summary>
	///       Possible
	///       <see cref="AddressBook.ABPersonInstantMessageKey.Service" />
	///       key values.
	///     </summary>
	///     <remarks>
	///       <para>
	///         Instant message information is stored within
	///         <see cref="Foundation.NSDictionary" /> instances where
	///         the the
	///         <see cref="AddressBook.ABPersonInstantMessageKey.Service" />
	///         key is used to store the service name, and the
	///         <see cref="AddressBook.ABPersonInstantMessageKey.Username" />
	///         key is used to store the service login name.
	///       </para>
	///       <para>
	///         The <c>ABPersonInstantMessageService</c> stores predefined
	///         <see cref="AddressBook.ABPersonInstantMessageKey.Service" />
	///         values.
	///       </para>
	///     </remarks>
	///     <altmember cref="AddressBook.ABPerson.GetInstantMessageServices" />
	///     <altmember cref="AddressBook.ABPerson.SetInstantMessages(AddressBook.ABMultiValue{Foundation.NSDictionary})" />
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonInstantMessageService {
	}

	/// <summary>Contains keys for instant-messaging information stored in an Address Book person record.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonInstantMessageKey {
	}

	/// <summary>Contains labels for URLs stored in an Address Book person record.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonUrlLabel {
	}

	/// <summary>Contains labels for related names stored in an Address Book person record.</summary>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABPersonRelatedNamesLabel {
	}

	/// <summary>
	///       Generic property labels.
	///     </summary>
	///     <remarks>
	///       <para>
	///         Labels are used with
	///         <see cref="AddressBook.ABAddressBook.LocalizedLabel(Foundation.NSString)" />,
	///         <see cref="AddressBook.ABMultiValueEntry{T}.Label" />,
	///         <see cref="AddressBook.ABMutableMultiValue{T}.Add" />, and
	///         <see cref="AddressBook.ABMutableMultiValue{T}.Insert" />.
	///       </para>
	///     </remarks>
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	public static partial class ABLabel {
	}

	/// <include file="../../docs/api/AddressBook/ABPerson.xml" path="/Documentation/Docs[@DocId='T:AddressBook.ABPerson']/*" />
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABPerson : ABRecord, IComparable, IComparable<ABPerson> {
		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreate ();

		/// <summary>
		///           Constructs and initializes a
		///           <see cref="AddressBook.ABPerson" /> instance.
		///         </summary>
		///         <remarks>
		///         </remarks>
		public ABPerson ()
			: base (ABPersonCreate (), true)
		{
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreateInSource (IntPtr source);

		/// <param name="source">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public ABPerson (ABRecord source)
			: base (ABPersonCreateInSource (source.GetNonNullHandle (nameof (source))), true)
		{
			GC.KeepAlive (source);
		}

		[Preserve (Conditional = true)]
		internal ABPerson (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal ABPerson (NativeHandle handle, ABAddressBook? addressbook)
			: base (handle, false)
		{
			AddressBook = addressbook;
		}

		/// <include file="../../docs/api/AddressBook/ABPerson.xml" path="/Documentation/Docs[@DocId='M:AddressBook.ABPerson.System#IComparable#CompareTo(System.Object)']/*" />
		int IComparable.CompareTo (object? o)
		{
			var other = o as ABPerson;
			if (other is null)
				throw new ArgumentException ("Can only compare to other ABPerson instances.", nameof (o));
			return CompareTo (other);
		}

		/// <include file="../../docs/api/AddressBook/ABPerson.xml" path="/Documentation/Docs[@DocId='M:AddressBook.ABPerson.CompareTo(AddressBook.ABPerson)']/*" />
		public int CompareTo (ABPerson? other)
		{
			return CompareTo (other!, ABPersonSortBy.LastName);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static int ABPersonComparePeopleByName (IntPtr person1, IntPtr person2, ABPersonSortBy ordering);
		/// <include file="../../docs/api/AddressBook/ABPerson.xml" path="/Documentation/Docs[@DocId='M:AddressBook.ABPerson.CompareTo(AddressBook.ABPerson,AddressBook.ABPersonSortBy)']/*" />
		public int CompareTo (ABPerson other, ABPersonSortBy ordering)
		{
			if (other is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (other));
			if (ordering != ABPersonSortBy.FirstName && ordering != ABPersonSortBy.LastName)
				throw new ArgumentException ("Invalid ordering value: " + ordering, "ordering");
			int result = ABPersonComparePeopleByName (Handle, other.Handle, ordering);
			GC.KeepAlive (other);
			return result;
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyLocalizedPropertyName (int /* ABPropertyID = int32_t */ propertyId);
		/// <param name="property">
		///           A <see cref="AddressBook.ABPersonProperty" />
		///           containing the property to get the localized name of.
		///         </param>
		///         <summary>
		///           Gets the localized name of a <paramref name="property" />.
		///         </summary>
		///         <returns>
		///           A <see cref="System.String" /> containing the localized
		///           name of a <paramref name="property" />.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public static string? LocalizedPropertyName (ABPersonProperty property)
		{
			return CFString.FromHandle (ABPersonCopyLocalizedPropertyName (ABPersonPropertyId.ToId (property)), true);
		}

		/// <param name="propertyId">A value that corresponds to one of the low-level kABPersonProperty fields.
		///         </param>
		///         <summary>
		///           Gets the localized name of a <paramref name="propertyId" />.
		///         </summary>
		///         <returns>
		///           A <see cref="System.String" /> containing the localized
		///           name of a <paramref name="propertyId" />.
		///         </returns>
		///         <remarks>
		///           <para>
		/// 	    Unlike the overload that takes a ABPersonProperty, the value
		/// 	    of the <paramref name="propertyId" /> is actually not a
		/// 	    constant and can vary at runtime (this is the native C
		/// 	    interface).
		/// 	  </para>
		///           <para>
		/// 	    This method is typically used on callbacks that provide an
		/// 	    "int propertyId" as a parameter.
		/// 	  </para>
		///         </remarks>
		public static string? LocalizedPropertyName (int propertyId)
		{
			return CFString.FromHandle (ABPersonCopyLocalizedPropertyName (propertyId), true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPropertyType ABPersonGetTypeOfProperty (int /* ABPropertyID = int32_t */ propertyId);
		/// <param name="property">
		///           A <see cref="AddressBook.ABPersonProperty" />
		///           specifying which property to query.
		///         </param>
		///         <summary>
		///           Gets the type of the property <paramref name="property" />.
		///         </summary>
		///         <returns>
		///           A <see cref="AddressBook.ABPropertyType" /> value containing
		///           the type of the property <paramref name="property" />.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public static ABPropertyType GetPropertyType (ABPersonProperty property)
		{
			return ABPersonGetTypeOfProperty (ABPersonPropertyId.ToId (property));
		}

		/// <param name="propertyId">A value that corresponds to one of
		/// 	the low-level kABPersonProperty fields.</param>
		///         <summary>Gets the type of the property <paramref name="propertyId" />.</summary>
		///         <returns>
		///           A <see cref="AddressBook.ABPropertyType" /> value containing
		///           the type of the property <paramref name="propertyId" />.
		///         </returns>
		///         <remarks>
		///           <para>
		/// 	    Unlike the overload that takes a ABPersonProperty, the value
		/// 	    of the <paramref name="propertyId" /> is actually not a
		/// 	    constant and can vary at runtime (this is the native C
		/// 	    interface).
		/// 	  </para>
		///           <para>
		/// 	    This method is typically used on callbacks that provide an
		/// 	    "int propertyId" as a parameter.
		/// 	  </para>
		///         </remarks>
		public static ABPropertyType GetPropertyType (int propertyId)
		{
			return ABPersonGetTypeOfProperty (propertyId);
		}

		[DllImport (Constants.AddressBookLibrary)]
		unsafe extern static byte ABPersonSetImageData (IntPtr person, IntPtr imageData, IntPtr* error);
		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyImageData (IntPtr person);

		/// <summary>
		///           Gets or sets the
		///           <see cref="AddressBook.ABPerson" />'s picture.
		///         </summary>
		///         <value>
		///           A <see cref="Foundation.NSData" /> containing
		///           the picture data.
		///         </value>
		///         <remarks>
		///         </remarks>
		///         <exception cref="CoreFoundation.CFException">
		///           The reason the picture couldn't be set.
		///         </exception>
		///         <altmember cref="AddressBook.ABPerson.HasImage" />
		///         <altmember cref="AddressBook.ABPerson.RemoveImage" />
		public NSData? Image {
			get { return Runtime.GetNSObject<NSData> (ABPersonCopyImageData (Handle), true); }
			set {
				IntPtr error;
				unsafe {
					if (ABPersonSetImageData (Handle, value.GetHandle (), &error) == 0)
						throw CFException.FromCFError (error);
					GC.KeepAlive (value);
				}
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static byte ABPersonHasImageData (IntPtr person);
		/// <summary>
		///           Gets a value indicating whether the
		///           <see cref="AddressBook.ABPerson" /> has a picture.
		///         </summary>
		///         <value>
		///           <see langword="true" /> if the
		///           <see cref="AddressBook.ABPerson" /> has a picture;
		///           otherwise, <see langword="false" />.
		///         </value>
		///         <remarks>
		///         </remarks>
		public bool HasImage {
			get { return ABPersonHasImageData (Handle) != 0; }
		}

		[DllImport (Constants.AddressBookLibrary)]
		unsafe extern static byte ABPersonRemoveImageData (IntPtr person, IntPtr* error);
		/// <summary>
		///           Removes a <see cref="AddressBook.ABPerson" />'s picture.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <exception cref="CoreFoundation.CFException">
		///           The reason the picture couldn't be removed.
		///         </exception>
		///         <altmember cref="AddressBook.ABPerson.Image" />
		public void RemoveImage ()
		{
			IntPtr error;
			unsafe {
				if (ABPersonRemoveImageData (Handle, &error) == 0)
					throw CFException.FromCFError (error);
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPersonCompositeNameFormat ABPersonGetCompositeNameFormat ();

		/// <summary>Developers should not use this deprecated property. Developers should use GetCompositeNameFormat (null) instead</summary>
		///         <value>
		///           A <see cref="AddressBook.ABPersonCompositeNameFormat" />
		///           which controls the format used for the person's composite name.
		///         </value>
		///         <remarks>
		///         </remarks>
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios")]
		[ObsoletedOSPlatform ("ios", "Use 'GetCompositeNameFormat (null)' instead.")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("maccatalyst", "Use 'GetCompositeNameFormat (null)' instead.")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public static ABPersonCompositeNameFormat CompositeNameFormat {
			get { return ABPersonGetCompositeNameFormat (); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPersonCompositeNameFormat ABPersonGetCompositeNameFormatForRecord (IntPtr record);

		/// <param name="record">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static ABPersonCompositeNameFormat GetCompositeNameFormat (ABRecord? record)
		{
			var result = ABPersonGetCompositeNameFormatForRecord (record.GetHandle ());
			GC.KeepAlive (record);
			return result;
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyCompositeNameDelimiterForRecord (IntPtr record);

		/// <param name="record">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static string? GetCompositeNameDelimiter (ABRecord? record)
		{
			var handle = ABPersonCopyCompositeNameDelimiterForRecord (record.GetHandle ());
			GC.KeepAlive (record);
			return CFString.FromHandle (handle, true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPersonSortBy ABPersonGetSortOrdering ();
		/// <summary>
		///           Gets the user's sort ordering preference for lists of persons.
		///         </summary>
		///         <value>
		///           A <see cref="AddressBook.ABPersonSortBy" /> which
		///           is the user's sort ordering preference for lists of persons.
		///         </value>
		///         <remarks>
		///         </remarks>
		public static ABPersonSortBy SortOrdering {
			get { return ABPersonGetSortOrdering (); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s first name.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s first name.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? FirstName {
			get { return PropertyToString (ABPersonPropertyId.FirstName); }
			set { SetValue (ABPersonPropertyId.FirstName, value); }
		}

		/// <summary>
		///           Gets or sets <see cref="AddressBook.ABPerson" />'s first name
		///           phonetic pronounciation.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s first name
		///           phonetic pronounciation.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? FirstNamePhonetic {
			get { return PropertyToString (ABPersonPropertyId.FirstNamePhonetic); }
			set { SetValue (ABPersonPropertyId.FirstNamePhonetic, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s last name.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s last name.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? LastName {
			get { return PropertyToString (ABPersonPropertyId.LastName); }
			set { SetValue (ABPersonPropertyId.LastName, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s last name
		///           phonetic pronounciation.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s last name phonetic pronounciation.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? LastNamePhonetic {
			get { return PropertyToString (ABPersonPropertyId.LastNamePhonetic); }
			set { SetValue (ABPersonPropertyId.LastNamePhonetic, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s middle name.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s middle name.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? MiddleName {
			get { return PropertyToString (ABPersonPropertyId.MiddleName); }
			set { SetValue (ABPersonPropertyId.MiddleName, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s middle name
		///           phonetic pronounciation.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s middle name phonetic pronounciation.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? MiddleNamePhonetic {
			get { return PropertyToString (ABPersonPropertyId.MiddleNamePhonetic); }
			set { SetValue (ABPersonPropertyId.MiddleNamePhonetic, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s prefix.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s prefix.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Prefix {
			get { return PropertyToString (ABPersonPropertyId.Prefix); }
			set { SetValue (ABPersonPropertyId.Prefix, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s suffix.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s suffix.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Suffix {
			get { return PropertyToString (ABPersonPropertyId.Suffix); }
			set { SetValue (ABPersonPropertyId.Suffix, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s nickname.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s nickname.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Nickname {
			get { return PropertyToString (ABPersonPropertyId.Nickname); }
			set { SetValue (ABPersonPropertyId.Nickname, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s organization.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s organization.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Organization {
			get { return PropertyToString (ABPersonPropertyId.Organization); }
			set { SetValue (ABPersonPropertyId.Organization, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s job title.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s job title.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? JobTitle {
			get { return PropertyToString (ABPersonPropertyId.JobTitle); }
			set { SetValue (ABPersonPropertyId.JobTitle, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s department.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s department.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Department {
			get { return PropertyToString (ABPersonPropertyId.Department); }
			set { SetValue (ABPersonPropertyId.Department, value); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopySource (IntPtr group);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABRecord? Source {
			get {
				var h = ABPersonCopySource (Handle);
				if (h == IntPtr.Zero)
					return null;

				return FromHandle (h, null);
			}
		}

		internal static string ToString (NativeHandle value)
		{
			return CFString.FromHandle (value)!;
		}

		/// <summary>
		///           Gets the <see cref="AddressBook.ABPerson" />'s email addresses.
		///         </summary>
		///         <returns>
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s email addresses.
		///         </returns>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="SetEmails" />
		public ABMultiValue<string>? GetEmails ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.Email));
		}

		static ABMultiValue<string>? CreateStringMultiValue (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;
			return new ABMultiValue<string> (handle, ABPerson.ToString, CFString.CreateNative, true);
		}

		/// <param name="value">
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s new email addresses
		///         </param>
		///         <summary>
		///           Sets the <see cref="AddressBook.ABPerson" />'s new email addresses.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPerson.GetEmails" />
		public void SetEmails (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.Email, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s birthday.
		///         </summary>
		///         <value>
		///           A <see cref="Foundation.NSDate" /> containing
		///           the person's birthday.
		///         </value>
		///         <remarks>
		///         </remarks>
		public NSDate? Birthday {
			get { return PropertyTo<NSDate> (ABPersonPropertyId.Birthday); }
			set { SetValue (ABPersonPropertyId.Birthday, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s note.
		///         </summary>
		///         <value>
		///           A <see cref="System.String" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s note.
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Note {
			get { return PropertyToString (ABPersonPropertyId.Note); }
			set { SetValue (ABPersonPropertyId.Note, value); }
		}

		/// <summary>
		///           Gets or sets the record's creation date.
		///         </summary>
		///         <value>
		///           A <see cref="Foundation.NSDate" /> containing
		///           the record's creation date.
		///         </value>
		///         <remarks>
		///         </remarks>
		public NSDate? CreationDate {
			get { return PropertyTo<NSDate> (ABPersonPropertyId.CreationDate); }
			set { SetValue (ABPersonPropertyId.CreationDate, value); }
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s modification date.
		///         </summary>
		///         <value>
		///           A <see cref="Foundation.NSDate" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s modification date.
		///         </value>
		///         <remarks>
		///         </remarks>
		public NSDate? ModificationDate {
			get { return PropertyTo<NSDate> (ABPersonPropertyId.ModificationDate); }
			set { SetValue (ABPersonPropertyId.ModificationDate, value); }
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public ABMultiValue<PersonAddress>? GetAllAddresses ()
		{
			return CreateDictionaryMultiValue<PersonAddress> (CopyValue (ABPersonPropertyId.Address), l => new PersonAddress (l));
		}

		// Obsolete
		/// <param name="value">
		///           A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSDictionary" />
		///           containing the <see cref="AddressBook.ABPerson" />'s new addresses.
		///         </param>
		///         <summary>
		///           Sets the <see cref="AddressBook.ABPerson" />'s new addresses.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPersonAddressKey" />
		///         <altmember cref="AddressBook.ABPerson.GetAllAddresses" />
		public void SetAddresses (ABMultiValue<NSDictionary>? value)
		{
			SetValue (ABPersonPropertyId.Address, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <param name="addresses">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetAddresses (ABMultiValue<PersonAddress>? addresses)
		{
			SetValue (ABPersonPropertyId.Address, addresses.GetHandle ());
			GC.KeepAlive (addresses);
		}

		// Obsolete
		static ABMultiValue<NSDictionary>? CreateDictionaryMultiValue (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;
			return new ABMultiValue<NSDictionary> (handle, true);
		}

		static ABMultiValue<T>? CreateDictionaryMultiValue<T> (NativeHandle handle, Func<NSDictionary, T> factory) where T : DictionaryContainer
		{
			if (handle == NativeHandle.Zero)
				return null;

			return new ABMultiValue<T> (handle,
				l => factory ((NSDictionary) (object) Runtime.GetNSObject (l)!),
				l => Runtime.RetainAndAutoreleaseNSObject (l.Dictionary),
				false);
		}

		/// <summary>
		///           Gets the <see cref="AddressBook.ABPerson" />'s dates.
		///         </summary>
		///         <returns>
		///           A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSDate" />
		///           containing the <see cref="AddressBook.ABPerson" />'s dates.
		///         </returns>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPersonDateLabel" />
		///         <altmember cref="AddressBook.ABPerson.SetDates(AddressBook.ABMultiValue{Foundation.NSDate})" />
		public ABMultiValue<NSDate>? GetDates ()
		{
			return CreateDateMultiValue (CopyValue (ABPersonPropertyId.Date));
		}

		static ABMultiValue<NSDate>? CreateDateMultiValue (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;
			return new ABMultiValue<NSDate> (handle, true);
		}

		/// <param name="value">
		///           A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSDate" />
		///           containing the <see cref="AddressBook.ABPerson" />'s new dates.
		///         </param>
		///         <summary>
		///           Sets the <see cref="AddressBook.ABPerson" />'s new dates.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPersonDateLabel" />
		///         <altmember cref="AddressBook.ABPerson.GetDates" />
		public void SetDates (ABMultiValue<NSDate>? value)
		{
			SetValue (ABPersonPropertyId.Date, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <summary>
		///           Gets or sets the <see cref="AddressBook.ABPerson" />'s
		///           <see cref="AddressBook.ABPersonKind" />.
		///         </summary>
		///         <value>
		///           A <see cref="AddressBook.ABPersonKind" /> containing
		///           the <see cref="AddressBook.ABPerson" />'s kind.
		///         </value>
		///         <remarks>
		///           <para>
		///             The <see cref="AddressBook.ABPersonKind" />
		///             controls whether the instance is a person or an organization.
		///           </para>
		///         </remarks>
		public ABPersonKind PersonKind {
			get { return ABPersonKindId.ToPersonKind (PropertyTo<NSNumber> (ABPersonPropertyId.Kind!)!); }
			set { SetValue (ABPersonPropertyId.Kind!, ABPersonKindId.FromPersonKind (value)); }
		}

		/// <summary>
		///           Gets the <see cref="AddressBook.ABPerson" />'s phone numbers.
		///         </summary>
		///         <returns>
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s phone numbers.
		///         </returns>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="SetPhones" />
		public ABMultiValue<string>? GetPhones ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.Phone));
		}

		/// <param name="value">
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s new phone numbers.
		///         </param>
		///         <summary>
		///           Sets the <see cref="AddressBook.ABPerson" />'s new phone numbers.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPerson.GetPhones" />
		public void SetPhones (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.Phone, value.GetHandle ());
			GC.KeepAlive (value);
		}

		ABMultiValue<NSDictionary>? GetInstantMessages ()
		{
			return CreateDictionaryMultiValue (CopyValue (ABPersonPropertyId.InstantMessage));
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public ABMultiValue<InstantMessageService>? GetInstantMessageServices ()
		{
			return CreateDictionaryMultiValue<InstantMessageService> (CopyValue (ABPersonPropertyId.InstantMessage), l => new InstantMessageService (l));
		}

		// Obsolete
		/// <summary>
		///   Sets the <see cref="AddressBook.ABPerson" />'s new instant messaging services.
		/// </summary>
		/// <param name="value">
		///   A <see cref="AddressBook.ABMultiValue{T}" /> of <see cref="Foundation.NSDictionary" />
		///   containing the <see cref="AddressBook.ABPerson" />'s new instant messaging services.
		/// </param>
		/// <remarks>
		/// </remarks>
		/// <altmember cref="AddressBook.ABPerson.GetInstantMessageServices" />
		public void SetInstantMessages (ABMultiValue<NSDictionary>? value)
		{
			SetValue (ABPersonPropertyId.InstantMessage, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <param name="services">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetInstantMessages (ABMultiValue<InstantMessageService>? services)
		{
			SetValue (ABPersonPropertyId.InstantMessage, services.GetHandle ());
			GC.KeepAlive (services);
		}

		[Advice ("Use GetSocialProfiles.")]
		ABMultiValue<NSDictionary>? GetSocialProfile ()
		{
			return CreateDictionaryMultiValue (CopyValue (ABPersonPropertyId.SocialProfile));
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public ABMultiValue<SocialProfile>? GetSocialProfiles ()
		{
			return CreateDictionaryMultiValue<SocialProfile> (CopyValue (ABPersonPropertyId.SocialProfile), l => new SocialProfile (l));
		}

		// Obsolete
		/// <param name="value">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetSocialProfile (ABMultiValue<NSDictionary>? value)
		{
			SetValue (ABPersonPropertyId.SocialProfile, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <param name="profiles">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetSocialProfile (ABMultiValue<SocialProfile>? profiles)
		{
			SetValue (ABPersonPropertyId.SocialProfile, profiles.GetHandle ());
			GC.KeepAlive (profiles);
		}

		/// <summary>
		///           Gets the <see cref="AddressBook.ABPerson" />'s URLs.
		///         </summary>
		///         <returns>
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s URLs.
		///         </returns>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="SetUrls" />
		public ABMultiValue<string>? GetUrls ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.Url));
		}

		/// <param name="value">
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s new URLs.
		///         </param>
		///         <summary>
		///           Sets the <see cref="AddressBook.ABPerson" />'s new URLs.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPerson.GetUrls" />
		public void SetUrls (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.Url, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <summary>
		///           Gets the <see cref="AddressBook.ABPerson" />'s related names.
		///         </summary>
		///         <returns>
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s related names.
		///         </returns>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="SetRelatedNames" />
		public ABMultiValue<string>? GetRelatedNames ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.RelatedNames));
		}

		/// <param name="value">
		///           A <see cref="AddressBook.ABMultiValue{T}" />
		///           containing the <see cref="AddressBook.ABPerson" />'s new related names.
		///         </param>
		///         <summary>
		///           Sets the <see cref="AddressBook.ABPerson" />'s new related names.
		///         </summary>
		///         <remarks>
		///         </remarks>
		///         <altmember cref="AddressBook.ABPerson.GetRelatedNames" />
		public void SetRelatedNames (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.RelatedNames, value.GetHandle ());
			GC.KeepAlive (value);
		}

		/// <param name="property">
		///           A <see cref="AddressBook.ABPersonProperty" />
		///           specifying which property to return.
		///         </param>
		///         <summary>
		///           Gets the specified property.
		///         </summary>
		///         <returns>
		///           A <see cref="System.Object" /> containing the value of the specified property.
		///         </returns>
		///         <remarks>
		///         </remarks>
		public object? GetProperty (ABPersonProperty property)
		{
			switch (property) {
			case ABPersonProperty.Address: return GetAllAddresses ();
			case ABPersonProperty.Birthday: return Birthday;
			case ABPersonProperty.CreationDate: return CreationDate;
			case ABPersonProperty.Date: return GetDates ();
			case ABPersonProperty.Department: return Department;
			case ABPersonProperty.Email: return GetEmails ();
			case ABPersonProperty.FirstName: return FirstName;
			case ABPersonProperty.FirstNamePhonetic: return FirstNamePhonetic;
			case ABPersonProperty.InstantMessage: return GetInstantMessages ();
			case ABPersonProperty.JobTitle: return JobTitle;
			case ABPersonProperty.Kind: return PersonKind;
			case ABPersonProperty.LastName: return LastName;
			case ABPersonProperty.LastNamePhonetic: return LastNamePhonetic;
			case ABPersonProperty.MiddleName: return MiddleName;
			case ABPersonProperty.MiddleNamePhonetic: return MiddleNamePhonetic;
			case ABPersonProperty.ModificationDate: return ModificationDate;
			case ABPersonProperty.Nickname: return Nickname;
			case ABPersonProperty.Note: return Note;
			case ABPersonProperty.Organization: return Organization;
			case ABPersonProperty.Phone: return GetPhones ();
			case ABPersonProperty.Prefix: return Prefix;
			case ABPersonProperty.RelatedNames: return GetRelatedNames ();
			case ABPersonProperty.Suffix: return Suffix;
			case ABPersonProperty.Url: return GetUrls ();
			case ABPersonProperty.SocialProfile: return GetSocialProfile ();
			}
			throw new ArgumentException ("Invalid property value: " + property);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyArrayOfAllLinkedPeople (IntPtr person);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public ABPerson? []? GetLinkedPeople ()
		{
			var linked = ABPersonCopyArrayOfAllLinkedPeople (Handle);
			return NSArray.ArrayFromHandle (linked, l => new ABPerson (l, null), releaseHandle: true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyImageDataWithFormat (IntPtr handle, nint format);

		/// <param name="format">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public NSData? GetImage (ABPersonImageFormat format)
		{
			return Runtime.GetNSObject<NSData> (ABPersonCopyImageDataWithFormat (Handle, (nint) (long) format), true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreateVCardRepresentationWithPeople (IntPtr people);

		/// <param name="people">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static NSData? GetVCards (params ABPerson [] people)
		{
			if (people is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (people));

			var ptrs = new NativeHandle [people.Length];
			for (int i = 0; i < people.Length; ++i) {
				ptrs [i] = people [i].Handle;
			}

			var ptr = ABPersonCreateVCardRepresentationWithPeople (CFArray.Create (ptrs));
			return Runtime.GetNSObject<NSData> (ptr, true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreatePeopleInSourceWithVCardRepresentation (IntPtr source, IntPtr vCardData);

		/// <param name="source">To be added.</param>
		///         <param name="vCardData">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static ABPerson? []? CreateFromVCard (ABRecord? source, NSData vCardData)
		{
			if (vCardData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (vCardData));

			// TODO: SIGSEGV when source is not null
			var res = ABPersonCreatePeopleInSourceWithVCardRepresentation (source.GetHandle (), vCardData.Handle);

			GC.KeepAlive (source);
			GC.KeepAlive (vCardData);

			return NSArray.ArrayFromHandle (res, l => new ABPerson (l, null), releaseHandle: true);
		}
	}

	/// <summary>Manages social profile configuration.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class SocialProfile : DictionaryContainer {
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public SocialProfile ()
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public SocialProfile (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Social profile service name.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonSocialProfileServiceKey value to access the underlying dictionary.</remarks>
		public string? ServiceName {
			get {
				return GetStringValue (ABPersonSocialProfile.ServiceKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.ServiceKey!, value);
			}
		}

		/// <summary>Represents the social profile username.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonSocialProfileUsernameKey value to access the underlying dictionary.</remarks>
		public string? Username {
			get {
				return GetStringValue (ABPersonSocialProfile.UsernameKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.UsernameKey!, value);
			}
		}

		/// <summary>Represents the social profile user identifier.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonSocialProfileUserIdentifierKey value to access the underlying dictionary.</remarks>
		public string? UserIdentifier {
			get {
				return GetStringValue (ABPersonSocialProfile.UserIdentifierKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.UserIdentifierKey!, value);
			}
		}

		/// <summary>Represents the social profile URL.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonSocialProfileURLKey value to access the underlying dictionary.</remarks>
		public string? Url {
			get {
				return GetStringValue (ABPersonSocialProfile.URLKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.URLKey!, value);
			}
		}
	}

	/// <summary>Manages instance message service configuration.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class InstantMessageService : DictionaryContainer {
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public InstantMessageService ()
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public InstantMessageService (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Instant message service name.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonInstantMessageServiceKey value to access the underlying dictionary.</remarks>
		public string? ServiceName {
			get {
				// TODO: It does not return ABPersonInstantMessageService value. Underlying
				// value is custom string, it coould be MT bug because this makes
				// ABPersonInstantMessageService constants useless
				return GetStringValue (ABPersonInstantMessageKey.Service!);
			}
			set {
				SetStringValue (ABPersonInstantMessageKey.Service!, value);
			}
		}

		/// <summary>Instant message service user name.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonInstantMessageUsernameKey value to access the underlying dictionary.</remarks>
		public string? Username {
			get {
				return GetStringValue (ABPersonInstantMessageKey.Username!);
			}
			set {
				SetStringValue (ABPersonInstantMessageKey.Username!, value);
			}
		}
	}

	/// <summary>Manages the person address.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[SupportedOSPlatform ("maccatalyst")]
	[ObsoletedOSPlatform ("maccatalyst", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class PersonAddress : DictionaryContainer {
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public PersonAddress ()
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public PersonAddress (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>City</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonAddressProperty value to access the underlying dictionary.</remarks>
		public string? City {
			get {
				return GetStringValue (ABPersonAddressKey.City!);
			}
			set {
				SetStringValue (ABPersonAddressKey.City!, value);
			}
		}

		/// <summary>Represents country name.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonAddressCountryKey value to access the underlying dictionary.</remarks>
		public string? Country {
			get {
				return GetStringValue (ABPersonAddressKey.Country!);
			}
			set {
				SetStringValue (ABPersonAddressKey.Country!, value);
			}
		}

		/// <summary>Represents country code.</summary>
		///         <value>The value must be in the form of 2 character ISO-3166 country codes.</value>
		///         <remarks>The property uses constant kABPersonAddressCountryCodeKey value to access the underlying dictionary.</remarks>
		public string? CountryCode {
			get {
				return GetStringValue (ABPersonAddressKey.CountryCode!);
			}
			set {
				SetStringValue (ABPersonAddressKey.CountryCode!, value);
			}
		}

		/// <summary>State</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonAddressStateKey value to access the underlying dictionary.</remarks>
		public string? State {
			get {
				return GetStringValue (ABPersonAddressKey.State!);
			}
			set {
				SetStringValue (ABPersonAddressKey.State!, value);
			}
		}

		/// <summary>Street</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonAddressStreetKey value to access the underlying dictionary.</remarks>
		public string? Street {
			get {
				return GetStringValue (ABPersonAddressKey.Street!);
			}
			set {
				SetStringValue (ABPersonAddressKey.Street!, value);
			}
		}

		/// <summary>ZIP</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kABPersonAddressZIPKey value to access the underlying dictionary.</remarks>
		public string? Zip {
			get {
				return GetStringValue (ABPersonAddressKey.Zip!);
			}
			set {
				SetStringValue (ABPersonAddressKey.Zip!, value);
			}
		}
	}
}

#endif // !MONOMAC
