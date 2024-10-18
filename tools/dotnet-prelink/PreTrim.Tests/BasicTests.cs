using Mono.Linker;
using Mono.Linker.Steps;
using Xamarin.Linker.Steps;

namespace PreTrim.Tests;

public class BasicTests {
	[Fact]
	public void MarkAllTypesAndMembers ()
	{
		string currentAssemblyPath = Path.GetDirectoryName (typeof (BasicTests).Assembly.Location)!;
		string basicAssemblyPath = Path.Combine (currentAssemblyPath, "Basic.dll");
		string outputFolder = Path.Combine (Path.GetTempPath (), nameof (MarkAllTypesAndMembers));
		Console.WriteLine(outputFolder);
		CustomStepHostDriver driver = new (CustomStepHostDriver.ConsoleLogger.Instance, outputFolder);
		driver.AddAssembly (basicAssemblyPath);
		driver.AddStep (new BasicStep ());
		driver.AddStep (new OutputStep ());
		driver.Run ();
		const string expectedDescriptorContents = $$$"""
			<linker>
				<assembly fullname="Basic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
					<type fullname="&lt;Module&gt;" preserve="nothing" required="true" />
					<type fullname="Program" preserve="nothing" required="true">
						<method signature="System.Void &lt;Main&gt;$(System.String[])" required="true" />
						<method signature="System.Void .ctor()" required="true" />
					</type>
					<type fullname="BasicClass" preserve="nothing" required="true">
						<method signature="System.Int32 BasicMethod()" required="true" />
						<method signature="System.Int32 get_BasicProperty()" required="true" />
						<method signature="System.Void set_BasicProperty(System.Int32)" required="true" />
						<method signature="System.Void add_BasicEvent(System.EventHandler)" required="true" />
						<method signature="System.Void remove_BasicEvent(System.EventHandler)" required="true" />
						<method signature="System.Void .ctor()" required="true" />
						<property signature="System.Int32 BasicProperty" required="true" />
						<field signature="System.Int32 &lt;BasicProperty&gt;k__BackingField" required="true" />
						<field signature="System.EventHandler BasicEvent" required="true" />
						<field signature="System.Int32 BasicField" required="true" />
						<event signature="System.EventHandler BasicEvent" required="true" />
					</type>
					<type fullname="BasicClass/BasicNestedClass" preserve="nothing" required="true">
						<method signature="System.Void .ctor()" required="true" />
					</type>
				</assembly>
			</linker>
			""";
		string descriptorPath = Path.Combine (outputFolder, "ILLink.Descriptors.xml");
		string actualDescriptorContents = File.ReadAllText (descriptorPath);
		Assert.Equal (expectedDescriptorContents, actualDescriptorContents);
	}

	[Fact]
	public void SDInnerBlockIsKept()
	{
		// assembly with SDInnerBlock and the fields has them kept via the descriptors file
		string currentAssemblyPath = Path.GetDirectoryName (typeof (BasicTests).Assembly.Location)!;
		string basicAssemblyPath = Path.Combine (currentAssemblyPath, "SDInnerBlock.dll");
		string outputFolder = Path.Combine (Path.GetTempPath (), nameof (SDInnerBlockIsKept));
		Console.WriteLine(outputFolder);
		CustomStepHostDriver driver = new (CustomStepHostDriver.ConsoleLogger.Instance, outputFolder);
		driver.AddAssembly (basicAssemblyPath);
		driver.AddStep (new PreserveBlockCodeHandler ());
		driver.AddStep (new OutputStep ());
		driver.Run ();
		const string expectedDescriptorContents = $$$"""
			<linker>
				<assembly fullname="SDInnerBlock, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
					<type fullname="ObjCRuntime.Trampolines/SDInnerBlock" preserve="nothing" required="false">
						<method signature="System.Void Invoke(System.IntPtr,System.Int32)" required="false" />
						<field signature="ObjCRuntime.Trampolines/SDInnerBlock/DInnerBlock Handler" required="false" />
					</type>
				</assembly>
			</linker>
			""";
		string descriptorPath = Path.Combine (outputFolder, "ILLink.Descriptors.xml");
		string actualDescriptorContents = File.ReadAllText (descriptorPath);
		Assert.Equal (expectedDescriptorContents, actualDescriptorContents);
	}
}
