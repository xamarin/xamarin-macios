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
					throw ErrorHelper.CreateError (1301, Errors.MT1301, abi);
				}
				writer.Write (deployment_target.Major);
				writer.Write ('.');
				writer.Write (deployment_target.Minor);
				writer.Write ('.');
				writer.Write (deployment_target.Revision < 0 ? 0 : deployment_target.Revision);
				writer.WriteLine ('"');
				break;
			default:
				throw ErrorHelper.CreateError (1300, Errors.MT1300, platform);
			}


			writer.WriteLine ("!llvm.module.flags = !{!0}");
			writer.WriteLine ("!llvm.ident = !{!1}");
			writer.WriteLine ("!0 = !{i32 1, !\"PIC Level\", i32 2}");
			writer.WriteLine ("!1 = !{!\"Apple LLVM version 7.0.0 (clang-700.0.72)\"}");
			writer.WriteLine ();

			string s;
			int line = 0;
			while ((s = reader.ReadLine ()) is not null) {
				++line;
				s = s.Trim ();
				if (s.Length == 0)
					continue;

				if (s.StartsWith (".asciz", StringComparison.Ordinal) || s.StartsWith (".ascii", StringComparison.Ordinal))
					s = FixAsciz (s, line);
				else if (s.StartsWith (".file", StringComparison.Ordinal))
					s = FixFile (s, line);
				else if (s.Contains ("\""))
					s = s.Replace ("\"", "\\22");

				writer.WriteLine ("module asm \"{0}\"", s);
			}
			reader.Close ();
			writer.Close ();

		}

		static bool IsDigit (byte c)
		{
			return (c >= '0' && c <= '9');
		}

		static bool IsHex (byte c)
		{
			return IsDigit (c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}

		static bool ParseHex (byte [] str, ref int start_index, ref byte result)
		{
			int i = 0;
			var sb = new StringBuilder ();
			while (start_index + i < str.Length && i < 2) {
				if (IsHex (str [start_index + i])) {
					sb.Append ((char) str [start_index + i]);
					++i;
				} else
					break;
			}
			if (i == 0)
				return false;

			result = System.Convert.ToByte (sb.ToString (), 16);
			start_index += i - 1; //the main loop will skip the last character

			return true;
		}

		static bool ParseOctal (byte [] str, ref int start_index, ref byte result, ref string error_msg)
		{
			int i = 0;
			var sb = new StringBuilder ();
			while (start_index + i < str.Length && i < 3) {
				if (IsDigit (str [start_index + i])) {
					sb.Append ((char) str [start_index + i]);
					++i;
				} else
					break;
			}
			if (i != 3) {
				error_msg = string.Format ("Column {0} expected 3 digits but got {1}, content is {2}", start_index, i, sb);
				return false;
			}

			result = System.Convert.ToByte (sb.ToString (), 8);
			start_index += i - 1; //the main loop will skip the last character

			return true;
		}

		string FixAsciz (string s, int line)
		{
			bool first = true;
			var m = Regex.Match (s, "^.asci(i|z)\\s*\"(.*)\"$");
			if (!m.Success)
				return s;
			var str = m.Groups [2].Value;

			var res = new StringBuilder (str.Length * 3);
			res.Append (".byte ");
			/* it's a regular C string, parse time! */
			var utf8 = Encoding.UTF8.GetBytes (str);
			for (int i = 0; i < utf8.Length; ++i) {
				byte to_append = 0;
				if (utf8 [i] == '\\') {
					++i;
					if (i >= utf8.Length)
						throw ErrorHelper.CreateError (1302, Errors.MT1302_A, input, line);
					switch (utf8 [i]) {
					case (byte) 'b':
						to_append = 0x8;
						break;
					case (byte) 'f':
						to_append = 0xc;
						break;
					case (byte) 'n':
						to_append = 0xa;
						break;
					case (byte) 'r':
						to_append = 0xd;
						break;
					case (byte) 't':
						to_append = 9;
						break;
					case (byte) '\"':
					case (byte) '\\':
						to_append = utf8 [i];
						break;
					case (byte) 'x':
					case (byte) 'X':
						++i;
						if (!ParseHex (utf8, ref i, ref to_append))
							throw ErrorHelper.CreateError (1302, Errors.MT1302_A, input, line);

						break;
					default:
						if (IsDigit (utf8 [i])) {
							string error_msg = null;
							if (!ParseOctal (utf8, ref i, ref to_append, ref error_msg))
								throw ErrorHelper.CreateError (1302, Errors.MT1302_B, input, line, error_msg);
						} else
							to_append = utf8 [i]; // "\K" is the same as "K"
						break;
					}
				} else {
					to_append = utf8 [i];
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

		string FixFile (string s, int line)
		{
			var m = Regex.Match (s, "^.file\\s*(\\d+)\\s*\"(.*)\"$");
			if (!m.Success)
				return s;
			var dbg_line = m.Groups [1].Value;
			var str = m.Groups [2].Value;

			var res = new StringBuilder (str.Length * 3);
			res.Append (".file " + dbg_line + " " + "\\22");

			for (int i = 0; i < str.Length; ++i) {
				if (str [i] == '\\' && i + 1 < str.Length && str [i + 1] == '\\') {
					i++;
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
