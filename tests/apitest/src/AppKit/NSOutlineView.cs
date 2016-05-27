using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
#else
using AppKit;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSOutlineViewTests
	{
		[Test]
		public void NSOutlineView_InsertNull ()
		{
			NSOutlineView v = new NSOutlineView ();
			v.BeginUpdates (); // We do this to prevent a crash: Insert/remove/move only works within a -beginUpdates/-endUpdates block or a View Based TableView
#if !XAMCORE_2_0
			v.InsertItems (new NSIndexSet (0), null, NSTableViewAnimationOptions.EffectFade);
#else
			v.InsertItems (new NSIndexSet (0), null, NSTableViewAnimation.None);
#endif
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

