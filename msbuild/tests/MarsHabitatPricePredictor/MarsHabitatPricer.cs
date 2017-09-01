// MarsHabitatPricer.cs
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

namespace MarsHabitatPricePredictor {
	/// <summary>
	/// Model Prediction Input Type
	/// </summary>
	public class MarsHabitatPricerInput : NSObject, IMLFeatureProvider
	{
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("solarPanels"), new NSString ("greenhouses"), new NSString ("size")
		);

		double solarPanels;
		double greenhouses;
		double size;

		/// <summary>
		/// Number of solar panels as double
		/// </summary>
		/// <value>Number of solar panels</value>
		public double SolarPanels {
			get { return solarPanels; }
			set {
				solarPanels = value;
			}
		}

		/// <summary>
		/// Number of greenhouses as double
		/// </summary>
		/// <value>Number of greenhouses</value>
		public double Greenhouses {
			get { return greenhouses; }
			set {
				greenhouses = value;
			}
		}

		/// <summary>
		/// Size in acres as double
		/// </summary>
		/// <value>Size in acres</value>
		public double Size {
			get { return size; }
			set {
				size = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			switch (featureName) {
			case "solarPanels":
				return MLFeatureValue.FromDouble (SolarPanels);
			case "greenhouses":
				return MLFeatureValue.FromDouble (Greenhouses);
			case "size":
				return MLFeatureValue.FromDouble (Size);
			default:
				return null;
			}
		}

		public MarsHabitatPricerInput (double solarPanels, double greenhouses, double size)
		{
			SolarPanels = solarPanels;
			Greenhouses = greenhouses;
			Size = size;
		}
	}

	/// <summary>
	/// Model Prediction Output Type
	/// </summary>
	public class MarsHabitatPricerOutput : NSObject, IMLFeatureProvider
	{
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("price")
		);

		double price;

		/// <summary>
		/// Price of the habitat (in millions) as double
		/// </summary>
		/// <value>Price of the habitat (in millions)</value>
		public double Price {
			get { return price; }
			set {
				price = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			switch (featureName) {
			case "price":
				return MLFeatureValue.FromDouble (Price);
			default:
				return null;
			}
		}

		public MarsHabitatPricerOutput (double price)
		{
			Price = price;
		}
	}

	/// <summary>
	/// Class for model loading and prediction
	/// </summary>
	public class MarsHabitatPricer : NSObject
	{
		readonly MLModel model;

		public MarsHabitatPricer ()
		{
			var url = NSBundle.MainBundle.GetUrlForResource ("MarsHabitatPricer", "mlmodelc");
			NSError err;

			model = MLModel.FromUrl (url, out err);
		}

		MarsHabitatPricer (MLModel model)
		{
			this.model = model;
		}

		public static MarsHabitatPricer FromUrl (NSUrl url, out NSError error)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			var model = MLModel.FromUrl (url, out error);

			if (model == null)
				return null;

			return new MarsHabitatPricer (model);
		}

		/// <summary>
		/// Make a prediction using the standard interface
		/// </summary>
		/// <param name="input">an instance of MarsHabitatPricerInput to predict from</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public MarsHabitatPricerOutput GetPrediction (MarsHabitatPricerInput input, out NSError error)
		{
			var prediction = model.GetPrediction (input, out error);

			if (prediction == null)
				return null;

			var price = prediction.GetFeatureValue ("price").DoubleValue;

			return new MarsHabitatPricerOutput (price);
		}

		/// <summary>
		/// Make a prediction using the convenience interface
		/// </summary>
		/// <param name="solarPanels">Number of solar panels as double</param>
		/// <param name="greenhouses">Number of greenhouses as double</param>
		/// <param name="size">Size in acres as double</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public MarsHabitatPricerOutput GetPrediction (double solarPanels, double greenhouses, double size, out NSError error)
		{
			var input = new MarsHabitatPricerInput (solarPanels, greenhouses, size);

			return GetPrediction (input, out error);
		}
	}
}
