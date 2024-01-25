using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

using Foundation;

using Moq;

using NUnit.Framework;

using ObjCRuntime;

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
		Mock<TypeCache> typeCache;
		Mock<AttributeManager> attributeManager;
		Nomenclator nomenclator;

		[SetUp]
		public void SetUp ()
		{
			testType = typeof (NSAnimationDelegate);
			var runtimeAssemblies = Directory.GetFiles (RuntimeEnvironment.GetRuntimeDirectory (), "*.dll");
			var resolver = new PathAssemblyResolver (runtimeAssemblies);
			typeCache = new ();
			attributeManager = new (typeCache.Object);
			nomenclator = new (attributeManager.Object);
		}

		[TestCase ("ComputeAnimationCurve")]
		public void GetDelegateNameNoEventArgsTest (string methodName)
		{
			var method = GetMethod (methodName, testType);
			var attr = new DelegateNameAttribute ("NSAnimationProgress");
			attributeManager.Setup (am => am.GetCustomAttribute<DelegateNameAttribute> (method))
				.Returns (attr);

			Assert.AreEqual ("NSAnimationProgress", nomenclator.GetDelegateName (method));
			attributeManager.Verify ();
		}

		[TestCase ("AnimationDidReachProgressMark")]
		public void GetDelegateNameEventArgsTest (string methodName)
		{
			var method = GetMethod (methodName, testType);
			var attr = new EventArgsAttribute ("NSAnimation");
			attributeManager.Setup (am => am.GetCustomAttribute<DelegateNameAttribute> (method))
				.Returns ((DelegateNameAttribute) null);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			Assert.AreEqual ("NSAnimation", nomenclator.GetDelegateName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateNameEventThrows ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager.Setup (am => am.GetCustomAttribute<DelegateNameAttribute> (method))
				.Returns ((DelegateNameAttribute) null);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns ((EventArgsAttribute) null);
			Assert.Throws<BindingException> (() => nomenclator.GetDelegateName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventNameNoAttribute ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager.Setup (am => am.GetCustomAttribute<EventNameAttribute> (method))
				.Returns ((EventNameAttribute) null);
			Assert.AreEqual ("DidAccelerate", nomenclator.GetEventName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEnventNameAttribute ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			string eventName = "DidAccelerateEventRaised";
			var attr = new EventNameAttribute (eventName);
			attributeManager.Setup (am => am.GetCustomAttribute<EventNameAttribute> (method))
				.Returns (attr);
			Assert.AreEqual (eventName, nomenclator.GetEventName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateApiName ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			var attr = new DelegateApiNameAttribute ("TestFramework");
			attributeManager.Setup (am => am.GetCustomAttribute<DelegateApiNameAttribute> (method))
				.Returns (attr);
			Assert.AreEqual ("TestFramework", nomenclator.GetDelegateApiName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateApiNameMissingAttr ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager.Setup (am => am.GetCustomAttribute<DelegateApiNameAttribute> (method))
				.Returns ((DelegateApiNameAttribute) null);
			Assert.AreEqual ("DidAccelerate", nomenclator.GetDelegateApiName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetDelegateApiNameDuplicate ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager.Setup (am => am.GetCustomAttribute<DelegateApiNameAttribute> (method))
				.Returns ((DelegateApiNameAttribute) null);
			Assert.AreEqual ("DidAccelerate", nomenclator.GetDelegateApiName (method));
			Assert.Throws<BindingException> (() => nomenclator.GetDelegateApiName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventArgNameSingleParamTest ()
		{
			var method = GetMethod ("DidAccelerateSingle", testType);
			Assert.AreEqual ("EventArgs", nomenclator.GetEventArgName (method));
		}

		[Test]
		public void GetEventArgsNameSeveralParamsNoAttr ()
		{
			var method = GetMethod ("DidAccelerate", testType);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns ((EventArgsAttribute) null);
			Assert.Throws<BindingException> (() => nomenclator.GetEventArgName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventArgsSkipGenerationEndWithEventArgs ()
		{
			var method = GetMethod ("DidAccelerateSeveral", testType);
			var attr = new EventArgsAttribute ("ThisIsATestEventArgs");
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			Assert.Throws<BindingException> (() => nomenclator.GetEventArgName (method));
			attributeManager.Verify ();
		}

		[Test]
		public void GetEventArgsSkipGeneration ()
		{
			var method = GetMethod ("DidAccelerateSeveral", testType);
			var attr = new EventArgsAttribute ("ThisIsATest", true);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			var name = nomenclator.GetEventArgName (method);
			Assert.AreEqual ("ThisIsATestEventArgs", name, "name");
			Assert.True (nomenclator.WasEventArgGenerated (name), "was generated");
		}

		[Test]
		public void GetEventArgsFullName ()
		{
			var method = GetMethod ("DidAccelerateSeveral", testType);
			var attr = new EventArgsAttribute ("ThisIsATest", false, true);
			attributeManager.Setup (am => am.GetCustomAttribute<EventArgsAttribute> (method))
				.Returns (attr);
			var name = nomenclator.GetEventArgName (method);
			Assert.AreEqual ("ThisIsATest", name, "name");
			Assert.False (nomenclator.WasEventArgGenerated (name), "was generated");
		}

		[Test]
		public void GetTrampolineNameNotGeneric ()
			=> Assert.AreEqual ("NSAnimationDelegate", nomenclator.GetTrampolineName (testType));

		[Test]
		public void GetTrampolineNameGeneric ()
		{
			var name1 = nomenclator.GetTrampolineName (typeof (GenericTrampoline<string>));
			var name2 = nomenclator.GetTrampolineName (typeof (GenericTrampoline<object>));
			Assert.AreEqual ("GenericTrampolineArity1V0", name1, "name1");
			Assert.AreEqual ("GenericTrampolineArity1V1", name2, "name2");
			Assert.AreNotEqual (name1, name2, "equal");
		}

		[Test]
		public void GetGeneratedTypeNameType ()
		{
			attributeManager.Setup (am => am.GetCustomAttributes<BindAttribute> (It.IsAny<Type> ()))
				.Returns (Array.Empty<BindAttribute> ());
			Assert.AreEqual ("NSAnimationDelegate", nomenclator.GetGeneratedTypeName (typeof (NSAnimationDelegate)));
		}

		[Test]
		public void GetGeneratedTypeNameGenericType ()
		{
			attributeManager.Setup (am => am.GetCustomAttributes<BindAttribute> (It.IsAny<Type> ()))
				.Returns (Array.Empty<BindAttribute> ());
			Assert.AreEqual ("GenericTrampoline", nomenclator.GetGeneratedTypeName (typeof (GenericTrampoline<string>).GetGenericTypeDefinition ()));
		}

		[Test]
		public void GetGeneratedTypeNameBindAttribute ()
		{
			var selectorName = "selectorName";
			var attr = new BindAttribute (selectorName);
			attributeManager.Setup (am => am.GetCustomAttributes<BindAttribute> (It.IsAny<Type> ()))
				.Returns (new [] { attr });
			Assert.AreEqual (selectorName, nomenclator.GetGeneratedTypeName (typeof (NSAnimationDelegate)));
		}
	}
}
