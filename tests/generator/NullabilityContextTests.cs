using System;
using System.Collections.Generic;
using bgen;
using NUnit.Framework;

#nullable enable

namespace GeneratorTests {
	
	[TestFixture]
	[Parallelizable (ParallelScope.All)]
	public class NullabilityContextTests {

		class ObjectTest {
			public int NotNullableValueTypeField = 0;
			public string NotNullableRefTypeField = string.Empty;
			public int? NullableValueTypeField = 0;
			public string? NullableRefTypeField = string.Empty;
			
			public int NotNullableValueTypeProperty { get; set; }
			public string NotNullableRefProperty { get; set; } = string.Empty;
			public int? NullableValueTypeProperty { get; set; }
			public string? NullableRefProperty { get; set; }
			
			public void NotNullableValueTypeParameter (int myParam) { }
			public void NotNullableRefParameter (string myParam) { }
			
			public void NullableValueTypeParameter (int? myParam) { }
			public void NullableRefTypeParameter (string? myParam) { }

			public int NotNullableValueTypeReturn () => 0;
			public string NotNullableRefTypeReturn () => string.Empty;

			public int? NullableValueTypeReturn () => null;
			public string? NullableRefTypeReturn () => null;
			
			// array return type tests
			public int [] NotNullableValueTypeArray () => Array.Empty<int> ();
			public int? [] NotNullableNullableValueTypeArray () => Array.Empty<int?> ();
			public int []? NullableValueTypeArray () => null;
			public int? []? NullableNullableValueTypeArray () => null;
			
			public string [] NotNullableRefTypeArray () => Array.Empty<string> ();
			public string? [] NotNullableNullableRefTypeArray () => Array.Empty<string?> ();
			public string []? NullableRefTypeArray () => null;
			public string? []? NullableNullableRefTypeArray () => null;

			public int [] [] NotNullableNestedArrayNotNullableValueType () => Array.Empty<int []> ();
			public int? [] [] NotNullableNestedArrayNullableValueType () => Array.Empty<int? []> ();
			public int? []? [] NotNullableNestedNullableArrayNullableValueType () => Array.Empty<int? []?> ();
			public int? []? []? NullableNestedNullableArrayNullableValueType () => null;
			
			
			public string [] [] NotNullableNestedArrayNotNullableRefType () => Array.Empty<string []> ();
			public string? [] [] NotNullableNestedArrayNullableRefType () => Array.Empty<string? []> ();
			public string? []? [] NotNullableNestedNullableArrayNullableRefType () => Array.Empty<string? []?> ();
			public string? []? []? NullableNestedNullableArrayNullableRefType () => null;

			public List<int> NotNullableValueTypeList () => new();
			public List<int?> NullableValueTypeList () => new();
			public List<int?>? NullableListNullableValueType () => null;
			
			public List<string> NotNullableRefTypeList () => new();
			public List<string?> NullableRefTypeList () => new();
			public List<string?>? NullableListNullableRefType () => null;

			public List<HashSet<int>> NestedGenericNotNullableValueType () => new();
			public List<HashSet<int?>> NestedGenericNullableValueType () => new();
			public List<HashSet<int?>?> NestedNullableGenericNullableValueType () => new();
			public List<HashSet<int?>?>? NullableNestedNullableGenericNullableValueType () => new();
			
			public List<HashSet<string>> NestedGenericNotNullableRefType () => new();
			public List<HashSet<string?>> NestedGenericNullableRefType () => new();
			public List<HashSet<string?>?> NestedNullableGenericNullableRefType () => new();
			public List<HashSet<string?>?>? NullableNestedNullableGenericNullableRefType () => new();

			public Dictionary<string, int> DictionaryStringInt () => new();
			public Dictionary<string, int?> DictionaryStringNullableInt () => new ();
			public Dictionary<string?, int?> DictionaryNullableStringNullableInt () => new();
			public Dictionary<string?, int?>? NullableDictionaryNullableStringNullableInt () => new ();
		}

		Type testType = typeof(object);

		[SetUp]
		public void SetUp ()
		{
			testType = typeof (ObjectTest);
		}
		
		[TestCase ("NotNullableValueTypeField", typeof(int))]
		[TestCase ("NotNullableRefTypeField", typeof(string))]
		public void NotNullableFieldTest (string fieldName, Type expectedType)
		{
			var fieldInfo = testType.GetField (fieldName);
			Assert.NotNull (fieldInfo, "fieldInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (fieldInfo);
			Assert.False (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}

		[TestCase ("NullableValueTypeField", typeof(int))]
		[TestCase ("NullableRefTypeField", typeof(string))]
		public void NullableFieldTest (string fieldName, Type expectedType)
		{
			var fieldInfo = testType.GetField (fieldName);
			Assert.NotNull (fieldInfo);
			var info = GeneratorNullabilityInfoContext.Create (fieldInfo); 
			Assert.True (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}
		
		[TestCase ("NotNullableValueTypeProperty", typeof(int))]
		[TestCase ("NotNullableRefProperty", typeof(string))]
		public void NotNullablePropertyTest (string propertyName, Type expectedType)
		{
			var propertyInfo = testType.GetProperty (propertyName);
			Assert.NotNull (propertyInfo, "propertyInto != null");
			var info = GeneratorNullabilityInfoContext.Create (propertyInfo);
			Assert.False (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}

		[TestCase ("NullableValueTypeProperty", typeof(int))]
		[TestCase ("NullableRefProperty", typeof(string))]
		public void NullablePropertyTest (string propertyName, Type expectedType)
		{
			var propertyInfo = testType.GetProperty (propertyName);
			Assert.NotNull (propertyInfo, "propertyInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (propertyInfo);
			Assert.True (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}
		
		[TestCase ("NotNullableValueTypeParameter", typeof(int))]
		[TestCase ("NotNullableRefParameter", typeof(string))]
		public void NotNullableParameterTest (string methodName, Type expectedType)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var paramInfo = memberInfo.GetParameters () [0];
			var info = GeneratorNullabilityInfoContext.Create (paramInfo);
			Assert.False (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}

		[TestCase ("NullableValueTypeParameter", typeof(int))]
		[TestCase ("NullableRefTypeParameter", typeof(string))]
		public void NullableParameterTest (string methdName, Type expectedType)
		{
			var memberInfo = testType.GetMethod(methdName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var paramInfo = memberInfo.GetParameters () [0];
			var info = GeneratorNullabilityInfoContext.Create (paramInfo);
			Assert.True (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}
		
		[TestCase ("NotNullableValueTypeReturn", typeof(int))]
		[TestCase ("NotNullableRefTypeReturn", typeof(string))]
		public void NotNullableReturnTypeTest (string methodName, Type expectedType)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.IsFalse (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}

		[TestCase ("NullableValueTypeReturn", typeof(int))]
		[TestCase ("NullableRefTypeReturn", typeof(string))]
		public void NullableReturnTypeTest (string methodName, Type expectedType)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.IsTrue (info.IsNullable, "isNullable");
			Assert.AreEqual (expectedType, info.Type);
		}

		[TestCase("NotNullableValueTypeArray", typeof(int[]), false, typeof(int), false)]
		[TestCase("NotNullableNullableValueTypeArray", typeof(int[]), false, typeof(int), true)]
		[TestCase("NullableValueTypeArray", typeof(int[]), true, typeof(int), false)]
		[TestCase("NullableNullableValueTypeArray", typeof(int[]), true, typeof(int), true)]
		[TestCase("NotNullableRefTypeArray", typeof(string[]), false, typeof(string), false)]
		[TestCase("NotNullableNullableRefTypeArray", typeof(string[]), false, typeof(string), true)]
		public void ReturnTypeArrayTests (string methodName, Type? expectedType, bool arrayIsNullable, Type expectedElementType, bool elementTypeIsNullable)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.AreEqual (arrayIsNullable, info.IsNullable, "info.IsNullable");
			Assert.AreEqual (expectedElementType, info.ElementType!.Type, "info.ElementType.Type");
			Assert.AreEqual (elementTypeIsNullable, info.ElementType.IsNullable, "info.ElementTyps.IsNullable");
		}
	
		[TestCase("NotNullableNestedArrayNotNullableValueType", typeof(int[][]), false, false, typeof(int), false)]
		[TestCase("NotNullableNestedArrayNullableValueType", typeof(int?[][]), false, false, typeof(int), true)]
		[TestCase("NotNullableNestedNullableArrayNullableValueType", typeof(int?[][]), false, true, typeof(int), true)]
		[TestCase("NullableNestedNullableArrayNullableValueType", typeof(int?[][]), true, true, typeof(int), true)]
		[TestCase("NotNullableNestedArrayNotNullableRefType", typeof(string[][]), false, false, typeof(string), false)]
		[TestCase("NotNullableNestedArrayNullableRefType", typeof(string?[][]), false, false, typeof(string), true)]
		[TestCase("NotNullableNestedNullableArrayNullableRefType", typeof(string?[][]), false, true, typeof(string), true)]
		[TestCase("NullableNestedNullableArrayNullableRefType", typeof(string?[][]), true, true, typeof(string), true)]
		public void ReturnNestedArrayTests (string methodName, Type? expectedType, bool isArrayNullable,
			bool isNestedArrayNullable, Type nestedArrayType, bool isNestedArrayElementNullable)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.AreEqual (isArrayNullable, info.IsNullable, "isArrayNullable");
			Assert.AreEqual (isNestedArrayNullable, info.ElementType!.IsNullable, "isNestedArrayNullable");
			Assert.AreEqual (nestedArrayType, info.ElementType.ElementType!.Type, "nestedArrayType");
			Assert.AreEqual (isNestedArrayElementNullable, info.ElementType.ElementType.IsNullable, "isNestedArrayElementNullable");
		}
		
		[TestCase("NotNullableValueTypeList", typeof(List<int>), false, typeof(int), false)]
		[TestCase("NullableValueTypeList", typeof(List<Nullable<int>>), false, typeof(int), true)]
		[TestCase("NullableListNullableValueType", typeof(List<Nullable<int>>), true, typeof(int), true)]
		[TestCase("NotNullableRefTypeList", typeof(List<string>), false, typeof(string), false)]
		[TestCase("NullableRefTypeList", typeof(List<string>), false, typeof(string), true)]
		[TestCase("NullableListNullableRefType", typeof(List<string>), true, typeof(string), true)]
		public void ReturnSimpleGenericType (string methodName, Type? expectedType, bool isGenericTypeNullable,
			Type? genericParameterType, bool isGenericParameterNull)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.AreEqual (expectedType, info.Type, "info.Type");
			Assert.AreEqual (isGenericTypeNullable, info.IsNullable, "info.IsNullable");
			Assert.AreEqual (genericParameterType, info.GenericTypeArguments![0].Type, "genericParameterType");
			Assert.AreEqual (isGenericParameterNull, info.GenericTypeArguments[0].IsNullable, "isGenericParameterNull");
		}
		
		[TestCase ("NestedGenericNotNullableValueType", typeof(List<HashSet<int>>), false, typeof(HashSet<int>), false, typeof(int), false)]
		[TestCase ("NestedGenericNullableValueType", typeof(List<HashSet<Nullable<int>>>), false, typeof(HashSet<Nullable<int>>), false, typeof(int), true)]
		[TestCase ("NestedNullableGenericNullableValueType", typeof(List<HashSet<Nullable<int>>>), false, typeof(HashSet<Nullable<int>>), true, typeof(int), true)]
		[TestCase ("NullableNestedNullableGenericNullableValueType", typeof(List<HashSet<Nullable<int>>>), true, typeof(HashSet<Nullable<int>>), true, typeof(int), true)]
		[TestCase ("NestedGenericNotNullableRefType", typeof(List<HashSet<string>>), false, typeof(HashSet<string>), false, typeof(string), false)]
		[TestCase ("NestedGenericNullableRefType", typeof(List<HashSet<string>>), false, typeof(HashSet<string>), false, typeof(string), true)]
		[TestCase ("NestedNullableGenericNullableRefType", typeof(List<HashSet<string>>), false, typeof(HashSet<string>), true, typeof(string), true)]
		[TestCase ("NullableNestedNullableGenericNullableRefType", typeof(List<HashSet<string>>), true, typeof(HashSet<string>), true, typeof(string), true)]
		public void ReturnNestedGeneric (string methodName, Type? expectedType, bool isGenericNullable,
			Type? nestedGenericType, bool isNestedGenericNullable, Type? nestedGenericArgumentType,
			bool isNullableNestedGenericArgument)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.AreEqual (expectedType, info.Type, "info.Type");
			Assert.AreEqual (isGenericNullable, info.IsNullable, "info.IsNullable");
			Assert.AreEqual (nestedGenericType, info.GenericTypeArguments![0].Type, "nestedGenericType");
			Assert.AreEqual (isNestedGenericNullable, info.GenericTypeArguments[0].IsNullable, "isNestedGenericNullable");
			Assert.AreEqual (nestedGenericArgumentType, info.GenericTypeArguments[0].GenericTypeArguments![0].Type, "nestedGenericArgumentType");
			Assert.AreEqual (isNullableNestedGenericArgument, info.GenericTypeArguments[0].GenericTypeArguments![0].IsNullable, "isNullableNestedGenericArgument");
		}
		
		[TestCase("DictionaryStringInt", typeof(Dictionary<string, int>), false, typeof(string), false, typeof(int), false)]
		[TestCase("DictionaryStringNullableInt", typeof(Dictionary<string, Nullable<int>>), false, typeof(string), false, typeof(int), true)]
		[TestCase("DictionaryNullableStringNullableInt", typeof(Dictionary<string, Nullable<int>>), false, typeof(string), true, typeof(int), true)]
		[TestCase("NullableDictionaryNullableStringNullableInt", typeof(Dictionary<string, Nullable<int>>), true, typeof(string), true, typeof(int), true)]
		public void ReturnDictionary (string methodName, Type? dictionaryType, bool isDictionaryNullable, Type? keyType,
			bool isKeyNullable, Type? valueType, bool isValueNullable)
		{
			var memberInfo = testType.GetMethod(methodName);
			Assert.NotNull (memberInfo, "memberInfo != null");
			var info = GeneratorNullabilityInfoContext.Create (memberInfo.ReturnParameter);
			Assert.AreEqual (dictionaryType, info.Type, "info.Type");
			Assert.AreEqual (isDictionaryNullable, info.IsNullable, "info.IsNullable");
			Assert.AreEqual (keyType, info.GenericTypeArguments![0].Type, "keyType");
			Assert.AreEqual (isKeyNullable, info.GenericTypeArguments[0].IsNullable, "isKeyNullable");
			Assert.AreEqual (valueType, info.GenericTypeArguments[1].Type, "valueType");
			Assert.AreEqual (isValueNullable, info.GenericTypeArguments[1].IsNullable, "isValueNullable");
		}
	}
}
