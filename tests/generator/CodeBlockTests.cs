using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace GeneratorTests {
	public class CodeBlockTests {
		string PerformWriting (ICodeBlock codeBlock)
		{
			using var writer = new StringWriter ();
			codeBlock.Print (writer);
			return writer.ToString ();
		}

		[Test]
		public void LineBlockTest ()
		{
			string expectedText =
				"{\n" +
				"    using System;\n" +
				"    using Network;\n" +
				"}\n";
			CodeBlock codeBlock = new (new string [] { "using System;", "using Network;" });
			string output = PerformWriting (codeBlock);
			Assert.AreEqual (expectedText, output);

			expectedText =
				"{\n" +
				"    using System;\n" +
				"    using Network;\n" +
				"    using NUnit;\n" +
				"}\n";
			codeBlock.AddLine ("using NUnit;");
			output = PerformWriting (codeBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void MethodBlockConstructionTest ()
		{
			string expectedText =
				"public void Foobinate(int count, bool isFoo)\n" +
				"{\n" +
				"    int fooCount = 1;\n" +
				"    string [] fooNames = new [] {\"foo\"};\n" +
				"}\n";
			MethodBlock methodBlock = new ("public void Foobinate", "int count", "bool isFoo")
			{
				"int fooCount = 1;",
				"string [] fooNames = new [] {\"foo\"};",
			};
			string output = PerformWriting (methodBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void MethodBlockTest ()
		{
			string expectedText =
				"public void Foobinate(int count, bool isFoo)\n" +
				"{\n" +
				"    int fooCount = 1;\n" +
				"    string[] fooNames = new [] {\"foo\"};\n" +
				"}\n";
			List<ICodeBlock> blocks = new List<ICodeBlock> () { new LineBlock ("int fooCount = 1;"), new LineBlock ("string[] fooNames = new [] {\"foo\"};") };
			MethodBlock methodBlock = new ("public void Foobinate", blocks, "int count", "bool isFoo");
			string output = PerformWriting (methodBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void CodeBlockAsClassTest ()
		{
			string expectedText =
				"public class FooClass\n" +
				"{\n" +
				"    public string FooText {get;set;}\n" +
				"}\n";
			CodeBlock codeBlock = new ("public class FooClass",
				"public string FooText {get;set;}");
			string output = PerformWriting (codeBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void IfBlockTest ()
		{
			string expectedText =
				"if (fooCount == 5)\n" +
				"{\n" +
				"    Console.WriteLine(fooCount);\n" +
				"}\n";
			IfBlock ifBlock = new ("fooCount == 5") {
				"Console.WriteLine(fooCount);"
			};
			string output = PerformWriting (ifBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void IfElseBlockTest ()
		{
			string expectedText =
				"if (fooCount == 5)\n" +
				"{\n" +
				"    Console.WriteLine(fooCount);\n" +
				"}\n" +
				"else\n" +
				"{\n" +
				"    Console.WriteLine(booCount);\n" +
				"}\n";
			IfBlock ifBlock = new IfBlock ("fooCount == 5") {
				"Console.WriteLine(fooCount);"
			}.AddElse (
				"Console.WriteLine(booCount);"
			);
			string output = PerformWriting (ifBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void IfElseIfElseBlockTest ()
		{
			string expectedText =
				"if (fooCount == 5)\n" +
				"{\n" +
				"    Console.WriteLine(fooCount);\n" +
				"}\n" +
				"else if (fooCount == 10)\n" +
				"{\n" +
				"    Console.WriteLine(booCount);\n" +
				"}\n" +
				"else\n" +
				"{\n" +
				"    Console.WriteLine(zooCount);\n" +
				"}\n";
			IfBlock ifBlock = new IfBlock ("fooCount == 5") {
				"Console.WriteLine(fooCount);"
			}.AddElseIf ("fooCount == 10",
				"Console.WriteLine(booCount);"
			).AddElse (
				"Console.WriteLine(zooCount);"
			);
			string output = PerformWriting (ifBlock);
			Assert.AreEqual (expectedText, output);
		}

		[Test]
		public void ComprehensiveTest ()
		{
			string expectedText = "using Network;\n" +
								  "\n" +
								  "public namespace FooSpace\n{\n" +
								  "    public class FooClass\n" +
								  "    {\n" +
								  "        public string FooText { get; set; }\n" +
								  "        public void Foobinate(int count, bool isFoo)\n" +
								  "        {\n" +
								  "        }\n" +
								  "    }\n" +
								  "}\n";
			BlockContainer blockContainer = new () { "using Network;", String.Empty };
			CodeBlock block = new CodeBlock ("public namespace FooSpace") {
				new CodeBlock ("public class FooClass") {
					"public string FooText { get; set; }",
					new MethodBlock ("public void Foobinate",
						"int count", "bool isFoo") { }
				}
			};
			blockContainer.Add (block);

			string output = PerformWriting (blockContainer);
			Assert.AreEqual (expectedText, output);
		}
	}
}
