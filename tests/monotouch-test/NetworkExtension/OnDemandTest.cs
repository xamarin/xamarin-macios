//
// Unit tests for OnDemand VPN / rules
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

using System;
using Foundation;
using NetworkExtension;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.NetworkExtension {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OnDemandTest {

		void OnDemandRule (NEOnDemandRule rule)
		{
			Assert.Null (rule.DnsSearchDomainMatch, "DnsSearchDomainMatch");
			rule.DnsSearchDomainMatch = new string [] { "1" };
			Assert.That (rule.DnsSearchDomainMatch.Length, Is.EqualTo (1), "DnsSearchDomainMatch-2");
			rule.DnsSearchDomainMatch = null;
			Assert.Null (rule.DnsSearchDomainMatch, "DnsSearchDomainMatch-3");

			Assert.Null (rule.DnsServerAddressMatch, "");
			rule.DnsServerAddressMatch = new string [] { "1", "2" };
			Assert.That (rule.DnsServerAddressMatch.Length, Is.EqualTo (2), "DnsServerAddressMatch-2");
			rule.DnsServerAddressMatch = null;
			Assert.Null (rule.DnsServerAddressMatch, "DnsServerAddressMatch-3");

			Assert.That (rule.InterfaceTypeMatch, Is.EqualTo ((NEOnDemandRuleInterfaceType) 0), "InterfaceTypeMatch");
			rule.InterfaceTypeMatch = NEOnDemandRuleInterfaceType.WiFi;
			Assert.That (rule.InterfaceTypeMatch, Is.EqualTo (NEOnDemandRuleInterfaceType.WiFi), "InterfaceTypeMatch-2");

			Assert.Null (rule.ProbeUrl, "ProbeUrl");
			using (var url = new NSUrl ("http://www.xamarin.com")) {
				rule.ProbeUrl = url;
				Assert.AreSame (url, rule.ProbeUrl, "ProbeUrl-2");
			}
			rule.ProbeUrl = null;
			Assert.Null (rule.ProbeUrl, "ProbeUrl-3");

			Assert.Null (rule.SsidMatch, "SsidMatch");
			rule.SsidMatch = new string [] { "1", "2", "3" };
			Assert.That (rule.SsidMatch.Length, Is.EqualTo (3), "SsidMatch-2");
			rule.SsidMatch = null;
			Assert.Null (rule.SsidMatch, "SsidMatch-3");
		}

		[Test]
		public void OnDemandRuleConnect ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			using (var rule = new NEOnDemandRuleConnect ()) {
				Assert.That (rule.Action, Is.EqualTo (NEOnDemandRuleAction.Connect), "Action");
				OnDemandRule (rule);
			}
		}

		[Test]
		public void OnDemandRuleDisconnect ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			using (var rule = new NEOnDemandRuleDisconnect ()) {
				Assert.That (rule.Action, Is.EqualTo (NEOnDemandRuleAction.Disconnect), "Action");
				OnDemandRule (rule);
			}
		}

		[Test]
		public void OnDemandRuleIgnore ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			using (var rule = new NEOnDemandRuleIgnore ()) {
				Assert.That (rule.Action, Is.EqualTo (NEOnDemandRuleAction.Ignore), "Action");
				OnDemandRule (rule);
			}
		}

		[Test]
		public void NEOnDemandRuleEvaluateConnection ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			using (var rule = new NEOnDemandRuleEvaluateConnection ()) {
				Assert.That (rule.Action, Is.EqualTo (NEOnDemandRuleAction.EvaluateConnection), "Action");
				OnDemandRule (rule);
				// 
				Assert.IsNull (rule.ConnectionRules, "ConnectionRules");
				rule.ConnectionRules = new NEEvaluateConnectionRule [] {
					new NEEvaluateConnectionRule ()
				};
				Assert.IsNotNull (rule.ConnectionRules, "ConnectionRules-2");
				rule.ConnectionRules = null;
				Assert.IsNull (rule.ConnectionRules, "ConnectionRules-3");
			}
		}

		[Test]
		public void EvaluateConnectionRule_Default ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			using (var rule = new NEEvaluateConnectionRule ()) {
				Assert.That (rule.Action, Is.EqualTo ((NEEvaluateConnectionRuleAction) 0), "Action");
				Assert.Null (rule.MatchDomains, "MatchDomains");
				Assert.Null (rule.ProbeUrl, "ProbeUrl");
				Assert.Null (rule.UseDnsServers, "UseDnsServers");
			}
		}

		[Test]
		public void EvaluateConnectionRule ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			using (var rule = new NEEvaluateConnectionRule (new [] { "xamarin.com" }, NEEvaluateConnectionRuleAction.ConnectIfNeeded)) {
				Assert.That (rule.Action, Is.EqualTo (NEEvaluateConnectionRuleAction.ConnectIfNeeded), "Action");
				Assert.That (rule.MatchDomains [0], Is.EqualTo ("xamarin.com"), "MatchDomains");
				Assert.Null (rule.ProbeUrl, "ProbeUrl");
				using (var url = new NSUrl ("http://www.xamarin.com")) {
					rule.ProbeUrl = url;
					Assert.AreSame (url, rule.ProbeUrl, "ProbeUrl-2");
				}
				rule.ProbeUrl = null;
				Assert.Null (rule.ProbeUrl, "ProbeUrl-3");

				Assert.Null (rule.UseDnsServers, "UseDnsServers");
				rule.UseDnsServers = new [] { "8.8.8.8" };
				Assert.That (rule.UseDnsServers [0], Is.EqualTo ("8.8.8.8"), "UseDnsServers-2");
				rule.UseDnsServers = null;
				Assert.Null (rule.UseDnsServers, "UseDnsServers-3");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
