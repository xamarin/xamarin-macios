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

#if !NET // this test is in cecil-tests in .NET

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

using NUnit.Framework;

using ObjCRuntime;

using Xamarin.Tests;
using Xamarin.Utils;

using ApplePlatform = ObjCRuntime.PlatformName;

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
				if (p is not null) {
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
				if (p is not null) {
					// here we want inherited members so we don't have to hunt inherited interfaces recursively
					foreach (var pm in p.GetMembers ()) {
						if (pm.ToString () != method)
							continue;
						return true;
					}
				}
				p = Assembly.GetType (intf.Namespace + "." + intf.Name.Substring (1) + "_Extensions");
				if (p is not null) {
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
			if (ta is null || ma is null)
				return;

			// need to skip members that are copied to satisfy interfaces (protocol members)
			if (FoundInProtocols (m, t))
				return;

			var taVersion = ta.Version;
			var maVersion = ma.Version;
			// Duplicate checks, e.g. same attribute on member and type (extranous metadata)
			if (maVersion == taVersion) {
				switch (t.FullName) {
				case "AppKit.INSAccessibility":
					// special case for [I]NSAccessibility type (10.9) / protocol (10.10) mix up
					// https://github.com/xamarin/xamarin-macios/issues/10009
					// better some dupes than being inaccurate when protocol members are inlined
					break;
				default:
					AddErrorLine ($"[FAIL] {maVersion} ({m}) == {taVersion} ({t})");
					break;
				}
			}
			// Consistency checks, e.g. member lower than type
			// note: that's valid in some cases, like a new base type being introduced
			if (maVersion < taVersion) {
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
				AddErrorLine ($"[FAIL] {maVersion} ({m}) < {taVersion} ({t})");
			}
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
				if (!(ca is AvailabilityBaseAttribute aa))
					continue;
				if (Filter (aa))
					continue;

				// FIXME should be `<=` but that another large change best done in a different PR
				bool isAvailableBeforeMinimum = false;
				var aaVersion = aa.Version;
				if ((aa.AvailabilityKind == AvailabilityKind.Introduced) && (aaVersion < Minimum)) {
					switch (aa.Architecture) {
					case PlatformArchitecture.All:
					case PlatformArchitecture.None:
						isAvailableBeforeMinimum = true;
						break;
					default:
						// An old API still needs the annotations when not available on all architectures
						// e.g. NSMenuView is macOS 10.0 but only 32 bits
						break;
					}
				}
				if (isAvailableBeforeMinimum)
					AddErrorLine ($"[FAIL] {aaVersion} <= {Minimum} (Min) on '{ToString (cap)}'.");
				if (aaVersion > Maximum)
					AddErrorLine ($"[FAIL] {aaVersion} > {Maximum} (Max) on '{ToString (cap)}'.");
				return aa;
			}
			return null;
		}

		bool IsUnavailable (ICustomAttributeProvider cap, out Version? version)
		{
			version = null;
			foreach (var a in cap.GetCustomAttributes (false)) {
				var ca = a;
				if (ca is UnavailableAttribute ua) {
					if (ua.Platform == Platform)
						return true;
				}
			}
			return false;
		}

		AvailabilityBaseAttribute GetAvailable (ICustomAttributeProvider cap, out Version? version)
		{
			version = null;
			foreach (var a in cap.GetCustomAttributes (false)) {
				var ca = a;
				if (ca is AvailabilityBaseAttribute aa) {
					if ((aa.AvailabilityKind != AvailabilityKind.Unavailable) && (aa.Platform == Platform))
						return aa;
				}
			}
			return null;
		}

		void CheckUnavailable (Type t, bool typeUnavailable, Version? typeUnavailableVersion, MemberInfo m)
		{
			// Turns out Version (13, 1, 0) > Version (13, 1) since undefined fields are -1
			// However, we consider them equal, so force a 0 Build if set to -1
			if (typeUnavailableVersion is not null && typeUnavailableVersion.Build == -1) {
				typeUnavailableVersion = new Version (typeUnavailableVersion.Major, typeUnavailableVersion.Minor, 0);
			}

			var ma = GetAvailable (m, out var availableVersion);
			if (typeUnavailable && (ma is not null)) {
				if (typeUnavailableVersion is not null && availableVersion is not null) {
					if (availableVersion >= typeUnavailableVersion)
						AddErrorLine ($"[FAIL] {m} in {m.DeclaringType.FullName} is marked with {ma} in {availableVersion} but the type {t.FullName} is [Unavailable ({Platform})] in {typeUnavailableVersion}");
				} else {
					AddErrorLine ($"[FAIL] {m} in {m.DeclaringType.FullName} is marked with {ma} but the type {t.FullName} is [Unavailable ({Platform})]");
				}
			}

			var mu = IsUnavailable (m, out var unavailableVersion);
			if (mu && (ma is not null)) {
				if (availableVersion is not null && unavailableVersion is not null) {
					// Apple is introducing and deprecating numerous APIs in the same Mac Catalyst version,
					// so specifically for Mac Catalyst, we do a simple 'greater than' version check,
					// instead of a 'greater than or equal' version like we do for the other platforms.

					if (Platform == ApplePlatform.MacCatalyst) {
						if (availableVersion > unavailableVersion)
							AddErrorLine ($"[FAIL] {m} is marked both [Unavailable ({Platform})] and {ma}, and it's available in version {availableVersion} which is > than the unavailable version {unavailableVersion}");
					} else {
						if (availableVersion >= unavailableVersion)
							AddErrorLine ($"[FAIL] {m} is marked both [Unavailable ({Platform})] and {ma}, and it's available in version {availableVersion} which is >= than the unavailable version {unavailableVersion}");
					}
				} else {
					// As documented in https://docs.microsoft.com/en-us/dotnet/standard/analyzers/platform-compat-analyzer#advanced-scenarios-for-attribute-combinations
					// it is valid, and required in places to declare a type both availabile and unavailable on a given platform.
					// Example:
					// 		[SupportedOSPlatform ("macos")]
					// 		[UnsupportedOSPlatform ("macos10.13")]
					// This API was introduced on macOS but became unavailable on 10.13
					// The legacy attributes described this with Deprecated, and did not need to double declare
					AddErrorLine ($"[FAIL] {m} in {m.DeclaringType.FullName} is marked both [Unavailable ({Platform})] and {ma}.");
				}
			}
		}

		[Test]
		public void Unavailable ()
		{
			//LogProgress = true;
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (SkipUnavailable (t))
					continue;
				if (LogProgress)
					Console.WriteLine ($"T: {t}");
				var tu = IsUnavailable (t, out var unavailableVersion);
				var ta = GetAvailable (t, out var availableVersion);
				if (tu && (ta is not null)) {
					if (availableVersion is not null && unavailableVersion is not null) {
						// Apple is introducing and deprecating numerous APIs in the same Mac Catalyst version,
						// so specifically for Mac Catalyst, we do a simple 'greater than' version check,
						// instead of a 'greater than or equal' version like we do for the other platforms.
						if (Platform == ApplePlatform.MacCatalyst) {
							if (availableVersion > unavailableVersion)
								AddErrorLine ($"[FAIL] {t.FullName} is marked both [Unavailable ({Platform})] and {ta}, and it's available in version {availableVersion} which is > than the unavailable version {unavailableVersion}");

						} else {
							if (availableVersion >= unavailableVersion)
								AddErrorLine ($"[FAIL] {t.FullName} is marked both [Unavailable ({Platform})] and {ta}, and it's available in version {availableVersion} which is >= than the unavailable version {unavailableVersion}");
						}
					} else {
						// As documented in https://docs.microsoft.com/en-us/dotnet/standard/analyzers/platform-compat-analyzer#advanced-scenarios-for-attribute-combinations
						// it is valid, and required in places to declare a type both availabile and unavailable on a given platform.
						// Example:
						// 		[SupportedOSPlatform ("macos")]
						// 		[UnsupportedOSPlatform ("macos10.13")]
						// This API was introduced on macOS but became unavailable on 10.13
						// The legacy attributes described this with Deprecated, and did not need to double declare
						AddErrorLine ($"[FAIL] {t.FullName} is marked both [Unavailable ({Platform})] and {ta}. Available: {availableVersion} Unavailable: {unavailableVersion}");
					}
				}

				foreach (var p in t.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (SkipUnavailable (t, p.Name))
						continue;
					if (LogProgress)
						Console.WriteLine ($"P: {p.Name}");
					CheckUnavailable (t, tu, unavailableVersion, p);
				}

				foreach (var m in t.GetMembers (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
					if (SkipUnavailable (t, m.Name))
						continue;
					if (LogProgress)
						Console.WriteLine ($"M: {m.Name}");
					CheckUnavailable (t, tu, unavailableVersion, m);
				}
			}
			AssertIfErrors ("{0} API with mixed [Unavailable] and availability attributes", Errors);
		}

		protected virtual bool SkipUnavailable (Type type)
		{
#if __MACCATALYST__
			switch (type.Namespace) {
			case "AddressBook": {
				// The entire framework was introduced and deprecated in the same Mac Catalyst version
				return true;
			}
			}
#endif

			switch (type.FullName) {
#if __MACCATALYST__
			case "SafariServices.SFContentBlockerErrorCode":
			case "SafariServices.SFContentBlockerErrorCodeExtensions":
				// introduced and deprecated in the same Mac Catalyst version
				return true;
#endif
			}
			return false;
		}

		protected virtual bool SkipUnavailable (Type type, string memberName)
		{
			switch (type.FullName) {
#if __MACOS__
			case "AppKit.NSDrawer":
				switch (memberName) {
				case "AccessibilityChildrenInNavigationOrder":
				case "get_AccessibilityChildrenInNavigationOrder":
				case "set_AccessibilityChildrenInNavigationOrder":
				case "AccessibilityCustomActions":
				case "get_AccessibilityCustomActions":
				case "set_AccessibilityCustomActions":
				case "AccessibilityCustomRotors":
				case "get_AccessibilityCustomRotors":
				case "set_AccessibilityCustomRotors":
					// NSDrawer was deprecated in macOS 10.13, but implements (and inlines) NSAccessibility, which added several new members in macOS 10.13, so ignore those members here.
					return true;
				}
				break;
			case "GLKit.GLKTextureLoader":
				switch (memberName) {
				case "GrayscaleAsAlpha":
				case "get_GrayscaleAsAlpha":
					// GLKTextureLoader is deprecated, but the GLKTextureLoaderGrayscaleAsAlpha value, which we've put inside the GLKTextureLoader class, isn't.
					return true;
				}
				break;
#endif
#if __MACCATALYST__
			case "AudioUnit.AudioComponent":
				switch (memberName) {
				case "LastActiveTime":
					// introduced and deprecated in the same Mac Catalyst version
					return true;
				}
				break;
			// Apple itself is inconsistent in the availability of the type compared to these selectors
			case "AVFoundation.AVCaptureStillImageOutput":
				switch (memberName) {
				case "AutomaticallyEnablesStillImageStabilizationWhenAvailable":
				case "CapturingStillImage":
				case "HighResolutionStillImageOutputEnabled":
				case "IsStillImageStabilizationActive":
				case "IsStillImageStabilizationSupported":
					return true;
				}
				break;
#endif
			case "CarPlay.CPApplicationDelegate":
				switch (memberName) {
				case "DidDiscardSceneSessions":
				case "GetConfiguration":
				case "GetHandlerForIntent":
				case "ShouldAutomaticallyLocalizeKeyCommands":
				case "ShouldRestoreSecureApplicationState":
				case "ShouldSaveSecureApplicationState":
					// CPApplicationDelegate is deprecated in macOS 10.15, but these members are pulled in from the UIApplicationDelegate protocol (which is not deprecated)
					return true;
				}
				break;
			case "CoreMedia.CMTimebase": {
				switch (memberName) {
				case "SetMasterTimebase":
				case "SetMasterClock":
					// These APIs were introduced and deprecated in the same version
					return true;
				}
				break;
			}
			case "GameKit.GKScore": {
				switch (memberName) {
				case "ReportLeaderboardScores":
				case "ReportLeaderboardScoresAsync":
					// Apple introduced and deprecated this method in the same OS version.
					return true;
				}
				break;
			}
			case "Intents.INNoteContentTypeResolutionResult": {
				switch (memberName) {
				case "GetConfirmationRequired":
				case "GetUnsupported":
					// These are static members that have been re-implemented from the base class - the base class isn't deprecated, while INNoteContentTypeResolutionResult is.
					return true;
				}
				break;
			}
			case "MobileCoreServices.UTType": {
				switch (memberName) {
				case "UniversalSceneDescriptionMobile":
				case "get_UniversalSceneDescriptionMobile":
					// Apple added new members to a deprecated enum
					return true;
				}
				break;
			}
			case "SceneKit.SCNLayer": {
				switch (memberName) {
				case "CurrentViewport":
				case "TemporalAntialiasingEnabled":
				case "get_CurrentViewport":
				case "get_TemporalAntialiasingEnabled":
				case "set_TemporalAntialiasingEnabled":
				case "get_UsesReverseZ":
				case "set_UsesReverseZ":
				case "UsesReverseZ":
					// SCNLayer is deprecated in macOS 10.15, but these members are pulled in from the SCNSceneRenderer protocol (which is not deprecated)
					return true;
				}
				break;
			}
			}
			return false;
		}

		static HashSet<string> member_level = new HashSet<string> ();

		void CheckDupes (MemberInfo m, Type t, ISet<string> type_level)
		{
			member_level.Clear ();
			foreach (var a in m.GetCustomAttributes (false)) {
				var s = String.Empty;
				if (a is AvailabilityBaseAttribute aa)
					s = aa.ToString ();
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
					if (a is AvailabilityBaseAttribute aa)
						type_level.Add (aa.ToString ());
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
	}
}

#endif // !NET
