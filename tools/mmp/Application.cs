namespace Xamarin.Bundler {
	public partial class Application
	{
		public bool Is32Build => !Driver.Is64Bit; 
		public bool Is64Build => Driver.Is64Bit; 
	}
}
