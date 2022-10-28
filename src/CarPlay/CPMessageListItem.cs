//
// CPMessageListItem.cs
//
// Authors:
//	Whitney Schmidt <whschm@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
// Licensed under MIT License.
//

using System;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace CarPlay {

	public partial class CPMessageListItem {

		public CPMessageListItem (string identifier, string text, CPMessageListItemLeadingConfiguration leadingConfiguration,
			CPMessageListItemTrailingConfiguration? trailingConfiguration, string? detailText, string? trailingText) :
				this (identifier, text, leadingConfiguration, trailingConfiguration, detailText, trailingText, CPMessageListItemType.Identifier)
		{
		}

		public CPMessageListItem (string identifier, string text, CPMessageListItemLeadingConfiguration leadingConfiguration, CPMessageListItemTrailingConfiguration? trailingConfiguration,
			string? detailText, string? trailingText, CPMessageListItemType type) : base (NSObjectFlag.Empty)
		{
			switch (type) {
			case CPMessageListItemType.Identifier:
				InitializeHandle (InitWithConversationIdentifier (identifier, text, leadingConfiguration, trailingConfiguration, detailText, trailingText),
					"initWithConversationIdentifier:text:leadingConfiguration:trailingConfiguration:detailText:trailingText:");
				break;
			case CPMessageListItemType.FullName:
				InitializeHandle (InitWithFullName (identifier, text, leadingConfiguration, trailingConfiguration, detailText, trailingText),
					"initWithFullName:phoneOrEmailAddress:leadingConfiguration:trailingConfiguration:detailText:trailingText:");
				break;
			default:
				throw new ArgumentException (nameof (type));
			}
		}
	}
}
