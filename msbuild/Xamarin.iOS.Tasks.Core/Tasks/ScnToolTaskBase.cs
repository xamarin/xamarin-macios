namespace Xamarin.iOS.Tasks
{
	public abstract class ScnToolTaskBase : Xamarin.MacDev.Tasks.ScnToolTaskBase
	{
		protected override string OperatingSystem {
			get { return "ios"; }
		}
	}
}
