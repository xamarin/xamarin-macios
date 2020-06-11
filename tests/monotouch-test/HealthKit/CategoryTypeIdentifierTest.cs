//
// Unit tests for HKCategoryTypeIdentifier
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;

using Foundation;
using HealthKit;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.HealthKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CategoryTypeIdentifier {

		[Test]
		public void EnumValues_22351 ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);

			foreach (HKCategoryTypeIdentifier value in Enum.GetValues (typeof (HKCategoryTypeIdentifier))) {

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
#if !__WATCHOS__
				case HKCategoryTypeIdentifier.AbdominalCramps:
				case HKCategoryTypeIdentifier.Acne:
				case HKCategoryTypeIdentifier.AppetiteChanges:
				case HKCategoryTypeIdentifier.GeneralizedBodyAche:
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
#endif
				default:
					if (!TestRuntime.CheckXcodeVersion (7, 0))
						continue;
					break;
				}

				try {
					using (var ct = HKCategoryType.Create (value)) {
						Assert.That (ct.Handle, Is.Not.EqualTo (IntPtr.Zero), value.ToString ());
					}
				}
				catch (Exception e) {
					Assert.Fail ("{0} could not be created: {1}", value, e);
				}
			}
		}
	}
}

#endif // !__TVOS__
