// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.Reflection;
using Foundation;
using NUnit.Framework;

namespace Linker.Shared.Reflection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class ReflectionTest {

		public void MethodWithParameters (string firstParameter, int secondParameter)
		{
		}

		[Test]
		public void ParameterInfoName ()
		{
			// linker will disable the metadata removal optimization if that property is used by user code
			// however it's used inside mscorlib.dll (and SDK) so it cannot be checked while testing
			//Assert.Null (typeof (ParameterInfo).GetProperty ("Name"), "Name");

			var mi = this.GetType ().GetMethod ("MethodWithParameters");
			var p = mi.GetParameters ();
#if DEBUG && !NET
			// dotnet has adopted (and adapted) the metadata reducer and runs it on it's own conditions
			var optimized = false;
#else
			var optimized = TestRuntime.IsLinkAll;
#endif

			if (!optimized) {
				// this optimization is only applied for release builds (not debug ones)
				// link sdk won't touch this assembly (user code) so the parameters will be available
				Assert.That (p [0].ToString (), Is.EqualTo ("System.String firstParameter"), "1");
				Assert.That (p [1].ToString (), Is.EqualTo ("Int32 secondParameter"), "2");
			} else {
				Assert.That (p [0].ToString (), Is.EqualTo ("System.String ").Or.EqualTo ("System.String"), "1");
				Assert.That (p [1].ToString (), Is.EqualTo ("Int32 ").Or.EqualTo ("Int32"), "2");
			}
		}
	}
}
