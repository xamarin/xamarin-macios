//
// SCNTechnique.cs: extensions to SCNTechnique
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)   
//
// Copyright Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using System.Runtime.Versioning;

#nullable enable

namespace SceneKit
{
#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class SCNTechnique 
	{
		public NSObject? this[NSString key] {
			get { return _GetObject (key); }
			set { _SetObject (value, key); }
		}
	}
}
