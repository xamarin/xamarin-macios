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
		static readonly NSString stackalloc_limit = new NSString (new string ('@', 128));
		static readonly NSString allochglobal = new NSString (new string ('#', 129));
		static readonly NSString long_string = new NSString (new string ('!', 4096));

		string s;

		public IEnumerable<object []> Handles ()
		{
			yield return new object [] { "nil", IntPtr.Zero };
			yield return new object [] { "empty", empty.Handle };
			yield return new object [] { "short_7bits", short_7bits.Handle };
			yield return new object [] { "short_accent", short_accent.Handle };
			yield return new object [] { "short_unicode", short_unicode.Handle };
			yield return new object [] { "short_emoji", short_emoji.Handle };
			yield return new object [] { "stackalloc_limit", stackalloc_limit.Handle };
			yield return new object [] { "allochglobal", allochglobal.Handle };
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

		public IEnumerable<object []> Strings ()
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

		public IEnumerable<object []> ArraysOfStrings ()
		{
			yield return new object [] { "null", null };
			yield return new object [] { "empty", new NSArray () };
			yield return new object [] { "one", NSArray.FromStrings ("1") };
			yield return new object [] { "few", NSArray.FromStrings ("Bonjour", "Québec", "汉语 漢語", "I'm feeling 🤪 tonight.") };
			yield return new object [] { "small_mutable", new NSMutableArray<NSString> (new NSString ("Québec"), new NSString ("汉语 漢語")) };
			var lot = new NSMutableArray ();
			for (int i = 0; i < 255; i++) // used to fit under the stackalloc limit of the new implementation
				lot.Add (new NSString (new string ('!', i)));
			yield return new object [] { "lot_mutable", lot };
			var large = new NSMutableArray ();
			for (int i = 0; i < 4096; i++)
				large.Add (new NSString (new string ('#', i)));
			yield return new object [] { "large_mutable", large };
		}

		/*
		 * Measure time required to create a managed `string[]` array from a native one using `CFArray.StringArrayFromHandle`
		 */
		[Benchmark]
		[ArgumentsSource (nameof (ArraysOfStrings))]
		public void CFArray_StringArrayFromHandle (string name, NSArray value)
		{
			CFArray.StringArrayFromHandle (value.GetHandle ());
		}

		/*
		 * Measure time required to create a managed `string[]` array from a native one using `CFArray.StringArrayFromHandle`
		 */
		[Benchmark]
		[ArgumentsSource (nameof (ArraysOfStrings))]
		public void NSArray_StringArrayFromHandle (string name, NSArray value)
		{
			NSArray.StringArrayFromHandle (value.GetHandle ());
		}

		/*
		 * Measure time required to create a managed `NSObject[]` array from a native one using `CFArray.ArrayFromHandle`
		 */
		[Benchmark]
		[ArgumentsSource (nameof (ArraysOfStrings))]
		public void CFArray_ArrayFromHandle (string name, NSArray value)
		{
			CFArray.ArrayFromHandle<NSString> (value.GetHandle ());
		}

		/*
		 * Measure time required to create a managed `NSObject[]` array from a native one using `CFArray.ArrayFromHandle`
		 */
		[Benchmark]
		[ArgumentsSource (nameof (ArraysOfStrings))]
		public void NSArray_ArrayFromHandle (string name, NSArray value)
		{
			NSArray.ArrayFromHandle<NSString> (value.GetHandle ());
		}
	}
}
