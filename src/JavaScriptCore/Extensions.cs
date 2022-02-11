//
// Extensions.cs: C#isms for JavaScriptCore
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.

using System;
using Foundation;
using System.Runtime.Versioning;

namespace JavaScriptCore {

#if NET
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class JSContext {
		
		public JSValue this[NSObject key] {
			get { return _GetObject (key); }
			set { _SetObject (value, key); }
		}
	}

#if NET
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class JSValue {

		public override string ToString ()
		{
			return _ToString ();
		}

		static public JSValue From (string value, JSContext context)
		{
			using (var str = new NSString (value)) {
				return From ((NSObject)str, context);
			}
		}

		public JSValue this[nuint index] {
			get { return _ObjectAtIndexedSubscript (index); }
			set { _SetObject (value, index); }
		}

		public JSValue this[NSObject key] {
			get { return _ObjectForKeyedSubscript (key); }
			set { _SetObject (value, key); }
		}
	}
}
