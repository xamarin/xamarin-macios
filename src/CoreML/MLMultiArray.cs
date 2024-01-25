//
// MLMultiArray.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;

using Foundation;

using ObjCRuntime;

namespace CoreML {
	public partial class MLMultiArray {
		static NSNumber [] ConvertArray (nint [] value)
		{
			if (value is null)
				return Array.Empty<NSNumber> ();

			return Array.ConvertAll<nint, NSNumber> (value, NSNumber.FromNInt);
		}

		// NSArray<NSNumber> => nint[]
		internal static nint [] ConvertArray (IntPtr handle)
		{
			return NSArray.ArrayFromHandle<nint> (handle, (v) => (nint) Messaging.IntPtr_objc_msgSend (v, Selector.GetHandle ("integerValue")));
		}

		public MLMultiArray (nint [] shape, MLMultiArrayDataType dataType, out NSError error)
			: this (ConvertArray (shape), dataType, out error)
		{
		}

		public MLMultiArray (IntPtr dataPointer, nint [] shape, MLMultiArrayDataType dataType, nint [] strides, Action<IntPtr> deallocator, out NSError error)
			: this (dataPointer, ConvertArray (shape), dataType, ConvertArray (strides), deallocator, out error)
		{
		}

		public NSNumber this [nint idx] {
			get { return GetObject (idx); }
			set { SetObject (value, idx); }
		}

		public NSNumber this [params nint [] indices] {
			get { return GetObject (indices); }
			set { SetObject (value, indices); }
		}

		public NSNumber this [NSNumber [] key] {
			get { return GetObject (key); }
			set { SetObject (value, key); }
		}

		public NSNumber GetObject (params nint [] indices)
		{
			using (var arr = NSArray.FromNSObjects<nint> (NSNumber.FromNInt, indices))
				return GetObjectInternal (arr.GetHandle ());
		}

		public void SetObject (NSNumber obj, params nint [] indices)
		{
			using (var arr = NSArray.FromNSObjects<nint> (NSNumber.FromNInt, indices))
				SetObjectInternal (obj, arr.GetHandle ());
		}

		public nint [] Shape {
			get {
				return ConvertArray (_Shape);
			}
		}

		public nint [] Strides {
			get {
				return ConvertArray (_Strides);
			}
		}
	}
}
