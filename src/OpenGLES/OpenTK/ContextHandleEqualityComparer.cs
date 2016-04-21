#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */
#endregion

using System;
using System.Collections.Generic;

namespace OpenTK
{
    class ContextHandleEqualityComparer : IEqualityComparer<ContextHandle> {

        public bool Equals(ContextHandle a, ContextHandle b)
        {
            return a.Equals(b);
        }

        public int GetHashCode (ContextHandle h)
        {
            return h.GetHashCode();
        }
    }
}

// vim: et sw=4 ts=4
