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
using System.Reflection;
using ObjCRuntime;

namespace Foundation {
	public partial class NSXpcInterface : NSObject {
		public static NSXpcInterface Create (Type interfaceType)
		{
			if (interfaceType is null)
				throw new ArgumentNullException (nameof (interfaceType));
			return Create (new Protocol (interfaceType));
		}

		public NSSet<Class> GetAllowedClasses (MethodInfo method, nuint argumentIndex, bool forReplyBlock)
		{
			if (method is null)
				throw new ArgumentNullException (nameof (method));

			ExportAttribute attribute = method.GetCustomAttribute<ExportAttribute> ();
			if (attribute is null)
				throw new ArgumentException ($"Method {method.Name} is not exposed to Objective-C", nameof (method));

			// The runtime ensures that the Selector property is non-null and a valid selector.
			Selector sel = new Selector (attribute.Selector);
			return GetAllowedClasses (sel, argumentIndex, forReplyBlock);
		}

		public void SetAllowedClasses (MethodInfo method, NSSet<Class> allowedClasses, nuint argumentIndex, bool forReplyBlock)
		{
			if (method is null)
				throw new ArgumentNullException (nameof (method));

			ExportAttribute attribute = method.GetCustomAttribute<ExportAttribute> ();
			if (attribute is null)
				throw new ArgumentException ($"Method {method.Name} is not exposed to Objective-C", nameof (method));

			// The runtime ensures that the Selector property is non-null and a valid selector.
			Selector sel = new Selector (attribute.Selector);
			SetAllowedClasses (allowedClasses, sel, argumentIndex, forReplyBlock);
		}
	}
}
