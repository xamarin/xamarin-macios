//
// AudioBufferList.cs: AudioBufferList wrapper class
//
// Author:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//
// Copyright 2010 Reinforce Lab.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace AudioToolbox
{
#if !XAMCORE_2_0
	[Obsolete ("Use 'AudioBuffers'.")]
	[StructLayout(LayoutKind.Sequential)]
	public class AudioBufferList {
		// Preserve is support, but Conditional is not, on fields and will mark the type (not optimal)
		// we can workaround this by pmaking sure the field can't be linked out if the type is marked
		// e.g. by using it inside ToString
		// [Preserve (Conditional=true)]
		internal int bufferCount;
		// mBuffers array size is variable. But here we uses fixed size of 2, because iPhone phone terminal two (L/R) channels.        
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		internal AudioBuffer [] buffers;
		
		public int BufferCount { get { return bufferCount; }}
		public AudioBuffer [] Buffers { get { return buffers; }}
		
		public AudioBufferList() 
		{
		}

		public AudioBufferList (int count)
		{
			bufferCount = count;
			buffers = new AudioBuffer [count];
		}

		public override string ToString ()
		{
			if (buffers != null && bufferCount > 0)
				return string.Format ("[buffers={0},bufferSize={1}]", buffers [0], buffers [0].DataByteSize);
			
			return "[empty]";
		}
	}

	[Obsolete ("Use 'AudioBuffers'.")]
	public class MutableAudioBufferList : AudioBufferList, IDisposable {
		public MutableAudioBufferList (int nubuffers, int bufferSize)
			: base (nubuffers)
		{
			for (int i = 0; i < bufferCount; i++) {
				buffers[i].NumberChannels = 1;
				buffers[i].DataByteSize = bufferSize;
				buffers[i].Data = Marshal.AllocHGlobal((int)bufferSize);
			}
		}
			
		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public virtual void Dispose (bool disposing)
		{
			if (buffers != null){
				foreach (var mbuf in buffers)
					Marshal.FreeHGlobal(mbuf.Data);
				buffers = null;
			}
		}

		~MutableAudioBufferList ()
		{
			Dispose (false);
		}
	}
#endif
}