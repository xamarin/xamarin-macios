using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mono.Unix;

namespace Xamarin.MacDev
{
	public static class PathUtils
	{
		static bool IsSeparator (char c)
		{
			return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar || c == Path.VolumeSeparatorChar;
		}

		static char ToOrdinalIgnoreCase (char c)
		{
			return (((uint) c - 'a') <= ((uint) 'z' - 'a')) ? (char) (c - 0x20) : c;
		}

		[DllImport ("/usr/lib/libc.dylib")]
		static extern IntPtr realpath (string path, IntPtr buffer);

		static string ResolveFullPath (string path)
		{
			if (Path.DirectorySeparatorChar == '\\')
				return Path.GetFullPath (path);

			const int PATHMAX = 4096 + 1;
			IntPtr buffer = IntPtr.Zero;

			try {
				buffer = Marshal.AllocHGlobal (PATHMAX);
				var result = realpath (path, buffer);
				return result == IntPtr.Zero ? "" : Marshal.PtrToStringAuto (buffer);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		public static string ResolveSymbolicLink (string path)
		{
			if (string.IsNullOrEmpty (path))
				return path;

			if (Path.DirectorySeparatorChar == '\\')
				return Path.GetFullPath (path);

			try {
				var alreadyVisted = new HashSet<string> ();

				while (true) {
					if (alreadyVisted.Contains (path))
						return string.Empty;

					alreadyVisted.Add (path);

					var linkInfo = new UnixSymbolicLinkInfo (path);
					if (linkInfo.IsSymbolicLink && linkInfo.HasContents) {
						string contentsPath = linkInfo.ContentsPath;

						if (!Path.IsPathRooted (contentsPath))
							path = Path.Combine (Path.GetDirectoryName (path), contentsPath);
						else
							path = contentsPath;

						path = ResolveFullPath (path);
						continue;
					}

					path = Path.Combine (ResolveSymbolicLink (Path.GetDirectoryName (path)), Path.GetFileName (path));

					return ResolveFullPath (path);
				}
			} catch {
				return path;
			}
		}

		public static string AbsoluteToRelative (string baseDirectory, string absolute)
		{
			if (!Path.IsPathRooted (absolute) || string.IsNullOrEmpty (baseDirectory))
				return absolute;

			// canonicalize the paths
			baseDirectory = Path.GetFullPath (baseDirectory).TrimEnd (Path.DirectorySeparatorChar);
			absolute = Path.GetFullPath (absolute);

			int baseDirectoryStartIndex = baseDirectory.Length;
			int absoluteStartIndex = absolute.Length;
			int separators = 0;
			int index = 0;

			while (index < absolute.Length) {
				if (ToOrdinalIgnoreCase (absolute[index]) != ToOrdinalIgnoreCase (baseDirectory[index]))
					break;

				if (IsSeparator (absolute[index])) {
					baseDirectoryStartIndex = index;
					absoluteStartIndex = index + 1;
					separators++;
				}

				index++;

				if (index >= baseDirectory.Length) {
					if (index >= absolute.Length || IsSeparator (absolute[index])) {
						baseDirectoryStartIndex = index;
						absoluteStartIndex = index + 1;
						separators++;
					}
					break;
				}
			}

			if (separators == 0)
				return absolute;

			if (absoluteStartIndex >= absolute.Length)
				return ".";

			if (index >= absolute.Length && IsSeparator (baseDirectory[index])) {
				absoluteStartIndex = index + 1;
				baseDirectoryStartIndex = index;
			}

			int parentDirCount = 0;
			while (baseDirectoryStartIndex < baseDirectory.Length) {
				if (IsSeparator (baseDirectory[baseDirectoryStartIndex]))
					parentDirCount++;
				baseDirectoryStartIndex++;
			}

			var size = (parentDirCount * 3) + (absolute.Length - absoluteStartIndex);
			var result = new char [size];
			index = 0;

			for (int i = 0; i < parentDirCount; i++) {
				result[index++] = '.';
				result[index++] = '.';
				result[index++] = Path.DirectorySeparatorChar;
			}

			while (absoluteStartIndex < absolute.Length)
				result[index++] = absolute[absoluteStartIndex++];

			return new string (result);
		}

		public static string RelativeToAbsolute (string baseDirectory, string relative)
		{
			return Path.GetFullPath (Path.Combine (baseDirectory, relative));
		}
	}
}
