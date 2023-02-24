using System;

using Foundation;
using ObjCRuntime;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface Whatever : IProtocol {
		[Export ("propA")]
		NSObject PropA {
			get;
			[NoiOS]
			set;
		}

		[Export ("propB")]
		NSObject PropB {
			[NoiOS]
			get;
			set;
		}
	}

	[Protocol]
	interface IProtocol {
		[Abstract]
		[Export ("iPropA")]
		NSObject IPropA {
			get;
			[NoiOS]
			set;
		}

		[Abstract]
		[Export ("iPropB")]
		NSObject IPropB {
			[NoiOS]
			get;
			set;
		}

		[Export ("iPropAOpt")]
		NSObject IPropAOpt {
			get;
			[NoiOS]
			set;
		}

		[Export ("iPropBOpt")]
		NSObject IPropBOpt {
			[NoiOS]
			get;
			set;
		}
	}
}
