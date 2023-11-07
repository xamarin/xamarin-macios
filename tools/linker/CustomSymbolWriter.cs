using System;
using System.IO;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Xamarin.Linker {
	public sealed class CustomSymbolWriterProvider : ISymbolWriterProvider {
		readonly DefaultSymbolWriterProvider default_symbol_writer_provider = new DefaultSymbolWriterProvider ();

		public ISymbolWriter GetSymbolWriter (ModuleDefinition module, string filename)
			=> new CustomSymbolWriter (default_symbol_writer_provider.GetSymbolWriter (module, filename));

		public ISymbolWriter GetSymbolWriter (ModuleDefinition module, Stream symbolStream)
			=> new CustomSymbolWriter (default_symbol_writer_provider.GetSymbolWriter (module, symbolStream));
	}

	// This is symbol writer wraps another symbol writer, and changes one thing:
	// it writes the filename part of the pdb (as opposed to the full path), in the debug header of the dll.
	// References:
	// https://bugzilla.xamarin.com/show_bug.cgi?id=54578 (Assemblies are duplicated in fat apps when built with .pdb, because pdb paths are different)
	// https://github.com/jbevain/cecil/issues/372 (first attempt at a fix: make cecil write the filename only)
	// https://github.com/jbevain/cecil/pull/554 (first fix attempt broke something else, so it had to be reverted)
	public sealed class CustomSymbolWriter : ISymbolWriter {
		readonly ISymbolWriter _symbolWriter;

		public CustomSymbolWriter (ISymbolWriter symbolWriter)
		{
			_symbolWriter = symbolWriter;
		}

		public ImageDebugHeader GetDebugHeader ()
		{
			var header = _symbolWriter.GetDebugHeader ();
			if (!header.HasEntries)
				return header;

			for (int i = 0; i < header.Entries.Length; i++) {
				header.Entries [i] = ProcessEntry (header.Entries [i]);
			}

			return header;
		}

		static ImageDebugHeaderEntry ProcessEntry (ImageDebugHeaderEntry entry)
		{
			if (entry.Directory.Type != ImageDebugType.CodeView)
				return entry;

			var reader = new BinaryReader (new MemoryStream (entry.Data));
			var newDataStream = new MemoryStream ();
			var writer = new BinaryWriter (newDataStream);

			var sig = reader.ReadUInt32 ();
			if (sig != 0x53445352)
				return entry;

			writer.Write (sig); // RSDS
			writer.Write (reader.ReadBytes (16)); // MVID
			writer.Write (reader.ReadUInt32 ()); // Age

			var fullPath = Encoding.UTF8.GetString (reader.ReadBytes (entry.Data.Length - 1 - 24)); // length - ending \0 - RSDS+MVID+AGE

			writer.Write (Encoding.UTF8.GetBytes (Path.GetFileName (fullPath)));
			writer.Write ((byte) 0);

			var newData = newDataStream.ToArray ();

			var directory = entry.Directory;
			directory.SizeOfData = newData.Length;

			return new ImageDebugHeaderEntry (directory, newData);
		}

		public ISymbolReaderProvider GetReaderProvider () => _symbolWriter.GetReaderProvider ();

		public void Write (MethodDebugInformation info) => _symbolWriter.Write (info);
#if NET
		public void Write () => _symbolWriter.Write ();
#endif

		public void Dispose () => _symbolWriter.Dispose ();
	}
}
