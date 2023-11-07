//
// UIActionSheet.cs: Extensions to UIActionSheet
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2015 Xamarin Inc.
//

#if IOS

using System;
using System.Collections;
using ObjCRuntime;
using Foundation;

namespace UIKit {
	public partial class UIActionSheet : IEnumerable {

#if !NET
		[Obsolete ("Use overload with a IUIActionSheetDelegate parameter")]
		public UIActionSheet (string title, UIActionSheetDelegate del, string cancelTitle, string destroy, params string [] other)
			: this (title, del as IUIActionSheetDelegate, cancelTitle, destroy, other)
		{
		}
#endif

		public UIActionSheet (string title, IUIActionSheetDelegate del, string cancelTitle, string destroy, params string [] other)
			: this (title, del, null, null, (string) null)
		{
			if (destroy is not null)
				DestructiveButtonIndex = AddButton (destroy);

			if (other is null) {
				if (cancelTitle is not null)
					CancelButtonIndex = AddButton (cancelTitle);
				return;
			}

			foreach (string b in other){
				if (b is not null)
					AddButton (b);
			}

			if (cancelTitle is not null)
				CancelButtonIndex = AddButton (cancelTitle);
		}
		
#if !NET
		[Obsolete ("Use overload with a IUIActionSheetDelegate parameter")]
		public UIActionSheet (string title, UIActionSheetDelegate del)
			: this (title, del as IUIActionSheetDelegate, null, null, (string) null)
		{
		}
#endif

		public UIActionSheet (string title, IUIActionSheetDelegate del)
		: this (title, del, null, null, (string) null) {}

		public UIActionSheet (string title)
		: this (title, null, null, null, (string) null) {}

		public void Add (string name)
		{
			AddButton (name);
		}

		public IEnumerator GetEnumerator ()
		{
			for (int i = 0; i < ButtonCount; i++)
				yield return ButtonTitle (i);
		}
	}
	
}

#endif // IOS
