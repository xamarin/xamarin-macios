using System;
namespace MarsHabitatPricePredictor.DataSources
{
    public struct SolarPanelDataSource
	{
		public double[] Values => new double[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5 };

		public string Title(nint index)
		{
			return Values[index].ToString();
		}

		public double Value(nint index)
		{
			return Values[index];
		}
	}
}
