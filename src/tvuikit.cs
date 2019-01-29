// Copyright 2018 Microsoft Corporation

using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TVUIKit {

	[TV (12,0)]
	[Native]
	public enum TVCaptionButtonViewMotionDirection : long {
		None = 0,
		Horizontal,
		Vertical,
		All,
	}

	[TV (12,0)]
	[BaseType (typeof (UIControl))]
	[DisableDefaultCtor] // initWithFrame is the designated initializer
	interface TVLockupView {

		[DesignatedInitializer] // inlined
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("contentView", ArgumentSemantic.Strong)]
		UIView ContentView { get; }

		[Export ("contentSize", ArgumentSemantic.Assign)]
		CGSize ContentSize { get; set; }

		[Export ("headerView", ArgumentSemantic.Strong)]
		TVLockupHeaderFooterView HeaderView { get; set; }

		[Export ("footerView", ArgumentSemantic.Strong)]
		TVLockupHeaderFooterView FooterView { get; set; }

		[Export ("contentViewInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets ContentViewInsets { get; set; }

		[Export ("focusSizeIncrease", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets FocusSizeIncrease { get; set; }
	}

	[TV (12,0)]
	[BaseType (typeof (TVLockupView))]
	interface TVCaptionButtonView {

		[DesignatedInitializer] // inlined
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("motionDirection", ArgumentSemantic.Assign)]
		TVCaptionButtonViewMotionDirection MotionDirection { get; set; }

		[NullAllowed, Export ("contentImage", ArgumentSemantic.Strong)]
		UIImage ContentImage { get; set; }

		[NullAllowed, Export ("contentText")]
		string ContentText { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }
	}

	[TV (12,0)]
	[BaseType (typeof (TVLockupView))]
	interface TVCardView {

		[DesignatedInitializer] // inlined
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NullAllowed, Export ("cardBackgroundColor", ArgumentSemantic.Strong)]
		UIColor CardBackgroundColor { get; set; }
	}

	[TV (12,0)]
	[BaseType (typeof (UIViewController))]
	// note: full screen so the default init makes sense
	interface TVDigitEntryViewController {
		[Export ("entryCompletionHandler", ArgumentSemantic.Copy)]
		Action<NSString> EntryCompletionHandler { get; set; }

		[NullAllowed, Export ("titleText")]
		string TitleText { get; set; }

		[NullAllowed, Export ("promptText")]
		string PromptText { get; set; }

		[Export ("secureDigitEntry")]
		bool SecureDigitEntry { [Bind ("isSecureDigitEntry")] get; set; }

		[Export ("numberOfDigits")]
		nuint NumberOfDigits { get; set; }

		[Export ("clearEntryAnimated:")]
		void ClearEntry (bool animated);
	}

	[TV (12,0)]
	[Protocol]
	interface TVLockupViewComponent {
		[Export ("updateAppearanceForLockupViewState:")]
		void UpdateAppearanceForLockupView (UIControlState state);
	}

	[TV (12,0)]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor] // initWithFrame is the designated initializer
	interface TVLockupHeaderFooterView : TVLockupViewComponent {

		[DesignatedInitializer] // inlined
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NullAllowed, Export ("titleLabel", ArgumentSemantic.Strong)]
		UILabel TitleLabel { get; }

		[NullAllowed, Export ("subtitleLabel", ArgumentSemantic.Strong)]
		UILabel SubtitleLabel { get; }

		[Export ("showsOnlyWhenAncestorFocused")]
		bool ShowsOnlyWhenAncestorFocused { get; set; }
	}

	[TV (12,0)]
	[BaseType (typeof (TVLockupView))]
	interface TVMonogramView {

		[DesignatedInitializer] // inlined
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NullAllowed, Export ("personNameComponents", ArgumentSemantic.Strong)]
		NSPersonNameComponents PersonNameComponents { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }
	}

	[TV (12,0)]
	[BaseType (typeof (TVLockupView))]
	interface TVPosterView {

		[DesignatedInitializer] // inlined
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithImage:")]
		IntPtr Constructor ([NullAllowed] UIImage image);

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[Export ("imageView", ArgumentSemantic.Strong)]
		UIImageView ImageView { get; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }
	}
}
