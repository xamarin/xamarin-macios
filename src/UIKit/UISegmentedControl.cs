// 
// UISegmentedControl.cs: Implements the managed UISegmentedControl
//
// Authors:
//   Miguel de Icaza
//     
// Copyright 2009 Novell, Inc
// Copyright 2011 Xamarin, Inc
//

#if !WATCH

using System;
using System.Collections;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;

#if XAMCORE_3_0
using TextAttributes = XamCore.UIKit.UIStringAttributes;
#else
using TextAttributes = XamCore.UIKit.UITextAttributes;
#endif

namespace XamCore.UIKit {
	public partial class UISegmentedControl {
		public UISegmentedControl (object [] args) : base (NSObjectFlag.Empty)
		{
			if (args == null)
				throw new ArgumentNullException ("args");

			NSObject [] nsargs = new NSObject [args.Length];
			
			for (int i = 0; i < args.Length; i++){
				object a = args [i];

				if (a == null)
					throw new ArgumentNullException (String.Format ("Element {0} in args is null", i));
				
				if (a is string)
					nsargs [i] = new NSString ((string) a);
				else if (a is UIImage)
					nsargs [i] = (UIImage) a;
				else
					throw new ArgumentException (String.Format ("non-string or UIImage at position {0} with type {1}", i, a.GetType ()));
			}
			using (NSArray nsa = NSArray.FromNSObjects (nsargs)){
				Handle = InitWithItems (nsa.Handle);
			}
		}

		public void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
		{
			if (attributes == null)
				throw new ArgumentNullException ("attributes");
                        
			using (var dict = attributes.Dictionary) {
				_SetTitleTextAttributes (dict, state);
			}
		}

		public TextAttributes GetTitleTextAttributes (UIControlState state)
		{
			using (var d = _GetTitleTextAttributes (state)) {
				return new TextAttributes (d);
			}
		}

		public partial class UISegmentedControlAppearance {
			public void SetTitleTextAttributes (TextAttributes attributes, UIControlState state)
			{
				if (attributes == null)
					throw new ArgumentNullException ("attributes");
	                        
				using (var dict = attributes.Dictionary) {
					_SetTitleTextAttributes (dict, state);
				}
			}

			public TextAttributes GetTitleTextAttributes (UIControlState state)
			{
				using (var d = _GetTitleTextAttributes (state)) {
					return new TextAttributes (d);
				}
			}
		}
	}
}

#endif // !WATCH
