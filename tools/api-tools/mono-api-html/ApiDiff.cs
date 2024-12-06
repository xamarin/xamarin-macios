//
// The main differences with mono-api-diff are:
// * this tool directly produce HTML similar to gdiff.sh used for Xamarin.iOS
// * this tool reports changes in an "evolutionary" way, not in a breaking way,
//   i.e. it does not assume the source assembly is right (but simply older)
// * the diff .xml output was not easy to convert back into the HTML format
//   that gdiff.sh produced
// 
// Authors
//    Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2013-2014 Xamarin Inc. http://www.xamarin.com
// Copyright 2018 Microsoft Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Mono.ApiTools {

	class State {
		public Formatter Formatter { get; set; }
		public Formatter [] Formatters { get; set; }
		public string Assembly { get; set; }
		public string Namespace { get; set; }
		public string Type { get; set; }
		public string BaseType { get; set; }
		public string Parent { get; set; }
		public List<Regex> IgnoreAdded { get; } = new List<Regex> ();
		public List<Regex> IgnoreNew { get; } = new List<Regex> ();
		public List<Regex> IgnoreRemoved { get; } = new List<Regex> ();
		public bool IgnoreParameterNameChanges { get; set; }
		public bool IgnoreVirtualChanges { get; set; }
		public bool IgnoreAddedPropertySetters { get; set; }
		public bool IgnoreNonbreaking { get; set; }
		public bool Lax { get; set; }
		public bool Colorize { get; set; } = true;
		public int Verbosity { get; set; }
		public string SourceFile;
		public string TargetFile;
		public Dictionary<string, string> TargetClassHierarchyMap;

		public void LogDebugMessage (string value)
		{
#if !EXCLUDE_DRIVER
			if (Verbosity == 0)
				return;
			Console.WriteLine (value);
#endif
		}
	}

#if !EXCLUDE_DRIVER
	class Program {

		public static int Main (string[] args)
		{
			var showHelp = false;
			List<string> extra = null;
			var config = new ApiDiffFormattedConfig ();

			var options = new Mono.Options.OptionSet {
				{ "h|help", "Show this help", v => showHelp = true },
				{ "i|ignore=", "Ignore new, added, and removed members whose description matches a given C# regular expression (see below).",
					v => {
						var r = new Regex (v);
						config.IgnoreAdded.Add (r);
						config.IgnoreRemoved.Add (r);
						config.IgnoreNew.Add (r);
					}
				},
				{ "a|ignore-added=", "Ignore added members whose description matches a given C# regular expression (see below).",
					v => config.IgnoreAdded.Add (new Regex (v))
				},
				{ "r|ignore-removed=", "Ignore removed members whose description matches a given C# regular expression (see below).",
					v => config.IgnoreRemoved.Add (new Regex (v))
				},
				{ "n|ignore-new=", "Ignore new namespaces and types whose description matches a given C# regular expression (see below).",
					v => config.IgnoreNew.Add (new Regex (v))
				},
				{ "ignore-changes-parameter-names", "Ignore changes to parameter names for identically prototyped methods.",
					v => config.IgnoreParameterNameChanges   = v != null
				},
				{ "ignore-changes-property-setters", "Ignore adding setters to properties.",
					v => config.IgnoreAddedPropertySetters = v != null
				},
				{ "ignore-changes-virtual", "Ignore changing non-`virtual` to `virtual` or adding `override`.",
					v => config.IgnoreVirtualChanges = v != null
				},
				{ "c|colorize:", "Colorize HTML output", v => config.Colorize = string.IsNullOrEmpty (v) ? true : bool.Parse (v) },
				{ "x|lax", "Ignore duplicate XML entries", v => config.IgnoreDuplicateXml = true },
				{ "ignore-nonbreaking", "Ignore all nonbreaking changes", v => config.IgnoreNonbreaking = true },
				{ "v|verbose:", "Verbosity level; when set, will print debug messages",
				  (int? v) => config.Verbosity = v ?? (config.Verbosity + 1)},
				{ "md|markdown=", "Output markdown to the specified file", v => config.MarkdownOutput = v },
				{ "html=", "Output html to the specified file", v => config.HtmlOutput = v },
				new Mono.Options.ResponseFileSource (),
			};

			try {
				extra = options.Parse (args);
			} catch (Mono.Options.OptionException e) {
				Console.WriteLine ("Option error: {0}", e.Message);
				extra = null;
			}

			if (showHelp || extra == null || extra.Count < 2 || extra.Count > 3) {
				Console.WriteLine (@"Usage: mono-api-html [options] <reference.xml> <assembly.xml> [diff.html]");
				Console.WriteLine ();
				Console.WriteLine ("Available options:");
				options.WriteOptionDescriptions (Console.Out);
				Console.WriteLine ();
				Console.WriteLine ("Ignoring Members:");
				Console.WriteLine ();
				Console.WriteLine ("  Members that were added can be filtered out of the diff by using the");
				Console.WriteLine ("  -i, --ignore-added option. The option takes a C# regular expression");
				Console.WriteLine ("  to match against member descriptions. For example, to ignore the");
				Console.WriteLine ("  introduction of the interfaces 'INSCopying' and 'INSCoding' on types");
				Console.WriteLine ("  pass the following to mono-api-html:");
				Console.WriteLine ();
				Console.WriteLine ("    mono-api-html ... -i 'INSCopying$' -i 'INSCoding$'");
				Console.WriteLine ();
				Console.WriteLine ("  The regular expressions will match any member description ending with");
				Console.WriteLine ("  'INSCopying' or 'INSCoding'.");
				Console.WriteLine ();
				return showHelp ? 0 : 1;
			}

			var input = extra [0];
			var output = extra [1];
			if (extra.Count > 2)
				config.HtmlOutput = extra [2];

			try {
				ApiDiffFormatted.Generate (input, output, config);
			} catch (Exception e) {
				Console.WriteLine (e);
				return 1;
			}
			return 0;
		}
	}
#endif

	public class ApiDiffFormattedConfig {
		public string HtmlOutput { get; set; }
		public string MarkdownOutput { get; set; }
		public List<Regex> IgnoreAdded { get; set; } = new List<Regex> ();
		public List<Regex> IgnoreNew { get; set; } = new List<Regex> ();
		public List<Regex> IgnoreRemoved { get; set; } = new List<Regex> ();
		public bool IgnoreParameterNameChanges { get; set; }
		public bool IgnoreVirtualChanges { get; set; }
		public bool IgnoreAddedPropertySetters { get; set; }
		public bool IgnoreNonbreaking { get; set; }
		public bool IgnoreDuplicateXml { get; set; }
		public bool Colorize { get; set; } = true;

		internal int Verbosity { get; set; }
	}

	public static class ApiDiffFormatted {
		public static void Generate (string firstInfo, string secondInfo, ApiDiffFormattedConfig config = null)
		{
			var state = CreateState (config, firstInfo, secondInfo);
			var ac = new AssemblyComparer (state);
			Generate (ac, state);
		}

		static void Generate (AssemblyComparer ac, State state)
		{
			ac.Compare ();

			foreach (var formatter in state.Formatters) {
				formatter.Flush ();
				if (formatter.StringBuilder.Length > 0) {
					var title = $"{ac.SourceAssembly}.dll";
					if (ac.SourceAssembly != ac.TargetAssembly)
						title += $" vs {ac.TargetAssembly}.dll";

					var sb = formatter.StringBuilder;
					formatter.PushOutput ();
					formatter.BeginDocument ($"API diff: {title}");
					formatter.BeginAssembly ();
					formatter.Write (sb);
					formatter.EndAssembly ();
					formatter.EndDocument ();
					formatter.Flush ();
				}
				File.WriteAllText (formatter.OutputPath, formatter.StringBuilder.ToString ());
			}
		}

		static State CreateState (ApiDiffFormattedConfig config, string firstInfo, string secondInfo)
		{
			if (config == null)
				config = new ApiDiffFormattedConfig ();

			var state = new State {
				Colorize = config.Colorize,
				Formatter = null,
				IgnoreAddedPropertySetters = config.IgnoreAddedPropertySetters,
				IgnoreVirtualChanges = config.IgnoreVirtualChanges,
				IgnoreNonbreaking = config.IgnoreNonbreaking,
				IgnoreParameterNameChanges = config.IgnoreParameterNameChanges,
				Lax = config.IgnoreDuplicateXml,
				SourceFile = firstInfo,
				TargetFile = secondInfo,

				Verbosity = config.Verbosity
			};

			state.IgnoreAdded.AddRange (config.IgnoreAdded);
			state.IgnoreNew.AddRange (config.IgnoreNew);
			state.IgnoreRemoved.AddRange (config.IgnoreRemoved);

			var formatters = new List<Formatter> ();
			if (!string.IsNullOrWhiteSpace (config.HtmlOutput))
				formatters.Add (new HtmlFormatter (state) { OutputPath = config.HtmlOutput });
			if (!string.IsNullOrWhiteSpace (config.MarkdownOutput))
				formatters.Add (new MarkdownFormatter (state) { OutputPath = config.MarkdownOutput });
			if (formatters.Count > 1) {
				state.Formatter = new MultiplexedFormatter (state, formatters.ToArray ());
			} else if (formatters.Count == 0) {
				throw new ArgumentException ("No output files.");
			} else {
				state.Formatter = formatters [0];
			}
			state.Formatters = formatters.ToArray ();

			if (state.IgnoreNonbreaking) {
				state.IgnoreAddedPropertySetters = true;
				state.IgnoreVirtualChanges = true;
				state.IgnoreNew.Add (new Regex (".*"));
				state.IgnoreAdded.Add (new Regex (".*"));
			}

			return state;
		}
	}
}
