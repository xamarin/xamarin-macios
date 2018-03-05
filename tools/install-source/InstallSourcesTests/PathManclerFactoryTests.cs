using System.IO;
using NUnit.Framework;

using InstallSources;

namespace InstallSourcesTests
{
	[TestFixture]
	public class PathManclerFactoryTests
	{
		string tmpRoot;
		string installDir;
		string fakeMonoPath;
		string fakeOpenTKPath;
		string fakeXamarinPath;
		
		PathManglerFactory factory;
		
		void CreateFakeFile (string path)
		{
			string[] lines = { "Test", "Fake", "file" };
			using (StreamWriter outputFile = new StreamWriter (path)) {
				foreach (string line in lines)
					outputFile.WriteLine(line);
			}
		}
		
		[SetUp]
		public void SetUp ()
		{
			installDir = Path.GetTempFileName ();
			if (File.Exists (installDir))
				File.Delete (installDir);
			installDir = Path.Combine (installDir, "Xamarin.iOS");
			Directory.CreateDirectory (installDir);
			// create temp dirs that will be used to place some files
			// to test their presence and decide that the correct
			// mangler is used
			tmpRoot = Path.GetTempFileName ();
			if (File.Exists (tmpRoot))
				File.Delete (tmpRoot);
			Directory.CreateDirectory (tmpRoot);

			// create a path for the mono paths
			var monoPath = Path.Combine (tmpRoot, "external", "mono");
			Directory.CreateDirectory (monoPath);
			fakeMonoPath = Path.Combine (monoPath, "FakeStirng.cs");
			CreateFakeFile (fakeMonoPath);

			// create a path for the xamarin paths
			var xamarinPath = Path.Combine (tmpRoot, "src");
			Directory.CreateDirectory (xamarinPath);
			fakeXamarinPath = Path.Combine (xamarinPath, "FakeObjc.cs");
			CreateFakeFile (fakeXamarinPath);
			
			var openTKPath = Path.Combine (tmpRoot, "external", "OpenTK");
			Directory.CreateDirectory (Path.Combine (openTKPath));
			fakeOpenTKPath = Path.Combine (openTKPath, "FakeMaths.cs");
			CreateFakeFile (fakeOpenTKPath);

			factory = new PathManglerFactory()
			{
				InstallDir = installDir,
				MonoSourcePath = monoPath,
				XamarinSourcePath = xamarinPath,
				OpenTKSourcePath = openTKPath
			};
		}
		
		[TearDown]
		public void TearDown ()
		{
			if (Directory.Exists (tmpRoot))
				Directory.Delete (tmpRoot, true);
			if (Directory.Exists (installDir))
				Directory.Delete (installDir);
		}
		
		[Test]
		public void TestIsMonoPath ()
		{
			var monoPathMap = fakeMonoPath.Replace (factory.MonoSourcePath, Path.Combine (installDir, "src", "Xamarin.iOS"));
			var xamarinPathMap = fakeXamarinPath.Replace(factory.XamarinSourcePath, Path.Combine (installDir, "src", "Xamarin.iOS"));
			Assert.IsTrue (factory.IsMonoPath (monoPathMap), "Present mono path");
			Assert.IsFalse (factory.IsMonoPath (xamarinPathMap), "Present xamarin path");
		
		}
		
		[Test]
		public void TestIsOpenTKPath ()
		{
			var openTKPathMap = fakeMonoPath.Replace (factory.OpenTKSourcePath, Path.Combine (installDir, "src", "Xamarin.iOS"));
			var xamarinPathMap = fakeXamarinPath.Replace(factory.XamarinSourcePath, Path.Combine (installDir, "src", "Xamarin.iOS"));
			Assert.IsTrue (factory.IsMonoPath (openTKPathMap), "Present openTK path");
			Assert.IsFalse (factory.IsMonoPath (xamarinPathMap), "Present xamarin path");
		}

		[Test]
		public void TestPathIsIgnored ()
		{
			var pathToIgnore = Path.Combine (factory.MonoSourcePath, "mcs", "mcs", "Foo.cs");
			var pathNotToIgnore = Path.Combine (factory.MonoSourcePath, "mcs", "class", "Foo.cs");
			Assert.IsTrue (factory.IsIgnored (pathToIgnore), "Do ignore.");
			Assert.IsFalse (factory.IsIgnored (pathNotToIgnore), "Do NOT ignore.");
		}
	}
}
