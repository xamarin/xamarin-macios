//
// Replacement.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Xamarin.Pmcs
{
	public enum ReplacementContext
	{
		Global,
		EnumBackingType
	}

	public enum ReplacementKind
	{
		Builtin,
		Exact,
		Regex,
		Prefix
	}

	public abstract class ExplicitReplacement : Replacement
	{
		protected abstract string Evaluate (string input);

		public sealed override bool Evaluate (string input, out string replacement)
		{
			replacement = Evaluate (input);
			return replacement != null;
		}
	}

	public class Replacement
	{
		Regex regex;

		public ReplacementContext Context { get; set; }
		public ReplacementKind Kind { get; set; }
		public string Pattern { get; set; }
		public string Replace { get; set; }

		public bool IsValid {
			get { return Pattern != null && Replace != null; }
		}

		public static Replacement Parse (string pattern, string replace)
		{
			if (String.IsNullOrEmpty (pattern))
				throw new ArgumentNullException ("pattern");

			var replacement = new Replacement ();

			if (pattern [0] == '/') {
				replacement.Kind = ReplacementKind.Regex;
				replacement.Pattern = pattern.Substring (1);
			} else if (pattern [0] == '^') {
				replacement.Kind = ReplacementKind.Prefix;
				replacement.Pattern = pattern.Substring (1);
			} else {
				replacement.Kind = ReplacementKind.Exact;
				replacement.Pattern = pattern;
			}

			replacement.Replace = replace;
			return replacement;
		}

		public virtual bool Evaluate (string input, out string replacement)
		{
			replacement = null;

			switch (Kind) {
			case ReplacementKind.Exact:
				if (input == Pattern)
					replacement = Replace;
				break;
			case ReplacementKind.Prefix:
				if (input.StartsWith (Pattern, StringComparison.Ordinal))
					replacement = Replace + input.Substring (Pattern.Length);
				break;
			case ReplacementKind.Regex:
				if (regex == null)
					regex = new Regex (Pattern, RegexOptions.Compiled | RegexOptions.Singleline);
				replacement = regex.Replace (input, Replace);
				break;
			}

			return input != replacement && replacement != null;
		}

		public override string ToString ()
		{
			switch (Kind) {
			case ReplacementKind.Builtin:
				return String.Format ("builtin: <{0}>", GetType ().FullName);
			case ReplacementKind.Exact:
				return String.Format ("exact: {0} => {1}", Pattern, Replace);
			case ReplacementKind.Prefix:
				return String.Format ("prefix: {0} => {1}", Pattern, Replace);
			case ReplacementKind.Regex:
				return String.Format ("regex: {0} => {1}", Pattern, Replace);
			default:
				return String.Format ("unknown: {0} => {1}", Pattern, Replace);
			}
		}
	}

	public class ReplacementCollection : IEnumerable<Replacement>
	{
		public ReplacementContext Context { get; private set; }

		public ReplacementCollection (ReplacementContext context)
		{
			Context = context;
		}

		List<Replacement> list = new List<Replacement> ();

		public void Add (string pattern, string replace)
		{
			Add (Replacement.Parse (pattern, replace));
		}

		public void Add (Replacement replacement)
		{
			replacement.Context = Context;
			list.Add (replacement);
		}

		public void AddRange (IEnumerable<Replacement> replacements)
		{
			foreach (var replacement in replacements)
				Add (replacement);
		}

		public bool Evaluate (string input, out string replacement)
		{
			foreach (var item in list) {
				if (item.Evaluate (input, out replacement))
					return true;
			}

			replacement = null;
			return false;
		}

		public int Count {
			get { return list.Count; }
		}

		public IEnumerator<Replacement> GetEnumerator ()
		{
			foreach (var item in list)
				yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}