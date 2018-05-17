using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using LogCast.Fallback;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger
{
    public class when_getting_log_file_date : Context
    {
        private DateTime _now;
        protected override bool SuppressAct => true;

        public override void Arrange()
        {
            _now = DateTime.Now;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(
                    "123" + FileFallbackLogger.LogFileSuffix).Returns(null).SetName("suffixed non-date fails");

                yield return new TestCaseData(
                    FileFallbackLogger.LogFileSuffix).Returns(null).SetName("log suffix alone fails");

                yield return new TestCaseData(
                    "1983-08-21" + FileFallbackLogger.LogFileSuffix).Returns(new DateTime(1983, 08, 21)).SetName("Valid date file name succeeds");

                yield return new TestCaseData(
                    "1983-Aug-21" + FileFallbackLogger.LogFileSuffix).Returns(null).SetName("Invalid date file name succeeds");

                yield return new TestCaseData(
                    FileFallbackLogger.GetFileName(string.Empty, _now)).Returns(_now.Date).SetName("Now-based file name succeeds");

                yield return new TestCaseData(
                    FileFallbackLogger.GetFileName(string.Empty, _now.Date)).Returns(_now.Date).SetName("Today-based file name succeeds");
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public DateTime? then(string fileName)
        {
            return FileFallbackLogger.GetLogFileDate(new FileInfo(fileName));
        }
    }
}