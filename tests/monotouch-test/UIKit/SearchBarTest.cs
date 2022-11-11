// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SearchBarTest {

#if !__TVOS__
		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UISearchBar sb = new UISearchBar (frame)) {
				Assert.That (sb.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void TextInput ()
		{
			// in iOS 7.1 the "Text Input Properties" stopped responsind in the introspection tests
			using (UISearchBar sb = new UISearchBar ()) {
				// that's a check that the feature still works
				Assert.That (sb.AutocapitalizationType, Is.EqualTo (UITextAutocapitalizationType.Sentences), "AutocapitalizationType");
				sb.AutocapitalizationType = UITextAutocapitalizationType.AllCharacters;

				// looks like 7.1 was alone to select 'default' over 'no' since iOS8 returned to old default
				var act = UIDevice.CurrentDevice.SystemVersion.StartsWith ("7.1") ? UITextAutocorrectionType.Default : UITextAutocorrectionType.No;
				Assert.That (sb.AutocorrectionType, Is.EqualTo (act), "AutocorrectionType");

				sb.AutocorrectionType = UITextAutocorrectionType.Yes;
				Assert.That (sb.KeyboardType, Is.EqualTo (UIKeyboardType.Default), "KeyboardType");
				sb.KeyboardType = UIKeyboardType.EmailAddress;
				Assert.That (sb.SpellCheckingType, Is.EqualTo (UITextSpellCheckingType.Default), "SpellCheckingType");
				sb.SpellCheckingType = UITextSpellCheckingType.Default;
			}
		}
#endif
	}
}

#endif // !__WATCHOS__
