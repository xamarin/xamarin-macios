using System;
using System.Linq;
using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.MacDev.Tasks;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ParseBundlerArgumentsTests : TestBase {
		class CustomParseBundlerArguments : ParseBundlerArgumentsTaskBase { }

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
			var true_variations = new string [] {
				"--nosymbolstrip",
				"-nosymbolstrip",
				"/nosymbolstrip",
				"/nosymbolstrip symbol1", // the argument to nosymbolstrip is optional, which means that any subsequent arguments are considered separate options.
			};
			var false_variations = new string [] {
				"--nosymbolstrip:symbol1",
				"-nosymbolstrip:symbol2",
				"/nosymbolstrip:symbol3",
				"--nosymbolstrip=symbol1",
				"-nosymbolstrip=symbol2",
				"/nosymbolstrip=symbol3",
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
			var true_variations = new string [] {
				"--dsym:false",
				"-dsym:0",
				"/dsym:no",
				"--dsym=disable",
			};
			var false_variations = new string [] {
				"--dsym:yes",
				"-dsym:1",
				"/dsym:true",
				"--dsym=enable",
				"-dsym",
				"/dsym",
				"--dsym",
				"--dsym true",
				"--dsym false", // the argument to dsym is optional, which means that any subsequent arguments are considered separate options.
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
		[TestCase ("--marshal-managed-exceptions default", "default")]
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
		[TestCase ("--marshal-objectivec-exceptions default", "default")]
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
		[TestCase ("--optimize default", "")] // // the argument to optimize is optional, which means that any subsequent arguments are considered separate options.
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

		[TestCase ("--registrar", "")]
		[TestCase ("--registrar:static", "static")]
		[TestCase ("--registrar static", "static")]
		[TestCase ("--registrar:default", "default")]
		[TestCase ("--registrar=dynamic,trace", "dynamic,trace")]
		[TestCase ("-registrar:dummy", "dummy")]
		[TestCase ("/registrar:dummy", "dummy")]
		[TestCase ("/registrar:dummy1 /registrar:dummy2", "dummy2")]
		public void Registrar (string input, string output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			Assert.AreEqual (output, task.Registrar, output);
		}

		[TestCase ("--xml", null, "")]
		[TestCase ("--xml", "", "")]
		[TestCase ("--xml:abc", null, "abc")]
		[TestCase ("--xml abc", null, "abc")]
		[TestCase ("--xml:abc --xml:def", null, "abc;def")]
		[TestCase ("--xml:abc --xml:def", "123", "123;abc;def")]
		[TestCase ("--xml:abc --xml:def", "123;456", "123;456;abc;def")]
		[TestCase ("-xml:dummy", null, "dummy")]
		[TestCase ("/xml:dummy", null, "dummy")]
		[TestCase ("/xml:dummy1 /xml:dummy2", null, "dummy1;dummy2")]
		[TestCase ("/xml:/path/a /xml:/path/b", null, "/path/a;/path/b")]
		public void XmlDefinitions (string input, string existing, string output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			if (existing is not null)
				task.XmlDefinitions = existing.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select (v => new TaskItem (v)).ToArray ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			Assert.AreEqual (output, string.Join (";", task.XmlDefinitions.Select (v => v.ItemSpec).ToArray ()), output);
		}

		[TestCase ("--custom_bundle_name", "")]
		[TestCase ("--custom_bundle_name:", "")]
		[TestCase ("--custom_bundle_name=", "")]
		[TestCase ("--custom_bundle_name=abc", "abc")]
		[TestCase ("--custom_bundle_name abc", "abc")]
		public void CustomBundleName (string input, string output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			Assert.AreEqual (output, task.CustomBundleName, output);
		}

		[TestCase ("--gcc_flags -dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("--gcc_flags=-dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("-gcc_flags -dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("-gcc_flags=-dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("--link_flags -dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("--link_flags=-dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("-link_flags -dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("-link_flags=-dead_strip", new string [] { "-dead_strip" })]
		[TestCase ("--gcc_flags \"-dead_strip -v\"", new string [] { "-dead_strip", "-v" })]
		public void CustomLinkFlags (string input, string [] output)
		{
			var task = CreateTask<CustomParseBundlerArguments> ();
			task.ExtraArgs = input;
			Assert.IsTrue (task.Execute (), input);
			CollectionAssert.AreEquivalent (output, task.CustomLinkFlags.Select (v => v.ItemSpec).ToArray (), string.Join (" ", output));
		}
	}
}
