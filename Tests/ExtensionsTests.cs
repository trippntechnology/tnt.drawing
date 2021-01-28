using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using TNT.Drawing;

namespace Tests
{
	[TestClass]
	public class ExtensionsTests
	{
		[TestMethod]
		public void LetWithReturn()
		{
			int? foo = null;
			bool ranLambda = false;

			var result = foo.Let(f =>
			{
				int? returnValue = 7;
				ranLambda = true;
				return returnValue;
			});
			Assert.IsNull(result);
			Assert.IsFalse(ranLambda);

			foo = 11;
			result = foo.Let(f =>
			{
				ranLambda = true;
				return 7;
			});
			Assert.AreEqual(7, result);
			Assert.IsTrue(ranLambda);

			result = foo.Let(f =>
			{
				ranLambda = true;
				return (int)f;
			});
			Assert.AreEqual(11, result);
			Assert.IsTrue(ranLambda);
		}

		[TestMethod]
		public void Let()
		{
			int? foo = null;
			bool ranLambda = false;

			foo.Let(f => { ranLambda = true; });
			Assert.IsFalse(ranLambda);

			foo = 11;
			foo.Let(f => { ranLambda = true; });
			Assert.IsTrue(ranLambda);

			foo.Let(f => { ranLambda = true; });
			Assert.IsTrue(ranLambda);
		}

		[TestMethod]
		public void Also()
		{
			int? value = null;
			int? setInAlso = null;

			var result = value.Also(v =>
			{
				setInAlso = 7;
			});
			Assert.IsNull(result);
			Assert.IsNull(value);
			Assert.IsNull(setInAlso);

			value = 7;
			result = value.Also(v =>
			{
				setInAlso = 11;
			});
			Assert.AreEqual(7, result);
			Assert.AreEqual(11, setInAlso);
		}

		[TestMethod]
		public void PointSubstract()
		{
			var p1 = new Point(11, 7);
			var p2 = new Point(31, 29);
			var expected = new Point(20, 22);
			Assert.AreEqual(expected, p2.Subtract(p1));
		}


		[TestMethod]
		public void PointAdd()
		{
			var p1 = new Point(11, 7);
			var p2 = new Point(31, 29);
			var expected = new Point(42, 36);
			Assert.AreEqual(expected, p2.Add(p1));
		}

		[TestMethod]
		public void Snap()
		{
			Assert.AreEqual(new Point(10, 20), new Point(11, 19).Snap(10));
			Assert.AreEqual(new Point(10, 20), new Point(14, 16).Snap(10));
			Assert.AreEqual(new Point(20, 20), new Point(15, 15).Snap(10));
			Assert.AreEqual(new Point(20, 10), new Point(16, 14).Snap(10));
			Assert.AreEqual(new Point(20, 10), new Point(19, 11).Snap(10));
			Assert.AreEqual(new Point(20, 20), new Point(20, 20).Snap(10));
		}

		[TestMethod]
		public void Deconstruct()
		{
			var p1 = new Point(11, 7);
			Assert.AreEqual((11, 7), p1.Deconstruct());
		}

		[TestMethod]
		public void RunNotNull_Test1()
		{
			int? v1 = null;
			int? v2 = null;
			int? result = null;

			Extensions.RunNotNull(v1, v2, (a, b) => Assert.Fail("Callback should not be called"));
			v2 = 2;
			Extensions.RunNotNull(v1, v2, (a, b) => Assert.Fail("Callback should not be called"));
			v1 = 1;
			Extensions.RunNotNull(v1, v2, (a, b) => result = a + b);
			Assert.AreEqual(v1 + v2, result);
		}

		[TestMethod]
		public void RunNotNull_Test2()
		{
			int? v1 = null;
			int? v2 = null;
			int? v3 = null;
			int? result = null;

			Extensions.RunNotNull(v1, v2, v3, (a, b, c) => Assert.Fail("Callback should not be called"));
			v3 = 3;
			Extensions.RunNotNull(v1, v2, v3, (a, b, c) => Assert.Fail("Callback should not be called"));
			v2 = 2;
			Extensions.RunNotNull(v1, v2, v3, (a, b, c) => Assert.Fail("Callback should not be called"));
			v1 = 1;
			Extensions.RunNotNull(v1, v2, v3, (a, b, c) => result = a + b + c);
			Assert.AreEqual(v1 + v2 + v3, result);
		}

		[TestMethod]
		public void AddNotNull()
		{
			var list = new List<int?>();
			int? v1 = null;
			list.AddNotNull(v1);
			Assert.AreEqual(0, list.Count);
			v1 = 1;
			list.AddNotNull(v1);
			Assert.AreEqual(1, list.Count);
		}

		[TestMethod]
		public void AdjacentTo()
		{
			var sut = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			var result = sut.AdjacentTo(5);
			CollectionAssert.AreEqual(new List<int>() { 4, 6 }, result);
		}
	}
}
