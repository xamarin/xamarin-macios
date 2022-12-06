using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

using Bindings.Test2;

namespace Xamarin.BindingTests2 {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BindingTest {
		[Test]
		public void Test ()
		{
			Assert.AreEqual (42, CFunctions.getIntOfChocolate (), "chocolate");
			Assert.AreEqual (42, Bindings.Test.CFunctions.theUltimateAnswer (), "theUltimateAnswer");
		}
	}
}
