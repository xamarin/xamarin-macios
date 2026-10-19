// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using NUnit.Framework;

using Xamarin.Tests;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class SignListTest {
		const int MaxZipDepth = 8;

		static readonly string [] signingGroups = { "Skip", "FirstParty", "ThirdParty", "MacDeveloperSign", "MacDeveloperSignHarden" };

		[Test]
		public void WorkloadPackagesHaveSigningEntries ()
		{
			var root = Configuration.SourceRoot;
			var signList = Path.Combine (root, "dotnet", "Workloads", "SignList.xml");
			var packageDir = Configuration.EvaluateVariable ("DOTNET_NUPKG_DIR");

			Assert.That (Directory.Exists (packageDir), Is.True, $"Package directory '{packageDir}' does not exist. Build or download the workload packages first.");
			var packages = Directory.GetFiles (packageDir, "*.nupkg", SearchOption.TopDirectoryOnly);
			Assert.That (packages, Is.Not.Empty, $"No nupkgs found in '{packageDir}'.");

			var patterns = ReadPatterns (signList);
			var matched = new bool [patterns.Count];
			var missing = new List<string> ();
			var notExecutables = new List<string> ();
			foreach (var package in packages) {
				using var archive = ZipFile.OpenRead (package);
				CheckArchive (archive, Path.GetFileName (package), "", "", patterns, matched, missing, notExecutables, 0);
			}
			if (!Configuration.AnyIgnoredPlatforms ()) {
				for (var i = 0; i < patterns.Count; i++) {
					if (!matched [i])
						Console.WriteLine ($"Unused SignList.xml entry: <{patterns [i].Group} Include=\"{patterns [i].Include}\" />");
				}
			}
			Assert.That (missing, Is.Empty, $"Files missing from {signList}:\n{string.Join ("\n", missing)}");
			Assert.That (notExecutables, Is.Empty, $"Files matched by a Mach-O signing entry in {signList} that aren't Mach-O executables:\n{string.Join ("\n", notExecutables)}");
		}

		static List<(string Group, string Include, Regex Pattern)> ReadPatterns (string signList)
		{
			var document = XDocument.Load (signList);
			var patterns = new List<(string Group, string Include, Regex Pattern)> ();
			foreach (var group in signingGroups) {
				foreach (var item in document.Descendants (group)) {
					var include = (string?) item.Attribute ("Include");
					if (string.IsNullOrEmpty (include)) {
						Assert.Fail ($"{group} entry without Include in {signList}");
						continue;
					}
					Assert.That (include, Does.Not.Contain ("/"), $"Use Windows-style separators in {signList}: {include}");
					var expression = Regex.Escape (include).Replace (@"\*", @"[^\\]*").Replace (@"\?", @"[^\\]");
					patterns.Add ((group, include, new Regex ($"(^|\\\\){expression}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)));
				}
			}
			Assert.That (patterns, Is.Not.Empty, $"No signing entries found in {signList}");
			return patterns;
		}

		static bool IsMachO (ZipArchiveEntry entry)
		{
			if (entry.Length < 4)
				return false;

			var magic = new byte [4];
			using var stream = entry.Open ();
			stream.ReadExactly (magic);
			// Thin Mach-O files (32/64-bit, both byte orders) and universal (fat) binaries.
			return BinaryPrimitives.ReadUInt32BigEndian (magic) is 0xFEEDFACE or 0xFEEDFACF or 0xCEFAEDFE or 0xCFFAEDFE or 0xCAFEBABE or 0xCAFEBABF;
		}

		static void CheckArchive (ZipArchive archive, string archiveName, string zipPath, string matchPrefix, IReadOnlyList<(string Group, string Include, Regex Pattern)> patterns, bool [] matched, List<string> missing, List<string> notExecutables, int depth)
		{
			foreach (var entry in archive.Entries) {
				if (entry.Name.Length == 0)
					continue;

				var name = entry.FullName.Replace ('/', '\\');
				if (entry.Name.EndsWith (".zip", StringComparison.OrdinalIgnoreCase)) {
					Assert.That (depth, Is.LessThan (MaxZipDepth), $"Nested ZIP depth exceeds {MaxZipDepth}: {archiveName}!{zipPath}{entry.FullName}");
					using var stream = entry.Open ();
					using var nested = new ZipArchive (stream, ZipArchiveMode.Read);
					CheckArchive (nested, archiveName, zipPath + entry.FullName + "!", matchPrefix + Path.GetFileNameWithoutExtension (entry.Name) + "\\", patterns, matched, missing, notExecutables, depth + 1);
				} else if (name.EndsWith (".dll", StringComparison.OrdinalIgnoreCase)
					|| name.EndsWith (".exe", StringComparison.OrdinalIgnoreCase)
					|| name.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase)) {
					var found = false;
					for (var i = 0; i < patterns.Count; i++) {
						if (patterns [i].Pattern.IsMatch (matchPrefix + name)) {
							matched [i] = true;
							found = true;
						}
					}
					if (!found)
						missing.Add ($"{archiveName}!{zipPath}{entry.FullName}");
				} else {
					// Executables don't have a well-known file extension, so only the ones with a Mach-O signing entry are checked.
					for (var i = 0; i < patterns.Count; i++) {
						if (!patterns [i].Group.StartsWith ("MacDeveloperSign", StringComparison.Ordinal) || !patterns [i].Pattern.IsMatch (matchPrefix + name))
							continue;
						matched [i] = true;
						// The debug symbols in a .dSYM are Mach-O files too, but they're not executables.
						if (!IsMachO (entry) || name.Contains (".dSYM\\", StringComparison.OrdinalIgnoreCase))
							notExecutables.Add ($"{archiveName}!{zipPath}{entry.FullName} (<{patterns [i].Group} Include=\"{patterns [i].Include}\" />)");
					}
				}
			}
		}
	}
}
