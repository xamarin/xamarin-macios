using System;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace bgen_tests {
	public class CodeBlockTests {
		[Test]
		public void LineBlockTest ()
		{
			int indent = 0;
			string inputText1 = "using System;";
			string inputText2 = "using Network;";
			string outputText = "{\n    using System;\n    using Network;\n}\n";
			CodeBlock c = new (indent);
			c.AddLine (inputText1);
			c.AddLine (inputText2);
			Assert.AreEqual (outputText, c.Print ());
		}

		[Test]
		public void CodeBlockAsClassTest ()
		{
			int indent = 0;
			string headerText = "public class FooClass";
			string variableText = "public string FooText {get;set;}";
			string outputText = "public class FooClass\n{\n    public string FooText {get;set;}\n}\n";
			CodeBlock c = new (indent, headerText);
			c.AddLine (variableText);
			Assert.AreEqual (outputText, c.Print ());
		}
	}
}
