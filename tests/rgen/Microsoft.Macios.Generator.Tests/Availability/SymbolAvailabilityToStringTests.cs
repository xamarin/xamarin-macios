using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class SymbolAvailabilityToStringTests {

	class TestDataToString : IEnumerable<object []> {

		public IEnumerator<object []> GetEnumerator ()
		{
			var builder = SymbolAvailability.CreateBuilder ();
			yield return [builder.ToImmutable (), "[]"];

			builder.Clear ();
			builder.AddSupportedVersion (ApplePlatform.iOS, new (16, 0));
			yield return [builder.ToImmutable (), "[{ Platform: iOS Supported: '16.0' Unsupported: [], Obsoleted: [] }]"];

			builder.Clear ();
			builder.AddSupportedVersion (ApplePlatform.TVOS, new (16, 0));
			yield return [builder.ToImmutable (), "[{ Platform: TVOS Supported: '16.0' Unsupported: [], Obsoleted: [] }]"];

			builder.Clear ();
			builder.AddSupportedVersion (ApplePlatform.MacOSX, new (11, 0));
			yield return [builder.ToImmutable (), "[{ Platform: MacOSX Supported: '11.0' Unsupported: [], Obsoleted: [] }]"];

			builder.Clear ();
			builder.AddSupportedVersion (ApplePlatform.MacCatalyst, new (16, 0));
			yield return [builder.ToImmutable (), "[{ Platform: MacCatalyst Supported: '16.0' Unsupported: [], Obsoleted: [] }]"];

			builder.Clear ();
			builder.AddSupportedVersion (ApplePlatform.iOS, new (16, 0));
			builder.AddSupportedVersion (ApplePlatform.MacOSX, new (11, 0));
			builder.AddSupportedVersion (ApplePlatform.MacCatalyst, new (16, 0));
			yield return [builder.ToImmutable (), "[{ Platform: MacOSX Supported: '11.0' Unsupported: [], Obsoleted: [] }, { Platform: iOS Supported: '16.0' Unsupported: [], Obsoleted: [] }, { Platform: MacCatalyst Supported: '16.0' Unsupported: [], Obsoleted: [] }]"];

			builder.Clear ();
			builder.AddSupportedVersion (ApplePlatform.iOS, new (16, 0));
			builder.AddUnsupportedVersion (ApplePlatform.MacOSX, new (11, 0), null);
			builder.AddSupportedVersion (ApplePlatform.MacCatalyst, new (16, 0));
			yield return [builder.ToImmutable (), "[{ Platform: MacOSX Supported: '' Unsupported: ['11.0': 'null'], Obsoleted: [] }, { Platform: iOS Supported: '16.0' Unsupported: [], Obsoleted: [] }, { Platform: MacCatalyst Supported: '16.0' Unsupported: [], Obsoleted: [] }]"];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}
	[Theory]
	[ClassData (typeof (TestDataToString))]
	void ToStringTest (SymbolAvailability availability, string expected)
		=> Assert.Equal (expected, availability.ToString ());
}
