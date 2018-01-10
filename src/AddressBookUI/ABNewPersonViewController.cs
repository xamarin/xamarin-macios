// 
// ABPersonViewController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

using System;

using AddressBook;
using Foundation;
using ObjCRuntime;

namespace AddressBookUI {

	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public class ABNewPersonCompleteEventArgs : EventArgs {

		public ABNewPersonCompleteEventArgs (ABPerson person)
		{
			Person = person;
		}

		public ABPerson Person {get; private set;}
		public bool Completed {
			get {return Person != null;}
		}
	}

	class InternalABNewPersonViewControllerDelegate : ABNewPersonViewControllerDelegate {

		internal EventHandler<ABNewPersonCompleteEventArgs> newPersonComplete;

		public InternalABNewPersonViewControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
#if XAMCORE_2_0
		public override void DidCompleteWithNewPerson (ABNewPersonViewController controller, ABPerson person)
		{
			controller.OnNewPersonComplete (new ABNewPersonCompleteEventArgs (person));
		}
#else
		public override void DidCompleteWithNewPerson (ABNewPersonViewController controller, IntPtr person)
		{
			controller.OnNewPersonComplete (
					new ABNewPersonCompleteEventArgs (
						person == IntPtr.Zero ? null : new ABPerson (person, controller.AddressBook)));
		}
#endif
	}

	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	partial class ABNewPersonViewController {

		ABPerson displayedPerson;
		public ABPerson DisplayedPerson {
			get {
				MarkDirty ();
				return BackingField.Get (ref displayedPerson, _DisplayedPerson, h => new ABPerson (h, AddressBook));
			}
			set {
				_DisplayedPerson = BackingField.Save (ref displayedPerson, value);
				MarkDirty ();
			}
		}

		ABAddressBook addressBook;
		public ABAddressBook AddressBook {
			get {
				MarkDirty ();
				return BackingField.Get (ref addressBook, _AddressBook, h => new ABAddressBook (h, false));
			}
			set {
				_AddressBook = BackingField.Save (ref addressBook, value);
				MarkDirty ();
			}
		}

		ABGroup parentGroup;
		public ABGroup ParentGroup {
			get {
				MarkDirty ();
				return BackingField.Get (ref parentGroup, _ParentGroup, h => new ABGroup (h, AddressBook));
			}
			set {
				_AddressBook = BackingField.Save (ref parentGroup, value);
				MarkDirty ();
			}
		}

		InternalABNewPersonViewControllerDelegate EnsureEventDelegate ()
		{
			var d = WeakDelegate as InternalABNewPersonViewControllerDelegate;
			if (d == null) {
				d = new InternalABNewPersonViewControllerDelegate ();
				WeakDelegate = d;
			}
			return d;
		}

		protected internal virtual void OnNewPersonComplete (ABNewPersonCompleteEventArgs e)
		{
			var h = EnsureEventDelegate ().newPersonComplete;
			if (h != null)
				h (this, e);
		}

		public event EventHandler<ABNewPersonCompleteEventArgs> NewPersonComplete {
			add {EnsureEventDelegate ().newPersonComplete += value;}
			remove {EnsureEventDelegate ().newPersonComplete -= value;}
		}
	}
}

