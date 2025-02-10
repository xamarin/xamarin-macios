// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using ObjCBindings;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Attributes;

public class FieldDataTests {
	[Fact]
	public void TestFieldDataEqualsDiffSymbolName ()
	{
		const string xSymbolName = "x";
		const string ySymbolName = "y";
		const string libraryName = "library";
		var x = new FieldData<EnumValue> (xSymbolName, libraryName, EnumValue.Default);
		var y = new FieldData<EnumValue> (ySymbolName, libraryName, EnumValue.Default);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Theory]
	[InlineData (null, null, true)]
	[InlineData ("", "", true)]
	[InlineData ("", null, false)]
	[InlineData (null, "", false)]
	[InlineData ("xLib", "yLib", false)]
	public void TestFieldDataEqualsDiffLibraryName (string? xLibName, string? yLibName, bool expected)
	{
		const string symbolName = "symbol";
		var x = new FieldData<EnumValue> (symbolName, xLibName, EnumValue.Default);
		var y = new FieldData<EnumValue> (symbolName, yLibName, EnumValue.Default);
		Assert.Equal (expected, x.Equals (y));
		Assert.Equal (expected, y.Equals (x));
		Assert.Equal (expected, x == y);
		Assert.Equal (!expected, x != y);
	}

	[Theory]
	[InlineData (StringComparison.Ordinal, StringComparison.Ordinal, true)]
	[InlineData (StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase, false)]
	public void TestFieldDataEqualsDiffFlag (StringComparison xFlag, StringComparison yFlag, bool expected)
	{
		const string symbolName = "symbol";
		const string libraryName = "library";
		var x = new FieldData<StringComparison> (symbolName, libraryName, xFlag);
		var y = new FieldData<StringComparison> (symbolName, libraryName, yFlag);
		Assert.Equal (expected, x.Equals (y));
		Assert.Equal (expected, y.Equals (x));
		Assert.Equal (expected, x == y);
		Assert.Equal (!expected, x != y);
	}


	class TestDataToString : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new FieldData<EnumValue> ("symbol", null, EnumValue.Default),
				"{ SymbolName: 'symbol', LibraryName: 'null', Flags: 'Default' }"
			];
			yield return [
				new FieldData<EnumValue> ("symbol", "lib", EnumValue.Default),
				"{ SymbolName: 'symbol', LibraryName: 'lib', Flags: 'Default' }"
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataToString))]
	void TestFieldDataToString (FieldData<EnumValue> x, string expected)
		=> Assert.Equal (expected, x.ToString ());
}
