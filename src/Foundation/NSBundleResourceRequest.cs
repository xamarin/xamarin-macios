using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

#if !MONOMAC

namespace Foundation {
	public partial class NSBundleResourceRequest : NSObject {
		static NSSet<NSString> MakeSetFromTags (string [] tags)
		{
			var x = new NSString [tags.Length];
			for (int i = 0; i < tags.Length; i++)
				x [i] = new NSString (tags [i]);
			return new NSSet<NSString> (x);
		}

		static NSSet<NSString> MakeSetFromTags (NSString [] tags)
		{
			return new NSSet<NSString> (tags);
		}

		public NSBundleResourceRequest (params string [] tags) : this (MakeSetFromTags (tags)) { }
		public NSBundleResourceRequest (NSBundle bundle, params string [] tags) : this (MakeSetFromTags (tags), bundle) { }

		public NSBundleResourceRequest (params NSString [] tags) : this (MakeSetFromTags (tags)) { }
		public NSBundleResourceRequest (NSBundle bundle, params NSString [] tags) : this (MakeSetFromTags (tags), bundle) { }

	}
}

#endif
