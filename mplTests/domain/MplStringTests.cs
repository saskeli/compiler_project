﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class MplStringTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            MplString sa = new MplString("bla");
            MplString sb = new MplString("bla");
            MplString sc = new MplString("bar");
            Assert.IsTrue((sa == sb).Val);
            Assert.IsTrue(sa.Equals(sb));
            Assert.IsFalse((sa == sc).Val);
            Assert.IsFalse(sa.Equals(sc));
        }

        [TestMethod()]
        public void GetTypeTest()
        {
            MplString sa = new MplString("bla");
            Assert.AreEqual(Type.String, sa.GetType());
        }

        [TestMethod()]
        public void AdditionTest()
        {
            MplString sa = new MplString("bla");
            sa += sa;
            string v = sa.Val;
            Assert.AreEqual("blabla", v);
        }

        [TestMethod()]
        public void NotEqualsTest()
        {
            MplString sa = new MplString("bla");
            MplString sb = new MplString("bla");
            MplString sc = new MplString("bar");
            Assert.IsFalse((sa != sb).Val);
            Assert.IsTrue((sa != sc).Val);
        }

        [TestMethod()]
        public void LessTest()
        {
            MplString sa = new MplString("bla");
            MplString sb = new MplString("bla");
            MplString sc = new MplString("bar");
            bool v = (sa < sc).Val;
            Assert.IsFalse(v);
            v = (sc < sa).Val;
            Assert.IsTrue(v);
            v = (sa < sb).Val;
            Assert.IsFalse(v);
        }

        [TestMethod()]
        public void GreaterTest()
        {
            MplString sa = new MplString("bla");
            MplString sb = new MplString("bla");
            MplString sc = new MplString("bar");
            bool v = (sa > sc).Val;
            Assert.IsTrue(v);
            v = (sc > sa).Val;
            Assert.IsFalse(v);
            v = (sa > sb).Val;
            Assert.IsFalse(v);
        }
    }
}