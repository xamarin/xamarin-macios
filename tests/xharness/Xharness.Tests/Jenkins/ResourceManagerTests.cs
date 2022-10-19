using System.Collections.Generic;
using System.Resources;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Xharness.Jenkins;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Xharness.Tests.Jenkins {
	[TestFixture]
	public class ResourceManagerTests {

		Mock<IHardwareDevice> device1;
		Mock<IHardwareDevice> device2;
		Xharness.Jenkins.ResourceManager resourceManager;

		[SetUp]
		public void SetUp ()
		{
			device1 = new Mock<IHardwareDevice> ();
			device2 = new Mock<IHardwareDevice> ();
			resourceManager = new Xharness.Jenkins.ResourceManager ();
		}

		[Test]
		public void GetDeviceResourceMissingTest ()
		{
			var devices = new List<IHardwareDevice> { device1.Object, device2.Object };
			// set the device expectations
			device1.Setup (d => d.UDID).Returns ("device1");
			device1.Setup (d => d.Name).Returns ("device1");

			device2.Setup (d => d.UDID).Returns ("device2");
			device2.Setup (d => d.Name).Returns ("device2");

			var resources = resourceManager.GetDeviceResources (devices);

			// assert that both are present
			Assert.NotNull (resources, "resources");
			Assert.AreEqual (2, resources.Count ());
			Assert.AreEqual (1, resources.Where (r => r.Name == "device1").Count (), "device1");
			Assert.AreEqual (1, resources.Where (r => r.Name == "device2").Count (), "device2");
		}

		[Test]
		public void GetDeviceResourcePresent ()
		{
			// call it twice to make sure we get the same results
			var devices = new List<IHardwareDevice> { device1.Object, device2.Object };
			// set the device expectations
			device1.Setup (d => d.UDID).Returns ("device1");
			device1.Setup (d => d.Name).Returns ("device1");

			device2.Setup (d => d.UDID).Returns ("device2");
			device2.Setup (d => d.Name).Returns ("device2");

			var resources = resourceManager.GetDeviceResources (devices);

			// assert that both are present
			Assert.NotNull (resources, "resources");
			Assert.AreEqual (2, resources.Count ());
			Assert.AreEqual (1, resources.Where (r => r.Name == "device1").Count (), "first device1");
			Assert.AreEqual (1, resources.Where (r => r.Name == "device2").Count (), "first device2");

			var resources2 = resourceManager.GetDeviceResources (devices);

			// assert that both are present
			Assert.NotNull (resources2, "resources2");
			Assert.AreEqual (2, resources2.Count ());
			Assert.AreEqual (1, resources2.Where (r => r.Name == "device1").Count (), "second device1");
			Assert.AreEqual (1, resources2.Where (r => r.Name == "device2").Count (), "decond device2");
		}

		[Test]
		public void GetDeviceResourcesAddMissingTest ()
		{
			var devices = new List<IHardwareDevice> { device1.Object };
			// set the device expectations
			device1.Setup (d => d.UDID).Returns ("device1");
			device1.Setup (d => d.Name).Returns ("device1");

			device2.Setup (d => d.UDID).Returns ("device2");
			device2.Setup (d => d.Name).Returns ("device2");

			var resources = resourceManager.GetDeviceResources (devices);

			// assert that both are present
			Assert.NotNull (resources, "resources");
			Assert.AreEqual (1, resources.Count ());
			Assert.AreEqual (1, resources.Where (r => r.Name == "device1").Count (), "first device1");
			Assert.AreEqual (0, resources.Where (r => r.Name == "device2").Count (), "first device2");

			var devices2 = new List<IHardwareDevice> { device1.Object, device2.Object };
			var resources2 = resourceManager.GetDeviceResources (devices2);

			// assert that both are present
			Assert.NotNull (resources2, "resources2");
			Assert.AreEqual (2, resources2.Count ());
			Assert.AreEqual (1, resources2.Where (r => r.Name == "device1").Count (), "second device1");
			Assert.AreEqual (1, resources2.Where (r => r.Name == "device2").Count (), "decond device2");
		}

		[Test]
		public void GetAllTest ()
		{
			var devices = new List<IHardwareDevice> { device1.Object, device2.Object };
			// set the device expectations
			device1.Setup (d => d.UDID).Returns ("device1");
			device1.Setup (d => d.Name).Returns ("device1");

			device2.Setup (d => d.UDID).Returns ("device2");
			device2.Setup (d => d.Name).Returns ("device2");

			var deviceResources = resourceManager.GetDeviceResources (devices);

			// now get all of the present resources and assert we have the desktop and nuget
			var allResources = resourceManager.GetAll ();

			Assert.AreEqual (deviceResources.Count () + 2, allResources.Count (), "count");
			Assert.AreEqual (1, allResources.Where (r => r.Name == "device1").Count ());
			Assert.AreEqual (1, allResources.Where (r => r.Name == "device2").Count ());
			Assert.AreEqual (1, allResources.Where (r => r.Name == "Desktop").Count ());
			Assert.AreEqual (1, allResources.Where (r => r.Name == "Nuget").Count ());
		}
	}
}
