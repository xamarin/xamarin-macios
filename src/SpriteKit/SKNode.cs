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

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace SpriteKit {
	public partial class SKNode : IEnumerable, IEnumerable<SKNode> {
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static T? FromFile<T> (string file) where T : SKNode
		{
			var fileHandle = CFString.CreateNative (file);
			try {
				var handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (T)), Selector.GetHandle ("nodeWithFileNamed:"), fileHandle);
				return Runtime.GetNSObject<T> (handle);
			} finally {
				CFString.ReleaseNative (fileHandle);
			}
		}

		public void Add (SKNode node)
		{
			AddChild (node);
		}

		public void AddNodes (params SKNode []? nodes)
		{
			if (nodes is null)
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

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
#endif
		public static SKNode? Create (string filename, Type [] types, out NSError error)
		{
			// Let's fail early.
			if (filename is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (filename));
			if (types is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (types));
			if (types.Length == 0)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (nameof (types), "Length must be greater than zero.");

			using (var classes = new NSMutableSet<Class> ((nint) types.Length)) {
				foreach (var type in types)
					classes.Add (new Class (type));
				return Create (filename, classes.Handle, out error);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
#endif
		public static SKNode? Create (string filename, NSSet<Class> classes, out NSError error)
		{
			// `filename` will be checked by `Create` later
			if (classes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (classes));
			if (classes.Count == 0)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (nameof (classes), "Length must be greater than zero.");

			return Create (filename, classes.Handle, out error);
		}
	}
}
