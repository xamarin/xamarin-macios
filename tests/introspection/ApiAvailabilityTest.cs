//
// Availability tests for introspection
//
// Authors:
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2017 Microsoft Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#if XAMCORE_2_0

using System;
using System.Reflection;
using NUnit.Framework;
using ObjCRuntime;

namespace Introspection {

	public class ApiAvailabilityTest : ApiBaseTest {
	
		protected Version Minimum { get; set; }
		protected Version Maximum { get; set; }
		protected Func<AvailabilityBaseAttribute,bool> Filter { get; set; }

		public ApiAvailabilityTest ()
		{
			Maximum = Version.Parse (Constants.SdkVersion);
#if __IOS__
			Minimum = new Version (6,0);
			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != PlatformName.iOS);
			};
#elif __TVOS__
			Minimum = new Version (9,0);
			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != PlatformName.TvOS);
			};
#elif __WATCHOS__
			Minimum = new Version (2,0);
			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != PlatformName.WatchOS);
			};
#else
			Minimum = new Version (10,7);
			// Need to special case macOS 'Maximum' version for OS minor subversions (can't change Constants.SdkVersion)
			// Please comment the code below if needed
			Maximum = new Version (10,14,4);
			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != PlatformName.MacOSX);
			};
#endif
		}

		[Test]
		public void Introduced ()
		{
			//LogProgress = true;
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (LogProgress)
					Console.WriteLine ($"T: {t}");
				var ta = CheckAvailability (t);
				foreach (var m in t.GetMembers ()) {
					if (LogProgress)
						Console.WriteLine ($"M: {m}");
					var ma = CheckAvailability (m);
					if (ta == null || ma == null)
						continue;
					// Duplicate checks, e.g. same attribute on member and type (extranous metadata)
					if (ma.Version == ta.Version) {
// about 4000
//						AddErrorLine ($"[FAIL] {ma.Version} (Member) == {ta.Version} (Type) on '{m}'.");
					}
					// Consistency checks, e.g. member lower than type
					// note: that's valid in some cases, like a new base type being introduced
					if (ma.Version < ta.Version) {
// about 8000
//						AddErrorLine ($"[FAIL] {ma.Version} (Member) < {ta.Version} (Type) on '{m}'.");
					}
				}
			}
			AssertIfErrors ("{0} API with unneeded or incorrect version information", Errors);
		}

#if XAMCORE_4_0
		[Test]
		public void Deprecated ()
		{
			// Warn about any API deprecated before the minimum (e.g. < iOS 6).
			// Those should not be exposed in future profiles.
			Assert.Fail ("TODO");
		}
#endif

		protected AvailabilityBaseAttribute CheckAvailability (ICustomAttributeProvider cap)
		{
			var attrs = cap.GetCustomAttributes (false);
			foreach (var a in attrs) {
				if (a is AvailabilityBaseAttribute aa) {
					if (Filter (aa))
						continue;
					if (aa.Version < Minimum) {
						switch (aa.Architecture) {
						case PlatformArchitecture.All:
						case PlatformArchitecture.None:
							AddErrorLine ($"[FAIL] {aa.Version} < {Minimum} (Min) on '{cap}'.");
							break;
						default:
							// An old API still needs the annotations when not available on all architectures
							// e.g. NSMenuView is macOS 10.0 but only 32 bits
							break;
						}
					}
					if (aa.Version > Maximum)
						AddErrorLine ($"[FAIL] {aa.Version} > {Maximum} (Max) on '{cap}'.");
					return aa;
				}
			}
			return null;
		}
	}
}

#endif

