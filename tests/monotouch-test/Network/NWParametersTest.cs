using System.Threading;

using Network;

using MonoTests.System.Net.Http;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWParametersTest {

		AutoResetEvent secureEvent;  // used to let us know the handler was indeed called.
		AutoResetEvent configureEvent;  // used to let us know the handler was indeed called.
		bool secureConnectionWasSet = false;
		bool protocolConfigured = false;
		List<NWInterface> interfaces = new List<NWInterface> ();
		ConnectionManager manager;
		NWConnection connection;

		[OneTimeSetUp]
		public void Init ()
		{
			// we want to use a single connection, since it is expensive
			interfaces = new List<NWInterface> ();

			manager = new ConnectionManager ();
			connection = manager.CreateConnection ();
			using (var path = connection.CurrentPath) {
				path.EnumerateInterfaces (EnumerateInterfacesHandler);
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

		[TearDown]
		public void TearDown ()
		{
			secureEvent = null;
			secureConnectionWasSet = false;
			protocolConfigured = false;
			manager?.Dispose ();
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
				Assert.That (secureConnectionWasSet, Is.True, "Configure TLS handler was not called.");
				Assert.That (protocolConfigured, Is.True, "Protocol configure handler was not called.");
			}
		}

		[Test]
		public void CreateSecureUpdTestDoNotSetUpProtocol ()
		{
			var setUpTls = CreateTlsHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: setUpTls))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				secureEvent.WaitOne ();
				Assert.That (secureConnectionWasSet, Is.True, "Configure TLS handler was not called.");
				Assert.That (protocolConfigured, Is.False, "Protocol configure handler was called.");
			}
		}

		[Test]
		public void CreateSecureUpdTestDoNotSetUpTls ()
		{
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: null, configureUdp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				configureEvent.WaitOne ();
				Assert.That (secureConnectionWasSet, Is.False, "Configure TLS handler was not called.");
				Assert.That (protocolConfigured, Is.True, "Protocol configure handler was not called.");
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
				Assert.That (secureConnectionWasSet, Is.True, "Configure TLS handler was not called.");
				Assert.That (protocolConfigured, Is.True, "Protocol configure handler was not called.");
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
				Assert.That (secureConnectionWasSet, Is.True, "Configure TLS handler was not called.");
				Assert.That (protocolConfigured, Is.False, "Protocol configure handler was called.");
			}
		}

		[Test]
		public void CreateSecureTcpTestDoNotSetUpTls ()
		{
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureTcp (configureTls: null, configureTcp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				configureEvent.WaitOne ();
				Assert.That (secureConnectionWasSet, Is.False, "Configure TLS handler was called.");
				Assert.That (protocolConfigured, Is.True, "Protocol configure handler was not called.");
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
				Assert.That (protocolConfigured, Is.True, "Protocol configure handler was not called.");
			}
		}
#endif

		[Test]
		public void MultiPathServicePropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.MultipathService;
				Assert.That (NWMultiPathService.Disabled, Is.EqualTo (defaultValue), "Default value changed.");
				var newValue = NWMultiPathService.Aggregate;
				parameters.MultipathService = newValue;
				Assert.That (parameters.MultipathService, Is.EqualTo (newValue), "New value was not stored.");
			}
		}

		[Test]
		public void ProtocolStackPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var stack = parameters.ProtocolStack;
				Assert.That (stack.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}

		[Test]
		public void LocalOnlyPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.LocalOnly;
				Assert.That (defaultValue, Is.False, "Default value changed.");
				parameters.LocalOnly = true;
				Assert.That (parameters.LocalOnly, Is.True, "New value was not stored.");
			}
		}

		[Test]
		public void PreferNoProxyPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.PreferNoProxy;
				Assert.That (defaultValue, Is.False, "Default value changed.");
				parameters.PreferNoProxy = true;
				Assert.That (parameters.PreferNoProxy, Is.True, "New value was not stored.");
			}
		}

		[Test]
		public void ExpiredDnsBehaviorPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.ExpiredDnsBehavior;
				Assert.That (defaultValue, Is.EqualTo (NWParametersExpiredDnsBehavior.Default), "Default value changed.");
				parameters.ExpiredDnsBehavior = NWParametersExpiredDnsBehavior.Allow;
				Assert.That (parameters.ExpiredDnsBehavior, Is.EqualTo (NWParametersExpiredDnsBehavior.Allow), "New value was not stored.");
			}
		}

		[Test]
		public void RequiredInterfacePropertyTest ()
		{

			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.RequiredInterface;
				Assert.That (defaultValue, Is.Null, "Default value changed.");
				// try to set a null value, we should have no issues
				parameters.RequiredInterface = null;
				Assert.That (parameters.RequiredInterface, Is.Null, "Value should still be null.");
				parameters.RequiredInterface = interfaces [0];
				Assert.That (parameters.RequiredInterface.Handle, Is.Not.EqualTo (IntPtr.Zero), "New value was not set.");
			}
		}

		[Test]
		public void ProhibitInterfaceTest ()
		{
			using (var parameters = new NWParameters ()) {
				Assert.Throws<ArgumentNullException> (() => parameters.ProhibitInterface (null), "");
				Assert.That (interfaces.Count, Is.Not.EqualTo (0), "No network interfaces found.");
				parameters.ProhibitInterface (interfaces [0]);
			}
		}

		[Test]
		public void RequiredInterfaceTypePropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.RequiredInterfaceType;
				Assert.That (defaultValue, Is.EqualTo (NWInterfaceType.Other), "Default value changed.");
				parameters.RequiredInterfaceType = NWInterfaceType.Wifi;
				Assert.That (parameters.RequiredInterfaceType, Is.EqualTo (NWInterfaceType.Wifi), "BNe value was not stored.");
			}
		}

		[Test]
		public void ProhibitInterfaceTypeTest ()
		{
			using (var parameters = new NWParameters ()) {
				var types = new List<NWInterfaceType> ();
				parameters.ProhibitInterfaceType (NWInterfaceType.Wifi);
				parameters.IterateProhibitedInterfaces ((type) => { types.Add (type); return true; });
				Assert.That (types.Contains (NWInterfaceType.Wifi), Is.True, "Type was not prohibited.");
			}
		}

		[Test]
		public void ReuseLocalAddressPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.ReuseLocalAddress;
				Assert.That (defaultValue, Is.False, "Default value changed.");
				parameters.ReuseLocalAddress = true;
				Assert.That (parameters.ReuseLocalAddress, Is.True, "New value was not stored.");
			}
		}

		[Test]
		public void FastOpenEnabledPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.FastOpenEnabled;
				Assert.That (defaultValue, Is.False, "Defalue value changed.");
				parameters.FastOpenEnabled = true;
				Assert.That (parameters.FastOpenEnabled, Is.True, "New value was not stored.");
			}
		}

		[Test]
		public void ServiceClassPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.ServiceClass;
				Assert.That (defaultValue, Is.EqualTo (NWServiceClass.BestEffort), "Default value changed.");
				parameters.ServiceClass = NWServiceClass.InteractiveVideo;
				Assert.That (parameters.ServiceClass, Is.EqualTo (NWServiceClass.InteractiveVideo), "New value was not stored.");
			}
		}

		[Test]
		public void LocalEndpointPropertyTest ()
		{
			Assert.Ignore ("nw_parameters_copy_local_endpoint always return null. Rdar filled 44095278.");
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (NetworkResources.MicrosoftUri.Host, "80")) {
				var defaultValue = parameters.LocalEndpoint;
				Assert.That (defaultValue, Is.Null, "Default value changed.");
				parameters.LocalEndpoint = endpoint;
				Assert.That (parameters.LocalEndpoint, Is.Not.Null, "New value was not stored.");
			}
		}

		[Test]
		public void IncludePeerToPeerPropertyTest ()
		{
			using (var parameters = new NWParameters ()) {
				var defaultValue = parameters.IncludePeerToPeer;
				Assert.That (defaultValue, Is.False, "Default value changed.");
				parameters.IncludePeerToPeer = true;
				Assert.That (parameters.IncludePeerToPeer, Is.True, "New value was not stored.");
			}
		}

		[Test]
		public void TestProhibitConstrained ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var parameters = new NWParameters ()) {
				var defaultValue = false;
				Assert.That (defaultValue, Is.False, "Default value changed.");
				parameters.ProhibitConstrained = true;
				Assert.That (parameters.ProhibitConstrained, Is.True, "New value was not stored.");
			}
		}

		[TestCase (NWParametersAttribution.Developer)]
		[TestCase (NWParametersAttribution.User)]
		public void AttributionPropertyTest (NWParametersAttribution attribution)
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			using (var parameters = new NWParameters ()) {
				Assert.DoesNotThrow (() => {
					parameters.Attribution = attribution;
				});
				Assert.That (parameters.Attribution, Is.EqualTo (attribution));
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
			Assert.That (nwParams, Is.Not.Null);
		}

		[Test]
		public void MiscPropertiesTest ()
		{
			TestRuntime.AssertXcodeVersion (26, 0);
			using var nwParams = NWParameters.CreateApplicationService ();
			Assert.That (nwParams.AllowUltraConstrained, Is.True.Or.False, "AllowUltraConstrained#1");
			nwParams.AllowUltraConstrained = true;
			Assert.That (nwParams.AllowUltraConstrained, Is.True, "AllowUltraConstrained#2");
			nwParams.AllowUltraConstrained = false;
			Assert.That (nwParams.AllowUltraConstrained, Is.False, "AllowUltraConstrained#1");
		}
	}
}
