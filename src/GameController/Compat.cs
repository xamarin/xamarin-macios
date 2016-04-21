//
// Compat.cs: Stuff we won't provide in Xamarin.iOS.dll
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.

#if !XAMCORE_2_0

using System;

namespace MonoTouch.GameController {

	public partial class GCControllerElement {
		[Obsolete ("Cannot create a default instance")]
		public GCControllerElement () {}
	}

	public partial class GCControllerAxisInput {
		[Obsolete ("Cannot create a default instance")]
		public GCControllerAxisInput () {}
	}

	public partial class GCControllerButtonInput {
		[Obsolete ("Cannot create a default instance")]
		public GCControllerButtonInput () {}
	}

	public partial class GCControllerDirectionPad {
		[Obsolete ("Cannot create a default instance")]
		public GCControllerDirectionPad () {}
	}

	public partial class GCGamepad {
		[Obsolete ("Cannot create a default instance")]
		public GCGamepad () {}
	}

	public partial class GCExtendedGamepad {
		[Obsolete ("Cannot create a default instance")]
		public GCExtendedGamepad () {}
	}
}

#endif
