using System;
using System.IO;
using System.Runtime.InteropServices;

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

		public static string ResolveSymbolicLinks (string path)
		{
			if (string.IsNullOrEmpty (path))
				return path;

			if (Path.DirectorySeparatorChar == '\\')
				return Path.GetFullPath (path);

			const int PATHMAX = 4096 + 1;
			var buffer = IntPtr.Zero;

			try {
				buffer = Marshal.AllocHGlobal (PATHMAX);
				var result = realpath (path, buffer);
				return result == IntPtr.Zero ? path : Marshal.PtrToStringAuto (buffer);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		public static string AbsoluteToRelative (string baseDirectory, string absolute)
		{
			if (string.IsNullOrEmpty (baseDirectory))
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
