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

namespace AudioUnitWrapper
{
#if !XAMCORE_2_0
    [Obsolete ("Use 'AudioConverter'.")]
    public class _AudioConverterEventArgs : EventArgs
    {
        #region Variables
        public uint NumberDataPackets;
        public readonly AudioBufferList Data;
        public readonly AudioStreamPacketDescription[] DataPacketDescription;
        #endregion

        #region Constructor
        public _AudioConverterEventArgs(
            uint _NumberDataPackets,
            AudioBufferList _Data,
            AudioStreamPacketDescription[] _DataPacketDescription)
        {
            NumberDataPackets = _NumberDataPackets;
            Data = _Data;
            DataPacketDescription = _DataPacketDescription;
        }
        #endregion
    }
#endif
}
