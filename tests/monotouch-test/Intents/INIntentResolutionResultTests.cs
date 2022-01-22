//
// Unit tests for INIntentResolutionResult
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

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

				Assert.IsInstanceOf (typeof (INCallRecordTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCallRecordTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCallRecordTypeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INDateComponentsRangeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INDateComponentsRangeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INDateComponentsRangeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INMessageAttributeOptionsResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INMessageAttributeOptionsResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INMessageAttributeOptionsResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INMessageAttributeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INMessageAttributeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INMessageAttributeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INPersonResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INPersonResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INPersonResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INPlacemarkResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INPlacemarkResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INPlacemarkResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INSpeakableStringResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INSpeakableStringResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INSpeakableStringResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INStringResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INStringResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INStringResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INBooleanResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INBooleanResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INBooleanResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INCarAirCirculationModeResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarAirCirculationModeResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarAirCirculationModeResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarAirCirculationModeResolutionResult.Unsupported; });
#else
			using (var needsValue = INCarAirCirculationModeResolutionResult.NeedsValue)
			using (var notRequired = INCarAirCirculationModeResolutionResult.NotRequired)
			using (var unsupported = INCarAirCirculationModeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INCarAirCirculationModeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCarAirCirculationModeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCarAirCirculationModeResolutionResult), unsupported, "Unsupported");
			}
#endif
		}

		[Test]
		public void INCarAudioSourceResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarAudioSourceResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarAudioSourceResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarAudioSourceResolutionResult.Unsupported; });
#else
			using (var needsValue = INCarAudioSourceResolutionResult.NeedsValue)
			using (var notRequired = INCarAudioSourceResolutionResult.NotRequired)
			using (var unsupported = INCarAudioSourceResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INCarAudioSourceResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCarAudioSourceResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCarAudioSourceResolutionResult), unsupported, "Unsupported");
			}
#endif
		}

		[Test]
		public void INCarDefrosterResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarDefrosterResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarDefrosterResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarDefrosterResolutionResult.Unsupported; });
#else
			using (var needsValue = INCarDefrosterResolutionResult.NeedsValue)
			using (var notRequired = INCarDefrosterResolutionResult.NotRequired)
			using (var unsupported = INCarDefrosterResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INCarDefrosterResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCarDefrosterResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCarDefrosterResolutionResult), unsupported, "Unsupported");
			}
#endif
		}

		[Test]
		public void INCarSeatResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarSeatResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarSeatResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INCarSeatResolutionResult.Unsupported; });
#else
			using (var needsValue = INCarSeatResolutionResult.NeedsValue)
			using (var notRequired = INCarSeatResolutionResult.NotRequired)
			using (var unsupported = INCarSeatResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INCarSeatResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCarSeatResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCarSeatResolutionResult), unsupported, "Unsupported");
			}
#endif
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

				Assert.IsInstanceOf (typeof (INCurrencyAmountResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCurrencyAmountResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCurrencyAmountResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INDoubleResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INDoubleResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INDoubleResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INDateComponentsResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INDateComponentsResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INDateComponentsResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INIntegerResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INIntegerResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INIntegerResolutionResult), unsupported, "Unsupported");
			}
		}

		[Test]
		public void INRadioTypeResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRadioTypeResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRadioTypeResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRadioTypeResolutionResult.Unsupported; });
#else
			using (var needsValue = INRadioTypeResolutionResult.NeedsValue)
			using (var notRequired = INRadioTypeResolutionResult.NotRequired)
			using (var unsupported = INRadioTypeResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INRadioTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INRadioTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INRadioTypeResolutionResult), unsupported, "Unsupported");
			}
#endif
		}

		[Test]
		public void INRelativeReferenceResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRelativeReferenceResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRelativeReferenceResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRelativeReferenceResolutionResult.Unsupported; });
#else
			using (var needsValue = INRelativeReferenceResolutionResult.NeedsValue)
			using (var notRequired = INRelativeReferenceResolutionResult.NotRequired)
			using (var unsupported = INRelativeReferenceResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INRelativeReferenceResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INRelativeReferenceResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INRelativeReferenceResolutionResult), unsupported, "Unsupported");
			}
#endif
		}

		[Test]
		public void INRelativeSettingResolutionResultPropertyTest ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRelativeSettingResolutionResult.NeedsValue; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRelativeSettingResolutionResult.NotRequired; });
			Assert.Throws<PlatformNotSupportedException> (() => { var v = INRelativeSettingResolutionResult.Unsupported; });
#else
			using (var needsValue = INRelativeSettingResolutionResult.NeedsValue)
			using (var notRequired = INRelativeSettingResolutionResult.NotRequired)
			using (var unsupported = INRelativeSettingResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INRelativeSettingResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INRelativeSettingResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INRelativeSettingResolutionResult), unsupported, "Unsupported");
			}
#endif
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

				Assert.IsInstanceOf (typeof (INRestaurantGuestResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INRestaurantGuestResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INRestaurantGuestResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INRestaurantResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INRestaurantResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INRestaurantResolutionResult), unsupported, "Unsupported");
			}
		}
#endif // !__WATCHOS__

		[Test]
		public void INTemperatureResolutionResultPropertyTest ()
		{
			using (var needsValue = INTemperatureResolutionResult.NeedsValue)
			using (var notRequired = INTemperatureResolutionResult.NotRequired)
			using (var unsupported = INTemperatureResolutionResult.Unsupported) {
				Assert.NotNull (needsValue, "NeedsValue Null");
				Assert.NotNull (notRequired, "NotRequired Null");
				Assert.NotNull (unsupported, "Unsupported Null");

				Assert.IsInstanceOf (typeof (INTemperatureResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INTemperatureResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INTemperatureResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INWorkoutGoalUnitTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INWorkoutGoalUnitTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INWorkoutGoalUnitTypeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INWorkoutLocationTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INWorkoutLocationTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INWorkoutLocationTypeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INBillPayeeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INBillPayeeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INBillPayeeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INBillTypeResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INBillTypeResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INBillTypeResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INCarSignalOptionsResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INCarSignalOptionsResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INCarSignalOptionsResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INPaymentAmountResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INPaymentAmountResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INPaymentAmountResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INPaymentStatusResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INPaymentStatusResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INPaymentStatusResolutionResult), unsupported, "Unsupported");
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

				Assert.IsInstanceOf (typeof (INPaymentAccountResolutionResult), needsValue, "NeedsValue");
				Assert.IsInstanceOf (typeof (INPaymentAccountResolutionResult), notRequired, "NotRequired");
				Assert.IsInstanceOf (typeof (INPaymentAccountResolutionResult), unsupported, "Unsupported");
			}
		}
	}
}
#endif
