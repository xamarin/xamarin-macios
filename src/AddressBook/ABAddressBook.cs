// 
// ABAddressBook.cs: Implements the managed ABAddressBook
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
// Copyright 2011-2013 Xamarin Inc.
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

using Foundation;
using CoreFoundation;
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
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ExternalChangeEventArgs : EventArgs {
		public ExternalChangeEventArgs (ABAddressBook addressBook, NSDictionary? info)
		{
			AddressBook = addressBook;
			Info = info;
		}

		public ABAddressBook AddressBook { get; private set; }
		public NSDictionary? Info { get; private set; }
	}

	// Quoth the docs: 
	//    http://developer.apple.com/iphone/library/documentation/AddressBook/Reference/ABPersonRef_iPhoneOS/Reference/reference.html#//apple_ref/c/func/ABPersonGetSortOrdering
	//
	//   "The value of these constants is undefined until one of the following has
	//    been called: ABAddressBookCreate, ABPersonCreate, ABGroupCreate."
	//
	// Meaning we can't rely on static constructors, as they could be invoked
	// before those functions have been invoked. :-/
	//
	// Note that the above comment was removed from iOS 6.0+ documentation (and were not part of OSX docs AFAIK).
	// It make sense since it's not possible to call those functions, from 6.0+ they will return NULL on devices,
	// unless the application has been authorized to access the address book.
	static class InitConstants {
		public static void Init () { }

		static InitConstants ()
		{
#if __MACCATALYST__
			// avoid TypeLoadException if used before macOS 11.x
			if (!SystemVersion.CheckiOS (14,0))
				return;
#endif
			// ensure we can init. This is needed before iOS6 (as per doc).
			IntPtr p = ABAddressBook.ABAddressBookCreate ();

			ABGroupProperty.Init ();
			ABLabel.Init ();
			ABPersonAddressKey.Init ();
			ABPersonDateLabel.Init ();
			ABPersonInstantMessageKey.Init ();
			ABPersonInstantMessageService.Init ();
			ABPersonKindId.Init ();
			ABPersonPhoneLabel.Init ();
			ABPersonPropertyId.Init ();
			ABPersonRelatedNamesLabel.Init ();
			ABPersonUrlLabel.Init ();
			ABSourcePropertyId.Init ();

			// From iOS 6.0+ this might return NULL, e.g. if the application is not authorized to access the
			// address book, and we would crash if we tried to release a null pointer
			if (p != IntPtr.Zero)
				CFObject.CFRelease (p);
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
	public class ABAddressBook : NativeObject, IEnumerable<ABRecord> {

		public static readonly NSString ErrorDomain;

		GCHandle sender;

		[DllImport (Constants.AddressBookLibrary)]
		internal extern static IntPtr ABAddressBookCreate ();

#if NET
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("ios")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
		[ObsoletedOSPlatform ("ios6.0", "Use the static Create method instead.")]
#else
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use the static Create method instead")]
#endif
		public ABAddressBook ()
			: this (ABAddressBookCreate (), true)
		{
		}

		[DllImport (Constants.AddressBookLibrary)]
		internal extern static IntPtr ABAddressBookCreateWithOptions (IntPtr dictionary, out IntPtr cfError);

		public static ABAddressBook? Create (out NSError? error)
		{
			var handle = ABAddressBookCreateWithOptions (IntPtr.Zero, out var e);
			if (handle == IntPtr.Zero) {
				error = Runtime.GetNSObject<NSError> (e);
				return null;
			}
			error = null;
			return new ABAddressBook (handle, true);
		}

		[Preserve (Conditional = true)]
		internal ABAddressBook (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
			InitConstants.Init ();
		}

		static ABAddressBook ()
		{
			ErrorDomain = Dlfcn.GetStringConstant (Libraries.AddressBook.Handle, "ABAddressBookErrorDomain")!;
		}

		protected override void Dispose (bool disposing)
		{
			if (sender.IsAllocated)
				sender.Free ();
			base.Dispose (disposing);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static nint ABAddressBookGetAuthorizationStatus ();

		public static ABAuthorizationStatus GetAuthorizationStatus ()
		{
			return (ABAuthorizationStatus) (long) ABAddressBookGetAuthorizationStatus ();
		}

		[DllImport (Constants.AddressBookLibrary)]
		unsafe extern static void ABAddressBookRequestAccessWithCompletion (IntPtr addressbook, BlockLiteral* completion);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void RequestAccess (Action<bool, NSError?> onCompleted)
		{
			if (onCompleted is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (onCompleted));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, byte, IntPtr, void> trampoline = &TrampolineCompletionHandler;
				using var block = new BlockLiteral (trampoline, onCompleted, typeof (ABAddressBook), nameof (TrampolineCompletionHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_completionHandler, onCompleted);
#endif
				ABAddressBookRequestAccessWithCompletion (Handle, &block);
			}
		}

#if !NET
		internal delegate void InnerCompleted (IntPtr block, byte success, IntPtr error);
		static readonly InnerCompleted static_completionHandler = TrampolineCompletionHandler;
		[MonoPInvokeCallback (typeof (InnerCompleted))]
#else
		[UnmanagedCallersOnly]
#endif
		static unsafe void TrampolineCompletionHandler (IntPtr block, byte success, IntPtr error)
		{
			var descriptor = (BlockLiteral*) block;
			var del = descriptor->Target as Action<bool, NSError?>;
			if (del is not null)
				del (success != 0, Runtime.GetNSObject<NSError> (error));
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABAddressBookHasUnsavedChanges (IntPtr addressBook);
		public bool HasUnsavedChanges {
			get {
				return ABAddressBookHasUnsavedChanges (GetCheckedHandle ());
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABAddressBookSave (IntPtr addressBook, out IntPtr error);
		public void Save ()
		{
			if (!ABAddressBookSave (GetCheckedHandle (), out var error))
				throw CFException.FromCFError (error);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static void ABAddressBookRevert (IntPtr addressBook);
		public void Revert ()
		{
			ABAddressBookRevert (GetCheckedHandle ());
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABAddressBookAddRecord (IntPtr addressBook, IntPtr record, out IntPtr error);
		public void Add (ABRecord record)
		{
			if (record is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (record));

			if (!ABAddressBookAddRecord (GetCheckedHandle (), record.Handle, out var error))
				throw CFException.FromCFError (error);
			record.AddressBook = this;
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABAddressBookRemoveRecord (IntPtr addressBook, IntPtr record, out IntPtr error);
		public void Remove (ABRecord record)
		{
			if (record is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (record));

			if (!ABAddressBookRemoveRecord (GetCheckedHandle (), record.Handle, out var error))
				throw CFException.FromCFError (error);
			record.AddressBook = null;
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static nint ABAddressBookGetPersonCount (IntPtr addressBook);
		public nint PeopleCount {
			get {
				return ABAddressBookGetPersonCount (GetCheckedHandle ());
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyArrayOfAllPeople (IntPtr addressBook);
		public ABPerson [] GetPeople ()
		{
			var cfArrayRef = ABAddressBookCopyArrayOfAllPeople (GetCheckedHandle ());
			return NSArray.ArrayFromHandle (cfArrayRef, h => new ABPerson (h, this));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyArrayOfAllPeopleInSource (IntPtr addressBook, IntPtr source);

		public ABPerson [] GetPeople (ABRecord source)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			var cfArrayRef = ABAddressBookCopyArrayOfAllPeopleInSource (GetCheckedHandle (), source.Handle);
			return NSArray.ArrayFromHandle (cfArrayRef, l => new ABPerson (l, this));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyArrayOfAllPeopleInSourceWithSortOrdering (IntPtr addressBook, IntPtr source, ABPersonSortBy sortOrdering);

		public ABPerson [] GetPeople (ABRecord source, ABPersonSortBy sortOrdering)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			var cfArrayRef = ABAddressBookCopyArrayOfAllPeopleInSourceWithSortOrdering (GetCheckedHandle (), source.Handle, sortOrdering);
			return NSArray.ArrayFromHandle (cfArrayRef, l => new ABPerson (l, this));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static nint ABAddressBookGetGroupCount (IntPtr addressBook);
		public nint GroupCount {
			get {
				return ABAddressBookGetGroupCount (GetCheckedHandle ());
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyArrayOfAllGroups (IntPtr addressBook);
		public ABGroup [] GetGroups ()
		{
			var cfArrayRef = ABAddressBookCopyArrayOfAllGroups (GetCheckedHandle ());
			return NSArray.ArrayFromHandle (cfArrayRef, h => new ABGroup (h, this));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyArrayOfAllGroupsInSource (IntPtr addressBook, IntPtr source);

		public ABGroup [] GetGroups (ABRecord source)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			var cfArrayRef = ABAddressBookCopyArrayOfAllGroupsInSource (GetCheckedHandle (), source.Handle);
			return NSArray.ArrayFromHandle (cfArrayRef, l => new ABGroup (l, this));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyLocalizedLabel (IntPtr label);
		public static string? LocalizedLabel (NSString label)
		{
			if (label is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (label));

			return CFString.FromHandle (ABAddressBookCopyLocalizedLabel (label.Handle));
		}

		[DllImport (Constants.AddressBookLibrary)]
#if NET
		extern unsafe static void ABAddressBookRegisterExternalChangeCallback (IntPtr addressBook,
			delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> callback, IntPtr context);
#else
		extern static void ABAddressBookRegisterExternalChangeCallback (IntPtr addressBook, ABExternalChangeCallback callback, IntPtr context);
#endif

		[DllImport (Constants.AddressBookLibrary)]
#if NET
		extern unsafe static void ABAddressBookUnregisterExternalChangeCallback (IntPtr addressBook, delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> callback, IntPtr context);
#else
		extern static void ABAddressBookUnregisterExternalChangeCallback (IntPtr addressBook, ABExternalChangeCallback callback, IntPtr context);
#endif

#if !NET
		delegate void ABExternalChangeCallback (IntPtr addressBook, IntPtr info, IntPtr context);

		[MonoPInvokeCallback (typeof (ABExternalChangeCallback))]
#else
		[UnmanagedCallersOnly]
#endif
		static void ExternalChangeCallback (IntPtr addressBook, IntPtr info, IntPtr context)
		{
			GCHandle s = GCHandle.FromIntPtr (context);
			var self = s.Target as ABAddressBook;
			if (self is null)
				return;
			self.OnExternalChange (new ExternalChangeEventArgs (
						   new ABAddressBook (addressBook, false),
						   Runtime.GetNSObject<NSDictionary> (info)));
		}

		object eventLock = new object ();

		EventHandler<ExternalChangeEventArgs>? externalChange;

		protected virtual void OnExternalChange (ExternalChangeEventArgs e)
		{
			GetCheckedHandle ();
			var h = externalChange;
			if (h is not null)
				h (this, e);
		}

		public event EventHandler<ExternalChangeEventArgs> ExternalChange {
			add {
				lock (eventLock) {
					if (externalChange is null) {
						sender = GCHandle.Alloc (this);
#if NET
						unsafe {
							ABAddressBookRegisterExternalChangeCallback (Handle, &ExternalChangeCallback, GCHandle.ToIntPtr (sender));
						}
#else
						ABAddressBookRegisterExternalChangeCallback (Handle, ExternalChangeCallback, GCHandle.ToIntPtr (sender));
#endif
					}
					externalChange += value;
				}
			}
			remove {
				lock (eventLock) {
					externalChange -= value;
					if (externalChange is null) {
#if NET
						unsafe {
							ABAddressBookUnregisterExternalChangeCallback (Handle, &ExternalChangeCallback, GCHandle.ToIntPtr (sender));
						}
#else
						ABAddressBookUnregisterExternalChangeCallback (Handle, ExternalChangeCallback, GCHandle.ToIntPtr (sender));
#endif
						sender.Free ();
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<ABRecord> GetEnumerator ()
		{
			GetCheckedHandle ();
			foreach (var p in GetPeople ())
				yield return p;
			foreach (var g in GetGroups ())
				yield return g;
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookGetGroupWithRecordID (IntPtr addressBook, int /* ABRecordID */ recordId);
		public ABGroup? GetGroup (int recordId)
		{
			var h = ABAddressBookGetGroupWithRecordID (Handle, recordId);
			if (h == IntPtr.Zero)
				return null;
			return new ABGroup (h, this);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookGetPersonWithRecordID (IntPtr addressBook, int /* ABRecordID */ recordId);
		public ABPerson? GetPerson (int recordId)
		{
			var h = ABAddressBookGetPersonWithRecordID (Handle, recordId);
			if (h == IntPtr.Zero)
				return null;
			return new ABPerson (h, this);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABAddressBookCopyPeopleWithName (IntPtr addressBook, IntPtr name);
		public ABPerson [] GetPeopleWithName (string name)
		{
			var nameHandle = CFString.CreateNative (name);
			try {
				var cfArrayRef = ABAddressBookCopyPeopleWithName (Handle, nameHandle);
				return NSArray.ArrayFromHandle (cfArrayRef, h => new ABPerson (h, this));
			} finally {
				CFString.ReleaseNative (nameHandle);
			}
		}

		// ABSource
		// http://developer.apple.com/library/IOs/#documentation/AddressBook/Reference/ABSourceRef_iPhoneOS/Reference/reference.html

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr /* CFArrayRef */ ABAddressBookCopyArrayOfAllSources (IntPtr /* ABAddressBookRef */ addressBook);
		public ABSource []? GetAllSources ()
		{
			var cfArrayRef = ABAddressBookCopyArrayOfAllSources (GetCheckedHandle ());
			return NSArray.ArrayFromHandle (cfArrayRef, h => new ABSource (h, this));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr /* ABRecordRef */ ABAddressBookCopyDefaultSource (IntPtr /* ABAddressBookRef */ addressBook);
		public ABSource? GetDefaultSource ()
		{
			var h = ABAddressBookCopyDefaultSource (GetCheckedHandle ());
			if (h == IntPtr.Zero)
				return null;
			return new ABSource (h, this);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr /* ABRecordRef */ ABAddressBookGetSourceWithRecordID (IntPtr /* ABAddressBookRef */ addressBook, int /* ABRecordID */ sourceID);
		public ABSource? GetSource (int sourceID)
		{
			var h = ABAddressBookGetSourceWithRecordID (GetCheckedHandle (), sourceID);
			if (h == IntPtr.Zero)
				return null;
			return new ABSource (h, this);
		}
	}
}

#endif // !MONOMAC
