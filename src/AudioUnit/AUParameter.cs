#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace AudioUnit {
	public partial class AUParameter {
		public string GetString (float? value)
		{
			unsafe {
				if (value is not null) {
					float f = value.Value;
					return this._GetString (new IntPtr (&f));
				} else {
					return this._GetString (IntPtr.Zero);
				}
			}
		}
	}
}
