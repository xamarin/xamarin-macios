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
			TestRuntime.AssertXcodeVersion (8,0);
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
				Assert.IsInstanceOfType (typeof (NSUnitAcceleration), bu, "type");
				Assert.That ("m/s²", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitAngle_BaseUnit ()
		{
			using (var bu = NSUnitAngle.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitAngle), bu, "type");
				Assert.That ("°", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitArea_BaseUnit ()
		{
			using (var bu = NSUnitArea.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitArea), bu, "type");
				Assert.That ("m²", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitConcentrationMass_BaseUnit ()
		{
			using (var bu = NSUnitConcentrationMass.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitConcentrationMass), bu, "type");
				Assert.That ("g/L", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitDispersion_BaseUnit ()
		{
			using (var bu = NSUnitDispersion.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitDispersion), bu, "type");
				Assert.That ("ppm", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitDuration_BaseUnit ()
		{
			using (var bu = NSUnitDuration.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitDuration), bu, "type");
				Assert.That ("s", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricCharge_BaseUnit ()
		{
			using (var bu = NSUnitElectricCharge.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitElectricCharge), bu, "type");
				Assert.That ("C", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricCurrent_BaseUnit ()
		{
			using (var bu = NSUnitElectricCurrent.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitElectricCurrent), bu, "type");
				Assert.That ("A", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricPotentialDifference_BaseUnit ()
		{
			using (var bu = NSUnitElectricPotentialDifference.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitElectricPotentialDifference), bu, "type");
				Assert.That ("V", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitElectricResistance_BaseUnit ()
		{
			using (var bu = NSUnitElectricResistance.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitElectricResistance), bu, "type");
				Assert.That ("Ω", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitEnergy_BaseUnit ()
		{
			using (var bu = NSUnitEnergy.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitEnergy), bu, "type");
				Assert.That ("J", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitFrequency_BaseUnit ()
		{
			using (var bu = NSUnitFrequency.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitFrequency), bu, "type");
				Assert.That ("Hz", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitFuelEfficiency_BaseUnit ()
		{
			using (var bu = NSUnitFuelEfficiency.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitFuelEfficiency), bu, "type");
				Assert.That ("L/100km", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitIlluminance_BaseUnit ()
		{
			using (var bu = NSUnitIlluminance.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitIlluminance), bu, "type");
				Assert.That ("lx", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitLength_BaseUnit ()
		{
			using (var bu = NSUnitLength.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitLength), bu, "type");
				Assert.That ("m", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitMass_BaseUnit ()
		{
			using (var bu = NSUnitMass.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitMass), bu, "type");
				Assert.That ("kg", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitPower_BaseUnit ()
		{
			using (var bu = NSUnitPower.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitPower), bu, "type");
				Assert.That ("W", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitPressure_BaseUnit ()
		{
			using (var bu = NSUnitPressure.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitPressure), bu, "type");
				Assert.That ("N/m²", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitSpeed_BaseUnit ()
		{
			using (var bu = NSUnitSpeed.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitSpeed), bu, "type");
				Assert.That ("m/s", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitTemperature_BaseUnit ()
		{
			using (var bu = NSUnitTemperature.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitTemperature), bu, "type");
				Assert.That ("K", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}

		[Test]
		public void NSUnitVolume_BaseUnit ()
		{
			using (var bu = NSUnitVolume.BaseUnit) {
				Assert.IsInstanceOfType (typeof (NSUnitVolume), bu, "type");
				Assert.That ("L", Is.EqualTo (bu.Symbol), "Symbol");
			}
		}
	}
}
