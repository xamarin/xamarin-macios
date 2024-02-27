//
// Extensions.cs: C#isms for JavaScriptCore
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.

#nullable enable

using System;
using Foundation;

namespace JavaScriptCore {

	public partial class JSContext {

		public JSValue this [NSObject key] {
			get { return _GetObject (key); }
			set { _SetObject (value, key); }
		}
	}

	public partial class JSValue {

		public override string ToString ()
		{
			return _ToString ();
		}

		static public JSValue From (string value, JSContext context)
		{
			using (var str = new NSString (value)) {
				return From ((NSObject) str, context);
			}
		}

		public JSValue this [nuint index] {
			get { return _ObjectAtIndexedSubscript (index); }
			set { _SetObject (value, index); }
		}

		public JSValue this [NSObject key] {
			get { return _ObjectForKeyedSubscript (key); }
			set { _SetObject (value, key); }
		}
	}
}
