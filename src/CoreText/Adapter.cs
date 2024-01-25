// 
// Adapter.cs: Helpers for writing Adapters
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc (http://www.xamarin.com)
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

#nullable enable

using System;
using System.Collections.Generic;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	internal static class Adapter {

		public static void AssertWritable (NSDictionary dictionary)
		{
			if (!(dictionary is NSMutableDictionary))
				throw new NotSupportedException ();
		}

		public static int? BitwiseOr (int? a, int? b)
		{
			return a.HasValue
				? a.Value | (b ?? 0)
				: b;
		}

		public static uint? BitwiseOr (uint? a, uint? b)
		{
			return a.HasValue
				? a.Value | (b ?? 0)
				: b;
		}

		public static int? GetInt32Value (IDictionary<NSObject, NSObject> dictionary, NSObject? key)
		{
			if (key is null)
				return null;
			var value = dictionary [key];
			if (value is null)
				return null;
			return ((NSNumber) value).Int32Value;
		}

		public static nuint? GetUnsignedIntegerValue (IDictionary<NSObject, NSObject> dictionary, NSObject? key)
		{
			if (key is null)
				return null;
			var value = dictionary [key];
			if (value is null)
				return null;
			return ((NSNumber) value).NUIntValue;
		}

		public static T []? GetNativeArray<T> (NSDictionary dictionary, NSObject? key, Converter<NativeHandle, T> converter)
		{
			if (key is null)
				return null;
			var cfArrayRef = CFDictionary.GetValue (dictionary.Handle, key.Handle);
			if (cfArrayRef == NativeHandle.Zero || CFArray.GetCount (cfArrayRef) == 0)
				return new T [0];
			return NSArray.ArrayFromHandle (cfArrayRef, converter);
		}

		public static float? GetSingleValue (IDictionary<NSObject, NSObject> dictionary, NSObject? key)
		{
			if (key is null)
				return null;
			var value = dictionary [key];
			if (value is null)
				return null;
			return ((NSNumber) value).FloatValue;
		}

		public static string []? GetStringArray (IDictionary<NSObject, NSObject> dictionary, NSObject? key)
		{
			if (key is null)
				return null;
			var value = dictionary [key];
			if (value is null)
				return Array.Empty<string> ();
			return CFArray.StringArrayFromHandle (value.Handle)!;
		}

		public static string? GetStringValue (IDictionary<NSObject, NSObject> dictionary, NSObject? key)
		{
			if (key is null)
				return null;
			var value = dictionary [key];
			if (value is null)
				return null;
			return ((NSString) value).ToString ();
		}

		public static uint? GetUInt32Value (IDictionary<NSObject, NSObject> dictionary, NSObject? key)
		{
			if (key is null)
				return null;
			var value = dictionary [key];
			if (value is null)
				return null;
			return ((NSNumber) value).UInt32Value;
		}

		public static bool? GetBoolValue (NSDictionary dictionary, NSObject key)
		{
			var value = dictionary [key];
			if (value is null)
				return null;
			return ((NSNumber) value).BoolValue;
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, int? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value.HasValue)
				dictionary [key] = new NSNumber (value.Value);
			else
				dictionary.Remove (key);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, float? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value.HasValue)
				dictionary [key] = new NSNumber (value.Value);
			else
				dictionary.Remove (key);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, uint? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value.HasValue)
				dictionary [key] = new NSNumber (value.Value);
			else
				dictionary.Remove (key);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, bool? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value.HasValue)
				dictionary [key] = new NSNumber (value.Value);
			else
				dictionary.Remove (key);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, nuint? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value.HasValue)
				dictionary [key] = new NSNumber (value.Value);
			else
				dictionary.Remove (key);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, IEnumerable<string>? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			List<string> v;
			if (value is null || (v = new List<string> (value)).Count == 0)
				SetValue (dictionary, key, (NSObject?) null);
			else
				using (var array = NSArray.FromStrings (v.ToArray ()))
					SetValue (dictionary, key, array);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, NSObject? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value is not null)
				dictionary [key] = value;
			else
				dictionary.Remove (key);
		}

		public static void SetValue (IDictionary<NSObject, NSObject> dictionary, NSObject key, string? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value is null)
				SetValue (dictionary, key, (NSObject?) null);
			else
				using (var s = new NSString (value))
					SetValue (dictionary, key, (NSObject) s);
		}

		public static void SetNativeValue<T> (NSDictionary dictionary, NSObject key, IEnumerable<T>? value)
			where T : INativeObject
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			List<NativeHandle> v;
			if (value is null || (v = GetHandles (value)).Count == 0)
				SetNativeValue (dictionary, key, (INativeObject?) null);
			else
				using (var array = CFArray.FromIntPtrs (v.ToArray ()))
					SetNativeValue (dictionary, key, array);
		}

		static List<NativeHandle> GetHandles<T> (IEnumerable<T> value)
			where T : INativeObject
		{
			var v = new List<NativeHandle> ();
			foreach (var e in value)
				v.Add (e.Handle);
			return v;
		}

		public static void SetNativeValue (NSDictionary dictionary, NSObject key, INativeObject? value)
		{
			if (key is null)
				throw new ArgumentOutOfRangeException (nameof (key));
			if (value is not null) {
				AssertWritable (dictionary);
				CFMutableDictionary.SetValue (dictionary.Handle, key.Handle, value.Handle);
			} else {
				IDictionary<NSObject, NSObject> d = dictionary;
				d.Remove (key);
			}
		}
	}
}
