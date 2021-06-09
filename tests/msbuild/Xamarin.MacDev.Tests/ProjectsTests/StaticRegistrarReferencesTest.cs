using NUnit.Framework;

using Xamarin.iOS.Tasks;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture (ApplePlatform.MacOSX)]
	[TestFixture (ApplePlatform.iOS)]
	public class StaticRegistrarReferencesTest : ProjectTest {
		string targetFrameworkIdentifier;

		public override string TargetFrameworkIdentifier {
			get { return targetFrameworkIdentifier; }
		}

		public StaticRegistrarReferencesTest (ApplePlatform platform) : base (null)
		{
			targetFrameworkIdentifier = platform.ToTargetPlatformIdentifier ();
		}

		[Test]
		public void Build ()
		{
			BuildCommonProject ("MyStaticRegistrarReferencesApp");
			ExecuteProject ();
		}
	}
}
