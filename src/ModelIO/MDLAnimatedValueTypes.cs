//
// MDLAnimatedValueTypes.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

#if NET
using Vector2 = global::System.Numerics.Vector2;
using Vector2d = global::CoreGraphics.NVector2d;
using Vector3 = global::CoreGraphics.NVector3;
using Vector3d = global::CoreGraphics.NVector3d;
using Vector4 = global::System.Numerics.Vector4;
using Vector4d = global::CoreGraphics.NVector4d;
using Matrix4 = global::CoreGraphics.NMatrix4;
using Matrix4d = global::CoreGraphics.NMatrix4d;
using Quaternion = global::System.Numerics.Quaternion;
using Quaterniond = global::CoreGraphics.NQuaterniond;
#else
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
#endif

#nullable enable

// Because of the lack of documentation when at the time of binding ModelIO for iOS 11 types
// we are implementing something similar to what swift is doing here
// https://github.com/apple/swift/blob/cbdf0ff1e7bfbd192c33d64c9c7d31fbb11f712c/stdlib/public/SDK/ModelIO/ModelIO.swift

namespace ModelIO {
	public partial class MDLAnimatedValue {

		public double []? KeyTimes {
			get {
				var wkt = WeakKeyTimes;
				if (wkt is null)
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
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));

			unsafe {
				fixed (float* ptr = array)
					_SetFloatArray ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual void SetValues (double [] array, double time)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));

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
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));

			unsafe {
				fixed (float* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloatArray ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (double [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));

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
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf<Vector3> ();

			unsafe {
				fixed (Vector3* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetFloat3Array);
			}
		}

		public virtual void SetValues (Vector3d [] array, double time)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf<Vector3d> ();

			unsafe {
				fixed (Vector3d* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetDouble3Array);
			}
		}

		public virtual Vector3 [] GetNVector3Values (double time)
		{
			var count = ElementCount;
			var timesArr = new Vector3 [(int) count];

			unsafe {
				int typeSize = sizeof (Vector3);
				fixed (Vector3* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetFloat3Array);
			}

			return timesArr;
		}

		public virtual Vector3d [] GetNVector3dValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Vector3d [(int) count];

			unsafe {
				int typeSize = sizeof (Vector3d);
				fixed (Vector3d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetDouble3Array);
			}

			return timesArr;
		}

		public virtual void Reset (Vector3 [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf<Vector3> ();

			unsafe {
				fixed (Vector3* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithFloat3Array);
			}
		}

		public virtual void Reset (Vector3d [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf<Vector3d> ();

			unsafe {
				fixed (Vector3d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithDouble3Array);
			}
		}

		public virtual Vector3 [] GetNVector3Values ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Vector3 [(int) count];

			unsafe {
				int typeSize = sizeof (Vector3);
				fixed (Vector3* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat3Array);
			}

			return timesArr;
		}

		public virtual Vector3d [] GetNVector3dValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Vector3d [(int) count];

			unsafe {
				int typeSize = sizeof (Vector3d);
				fixed (Vector3d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble3Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedQuaternionArray {

		public virtual void SetValues (Quaternion [] array, double time)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf<Quaternion> ();

			unsafe {
				fixed (Quaternion* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetFloatQuaternionArray);
			}
		}

		public virtual void SetValues (Quaterniond [] array, double time)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf<Quaterniond> ();

			unsafe {
				fixed (Quaterniond* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, time, _SetDoubleQuaternionArray);
			}
		}

		public virtual Quaternion [] GetQuaternionValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Quaternion [(int) count];

			unsafe {
				int typeSize = sizeof (Quaternion);
				fixed (Quaternion* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetFloatQuaternionArray);
			}

			return timesArr;
		}

		public virtual Quaterniond [] GetQuaterniondValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Quaterniond [(int) count];

			unsafe {
				int typeSize = sizeof (Quaterniond);
				fixed (Quaterniond* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, time, _GetDoubleQuaternionArray);
			}

			return timesArr;
		}

		public virtual void Reset (Quaternion [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf<Quaternion> ();

			unsafe {
				fixed (Quaternion* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithFloatQuaternionArray);
			}
		}

		public virtual void Reset (Quaterniond [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			int typeSize = Marshal.SizeOf<Quaterniond> ();

			unsafe {
				fixed (Quaterniond* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, values.Length, times, _ResetWithDoubleQuaternionArray);
			}
		}

		public virtual Quaternion [] GetQuaternionValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Quaternion [(int) count];

			unsafe {
				int typeSize = sizeof (Quaternion);
				fixed (Quaternion* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloatQuaternionArray);
			}

			return timesArr;
		}

		public virtual Quaterniond [] GetQuaterniondValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Quaterniond [(int) count];
			unsafe {
				int typeSize = sizeof (Quaterniond);
				fixed (Quaterniond* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDoubleQuaternionArray);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedScalar {

		public virtual void Reset (float [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));

			unsafe {
				fixed (float* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloatArray ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (double [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));

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
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Vector2> ();

			unsafe {
				fixed (Vector2* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat2Array);
			}
		}

		public virtual void Reset (Vector2d [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Vector2d> ();

			unsafe {
				fixed (Vector2d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble2Array);
			}
		}

		public virtual Vector2 [] GetVector2Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector2 [(int) count];

			unsafe {
				int typeSize = sizeof (Vector2);
				fixed (Vector2* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat2Array);
			}

			return timesArr;
		}

		public virtual Vector2d [] GetVector2dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector2d [(int) count];

			unsafe {
				int typeSize = sizeof (Vector2d);
				fixed (Vector2d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble2Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedVector3 {

		public virtual void Reset (Vector3 [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Vector3> ();

			unsafe {
				fixed (Vector3* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat3Array);
			}
		}

		public virtual void Reset (Vector3d [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Vector3d> ();

			unsafe {
				fixed (Vector3d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble3Array);
			}
		}

		public virtual Vector3 [] GetNVector3Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector3 [(int) count];

			unsafe {
				int typeSize = sizeof (Vector3);
				fixed (Vector3* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat3Array);
			}

			return timesArr;
		}

		public virtual Vector3d [] GetNVector3dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector3d [(int) count];

			unsafe {
				int typeSize = sizeof (Vector3d);
				fixed (Vector3d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble3Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedVector4 {

		public virtual void Reset (Vector4 [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Vector4> ();

			unsafe {
				fixed (Vector4* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat4Array);
			}
		}

		public virtual void Reset (Vector4d [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Vector4d> ();

			unsafe {
				fixed (Vector4d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble4Array);
			}
		}

		public virtual Vector4 [] GetVector4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector4 [(int) count];

			unsafe {
				int typeSize = sizeof (Vector4);
				fixed (Vector4* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat4Array);
			}

			return timesArr;
		}

		public virtual Vector4d [] GetVector4dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector4d [(int) count];

			unsafe {
				int typeSize = sizeof (Vector4d);
				fixed (Vector4d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble4Array);
			}

			return timesArr;
		}
	}

	public partial class MDLAnimatedMatrix4x4 {

		public virtual void Reset (Matrix4 [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Matrix4> ();

			unsafe {
				fixed (Matrix4* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloat4x4Array);
			}
		}

		public virtual void Reset (Matrix4d [] values, double [] times)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			if (times is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new InvalidOperationException ($"The length of the '{nameof (values)}' array and the '{nameof (times)}' array must match.");
			int typeSize = Marshal.SizeOf<Matrix4d> ();

			unsafe {
				fixed (Matrix4d* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDouble4x4Array);
			}
		}

		public virtual Matrix4 [] GetNMatrix4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Matrix4 [(int) count];

			unsafe {
				int typeSize = sizeof (Matrix4);
				fixed (Matrix4* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat4x4Array);
			}

			return timesArr;
		}

		public virtual Matrix4d [] GetNMatrix4dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Matrix4d [(int) count];

			unsafe {
				int typeSize = sizeof (Matrix4d);
				fixed (Matrix4d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble4x4Array);
			}

			return timesArr;
		}
	}

	public partial class MDLMatrix4x4Array {

		public virtual void SetValues (Matrix4 [] array)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf<Matrix4> ();

			unsafe {
				fixed (Matrix4* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, _SetFloat4x4Array);
			}
		}

		public virtual void SetValues (Matrix4d [] array)
		{
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (array));
			int typeSize = Marshal.SizeOf<Matrix4d> ();

			unsafe {
				fixed (Matrix4d* arrptr = array)
					MDLMemoryHelper.SetValues (typeSize, (IntPtr) arrptr, array.Length, _SetDouble4x4Array);
			}
		}

		public virtual Matrix4 [] GetNMatrix4Values ()
		{
			var count = ElementCount;
			var timesArr = new Matrix4 [(int) count];

			unsafe {
				int typeSize = sizeof (Matrix4);
				fixed (Matrix4* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetFloat4x4Array);
			}

			return timesArr;
		}

		public virtual Matrix4d [] GetNMatrix4dValues ()
		{
			var count = ElementCount;
			var timesArr = new Matrix4d [(int) count];

			unsafe {
				int typeSize = sizeof (Matrix4d);
				fixed (Matrix4d* arrptr = timesArr)
					MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, count, _GetDouble4x4Array);
			}

			return timesArr;
		}
	}

	static class MDLMemoryHelper {
		// you must free the returned `ptr` (if non null) but use the `alignedPtr` for native calls
		static unsafe IntPtr GetAlignedPtrForArray (IntPtr arrptr, int size, bool copy, out IntPtr alignedPtr)
		{
			// use the original pointer if it's already aligned on a 16 bytes boundary
			if (((nuint) (ulong) arrptr & 15) == 0) {
				alignedPtr = arrptr;
				// nothing to free
				return IntPtr.Zero;
			}
			var ptr = Marshal.AllocHGlobal (size + 16);
			alignedPtr = new IntPtr (((nint) (ptr + 15) >> 4) << 4);
			if (copy)
				Buffer.MemoryCopy ((void*) arrptr, (void*) alignedPtr, size, size);
			return ptr;
		}

		internal static void SetValues (int typeSize, IntPtr arrptr, int arrLength, Action<IntPtr, nuint> setFunc)
		{
			var size = typeSize * arrLength;
			// get a 16 bytes aligned pointer for `arrptr` and call native code using it
			var allocated_ptr = GetAlignedPtrForArray (arrptr, size, copy: true, out var aligned_ptr);
			setFunc (aligned_ptr, (nuint) arrLength);
			// free the potentially allocated memory using to align memory
			if (allocated_ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (allocated_ptr);
		}

		internal static void SetValues (int typeSize, IntPtr arrptr, int arrLength, double time, Action<IntPtr, nuint, double> setFunc)
		{
			var size = typeSize * arrLength;
			// get a 16 bytes aligned pointer for `arrptr` and call native code using it
			var allocated_ptr = GetAlignedPtrForArray (arrptr, size, copy: true, out var aligned_ptr);
			setFunc (aligned_ptr, (nuint) arrLength, time);
			// free the potentially allocated memory using to align memory
			if (allocated_ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (allocated_ptr);
		}

		internal unsafe static nuint FetchValues (int typeSize, IntPtr arrptr, nuint count, Func<IntPtr, nuint, nuint> getFunc)
		{
			var size = typeSize * (int) count;
			// get a 16 bytes aligned pointer for `arrptr` and call native code using it
			var allocated_ptr = GetAlignedPtrForArray (arrptr, size, copy: false, out var aligned_ptr);
			var retVal = getFunc (aligned_ptr, count);
			if (allocated_ptr != IntPtr.Zero) {
				// move memory back to the original pointer, then free the aligned memory
				Buffer.MemoryCopy ((void*) aligned_ptr, (void*) arrptr, size, size);
				Marshal.FreeHGlobal (allocated_ptr);
			}
			return retVal;
		}

		internal unsafe static nuint FetchValues (int typeSize, IntPtr arrptr, nuint count, double time, Func<IntPtr, nuint, double, nuint> getFunc)
		{
			var size = typeSize * (int) count;
			// get a 16 bytes aligned pointer for `arrptr` and call native code using it
			var allocated_ptr = GetAlignedPtrForArray (arrptr, size, copy: false, out var aligned_ptr);
			var retVal = getFunc (aligned_ptr, count, time);
			if (allocated_ptr != IntPtr.Zero) {
				// move memory back to the original pointer, then free the aligned memory
				Buffer.MemoryCopy ((void*) aligned_ptr, (void*) arrptr, size, size);
				Marshal.FreeHGlobal (allocated_ptr);
			}
			return retVal;
		}

		internal static void Reset (int typeSize, IntPtr valuesPtr, double [] times, Action<IntPtr, IntPtr, nuint> resetFunc)
		{
			var size = typeSize * times.Length;
			// get a 16 bytes aligned pointer for `valuesPtr` and call native code using it
			var allocated_ptr = GetAlignedPtrForArray (valuesPtr, size, copy: true, out var aligned_ptr);
			unsafe {
				fixed (double* timesPtr = times)
					resetFunc (aligned_ptr, (IntPtr) timesPtr, (nuint) times.Length);
			}
			// free the potentially allocated memory using to align memory
			if (allocated_ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (allocated_ptr);
		}

		internal static void Reset (int typeSize, IntPtr valuesPtr, int valuesLength, double [] times, Action<IntPtr, nuint, IntPtr, nuint> resetFunc)
		{
			var size = typeSize * valuesLength;
			// get a 16 bytes aligned pointer for `valuesPtr` and call native code using it
			var allocated_ptr = GetAlignedPtrForArray (valuesPtr, size, copy: true, out var aligned_ptr);
			unsafe {
				fixed (double* timesPtr = times)
					resetFunc (aligned_ptr, (nuint) valuesLength, (IntPtr) timesPtr, (nuint) times.Length);
			}
			// free the potentially allocated memory using to align memory
			if (allocated_ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (allocated_ptr);
		}
	}
}
