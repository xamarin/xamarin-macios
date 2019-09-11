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

	[TV (13,0)]
	[BaseType (typeof (UICollectionViewCell))]
	interface TVCollectionViewFullScreenCell {

		// inlined from base type
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; }

		[Export ("contentBleed")]
		UIEdgeInsets ContentBleed { get; }

		[Export ("parallaxOffset")]
		nfloat ParallaxOffset { get; }

		[Export ("normalizedPosition")]
		nfloat NormalizedPosition { get; }

		[Export ("maskAmount")]
		nfloat MaskAmount { get; }

		[Export ("maskedBackgroundView")]
		UIView MaskedBackgroundView { get; }

		[Export ("maskedContentView")]
		UIView MaskedContentView { get; }

		[Export ("maskAmountWillChange:")]
		void MaskAmountWillChange (nfloat nextMaskAmount);

		[Export ("maskAmountDidChange")]
		void MaskAmountDidChange ();

		[Export ("normalizedPositionWillChange:")]
		void NormalizedPositionWillChange (nfloat nextNormalizedPosition);

		[Export ("normalizedPositionDidChange")]
		void NormalizedPositionDidChange ();
	}

	[TV (13,0)]
	[BaseType (typeof (UICollectionViewLayoutAttributes))]
	[DisableDefaultCtor]
	interface TVCollectionViewFullScreenLayoutAttributes {
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("contentBleed", ArgumentSemantic.Assign)]
		UIEdgeInsets ContentBleed { get; set; }

		[Export ("normalizedPosition")]
		nfloat NormalizedPosition { get; set; }

		[Export ("maskAmount")]
		nfloat MaskAmount { get; set; }

		[Export ("parallaxOffset")]
		nfloat ParallaxOffset { get; set; }

		// inlined from base type

		[Static][New]
		[Export ("layoutAttributesForCellWithIndexPath:")]
		TVCollectionViewFullScreenLayoutAttributes CreateForCell (NSIndexPath indexPath);

		[Static][New]
		[Export ("layoutAttributesForDecorationViewOfKind:withIndexPath:")]
		TVCollectionViewFullScreenLayoutAttributes CreateForDecorationView (NSString kind, NSIndexPath indexPath);

		[Static][New]
		[Export ("layoutAttributesForSupplementaryViewOfKind:withIndexPath:")]
		TVCollectionViewFullScreenLayoutAttributes CreateForSupplementaryView (NSString kind, NSIndexPath indexPath);
	}

	[TV (13,0)]
	[Protocol][Model (AutoGeneratedName = true)]
	[BaseType (typeof (UICollectionViewDelegate))]
	interface TVCollectionViewDelegateFullScreenLayout {
		[Export ("collectionView:layout:willCenterCellAtIndexPath:")]
		void WillCenterCell (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout, NSIndexPath indexPath);

		[Export ("collectionView:layout:didCenterCellAtIndexPath:")]
		void DidCenterCell (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout, NSIndexPath indexPath);
	}

	[TV (13,0)]
	[BaseType (typeof (UICollectionViewLayout))]
	[DesignatedDefaultCtor] // same as base type
	interface TVCollectionViewFullScreenLayout {
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("interitemSpacing")]
		nfloat InteritemSpacing { get; set; }

		[Export ("parallaxFactor")]
		nfloat ParallaxFactor { get; set; }

		[Export ("maskAmount")]
		nfloat MaskAmount { get; set; }

		[Export ("maskInset", ArgumentSemantic.Assign)]
		UIEdgeInsets MaskInset { get; set; }

		[NullAllowed, Export ("centerIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath CenterIndexPath { get; }

		[Export ("transitioningToCenterIndexPath")]
		bool TransitioningToCenterIndexPath { [Bind ("isTransitioningToCenterIndexPath")] get; }
	}
}
