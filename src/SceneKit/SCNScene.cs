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

#nullable enable

namespace SceneKit {
	public partial class SCNScene : IEnumerable<SCNNode> {
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
