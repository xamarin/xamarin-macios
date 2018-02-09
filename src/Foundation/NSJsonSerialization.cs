//
// NSJsonSerialization.cs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014, Xamarin Inc.
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {

	public partial class NSJsonSerialization {

#if !XAMCORE_2_0
		[Obsolete ("Use the 'Deserialize(NSData,NSJsonReadingOptions,out NSError)' overload instead.")]
		public static NSObject Deserialize (NSData data, NSJsonReadingOptions opt, NSError error)
		{
			return Deserialize (data, opt, out error);
		}
#endif
	}
}
