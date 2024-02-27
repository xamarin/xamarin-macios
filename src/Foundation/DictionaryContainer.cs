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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if !COREBUILD
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#if !WATCH
using CoreMedia;
#endif // !WATCH
#endif

#if HAS_UIKIT
using UIKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Foundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public abstract class DictionaryContainer {
#if !COREBUILD
		protected DictionaryContainer ()
		{
			Dictionary = new NSMutableDictionary ();
		}

		protected DictionaryContainer (NSDictionary? dictionary)
		{
			Dictionary = dictionary ?? new NSMutableDictionary ();
		}

		public NSDictionary Dictionary { get; private set; }

		protected T []? GetArray<T> (NSString key) where T : NSObject
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
			return NSArray.ArrayFromHandle<T> (value);
		}

		protected T []? GetArray<T> (NSString key, Func<NativeHandle, T> creator)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
			return NSArray.ArrayFromHandleFunc<T> (value, creator);
		}

		protected int? GetInt32Value (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).Int32Value;
		}

		protected uint? GetUInt32Value (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).UInt32Value;
		}

		protected nint? GetNIntValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).NIntValue;
		}

		protected nuint? GetNUIntValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).NUIntValue;
		}

		protected long? GetLongValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).Int64Value;
		}

		protected uint? GetUIntValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).UInt32Value;
		}

		protected float? GetFloatValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).FloatValue;
		}

		protected double? GetDoubleValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return ((NSNumber) value).DoubleValue;
		}

		protected bool? GetBoolValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
			if (value == IntPtr.Zero)
				return null;

			return CFBoolean.GetValue (value);
		}

		protected T? GetNativeValue<T> (NSString key) where T : class, INativeObject
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			return Runtime.GetINativeObject<T> (Dictionary.LowlevelObjectForKey (key.Handle), false);
		}

		protected string []? GetStringArrayValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var array = Dictionary.LowlevelObjectForKey (key.Handle);
			return CFArray.StringArrayFromHandle (array)!;
		}

		protected NSDictionary? GetNSDictionary (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			Dictionary.TryGetValue (key, out value);
			return value as NSDictionary;
		}

		protected NSDictionary<TKey, TValue>? GetNSDictionary<TKey, TValue> (NSString key)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			Dictionary.TryGetValue (key, out value);
			return value as NSDictionary<TKey, TValue>;
		}

#if NET
		protected T? GetStrongDictionary<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> (NSString key)
#else
		protected T? GetStrongDictionary<T> (NSString key)
#endif
			where T : DictionaryContainer
		{
			return GetStrongDictionary (key, dict =>
				(T?) Activator.CreateInstance (typeof (T), new object [] { dict }));
		}

		protected T? GetStrongDictionary<T> (NSString? key, Func<NSDictionary, T?> createStrongDictionary)
			where T : DictionaryContainer
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var dict = GetNSDictionary (key);
			if (dict is null)
				return null;

			return createStrongDictionary (dict);
		}

		protected NSString? GetNSStringValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			Dictionary.TryGetValue (key, out value);
			return value as NSString;
		}

		protected string? GetStringValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			NSObject value;
			if (!Dictionary.TryGetValue (key, out value))
				return null;

			return CFString.FromHandle (value.Handle);
		}

		protected string? GetStringValue (string key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var keyHandle = CFString.CreateNative (key);
			try {
				return CFString.FromHandle (CFDictionary.GetValue (Dictionary.Handle, keyHandle));
			} finally {
				CFString.ReleaseNative (keyHandle);
			}
		}

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

#if !WATCH
		protected CMTime? GetCMTimeValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			if (dictValue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictValue));
			var value = CMTime.FromDictionary (dictValue);
			if (value.IsInvalid)
				return null;

			return value;
		}
#endif // !WATCH

#if HAS_UIKIT
		protected UIEdgeInsets? GetUIEdgeInsets (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			if (!Dictionary.TryGetValue (key, out var value))
				return null;

			if (value is NSValue size)
				return size.UIEdgeInsetsValue;

			return null;
		}
#endif

		bool NullCheckAndRemoveKey ([NotNullWhen (true)] NSString key, bool removeEntry)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			if (removeEntry)
				RemoveValue (key);

			return !removeEntry;
		}

		protected void SetArrayValue (NSString key, NSNumber []? values)
		{
			if (NullCheckAndRemoveKey (key, values is null))
				Dictionary [key] = NSArray.FromNSObjects (values);
		}

		protected void SetArrayValue<T> (NSString key, T []? values)
		{
			if (NullCheckAndRemoveKey (key, values is null)) {
				var nsValues = new NSObject [values!.Length];
				for (var i = 0; i < values.Length; i++)
					nsValues [i] = NSObject.FromObject (values [i]);
				Dictionary [key] = NSArray.FromNSObjects (nsValues);
			}
		}

		protected void SetArrayValue (NSString key, string []? values)
		{
			if (NullCheckAndRemoveKey (key, values is null))
				Dictionary [key] = NSArray.FromStrings (values);
		}

		protected void SetArrayValue (NSString key, INativeObject []? values)
		{
			if (NullCheckAndRemoveKey (key, values is null))
				CFMutableDictionary.SetValue (Dictionary.Handle, key.Handle, CFArray.FromNativeObjects (values!).Handle);
		}

		#region Sets CFBoolean value

		protected void SetBooleanValue (NSString key, bool? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				CFMutableDictionary.SetValue (Dictionary.Handle, key.Handle, value!.Value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle);
		}

		#endregion

		#region Sets NSNumber value

		protected void SetNumberValue (NSString key, int? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		protected void SetNumberValue (NSString key, uint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		protected void SetNumberValue (NSString key, nint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		protected void SetNumberValue (NSString key, nuint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		protected void SetNumberValue (NSString key, long? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		protected void SetNumberValue (NSString key, float? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		protected void SetNumberValue (NSString key, double? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = new NSNumber (value!.Value);
		}

		#endregion

		#region Sets NSString value

		protected void SetStringValue (NSString key, string? value)
		{
			SetStringValue (key, value is null ? (NSString) null! : new NSString (value));
		}

		protected void SetStringValue (NSString key, NSString? value)
		{
			if (NullCheckAndRemoveKey (key, value is null))
				Dictionary [key] = value;
		}

		#endregion

		#region Sets Native value

		protected void SetNativeValue (NSString key, INativeObject? value, bool removeNullValue = true)
		{
			if (NullCheckAndRemoveKey (key, removeNullValue && value is null))
				CFMutableDictionary.SetValue (Dictionary.Handle, key.Handle, value.GetHandle ());
		}

		#endregion

		protected void RemoveValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			((NSMutableDictionary) Dictionary).Remove (key);
		}

		#region Sets structs values

		protected void SetCGRectValue (NSString key, CGRect? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value!.Value.ToDictionary ();
		}

		protected void SetCGSizeValue (NSString key, CGSize? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value!.Value.ToDictionary ();
		}

		protected void SetCGPointValue (NSString key, CGPoint? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value!.Value.ToDictionary ();
		}

#if !WATCH
		protected void SetCMTimeValue (NSString key, CMTime? value)
		{
			if (NullCheckAndRemoveKey (key, !value.HasValue))
				Dictionary [key] = value!.Value.ToDictionary ();
		}
#endif //!WATCH

#if HAS_UIKIT
		protected void SetUIEdgeInsets (NSString key, UIEdgeInsets? value)
		{
			SetNativeValue (key, value is null ? null : NSValue.FromUIEdgeInsets (value.Value));
		}
#endif
		#endregion
#endif
	}

#if !COREBUILD
	static class DictionaryContainerHelper {

		// helper to avoid the (common pattern)
		// 	var p = x is null ? NativeHandle.Zero : h.Dictionary.Handle;
		static public NativeHandle GetHandle (this DictionaryContainer? self)
		{
			return self is null ? NativeHandle.Zero : self.Dictionary.Handle;
		}

		// helper to avoid the (common pattern)
		// 	var p = x is null ? null : x.Dictionary;
		static public NSDictionary? GetDictionary (this DictionaryContainer? self)
		{
			return self is null ? null : self.Dictionary;
		}
	}
#endif
}
