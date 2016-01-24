﻿using System;

using FactoryBot.Extensions;
using FactoryBot.Generators.Strings;

using NUnit.Framework;

namespace FactoryBot.Tests.Generators.Strings
{
    public class WordRandomGeneratorTest
    {
        [Test]
        public void GenerateNextWithoutThreshold()
        {
            var generator = new WordRandomGenerator();

            var str1 = (string)generator.Next();
            var str2 = (string)generator.Next();

            Assert.That(str1, Is.Not.Null);
            Assert.That(str2, Is.Not.Null.And.Not.EqualTo(str1));
        }

        [Test]
        public void GenerateNextWithThreshold()
        {
            var generator = new WordRandomGenerator(3, 15);

            var str1 = (string)generator.Next();
            var str2 = (string)generator.Next();

            Assert.That(str1, Is.Not.Null);
            Assert.That(str2, Is.Not.Null.And.Not.EqualTo(str1));
            Assert.That(str1.Words(), Has.Length.InRange(3, 15));
            Assert.That(str2.Words(), Has.Length.InRange(3, 15));
        }

        [Test]
        public void GenerateNextWithConstantThreshold()
        {
            var generator = new WordRandomGenerator(10, 10);

            var str1 = (string)generator.Next();
            var str2 = (string)generator.Next();
            var str3 = (string)generator.Next();

            Assert.That(str1.Words(), Has.Length.EqualTo(10));
            Assert.That(str2.Words(), Has.Length.EqualTo(10));
            Assert.That(str3.Words(), Has.Length.EqualTo(10));
        }

        [Test]
        [TestCase(-10, 40)]
        [TestCase(10, -40)]
        [TestCase(40, 10)]
        [TestCase(0, 0)]
        public void CreateWithInvalidThreshold(int min, int max)
        {
            Assert.That(() => new WordRandomGenerator(min, max), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}