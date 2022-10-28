#nullable enable
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using NUnit.Framework;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {
	public class ErrorKnowledgeBaseTests {

		ErrorKnowledgeBase? errorKnowledgeBase;
		string? testFile;

		[SetUp]
		public void SetUp ()
		{
			errorKnowledgeBase = new ErrorKnowledgeBase ();
			testFile = Path.GetTempFileName ();
		}

		[TearDown]
		public void TearDown ()
		{
			errorKnowledgeBase = null;
		}

		[Test]
		public void IsMonoMulti3IssuePresentTest ()
		{
			using var log = new LogFile ("test", testFile);
			log.WriteLine ("Some noise");
			log.WriteLine ("error MT5210: Native linking failed, undefined symbol: ___multi3");
			log.WriteLine ("Some noise");
			log.Flush ();
			Assert.IsTrue (errorKnowledgeBase!.IsKnownBuildIssue (log, out var failureMessage));
			Assert.IsNotNull (failureMessage);
		}

		[Test]
		public void IsMonoMulti3IssueMissingTest ()
		{
			using var log = new LogFile ("test", testFile);
			log.WriteLine ("Some noise");
			log.WriteLine ("Some noise");
			log.Flush ();
			Assert.IsFalse (errorKnowledgeBase!.IsKnownBuildIssue (log, out var failureMessage));
			Assert.IsNull (failureMessage);
		}

		[Test]
		public void IsHE0038ErrorPresentTest ()
		{
			using var log = new LogFile ("test", testFile);
			log.WriteLine ("Some noise");
			log.WriteLine ("error HE0038: Failed to launch the app");
			log.WriteLine ("Some noise");
			log.Flush ();
			Assert.IsTrue (errorKnowledgeBase!.IsKnownTestIssue (log, out var failureMessage));
			Assert.IsNotNull (failureMessage);
		}

		[Test]
		public void IsHE0038ErrorMissingTest ()
		{
			using var log = new LogFile ("test", testFile);
			log.WriteLine ("Some noise");
			log.WriteLine ("Some noise");
			log.Flush ();
			Assert.IsFalse (errorKnowledgeBase!.IsKnownTestIssue (log, out var failureMessage));
			Assert.IsNull (failureMessage);
		}
	}
}
