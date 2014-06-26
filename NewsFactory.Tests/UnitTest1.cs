using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NewsFactory.Foundation.Common;

namespace NewsFactory.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateEmptyWordIndex()
        {
            var wi = new WordIndex();
        }

        [TestMethod]
        public void AddStringsToWordInedx()
        {
            var wi = new WordIndex();
            wi.AddRange(new string[] { "hello world" });
            Assert.AreEqual(2, wi.WordsCount);
        }

        [TestMethod]
        public void TestWordVectror()
        {
            var wi = new WordIndex();
            wi.AddRange(new string[] { "hello world" });
            var wv = wi.WordToVector("world hello");

            Assert.AreEqual(2, wv.Count);
            Assert.IsTrue(wv.Contains(0));
            Assert.IsTrue(wv.Contains(1));
        }

        [TestMethod]
        public void TestWordVectror2()
        {
            var wi = new WordIndex();
            wi.AddRange(new string[] { "hello world" });
            var wv = wi.WordToVector("hello world");

            Assert.AreEqual(2, wv.Count);
            Assert.IsTrue(wv.Contains(0));
            Assert.IsTrue(wv.Contains(1));
        }

        [TestMethod]
        public void TestWordVectror3()
        {
            var wi = new WordIndex();
            wi.AddRange(new string[] { "hello world" });
            var wv = wi.WordToVector("world goodbuuy");

            Assert.AreEqual(1, wv.Count);
            Assert.IsTrue(wv.Contains(0) || wv.Contains(1));
        }

        [TestMethod]
        public void TestWordVectror2WithStopwords()
        {
            var wi = new WordIndex();
            wi.AddRange(new string[] { "hello the world" });
            var wv = wi.WordToVector("hello world a");

            Assert.AreEqual(2, wv.Count);
            Assert.IsTrue(wv.Contains(0));
            Assert.IsTrue(wv.Contains(1));
        }
    }
}
