//
// SKWarpGeometryGrid.cs: SKWarpGeometryGrid class
//
// Authors:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2016 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;

using Vector2 = global::OpenTK.Vector2;

#if XAMCORE_2_0 || !MONOMAC
namespace XamCore.SpriteKit
{
	public partial class SKWarpGeometryGrid
	{
		public static SKWarpGeometryGrid Create (nint cols, nint rows, Vector2 [] sourcePositions, Vector2 [] destPositions)
		{
			if (cols < 1 || rows < 1)
				throw new InvalidOperationException ("cols and rows should be >= 1");

			if (sourcePositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("sourcePositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			if (destPositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("destPositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			IntPtr spPtr = IntPtr.Zero;
			GCHandle spGch = new GCHandle ();
			if (sourcePositions != null) {
				spGch = GCHandle.Alloc (sourcePositions, GCHandleType.Pinned);
				spPtr = spGch.AddrOfPinnedObject ();
			}

			IntPtr dpPtr = IntPtr.Zero;
			GCHandle dpGch = new GCHandle ();
			if (destPositions != null) {
				dpGch = GCHandle.Alloc (destPositions, GCHandleType.Pinned);
				dpPtr = dpGch.AddrOfPinnedObject ();
			}

			var grid = GridWithColumns (cols, rows, spPtr, dpPtr);

			spGch.Free ();
			dpGch.Free ();

			return grid;
		}

		[DesignatedInitializer]
		public SKWarpGeometryGrid (nint cols, nint rows, Vector2 [] sourcePositions, Vector2 [] destPositions)
		{
			if (cols < 1 || rows < 1)
				throw new InvalidOperationException ("cols and rows should be >= 1");

			if (sourcePositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("sourcePositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			if (destPositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("destPositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			IntPtr spPtr = IntPtr.Zero;
			GCHandle spGch = new GCHandle ();
			if (sourcePositions != null) {
				spGch = GCHandle.Alloc (sourcePositions, GCHandleType.Pinned);
				spPtr = spGch.AddrOfPinnedObject ();
			}

			IntPtr dpPtr = IntPtr.Zero;
			GCHandle dpGch = new GCHandle ();
			if (destPositions != null) {
				dpGch = GCHandle.Alloc (destPositions, GCHandleType.Pinned);
				dpPtr = dpGch.AddrOfPinnedObject ();
			}

			InitializeHandle (InitWithColumns (cols, rows, spPtr, dpPtr), "initWithColumns:rows:sourcePositions:destPositions:");

			spGch.Free ();
			dpGch.Free ();
		}

		public SKWarpGeometryGrid GetGridByReplacingSourcePositions (Vector2 [] sourcePositions)
		{
			if (sourcePositions == null)
				throw new ArgumentNullException ("sourcePositions");
			// TODO: Verify this assumption when/if doc is updated or headers changed in newer betas.
			if (sourcePositions.Length < ((NumberOfColumns + 1) * (NumberOfRows + 1)))
				throw new InvalidOperationException ("sourcePositions should have a minimum lenght of (NumberOfColumns + 1) * (NumberOfRows + 1)");

			var gch = GCHandle.Alloc (sourcePositions, GCHandleType.Pinned);
			var ptr = gch.AddrOfPinnedObject ();

			var grid = _GridByReplacingSourcePositions (ptr);

			gch.Free ();

			return grid;
		}

		public SKWarpGeometryGrid GetGridByReplacingDestPositions (Vector2 [] destPositions)
		{
			if (destPositions == null)
				throw new ArgumentNullException ("destPositions");
			// TODO: Verify this assumption when/if doc is updated or headers changed in newer betas.
			if (destPositions.Length < ((NumberOfColumns + 1) * (NumberOfRows + 1)))
				throw new InvalidOperationException ("destPositions should have a minimum lenght of (NumberOfColumns + 1) * (NumberOfRows + 1)");

			var gch = GCHandle.Alloc (destPositions, GCHandleType.Pinned);
			var ptr = gch.AddrOfPinnedObject ();

			var grid = _GridByReplacingDestPositions (ptr);

			gch.Free ();

			return grid;
		}
	}
}
#endif // XAMCORE_2_0
