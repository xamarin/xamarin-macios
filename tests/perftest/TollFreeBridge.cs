using System;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using BenchmarkDotNet.Attributes;

using Bindings.Test;

namespace PerfTest {

	public class TollFreeString {

		static readonly NSString empty = new NSString ("");
		static readonly NSString short_7bits = new NSString ("description");
		static readonly NSString short_accent = new NSString ("Sébastien");
		static readonly NSString short_unicode = new NSString ("ЉЩщӃ");
		static readonly NSString short_emoji = new NSString ("I'm 😃 it works!");
		static readonly NSString long_string = new NSString (new string ('!', 4096));

		string s;

		public IEnumerable<object[]> Handles ()
		{
			yield return new object [] { "empty", empty.Handle };
			yield return new object [] { "short_7bits", short_7bits.Handle };
			yield return new object [] { "short_accent", short_accent.Handle };
			yield return new object [] { "short_unicode", short_unicode.Handle };
			yield return new object [] { "short_emoji", short_emoji.Handle };
			yield return new object [] { "long_string", long_string.Handle };
		}

		/*
		 * Measure time required to get a string - using NSString (ObjC) selector-based API
		 */

		[Benchmark]
		[ArgumentsSource (nameof (Handles))]
		public string NSString_FromString (string name, IntPtr value)
		{
			var d = NSString.FromHandle (value);
			return s = d;
		}

		/*
		 * Measure time required to get a string - using CFString (C) p/invoke-based API
		 */

		[Benchmark]
		[ArgumentsSource (nameof (Handles))]
		public string CFString_FromString (string name, IntPtr value)
		{
			var d = CFString.FromHandle (value);
			return s = d;
		}

		public IEnumerable<object[]> Strings ()
		{
			yield return new object [] { "empty", "" };
			yield return new object [] { "short_7bits", "Bonjour" };
			yield return new object [] { "short_accent", "Québec" };
			yield return new object [] { "short_unicode", "汉语 漢語" };
			yield return new object [] { "short_emoji", "I'm feeling 🤪 tonight." };
			yield return new object [] { "long_string", new string ('?', 4096) };
		}

		/*
		 * Measure time required to create and release a native string - using NSString (ObjC) selector-based API
		 */

		[Benchmark]
		[ArgumentsSource (nameof (Strings))]
		public void NSString_CreateRelease (string name, string value)
		{
			var p = NSString.CreateNative (value);
			NSString.ReleaseNative (p);
		}

		/*
		 * Measure time required to create and release a native string - using CFString (C) p/invoke-based API
		 */

		[Benchmark]
		[ArgumentsSource (nameof (Strings))]
		public void CFString_CreateRelease (string name, string value)
		{
			var p = CFString.CreateNative (value);
			CFString.ReleaseNative (p);
		}
    }
}
