//
// INSpeakableString.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;

using ObjCRuntime;

namespace Intents {
	public partial class INSpeakableString {
		public INSpeakableString (string identifier, string spokenPhrase, string pronunciationHint)
			: base (NSObjectFlag.Empty)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			InitializeHandle (InitWithVocabularyIdentifier (identifier, spokenPhrase, pronunciationHint));
#if !TVOS
			else
				InitializeHandle (InitWithIdentifier (identifier, spokenPhrase, pronunciationHint));
#endif
		}
	}
}
