//
// Unit tests for CAEmitterBehavior
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !NET

using Foundation;
using CoreAnimation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreAnimation {

	[Ignore ("CAEmitterBehavior is now being rejected by Apple")]
	// kept around in case it gets ressurected since the radar is not closed
	// https://trello.com/c/J8BDDUV9/86-33590997-coreanimation-quartzcore-api-removals
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EmitterBehaviorTest {

		[Test]
		public void AllBehaviorTypes ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			// turns out there's 2 undocumented behaviors: colorOverDistance and valueOverDistance
			foreach (var type in CAEmitterBehavior.BehaviorTypes) {
				using (var eb = CAEmitterBehavior.Create (type)) {
					Assert.True (eb.Enabled, type + ".Enabled");
					Assert.Null (eb.Name, type + ".Name");
					Assert.That (eb.Type, Is.EqualTo ((string) type), type + ".Type");
				}
			}
		}

		[Test]
		public void ColorOverDistance ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			// undocumented - we'll track it over the betas :)
			using (var eb = CAEmitterBehavior.Create ((NSString) "colorOverDistance")) {
				Assert.True (eb.Enabled, "Enabled.1");
				eb.Enabled = false;
				Assert.False (eb.Enabled, "Enabled.2");

				Assert.Null (eb.Name, "Name.1");
				eb.Name = "Xamarin";
				Assert.That (eb.Name, Is.EqualTo ("Xamarin"), "Name.2");

				Assert.That (eb.Type, Is.EqualTo ("colorOverDistance"), "Type");
			}
		}

		[Test]
		public void ValueOverDistance ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			// undocumented - we'll track it over the betas :)
			using (var eb = CAEmitterBehavior.Create ((NSString) "valueOverDistance")) {
				Assert.True (eb.Enabled, "Enabled.1");
				eb.Enabled = false;
				Assert.False (eb.Enabled, "Enabled.2");

				Assert.Null (eb.Name, "Name.1");
				eb.Name = "Xamarin";
				Assert.That (eb.Name, Is.EqualTo ("Xamarin"), "Name.2");

				Assert.That (eb.Type, Is.EqualTo ("valueOverDistance"), "Type");
			}
		}
	}
}

#endif // !__WATCHOS__ && !NET
