//
// UIEvent.cs: Extensions to the UIEvent class
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012, Xamarin Inc
//

#if !WATCH

using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;

namespace XamCore.UIKit {
	public partial class UIEvent {

		public override string ToString ()
		{
			return String.Format ("[Time={0} ({1}{2})]", Timestamp, Type, Subtype != UIEventSubtype.None ? "." + Subtype : "");
		}
	}
}

#endif // !WATCH
