using Foundation;
using ObjCRuntime;
using AppKit;
using CoreFoundation;

namespace GHIssue7304 {

	[BaseType (typeof (NSObject))]
	interface FooClass {

		[Internal]
		[Export ("architecture")]
		int _Arch { get; set; }

		CFBundle.Architecture Arch {
			[Wrap ("(CFBundle.Architecture) _Arch")]
			get;
			[Wrap ("_Arch = (int)value")]
			set;
		}
	}

}
