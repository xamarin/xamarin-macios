using System;
using System.Collections.Generic;
using System.IO;

namespace Xamarin.Tests
{
	public static class MachO
	{
		public static List<string> GetArchitectures (string file)
		{
			var result = new List<string> ();

			using (var fs = File.OpenRead (file)) {
				using (var reader = new BinaryReader (fs)) {
					int magic = reader.ReadInt32 ();
					switch ((uint) magic) {
					case 0xCAFEBABE: // little-endian fat binary
						throw new NotImplementedException ("little endian fat binary");
					case 0xBEBAFECA:
						int architectures = System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ());
						for (int i = 0; i < architectures; i++) {
							result.Add (GetArch (System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ()), System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ())));
							// skip to next entry
							reader.ReadInt32 (); // offset
							reader.ReadInt32 (); // size
							reader.ReadInt32 (); // align
						}
						break;
					case 0xFEEDFACE: // little-endian mach-o header
					case 0xFEEDFACF: // little-endian 64-big mach-o header
						result.Add (GetArch (reader.ReadInt32 (), reader.ReadInt32 ()));
						break;
					case 0xCFFAEDFE:
					case 0xCEFAEDFE:
						result.Add (GetArch (System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ()), System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ())));
						break;
					default:
						throw new Exception (string.Format ("File '{0}' is neither a Universal binary nor a Mach-O binary (magic: 0x{1})", file, magic.ToString ("x")));
					}
				}
			}

			return result;
		}

		static string GetArch (int cputype, int cpusubtype)
		{
			const int ABI64 = 0x01000000;
			const int X86 = 7;
			const int ARM = 12;

			switch (cputype) {
			case ARM : // arm
				switch (cpusubtype) {
				case 6: return "armv6";
				case 9: return "armv7";
				case 11: return "armv7s";
				default:
					return "unknown arm variation: " + cpusubtype.ToString ();
				}
			case ARM  | ABI64:
				switch (cpusubtype) {
				case 0:
					return "arm64";
				default:
					return "unknown arm/64 variation: " + cpusubtype.ToString ();
				}
			case X86: // x86
				return "i386";
			case X86 | ABI64: // x64
				return "x86_64";
			}

			return string.Format ("unknown: {0}/{1}", cputype, cpusubtype);
		}
	}
}
