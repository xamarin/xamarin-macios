//
// IDynamicRegistrar.cs:
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 - 2014 Xamarin Inc. 
//

using System;
using System.Collections.Generic;
using System.Reflection;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Registrar {
#if !COREBUILD
	class LazyMapEntry {
		public string Typename;
		public bool IsCustomType;
	}
#endif
}

