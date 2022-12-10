#if !NET // https://github.com/xamarin/xamarin-macios/issues/11710
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Foundation;
using NUnit.Framework;

namespace LinkAll.Mef {

	// From Desk Case 70807
	public interface IStorageType {
	}

	[System.ComponentModel.Composition.Export (typeof (IStorageType))]
	[Preserve (AllMembers = true)]
	public class Storage : IStorageType {
	}

	[Preserve (AllMembers = true)]
	[TestFixture]
	public class MEFTests {
		CompositionContainer _container;

		[ImportMany]
		public IEnumerable<Lazy<IStorageType>> StorageTypes { get; set; }

		[Test]
		public void MEF_Basic_Import_Test ()
		{
			var catalog = new AggregateCatalog ();
			//Adds all the parts found in the same assembly
			catalog.Catalogs.Add (new AssemblyCatalog (typeof (MEFTests).Assembly));

			//Create the CompositionContainer with the parts in the catalog
			_container = new CompositionContainer (catalog);

			this._container.SatisfyImportsOnce (this);

			Assert.IsTrue (StorageTypes.Count () > 0, "No MEF imports found?");
		}

		[Test]
		public void ExportFactoryCreator ()
		{
			// the above code makes sure that ExportFactoryCreator is present
			var efc = Type.GetType ("System.ComponentModel.Composition.ReflectionModel.ExportFactoryCreator, System.ComponentModel.Composition");
			Assert.NotNull (efc, "ExportFactoryCreator");

			// and there's nothing else that refers to them - hence bug: https://bugzilla.xamarin.com/show_bug.cgi?id=29063
			// as it's used thru reflection in CreateStronglyTypedExportFactoryFactory
			var t = efc.GetMethod ("CreateStronglyTypedExportFactoryOfT", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); // same binding flags as MS code
			Assert.NotNull (t, "CreateStronglyTypedExportFactoryOfT");
			var tm = efc.GetMethod ("CreateStronglyTypedExportFactoryOfTM", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); // same binding flags as MS code
			Assert.NotNull (tm, "CreateStronglyTypedExportFactoryOfTM");
		}

		[Test]
		public void ExportServices ()
		{
			var es = Type.GetType ("System.ComponentModel.Composition.ExportServices, System.ComponentModel.Composition");
			Assert.NotNull (es, "ExportServices");
			// unlike the test code for ExportFactoryCreator the method can be marked by other call site, so this test is not 100% conclusive

			// used, thru reflection, from CreateStronglyTypedLazyFactory method 
			var t = es.GetMethod ("CreateStronglyTypedLazyOfT", BindingFlags.NonPublic | BindingFlags.Static); // same binding flags as MS code
			Assert.NotNull (t, "CreateStronglyTypedLazyOfT");
			var tm = es.GetMethod ("CreateStronglyTypedLazyOfTM", BindingFlags.NonPublic | BindingFlags.Static); // same binding flags as MS code
			Assert.NotNull (tm, "CreateStronglyTypedLazyOfTM");

			// used, thru reflection, from CreateSemiStronglyTypedLazyFactory method 
			var l = es.GetMethod ("CreateSemiStronglyTypedLazy", BindingFlags.NonPublic | BindingFlags.Static); // same binding flags as MS code
			Assert.NotNull (l, "CreateSemiStronglyTypedLazy");
		}
	}
}
#endif // !NET
