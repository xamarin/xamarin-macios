using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

public class TabbedStringBuilderTests {
	StringBuilder sb;

	public TabbedStringBuilderTests ()
	{
		sb = new ();
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void ConstructorNotBlockTest (uint tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendLine ("Test");
			result = block.ToString ();
		}

		Assert.Equal ($"{expectedTabs}Test\n", result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void ConstructorBlockTest (uint tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			block.AppendLine ("Test");
			result = block.ToString ();
		}

		Assert.Equal ($"{expectedTabs}{{\n{expectedTabs}\tTest\n{expectedTabs}}}\n", result);
	}

	[Theory]
	[InlineData (0)]
	[InlineData (1)]
	[InlineData (5)]
	public void AppendLineTest (uint tabCount)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendLine ();
			result = block.ToString ();
		}

		// an empty line should have not tabs
		Assert.Equal ("\n", result);
	}

	[Theory]
	[InlineData ("// test comment", 0, "")]
	[InlineData ("var t = 1;", 1, "\t")]
	[InlineData ("Console.WriteLine (\"1\");", 5, "\t\t\t\t\t")]
	public void AppendLineStringTest (string line, uint tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			block.AppendLine (line);
			result = block.ToString ();
		}

		Assert.Equal ($"{expectedTabs}{{\n{expectedTabs}\t{line}\n{expectedTabs}}}\n", result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendInterpolatedLineTest (uint tabCount, string expectedTabs)
	{
		string result;
		var val1 = "Hello";
		var val2 = "World";
		var val3 = '!';
		var line = "Hello World!";
		var expected = $"{expectedTabs}{{\n{expectedTabs}\t{line}\n{expectedTabs}}}\n";
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			block.AppendLine ($"{val1} {val2}{val3}");
			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}


	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendRawTest (uint tabCount, string expectedTabs)
	{
		var input = @"
## Raw string
Because we are using a raw string  we expected:
  1. The string to be split in lines
  2. All lines should have the right indentation
     - This means nested one
  3. And all lines should have the correct tabs
";
		var expected = $@"
{expectedTabs}## Raw string
{expectedTabs}Because we are using a raw string  we expected:
{expectedTabs}  1. The string to be split in lines
{expectedTabs}  2. All lines should have the right indentation
{expectedTabs}     - This means nested one
{expectedTabs}  3. And all lines should have the correct tabs
";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendRaw (input);
			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendGeneratedCodeAttributeTest (uint tabCount, string expectedTabs)
	{
		var expected = $"{expectedTabs}[BindingImpl (BindingImplOptions.GeneratedCode)]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendGeneratedCodeAttribute (false);
			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendGeneratedCodeAttributeOptimizableTest (uint tabCount, string expectedTabs)
	{
		var expected =
			$"{expectedTabs}[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendGeneratedCodeAttribute ();
			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendEditorBrowsableAttributeTest (uint tabCount, string expectedTabs)
	{
		var expected = $"{expectedTabs}[EditorBrowsable (EditorBrowsableState.Never)]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendEditorBrowsableAttribute ();
			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void CreateEmptyBlockTest (uint tabCount, string expectedTabs)
	{
		var blockContent = "// the test";
		var expected = $@"{expectedTabs}{{
{expectedTabs}{"\t"}{blockContent}
{expectedTabs}}}
";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			using (var nested = block.CreateBlock (true)) {
				nested.AppendLine (blockContent);
			}

			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "", "if (true)")]
	[InlineData (1, "\t", "using (var t = new StringBuilder)")]
	[InlineData (5, "\t\t\t\t\t", "fixed (*foo)")]
	public void CreateBlockTest (uint tabCount, string expectedTabs, string blockType)
	{
		var blockContent = "// the test";
		var expected = $@"{expectedTabs}{blockType}
{expectedTabs}{{
{expectedTabs}{"\t"}{blockContent}
{expectedTabs}}}
";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			using (var nested = block.CreateBlock (blockType, true)) {
				nested.AppendLine (blockContent);
			}

			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void WriteHeaderTest (uint tabCount, string expectedTabs)
	{
		var expected = $@"{expectedTabs}// <auto-generated />

{expectedTabs}#nullable enable

";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.WriteHeader ();
			result = block.ToString ();
		}

		Assert.Equal (expected, result);
	}


	public static IEnumerable<object []> AppendMemberAvailabilityTestData {
		get {
			var builder = SymbolAvailability.CreateBuilder ();

			// single platform, available no version
			builder.Add (new SupportedOSPlatformData ("ios"));
			yield return [builder.ToImmutable (), "[SupportedOSPlatform (\"ios\")]\n"];
			builder.Clear ();

			// single platform available with version
			builder.Add (new SupportedOSPlatformData ("macos13.0"));
			yield return [builder.ToImmutable (), "[SupportedOSPlatform (\"macos13.0\")]\n"];
			builder.Clear ();

			// single platform available with version, unavailable with version
			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new UnsupportedOSPlatformData ("ios13.0"));
			yield return [builder.ToImmutable (), "[SupportedOSPlatform (\"ios\")]\n[UnsupportedOSPlatform (\"ios13.0\")]\n"];
			builder.Clear ();

			// several platforms available no version
			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new SupportedOSPlatformData ("tvos"));
			builder.Add (new SupportedOSPlatformData ("macos"));
			yield return [builder.ToImmutable (), "[SupportedOSPlatform (\"macos\")]\n[SupportedOSPlatform (\"ios\")]\n[SupportedOSPlatform (\"tvos\")]\n"];
			builder.Clear ();

			// several platforms available with version 
			builder.Add (new SupportedOSPlatformData ("ios12.0"));
			builder.Add (new SupportedOSPlatformData ("tvos12.0"));
			builder.Add (new SupportedOSPlatformData ("macos10.0"));
			yield return [builder.ToImmutable (), "[SupportedOSPlatform (\"macos10.0\")]\n[SupportedOSPlatform (\"ios12.0\")]\n[SupportedOSPlatform (\"tvos12.0\")]\n"];
			builder.Clear ();

			// several platforms unsupported
			// several platforms available with version 
			builder.Add (new UnsupportedOSPlatformData ("ios12.0"));
			builder.Add (new UnsupportedOSPlatformData ("tvos12.0"));
			builder.Add (new UnsupportedOSPlatformData ("macos"));
			yield return [builder.ToImmutable (), "[UnsupportedOSPlatform (\"macos\")]\n[UnsupportedOSPlatform (\"ios12.0\")]\n[UnsupportedOSPlatform (\"tvos12.0\")]\n"];
			builder.Clear ();
		}
	}

	[Theory]
	[MemberData (nameof (AppendMemberAvailabilityTestData))]
	void AppendMemberAvailabilityTest (SymbolAvailability availability, string expectedString)
	{
		var block = new TabbedStringBuilder (sb);
		block.AppendMemberAvailability (availability);
		var result = block.ToString ();
		Assert.Equal (expectedString, result);
	}
}
