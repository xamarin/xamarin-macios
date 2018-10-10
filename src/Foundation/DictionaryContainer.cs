// 
// DictionaryContainer.cs: Foundation implementation for NSDictionary based setting classes
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, 2014 Xamarin Inc
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
//

using System;
using System.Linq;
using System.Runtime.InteropServices;

#if !COREBUILD
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#if !WATCH
using CoreMedia;
#endif // !WATCH
#endif

namespace Foundation {

	public abstract class DictionaryContainer
	{
#if !COREBUILD
		protected DictionaryContainer ()
		{
			Dictionary = new NSMutableDictionary ();
		}

		protected DictionaryContainer (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}
		
		public NSDictionary Dictionary { get; private set; }

		protected T[] GetArray<T> (NSString key) where T : NSObject
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
			if (value == IntPtr.Zero)
				return null;

			return NSArray.ArrayFromHandle<T> (value);
		}

		protected T[] GetArray<T> (NSString key, Func<IntPtr, T> creator)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
			if (value == IntPtr.Zero)
				return null;

			return NSArray.ArrayFromHandleFunc<T> (value, creator);
		}

		protected int? GetInt32Value (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).Int32Value;
		}

		protected uint? GetUInt32Value (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).UInt32Value;
		}

		protected nint? GetNIntValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

#if XAMCORE_2_0
			return ((NSNumber) value).NIntValue;
#else
			return ((NSNumber) value).IntValue;
#endif
		}

		protected nuint? GetNUIntValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

#if XAMCORE_2_0
			return ((NSNumber) value).NUIntValue;
#else
			return ((NSNumber) value).UnsignedIntegerValue;
#endif
		}

		protected long? GetLongValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).Int64Value;
		}

		protected uint? GetUIntValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).UInt32Value;
		}

		protected float? GetFloatValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).FloatValue;
		}

		protected double? GetDoubleValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).DoubleValue;
		}

		protected bool? GetBoolValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
			if (value == IntPtr.Zero)
				return null;

			return CFBoolean.GetValue (value);
		}

		protected T GetNativeValue<T> (NSString key) where T : class, INativeObject
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			return Runtime.GetINativeObject<T> (Dictionary.LowlevelObjectForKey (key.Handle), false);
		}

		protected NSDictionary GetNSDictionary (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			Dictionary.TryGetValue (key, out value);
			return value as NSDictionary;
		}
		
#if XAMCORE_2_0
		protected NSDictionary <TKey, TValue> GetNSDictionary <TKey, TValue> (NSString key)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			Dictionary.TryGetValue (key, out value);
			return value as NSDictionary <TKey, TValue>;
		}
#endif

		protected T GetStrongDictionary<T> (NSString key) where T : DictionaryContainer
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			var dict = GetNSDictionary (key);
			if (dict == null)
				return null;
			T value = (T)Activator.CreateInstance (typeof(T),
				new object[] { dict }
			);

			return value;
		}

		protected NSString GetNSStringValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			Dictionary.TryGetValue (key, out value);
			return value as NSString;
		}

		protected string GetStringValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;
			
			return CFString.FetchString (value.Handle);
		}

		protected string GetStringValue (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			using (var str = new CFString (key)) {
				return CFString.FetchString (CFDictionary.GetValue (Dictionary.Handle, str.handle));
			}
		}
#if XAMCORE_2_0
		protected CGRect? GetCGRectValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			CGRect value;
			if (!CGRect.TryParse (dictValue, out value))
				return null;

			return value;
		}

		protected CGSize? GetCGSizeValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			CGSize value;
			if (!CGSize.TryParse (dictValue, out value))
				return null;

			return value;
		}

		protected CGPoint? GetCGPointValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			CGPoint value;
			if (!CGPoint.TryParse (dictValue, out value))
				return null;

			return value;
		}
#endif // XAMCORE_2_0
#if !WATCH
		protected CMTime? GetCMTimeValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			var value = CMTime.FromDictionary (dictValue);
			if (value.IsInvalid)
				return null;

			return value;
		}
#endif // !WATCH
		bool NullCheckAndRemoveKey (NSString key, bool removeEntry)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			if (removeEntry)
				RemoveValue (key);

			return !removeEntry;
		}

		protected void SetArrayValue (NSString key, NSNumber[] values)
		{
			if (NullCheckAndRemoveKey (key, values == null))
				Dictionary [key] = NSArray.FromNSObjects (values);
		}

		protected void SetArrayValue<T> (NSString key, T[] values)
		{
			if (NullCheckAndRemoveKey (key, values == null))
				Dictionary [key] = NSArray.FromNSObjects (values.Select (x => NSObject.FromObject (x)).ToArray ());
		}

		protected void SetArrayValue (NSString key, string[] values)
		{
			if (NullCheckAndRemoveKey (key, values == null))
				Dictionary [key] = NSArray.FromStrings (values);
		}

		protected void SetArrayValue (NSString key, INativeObject[] values)
		{
			if (NullCheckAndRemoveKey (key, values == null))
				CFMutableDictionary.SetValue (Dictionary.Handle, key.Handle, CFArray.FromNativeObjects (values).Handle);
		}

		#region Sets CFBoolean value

		protected void SetBooleanValue (NSString key, bool? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				CFMutableDictionary.SetValue (Dictionary.Handle, key.Handle, value.Value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle);			
		}

		#endregion

		#region Sets NSNumber value

		protected void SetNumberValue (NSString key, int? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);				
		}

		protected void SetNumberValue (NSString key, uint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);				
		}

#if XAMCORE_2_0
		protected void SetNumberValue (NSString key, nint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);	
		}

		protected void SetNumberValue (NSString key, nuint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);	
		}
#endif

		protected void SetNumberValue (NSString key, long? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);				
		}

		protected void SetNumberValue (NSString key, float? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);				
		}

		protected void SetNumberValue (NSString key, double? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value.Value);				
		}

		#endregion

		#region Sets NSString value

		protected void SetStringValue (NSString key, string value)
		{
			SetStringValue (key, value == null ? (NSString) null : new NSString (value));
		}

		protected void SetStringValue (NSString key, NSString value)
		{
			if (NullCheckAndRemoveKey (key, value == null))
				Dictionary [key] = value;
		}

		#endregion

		#region Sets Native value

		protected void SetNativeValue (NSString key, INativeObject value, bool removeNullValue = true)
		{
			if (NullCheckAndRemoveKey (key, removeNullValue && value == null))
				CFMutableDictionary.SetValue (Dictionary.Handle, key.Handle, value == null ? IntPtr.Zero : value.Handle);
		}

		#endregion

		protected void RemoveValue (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			((NSMutableDictionary) Dictionary).Remove (key);
		}

		#region Sets structs values

#if XAMCORE_2_0
		protected void SetCGRectValue (NSString key, CGRect? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value.Value.ToDictionary ();
		}

		protected void SetCGSizeValue (NSString key, CGSize? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value.Value.ToDictionary ();
		}

		protected void SetCGPointValue (NSString key, CGPoint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value.Value.ToDictionary ();
		}
#endif // XAMCORE_2_0
#if !WATCH
		protected void SetCMTimeValue (NSString key, CMTime? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value.Value.ToDictionary ();
		}
#endif //!WATCH
		#endregion
#endif
	}

#if !COREBUILD
	static class DictionaryContainerHelper {

		// helper to avoid the (common pattern)
		// 	var p = x == null ? IntPtr.Zero : h.Dictionary.Handle;
		static public IntPtr GetHandle (this DictionaryContainer self)
		{
			return self == null ? IntPtr.Zero : self.Dictionary.Handle;
		}

		// helper to avoid the (common pattern)
		// 	var p = x == null ? null : x.Dictionary;
		static public NSDictionary GetDictionary (this DictionaryContainer self)
		{
			return self == null ? null : self.Dictionary;
		}
	}
#endif
}
