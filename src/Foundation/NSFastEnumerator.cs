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
		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static nuint objc_msgSend (IntPtr receiver, IntPtr selector, ref NSFastEnumerationState arg1, IntPtr[] arg2, nuint arg3);
	}

	internal class NSFastEnumerator<T> : IEnumerator<T>
		where T: class, INativeObject
	{
		NSFastEnumerationState state;
		NSObject collection;
		IntPtr[] array;
		nuint count;
		IntPtr mutationValue;
		nuint current;
		bool started;

		public NSFastEnumerator (NSObject collection)
		{
			this.collection = collection;
		}

		void Fetch ()
		{
			if (array == null)
				array = new IntPtr [16];
			count = NSFastEnumerator.objc_msgSend (collection.Handle, Selector.GetHandle ("countByEnumeratingWithState:objects:count:"), ref state, array, (nuint) array.Length);
			if (!started) {
				started = true;
				mutationValue = Marshal.ReadIntPtr (state.mutationsPtr);
			}
			current = 0;
		}

		void VerifyNonMutated ()
		{
			if (mutationValue != Marshal.ReadIntPtr (state.mutationsPtr))
				throw new InvalidOperationException ("Collection was modified"); 
		}

#region IEnumerator implementation
		bool System.Collections.IEnumerator.MoveNext ()
		{
			if (array == null || current == count - 1) {
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
			state = new NSFastEnumerationState ();
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
			// Nothing to do
		}
#endregion

#region IEnumerator<T> implementation
		public T Current {
			get {
				return Runtime.GetINativeObject<T> (Marshal.ReadIntPtr (state.itemsPtr, IntPtr.Size * (int) current), false);
			}
		}
#endregion
	}
}
