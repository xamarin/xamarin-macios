using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace GameController {

#if NET
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("tvos16.0")]
#else 
	[TV (16,0), Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
#endif
	[Register ("GCPhysicalInputElementCollection", SkipRegistration = true)]
	public sealed partial class GCPhysicalInputElementCollection<TKey, TValue> : GCPhysicalInputElementCollection
		where TKey : NSString 
		where TValue : class, IGCPhysicalInputElement {

		public GCPhysicalInputElementCollection (NSObjectFlag coder)
			: base (coder)
		{
		}

		public TValue? ElementForAlias (TKey alias)
			=> Runtime.GetINativeObject<TValue> (_ObjectForKeyedSubscript (alias), false);

		public TValue? ObjectForKeyedSubscript (TKey key)
			=> Runtime.GetINativeObject<TValue> (_ObjectForKeyedSubscript (key), false);

		public NSEnumerator<IGCPhysicalInputElement> ElementEnumerator
			=> Runtime.GetNSObject<NSEnumerator<IGCPhysicalInputElement>> (_ElementEnumerator)!; 

	}
}
