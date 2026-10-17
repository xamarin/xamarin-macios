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

using System.Diagnostics.CodeAnalysis;

#if !COREBUILD
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
#endif

#if HAS_UIKIT
using UIKit;
#endif

#nullable enable

namespace Foundation {
	/// <summary>Convenience class to provide strongly-typed classes that wrap <see cref="NSDictionary" /> based settings.</summary>
	/// <remarks>
	///   <para>
	///     Many APIs accept configuration options as untyped
	///     <see cref="NSDictionary" /> values, or return untyped <see cref="NSDictionary" /> values.
	///     The managed bindings offer strong-typed versions of those
	///     dictionaries, which allow developers to get code completion
	///     while passing parameters, and to extract data from return
	///     values.  The <see cref="DictionaryContainer" /> class is an abstract base class that
	///     encapsulates the common code to wrap <see cref="NSDictionary" /> dictionaries like this.
	///   </para>
	///   <para>
	///     When creating a strongly-typed wrapper for an <see cref="NSDictionary" />,
	///     subclass the <see cref="DictionaryContainer" /> and provide two constructors:
	///     one taking an <see cref="NSDictionary" /> (to create the wrapper) and a default
	///     costructor. They should each call the corresponding base class (<see cref="DictionaryContainer" />) constructor.
	///     Then use one of the various Get and Set methods exposed by this class to get and set values.  This is
	///     how a sample class would work:
	///   </para>
	///   <example>
	///     <code lang="csharp lang-csharp"><![CDATA[
	/// public class MyProperties : DictionaryContainer {
	///     public MyProperties () : base ()
	///     {
	///     }
	///
	///     public MyProperties (NSDictionary dictionary) : base (dictionary)
	///     {
	///     }
	///
	///     static NSString boolKey = new NSString ("SomeBoolKey");
	///
	///     public bool MyBool {
	///         get => return GetInt32Value (boolKey);
	///         set => SetInt32Value (boolKey, value);
	///     }
	/// }]]></code>
	///   </example>
	/// </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class DictionaryContainer {
#if !COREBUILD
		/// <summary>Creates a new container for an empty <see cref="NSDictionary" />.</summary>
		protected DictionaryContainer ()
		{
			Dictionary = new NSMutableDictionary ();
		}

		/// <summary>Creates a new container wrapping the specified <see cref="NSDictionary" />.</summary>
		/// <param name="dictionary">The dictionary to be wrapped.</param>
		protected DictionaryContainer (NSDictionary? dictionary)
		{
			Dictionary = dictionary ?? new NSMutableDictionary ();
		}

		/// <summary>Gets the wrapped <see cref="Foundation.NSDictionary" />.</summary>
		/// <value>The wrapped <see cref="NSDictionary" />.</value>
		public NSDictionary Dictionary { get; private set; }

		static T? Create<T> (NativeHandle handle) where T : DictionaryContainer, new()
		{
			if (handle == NativeHandle.Zero)
				return null;
			var dict = Runtime.GetNSObject<NSDictionary> (handle, owns: false);
			if (dict is null)
				return null;
			var rv = new T ();
			rv.Dictionary = dict;
			return rv;
		}

		/// <summary>Returns the nullable <typeparamref name="T" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns><see langword="true" /> if the specified <paramref name="key" /> was found in the dictionary, <see langword="false" /> otherwise.</returns>
		protected T? GetNativeValue<T> (NSString key) where T : class, INativeObject
		{
			if (!TryGetNativeValue (key, out var value))
				return null;

			return Runtime.GetINativeObject<T> (value, false);
		}

		/// <summary>Returns the nullable <typeparamref name="T" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="object">The <typeparamref name="T" /> instance that was found (or <see langword="null" /> if no value was found for the specified key).</param>
		/// <remarks>This method will throw an exception if the dictionary contains a value for the specified key that is not an <typeparamref name="T" />.</remarks>
		/// <returns><see langword="true" /> if the specified <paramref name="key" /> was found in the dictionary, <see langword="false" /> otherwise.</returns>
		protected bool TryGetNSObject<T> (NSString key, [NotNullWhen (true)] out T? @object) where T : NSObject
		{
			@object = null;

			if (!TryGetNativeValue (key, out var handle))
				return false;

			// Don't use GetNSObject<T>, because then we don't get any type checking.
			var value = Runtime.GetNSObject (handle, false);
			if (value is null)
				return false;
			@object = value as T;
			if (@object is null)
				throw new InvalidOperationException (string.Format ("The stored type for the key '{0}' is not '{1}', it's '{2}' (value: {3}).", key, typeof (T), value.GetType (), value));

			return true;
		}

		T? GetNSObject<T> (NSString key) where T : NSObject
		{
			if (!TryGetNSObject<T> (key, out var value))
				return null;
			return value;
		}

		/// <summary>Returns the nullable <see cref="NSNumber" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="number">The <see cref="NSNumber" /> instance that was found (or <see langword="null" /> if no value was found for the specified key).</param>
		/// <remarks>This method will throw an exception if the dictionary contains a value for the specified key that is not an <see cref="NSNumber" />.</remarks>
		/// <returns><see langword="true" /> if the specified <paramref name="key" /> was found in the dictionary, <see langword="false" /> otherwise.</returns>
		bool TryGetNSNumber (NSString key, [NotNullWhen (true)] out NSNumber? number)
		{
			return TryGetNSObject<NSNumber> (key, out number);
		}

		/// <summary>Returns the handle associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The returned handle that was found (or <see cref="NativeHandle.Zero" /> if no value was found for the specified key).</param>
		/// <returns><see langword="true" /> if the specified <paramref name="key" /> was found in the dictionary and the found value was not <see cref="NativeHandle.Zero" />, <see langword="false" /> otherwise.</returns>
		protected bool TryGetNativeValue (NSString key, out NativeHandle value)
		{
			var rv = TryGetNativeValue (key.GetHandle (), out value);
			GC.KeepAlive (key);
			return rv;
		}

		/// <summary>Returns the handle associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The returned handle that was found (or <see cref="NativeHandle.Zero" /> if no value was found for the specified key).</param>
		/// <returns><see langword="true" /> if the specified <paramref name="key" /> was found in the dictionary and the found value was not <see cref="NativeHandle.Zero" />, <see langword="false" /> otherwise.</returns>
		bool TryGetNativeValue (NativeHandle key, out NativeHandle value)
		{
			if (key == IntPtr.Zero)
				throw new ArgumentNullException (nameof (key));

			value = default;

			if (!CFDictionary.TryGetValue (Dictionary.Handle, key, out var handle))
				return false;

			value = handle;
			return value != NativeHandle.Zero;
		}

		/// <summary>Returns the nullable array of <typeparamref name="T" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored array of <typeparamref name="T" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected T []? GetArray<T> (NSString key) where T : NSObject
		{
			if (!TryGetNativeValue (key, out var value))
				return null;

			return NSArray.ArrayFromHandleDropNullElements<T> (value);
		}

		/// <summary>Returns the nullable array of <typeparamref name="T" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="creator">A callback to convert between the native object stored in the dictionary to the desired return type.</param>
		/// <returns>The stored array of <typeparamref name="T" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected T []? GetArray<T> (NSString key, Func<NativeHandle, T> creator)
		{
			if (!TryGetNativeValue (key, out var value))
				return null;

			return NSArray.ArrayFromHandleDropNullElements<T> (value, (v) => creator (v), NSNullBehavior.DropIfIncompatible);
		}

		/// <summary>Retrieves the <see cref="DictionaryContainer" /> array associeted with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The <see cref="DictionaryContainer" /> array associeted with <paramref name="key" />.</returns>
		protected T []? GetArrayOfDictionariesValue<T> (NSString key) where T : DictionaryContainer, new()
		{
			if (!TryGetNativeValue (key, out var value))
				return null;

			return NSArray.ArrayFromHandleDropNullElements<T> (value, (handle) => Create<T> (handle)!, NSNullBehavior.DropIfIncompatible);
		}

		/// <summary>Returns the nullable <see cref="byte" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="byte" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected byte? GetByteValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.ByteValue;
		}

		/// <summary>Returns the nullable <see cref="sbyte" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="sbyte" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected sbyte? GetSByteValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.SByteValue;
		}

		/// <summary>Returns the nullable <see cref="short" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="short" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected short? GetInt16Value (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.Int16Value;
		}

		/// <summary>Returns the nullable <see cref="ushort" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="ushort" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected ushort? GetUInt16Value (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.UInt16Value;
		}

		/// <summary>Returns the nullable <see cref="int" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="int" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected int? GetInt32Value (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.Int32Value;
		}

		/// <summary>Returns the nullable <see cref="uint" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="uint" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected uint? GetUInt32Value (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.UInt32Value;
		}

		/// <summary>Returns the nullable <see cref="nint" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="nint" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected nint? GetNIntValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.NIntValue;
		}

		/// <summary>Returns the nullable <see cref="nuint" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="nuint" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected nuint? GetNUIntValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.NUIntValue;
		}

		/// <summary>Returns the nullable <see cref="long" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="long" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected long? GetLongValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.Int64Value;
		}

		/// <summary>Returns the nullable <see cref="ulong" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="ulong" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected ulong? GetULongValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.UInt64Value;
		}

		/// <summary>Returns the nullable <see cref="uint" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="uint" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected uint? GetUIntValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.UInt32Value;
		}

		/// <summary>Returns the nullable <see cref="float" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="float" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected float? GetFloatValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.FloatValue;
		}

		/// <summary>Returns the nullable <see cref="nfloat" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="nfloat" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected nfloat? GetNFloatValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			// we only support 64-bit architectures at the moment, so this is safe
			Validate64BitNFloat ();

			return (nfloat) number.DoubleValue;
		}

		/// <summary>Returns the nullable <see cref="double" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="double" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSNumber" />.</remarks>
		protected double? GetDoubleValue (NSString key)
		{
			if (!TryGetNSNumber (key, out var number))
				return null;

			return number.DoubleValue;
		}

		/// <summary>Returns the nullable <see cref="bool" /> associated with the specified <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="bool" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected bool? GetBoolValue (NSString key)
		{
			if (!TryGetNativeValue (key, out var value))
				return null;

			return CFBoolean.GetValue (value);
		}

		/// <summary>Returns the nullable DateTime associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="DateTime" /> value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		/// <remarks>The stored value must be stored as an <see cref="NSDate" />.</remarks>
		protected DateTime? GetDateTimeValue (NSString key)
		{
			if (!TryGetNSObject<NSDate> (key, out var date))
				return null;

			return (DateTime) date;
		}

		/// <summary>Returns the nullable string array associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored string array value if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected string []? GetStringArrayValue (NSString key)
		{
			if (!TryGetNativeValue (key, out var array))
				return null;

			return CFArray.StringArrayFromHandle (array)!;
		}

		/// <summary>Returns the nullable <see cref="NSDictionary" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="NSDictionary" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected NSDictionary? GetNSDictionary (NSString key)
		{
			return GetNSObject<NSDictionary> (key);
		}

		/// <summary>Returns the nullable <see cref="NSDictionary{TKey,TValue}" /> associated with <paramref name="key" />.</summary>
		/// <typeparam name="TKey">The type of keys in the stored dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values in the stored dictionary.</typeparam>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="NSDictionary{TKey,TValue}" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected NSDictionary<TKey, TValue>? GetNSDictionary<TKey, TValue> (NSString key)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			return GetNativeValue<NSDictionary<TKey, TValue>> (key);
		}

		/// <summary>Returns an <see cref="NSData" /> with the contents of a value type of type <typeparamref name="T" /> associated with <paramref name="key" />.</summary>
		/// <typeparam name="T">The value type stored in the type <see cref="NSData" />.</typeparam>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The value type initialized from the data in the <see cref="NSData" />, or <see langword="null" /> if the specified <paramref name="key" /> is not present in this dictionary.</returns>
		protected T? GetNSDataAsValueType<T> (NSString key) where T : unmanaged
		{
			if (!TryGetNSObject<NSData> (key, out var data))
				return null;
			return data.ToValueType<T> ();
		}

		/// <summary>Sets an <see cref="NSData" /> with the contents of a value type of type <typeparamref name="T" /> associated with <paramref name="key" />.</summary>
		/// <typeparam name="T">The value type stored in the type <see cref="NSData" />.</typeparam>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The value to store in the <see cref="NSData" />.</param>
		protected void SetValueTypeAsNSData<T> (NSString key, T? value) where T : unmanaged
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var data = NSData.CreateFromValueType<T> (value);
			SetNativeValue (key, data);
		}

		/// <summary>Returns the nullable <typeparamref name="T" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <typeparamref name="T" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected T? GetStrongDictionary<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> (NSString key)
			where T : DictionaryContainer
		{
			return GetStrongDictionary (key, dict =>
				(T?) Activator.CreateInstance (typeof (T), new object [] { dict }));
		}

		/// <summary>Returns the nullable <typeparamref name="T" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="createStrongDictionary">A callback to convert between an <see cref="NSDictionary" /> and <typeparamref name="T" />.</param>
		/// <returns>The stored <typeparamref name="T" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected T? GetStrongDictionary<T> (NSString key, Func<NSDictionary, T?> createStrongDictionary)
			where T : DictionaryContainer
		{
			var dict = GetNSDictionary (key);
			if (dict is null)
				return null;

			return createStrongDictionary (dict);
		}

		/// <summary>Returns the nullable <see cref="NSString" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="NSString" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected NSString? GetNSStringValue (NSString key)
		{
			return GetNSObject<NSString> (key);
		}

		/// <summary>Returns the nullable <see cref="string" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="string" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected string? GetStringValue (NSString key)
		{
			if (!TryGetNativeValue (key, out var value))
				return null;

			return CFString.FromHandle (value);
		}

		/// <inheritdoc cref="GetStringValue(NSString)" />
		protected string? GetStringValue (string key)
		{
			using var keyHandle = new TransientCFString (key);
			if (TryGetNativeValue ((IntPtr) keyHandle, out var value))
				return null;
			return CFString.FromHandle (value);
		}

		/// <summary>Returns the nullable <see cref="CGRect" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="CGRect" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected CGRect? GetCGRectValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			if (!CGRect.TryParse (dictValue, out var value))
				return null;

			return value;
		}

		/// <summary>Returns the nullable <see cref="CGSize" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="CGSize" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected CGSize? GetCGSizeValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			if (!CGSize.TryParse (dictValue, out var value))
				return null;

			return value;
		}

		/// <summary>Returns the nullable <see cref="CGPoint" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="CGPoint" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected CGPoint? GetCGPointValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			if (!CGPoint.TryParse (dictValue, out var value))
				return null;

			return value;
		}

		/// <summary>Returns the nullable <see cref="CMTime" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="CMTime" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected CMTime? GetCMTimeValue (NSString key)
		{
			var dictValue = GetNSDictionary (key);
			if (dictValue is null)
				return null;
			var value = CMTime.FromDictionary (dictValue);
			if (value.IsInvalid)
				return null;

			return value;
		}

#if HAS_UIKIT
		/// <summary>Returns the nullable <see cref="UIEdgeInsets" /> associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <returns>The stored <see cref="UIEdgeInsets" /> if present in the dictionary, <see langword="null" /> otherwise.</returns>
		protected UIEdgeInsets? GetUIEdgeInsets (NSString key)
		{
			if (!TryGetNSObject<NSValue> (key, out var value))
				return null;

			return value.UIEdgeInsetsValue;
		}
#endif

		bool NullCheckValueTypeAndRemoveKey<T> ([NotNullWhen (true)] NSString key, [NotNullWhen (true)] T? valueType) where T : struct
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			if (!valueType.HasValue)
				RemoveValue (key);

			return valueType.HasValue;
		}

		bool NullCheckObjectAndRemoveKey ([NotNullWhen (true)] NSString key, [NotNullWhen (true)] object? obj)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			if (obj is null)
				RemoveValue (key);

			return obj is not null;
		}

		/// <summary>Associates the <see cref="NSNumber" /> array <paramref name="values" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="values">The <see cref="NSNumber" /> array to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetArrayValue (NSString key, NSNumber []? values)
		{
			if (NullCheckObjectAndRemoveKey (key, values))
				Dictionary [key] = NSArray.FromNSObjects (values);
		}

		/// <summary>Associates an array of <typeparamref name="T" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="values">The array of <typeparamref name="T" /> to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetArrayValue<T> (NSString key, T []? values)
		{
			if (NullCheckObjectAndRemoveKey (key, values)) {
				var nsValues = new NSObject [values.Length];
				for (var i = 0; i < values.Length; i++)
					nsValues [i] = NSObject.FromObject (values [i])!;
				Dictionary [key] = NSArray.FromNSObjects (nsValues);
			}
		}

		/// <summary>Associates a <see langword="string" /> array  with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="values">The <see langword="string" /> to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetArrayValue (NSString key, string []? values)
		{
			if (NullCheckObjectAndRemoveKey (key, values))
				Dictionary [key] = NSArray.FromStrings (values);
		}

		/// <summary>Associates an array of <see cref="INativeObject" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="values">The array of <see cref="INativeObject" /> to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetArrayValue (NSString key, INativeObject []? values)
		{
			SetArrayValue<INativeObject> (key, values, NativeObjectExtensions.GetHandle);
		}

		/// <summary>Associates an array of <typeparamref name="T" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="values">The array of <typeparamref name="T" /> to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		/// <param name="convert">A callback to convert between the array's element type to <see cref="NativeHandle" /></param>
		protected void SetArrayValue<T> (NSString key, T []? values, Func<T, NativeHandle> convert)
		{
			if (NullCheckObjectAndRemoveKey (key, values)) {
				var handles = new NativeHandle [values.Length];
				for (var i = 0; i < handles.Length; i++)
					handles [i] = convert (values [i]);
				var array = CFArray.FromIntPtrs (handles!);
				CFMutableDictionary.SetValue (Dictionary, key, array);
			}
		}

		/// <summary>Associates a <see cref="DictionaryContainer" /> array <paramref name="values" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="values">The <see cref="DictionaryContainer" /> array to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetArrayOfDictionariesValue (NSString key, DictionaryContainer []? values)
		{
			SetArrayValue<DictionaryContainer> (key, values, DictionaryContainerHelper.GetHandle);
		}

		#region Sets CFBoolean value

		/// <summary>Associates a nullable <see cref="bool" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="bool" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetBooleanValue (NSString key, bool? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value)) {
				var v = value.Value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle;
				CFMutableDictionary.SetValue (Dictionary, key, v);
			}
		}

		#endregion

		#region Sets NSNumber value

		/// <summary>Associates a nullable <see cref="byte" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="byte" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, byte? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="sbyte" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="sbyte" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, sbyte? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="short" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="short" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, short? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="ushort" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="ushort" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, ushort? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="int" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="int" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, int? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="uint" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="uint" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>

		protected void SetNumberValue (NSString key, uint? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="nint" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="nint" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, nint? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="nuint" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="nuint" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, nuint? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="long" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="long" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, long? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="ulong" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="ulong" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, ulong? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="float" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="float" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, float? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		/// <summary>Associates a nullable <see cref="nfloat" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="nfloat" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, nfloat? value)
		{
			// we only support 64-bit architectures at the moment, so this is safe
			Validate64BitNFloat ();

			SetNumberValue (key, (double?) value);
		}

		/// <summary>Associates a nullable <see cref="double" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="double" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetNumberValue (NSString key, double? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = new NSNumber (value.Value);
		}

		#endregion

		#region Sets NSString value

		/// <summary>Associates a nullable <see cref="string" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="string" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetStringValue (NSString key, string? value)
		{
			SetStringValue (key, (NSString?) value);
		}

		/// <summary>Associates a nullable <see cref="NSString" /> with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="NSString" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetStringValue (NSString key, NSString? value)
		{
			if (NullCheckObjectAndRemoveKey (key, value))
				Dictionary [key] = value;
		}

		#endregion

		#region Sets Native value

		/// <summary>Associates a nullable <see cref="INativeObject" /> value with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="INativeObject" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		/// <param name="removeNullValue">Specifies whether any existing values should be removed if <paramref name="value" /> is <see langword="null" />.</param>
		protected void SetNativeValue (NSString key, INativeObject? value, bool removeNullValue = true)
		{
			if (NullCheckObjectAndRemoveKey (key, removeNullValue ? value : key /* key is just an object that's not null */))
				CFMutableDictionary.SetValue (Dictionary, key, value);
		}

		#endregion

		/// <summary>Removes from the dictionary the value associated with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		protected void RemoveValue (NSString key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			((NSMutableDictionary) Dictionary).Remove (key);
		}

		#region Sets structs values

		/// <summary>Associates a nullable <see cref="CGRect" /> value with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="CGRect" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetCGRectValue (NSString key, CGRect? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = value.Value.ToDictionary ();
		}

		/// <summary>Associates a nullable <see cref="CGSize" /> value with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="CGSize" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetCGSizeValue (NSString key, CGSize? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = value.Value.ToDictionary ();
		}

		/// <summary>Associates a nullable <see cref="CGPoint" /> value with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="CGPoint" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetCGPointValue (NSString key, CGPoint? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = value.Value.ToDictionary ();
		}

		/// <summary>Associates a nullable <see cref="CMTime" /> value with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="CMTime" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetCMTimeValue (NSString key, CMTime? value)
		{
			if (NullCheckValueTypeAndRemoveKey (key, value))
				Dictionary [key] = value.Value.ToDictionary ();
		}

#if HAS_UIKIT
		/// <summary>Associates a nullable <see cref="UIEdgeInsets" /> value with <paramref name="key" />.</summary>
		/// <param name="key">The identifier of the value to get.</param>
		/// <param name="value">The nullable <see cref="UIEdgeInsets" /> value to be associated with <paramref name="key" />. Pass <see langword="null" /> to remove any existing association.</param>
		protected void SetUIEdgeInsets (NSString key, UIEdgeInsets? value)
		{
			SetNativeValue (key, value is null ? null : NSValue.FromUIEdgeInsets (value.Value));
		}
#endif
		#endregion
#endif

		unsafe static void Validate64BitNFloat ()
		{
			var size = sizeof (nfloat);
			if (size != 8)
				throw new InvalidOperationException ($"Unexpected nfloat size: {size}");
		}
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
		[return: NotNullIfNotNull (nameof (self))]
		static public NSDictionary? GetDictionary (this DictionaryContainer? self)
		{
			return self is null ? null : self.Dictionary;
		}
	}
#endif
}
