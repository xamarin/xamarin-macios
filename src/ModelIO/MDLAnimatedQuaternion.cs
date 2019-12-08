//
// MDLAnimatedQuaternion.cs
//
// Authors:
//	Rolf Bjarne Kvinge <rolf.kvinge@microsoft.com>
//
// Copyright 2019 Microsoft Corp. All rights reserved.
//

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

namespace ModelIO {
	public partial class MDLAnimatedQuaternion {
		public virtual void Reset (Quaternion [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new ArgumentOutOfRangeException ($"The '{nameof (values)}' and '{nameof (times)}' arrays must have the same length");
			int typeSize = Marshal.SizeOf (typeof (Quaternion));

			unsafe {
				fixed (Quaternion* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithFloatQuaternionArray);
			}
		}

		public virtual void Reset (Quaterniond [] values, double [] times)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			if (times == null)
				throw new ArgumentNullException (nameof (times));
			if (values.Length != times.Length)
				throw new ArgumentOutOfRangeException ($"The '{nameof (values)}' and '{nameof (times)}' arrays must have the same length");

			int typeSize = Marshal.SizeOf (typeof (Quaterniond));

			unsafe {
				fixed (Quaterniond* valuesPtr = values)
					MDLMemoryHelper.Reset (typeSize, (IntPtr) valuesPtr, times, _ResetWithDoubleQuaternionArray);
			}
		}

		public virtual Quaternion [] GetQuaternionValues (nuint maxCount)
		{
			var timesArr = new Quaternion [(int) maxCount];

			unsafe {
				int typeSize = sizeof (Quaternion);
				fixed (Quaternion* arrptr = timesArr) {
					var rv = MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, maxCount, _GetFloatQuaternionArray);
					Array.Resize (ref timesArr, (int) rv);
				}
			}

			return timesArr;
		}

		public virtual Quaterniond [] GetQuaterniondValues (nuint maxCount)
		{
			var timesArr = new Quaterniond [(int) maxCount];

			unsafe {
				int typeSize = sizeof (Quaterniond);
				fixed (Quaterniond* arrptr = timesArr) {
					var rv = MDLMemoryHelper.FetchValues (typeSize, (IntPtr) arrptr, maxCount, _GetDoubleQuaternionArray);
					Array.Resize (ref timesArr, (int) rv);
				}
			}

			return timesArr;
		}
	}
}
