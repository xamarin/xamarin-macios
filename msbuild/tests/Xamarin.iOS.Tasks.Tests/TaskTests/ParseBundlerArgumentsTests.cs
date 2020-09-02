using System;
using System.IO;
using NUnit.Framework;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class ParseBundlerArgumentsTests : TestBase
	{
		class CustomParseBundlerArguments : ParseBundlerArgumentsTaskBase {}

		[Test]
		public void NoExtraArgs ()
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			Assert.IsTrue (task.Execute (), "execute");
			Assert.AreEqual ("false", task.NoSymbolStrip, "nosymbolstrip");
			Assert.AreEqual ("false", task.NoDSymUtil, "nodsymutil");

			task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = string.Empty;
			Assert.IsTrue (task.Execute (), "execute");
			Assert.AreEqual ("false", task.NoSymbolStrip, "nosymbolstrip");
			Assert.AreEqual ("false", task.NoDSymUtil, "nodsymutil");
		}

		[Test]
		public void NoSymbolStrip ()
		{
			var true_variations = new string [] { "--nosymbolstrip", "-nosymbolstrip", "/nosymbolstrip" };
			var false_variations = new string [] {
				"--nosymbolstrip:symbol1",
				"-nosymbolstrip:symbol2",
				"/nosymbolstrip:symbol3",
				"--nosymbolstrip=symbol1",
				"-nosymbolstrip=symbol2",
				"/nosymbolstrip=symbol3"
			};

			foreach (var variation in false_variations) {
				var task = CreateTask<CustomParseBundlerArguments> ();
				task.ExtraArgs = variation;
				Assert.IsTrue (task.Execute (), "execute: " + variation);
				Assert.AreEqual ("false", task.NoSymbolStrip, "nosymbolstrip: " + variation);
				Assert.AreEqual ("false", task.NoDSymUtil, "nodsymutil: " + variation);
			}

			foreach (var variation in true_variations) {
				var task = CreateTask<CustomParseBundlerArguments> ();
				task.ExtraArgs = variation;
				Assert.IsTrue (task.Execute (), "execute: " + variation);
				Assert.AreEqual ("true", task.NoSymbolStrip, "nosymbolstrip: " + variation);
				Assert.AreEqual ("false", task.NoDSymUtil, "nodsymutil: " + variation);
			}
		}

		[Test]
		public void NoDSymUtil ()
		{
			var true_variations = new string [] { "--dsym:false", "-dsym:0", "/dsym:no", "--dsym=disable" };
			var false_variations = new string [] {
				"--dsym:yes",
				"-dsym:1",
				"/dsym:true",
				"--dsym=enable",
				"-dsym",
				"/dsym",
				"--dsym"
			};

			foreach (var variation in false_variations) {
				var task = CreateTask<CustomParseBundlerArguments> ();
				task.ExtraArgs = variation;
				Assert.IsTrue (task.Execute (), "execute: " + variation);
				Assert.AreEqual ("false", task.NoSymbolStrip, "nosymbolstrip: " + variation);
				Assert.AreEqual ("false", task.NoDSymUtil, "nodsymutil: " + variation);
			}

			foreach (var variation in true_variations) {
				var task = CreateTask<CustomParseBundlerArguments> ();
				task.ExtraArgs = variation;
				Assert.IsTrue (task.Execute (), "execute: " + variation);
				Assert.AreEqual ("false", task.NoSymbolStrip, "nosymbolstrip: " + variation);
				Assert.AreEqual ("true", task.NoDSymUtil, "nodsymutil: " + variation);
			}
		}

		[Test]
		[TestCase ("--marshal-managed-exceptions", "")]
		[TestCase ("--marshal-managed-exceptions:", "")]
		[TestCase ("--marshal-managed-exceptions:default", "default")]
		[TestCase ("--marshal-managed-exceptions:dummy", "dummy")]
		[TestCase ("-marshal-managed-exceptions:dummy", "dummy")]
		[TestCase ("/marshal-managed-exceptions:dummy", "dummy")]
		public void MarshalManagedExceptionMode (string input, string output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			Assert.AreEqual (output, task.MarshalManagedExceptionMode, output);
		}

		[Test]
		[TestCase ("--marshal-objectivec-exceptions", "")]
		[TestCase ("--marshal-objectivec-exceptions:", "")]
		[TestCase ("--marshal-objectivec-exceptions:default", "default")]
		[TestCase ("--marshal-objectivec-exceptions:dummy", "dummy")]
		[TestCase ("-marshal-objectivec-exceptions:dummy", "dummy")]
		[TestCase ("/marshal-objectivec-exceptions:dummy", "dummy")]
		public void MarshalObjetiveCExceptionMode (string input, string output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			Assert.AreEqual (output, task.MarshalObjectiveCExceptionMode, output);
		}

		[Test]
		[TestCase ("--optimize", "")]
		[TestCase ("--optimize:", "")]
		[TestCase ("--optimize:default", "default")]
		[TestCase ("--optimize:dummy", "dummy")]
		[TestCase ("-optimize:dummy", "dummy")]
		[TestCase ("/optimize:dummy", "dummy")]
		[TestCase ("/optimize:dummy1 -optimize:dummy2", "dummy1,dummy2")]
		[TestCase ("/optimize:+all,-none -optimize:allornone", "+all,-none,allornone")]
		public void Optimize (string input, string output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			Assert.AreEqual (output, task.Optimize, output);
		}
	}
}

