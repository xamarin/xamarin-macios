using CoreFoundation;
using ObjCRuntime;
using Foundation;

using System;

namespace Symbols {
	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface NSSymbolEffect : NSCopying, NSSecureCoding { }

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
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
		NSSymbolEffectOptions CreatehRepeating ();

		[Export ("optionsWithRepeating")]
		NSSymbolEffectOptions GetWithRepeating ();

		[Static]
		[Export ("optionsWithNonRepeating")]
		NSSymbolEffectOptions CreatesWithNonRepeating ();

		[Export ("optionsWithNonRepeating")]
		NSSymbolEffectOptions GetWithNonRepeating ();

		[Static]
		[Export ("optionsWithRepeatCount:")]
		NSSymbolEffectOptions CreateWithRepeatCount (nint count);

		[Export ("optionsWithRepeatCount:")]
		NSSymbolEffectOptions GetWithRepeatCount (nint count);

		[Static]
		[Export ("optionsWithSpeed:")]
		NSSymbolEffectOptions CreateWithSpeed (double speed);

		[Export ("optionsWithSpeed:")]
		NSSymbolEffectOptions GetWithSpeed (double speed);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	interface NSSymbolPulseEffect {
		[Static]
		[Export ("effect")]
		NSSymbolPulseEffect CreateEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolPulseEffect ByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolPulseEffect WholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	interface NSSymbolBounceEffect {
		[Static]
		[Export ("effect")]
		NSSymbolBounceEffect CreateEffect ();

		[Static]
		[Export ("bounceUpEffect")]
		NSSymbolBounceEffect CreateBounceUpEffect ();

		[Static]
		[Export ("bounceDownEffect")]
		NSSymbolBounceEffect CreateBounceDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolBounceEffect ByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolBounceEffect WholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	interface NSSymbolVariableColorEffect {
		[Static]
		[Export ("effect")]
		NSSymbolVariableColorEffect CreateEffect ();

		[Export ("effectWithIterative")]
		NSSymbolVariableColorEffect GetEffectWithIterative ();

		[Export ("effectWithCumulative")]
		NSSymbolVariableColorEffect GetEffectWithCumulative ();

		[Export ("effectWithReversing")]
		NSSymbolVariableColorEffect GetEffectWithReversing ();

		[Export ("effectWithNonReversing")]
		NSSymbolVariableColorEffect GetEffectWithNonReversing ();

		[Export ("effectWithHideInactiveLayers")]
		NSSymbolVariableColorEffect GetEffectWithHideInactiveLayers ();

		[Export ("effectWithDimInactiveLayers")]
		NSSymbolVariableColorEffect GetEffectWithDimInactiveLayers ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
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
		NSSymbolScaleEffect ByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolScaleEffect WholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	interface NSSymbolAppearEffect {
		[Static]
		[Export ("effect")]
		NSSymbolAppearEffect CreateEffect ();

		[Static]
		[Export ("appearUpEffect")]
		NSSymbolAppearEffect CreateAppearUpEffect ();

		[Static]
		[Export ("appearDownEffect")]
		NSSymbolAppearEffect CreateAppearDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolAppearEffect ByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolAppearEffect WholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	interface NSSymbolDisappearEffect {
		[Static]
		[Export ("effect")]
		NSSymbolDisappearEffect CreateEffect ();

		[Static]
		[Export ("disappearUpEffect")]
		NSSymbolDisappearEffect CreateDisappearUpEffect ();

		[Static]
		[Export ("disappearDownEffect")]
		NSSymbolDisappearEffect CreateDisappearDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolDisappearEffect ByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolDisappearEffect WholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolContentTransition))]
	interface NSSymbolReplaceContentTransition {
		[Static]
		[Export ("transition")]
		NSSymbolReplaceContentTransition CreateTransition ();

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
		NSSymbolReplaceContentTransition ByLayer ();

		[Export ("transitionWithWholeSymbol")]
		NSSymbolReplaceContentTransition WholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolContentTransition))]
	interface NSSymbolAutomaticContentTransition {
		[Static]
		[Export ("transition")]
		NSSymbolAutomaticContentTransition CreateTransition ();
	}

}
