using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public static class AssetPackUtils {
		const string Base36Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		const int HashEverythingLimit = 96;

		static unsafe ulong ComputeHash (string tags)
		{
			var value = (ulong) tags.Length;
			char [] buffer;

			if (tags.Length > HashEverythingLimit) {
				buffer = new char [HashEverythingLimit];
				int index = 0;

				while (index < 32) {
					buffer [index] = tags [index];
					index++;
				}

				for (int i = (tags.Length >> 1) - 16; i < 32; i++)
					buffer [index++] = tags [i];

				for (int i = tags.Length - 32; i < tags.Length; i++)
					buffer [index++] = tags [i];
			} else {
				buffer = tags.ToCharArray ();
			}

			fixed (char* bufptr = buffer) {
				ushort* end4 = ((ushort*) bufptr) + (tags.Length & ~3);
				ushort* end = ((ushort*) bufptr) + tags.Length;
				ushort* cur = (ushort*) bufptr;

				while (cur < end4) {
					value = (value * 67503105) + (ulong) (cur [0] * 16974593) + (ulong) (cur [1] * 66049) + (ulong) (cur [2] * 257) + (ulong) cur [3];
					cur += 4;
				}

				while (cur < end) {
					value = (value * 257) + (ulong) *cur;
					cur++;
				}
			}

			return value + (value << (tags.Length & 31));
		}

		static string Base36Encode (ulong value)
		{
			var encoded = new char [13];
			int index = 12;

			do {
				encoded [index--] = Base36Alphabet [(int) (value % 36)];
				value /= 36;
			} while (value != 0);

			while (index >= 0)
				encoded [index--] = '0';

			return new string (encoded, 0, 13);
		}

		public static string [] ParseTags (string value)
		{
			if (string.IsNullOrEmpty (value))
				return new string [0];

			return value.Split (new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select (tag => tag.Trim ()).ToArray ();
		}

		public static IList<string> GetResourceTags (ITaskItem item)
		{
			var tags = ParseTags (item.GetMetadata ("ResourceTags"));

			if (tags.Length == 0)
				return null;

			Array.Sort (tags, StringComparer.Ordinal);

			return tags;
		}

		public static string GetAssetPackDirectory (string outputPath, string bundleIdentifier, IList<string> tags, out string hash)
		{
			bool addHash = false;
			string id;

			for (int i = 0; i < tags.Count; i++) {
				if (tags [i].IndexOf ('+') != -1) {
					addHash = true;
					break;
				}
			}

			hash = Base36Encode (ComputeHash (string.Join (" ", tags)));

			id = string.Join ("+", tags);

			if (addHash)
				id += "+" + hash;

			return Path.Combine (outputPath, "OnDemandResources", bundleIdentifier + "." + id + ".assetpack");
		}
	}
}
