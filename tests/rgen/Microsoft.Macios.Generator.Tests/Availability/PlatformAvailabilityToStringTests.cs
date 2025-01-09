// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class PlatformAvailabilityToStringTests {

	class TestDataToString : IEnumerable<object []> {

		public IEnumerator<object []> GetEnumerator ()
		{
			// build platform availabilities to ensure that the string returned is the
			// expected one. We will use a builder that we can clear after each yield return
			var builder = PlatformAvailability.CreateBuilder (ApplePlatform.iOS);
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: [], Obsoleted: [] }"];

			builder.Clear ();
			builder.AddSupportedVersion (new Version (16, 0));
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '16.0', Unsupported: [], Obsoleted: [] }"];

			builder.Clear ();
			builder.AddUnsupportedVersion (new Version (16, 0), null);
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: ['16.0': 'null'], Obsoleted: [] }"];

			builder.Clear ();
			builder.AddUnsupportedVersion (new Version (16, 0), "Not supported.");
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: ['16.0': 'Not supported.'], Obsoleted: [] }"];

			builder.Clear ();
			builder.AddUnsupportedVersion (new Version (16, 0), "Not supported.");
			builder.AddUnsupportedVersion (new Version (18, 0), "Not supported.");
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: ['16.0': 'Not supported.', '18.0': 'Not supported.'], Obsoleted: [] }"];

			builder.Clear ();
			builder.AddObsoletedVersion (new Version (16, 0), null, null);
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: [], Obsoleted: ['16.0': ('null', 'null')] }"];

			builder.Clear ();
			builder.AddObsoletedVersion (new Version (16, 0), "Obsoleted method", null);
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: [], Obsoleted: ['16.0': ('Obsoleted method', 'null')] }"];

			builder.Clear ();
			builder.AddObsoletedVersion (new Version (16, 0), "Obsoleted method", "https://bing.com");
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: [], Obsoleted: ['16.0': ('Obsoleted method', 'https://bing.com')] }"];

			builder.Clear ();
			builder.AddObsoletedVersion (new Version (16, 0), "Obsoleted method", "https://bing.com");
			builder.AddObsoletedVersion (new Version (18, 0), "Obsoleted method", "https://bing.com");
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '', Unsupported: [], Obsoleted: ['16.0': ('Obsoleted method', 'https://bing.com'), '18.0': ('Obsoleted method', 'https://bing.com')] }"];

			builder.Clear ();
			builder.AddSupportedVersion (new Version (16, 0));
			builder.AddUnsupportedVersion (new Version (16, 0), "Not supported.");
			builder.AddUnsupportedVersion (new Version (18, 0), "Not supported.");
			builder.AddObsoletedVersion (new Version (16, 0), "Obsoleted method", "https://bing.com");
			builder.AddObsoletedVersion (new Version (18, 0), "Obsoleted method", "https://bing.com");
			yield return [builder.ToImmutable (), "{ Platform: 'iOS', Supported: '16.0', Unsupported: ['16.0': 'Not supported.', '18.0': 'Not supported.'], Obsoleted: ['16.0': ('Obsoleted method', 'https://bing.com'), '18.0': ('Obsoleted method', 'https://bing.com')] }"];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataToString))]
	void ToStringTest (PlatformAvailability availability, string expected)
		=> Assert.Equal (expected, availability.ToString ());
}
