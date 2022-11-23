namespace macOSApp1;

[Register ("View1")]
public class View1 : NSView {
	public View1 ()
	{
		// Set a light blue background color
		WantsLayer = true;
		Layer!.BackgroundColor = NSColor.FromRgb (51, 171, 249).CGColor;
	}
}
