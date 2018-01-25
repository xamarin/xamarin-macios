//
// Copyright 2011, Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {

	public partial class NSUbiquitousKeyValueStore {
		public NSObject this [NSString key] {
			get {
				return ObjectForKey (key);
			}
			set {
				SetObjectForKey (value, key);
			}
		}

		public NSObject this [string key] {
			get {
				return ObjectForKey (key);
			}
			set {
				SetObjectForKey (value, key);
			}
		}

		public void SetString (string key, string value)
		{
			_SetString (value, key);
		}

		public void SetData (string key, NSData value)
		{
			_SetData (value, key);
		}

		public void SetArray (string key, NSObject [] value)
		{
			_SetArray (value, key);
		}

		public void SetDictionary (string key, NSDictionary value)
		{
			_SetDictionary (value, key);
		}

		public void SetLong (string key, long value)
		{
			_SetLong (value, key);
		}

		public void SetDouble (string key, double value)
		{
			_SetDouble (value, key);
		}

		public void SetBool (string key, bool value)
		{
			_SetBool (value, key);
		}

#if !XAMCORE_2_0
		public NSDictionary ToDictionary ()
		{
			return DictionaryRepresentation ();
		}
#endif
	}
}
