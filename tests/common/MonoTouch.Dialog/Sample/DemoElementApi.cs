//
// Sample showing the core Element-based API to create a dialog
//
using System;
using System.Linq;
using MonoTouch.Dialog;
using UIKit;

namespace Sample
{
	public partial class AppDelegate 
	{		
		public void DemoElementApi ()
		{
			var root = CreateRoot ();
				
			var dv = new DialogViewController (root, true);
			navigation.PushViewController (dv, true);				
		}

		RootElement CreateRoot ()
		{
			return new RootElement ("Settings") {
				new Section (){
					new BooleanElement ("Airplane Mode", false),
					new RootElement ("Notifications", 0, 0) {
						new Section (null, "Turn off Notifications to disable Sounds\n" +
							     "Alerts and Home Screen Badges for the\napplications below."){
							new BooleanElement ("Notifications", false)
						}
					}},
				new Section (){
					CreateSoundSection (),
					new RootElement ("Brightness"){
						new Section (){
							new FloatElement (null, null, 0.5f),
							new BooleanElement ("Auto-brightness", false),
						}
					},
					new RootElement ("Wallpaper"){
						new Section (){
							new ImageElement (null),
							new ImageElement (null),
							new ImageElement (null)
						}
					}
				},
				new Section () {
					new EntryElement ("Login", "Your login name", null)
					{
						EnablesReturnKeyAutomatically = true,
						AlignEntryWithAllSections = true,
					},
					new EntryElement ("Password", "Your password", null, true)
					{
						EnablesReturnKeyAutomatically = true,
						AlignEntryWithAllSections = true,
					},
					new DateElement ("Select Date", DateTime.Now),
					new TimeElement ("Select Time", DateTime.Now),
				},
				new Section () {
					new EntryElement ("Another Field", "Aligns with above fields", null)
					{
						AlignEntryWithAllSections = true,
					},
				},
				new Section () {
					CreateGeneralSection (),		
					//new RootElement ("Mail, Contacts, Calendars"),
					//new RootElement ("Phone"),
					//new RootElement ("Safari"),
					//new RootElement ("Messages"),
					//new RootElement ("iPod"),
					//new RootElement ("Photos"),
					//new RootElement ("Store"), 
				},
				new Section () {
					new HtmlElement ("About", "http://monotouch.net"),
					new MultilineElement ("Remember to eat\nfruits and vegetables\nevery day")
				}
			};		
		}
		
		RootElement CreateSoundSection ()
		{
			return new RootElement ("Sounds"){
				new Section ("Silent") {
					new BooleanElement ("Vibrate", true),
				},
				new Section ("Ring") {
					new BooleanElement ("Vibrate", true),
					new FloatElement (null, null, 0.8f),
					new RootElement ("Ringtone", new RadioGroup (0)){
						new Section ("Custom"){
							new RadioElement ("Circus Music"),
							new RadioElement ("True Blood"),
						},
						new Section ("Standard"){
							from n in "Marimba,Alarm,Ascending,Bark,Xylophone".Split (',')
								select (Element) new RadioElement (n)
						}
					},
					new RootElement ("New Text Message", new RadioGroup (3)){
						new Section (){
							from n in "None,Tri-tone,Chime,Glass,Horn,Bell,Eletronic".Split (',')
								select (Element) new RadioElement (n)
						}
					},
					new BooleanElement ("New Voice Mail", false),
					new BooleanElement ("New Mail", false),
					new BooleanElement ("Sent Mail", true),
					new BooleanElement ("Calendar Alerts", true),
					new BooleanElement ("Lock Sounds", true),
					new BooleanElement ("Keyboard Clicks", false)
				}
			};
		}
		
		public RootElement CreateGeneralSection ()
		{
			return new RootElement ("General") {
				new Section (){
					new RootElement ("About"){
						new Section ("My Phone") {
							new RootElement ("Network", new RadioGroup (null, 0)) {
								new Section (){
									new RadioElement ("My First Network"),
									new RadioElement ("Second Network"),
								}
							},
							new StringElement ("Songs", "23"),
							new StringElement ("Videos", "3"),
							new StringElement ("Photos", "24"),
							new StringElement ("Applications", "50"),
							new StringElement ("Capacity", "14.6GB"),
							new StringElement ("Available", "12.8GB"),
							new StringElement ("Version", "3.0 (FOOBAR)"),
							new StringElement ("Carrier", "My Carrier"),
							new StringElement ("Serial Number", "555-3434"),
							new StringElement ("Model", "The"),
							new StringElement ("Wi-Fi Address", "11:22:33:44:55:66"),
							new StringElement ("Bluetooth", "aa:bb:cc:dd:ee:ff:00"),
						},
						new Section () {
							new HtmlElement ("Monologue", "http://www.go-mono.com/monologue"),
						}
					},
					new RootElement ("Usage"){
						new Section ("Time since last full charge"){
							new StringElement ("Usage", "0 minutes"),
							new StringElement ("Standby", "0 minutes"),
						},
						new Section ("Call time") {
							new StringElement ("Current Period", "4 days, 21 hours"),
							new StringElement ("Lifetime", "7 days, 20 hours")
						},
						new Section ("Celullar Network Data"){
							new StringElement ("Sent", "10 bytes"),
							new StringElement ("Received", "30 TB"),
						},
						new Section (null, "Last Reset: 1/1/08 4:44pm"){
							new StringElement ("Reset Statistics"){
								Alignment = UITextAlignment.Center
							}
						}
					}
				},
				new Section (){
					new RootElement ("Network"){
						new Section (null, "Using 3G loads data faster\nand burns the battery"){
							new BooleanElement ("Enable 3G", true)
						},
						new Section (null, "Turn this on if you are Donald Trump"){
							new BooleanElement ("Data Roaming", false),
						},
						new Section (){
							new RootElement ("VPN", 0, 0){
								new Section (){
									new BooleanElement ("VPN", false),
								},
								new Section ("Choose a configuration"){
									new StringElement ("Add VPN Configuration")
								}
							}
						}
					},
					new RootElement ("Bluetooth", 0, 0){
						new Section (){
							new BooleanElement ("Bluetooth", false)
						}
					},
					new BooleanElement ("Location Services", true),
				},
				new Section (){
					new RootElement ("Auto-Lock", new RadioGroup (0)){
						new Section (){
							new RadioElement ("1 Minute"),
							new RadioElement ("2 Minutes"),
							new RadioElement ("3 Minutes"),
							new RadioElement ("4 Minutes"),
							new RadioElement ("5 Minutes"),
							new RadioElement ("Never"),
						}
					},
					// Can be implemented with StringElement + Tapped, but makes sample larger
					// new StringElement ("Passcode lock"),
					new BooleanElement ("Restrictions", false),
				},
				new Section () {
					new RootElement ("Home", new RadioGroup (2)){
						new Section ("Double-click the Home Button for:"){
							new RadioElement ("Home"),
							new RadioElement ("Search"),
							new RadioElement ("Phone favorites"),
							new RadioElement ("Camera"),
							new RadioElement ("iPod"),
						},
						new Section (null, "When playing music, show iPod controls"){
							new BooleanElement ("iPod Controls", true),
						}
						// Missing feature: sortable list of data
						// SearchResults
					},
					new RootElement ("Date & Time"){
						new Section (){
							new BooleanElement ("24-Hour Time", false),
						},
						new Section (){
							new BooleanElement ("Set Automatically", false),
							// TimeZone: Can be implemented with string + tapped event
							// SetTime: Can be imeplemeneted with String + Tapped Event
						}
					},
					new RootElement ("Keyboard"){
						new Section (null, "Double tapping the space bar will\n" +
							"insert a period followed by a space"){
							new BooleanElement ("Auto-Correction", true),
							new BooleanElement ("Auto-Capitalization", true),
							new BooleanElement ("Enable Caps Lock", false),
							new BooleanElement ("\".\" Shortcut", true),
						},
						new Section (){
							new RootElement ("International Keyboards", new Group ("kbd")){
								new Section ("Using Checkboxes"){
									new CheckboxElement ("English", true, "kbd"),
									new CheckboxElement ("Spanish", false, "kbd"),
									new CheckboxElement ("French", false, "kbd"),
								},
								new Section ("Using BooleanElement"){
									new BooleanElement ("Portuguese", true, "kbd"),
									new BooleanElement ("German", false, "kbd"),
								},
								new Section ("Or mixing them"){
									new BooleanElement ("Italian", true, "kbd"),
									new CheckboxElement ("Czech", true, "kbd"),
								}
							}
						}
					}
				}
			};
		}
	
	}
}
