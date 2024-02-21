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

#nullable enable

namespace SceneKit {

	public partial class SCNSkinner {

		static SCNMatrix4 []? FromNSArray (NSArray? nsa)
		{
			if (nsa is null)
				return null;

			var count = nsa.Count;
			var ret = new SCNMatrix4 [count];
			for (nuint i = 0; i < count; i++)
				ret [i] = Runtime.GetNSObject<NSValue> (nsa.ValueAt (i))!.SCNMatrix4Value;

			return ret;
		}

		static NSArray ToNSArray (SCNMatrix4 []? items)
		{
			if (items is null)
				return new NSArray ();

			var count = items.Length;
			var buf = Marshal.AllocHGlobal ((IntPtr) (count * IntPtr.Size));

			for (nint i = 0; i < count; i++) {
				var item = NSValue.FromSCNMatrix4 (items [i]);
				var h = item?.Handle ?? NSNull.Null.Handle;
				Marshal.WriteIntPtr (buf, (int) (i * IntPtr.Size), h);
			}

			var nsa = new NSArray (NSArray.FromObjects (buf, count));
			Marshal.FreeHGlobal (buf);

			return nsa;
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SCNMatrix4 []? BoneInverseBindTransforms {
			get { return FromNSArray (_BoneInverseBindTransforms); }
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
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
