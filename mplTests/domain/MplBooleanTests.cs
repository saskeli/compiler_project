using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;

namespace mplTests.domain
{
    [TestClass]
    public class MplBooleanTests
    {
        [TestMethod]
        public void MplBooleanTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            Assert.IsTrue(b.Val);
            Assert.IsFalse(a.Val);
        }

        [TestMethod]
        public void GetTypeTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            Assert.AreEqual(Type.Bool, a.GetType());
        }

        [TestMethod]
        public void EqualsTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            MplBoolean c = new MplBoolean(true, 0, 0);
            Assert.IsTrue(b.Equals(c));
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void EqualityTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            MplBoolean c = new MplBoolean(true, 0, 0);
            Assert.IsTrue((b == c).Val);
            Assert.IsFalse((a == b).Val);
        }

        [TestMethod]
        public void InequalityTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            MplBoolean c = new MplBoolean(true, 0, 0);
            Assert.IsTrue((a != c).Val);
            Assert.IsFalse((c != b).Val);
        }

        [TestMethod]
        public void LessTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            MplBoolean c = new MplBoolean(true, 0, 0);
            Assert.IsFalse((b < c).Val);
            Assert.IsTrue((a < b).Val);
            Assert.IsFalse((b < a).Val);
        }

        [TestMethod]
        public void GreaterTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            MplBoolean c = new MplBoolean(true, 0, 0);
            Assert.IsFalse((b > c).Val);
            Assert.IsFalse((a > b).Val);
            Assert.IsTrue((b > a).Val);
        }

        [TestMethod]
        public void NegationTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            Assert.IsTrue((!a).Val);
            Assert.IsFalse((!b).Val);
        }

        [TestMethod]
        public void ConjunctionTest()
        {
            MplBoolean a = new MplBoolean(false, 0, 0);
            MplBoolean b = new MplBoolean(true, 0, 0);
            MplBoolean c = new MplBoolean(true, 0, 0);
            Assert.IsFalse((a & b).Val);
            Assert.IsTrue((b & c).Val);
            Assert.IsTrue((c & b).Val);
        }
    }
}