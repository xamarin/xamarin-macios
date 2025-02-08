// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Generator.IO;

namespace Microsoft.Macios.Transformer.Tests.IO;

public class TabbedStreamWriterTests : IDisposable {
	readonly string tempFile = Path.GetTempFileName ();

	public void Dispose ()
	{
		if (File.Exists (tempFile))
			File.Delete (tempFile);
	}

	string ReadFile ()
	{
		using var reader = new StreamReader (tempFile);
		return reader.ReadToEnd ();
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void ConstructorNotBlockTest (int tabCount, string expectedTabs)
	{
		using (var block = new TabbedStreamWriter (tempFile, tabCount)) {
			block.WriteLine ("Test");
		}
		Assert.Equal ($"{expectedTabs}Test\n", ReadFile ());
	}

	[Theory]
	[InlineData (0, "")]
	[InlineData (1, "\t")]
	[InlineData (5, "\t\t\t\t\t")]
	public void ConstructorBlockTest (int tabCount, string expectedTabs)
	{
		using (var block = new TabbedStreamWriter (tempFile, tabCount, true)) {
			block.WriteLine ("Test");
		}

		Assert.Equal ($"{expectedTabs}{{\n{expectedTabs}\tTest\n{expectedTabs}}}\n", ReadFile ());
	}

	[Fact]
	public void ConstructorBlockNestedTest ()
	{
		using (var block = new TabbedStreamWriter (tempFile, 0, false)) {
			block.WriteLine ("// create the first block");
			using (var block2 = block.CreateBlock ("using (var test1 = new Test ())", true)) {
				block2.WriteLine ("// call in first block");
			}
			block.WriteLine ();
			block.WriteLine ("// create second block");
			using (var block3 = block.CreateBlock ("using (var test2 = new Test ())", true)) {
				block3.WriteLine ("// create nested block");
				using (var block4 = block3.CreateBlock ("using (var test3 = new Test ())", true)) {
					block4.WriteLine ("// code inside test2.test3");
				}
			}
		}

		const string expectedResult = @"// create the first block
using (var test1 = new Test ())
{
	// call in first block
}

// create second block
using (var test2 = new Test ())
{
	// create nested block
	using (var test3 = new Test ())
	{
		// code inside test2.test3
	}
}
";
		Assert.Equal (expectedResult, ReadFile ());
	}

	[Fact]
	public async Task ConstructorBlockNestedAsyncTest ()
	{
		await using (var block = new TabbedStreamWriter (tempFile, 0, false)) {
			await block.WriteLineAsync ("// create the first block");
			await using (var block2 = block.CreateBlock ("using (var test1 = new Test ())", true)) {
				await block2.WriteLineAsync ("// call in first block");
			}
			await block.WriteLineAsync ();
			await block.WriteLineAsync ("// create second block");
			await using (var block3 = block.CreateBlock ("using (var test2 = new Test ())", true)) {
				await block3.WriteLineAsync ("// create nested block");
				await using (var block4 = block3.CreateBlock ("using (var test3 = new Test ())", true)) {
					await block4.WriteLineAsync ("// code inside test2.test3");
				}
			}
		}

		const string expectedResult = @"// create the first block
using (var test1 = new Test ())
{
	// call in first block
}

// create second block
using (var test2 = new Test ())
{
	// create nested block
	using (var test3 = new Test ())
	{
		// code inside test2.test3
	}
}
";
		Assert.Equal (expectedResult, ReadFile ());
	}
}
