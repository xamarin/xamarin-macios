// SqueezeNet.cs
//
// This file was automatically generated and should not be edited.
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using CoreML;
using CoreVideo;
using Foundation;

namespace MyCoreMLApp {
	/// <summary>
	/// Model Prediction Input Type
	/// </summary>
	public class SqueezeNetInput : NSObject, IMLFeatureProvider {
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("image")
		);

		CVPixelBuffer image;

		/// <summary>
		/// Input image to be classified as color (kCVPixelFormatType_32BGRA) image buffer, 227 pizels wide by 227 pixels high
		/// </summary>
		/// <value>Input image to be classified</value>
		public CVPixelBuffer Image {
			get { return image; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));

				image = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			switch (featureName) {
			case "image":
				return MLFeatureValue.Create (Image);
			default:
				return null;
			}
		}

		public SqueezeNetInput (CVPixelBuffer image)
		{
			if (image is null)
				throw new ArgumentNullException (nameof (image));

			Image = image;
		}
	}

	/// <summary>
	/// Model Prediction Output Type
	/// </summary>
	public class SqueezeNetOutput : NSObject, IMLFeatureProvider {
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("classLabelProbs"), new NSString ("classLabel")
		);

		NSDictionary<NSObject, NSNumber> classLabelProbs;
		string classLabel;

		/// <summary>
		/// Probability of each category as dictionary of strings to doubles
		/// </summary>
		/// <value>Probability of each category</value>
		public NSDictionary<NSObject, NSNumber> ClassLabelProbs {
			get { return classLabelProbs; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));

				classLabelProbs = value;
			}
		}

		/// <summary>
		/// Most likely image category as string value
		/// </summary>
		/// <value>Most likely image category</value>
		public string ClassLabel {
			get { return classLabel; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));

				classLabel = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			MLFeatureValue value;
			NSError err;

			switch (featureName) {
			case "classLabelProbs":
				value = MLFeatureValue.Create (ClassLabelProbs, out err);
				if (err is not null)
					err.Dispose ();
				return value;
			case "classLabel":
				return MLFeatureValue.Create (ClassLabel);
			default:
				return null;
			}
		}

		public SqueezeNetOutput (NSDictionary<NSObject, NSNumber> classLabelProbs, string classLabel)
		{
			if (classLabelProbs is null)
				throw new ArgumentNullException (nameof (classLabelProbs));

			if (classLabel is null)
				throw new ArgumentNullException (nameof (classLabel));

			ClassLabelProbs = classLabelProbs;
			ClassLabel = classLabel;
		}
	}

	/// <summary>
	/// Class for model loading and prediction
	/// </summary>
	public class SqueezeNet : NSObject {
		readonly MLModel model;

		public SqueezeNet ()
		{
			var url = NSBundle.MainBundle.GetUrlForResource ("SqueezeNet", "mlmodelc");
			NSError err;

			model = MLModel.Create (url, out err);
		}

		SqueezeNet (MLModel model)
		{
			this.model = model;
		}

		public static SqueezeNet FromUrl (NSUrl url, out NSError error)
		{
			if (url is null)
				throw new ArgumentNullException (nameof (url));

			var model = MLModel.Create (url, out error);

			if (model is null)
				return null;

			return new SqueezeNet (model);
		}

		/// <summary>
		/// Make a prediction using the standard interface
		/// </summary>
		/// <param name="input">an instance of SqueezeNetInput to predict from</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public SqueezeNetOutput GetPrediction (SqueezeNetInput input, out NSError error)
		{
			var prediction = model.GetPrediction (input, out error);

			if (prediction is null)
				return null;

			var classLabelProbs = prediction.GetFeatureValue ("classLabelProbs").DictionaryValue;
			var classLabel = prediction.GetFeatureValue ("classLabel").StringValue;

			return new SqueezeNetOutput (classLabelProbs, classLabel);
		}

		/// <summary>
		/// Make a prediction using the convenience interface
		/// </summary>
		/// <param name="image">Input image to be classified as color (kCVPixelFormatType_32BGRA) image buffer, 227 pizels wide by 227 pixels high</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public SqueezeNetOutput GetPrediction (CVPixelBuffer image, out NSError error)
		{
			var input = new SqueezeNetInput (image);

			return GetPrediction (input, out error);
		}
	}
}
