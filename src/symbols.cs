using CoreFoundation;
using ObjCRuntime;
using Foundation;

using System;

namespace Symbols {
	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSSymbolEffect : NSCopying, NSSecureCoding { }

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSSymbolContentTransition : NSCopying, NSSecureCoding { }

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSSymbolEffectOptions : NSCopying, NSSecureCoding {
		[Static]
		[Export ("options")]
		NSSymbolEffectOptions Create ();

		[Static]
		[Export ("optionsWithRepeating")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.WatchOS, 11, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		NSSymbolEffectOptions CreateRepeating ();

		[Export ("optionsWithRepeating")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.WatchOS, 11, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		NSSymbolEffectOptions GetRepeating ();

		[Static]
		[Export ("optionsWithNonRepeating")]
		NSSymbolEffectOptions CreateNonRepeating ();

		[Export ("optionsWithNonRepeating")]
		NSSymbolEffectOptions GetNonRepeating ();

		[Static]
		[Export ("optionsWithRepeatCount:")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.WatchOS, 11, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		NSSymbolEffectOptions Create (nint repeatCount);

		[Export ("optionsWithRepeatCount:")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.WatchOS, 11, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic' instead.")]
		NSSymbolEffectOptions Get (nint repeatCount);

		[Static]
		[Export ("optionsWithSpeed:")]
		NSSymbolEffectOptions Create (double speed);

		[Export ("optionsWithSpeed:")]
		NSSymbolEffectOptions Get (double speed);

		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("optionsWithRepeatBehavior:")]
		NSSymbolEffectOptions Create (NSSymbolEffectOptionsRepeatBehavior behavior);

		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("optionsWithRepeatBehavior:")]
		NSSymbolEffectOptions Get (NSSymbolEffectOptionsRepeatBehavior behavior);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolPulseEffect {
		[Static]
		[Export ("effect")]
		NSSymbolPulseEffect Create ();

		[Export ("effectWithByLayer")]
		NSSymbolPulseEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolPulseEffect WholeSymbol { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolBounceEffect {
		[Static]
		[Export ("effect")]
		NSSymbolBounceEffect Create ();

		[Static]
		[Export ("bounceUpEffect")]
		NSSymbolBounceEffect CreateBounceUpEffect ();

		[Static]
		[Export ("bounceDownEffect")]
		NSSymbolBounceEffect CreateBounceDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolBounceEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolBounceEffect WholeSymbol { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolVariableColorEffect {
		[Static]
		[Export ("effect")]
		NSSymbolVariableColorEffect Create ();

		[Export ("effectWithIterative")]
		NSSymbolVariableColorEffect Iterative { get; }

		[Export ("effectWithCumulative")]
		NSSymbolVariableColorEffect Cumulative { get; }

		[Export ("effectWithReversing")]
		NSSymbolVariableColorEffect Reversing { get; }

		[Export ("effectWithNonReversing")]
		NSSymbolVariableColorEffect NonReversing { get; }

		[Export ("effectWithHideInactiveLayers")]
		NSSymbolVariableColorEffect HideInactiveLayers { get; }

		[Export ("effectWithDimInactiveLayers")]
		NSSymbolVariableColorEffect DimInactiveLayers { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolScaleEffect {
		[Static]
		[Export ("effect")]
		NSSymbolScaleEffect Create ();

		[Static]
		[Export ("scaleUpEffect")]
		NSSymbolScaleEffect CreateScaleUpEffect ();

		[Static]
		[Export ("scaleDownEffect")]
		NSSymbolScaleEffect CreateScaleDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolScaleEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolScaleEffect WholeSymbol { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolAppearEffect {
		[Static]
		[Export ("effect")]
		NSSymbolAppearEffect Create ();

		[Static]
		[Export ("appearUpEffect")]
		NSSymbolAppearEffect CreateAppearUpEffect ();

		[Static]
		[Export ("appearDownEffect")]
		NSSymbolAppearEffect CreateAppearDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolAppearEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolAppearEffect WholeSymbol { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolDisappearEffect {
		[Static]
		[Export ("effect")]
		NSSymbolDisappearEffect Create ();

		[Static]
		[Export ("disappearUpEffect")]
		NSSymbolDisappearEffect CreateDisappearUpEffect ();

		[Static]
		[Export ("disappearDownEffect")]
		NSSymbolDisappearEffect CreateDisappearDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolDisappearEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolDisappearEffect WholeSymbol { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolContentTransition))]
	[DisableDefaultCtor]
	interface NSSymbolReplaceContentTransition {
		[Static]
		[Export ("transition")]
		NSSymbolReplaceContentTransition Create ();

		[Static]
		[Export ("replaceDownUpTransition")]
		NSSymbolReplaceContentTransition CreateReplaceDownUpTransition ();

		[Static]
		[Export ("replaceUpUpTransition")]
		NSSymbolReplaceContentTransition CreateReplaceUpUpTransition ();

		[Static]
		[Export ("replaceOffUpTransition")]
		NSSymbolReplaceContentTransition CreateReplaceOffUpTransition ();

		[Export ("transitionWithByLayer")]
		NSSymbolReplaceContentTransition ByLayer { get; }

		[Export ("transitionWithWholeSymbol")]
		NSSymbolReplaceContentTransition WholeSymbol { get; }

		[Static]
		[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("magicTransitionWithFallback:")]
		NSSymbolMagicReplaceContentTransition CreateMagicTransition (NSSymbolReplaceContentTransition fallback);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolContentTransition))]
	[DisableDefaultCtor]
	interface NSSymbolAutomaticContentTransition {
		[Static]
		[Export ("transition")]
		NSSymbolAutomaticContentTransition Create ();
	}

	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSSymbolEffectOptionsRepeatBehavior : NSCopying, NSSecureCoding {
		[Static]
		[Export ("behaviorPeriodic")]
		NSSymbolEffectOptionsRepeatBehavior CreatePeriodic ();

		[Static]
		[Export ("behaviorPeriodicWithCount:")]
		NSSymbolEffectOptionsRepeatBehavior CreatePeriodic (nint count);

		[Static]
		[Export ("behaviorPeriodicWithDelay:")]
		NSSymbolEffectOptionsRepeatBehavior CreatePeriodic (double delay);

		[Static]
		[Export ("behaviorPeriodicWithCount:delay:")]
		NSSymbolEffectOptionsRepeatBehavior CreatePeriodic (nint count, double delay);

		[Static]
		[Export ("behaviorContinuous")]
		NSSymbolEffectOptionsRepeatBehavior CreateContinuous ();
	}

	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolWiggleEffect {
		[Static]
		[Export ("effect")]
		NSSymbolWiggleEffect Create ();

		[Static]
		[Export ("wiggleClockwiseEffect")]
		NSSymbolWiggleEffect CreateClockwiseEffect ();

		[Static]
		[Export ("wiggleCounterClockwiseEffect")]
		NSSymbolWiggleEffect CreateCounterClockwiseEffect ();

		[Static]
		[Export ("wiggleLeftEffect")]
		NSSymbolWiggleEffect CreateLeftEffect ();

		[Static]
		[Export ("wiggleRightEffect")]
		NSSymbolWiggleEffect CreateRightEffect ();

		[Static]
		[Export ("wiggleUpEffect")]
		NSSymbolWiggleEffect CreateUpEffect ();

		[Static]
		[Export ("wiggleDownEffect")]
		NSSymbolWiggleEffect CreateDownEffect ();

		[Static]
		[Export ("wiggleForwardEffect")]
		NSSymbolWiggleEffect CreateForwardEffect ();

		[Static]
		[Export ("wiggleBackwardEffect")]
		NSSymbolWiggleEffect CreateBackwardEffect ();

		[Static]
		[Export ("wiggleCustomAngleEffect:")]
		NSSymbolWiggleEffect CreateCustomAngleEffect (double angle);

		[Export ("effectWithByLayer")]
		NSSymbolWiggleEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolWiggleEffect WholeSymbol { get; }
	}

	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolRotateEffect {
		[Static]
		[Export ("effect")]
		NSSymbolRotateEffect Create ();

		[Static]
		[Export ("rotateClockwiseEffect")]
		NSSymbolRotateEffect CreateClockwiseEffect ();

		[Static]
		[Export ("rotateCounterClockwiseEffect")]
		NSSymbolRotateEffect CreateCounterClockwiseEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolRotateEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolRotateEffect WholeSymbol { get; }
	}

	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	[DisableDefaultCtor]
	interface NSSymbolBreatheEffect {
		[Static]
		[Export ("effect")]
		NSSymbolBreatheEffect Create ();

		[Static]
		[Export ("breathePulseEffect")]
		NSSymbolBreatheEffect CreatePulseEffect ();

		[Static]
		[Export ("breathePlainEffect")]
		NSSymbolBreatheEffect CreatePlainEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolBreatheEffect ByLayer { get; }

		[Export ("effectWithWholeSymbol")]
		NSSymbolBreatheEffect WholeSymbol { get; }
	}

	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSSymbolContentTransition))]
	[DisableDefaultCtor]
	interface NSSymbolMagicReplaceContentTransition {
	}
}
