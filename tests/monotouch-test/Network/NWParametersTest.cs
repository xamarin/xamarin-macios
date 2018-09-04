using System;
using System.Collections.Generic;
using System.Threading;
#if XAMCORE_2_0
using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.Security;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	public class NWParametersTest {

		AutoResetEvent secureEvent;  // used to let us know the handler was indeed called.
		AutoResetEvent configureEvent;  // used to let us know the handler was indeed called.
		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the NWPath 
		bool secureConnectionWasSet = false;
		bool protocolConfigured = false;
		List<NWInterface> interfaces = new List<NWInterface> ();

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			secureEvent = new AutoResetEvent(false);
			configureEvent = new AutoResetEvent(false);
			connectedEvent = new AutoResetEvent(false);
			secureConnectionWasSet = false;
			protocolConfigured = false;
			interfaces = new List<NWInterface> ();
			// we create a connection which we are going to use to get the availabe
			// interfaces, that way we can later test protperties of the NWParameters class.
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				connectedEvent.WaitOne (500);
				using (var path = connection.CurrentPath)
				{
					path.EnumerateInterfaces (EnumerateInterfacesHandler);
				}
			}
		}

		[TearDown]
		public void TearDown ()
		{
			secureEvent = null;
			secureConnectionWasSet = false;
			protocolConfigured = false;
			foreach (var i in interfaces) {
				i.Dispose ();
			}
		}

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			var errno = (SslStatus)(error != null ? error.ErrorCode : 0);
			switch (state){
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			}
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
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				secureEvent.WaitOne ();
				configureEvent.WaitOne ();
				connection.Cancel ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}

		[Test]
		public void CreateSecureUpdTestDoNotSetUpProtocol ()
		{
			var setUpTls = CreateTlsHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: setUpTls))
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				secureEvent.WaitOne ();
				connection.Cancel ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.False (protocolConfigured, "Protocol configure handler was called.");
			}
		}

		[Test]
		public void CreateSecureUpdTestDoNotSetUpTls ()
		{
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureUdp (configureTls: null, configureUdp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				configureEvent.WaitOne ();
				connection.Cancel ();
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
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				secureEvent.WaitOne ();
				configureEvent.WaitOne ();
				connection.Cancel ();
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
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				secureEvent.WaitOne ();
				connection.Cancel ();
				Assert.True (secureConnectionWasSet, "Configure TLS handler was not called.");
				Assert.False (protocolConfigured, "Protocol configure handler was called.");
			}
		}

		[Test]
		public void CreateSecureTcpTestDoNotSetUpTls ()
		{
			var setUpProtocol = CreateConfigureProtocolHandler ();

			using (var parameters = NWParameters.CreateSecureTcp (configureTls: null, configureTcp: setUpProtocol))
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			using (var connection = new NWConnection (endpoint, parameters)) {
				connection.SetQueue (DispatchQueue.MainQueue);
				connection.Start ();
				configureEvent.WaitOne ();
				connection.Cancel ();
				Assert.False (secureConnectionWasSet, "Configure TLS handler was called.");
				Assert.True (protocolConfigured, "Protocol configure handler was not called.");
			}
		}

		[Test]
		public void MultiPathServicePropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
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
			using (var parameters = new NWParameters ())
			{
				var stack = parameters.ProtocolStack;
				Assert.AreNotEqual (IntPtr.Zero, stack.Handle);
			}
		}

		[Test]
		public void LocalOnlyPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.LocalOnly;
				Assert.False (defaultValue, "Default value changed.");
				parameters.LocalOnly = true;
				Assert.True (parameters.LocalOnly, "New value was not stored.");
			}
		}

		[Test]
		public void PreferNoProxyPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.PreferNoProxy;
				Assert.False (defaultValue, "Default value changed.");
				parameters.PreferNoProxy = true;
				Assert.True (parameters.PreferNoProxy, "New value was not stored.");
			}
		}

		[Test]
		public void ExpiredDnsBehaviorPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.ExpiredDnsBehavior;
				Assert.AreEqual (NWParametersExpiredDnsBehavior.Default, defaultValue, "Default value changed.");
				parameters.ExpiredDnsBehavior = NWParametersExpiredDnsBehavior.Allow;
				Assert.AreEqual (NWParametersExpiredDnsBehavior.Allow, parameters.ExpiredDnsBehavior, "New value was not stored.");
			}
		}

		[Test]
		public void RequiredInterfacePropertyTest ()
		{

			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.RequiredInterface;
				Assert.IsNull (defaultValue, "Default value changed.");
				// try to set a null value, we should have no issues
				parameters.RequiredInterface = null;
				Assert.IsNull (parameters.RequiredInterface, "Value should still be null.");
				parameters.RequiredInterface = interfaces[0];
				Assert.AreNotEqual (IntPtr.Zero, parameters.RequiredInterface.Handle, "New value was not set.");
			}
		}

		[Test]
		public void ProhibitInterfaceTest ()
		{
			using (var parameters = new NWParameters ())
			{
				Assert.Throws<ArgumentNullException> (() => parameters.ProhibitInterface (null), "");
				Assert.AreNotEqual (0, interfaces.Count, "No network interfaces found.");
				parameters.ProhibitInterface (interfaces[0]);
			}
		} 

		[Test]
		public void RequiredInterfaceTypePropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.RequiredInterfaceType;
				Assert.AreEqual (NWInterfaceType.Other, defaultValue, "Default value changed.");
				parameters.RequiredInterfaceType = NWInterfaceType.Wifi;
				Assert.AreEqual (NWInterfaceType.Wifi, parameters.RequiredInterfaceType, "BNe value was not stored.");
			}
		}

		[Test]
		public void ProhibitInterfaceTypeTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var types = new List<NWInterfaceType> ();
				parameters.ProhibitInterfaceType (NWInterfaceType.Wifi);
				parameters.IterateProhibitedInterfaces ((type) => {types.Add (type); return true;} );
				Assert.True (types.Contains (NWInterfaceType.Wifi), "Type was not prohibited.");	
			}
		}

		[Test]
		public void ReuseLocalAddressPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.ReuseLocalAddress;
				Assert.False (defaultValue, "Default value changed.");
				parameters.ReuseLocalAddress = true;
				Assert.True (parameters.ReuseLocalAddress, "New value was not stored.");
			}
		}

		[Test]
		public void FastOpenEnabledPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.FastOpenEnabled;
				Assert.False (defaultValue, "Defalue value changed.");
				parameters.FastOpenEnabled = true;
				Assert.True (parameters.FastOpenEnabled, "New value was not stored.");
			}
		}

		[Test]
		public void ServiceClassPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
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
			using (var endpoint = NWEndpoint.Create ("wwww.google.com", "80"))
			{
				var defaultValue = parameters.LocalEndpoint;
				Assert.IsNull (defaultValue, "Default value changed.");
				parameters.LocalEndpoint = endpoint;
				Assert.IsNotNull (parameters.LocalEndpoint, "New value was not stored.");
			}
		}

		[Test]
		public void IncludePeerToPeerPropertyTest ()
		{
			using (var parameters = new NWParameters ())
			{
				var defaultValue = parameters.IncludePeerToPeer;
				Assert.False (defaultValue, "Default value changed.");
				parameters.IncludePeerToPeer = true;
				Assert.True (parameters.IncludePeerToPeer, "New value was not stored.");
			}
		}
	}
}
