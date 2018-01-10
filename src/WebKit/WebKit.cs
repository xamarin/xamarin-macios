using Foundation;

namespace WebKit {
	
	public partial class WebFrame {
		public void LoadHtmlString (string htmlString, NSUrl baseUrl)
		{
			LoadHtmlString ((NSString) htmlString, baseUrl);
		}
	}

#if !XAMCORE_2_0
	public partial class WebScriptObject {
		public void SetWebScriptValueAtIndex (int /* unsigned int */ index, NSObject value)
		{
			SetWebScriptValueAtIndexvalue (index, value);
		}
	}
#endif
}
