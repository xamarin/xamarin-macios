namespace ObjCRuntime
{
	internal enum CpuArchitecture : ulong
	{
		// (cputype << 32) | cpusubtype
		I386     =   (7UL               << 32),
		X86_64   = (( 7UL | 0x01000000) << 32),
		Armv7    =  (12UL               << 32) | 9,
		Armv7s   =  (12UL               << 32) | 11,
		Armv7k   =  (12UL               << 32) | 12,
		Arm64    = ((12UL | 0x01000000) << 32),
		Arm64e   = ((12UL | 0x01000000) << 32) | 0x00000002,
		Arm64_32 = ((12UL | 0x02000000) << 32) | 0x00000001,
	}
}
