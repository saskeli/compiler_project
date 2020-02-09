using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class MplBooleanTests
    {
        [TestMethod()]
        public void MplBooleanTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            Assert.IsTrue(b.Val);
            Assert.IsFalse(a.Val);
        }

        [TestMethod()]
        public void GetTypeTest()
        {
            MplBoolean a = new MplBoolean(false);
            Assert.AreEqual(Type.Bool, a.GetType());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            MplBoolean c = new MplBoolean(true);
            Assert.IsTrue(b.Equals(c));
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod()]
        public void EqualityTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            MplBoolean c = new MplBoolean(true);
            Assert.IsTrue((b == c).Val);
            Assert.IsFalse((a == b).Val);
        }

        [TestMethod()]
        public void InequalityTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            MplBoolean c = new MplBoolean(true);
            Assert.IsTrue((a != c).Val);
            Assert.IsFalse((c != b).Val);
        }

        [TestMethod()]
        public void LessTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            MplBoolean c = new MplBoolean(true);
            Assert.IsFalse((b < c).Val);
            Assert.IsTrue((a < b).Val);
            Assert.IsFalse((b < a).Val);
        }

        [TestMethod()]
        public void GreaterTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            MplBoolean c = new MplBoolean(true);
            Assert.IsFalse((b > c).Val);
            Assert.IsFalse((a > b).Val);
            Assert.IsTrue((b > a).Val);
        }

        [TestMethod()]
        public void NegationTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            Assert.IsTrue((!a).Val);
            Assert.IsFalse((!b).Val);
        }

        [TestMethod()]
        public void ConjunctionTest()
        {
            MplBoolean a = new MplBoolean(false);
            MplBoolean b = new MplBoolean(true);
            MplBoolean c = new MplBoolean(true);
            Assert.IsFalse((a & b).Val);
            Assert.IsTrue((b & c).Val);
            Assert.IsTrue((c & b).Val);
        }
    }
}