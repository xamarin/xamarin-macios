using Microsoft.Macios.Generator.Extensions;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class StringExtensionsTests {

	[Theory]
	[InlineData ("", false)]
	[InlineData ("3Test", false)]
	[InlineData ("class", false)]
	[InlineData ("struct", false)]
	[InlineData ("if", false)]
	[InlineData ("else", false)]
	[InlineData ("!test", false)]
	[InlineData ("AVFoundation", true)]
	[InlineData ("AVFoundation Test", false)]
	[InlineData (null, false)]
	public void IsValidIdentifier (string? identifier, bool expected)
		=> Assert.Equal (expected, identifier.IsValidIdentifier ());
}
