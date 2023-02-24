// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ObjCRuntime {

	// infrastucture code - not intended to be used directly by user code
	[EditorBrowsable (EditorBrowsableState.Never)]
	public abstract class TrampolineBlockBase {

		readonly IntPtr blockPtr;

		internal TrampolineBlockBase (IntPtr block, bool owns)
		{
			if (owns) {
				blockPtr = block;
			} else {
				blockPtr = BlockLiteral.Copy (block);
			}
		}

		protected unsafe TrampolineBlockBase (BlockLiteral* block)
			: this ((IntPtr) block, false)
		{
		}

		~TrampolineBlockBase ()
		{
			Runtime.ReleaseBlockOnMainThread (blockPtr);
		}

		protected IntPtr BlockPointer {
			get { return blockPtr; }
		}

		protected unsafe static object GetExistingManagedDelegate (IntPtr block)
		{
			if (!BlockLiteral.IsManagedBlock (block))
				return null;
			return ((BlockLiteral*) block)->Target;
		}
	}
}
