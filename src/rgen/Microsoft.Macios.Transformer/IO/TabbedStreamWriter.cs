// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Generator.IO;

class TabbedStreamWriter : TabbedWriter<StreamWriter> {
	readonly string path;
	TabbedStreamWriter? parent;

	TabbedStreamWriter (string filePath, FileMode mode, FileAccess access, FileShare share,
		int currentCount = 0, bool block = false)
		: base (new StreamWriter (new FileStream (filePath, mode, access, share)), currentCount, block)
	{
		// keep track of the path we are writing to
		path = filePath;
	}

	public TabbedStreamWriter (string filePath, int currentCount = 0, bool block = false)
		: this (filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, currentCount, block) { }

	public override TabbedWriter<StreamWriter> CreateBlock (string line, bool block)
	{
		// when we create a new block, the first thing we want to do is make sure that
		// we flush the current writer, so we do not mix the current block with the new one
		if (!string.IsNullOrEmpty (line)) {
			Writer.WriteLine (line);
		}
		Writer.Flush ();

		// the issue now is that on the dispose method we are going to close and flush the writer, which means that
		// we cannot use the writer of the parent, else we will have problems because the parent object will be closed.
		// This means that in this method, it is safe to assume that we are create the tile stream as a read/write stream
		// in append mode. If the file does not exist, we are ok with an exception.
		var newBlock = new TabbedStreamWriter (path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, Writer.Indent,
			block);
		// set the parent to point to this stream, this way when we are closed, we can tell it to move fwd
		newBlock.parent = this;
		return newBlock;
	}

	public void Flush ()
	{
		Writer.Flush ();
	}

	public async Task FlushAsync ()
	{
		await Writer.FlushAsync ();
	}

	void SyncParent ()
	{
		// if we have a parent, move the stream to the end
		parent?.InnerWriter.BaseStream.Seek (0, SeekOrigin.End);
		parent = null;
	}

	public override void Close ()
	{
		base.Close ();
		// if we have a parent, move the stream to the end
		SyncParent ();
	}

	public override async Task CloseAsync ()
	{
		await base.CloseAsync ();
		SyncParent ();
	}
}
