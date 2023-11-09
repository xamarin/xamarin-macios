#if !__WATCHOS__
using System;
using System.Collections.Generic;
using Foundation;
using Network;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProxyConfigTests {
		NWRelayHop? hop;
		NWEndpoint? endpoint;
		NWProxyConfig? config;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (15, 0);

		[SetUp]
		public void SetUp ()
		{
			endpoint = NWEndpoint.Create ("https://github.com");
			hop = NWRelayHop.Create (endpoint, null, null);
			config = NWProxyConfig.CreateHttpConnect (endpoint, null);
		}

		[TearDown]
		public void TearDown ()
		{
			config?.Dispose ();
			endpoint?.Dispose ();
			hop?.Dispose ();
		}

		[Test]
		public void CreateRelayTest ()
		{
			config = null;
			Assert.DoesNotThrow (
				() => config = NWProxyConfig.CreateRelay (hop, null), "Throws");
			Assert.NotNull (config, "Not null");
		}

		[Test]
		public void CreateObliviousHttpTest ()
		{
			config = null;
			Assert.DoesNotThrow (
				() => config = NWProxyConfig.CreateObliviousHttp (hop, "", new byte [0]), "Throws");
			Assert.Null (config, "Not null");
		}

		[Test]
		public void CreateHttpConnectTest ()
		{
			config = null;
			Assert.DoesNotThrow (
				() => config = NWProxyConfig.CreateHttpConnect (endpoint, null),
			"Throws");
			Assert.NotNull (config, "Not null");
		}

		[Test]
		public void CreateSocksV5Test ()
		{
			config = null;
			Assert.DoesNotThrow (
				() => config = NWProxyConfig.CreateSocksV5 (endpoint),
			"Throws");
			Assert.NotNull (config, "Not null");
		}

		[Test]
		public void SetUsernameAndPasswordTest ()
		{
			Assert.DoesNotThrow (
				() => config.SetUsernameAndPassword ("mandel", "test"),
				"Throws on set");
			Assert.Throws<ArgumentNullException> (
				() => config.SetUsernameAndPassword (null!, "test"),
				"Throws on null username");
			Assert.Throws<ArgumentNullException> (
				() => config.SetUsernameAndPassword ("mandel", null!),
				"Throws on null password");
		}

		[Test]
		public void FailoverAllowedTest ()
		{
			config.FailoverAllowed = true;
			Assert.True (config.FailoverAllowed);
		}

		[Test]
		public void AddMatchDomainTest ()
		{
			Assert.DoesNotThrow (
				() => config.AddMatchDomain ("microsoft.com"),
				"Set match domain");

			Assert.Throws<ArgumentNullException> (
				() => config.AddMatchDomain (null!),
				"Set null match domain");
		}

		[Test]
		public void ClearMatchDomainsTest ()
		{
			Assert.DoesNotThrow (
				() => config.ClearMatchDomains (),
				"clear empty match domains");
			config.AddMatchDomain ("microsoft.com");
			Assert.DoesNotThrow (
				() => config.ClearMatchDomains (),
				"clear match domains");
		}

		[Test]
		public void AddExcludedDomainTest ()
		{
			Assert.DoesNotThrow (
				() => config.AddExcludedDomain ("microsoft.com"),
				"Set match domain");

			Assert.Throws<ArgumentNullException> (
				() => config.AddExcludedDomain (null!),
				"Set null match domain");
		}

		[Test]
		public void ClearExcludedDomainsTest ()
		{
			Assert.DoesNotThrow (
				() => config.ClearExcludedDomains (),
				"clear empty match domains");
			config.AddExcludedDomain ("microsoft.com");
			Assert.DoesNotThrow (
				() => config.ClearExcludedDomains (),
				"clear match domains");
		}

		[Test]
		public void EnumerateMatchDomainsTest ()
		{
			var domains = new [] { "microsoft.com", "msn.com", "github.com" };
			foreach (var d in domains) {
				config.AddMatchDomain (d);
			}

			// enumerate takes a cb that we will use to add the found domain to a list
			// that has been captured, then we compare the sizes
			var foundDomains = new List<string> ();
			Action<string?> cb = s => {
				if (s is not null)
					foundDomains.Add (s);
			};
			config.EnumerateMatchDomains (cb);
			Asserts.AreEqual (domains.Length, foundDomains.Count, "Domain count");
		}

		[Test]
		public void EnumerateExcludedDomainsTest ()
		{
			var domains = new [] { "microsoft.com", "msn.com", "github.com" };
			foreach (var d in domains) {
				config.AddExcludedDomain (d);
			}

			// enumerate takes a cb that we will use to add the found domain to a list
			// that has been captured, then we compare the sizes
			var foundDomains = new List<string> ();
			Action<string?> cb = s => {
				if (s is not null)
					foundDomains.Add (s);
			};
			config.EnumerateExcludedDomains (cb);
			Asserts.AreEqual (domains.Length, foundDomains.Count, "Domain count");
		}
	}
}
#endif
