using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace Intents {

#if !TVOS
	public partial class INPerson {
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		public enum INPersonType {
			Me = 0,
			ContactSuggestion = 1,
		}

		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		public INPerson (INPersonHandle personHandle, NSPersonNameComponents? nameComponents, string? displayName, INImage? image, string? contactIdentifier, string? customIdentifier, bool isMe, INPersonSuggestionType suggestionType) :
			this (personHandle, nameComponents, displayName, image, contactIdentifier, customIdentifier, isMe, suggestionType, INPersonType.Me)
		{
		}

		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		public INPerson (INPersonHandle personHandle, NSPersonNameComponents? nameComponents, string? displayName, INImage? image, string? contactIdentifier, string? customIdentifier, bool isMe, INPersonSuggestionType suggestionType, INPersonType personType) : base (NSObjectFlag.Empty)
		{
			switch (personType) {
			case INPersonType.Me:
				InitializeHandle (InitWithMe (personHandle, nameComponents, displayName, image, contactIdentifier, customIdentifier, isMe, suggestionType),
					"initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isMe:suggestionType:");
				break;
			case INPersonType.ContactSuggestion:
				InitializeHandle (InitWithContactSuggestion (personHandle, nameComponents, displayName, image, contactIdentifier, customIdentifier, isMe, suggestionType),
					"initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isContactSuggestion:suggestionType:");
				break;
			default:
				throw new ArgumentException (nameof (personType));
			}
		}
	}
#endif // !TVOS
}
