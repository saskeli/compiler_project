using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using mpl.Exceptions;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class MplIntegerTests
    {
        private MplInteger a;
        private MplInteger b;
        private MplInteger c;
        private MplInteger d;

        [TestInitialize]
        public void TestInitialize()
        {
            a = new MplInteger(0);
            b = new MplInteger(2);
            c = new MplInteger(-2);
            d = new MplInteger(0);
        }

        [TestMethod()]
        public void MplIntegerTest()
        {
            Assert.AreEqual(0, a.Val);
            Assert.AreEqual(2, b.Val);
            Assert.AreEqual(-2, c.Val);
            Assert.AreEqual(0, d.Val);
        }

        [TestMethod()]
        public void GetTypeTest()
        {
            Assert.AreEqual(Type.Int, a.GetType());
            Assert.AreEqual(Type.Int, b.GetType());
            Assert.AreEqual(Type.Int, c.GetType());
            Assert.AreEqual(Type.Int, d.GetType());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.IsTrue(a.Equals(d));
            Assert.IsTrue(a.Equals((object)d));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(c));
            Assert.IsFalse(b.Equals(c));
            Assert.IsFalse(b.Equals(d));
        }

        [TestMethod()]
        public void AdditionTest()
        {
            Assert.AreEqual(2, (a + b).Val);
            Assert.AreEqual(-2, (a + c).Val);
            Assert.AreEqual(0, (a + d).Val);
            Assert.AreEqual(0, (b + c).Val);
            Assert.AreEqual(2, (b + d).Val);
            Assert.AreEqual(-2, (c + d).Val);
            Assert.AreEqual(0, (d + a).Val);
        }

        [TestMethod()]
        public void SubtractionTest()
        {
            Assert.AreEqual(-2, (a - b).Val);
            Assert.AreEqual(2, (a - c).Val);
            Assert.AreEqual(0, (a - d).Val);
            Assert.AreEqual(4, (b - c).Val);
            Assert.AreEqual(2, (b - d).Val);
            Assert.AreEqual(-2, (c - d).Val);
            Assert.AreEqual(0, (d - a).Val);
        }

        [TestMethod()]
        public void MultiplicationTest()
        {
            Assert.AreEqual(0, (a * b).Val);
            Assert.AreEqual(0, (a * c).Val);
            Assert.AreEqual(0, (a * d).Val);
            Assert.AreEqual(-4, (b * c).Val);
            Assert.AreEqual(0, (b * d).Val);
            Assert.AreEqual(0, (c * d).Val);
            Assert.AreEqual(0, (d * a).Val);
        }

        [TestMethod()]
        [ExpectedException(typeof(MplDivideByZeroException), 
            "Expected zero division error for 2 / 0")]
        public void DivisionTest()
        {
            Assert.AreEqual(0, (a / b).Val);
            Assert.AreEqual(0, (a / c).Val);
            Assert.AreEqual(-1, (b / c).Val);
            Assert.AreEqual(-1, (c / b).Val);
            Assert.AreEqual(1, (b / b).Val);
            MplInteger i = b / a;
        }

        [TestMethod()]
        public void EqualityTest()
        {
            Assert.IsTrue((a == a).Val);
            Assert.IsTrue((a == d).Val);
            Assert.IsFalse((a == b).Val);
            Assert.IsFalse((b == c).Val);
            Assert.IsFalse((d == b).Val);
        }

        [TestMethod()]
        public void InequalityTest()
        {
            Assert.IsFalse((a != a).Val);
            Assert.IsFalse((a != d).Val);
            Assert.IsTrue((a != b).Val);
            Assert.IsTrue((b != c).Val);
            Assert.IsTrue((d != b).Val);
        }

        [TestMethod()]
        public void LessTest()
        {
            Assert.IsTrue((a < b).Val);
            Assert.IsFalse((a < c).Val);
            Assert.IsFalse((a < d).Val);
            Assert.IsFalse((b < c).Val);
            Assert.IsTrue((c < b).Val);
        }

        [TestMethod()]
        public void GreaterTest()
        {
            Assert.IsFalse((a > b).Val);
            Assert.IsTrue((a > c).Val);
            Assert.IsFalse((a > d).Val);
            Assert.IsTrue((b > c).Val);
            Assert.IsFalse((c > b).Val);
        }
    }
}
