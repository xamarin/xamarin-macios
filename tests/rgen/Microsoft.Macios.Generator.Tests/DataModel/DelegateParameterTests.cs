using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class DelegateParameterTests {

	class TestDataEquals : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// diff pos
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 1,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff type
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "int",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff name
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg2",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff blittable 
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: true
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff optional
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = true,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff is params
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = true,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff is this
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = true,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff is nullable
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = true,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff is smart enum
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = true,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff is array
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = true,
					ReferenceKind = ReferenceKind.None,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];

			// diff ref type
			yield return [
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.In,
				},
				new DelegateParameter (
					position: 0,
					type: "string",
					name: "arg1",
					isBlittable: false
				) {
					IsOptional = false,
					IsParams = false,
					IsThis = false,
					IsNullable = false,
					IsSmartEnum = false,
					IsArray = false,
					ReferenceKind = ReferenceKind.None,
				},
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataEquals))]
	void CompareDiffPosition (DelegateParameter x, DelegateParameter y)
	{
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

}
