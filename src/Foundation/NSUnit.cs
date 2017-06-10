﻿// 
// NSUnit.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//     
// Copyright 2017 Xamarin Inc.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Foundation {
#if !XAMCORE_4_0
	public partial class NSUnit {
		[Obsolete ("Use .ctor(string)")]
		public NSUnit () { }
	}

	public partial class NSUnitAcceleration {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitAcceleration () { }
	}

	public partial class NSUnitAngle {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitAngle () { }
	}

	public partial class NSUnitArea {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitArea () { }
	}

	public partial class NSUnitConcentrationMass {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitConcentrationMass () { }
	}

	public partial class NSUnitDispersion {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitDispersion () { }
	}

	public partial class NSUnitDuration {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitDuration () { }
	}

	public partial class NSUnitElectricCharge {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricCharge () { }
	}

	public partial class NSUnitElectricCurrent {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricCurrent () { }
	}

	public partial class NSUnitElectricPotentialDifference {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricPotentialDifference () { }
	}

	public partial class NSUnitElectricResistance {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitElectricResistance () { }
	}

	public partial class NSUnitEnergy {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitEnergy () { }
	}

	public partial class NSUnitFrequency {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitFrequency () { }
	}

	public partial class NSUnitFuelEfficiency {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitFuelEfficiency () { }
	}

	public partial class NSUnitLength {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitLength () { }
	}

	public partial class NSUnitIlluminance {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitIlluminance () { }
	}

	public partial class NSUnitMass {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitMass () { }
	}

	public partial class NSUnitPower {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitPower () { }
	}

	public partial class NSUnitPressure {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitPressure () { }
	}

	public partial class NSUnitSpeed {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitSpeed () { }
	}

	public partial class NSUnitVolume {
		[Obsolete ("Use .ctor(string, NSUnitConverter) or any of the static properties.")]
		public NSUnitVolume () { }
	}

	public partial class NSDimension {
		[Obsolete ("Not intended to be directly instantiated, this is an abstract class.")]
		public NSDimension () { }
	}
#endif
}
