using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;

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

