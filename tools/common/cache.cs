// Copyright 2012 Xamarin Inc. All rights reserved.
//#define DEBUG_COMPARE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Utils;
using Xamarin.Bundler;

public class Cache {
#if MMP
	const string NAME = "mmp";
#elif MTOUCH
	const string NAME = "mtouch";
#else
	#error Wrong defines
#endif

	string cache_dir;
	bool temporary_cache;
	string[] arguments;

	public Cache (string[] arguments)
	{
		this.arguments = arguments;
	}

	public bool IsCacheTemporary {
		get { return temporary_cache; }
	}
	
	// see --cache=DIR
	public string Location {
		get {
			if (cache_dir == null) {
				do {
					cache_dir = Path.Combine (Path.GetTempPath (), NAME + ".cache", Path.GetRandomFileName ());
					if (File.Exists (cache_dir) || Directory.Exists (cache_dir))
						continue;
					Directory.CreateDirectory (cache_dir);
					break;
				} while (true);

				cache_dir = Target.GetRealPath (cache_dir);

				temporary_cache = true;
				if (!Directory.Exists (cache_dir))
					Directory.CreateDirectory (cache_dir);
#if DEBUG
				Console.WriteLine ("Cache defaults to {0}", cache_dir);
#endif
			}
			return cache_dir;
		}
		set {
			cache_dir = value;
			if (!Directory.Exists (cache_dir))
				Directory.CreateDirectory (cache_dir);
			cache_dir = Target.GetRealPath (Path.GetFullPath (cache_dir));
		}
	}
	
	public void Clean ()
	{
#if DEBUG
		Console.WriteLine ("Cache.Clean: {0}" , Location);
#endif
		Directory.Delete (Location, true);
		Directory.CreateDirectory (Location);
	}

	public static bool CompareDirectories (string a, string b, bool ignore_cache = false)
	{
		if (Driver.Force && !ignore_cache) {
			Driver.Log (6, "Directories {0} and {1} are considered different because -f was passed to " + NAME + ".", a, b);
			return false;
		}

		var diff = new StringBuilder ();
		if (Driver.RunCommand ("diff", $"-ur {StringUtils.Quote (a)} {StringUtils.Quote (b)}", output: diff, suppressPrintOnErrors: true) != 0) {
			Driver.Log (1, "Directories {0} and {1} are considered different because diff said so:\n{2}", a, b, diff);
			return false;
		}

		return true;
	}

	public static bool CompareFiles (string a, string b, bool ignore_cache = false)
	{
		if (Driver.Force && !ignore_cache) {
			Driver.Log (6, "Files {0} and {1} are considered different because -f was passed to " + NAME + ".", a, b);
			return false;
		}

		return FileCopier.CompareFiles (a, b, ignore_cache);
	}

	public static bool CompareAssemblies (string a, string b, bool ignore_cache = false, bool compare_guids = false)
	{
		if (Driver.Force && !ignore_cache) {
			Driver.Log (6, "Assemblies {0} and {1} are considered different because -f was passed to " + NAME + ".", a, b);
			return false;
		}

		if (!File.Exists (b)) {
			Driver.Log (6, "Assemblies {0} and {1} are considered different because the latter doesn't exist.", a, b);
			return false;
		}

		using (var astream = new AssemblyReader (a) { CompareGUIDs = compare_guids }) {
			using (var bstream = new AssemblyReader (b) { CompareGUIDs = compare_guids }) {
				bool rv;
				Driver.Log (6, "Comparing assemblies {0} and {1}...", a, b);
				rv = CompareStreams (astream, bstream, ignore_cache);
				Driver.Log (6, " > {0}", rv ? "Identical" : "Different");
				return rv;
			}
		}
	}

	public unsafe static bool CompareStreams (Stream astream, Stream bstream, bool ignore_cache = false)
	{
		if (Driver.Force && !ignore_cache) {
			Driver.Log (6, " > streams are considered different because -f was passed to " + NAME + ".");
			return false;
		}
		
		return FileCopier.CompareStreams (astream, bstream, ignore_cache);
	}
	
	string GetArgumentsForCacheData ()
	{
		var sb = new StringBuilder ();
		var args = new List<string> (arguments);

		sb.Append ("# Version: ").Append (Constants.Version).Append ('.').Append (Constants.Revision).AppendLine ();
		if (args.Count > 0)
			sb.Append ("# [first argument, ignore] # ").AppendLine (args [0]);
		sb.Append (Driver.GetFullPath ()).AppendLine (" \\");
		CollectArgumentsForCache (args, 1, sb);
		return sb.ToString ();
	}

	void CollectArgumentsForCache (IList<string> args, int firstArgument, StringBuilder sb)
	{
		for (int i = firstArgument; i < args.Count; i++) {
			var arg = args [i];
			switch (arg) {
			// Remove arguments that don't affect the cache status.
			case "":
			case "/v":
			case "-v":
			case "--v":
			case "/f":
			case "-f":
			case "--f":
			case "/time":
			case "-time":
			case "--time":
				break;
			default:
				if (arg [0] == '@')
					CollectArgumentsForCache (File.ReadAllLines (arg.Substring (1)), 0, sb);
				
				sb.Append ('\t').Append (StringUtils.Quote (arg)).AppendLine (" \\");
				break;
			}
		}
	}

	public bool IsCacheValid ()
	{
		var name = "arguments";
		var pcache = Path.Combine (Location, name);

		if (!File.Exists (pcache)) {
			Driver.Log (3, "A full rebuild will be performed because the cache is either incomplete or entirely missing.");
			return false;
		} else if (GetArgumentsForCacheData () != File.ReadAllText (pcache)) {
			Driver.Log (3, "A full rebuild will be performed because the arguments to " + NAME + " has changed with regards to the cached data.");
			return false;
		}

		// Check if mtouch/mmp has been modified.
		var executable = System.Reflection.Assembly.GetExecutingAssembly ().Location;
		if (!Application.IsUptodate (executable, pcache)) {
			Driver.Log (3, "A full rebuild will be performed because " + NAME + " has been modified.");
			return false;
		}

		return true;
	}

	public bool VerifyCache ()
	{
		if (!IsCacheValid ()) {
			Clean ();
			return false;
		}

		return true;
	}

	public void ValidateCache ()
	{
		var name = "arguments";
		var pcache = Path.Combine (Location, name);
		File.WriteAllText (pcache, GetArgumentsForCacheData ());
	}

	// A stream that reads an assembly and skips the header and the GUID table.
	class AssemblyReader : Stream {
		string filename;
		FileStream stream;
		long guid_table_start;
		long guid_table_length;

		public bool CompareGUIDs;

		public AssemblyReader (string filename)
		{
			this.filename = filename;

			// Need to figure out where the #GUID table is so we can ignore it.
			FindGUIDTable ();

			stream = File.OpenRead (filename);
		}
		
		public override int Read (byte[] buffer, int offset, int count)
		{
			// read the header, always the same 136 bytes, followed by a 4 bytes timestamp (which we must ignore)
			// the rest (except the #GUID table) is safe to compare.
			if (stream.Position < 136) {
				// read the first 136 bytes
				int read = stream.Read (buffer, offset, 136 - (int) stream.Position);
				if (stream.Position == 136) {
					// skip the timestamp
					stream.Position += 4;
// 					this prints the timestamp:
//					byte[] buf = new byte[4];
//					stream.Read (buf, 0, 4);
//					int t2 = (buf [3] << 24) + (buf [2] << 16) + (buf [1] << 8) + buf [0];
//					var d = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//					var d2 = d.AddSeconds (t2);
//					Console.WriteLine ("TS of {1}: {0}", d2, filename);
				}
				return read; // don't bother reading more, this makes the implementation easier.
			}

			if (CompareGUIDs)
				return stream.Read (buffer, offset, count);

			if (stream.Position + count < guid_table_start) {
				// entire read before guid table
				return stream.Read (buffer, offset, count);
			} else if (stream.Position >= guid_table_start + guid_table_length) {
				// entire read after guid table
				return stream.Read (buffer, offset, count);
			} else {
				int read = 0;
				// read up intil guid table
				read = stream.Read (buffer, offset, (int) (guid_table_start - stream.Position));
				// skip guid table
				stream.Position += guid_table_length;
				// read after guid table
				if (count - read > 0)
					read += stream.Read (buffer, offset + read, count - read);
				return read;
			}
		}

		void FindGUIDTable ()
		{
			using (var fs = File.OpenRead (filename)) {
				using (var str = new BinaryReader (fs)) {
					str.BaseStream.Position = 0;
					if (str.ReadByte () != 0x4d || str.ReadByte () != 0x5a)
						return; // MZ header

					str.BaseStream.Position = 0x80;
					if (str.ReadByte () != 'P' || str.ReadByte () != 'E' || str.ReadByte () != 0 || str.ReadByte () != 0)
						return; // PE signature ("PE\0\0")

					// Read the PE file header

					if (str.ReadByte () != 0x4c || str.ReadByte () != 0x01)
						return; // PE file header -> Machine (always 0x014c)

					ushort sectionCount = str.ReadUInt16 ();
					str.BaseStream.Position += 12;
					ushort optionalHeaderSize = str.ReadUInt16 ();
					if (optionalHeaderSize < 224)
						return; // optional header is not big enough

					str.BaseStream.Position += 2;

					// Read the optional PE header
					str.BaseStream.Position += 208;
					int cliHeaderRVA = str.ReadInt32 ();
					/*int cliHeaderSize = */str.ReadInt32 ();

					str.BaseStream.Position += 8;

					// Read the sections, looking for the ".text" section.
					int sectionHeaderPosition = (int) str.BaseStream.Position;
					int textSectionPosition = -1;
					uint virtualAddress = uint.MaxValue;
					uint pointerToRawData = 0;
					for (int i = 0; i < sectionCount; i++) {
						str.BaseStream.Position = sectionHeaderPosition + 40 * i;
						if (str.ReadByte () != '.' || str.ReadByte () != 't' || str.ReadByte () != 'e' || str.ReadByte () != 'x' || str.ReadByte () != 't' || str.ReadByte () != 0)
							continue;

						textSectionPosition = sectionHeaderPosition + 40 * i;
						str.BaseStream.Position = textSectionPosition + 12;
						virtualAddress = str.ReadUInt32 ();
						str.BaseStream.Position += 4;
						pointerToRawData = str.ReadUInt32 ();
						break;
					}

					if (virtualAddress == uint.MaxValue)
						return;

					// Now we can calculate the file position of the CLI header
					str.BaseStream.Position = cliHeaderRVA - (virtualAddress - pointerToRawData);
					str.BaseStream.Position += 8;
					uint metadataRVA = str.ReadUInt32 ();
					/*uint metadataSize = */str.ReadUInt32 ();

					// Find and read the metadata header
					uint metadataRootPosition = metadataRVA - (virtualAddress - pointerToRawData); 
					str.BaseStream.Position = metadataRootPosition;
					if (str.ReadByte () != 0x42 || str.ReadByte () != 0x53 || str.ReadByte () != 0x4a || str.ReadByte () != 0x42)
						return; // Invalid magic signature.

					str.BaseStream.Position += 8;
					int dynamicLength = str.ReadInt32 ();
					str.BaseStream.Position += dynamicLength;
					str.BaseStream.Position += 2; // flags
					ushort metadataStreams = str.ReadUInt16 ();
					for (ushort i = 0; i < metadataStreams; i++) {
						uint offset = str.ReadUInt32 ();
						uint size = str.ReadUInt32 ();
						byte[] name = new byte [32];

						for (int k = 0; k < 8; k++) {
							str.Read (name, k * 4, 4);
							if (name [k * 4 + 3] == 0)
								break;
						}

						if (name [0] == '#' && name [1] == 'G' && name [2] == 'U' && name [3] == 'I' && name [4] == 'D' && name [5] == 0) {
							// found the GUID table.
							guid_table_start = metadataRootPosition + offset;
							guid_table_length = size;
							return;
						}
					}
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			stream.Dispose ();
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}

		public override bool CanRead {
			get {
				throw new NotImplementedException ();
			}
		}

		public override bool CanSeek {
			get {
				throw new NotImplementedException ();
			}
		}

		public override bool CanWrite {
			get {
				throw new NotImplementedException ();
			}
		}

		public override long Length {
			get {
				return stream.Length - 140 - guid_table_length;
			}
		}

		public override long Position {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
	}

#if false
	static public void ComputeDependencies (IEnumerable<string> assemblies, MonoTouchResolver resolver)
	{
		// note: Parallel.ForEach (with lock to add on 'digests') turns out (much) slower
		// (linksdk.app with 20 assemblies)
		// likely because it's faster (using commoncrypto) than it seems
		foreach (string a in assemblies) {
			string key = Path.GetFileNameWithoutExtension (a);
			using (Stream fs = File.OpenRead (a)) {
				string digest = ComputeDigest (fs, 140);
				digests.Add (key, digest);
			}
		}
		
		Dictionary<string, HashSet<string>> dependencies = new Dictionary<string, HashSet<string>> ();
		foreach (string a in assemblies) {
			HashSet<string> references;
			AssemblyDefinition ad = resolver.Load (a);
			foreach (AssemblyNameReference ar in ad.MainModule.AssemblyReferences) {
				if (!dependencies.TryGetValue (ar.Name, out references)) {
					references = new HashSet<string> ();
					dependencies.Add (ar.Name, references);
				}
				references.Add (ad.Name.Name);
			}
		}
#if DEBUG
		foreach (var kvp in dependencies) {
			Console.WriteLine ("The following assemblies depends on {0}", kvp.Key);
			foreach (var s in kvp.Value)
				Console.WriteLine ("\t{0}", s);
		}
#endif
		// if a dependency has changed everything that depends on it must be cleaned
		foreach (var kvp in dependencies) {
			string cname = kvp.Key + ".*.cache." + GetDigestForAssembly (kvp.Key) + ".o";
			var files = Directory.GetFiles (Location, cname);
			if (files.Length != 0)
				continue;

			Clean (kvp.Key + "*");
			foreach (var deps in kvp.Value)
				Clean (deps + "*");
		}
	}
#endif
}
