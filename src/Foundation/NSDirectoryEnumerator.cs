//
// NSDirectoryEnumerator.cs:
// Author:
//   Miguel de Icaza
//
// Copyright 2011 - 2014 Xamarin Inc
//
using ObjCRuntime;
using System;
using System.Collections.Generic;
using System.Collections;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {
	public partial class NSDirectoryEnumerator : IEnumerator<NSString>, IEnumerator<string>, IEnumerator {
		NSObject current;

		bool IEnumerator.MoveNext ()
		{
			current = NextObject ();
			return current is not null;
		}

		void IEnumerator.Reset ()
		{
			throw new InvalidOperationException ();
		}

		string IEnumerator<string>.Current {
			get {
				return current.ToString ();
			}
		}

		NSString IEnumerator<NSString>.Current {
			get {
				return current as NSString;
			}
		}

		object IEnumerator.Current {
			get {
				return current;
			}
		}
	}
}
