//
// SCNScene.cs: extensions to SCNScene
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)   
//
// Copyright Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

#nullable enable

namespace SceneKit
{
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class SCNScene : IEnumerable<SCNNode>
	{
		public void Add (SCNNode node)
		{
			RootNode.AddChildNode (node);
		}

		public IEnumerator<SCNNode> GetEnumerator ()
		{
			return RootNode.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}
