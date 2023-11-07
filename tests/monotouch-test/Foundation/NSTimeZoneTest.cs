//
// Unit tests for NSTimeZone
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSTimeZoneTest {

		[Test]
		public void KnownTimeZoneNames ()
		{
			Assert.That (NSTimeZone.KnownTimeZoneNames.Count, Is.GreaterThan (400), "KnownTimeZoneNames"); // 416 on iOS5
		}

		[Test]
		public void AbbreviationsTest ()
		{
			Assert.That (NSTimeZone.Abbreviations.ContainsKey (new NSString ("CST")));
		}

		[Test]
		public void AbbreviationTest ()
		{
			var timezone = NSTimeZone.LocalTimeZone;
			Assert.NotNull (timezone.Abbreviation ());
		}

		[Test]
		public void All_28300 ()
		{
			foreach (var name in NSTimeZone.KnownTimeZoneNames) {
				// simulator uses OSX to get timezones which might have some holes,
				// e.g. @"Pacific/Bougainville" does not seems to be available in Mavericks
#if !MONOMAC && !__MACCATALYST__
				if (Runtime.Arch == Arch.SIMULATOR) {
					if (!File.Exists (Path.Combine ("/usr/share/zoneinfo/", name)))
						continue;
				}
#endif
				TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById (name);
				Assert.NotNull (tzi.GetUtcOffset (DateTime.Now), name);
			}

			Assert.NotNull (TimeZoneInfo.Local.GetUtcOffset (DateTime.Now), "Local");
		}
	}
}
