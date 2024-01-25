//
// Unit tests for HKCategoryTypeIdentifier
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if HAS_HEALTHKIT

using System;
using System.Collections.Generic;

using Foundation;

using HealthKit;

using NUnit.Framework;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CategoryTypeIdentifier {

		[Test]
		public void EnumValues_22351 ()
		{
#if MONOMAC
			TestRuntime.AssertXcodeVersion (14, 0);
#else
			TestRuntime.AssertXcodeVersion (6, 0);
#endif

			var failures = new List<string> ();

#if NET
			foreach (var value in Enum.GetValues<HKCategoryTypeIdentifier> ()) {
#else
			foreach (HKCategoryTypeIdentifier value in Enum.GetValues (typeof (HKCategoryTypeIdentifier))) {
#endif

				switch (value) {
				case HKCategoryTypeIdentifier.SleepAnalysis:
					break;
				case HKCategoryTypeIdentifier.MindfulSession:
					if (!TestRuntime.CheckXcodeVersion (8, 0))
						continue;
					break;
				case HKCategoryTypeIdentifier.HighHeartRateEvent:
				case HKCategoryTypeIdentifier.LowHeartRateEvent:
				case HKCategoryTypeIdentifier.IrregularHeartRhythmEvent:
					if (!TestRuntime.CheckXcodeVersion (10, 2))
						continue;
					break;
				case HKCategoryTypeIdentifier.AudioExposureEvent:
				case HKCategoryTypeIdentifier.ToothbrushingEvent:
					if (!TestRuntime.CheckXcodeVersion (11, 0))
						continue;
					break;
				case HKCategoryTypeIdentifier.GeneralizedBodyAche:
				case HKCategoryTypeIdentifier.AbdominalCramps:
				case HKCategoryTypeIdentifier.Acne:
				case HKCategoryTypeIdentifier.AppetiteChanges:
				case HKCategoryTypeIdentifier.Bloating:
				case HKCategoryTypeIdentifier.BreastPain:
				case HKCategoryTypeIdentifier.ChestTightnessOrPain:
				case HKCategoryTypeIdentifier.Chills:
				case HKCategoryTypeIdentifier.Constipation:
				case HKCategoryTypeIdentifier.Coughing:
				case HKCategoryTypeIdentifier.Diarrhea:
				case HKCategoryTypeIdentifier.Dizziness:
				case HKCategoryTypeIdentifier.Fainting:
				case HKCategoryTypeIdentifier.Fatigue:
				case HKCategoryTypeIdentifier.Fever:
				case HKCategoryTypeIdentifier.Headache:
				case HKCategoryTypeIdentifier.Heartburn:
				case HKCategoryTypeIdentifier.HotFlashes:
				case HKCategoryTypeIdentifier.LowerBackPain:
				case HKCategoryTypeIdentifier.LossOfSmell:
				case HKCategoryTypeIdentifier.LossOfTaste:
				case HKCategoryTypeIdentifier.MoodChanges:
				case HKCategoryTypeIdentifier.Nausea:
				case HKCategoryTypeIdentifier.PelvicPain:
				case HKCategoryTypeIdentifier.RapidPoundingOrFlutteringHeartbeat:
				case HKCategoryTypeIdentifier.RunnyNose:
				case HKCategoryTypeIdentifier.ShortnessOfBreath:
				case HKCategoryTypeIdentifier.SinusCongestion:
				case HKCategoryTypeIdentifier.SkippedHeartbeat:
				case HKCategoryTypeIdentifier.SleepChanges:
				case HKCategoryTypeIdentifier.SoreThroat:
				case HKCategoryTypeIdentifier.Vomiting:
				case HKCategoryTypeIdentifier.Wheezing:
					if (!TestRuntime.CheckXcodeVersion (11, 6))
						continue;
					break;
				case HKCategoryTypeIdentifier.BladderIncontinence:
				case HKCategoryTypeIdentifier.DrySkin:
				case HKCategoryTypeIdentifier.HairLoss:
				case HKCategoryTypeIdentifier.MemoryLapse:
				case HKCategoryTypeIdentifier.NightSweats:
				case HKCategoryTypeIdentifier.VaginalDryness:
				case HKCategoryTypeIdentifier.EnvironmentalAudioExposureEvent:
				case HKCategoryTypeIdentifier.HandwashingEvent:
					if (!TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch))
						continue;
					break;
				case HKCategoryTypeIdentifier.HeadphoneAudioExposureEvent:
					if (!TestRuntime.CheckXcodeVersion (12, 2))
						continue;
					break;
				case HKCategoryTypeIdentifier.Pregnancy:
				case HKCategoryTypeIdentifier.Lactation:
				case HKCategoryTypeIdentifier.Contraceptive:
				case HKCategoryTypeIdentifier.LowCardioFitnessEvent:
					if (!TestRuntime.CheckXcodeVersion (12, 3))
						continue;
					break;
				case HKCategoryTypeIdentifier.AppleWalkingSteadinessEvent:
				case HKCategoryTypeIdentifier.PregnancyTestResult:
				case HKCategoryTypeIdentifier.ProgesteroneTestResult:
					if (!TestRuntime.CheckXcodeVersion (13, 0))
						continue;
					break;
				case HKCategoryTypeIdentifier.InfrequentMenstrualCycles:
				case HKCategoryTypeIdentifier.IrregularMenstrualCycles:
				case HKCategoryTypeIdentifier.PersistentIntermenstrualBleeding:
				case HKCategoryTypeIdentifier.ProlongedMenstrualPeriods:
					if (!TestRuntime.CheckXcodeVersion (14, 1))
						continue;
					break;
				default:
					if (!TestRuntime.CheckXcodeVersion (7, 0))
						continue;
					break;
				}

				try {
					using (var ct = HKCategoryType.Create (value)) {
						Assert.That (ct.Handle, Is.Not.EqualTo (IntPtr.Zero), value.ToString ());
					}
				} catch (Exception e) {
					failures.Add ($"{value} could not be created: {e}");
				}
			}

			Assert.That (failures, Is.Empty, "No failures");
		}
	}
}

#endif // HAS_HEALTHKIT
