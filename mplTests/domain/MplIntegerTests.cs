using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using mpl.Exceptions;
using Type = mpl.domain.Type;

namespace mplTests.domain
{
    [TestClass]
    public class MplIntegerTests
    {
        private MplInteger _a;
        private MplInteger _b;
        private MplInteger _c;
        private MplInteger _d;

        [TestInitialize]
        public void TestInitialize()
        {
            _a = new MplInteger(0, 0, 0);
            _b = new MplInteger(2, 0, 0);
            _c = new MplInteger(-2, 0, 0);
            _d = new MplInteger(0, 0, 0);
        }

        [TestMethod]
        public void MplIntegerTest()
        {
            Assert.AreEqual(0, _a.Val);
            Assert.AreEqual(2, _b.Val);
            Assert.AreEqual(-2, _c.Val);
            Assert.AreEqual(0, _d.Val);
        }

        [TestMethod]
        public void GetTypeTest()
        {
            Assert.AreEqual(Type.Int, _a.GetType());
            Assert.AreEqual(Type.Int, _b.GetType());
            Assert.AreEqual(Type.Int, _c.GetType());
            Assert.AreEqual(Type.Int, _d.GetType());
        }

        [TestMethod]
        public void EqualsTest()
        {
            Assert.IsTrue(_a.Equals(_d));
            Assert.IsTrue(_a.Equals((object)_d));
            Assert.IsFalse(_a.Equals(_b));
            Assert.IsFalse(_a.Equals(_c));
            Assert.IsFalse(_b.Equals(_c));
            Assert.IsFalse(_b.Equals(_d));
        }

        [TestMethod]
        public void AdditionTest()
        {
            Assert.AreEqual(2, (_a + _b).Val);
            Assert.AreEqual(-2, (_a + _c).Val);
            Assert.AreEqual(0, (_a + _d).Val);
            Assert.AreEqual(0, (_b + _c).Val);
            Assert.AreEqual(2, (_b + _d).Val);
            Assert.AreEqual(-2, (_c + _d).Val);
            Assert.AreEqual(0, (_d + _a).Val);
        }

        [TestMethod]
        public void SubtractionTest()
        {
            Assert.AreEqual(-2, (_a - _b).Val);
            Assert.AreEqual(2, (_a - _c).Val);
            Assert.AreEqual(0, (_a - _d).Val);
            Assert.AreEqual(4, (_b - _c).Val);
            Assert.AreEqual(2, (_b - _d).Val);
            Assert.AreEqual(-2, (_c - _d).Val);
            Assert.AreEqual(0, (_d - _a).Val);
        }

        [TestMethod]
        public void MultiplicationTest()
        {
            Assert.AreEqual(0, (_a * _b).Val);
            Assert.AreEqual(0, (_a * _c).Val);
            Assert.AreEqual(0, (_a * _d).Val);
            Assert.AreEqual(-4, (_b * _c).Val);
            Assert.AreEqual(0, (_b * _d).Val);
            Assert.AreEqual(0, (_c * _d).Val);
            Assert.AreEqual(0, (_d * _a).Val);
        }

        [TestMethod]
        [ExpectedException(typeof(MplDivideByZeroException), 
            "Expected zero division error for 2 / 0")]
        public void DivisionTest()
        {
            Assert.AreEqual(0, (_a / _b).Val);
            Assert.AreEqual(0, (_a / _c).Val);
            Assert.AreEqual(-1, (_b / _c).Val);
            Assert.AreEqual(-1, (_c / _b).Val);
            Assert.AreEqual(1, (_b / _b).Val);
            MplInteger _ = _b / _a;
        }

        [TestMethod]
        public void EqualityTest()
        {
#pragma warning disable CS1718 // Comparison made to same variable
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue((_a == _a).Val);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.IsTrue((_a == _d).Val);
            Assert.IsFalse((_a == _b).Val);
            Assert.IsFalse((_b == _c).Val);
            Assert.IsFalse((_d == _b).Val);
        }

        [TestMethod]
        public void InequalityTest()
        {
#pragma warning disable CS1718 // Comparison made to same variable
            // ReSharper disable once EqualExpressionComparison
            Assert.IsFalse((_a != _a).Val);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.IsFalse((_a != _d).Val);
            Assert.IsTrue((_a != _b).Val);
            Assert.IsTrue((_b != _c).Val);
            Assert.IsTrue((_d != _b).Val);
        }

        [TestMethod]
        public void LessTest()
        {
            Assert.IsTrue((_a < _b).Val);
            Assert.IsFalse((_a < _c).Val);
            Assert.IsFalse((_a < _d).Val);
            Assert.IsFalse((_b < _c).Val);
            Assert.IsTrue((_c < _b).Val);
        }

        [TestMethod]
        public void GreaterTest()
        {
            Assert.IsFalse((_a > _b).Val);
            Assert.IsTrue((_a > _c).Val);
            Assert.IsFalse((_a > _d).Val);
            Assert.IsTrue((_b > _c).Val);
            Assert.IsFalse((_c > _b).Val);
        }
    }
}
