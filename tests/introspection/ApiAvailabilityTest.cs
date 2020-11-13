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
			// Need to special case watchOS 'Maximum' version for OS minor subversions (can't change Constants.SdkVersion)
			//Maximum = new Version (6,2,5);
			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != PlatformName.WatchOS);
			};
#else
			Minimum = new Version (10,9);
			// Need to special case macOS 'Maximum' version for OS minor subversions (can't change Constants.SdkVersion)
			// Please comment the code below if needed
			Maximum = new Version (11,0,0);
			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != PlatformName.MacOSX);
			};
#endif
		}

		bool FoundInProtocols (MemberInfo m, Type t)
		{
			var method = m.ToString ();
			foreach (var intf in t.GetInterfaces ()) {
				var p = Assembly.GetType (intf.FullName);
				if (p != null) {
					// here we want inherited members so we don't have to hunt inherited interfaces recursively
					foreach (var pm in p.GetMembers ()) {
						if (pm.ToString () != method)
							continue;
						return true;
					}
					foreach (var ca in p.GetCustomAttributes<Foundation.ProtocolMemberAttribute> ()) {
						// TODO check signature in [ProtocolMember]
						if (ca.IsProperty) {
							if (m.Name == "get_" + ca.Name)
								return true;
							if (m.Name == "set_" + ca.Name)
								return true;
						}
						if (m.Name == ca.Name)
							return true;
					}
				}
				p = Assembly.GetType (intf.Namespace + "." + intf.Name.Substring (1));
				if (p != null) {
					// here we want inherited members so we don't have to hunt inherited interfaces recursively
					foreach (var pm in p.GetMembers ()) {
						if (pm.ToString () != method)
							continue;
						return true;
					}
				}
				p = Assembly.GetType (intf.Namespace + "." + intf.Name.Substring (1) + "_Extensions");
				if (p != null) {
					// here we want inherited members so we don't have to hunt inherited interfaces recursively
					foreach (var pm in p.GetMembers ()) {
						// map extension method to original @optional
						if (m.Name != pm.Name)
							continue;
						var parameters = (pm as MethodInfo).GetParameters ();
						if (parameters.Length == 0)
							continue;
						var pattern = "(" + parameters [0].ParameterType.FullName;
						if (parameters.Length > 1)
							pattern += ", ";
						var s = pm.ToString ().Replace (pattern, "(");
						if (s != method)
							continue;
						return true;
					}
				}
			}
			return false;
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
				foreach (var m in t.GetMembers (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (LogProgress)
						Console.WriteLine ($"M: {m}");
					var ma = CheckAvailability (m);
					if (ta == null || ma == null)
						continue;

					// need to skip members that are copied to satisfy interfaces (protocol members)
					if (FoundInProtocols (m, t))
						continue;

					// Duplicate checks, e.g. same attribute on member and type (extranous metadata)
					if (ma.Version == ta.Version) {
						switch (t.FullName) {
						case "AppKit.INSAccessibility":
							// special case for [I]NSAccessibility type (10.9) / protocol (10.10) mix up
							// https://github.com/xamarin/xamarin-macios/issues/10009
							// better some dupes than being inaccurate when protocol members are inlined
							break;
						default:
							AddErrorLine ($"[FAIL] {ma.Version} ({m}) == {ta.Version} ({t})");
							break;
						}
					}
					// Consistency checks, e.g. member lower than type
					// note: that's valid in some cases, like a new base type being introduced
					if (ma.Version < ta.Version) {
						switch (t.FullName) {
						case "CoreBluetooth.CBPeer":
							switch (m.ToString ()) {
							// type added later and existing property was moved
							case "Foundation.NSUuid get_Identifier()":
							case "Foundation.NSUuid Identifier":
								continue;
							}
							break;
						case "MetricKit.MXUnitAveragePixelLuminance":
						case "MetricKit.MXUnitSignalBars":
							// design bug wrt generics leading to redefinition of some members in subclasses
							if (m.ToString () == "System.String Symbol")
								continue;
							break;
						}
						AddErrorLine ($"[FAIL] {ma.Version} ({m}) < {ta.Version} ({t})");
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
