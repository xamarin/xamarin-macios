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

#if !WATCH
using CoreAnimation;
#endif
using Foundation;

namespace SceneKit
{
	public partial class SCNNode : IEnumerable, IEnumerable<SCNNode>
	{
		public void Add (SCNNode node)
		{
			AddChildNode (node);
		}

		public void AddNodes (params SCNNode [] nodes)
		{
			if (nodes == null)
				return;
			foreach (var n in nodes)
				AddChildNode (n);
		}
		
		public IEnumerator<SCNNode> GetEnumerator ()
		{
			foreach (var node in ChildNodes)
				yield return node;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

#if !WATCH
		public void AddAnimation (CAAnimation animation, string key)
		{
			if (key == null) {
				((ISCNAnimatable) this).AddAnimation (animation, (NSString)null);
			} else {
				using (var s = new NSString (key))
					((ISCNAnimatable) this).AddAnimation (animation, s);
			}
		}

		public void RemoveAnimation (string key, nfloat duration)
		{
			if (string.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			using (var s = new NSString (key))
				((ISCNAnimatable)this).RemoveAnimation (s, duration);
		}

		public void RemoveAnimation (string key)
		{
			if (string.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			using (var s = new NSString (key))
				((ISCNAnimatable)this).RemoveAnimation (s);
		}

		public CAAnimation GetAnimation (string key)
		{
			if (string.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			CAAnimation animation = null;
			using (var s = new NSString (key))
				animation = ((ISCNAnimatable)this).GetAnimation (s);

			return animation;
		}

		public void PauseAnimation (string key)
		{
			if (string.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			using (var s = new NSString (key))
				((ISCNAnimatable)this).PauseAnimation (s);
		}

		public void ResumeAnimation (string key)
		{
			if (string.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			using (var s = new NSString (key))
				((ISCNAnimatable)this).ResumeAnimation (s);
		}

		public bool IsAnimationPaused (string key)
		{
			if (string.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			bool isPaused;

			using (var s = new NSString (key))
				isPaused = ((ISCNAnimatable)this).IsAnimationPaused (s);

			return isPaused;
		}

#if !XAMCORE_4_0
		// SCNNodePredicate is defined as:
		// 	delegate bool SCNNodePredicate (SCNNode node, out bool stop);
		// but the actual objective-c definition of the block is
		// 	void (^)(SCNNode *child, BOOL *stop)
		//
		[Obsolete ("Use the overload that takes a 'SCNNodeHandler' instead.")]
		public virtual void EnumerateChildNodes (SCNNodePredicate predicate)
		{
			SCNNodeHandler predHandler = (SCNNode node, out bool stop) => {
				predicate (node, out stop);
			};
			EnumerateChildNodes (predHandler);
		}
#endif

#endif // !WATCH
	}
}
