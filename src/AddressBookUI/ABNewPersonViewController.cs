// 
// ABPersonViewController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using AddressBook;

namespace AddressBookUI {

	/// <summary>Provides data for the <see cref="AddressBookUI.ABNewPersonViewController.NewPersonComplete" /> event.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABNewPersonCompleteEventArgs : EventArgs {

		/// <param name="person">To be added.</param>
		///         <summary>Initializes a new instance of the ABNewPersonCompleteEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public ABNewPersonCompleteEventArgs (ABPerson? person)
		{
			Person = person;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPerson? Person { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Completed {
			get { return Person is not null; }
		}
	}

	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	class InternalABNewPersonViewControllerDelegate : ABNewPersonViewControllerDelegate {

		internal EventHandler<ABNewPersonCompleteEventArgs>? newPersonComplete;

		public InternalABNewPersonViewControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override void DidCompleteWithNewPerson (ABNewPersonViewController controller, ABPerson? person)
		{
			controller.OnNewPersonComplete (new ABNewPersonCompleteEventArgs (person));
		}
	}

	partial class ABNewPersonViewController {

		ABPerson? displayedPerson;
		/// <summary>Gets or sets the <see cref="AddressBook.ABPerson" /> whose data is used to prepopulate the <see cref="AddressBookUI.ABNewPersonViewController" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPerson? DisplayedPerson {
			get {
				MarkDirty ();
				return BackingField.Get (ref displayedPerson, _DisplayedPerson, h => new ABPerson (h, AddressBook));
			}
			set {
				_DisplayedPerson = BackingField.Save (ref displayedPerson, value);
				MarkDirty ();
			}
		}

		ABAddressBook? addressBook;
		/// <summary>Gets or sets the <see cref="AddressBook.ABAddressBook" /> to which the contact will be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABAddressBook? AddressBook {
			get {
				MarkDirty ();
				return BackingField.Get (ref addressBook, _AddressBook, h => new ABAddressBook (h, false));
			}
			set {
				_AddressBook = BackingField.Save (ref addressBook, value);
				MarkDirty ();
			}
		}

		ABGroup? parentGroup;
		/// <summary>Gets or sets the <see cref="AddressBook.ABGroup" /> to which the new contact should be saved.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABGroup? ParentGroup {
			get {
				MarkDirty ();
				return BackingField.Get (ref parentGroup, _ParentGroup, h => (AddressBook is null) ? null : new ABGroup (h, AddressBook));
			}
			set {
				_AddressBook = BackingField.Save (ref parentGroup, value);
				MarkDirty ();
			}
		}

		InternalABNewPersonViewControllerDelegate EnsureEventDelegate ()
		{
			var d = WeakDelegate as InternalABNewPersonViewControllerDelegate;
			if (d is null) {
				d = new InternalABNewPersonViewControllerDelegate ();
				WeakDelegate = d;
			}
			return d;
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnNewPersonComplete (ABNewPersonCompleteEventArgs e)
		{
			var h = EnsureEventDelegate ().newPersonComplete;
			if (h is not null)
				h (this, e);
		}

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		public event EventHandler<ABNewPersonCompleteEventArgs> NewPersonComplete {
			add { EnsureEventDelegate ().newPersonComplete += value; }
			remove { EnsureEventDelegate ().newPersonComplete -= value; }
		}
	}
}
