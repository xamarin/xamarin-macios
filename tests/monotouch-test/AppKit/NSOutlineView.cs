#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSOutlineViewTests {
		[Test]
		public void NSOutlineView_InsertNull ()
		{
			NSOutlineView v = new NSOutlineView ();
			v.BeginUpdates (); // We do this to prevent a crash: Insert/remove/move only works within a -beginUpdates/-endUpdates block or a View Based TableView
			v.InsertItems (new NSIndexSet (0), null, NSTableViewAnimation.None);
			v.EndUpdates ();
		}

		[Test]
		public void NSOutlineView_DelegateDataSourceNull ()
		{
			NSOutlineView v = new NSOutlineView ();
			v.WeakDelegate = null;
			v.Delegate = null;
			v.WeakDataSource = null;
			v.DataSource = null;
		}
	}
}

#endif // __MACOS__
