# Required Reasons API usage in .NET, Mono and the BCL

The tables provide lists of C# .NET APIs that call the [Required Reasons APIs][RequiredReasonAPI] organized by category. These API usages are present in your app even if you do not explicitly call them. Therefore, you will be required to provide the API categories and reasons provided below in your apps `PrivacyInfo.xcprivacy` file. You may have to provide additional reason codes if you use the APIs directly, see [Required Reasons APIs][RequiredReasonAPI] for more information on the reason codes.

**Note:** The following lists are verified only for .NET versions 8.0.0 and later.

### [File timestamp APIs][FileTimestampAPIs]

The following APIs either directly or indirectly access file timestamps and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryFileTimestamp` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. Refer to [File timestamp APIs][FileTimestampAPIs] for any additional relevant values to add to the `NSPrivacyAccessedAPITypeReasons` array.
| .NET API | Internal Usages | CoreClr Usages | Mono Usages
| - | - | - | - |
| [System.Diagnostics.FileVersionInfo](https://learn.microsoft.com/dotnet/api/System.Diagnostics.FileVersionInfo) | [Interop.Sys.LStat](https://source.dot.net/#System.Private.CoreLib/src/libraries/Common/src/Interop/Unix/System.Native/Interop.Stat.cs,65) | SystemNative_LStat | g_file_test
| [System.IO.Compression.ZipFile.CreateFromDirectory](https://learn.microsoft.com/dotnet/api/System.IO.Compression.ZipFile.CreateFromDirectory) | [Interop.Sys.Stat](https://source.dot.net/#System.Private.CoreLib/src/libraries/Common/src/Interop/Unix/System.Native/Interop.Stat.cs,62) | SystemNative_Stat | mono_file_map_size
| [System.IO.Directory.CreateDirectory(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.CreateDirectory) | [Interop.Sys.FStat](https://source.dot.net/#System.Private.CoreLib/src/libraries/Common/src/Interop/Unix/System.Native/Interop.Stat.cs,59) | SystemNative_FStat | 
| [System.IO.Directory.CreateDirectory(string, UnixFileMode)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.CreateDirectory) | [System.Runtime.Loader.AssemblyLoadContext.ResolveSatelliteAssembly](https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Runtime/Loader/AssemblyLoadContext.cs,763)
| [System.IO.Directory.Delete(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.Delete)
| [System.IO.Directory.Exists(string?)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.Exists)
| [System.IO.Directory.GetCreationTime(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.GetCreationTime)
| [System.IO.Directory.GetCreationTimeUtc(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.GetCreationTimeUtc)
| [System.IO.Directory.GetLastAccessTime(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.GetLastAccessTime)
| [System.IO.Directory.GetLastAccessTimeUtc(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.GetLastAccessTimeUtc)
| [System.IO.Directory.GetLastWriteTime(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.GetLastWriteTime)
| [System.IO.Directory.GetLastWriteTimeUtc(string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.GetLastWriteTimeUtc)
| [System.IO.Directory.Move(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.Directory.Move)
| [System.IO.DirectoryInfo.Delete(string?)](https://learn.microsoft.com/dotnet/api/System.IO.DirectoryInfo.Delete)
| [System.IO.DirectoryInfo.MoveTo(string)](https://learn.microsoft.com/dotnet/api/System.IO.DirectoryInfo.MoveTo)
| [System.IO.Enumeration.FileSystemEntry.Attributes](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.Attributes)
| [System.IO.Enumeration.FileSystemEntry.CreationTime](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.CreationTime)
| [System.IO.Enumeration.FileSystemEntry.CreationTimeUtc](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.CreationTimeUtc)
| [System.IO.Enumeration.FileSystemEntry.IsHidden](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.IsHidden)
| [System.IO.Enumeration.FileSystemEntry.LastAccessTime](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.Attributes)
| [System.IO.Enumeration.FileSystemEntry.LastAccessTimeUtc](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.LastAccessTimeUtc)
| [System.IO.Enumeration.FileSystemEntry.LastWriteTime](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.LastWriteTime)
| [System.IO.Enumeration.FileSystemEntry.LastWriteTimeUtc](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.LastWriteTimeUtc)
| [System.IO.Enumeration.FileSystemEntry.Length](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.Length)
| [System.IO.Enumeration.FileSystemEntry.ToFileSystemInfo()](https://learn.microsoft.com/dotnet/api/System.IO.Enumeration.FileSystemEntry.ToFileSystemInfo)
| [System.IO.File.Copy(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.File.Copy)
| [System.IO.File.Copy(string, string, boolean)](https://learn.microsoft.com/dotnet/api/System.IO.File.Copy)
| [System.IO.File.Delete(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.Delete)
| [System.IO.File.Exists(string?)](https://learn.microsoft.com/dotnet/api/System.IO.File.Exists)
| [System.IO.File.GetAttributes(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetAttributes)
| [System.IO.File.GetAttributes(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetAttributes)
| [System.IO.File.GetCreationTime(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetCreationTime)
| [System.IO.File.GetCreationTime(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetCreationTime)
| [System.IO.File.GetCreationTimeUtc(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetCreationTimeUtc)
| [System.IO.File.GetCreationTimeUtc(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetCreationTimeUtc)
| [System.IO.File.GetLastAccessTime(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastAccessTime)
| [System.IO.File.GetLastAccessTime(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastAccessTime)
| [System.IO.File.GetLastAccessTimeUtc(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastAccessTimeUtc)
| [System.IO.File.GetLastAccessTimeUtc(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastAccessTimeUtc)
| [System.IO.File.GetLastWriteTime(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastWriteTime)
| [System.IO.File.GetLastWriteTime(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastWriteTime)
| [System.IO.File.GetLastWriteTimeUtc(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastWriteTimeUtc)
| [System.IO.File.GetLastWriteTimeUtc(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetLastWriteTimeUtc)
| [System.IO.File.GetUnixFileMode(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetUnixFileMode)
| [System.IO.File.GetUnixFileMode(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.GetUnixFileMode)
| [System.IO.File.Move(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.File.Move)
| [System.IO.File.Move(string, string, boolean)](https://learn.microsoft.com/dotnet/api/System.IO.File.Move)
| [System.IO.File.OpenHandle(string, FileMode, FileAccess, FileShare, FileOptions, long)](https://learn.microsoft.com/dotnet/api/System.IO.File.OpenHandle)
| [System.IO.File.Replace(string, string, string)](https://learn.microsoft.com/dotnet/api/System.IO.File.Replace)
| [System.IO.File.Replace(string, string, string, boolean)](https://learn.microsoft.com/dotnet/api/System.IO.File.Replace)
| [System.IO.File.ReadAllBytes(string)](https://learn.microsoft.com/dotnet/api/System.IO.File.ReadAllBytes)
| [System.IO.File.ReadAllBytesAsync(string, CancellationToken)](https://learn.microsoft.com/dotnet/api/System.IO.File.ReadAllBytesAsync)
| [System.IO.FileInfo.Delete()](https://learn.microsoft.com/dotnet/api/System.IO.FileInfo.Delete)
| [System.IO.FileInfo.MoveTo(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.FileInfo.MoveTo)
| [System.IO.FileInfo.MoveTo(string, string, boolean)](https://learn.microsoft.com/dotnet/api/System.IO.FileInfo.MoveTo)
| [System.IO.FileInfo.Replace(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.FileInfo.Replace)
| [System.IO.FileInfo.Replace(string, string, boolean)](https://learn.microsoft.com/dotnet/api/System.IO.FileInfo.Replace)
| [System.IO.FileSystemInfo.Attributes](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.Attributes)
| [System.IO.FileSystemInfo.CreationTime](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.CreationTime)
| [System.IO.FileSystemInfo.CreationTimeUtc](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.CreationTimeUtc)
| [System.IO.FileSystemInfo.LastAccessTime](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.Attributes)
| [System.IO.FileSystemInfo.LastAccessTimeUtc](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.LastAccessTimeUtc)
| [System.IO.FileSystemInfo.LastWriteTime](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.LastWriteTime)
| [System.IO.FileSystemInfo.LastWriteTimeUtc](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.LastWriteTimeUtc)
| [System.IO.FileSystemInfo.Length](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.Length)
| [System.IO.FileSystemInfo.Refresh()](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.Refresh)
| [System.IO.FileSystemInfo.UnixFileMode](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemInfo.UnixFileMode)
| [System.IO.FileSystemWatcher](https://learn.microsoft.com/dotnet/api/System.IO.FileSystemWatcher)
| [System.IO.IsolatedStorage.IsolatedStorageFile.MoveDirectory(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.IsolatedStorage.IsolatedStorageFile.MoveDirectory)
| [System.IO.IsolatedStorage.IsolatedStorageFile.MoveFile(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.IsolatedStorage.IsolatedStorageFile.MoveFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode, string?)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode, string?, long)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode, string?, long, MemoryMappedFileAccess)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.Path.Exists(string?)](https://learn.microsoft.com/dotnet/api/System.IO.Path.Exists)
| [System.IO.Pipes.AnonymousPipeClientStream](https://learn.microsoft.com/dotnet/api/System.IO.Pipes.AnonymousPipeClientStream)
| [System.IO.Pipes.AnonymousPipeServerStream](https://learn.microsoft.com/dotnet/api/System.IO.Pipes.AnonymousPipeServerStream)
| [System.IO.Pipes.NamedPipeClientStream](https://learn.microsoft.com/dotnet/api/System.IO.Pipes.NamedPipeClientStream)
| [System.IO.Pipes.NamedPipeServerStream](https://learn.microsoft.com/dotnet/api/System.IO.Pipes.NamedPipeServerStream)
| [System.IO.RandomAccess.GetLength(SafeFileHandle)](https://learn.microsoft.com/dotnet/api/System.IO.RandomAccess.GetLength)
| [System.Formats.Tar.TarWriter.WriteEntry(TarEntry)](https://learn.microsoft.com/dotnet/api/System.Formats.Tar.TarWriter.WriteEntry)
| [System.Formats.Tar.TarWriter.WriteEntry(string, string)](https://learn.microsoft.com/dotnet/api/System.Formats.Tar.TarWriter.WriteEntry)
| [System.Formats.Tar.TarWriter.WriteEntryAsync(TarEntry, CancellationToken)](https://learn.microsoft.com/dotnet/api/System.Formats.Tar.TarWriter.WriteEntryAsync)
| [System.Formats.Tar.TarWriter.WriteEntryAsync(string, string, CancellationToken)](https://learn.microsoft.com/dotnet/api/System.Formats.Tar.TarWriter.WriteEntryAsync)
| [System.Net.Sockets.Socket.SendPacketsAsync(SocketAsyncEventArgs)](https://learn.microsoft.com/dotnet/api/System.Net.Sockets.Socket.SendPacketsAsync)
| [System.TimeZoneInfo.Local](https://learn.microsoft.com/dotnet/api/System.TimeZoneInfo.Local) 



For example, if you use any of the APIs listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSPrivacyAccessedAPITypes</key>
    <array>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>C617.1</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Additional reason codes from [File timestamp APIs][FileTimestampAPIs] can be provided in the array following the `NSPrivacyAccessedAPITypeReasons` key.

### [System boot time APIs][SystemBootTimeAPIs]

The following APIs either directly or indirectly access the system boot time and require reasons for use. Use the string `NSPrivacyAccessedAPICategorySystemBootTime` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you only access the system boot time from the list of APIs below, then use the `35F9.1` value in the `NSPrivacyAccessedAPITypeReasons` array.
| .NET API | Internal Usages | CoreClr Usages | Mono Usages
| - | - | - | - |
| [System.Environment.TickCount](https://learn.microsoft.com/dotnet/api/System.Environment.TickCount) | | | mono_msec_boottime
| [System.Environment.TickCount64](https://learn.microsoft.com/dotnet/api/System.Environment.TickCount64) | | | mono_domain_finalize
| | | | mono_join_uninterrupted
| | | | mono_msec_ticks
| | | | mono_100ns_ticks
| | | | threads_wait_pending_joinable_threads
| | | | current_time

For example, if you use any of the APIs listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSPrivacyAccessedAPITypes</key>
    <array>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategorySystemBootTime</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>35F9.1</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

### [Disk space APIs][DiskSpaceAPIs]
 
The following APIs either directly or indirectly access the available disk space and require reasons for use. Use the string `NSPrivacyAccessedAPICategoryDiskSpace` as the value for the `NSPrivacyAccessedAPIType` key in your `NSPrivacyAccessedAPITypes` dictionary. If you access the available disk space from the list of APIs below, then use [Disk space APIs][DiskSpaceAPIs] to determine the correct values to place in the `NSPrivacyAccessedAPITypeReasons` array.
| .NET API | Internal Usages | CoreClr Usages | Mono Usages
| - | - | - | - |
| [System.IO.DriveInfo.AvailableFreeSpace](https://learn.microsoft.com/dotnet/api/System.IO.DriveInfo.AvailableFreeSpace) | [Interop.Sys.TryGetFileSystemType](https://source.dot.net/#System.Private.CoreLib/src/libraries/Common/src/Interop/Unix/System.Native/Interop.UnixFileSystemTypes.cs,155) | SystemNative_GetFileSystemType | |
| [System.IO.DriveInfo.DriveFormat](https://learn.microsoft.com/dotnet/api/System.IO.DriveInfo.DriveFormat) | [Interop.Sys.GetSpaceInfoForMountPoint](https://source.dot.net/#System.IO.FileSystem.DriveInfo/src/libraries/Common/src/Interop/Unix/System.Native/Interop.MountPoints.FormatInfo.cs,34) | SystemNative_GetSpaceInfoForMountPoint | |
| [System.IO.DriveInfo.DriveType](https://learn.microsoft.com/dotnet/api/System.IO.DriveInfo.DriveType) | [Interop.Sys.GetFormatInfoForMountPoint](https://source.dot.net/#System.IO.FileSystem.DriveInfo/src/libraries/Common/src/Interop/Unix/System.Native/Interop.MountPoints.FormatInfo.cs,37) | SystemNative_GetFormatInfoForMountPoint |
| [System.IO.DriveInfo.TotalFreeSpace](https://learn.microsoft.com/dotnet/api/System.IO.DriveInfo.TotalFreeSpace)
| [System.IO.DriveInfo.TotalSize](https://learn.microsoft.com/dotnet/api/System.IO.DriveInfo.TotalSize)
| [System.IO.File.Copy(string, string)](https://learn.microsoft.com/dotnet/api/System.IO.File.Copy) 
| [System.IO.File.Copy(string, string, boolean)](https://learn.microsoft.com/dotnet/api/System.IO.File.Copy)
| [System.IO.File.OpenHandle(string, FileMode, FileAccess, FileShare, FileOptions, long)](https://learn.microsoft.com/dotnet/api/System.IO.File.OpenHandle)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode, string?)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode, string?, long)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(string, FileMode, string?, long, MemoryMappedFileAccess)](https://learn.microsoft.com/dotnet/api/System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile)
| [System.TimeZoneInfo.Local](https://learn.microsoft.com/dotnet/api/System.TimeZoneInfo.Local) 
| [System.Net.Sockets.Socket.SendPacketsAsync(SocketAsyncEventArgs)](https://learn.microsoft.com/dotnet/api/System.Net.Sockets.Socket.SendPacketsAsync)

For example, if you use any of the APIs listed above, your `PrivacyInfo.xcprivacy` would contain the `dict` element in the `NSPrivacyAccessedAPITypes` key's array as shown below:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSPrivacyAccessedAPITypes</key>
    <array>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryDiskSpace</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>E174.1</string>
            </array>
        </dict>
    </array>
</dict>
</plist>
```

Reason codes from [Disk space APIs][DiskSpaceAPIs] can be provided in the array following the `NSPrivacyAccessedAPITypeReasons` key.


[RequiredReasonAPI]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api
[PrivacyManifestFiles]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files
[C#NETAPIs]: #c-net-apis-in-net-maui
[FileTimestampAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278393
[SystemBootTimeAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278394
[DiskSpaceAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278397
[ActiveKeyboardAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278400
[UserDefaultsAPIs]: https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api#4278401
