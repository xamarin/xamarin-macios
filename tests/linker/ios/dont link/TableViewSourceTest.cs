//
// UITableViewSourceTest
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace DontLink.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TableViewSourceTest {

		// UITableViewSource is Xamarin.iOS specific and should include everything from
		// UITableViewDelegate and UITableViewDataSource - but it's easy to forget to
		// update its members (e.g. bug 8298 for iOS6 additions).

		// note: test executed inside the dontlink.app on purpose ;-)
		
		[Test]
		public void ValidateMembers ()
		{
			var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

			var methods = new List<string> ();
			foreach (var mi in typeof (UITableViewDelegate).GetMethods (flags))
				methods.Add (mi.ToString ());
			foreach (var mi in typeof (UITableViewDataSource).GetMethods (flags))
				methods.Add (mi.ToString ());

			var tvsource = new List<string> ();
			foreach (var mi in typeof (UITableViewSource).GetMethods (flags))
				tvsource.Add (mi.ToString ());

			methods.RemoveAll (delegate (string name) {
				return tvsource.Contains (name);
			});
			Assert.That (methods.Count, Is.EqualTo (0), "Incomplete bindings! Please add the following members to UITableViewSource: " + String.Join (", ", methods));
		}
	}
}

#endif // !__WATCHOS__
