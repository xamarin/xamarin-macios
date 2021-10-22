using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace Foundation {

	public partial class NSOrderedCollectionChange
	{

		public static NSOrderedCollectionChange ChangeWithObject (NSObject? anObject, NSCollectionChangeType type, nuint index)
			=> new NSOrderedCollectionChange (NSOrderedCollectionChange._ChangeWithObject (anObject!.Handle, type, index));

		public static NSOrderedCollectionChange ChangeWithObject (NSObject? anObject, NSCollectionChangeType type, nuint index, nuint associatedIndex)
			=> new NSOrderedCollectionChange (NSOrderedCollectionChange._ChangeWithObject (anObject!.Handle, type, index, associatedIndex));

		NSObject? Object => Runtime.GetNSObject<NSObject> (_Object);

	}
}
