// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.IO;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.IO;

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
	public void ConstructorNotBlockTest (int tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.WriteLine ("Test");
			result = block.ToCode ();
		}

		Assert.Equal ($"{expectedTabs}Test\n", result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void ConstructorBlockTest (int tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			block.WriteLine ("Test");
			result = block.ToCode ();
		}

		Assert.Equal ($"{expectedTabs}{{\n{expectedTabs}\tTest\n{expectedTabs}}}\n", result);
	}

	[Theory]
	[InlineData (0)]
	[InlineData (1)]
	[InlineData (5)]
	public void AppendLineTest (int tabCount)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.WriteLine ();
			result = block.ToCode ();
		}

		// an empty line should have not tabs
		Assert.Equal ("\n", result);
	}
	
	[Theory]
	[InlineData (0)]
	[InlineData (1)]
	[InlineData (5)]
	public async Task AppendLineTestAsync (int tabCount)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			await block.WriteLineAsync ();
			result = block.ToCode ();
		}

		// an empty line should have not tabs
		Assert.Equal ("\n", result);
	}

	[Theory]
	[InlineData ("// test comment", 0, "")]
	[InlineData ("var t = 1;", 1, "\t")]
	[InlineData ("Console.WriteLine (\"1\");", 5, "\t\t\t\t\t")]
	public void AppendLineStringTest (string line, int tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			block.WriteLine (line);
			result = block.ToCode ();
		}

		Assert.Equal ($"{expectedTabs}{{\n{expectedTabs}\t{line}\n{expectedTabs}}}\n", result);
	}
	
	[Theory]
	[InlineData ("// test comment", 0, "")]
	[InlineData ("var t = 1;", 1, "\t")]
	[InlineData ("Console.WriteLine (\"1\");", 5, "\t\t\t\t\t")]
	public async Task AppendLineStringTestAsync (string line, int tabCount, string expectedTabs)
	{
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			await block.WriteLineAsync (line);
			result = block.ToCode ();
		}

		Assert.Equal ($"{expectedTabs}{{\n{expectedTabs}\t{line}\n{expectedTabs}}}\n", result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendInterpolatedLineTest (int tabCount, string expectedTabs)
	{
		string result;
		var val1 = "Hello";
		var val2 = "World";
		var val3 = '!';
		var line = "Hello World!";
		var expected = $"{expectedTabs}{{\n{expectedTabs}\t{line}\n{expectedTabs}}}\n";
		using (var block = new TabbedStringBuilder (sb, tabCount, true)) {
			block.WriteLine ($"{val1} {val2}{val3}");
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}


	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendRawTest (int tabCount, string expectedTabs)
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
			block.WriteRaw (input);
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}
	
	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public async Task AppendRawTestAsync (int tabCount, string expectedTabs)
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
			await block.WriteRawAsync (input);
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendGeneratedCodeAttributeTest (int tabCount, string expectedTabs)
	{
		var expected = $"{expectedTabs}[BindingImpl (BindingImplOptions.GeneratedCode)]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendGeneratedCodeAttribute (false);
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendGeneratedCodeAttributeOptimizableTest (int tabCount, string expectedTabs)
	{
		var expected =
			$"{expectedTabs}[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendGeneratedCodeAttribute ();
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (EditorBrowsableState.Advanced, 0, "")]
	[InlineData (EditorBrowsableState.Never, 1, "\t")]
	[InlineData (EditorBrowsableState.Always, 5, "\t\t\t\t\t")]
	public void AppendEditorBrowsableAttributeTest (EditorBrowsableState state, int tabCount, string expectedTabs)
	{
		var expected = $"{expectedTabs}[EditorBrowsable (EditorBrowsableState.{state})]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendEditorBrowsableAttribute (state);
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendNotificationAdviceTests (int tabCount, string expectedTabs)
	{
		var className = "TestClass";
		var notificationName = "DidWriteAttribute";
		var expected = $"{expectedTabs}[Advice (\"Use '{className}.Notifications.{notificationName}' helper method instead.\")]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendNotificationAdvice (className, notificationName);
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void AppendDesignatedInitializer (int tabCount, string expectedTabs)
	{
		var expected = $"{expectedTabs}[DesignatedInitializer]\n";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.AppendDesignatedInitializer ();
			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void CreateEmptyBlockTest (int tabCount, string expectedTabs)
	{
		var blockContent = "// the test";
		var expected = $@"{expectedTabs}{{
{expectedTabs}{"\t"}{blockContent}
{expectedTabs}}}
";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			using (var nested = block.CreateBlock (true)) {
				nested.WriteLine (blockContent);
			}

			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "", "if (true)")]
	[InlineData (1, "\t", "using (var t = new StringBuilder)")]
	[InlineData (5, "\t\t\t\t\t", "fixed (*foo)")]
	public void CreateBlockTest (int tabCount, string expectedTabs, string blockType)
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
				nested.WriteLine (blockContent);
			}

			result = block.ToCode ();
		}

		Assert.Equal (expected, result);
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void WriteHeaderTest (int tabCount, string expectedTabs)
	{
		var expected = $@"{expectedTabs}// <auto-generated />

{expectedTabs}#nullable enable

";
		string result;
		using (var block = new TabbedStringBuilder (sb, tabCount)) {
			block.WriteHeader ();
			result = block.ToCode ();
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
		var result = block.ToCode ();
		Assert.Equal (expectedString, result);
	}

	[Fact]
	public void ClearTests ()
	{
		var block = new TabbedStringBuilder (sb);
		var line = "My Line";
		block.Write (line);
		Assert.Equal (line, block.ToCode ());
		block.Clear ();
		Assert.Equal (string.Empty, block.ToCode ());
	}

	[Fact]
	public void CreateBlockStringArray ()
	{
		var expecteString =
@"using (var m1 = new MemoryStream())
using (var m2 = new MemoryStream())
using (var m3 = new MemoryStream())
{
	// this is an example with several usings
}
";
		var baseBlock = new TabbedStringBuilder (sb);
		// create a list of lines to get the new block
		var usingStatements = new [] {
			"using (var m1 = new MemoryStream())",
			"using (var m2 = new MemoryStream())",
			"using (var m3 = new MemoryStream())",
		};
		using (var usingBlock = baseBlock.CreateBlock (usingStatements, true)) {
			usingBlock.WriteLine ("// this is an example with several usings");
		}
		Assert.Equal (expecteString, baseBlock.ToCode ());
	}
}
