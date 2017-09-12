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
using XamCore.Foundation;

using Vector2 = global::Simd.VectorFloat2;
using Vector2d = global::Simd.VectorDouble2;
using Vector3 = global::Simd.VectorFloat3;
using Vector3d = global::Simd.VectorDouble3;
using Vector4 = global::Simd.VectorFloat4;
using Vector4d = global::Simd.VectorDouble4;
using Matrix4 = global::Simd.MatrixFloat4x4;
using Matrix4d = global::Simd.MatrixDouble4x4;
using Quaternion = global::OpenTK.Quaternion;
using Quaterniond = global::OpenTK.Quaterniond;

// Because of the lack of documentation when at the time of binding ModelIO for iOS 11 types
// we are implementing something similar to what swift is doing here
// https://github.com/apple/swift/blob/cbdf0ff1e7bfbd192c33d64c9c7d31fbb11f712c/stdlib/public/SDK/ModelIO/ModelIO.swift

namespace XamCore.ModelIO {
	public partial class MDLAnimatedValue {

		public double [] KeyTimes {
			get {
				if (WeakKeyTimes == null)
					return null;

				var count = WeakKeyTimes.Length;
				var ret = new double [count];

				for (int i = 0; i < count; i++)
					ret [i] = WeakKeyTimes [i].DoubleValue;

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

			unsafe {
				fixed (Vector3* ptr = array)
					_SetFloat3Array ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual void SetValues (Vector3d [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (Vector3d* ptr = array)
					_SetDouble3Array ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual Vector3 [] GetVector3Values (double time)
		{
			var count = ElementCount;
			var timesArr = new Vector3 [(int) count];

			unsafe {
				fixed (Vector3* ptr = timesArr)
					_GetFloat3Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Vector3d [] GetVector3dValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Vector3d [(int) count];

			unsafe {
				fixed (Vector3d* ptr = timesArr)
					_GetDouble3Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual void Reset (Vector3 [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Vector3* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloat3Array ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (Vector3d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Vector3d* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDouble3Array ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual Vector3 [] GetVector3Values ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Vector3 [(int) count];

			unsafe {
				fixed (Vector3* ptr = timesArr)
					_GetFloat3Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Vector3d [] GetVector3dValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Vector3d [(int) count];

			unsafe {
				fixed (Vector3d* ptr = timesArr)
					_GetDouble3Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}
	}

	public partial class MDLAnimatedQuaternionArray {

		public virtual void SetValues (Quaternion [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (Quaternion* ptr = array)
					_SetFloatQuaternionArray ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual void SetValues (Quaterniond [] array, double time)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (Quaterniond* ptr = array)
					_SetDoubleQuaternionArray ((IntPtr) ptr, (nuint) array.Length, time);
			}
		}

		public virtual Quaternion [] GetQuaternionValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Quaternion [(int) count];

			unsafe {
				fixed (Quaternion* ptr = timesArr)
					_GetFloatQuaternionArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Quaterniond [] GetQuaterniondValues (double time)
		{
			var count = ElementCount;
			var timesArr = new Quaterniond [(int) count];

			unsafe {
				fixed (Quaterniond* ptr = timesArr)
					_GetDoubleQuaternionArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual void Reset (Quaternion [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Quaternion* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloatQuaternionArray ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (Quaterniond [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Quaterniond* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDoubleQuaternionArray ((IntPtr) valuesPtr, (nuint) values.Length, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual Quaternion [] GetVector3Values ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Quaternion [(int) count];

			unsafe {
				fixed (Quaternion* ptr = timesArr)
					_GetFloatQuaternionArray ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Quaterniond [] GetVector3dValues ()
		{
			var count = ElementCount * TimeSampleCount;
			var timesArr = new Quaterniond [(int) count];

			unsafe {
				fixed (Quaterniond* ptr = timesArr)
					_GetDoubleQuaternionArray ((IntPtr) ptr, count);
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
					_GetFloatArray ((IntPtr) ptr, count);
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

			unsafe {
				fixed (Vector2* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloat2Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (Vector2d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Vector2d* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDouble2Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual Vector2 [] GetVector2Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector2 [(int) count];

			unsafe {
				fixed (Vector2* ptr = timesArr)
					_GetFloat2Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Vector2d [] GetVector2dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector2d [(int) count];

			unsafe {
				fixed (Vector2d* ptr = timesArr)
					_GetDouble2Array ((IntPtr) ptr, count);
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

			unsafe {
				fixed (Vector3* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloat3Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (Vector3d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Vector3d* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDouble3Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual Vector3 [] GetVector3Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector3 [(int) count];

			unsafe {
				fixed (Vector3* ptr = timesArr)
					_GetFloat3Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Vector3d [] GetVector3dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector3d [(int) count];

			unsafe {
				fixed (Vector3d* ptr = timesArr)
					_GetDouble3Array ((IntPtr) ptr, count);
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

			unsafe {
				fixed (Vector4* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloat4Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (Vector4d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Vector4d* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDouble4Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual Vector4 [] GetVector4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector4 [(int) count];

			unsafe {
				fixed (Vector4* ptr = timesArr)
					_GetFloat4Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Vector4d [] GetVector4dValues ()
		{
			var count = TimeSampleCount;
			var timesArr = new Vector4d [(int) count];

			unsafe {
				fixed (Vector4d* ptr = timesArr)
					_GetDouble4Array ((IntPtr) ptr, count);
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

			unsafe {
				fixed (Matrix4* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithFloat4x4Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual void Reset (Matrix4d [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));

			unsafe {
				fixed (Matrix4d* valuesPtr = values)
				fixed (double* timesPtr = times)
					_ResetWithDouble4x4Array ((IntPtr) valuesPtr, (IntPtr) timesPtr, (nuint) times.Length);
			}
		}

		public virtual Matrix4 [] GetMatrixFloat4x4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Matrix4 [(int) count];

			unsafe {
				fixed (Matrix4* ptr = timesArr)
					_GetFloat4x4Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Matrix4d [] GetMatrixDouble4x4Values ()
		{
			var count = TimeSampleCount;
			var timesArr = new Matrix4d [(int) count];

			unsafe {
				fixed (Matrix4d* ptr = timesArr)
					_GetDouble4x4Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}
	}

	public partial class MDLMatrix4x4Array {

		public virtual void SetValues (Matrix4 [] array)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (Matrix4* ptr = array)
					_SetFloat4x4Array ((IntPtr) ptr, (nuint) array.Length);
			}
		}

		public virtual void SetValues (Matrix4d [] array)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));

			unsafe {
				fixed (Matrix4d* ptr = array)
					_SetDouble4x4Array ((IntPtr) ptr, (nuint) array.Length);
			}
		}

		public virtual Matrix4 [] GetMatrixFloat4x4Values ()
		{
			var count = ElementCount;
			var timesArr = new Matrix4 [(int) count];

			unsafe {
				fixed (Matrix4* ptr = timesArr)
					_GetFloat4x4Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}

		public virtual Matrix4d [] GetMatrixDouble4x4Values ()
		{
			var count = ElementCount;
			var timesArr = new Matrix4d [(int) count];

			unsafe {
				fixed (Matrix4d* ptr = timesArr)
					_GetDouble4x4Array ((IntPtr) ptr, count);
			}
			return timesArr;
		}
	}
}
#endif