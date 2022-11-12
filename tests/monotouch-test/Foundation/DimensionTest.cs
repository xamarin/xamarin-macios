//
// Unit tests for NSDimension
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DimensionTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
		}

		[Test]
		public void BaseUnit ()
		{
			Assert.Throws<NotImplementedException> (() => { var bu = NSDimension.BaseUnit; }, "Base type must implement this");
		}

		[Test]
		public void NSUnitAcceleration_BaseUnit ()
		{
			using (var bu = NSUnitAcceleration.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitAcceleration), bu, "type");
				Assert.That ("m/s²", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitAngle_BaseUnit ()
		{
			using (var bu = NSUnitAngle.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitAngle), bu, "type");
				Assert.That ("°", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitArea_BaseUnit ()
		{
			using (var bu = NSUnitArea.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitArea), bu, "type");
				Assert.That ("m²", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitConcentrationMass_BaseUnit ()
		{
			using (var bu = NSUnitConcentrationMass.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitConcentrationMass), bu, "type");
				Assert.That ("g/L", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitDispersion_BaseUnit ()
		{
			using (var bu = NSUnitDispersion.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitDispersion), bu, "type");
				Assert.That ("ppm", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitDuration_BaseUnit ()
		{
			using (var bu = NSUnitDuration.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitDuration), bu, "type");
				Assert.That ("s", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricCharge_BaseUnit ()
		{
			using (var bu = NSUnitElectricCharge.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitElectricCharge), bu, "type");
				Assert.That ("C", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricCurrent_BaseUnit ()
		{
			using (var bu = NSUnitElectricCurrent.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitElectricCurrent), bu, "type");
				Assert.That ("A", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricPotentialDifference_BaseUnit ()
		{
			using (var bu = NSUnitElectricPotentialDifference.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitElectricPotentialDifference), bu, "type");
				Assert.That ("V", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricResistance_BaseUnit ()
		{
			using (var bu = NSUnitElectricResistance.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitElectricResistance), bu, "type");
				Assert.That ("Ω", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitEnergy_BaseUnit ()
		{
			using (var bu = NSUnitEnergy.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitEnergy), bu, "type");
				Assert.That ("J", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitFrequency_BaseUnit ()
		{
			using (var bu = NSUnitFrequency.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitFrequency), bu, "type");
				Assert.That ("Hz", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitFuelEfficiency_BaseUnit ()
		{
			using (var bu = NSUnitFuelEfficiency.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitFuelEfficiency), bu, "type");
				Assert.That ("L/100km", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitIlluminance_BaseUnit ()
		{
			using (var bu = NSUnitIlluminance.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitIlluminance), bu, "type");
				Assert.That ("lx", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitLength_BaseUnit ()
		{
			using (var bu = NSUnitLength.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitLength), bu, "type");
				Assert.That ("m", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitMass_BaseUnit ()
		{
			using (var bu = NSUnitMass.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitMass), bu, "type");
				Assert.That ("kg", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitPower_BaseUnit ()
		{
			using (var bu = NSUnitPower.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitPower), bu, "type");
				Assert.That ("W", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitPressure_BaseUnit ()
		{
			using (var bu = NSUnitPressure.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitPressure), bu, "type");
				Assert.That ("N/m²", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitSpeed_BaseUnit ()
		{
			using (var bu = NSUnitSpeed.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitSpeed), bu, "type");
				Assert.That ("m/s", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitTemperature_BaseUnit ()
		{
			using (var bu = NSUnitTemperature.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitTemperature), bu, "type");
				Assert.That ("K", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitVolume_BaseUnit ()
		{
			using (var bu = NSUnitVolume.BaseUnit) {
				Assert.IsInstanceOf (typeof (NSUnitVolume), bu, "type");
				Assert.That ("L", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}
	}
}
