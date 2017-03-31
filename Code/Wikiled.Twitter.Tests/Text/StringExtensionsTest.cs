﻿using NUnit.Framework;
using Wikiled.Twitter.Text;

namespace Wikiled.Twitter.Tests.Text
{
    [TestFixture]
    public class StringExtensionsTest
    {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void SliceTest()
        {
            string str;
            str = "Hello world!";
            Assert.AreEqual("Hello world!", str.Slice(0));
            Assert.AreEqual("lo world!", str.Slice(3));
            Assert.AreEqual("lo wo", str.Slice(3, 8));
            Assert.AreEqual("H", str.Slice(0, 1));
            Assert.AreEqual("!", str.Slice(-1));
            Assert.AreEqual("lo world", str.Slice(3, -1));
            Assert.AreEqual("", str.Slice(-1, -1));
        }
    }
}