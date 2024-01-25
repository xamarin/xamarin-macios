// Copyright (c) Microsoft Corp

using System;

using Microsoft.Build.Framework;

using NUnit.Framework;

namespace Xamarin.MacDev.Tasks {

	[TestFixture]
	public class VerbosityTest {

		// when executed from VSfM the command line looks like this:
		[TestCase ("/Users/poupou/Library/Caches/VisualStudio/8.0/MSBuild/98811_1/MonoDevelop.MSBuildBuilder.exe 64249 False ...", LoggerVerbosity.Normal)]
		// when executed from the terminal it looks like:
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /t:rebuild introspection-ios.csproj /v:q", LoggerVerbosity.Quiet)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /v:quiet /t:build introspection-ios.csproj", LoggerVerbosity.Quiet)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /t:clean introspection-ios.csproj /v:m", LoggerVerbosity.Minimal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /v:minimal introspection-ios.csproj", LoggerVerbosity.Minimal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -t:rebuild introspection-ios.csproj -v:n", LoggerVerbosity.Normal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -v:normal -t:build introspection-ios.csproj", LoggerVerbosity.Normal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -t:clean introspection-ios.csproj -v:d", LoggerVerbosity.Detailed)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -v:detailed introspection-ios.csproj", LoggerVerbosity.Detailed)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -t:clean introspection-ios.csproj -v:diag", LoggerVerbosity.Diagnostic)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -v:diagnostic introspection-ios.csproj", LoggerVerbosity.Diagnostic)]

		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /t:rebuild introspection-ios.csproj /verbosity:quiet", LoggerVerbosity.Quiet)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /verbosity:q /t:build introspection-ios.csproj", LoggerVerbosity.Quiet)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /t:clean introspection-ios.csproj /verbosity:minimal", LoggerVerbosity.Minimal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll /verbosity:m introspection-ios.csproj", LoggerVerbosity.Minimal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -t:rebuild introspection-ios.csproj -verbosity:normal", LoggerVerbosity.Normal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -verbosity:n -t:build introspection-ios.csproj", LoggerVerbosity.Normal)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -t:clean introspection-ios.csproj -verbosity:detailed", LoggerVerbosity.Detailed)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -verbosity:d introspection-ios.csproj", LoggerVerbosity.Detailed)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -t:clean introspection-ios.csproj -verbosity:diagnostic", LoggerVerbosity.Diagnostic)]
		[TestCase ("/Library/Frameworks/Mono.framework/Versions/6.6.0/lib/mono/msbuild/15.0/bin/MSBuild.dll -verbosity:diag introspection-ios.csproj", LoggerVerbosity.Diagnostic)]

		[TestCase ("/prev:q", LoggerVerbosity.Normal)]
		[TestCase ("/evil-command-line-v:diag", LoggerVerbosity.Normal)]

		public void FromString (string commandLine, LoggerVerbosity expected)
		{
			var result = VerbosityUtils.GetVerbosityLevel (expected);
			Assert.That (VerbosityUtils.GetVerbosityLevel (commandLine), Is.EqualTo (result), commandLine);
		}

		[TestCase (LoggerVerbosity.Quiet, "-q -q -q -q")]
		[TestCase (LoggerVerbosity.Minimal, "-q -q")]
		[TestCase (LoggerVerbosity.Normal, "")]
		[TestCase (LoggerVerbosity.Detailed, "-v -v")]
		[TestCase (LoggerVerbosity.Diagnostic, "-v -v -v -v")]
		[TestCase ((LoggerVerbosity) (-1), "")]
		public void FromLoggerVerbosity (LoggerVerbosity v, string expectedResult)
		{
			var s = String.Join (" ", VerbosityUtils.GetVerbosityLevel (v));
			Assert.That (s, Is.EqualTo (expectedResult), v.ToString ());
		}
	}
}
