MonoTouch.Dialog
================

MonoTouch.Dialog is a foundation to create dialog boxes and show
table-based information without having to write dozens of delegates
and controllers for the user interface.  Table support Pull-to-Refresh
as well as built-in searching.

![screenshot](http://tirania.org/images/MTDialogSample.png "Sample") 

This was created with the following code:

    return new RootElement ("Settings") {
        new Section (){
            new BooleanElement ("Airplane Mode", false),
            new RootElement ("Notifications", 0, 0) {
                new Section (null, 
    		    	 "Turn off Notifications to disable Sounds\n" +
                         "Alerts and Home Screen Badges for the\napplications below."){
                    new BooleanElement ("Notifications", false)
                }
            }},
        new Section (){
	    new RootElement ("Sounds"){
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
                            from name in "Marimba,Alarm,Ascending,Bark".Split (',')
                                 select (Element) new RadioElement (name)
                        }
                    },
                    new RootElement ("New Text Message", new RadioGroup (3)){
                        new Section (){
                            from name in "None,Tri-tone,Chime,Glass,Horn,Bell,Electronic".Split (',')
                                select (Element) new RadioElement (name)
                        }
                    },
                    new BooleanElement ("New Voice Mail", false),
                    new BooleanElement ("New Mail", false),
                    new BooleanElement ("Sent Mail", true),
                }
            },
            new RootElement ("Brightness"){
                new Section (){
                    new FloatElement (null, null, 0.5f),
                    new BooleanElement ("Auto-brightness", false),
                }
            },
            new RootElement ("Wallpaper"){ MakeWallPaper (); }
        },
        new Section () {
            new EntryElement ("Login", "Your login name", "miguel"),
            new EntryElement ("Password", "Your password", "password", true),
            new DateElement ("Select Date", DateTime.Now),
        },
    }

It is also possible to create user interfaces using a local or remote
Json objects, this can be useful to create interfaces driven by data
generated on a server.  See the section below "Json" for more
information.

In addition to being a simple way to create dialogs, it also has been
growing to contains a number of utility functions that are useful for
iPhone development.

MonoTouch.Dialog is a retained system for implementing UITableViews
as opposed to the on-demand nature of UITableView.

Currently this supports creating Dialogs based on navigation controllers 
that support both basic and advanced cells.

Some basic cells include:

  * On/Off controls
  * Slider (floats)
  * String informational rendering
  * Text Entry
  * Password Entry
  * Jump to HTML page
  * Radio elements
  * Dates, Times and Dates+Times
  * Pull-to-refresh functionality.
  * Activity indicators

Advanced cells include:
  * Container for arbitrary UIViews
  * Mail-like message displays
  * Styled cells, with optional image downloading

The entire UI for TweetStation (http://github.com/migueldeicaza/TweetStation)
an app published on the AppStore was built entirely using MonoTouch.Dialog.

You can download the app from the AppStore, and read the code to learn about
some advanced used cases of MonoTouch.Dialog.

Miguel (miguel@gnome.org)

Additional Elements
===================

Additional user-contributed elements are now available in 

http://github.com/xamarin/monotouch-element-pack


Using MonoTouch.Dialog
======================

MonoTouch.Dialog core entry point is a UIViewController called the
MonoTouch.Dialog.DialogViewController.  You initialize instances of
this object from an object of type "RootElement" or "JsonElement".

RootElements can be created either manually with the "Elements" API by
creating the various nodes necessary to render the information.  You
would use this if you need control, if you want to extend the features
supported by MonoTouch.Dialogs or if you want to dynamically generate
the content for your dialog.   This is what is used for example in 
TweetStation for the main timeline views.

Additionally, there is a trivial Reflection-based constructor that can
be used for quickly putting together dialogs, for example, creating an
account page is as trivial as:

    class AccountInfo {
        [Section]
        public bool AirplaneMode;
    
        [Section ("Data Entry", "Your credentials")]
    
        [Entry ("Enter your login name")]
        public string Login;
    
        [Caption ("Password"), Password ("Enter your password")]
        public string passwd;

        [Section ("Travel options")]
        public SeatPreference preference;
    }

    void Setup ()
    {
        account = new AccountInfo ();
    
        var bc = new BindingContext (this, account, "Seat Selection");
    }

Which produces this UI:

![Rendering of AccountInfo](http://github.com/migueldeicaza/MonoTouch.Dialog/raw/master/sample.png)

This is what the Elements API usage looks like, it is a more flexible 
API and the one I suggest you use for anything that requires
customizations and goes beyond the basics of the Reflection-based
attributes:

        var root = new RootElement ("Settings") {
          new Section (){
            new BooleanElement ("Airplane Mode", false),
            new RootElement ("Notifications", 0, 0) {
              new Section (null, 
                  "Turn off Notifications to disable Sounds\n" +
                  "Alerts and Home Screen Badges for the."){
                new BooleanElement ("Notifications", false)
              }
            }},
          new Section (){
            new RootElement ("Brightness"){
              new Section (){
                new FloatElement (null, null, 0.5f),
                new BooleanElement ("Auto-brightness", false),
		new UILabel (new RectangleF (10, 10, 100, 40) {
		    Text = "I am a simple UILabel!"
		}
              }
            },
          },
          new Section () {
            new EntryElement ("Login", "enter", "miguel"),
            new EntryElement ("Password", "enter", "password", true),
            new DateElement ("Select Date", DateTime.Now),
            new TimeElement ("Select Time", DateTime.Now),
          },

To create nested UIs that provide automatic navigation, you would just
create an instance of that class.  

Autorotation is supported by default by setting the Autorotate property
in the DialogViewController.   Setting this value will propagate to 
the various components that are shiped with MonoTouch.Dialog like the
WebView and the date and time pickers

Pull to Refresh Support
-----------------------

Pull to Refresh is a visual effect originally found in Tweetie2 which
became a popular effect among many applications.

To add automatic pull-to-refersh support to your dialogs, you only
need to do two things: hook up an event handler to be notified when
the user pulls the data and notify the DialogViewController when the
data has been loaded to go back to its default state.

Hooking up a notification is simple, just connect to the
RefreshRequested event on the DialogViewController, like this:

        dvc.RefreshRequested += OnUserRequestedRefresh;

Then on your method OnUserRequestedRefresh, you would queue some data
loading, request some data from the net, or spin a thread to compute
the data.  Once the data has been loaded, you must notify the
DialogViewController that the new data is in, and to restore the view
to its default state, you do this by calling ReloadComplete:

       dvc.ReloadComplete ();

Search Support
--------------

To support searching, set the EnableSearch property on your
DialogViewController.   You can also set the SearchPlaceholder
property to use as the watermark text in the search bar.

Searching will change the contents of the view as the user types, it
searches the visible fields and shows those to the user.  The
DialogViewController exposes three methods to programatically
initiate, terminate or trigger a new filter operation on the results:

	  StartSearch, FinishSearch, PerformFilter

The system is extensible, so you can alter this behavior if you want,
details are below.

Samples Included
----------------

The sample program exercises various features of the API and should be
a useful guide on how to implement certain features.  One of the demos
uses the Elements API to replicate the "Settings" application on the
iPhone.  

The High-Level Reflection API
=============================

The Reflection-based dialog construction is used by creating an object
of class MonoTouch.Dialog.BindingContext, the method takes three
parameters:

  * An object that will be used to resolve Tap targets.

  * The object that will be edited.

A very simple dialog that contains a checkbox is shown here:

    class Settings {
        public bool AirplaneMode;
    }

The above will generate a page that contains a single item with the
caption "Airplane Mode" and a on/off switch.   The caption is computed
based on the field name.  In this case "AirplaneMode" becomes
"Airplane Mode".   MonoTouch.Dialogs supports other conventions so
"AirplaneMode", "airplaneMode" and "airplane_mode" all produce the
same caption "Airplane Mode".

If you need to control the actual caption (for example to include
special characters, use a different spelling or you are reusing an
existing class) you just need to attach the [Caption] attribute to
your variable, like this:

        [Caption ("Your name is:")]
        string userName;

The dialog contents are rendered in the same order that the fields are
declared in the class.  You can use the [Section] attribute to
group information in sections that make sense.   You can use the
[Section] attribute in a few ways:

        [Section]

>>Creates a new section, with no headers or footers.

        [Section (header)]

>> Creates a new section, with the specified header and no footer.

        [Section (header, footer)]**
>> Creates a new section with the specified header and footer.

These are the current widgets supported by the Reflection API:

### String constants and Buttons. ###

  Use the string type.   If the type has a value, in 
  addition to showing the caption, it will render its
  value on the right.

  You can add the [OnTap] attribute to your string 
  to invoke a method on demand.

  You can add the [Multiline] attribute to your string
  to make the cell render in multiple lines.   And you 
  can use the [Html] attribute on a string, in that
  case the value of the string should contain the url
  to load in the embedded UIWebView. 

  The [Aligntment] attribute takes a parameter a UITextAlingment
  that determines how the string should be rendered

  Examples:

        public string Version = "1.2.3";

        [OnTap ("Login")]
        public string Login;

        [Caption ("(C) FooBar, Inc")]
        string copyright;

        [Caption ("This is a\nmultiline caption")]
        [Multiline]
        string multiline;

        [Caption ("Date")]
        [Alignment (UITextAlignment.Center)]
        string centered;

### Text Entry and Password Entries.###

  Use the string type for your field and annotate the 
  string with the [Entry] attribute.   If you provide
  an argument to the [Entry] attribute, it will be used
  as the greyed-out placeholder value for the UITextField.

  Use the [Password] attribute instead of [Entry] to 
  create a secure entry line.
 
  Examples:

        [Entry ("Your username")]
        public string Login;
  
        [Entry]
        public string StreetName;

        [Password, Caption ("Password")]
        public string passwd;

  You can also specify both the Placeholder and the keyboard type
  to use on the Entry using a few of the Entry attributes:

	[Entry (KeyboardType=UIKeyboardType.NumberPad,Placeholder="Your Zip code")]
	public string ZipCode;

### On/off switches ###

  Use a bool value to store an on/off setting, by default you
  will get an On/off switch, but you can change this behavior to
  just show a checkbox instead by using the [Checkbox] attribute:

  Examples:

        bool OnOffSwitch;

        [Checkbox]
        bool ReadyToRun;

### Float values ###

  Using a float in your source will provide a slider on the 
  screen.   You can control the ranges of the values by
  using the [Range (low,high)] attribute.   Otherwise the
  default is to edit a value between 0 and 1.

  Examples:

        float brightness;

        [Range (0, 10), Caption ("Grade")]
        float studentGrade;

### Date Editing ###

  Use a "DateTime" object in your class to present a date
  picker.

  By default this will provide a date and time editor, if you
  only want to edit the date, set the [Date] attribute, if you
  only want to edit the time, set the [Time] attribute:

  Examples:

        [Date]
        DateTime birthday;

        [Time]
        DateTime alarm;

        [Caption ("Meeting Time")]
        DateTime meetingTime;

### Enumeration value ###

  Monotouch.Dialogs will automatically turn an enumeration
  into a radio selection.   Merely specify the enumeration
  in your file:

  Examples:

          enum SeatPreference { Window, Aisle, MiddleSeat }

          [Caption ("Seat Preference")]
          SeatPreference seat;

  Additionally, the [Caption] attribute can be applied to the
  individual elements of an enumeration value to customize how they
  are rendered.
  
### Images ###

  Variables with type UIImage will render the image as a 
  thumbnail and will invoke the image picker if tapped on.

  Examples:

        UIImage ProfilePicture;

### Ignoring Some Fields ###

  If you want to ignore a particular field just apply the [Skip]
  attribute to the field.   

  Examples:
        [Skip] Guid UniquId;

### Nested Dialogs ###

  To create nested dialogs just use a nested class, the reflection
  binder will create the necessary navigation bits based on the
  container model.

  The value for a nested dialog must not be null.

  Examples:

	class MainSettings {
	    string Subject;
	    string RoomName;
	    TimeRange Time;
	}

	class TimeRange {
	    [Time] DateTime Start;
	    [Time] DateTime End;
	}

To initialize:

	new MainSettings () {
	    Subject = "Review designs",
	    RoomName = "Conference Room II",
	    Time = new TimeRange {
	        Start = DateTime.Now,
		End   = DateTime.Now
            }
        }

### IEnumerable as a Radio Source ###

You can use any type that implements IEnumerable, including
generics (which implement IEnumerable) as a source of values
for creating a one-of-many selector, similar to the radio-like
selection that you get from an enumeration.

To use this, you will need an int value that has the [RadioSelection]
attribute set to hold the value that will be selected on startup
and also to hold the new value when done.

For example:

        class MainSettings {
	    [RadioSelection ("Themes")]
	    public int CurrentTheme;
	    public IList<string> Themes;
	}

The value rendered is the value rendered by calling ToString() on the
value returned by IEnumerable.

Creating a Dialog From the Object
---------------------------------

Once you have created your class with the proper attributes, you
create a binding context, like this:

        BindingContext context;

        public void Setup ()
        {
            // Create the binder.
            context = new BindingContext (this, data, "Settings");

            // Create our UI
            // Pass our UI (context.Root) and request animation (true)
            var viewController = new DialogViewController (context.Root, true);

            navigation.PushViewController (viewController, true);
        }

This will render the information.   To fetch the values back after
editing you need to call context.Fetch ().   You can do this from your
favorite handler, and at that point you can also call
context.Dispose() to assist the GC in releasing any large resources it
might have held.

The Elements API
================

All that the Reflection API does is create a set of nodes from the
Elements API.   

First a sample of how you would create a UI taking advantage of 
C# 3.0 initializers:

        var root = new RootElement ("Settings") {
          new Section (){
            new BooleanElement ("Airplane Mode", false),
            new RootElement ("Notifications", 0, 0) {
              new Section (null, 
                  "Turn off Notifications to disable Sounds\n" +
                  "Alerts and Home Screen Badges for the."){
                new BooleanElement ("Notifications", false)
              }
            }},
          new Section (){
            new RootElement ("Brightness"){
              new Section (){
                new FloatElement (null, null, 0.5f),
                new BooleanElement ("Auto-brightness", false),
				new UILabel ("I am a simple UILabel!"),
              }
            },
          },
          new Section () {
            new EntryElement ("Login", "enter", "miguel"),
            new EntryElement ("Password", "enter", "password", true),
            new DateElement ("Select Date", DateTime.Now),
            new TimeElement ("Select Time", DateTime.Now),
          },

You will need a RootElement to get things rolling.   The nested
structure created by Sections() and Elements() are merely calls to
either RootElement.Add () or Section.Add() that the C# compiler 
invokes for us.

The basic principle is that the DialogViewController shows one
RootElement, and a RootElement is made up of Sections which in turn
can contain any kind of Element (including other RootElements).

RootElements inside a Section when tapped have the effect of activating
a nested UI on a new DialogViewController. 

Another advantage of the C# 3.0 syntax is that it can be integrated
with LINQ, you can use integrated queries to generate the user
interface based on your data.  The following example is taken from the
MIX Conference:

	 RootElement MakeDay (DateTime day){
	 	return new RootElement ("Sessions for " + day) {
	 	    from s in AppDelegate.ConferenceData.Sessions
	 		where s.Start.Day == day
	 		orderby s.Start ascending
	 		group s by s.Start.ToString() into g
	 		select new Section (MakeCaption ("", Convert.ToDateTime(g.Key))) 
	 			from hs in g
	 			   select (Element) new SessionElement (hs)
	 	};
	}

This generates the user interface for the sessions on a given day

The hierarchy of Elements looks like this:

        Element
           BadgeElement
           BoolElement
              BooleanElement       - uses an on/off slider
              BooleanImageElement  - uses images for true/false
           EntryElement
           FloatElement
           HtmlElement
           ImageElement
	   MessageElement
           MultilineElement
           RootElement (container for Sections)
           Section (only valid container for Elements)
           StringElement
              CheckboxElement
              DateTimeElement
                  DateElement
                  TimeElement
              ImageStringElement
              RadioElement
              StyleStringElement
          UIViewElement
        
Additionally notice that when adding elements to a section, you
can use either Elements or UIViews directly.   The UIViews are
just wrapped in a special UIViewElement element.

You can also create your own Elements by subclassing one of the 
above elements and overriding a handful of methods.

RootElement
-----------

RootElements are responsible for showing a full configuration page.

At least one RootElement is required to start the MonoTouch.Dialogs
process.

If a RootElement is initialized with a section/element value then
this value is used to locate a child Element that will provide
a summary of the configuration which is rendered on the right-side
of the display.

RootElements are also used to coordinate radio elements.  The
RadioElement members can span multiple Sections (for example to
implement something similar to the ring tone selector and separate
custom ring tones from system ringtones).  The summary view will show
the radio element that is currently selected.  To use this, create
the Root element with the Group constructor, like this:

       var root = new RootElement ("Meals", new RadioGroup ("myGroup", 0))

The name of the group in RadioGroup is used to show the selected value
in the containing page (if any) and the value, which is zero in this
case, is the index of the first selected item. 

Root elements can also be used inside Sections to trigger
loading a new nested configuration page.   When used in this mode
the caption provided is used while rendered inside a section and
is also used as the Title for the subpage.   For example:

	var root = new RootElement ("Meals") {
	    new Section ("Dinner"){
                new RootElement ("Desert", new RadioGroup ("desert", 2) {
                    new Section () {
                        new RadioElement ("Ice Cream", "desert"),
                        new RadioElement ("Milkshake", "desert"),
                        new RadioElement ("Chocolate Cake", "desert")
                    }
                }
            }
        }

In the above example, when the user taps on "Desert", MonoTouch.Dialog
will create a new page and navigate to it with the root being "Desert"
and having a radio group with three values.

In this particular sample, the radio group will select "Chocolate
Cake" in the "Desert" section because we passed the value "2" to the
RadioGroup.  This means pick the 3rd item on the list (zero-index). 

Sections are added by calling the Add method or using the C# 4
initializer syntax.  The Insert methods are provided to insert
sections with an animation.

Additionally, you can create RootElement by using LINQ, like this:

        new RootElement ("LINQ root") {
            from x in new string [] { "one", "two", "three" }
               select new Section (x) {
                  from y in "Hello:World".Split (':')
                    select (Element) new StringElement (y)
               }
        }

If you create the RootElement with a Group instance (instead of a
RadioGroup) the summary value of the RootElement when displayed in a
Section will be the cummulative count of all the BooleanElements and
CheckboxElements that have the same key as the Group.Key value.

JsonElement
-----------

The JsonElement is a sublcass of RootElement that extends a RootElement
to be able to load the contents of nested child from a local or remote
url.

The JsonElement is a RootElement that can be instantiated in two
forms.  One version creates a RootElement that will load the contents
on demand, these are created by using the JsonElement constructors,
which take an extra argument at the end, the url to load the contents
from:

    var je = new JsonElement ("Dynamic Data", "http://tirania.org/tmp/demo.json");

The other form creates the data from a local file or an existing
System.Json.JsonObject that you have already parsed:

    var je = JsonElement.FromFile ("json.sample");

    using (var reader = File.OpenRead ("json.sample"))
        return JsonElement.FromJson (JsonObject.Load (reader) as JsonObject, arg);

See the section "Json Syntax" for the sample file format and the
description of the Json objects.

Sections
--------

Sections are used to group elements in the screen and they are the
only valid direct child of the RootElement.    Sections can contain
any of the standard elements, including new RootElements.

RootElements embedded in a section are used to navigate to a new
deeper level.

Sections can have headers and footers either as strings, or as
UIViews.  Typically you will just use the strings, but to create
custom UIs you can use any UIView as the header or the footer.  You
can either use a string or a view, you would create them like this:

        var section = new Section ("Header", "Footer")

To use views, just pass the views to the constructor:

        var header = new UIImageView (Image.FromFile ("sample.png"));
        var section = new Section (image)


Standard Elements
-----------------

MonoTouch.Dialog comes with various standard elements that you can
use, the basics include:

  * BooleanElement
  * CheckboxElement
  * FloatElement
  * HtmlElement (to load web pages)
  * ImageElement (to pick images)
  * StringElement
    To render static strings
    To render strings with a read-only value.
    To be used as "buttons", pass a delegate for this.
  * StyledStringElement
    Similar to StringElement but allows for the Font, TextColor, 
    images and accessories to be set on a per-cell basis.
  * MultilineElement
    Derives from StringElement, used to render multi-line cells.
  * RadioElements (to provide a radio-button feature).
  * EntryElement (to enter one-line text or passwords)
  * DateTimeElement (to edit dates and times).
  * DateElement (to edit just dates)
  * TimeElement (to edit just times)
  * BadgeElement 
    To render images (57x57) or calendar entries next to the text.

The more sophisticated elements include:

  * MessageElement
    To show Mail-like information showing a sender, a date, a
    summary and a message extract with optional message counts
    and read/unread indicators.

  * OwnerDrawnElement
    Allows developers to easily create elements that are drawn
    on demand.

  * UIViewElement
    Can we used to host a standard UIView as an element, in this
    case no cell reuse will take place.

Values
------

Elements that are used to capture user input expose a public "Value"
property that holds the current value of the element at any time.  It
is automatically updated as the user uses the application and does not
require any programmer intervention to fetch the state of the control.

This is the behavior for all of the Elements that are part of
MonoTouch.Dialog but it is not required for user-created elements.

StringElement
-------------

You can use this element to show static strings as a cell in your table,
and it is possible to use them as buttons by providing the constructor
with an NSAction delegate.   If the cell is tapped, this method is invoked
for example:

	 var l = new StringElement ("Calculate Total", delegate { ComputeTotal (); });

The cost of a StringElement is very low, it uses 8 bytes: 4 for the label alignment
information, and 4 for the text to be displayed.

StyledStringElement
-------------------

This class derives from StringElement but lets developers customize a handful of
properties like the Font, the text color, the background cell color, the line
breaking mode, the number of lines to display and whether an accessory should
be displayed.

For example:

	 var l = new StyleStringElement ("Report Spam") {
		BackgroundUri = new Uri ("file://" + Path.GetFullPath ("cute.png"),
		TextColor = UIColor.White
	 };

The StyledStringElement also can be configured at creation time to
pick one of the four standard cell types to render information, for example:

	 new StyledStringElement ("Default", "Invisible value", UITableViewCellStyle.Default),
	 new StyledStringElement ("Value1", "Aligned on each side", UITableViewCellStyle.Value1),
	 new StyledStringElement ("Value2", "Like the Addressbook", UITableViewCellStyle.Value2),
	 new StyledStringElement ("Subtitle", "Makes it sound more important", UITableViewCellStyle.Subtitle),
	 new StyledStringElement ("Subtitle", "Brown subtitle", UITableViewCellStyle.Subtitle) {
	 	 DetailColor = UIColor.Brown
	 }

See the Styled element sample for more information.

EntryElement
------------

The EntryElement is used to get user input and is initialized with
three values: the caption for the entry that will be shown to the
user, a placeholder text (this is the greyed-out text that provides a
hint to the user) and the value of the text.

The placeholder and value can be null, the caption can not.

At any point, the value of the EntryElement can be retrieved by
accessing its Value property.

Additionally the KeyboardType property can be set at creation time to
the keyboard type style desired for the data entry.  This can be used
to configure the keyboard for numeric input, phone input, url input or
email address input (The values of UIKeyboardType).

UIViewElement
-------------

Use this element to quickly add a standard UIView as cell in a UITableView,
you can control whether the cell can be selected or whether it is transparent
by passing one of the CellFlags to the constructor.

ActivityElement
---------------

This element shows a UIActivity indicator in the view, use this while your
application is loading data and you want to provide the user with some
visual feedback that your application is busy.


LoadMoreElement
---------------

Use this element to allow users to load more items in your list. 
You can customize the normal and loading captions, as well as the
font and text color.  The UIActivity indicator starts animating,
and the loading caption is displayed when a user taps the cell,
and then the NSAction passed into the constructor is executed.
Once your code in the NSAction is finished, the UIActivity indicator
stops animating and the normal caption is displayed again.

MessageElement
--------------

The message element can be used to render rows that render
message-like information, that includes a sender, a subject, a time, a
greyed out snippet and a couple of status features like read/unread or
the message count:

![MessageElement](http://tirania.org/s/a8f54e89.png)

The contents are controlled by a few properties:

	 string Sender, Body, Subject;
	 DateTime Date;
	 bool NewFlag;
	 int MessageCount;

You create them like this:

	 var msg = new MessageElement () {
	     Sender = "Miguel de Icaza",
	     Subject = "iPhone count",
	     Body = "We should discuss how many iPhones to get next week, should we go for a six-pack or six units?",
	     NewFlag = true,
	 }
	 msg.Tapped += delegate { ShowEmail (); }

OwnerDrawnElement
-----------------

This element must be subclassed as it is an abstract class.  You 
should override the Height(RectangleF bounds) method in which you
should return the height of the element, as well as 
Draw(RectangleF bounds, CGContext context, UIView view) in which
you should do all your customized drawing within the given bounds,
using the context and view parameters.
This element does the heavy lifting of subclassing a UIView, and 
placing it in the Cell to be returned, leaving you only needing to
implement two simple overrides.  You can see a better sample implementation
in the Sample app in the DemoOwnerDrawnElement.cs file.

Here's a very simple example of implementing the class:

	 public class SampleOwnerDrawnElement : OwnerDrawnElement
	 {
	 	public SampleOwnerDrawnElement (string text) : base(UITableViewCellStyle.Default, "sampleOwnerDrawnElement")
	 	{
	 		this.Text = text;
	 	}
	 	
	 	public string Text
	 	{
	 		get;set;	
	 	}
	 	
	 	public override void Draw (RectangleF bounds, CGContext context, UIView view)
	 	{
	 		UIColor.White.SetFill();
	 		context.FillRect(bounds);
	 		
	 		UIColor.Black.SetColor();	
	 		view.DrawString(this.Text, new RectangleF(10, 15, bounds.Width - 20, bounds.Height - 30), UIFont.BoldSystemFontOfSize(14.0f), UILineBreakMode.TailTruncation);
	 	}
	 	
	 	public override float Height (RectangleF bounds)
	 	{
	 		return 44.0f;
	 	}
	 }

Booleans
--------

The BoolElement is the base class for both the UISwitch-based boolean
rendered image as well as the BooleanImageElement which is a boolean
that can render the stage using a string.

Validation
----------

Elements do not provide validation themselves as the models that are
well suited for web pages and desktop applications do not map
directly to the iPhone interaction model.

If you want to do data validation, you should do this when the user
triggers an action with the data entered.  For example a "Done" or
"Next" buttons on the top toolbar, or some StringElement used as a
button to go to the next stage.   

This is where you would perform basic input validation, and perhaps
more complicated validation like checking for the validity of a
user/password combination with a server.

How you notify the user of an error is application specific: you could
pop up a UIAlertView or show a hint.

Creating Your Own Elements
--------------------------

You can create your own element by deriving from either an existing
Element or by deriving from the root class Element.

To create your own Element, you will want to override the following
methods:

        // To release any heavy resources that you might have
        void Dispose (bool disposing);

        // To retrieve the UITableViewCell for your element
        // you would need to prepare the cell to be reused, in the
        // same way that UITableView expects reusable cells to work
        UITableViewCell GetCell (UITableView tv)

        // To retrieve a "summary" that can be used with
        // a root element to render a summary one level up.  
        string Summary ()

        // To detect when the user has tapped on the cell
        void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)

        // If you support search, to probe if the cell matches the user input
        bool Matches (string text)

If your element can have a variable size, you need to implement the
IElementSizing interface, which contains one method:

    	// Returns the height for the cell at indexPath.Section, indexPath.Row
        float GetHeight (UITableView tableView, NSIndexPath indexPath);

If you are planning on implemeneting your GetCell method by calling
"base.GetCell(tv)" and customizing the returned cell, you need to also
override the CellKey property to return a key that will be unique to
your Element, like this:

	    static NSString MyKey = new NSString ("MyKey");
	    protected override NSString CellKey {
	        get {
	            return MyKey;
	        }
	    }

This works for most elements, but not for the StringElement and StyledStringElement
as those use their own set of keys for various rendering scenarios.   You would have
to replicate the code in those classes.

Customizing the DialogViewController
====================================

Both the Reflection and the Elements API use the same
DialogViewController.  Sometimes you will want to customize the look
of the view or you might want to use some features of the
UITableViewController that go beyond the basic creation of UIs.

The DialogViewController is merely a subclass of the
UITableViewController and you can customize it in the same way that
you would customize a UITableViewController.

For example, if you wanted to change the list style to be either
Grouped or Plain, you could set this value by changing the property
when you create the controller, like this:

        var myController = new DialogViewController (root, true){
            Style = UITableViewStyle.Grouped;
        }

For more advanced customizations, like setting the default background
for the DialogViewController, you would need to create a subclass of
it and override the proper methods.   

This example shows how to use an image as the background for the
DialogViewController:

    class SpiffyDialogViewController : DialogViewController {
        UIImage image;
    
        public SpiffyDialogViewController (RootElement root, bool pushing, UIImage image) 
            : base (root, pushing) 
        {
            this.image = image;
        }
    
        public override LoadView ()
        {
            base.LoadView ();
            var color = UIColor.FromPatternImage(image);
            TableView.BackgroundColor = UIColor.Clear;
            ParentViewController.View.BackgroundColor = color;
        }
    }

Another customization point is the following virtual methods in the
DialogViewController:

    public override Source CreateSizingSource (bool unevenRows)

This method should return a subclass of DialogViewController.Source
for cases where your cells are evenly sized, or a subclass of
DialogViewController.SizingSource if your cells are uneven.

You can use this override to capture any of the UITableViewSource
methods.   For example, TweetStation uses this to track when the
user has scrolled to the top and update accordingly the number
of unread tweets.

Editing Cells
-------------

Editing cells is one of those cases where you will need to customize
the UITableView source.  To do this, you need to create a subclass of
DialogViewController and override the CreateSizingSource method to
return instances of custom versions of DialogViewController.Source or
DialogViewController.SizingSource.

In these methods you will need to override three methods:

        bool CanEditRow (UITableView tableView, NSIndexPath indexPath)

        UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)

        void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)

See the DemoEditing.cs sample for an example that shows what these
methods should do.

Image Loading
=============

MonoTouch.Dialog now incorporates TweetStation's image loader.  This
image loader can be used to load images in the background, supports
caching and can notify your code when the image has been loaded.

It will also limit the number of outgoing network connections.

The image loader is implemented in the ImageLoader class, all you need
to do is call the DefaultRequestImage method, you will need to provide
the Uri for the image you want to load, as well as an instance of the
IImageUpdated interface which will be invoked when the image has been
loaded.

The ImageLoader exposes a "Purge()" method that you can call when you
want to release all of the images that are currently cached in memory.
The current code has a cache for 50 images.  If you want to use a
different cache size (for instance, if you are expecting the images to
be too large that 50 images would be too much), you can just create
instances of ImageLoader and pass the number of images you want to
keep in the cache.

Json Syntax
===========

Sample:

      {     
        "title": "Json Sample",
        "sections": [ 
            {
                "header": "Booleans",
                "footer": "Slider or image-based",
		"id": "first-section",
                "elements": [
                    { 
                        "type" : "boolean",
                        "caption" : "Demo of a Boolean",
                        "value"   : true
                    }, {
                        "type": "boolean",
                        "caption" : "Boolean using images",
                        "value"   : false,
                        "on"      : "favorite.png",
                        "off"     : "~/favorited.png"
                    }, {
  	  	      "type": "root",
  	  	      "title": "Tap for nested controller",
  	  	      "sections": [ {
  	  	     	 "header": "Nested view!",
  	  	     	 "elements": [
  	  	     	   {
  	  	     	     "type": "boolean",
  	  	     	     "caption": "Just a boolean",
			     "id": "the-boolean",
  	  	     	     "value": false
  	  	     	   },
  	  	     	   {
  	  	     	     "type": "string",
  	  	     	     "caption": "Welcome to the nested controller"
  	  	     	   }
  	  	     	 ]
  	  	       }
  	  	     ]
  	  	   }
                ]
            }, {
            	  "header": "Entries",
        	  "elements" : [
        	      {
        		  "type": "entry",
        		  "caption": "Username",
        		  "value": "",
        		  "placeholder": "Your account username"
        	      }
        	  ]
            }
        ]
      }
     
Every element in the tree can contain the property "id".  It is
possible at runtime to reference individual sections or elements using
the JsonElement indexer.   Like this:

     var jsonElement = JsonElement.FromFile ("demo.json");
     var firstSection = jsonElement ["first-section"] as Section;
     var theBoolean = jsonElement ["the-boolean"] as BooleanElement


Root Element Syntax
-------------------

The Root element contains the following values:

  * title
  * sections (optional)

The root element can appear inside a section as an element to create a
nested controller.   In that case, the extra property "type" must be
set to "root".

### url ###

If the "url" property is set, if the user taps on this RootElement,
the code will request a file from the specified url and will make the
contents the new information displayed.

You can use this to create extend the user interface from the server
based on what the user taps.

### group ###

If set, this sets the groupname for the root element.   Group names
are used to pick a summary that is displayed as the value of the root
element from one of the nested elements in the element.

This is either the value of a checkbox or the value of a radio
button. 

### radioselected ###

Identifies the radio item that is selected in nested elements

### title ###

If present, it will be the title used for the RootElement

### type ###

Must be set to "root" when this appears in a section (this is used to
nest controllers).

### sections ###

This is a Json array with individual sections

Section Syntax
--------------

The section contains:

   * header (optional)
   * footer (optional)
   * elements array

### header ###

If present, the header text is displayed as a caption of the section.

### footer ###

If present, the footer is displayed at the bottom of the section.

### elements ###

This is an array of elements.  Each element must contain at least one
key, the "type" key that is used to identify the kind of element to
create.  Some of the elements share some common properties like
"caption" and "value".

These are the list of supported elements:

   * string elements (both with and without styling)
   * entry lines (regular or password)
   * boolean values (using switches or images)

String elements can be used as buttons by providing a method to invoke
when the user taps on either the cell or the accessory,

Rendering Elements
------------------

The rendering elements are based on the C# StringElement and
StyledStringElement and they can render information in various ways
and it is possible to render them in various ways.

The simplest elements can be created like this:

	   {
	        "type": "string",
		"caption": "Json Serializer",
	   }

This will show a simple string with all of the defaults: font,
background, text color and decorations.   It is possible to hook up
actions to these elements and make them behave like buttons by
setting the "ontap" property or the "onaccessorytap" properties:

	    {
	        "type":    "string",
		"caption": "View Photos",
		"ontap:    "Acme.PhotoLibrary.ShowPhotos"
	    }

The above will invoke the "ShowPhotos" method in the class
"Acme.PhotoLibrary".  The "onaccessorytap" is similar, but it will
only be invoked if the user taps on the accessory instead of tapping
on the cell.   To enable this, you must also set the accessory:

	    {
	        "type":     "string",
		"caption":  "View Photos",
		"ontap:     "Acme.PhotoLibrary.ShowPhotos",
		"accessory: "detail-disclosure",
		"onaccessorytap": "Acme.PhotoLibrary.ShowStats"
	    }

Rendering elements can display two strings at once, one is the caption
and another is the value.   How these strings are rendered depend on
the style, you can set this using the "style" property.   The default
will show the caption on the left, and the value on the right.   See
the section on style for more details.

Colors are encoded using the '#' symbol followed by hex numbers that
represent the values for the red, green, blue and maybe alpha values.  

The contents can be encoded in short form (3 or 4 hex digits) which
represents either RGB or RGBA values.   Or the long form  (6 or 8
digits) which represent either RGB or RGBA values.

The short version is a shorthand to writing the same hex digit twice.
So the "#1bc" constant is intepreted as red=0x11, green=0xbb and
blue=0xcc.

If the alpha value is not present, the color is opaque.

Some examples:

	    "background": "#f00"
	    "background": "#fa08f880"

### accessory ###

Determines the kind of accessory to be shown in your rendering
element, the possible values are:

   * checkmark
   * detail-disclosure
   * disclosure-indicator

If the value is not present, no accessory is shown

### background ###

The background property sets the background color for the cell.   The
value is either a URL to an image (in this case, the async image
downloader will be invoked and the background will be updated once the
image is downloaded) or it can be a color specified using the color
syntax. 


### caption ###

The main string to be shown on the rendering element.  The font and
color can be customized by setting the "textcolor" and "font"
properties.  The rendering style is determined by the "style" property.

### color and detailcolor ###

The color to be used for the main text or the detailed text.

### detailfont and font ###

The font to use for the caption or the detail text.   The format of a font
specification is the font name followed optionally by a dash and the
point size.  

The following are valid font specifications:

"Helvetica"

"Helvetica-14"

### linebreak ###

Determines how lines are broken up.   

The possible values are:

   * character-wrap
   * clip
   * head-truncation
   * middle-truncation
   * tail-truncation
   * word-wrap

Both character-wrap and word-wrap can be used together with the
"lines" property set to zero to turn the rendering element into a
multi-line element.

### ontap and onaccessorytap ###

These properties must point to a static method name in your
application that takes an object as a paramter.

When you create your hierarchy using the JsonDialog.FromFile or
JsonDialog.FromJson methods you can pass an optional object value.
This object value is then passed to your methods.   You can use this
to pass some context to your static method.   For example:

   class Foo {
       Foo ()
       {
           root = JsonDialog.FromJson (myJson, this);
       }

       static void Callback (object obj)
       {
            Foo myFoo = (Foo) obj;
	    obj.Callback ();
       }
   }

### lines ###

If this is set to zero, it will make the element auto-size depending
on the content of the strings contained.  

For this to work, you must also set the "linebreak" property to
"character-wrap" or "word-wrap".

### style ###

The style determines the kind of cell style that will be used to
render the content and they correspond to the UITableViewCellStyle
enumeration values.   The possible values are:

  * "default": 
  * "value1"
  * "value2"
  * "subtitle": text with a subtitle.

### subtitle ###

The value to use for the subtitle.   This is a shortcut to set the
style to "subtitle" and set the "value" property to a string.   This
does both with a single entry.

### textcolor ###

The color to use for the text.

### value ###

The secondary value to be shown on the rendering element.   The layout
of this is affected by the "style" setting.   The font and color can
be customized by setting the "detailfont" and "detailcolor".

Boolean Elements
----------------

Boolean elements should set the type to "bool", can contain a
"caption" to display and the "value" is set to either true or false.

If the "on" and "off" properties are set, they are assumed to be
images.  The images are resolved relative to the current working
directory in the application.  If you want to reference
bundle-relative files, you can use the "~" as a shortcut to represent
the application bundle directory.  For example "~/favorite.png" will
be the favorite.png that is contained in the bundle file.

For example:

              { 
                  "type" : "boolean",
                  "caption" : "Demo of a Boolean",
                  "value"   : true
              }

	      {
                  "type": "boolean",
                  "caption" : "Boolean using images",
                  "value"   : false,
                  "on"      : "favorite.png",
                  "off"     : "~/favorited.png"
              }

### type ###

Type can be set to either "boolean" or "checkbox".  If set to boolean
it will use a UISlider or images (if both "on" and "off" are set).  If
set to checkbox, it will use a checkbox.

The "group" property can be used to tag a boolean element as belonging
to a particular group.  This is useful if the containing root also has
a "group" property as the root will summarize the results with a count
of all the booleans (or checkboxes) that belong to the same group.

Entry Elements
--------------

You use entry elements to allow the user to enter data.  The type for
entry elements is either "entry" or "password".

The "caption" property is set to the text to show on the right, and
the "value" is set to the initial value to set the entry to.   The
"placeholder" is used to show a hint to the user for empty entries (it
is shown greyed out).

Here are some examples:

	{
		"type": "entry",
		"caption": "Username",
		"value": "",
		"placeholder": "Your account username"
	}, {
		"type": "password",
		"caption": "Password",
		"value": "",
		"placeholder": "You password"
	}, {
		"type": "entry",
		"caption": "Zip Code",
		"value": "01010",
		"placeholder": "your zip code",
		"keyboard": "numbers"
	}, {
		"type": "entry",
		"return-key": "route",
		"caption": "Entry with 'route'",
		"placeholder": "captialization all + no corrections",
		"capitalization": "all",
		"autocorrect": "no"
	}

### autocorrect ###

Determines the auto-correction style to use for the entry.

The possible values are true or false (or the strings "yes" and "no").

### capitalization ###

The capitalization style to use for the entry.  The possible values
are:

   * all
   * none
   * sentences
   * words

### caption ###

The caption to use for the entry

### keyboard ###

The keyboard type to use for data entry.   

The possible values are:

   * ascii
   * decimal
   * default
   * email
   * name
   * numbers
   * numbers-and-punctuation
   * twitter
   * url

### placeholder ###

The hint text that is shown when the entry has an empty value.   

### return-key ###

The label used for the return key.

The possible values are:

   * default
   * done
   * emergencycall
   * go
   * google
   * join
   * next
   * route
   * search
   * send
   * yahoo

### value ###

The initial value for the entry

Radio Elements
--------------

Radio elements have type "radio".   The item that is selected is
picked by the radioselected property on its containing root element.

Additionally, if a value is set for the "group" property, this radio
button belongs to that group.

Date and Time Elements
----------------------

The element types "datetime", "date" and "time" are used to render
dates with times, dates or times.  These elements take as parameters a
caption and a value.  The value can be written in any format supported
by the .NET DateTime.Parse function.

Example:

      	"header": "Dates and Times",
      	"elements": [
       		{
       			"type": "datetime",
       			"caption": "Date and Time",
       			"value": "Sat, 01 Nov 2008 19:35:00 GMT"
       		}, {
       			"type": "date",
       			"caption": "Date",
       			"value": "10/10"
       		}, {
       			"type": "time",
       			"caption": "Time",
       			"value": "11:23"
			}       		
      	]

Html/Web Element
================

You can create a cell that when tapped will embed a UIWebView that
renders the contents of a specified URL, either local or remote using
the "html" type.

The only two properties for this element are "caption" and "url":

	{
		"type": "html",
		"caption": "Miguel's blog",
		"url": "http://tirania.org/blog" 
	}

