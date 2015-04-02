using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.UAP;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class ReconnectAttemptsToLocalizedStringConverterTests
    {
        private ReconnectAttemptsToLocalizedStringConverter _converter;
        private IStringTable _stringTable;
        int _testAttempts;

        [TestInitialize]
        public void TestSetup()
        {            
            _stringTable = new Mock.LocalizedString();
            _converter = new ReconnectAttemptsToLocalizedStringConverter();
            _converter.LocalizedString = _stringTable;
            _testAttempts = 10;
        }

        [TestMethod]
        public void ConvertResultContainsAttempts()
        {
            object result = _converter.Convert(_testAttempts, null, null, null);

            Assert.IsTrue(result is string);
            Assert.IsTrue((result as string).Contains(_testAttempts.ToString()));
        }

        [TestMethod]
        public void ConvertResultContainsMaxAttempts()
        {
            object result = _converter.Convert(_testAttempts, null, null, null);

            Assert.IsTrue(result is string);
            Assert.IsTrue((result as string).Contains(_testAttempts.ToString()));
        }

        [TestMethod]
        public void ConvertNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converter.Convert(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertNonIntThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                string someValue = "10";
                _converter.Convert(someValue, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.LocalizedString = null;
                _converter.Convert(_testAttempts, null, null, null);
            }));
        }


        [TestMethod]
        public void ConvertBackThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                string _testString = "abc";
                _converter.ConvertBack(_testString, null, null, null);
            }));            
        }
    }
}
