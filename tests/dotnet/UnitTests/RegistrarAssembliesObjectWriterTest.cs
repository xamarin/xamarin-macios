// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;

using NUnit.Framework;
using Xamarin.Utils;

namespace Xamarin.Tests {
	[TestFixture]
	public class RegistrarAssembliesObjectWriterTest {
		static readonly (string Name, Guid Mvid) [] assemblies = [
			("AppA", Guid.Parse ("11111111-2222-3333-4444-555555555555")),
			("Syst\u00e8me", Guid.Parse ("66666666-7777-8888-9999-aaaaaaaaaaaa")),
		];

		[TestCase (Abi.ARM64, ApplePlatform.iOS, false, 2u, 0x0100000cu, 0u)]
		[TestCase (Abi.ARM64, ApplePlatform.iOS, true, 7u, 0x0100000cu, 0u)]
		[TestCase (Abi.x86_64, ApplePlatform.iOS, true, 7u, 0x01000007u, 3u)]
		[TestCase (Abi.ARM64, ApplePlatform.TVOS, false, 3u, 0x0100000cu, 0u)]
		[TestCase (Abi.ARM64, ApplePlatform.TVOS, true, 8u, 0x0100000cu, 0u)]
		[TestCase (Abi.x86_64, ApplePlatform.MacOSX, false, 1u, 0x01000007u, 3u)]
		[TestCase (Abi.ARM64, ApplePlatform.MacCatalyst, false, 6u, 0x0100000cu, 0u)]
		public void ObjectLayout (Abi abi, ApplePlatform platform, bool simulator, uint buildPlatform, uint cpuType, uint cpuSubtype)
		{
			var bytes = RegistrarAssembliesObjectWriter.Create (assemblies, abi, platform, simulator, new Version (17, 3, 2), new Version (27, 0));
			using var reader = new BinaryReader (new MemoryStream (bytes));

			Assert.That (reader.ReadUInt32 (), Is.EqualTo (MachO.MH_MAGIC_64), "Mach-O magic");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (cpuType), "CPU type");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (cpuSubtype), "CPU subtype");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (MachO.MH_OBJECT), "File type");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (3u), "Load command count");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (280u), "Load command size");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (0x2000u), "Subsections via symbols");

			reader.BaseStream.Position = 32;
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (0x19u), "Segment command");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (232u), "Segment command size");

			reader.BaseStream.Position = 32 + 72;
			Assert.That (ReadName (reader), Is.EqualTo ("__cstring"), "String section");
			Assert.That (ReadName (reader), Is.EqualTo ("__TEXT"), "String segment");
			Assert.That (reader.ReadUInt64 (), Is.Zero, "String address");
			var stringSize = reader.ReadUInt64 ();
			var stringOffset = reader.ReadUInt32 ();
			Assert.That (stringOffset, Is.EqualTo (312u), "String file offset");

			reader.BaseStream.Position = 32 + 72 + 80;
			Assert.That (ReadName (reader), Is.EqualTo ("__const"), "Pointer section");
			Assert.That (ReadName (reader), Is.EqualTo ("__DATA"), "Pointer segment");
			var tableAddress = reader.ReadUInt64 ();
			Assert.That (tableAddress % 8, Is.Zero, "Pointer alignment");
			Assert.That (reader.ReadUInt64 (), Is.EqualTo (32ul), "Pointer table size");
			var tableOffset = reader.ReadUInt32 ();
			Assert.That ((ulong) tableOffset, Is.EqualTo (stringOffset + tableAddress), "Pointer file offset");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (3u), "Pointer section alignment");
			var relocationOffset = reader.ReadUInt32 ();
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (4u), "Relocation count");

			reader.BaseStream.Position = 32 + 232;
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (0x32u), "Build version command");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (24u), "Build version command size");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (buildPlatform), "Target platform");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (0x00110302u), "Minimum OS version");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (0x001b0000u), "SDK version");

			reader.BaseStream.Position = 32 + 232 + 24;
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (2u), "Symbol table command");
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (24u), "Symbol table command size");
			var symbolOffset = reader.ReadUInt32 ();
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (1u), "Symbol count");
			var symbolStringOffset = reader.ReadUInt32 ();

			var texts = new [] { assemblies [0].Name, assemblies [0].Mvid.ToString (), assemblies [1].Name, assemblies [1].Mvid.ToString () };
			var expectedStrings = Encoding.UTF8.GetBytes (string.Join ("\0", texts) + "\0");
			Assert.That (stringSize, Is.EqualTo ((ulong) expectedStrings.Length), "String section size");
			reader.BaseStream.Position = stringOffset;
			Assert.That (reader.ReadBytes (expectedStrings.Length), Is.EqualTo (expectedStrings), "Assembly names and MVIDs");

			reader.BaseStream.Position = tableOffset;
			ulong stringAddress = 0;
			foreach (var text in texts) {
				Assert.That (reader.ReadUInt64 (), Is.EqualTo (stringAddress), $"Pointer to {text}");
				stringAddress += (ulong) Encoding.UTF8.GetByteCount (text) + 1;
			}

			reader.BaseStream.Position = relocationOffset;
			for (var i = texts.Length - 1; i >= 0; i--) {
				Assert.That (reader.ReadInt32 (), Is.EqualTo (i * 8), $"Relocation {i} address");
				Assert.That (reader.ReadUInt32 (), Is.EqualTo (1u | (3u << 25)), $"Relocation {i} target");
			}

			reader.BaseStream.Position = symbolOffset;
			Assert.That (reader.ReadUInt32 (), Is.EqualTo (1u), "Symbol string index");
			Assert.That (reader.ReadByte (), Is.EqualTo (0x0f), "External section symbol");
			Assert.That (reader.ReadByte (), Is.EqualTo (2), "Pointer section index");
			Assert.That (reader.ReadUInt16 (), Is.Zero, "Symbol description");
			Assert.That (reader.ReadUInt64 (), Is.EqualTo (tableAddress), "Symbol address");
			reader.BaseStream.Position = symbolStringOffset;
			Assert.That (reader.ReadByte (), Is.Zero, "Initial string table byte");
			const string symbol = "___xamarin_registration_assemblies";
			Assert.That (Encoding.ASCII.GetString (reader.ReadBytes (symbol.Length + 1)), Is.EqualTo (symbol + "\0"), "Registration symbol");
		}

		[Test]
		public void UnsupportedArchitecture ()
		{
			Assert.Throws<NotSupportedException> (() => RegistrarAssembliesObjectWriter.Create (assemblies, Abi.ARM64e, ApplePlatform.iOS, false, new Version (17, 0), new Version (27, 0)));
		}

		[Test]
		public void UnsupportedVersion ()
		{
			Assert.Throws<ArgumentOutOfRangeException> (() => RegistrarAssembliesObjectWriter.Create (assemblies, Abi.ARM64, ApplePlatform.iOS, false, new Version (17, 0, 256), new Version (27, 0)));
		}

		static string ReadName (BinaryReader reader)
		{
			return Encoding.ASCII.GetString (reader.ReadBytes (16)).TrimEnd ('\0');
		}
	}
}
