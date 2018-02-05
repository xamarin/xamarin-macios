//
// MDLAnimatedValueTypes.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

using Vector2 = global::OpenTK.Vector2;
using Vector2d = global::OpenTK.Vector2d;
using Vector3 = global::OpenTK.NVector3;
using Vector3d = global::OpenTK.NVector3d;
using Vector4 = global::OpenTK.Vector4;
using Vector4d = global::OpenTK.Vector4d;
using Matrix4 = global::OpenTK.NMatrix4;
using Matrix4d = global::OpenTK.NMatrix4d;
using Quaternion = global::OpenTK.Quaternion;
using Quaterniond = global::OpenTK.Quaterniond;

// Because of the lack of documentation when at the time of binding ModelIO for iOS 11 types
// we are implementing something similar to what swift is doing here
// https://github.com/apple/swift/blob/cbdf0ff1e7bfbd192c33d64c9c7d31fbb11f712c/stdlib/public/SDK/ModelIO/ModelIO.swift

namespace ModelIO {
	public partial class MDLAnimatedValue {

		public double [] KeyTimes {
			get {
				var wkt = WeakKeyTimes;
				if (wkt == null)
					return null;

				var count = wkt.Length;
				var ret = new double [count];

				for (int i = 0; i < count; i++)
					ret [i] = wkt [i].DoubleValue;

				return ret;
			}
		}

		// Follow suit with swift's but making this
		// a method instead of a property for three reasons:
		// 1. Debugger performance.
		// 2. _GetTimes return value is ignored and could turn out useful at some point.
		// 3. Lack of documentation at the moment of binding this.
		// [1]: https://github.com/apple/swift/blob/cbdf0ff1e7bfbd192c33d64c9c7d31fbb11f712c/stdlib/public/SDK/ModelIO/ModelIO.swift#L50
		public virtual double [] GetTimes ()
		{
			var count = TimeSampleCount;
			var timesArr = new double [(int) count];
			unsafe {
				fixed (double* ptr = timesArr)
					_GetTimes ((IntPtr) ptr, count);
			}
			return timesArr;
		}
	}

	public partial class MDLAnimatedScalarArray {

		public virtual void SetValues (float [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (float* ptr = array)
					_SetFloatArray ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual void SetValues (double [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (double* ptr = array)
					_SetDoubleArray ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual float [] GetFloatValues (double time)
		{
			var count = ElementCount;
			var timesArr = new float [(int) count];

			unsafe {
				fixed (float* ptr = timesArr)
					_GetFloatArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual double [] GetDoubleValues (double time)
		{
			var count = ElementCount;
			var timesArr = new double [(int) count];

			unsafe {
				fixed (double* ptr = timesArr)
					_GetDoubleArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual void Reset (float [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (float* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloatArray ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (double [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (double* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDoubleArray ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual float [] GetFloatValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new float [(int) count];

			unsafe {
				fixed (float* ptr = timesArr)
					_GetFloatArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual double [] GetDoubleValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new double [(int) count];

			unsafe {
				fixed (double* ptr = timesArr)
					_GetDoubleArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}
	}

	public partial class MDLAnimatedVector3Array {

		public virtual void SetValues (Vector3 [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf (typeof (Vector3));

			unsafe {
				fixed (Vector3* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetFloat3Array);
			}
		}

		public virtual void SetValues (Vector3d [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf (typeof (Vector3d));

			unsafe {
				fixed (Vector3d* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetDouble3Array);
			}
		}

		public virtual Vector3 [] GetNVector3Values (double time)
		{
			var count = ElementCount;
			var timesArr = new Vector3 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector3));

			unsafe {
				fixed (Vector3* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetFloat3Array);
			}

			return timesArr;
		}

		public virtual Vector3d [] GetNVector3dValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Vector3d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector3d));

			unsafe {
				fixed (Vector3d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetDouble3Array);
			}

			return timesArr;
		}

		public virtual void Reset (Vector3 [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf (typeof (Vector3));

			unsafe {
				fixed (Vector3* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithFloat3Array);
			}
		}

		public virtual void Reset (Vector3d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf (typeof (Vector3d));

			unsafe {
				fixed (Vector3d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithDouble3Array);
			}
		}

		public virtual Vector3 [] GetNVector3Values ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Vector3 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector3));

			unsafe {
				fixed (Vector3* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat3Array);
			}

			return timesArr;
		}

		public virtual Vector3d [] GetNVector3dValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Vector3d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector3d));

			unsafe {
				fixed (Vector3d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble3Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedQuaternionArray {

		public virtual void SetValues (Quaternion [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf (typeof (Quaternion));

			unsafe {
				fixed (Quaternion* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetFloatQuaternionArray);
			}
		}

		public virtual void SetValues (Quaterniond [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf (typeof (Quaterniond));

			unsafe {
				fixed (Quaterniond* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetDoubleQuaternionArray);
			}
		}

		public virtual Quaternion [] GetQuaternionValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Quaternion [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Quaternion));

			unsafe {
				fixed (Quaternion* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetFloatQuaternionArray);
			}

			return timesArr;
		}

		public virtual Quaterniond [] GetQuaterniondValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Quaterniond [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Quaterniond));

			unsafe {
				fixed (Quaterniond* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetDoubleQuaternionArray);
			}

			return timesArr;
		}

		public virtual void Reset (Quaternion [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf (typeof (Quaternion));

			unsafe {
				fixed (Quaternion* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithFloatQuaternionArray);
			}
		}

		public virtual void Reset (Quaterniond [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf (typeof (Quaterniond));

			unsafe {
				fixed (Quaterniond* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithDoubleQuaternionArray);
			}
		}

		public virtual Quaternion [] GetQuaternionValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Quaternion [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Quaternion));

			unsafe {
				fixed (Quaternion* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloatQuaternionArray);
			}

			return timesArr;
		}

		public virtual Quaterniond [] GetQuaterniondValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Quaterniond [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Quaterniond));

			unsafe {
				fixed (Quaterniond* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDoubleQuaternionArray);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedScalar {

		public virtual void Reset (float [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (float* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloatArray ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (double [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (double* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDoubleArray ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual float [] GetFloatValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new float [(int) count];

			unsafe {
				fixed (float* ptr = timesArr)
					_GetFloatArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual double [] GetDoubleValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new double [(int) count];

			unsafe {
				fixed (double* ptr = timesArr)
					_GetDoubleArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}
	}

	public partial class MDLAnimatedVector2 {

		public virtual void Reset (Vector2 [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Vector2));

			unsafe {
				fixed (Vector2* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat2Array);
			}
		}

		public virtual void Reset (Vector2d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Vector2d));

			unsafe {
				fixed (Vector2d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble2Array);
			}
		}

		public virtual Vector2 [] GetVector2Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector2 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector2));

			unsafe {
				fixed (Vector2* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat2Array);
			}

			return timesArr;
		}

		public virtual Vector2d [] GetVector2dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector2d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector2d));

			unsafe {
				fixed (Vector2d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble2Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedVector3 {

		public virtual void Reset (Vector3 [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Vector3));

			unsafe {
				fixed (Vector3* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat3Array);
			}
		}

		public virtual void Reset (Vector3d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Vector3d));

			unsafe {
				fixed (Vector3d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble3Array);
			}
		}

		public virtual Vector3 [] GetNVector3Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector3 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector3));

			unsafe {
				fixed (Vector3* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat3Array);
			}

			return timesArr;
		}

		public virtual Vector3d [] GetNVector3dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector3d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector3d));

			unsafe {
				fixed (Vector3d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble3Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedVector4 {

		public virtual void Reset (Vector4 [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Vector4));

			unsafe {
				fixed (Vector4* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat4Array);
			}
		}

		public virtual void Reset (Vector4d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Vector4d));

			unsafe {
				fixed (Vector4d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble4Array);
			}
		}

		public virtual Vector4 [] GetVector4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector4 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector4));

			unsafe {
				fixed (Vector4* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat4Array);
			}

			return timesArr;
		}

		public virtual Vector4d [] GetVector4dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector4d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Vector4d));

			unsafe {
				fixed (Vector4d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble4Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedMatrix4x4 {

		public virtual void Reset (Matrix4 [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Matrix4));

			unsafe {
				fixed (Matrix4* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat4x4Array);
			}
		}

		public virtual void Reset (Matrix4d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf (typeof (Matrix4d));

			unsafe {
				fixed (Matrix4d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble4x4Array);
			}
		}

		public virtual Matrix4 [] GetNMatrix4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Matrix4 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Matrix4));

			unsafe {
				fixed (Matrix4* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat4x4Array);
			}

			return timesArr;
		}

		public virtual Matrix4d [] GetNMatrix4dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Matrix4d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Matrix4d));

			unsafe {
				fixed (Matrix4d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble4x4Array);
			}

			return timesArr;
		}
	}

	public partial class MDLMatrix4x4Array {

		public virtual void SetValues (Matrix4 [] array)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf (typeof (Matrix4));

			unsafe {
				fixed (Matrix4* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, _SetFloat4x4Array);
			}
		}

		public virtual void SetValues (Matrix4d [] array)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf (typeof (Matrix4d));

			unsafe {
				fixed (Matrix4d* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, _SetDouble4x4Array);
			}
		}

		public virtual Matrix4 [] GetNMatrix4Values ()
		{
			var count = ElementCount;
			var timesArr = new Matrix4 [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Matrix4));

			unsafe {
				fixed (Matrix4* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat4x4Array);
			}

			return timesArr;
		}

		public virtual Matrix4d [] GetNMatrix4dValues ()
		{
			var count = ElementCount;
			var timesArr = new Matrix4d [(int) count];
			int typeSize = Marshal.SizeOf (typeof (Matrix4d));

			unsafe {
				fixed (Matrix4d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble4x4Array);
			}

			return timesArr;
		}
	}

	static class MDLMemoryHelper {
		static IntPtr GetAlignedPtrForArray (int typeSize, int managedArrayCount, out int nativeArrayLength, out IntPtr alignedPtr)
		{
			nativeArrayLength = typeSize * managedArrayCount;
			var ptr = Marshal.AllocHGlobal (nativeArrayLength + 16);
			alignedPtr = new IntPtr (((nint) (ptr + 15) >> 4) << 4);
			return ptr;
		}

		internal static void SetValues (int typeSize, IntPtr arrptr, int arrLength, Action<IntPtr, nuint> setFunc)
		{
			int nativeArrayLength;
			IntPtr aligned_ptr = arrptr;
			var ptr = IntPtr.Zero;

			if ((nuint) aligned_ptr % 16 != 0) {
				ptr = GetAlignedPtrForArray (typeSize, arrLength, out nativeArrayLength, out aligned_ptr);
				Runtime.memcpy (aligned_ptr, arrptr, nativeArrayLength);
			}

			setFunc (aligned_ptr, (nuint) arrLength);

			if (ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);
		}

		internal static void SetValues (int typeSize, IntPtr arrptr, int arrLength, double time, Action<IntPtr, nuint, double> setFunc)
		{
			int nativeArrayLength;
			IntPtr aligned_ptr = arrptr;
			var ptr = IntPtr.Zero;

			if ((nuint) aligned_ptr % 16 != 0) {
				ptr = GetAlignedPtrForArray (typeSize, arrLength, out nativeArrayLength, out aligned_ptr);
				Runtime.memcpy (aligned_ptr, arrptr, nativeArrayLength);
			}

			setFunc (aligned_ptr, (nuint) arrLength, time);

			if (ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);
		}

		internal static nuint FetchValues (int typeSize, IntPtr arrptr, nuint count, Func<IntPtr, nuint, nuint> getFunc)
		{
			int nativeArrayLength;
			IntPtr aligned_ptr = arrptr;
			var ptr = IntPtr.Zero;
			nuint retVal;

			if ((nuint) aligned_ptr % 16 != 0) {
				ptr = GetAlignedPtrForArray (typeSize, (int) count, out nativeArrayLength, out aligned_ptr);
				retVal = getFunc (aligned_ptr, count);
				Runtime.memcpy (arrptr, aligned_ptr, nativeArrayLength);
			} else
				retVal = getFunc (arrptr, count);

			if (ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);

			return retVal;
		}

		internal static nuint FetchValues (int typeSize, IntPtr arrptr, nuint count, double time, Func<IntPtr, nuint, double, nuint> getFunc)
		{
			int nativeArrayLength;
			IntPtr aligned_ptr = arrptr;
			var ptr = IntPtr.Zero;
			nuint retVal;

			if ((nuint) aligned_ptr % 16 != 0) {
				ptr = GetAlignedPtrForArray (typeSize, (int) count, out nativeArrayLength, out aligned_ptr);
				retVal = getFunc (aligned_ptr, count, time);
				Runtime.memcpy (arrptr, aligned_ptr, nativeArrayLength);
			} else
				retVal = getFunc (arrptr, count, time);

			if (ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);

			return retVal;
		}

		internal static void Reset (int typeSize, IntPtr valuesPtr, double [] times, Action<IntPtr, IntPtr, nuint> resetFunc)
		{
			int nativeArrayLength;
			IntPtr aligned_ptr = valuesPtr;
			var ptr = IntPtr.Zero;

			if ((nuint) aligned_ptr % 16 != 0) {
				ptr = GetAlignedPtrForArray (typeSize, times.Length, out nativeArrayLength, out aligned_ptr);
				Runtime.memcpy (aligned_ptr, valuesPtr, nativeArrayLength);
			}

			unsafe {
				fixed (double* timesPtr = times)
					resetFunc (aligned_ptr, (IntPtr) timesPtr, (nuint) times.Length);
			}

			if (ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);
		}

		internal static void Reset (int typeSize, IntPtr valuesPtr, int valuesLength, double [] times, Action<IntPtr, nuint, IntPtr, nuint> resetFunc)
		{
			int nativeArrayLength;
			IntPtr aligned_ptr = valuesPtr;
			var ptr = IntPtr.Zero;

			if ((nuint) aligned_ptr % 16 != 0) {
				ptr = GetAlignedPtrForArray (typeSize, valuesLength, out nativeArrayLength, out aligned_ptr);
				Runtime.memcpy (aligned_ptr, valuesPtr, nativeArrayLength);
			}

			unsafe {
				fixed (double* timesPtr = times)
					resetFunc (aligned_ptr, (nuint) valuesLength, (IntPtr) timesPtr, (nuint) times.Length);
			}

			if (ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);
		}
	}
}
#endif