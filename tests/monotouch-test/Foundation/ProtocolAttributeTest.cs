//
// Unit tests for ProtocolAttribute (yeah, really!)
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ProtocolAttributeTest {

		[Test]
		public void ModelMustBeProtocol ()
		{
			int count = 0;

			//
			// Note that [Model], but no [Protocol] is not a universal truth (so it's
			// not enforced in the generator), but it should be true for monotouch.dll.
			//

			foreach (var type in typeof (NSObject).Assembly.GetTypes ()) {
				if (!type.IsSubclassOf (typeof (NSObject)))
					continue;

				var register = (RegisterAttribute) Attribute.GetCustomAttribute (type, typeof (RegisterAttribute), false);
				if (register is not null && !register.IsWrapper)
					continue;

				if (Attribute.GetCustomAttribute (type, typeof (ModelAttribute), false) is null)
					continue;

				if (Attribute.GetCustomAttribute (type, typeof (ProtocolAttribute), false) is null) {
					Console.WriteLine ("{0} must have a [Protocol] attribute if it has a [Model] attribute", type.FullName);
					count++;
				}
			}

			if (count > 0)
				Assert.Fail ("Found {0} types with a [Model] attribute (and no [Register(false)] attribute signalling that they're not wrapper types), but without a [Protocol] attribute.", count);
		}
	}
}
