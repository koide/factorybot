﻿using System;
using System.IO;
using System.Reflection;
using FactoryBot.Tests.Models;
using NUnit.Framework;

namespace FactoryBot.Tests.Generators.Strings
{
    [TestFixture]
    public class FilePathGeneratorTest : GeneratorTestKit
    {
        private string _folderToRemove = default!;

        [TearDown]
        public void Terminate()
        {
            if (!string.IsNullOrWhiteSpace(_folderToRemove) && Directory.Exists(_folderToRemove))
            {
                Directory.Delete(_folderToRemove, true);
            }
        }

        [Test]
        public void Filename_NoCondition_ReturnsNewFilename() => AssertGeneratorValuesAreNotTheSame(x => new AllTypesModel { String = x.Strings.Filename() });

        [Test]
        public void Filename_NoCondition_ReturnsNotExistingFileFromEverywhere()
        {
            AssertGeneratorValue<string>(
                x => new AllTypesModel { String = x.Strings.Filename() },
                AssertFilePathValidButNotExisting);
        }

        [Test]
        public void Filename_NotExistingFileFromFolder_ReturnsFilename()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AssertGeneratorValue<string>(
                x => new AllTypesModel { String = x.Strings.Filename(folder!, false) },
                x =>
                    {
                        AssertFilePathValidButNotExisting(x);
                        Assert.That(x, Does.StartWith(folder + Path.DirectorySeparatorChar));
                    });
        }

        [Test]
        public void Filename_ExistingFileFromEverywhere_ReturnsFilename()
        {
            AssertGeneratorValue<string>(
                x => new AllTypesModel { String = x.Strings.Filename(null!, true) },
                AssertFileExists);
        }

        [Test]
        public void Filename_ExistingFileFromFolder_ReturnsFilename()
        {
            var folder = Directory.GetCurrentDirectory();
            AssertGeneratorValue<string>(
                x => new AllTypesModel { String = x.Strings.Filename(folder, true) },
                x =>
                    {
                        AssertFileExists(x);
                        Assert.That(x, Does.StartWith(folder));
                    });
        }

        [Test]
        public void Filename_ExistingFileFromNotExistingFolder_ThrowsError()
        {
            var folder = Directory.GetCurrentDirectory();
            while (Directory.Exists(folder))
            {
                folder += new Guid().ToString("D") + Path.DirectorySeparatorChar;
            }

            ExpectInitException<IOException>(x => new AllTypesModel { String = x.Strings.Filename(folder, true) });
        }

        [Test]
        public void Filename_ExistingFileFromEmptyFolder_ThrowsError()
        {
            var directory = new DirectoryInfo(new Guid().ToString("D"));
            directory.Create();
            _folderToRemove = directory.FullName;

            ExpectBuildException<IOException>(x => new AllTypesModel {String = x.Strings.Filename(directory.FullName, true)});
        }

        private static void AssertFilePathValidButNotExisting(string path)
        {
            Assert.That(Path.IsPathRooted(path), $"Path is incorrect. Actual: {path}");
            Assert.That(Path.GetFileName(path), Is.Not.Empty, "File name is absent.");
        }

        private static void AssertFileExists(string path)
        {
            Assert.That(Path.GetFileName(path), Is.Not.Empty, "File name is absent");
            Assert.That(path, Does.Exist);
        }
    }
}