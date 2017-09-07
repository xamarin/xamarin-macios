using System;
using System.Linq;
using CoreML;
using Foundation;
using UIKit;
using Vision;
using MarsHabitatPricePredictor.DataSources;

namespace MarsHabitatPricePredictor
{
	public partial class ViewController : UIViewController
	{
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		MarsHabitatPricer model;

		NSNumberFormatter priceFormatter = new NSNumberFormatter {
			NumberStyle = NSNumberFormatterStyle.Currency,
			MaximumFractionDigits = 0,
			UsesGroupingSeparator = true,
			Locale = new NSLocale ("en_US")
		};

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			BuildUI ();

			// Load the ML model
			model = new MarsHabitatPricer ();

			updatePredictedPrice ();
		}

		void updatePredictedPrice ()
		{
			var solarPanels = datasource.GetValue (pickerView.SelectedRowInComponent (0), Feature.SolarPanels);
			var greenhouses = datasource.GetValue (pickerView.SelectedRowInComponent (1), Feature.Greenhouses);
			var size = datasource.GetValue (pickerView.SelectedRowInComponent (2), Feature.Size);

			var pricerInput = new MarsHabitatPricerInput (solarPanels, greenhouses, size);

			// Use the ML model
			var outFeatures = model.GetPrediction (pricerInput, out NSError err2);
			var result = outFeatures?.Price;

			if (outFeatures != null)
				priceLabel.Text = "Predicted Price (millions) " + priceFormatter.StringFor (new NSNumber (result.Value));

			Console.WriteLine (err2 == null ? $"result was {result.Value}" : "Unexpected runtime error " + err2.Description);
		}

		#region user interface
		UIPickerView pickerView = new UIPickerView ();
		PickerViewDataSource datasource = new PickerViewDataSource ();
		UILabel priceLabel = new UILabel ();
		UILabel headingLabel = new UILabel (), solarPanelLabel = new UILabel (), greenhousesLabel = new UILabel (), sizeLabel = new UILabel ();

		void BuildUI ()
		{
			headingLabel.Frame = new CoreGraphics.CGRect (0, 20, View.Bounds.Width, 38);
			headingLabel.Text = "Mars Habitat Price Predictor";
			headingLabel.TextAlignment = UITextAlignment.Center;
			headingLabel.Font = UIFont.BoldSystemFontOfSize (20);
			this.View.Add (headingLabel);
			var topMargin = 60;
			solarPanelLabel.Frame = new CoreGraphics.CGRect (0, topMargin, View.Bounds.Width / 3, 38);
			solarPanelLabel.Text = "Solar Panels";
			solarPanelLabel.TextAlignment = UITextAlignment.Center;
			this.View.Add (solarPanelLabel);
			greenhousesLabel.Frame = new CoreGraphics.CGRect (View.Bounds.Width / 2 - View.Bounds.Width / 6, topMargin, View.Bounds.Width / 3, 38);
			greenhousesLabel.Text = "Greenhouses";
			greenhousesLabel.TextAlignment = UITextAlignment.Center;
			this.View.Add (greenhousesLabel);
			sizeLabel.Frame = new CoreGraphics.CGRect (View.Bounds.Width / 3 * 2, topMargin, View.Bounds.Width / 3, 38);
			sizeLabel.Text = "Acres";
			sizeLabel.TextAlignment = UITextAlignment.Center;
			this.View.Add (sizeLabel);

			pickerView.ShowSelectionIndicator = true;
			pickerView.Frame = new CoreGraphics.CGRect (0, topMargin, View.Bounds.Width, 250);
			datasource.OnSelected += updatePredictedPrice;
			pickerView.Delegate = datasource;
			pickerView.DataSource = datasource;
			pickerView.Select (2, Feature.SolarPanels, false); // default select the second item in each list
			pickerView.Select (2, Feature.Greenhouses, false);
			pickerView.Select (2, Feature.Size, false);
			this.View.Add (pickerView);

			priceLabel.Frame = new CoreGraphics.CGRect (0, 300, View.Bounds.Width, 38);
			priceLabel.Text = "$$$$$";
			priceLabel.TextAlignment = UITextAlignment.Center;
			priceLabel.Font = UIFont.BoldSystemFontOfSize (20);
			this.View.Add (priceLabel);
		}
		#endregion
	}
}
