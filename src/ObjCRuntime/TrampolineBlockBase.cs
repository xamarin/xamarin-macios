// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {

	public abstract class TrampolineBlockBase {
		protected IntPtr blockPtr;

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr _Block_copy (IntPtr ptr);

		protected unsafe TrampolineBlockBase (BlockLiteral *block)
		{
			blockPtr = _Block_copy ((IntPtr) block);
		}

		~TrampolineBlockBase ()
		{
			Runtime.ReleaseBlockOnMainThread (blockPtr);
		}

		protected unsafe static object GetExistingManagedDelegate (IntPtr block)
		{
			if (!BlockLiteral.IsManagedBlock (block))
				return null;
			return ((BlockLiteral *) block)->Target;
		}
	}
}
