//
// SCNNode.cs: extensions to SCNNode
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)   
//
// Copyright Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;

namespace SpriteKit
{
#if XAMCORE_2_0 || !MONOMAC
	public partial class SKNode : IEnumerable, IEnumerable<SKNode>
	{
		[iOS (8,0), Mac (10,10)]
		public static T FromFile<T> (string file) where T : SKNode
		{
			IntPtr handle;
			using (var s = new NSString (file))
				handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle(typeof(T)), Selector.GetHandle ("nodeWithFileNamed:"), s.Handle);
			return Runtime.GetNSObject<T> (handle) ;
		}

		public void Add (SKNode node)
		{
			AddChild (node);
		}

		public void AddNodes (params SKNode [] nodes)
		{
			if (nodes == null)
				return;
			foreach (var n in nodes)
				AddChild (n);
		}
		
		public IEnumerator<SKNode> GetEnumerator ()
		{
			foreach (var node in Children)
				yield return node;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
#endif
}
