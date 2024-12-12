#if HAS_MAPKIT && !WATCH

using System;

using Foundation;
using MapKit;

using NUnit.Framework;

using Xamarin.Utils;

namespace MonoTouchFixtures.MapKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AddressFilterTest {
		[Test]
		public void Constructors ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using (var filter = new MKAddressFilter (MKAddressFilterOption.Country, MKAddressFilterConstructorOption.Exclude)) {
				Assert.IsNotNull (filter, "Exclude filter");
			}
			using (var filter = new MKAddressFilter (MKAddressFilterOption.SubAdministrativeArea, MKAddressFilterConstructorOption.Include)) {
				Assert.IsNotNull (filter, "Include filter");
			}
		}
	}
}

#endif // HAS_MAPKIT && !WATCH
