using Foundation;

namespace Simple {
  [BaseType (typeof (NSObject))]
  interface SimpleClass {
    [Export ("doIt")]
    int DoIt ();
  }
}