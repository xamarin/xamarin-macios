//
// Unit tests for ABSource
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using AddressBook;
using ObjCRuntime;
#else
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AddressBook {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SourceTest {
		
		[Test]
		public void Default ()
		{
			if (Runtime.Arch != Arch.SIMULATOR)
				return;
			
			// we assume the simulator defaults (e.g. after a reset)
			ABSource source = new ABAddressBook ().GetDefaultSource ();
			Assert.Null (source.Name, "Name");
			Assert.That (source.SourceType, Is.EqualTo (ABSourceType.Local), "SourceType");

			// ABRecord
			// some bots returns -1 (invalid) and I get 0 after a reset (maybe permission related?)
			Assert.That (source.Id, Is.LessThanOrEqualTo (0), "Id");
			// iOS [9,11.2[ returned ABRecordType.Person, otherwise ABRecordType.Source
			Assert.That (source.Type, Is.Not.EqualTo (ABRecordType.Group), "Type");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
