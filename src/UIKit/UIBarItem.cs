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
using XamCore.ObjCRuntime;
using XamCore.Foundation;

#if XAMCORE_3_0
using TextAttributes = XamCore.UIKit.UIStringAttributes;
#else
using TextAttributes = XamCore.UIKit.UITextAttributes;
#endif

namespace XamCore.UIKit {
	public partial class UIBarItem {
		[iOS (5,0)]
		public void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
		{
			using (var dict = attributes == null ? null : attributes.Dictionary)
				_SetTitleTextAttributes (dict, state);
		}

		[iOS (5,0)]
		public TextAttributes GetTitleTextAttributes (UIControlState state)
		{
			using (var d = _GetTitleTextAttributes (state)){
				return new TextAttributes (d);
			}
		}

		public partial class UIBarItemAppearance {
			public virtual void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
			{
				if (attributes == null)
					throw new ArgumentNullException ("attributes");
				using (var dict = attributes.Dictionary){
					_SetTitleTextAttributes (dict, state);
				}
			}

			public virtual TextAttributes GetTitleTextAttributes (UIControlState state)
			{
				using (var d = _GetTitleTextAttributes (state)){
					return new TextAttributes (d);
				}
			}
		}
	}
}

#endif // !WATCH
