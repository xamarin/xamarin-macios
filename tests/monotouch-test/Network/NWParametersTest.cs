#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Threading;

using CoreFoundation;
using Foundation;
using Network;

using NUnit.Framework;
using MonoTests.System.Net.Http;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWParametersTest {

		AutoResetEvent secureEvent;  // used to let us know the handler was indeed called.
		AutoResetEvent configureEvent;  // used to let us know the handler was indeed called.
		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the NWPath 
		bool secureConnectionWasSet = false;
		bool protocolConfigured = false;
		List<NWInterface> interfaces = new List<NWInterface> ();
		string host;
		NWConnection connection;


		[OneTimeSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			// we want to use a single connection, since it is expensive
			connectedEvent = new AutoResetEvent (false);
			host = NetworkResources.MicrosoftUri.Host;
			interfaces = new List<NWInterface> ();
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (host, "80")) {
				connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.Start ();
				Assert.True (connectedEvent.WaitOne (20000), "Connection timed out.");
				using (var path = connection.CurrentPath) {
					path.EnumerateInterfaces (EnumerateInterfacesHandler);
				}
			}
		}

		[OneTimeTearDown]
		public void Dispose ()
		{
			connection?.Dispose ();
			if (interfaces is not null) {
				foreach (var i in interfaces)
					i.Dispose ();
			}
		}

		[SetUp]
		public void SetUp ()
		{
			secureEvent = new AutoResetEvent (false);
			configureEvent = new AutoResetEvent (false);
			secureConnectionWasSet = false;
			protocolConfigured = false;
		}

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			switch (state) {
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			case NWConnectionState.Cancelled:
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ("Network connection could not be performed.");
				break;
			}
		}

		[TearDown]
		public void TearDown ()
		{
			secureEvent = null;
			secureConnectionWasSet = false;
			protocolConfigured = false;
		}

		void EnumerateInterfacesHandler (NWInterface nwInterface)
		{
			interfaces.Add (nwInterface);
		}

		Action<NWProtocolOptions> CreateTlsHandler ()
		{
			return (NWProtocolOptions options) => {
				secureConnectionWasSet = true;
				secureEvent.Set ();
			};
		}

		Action<NWProtocolOptions> CreateConfigureProtocolHandler ()
		{
			return (NWProtocolOptions options) => {
				protocolConfigured = true;
				configureEvent.Set ();
			};
		}

		[Test]
		public void CreateSecureUpdTest ()
		{
			var setUpTls = CreateTlsHandler ();
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: setUpTls, configureUdp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				secureEvent.WaitOne ();
				configureEvent.WaitOne ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}

		[Test]
		public void CreateSecureUpdTestDoNotSetUpProtocol ()
		{
			var setUpTls = CreateTlsHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: setUpTls))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				secureEvent.WaitOne ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.False (protocolConfigured, "Protocol configure handler was called.");
			}
		}

		[Test]
		public void CreateSecureUpdTestDoNotSetUpTls ()
		{
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: null, configureUdp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				configureEvent.WaitOne ();
				Assert.False (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}

		[Test]
		public void CreateSecureTcpTest ()
		{
			var setUpTls = CreateTlsHandler ();
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureTcp (configureTls: setUpTls, configureTcp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				secureEvent.WaitOne ();
				configureEvent.WaitOne ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}

		[Test]
		public void CreateSecureTcpTestDoNotSetUpProtocol ()
		{
			var setUpTls = CreateTlsHandler ();
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureTcp (configureTls: setUpTls))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				secureEvent.WaitOne ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.False (protocolConfigured, "Protocol configure handler was called.");
			}
		}

		[Test]
		public void CreateSecureTcpTestDoNotSetUpTls ()
		{
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureTcp (configureTls: null, configureTcp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				configureEvent.WaitOne ();
				Assert.False (secureConnectionWasSet, "Configure TLS handler was called.");
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}

#if MONOMAC
		[Test]
		public void CreateCustomIP ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			byte ipVersion = 10;
			var setUpProtocol = CreateConfigureProtocolHandler ();
			using (var parameters = NWParameters.CreateCustomIP (ipVersion, setUpProtocol))
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80")) {
				configureEvent.WaitOne ();
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}
#endif

		[Test]
		public void MultiPathServicePropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.MultipathService;
				Assert.AreEqual (defaultValue, NWMultiPathService.Disabled, "Default value changed.");
				var newValue = NWMultiPathService.Aggregate;
				parameters.MultipathService = newValue;
				Assert.AreEqual (newValue, parameters.MultipathService, "New value was not stored.");
			}
		}

		[Test]
		public void ProtocolStackPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var stack = parameters.ProtocolStack;
				Assert.AreNotEqual (IntPtr.Zero, stack.Handle);
			}
		}

		[Test]
		public void LocalOnlyPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.LocalOnly;
				Assert.False (defaultValue, "Default value changed.");
				parameters.LocalOnly = true;
				Assert.True (parameters.LocalOnly, "New value was not stored.");
			}
		}

		[Test]
		public void PreferNoProxyPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.PreferNoProxy;
				Assert.False (defaultValue, "Default value changed.");
				parameters.PreferNoProxy = true;
				Assert.True (parameters.PreferNoProxy, "New value was not stored.");
			}
		}

		[Test]
		public void ExpiredDnsBehaviorPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.ExpiredDnsBehavior;
				Assert.AreEqual (NWParametersExpiredDnsBehavior.Default, defaultValue, "Default value changed.");
				parameters.ExpiredDnsBehavior = NWParametersExpiredDnsBehavior.Allow;
				Assert.AreEqual (NWParametersExpiredDnsBehavior.Allow, parameters.ExpiredDnsBehavior, "New value was not stored.");
			}
		}

		[Test]
		public void RequiredInterfacePropertyTest ()
		{

			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.RequiredInterface;
				Assert.IsNull (defaultValue, "Default value changed.");
				// try to set a null value, we should have no issues
				parameters.RequiredInterface = null;
				Assert.IsNull (parameters.RequiredInterface, "Value should still be null.");
				parameters.RequiredInterface = interfaces [0];
				Assert.AreNotEqual (IntPtr.Zero, parameters.RequiredInterface.Handle, "New value was not set.");
			}
		}

		[Test]
		public void ProhibitInterfaceTest ()
		{
			using (var parameters = new NWParameters ()) {
				Assert.Throws<ArgumentNullException> (() => parameters.ProhibitInterface (null), "");
				Assert.AreNotEqual (0, interfaces.Count, "No network interfaces found.");
				parameters.ProhibitInterface (interfaces [0]);
			}
		}

		[Test]
		public void RequiredInterfaceTypePropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.RequiredInterfaceType;
				Assert.AreEqual (NWInterfaceType.Other, defaultValue, "Default value changed.");
				parameters.RequiredInterfaceType = NWInterfaceType.Wifi;
				Assert.AreEqual (NWInterfaceType.Wifi, parameters.RequiredInterfaceType, "BNe value was not stored.");
			}
		}

		[Test]
		public void ProhibitInterfaceTypeTest ()
		{
			using (var parameters = new NWParameters ()) {
				var types = new List<NWInterfaceType> ();
				parameters.ProhibitInterfaceType (NWInterfaceType.Wifi);
				parameters.IterateProhibitedInterfaces ((type) => { types.Add (type); return true; });
				Assert.True (types.Contains (NWInterfaceType.Wifi), "Type was not prohibited.");
			}
		}

		[Test]
		public void ReuseLocalAddressPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.ReuseLocalAddress;
				Assert.False (defaultValue, "Default value changed.");
				parameters.ReuseLocalAddress = true;
				Assert.True (parameters.ReuseLocalAddress, "New value was not stored.");
			}
		}

		[Test]
		public void FastOpenEnabledPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.FastOpenEnabled;
				Assert.False (defaultValue, "Defalue value changed.");
				parameters.FastOpenEnabled = true;
				Assert.True (parameters.FastOpenEnabled, "New value was not stored.");
			}
		}

		[Test]
		public void ServiceClassPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.ServiceClass;
				Assert.AreEqual (NWServiceClass.BestEffort, defaultValue, "Default value changed.");
				parameters.ServiceClass = NWServiceClass.InteractiveVideo;
				Assert.AreEqual (NWServiceClass.InteractiveVideo, parameters.ServiceClass, "New value was not stored.");
			}
		}

		[Test]
		public void LocalEndpointPropertyTest ()
		{
			Assert.Ignore ("nw_parameters_copy_local_endpoint always return null. Rdar filled 44095278.");
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				var defaultValue = parameters.LocalEndpoint;
				Assert.IsNull (defaultValue, "Default value changed.");
				parameters.LocalEndpoint = endpoint;
				Assert.IsNotNull (parameters.LocalEndpoint, "New value was not stored.");
			}
		}

		[Test]
		public void IncludePeerToPeerPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.IncludePeerToPeer;
				Assert.False (defaultValue, "Default value changed.");
				parameters.IncludePeerToPeer = true;
				Assert.True (parameters.IncludePeerToPeer, "New value was not stored.");
			}
		}

		[Test]
		public void TestProhibitConstrained ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var parameters = new NWParameters ()) {
				var defaultValue = false;
				Assert.False (defaultValue, "Default value changed.");
				parameters.ProhibitConstrained = true;
				Assert.True (parameters.ProhibitConstrained, "New value was not stored.");
			}
		}

		[Test]
		public void AttributionPropertyTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			using (var parameters = new NWParameters ()) {
				Assert.DoesNotThrow (() => {
					parameters.Attribution = NWParametersAttribution.Developer;
				});
				Assert.AreEqual (NWParametersAttribution.Developer, parameters.Attribution);
			}
		}

		[Test]
		[Ignore ("Crashes everywhere. Feedback filed: https://github.com/xamarin/maccore/issues/2675")]
		public void SetPrivacyContextTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			using (var privacy = NWPrivacyContext.Default)
			using (var parameters = new NWParameters ()) {
				Assert.DoesNotThrow (() => {
					parameters.SetPrivacyContext (privacy);
				});
			}
		}

		[Test]
		public void CreateApplicationServiceTest ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
			using var nwParams = NWParameters.CreateApplicationService ();
			Assert.NotNull (nwParams);
		}
	}
}
#endif
