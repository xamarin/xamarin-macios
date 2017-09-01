using System;
namespace MarsHabitatPricePredictor.DataSources
{
	public struct SizeDataSource
	{
		public double[] Values => new double[] { 750,
		1000,
		1500,
		2000,
		3000,
		4000,
		5000,
		10_000 };

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
