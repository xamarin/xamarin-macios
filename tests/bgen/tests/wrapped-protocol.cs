// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Foundation;
using ObjCRuntime;

namespace WrappedProtocol {
	interface ILocalDelegate { }

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface LocalDelegate {
		[Export ("didInvoke")]
		void DidInvoke ();
	}

	[BaseType (typeof (NSObject))]
	interface DelegateOwner {
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Wrap ("WeakDelegate")]
		INSUserActivityDelegate Delegate { get; set; }

		[Wrap ("WeakDelegate")]
		ILocalDelegate LocalDelegate { get; set; }

		[NullAllowed, Wrap ("WeakDelegate")]
		INSUserActivityDelegate ReadOnlyDelegate { get; }

		[NullAllowed, Wrap ("WeakDelegate")]
		NSObject ObjectDelegate { get; set; }

		[NullAllowed, Wrap ("WeakDelegate")]
		LocalDelegate ModelDelegate { get; set; }

		[NullAllowed, Wrap ("WeakDelegate")]
		NSUserActivityDelegate ImportedModelDelegate { get; set; }

		[Static, NullAllowed, Export ("sharedDelegate", ArgumentSemantic.Weak)]
		NSObject WeakSharedDelegate { get; set; }

		[Static, NullAllowed, Wrap ("WeakSharedDelegate")]
		INSUserActivityDelegate SharedDelegate { get; set; }
	}
}
