using System.Reflection;

namespace Xamarin.Tests {
	public partial class Configuration {
		static string TestAssemblyDirectory {
			get {
				return Assembly.GetExecutingAssembly ().Location;
			}
		}
	}
}
