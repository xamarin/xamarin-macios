﻿// 
// NSUnit.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//     
// Copyright 2017 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

namespace Foundation {
#if !XAMCORE_4_0
	public partial class NSUnit {
		[Obsolete ("Use .ctor(string)")]
		public NSUnit () { }
	}

	public partial class NSUnitAcceleration {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitAcceleration () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitAngle {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitAngle () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitArea {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitArea () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitConcentrationMass {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitConcentrationMass () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitDispersion {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitDispersion () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitDuration {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitDuration () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitElectricCharge {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricCharge () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitElectricCurrent {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricCurrent () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitElectricPotentialDifference {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricPotentialDifference () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitElectricResistance {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricResistance () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitEnergy {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitEnergy () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitFrequency {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitFrequency () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitFuelEfficiency {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitFuelEfficiency () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitLength {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitLength () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitIlluminance {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitIlluminance () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitMass {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitMass () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitPower {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitPower () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitPressure {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitPressure () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitSpeed {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitSpeed () : base (NSObjectFlag.Empty) { }
	}

	public partial class NSUnitVolume {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitVolume () : base (NSObjectFlag.Empty) { }
	}
#endif
}
