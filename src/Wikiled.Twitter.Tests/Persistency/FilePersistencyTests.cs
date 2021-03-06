﻿using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Tweetinvi.Models.DTO;
using Wikiled.Twitter.Persistency;

namespace Wikiled.Twitter.Tests.Persistency
{
    [TestFixture]
    public class FilePersistencyTests
    {
        private FilePersistency instance;

        private Mock<IStreamSource> stream;

        private Mock<ITweetDTO> tweet;

        [SetUp]
        public void Setup()
        {
            stream = new Mock<IStreamSource>();
            tweet = new Mock<ITweetDTO>();
            instance = new FilePersistency(new NullLogger<FilePersistency>(), stream.Object);
        }

        [Test]
        public void SaveError()
        {
            stream.Setup(item => item.GetStream()).Throws<NullReferenceException>();
            instance.Save(tweet.Object);
        }

        [Test]
        public void Load()
        {
            var result = new FileLoader(new NullLogger<FileLoader>()).Load(Path.Combine(TestContext.CurrentContext.TestDirectory, @"data\data_20160311_1115.dat"));
            Assert.AreEqual(7725, result.Length);
        }
    }
}
