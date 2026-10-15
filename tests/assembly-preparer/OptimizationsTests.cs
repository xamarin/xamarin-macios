// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AssemblyPreparerTests;

public class OptimizationsTests {
	[TestCase ("redirect-class-handles")]
	[TestCase ("+redirect-class-handles")]
	[TestCase ("-redirect-class-handles")]
	public void RemovedClassHandleOptimization (string option)
	{
		var optimizations = new Optimizations ();
		var messages = new List<ProductException> ();

		optimizations.Parse (ApplePlatform.iOS, option, messages);

		Assert.That (messages, Has.Count.EqualTo (1), "Diagnostics");
		Assert.That (messages [0].Code, Is.EqualTo (134), "Error code");
		Assert.That (messages [0].Message, Is.EqualTo ("The optimization 'redirect-class-handles' is no longer supported."), "Error message");
	}

	[Test]
	public void AllExcludesRemovedClassHandleOptimization ()
	{
		var optimizations = new Optimizations ();
		var messages = new List<ProductException> ();

		optimizations.Parse (ApplePlatform.iOS, "all", messages);

		Assert.That (messages, Is.Empty, "Diagnostics");
		Assert.That (optimizations.ToString (), Does.Not.Contain ("redirect-class-handles"), "Enabled optimizations");
	}
}
