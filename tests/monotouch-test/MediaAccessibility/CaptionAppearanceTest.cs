//
// MACaptionAppearance Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
using MediaAccessibility;
using NUnit.Framework;

namespace MonoTouchFixtures.MediaAccessibility {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class CaptionAppearanceTest {
		[Test]
		[Culture ("en")] // this setting depends on locale of the device according to apple docs on MACaptionAppearanceGetDisplayType, we know english works
		public void GetDisplayType ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			Assert.That (MACaptionAppearance.GetDisplayType (MACaptionAppearanceDomain.Default), Is.EqualTo (MACaptionAppearanceDisplayType.Automatic).Or.EqualTo (MACaptionAppearanceDisplayType.AlwaysOn).Or.EqualTo (MACaptionAppearanceDisplayType.ForcedOnly), "Default");
		}

		[Test]
		public void DidDisplayCaptions ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			// there's a known bug with UIPasteboard and NSAttributedString - and it makes our tests hang
			var nsa = new NSAttributedString [0];
			MACaptionAppearance.DidDisplayCaptions (nsa);
			nsa = new [] { new NSAttributedString ("Bonjour") };
			MACaptionAppearance.DidDisplayCaptions (nsa);
			nsa = null;
			MACaptionAppearance.DidDisplayCaptions (nsa);

			var a = new string [0];
			MACaptionAppearance.DidDisplayCaptions (a);
			a = new [] { "Hello", "World" };
			MACaptionAppearance.DidDisplayCaptions (a);
			a = null;
			MACaptionAppearance.DidDisplayCaptions (a);
		}

#if NET
		[Test]
		public void IsCustomized ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			foreach (var value in Enum.GetValues<MACaptionAppearanceDomain> ()) {
				Assert.That (MACaptionAppearance.IsCustomized (value), Is.EqualTo (true).Or.EqualTo (false), value.ToString ());
			}
		}
#endif // NET
	}
}

#endif // !__WATCHOS__
