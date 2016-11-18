// Copyright 2013,2016 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Xamarin.Utils;

namespace Xamarin.Bundler {
	public class BitcodeConverter {
		string input;
		string outputFile;
		Abi abi;
		ApplePlatform platform;
		Version deployment_target;

		public BitcodeConverter (string input, string outputFile, ApplePlatform platform, Abi abi, Version deploymentTarget)
		{
			this.input = input;
			this.outputFile = outputFile;
			this.abi = abi;
			this.platform = platform;
			deployment_target = deploymentTarget;
		}

		public void Convert ()
		{
			Driver.Log (3, "Converting '{0}' to bitcode file {1}.", input, outputFile);

			var reader = new StreamReader (input);
			var writer = new StreamWriter (outputFile);

			writer.WriteLine ("; ModuleID = '{0}'", input);

			//This is for x86_64
			switch (platform) {
			case ApplePlatform.TVOS:
				if ((abi & Abi.ARM64) != 0) {
					writer.WriteLine ("target datalayout = \"e-m:o-i64:64-i128:128-n32:64-S128\"");
					writer.Write ("target triple = \"arm64-apple-tvos");
				} else if ((abi & Abi.x86_64) != 0) {
					writer.WriteLine ("target datalayout = \"e-m:o-p:32:32-f64:32:64-f80:128-n8:16:32-S128\"");
					//not 100% of this one (below)
					writer.Write ("target triple = \"x86_64-apple-tvos");
				} else {
					throw ErrorHelper.CreateError (1301, "Unsupported TvOS ABI: {0}.", abi);
				}
				writer.Write (deployment_target.Major);
				writer.Write ('.');
				writer.Write (deployment_target.Minor);
				writer.Write ('.');
				writer.Write (deployment_target.Revision < 0 ? 0 : deployment_target.Revision);
				writer.WriteLine ('"');
				break;
			default:
				throw ErrorHelper.CreateError (1300, "Unsupported bitcode platform: {0}.", platform);
			}


			writer.WriteLine ("!llvm.module.flags = !{!0}");
			writer.WriteLine ("!llvm.ident = !{!1}");
			writer.WriteLine ("!0 = !{i32 1, !\"PIC Level\", i32 2}");
			writer.WriteLine ("!1 = !{!\"Apple LLVM version 7.0.0 (clang-700.0.72)\"}");
			writer.WriteLine ();

			string s;
			int line = 0;
			while ((s = reader.ReadLine ()) != null) {
				++line;
				s = s.Trim ();
				if (s.Length == 0)
					continue;

				if (s.StartsWith (".asciz", StringComparison.Ordinal) || s.StartsWith (".ascii", StringComparison.Ordinal))
					s = FixAsciz (s, line);
				else if (s.StartsWith (".file"))
					s = FixFile (s, line);
				else if (s.Contains ("\""))
					s = s.Replace ("\"", "\\22");

				writer.WriteLine ("module asm \"{0}\"", s);
			}
			reader.Close ();
			writer.Close ();
			
		}

		static bool IsDigit (char c) {
			return (c >= '0' && c <= '9');
		}

		static bool IsHex (char c) {
			return IsDigit (c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}

		static bool ParseHex (string str, ref int start_index, ref int result)
		{
			int i = 0;
			while (start_index + i < str.Length && i < 2) {
				if (IsHex (str [start_index + i]))
					++i;
				else
					break;
			}
			if (i == 0)
				return false;

			result = System.Convert.ToInt32 (str.Substring (start_index, i), 16);
			start_index += i - 1; //the main loop will skip the last character

			return true;
		}

		static bool ParseOctal (string str, ref int start_index, ref int result, ref string error_msg)
		{
			int i = 0;
			while (start_index + i < str.Length && i < 3) {
				if (IsDigit (str [start_index + i]))
					++i;
				else
					break;
			}
			if (i != 3) {
				error_msg = string.Format ("Column {0} expected 3 digits but got {1}, content is {2}", start_index, i, str.Substring (start_index - 1, 4));
				return false;
			}

			result = System.Convert.ToInt32 (str.Substring (start_index, i), 8);
			start_index += i - 1; //the main loop will skip the last character

			return true;
		}

		string FixAsciz (string s, int line) {
			bool first = true;
			var m = Regex.Match (s, "^.asci(i|z)\\s*\"(.*)\"$");
			if (!m.Success)
				return s;
			var str = m.Groups [2].Value;

			var res = new StringBuilder (str.Length * 3);
			res.Append (".byte ");
			/* it's a regular C string, parse time! */
			for (int i = 0; i < str.Length; ++i) {
				int to_append = 0;
				if (str [i] == '\\') {
					++i;
					if (i >= str.Length)
						throw ErrorHelper.CreateError (1302, "Invalid escape sequence when converting .s to .ll, \\ as the last characted in {0}:{1}.", input, line);
					switch (str [i]) {
					case 'b':
						to_append = 0x8;
						break;
					case 'f':
						to_append = 0xc;
						break;
					case 'n':
						to_append = 0xa;
						break;
					case 'r':
						to_append = 0xd;
						break;
					case 't':
						to_append = 9;
						break;
					case '\"':
					case '\\':
						to_append = str [i];
						break;
					case 'x':
					case 'X':
						++i;
						if (!ParseHex (str, ref i, ref to_append))
							throw ErrorHelper.CreateError (1302, "Invalid escape sequence when converting .s to .ll, bad hex escape in {0}:{1}.", input, line);

						break;
					default:
						if (IsDigit (str [i])) {
							string error_msg = null;
							if (!ParseOctal (str, ref i, ref to_append, ref error_msg))
								throw ErrorHelper.CreateError (1302, "Invalid escape sequence when converting .s to .ll, bad octal escape in {0}:{1} due to {2}.", input, line, error_msg);
						} else
							to_append = str [i]; // "\K" is the same as "K"
						break;
					}
				} else {
					to_append = str [i];
				}

				if (!first)
					res.Append (", ");
				first = false;
				res.Append (to_append.ToString ());
			}

			if (s.StartsWith (".asciz", StringComparison.Ordinal)) {
				if (!first)
					res.Append (", ");
				res.Append ("0");
			}
			return res.ToString ();
		}

		string FixFile (string s, int line) {
			var m = Regex.Match (s, "^.file\\s*(\\d+)\\s*\"(.*)\"$");
			if (!m.Success)
				return s;
			var dbg_line = m.Groups [1].Value;
			var str = m.Groups [2].Value;

			var res = new StringBuilder (str.Length * 3);
			res.Append (".file " + dbg_line + " " + "\\22");

			for (int i = 0; i < str.Length; ++i) {
				if (str [i] == '\\' && i + 1 < str.Length && str [i + 1] == '\\') {
					i ++;
					res.Append ("\\5c\\5c");
				} else {
					res.Append (str [i]);
				}
			}
			res.Append ("\\22");

			return res.ToString ();
		}
	}
}

