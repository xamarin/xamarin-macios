using System;

using Foundation;
using ObjCRuntime;

namespace NS {
	[Introduced (PlatformName.TvOS, 120, 0)]
	[BaseType (typeof (NSObject))]
	interface Whatever : IProtocol, IProtocolLower {
	}

	[Introduced (PlatformName.TvOS, 130, 0)]
	[Protocol]
	interface IProtocol {
		[Introduced (PlatformName.TvOS, 140, 0)]
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
			[Introduced (PlatformName.TvOS, 150, 0)]
			set;
		}

		[Abstract]
		[Export ("iPropC")]
		NSObject IPropC { get; set; }

		[Introduced (PlatformName.TvOS, 140, 0)]
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
			[Introduced (PlatformName.TvOS, 150, 0)]
			set;
		}

		[Export ("iPropCOpt")]
		NSObject IPropCOpt { get; set; }
	}

	[Introduced (PlatformName.TvOS, 90, 0)]
	[Protocol]
	interface IProtocolLower {
		[Introduced (PlatformName.TvOS, 100, 0)]
		[Abstract]
		[Export ("iPropD")]
		NSObject IPropD {
			get;
			[NoiOS]
			set;
		}

		[Abstract]
		[Export ("iPropE")]
		NSObject IPropE {
			[NoiOS]
			get;
			[Introduced (PlatformName.TvOS, 110, 0)]
			set;
		}

		[Abstract]
		[Export ("iPropF")]
		NSObject IPropF { get; set; }

		[Introduced (PlatformName.TvOS, 100, 0)]
		[Export ("iPropDOpt")]
		NSObject IPropDOpt {
			get;
			[NoiOS]
			set;
		}

		[Export ("iPropEOpt")]
		NSObject IPropEOpt {
			[NoiOS]
			get;
			[Introduced (PlatformName.TvOS, 110, 0)]
			set;
		}

		[Export ("iPropFOpt")]
		NSObject IPropFOpt { get; set; }
	}
}
