//
// Unit tests for CMTimeRange
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
using System;
using Foundation;
using CoreMedia;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMTimeRangeTests {
		[Test]
		public void InvalidRangeFieldTest ()
		{
			var invalid = CMTimeRange.InvalidRange;
			Assert.NotNull (invalid, "CMTimeRange.InvalidRange Should Not be null");
			Assert.NotNull (invalid.Duration, "CMTimeRange.InvalidRange.Duration Should Not be null");
			Assert.NotNull (invalid.Start, "CMTimeRange.InvalidRange.Duration Should Not be null");
			Assert.That (invalid.Duration.IsInvalid, "CMTimeRange.InvalidRange.Duration.IsInvalid");
			Assert.That (invalid.Start.IsInvalid, "CMTimeRange.InvalidRange.Start.IsInvalid");
			Assert.That (invalid.Duration.Description, Is.EqualTo ("{INVALID}"), "Duration Description");
			Assert.That (invalid.Start.Description, Is.EqualTo ("{INVALID}"), "Start Description");
		}

		[Test]
		public void InvalidMappingFieldTest ()
		{
			var invalid = CMTimeRange.InvalidMapping;
			Assert.NotNull (invalid, "CMTimeRange.InvalidMapping Should Not be null");
			Assert.NotNull (invalid.Duration, "CMTimeRange.InvalidMapping.Duration Should Not be null");
			Assert.NotNull (invalid.Start, "CMTimeRange.InvalidMapping.Duration Should Not be null");
			Assert.That (invalid.Duration.IsInvalid, "CMTimeRange.InvalidMapping.Duration.IsInvalid");
			Assert.That (invalid.Start.IsInvalid, "CMTimeRange.InvalidMapping.Start.IsInvalid");
			Assert.That (invalid.Duration.Description, Is.EqualTo ("{INVALID}"), "Duration Description");
			Assert.That (invalid.Start.Description, Is.EqualTo ("{INVALID}"), "Start Description");
		}

		[Test]
		public void ZeroFieldTest ()
		{
			var zero = CMTimeRange.Zero;
			Assert.NotNull (zero, "CMTimeRange.Zero Should Not be null");
			Assert.NotNull (zero.Duration, "CMTimeRange.Zero.Duration Should Not be null");
			Assert.NotNull (zero.Start, "CMTimeRange.Zero.Duration Should Not be null");
			Assert.That (!zero.Duration.IsInvalid, "CMTimeRange.Zero.Duration.IsInvalid");
			Assert.That (!zero.Start.IsInvalid, "CMTimeRange.Zero.Start.IsInvalid");
			Assert.That (zero.Duration.Description, Is.EqualTo ("{0/1 = 0.000}"), "Duration Description");
			Assert.That (zero.Start.Description, Is.EqualTo ("{0/1 = 0.000}"), "Start Description");
		}
	}
}
