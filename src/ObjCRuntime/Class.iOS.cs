//
// Class.cs
//
// Copyright 2009 Novell, Inc
// Copyright 2011 Xamarin Inc. All rights reserved.
//

#if !MONOMAC

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XamCore.Foundation;

namespace XamCore.ObjCRuntime {
	public partial class Class : INativeObject {
#if !COREBUILD
#if !XAMCORE_2_0
		// This method is used internally by the -mapinject feature in mtouch.
		[System.ComponentModel.EditorBrowsable (System.ComponentModel.EditorBrowsableState.Never)]
		public static void RegisterMethods (Type type, Dictionary<IntPtr, MethodDescription> methods)
		{
			Runtime.Registrar.RegisterMethods (type, methods);
		}
#endif
#endif // !COREBUILD
	}
}

#endif // !MONOMAC
