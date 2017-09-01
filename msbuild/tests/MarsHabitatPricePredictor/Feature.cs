using System;
namespace MarsHabitatPricePredictor
{
    // nint requirement means this couldn't be enum 
    // couldn't use it every where I wanted to either :(
    public static class Feature
    {
        public static nint SolarPanels = 0;
        public static nint Greenhouses = 1;
        public static nint Size = 2;
    }
}
