﻿using FactoryBot.Tests.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace FactoryBot.Tests.BotTests
{
    [TestFixture]
    public class Bot_AutoBindingTest
    {
        [TearDown]
        public void Terminate() => Bot.ForgetAll();

        [Test]
        public void AutoBinding_Integer_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Integer);

        [Test]
        public void AutoBinding_Short_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Short);

        [Test]
        public void AutoBinding_Byte_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Byte);

        [Test]
        public void AutoBinding_Long_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Long);

        [Test]
        public void AutoBinding_Double_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Double);

        [Test]
        public void AutoBinding_Float_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Float);

        [Test]
        public void AutoBinding_Decimal_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Decimal);

        [Test]
        public void AutoBinding_String_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.String, false);

        [Test]
        public void AutoBinding_DateTime_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.DateTime);

        [Test]
        public void AutoBinding_TimeSpan_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.TimeSpan);

        [Test, Ignore("Flickering test")]
        public void AutoBinding_Enum_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Enum);

        [Test]
        public void AutoBinding_Guid_ShouldBindDefaultValue() => AutoBindingTestDefaultValue(x => x.Guid);

        [Test]
        public void AutoBinding_Boolean_ShouldBindDefaultValue()
        {
            Bot.DefineAuto<AllTypesModel>();

            Assert.That(() => Bot.Build<AllTypesModel>(), Throws.Nothing);
        }

        [Test]
        public void AutoBinding_NoPublicConstructor_ShouldThrowException() => Assert.Throws(typeof(BuildFailedException), () => Bot.DefineAuto<NoPublicConstructorModel>());

        [Test]
        public void AutoBinding_AbstractClass_ShouldThrowException() => Assert.Throws(typeof(BuildFailedException), () => Bot.DefineAuto<AbstractModel>());

        [Test]
        public void AutoBinding_NonDefaultConstructor_ShouldBindCorrectly()
        {
            Bot.DefineAuto<Model2>();

            var model = Bot.Build<Model2>();

            Assert.That(model.Number, Is.Not.Zero);
            Assert.That(model.Date, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void AutoBinding_DefaultSettingsAndFlatHierarchy_ShouldBindAllProperties()
        {
            Bot.DefineAuto<Model1>();

            var model = Bot.Build<Model1>();

            Assert.That(model.Number, Is.Not.Zero);
            Assert.That(model.Text, Has.Length.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void AutoBinding_DefaultSettingsAndNestedHierarchy_ShouldBindAllProperties()
        {
            Bot.DefineAuto<Model3>();

            var model = Bot.Build<Model3>();

            Assert.That(model.Number, Is.Not.Zero);
            Assert.That(model.Number, Is.Not.Null);
            Assert.That(model.Nested.Text, Is.Not.Null);
            Assert.That(model.Nested.Number, Is.Not.Zero);
        }

        [Test]
        public void AutoBinding_DefaultSettingsAndCollections_ShouldBindArray()
        {
            Bot.DefineAuto<Model4>();

            var model = Bot.Build<Model4>();

            Assert.That(model.SimpleArray, Is.Not.Empty.And.All.Not.Zero);

            Assert.That(model.ComplexArray, Is.Not.Empty.And.All.Not.Null);
            Assert.That(model.ComplexArray[0].Number, Is.Not.Zero);
            Assert.That(model.ComplexArray[0].Nested.Text, Is.Not.Null);
        }

        [Test]
        public void AutoBinding_DefaultSettingsAndCollections_ShouldBindList()
        {
            Bot.DefineAuto<Model4>();

            var model = Bot.Build<Model4>();

            Assert.That(model.SimpleList, Is.Not.Empty.And.All.Not.Zero);

            Assert.That(model.ComplexList, Is.Not.Empty.And.All.Not.Null);
            Assert.That(model.ComplexList[0].Number, Is.Not.Zero);
            Assert.That(model.ComplexList[0].Text, Is.Not.Null);
        }

        [Test]
        public void AutoBinding_DefaultSettingsAndCollections_ShouldBindDictionary()
        {
            Bot.DefineAuto<Model4>();

            var model = Bot.Build<Model4>();

            Assert.That(model.SimpleDictionary, Is.Not.Empty);
            Assert.That(model.SimpleDictionary.Values.First(), Is.Not.Null);

            Assert.That(model.ComplexDictionary, Is.Not.Empty);
            Assert.That(model.ComplexDictionary.Keys.First().Text, Is.Not.Null);
            Assert.That(model.ComplexDictionary.Values.First().Number, Is.Not.Zero);
        }

        [Test]
        public void AutoBinding_OverrideDefinition_ShouldBindAllPropertiesWithOverride()
        {
            Bot.DefineAuto(x => new Model1() { Number = x.Integer.Any(1, 10) });

            var model = Bot.Build<Model1>();

            Assert.That(model.Number, Is.InRange(1, 10));
            Assert.That(model.Text, Is.Not.Null);
        }

        [Test]
        public void AutoBinding_OverrideDefaultGenerators_ShouldBindAllPropertiesWithOverride()
        {
            Bot.SetDefaultAutoGenerator(x => x.Strings.Guid());
            Bot.DefineAuto<Model1>();

            var model = Bot.Build<Model1>();

            Assert.That(model.Text, Does.Match("^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$"));
            Assert.That(model.Number, Is.Not.Zero);
        }

        [Test]
        public void AutoBinding_BuildOverrides_ShouldBindAllPropertiesWithOverride()
        {
            Bot.DefineAuto<Model1>();
            
            var model = Bot.Build<Model1>(x => x.Number = 10);

            Assert.That(model.Number, Is.EqualTo(10));
            Assert.That(model.Text, Is.Not.Null);
        }

        [Test]
        public void AutoBinding_BuildCustom_ShouldBindAllPropertiesWithOverride()
        {
            Bot.DefineAuto<Model1>();

            var model = Bot.BuildCustom(x => new Model1(x.Integer.Any(1, 15), "test"));

            Assert.That(model.Number, Is.InRange(1, 15));
            Assert.That(model.Text, Is.EqualTo("test"));
        }

        private void AutoBindingTestDefaultValue<T>(Func<AllTypesModel, T> getActual, bool primitive = true)
        {
            Bot.DefineAuto<AllTypesModel>();
            
            T actualValue = default!;
            for (var i = 0; i < 3; i++)
            {
                var model = Bot.Build<AllTypesModel>();
                actualValue = getActual(model);

                if (primitive && actualValue!.Equals(default))
                {
                    continue;
                }

                break;
            }

            Assert.That(actualValue, Is.Not.EqualTo(default(T)));
        }
    }
}
