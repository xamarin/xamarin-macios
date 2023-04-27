//
// UIBarItem extension
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, 2015 Xamarin Inc.
//

#if !WATCH

using System;
using ObjCRuntime;
using Foundation;

#if XAMCORE_3_0
using TextAttributes = UIKit.UIStringAttributes;
#else
using TextAttributes = UIKit.UITextAttributes;
#endif

namespace UIKit {
	public partial class UIBarItem {

		public void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
		{
			using (var dict = attributes is null ? null : attributes.Dictionary)
				_SetTitleTextAttributes (dict, state);
		}

		public TextAttributes GetTitleTextAttributes (UIControlState state)
		{
			using (var d = _GetTitleTextAttributes (state)) {
				return new TextAttributes (d);
			}
		}

		public partial class UIBarItemAppearance {
			public virtual void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
			{
				if (attributes is null)
					throw new ArgumentNullException ("attributes");
				using (var dict = attributes.Dictionary) {
					_SetTitleTextAttributes (dict, state);
				}
			}

			public virtual TextAttributes GetTitleTextAttributes (UIControlState state)
			{
				using (var d = _GetTitleTextAttributes (state)) {
					return new TextAttributes (d);
				}
			}
		}
	}
}

#endif // !WATCH
