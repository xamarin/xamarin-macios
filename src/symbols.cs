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
		NSSymbolEffectOptions CreateOptions ();

		[Static]
		[Export ("optionsWithRepeating")]
		NSSymbolEffectOptions CreateOptionsWithRepeating ();

		[Export ("optionsWithRepeating")]
		NSSymbolEffectOptions GetOptionsWithRepeating ();

		[Static]
		[Export ("optionsWithNonRepeating")]
		NSSymbolEffectOptions CreateOptionsWithNonRepeating ();

		[Export ("optionsWithNonRepeating")]
		NSSymbolEffectOptions GetOptionsWithNonRepeating ();

		[Static]
		[Export ("optionsWithRepeatCount:")]
		NSSymbolEffectOptions CreateOptionsWithRepeatCount (nint count);

		[Export ("optionsWithRepeatCount:")]
		NSSymbolEffectOptions GetOptionsWithRepeatCount (nint count);

		[Static]
		[Export ("optionsWithSpeed:")]
		NSSymbolEffectOptions CreateOptionsWithSpeed (double speed);

		[Export ("optionsWithSpeed:")]
		NSSymbolEffectOptions GetOptionsWithSpeed (double speed);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolEffect))]
	interface NSSymbolPulseEffect {
		[Static]
		[Export ("effect")]
		NSSymbolPulseEffect CreateEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolPulseEffect GetEffectWithByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolPulseEffect GetEffectWithWholeSymbol ();
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
		NSSymbolBounceEffect GetEffectWithByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolBounceEffect GetEffectWithWholeSymbol ();
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
		NSSymbolScaleEffect CreateEffect ();

		[Static]
		[Export ("scaleUpEffect")]
		NSSymbolScaleEffect CreateScaleUpEffect ();

		[Static]
		[Export ("scaleDownEffect")]
		NSSymbolScaleEffect CreateScaleDownEffect ();

		[Export ("effectWithByLayer")]
		NSSymbolScaleEffect GetEffectWithByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolScaleEffect GetEffectWithWholeSymbol ();
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
		NSSymbolAppearEffect GetEffectWithByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolAppearEffect GetEffectWithWholeSymbol ();
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
		NSSymbolDisappearEffect GetEffectWithByLayer ();

		[Export ("effectWithWholeSymbol")]
		NSSymbolDisappearEffect GetEffectWithWholeSymbol ();
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
		NSSymbolReplaceContentTransition GetTransitionWithByLayer ();

		[Export ("transitionWithWholeSymbol")]
		NSSymbolReplaceContentTransition GetTransitionWithWholeSymbol ();
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSSymbolContentTransition))]
	interface NSSymbolAutomaticContentTransition {
		[Static]
		[Export ("transition")]
		NSSymbolAutomaticContentTransition CreateTransition ();
	}

}
