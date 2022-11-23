//
// Unit tests for MDLAnimatedValueTypes
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Linq;
using CoreGraphics;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
using NUnit.Framework;
using System.Runtime.InteropServices;

#if NET
using System.Numerics;
using Vector2d = global::CoreGraphics.NVector2d;
using Vector3d = global::CoreGraphics.NVector3d;
using Vector4d = global::CoreGraphics.NVector4d;
using Matrix4d = global::CoreGraphics.NMatrix4d;
using Quaterniond = global::CoreGraphics.NQuaterniond;
using Matrix4 = global::System.Numerics.Matrix4x4;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.ModelIO {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class A_MDLAnimatedValueTypesTests {

		[OneTimeSetUp]
		public void Setup ()
		{
#if MONOMAC
			// ModelIO seems to be broken in Xcode 12.2 Beta 3 so disabling for now.
			if (TestRuntime.CheckExactXcodeVersion (12, 2, beta: 3))
				Assert.Inconclusive ("ModelIO is not working in Xcode 12.2 Beta 3");
#endif
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void MDLAnimatedScalarTest ()
		{
			// float

			var floatScalar = new MDLAnimatedScalar ();
			for (int i = 0; i < TestMatrices.FloatArray.Length; i++)
				floatScalar.SetValue (TestMatrices.FloatArray [i], i);

			for (int i = 0; i < TestMatrices.FloatArray.Length; i++)
				Asserts.AreEqual (TestMatrices.FloatArray [i], floatScalar.GetFloat (i), $"floatScalar iter: {i}");

			var allFloatValues = floatScalar.GetFloatValues ();
			for (int i = 0; i < allFloatValues.Length; i++)
				Asserts.AreEqual (TestMatrices.FloatArray [i], allFloatValues [i], $"allFloatValues iter: {i}");

			var onesFloatArr = TestMatrices.CreateOnesArray<float> ((int) floatScalar.TimeSampleCount);
			var timesfArr = TestMatrices.GetTimesArray (onesFloatArr.Length);
			floatScalar.Reset (onesFloatArr, timesfArr);
			for (int i = 0; i < onesFloatArr.Length; i++)
				Asserts.AreEqual (onesFloatArr [i], floatScalar.GetFloat (i), $"onesFloatArr iter: {i}");

			floatScalar.Reset (Array.Empty<float> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) floatScalar.TimeSampleCount, "floatScalar.TimeSampleCount");
			Assert.AreEqual (Array.Empty<float> (), floatScalar.GetFloatValues (), "floatScalar Empty");

			// double

			var doubleScalar = new MDLAnimatedScalar ();
			for (int i = 0; i < TestMatrices.DoubleArray.Length; i++)
				doubleScalar.SetValue (TestMatrices.DoubleArray [i], i);

			for (int i = 0; i < TestMatrices.FloatArray.Length; i++)
				Asserts.AreEqual (TestMatrices.DoubleArray [i], doubleScalar.GetDouble (i), $"doubleScalar iter: {i}");

			var allDoubleValues = doubleScalar.GetDoubleValues ();
			for (int i = 0; i < allDoubleValues.Length; i++)
				Asserts.AreEqual (TestMatrices.DoubleArray [i], allDoubleValues [i], $"allDoubleValues iter: {i}");

			var onesDoubleArr = TestMatrices.CreateOnesArray<double> ((int) doubleScalar.TimeSampleCount);
			var timesdArr = TestMatrices.GetTimesArray (onesDoubleArr.Length);
			doubleScalar.Reset (onesDoubleArr, timesdArr);
			for (int i = 0; i < onesDoubleArr.Length; i++)
				Asserts.AreEqual (onesDoubleArr [i], doubleScalar.GetFloat (i), $"onesDoubleArr iter: {i}");

			doubleScalar.Reset (Array.Empty<double> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) doubleScalar.TimeSampleCount, "doubleScalar.TimeSampleCount");
			Assert.AreEqual (Array.Empty<double> (), doubleScalar.GetDoubleValues (), "doubleScalar Empty");
		}

		[Test]
		public void MDLAnimatedVector2Test ()
		{
			// Vector2

			var vector2Values = new MDLAnimatedVector2 ();
			for (int i = 0; i < TestMatrices.Vector2Array.Length; i++)
				vector2Values.SetValue (TestMatrices.Vector2Array [i], i);

			for (int i = 0; i < TestMatrices.Vector2Array.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector2Array [i], vector2Values.GetVector2Value (i), $"vector2Values iter: {i}");

			var allVector2Values = vector2Values.GetVector2Values ();
			for (int i = 0; i < allVector2Values.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector2Array [i], allVector2Values [i], $"allVector2Values iter: {i}");

			var onesVector2Arr = TestMatrices.CreateOnesArray<Vector2> ((int) vector2Values.TimeSampleCount);
			var timesVector2Arr = TestMatrices.GetTimesArray (onesVector2Arr.Length);
			vector2Values.Reset (onesVector2Arr, timesVector2Arr);
			for (int i = 0; i < onesVector2Arr.Length; i++)
				Asserts.AreEqual (onesVector2Arr [i], vector2Values.GetVector2Value (i), $"onesVector2Arr iter: {i}");

			vector2Values.Reset (Array.Empty<Vector2> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) vector2Values.TimeSampleCount, "vector2Values.TimeSampleCount");
			Assert.AreEqual (Array.Empty<Vector2> (), vector2Values.GetVector2Values (), "vector2Values Empty");

			// Vector2d

			var vector2dValues = new MDLAnimatedVector2 ();
			for (int i = 0; i < TestMatrices.Vector2dArray.Length; i++)
				vector2dValues.SetValue (TestMatrices.Vector2dArray [i], i);

			for (int i = 0; i < TestMatrices.Vector2dArray.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector2dArray [i], vector2dValues.GetVector2dValue (i), $"vector2dValues iter: {i}");

			var allVector2dValues = vector2dValues.GetVector2dValues ();
			for (int i = 0; i < allVector2dValues.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector2dArray [i], allVector2dValues [i], $"allVector2dValues iter: {i}");

			var onesVector2dArr = TestMatrices.CreateOnesArray<Vector2d> ((int) vector2dValues.TimeSampleCount);
			var timesVector2dArr = TestMatrices.GetTimesArray (onesVector2dArr.Length);
			vector2dValues.Reset (onesVector2dArr, timesVector2dArr);
			for (int i = 0; i < onesVector2dArr.Length; i++)
				Asserts.AreEqual (onesVector2dArr [i], vector2dValues.GetVector2dValue (i), $"onesVector2dArr iter: {i}");

			vector2dValues.Reset (Array.Empty<Vector2d> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) vector2dValues.TimeSampleCount, "vector2dValues.TimeSampleCount");
			Assert.AreEqual (Array.Empty<Vector2d> (), vector2dValues.GetVector2dValues (), "vector2dValues Empty");
		}

		[Test]
		public void MDLAnimatedVector3Test ()
		{
			// Vector3

			var vector3Values = new MDLAnimatedVector3 ();
			for (int i = 0; i < TestMatrices.NVector3Array.Length; i++)
				vector3Values.SetValue (TestMatrices.NVector3Array [i], i);

			for (int i = 0; i < TestMatrices.NVector3Array.Length; i++)
				Asserts.AreEqual (TestMatrices.NVector3Array [i], vector3Values.GetNVector3Value (i), $"vector3Values iter: {i}");

			var allVector3Values = vector3Values.GetNVector3Values ();
			for (int i = 0; i < allVector3Values.Length; i++)
				Asserts.AreEqual (TestMatrices.NVector3Array [i], allVector3Values [i], $"allVector3Values iter: {i}");

			var onesVector3Arr = TestMatrices.CreateOnesArray<NVector3> ((int) vector3Values.TimeSampleCount);
			var timesVector3Arr = TestMatrices.GetTimesArray (onesVector3Arr.Length);
			vector3Values.Reset (onesVector3Arr, timesVector3Arr);
			for (int i = 0; i < onesVector3Arr.Length; i++)
				Asserts.AreEqual (onesVector3Arr [i], vector3Values.GetNVector3Value (i), $"onesVector3Arr iter: {i}");

			vector3Values.Reset (Array.Empty<NVector3> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) vector3Values.TimeSampleCount, "vector3Values.TimeSampleCount");
			Assert.AreEqual (Array.Empty<NVector3> (), vector3Values.GetNVector3Values (), "vector3Values Empty");

			// Vector3d

			var vector3dValues = new MDLAnimatedVector3 ();
			for (int i = 0; i < TestMatrices.NVector3dArray.Length; i++)
				vector3dValues.SetValue (TestMatrices.NVector3dArray [i], i);

			for (int i = 0; i < TestMatrices.NVector3dArray.Length; i++)
				Asserts.AreEqual (TestMatrices.NVector3dArray [i], vector3dValues.GetNVector3dValue (i), $"vector3dValues iter: {i}");

			var allVector3dValues = vector3dValues.GetNVector3dValues ();
			for (int i = 0; i < allVector3dValues.Length; i++)
				Asserts.AreEqual (TestMatrices.NVector3dArray [i], allVector3dValues [i], $"allVector3dValues iter: {i}");

			var onesVector3dArr = TestMatrices.CreateOnesArray<NVector3d> ((int) vector3dValues.TimeSampleCount);
			var timesVector3dArr = TestMatrices.GetTimesArray (onesVector3dArr.Length);
			vector3dValues.Reset (onesVector3dArr, timesVector3dArr);
			for (int i = 0; i < onesVector3dArr.Length; i++)
				Asserts.AreEqual (onesVector3dArr [i], vector3dValues.GetNVector3dValue (i), $"onesVector3dArr iter: {i}");

			vector3dValues.Reset (Array.Empty<NVector3d> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) vector3dValues.TimeSampleCount, "vector3dValues.TimeSampleCount");
			Assert.AreEqual (Array.Empty<NVector3d> (), vector3dValues.GetNVector3dValues (), "vector3dValues Empty");
		}

		[Test]
		public void MDLAnimatedVector4Test ()
		{
			// Vector4

			var vector4Values = new MDLAnimatedVector4 ();
			for (int i = 0; i < TestMatrices.Vector4Array.Length; i++)
				vector4Values.SetValue (TestMatrices.Vector4Array [i], i);

			for (int i = 0; i < TestMatrices.Vector4Array.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector4Array [i], vector4Values.GetVector4Value (i), $"vector4Values iter: {i}");

			var allVector4Values = vector4Values.GetVector4Values ();
			for (int i = 0; i < allVector4Values.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector4Array [i], allVector4Values [i], $"allVector4Values iter: {i}");

			var onesVector4Arr = TestMatrices.CreateOnesArray<Vector4> ((int) vector4Values.TimeSampleCount);
			var timesVector4Arr = TestMatrices.GetTimesArray (onesVector4Arr.Length);
			vector4Values.Reset (onesVector4Arr, timesVector4Arr);
			for (int i = 0; i < onesVector4Arr.Length; i++)
				Asserts.AreEqual (onesVector4Arr [i], vector4Values.GetVector4Value (i), $"onesVector4Arr iter: {i}");

			vector4Values.Reset (Array.Empty<Vector4> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) vector4Values.TimeSampleCount, "vector4Values.TimeSampleCount");
			Assert.AreEqual (Array.Empty<Vector4> (), vector4Values.GetVector4Values (), "vector4Values Empty");

			// Vector4d

			var vector4dValues = new MDLAnimatedVector4 ();
			for (int i = 0; i < TestMatrices.Vector4dArray.Length; i++)
				vector4dValues.SetValue (TestMatrices.Vector4dArray [i], i);

			for (int i = 0; i < TestMatrices.Vector4dArray.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector4dArray [i], vector4dValues.GetVector4dValue (i), $"vector4dValues iter: {i}");

			var allVector4dValues = vector4dValues.GetVector4dValues ();
			for (int i = 0; i < allVector4dValues.Length; i++)
				Asserts.AreEqual (TestMatrices.Vector4dArray [i], allVector4dValues [i], $"allVector4dValues iter: {i}");

			var onesVector4dArr = TestMatrices.CreateOnesArray<Vector4d> ((int) vector4dValues.TimeSampleCount);
			var timesVector4dArr = TestMatrices.GetTimesArray (onesVector4dArr.Length);
			vector4dValues.Reset (onesVector4dArr, timesVector4dArr);
			for (int i = 0; i < onesVector4dArr.Length; i++)
				Asserts.AreEqual (onesVector4dArr [i], vector4dValues.GetVector4dValue (i), $"onesVector4dArr iter: {i}");

			vector4dValues.Reset (Array.Empty<Vector4d> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) vector4dValues.TimeSampleCount, "vector4dValues.TimeSampleCount");
			Assert.AreEqual (Array.Empty<Vector4d> (), vector4dValues.GetVector4dValues (), "vector4dValues Empty");
		}

		[Test]
		public void MDLAnimatedMatrix4x4Test ()
		{
			// NMatrix4

			var nMatrix4Values = new MDLAnimatedMatrix4x4 ();
			for (int i = 0; i < TestMatrices.NMatrix4Array.Length; i++)
				nMatrix4Values.SetValue (TestMatrices.NMatrix4Array [i], i);

			for (int i = 0; i < TestMatrices.NMatrix4Array.Length; i++)
				Asserts.AreEqual (TestMatrices.NMatrix4Array [i], nMatrix4Values.GetNMatrix4Value (i), $"nMatrix4Values iter: {i}");

			var allNMatrix4Values = nMatrix4Values.GetNMatrix4Values ();
			for (int i = 0; i < allNMatrix4Values.Length; i++)
				Asserts.AreEqual (TestMatrices.NMatrix4Array [i], allNMatrix4Values [i], $"allNMatrix4Values iter: {i}");

			var onesNMatrix4Arr = TestMatrices.CreateOnesArray<NMatrix4> ((int) nMatrix4Values.TimeSampleCount);
			var timesNMatrix4Arr = TestMatrices.GetTimesArray (onesNMatrix4Arr.Length);
			nMatrix4Values.Reset (onesNMatrix4Arr, timesNMatrix4Arr);
			for (int i = 0; i < onesNMatrix4Arr.Length; i++)
				Asserts.AreEqual (onesNMatrix4Arr [i], nMatrix4Values.GetNMatrix4Value (i), $"onesNMatrix4Arr iter: {i}");

			nMatrix4Values.Reset (Array.Empty<NMatrix4> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) nMatrix4Values.TimeSampleCount, "nMatrix4Values.TimeSampleCount");
			Assert.AreEqual (Array.Empty<NMatrix4> (), nMatrix4Values.GetNMatrix4Values (), "nMatrix4Values Empty");

			// NMatrix4d

			var nMatrix4dValues = new MDLAnimatedMatrix4x4 ();
			for (int i = 0; i < TestMatrices.NMatrix4dArray.Length; i++)
				nMatrix4dValues.SetValue (TestMatrices.NMatrix4dArray [i], i);

			for (int i = 0; i < TestMatrices.NMatrix4dArray.Length; i++)
				Asserts.AreEqual (TestMatrices.NMatrix4dArray [i], nMatrix4dValues.GetNMatrix4dValue (i), $"nMatrix4dValues iter: {i}");

			var allNMatrix4dValues = nMatrix4dValues.GetNMatrix4dValues ();
			for (int i = 0; i < allNMatrix4dValues.Length; i++)
				Asserts.AreEqual (TestMatrices.NMatrix4dArray [i], allNMatrix4dValues [i], $"allNMatrix4dValues iter: {i}");

			var onesNMatrix4dArr = TestMatrices.CreateOnesArray<NMatrix4d> ((int) nMatrix4dValues.TimeSampleCount);
			var timesNMatrix4dArr = TestMatrices.GetTimesArray (onesNMatrix4dArr.Length);
			nMatrix4dValues.Reset (onesNMatrix4dArr, timesNMatrix4dArr);
			for (int i = 0; i < onesNMatrix4dArr.Length; i++)
				Asserts.AreEqual (onesNMatrix4dArr [i], nMatrix4dValues.GetNMatrix4dValue (i), $"onesNMatrix4dArr iter: {i}");

			nMatrix4dValues.Reset (Array.Empty<NMatrix4d> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) nMatrix4dValues.TimeSampleCount, "nMatrix4dValues.TimeSampleCount");
			Assert.AreEqual (Array.Empty<NMatrix4d> (), nMatrix4dValues.GetNMatrix4dValues (), "nMatrix4dValues Empty");
		}

		[Test]
		public void MDLAnimatedScalarArrayTest ()
		{
			// Floats

			var floatArr = new MDLAnimatedScalarArray ((nuint) TestMatrices.FloatArray.Length);
			floatArr.SetValues (TestMatrices.FloatArray, 5);

			var retFloatArr = floatArr.GetFloatValues ();
			for (int i = 0; i < retFloatArr.Length; i++)
				Asserts.AreEqual (TestMatrices.FloatArray [i], retFloatArr [i], $"retFloatArr iter: {i}");

			var retFloatArr2 = floatArr.GetFloatValues (5);
			for (int i = 0; i < retFloatArr.Length; i++)
				Asserts.AreEqual (retFloatArr [i], retFloatArr2 [i], $"retFloatArr2 iter: {i}");

			var onesFloatArr = TestMatrices.CreateOnesArray<float> ((int) floatArr.ElementCount);
			var timesFloatArr = new [] { 5d };
			floatArr.Reset (onesFloatArr, timesFloatArr);
			var floatValues = floatArr.GetFloatValues (5);
			for (int i = 0; i < onesFloatArr.Length; i++)
				Asserts.AreEqual (onesFloatArr [i], floatValues [i], $"onesFloatArr iter: {i}");

			floatArr.Reset (Array.Empty<float> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) floatArr.TimeSampleCount, "floatArr.TimeSampleCount");
			Assert.AreEqual (Array.Empty<float> (), floatArr.GetFloatValues (), "floatArr Empty");

			// Doubles

			var doubleArr = new MDLAnimatedScalarArray ((nuint) TestMatrices.DoubleArray.Length);
			doubleArr.SetValues (TestMatrices.DoubleArray, 5);

			var retDoubleArr = doubleArr.GetDoubleValues ();
			for (int i = 0; i < retDoubleArr.Length; i++)
				Asserts.AreEqual (TestMatrices.DoubleArray [i], retDoubleArr [i], $"retDoubleArr iter: {i}");

			var retDoubleArr2 = doubleArr.GetDoubleValues (5);
			for (int i = 0; i < retDoubleArr.Length; i++)
				Asserts.AreEqual (retDoubleArr [i], retDoubleArr2 [i], $"retDoubleArr2 iter: {i}");

			var onesDoubleArr = TestMatrices.CreateOnesArray<double> ((int) doubleArr.ElementCount);
			var timesDoubleArr = new [] { 5d };
			doubleArr.Reset (onesDoubleArr, timesDoubleArr);
			var doubleValues = doubleArr.GetDoubleValues (5);
			for (int i = 0; i < onesDoubleArr.Length; i++)
				Asserts.AreEqual (onesDoubleArr [i], doubleValues [i], $"onesDoubleArr iter: {i}");

			doubleArr.Reset (Array.Empty<double> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) doubleArr.TimeSampleCount, "doubleArr.TimeSampleCount");
			Assert.AreEqual (Array.Empty<double> (), doubleArr.GetFloatValues (), "doubleArr Empty");
		}

		[Test]
		public void MDLAnimatedVector3ArrayTest ()
		{
			// NVector3

			var nVector3Arr = new MDLAnimatedVector3Array ((nuint) TestMatrices.NVector3Array.Length);
			nVector3Arr.SetValues (TestMatrices.NVector3Array, 5);

			var retnVector3Arr = nVector3Arr.GetNVector3Values ();
			for (int i = 0; i < retnVector3Arr.Length; i++)
				Asserts.AreEqual (TestMatrices.NVector3Array [i], retnVector3Arr [i], $"retnVector3Arr iter: {i}");

			var retnVector3Arr2 = nVector3Arr.GetNVector3Values (5);
			for (int i = 0; i < retnVector3Arr.Length; i++)
				Asserts.AreEqual (retnVector3Arr [i], retnVector3Arr2 [i], $"retnVector3Arr2 iter: {i}");

			var onesnVector3Arr = TestMatrices.CreateOnesArray<NVector3> ((int) nVector3Arr.ElementCount);
			var timesnVector3Arr = new [] { 5d };
			nVector3Arr.Reset (onesnVector3Arr, timesnVector3Arr);
			var nVector3Values = nVector3Arr.GetNVector3Values (5);
			for (int i = 0; i < onesnVector3Arr.Length; i++)
				Asserts.AreEqual (onesnVector3Arr [i], nVector3Values [i], $"onesnVector3Arr iter: {i}");

			nVector3Arr.Reset (Array.Empty<NVector3> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) nVector3Arr.TimeSampleCount, "nVector3Arr.TimeSampleCount");
			Assert.AreEqual (Array.Empty<NVector3> (), nVector3Arr.GetNVector3Values (), "nVector3Arr Empty");

			// NVector3d

			var nVector3dArr = new MDLAnimatedVector3Array ((nuint) TestMatrices.NVector3dArray.Length);
			nVector3dArr.SetValues (TestMatrices.NVector3dArray, 5);

			var retnVector3dArr = nVector3dArr.GetNVector3dValues ();
			for (int i = 0; i < retnVector3dArr.Length; i++)
				Asserts.AreEqual (TestMatrices.NVector3dArray [i], retnVector3dArr [i], $"retnVector3dArr iter: {i}");

			var retnVector3dArr2 = nVector3dArr.GetNVector3dValues (5);
			for (int i = 0; i < retnVector3dArr.Length; i++)
				Asserts.AreEqual (retnVector3dArr [i], retnVector3dArr2 [i], $"retnVector3dArr2 iter: {i}");

			var onesnVector3dArr = TestMatrices.CreateOnesArray<NVector3d> ((int) nVector3dArr.ElementCount);
			var timesnVector3dArr = new [] { 5d };
			nVector3dArr.Reset (onesnVector3dArr, timesnVector3dArr);
			var nVector3dValues = nVector3dArr.GetNVector3dValues (5);
			for (int i = 0; i < onesnVector3dArr.Length; i++)
				Asserts.AreEqual (onesnVector3dArr [i], nVector3dValues [i], $"onesnVector3dArr iter: {i}");

			nVector3dArr.Reset (Array.Empty<NVector3d> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) nVector3dArr.TimeSampleCount, "nVector3dArr.TimeSampleCount");
			Assert.AreEqual (Array.Empty<NVector3d> (), nVector3dArr.GetNVector3dValues (), "nVector3dArr Empty");
		}

		[Test]
		public void MDLAnimatedQuaternionArrayTest ()
		{
			// Quaternion

			var quaternionArr = new MDLAnimatedQuaternionArray ((nuint) TestMatrices.QuaternionArray.Length);
			quaternionArr.SetValues (TestMatrices.QuaternionArray, 5);

			var retnQuaternionArr = quaternionArr.GetQuaternionValues ();
			for (int i = 0; i < retnQuaternionArr.Length; i++)
				Asserts.AreEqual (TestMatrices.QuaternionArray [i], retnQuaternionArr [i], $"retnQuaternionArr iter: {i}");

			var retnQuaternionArr2 = quaternionArr.GetQuaternionValues (5);
			for (int i = 0; i < retnQuaternionArr.Length; i++)
				Asserts.AreEqual (retnQuaternionArr [i], retnQuaternionArr2 [i], $"retnQuaternionArr2 iter: {i}");

			var onesQuaternionArr = TestMatrices.CreateOnesArray<Quaternion> ((int) quaternionArr.ElementCount);
			var timesQuaternionArr = new [] { 5d };
			quaternionArr.Reset (onesQuaternionArr, timesQuaternionArr);
			var quaternionValues = quaternionArr.GetQuaternionValues (5);
			for (int i = 0; i < onesQuaternionArr.Length; i++)
				Asserts.AreEqual (onesQuaternionArr [i], quaternionValues [i], $"onesQuaternionArr iter: {i}");

			quaternionArr.Reset (Array.Empty<Quaternion> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) quaternionArr.TimeSampleCount, "quaternionArr.TimeSampleCount");
			Assert.AreEqual (Array.Empty<Quaternion> (), quaternionArr.GetQuaternionValues (), "quaternionArr Empty");

			// Quaterniond

			var quaterniondArr = new MDLAnimatedQuaternionArray ((nuint) TestMatrices.QuaterniondArray.Length);
			quaterniondArr.SetValues (TestMatrices.QuaterniondArray, 5);

			var retQuaterniondArr = quaterniondArr.GetQuaterniondValues ();
			for (int i = 0; i < retQuaterniondArr.Length; i++)
				Asserts.AreEqual (TestMatrices.QuaterniondArray [i], retQuaterniondArr [i], $"retQuaterniondArr iter: {i}");

			var retQuaterniondArr2 = quaterniondArr.GetQuaterniondValues (5);
			for (int i = 0; i < retQuaterniondArr.Length; i++)
				Asserts.AreEqual (retQuaterniondArr [i], retQuaterniondArr2 [i], $"retQuaternionArr2 iter: {i}");

			var onesQuaterniondArr = TestMatrices.CreateOnesArray<Quaterniond> ((int) quaterniondArr.ElementCount);
			var timesQuaterniondArr = new [] { 5d };
			quaterniondArr.Reset (onesQuaterniondArr, timesQuaterniondArr);
			var quaterniondValues = quaterniondArr.GetQuaterniondValues (5);
			for (int i = 0; i < onesQuaterniondArr.Length; i++)
				Asserts.AreEqual (onesQuaterniondArr [i], quaterniondValues [i], $"onesQuaterniondArr iter: {i}");

			quaterniondArr.Reset (Array.Empty<Quaterniond> (), Array.Empty<double> ());
			Assert.AreEqual (0, (int) quaterniondArr.TimeSampleCount, "quaterniondArr.TimeSampleCount");
			Assert.AreEqual (Array.Empty<Quaterniond> (), quaterniondArr.GetQuaterniondValues (), "quaterniondArr Empty");
		}

		[Test]
		public void MDLMatrix4x4ArrayTest ()
		{
			// NMatrix4

			var nMatrix4Arr = new MDLMatrix4x4Array ((nuint) TestMatrices.NMatrix4Array.Length);
			nMatrix4Arr.SetValues (TestMatrices.NMatrix4Array);

			var retNMatrix4Arr = nMatrix4Arr.GetNMatrix4Values ();
			for (int i = 0; i < retNMatrix4Arr.Length; i++)
				Asserts.AreEqual (TestMatrices.NMatrix4Array [i], retNMatrix4Arr [i], $"retNMatrix4Arr iter: {i}");

			// NMatrix4d

			var nMatrix4dArr = new MDLMatrix4x4Array ((nuint) TestMatrices.NMatrix4dArray.Length);
			nMatrix4dArr.SetValues (TestMatrices.NMatrix4dArray);

			var retNMatrix4dArr = nMatrix4dArr.GetNMatrix4dValues ();
			for (int i = 0; i < retNMatrix4dArr.Length; i++)
				Asserts.AreEqual (TestMatrices.NMatrix4dArray [i], retNMatrix4dArr [i], $"retNMatrix4dArr iter: {i}");
		}

		[Test]
		public void MDLAnimatedQuaternionTest ()
		{
			// Quaternion
			TestRuntime.AssertXcodeVersion (11, 0);

			using (var animatedQuat = new MDLAnimatedQuaternion ()) {
				var quat = new Quaternion (1.1f, 2.2f, 3.3f, 4.4f);

				animatedQuat.SetQuaternion (quat, 0);
				var returnedQuat = animatedQuat.GetFloatQuaternion (0);
				Asserts.AreEqual (quat, returnedQuat, "A");

				animatedQuat.Reset (TestMatrices.QuaternionArray, TestMatrices.GetTimesArray (TestMatrices.QuaternionArray.Length));
				var arr = animatedQuat.GetQuaternionValues (20);
				Asserts.AreEqual (TestMatrices.QuaternionArray, arr, "C1");
			}


			using (var animatedQuat = new MDLAnimatedQuaternion ()) {
				var quatd = new Quaterniond (10.1, 20.2, 30.3, 40.4);

				animatedQuat.SetQuaternion (quatd, 0);
				Asserts.AreEqual (quatd, animatedQuat.GetDoubleQuaternion (0), 0.0001, "B");

				animatedQuat.Reset (TestMatrices.QuaterniondArray, TestMatrices.GetTimesArray (TestMatrices.QuaterniondArray.Length));
				var arrd = animatedQuat.GetQuaterniondValues (20);
				Asserts.AreEqual (TestMatrices.QuaterniondArray, arrd, "C2");
			}
		}

		[Test]
		public unsafe void OpenTKSizeOfTests ()
		{
			Assert.AreEqual (sizeof (Matrix4), Marshal.SizeOf (typeof (Matrix4)));
			Assert.AreEqual (sizeof (Matrix4d), Marshal.SizeOf (typeof (Matrix4d)));

			Assert.AreEqual (sizeof (Quaternion), Marshal.SizeOf (typeof (Quaternion)));
			Assert.AreEqual (sizeof (Quaterniond), Marshal.SizeOf (typeof (Quaterniond)));

			Assert.AreEqual (sizeof (Vector2), Marshal.SizeOf (typeof (Vector2)));
			Assert.AreEqual (sizeof (Vector3), Marshal.SizeOf (typeof (Vector3)));
			Assert.AreEqual (sizeof (Vector4), Marshal.SizeOf (typeof (Vector4)));
			Assert.AreEqual (sizeof (Vector2d), Marshal.SizeOf (typeof (Vector2d)));
			Assert.AreEqual (sizeof (Vector3d), Marshal.SizeOf (typeof (Vector3d)));
			Assert.AreEqual (sizeof (Vector4d), Marshal.SizeOf (typeof (Vector4d)));
		}

	}

	[Preserve (AllMembers = true)]
	public static class TestMatrices {
		public static float [] FloatArray = {
			0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f, 0.9884372f, 0.1313103f, 0.3327205f, 0.01164342f, 0.6563147f, 0.7923161f, 0.6764754f, 0.07481737f, 0.03239552f, 0.7156482f, 0.6136858f, 0.1864168f,
			0.7717745f, 0.559364f, 0.00918373f, 0.6579159f, 0.123461f, 0.9993145f, 0.5487496f, 0.2823398f, 0.9710717f, 0.8750508f, 0.472472f, 0.2608089f, 0.5771761f, 0.5617125f, 0.176998f, 0.1271691f,
			0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f, 0.8561344f, 0.009231919f, 0.6150167f, 0.7542298f, 0.550727f, 0.3625788f, 0.6639862f, 0.5763468f, 0.9717328f, 0.003812184f, 0.985266f, 0.7540002f,
			9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f, 6.389504E+07f, 1.172175E+09f, 1.399567E+09f, 1.187143E+09f, 3.729208E+07f, 5.50313E+08f, 1.847369E+09f, 1.612405E+09f, 1.699488E+08f, 4.952176E+08f, 1.07262E+09f, 2.035059E+09f,
			1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f, 1.966322E+09f, 1.1814E+09f, 8.464673E+08f, 1.940651E+09f, 1.229937E+09f, 1.367379E+09f, 1.900015E+09f, 1.516109E+09f, 2.146064E+09f, 1.870971E+09f, 1.046267E+09f, 1.088363E+09f,
			2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f, 3.705524E+08f, 1.984941E+09f, 2.175935E+07f, 4.633518E+08f, 1.801243E+09f, 1.616996E+09f, 1.620852E+09f, 7291498f, 1.012728E+09f, 2.834145E+08f, 3.5328E+08f, 1.35012E+09f,
			0.4904693f, 0.841727f, 0.2294401f, 0.5736054f, 0.5406881f, 0.2172498f, 0.1261143f, 0.6736677f, 0.4570194f, 0.9091009f, 0.7669608f, 0.8468134f, 0.01802658f, 0.3850208f, 0.3730424f, 0.2440258f,
			0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f, 0.340791f, 0.2192288f, 0.5144276f, 0.01813344f, 0.07687104f, 0.7971596f, 0.6393988f, 0.9002907f, 0.1011457f, 0.5047605f, 0.7202546f, 0.07729452f,
			8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f, 1.212676E+09f, 4.131035E+08f, 1.032453E+09f, 2.074689E+08f, 1.536594E+09f, 3.266183E+07f, 5.222072E+08f, 7.923175E+08f, 1.762531E+09f, 7.901702E+08f, 8.1975E+08f, 1.630734E+09f,
			0.006755914f, 0.07464754f, 0.287938f, 0.3724834f, 0.1496783f, 0.6224982f, 0.7150125f, 0.5554719f, 0.4638171f, 0.4200902f, 0.4867154f, 0.773377f, 0.3558737f, 0.4043404f, 0.04670618f, 0.7695189f,
		};

		public static double [] DoubleArray = {
			0.1532144d, 0.5451511d, 0.2004739d, 0.8351463d, 0.9884372d, 0.1313103d, 0.3327205d, 0.01164342d, 0.6563147d, 0.7923161d, 0.6764754d, 0.07481737d, 0.03239552d, 0.7156482d, 0.6136858d, 0.1864168d,
			0.7717745d, 0.559364d, 0.00918373d, 0.6579159d, 0.123461d, 0.9993145d, 0.5487496d, 0.2823398d, 0.9710717d, 0.8750508d, 0.472472d, 0.2608089d, 0.5771761d, 0.5617125d, 0.176998d, 0.1271691d,
			0.2023053d, 0.4701468d, 0.6618567d, 0.7685714d, 0.8561344d, 0.009231919d, 0.6150167d, 0.7542298d, 0.550727d, 0.3625788d, 0.6639862d, 0.5763468d, 0.9717328d, 0.003812184d, 0.985266d, 0.7540002d,
			9.799572E+08d, 1.64794E+09d, 1.117296E+09d, 1.239858E+09d, 6.389504E+07d, 1.172175E+09d, 1.399567E+09d, 1.187143E+09d, 3.729208E+07d, 5.50313E+08d, 1.847369E+09d, 1.612405E+09d, 1.699488E+08d, 4.952176E+08d, 1.07262E+09d, 2.035059E+09d,
			1.102396E+09d, 3.082477E+08d, 1.126484E+09d, 5.022931E+08d, 1.966322E+09d, 1.1814E+09d, 8.464673E+08d, 1.940651E+09d, 1.229937E+09d, 1.367379E+09d, 1.900015E+09d, 1.516109E+09d, 2.146064E+09d, 1.870971E+09d, 1.046267E+09d, 1.088363E+09d,
			2.263112E+08d, 8.79644E+08d, 1.303282E+09d, 1.654159E+09d, 3.705524E+08d, 1.984941E+09d, 2.175935E+07d, 4.633518E+08d, 1.801243E+09d, 1.616996E+09d, 1.620852E+09d, 7291498d, 1.012728E+09d, 2.834145E+08d, 3.5328E+08d, 1.35012E+09d,
			0.4904693d, 0.841727d, 0.2294401d, 0.5736054d, 0.5406881d, 0.2172498d, 0.1261143d, 0.6736677d, 0.4570194d, 0.9091009d, 0.7669608d, 0.8468134d, 0.01802658d, 0.3850208d, 0.3730424d, 0.2440258d,
			0.1252193d, 0.08986127d, 0.3407605d, 0.9144857d, 0.340791d, 0.2192288d, 0.5144276d, 0.01813344d, 0.07687104d, 0.7971596d, 0.6393988d, 0.9002907d, 0.1011457d, 0.5047605d, 0.7202546d, 0.07729452d,
			8.176959E+08d, 1.386156E+09d, 5.956444E+08d, 4.210506E+08d, 1.212676E+09d, 4.131035E+08d, 1.032453E+09d, 2.074689E+08d, 1.536594E+09d, 3.266183E+07d, 5.222072E+08d, 7.923175E+08d, 1.762531E+09d, 7.901702E+08d, 8.1975E+08d, 1.630734E+09d,
			0.006755914d, 0.07464754d, 0.287938d, 0.3724834d, 0.1496783d, 0.6224982d, 0.7150125d, 0.5554719d, 0.4638171d, 0.4200902d, 0.4867154d, 0.773377d, 0.3558737d, 0.4043404d, 0.04670618d, 0.7695189d,
		};

		public static Vector2 [] Vector2Array = {
			new Vector2 (0.1532144f, 0.5451511f),
			new Vector2 (0.7717745f, 0.559364f),
			new Vector2 (0.2023053f, 0.4701468f),
			new Vector2 (0.4904693f, 0.841727f),
			new Vector2 (0.1252193f, 0.08986127f),
			new Vector2 (0.006755914f, 0.07464754f),
			new Vector2 (9.799572E+08f, 1.64794E+09f),
			new Vector2 (1.102396E+09f, 3.082477E+08f),
			new Vector2 (2.263112E+08f, 8.79644E+08f),
			new Vector2 (8.176959E+08f, 1.386156E+09f),
		};

		public static Vector2d [] Vector2dArray = {
			new Vector2d (0.1532144d, 0.5451511d),
			new Vector2d (0.7717745d, 0.559364d),
			new Vector2d (0.2023053d, 0.4701468d),
			new Vector2d (0.4904693d, 0.841727d),
			new Vector2d (0.1252193d, 0.08986127d),
			new Vector2d (0.006755914d, 0.07464754d),
			new Vector2d (9.799572E+08d, 1.64794E+09d),
			new Vector2d (1.102396E+09d, 3.082477E+08d),
			new Vector2d (2.263112E+08d, 8.79644E+08d),
			new Vector2d (8.176959E+08d, 1.386156E+09d),
		};

		public static NVector3 [] NVector3Array = {
			new NVector3 (0.1532144f, 0.5451511f, 0.2004739f),
			new NVector3 (0.7717745f, 0.559364f, 0.00918373f),
			new NVector3 (0.2023053f, 0.4701468f, 0.6618567f),
			new NVector3 (0.4904693f, 0.841727f, 0.2294401f),
			new NVector3 (0.1252193f, 0.08986127f, 0.3407605f),
			new NVector3 (0.006755914f, 0.07464754f, 0.287938f),
			new NVector3 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f),
			new NVector3 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f),
			new NVector3 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f),
			new NVector3 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f),
		};

		public static NVector3d [] NVector3dArray = {
			new NVector3d (0.1532144d, 0.5451511d, 0.2004739d),
			new NVector3d (0.7717745d, 0.559364d, 0.00918373d),
			new NVector3d (0.2023053d, 0.4701468d, 0.6618567d),
			new NVector3d (0.4904693d, 0.841727d, 0.2294401d),
			new NVector3d (0.1252193d, 0.08986127d, 0.3407605d),
			new NVector3d (0.006755914d, 0.07464754d, 0.287938d),
			new NVector3d (9.799572E+08d, 1.64794E+09d, 1.117296E+09d),
			new NVector3d (1.102396E+09d, 3.082477E+08d, 1.126484E+09d),
			new NVector3d (2.263112E+08d, 8.79644E+08d, 1.303282E+09d),
			new NVector3d (8.176959E+08d, 1.386156E+09d, 5.956444E+08d),
		};

		public static Vector4 [] Vector4Array = {
			new Vector4 (0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f),
			new Vector4 (0.7717745f, 0.559364f, 0.00918373f, 0.6579159f),
			new Vector4 (0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f),
			new Vector4 (0.4904693f, 0.841727f, 0.2294401f, 0.5736054f),
			new Vector4 (0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f),
			new Vector4 (0.006755914f, 0.07464754f, 0.287938f, 0.3724834f),
			new Vector4 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f),
			new Vector4 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f),
			new Vector4 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f),
			new Vector4 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f),
		};

		public static Vector4d [] Vector4dArray = {
			new Vector4d (0.1532144d, 0.5451511d, 0.2004739d, 0.8351463d),
			new Vector4d (0.7717745d, 0.559364d, 0.00918373d, 0.6579159d),
			new Vector4d (0.2023053d, 0.4701468d, 0.6618567d, 0.7685714d),
			new Vector4d (0.4904693d, 0.841727d, 0.2294401d, 0.5736054d),
			new Vector4d (0.1252193d, 0.08986127d, 0.3407605d, 0.9144857d),
			new Vector4d (0.006755914d, 0.07464754d, 0.287938d, 0.3724834d),
			new Vector4d (9.799572E+08d, 1.64794E+09d, 1.117296E+09d, 1.239858E+09d),
			new Vector4d (1.102396E+09d, 3.082477E+08d, 1.126484E+09d, 5.022931E+08d),
			new Vector4d (2.263112E+08d, 8.79644E+08d, 1.303282E+09d, 1.654159E+09d),
			new Vector4d (8.176959E+08d, 1.386156E+09d, 5.956444E+08d, 4.210506E+08d),
		};

		public static Quaternion [] QuaternionArray = {
			new Quaternion (0.1825742f, 0.3651484f, 0.5477226f, 0.7302967f),
			new Quaternion (-0.1858319f, 0.09661285f, -0.3279344f, 0.2446305f),
			new Quaternion (0.5446088f, 0.1227905f, -0.1323988f, 0.8190203f),
			new Quaternion (21.5f, 6.2f, -8.7f, 13.4f),
			new Quaternion ((Vector3) NVector3Array [0], 0f),
			new Quaternion ((Vector3) NVector3Array [1], 1f),
			new Quaternion ((Vector3) NVector3Array [2], 2f),
			new Quaternion ((Vector3) NVector3Array [3], 3f),
			new Quaternion ((Vector3) NVector3Array [4], 4f),
			new Quaternion ((Vector3) NVector3Array [5], 5f),
			new Quaternion ((Vector3) NVector3Array [6], 6f),
			new Quaternion ((Vector3) NVector3Array [7], 7f),
			new Quaternion ((Vector3) NVector3Array [8], 8f),
			new Quaternion ((Vector3) NVector3Array [9], 9f),
		};

		public static Quaterniond [] QuaterniondArray = {
			new Quaterniond (0.0d, 1.1d, 2.2d, 3.3d),
			new Quaterniond (0.1825742d, 0.3651484d, 0.5477226d, 0.7302967d),
			new Quaterniond (-0.1858319d, 0.09661285d, -0.3279344d, 0.2446305d),
			new Quaterniond (0.5446088d, 0.1227905d, -0.1323988d, 0.8190203d),
			new Quaterniond (21.5d, 6.2d, -8.7d, 13.4d),
			new Quaterniond ((Vector3d) NVector3dArray [0], 0d),
			new Quaterniond ((Vector3d) NVector3dArray [1], 1d),
			new Quaterniond ((Vector3d) NVector3dArray [2], 2d),
			new Quaterniond ((Vector3d) NVector3dArray [3], 3d),
			new Quaterniond ((Vector3d) NVector3dArray [4], 4d),
			new Quaterniond ((Vector3d) NVector3dArray [5], 5d),
			new Quaterniond ((Vector3d) NVector3dArray [6], 6d),
			new Quaterniond ((Vector3d) NVector3dArray [7], 7d),
			new Quaterniond ((Vector3d) NVector3dArray [8], 8d),
			new Quaterniond ((Vector3d) NVector3dArray [9], 9d),
		};

		public static NMatrix4 [] NMatrix4Array = {
			new NMatrix4 (0.1532144f, 0.5451511f, 0.2004739f, 0.8351463f, 0.9884372f, 0.1313103f, 0.3327205f, 0.01164342f, 0.6563147f, 0.7923161f, 0.6764754f, 0.07481737f, 0.03239552f, 0.7156482f, 0.6136858f, 0.1864168f),
			new NMatrix4 (0.7717745f, 0.559364f, 0.00918373f, 0.6579159f, 0.123461f, 0.9993145f, 0.5487496f, 0.2823398f, 0.9710717f, 0.8750508f, 0.472472f, 0.2608089f, 0.5771761f, 0.5617125f, 0.176998f, 0.1271691f),
			new NMatrix4 (0.2023053f, 0.4701468f, 0.6618567f, 0.7685714f, 0.8561344f, 0.009231919f, 0.6150167f, 0.7542298f, 0.550727f, 0.3625788f, 0.6639862f, 0.5763468f, 0.9717328f, 0.003812184f, 0.985266f, 0.7540002f),
			new NMatrix4 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f, 1.239858E+09f, 6.389504E+07f, 1.172175E+09f, 1.399567E+09f, 1.187143E+09f, 3.729208E+07f, 5.50313E+08f, 1.847369E+09f, 1.612405E+09f, 1.699488E+08f, 4.952176E+08f, 1.07262E+09f, 2.035059E+09f),
			new NMatrix4 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f, 5.022931E+08f, 1.966322E+09f, 1.1814E+09f, 8.464673E+08f, 1.940651E+09f, 1.229937E+09f, 1.367379E+09f, 1.900015E+09f, 1.516109E+09f, 2.146064E+09f, 1.870971E+09f, 1.046267E+09f, 1.088363E+09f),
			new NMatrix4 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f, 1.654159E+09f, 3.705524E+08f, 1.984941E+09f, 2.175935E+07f, 4.633518E+08f, 1.801243E+09f, 1.616996E+09f, 1.620852E+09f, 7291498f, 1.012728E+09f, 2.834145E+08f, 3.5328E+08f, 1.35012E+09f),
			new NMatrix4 (0.4904693f, 0.841727f, 0.2294401f, 0.5736054f, 0.5406881f, 0.2172498f, 0.1261143f, 0.6736677f, 0.4570194f, 0.9091009f, 0.7669608f, 0.8468134f, 0.01802658f, 0.3850208f, 0.3730424f, 0.2440258f),
			new NMatrix4 (0.1252193f, 0.08986127f, 0.3407605f, 0.9144857f, 0.340791f, 0.2192288f, 0.5144276f, 0.01813344f, 0.07687104f, 0.7971596f, 0.6393988f, 0.9002907f, 0.1011457f, 0.5047605f, 0.7202546f, 0.07729452f),
			new NMatrix4 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f, 4.210506E+08f, 1.212676E+09f, 4.131035E+08f, 1.032453E+09f, 2.074689E+08f, 1.536594E+09f, 3.266183E+07f, 5.222072E+08f, 7.923175E+08f, 1.762531E+09f, 7.901702E+08f, 8.1975E+08f, 1.630734E+09f),
			new NMatrix4 (0.006755914f, 0.07464754f, 0.287938f, 0.3724834f, 0.1496783f, 0.6224982f, 0.7150125f, 0.5554719f, 0.4638171f, 0.4200902f, 0.4867154f, 0.773377f, 0.3558737f, 0.4043404f, 0.04670618f, 0.7695189f),
		};

		public static NMatrix4d [] NMatrix4dArray = {
			new NMatrix4d (0.1532144d, 0.5451511d, 0.2004739d, 0.8351463d, 0.9884372d, 0.1313103d, 0.3327205d, 0.01164342d, 0.6563147d, 0.7923161d, 0.6764754d, 0.07481737d, 0.03239552d, 0.7156482d, 0.6136858d, 0.1864168d),
			new NMatrix4d (0.7717745d, 0.559364d, 0.00918373d, 0.6579159d, 0.123461d, 0.9993145d, 0.5487496d, 0.2823398d, 0.9710717d, 0.8750508d, 0.472472d, 0.2608089d, 0.5771761d, 0.5617125d, 0.176998d, 0.1271691d),
			new NMatrix4d (0.2023053d, 0.4701468d, 0.6618567d, 0.7685714d, 0.8561344d, 0.009231919d, 0.6150167d, 0.7542298d, 0.550727d, 0.3625788d, 0.6639862d, 0.5763468d, 0.9717328d, 0.003812184d, 0.985266d, 0.7540002d),
			new NMatrix4d (9.799572E+08d, 1.64794E+09d, 1.117296E+09d, 1.239858E+09d, 6.389504E+07d, 1.172175E+09d, 1.399567E+09d, 1.187143E+09d, 3.729208E+07d, 5.50313E+08d, 1.847369E+09d, 1.612405E+09d, 1.699488E+08d, 4.952176E+08d, 1.07262E+09d, 2.035059E+09d),
			new NMatrix4d (1.102396E+09d, 3.082477E+08d, 1.126484E+09d, 5.022931E+08d, 1.966322E+09d, 1.1814E+09d, 8.464673E+08d, 1.940651E+09d, 1.229937E+09d, 1.367379E+09d, 1.900015E+09d, 1.516109E+09d, 2.146064E+09d, 1.870971E+09d, 1.046267E+09d, 1.088363E+09d),
			new NMatrix4d (2.263112E+08d, 8.79644E+08d, 1.303282E+09d, 1.654159E+09d, 3.705524E+08d, 1.984941E+09d, 2.175935E+07d, 4.633518E+08d, 1.801243E+09d, 1.616996E+09d, 1.620852E+09d, 7291498d, 1.012728E+09d, 2.834145E+08d, 3.5328E+08d, 1.35012E+09d),
			new NMatrix4d (0.4904693d, 0.841727d, 0.2294401d, 0.5736054d, 0.5406881d, 0.2172498d, 0.1261143d, 0.6736677d, 0.4570194d, 0.9091009d, 0.7669608d, 0.8468134d, 0.01802658d, 0.3850208d, 0.3730424d, 0.2440258d),
			new NMatrix4d (0.1252193d, 0.08986127d, 0.3407605d, 0.9144857d, 0.340791d, 0.2192288d, 0.5144276d, 0.01813344d, 0.07687104d, 0.7971596d, 0.6393988d, 0.9002907d, 0.1011457d, 0.5047605d, 0.7202546d, 0.07729452d),
			new NMatrix4d (8.176959E+08d, 1.386156E+09d, 5.956444E+08d, 4.210506E+08d, 1.212676E+09d, 4.131035E+08d, 1.032453E+09d, 2.074689E+08d, 1.536594E+09d, 3.266183E+07d, 5.222072E+08d, 7.923175E+08d, 1.762531E+09d, 7.901702E+08d, 8.1975E+08d, 1.630734E+09d),
			new NMatrix4d (0.006755914d, 0.07464754d, 0.287938d, 0.3724834d, 0.1496783d, 0.6224982d, 0.7150125d, 0.5554719d, 0.4638171d, 0.4200902d, 0.4867154d, 0.773377d, 0.3558737d, 0.4043404d, 0.04670618d, 0.7695189d),
		};

		public static T [] CreateOnesArray<T> (int size) where T : struct
		{
			T [] array = null;
			var t = typeof (T);

			if (t == typeof (float))
				array = Enumerable.Repeat (1f, size).ToArray () as T [];
			else if (t == typeof (double))
				array = Enumerable.Repeat (1d, size).ToArray () as T [];
			else if (t == typeof (Vector2))
				array = Enumerable.Repeat (new Vector2 (1f, 1f), size).ToArray () as T [];
			else if (t == typeof (Vector2d))
				array = Enumerable.Repeat (new Vector2d (1d, 1d), size).ToArray () as T [];
			else if (t == typeof (NVector3))
				array = Enumerable.Repeat (new NVector3 (1f, 1f, 1f), size).ToArray () as T [];
			else if (t == typeof (NVector3d))
				array = Enumerable.Repeat (new NVector3d (1d, 1d, 1d), size).ToArray () as T [];
			else if (t == typeof (Vector4))
				array = Enumerable.Repeat (new Vector4 (1f, 1f, 1f, 1f), size).ToArray () as T [];
			else if (t == typeof (Vector4d))
				array = Enumerable.Repeat (new Vector4d (1d, 1d, 1d, 1d), size).ToArray () as T [];
			else if (t == typeof (Quaternion))
				array = Enumerable.Repeat (new Quaternion (1f, 1f, 1f, 1f), size).ToArray () as T [];
			else if (t == typeof (Quaterniond))
				array = Enumerable.Repeat (new Quaterniond (1d, 1d, 1d, 1d), size).ToArray () as T [];
			else if (t == typeof (NMatrix4))
				array = Enumerable.Repeat (new NMatrix4 (1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f), size).ToArray () as T [];
			else if (t == typeof (NMatrix4d))
				array = Enumerable.Repeat (new NMatrix4d (1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d), size).ToArray () as T [];
			else
				throw new NotImplementedException ();

			return array;
		}

		public static double [] GetTimesArray (int length)
		{
			return Enumerable.Range (0, length).Select (v => (double) v).ToArray ();
		}
	}
}
#endif
