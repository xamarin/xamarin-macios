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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Tests;

namespace Introspection {

	public class ApiAvailabilityTest : ApiBaseTest {

		protected Version Minimum { get; set; }
		protected Version Maximum { get; set; }
		protected Func<AvailabilityBaseAttribute, bool> Filter { get; set; }
		protected PlatformName Platform { get; set; }

		public ApiAvailabilityTest ()
		{
			Maximum = Version.Parse (Constants.SdkVersion);
#if __MACCATALYST__
			Platform = PlatformName.MacCatalyst;
			Minimum = Xamarin.SdkVersions.MinMacCatalystVersion;
#elif __IOS__
			Platform = PlatformName.iOS;
			Minimum = Xamarin.SdkVersions.MiniOSVersion;
#elif __TVOS__
			Platform = PlatformName.TvOS;
			Minimum = Xamarin.SdkVersions.MinTVOSVersion;
#elif __WATCHOS__
			Platform = PlatformName.WatchOS;
			Minimum = Xamarin.SdkVersions.MinWatchOSVersion;
#elif MONOMAC
			Platform = PlatformName.MacOSX;
			Minimum = Xamarin.SdkVersions.MinOSXVersion;
#else
			#error No Platform Defined
#endif

			Filter = (AvailabilityBaseAttribute arg) => {
				return (arg.AvailabilityKind != AvailabilityKind.Introduced) || (arg.Platform != Platform);
			};
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

		void CheckIntroduced (Type t, AvailabilityBaseAttribute ta, MemberInfo m)
		{
			var ma = CheckAvailability (m);
			if (ta == null || ma == null)
				return;

			// need to skip members that are copied to satisfy interfaces (protocol members)
			if (FoundInProtocols (m, t))
				return;

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
						return;
					}
					break;
				case "MetricKit.MXUnitAveragePixelLuminance":
				case "MetricKit.MXUnitSignalBars":
					// design bug wrt generics leading to redefinition of some members in subclasses
					if (m.ToString () == "System.String Symbol")
						return;
					break;
				}
				AddErrorLine ($"[FAIL] {ma.Version} ({m}) < {ta.Version} ({t})");
			}
		}

		[Test]
#if NET
		[Ignore ("Requires attributes update - see status in https://github.com/xamarin/xamarin-macios/issues/10834")]
#endif
		public void Introduced ()
		{
			//LogProgress = true;
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (LogProgress)
					Console.WriteLine ($"T: {t}");
				var ta = CheckAvailability (t);

				foreach (var p in t.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (LogProgress)
						Console.WriteLine ($"P: {p}");
					CheckIntroduced (t, ta, p);
				}

				// this checks getter and setters which have copies of availability attributes (in legacy)
				foreach (var m in t.GetMembers (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (LogProgress)
						Console.WriteLine ($"M: {m}");
					CheckIntroduced (t, ta, m);
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

		string ToString (ICustomAttributeProvider cap)
		{
			var s = cap.ToString ();
			if (cap is MemberInfo mi) {
				var i = s.IndexOf (' ');
				if (i != -1) {
					// a method/property without the declaring type is hard to track down
					s = s.Insert (i + 1, mi.DeclaringType + "::");
				}
			}
			return s;
		}

		protected AvailabilityBaseAttribute CheckAvailability (ICustomAttributeProvider cap)
		{
			var attrs = cap.GetCustomAttributes (false);
			foreach (var ca in attrs) {
				var a = ca;
#if NET
				a = (a as OSPlatformAttribute)?.Convert ();
#endif
				if (a is AvailabilityBaseAttribute aa) {
					if (Filter (aa))
						continue;
					// FIXME should be `<=` but that another large change best done in a different PR
					if ((aa.AvailabilityKind == AvailabilityKind.Introduced) && (aa.Version < Minimum)) {
						switch (aa.Architecture) {
						case PlatformArchitecture.All:
						case PlatformArchitecture.None:
							AddErrorLine ($"[FAIL] {aa.Version} <= {Minimum} (Min) on '{ToString (cap)}'.");
							break;
						default:
							// An old API still needs the annotations when not available on all architectures
							// e.g. NSMenuView is macOS 10.0 but only 32 bits
							break;
						}
					}
					if (aa.Version > Maximum)
						AddErrorLine ($"[FAIL] {aa.Version} > {Maximum} (Max) on '{ToString (cap)}'.");
					return aa;
				}
			}
			return null;
		}

		bool IsUnavailable (ICustomAttributeProvider cap)
		{
			foreach (var a in cap.GetCustomAttributes (false)) {
				var ca = a;
#if NET
				ca = (a as OSPlatformAttribute)?.Convert ();
#endif
				if (ca is UnavailableAttribute ua) {
					if (ua.Platform == Platform)
						return true;
				}
			}
			return false;
		}

		AvailabilityBaseAttribute GetAvailable (ICustomAttributeProvider cap)
		{
			foreach (var a in cap.GetCustomAttributes (false)) {
				var ca = a;
#if NET
				ca = (a as OSPlatformAttribute)?.Convert ();
#endif
				if (ca is AvailabilityBaseAttribute aa) {
					if ((aa.AvailabilityKind != AvailabilityKind.Unavailable) && (aa.Platform == Platform))
						return aa;
				}
			}
			return null;
		}

		void CheckUnavailable (Type t, bool typeUnavailable, MemberInfo m)
		{
			var ma = GetAvailable (m);
			if (typeUnavailable && (ma != null))
				AddErrorLine ($"[FAIL] {m} is marked with {ma} but the type {t.FullName} is [Unavailable ({Platform})].");

			var mu = IsUnavailable (m);
			if (mu && (ma != null))
				AddErrorLine ($"[FAIL] {m} is marked both [Unavailable ({Platform})] and {ma}.");
		}

		[Test]
		public void Unavailable ()
		{
			//LogProgress = true;
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (LogProgress)
					Console.WriteLine ($"T: {t}");
				var tu = IsUnavailable (t);
				var ta = GetAvailable (t);
				if (tu && (ta != null))
					AddErrorLine ($"[FAIL] {t.FullName} is marked both [Unavailable ({Platform})] and {ta}.");

				foreach (var p in t.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (LogProgress)
						Console.WriteLine ($"P: {p}");
					CheckUnavailable (t, tu, p);
				}

				foreach (var m in t.GetMembers (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (LogProgress)
						Console.WriteLine ($"M: {m}");
					CheckUnavailable (t, tu, m);
				}
			}
			AssertIfErrors ("{0} API with mixed [Unavailable] and availability attributes", Errors);
		}

		static HashSet<string> member_level = new HashSet<string> ();

		void CheckDupes (MemberInfo m, Type t, ISet<string> type_level)
		{
			member_level.Clear ();
			foreach (var a in m.GetCustomAttributes (false)) {
				var s = String.Empty;
#if NET
				if (a is OSPlatformAttribute aa)
					s = $"[{a.GetType().Name} (\"{aa.PlatformName}\")]";
#else
				if (a is AvailabilityBaseAttribute aa)
					s = aa.ToString ();
#endif
				if (s.Length > 0) {
					if (type_level.Contains (s))
						AddErrorLine ($"[FAIL] Both '{t}' and '{m}' are marked with `{s}`.");
					if (member_level.Contains (s))
						AddErrorLine ($"[FAIL] '{m}' is decorated more than once with `{s}`.");
					else
						member_level.Add (s);
				}
			}
		}

		[Test]
		public void Duplicates ()
		{
			HashSet<string> type_level = new HashSet<string> ();
			//LogProgress = true;
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (LogProgress)
					Console.WriteLine ($"T: {t}");

				type_level.Clear ();
				foreach (var a in t.GetCustomAttributes (false)) {
#if NET
					if (a is OSPlatformAttribute aa)
						type_level.Add ($"[{a.GetType().Name} (\"{aa.PlatformName}\")]");
#else
					if (a is AvailabilityBaseAttribute aa)
						type_level.Add (aa.ToString ());
#endif
				}

				foreach (var p in t.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
					if (LogProgress)
						Console.WriteLine ($"P: {p}");
					CheckDupes (p, t, type_level);
				}

				foreach (var m in t.GetMembers (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
					if (LogProgress)
						Console.WriteLine ($"M: {m}");
					CheckDupes (m, t, type_level);
				}
			}
			AssertIfErrors ("{0} API with members duplicating type-level attributes", Errors);
		}

#if NET
		string CheckLegacyAttributes (ICustomAttributeProvider cap)
		{
			var sb = new StringBuilder ();
			foreach (var a in cap.GetCustomAttributes (false)) {
				if (a is AvailabilityBaseAttribute aa) {
					sb.AppendLine (aa.ToString ());
				}
			}
			return sb.ToString ();
		}

		[Test]
		[Ignore ("work in progress")]
		public void LegacyAttributes ()
		{
			//LogProgress = true;
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (LogProgress)
					Console.WriteLine ($"T: {t}");
				var type_level = CheckLegacyAttributes (t);
				if (type_level.Length > 0)
					AddErrorLine ($"[FAIL] '{t.FullName}' has legacy attribute(s): {type_level}");

				foreach (var p in t.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
					if (LogProgress)
						Console.WriteLine ($"P: {p}");

					var member_level = CheckLegacyAttributes (p);
					if (member_level.Length > 0)
						AddErrorLine ($"[FAIL] '{t.FullName}::{p.Name}' has legacy attribute(s): {member_level}");
				}

				foreach (var m in t.GetMembers (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
					if (LogProgress)
						Console.WriteLine ($"M: {m}");

					var member_level = CheckLegacyAttributes (m);
					if (member_level.Length > 0)
						AddErrorLine ($"[FAIL] '{t.FullName}::{m.Name}' has legacy attribute(s): {member_level}");
				}
			}
			AssertIfErrors ("{0} API with mixed legacy availability attributes", Errors);
		}
#endif
	}
}
