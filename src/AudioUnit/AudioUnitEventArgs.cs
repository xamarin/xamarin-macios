//
// AudioUnitEventArgs.cs: AudioUnit callback argument
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioToolbox;

namespace AudioUnit
{
#if !XAMCORE_2_0
    [Obsolete]
    public class AudioUnitEventArgs : EventArgs
    {
        #region Variables
        public readonly AudioUnitRenderActionFlags ActionFlags;
        public readonly AudioTimeStamp TimeStamp;
        public readonly int BusNumber;
        public readonly int NumberFrames;
        public readonly AudioBufferList Data;
        #endregion

        #region Constructor
        public AudioUnitEventArgs(AudioUnitRenderActionFlags actionFlags,
				  AudioTimeStamp timestamp,
				  int busNumber,
				  int frames,
				  AudioBufferList data)
        {
            ActionFlags = actionFlags;
            this.TimeStamp = timestamp;
            BusNumber = busNumber;
            NumberFrames = frames;
            Data = data;
        }
        #endregion
    }
#endif
}
