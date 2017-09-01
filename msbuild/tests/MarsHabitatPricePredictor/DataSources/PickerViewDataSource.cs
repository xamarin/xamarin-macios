using System;
using Foundation;
using UIKit;

namespace MarsHabitatPricePredictor.DataSources
{
    public class PickerViewDataSource : NSObject, IUIPickerViewDataSource, IUIPickerViewDelegate
    {
        const int solarPanels = 0;
        const int greenhouses = 1;
        const int size = 2;

        SolarPanelDataSource solarPanelsDataSource = new SolarPanelDataSource();
        GreenhousesDataSource greenhousesDataSource = new GreenhousesDataSource();
        SizeDataSource sizeDataSource = new SizeDataSource();

        public event Action OnSelected;

		[Export("pickerView:didSelectRow:inComponent:")]
		public void Selected(UIPickerView picker, nint row, nint component)
		{
            OnSelected?.Invoke();
        }

		
        [Export("pickerView:titleForRow:forComponent:")]
        public string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            switch (component)   
            {
                case solarPanels:
                    return solarPanelsDataSource.Title(row);
                case greenhouses:
                    return greenhousesDataSource.Title(row);
                case size:
                    return sizeDataSource.Title(row);
                default:
                    return "";
            }
        }

        public double GetValue (nint row, nint component)
		{
			switch (component)
			{
				case solarPanels:
					return solarPanelsDataSource.Value(row);
				case greenhouses:
					return greenhousesDataSource.Value(row);
                case size:
					return sizeDataSource.Value(row);
                default:
                    return 0;
			}
        }

        [Export("numberOfComponentsInPickerView:")]
        public nint GetComponentCount(UIPickerView pickerView)
        {
            return 3;
        }

        [Export("pickerView:numberOfRowsInComponent:")]
        public nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
			switch (component)
			{
				case solarPanels:
                    return solarPanelsDataSource.Values.Length;
				case greenhouses:
					return greenhousesDataSource.Values.Length;
                case size:
					return sizeDataSource.Values.Length;
                default:
                    return 0;
			}
        }
    }
}
