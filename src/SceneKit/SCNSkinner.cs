//
// SCNSkinner.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace SceneKit {

	public partial class SCNSkinner {

		static SCNMatrix4 [] FromNSArray (NSArray nsa)
		{
			if (nsa == null)
				return null;

			var count = nsa.Count;
			var ret = new SCNMatrix4 [count];
			for (nuint i = 0; i < count; i++)
				ret [i] = Runtime.GetNSObject<NSValue> (nsa.ValueAt (i)).SCNMatrix4Value;

			return ret;
		}

		static NSArray ToNSArray (SCNMatrix4 [] items)
		{
			if (items == null)
				return new NSArray ();

			var count = items.Length;
			var buf = Marshal.AllocHGlobal ((IntPtr)(count * IntPtr.Size));

			for (nint i = 0; i < count; i++) {
				var item = NSValue.FromSCNMatrix4 (items [i]);
				var h = item == null ? NSNull.Null.Handle : item.Handle;
				Marshal.WriteIntPtr (buf, (int)(i * IntPtr.Size), h);
			}

			var nsa = new NSArray (NSArray.FromObjects (buf, count));
			Marshal.FreeHGlobal (buf);

			return nsa;
		}

		[Mac (10, 10)]
		[iOS (8, 0)]
		public SCNMatrix4 [] BoneInverseBindTransforms {
			get { return FromNSArray (_BoneInverseBindTransforms); }
		}

		[Mac (10, 10)]
		[iOS (8, 0)]
		public static SCNSkinner Create (SCNGeometry baseGeometry,
			SCNNode [] bones, SCNMatrix4 [] boneInverseBindTransforms,
			SCNGeometrySource boneWeights, SCNGeometrySource boneIndices)
		{
			return _Create (
				baseGeometry,
				bones,
				ToNSArray (boneInverseBindTransforms),
				boneWeights,
				boneIndices
			);
		}
	}
}
