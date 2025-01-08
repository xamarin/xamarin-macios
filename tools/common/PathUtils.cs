using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

namespace Xamarin.Utils {
	public static class PathUtils {
		static bool IsSeparator (char c)
		{
			return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar || c == Path.VolumeSeparatorChar;
		}

		static char ToOrdinalIgnoreCase (char c)
		{
			return (((uint) c - 'a') <= ((uint) 'z' - 'a')) ? (char) (c - 0x20) : c;
		}

#if NET
		[return: NotNullIfNotNull (nameof (path))]
#else
		[return: NotNullIfNotNull ("path")]
#endif
		public static string? EnsureTrailingSlash (this string? path)
		{
			if (path is null)
				return null;

			if (path.Length == 0)
				return Path.DirectorySeparatorChar.ToString ();

			if (path [path.Length - 1] != Path.DirectorySeparatorChar)
				path += Path.DirectorySeparatorChar;

			return path;
		}

		[DllImport ("/usr/lib/libc.dylib")]
		static extern IntPtr realpath (string path, IntPtr buffer);

#if NET
		[return: NotNullIfNotNull (nameof (path))]
#else
		[return: NotNullIfNotNull ("path")]
#endif
		public static string? ResolveSymbolicLinks (string? path)
		{
#if !NET
			if (path is null)
				return null;
#endif

			if (string.IsNullOrEmpty (path))
				return path;

			if (Path.DirectorySeparatorChar == '\\')
				return Path.GetFullPath (path);

			const int PATHMAX = 4096 + 1;
			var buffer = IntPtr.Zero;

			try {
				buffer = Marshal.AllocHGlobal (PATHMAX);
				var result = realpath (path, buffer);
				return result == IntPtr.Zero ? path : Marshal.PtrToStringAuto (buffer)!;
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		/// <summary>This works like <see cref="AbsoluteToRelative" />, except that it works as it executing on Windows on all platforms.</summary>
		/// <param name="absoluteBaseDirectory">The directory the return value should be relative to. Must be an absolute path.</param>
		/// <param name="absolutePath">The path whose relative value should be computed. Must be an absolute path.</param>
		public static string AbsoluteToRelativeWindows (string? absoluteBaseDirectory, string absolutePath)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				// If we're already executing on Windows, we don't need our special implementation.
				return AbsoluteToRelative (absoluteBaseDirectory, absolutePath);
			}

			// If there's no base directory, then return the input path.
			if (string.IsNullOrEmpty (absoluteBaseDirectory))
				return absolutePath;

			if (!IsWindowsPathRooted (absolutePath, out var absoluteDriveLetter, out var absoluteUnrooted)) {
				// We can't make 'absolutePath' an absolute path if it's already not, because we need the current directory for that, which we don't have.
				throw new ArgumentOutOfRangeException (nameof (absolutePath), "Must be an absolute path", absolutePath);
			}

			if (!IsWindowsPathRooted (absoluteBaseDirectory, out var baseDirectoryDriveLetter, out var baseDirectoryUnrooted)) {
				// We can't make 'absoluteBaseDirectory' an absolute path if it's already not, because we need the current directory for that, which we don't have.
				throw new ArgumentOutOfRangeException (nameof (absoluteBaseDirectory), "Must be an absolute path", absoluteBaseDirectory);
			}

			// If the paths are on different drives, return the input path, since there's no relative path between them.
			if (absoluteDriveLetter != baseDirectoryDriveLetter)
				return absolutePath;

			// Convert to mac-style paths, and reuse the existing AbsoluteToRelative implementation
			var macRelative = AbsoluteToRelative (baseDirectoryUnrooted.Replace ('\\', '/'), absoluteUnrooted.Replace ('\\', '/'));
			// Convert the result back to windows-style paths
			return macRelative.Replace ('/', '\\');
		}

		/// <summary>This works like Path.Combine, except that it works as it executing on Windows on all platforms.</summary>
		public static string PathCombineWindows (string pathA, string pathB)
		{
			// If B is a rooted path, return it directly
			if (IsWindowsPathRooted (pathB, out var _, out var _))
				return pathB;

			var path = pathA + "\\" + pathB;
			path = path.Replace ('/', '\\'); // canonicalize (the forward slash is a valid directory separator on Windows)

			// Strip the root, if there is a root
			var isRooted = IsWindowsPathRooted (path, out var rootLetter, out var unrootedPath);

			// Evaluate '..' and '.'
			var segments = new List<string> (unrootedPath.Split ('\\'));
			var idx = 0;
			while (idx < segments.Count) {
				if (segments [idx] == ".") {
					segments.RemoveAt (idx);
					continue;
				}
				if (segments [idx] == "..") {
					if (idx > 0) {
						segments.RemoveAt (idx);
						segments.RemoveAt (idx - 1);
					} else {
						idx++;
					}
					continue;
				}
				idx++;
			}

			var canonicalizedUnrootedPath = string.Join ("\\", segments);
			if (isRooted)
				return rootLetter + "\\" + canonicalizedUnrootedPath;
			return canonicalizedUnrootedPath;
		}

		/// <summary>Checks if <paramref name="path" /> is a rooted Windows path.</summary>
		static bool IsWindowsPathRooted (string? path, [NotNullWhen (true)] out string? root, out string unrootedPath)
		{
			root = null;
			unrootedPath = path ?? string.Empty;

#if NET
			if (string.IsNullOrEmpty (path))
#else
			if (path is null || string.IsNullOrEmpty (path))
#endif
			{
				return false;
			}

			if (path [0] == Path.DirectorySeparatorChar || path [0] == Path.AltDirectorySeparatorChar) {
				root = path [0].ToString ();
				unrootedPath = path.Substring (1);
				return true;
			}

			if (path.Length < 2)
				return false;

			if (IsValidWindowsDriveLetter (path [0]) && path [1] == ':') {
				root = path.Substring (0, 2);
				unrootedPath = path.Substring (2);
				return true;
			}

			return false;
		}

		static bool IsValidWindowsDriveLetter (char c)
		{
			return (c >= 'a' && c <= 'z') ||
				   (c >= 'A' && c <= 'Z');
		}

		// Path.GetRelative doesn't exist on the net4.7.2 (which we need to target), so implement our own.
		public static string AbsoluteToRelative (string? baseDirectory, string absolute)
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
				if (ToOrdinalIgnoreCase (absolute [index]) != ToOrdinalIgnoreCase (baseDirectory [index]))
					break;

				if (IsSeparator (absolute [index])) {
					baseDirectoryStartIndex = index;
					absoluteStartIndex = index + 1;
					separators++;
				}

				index++;

				if (index >= baseDirectory.Length) {
					if (index >= absolute.Length || IsSeparator (absolute [index])) {
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

			if (index >= absolute.Length && IsSeparator (baseDirectory [index])) {
				absoluteStartIndex = index + 1;
				baseDirectoryStartIndex = index;
			}

			int parentDirCount = 0;
			while (baseDirectoryStartIndex < baseDirectory.Length) {
				if (IsSeparator (baseDirectory [baseDirectoryStartIndex]))
					parentDirCount++;
				baseDirectoryStartIndex++;
			}

			var size = (parentDirCount * 3) + (absolute.Length - absoluteStartIndex);
			var result = new char [size];
			index = 0;

			for (int i = 0; i < parentDirCount; i++) {
				result [index++] = '.';
				result [index++] = '.';
				result [index++] = Path.DirectorySeparatorChar;
			}

			while (absoluteStartIndex < absolute.Length)
				result [index++] = absolute [absoluteStartIndex++];

			return new string (result);
		}

		public static string RelativeToAbsolute (string baseDirectory, string relative)
		{
			return Path.GetFullPath (Path.Combine (baseDirectory, relative));
		}

		[DllImport ("/usr/lib/libSystem.dylib", SetLastError = true)]
		static extern int symlink (string path1, string path2);

		public static bool Symlink (string target, string symlink)
		{
			return PathUtils.symlink (target, symlink) == 0;
		}

		public static void CreateSymlink (string symlink, string target)
		{
			FileDelete (symlink); // Delete any existing symlinks.
			var rv = PathUtils.symlink (target, symlink);
			if (rv != 0)
				throw new Exception (string.Format ("Could not create the symlink '{0}': {1}", symlink, Marshal.GetLastWin32Error ()));
		}

		[DllImport ("/usr/lib/libSystem.dylib", SetLastError = true)]
		static extern int readlink (string path, [Out] byte [] buffer, IntPtr len);

		public static string GetSymlinkTarget (string path)
		{
			byte []? buffer = null;
			int rv;
			do {
				buffer = new byte [(buffer?.Length ?? 0) + 1024];
				rv = readlink (path, buffer, (IntPtr) (buffer.Length - 1));
			} while (rv == buffer.Length - 1);

			if (rv == -1)
				throw new Exception (string.Format ("Could not readlink '{0}': {1}", path, Marshal.GetLastWin32Error ()));

			return Encoding.UTF8.GetString (buffer, 0, rv);
		}

		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern int unlink (string pathname);

		// File.Delete can't always delete symlinks (in particular if the symlink points to a file that doesn't exist).
		public static void FileDelete (string file)
		{
			unlink (file);
			// ignore any errors.
		}

		struct Timespec {
			public IntPtr tv_sec;
			public IntPtr tv_nsec;
		}

		struct Stat { /* when _DARWIN_FEATURE_64_BIT_INODE is defined */
			public uint st_dev;
			public ushort st_mode;
			public ushort st_nlink;
			public ulong st_ino;
			public uint st_uid;
			public uint st_gid;
			public uint st_rdev;
			public Timespec st_atimespec;
			public Timespec st_mtimespec;
			public Timespec st_ctimespec;
			public Timespec st_birthtimespec;
			public ulong st_size;
			public ulong st_blocks;
			public uint st_blksize;
			public uint st_flags;
			public uint st_gen;
			public uint st_lspare;
			public ulong st_qspare_1;
			public ulong st_qspare_2;
		}

		[DllImport ("/usr/lib/libc.dylib", EntryPoint = "lstat$INODE64", SetLastError = true)]
		static extern int lstat_x64 (string file_name, out Stat buf);

		[DllImport ("/usr/lib/libc.dylib", EntryPoint = "lstat", SetLastError = true)]
		static extern int lstat_arm64 (string file_name, out Stat buf);

		static int lstat (string path, out Stat buf)
		{
			if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) {
				return lstat_arm64 (path, out buf);
			} else {
				return lstat_x64 (path, out buf);
			}
		}

		public static bool IsSymlink (string file)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				var attr = File.GetAttributes (file);
				return attr.HasFlag (FileAttributes.ReparsePoint);
			}
			Stat buf;
			var rv = lstat (file, out buf);
			if (rv != 0)
				throw new Exception (string.Format ("Could not lstat '{0}': {1}", file, Marshal.GetLastWin32Error ()));
			const int S_IFLNK = 40960;
			return (buf.st_mode & S_IFLNK) == S_IFLNK;
		}

		public static bool IsSymlinkOrContainsSymlinks (string directoryOrFile)
		{
			if (IsSymlink (directoryOrFile))
				return true;

			if (!Directory.Exists (directoryOrFile))
				return false;

			foreach (var entry in Directory.EnumerateFileSystemEntries (directoryOrFile)) {
				if (IsSymlinkOrContainsSymlinks (entry))
					return true;
			}

			return false;
		}

		// Replace any windows-style slashes with mac-style slashes.
#if NET
		[return: NotNullIfNotNull (nameof (path))]
#else
		[return: NotNullIfNotNull ("path")]
#endif
		public static string? ConvertToMacPath (string? path)
		{
#if !NET
			if (path is null)
				return null;
#endif

			if (string.IsNullOrEmpty (path))
				return path;

			return path.Replace ('\\', '/');
		}

		public static bool OSSupportsLongPaths {
			get {
				if (Environment.OSVersion.Platform != PlatformID.Win32NT)
					return true;

				return IsLongPathsEnabledRegistry;
			}
		}

		const string WINDOWS_FILE_SYSTEM_REGISTRY_KEY = @"SYSTEM\CurrentControlSet\Control\FileSystem";
		const string WINDOWS_LONG_PATHS_ENABLED_VALUE_NAME = "LongPathsEnabled";

		static bool IsLongPathsEnabledRegistry {
			get {
#if NET
				if (!OperatingSystem.IsWindows ())
					return false;
#endif

				using (var fileSystemKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey (WINDOWS_FILE_SYSTEM_REGISTRY_KEY)) {
					if (fileSystemKey is null)
						return false;
					var longPathsEnabledValue = fileSystemKey.GetValue (WINDOWS_LONG_PATHS_ENABLED_VALUE_NAME, 0);
					return Convert.ToInt32 (longPathsEnabledValue) == 1;
				}
			}
		}
	}
}
