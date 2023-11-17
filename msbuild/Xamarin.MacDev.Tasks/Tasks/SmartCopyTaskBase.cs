using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class SmartCopyTaskBase : XamarinTask {
		readonly List<ITaskItem> copied = new List<ITaskItem> ();

		#region Inputs

		public ITaskItem [] DestinationFiles { get; set; }

		public ITaskItem DestinationFolder { get; set; }

		[Required]
		public ITaskItem [] SourceFiles { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] CopiedFiles { get; set; }

		#endregion

		static bool FileChanged (string source, string target)
		{
			if (!File.Exists (target))
				return true;

			var sourceInfo = new FileInfo (source);
			var targetInfo = new FileInfo (target);

			return sourceInfo.Length != targetInfo.Length || File.GetLastWriteTimeUtc (source) > File.GetLastWriteTimeUtc (target);
		}

		void EnsureDirectoryExists (string path)
		{
			if (Directory.Exists (path))
				return;

			if (File.Exists (path))
				File.Delete (path);

			Log.LogMessage (MSBStrings.M0164, path);

			Directory.CreateDirectory (path);
		}

		void CopyFile (string source, string target, string targetItemSpec)
		{
			var dirName = Path.GetDirectoryName (target);

			EnsureDirectoryExists (dirName);

			Log.LogMessage (MessageImportance.Normal, MSBStrings.M0165, source, target);

			File.Copy (source, target, true);
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				if (stat (target, out var stat_rv) == 0) {
					// ensure it's world read-able or this might trigger an appstore rejection
					chmod (target, stat_rv.st_mode | FilePermissions.S_IROTH);
				}
			}

			copied.Add (new TaskItem (targetItemSpec));
		}

		public override bool Execute ()
		{
			if (DestinationFiles is not null && DestinationFolder is not null) {
				Log.LogError (MSBStrings.E0166);
				return false;
			}

			try {
				if (DestinationFiles is not null) {
					if (DestinationFiles.Length != SourceFiles.Length) {
						Log.LogError (MSBStrings.E0167);
						return false;
					}

					for (int i = 0; i < SourceFiles.Length; i++) {
						var target = DestinationFiles [i].GetMetadata ("FullPath");
						var source = SourceFiles [i].GetMetadata ("FullPath");
						var targetDir = Path.GetDirectoryName (target);

						EnsureDirectoryExists (targetDir);

						if (FileChanged (source, target))
							CopyFile (source, target, DestinationFiles [i].ItemSpec);
					}
				} else if (DestinationFolder is not null) {
					var destinationFolder = DestinationFolder.GetMetadata ("FullPath");

					EnsureDirectoryExists (destinationFolder);

					foreach (var item in SourceFiles) {
						var target = Path.Combine (destinationFolder, Path.GetFileName (item.ItemSpec));
						var source = item.GetMetadata ("FullPath");

						if (FileChanged (source, target))
							CopyFile (source, target, Path.Combine (DestinationFolder.ItemSpec, Path.GetFileName (item.ItemSpec)));
					}
				} else {
					Log.LogError (MSBStrings.E0166);
					return false;
				}
			} catch (Exception ex) {
				Log.LogError (ex.ToString ());
			}

			CopiedFiles = copied.ToArray ();

			return !Log.HasLoggedErrors;
		}

		[DllImport ("/usr/lib/libc.dylib", EntryPoint = "stat$INODE64")]
		static extern int stat_x64 (string file_name, out Stat buf);

		[DllImport ("/usr/lib/libc.dylib", EntryPoint = "stat")]
		static extern int stat_arm64 (string file_name, out Stat buf);

		static int stat (string path, out Stat buf)
		{
			if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) {
				return stat_arm64 (path, out buf);
			} else {
				return stat_x64 (path, out buf);
			}
		}

		[DllImport ("libc")]
		static extern int chmod (string path, FilePermissions mode);

		/* when _DARWIN_FEATURE_64_BIT_INODE is defined */
		struct Stat {
			public uint st_dev;
			public FilePermissions st_mode;
			public ushort st_nlink;
			public ulong st_ino;
			public uint st_uid;
			public uint st_gid;
			public uint st_rdev;
			public timespec st_atimespec;
			public timespec st_mtimespec;
			public timespec st_ctimespec;
			public timespec st_birthtimespec;
			public ulong st_size;
			public ulong st_blocks;
			public uint st_blksize;
			public uint st_flags;
			public uint st_gen;
			public uint st_lspare;
			public ulong st_qspare_1;
			public ulong st_qspare_2;
		}

		struct timespec {
			public IntPtr tv_sec;
			public IntPtr tv_nsec;
		}

		[Flags]
		public enum FilePermissions : ushort {
			ACCESSPERMS = 0x1FF,
			ALLPERMS = 0xFFF,
			DEFFILEMODE = 0x1B6,
			S_IFBLK = 0x6000,
			S_IFCHR = 0x2000,
			S_IFDIR = 0x4000,
			S_IFIFO = 0x1000,
			S_IFLNK = 0xA000,
			S_IFMT = 0xF000,
			S_IFREG = 0x8000,
			S_IFSOCK = 0xC000,
			S_IRGRP = 0x20,
			S_IROTH = 0x4,
			S_IRUSR = 0x100,
			S_IRWXG = 0x38,
			S_IRWXO = 0x7,
			S_IRWXU = 0x1C0,
			S_ISGID = 0x400,
			S_ISUID = 0x800,
			S_ISVTX = 0x200,
			S_IWGRP = 0x10,
			S_IWOTH = 0x2,
			S_IWUSR = 0x80,
			S_IXGRP = 0x8,
			S_IXOTH = 0x1,
			S_IXUSR = 0x40
		}

	}
}
