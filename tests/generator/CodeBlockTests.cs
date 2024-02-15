using System;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace bgen_tests {
	public class CodeBlockTests {
		string PerformWriting (ICodeBlock codeBlock)
		{
			MemoryStream memoryStream = new();
			StreamWriter writer = new StreamWriter (memoryStream);
			codeBlock.Print (writer);
			writer.Flush ();
			memoryStream.Position = 0;
			using StreamReader reader = new StreamReader (memoryStream);
			return reader.ReadToEnd ();
		}

		[Test]
		public void LineBlockTest ()
		{
			string inputText1 = "using System;";
			string inputText2 = "using Network;";
			string outputText = "{\n    using System;\n    using Network;\n}\n";
			CodeBlock codeBlock = new(currentIndent:0);
			codeBlock.AddLine (inputText1);
			codeBlock.AddLine (inputText2);
			string output = PerformWriting (codeBlock);
			Assert.AreEqual (outputText, output);
		}

		[Test]
		public void CodeBlockAsClassTest ()
		{
			string headerText = "public class FooClass";
			string variableText = "public string FooText {get;set;}";
			string outputText = "public class FooClass\n{\n    public string FooText {get;set;}\n}\n";
			CodeBlock codeBlock = new(currentIndent:0, headerText);
			codeBlock.AddLine (variableText);
			string output = PerformWriting (codeBlock);
			Assert.AreEqual (outputText, output);
		}
	}
}
