using System;
namespace MarsHabitatPricePredictor.DataSources
{
    public struct GreenhousesDataSource
    {
        public double[] Values => new double[] { 1, 2, 3, 4, 5 };

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
