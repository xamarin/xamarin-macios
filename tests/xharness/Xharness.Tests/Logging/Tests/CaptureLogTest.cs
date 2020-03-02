using System;
using System.IO;
using NUnit.Framework;

namespace Xharness.Tests.Logging.Tests {

	[TestFixture]
	public class CaptureLogTest {

		string filePath;

		[SetUp]
		public void SetUp ()
		{
			filePath = Path.GetTempFileName ();
		}

		[TearDown]
		public void TeatDown ()
		{
			if (File.Exists (filePath))
				File.Delete (filePath);
		}

		[Test]
		public void ConstrutorMissingFile ()
		{

		}

		[Test]
		public void ConstructorNullFilePath ()
		{
		}


		[Test]
		public void CaptureRightOrder ()
		{

		}

		[Test]
		public void CaptureMiddle ()
		{

		}

		[Test]
		public void CaptureMissingFile ()
		{

		}

		[Test]
		public void CaptureWrongOrder ()
		{

		}

		[Test]
		public void FullPathTest ()
		{

		}
	}
}
