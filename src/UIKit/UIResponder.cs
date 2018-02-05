//
// Additional methods for UIResponder
//
// Authors:
// 	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2011-2014 Xamarin Inc. All rights reserved
//

// the Copy() is pretty bad since it hide the "right" NSCopy protocol support for all UIResponder subclass
#if !XAMCORE_2_0

using System;
using Foundation;

namespace UIKit {

	public partial class UIResponder : NSObject {

		// The original implementation for UIResponderStandardEditActions protocol did not have any parameter
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1324

		[Obsolete ("Override Cut(NSObject)")]
		public virtual void Cut ()
		{
			Cut (null);
		}

		[Obsolete ("Override Copy(NSObject)")]
		public new virtual void Copy ()
		{
			Copy (null);
		}

		[Obsolete ("Override Paste(NSObject)")]
		public virtual void Paste ()
		{
			Paste ((NSObject)null);
		}

		[Obsolete ("Override Delete(NSObject)")]
		public virtual void Delete ()
		{
			Delete (null);
		}

		[Obsolete ("Override Select(NSObject)")]
		public virtual void Select ()
		{
			Select (null);
		}

		[Obsolete ("Override SelectAll(NSObject)")]
		public virtual void SelectAll ()
		{
			SelectAll (null);
		}
	}
}

#endif
