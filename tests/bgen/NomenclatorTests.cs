using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using Moq;
using Xamarin.Bundler;

namespace GeneratorTests {
	[TestFixture]
	public class NomenclatorTests : ReflectionTest {

		interface NSAnimationDelegate {
			[Export ("animation:valueForProgress:"), DelegateName ("NSAnimationProgress"),
			 DefaultValueFromArgumentAttribute ("progress")]
			float ComputeAnimationCurve (object animation, float progress);

			[Export ("animation:didReachProgressMark:"), EventArgs ("NSAnimation")]
			void AnimationDidReachProgressMark (object animation, float progress);

			[Export ("accelerometer:didAccelerate:"), EventArgs ("UIAccelerometer"), EventName ("Acceleration")]
			void DidAccelerate (object accelerometer, object acceleration);

			[Export ("accelerometer:")]
			void DidAccelerateSingle (object accelerometer);

			[Export ("accelerometer:")]
			void DidAccelerateSeveral (object accelerometer, object second, object last);
		}

		interface GenericTrampoline<T> where T : class {
			[Export ("accelerometer:")]
			void DidAccelerateSeveral (object accelerometer, object second, object last);
		}

		Type testType = typeof (object);
		Mock<TypeCache>? typeCache;
		Mock<AttributeManager>? attributeManager;
		Nomenclator? nomenclator;

		[SetUp]
		public void SetUp ()
		{
			testType = typeof (NSAnimationDelegate);
			var runtimeAssemblies = Directory.GetFiles (RuntimeEnvironment.GetRuntimeDirectory (), "*.dll");
			var resolver = new PathAssemblyResolver (runtimeAssemblies);
			typeCache = new ();
			attributeManager = new (new Mock<IToolLog> ().Object, typeCache.Object);
			nomenclator = new (attributeManager.Object);
		}

		[TestCase ("ComputeAnimationCurve")]
		public void GetDelegateNameNoEventArgsTest (string methodName)
		{
			var method = GetMethod (methodName, testType);
			var attr = new DelegateNameAttribute ("NSAnimationProgress");
			attributeManager!.Setup (am => am.GetCustomAttribute<DelegateNameAttribute> (method))
				.Returns (attr);

			Assert.That (nomenclator!.GetDelegateName (method), Is.EqualTo ("NSAnimationProgress"));
			attributeManager.Verify ();
		}

		[TestCase ("AnimationDidReachProgressMark")]
		public void GetDelegateNameEventArgsTest (string methodName)
		{
			var method = GetMethod (methodName, testType);
			var attr = new EventArgsAttribute ("NSAnimation");
			attributeManager!.Setup (am => am.GetCustomAttribute<DelegateNameAttribute> (method))
				.Returns ((DelegateNameAttribute?) null);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			Assert.That (nomenclator!.GetDelegateName (method), Is.EqualTo ("NSAnimation"));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateNameEventThrows ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager!.Setup (am => am.GetCustomAttribute<DelegateNameAttribute> (method))
				.Returns ((DelegateNameAttribute?) null);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns ((EventArgsAttribute?) null);
			Assert.Throws<BindingException> (() => nomenclator!.GetDelegateName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventNameNoAttribute ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager!.Setup (am => am.GetCustomAttribute<EventNameAttribute> (method))
				.Returns ((EventNameAttribute?) null);
			Assert.That (nomenclator!.GetEventName (method), Is.EqualTo ("DidAccelerate"));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEnventNameAttribute ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			string eventName = "DidAccelerateEventRaised";
			var attr = new EventNameAttribute (eventName);
			attributeManager!.Setup (am => am.GetCustomAttribute<EventNameAttribute> (method))
				.Returns (attr);
			Assert.That (nomenclator!.GetEventName (method), Is.EqualTo (eventName));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateApiName ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			var attr = new DelegateApiNameAttribute ("TestFramework");
			attributeManager!.Setup (am => am.GetCustomAttribute<DelegateApiNameAttribute> (method))
				.Returns (attr);
			Assert.That (nomenclator!.GetDelegateApiName (method), Is.EqualTo ("TestFramework"));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateApiNameMissingAttr ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager!.Setup (am => am.GetCustomAttribute<DelegateApiNameAttribute> (method))
				.Returns ((DelegateApiNameAttribute?) null);
			Assert.That (nomenclator!.GetDelegateApiName (method), Is.EqualTo ("DidAccelerate"));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateApiNameDuplicate ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager!.Setup (am => am.GetCustomAttribute<DelegateApiNameAttribute> (method))
				.Returns ((DelegateApiNameAttribute?) null);
			Assert.That (nomenclator!.GetDelegateApiName (method), Is.EqualTo ("DidAccelerate"));
			Assert.Throws<BindingException> (() => nomenclator.GetDelegateApiName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventArgNameSingleParamTest ()
		{
			var method = GetMethod ("DidAccelerateSingle", testType);
			Assert.That (nomenclator!.GetEventArgName (method), Is.EqualTo ("EventArgs"));
		}

		[Test]
		public void GetEventArgsNameSeveralParamsNoAttr ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager!.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns ((EventArgsAttribute?) null);
			Assert.Throws<BindingException> (() => nomenclator!.GetEventArgName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventArgsSkipGenerationEndWithEventArgs ()
		{
			var method = GetMethod ("DidAccelerateSeveral", testType);
			var attr = new EventArgsAttribute ("ThisIsATestEventArgs");
			attributeManager!.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			Assert.Throws<BindingException> (() => nomenclator!.GetEventArgName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventArgsSkipGeneration ()
		{
			var method = GetMethod ("DidAccelerateSeveral", testType);
			var attr = new EventArgsAttribute ("ThisIsATest", true);
			attributeManager!.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			var name = nomenclator!.GetEventArgName (method);
			Assert.That (name, Is.EqualTo ("ThisIsATestEventArgs"), "name");
			Assert.That (nomenclator.WasEventArgGenerated (name), Is.True, "was generated");
		}

		[Test]
		public void GetEventArgsFullName ()
		{
			var method = GetMethod ("DidAccelerateSeveral", testType);
			var attr = new EventArgsAttribute ("ThisIsATest", false, true);
			attributeManager!.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			var name = nomenclator!.GetEventArgName (method);
			Assert.That (name, Is.EqualTo ("ThisIsATest"), "name");
			Assert.That (nomenclator.WasEventArgGenerated (name), Is.False, "was generated");
		}

		[Test]
		public void GetTrampolineNameNotGeneric ()
			=> Assert.That (nomenclator!.GetTrampolineName (testType), Is.EqualTo ("NSAnimationDelegate"));

		[Test]
		public void GetTrampolineNameGeneric ()
		{
			var name1 = nomenclator!.GetTrampolineName (typeof (GenericTrampoline<string>));
			var name2 = nomenclator!.GetTrampolineName (typeof (GenericTrampoline<object>));
			Assert.That (name1, Is.EqualTo ("GenericTrampolineArity1V0"), "name1");
			Assert.That (name2, Is.EqualTo ("GenericTrampolineArity1V1"), "name2");
			Assert.That (name2, Is.Not.EqualTo (name1), "equal");
		}

		[Test]
		public void GetGeneratedTypeNameType ()
		{
			attributeManager!.Setup (am => am.GetCustomAttributes<BindAttribute> (It.IsAny<Type> ()))
				.Returns (Array.Empty<BindAttribute> ());
			Assert.That (nomenclator!.GetGeneratedTypeName (typeof (NSAnimationDelegate)), Is.EqualTo ("NSAnimationDelegate"));
		}

		[Test]
		public void GetGeneratedTypeNameGenericType ()
		{
			attributeManager!.Setup (am => am.GetCustomAttributes<BindAttribute> (It.IsAny<Type> ()))
				.Returns (Array.Empty<BindAttribute> ());
			Assert.That (nomenclator!.GetGeneratedTypeName (typeof (GenericTrampoline<string>).GetGenericTypeDefinition ()), Is.EqualTo ("GenericTrampoline"));
		}

		[Test]
		public void GetGeneratedTypeNameBindAttribute ()
		{
			var selectorName = "selectorName";
			var attr = new BindAttribute (selectorName);
			attributeManager!.Setup (am => am.GetCustomAttributes<BindAttribute> (It.IsAny<Type> ()))
				.Returns (new [] { attr });
			Assert.That (nomenclator!.GetGeneratedTypeName (typeof (NSAnimationDelegate)), Is.EqualTo (selectorName));
		}
	}
}
