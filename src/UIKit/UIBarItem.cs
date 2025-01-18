//
// UIBarItem extension
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, 2015 Xamarin Inc.
//

using System;
using ObjCRuntime;
using Foundation;

using TextAttributes = UIKit.UIStringAttributes;

namespace UIKit {
	public partial class UIBarItem {

		public void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
		{
			var dict = attributes?.Dictionary;
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
				var dict = attributes.Dictionary;
				_SetTitleTextAttributes (dict, state);
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
