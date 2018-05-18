using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger
{
    public class when_instantiating : Context
    {
        protected override bool SuppressAct => true;

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        static IEnumerable<TestCaseData> ValidInputDirectories
        {
            get
            {
                yield return new TestCaseData("dir\\", $"{BaseDir}\\dir").SetName("'dir\\' is prefixed with root");
                yield return new TestCaseData("dir", $"{BaseDir}\\dir").SetName("'dir' is prefixed with root");
                yield return new TestCaseData("dir\\file.txt", $"{BaseDir}\\dir").SetName("directory is prefixed with root, filename is skipped");
                yield return new TestCaseData("%nonexisting%\\dir\\", $"{BaseDir}\\nonexisting\\dir").SetName("non-existing environment variable is flattenned out");
                yield return new TestCaseData("C:\\dir", "C:\\dir").SetName("rooted 'dir' is taken as is");
                yield return new TestCaseData("C:\\dir\\", "C:\\dir").SetName("rooted 'dir\\' is taken as is");
                yield return new TestCaseData("C:\\dir\\file.txt", "C:\\dir").SetName("rooted directory is taken, filename skipped");
            }
        }

        [TestCaseSource(nameof(ValidInputDirectories))]
        public void and_input_directory_is_valid_then(string inputDirectory, string expectedResultDirectory)
        {
            InitSut(inputDirectory);
            SutMock.Object.Write("message");
            SutMock.Verify(l => l.AppendFallbackFile(It.IsAny<string>(),
                It.Is<string>(path => Path.GetDirectoryName(path) == expectedResultDirectory)),
            Times.Once);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        static IEnumerable<TestCaseData> InvalidInputDirectories
        {
            get
            {
                yield return new TestCaseData(null).SetName("disabled by empty directory");
                yield return new TestCaseData(" ").SetName("disabled by default directory");
                yield return new TestCaseData("file.txt").SetName("disabled by bare filename");
            }
        }

        [TestCaseSource(nameof(InvalidInputDirectories))]
        public void and_input_directory_is_invalid_then(string inputDirectory)
        {
            InitSut(inputDirectory);
            SutMock.Object.Write("message");
            SutMock.Verify(l => l.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void then_environment_variable_is_resolved()
        {
            Environment.SetEnvironmentVariable("fallback_root", @"C:\GIT");

            InitSut(@"%fallback_root%\dir");
            SutMock.Object.Write("message");
            SutMock.Verify(l => l.AppendFallbackFile(It.IsAny<string>(),
                    It.Is<string>(path => Path.GetDirectoryName(path) == @"C:\GIT\dir")),
                Times.Once);
        }
    }
}