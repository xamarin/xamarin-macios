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

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern IntPtr _Block_copy (IntPtr ptr);

		protected unsafe TrampolineBlockBase (BlockLiteral *block)
		{
			blockPtr = _Block_copy ((IntPtr) block);
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
			return ((BlockLiteral *) block)->Target;
		}
	}
}
