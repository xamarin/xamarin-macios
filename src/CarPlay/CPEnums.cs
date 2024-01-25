//
// CPEnums.cs
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

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	public enum CPMessageListItemType {
		Identifier,
		FullName,
	}
}
