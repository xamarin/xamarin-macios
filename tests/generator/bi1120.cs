using Foundation;
using ObjCRuntime;

#if NET   
using System.Runtime.Versioning;                                                                                                                      |    NSString password);
#endif

namespace NS {

#if NET
	[SupportedOSPlatform ("maccatalyst")]
#else
	[MacCatalyst (15,0)]
#endif
	public class  Foo {
			void Method () { }
	}
}
