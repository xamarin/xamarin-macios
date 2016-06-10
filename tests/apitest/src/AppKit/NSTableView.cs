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
	public class NSTableViewTests
	{
		[Test]
		public void NSTableView_DelegateDataSourceNull ()
		{
			NSTableView v = new NSTableView ();
			v.WeakDelegate = null;
			v.Delegate = null;
			v.WeakDataSource = null;
			v.DataSource = null;
		}
	}
}

