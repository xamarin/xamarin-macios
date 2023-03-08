//
// WebKit/WKWindowFeatures.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace WebKit {
	public partial class WKWindowFeatures {
		public bool? MenuBarVisibility {
			get => menuBarVisibility?.BoolValue;
		}

		public bool? StatusBarVisibility {
			get => statusBarVisibility?.BoolValue;
		}

		public bool? ToolbarsVisibility {
			get => toolbarsVisibility?.BoolValue;
		}

		public bool? AllowsResizing {
			get => allowsResizing?.BoolValue;
		}

		static nfloat? NFloatValue (NSNumber? number)
		{
			if (number is null)
				return null;
			else if (IntPtr.Size == 4)
				return (nfloat) number.FloatValue;
			else
				return (nfloat) number.DoubleValue;
		}

		public nfloat? X {
			get { return NFloatValue (x); }
		}

		public nfloat? Y {
			get { return NFloatValue (y); }
		}

		public nfloat? Width {
			get { return NFloatValue (width); }
		}

		public nfloat? Height {
			get { return NFloatValue (height); }
		}
	}
}
