//
// Unit tests for INIntentResolutionResult
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && XAMCORE_2_0 && !MONOMAC

using System;
using NUnit.Framework;

using Foundation;
using Intents;

namespace MonoTouchFixtures.Intents {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class INIntentResolutionResultTests {

		[SetUp]
		public void Setup ()
		{
#if !__WATCHOS__
			TestRuntime.AssertXcodeVersion (8, 0);
#else
			TestRuntime.AssertXcodeVersion (8, 3);
#endif
		}

		[Test]
		public void INIntentResolutionResultIsAbstractTest ()
		{
			Assert.Throws<NotImplementedException> (() => { var needsValue = INIntentResolutionResult.NeedsValue; }, "Base type must implement NeedsValue");
			Assert.Throws<NotImplementedException> (() => { var notRequired = INIntentResolutionResult.NotRequired; }, "Base type must implement NotRequired");
			Assert.Throws<NotImplementedException> (() => { var unsupported = INIntentResolutionResult.Unsupported; }, "Base type must implement Unsupported");
		}

		[Test]
		public void INCallRecordTypeResolutionResultPropertyTest ()
		{
			using (var needsValue = INCallRecordTypeResolutionResult.NeedsValue)
			using (var notRequired = INCallRecordTypeResolutionResult.NotRequired)
			using (var unsupported = INCallRecordTypeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCallRecordTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCallRecordTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCallRecordTypeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INDateComponentsRangeResolutionResultPropertyTest ()
		{
			using (var needsValue = INDateComponentsRangeResolutionResult.NeedsValue)
			using (var notRequired = INDateComponentsRangeResolutionResult.NotRequired)
			using (var unsupported = INDateComponentsRangeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INDateComponentsRangeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INDateComponentsRangeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INDateComponentsRangeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INMessageAttributeOptionsResolutionResultPropertyTest ()
		{
			using (var needsValue = INMessageAttributeOptionsResolutionResult.NeedsValue)
			using (var notRequired = INMessageAttributeOptionsResolutionResult.NotRequired)
			using (var unsupported = INMessageAttributeOptionsResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INMessageAttributeOptionsResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INMessageAttributeOptionsResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INMessageAttributeOptionsResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INMessageAttributeResolutionResultPropertyTest ()
		{
			using (var needsValue = INMessageAttributeResolutionResult.NeedsValue)
			using (var notRequired = INMessageAttributeResolutionResult.NotRequired)
			using (var unsupported = INMessageAttributeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INMessageAttributeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INMessageAttributeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INMessageAttributeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INPersonResolutionResultPropertyTest ()
		{
			using (var needsValue = INPersonResolutionResult.NeedsValue)
			using (var notRequired = INPersonResolutionResult.NotRequired)
			using (var unsupported = INPersonResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INPersonResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INPersonResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INPersonResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INPlacemarkResolutionResultPropertyTest ()
		{
			using (var needsValue = INPlacemarkResolutionResult.NeedsValue)
			using (var notRequired = INPlacemarkResolutionResult.NotRequired)
			using (var unsupported = INPlacemarkResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INPlacemarkResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INPlacemarkResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INPlacemarkResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INSpeakableStringResolutionResultPropertyTest ()
		{
			using (var needsValue = INSpeakableStringResolutionResult.NeedsValue)
			using (var notRequired = INSpeakableStringResolutionResult.NotRequired)
			using (var unsupported = INSpeakableStringResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INSpeakableStringResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INSpeakableStringResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INSpeakableStringResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INStringResolutionResultPropertyTest ()
		{
			using (var needsValue = INStringResolutionResult.NeedsValue)
			using (var notRequired = INStringResolutionResult.NotRequired)
			using (var unsupported = INStringResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INStringResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INStringResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INStringResolutionResult), unsupported, "Unsupported");
			}
		}

#if !MONOMAC // iOS only Result types
		[Test]
		public void INBooleanResolutionResultPropertyTest ()
		{
			using (var needsValue = INBooleanResolutionResult.NeedsValue)
			using (var notRequired = INBooleanResolutionResult.NotRequired)
			using (var unsupported = INBooleanResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INBooleanResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INBooleanResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INBooleanResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INCarAirCirculationModeResolutionResultPropertyTest ()
		{
		
			using (var needsValue = INCarAirCirculationModeResolutionResult.NeedsValue)
			using (var notRequired = INCarAirCirculationModeResolutionResult.NotRequired)
			using (var unsupported = INCarAirCirculationModeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCarAirCirculationModeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCarAirCirculationModeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCarAirCirculationModeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INCarAudioSourceResolutionResultPropertyTest ()
		{
			using (var needsValue = INCarAudioSourceResolutionResult.NeedsValue)
			using (var notRequired = INCarAudioSourceResolutionResult.NotRequired)
			using (var unsupported = INCarAudioSourceResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCarAudioSourceResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCarAudioSourceResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCarAudioSourceResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INCarDefrosterResolutionResultPropertyTest ()
		{
			using (var needsValue = INCarDefrosterResolutionResult.NeedsValue)
			using (var notRequired = INCarDefrosterResolutionResult.NotRequired)
			using (var unsupported = INCarDefrosterResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCarDefrosterResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCarDefrosterResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCarDefrosterResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INCarSeatResolutionResultPropertyTest ()
		{
			using (var needsValue = INCarSeatResolutionResult.NeedsValue)
			using (var notRequired = INCarSeatResolutionResult.NotRequired)
			using (var unsupported = INCarSeatResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCarSeatResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCarSeatResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCarSeatResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INCurrencyAmountResolutionResultPropertyTest ()
		{
			using (var needsValue = INCurrencyAmountResolutionResult.NeedsValue)
			using (var notRequired = INCurrencyAmountResolutionResult.NotRequired)
			using (var unsupported = INCurrencyAmountResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCurrencyAmountResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCurrencyAmountResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCurrencyAmountResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INDoubleResolutionResultPropertyTest ()
		{
			using (var needsValue = INDoubleResolutionResult.NeedsValue)
			using (var notRequired = INDoubleResolutionResult.NotRequired)
			using (var unsupported = INDoubleResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INDoubleResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INDoubleResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INDoubleResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INDateComponentsResolutionResultPropertyTest ()
		{
			using (var needsValue = INDateComponentsResolutionResult.NeedsValue)
			using (var notRequired = INDateComponentsResolutionResult.NotRequired)
			using (var unsupported = INDateComponentsResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INDateComponentsResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INDateComponentsResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INDateComponentsResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INIntegerResolutionResultPropertyTest ()
		{
			using (var needsValue = INIntegerResolutionResult.NeedsValue)
			using (var notRequired = INIntegerResolutionResult.NotRequired)
			using (var unsupported = INIntegerResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INIntegerResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INIntegerResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INIntegerResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INRadioTypeResolutionResultPropertyTest ()
		{
			using (var needsValue = INRadioTypeResolutionResult.NeedsValue)
			using (var notRequired = INRadioTypeResolutionResult.NotRequired)
			using (var unsupported = INRadioTypeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INRadioTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INRadioTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INRadioTypeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INRelativeReferenceResolutionResultPropertyTest ()
		{
			using (var needsValue = INRelativeReferenceResolutionResult.NeedsValue)
			using (var notRequired = INRelativeReferenceResolutionResult.NotRequired)
			using (var unsupported = INRelativeReferenceResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INRelativeReferenceResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INRelativeReferenceResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INRelativeReferenceResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
#if __WATCHOS__
		[ExpectedException (typeof (PlatformNotSupportedException))]
#endif
		public void INRelativeSettingResolutionResultPropertyTest ()
		{
			using (var needsValue = INRelativeSettingResolutionResult.NeedsValue)
			using (var notRequired = INRelativeSettingResolutionResult.NotRequired)
			using (var unsupported = INRelativeSettingResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INRelativeSettingResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INRelativeSettingResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INRelativeSettingResolutionResult), unsupported, "Unsupported");
			}
		}

#if !__WATCHOS__
		[Test]
		public void INRestaurantGuestResolutionResultPropertyTest ()
		{
			using (var needsValue = INRestaurantGuestResolutionResult.NeedsValue)
			using (var notRequired = INRestaurantGuestResolutionResult.NotRequired)
			using (var unsupported = INRestaurantGuestResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INRestaurantGuestResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INRestaurantGuestResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INRestaurantGuestResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INRestaurantResolutionResultPropertyTest ()
		{
			using (var needsValue = INRestaurantResolutionResult.NeedsValue)
			using (var notRequired = INRestaurantResolutionResult.NotRequired)
			using (var unsupported = INRestaurantResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INRestaurantResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INRestaurantResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INRestaurantResolutionResult), unsupported, "Unsupported");
			}
		}
#endif

		[Test]
		public void INTemperatureResolutionResultPropertyTest ()
		{
			using (var needsValue = INTemperatureResolutionResult.NeedsValue)
			using (var notRequired = INTemperatureResolutionResult.NotRequired)
			using (var unsupported = INTemperatureResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INTemperatureResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INTemperatureResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INTemperatureResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INWorkoutGoalUnitTypeResolutionResultPropertyTest ()
		{
			using (var needsValue = INWorkoutGoalUnitTypeResolutionResult.NeedsValue)
			using (var notRequired = INWorkoutGoalUnitTypeResolutionResult.NotRequired)
			using (var unsupported = INWorkoutGoalUnitTypeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INWorkoutGoalUnitTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INWorkoutGoalUnitTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INWorkoutGoalUnitTypeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INWorkoutLocationTypeResolutionResultPropertyTest ()
		{
			using (var needsValue = INWorkoutLocationTypeResolutionResult.NeedsValue)
			using (var notRequired = INWorkoutLocationTypeResolutionResult.NotRequired)
			using (var unsupported = INWorkoutLocationTypeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INWorkoutLocationTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INWorkoutLocationTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INWorkoutLocationTypeResolutionResult), unsupported, "Unsupported");
			}
		}
#endif

		[Test]
		public void INBillPayeeResolutionResultPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 3);

			using (var needsValue = INBillPayeeResolutionResult.NeedsValue)
			using (var notRequired = INBillPayeeResolutionResult.NotRequired)
			using (var unsupported = INBillPayeeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INBillPayeeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INBillPayeeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INBillPayeeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INBillTypeResolutionResultPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 3);

			using (var needsValue = INBillTypeResolutionResult.NeedsValue)
			using (var notRequired = INBillTypeResolutionResult.NotRequired)
			using (var unsupported = INBillTypeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INBillTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INBillTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INBillTypeResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INCarSignalOptionsResolutionResultPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 3);

			using (var needsValue = INCarSignalOptionsResolutionResult.NeedsValue)
			using (var notRequired = INCarSignalOptionsResolutionResult.NotRequired)
			using (var unsupported = INCarSignalOptionsResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INCarSignalOptionsResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INCarSignalOptionsResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INCarSignalOptionsResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INPaymentAmountResolutionResultPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 3);

			using (var needsValue = INPaymentAmountResolutionResult.NeedsValue)
			using (var notRequired = INPaymentAmountResolutionResult.NotRequired)
			using (var unsupported = INPaymentAmountResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INPaymentAmountResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INPaymentAmountResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INPaymentAmountResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INPaymentStatusResolutionResultPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 3);

			using (var needsValue = INPaymentStatusResolutionResult.NeedsValue)
			using (var notRequired = INPaymentStatusResolutionResult.NotRequired)
			using (var unsupported = INPaymentStatusResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INPaymentStatusResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INPaymentStatusResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INPaymentStatusResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INPaymentAccountResolutionResultPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 3);

			using (var needsValue = INPaymentAccountResolutionResult.NeedsValue)
			using (var notRequired = INPaymentAccountResolutionResult.NotRequired)
			using (var unsupported = INPaymentAccountResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOfType (typeof (INPaymentAccountResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOfType (typeof (INPaymentAccountResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOfType (typeof (INPaymentAccountResolutionResult), unsupported, "Unsupported");
			}
		}
	}
}
#endif
