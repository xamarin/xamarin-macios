// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Availability;

public class AvailabilityTriviaTests {
	
	[Fact]
	public void AllSupportedPlatforms ()
	{
		SymbolAvailability.Builder builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios12.0"));
		builder.Add (new SupportedOSPlatformData ("tvos12.0"));
		builder.Add (new SupportedOSPlatformData ("macos10.14"));
		builder.Add (new SupportedOSPlatformData ("macCatalyst13.0"));
		var availability = builder.ToImmutable ();
		var trivia = new AvailabilityTrivia (availability);
		Assert.Null (trivia.Start);
		Assert.Null (trivia.End);
		Assert.Null (availability.Trivia);
	}

	[Fact]
	public void SomeUnsupportedVersions () {
		SymbolAvailability.Builder builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios12.0"));
		builder.Add (new UnsupportedOSPlatformData ("ios9.0"));
		builder.Add (new SupportedOSPlatformData ("tvos12.0"));
		builder.Add (new SupportedOSPlatformData ("macos10.14"));
		builder.Add (new SupportedOSPlatformData ("maccatalyst13.0"));
		var availability = builder.ToImmutable ();
		var trivia = new AvailabilityTrivia (availability);
		Assert.Null (trivia.Start);
		Assert.Null (trivia.End);
		Assert.Null (availability.Trivia);
	}
	
	[Fact]
	public void SingleFullyUnsupportedPlatform () {
		SymbolAvailability.Builder builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new UnsupportedOSPlatformData ("ios"));
		builder.Add (new SupportedOSPlatformData ("tvos12.0"));
		builder.Add (new SupportedOSPlatformData ("macos10.14"));
		builder.Add (new SupportedOSPlatformData ("maccatalyst13.0"));
		var availability = builder.ToImmutable ();
		var trivia = new AvailabilityTrivia (availability);
		Assert.Equal ("#if !IOS", trivia.Start);
		Assert.Equal ("#endif", trivia.End);
		Assert.NotNull (availability.Trivia);
	}
	
	[Fact]
	public void DoubleUnsupportedPlatform () {
		SymbolAvailability.Builder builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios"));
		builder.Add (new SupportedOSPlatformData ("tvos12.0"));
		builder.Add (new UnsupportedOSPlatformData ("macos"));
		builder.Add (new UnsupportedOSPlatformData ("maccatalyst"));
		var availability = builder.ToImmutable ();
		var trivia = new AvailabilityTrivia (availability);
		Assert.Equal ("#if IOS || TVOS", trivia.Start);
		Assert.Equal ("#endif", trivia.End);
		Assert.NotNull (availability.Trivia);
	}
	
	[Fact]
	public void SingleSupportedPlatform () {
		SymbolAvailability.Builder builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios"));
		builder.Add (new UnsupportedOSPlatformData( "tvos"));
		builder.Add (new UnsupportedOSPlatformData ("macos"));
		builder.Add (new UnsupportedOSPlatformData ("maccatalyst"));
		var availability = builder.ToImmutable ();
		var trivia = new AvailabilityTrivia (availability);
		Assert.Equal ("#if IOS", trivia.Start);
		Assert.Equal ("#endif", trivia.End);
		Assert.NotNull (availability.Trivia);
	}

}
