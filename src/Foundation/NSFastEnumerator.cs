//
// This file contains a class used to consume NSFastEnumeration
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2015, Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {
	internal class NSFastEnumerator {
		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public unsafe extern static nuint objc_msgSend (IntPtr receiver, IntPtr selector, NSFastEnumerationState* arg1, IntPtr* arg2, nuint arg3);
	}

	internal class NSFastEnumerator<T> : IEnumerator<T>
		where T : class, INativeObject {
		unsafe NSFastEnumerationState* state;
		NSObject collection;
		nuint count;
		IntPtr mutationValue;
		nuint current;
		bool started;

		public NSFastEnumerator (NSObject collection)
		{
			this.collection = collection;

			unsafe {
				// Create one blob of native memory that holds both our NSFastEnumerationState and the array of pointers we pass to the enumerator.
				//
				// Note that we *must* pass native memory to the countByEnumeratingWithState:objects:count: method
				// (and not a field on the NSFastEnumerator instance), because:
				// * The pointers in the state (NSFastEnumerationState.mutationsPtr / NSFastEnumerationState.itemsPtr) might point back into the structure.
				// * We access those pointers using unsafe code (in a way the GC doesn't see).
				// * If the GC happens to move the NSFastEnumerator instance in memory, it won't update these pointers.
				// * The next time we read these pointers, we'll read random memory, and thus get random results.
				// * Ref: https://github.com/xamarin/maccore/issues/2606.
				// * It would probably also work to create a pinned GCHandle to the NSFastEnumerator structure (instead of allocating native memory), but that doesn't seem easier on the GC.
				state = (NSFastEnumerationState*) Marshal.AllocHGlobal (sizeof (NSFastEnumerationState));
				// Zero-initialize
				*state = default (NSFastEnumerationState);
			}
		}

		void Fetch ()
		{
			unsafe {
				count = NSFastEnumerator.objc_msgSend (collection.Handle, Selector.GetHandle ("countByEnumeratingWithState:objects:count:"), state, &state->array1, (nuint) NSFastEnumerationState.ArrayLength);
				if (!started) {
					started = true;
					mutationValue = *state->mutationsPtr;
				}
			}
			current = 0;
		}

		unsafe void VerifyNonMutated ()
		{
			if (mutationValue != *state->mutationsPtr)
				throw new InvalidOperationException ("Collection was modified");
		}

		#region IEnumerator implementation
		bool System.Collections.IEnumerator.MoveNext ()
		{
			if (!started || current == count - 1) {
				Fetch ();
				if (count == 0)
					return false;
			} else {
				current++;
			}
			VerifyNonMutated ();
			return true;
		}

		void System.Collections.IEnumerator.Reset ()
		{
			unsafe {
				*state = new NSFastEnumerationState ();
			}
			started = false;
		}

		object System.Collections.IEnumerator.Current {
			get {
				VerifyNonMutated ();
				return Current;
			}
		}
		#endregion

		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			unsafe {
				Marshal.FreeHGlobal ((IntPtr) state);
				state = null;
			}
		}
		#endregion

		#region IEnumerator<T> implementation
		public unsafe T Current {
			get {
				IntPtr ptr;
				unsafe {
					ptr = state->itemsPtr [(int) current];
				}
				return Runtime.GetINativeObject<T> (ptr, false);
			}
		}
		#endregion
	}
}
