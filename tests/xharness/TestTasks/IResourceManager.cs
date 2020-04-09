namespace Xharness.TestTasks {

	/// <summary>
	/// Managed the resorces that can be used by the different tests tasks so that when ran async they do not step
	/// on eachother.
	/// </summary>
	public interface IResourceManager
	{
		Resource DesktopResource { get; }
		Resource NugetResource { get; }
	}
}
