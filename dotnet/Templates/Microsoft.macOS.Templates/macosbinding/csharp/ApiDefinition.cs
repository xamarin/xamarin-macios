namespace macOSBinding1 {

	// The first step to creating a binding is to add your native framework ("MyLibrary.xcframework")
	// to the project.
	// Open your binding csproj and add a section like this
	// <ItemGroup>
	//   <NativeReference Include="MyLibrary.xcframework">
	//     <Kind>Framework</Kind>
	//     <Frameworks></Frameworks>
	//   </NativeReference>
	// </ItemGroup>
	//
	// Once you've added it, you will need to customize it for your specific library:
	//  - Change the Include to the correct path/name of your library
	//  - Change Kind to Static (.a) or Framework (.framework/.xcframework) based upon the library kind and extension.
	//    - Dynamic (.dylib) is a third option but rarely if ever valid, and only on macOS and Mac Catalyst
	//  - If your library depends on other frameworks, add them inside <Frameworks></Frameworks>
	// Example:
	// <NativeReference Include="libs\MyTestFramework.xcframework">
	//   <Kind>Framework</Kind>
	//   <Frameworks>CoreLocation ModelIO</Frameworks>
	// </NativeReference>
	// 
	// Once you've done that, you're ready to move on to binding the API...
	//
	// Here is where you'd define your API definition for the native Objective-C library.
	//
	// For example, to bind the following Objective-C class:
	//
	//     @interface Widget : NSObject {
	//     }
	//
	// The C# binding would look like this:
	//
	//     [BaseType (typeof (NSObject))]
	//     interface Widget {
	//     }
	//
	// To bind Objective-C properties, such as:
	//
	//     @property (nonatomic, readwrite, assign) CGPoint center;
	//
	// You would add a property definition in the C# interface like so:
	//
	//     [Export ("center")]
	//     CGPoint Center { get; set; }
	//
	// To bind an Objective-C method, such as:
	//
	//     -(void) doSomething:(NSObject *)object atIndex:(NSInteger)index;
	//
	// You would add a method definition to the C# interface like so:
	//
	//     [Export ("doSomething:atIndex:")]
	//     void DoSomething (NSObject object, nint index);
	//
	// Objective-C "constructors" such as:
	//
	//     -(id)initWithElmo:(ElmoMuppet *)elmo;
	//
	// Can be bound as:
	//
	//     [Export ("initWithElmo:")]
	//     NativeHandle Constructor (ElmoMuppet elmo);
	//
	// For more information, see https://aka.ms/ios-binding
	//

}
