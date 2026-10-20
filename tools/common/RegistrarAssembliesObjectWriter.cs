// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Utils;

#nullable enable

namespace Xamarin {
	internal static class RegistrarAssembliesObjectWriter {
		const uint SectionSize = 80;
		const uint SegmentCommandSize = 72 + 2 * SectionSize;
		const uint LoadCommandsSize = SegmentCommandSize + 24 + 24;
		const uint DataOffset = 32 + LoadCommandsSize;
		const uint SubsectionsViaSymbols = 0x2000;
		static readonly byte [] symbolName = Encoding.ASCII.GetBytes ("\0___xamarin_registration_assemblies\0");

		public static byte [] Create (IReadOnlyList<(string Name, Guid Mvid)> assemblies, Abi abi, ApplePlatform platform, bool simulator, Version minimumVersion, Version sdkVersion)
		{
			if (assemblies is null)
				throw new ArgumentNullException (nameof (assemblies));
			if (assemblies.Count == 0)
				throw new ArgumentException ("At least one assembly is required.", nameof (assemblies));
			if (minimumVersion is null)
				throw new ArgumentNullException (nameof (minimumVersion));
			if (sdkVersion is null)
				throw new ArgumentNullException (nameof (sdkVersion));

			uint cpuType;
			uint cpuSubtype;
			switch (abi & Abi.ArchMask) {
			case Abi.ARM64:
				cpuType = 0x0100000c;
				cpuSubtype = 0;
				break;
			case Abi.x86_64:
				cpuType = 0x01000007;
				cpuSubtype = 3;
				break;
			default:
				throw new NotSupportedException ($"Cannot emit a registrar object for architecture {abi}.");
			}

			uint buildPlatform;
			switch (platform) {
			case ApplePlatform.MacOSX:
				buildPlatform = (uint) MachO.Platform.MacOS;
				break;
			case ApplePlatform.iOS:
				buildPlatform = (uint) (simulator ? MachO.Platform.IOSSimulator : MachO.Platform.IOS);
				break;
			case ApplePlatform.TVOS:
				buildPlatform = (uint) (simulator ? MachO.Platform.TvOSSimulator : MachO.Platform.TvOS);
				break;
			case ApplePlatform.MacCatalyst:
				buildPlatform = (uint) MachO.Platform.MacCatalyst;
				break;
			default:
				throw new NotSupportedException ($"Cannot emit a registrar object for platform {platform}.");
			}

			using var strings = new MemoryStream ();
			var stringOffsets = new List<uint> (checked(assemblies.Count * 2));
			foreach (var (name, mvid) in assemblies) {
				if (name is null || name.IndexOf ('\0') >= 0)
					throw new ArgumentException ("Assembly names cannot be null or contain a NUL character.", nameof (assemblies));
				WriteCString (strings, stringOffsets, name);
				WriteCString (strings, stringOffsets, mvid.ToString ());
			}

			var stringBytes = strings.ToArray ();
			var stringSize = checked((uint) stringBytes.Length);
			var tableAddress = checked((stringSize + 7) & ~7u);
			var pointerCount = checked((uint) stringOffsets.Count);
			var tableSize = checked(pointerCount * 8);
			var segmentSize = checked(tableAddress + tableSize);
			var relocationOffset = checked(DataOffset + segmentSize);
			var symbolOffset = checked(relocationOffset + tableSize);
			var stringTableOffset = checked(symbolOffset + 16);
			var stringTableSize = checked(((uint) symbolName.Length + 3) & ~3u);

			using var output = new MemoryStream ();
			using var writer = new BinaryWriter (output);
			writer.Write (MachO.MH_MAGIC_64);
			writer.Write (cpuType);
			writer.Write (cpuSubtype);
			writer.Write (MachO.MH_OBJECT);
			writer.Write (3u);
			writer.Write (LoadCommandsSize);
			writer.Write (SubsectionsViaSymbols);
			writer.Write (0u);

			writer.Write ((uint) MachO.LoadCommands.Segment64);
			writer.Write (SegmentCommandSize);
			WriteName (writer, "");
			writer.Write (0ul);
			writer.Write ((ulong) segmentSize);
			writer.Write ((ulong) DataOffset);
			writer.Write ((ulong) segmentSize);
			writer.Write (7u);
			writer.Write (7u);
			writer.Write (2u);
			writer.Write (0u);
			WriteSection (writer, "__cstring", "__TEXT", 0, stringSize, DataOffset, 0, 0, 0, 2);
			WriteSection (writer, "__const", "__DATA", tableAddress, tableSize, DataOffset + tableAddress, 3, relocationOffset, pointerCount, 0);

			writer.Write ((uint) MachO.LoadCommands.BuildVersion);
			writer.Write (24u);
			writer.Write (buildPlatform);
			writer.Write (EncodeVersion (minimumVersion));
			writer.Write (EncodeVersion (sdkVersion));
			writer.Write (0u);

			writer.Write ((uint) MachO.LoadCommands.Symtab);
			writer.Write (24u);
			writer.Write (symbolOffset);
			writer.Write (1u);
			writer.Write (stringTableOffset);
			writer.Write (stringTableSize);

			if (output.Position != DataOffset)
				throw new InvalidOperationException ("Invalid registrar Mach-O load command size.");

			writer.Write (stringBytes);
			while (output.Position < DataOffset + tableAddress)
				writer.Write ((byte) 0);
			foreach (var offset in stringOffsets)
				writer.Write ((ulong) offset);

			for (var i = stringOffsets.Count - 1; i >= 0; i--) {
				writer.Write (checked(i * 8));
				writer.Write (1u | (3u << 25)); // Local 64-bit relocation against __cstring (section 1).
			}

			if (output.Position != symbolOffset)
				throw new InvalidOperationException ("Invalid registrar Mach-O relocation size.");

			writer.Write (1u);
			writer.Write ((byte) 0x0f);
			writer.Write ((byte) 2);
			writer.Write ((ushort) 0);
			writer.Write ((ulong) tableAddress);
			writer.Write (symbolName);
			while (output.Position < stringTableOffset + stringTableSize)
				writer.Write ((byte) 0);

			return output.ToArray ();
		}

		static void WriteCString (MemoryStream stream, List<uint> offsets, string value)
		{
			offsets.Add (checked((uint) stream.Position));
			var bytes = Encoding.UTF8.GetBytes (value);
			stream.Write (bytes, 0, bytes.Length);
			stream.WriteByte (0);
		}

		static void WriteSection (BinaryWriter writer, string name, string segment, ulong address, ulong size, uint offset, uint alignment, uint relocationOffset, uint relocations, uint flags)
		{
			WriteName (writer, name);
			WriteName (writer, segment);
			writer.Write (address);
			writer.Write (size);
			writer.Write (offset);
			writer.Write (alignment);
			writer.Write (relocationOffset);
			writer.Write (relocations);
			writer.Write (flags);
			writer.Write (0u);
			writer.Write (0u);
			writer.Write (0u);
		}

		static void WriteName (BinaryWriter writer, string name)
		{
			var bytes = Encoding.ASCII.GetBytes (name);
			if (bytes.Length > 16)
				throw new ArgumentException ($"Mach-O name is too long: {name}", nameof (name));
			writer.Write (bytes);
			writer.Write (new byte [16 - bytes.Length]);
		}

		static uint EncodeVersion (Version version)
		{
			if (version.Major < 0 || version.Major > ushort.MaxValue || version.Minor < 0 || version.Minor > byte.MaxValue || version.Build > byte.MaxValue || version.Revision > 0)
				throw new ArgumentOutOfRangeException (nameof (version), version, "Mach-O build versions must have at most three components (major.minor.patch).");
			return ((uint) version.Major << 16) | ((uint) version.Minor << 8) | (uint) Math.Max (version.Build, 0);
		}
	}
}
