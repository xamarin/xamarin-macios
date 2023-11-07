using System;

using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#endif
using CoreGraphics;

#nullable enable

namespace CoreAnimation {
	public partial class CABasicAnimation {
		public T GetFromAs<T> () where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (_From, false)!;
		}

		public void SetFrom (INativeObject value)
		{
			_From = value.Handle;
		}

		public T GetToAs<T> () where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (_To, false)!;
		}

		public void SetTo (INativeObject value)
		{
			_To = value.Handle;
		}

		public T GetByAs<T> () where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (_By, false)!;
		}

		public void SetBy (INativeObject value)
		{
			_By = value.Handle;
		}
	}
}
