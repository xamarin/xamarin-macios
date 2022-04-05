//
// CPNavigationAlert.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation. All rights reserved.
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace CarPlay {
	public partial class CPNavigationAlert {
		// Defined inside CPNavigationAlert.h
		// static NSTimeInterval const CPNavigationAlertMinimumDuration = 5;
		public const double MinimumDuration = 5;
	}
}
