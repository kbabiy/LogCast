using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using BddStyle.NUnit;
using FluentAssertions;

namespace LogCast.Test.given_LogManager
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class when_getting_logger : ContextBase
    {
        [TestCase("name1")]
        [TestCase("<name2>")]
        [TestCase("name3.postfix")]
        public void then_logger_name_is_not_changed_for_(string name)
        {
            LogManager.GetLogger(name).Name.Should().Be(name);
        }

        [Test]
        public void then_calling_class_is_used_as_default_logger_name()
        {
            LogManager.GetLogger().Name.Should().Be(nameof(when_getting_logger));
        }

        [Test]
        public void then_rooted_file_path_is_trimmed_to_name()
        {
            LogManager.GetLogger(@"C:\Users\name.ext").Name.Should().Be("name");
        }

        [Test]
        public void then_getting_logger_by_type_uses_type_parameter_name()
        {
            LogManager.GetLogger<ArgumentException>().Name.Should().Be(nameof(ArgumentException));
        }

        [Test]
        public void then_getting_logger_by_type_uses_type_name()
        {
            LogManager.GetLogger(typeof(ArgumentException)).Name.Should().Be(nameof(ArgumentException));
        }
    }
}