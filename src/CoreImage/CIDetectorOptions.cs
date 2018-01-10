//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013, 2016 Xamarin, Inc.
//

using System;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;

namespace CoreImage {

	public partial class CIDetectorOptions  {

		public FaceDetectorAccuracy? Accuracy { get; set; }
		public float? MinFeatureSize { get; set; }

		[iOS (10,0)][Mac (10,12)]
		public int? MaxFeatureCount { get; set; }

		public bool? TrackingEnabled { get; set; }
		public bool? EyeBlink { get; set; }
		public bool? Smile { get; set; }
		public float? AspectRatio { get; set; }
		public float? FocalLength { get; set; }

		public float? NumberOfAngles { get; set; }

		public bool? ReturnSubFeatures { get; set; }

		public CIImageOrientation? ImageOrientation { get; set; }
		
		internal NSDictionary ToDictionary ()
		{
			// We now have 11 possible keys so begining with 6 *might* be optimal
			List<NSObject> keys = new List<NSObject> (6);
			List<NSObject> values = new List<NSObject> (6);

			if (CIDetector.Accuracy != null) {
				keys.Add (CIDetector.Accuracy);
				values.Add (Accuracy == FaceDetectorAccuracy.High ? CIDetector.AccuracyHigh :CIDetector.AccuracyLow);
			}

			// MinFeatureSize exists only in iOS6+, before this the field is null (and would throw if used)
			if (CIDetector.MinFeatureSize != null && MinFeatureSize != null) {
				keys.Add (CIDetector.MinFeatureSize);
				values.Add (new NSNumber (MinFeatureSize.Value));
			}

			// Tracking exists only in iOS6+, before this the field is null (and would throw if used)
			if (CIDetector.Tracking != null && TrackingEnabled != null) {
				keys.Add (CIDetector.Tracking);
				values.Add (NSObject.FromObject (TrackingEnabled.Value));
			}

			// EyeBlink exists only in iOS7+, before this the field is null (and would throw if used)
			if (CIDetector.EyeBlink != null && EyeBlink != null) {
				keys.Add (CIDetector.EyeBlink);
				values.Add (NSObject.FromObject (EyeBlink.Value));
			}

			// Smile exists only in iOS7+, before this the field is null (and would throw if used)
			if (CIDetector.Smile != null && Smile != null) {
				keys.Add (CIDetector.Smile);
				values.Add (NSObject.FromObject (Smile.Value));
			}
			// AspectRation exists only in iOS8+, before this the field is null (and would throw if used)
			if (CIDetector.AspectRatio != null && AspectRatio != null){
				keys.Add (CIDetector.AspectRatio);
				values.Add (new NSNumber (AspectRatio.Value));
			}
			// FocalLength exists only in iOS8+, before this the field is null (and would throw if used)
			if (CIDetector.FocalLength != null && FocalLength != null){
				keys.Add (CIDetector.FocalLength);
				values.Add (new NSNumber (FocalLength.Value));
			}
			if (CIDetector.NumberOfAngles != null && NumberOfAngles != null){
				keys.Add (CIDetector.NumberOfAngles);
				values.Add (new NSNumber (NumberOfAngles.Value));
			}

			if (CIDetector.ReturnSubFeatures != null && ReturnSubFeatures != null){
				keys.Add (CIDetector.ReturnSubFeatures);
				values.Add (new NSNumber (ReturnSubFeatures.Value));
			}

			if (CIDetector.ImageOrientation != null && ImageOrientation != null) {
				keys.Add (CIDetector.ImageOrientation);
				values.Add (new NSNumber ((int) ImageOrientation.Value));
			}

			if (CIDetector.MaxFeatureCount != null && MaxFeatureCount != null) {
				keys.Add (CIDetector.MaxFeatureCount);
				values.Add (new NSNumber ((int) MaxFeatureCount.Value));
			}

			return NSDictionary.FromObjectsAndKeys (values.ToArray (), keys.ToArray ());
		}
	}
}
