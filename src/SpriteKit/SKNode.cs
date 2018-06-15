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

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		public static SKNode Create (string filename, Type [] types, out NSError error)
		{
			// Let's fail early.
			if (filename == null)
				throw new ArgumentNullException (nameof (filename));
			if (types == null)
				throw new ArgumentNullException (nameof (filename));
			if (types.Length == 0)
				throw new InvalidOperationException ($"'{nameof (filename)}' length must be greater than zero.");

			using (var classes = new NSMutableSet<Class> (types.Length)) {
				foreach (var type in types)
					classes.Add (new Class (type));
				return Create (filename, classes.Handle, out error);
			}
		}

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		public static SKNode Create (string filename, NSSet<Class> classes, out NSError error) => Create (filename, classes.Handle, out error);
	}
#endif
}
