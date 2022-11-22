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
using ObjCRuntime;

#if NET
using Vector2 = global::System.Numerics.Vector2;
#else
using Vector2 = global::OpenTK.Vector2;
#endif

#nullable enable

namespace SpriteKit {
	public partial class SKWarpGeometryGrid {
		public unsafe static SKWarpGeometryGrid Create (nint cols, nint rows, Vector2 [] sourcePositions, Vector2 [] destPositions)
		{
			if (cols < 1 || rows < 1)
				throw new InvalidOperationException ("cols and rows should be >= 1");

			if (sourcePositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("sourcePositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			if (destPositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("destPositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			fixed (Vector2* source_ptr = sourcePositions)
			fixed (Vector2* dest_ptr = destPositions)
				return GridWithColumns (cols, rows, (IntPtr) source_ptr, (IntPtr) dest_ptr);
		}

		[DesignatedInitializer]
		public unsafe SKWarpGeometryGrid (nint cols, nint rows, Vector2 [] sourcePositions, Vector2 [] destPositions)
		{
			if (cols < 1 || rows < 1)
				throw new InvalidOperationException ("cols and rows should be >= 1");

			if (sourcePositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("sourcePositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			if (destPositions.Length < ((cols + 1) * (rows + 1)))
				throw new InvalidOperationException ("destPositions should have a minimum lenght of (cols + 1) * (rows + 1)");

			fixed (Vector2* source_ptr = sourcePositions)
			fixed (Vector2* dest_ptr = destPositions)
				InitializeHandle (InitWithColumns (cols, rows, (IntPtr) source_ptr, (IntPtr) dest_ptr), "initWithColumns:rows:sourcePositions:destPositions:");
		}

		public unsafe SKWarpGeometryGrid GetGridByReplacingSourcePositions (Vector2 [] sourcePositions)
		{
			if (sourcePositions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourcePositions));
			// TODO: Verify this assumption when/if doc is updated or headers changed in newer betas.
			if (sourcePositions.Length < ((NumberOfColumns + 1) * (NumberOfRows + 1)))
				throw new InvalidOperationException ("sourcePositions should have a minimum lenght of (NumberOfColumns + 1) * (NumberOfRows + 1)");

			fixed (Vector2* ptr = sourcePositions)
				return _GridByReplacingSourcePositions ((IntPtr) ptr);
		}

		public unsafe SKWarpGeometryGrid GetGridByReplacingDestPositions (Vector2 [] destPositions)
		{
			if (destPositions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destPositions));
			// TODO: Verify this assumption when/if doc is updated or headers changed in newer betas.
			if (destPositions.Length < ((NumberOfColumns + 1) * (NumberOfRows + 1)))
				throw new InvalidOperationException ("destPositions should have a minimum lenght of (NumberOfColumns + 1) * (NumberOfRows + 1)");

			fixed (Vector2* ptr = destPositions)
				return _GridByReplacingDestPositions ((IntPtr) ptr);
		}
	}
}
