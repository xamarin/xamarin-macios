//
// SCNPhysicsShape.cs: extensions to SCNPhysicsShape
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

namespace SceneKit
{
	public partial class SCNPhysicsShape
	{
		public static void NewNonBreakingAPI ()
		{
		}

		public static SCNPhysicsShape Create (SCNPhysicsShape [] shapes, SCNMatrix4 [] transforms)
		{
			if (shapes == null)
				throw new ArgumentNullException ("shapes");

			if (transforms == null)
				throw new ArgumentNullException ("transforms");

			var t = new NSValue [transforms.Length];
			for (var i = 0; i < t.Length; i++)
				t [i] = NSValue.FromSCNMatrix4 (transforms [i]);

			return Create (shapes, t);
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the 'Create' method that takes a 'SCNMatrix4 []'.")]
		public static SCNPhysicsShape Create (SCNPhysicsShape [] shapes, SCNVector3 [] transforms)
		{
			if (shapes == null)
				throw new ArgumentNullException ("shapes");

			if (transforms == null)
				throw new ArgumentNullException ("transforms");

			var t = new NSValue [transforms.Length];
			for (var i = 0; i < t.Length; i++)
				t [i] = NSValue.FromVector (transforms [i]);

			return Create (shapes, t);
		}
#endif

		public static SCNPhysicsShape Create (SCNGeometry geometry,
			SCNPhysicsShapeType? shapeType = null,
			bool? keepAsCompound = null,
			SCNVector3? scale = null)
		{
			return Create (geometry, new SCNPhysicsShapeOptions {
				ShapeType = shapeType,
				KeepAsCompound = keepAsCompound,
				Scale = scale
			}.ToDictionary ());
		}

		public static SCNPhysicsShape Create (SCNGeometry geometry, SCNPhysicsShapeOptions options)
		{
			return Create (geometry, options.ToDictionary ());
		}

		public static SCNPhysicsShape Create (SCNNode node,
			SCNPhysicsShapeType? shapeType = null,
			bool? keepAsCompound = null,
			SCNVector3? scale = null)
		{
			return Create (node, new SCNPhysicsShapeOptions {
				ShapeType = shapeType,
				KeepAsCompound = keepAsCompound,
				Scale = scale
			}.ToDictionary ());
		}

		public static SCNPhysicsShape Create (SCNNode node, SCNPhysicsShapeOptions options)
		{
			return Create (node, options.ToDictionary ());
		}

		public SCNPhysicsShapeOptions Options {
			get {
				var o = _Options;
				if (o == null)
					return null;
				return new SCNPhysicsShapeOptions (o);
			}
		}
	}

	[Mac (10, 10)]
	[iOS (8, 0)]
	public enum SCNPhysicsShapeType
	{
		ConvexHull,
		BoundingBox,
		ConcavePolyhedron
	}

	[Mac (10, 10)]
	[iOS (8, 0)]
	public class SCNPhysicsShapeOptions
	{
		public SCNPhysicsShapeType? ShapeType { get; set; }
		public bool? KeepAsCompound { get; set; }
		public SCNVector3? Scale { get; set; }

		public SCNPhysicsShapeOptions () {}
		
		internal SCNPhysicsShapeOptions (NSDictionary source)
		{
			var ret = source [SCNPhysicsShapeOptionsKeys.Type] as NSString;
			if (ret != null){
				if (ret == SCNPhysicsShapeOptionsTypes.BoundingBox)
					ShapeType = SCNPhysicsShapeType.BoundingBox;
				else if (ret == SCNPhysicsShapeOptionsTypes.ConcavePolyhedron)
					ShapeType = SCNPhysicsShapeType.ConcavePolyhedron;
				else if (ret == SCNPhysicsShapeOptionsTypes.ConvexHull)
					ShapeType = SCNPhysicsShapeType.ConvexHull;
			}
			var bret = source [SCNPhysicsShapeOptionsKeys.KeepAsCompound] as NSNumber;
			if (bret != null)
				KeepAsCompound = bret.Int32Value != 0;
			var nret = source [SCNPhysicsShapeOptionsKeys.Scale] as NSValue;
			if (nret != null)
				Scale = nret.Vector3Value;
		}
		
		public NSDictionary ToDictionary ()
		{
			var n = 0;
			if (ShapeType.HasValue) n++;
			if (KeepAsCompound.HasValue) n++;
			if (Scale.HasValue) n++;

			if (n == 0)
				return null;

			var i = 0;
			var keys = new NSString [n];
			var values = new NSObject [n];

			if (ShapeType.HasValue) {
				keys [i] = SCNPhysicsShapeOptionsKeys.Type;
				switch (ShapeType.Value) {
				case SCNPhysicsShapeType.BoundingBox:
					values [i] = SCNPhysicsShapeOptionsTypes.BoundingBox;
					break;
				case SCNPhysicsShapeType.ConcavePolyhedron:
					values [i] = SCNPhysicsShapeOptionsTypes.ConcavePolyhedron;
					break;
				case SCNPhysicsShapeType.ConvexHull:
				default:
					values [i] = SCNPhysicsShapeOptionsTypes.ConvexHull;
					break;
				}
			}

			if (KeepAsCompound.HasValue) {
				keys [i] = SCNPhysicsShapeOptionsKeys.KeepAsCompound;
				values [i] = new NSNumber (KeepAsCompound.Value);
			}

			if (Scale.HasValue) {
				keys [i] = SCNPhysicsShapeOptionsKeys.Scale;
				values [i] = NSValue.FromVector (Scale.Value);
			}

			return NSDictionary.FromObjectsAndKeys (values, keys);
		}
	}
}
