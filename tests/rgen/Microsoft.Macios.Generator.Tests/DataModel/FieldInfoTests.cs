// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class FieldInfoTests {

	[Fact]
	public void CompareSame ()
	{
		var x = new FieldInfo<ObjCBindings.Property> (new ("test"), "");
		var y = new FieldInfo<ObjCBindings.Property> (new ("test"), "");

		Assert.True (x.Equals (y));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void CompareDiffAttr ()
	{
		var x = new FieldInfo<ObjCBindings.Property> (new ("xData"), "");
		var y = new FieldInfo<ObjCBindings.Property> (new ("test"), "");

		Assert.False (x.Equals (y));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffLibraryName ()
	{
		var x = new FieldInfo<ObjCBindings.Property> (new ("test"), "xLib");
		var y = new FieldInfo<ObjCBindings.Property> (new ("test"), "yLib");

		Assert.False (x.Equals (y));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffLibraryPath ()
	{
		var x = new FieldInfo<ObjCBindings.Property> (new ("test"), "lib", "xpath");
		var y = new FieldInfo<ObjCBindings.Property> (new ("test"), "lib");

		Assert.False (x.Equals (y));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void DeconstructTests ()
	{
		var x = new FieldInfo<ObjCBindings.Property> (new ("test"), "lib", "xpath");
		var (attr, name, path) = x;
		Assert.Equal (x.FieldData, attr);
		Assert.Equal (name, x.LibraryName);
		Assert.Equal (path, x.LibraryPath);
	}
}
