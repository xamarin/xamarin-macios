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
#if __MACCATALYST__ && !NET
		// This method exists to improve compatibility with assemblies that reference Xamarin.iOS.dll in Mac Catalyst,
		// such as Xamarin.Forms. Setting the IsError field to true in the attribute means that everything will work at runtime,
		// but that nobody can use this method at compile time.
		// This is disabled in .NET because we won't try to be compatible with Xamarin.iOS.dll in .NET.
		[Obsolete ("Use the overload that takes a 'UIStringAttributes' instead.", true)]
		public void SetTitleTextAttributes (UIKit.UITextAttributes attributes, UIControlState state)
		{
			using (var dict = attributes == null ? null : attributes.Dictionary)
				_SetTitleTextAttributes (dict, state);
		}
#endif

		public void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
		{
			using (var dict = attributes == null ? null : attributes.Dictionary)
				_SetTitleTextAttributes (dict, state);
		}

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
