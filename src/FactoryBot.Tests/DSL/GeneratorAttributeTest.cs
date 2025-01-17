﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FactoryBot.DSL.Attributes;
using FactoryBot.Generators;
using NUnit.Framework;

namespace FactoryBot.Tests.DSL
{
    [TestFixture]
    public class GeneratorAttributeTest
    {
        [Test]
        public void Constructor_WithNonGeneratorClass_ThrowsError() => Assert.That(() => new GeneratorAttribute(typeof(string)), Throws.ArgumentException);

        [Test]
        public void CreateGenerator_GeneratorWithoutParameters_ReturnsGenerator()
        {
            var attr = new GeneratorAttribute(typeof(TestGenerator));

            dynamic generator = attr.CreateGenerator(GetDSLMethod(x => x.GetTestGenerator()), new Dictionary<string, object>());

            Assert.That(generator, Is.Not.Null.And.InstanceOf<TestGenerator>());
            Assert.That(generator.Length, Is.EqualTo(5));
            Assert.That(generator.Source, Is.EqualTo("The really long expression."));
        }

        [Test]
        public void CreateGenerator_GeneratorWithParameters_ReturnsGenerator()
        {
            var attr = new GeneratorAttribute(typeof(TestGenerator));

            dynamic generator = attr.CreateGenerator(
                GetDSLMethod(x => x.GetTestGenerator(1, "a")),
                new Dictionary<string, object> { ["length"] = 15, ["source"] = "The short one." });

            Assert.That(generator, Is.Not.Null.And.InstanceOf<TestGenerator>());
            Assert.That(generator.Length, Is.EqualTo(15));
            Assert.That(generator.Source, Is.EqualTo("The short one."));
        }

        [Test]
        public void CreateGenerator_IncorrectParameters_ThrowsError()
        {
            var attr = new GeneratorAttribute(typeof(TestGenerator));

            Assert.Throws<MissingMethodException>(
                () =>
                attr.CreateGenerator(
                    GetDSLMethod(x => x.GetTestGenerator(1, "adf")),
                    new Dictionary<string, object> { ["source1"] = 15, ["length"] = "The short one." }));
        }

        [Test]
        public void CreateGenerator_NonGenericGeneratorFromGenericMethod_ReturnsGenerator()
        {
            var attr = new GeneratorAttribute(typeof(TestGenerator));

            var generator = attr.CreateGenerator(
                GetDSLMethod(x => x.GetTestGenericGenerator<int, string>()),
                new Dictionary<string, object>());

            Assert.That(generator, Is.Not.Null.And.InstanceOf<TestGenerator>());
        }

        [Test]
        public void CreateGenerator_GenericGenerator_ReturnsGenerator()
        {
            var attr = new GeneratorAttribute(typeof(TestGenericGenerator<,>));

            dynamic generator = attr.CreateGenerator(
                GetDSLMethod(x => x.GetTestGenericGenerator("a", 33)),
                new Dictionary<string, object> { ["value1"] = "test", ["value2"] = 554 });

            Assert.That(generator, Is.InstanceOf<TestGenericGenerator<string, int>>());
            Assert.That(generator.Value1, Is.EqualTo("test"));
            Assert.That(generator.Value2, Is.EqualTo(554)); 
        }

        [Test]
        public void CreateGenerator_GenericGeneratorWithNoTypeArguments_ThrowsError()
        {
            var attr = new GeneratorAttribute(typeof(TestGenericGenerator<,>));

            Assert.That(
                () => attr.CreateGenerator(GetDSLMethod(x => x.GetTestGenerator()), new Dictionary<string, object>()),
                Throws.ArgumentException);
        }

        [Test]
        public void CreateGenerator_WithDefaultParameters_ReturnsGenerator()
        {
            var attr = new GeneratorAttribute(typeof(TestGeneratorWithDefaultParameters));

            dynamic generator = attr.CreateGenerator(
                GetDSLMethod(x => x.GetTestGeneratorWithDefaultParameters(0, "")),
                new Dictionary<string, object> { ["numberInteger"] = 10, ["text2"] = "abc" });

            Assert.That(generator, Is.InstanceOf<TestGeneratorWithDefaultParameters>());
            Assert.That(generator.NumberInteger, Is.EqualTo(10));
            Assert.That(generator.Text1, Is.EqualTo("a"));
            Assert.That(generator.Text2, Is.EqualTo("abc"));
        }

        private static MethodInfo GetDSLMethod(Expression<Func<TestDSL, object>> getMethodExpr) => ((MethodCallExpression)getMethodExpr.Body).Method;

        private class TestDSL
        {
            public object GetTestGenerator() => default!;

#pragma warning disable IDE0060 // Remove unused parameter

            public object GetTestGenerator(int length, string source) => default!;

            public object GetTestGenericGenerator<T1, T2>() => default!;

            public object GetTestGenericGenerator<T1, T2>(T1 value1, T2 value2) => default!;

            public object GetTestGeneratorWithDefaultParameters(int numberInteger, string text2) => default!;

#pragma warning restore IDE0060 // Remove unused parameter
        }

        private class TestGenerator : IGenerator
        {
            public int Length { get; }

            public string Source { get; }

            public TestGenerator() : this(5, "The really long expression.")
            {
            }

            public TestGenerator(int length, string source)
            {
                Length = length;
                Source = source;
            }

            public object Next() => throw new NotSupportedException();
        }

        private class TestGenericGenerator<T1, T2> : IGenerator
        {
            public TestGenericGenerator(T1 value1, T2 value2)
            {
                Value1 = value1;
                Value2 = value2;
            }

            public T1 Value1 { get; }

            public T2 Value2 { get; }

            public object Next() => throw new NotSupportedException();
        }

        private class TestGeneratorWithDefaultParameters : IGenerator
        {
            public TestGeneratorWithDefaultParameters(int numberInteger, string text1 = "a", string text2 = "b")
            {
                NumberInteger = numberInteger;
                Text1 = text1;
                Text2 = text2;
            }

            public int NumberInteger { get; }

            public string Text1 { get; }

            public string Text2 { get; }

            public object Next() => throw new NotSupportedException();
        }
    }
}