using System;
using System.Collections.Generic;
using System.Threading;
using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {


	// The following class just has a subset of the methods in the class, the API allows to perform udp 
	// communications between bonjour services, which is very fragile in the CI, a sample 
	// should be added: https://github.com/xamarin/xamarin-macios/issues/9642
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWConnectionGroupTest {
		NWEndpoint endpoint;
		NWMulticastGroup descriptor;
		NWParameters parameters;
		NWConnectionGroup connectionGroup;

		[SetUp]
		public void SetUp ()
		{ 
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			endpoint = NWEndpoint.Create ("224.0.0.251", "5353");
			parameters = NWParameters.CreateUdp ();
			descriptor = new NWMulticastGroup (endpoint);
			connectionGroup = new NWConnectionGroup (descriptor, parameters);
		}

		[TearDown]
		public void TearDown ()
		{
			endpoint?.Dispose ();
			endpoint = null;
			parameters?.Dispose ();
			parameters = null;
			descriptor?.Dispose ();
			descriptor = null;
			connectionGroup?.Dispose ();
			connectionGroup = null;
		}
		
		[Test]
		public void GroupDescriptorTest ()
			=> Assert.NotNull (connectionGroup.GroupDescriptor);

		[Test]
		public void ParametersTest ()
			=> Assert.NotNull (connectionGroup.Parameters);

		[Test]
		public void SetQueueTest ()
		{
			using var q = new DispatchQueue (label: "monitor");
			Assert.DoesNotThrow (() => {
				connectionGroup.SetQueue (q);
			});
		}
	}
}
